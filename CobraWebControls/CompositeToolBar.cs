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

namespace CobraWebControls
{
    [ToolboxData("<{0}:CompositeToolBar runat=server></{0}:CompositeToolBar>")]
    public class CompositeToolBar : WebControl
    {
        protected const String ctToolBarStyle  = "ToolBarStyle.css";
        protected const String ctToolBarScript = "ToolBarScript.js";
        
        const String ctToolLinkTemplate         = "@cmd%toollink?_f=$FORMNAME";
        const String ctRegExFormLink            = "^[[](?<FormName>[^]]*)[]]$";

        const String ctGRPFormName              = "FormName";

        const String ctHome                     = "#HOME";
        
        const String ctCLSToolBar               = "ToolBar";
        const String ctCLSToolIcon              = "ToolIcon";
        const String ctCLSToolIconImage         = "ToolIconImage";

        ToolBarManager      clToolBarManager;
        FormInfoManager     clParentForm;

        public CompositeToolBar(FormInfoManager paParentForm, String paToolBarName)
        {
            clToolBarManager = ToolBarManager.CreateInstance(paToolBarName);
            clParentForm     = paParentForm;
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetToolBarStyleSheetUrl(ctToolBarStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetToolBarScriptUrl(ctToolBarScript));
        }

        protected String GetHyperLink(String paToolItemName, String paLink, out String paFormName)
        {
            Match   lcMatch;

            paFormName = String.Empty;

            if (!String.IsNullOrEmpty(paLink))
            {
                if (paLink == ctHome)
                    return (ctToolLinkTemplate.Replace("$FORMNAME", HttpUtility.UrlEncode(General.Base64Encode(paFormName = ApplicationFrame.GetInstance().ActiveSubscription.GetHomeForm()))));
                else if ((lcMatch = Regex.Match(paLink, ctRegExFormLink)).Success)
                    return (ctToolLinkTemplate.Replace("$FORMNAME", HttpUtility.UrlEncode(General.Base64Encode(paFormName = lcMatch.Groups[ctGRPFormName].Value))));                
                else return (paLink);
            }
            else return (null);
            
        }

        protected void RenderToolBarIcon(ComponentController paComponentController, ToolBarRow paToolBarRow)
        {
            String lcToolLink;
            String lcFormName;

            if (!String.IsNullOrEmpty(lcToolLink = GetHyperLink(paToolBarRow.ItemName, paToolBarRow.Link, out lcFormName)))
            {
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSToolIcon);
                paComponentController.AddAttribute(HtmlAttribute.Style, paToolBarRow.ItemCss);
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_FormName, lcFormName);
                
                if (paToolBarRow.Link == ctHome)
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "home");

                if (clToolBarManager.IsAttributeSet(paToolBarRow, ToolBarManager.ToolItemAttribute.PopUp))
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Action, "popup");

                if (clToolBarManager.IsAttributeSet(paToolBarRow, ToolBarManager.ToolItemAttribute.Strict))
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Mode, "strict");

                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, lcToolLink);                
                paComponentController.RenderBeginTag(HtmlTag.A);

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSToolIconImage);
                paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetToolBarImageUrl(paToolBarRow.ItemIcon));
                paComponentController.RenderBeginTag(HtmlTag.Img);
                paComponentController.RenderEndTag();

                paComponentController.RenderEndTag();
            }
        }

        protected void RenderBrowserMode(ComponentController paComponentController)
        {
            ToolBarRow lcToolBarRow;

            IncludeExternalLinkFiles(paComponentController);

            if (clToolBarManager.ActiveList != null)
            {
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Count, clToolBarManager.ActiveList.Rows.Count.ToString());
            }

            paComponentController.AddElementType(ComponentController.ElementType.ToolBar);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSToolBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            if (clToolBarManager.ActiveList != null)
            {
                for (int lcCount = 0; lcCount < clToolBarManager.ActiveList.Rows.Count; lcCount++)
                {
                    if ((lcToolBarRow = clToolBarManager.GetToolBarRow(lcCount)) != null)
                        RenderToolBarIcon(paComponentController, lcToolBarRow);                    
                }
            }

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

