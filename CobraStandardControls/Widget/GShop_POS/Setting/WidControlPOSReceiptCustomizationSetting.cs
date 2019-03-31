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
using System.IO;

namespace CobraStandardControls
{
    public class WidControlPOSReceiptCustomizationSetting : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlPOSReceiptCustomizationSettingStyle     = "WidControlPOSReceiptCustomizationSetting.css";
        protected const String ctWidControlPOSReceiptCustomizationSettingScript    = "WidControlPOSReceiptCustomizationSetting.js";

        const String ctCLSWidControlPOSReceiptCustomizationSetting                 = "WidControlPOSReceiptCustomizationSetting";
        const String ctCLSContainer                                         = "Container";
        const String ctCLSSettingGroup                                      = "SettingGroup";
        const String ctCLSSectionHeader                                     = "SectionHeader";
        const String ctCLSToggleSwitch                                      = "ToggleSwitch";    
        const String ctCLSLogoImageDiv                                      = "LogoImageDiv";
        const String ctCLSImageWrapperDiv                                   = "ImageWrapperDiv";

        const String ctDYTHeaderTextTitle                                   = "@@POS.ReceiptCustomizationSetting.HeaderTextTitle";
        const String ctDYTBusinessName                                      = "@@POS.ReceiptCustomizationSetting.BusinessName";
        const String ctDYTAddress                                           = "@@POS.ReceiptCustomizationSetting.Address";
        const String ctDYTFootNoteTitle                                     = "@@POS.ReceiptCustomizationSetting.FootNoteTitle";
        const String ctDYTFootNote                                          = "@@POS.ReceiptCustomizationSetting.FootNote";
        const String ctDYTReceiptLogoTitle                                  = "@@POS.ReceiptCustomizationSetting.ReceiptLogoTitle";
        const String ctDYTReceiptLogo                                       = "@@POS.ReceiptCustomizationSetting.ReceiptLogo";

        const String ctDYTImagePopUpTitle                                   = "@@POS.ReceiptCutomizationSetting.ImagePopUp.Title";
        
        const String ctBLKReceiptLayout                                     = "receiptlayout";
        const String ctBLKHeaderText                                        = "headertext";
        const String ctBLKReceiptLogo                                       = "receiptlogo";
        const String ctBLKFootNote                                          = "footnote";

        const String ctSETReceiptCustomization                              = "POS.ReceiptLayoutInfo.Customization";
        const String ctSETTestPrintReceipt                                  = "POS.TestPrintReceipt";

        const String ctKEYBusinessName                                      = "BusinessName";
        const String ctKEYAddress                                           = "Address";
        const String ctKEYFootNote                                          = "FootNote";
        const String ctKEYWidth                                             = "Width";
        const String ctKEYDarkness                                          = "Darkness";
        const String ctKEYNumberMode                                        = "LocalNumberMode";
        const String ctKEYLogoName                                          = "LogoName";
        const String ctKEYRenderLogo                                        = "RenderLogo";

        const String ctTYPBusinessName                                      = "businessname";
        const String ctTYPAddress                                           = "address";
        const String ctTYPLogoName                                          = "logoname";
        const String ctTYPFootNote                                          = "footnote";

        const String ctCTNContent                                           = "content";
        const String ctCTNExternalComponent                                 = "externalcomponent";

        const String ctCMDOpenImagePopUp                                    = "@cmd%openimagepopup";
        const String ctCMDSuppressImage                                     = "@cmd%suppressimage";

        const int   ctTextBoxMaxLength                                      = 300;
        
        public CompositeFormInterface SCI_ParentForm { get; set; }

        LanguageManager     clLanguageManager;
        SettingManager      clSettingManager;        

