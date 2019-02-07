using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CobraFrame;
using CobraFoundation;
using CobraWebFrame;
using CobraResources;
using System.Text.RegularExpressions;
using System.Data;

namespace CobraStandardControls
{    
    public class SubControlWidgetGroup : WebControl
    {
        protected const String ctSubControlWidgetGroupStyle = "SubControlWidgetGroup.css";
        protected const String ctSubControlWidgetGroupScript = "SubControlWidgetGroup.js";

        const String ctDefaultIcon              = "/images/Widget_Default.png";
        const String ctRegExFormLink            = "^[[](?<FormName>[^]]*)[]]$";
        const String ctGRPFormName              = "FormName";
        const String ctFormLinkTemplate         = "@cmd%formlink?_f=$FORMNAME";
        const String ctOtherLinkTemplate        = "@cmd%link?$LINK";

        const String ctCLSSubControlWidgetGroup  = "SubControlWidgetGroup";
        const String ctCLSWidgetAnchor            = "WidgetAnchor";
        const String ctCLSWidgetGroupIcon        = "WidgetGroupIcon";
        const String ctCLSMinuteIconContainer    = "MinuteIconContainer";
        const String ctCLSMinuteIcon             = "MinuteIcon";
        const String ctCLSWidgetLabel            = "WidgetLabel";

        const String ctCLSWidgetPopUp            = "WidgetPopUp";
        const String ctCLSPopUpPanel             = "PopUpPanel";
   //     const String ctCLSPopUpHeader            = "PopUpHeader";
        const String ctCLSChildContainer         = "ChildContainer";
        const String ctCLSInnerContainer         = "InnerContainer";

        const String ctTYPGroup                  = "GROUP";

        DataRow[]                   clWidgetRows;
        ViewWidgetSubscriptionRow   clWidgetSubscriptionRow;
        String[]                    clEffectiveRole;
        
        LanguageManager             clLanguageManager;
        
