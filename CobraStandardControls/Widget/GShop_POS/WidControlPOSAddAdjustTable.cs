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
using CobraBusinessFrame;

namespace CobraStandardControls
{
    public class WidControlPOSAddAdjustTable : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlPOSAddAdjustTableStyle     = "WidControlPOSAddAdjustTable.css";
        protected const String ctWidControlPOSAddAdjustTableScript    = "WidControlPOSAddAdjustTable.js";
        
        const String ctCLSWidControlPOSAddAdjustTable        = "WidControlPOSAddAdjustTable";        
        const String ctCLSButtonPanel                       = "ButtonPanel";
        const String ctCLSAddButton                         = "AddButton";        
        const String ctCLSCloseButton                       = "CloseButton";

        const String ctCTLSelectionPanel                    = "SELECTIONPANEL";

        const String ctDEFAddButtonText                     = "@@POS.Button.Add";
        const String ctDEFUpdateButtonText                  = "@@POS.Button.Update";
        const String ctDEFCloseButtonText                   = "@@POS.Button.Close";        

        //const String ctSETTableGroupLimit                   = "POS.SystemTableGroupLimit";
        //const String ctSETTableLimit                        = "POS.SystemTableLimit";

        const String ctKEYTableGroupLimit                   = "tablegrouplimit";
        const String ctKEYTableLimit                        = "tablelimit";

        const String ctCMDUpdate                            = "@cmd%update";
        const String ctCMDClose                             = "@cmd%close";

        const String ctFPMControlMode                       = "FPM_ControlMode";

        const int    ctDEFItemLimit                         = 500;

        public enum ControlMode { Group, Table }

        public CompositeFormInterface SCI_ParentForm { get; set; }
                
        public String SC_UnitRelationshipText   { get; set; }
        public String SC_AddButtonText          { get; set; }
        public String SC_UpdateButtonText       { get; set; }
        public String SC_CloseButtonText        { get; set; }
        
        POSTableListRow             clTableListRow;
        LanguageManager             clLanguageManager;
        SettingManager              clSettingManager;
        ControlMode                 clControlMode;

        public WidControlPOSAddAdjustTable()
        {
            clTableListRow = RetrieveRow();

            clControlMode           = General.ParseEnum<ControlMode>(ApplicationFrame.GetInstance().ActiveFormInfoManager.GetFormParam(ctFPMControlMode), ControlMode.Table); 
           
            SC_AddButtonText        = ctDEFAddButtonText;
            SC_UpdateButtonText     = ctDEFUpdateButtonText;
            SC_CloseButtonText      = ctDEFCloseButtonText;

            clLanguageManager = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSettingManager  = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSAddAdjustTableStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSAddAdjustTableScript));
        }

        private void RenderButtonPanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSAddButton);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDUpdate);            
            paComponentController.RenderBeginTag(HtmlTag.A);

            if (clTableListRow == null)
                paComponentController.Write(clLanguageManager.GetText(SC_AddButtonText));
            else
                paComponentController.Write(clLanguageManager.GetText(SC_UpdateButtonText));

            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCloseButton);            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDClose);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(clLanguageManager.GetText(SC_CloseButtonText));
            paComponentController.RenderEndTag();        

            paComponentController.RenderEndTag();
        }

        private POSTableListRow RetrieveRow()
        {
            DataRow lcDataRow;

            if ((lcDataRow = ApplicationFrame.GetInstance().ActiveFormInfoManager.RunRetrieveRow()) != null) return (new POSTableListRow(lcDataRow));
            else return (null);
        }

        private void CustomComponentRendererHandler(ComponentController paComponentController, InputInfoRow paInputInfoRow, String paActiveValue)
        {
            if (paInputInfoRow.ControlType.ToUpper() == ctCTLSelectionPanel)
            {
                SubControlSelectionPanel lcSubControlSelectionPanel;

                lcSubControlSelectionPanel = new SubControlSelectionPanel(paInputInfoRow.InputMode.ToLower(), paInputInfoRow.AdditionalInfo, paInputInfoRow.InputLabel, paInputInfoRow.QueryName);
                lcSubControlSelectionPanel.RenderChildMode(paComponentController);
            }
        }
    
        protected void RenderBrowserMode(ComponentController paComponentController)
        {
            InputInfoManager    lcInputInfoManager;
            MetaDataRow         lcMetaDataRow;
            //int                 lcTableGroupLimit;
            //int                 lcTableLimit;

            IncludeExternalLinkFiles(paComponentController);

            lcInputInfoManager = ApplicationFrame.GetInstance().ActiveFormInfoManager.FieldInfoManager.ActiveInputInfoManager;
            lcInputInfoManager.CustomComponentRenderer += CustomComponentRendererHandler;

            if (clTableListRow != null)
                lcMetaDataRow = new MetaDataRow(clTableListRow.Row);
            else
                lcMetaDataRow = null;

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_SystemTableLimit, General.ParseInt(clSettingManager.SystemConfig.GetData(ctKEYTableLimit), 7).ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_SystemTableGroupLimit, General.ParseInt(clSettingManager.SystemConfig.GetData(ctKEYTableGroupLimit), 0).ToString());                        
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ControlMode, clControlMode.ToString().ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_StatusText, clLanguageManager.GetText(SC_UnitRelationshipText));
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSAddAdjustTable);
            paComponentController.RenderBeginTag(HtmlTag.Div);
                        
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            if (lcInputInfoManager != null)
            {
                lcInputInfoManager.RenderAllSubGroups(paComponentController, lcMetaDataRow);
            }

            paComponentController.RenderEndTag();

      //      RenderButtonPanel(paComponentController);
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

        public void RenderChildMode(ComponentController paComponentController, String paRenderMode = null)
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

