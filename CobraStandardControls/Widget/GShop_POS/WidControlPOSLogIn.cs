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

namespace CobraStandardControls
{    
    public class WidControlPOSLogIn : WebControl, WidgetControlInterface 
    {
        const String ctWidControlPOSLogInStyle    = "WidControlPOSLogIn.css";
        const String ctWidControlPOSLogInScript   = "WidControlPOSLogIn.js";

        const String ctCLSWidControlPOSLogIn      = "WidControlPOSLogIn";
        const String ctCLSField                   = "Field";
        const String ctCLSLogInButtonDiv          = "LogInButtonDiv";
        const String ctCLSStatusDiv               = "StatusDiv";
        const String ctCLSCreateAccountButton     = "CreateAccountButton";
                
        const String ctDYTLogInIDPlaceHolder      = "@@POS.LogIn.LogInIDPlaceHolder";
        const String ctDYTPasswordPlaceHolder     = "@@POS.LogIn.PasswordPlaceHolder";
        const String ctDYTResetPasswordText       = "@@POS.LogIn.ResetPassword";

        const String ctCOLLoginID                 = "LoginID";
        const String ctCOLPassword                = "Password";

        const int ctMaxLength                     = 15;
        
        const String ctCMDLogIn                   = "@cmd%login";
        const String ctCMDResetPassword           = "@cmd%resetpassword";
        

        const String ctPRMResetPassword           = "FormPOSResetPassword";

        const String ctTPLFormTemplate            = "?_f=$FORMNAME";        

        public CompositeFormInterface SCI_ParentForm { get; set; }
        
        LanguageManager     clLanguageManager;
        SettingManager      clSettingManager;

        public String       SC_LandingPage 
        {
            get { return (clLandingPage); }
            set { clLandingPage = (!String.IsNullOrWhiteSpace(value)) ? ctTPLFormTemplate.Replace("$FORMNAME", General.Base64Encode(value)) : String.Empty; }
        }

        public String clLandingPage;
                
        public WidControlPOSLogIn()
        {
            clLandingPage       = String.Empty;            

            clLanguageManager   = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSettingManager    = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;
        }              
       
        protected void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;
            
            lcCSSStyleManager   = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSLogInStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSLogInScript));
        }
        
        protected void RenderLogInIDFieldDiv(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSField);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.LogInID);            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctCOLLoginID);
            paComponentController.AddAttribute(HtmlAttribute.Maxlength, ctMaxLength.ToString());
            paComponentController.AddAttribute(HtmlAttribute.PlaceHolder, clLanguageManager.GetText(ctDYTLogInIDPlaceHolder));
            paComponentController.AddAttribute(HtmlAttribute.Type, "text");            
            paComponentController.RenderBeginTag(HtmlTag.Input);
            paComponentController.RenderEndTag();
          
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.user));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        protected void RenderPasswordFieldDiv(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSField);
            paComponentController.RenderBeginTag(HtmlTag.Div);
                        
            paComponentController.AddElementType(ComponentController.ElementType.Password);                        
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctCOLPassword);
            paComponentController.AddAttribute(HtmlAttribute.Maxlength, ctMaxLength.ToString());
            paComponentController.AddAttribute(HtmlAttribute.PlaceHolder, clLanguageManager.GetText(ctDYTPasswordPlaceHolder));
            paComponentController.AddAttribute(HtmlAttribute.Type, "password");
            
            paComponentController.RenderBeginTag(HtmlTag.Input);
            paComponentController.RenderEndTag();

            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.pad_lock));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        protected void RenderLogInButtonDiv(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSLogInButtonDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
                             
            paComponentController.AddElementType(ComponentController.ElementType.LogInButton);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDLogIn);
            paComponentController.AddAttribute(HtmlAttribute.Type, "button");
            paComponentController.RenderBeginTag(HtmlTag.Button);
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.arrow_right));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        protected void RenderStatusBar(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSStatusDiv);
            paComponentController.AddElementType(ComponentController.ElementType.StatusBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.RenderEndTag();
        }

        protected void RenderResetPasswordLink(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCreateAccountButton);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Parameter, ctPRMResetPassword);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDResetPassword);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(clLanguageManager.GetText(ctDYTResetPasswordText));
            paComponentController.RenderEndTag();
        }

        protected void RenderBrowserMode(ComponentController paComponentController)
        {
            IncludeExternalLinkFiles(paComponentController);

            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSLogIn);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_LandingPage, SC_LandingPage);            
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderLogInIDFieldDiv(paComponentController);
            RenderPasswordFieldDiv(paComponentController);
            RenderLogInButtonDiv(paComponentController);
            RenderStatusBar(paComponentController);
            RenderResetPasswordLink(paComponentController);

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
