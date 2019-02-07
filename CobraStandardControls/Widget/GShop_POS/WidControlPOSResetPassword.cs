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
    public class WidControlPOSResetPassword : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlPOSResetPasswordStyle    = "WidControlPOSResetPassword.css";
        protected const String ctWidControlPOSResetPasswordScript   = "WidControlPOSResetPassword.js";

        const String ctCLSWidControlPOSResetPassword                = "WidControlPOSResetPassword";
        const String ctCLSButtonPanel                               = "ButtonPanel";
        const String ctCLSSubmitButton                              = "SubmitButton";
        const String ctCLSCloseButton                               = "CloseButton";

        const String ctCLSHeaderBar                                 = "HeaderBar";
        const String ctCLSLogInID                                   = "LogInID";        
        const String ctCLSEmailMobile                               = "EmailMobile";
        const String ctCLSInfoText                                  = "InfoText";   
        
        const String ctDYTSubmitButtonText                          = "@@POS.Button.Submit";        
        const String ctDYTCloseButtonText                           = "@@POS.Button.Close";

        const String ctDYTTitle                                     = "@@POS.ResetPassword.Title";
        const String ctDYTLogInID                                   = "@@POS.Shared.LogInID";
        const String ctDYTEmailMobile                               = "@@POS.ResetPassword.EmailMobile";        
        
        const String ctCMDSubmit                                    = "@cmd%submit";
        const String ctCMDClose                                     = "@cmd%close";

        const int    ctMaxLength                                    = 15;

        public CompositeFormInterface SCI_ParentForm { get; set; }
                
        LanguageManager     clLanguageManager;
        SettingManager      clSettingManager;

        public WidControlPOSResetPassword()
        {
            clLanguageManager = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSettingManager = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSResetPasswordStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSResetPasswordScript));
        }

        private void RenderButtonPanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSubmitButton);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDSubmit);
            paComponentController.RenderBeginTag(HtmlTag.A);                        
            paComponentController.Write(clLanguageManager.GetText(ctDYTSubmitButtonText));            
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

            paComponentController.AddAttribute(HtmlAttribute.Maxlength, ctMaxLength.ToString());
            paComponentController.AddElementType(ComponentController.ElementType.InputBox);
            paComponentController.AddAttribute(HtmlAttribute.Type, "text");            
            paComponentController.RenderBeginTag(HtmlTag.Input);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderEmailMobileDiv(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSEmailMobile);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.InputLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(ctDYTEmailMobile));
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Maxlength, ctMaxLength.ToString());
            paComponentController.AddElementType(ComponentController.ElementType.InputBox);
            paComponentController.AddAttribute(HtmlAttribute.Type, "text");
            paComponentController.RenderBeginTag(HtmlTag.Input);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

       
        private void RenderContentContainer(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderLogInIDDiv(paComponentController);
            RenderEmailMobileDiv(paComponentController);            

            paComponentController.RenderEndTag();
        }

        protected void RenderBrowserMode(ComponentController paComponentController)
        {
            IncludeExternalLinkFiles(paComponentController);
                                    
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSResetPassword);
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

