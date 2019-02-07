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
    public class WidControlPOSAddAdjustUnit : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlPOSAddAdjustUnitStyle     = "WidControlPOSAddAdjustUnit.css";
        protected const String ctWidControlPOSAddAdjustUnitScript    = "WidControlPOSAddAdjustUnit.js";
        
        const String ctCLSWidControlPOSAddAdjustUnit        = "WidControlPOSAddAdjustUnit";        
        const String ctCLSButtonPanel                       = "ButtonPanel";
        const String ctCLSAddButton                         = "AddButton";        
        const String ctCLSCloseButton                       = "CloseButton";

        const String ctCTLSelectionPanel                    = "SELECTIONPANEL";

        const String ctDYTAddButtonText                     = "@@POS.Button.Add";
        const String ctDYTUpdateButtonText                  = "@@POS.Button.Update";
        const String ctDYTCloseButtonText                   = "@@POS.Button.Close";
        const String ctDYTUnitRelationshipText              = "@@POS.UnitRelationshipText";

        const String ctCMDUpdate                            = "@cmd%update";
        const String ctCMDClose                             = "@cmd%close";

        const String ctFPMControlMode                       = "FPM_ControlMode";

        public enum ControlMode                             { Base, Custom }

        public CompositeFormInterface SCI_ParentForm    { get; set; }
        
        POSUnitRow                  clUnitRow;
        LanguageManager             clLanguageManager;
        ControlMode                 clControlMode;

        public WidControlPOSAddAdjustUnit()
        {
            clUnitRow       = RetrieveRow();
            clControlMode = General.ParseEnum<ControlMode>(ApplicationFrame.GetInstance().ActiveFormInfoManager.GetFormParam(ctFPMControlMode), ControlMode.Custom);

            clLanguageManager       = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSAddAdjustUnitStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSAddAdjustUnitScript));
        }

        private void RenderButtonPanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSAddButton);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDUpdate);            
            paComponentController.RenderBeginTag(HtmlTag.A);

            if (clUnitRow == null)
                paComponentController.Write(clLanguageManager.GetText(ctDYTAddButtonText));
            else
                paComponentController.Write(clLanguageManager.GetText(ctDYTUpdateButtonText));

            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCloseButton);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDClose);            
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(clLanguageManager.GetText(ctDYTCloseButtonText));
            paComponentController.RenderEndTag();        

            paComponentController.RenderEndTag();
        }

        private POSUnitRow RetrieveRow()
        {
            DataRow lcDataRow;

            if ((lcDataRow = ApplicationFrame.GetInstance().ActiveFormInfoManager.RunRetrieveRow()) != null) return (new POSUnitRow(lcDataRow));
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
            InputInfoManager lcInputInfoManager;
            MetaDataRow lcMetaDataRow;

            IncludeExternalLinkFiles(paComponentController);
            
            lcInputInfoManager = ApplicationFrame.GetInstance().ActiveFormInfoManager.FieldInfoManager.ActiveInputInfoManager;
            lcInputInfoManager.CustomComponentRenderer += CustomComponentRendererHandler;

            if (clUnitRow != null)
                lcMetaDataRow = new MetaDataRow(clUnitRow.Row);
            else
                lcMetaDataRow = null;

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ControlMode, clControlMode.ToString().ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_StatusText,clLanguageManager.GetText(ctDYTUnitRelationshipText));
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSAddAdjustUnit);
            paComponentController.RenderBeginTag(HtmlTag.Div);
                        
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            if (lcInputInfoManager != null)
            {
                lcInputInfoManager.RenderAllSubGroups(paComponentController, lcMetaDataRow);
            }

            paComponentController.RenderEndTag();

            //RenderButtonPanel(paComponentController);
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

