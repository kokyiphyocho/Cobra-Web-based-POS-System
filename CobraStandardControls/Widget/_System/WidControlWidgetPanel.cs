using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CobraFrame;
using CobraFoundation;
using CobraWebFrame;
using CobraResources;

namespace CobraStandardControls
{
    [ToolboxData("<{0}:SubCompositeWidgetPanel runat=server></{0}:SubCompositeWidgetPanel>")]
    public class WidControlWidgetPanel : WebControl
    {
        protected const String ctWidControlWidgetPanelStyle     = "WidControlWidgetPanel.css";
        protected const String ctWidControlWidgetPanelScript    = "WidControlWidgetPanel.js";

        const String ctCLSWidgetPanel                           = "WidControlWidgetPanel";

        const String ctTYPGROUP                                 = "GROUP";
                
        const String ctSETRestrictedWidget                      = "POS.User.$USERID.RestrictedWidgets";
        
        const String ctFLTPrimaryWidget                         = "Grouping = '' Or Type = 'GROUP'";
        const String ctFLTChildWidget                           = "Grouping = '$GROUP' And Type = 'WIDGET'";
        const String ctSortOrder                                = "ClassificationCode, SortPriority";

        const String ctSTAActive                                = "ACTIVE";

        public DataTable    SC_WidgetList               { get; set; }
        public String[]     SC_EffectiveRole            { get; set; }

        SettingManager                  clSettingManager;        
        String                          clRestrictedWidgets;

        public WidControlWidgetPanel()
        {
            SC_WidgetList       = null;
            SC_EffectiveRole    = new String[0];

            clSettingManager        = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;
            clRestrictedWidgets     =  clSettingManager.GetSettingValue(ctSETRestrictedWidget.Replace("$USERID", ApplicationFrame.GetInstance().ActiveSessionController.User.ActiveRow.UserID.ToString()));
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.System, ctWidControlWidgetPanelStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.System, ctWidControlWidgetPanelScript));
        }

        private bool VerifyRole(String paRequireRole)
        {            
            if (!String.IsNullOrWhiteSpace(paRequireRole))
            {
                for (int lcCount = 0; lcCount < SC_EffectiveRole.Length; lcCount++)
                    if (paRequireRole.Contains(SC_EffectiveRole[lcCount])) return(true);

                return (false);
            }
            return (true);
        }

        private void RenderWidget(ComponentController paComponentController, ViewWidgetSubscriptionRow paWidgetSubscriptionRow)
        {
            SubControlWidgetIcon lcSubCompositeWidget;

            if ((paWidgetSubscriptionRow != null) && (VerifyRole(paWidgetSubscriptionRow.RequireRole)) &&  (!clRestrictedWidgets.Contains(paWidgetSubscriptionRow.WidgetName)))
            {                
                lcSubCompositeWidget = new SubControlWidgetIcon();

                lcSubCompositeWidget.SC_WidgetIcon = paWidgetSubscriptionRow.Icon;
                lcSubCompositeWidget.SC_WidgetLabel = paWidgetSubscriptionRow.IconLabel;
                lcSubCompositeWidget.SC_WidgetLink = paWidgetSubscriptionRow.Link;
                lcSubCompositeWidget.SC_WidgetCategory = paWidgetSubscriptionRow.Category;

                lcSubCompositeWidget.RenderChildMode(paComponentController);
            }
        }

        private void RenderWidgetGroup(ComponentController paComponentController, ViewWidgetSubscriptionRow paWidgetSubscriptionRow)
        {
            SubControlWidgetGroup lcSubCompositeGroup;

            if ((paWidgetSubscriptionRow != null) && (VerifyRole(paWidgetSubscriptionRow.RequireRole)))
            {
                lcSubCompositeGroup = new SubControlWidgetGroup(paWidgetSubscriptionRow, GetWidgetRowList(paWidgetSubscriptionRow.Grouping));
                lcSubCompositeGroup.RenderChildMode(paComponentController);
            }
        }
        
        private DataRow[] GetWidgetRowList(String paGroup)
        {
            if (String.IsNullOrWhiteSpace(paGroup)) return (SC_WidgetList.Select(ctFLTPrimaryWidget,ctSortOrder));
            else return (SC_WidgetList.Select(ctFLTChildWidget.Replace("$GROUP", paGroup), ctSortOrder));
        } 

        private void RenderWidgetEntries(ComponentController paComponentController, DataRow[] paWidgetRows)
        {            
            ViewWidgetSubscriptionRow   lcWidgetSubscriptionRow;
            String                      lcPreviousWidgetName;
            
            if (paWidgetRows != null) 
            {
                lcWidgetSubscriptionRow     = new ViewWidgetSubscriptionRow(null);
                lcPreviousWidgetName        = String.Empty;

                for (int lcCount = 0; lcCount < paWidgetRows.Length; lcCount++)
                {
                    lcWidgetSubscriptionRow.Row = paWidgetRows[lcCount];                    

                    if (lcWidgetSubscriptionRow.Type != ctTYPGROUP)
                    {
                        if ((lcWidgetSubscriptionRow.WidgetName != lcPreviousWidgetName) && (lcWidgetSubscriptionRow.Status == ctSTAActive))                        
                            RenderWidget(paComponentController, lcWidgetSubscriptionRow);
                    }
                    else
                    {
                        if (clSettingManager.AppGrouping.Contains(lcWidgetSubscriptionRow.Grouping.ToLower()))
                            RenderWidgetGroup(paComponentController, lcWidgetSubscriptionRow);
                        else
                            RenderWidgetEntries(paComponentController, GetWidgetRowList(lcWidgetSubscriptionRow.Grouping));
                    }

                    lcPreviousWidgetName = lcWidgetSubscriptionRow.WidgetName;
                }               
            }
        }

        protected void RenderBrowserMode(ComponentController paComponentController)
        {
            IncludeExternalLinkFiles(paComponentController);

            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidgetPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            RenderWidgetEntries(paComponentController,GetWidgetRowList(null));
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
