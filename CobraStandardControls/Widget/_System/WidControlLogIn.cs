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
    [ToolboxData("<{0}:WidControlLogIn runat=server></{0}:WidControlLogIn>")]
    public class WidControlLogIn : WebControl, WidgetControlInterface 
    {
        protected const String ctWidControlLogInStyle       = "WidControlLogIn.css";
        protected const String ctWidControlLogInScript      = "WidControlLogIn.js";

        protected const String ctCLSWidControlLogIn         = "WidControlLogIn";
        protected const String ctCLSField                   = "Field";
        protected const String ctCLSLogInButtonDiv          = "LogInButtonDiv";
        protected const String ctCLSStatusDiv               = "StatusDiv";
        protected const String ctCLSCreateAccountButton     = "CreateAccountButton";

        protected const String ctNAMELoginID                = "LoginID";
        protected const String ctNAMEPassword               = "Password";
                
        protected const String ctDEFUserNamePlaceHolder     = "User Name";
        protected const String ctDEFPasswordPlaceHolder     = "Password";

        protected const String ctDEFCreateAccountButtonText = "Create New Account";
        
        protected const String ctLandingFormTemplate        = "?_f=$FORMNAME";

        protected const String ctCMDCreateAccount           = "@cmd%createaccount";
        protected const String ctPRMCreateAccountParam      = "FormCreatePublicUser";

        public CompositeFormInterface SCI_ParentForm { get; set; }

        public String SC_UserNamePlaceHolder        { get; set; }
        public String SC_PasswordPlaceHolder        { get; set; }
        public String SC_ButtonText                 { get; set; }
        public String SC_CreateAccountButtonText    { get; set; }
        
        public String SC_UserIDSymbol               { get; set; }
        public String SC_PasswordSymbol             { get; set; }

        public String SC_UserName                   { get; set; }
        public String SC_Password                   { get; set; }

        public bool   SC_UserNameDisabled           { get; set; }
        public bool   SC_PasswordDisabled           { get; set; }

        public bool   SC_CreateAccountButtonVisible { get; set; }

        public bool   SC_PhoneNoMode                { get; set; }
        public bool   SC_PinCodeMode                { get; set; }
        

        public String SC_LandingPage 
        {
            get { return (clLandingPage); }
            set { clLandingPage = (!String.IsNullOrWhiteSpace(value)) ? ctLandingFormTemplate.Replace("$FORMNAME", General.Base64Encode(value)) : String.Empty; }
        }

        public String clLandingPage;
                
        public WidControlLogIn()
        {
            SC_UserNamePlaceHolder      = ctDEFUserNamePlaceHolder;
            SC_PasswordPlaceHolder      = ctDEFPasswordPlaceHolder;
            SC_CreateAccountButtonText  = ctDEFCreateAccountButtonText;

            SC_UserIDSymbol             = ComponentController.UnicodeStr((int)Fontawesome.user);
            SC_PasswordSymbol           = ComponentController.UnicodeStr((int)Fontawesome.pad_lock);
                        
            SC_LandingPage              = String.Empty;

            SC_UserName                 = String.Empty;
            SC_Password                 = String.Empty;            

            SC_UserNameDisabled         = false;
            SC_PasswordDisabled         = false;

            SC_CreateAccountButtonVisible = false;

            SC_PhoneNoMode              = false;
            SC_PinCodeMode              = false;

            SC_ButtonText = ComponentController.UnicodeStr((int)Fontawesome.arrow_right);

            clLandingPage = String.Empty;
        }              
       
        protected void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;
            
            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.System, ctWidControlLogInStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.System, ctWidControlLogInScript));
        }
        
        protected void RenderUserNameFieldDiv(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSField);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.UserName);                        
            paComponentController.AddAttribute(HtmlAttribute.PlaceHolder, SC_UserNamePlaceHolder);
            paComponentController.AddAttribute(HtmlAttribute.Type, "text");
            paComponentController.AddAttribute(HtmlAttribute.Name, ctNAMELoginID);
            
            if (SC_PhoneNoMode) paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_InputMode, "phoneno");
            if (SC_UserNameDisabled) paComponentController.AddAttribute(HtmlAttribute.Disabled, true.ToString());
            if (!String.IsNullOrWhiteSpace(SC_UserName)) paComponentController.AddAttribute(HtmlAttribute.Value, SC_UserName);

            paComponentController.RenderBeginTag(HtmlTag.Input);
            paComponentController.RenderEndTag();
          
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(SC_UserIDSymbol);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        protected void RenderPasswordFieldDiv(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSField);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.Password);                        
            paComponentController.AddAttribute(HtmlAttribute.PlaceHolder, SC_PasswordPlaceHolder);
            paComponentController.AddAttribute(HtmlAttribute.Type, "password");
            paComponentController.AddAttribute(HtmlAttribute.Name, ctNAMEPassword);

            if (SC_PinCodeMode) paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_InputMode, "pincode");
            if (SC_PasswordDisabled) paComponentController.AddAttribute(HtmlAttribute.Disabled, true.ToString());
            if (!String.IsNullOrWhiteSpace(SC_Password)) paComponentController.AddAttribute(HtmlAttribute.Value, SC_Password);

            paComponentController.RenderBeginTag(HtmlTag.Input);
            paComponentController.RenderEndTag();

            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(SC_PasswordSymbol);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        protected void RenderLogInButtonDiv(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSLogInButtonDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.LogInButton);            
            paComponentController.AddAttribute(HtmlAttribute.Type, "button");
            paComponentController.RenderBeginTag(HtmlTag.Button);
            paComponentController.Write(SC_ButtonText);
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

        protected void RenderCreateAccountButton(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCreateAccountButton);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Parameter, ctPRMCreateAccountParam);
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDCreateAccount);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(SC_CreateAccountButtonText);
            paComponentController.RenderEndTag();
        }

        protected void RenderBrowserMode(ComponentController paComponentController)
        {
            IncludeExternalLinkFiles(paComponentController);

            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlLogIn);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_LandingPage, SC_LandingPage);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderUserNameFieldDiv(paComponentController);
            RenderPasswordFieldDiv(paComponentController);
            RenderLogInButtonDiv(paComponentController);
            RenderStatusBar(paComponentController);
            if (SC_CreateAccountButtonVisible) RenderCreateAccountButton(paComponentController);

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
