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
    public class WidControlPOSChangePassword : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlPOSChangePasswordStyle   = "WidControlPOSChangePassword.css";
        protected const String ctWidControlPOSChangePasswordScript = "WidControlPOSChangePassword.js";

        const String ctCLSWidControlPOSChangePassword               = "WidControlPOSChangePassword";
        const String ctCLSButtonPanel                               = "ButtonPanel";
        const String ctCLSUpdateButton                              = "UpdateButton";
        const String ctCLSCloseButton                               = "CloseButton";

        const String ctCLSHeaderBar                                 = "HeaderBar";
        const String ctCLSLogInID                                   = "LogInID";
        const String ctCLSPassword                                  = "Password";
        const String ctCLSConfirmPassword                           = "ConfirmPassword";
        
        const String ctDYTUpdateButtonText                          = "@@POS.Button.Update";
        const String ctDYTCloseButtonText                           = "@@POS.Button.Close";

        const String ctDYTTitle                                     = "@@POS.ChangePassword.Title";
        const String ctDYTLogInID                                   = "@@POS.Shared.LogInID";
        const String ctDYTPassword                                  = "@@POS.Shared.Password";
        const String ctDYTConfirmPassword                           = "@@POS.Shared.ConfirmPassword";
        

        const String ctCMDUpdate                                    = "@cmd%update";
        const String ctCMDClose                                     = "@cmd%close";

        const int    ctMaxLength                                    = 15;

        public CompositeFormInterface SCI_ParentForm                { get; set; }

        UserRow                     clUserRow;
        LanguageManager             clLanguageManager;
        SettingManager              clSettingManager;

        public WidControlPOSChangePassword()
        {
            clUserRow = RetrieveRow();

            clLanguageManager       = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSettingManager        = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager       = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager     = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSChangePasswordStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSChangePasswordScript));
        }

        private void RenderButtonPanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSUpdateButton);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDUpdate);
            paComponentController.RenderBeginTag(HtmlTag.A);                        
            paComponentController.Write(clLanguageManager.GetText(ctDYTUpdateButtonText));
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCloseButton);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDClose);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(clLanguageManager.GetText(ctDYTCloseButtonText));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderHeaderBar(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSHeaderBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.Write(clLanguageManager.GetText(ctDYTTitle));

            paComponentController.RenderEndTag();
        }

        private void RenderLogInIDDiv(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSLogInID);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.InputLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(ctDYTLogInID));
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.ReadOnly, true.ToString());
            paComponentController.AddAttribute(HtmlAttribute.Maxlength, ctMaxLength.ToString());
            paComponentController.AddElementType(ComponentController.ElementType.InputBox);
            paComponentController.AddAttribute(HtmlAttribute.Type, "text");
            if (clUserRow != null) paComponentController.AddAttribute(HtmlAttribute.Value, clUserRow.LoginID);
            paComponentController.RenderBeginTag(HtmlTag.Input);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderPasswordDiv(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPassword);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.InputLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(ctDYTPassword));
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Maxlength, ctMaxLength.ToString());
            paComponentController.AddElementType(ComponentController.ElementType.InputBox);
            paComponentController.AddAttribute(HtmlAttribute.Type, "password");
            paComponentController.RenderBeginTag(HtmlTag.Input);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderConfirmPasswordDiv(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSConfirmPassword);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.InputLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(ctDYTConfirmPassword));
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Maxlength, ctMaxLength.ToString());
            paComponentController.AddElementType(ComponentController.ElementType.InputBox);
            paComponentController.AddAttribute(HtmlAttribute.Type, "password");
            paComponentController.RenderBeginTag(HtmlTag.Input);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderContentContainer(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderLogInIDDiv(paComponentController);
            RenderPasswordDiv(paComponentController);
            RenderConfirmPasswordDiv(paComponentController);

            paComponentController.RenderEndTag();
        }

        private UserRow RetrieveRow()
        {
            DataRow lcDataRow;

            if ((lcDataRow = ApplicationFrame.GetInstance().ActiveFormInfoManager.RunRetrieveRow()) != null) return (new UserRow(lcDataRow));
            else return (null);
        }

        protected void RenderBrowserMode(ComponentController paComponentController)
        {
            IncludeExternalLinkFiles(paComponentController);

            if (clUserRow == null) paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DataID, (-1).ToString());
            else paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DataID, clUserRow.UserID.ToString());
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSChangePassword);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderHeaderBar(paComponentController);

            RenderContentContainer(paComponentController);

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