        public SubControlWidgetGroup(ViewWidgetSubscriptionRow paWidgetSubscriptionRow, DataRow[] paWidgetRows)
        {
            clWidgetRows                = paWidgetRows;
            clWidgetSubscriptionRow     = paWidgetSubscriptionRow;
            clLanguageManager           = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clEffectiveRole             = ApplicationFrame.GetInstance().GetEffectiveRoleList();
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.System, ctSubControlWidgetGroupStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.System, ctSubControlWidgetGroupScript));
        }

        //protected String GetHyperLink()
        //{
        //    Match lcMatch;

        //    if ((lcMatch = Regex.Match(SC_WidgetLink, ctRegExFormLink)).Success)
        //        return (ctFormLinkTemplate.Replace("$FORMNAME", HttpUtility.UrlEncode(General.Base64Encode(lcMatch.Groups[ctGRPFormName].Value))));
        //    else return (ctOtherLinkTemplate.Replace("$LINK", SC_WidgetLink));
        //}

        private bool VerifyRole(String paRequireRole)
        {
            if (!String.IsNullOrWhiteSpace(paRequireRole))
            {
                for (int lcCount = 0; lcCount < clEffectiveRole.Length; lcCount++)
                    if (paRequireRole.Contains(clEffectiveRole[lcCount])) return (true);

                return (false);
            }
            return (true);
        }

        private void RenderChildWidget(ComponentController paComponentController, ViewWidgetSubscriptionRow paWidgetSubscriptionRow)
        {
            SubControlWidgetIcon lcSubCompositeWidget;

            if ((paWidgetSubscriptionRow != null) && (VerifyRole(paWidgetSubscriptionRow.RequireRole)))
            {
                lcSubCompositeWidget = new SubControlWidgetIcon();

                lcSubCompositeWidget.SC_WidgetIcon = paWidgetSubscriptionRow.Icon;
                lcSubCompositeWidget.SC_WidgetLabel = paWidgetSubscriptionRow.IconLabel;
                lcSubCompositeWidget.SC_WidgetLink = paWidgetSubscriptionRow.Link;
                lcSubCompositeWidget.SC_WidgetCategory = paWidgetSubscriptionRow.Category;

                lcSubCompositeWidget.RenderChildMode(paComponentController);
            }
        }

        private void RenderChildWidgetEntries(ComponentController paComponentController)
        {
            ViewWidgetSubscriptionRow lcWidgetSubscriptionRow;

            if (clWidgetRows != null)
            {
                lcWidgetSubscriptionRow = new ViewWidgetSubscriptionRow(null);

                for (int lcCount = 0; lcCount < clWidgetRows.Length; lcCount++)
                {
                    lcWidgetSubscriptionRow.Row = clWidgetRows[lcCount];

                    if (lcWidgetSubscriptionRow.Type != ctTYPGroup)
                        RenderChildWidget(paComponentController, lcWidgetSubscriptionRow);
                }
            }
        }

        //private void RenderGroupPopUpHeader(ComponentController paComponentController)
        //{
        //    paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPopUpHeader);
        //    paComponentController.RenderBeginTag(HtmlTag.Div);
        //    paComponentController.Write(clLanguageManager.GetText(clWidgetSubscriptionRow.IconLabel));
        //    paComponentController.RenderEndTag();
        //}

        private void RenderGroupPopUpChildContainer(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSChildContainer);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInnerContainer);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderChildWidgetEntries(paComponentController);

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderGroupPopUp(ComponentController paComponentController)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, clWidgetSubscriptionRow.WidgetName);
            paComponentController.AddElementType(ComponentController.ElementType.PopUp);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidgetPopUp);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPopUpPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

           // RenderGroupPopUpHeader(paComponentController);
            RenderGroupPopUpChildContainer(paComponentController);            

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderMinuteWidget(ComponentController paComponentController, ViewWidgetSubscriptionRow paWidgetSubscriptionRow)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSMinuteIcon);
            paComponentController.AddAttribute(HtmlAttribute.Src, paWidgetSubscriptionRow.Icon);
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();
        }

        private void RenderMinuteWidgetEntries(ComponentController paComponentController)
        {
            ViewWidgetSubscriptionRow lcWidgetSubscriptionRow;

            if (clWidgetRows != null)
            {
                lcWidgetSubscriptionRow = new ViewWidgetSubscriptionRow(null);

                for (int lcCount = 0; lcCount < clWidgetRows.Length; lcCount++)
                {
                    lcWidgetSubscriptionRow.Row = clWidgetRows[lcCount];

                    if (lcWidgetSubscriptionRow.Type != ctTYPGroup) 
                        RenderMinuteWidget(paComponentController, lcWidgetSubscriptionRow);                    
                }
            }
        }

        protected void RenderWidgetIcon(ComponentController paComponentController)
        {
            //paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Mode, SC_WidgetCategory.ToLower());
            //paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, GetHyperLink());
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidgetAnchor);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.WidgetIcon);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidgetGroupIcon);            
            paComponentController.RenderBeginTag(HtmlTag.Div);
            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSMinuteIconContainer);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            RenderMinuteWidgetEntries(paComponentController);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            RenderGroupPopUp(paComponentController);

            paComponentController.RenderEndTag();
        }

        protected void RenderWidgetLabel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidgetLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(clWidgetSubscriptionRow.IconLabel));            
            paComponentController.RenderEndTag();
        }

        protected void RenderBrowserMode(ComponentController paComponentController)
        {
            IncludeExternalLinkFiles(paComponentController);

            paComponentController.AddElementType(ComponentController.ElementType.Element);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSubControlWidgetGroup);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            RenderWidgetIcon(paComponentController);
            RenderWidgetLabel(paComponentController);
            paComponentController.RenderEndTag();
        }

        protected void RenderDesignMode(ComponentController paComponentController)
        {
            paComponentController.AddStyle(CSSStyle.Border, "2px Solid Black");
            paComponentController.AddStyle(CSSStyle.Height, this.Height.ToString());
            paComponentController.AddStyle(CSSStyle.Width, this.Width.ToString());
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(this.GetType().ToString());
            paComponentController.RenderEndTag();
        }

        public void RenderChildMode(ComponentController paComponentController)
        {
            RenderBrowserMode(paComponentController);
        }

        protected override void Render(HtmlTextWriter paHtmlTextWriter)
        {
            if (!DesignMode) RenderBrowserMode(new ComponentController(paHtmlTextWriter));
            else RenderDesignMode(new ComponentController(paHtmlTextWriter));
        }

    }
}
