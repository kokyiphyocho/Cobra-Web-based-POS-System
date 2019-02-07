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
    public class WidControlCreateUser : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlCreateUserStyle  = "WidControlCreateUser.css";
        protected const String ctWidControlCreateUserScript = "WidControlCreateUser.js";

        protected const String ctJQueryUIScript             = "jquery-ui.min.js";
        protected const String ctJQueryUIStyle              = "jquery-ui.min.css";

        protected const String ctSumoSelectScript           = "jquery.sumoselect.min.js";
        protected const String ctSumoSelectStyle            = "sumoselect.css";
        
        protected const String ctJQueryUIFunctionScript     = "jquery-ui.functions.js";

        const String ctCLSWidControlCreateUser              = "WidControlCreateUser";
        const String ctCLSConfirmationTitle                 = "ConfirmationTitle";
        const String ctCLSButtonPanel                       = "ButtonPanel";
        const String ctCLSUpdateButton                      = "CreateButton";
        const String ctCLSConfirmButton                     = "ConfirmButton";
        const String ctCLSBackButton                        = "BackButton";
        const String ctCLSCancelButton                      = "CancelButton";

        const String ctCTLSelectionPanel                    = "SELECTIONPANEL";

        const String ctDEFCreateButtonText                  = "Create";
        const String ctDEFCancelButtonText                  = "Cancel";
        const String ctDEFConfirmButtonText                 = "Confirm";
        const String ctDEFBackButtonText                    = "Back";

        const String ctDEFConfirmationTitle                 = "CONFIRMATION OF INFORMATION";

        const String ctCMDCreate                            = "@cmd%create";
        const String ctCMDCancel                            = "@cmd%cancel";
        const String ctCMDConfirm                           = "@cmd%confirm";
        const String ctCMDBack                              = "@cmd%back";

        public CompositeFormInterface SCI_ParentForm { get; set; }

        public DataRow SC_ActiveDataRow { get; set; }

        public String SC_ConfirmationTitle { get; set; }

        public String SC_CreateButtonText { get; set; }
        public String SC_CancelButtonText { get; set; }
        public String SC_ConfirmButtonText { get; set; }
        public String SC_BackButtonText { get; set; }


        public WidControlCreateUser()
        {
            SC_ActiveDataRow = null;

            SC_ConfirmationTitle    = ctDEFConfirmationTitle;

            SC_CreateButtonText     = ctDEFCreateButtonText;
            SC_CancelButtonText     = ctDEFCancelButtonText;
            SC_ConfirmButtonText    = ctDEFConfirmButtonText;
            SC_BackButtonText       = ctDEFBackButtonText;
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetFoundationStyleSheetUrl(ctJQueryUIStyle));
            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetFoundationStyleSheetUrl(ctSumoSelectStyle));

            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetFoundationScriptUrl(ctJQueryUIScript));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetFoundationScriptUrl(ctSumoSelectScript));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetFoundationScriptUrl(ctJQueryUIFunctionScript));

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.System, ctWidControlCreateUserStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.System, ctWidControlCreateUserScript));
        }

        private void RenderButtonPanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSUpdateButton);
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDCreate);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(SC_CreateButtonText);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCancelButton);
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDCancel);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(SC_CancelButtonText);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSBackButton);
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDBack);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(SC_BackButtonText);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSConfirmButton);
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDConfirm);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(SC_ConfirmButtonText);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderConfirmationTitle(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSConfirmationTitle);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(SC_ConfirmationTitle);
            paComponentController.RenderEndTag();
        }

        private void CustomComponentRendererHandler(ComponentController paComponentController, InputInfoRow paInputInfoRow, String paActiveValue)
        {
            if (paInputInfoRow.ControlType.ToUpper() == ctCTLSelectionPanel)
            {
                SubControlSelectionPanel lcSubControlSelectionPanel;

                lcSubControlSelectionPanel = new SubControlSelectionPanel(paInputInfoRow.InputMode.ToLower(), paInputInfoRow.AdditionalInfo, paInputInfoRow.InputLabel, paInputInfoRow.QueryName, paInputInfoRow.LinkColumn);
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
            lcMetaDataRow = new MetaDataRow(SC_ActiveDataRow);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ControlMode, "standard");
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlCreateUser);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderConfirmationTitle(paComponentController);

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
