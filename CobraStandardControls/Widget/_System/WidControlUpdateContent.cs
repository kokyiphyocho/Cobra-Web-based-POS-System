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
    public class WidControlUpdateContent : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlUpdateContentStyle   = "WidControlUpdateContent.css";
        protected const String ctWidControlUpdateContentScript  = "WidControlUpdateContent.js";

        const String ctCLSWidControlUpdateContent = "WidControlUpdateContent";        
        const String ctCLSButtonPanel           = "ButtonPanel";
        const String ctCLSUpdateButton          = "UpdateButton";
        const String ctCLSCancelButton          = "CancelButton";

        const String ctCTLSelectionPanel        = "SELECTIONPANEL";
        const String ctCTLColorSelectionPanel   = "COLORSELECTIONPANEL";
        const String ctCTLImageSelectionPanel   = "IMAGESELECTIONPANEL";

        const String ctDEFUpdateButtonText      = "Update";
        const String ctDEFCancelButtonText      = "Cancel";        
                        
        const String ctCMDUpdate                = "@cmd%update";
        const String ctCMDCancel                = "@cmd%cancel";

        public CompositeFormInterface SCI_ParentForm { get; set; }

        public DataRow      SC_ActiveDataRow        { get; set; }

        public String       SC_ConfirmationTitle    { get; set; }

        public String       SC_SaveButtonText       { get; set; }
        public String       SC_CancelButtonText     { get; set; }        
        
        public WidControlUpdateContent()
        {
            SC_ActiveDataRow = null;
                        
            SC_SaveButtonText       = ctDEFUpdateButtonText;
            SC_CancelButtonText     = ctDEFCancelButtonText;                      
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.System, ctWidControlUpdateContentStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.System, ctWidControlUpdateContentScript));
        }

        private void RenderButtonPanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSUpdateButton);
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDUpdate);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(SC_SaveButtonText);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCancelButton);
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDCancel);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(SC_CancelButtonText);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void CustomComponentRendererHandler(ComponentController paComponentController, InputInfoRow paInputInfoRow, String paActiveValue)
        {
            switch(paInputInfoRow.ControlType.ToUpper())
            {
                case ctCTLSelectionPanel :
                    {
                        SubControlSelectionPanel lcSubControlSelectionPanel;

                        lcSubControlSelectionPanel = new SubControlSelectionPanel(paInputInfoRow.InputMode.ToLower(), paInputInfoRow.AdditionalInfo, paInputInfoRow.InputLabel, paInputInfoRow.QueryName, paInputInfoRow.LinkColumn);
                        lcSubControlSelectionPanel.RenderChildMode(paComponentController);

                        break;
                    }

                case ctCTLImageSelectionPanel :
                    {
                        SubControlSelectionPanel lcSubControlSelectionPanel;

                        lcSubControlSelectionPanel = new SubControlSelectionPanel(paInputInfoRow.InputMode.ToLower(), paInputInfoRow.AdditionalInfo, paInputInfoRow.InputLabel, paInputInfoRow.QueryName, paInputInfoRow.LinkColumn);
                        lcSubControlSelectionPanel.SetSelectionMode(SubControlSelectionPanel.SelectionMode.Image);
                        lcSubControlSelectionPanel.RenderChildMode(paComponentController);

                        break;
                    }

                case ctCTLColorSelectionPanel:
                    {
                        SubControlSelectionPanel lcSubControlSelectionPanel;

                        lcSubControlSelectionPanel = new SubControlSelectionPanel(paInputInfoRow.InputMode.ToLower(), paInputInfoRow.AdditionalInfo, paInputInfoRow.InputLabel, paInputInfoRow.QueryName, paInputInfoRow.LinkColumn);
                        lcSubControlSelectionPanel.SetSelectionMode(SubControlSelectionPanel.SelectionMode.Color);
                        lcSubControlSelectionPanel.RenderChildMode(paComponentController);

                        break;
                    }
            }
        }
        
        protected void RenderBrowserMode(ComponentController paComponentController)
        {
            InputInfoManager    lcInputInfoManager;
            MetaDataRow         lcMetaDataRow;

            IncludeExternalLinkFiles(paComponentController);

            lcInputInfoManager = ApplicationFrame.GetInstance().ActiveFormInfoManager.FieldInfoManager.ActiveInputInfoManager;
            lcInputInfoManager.CustomComponentRenderer +=  CustomComponentRendererHandler;
            lcMetaDataRow = new MetaDataRow(SC_ActiveDataRow);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ControlMode, "standard");
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlUpdateContent);
            paComponentController.RenderBeginTag(HtmlTag.Div);
                    
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            if (lcInputInfoManager != null)
            {
                lcInputInfoManager.RenderAllSubGroups(paComponentController, lcMetaDataRow);  
            }

            paComponentController.RenderEndTag();

            RenderButtonPanel(paComponentController);
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