        public WidControlPOSReceiptCustomizationSetting()
        {
            clLanguageManager       = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSettingManager        = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;           
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSReceiptCustomizationSettingStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSReceiptCustomizationSettingScript));
        }

        private void RenderSectionHeader(ComponentController paComponentController, String paHeadingText)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSectionHeader);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.Write(clLanguageManager.GetText(paHeadingText));

            paComponentController.RenderEndTag();
        }

        private void RenderTextInputBoxRow(ComponentController paComponentController, String paType, String paName, String paLabel, int paMaxLength)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "textinputrow");
            paComponentController.AddElementType(ComponentController.ElementType.InputRow);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.InputLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(paLabel));
            paComponentController.RenderEndTag();

            paComponentController.AddElementType(ComponentController.ElementType.InputBox);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, paType);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctSETReceiptCustomization);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Name, paName);
            paComponentController.AddAttribute(HtmlAttribute.Maxlength, paMaxLength.ToString());
            paComponentController.AddAttribute(HtmlAttribute.Type, "text");
            paComponentController.RenderBeginTag(HtmlTag.Input);

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderTextAreaRow(ComponentController paComponentController, String paType, String paName, String paLabel, int paMaxLength)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "textarearow");
            paComponentController.AddElementType(ComponentController.ElementType.InputRow);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.InputLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(paLabel));
            paComponentController.RenderEndTag();

            paComponentController.AddElementType(ComponentController.ElementType.InputBox);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, paType);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctSETReceiptCustomization);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Name, paName);
            paComponentController.AddAttribute(HtmlAttribute.Maxlength, paMaxLength.ToString());            
            paComponentController.RenderBeginTag(HtmlTag.Textarea);

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderLogoImageInput(ComponentController paComponentController)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "imagerow");
            paComponentController.AddElementType(ComponentController.ElementType.InputRow);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.InputLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(ctDYTReceiptLogo));
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctSETReceiptCustomization);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Name, ctKEYRenderLogo);
            paComponentController.AddElementType(ComponentController.ElementType.Panel);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSLogoImageDiv);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "logoimage");
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.Wrapper);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSImageWrapperDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctSETReceiptCustomization);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Name, ctKEYLogoName);            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctTYPLogoName);                        
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Path, UploadManager.GetInstance().UploadPath);
            paComponentController.AddAttribute(HtmlAttribute.Value, UploadManager.GetInstance().ReceiptLogoFileName);
            paComponentController.RenderBeginTag(HtmlTag.Img);            
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDOpenImagePopUp);
            paComponentController.AddElementType(ComponentController.ElementType.Button);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.picture));
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDSuppressImage);
            paComponentController.AddElementType(ComponentController.ElementType.Button);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.ban_circle));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderImageUploader(ComponentController paComponentController)
        {
            SubControlImageProcessor lcSubControlImageProcessor;

            lcSubControlImageProcessor = new SubControlImageProcessor(clLanguageManager.GetText(ctDYTImagePopUpTitle));

            lcSubControlImageProcessor.RenderChildMode(paComponentController);
        }

        private void RenderReceiptOutput(ComponentController paComponentController)
        {
            SubControlPOSReceiptOutput lcSubControlPOSReceiptOutput;

            lcSubControlPOSReceiptOutput = new SubControlPOSReceiptOutput();

            lcSubControlPOSReceiptOutput.RenderChildMode(paComponentController);
        }       

        private void RenderHeaderTextPanel(ComponentController paComponentController)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctBLKHeaderText);
            paComponentController.AddElementType(ComponentController.ElementType.Block);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSettingGroup);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderSectionHeader(paComponentController, ctDYTHeaderTextTitle);
            RenderTextInputBoxRow(paComponentController, ctTYPBusinessName, ctKEYBusinessName, ctDYTBusinessName, ctTextBoxMaxLength);
            RenderTextAreaRow(paComponentController, ctTYPAddress, ctKEYAddress, ctDYTAddress, ctTextBoxMaxLength);
            
            paComponentController.RenderEndTag();  
        }

        private void RenderReceiptLogoPanel(ComponentController paComponentController)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctBLKReceiptLogo);
            paComponentController.AddElementType(ComponentController.ElementType.Block);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSettingGroup);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderSectionHeader(paComponentController, ctDYTReceiptLogoTitle);
            RenderLogoImageInput(paComponentController);

            paComponentController.RenderEndTag();
        }

        private void RenderFootNotePanel(ComponentController paComponentController)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctBLKFootNote);
            paComponentController.AddElementType(ComponentController.ElementType.Block);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSettingGroup);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderSectionHeader(paComponentController, ctDYTFootNoteTitle);
            RenderTextAreaRow(paComponentController, ctTYPFootNote, ctKEYFootNote, ctDYTFootNote, ctTextBoxMaxLength);            

            paComponentController.RenderEndTag();
        }

        private void RenderContainer(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSContainer);
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctCTNContent);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderReceiptLogoPanel(paComponentController);
            RenderHeaderTextPanel(paComponentController);            
            RenderFootNotePanel(paComponentController);           

            paComponentController.RenderEndTag();
        }

        private void RenderExternalComponents(ComponentController paComponentController)
        {
            RenderImageUploader(paComponentController);
            RenderReceiptOutput(paComponentController);
        }

        private void RenderExternalComponentsContainer(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctCTNExternalComponent);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.RenderEndTag();
        }

        private void RenderBrowserMode(ComponentController paComponentController)
        {
            String lcBase64ReceiptCustomization;
            String lcBase64TestPrintTemplate;

            lcBase64ReceiptCustomization    = General.Base64Encode(clSettingManager.GetSettingValue(ctSETReceiptCustomization));
            lcBase64TestPrintTemplate       = General.Base64Encode(clSettingManager.GetSettingValue(ctSETTestPrintReceipt, "{}"));
            
            IncludeExternalLinkFiles(paComponentController);

            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddElementAttribute(ctSETReceiptCustomization, lcBase64ReceiptCustomization);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Template, lcBase64TestPrintTemplate);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSReceiptCustomizationSetting);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            RenderContainer(paComponentController);
            paComponentController.RenderEndTag();

            RenderExternalComponentsContainer(paComponentController);
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
            if (paRenderMode == null) RenderBrowserMode(paComponentController);
            else if (paRenderMode == "externalcomponent") RenderExternalComponents(paComponentController);
        }

        protected override void Render(HtmlTextWriter paHtmlTextWriter)
        {
            if (!DesignMode) RenderBrowserMode(new ComponentController(paHtmlTextWriter));
            else RenderDesignMode(new ComponentController(paHtmlTextWriter));
        }
    }
}

