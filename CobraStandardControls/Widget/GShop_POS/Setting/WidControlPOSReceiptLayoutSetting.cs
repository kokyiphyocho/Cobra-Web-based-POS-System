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
    public class WidControlPOSReceiptLayoutSetting : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlPOSReceiptLayoutSettingStyle     = "WidControlPOSReceiptLayoutSetting.css";
        protected const String ctWidControlPOSReceiptLayoutSettingScript    = "WidControlPOSReceiptLayoutSetting.js";

        const String ctCLSWidControlPOSReceiptLayoutSetting                 = "WidControlPOSReceiptLayoutSetting";
        const String ctCLSContainer                                         = "Container";
        const String ctCLSSettingGroup                                      = "SettingGroup";
        const String ctCLSSectionHeader                                     = "SectionHeader";
        const String ctCLSToggleSwitch                                      = "ToggleSwitch";    
        const String ctCLSLogoImageDiv                                      = "LogoImageDiv";

        const String ctDYTReceiptLayoutTitle                                = "@@POS.ReceiptLayoutSetting.ReceiptLayoutTitle";
        const String ctDYTReceiptWidth                                      = "@@POS.ReceiptLayoutSetting.ReceiptWidth";
        const String ctDYTDarkness                                          = "@@POS.ReceiptLayoutSetting.Darkness";
        const String ctDYTLocalNumberMode                                   = "@@POS.ReceiptLayoutSetting.LocalNumberMode";
        const String ctDYTHeaderTextTitle                                   = "@@POS.ReceiptLayoutSetting.HeaderTextTitle";
        const String ctDYTBusinessName                                      = "@@POS.ReceiptLayoutSetting.BusinessName";
        const String ctDYTAddress                                           = "@@POS.ReceiptLayoutSetting.Address";
        const String ctDYTFootNoteTitle                                     = "@@POS.ReceiptLayoutSetting.FootNoteTitle";
        const String ctDYTFootNote                                          = "@@POS.ReceiptLayoutSetting.FootNote";
        const String ctDYTReceiptLogoTitle                                  = "@@POS.ReceiptLayoutSetting.ReceiptLogoTitle";
        const String ctDYTReceiptLogo                                       = "@@POS.ReceiptLayoutSetting.ReceiptLogo";
        
        const String ctBLKReceiptLayout                                     = "receiptlayout";
        const String ctBLKHeaderText                                        = "headertext";
        const String ctBLKReceiptLogo                                       = "receiptlogo";
        const String ctBLKFootNote                                          = "footnote";

        const String ctSETReceiptCustomization                              = "POS.ReceiptLayoutInfo.Customization";
        const String ctSETReceiptLayout                                     = "POS.ReceiptLayoutInfo.Layout";
        const String ctSETPrimaryPrinterSetting                             = "POS.PrimaryPrinterSetting";
        const String ctSETTestPrintReceipt                                  = "POS.TestPrintReceipt";

        const String ctKEYBusinessName                                      = "BusinessName";
        const String ctKEYAddress                                           = "Address";
        const String ctKEYFootNote                                          = "FootNote";
        const String ctKEYWidth                                             = "Width";
        const String ctKEYDarkness                                          = "Darkness";
        const String ctKEYNumberMode                                        = "LocalNumberMode";
        const String ctKEYLogoName                                          = "LogoName";

        const String ctTYPBusinessName                                      = "businessname";
        const String ctTYPAddress                                           = "address";
        const String ctTYPLogoName                                          = "logoname";
        const String ctTYPFootNote                                          = "footnote";

        const String ctCTNContent                                           = "content";
        const String ctCTNExternalComponent                                 = "externalcomponent";

        const String ctCMDToggle                                            = "@cmd%toggle";

        const String ctLogoFileName                                         = "System/ReceiptLogo.png";

        const int   ctTextBoxMaxLength                                      = 300;
        const int   ctNumberBoxMaxLength                                    = 3;

        public CompositeFormInterface SCI_ParentForm { get; set; }

        LanguageManager     clLanguageManager;
        SettingManager      clSettingManager;
        String              clTestPrintTemplate;

        public WidControlPOSReceiptLayoutSetting()
        {
            clLanguageManager       = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSettingManager        = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;
            clTestPrintTemplate     = General.Base64Encode(clSettingManager.GetSettingValue(ctSETTestPrintReceipt, "{}"));
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSReceiptLayoutSettingStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSReceiptLayoutSettingScript));
        }

        protected void RenderToggleButton(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSToggleSwitch);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDToggle);
            paComponentController.AddElementType(ComponentController.ElementType.Button);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "leftbar");
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "key");
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderSectionHeader(ComponentController paComponentController, String paHeadingText)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSectionHeader);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.Write(clLanguageManager.GetText(paHeadingText));

            paComponentController.RenderEndTag();
        }

        private void RenderToggleButtonRow(ComponentController paComponentController, String paName,  String paLabel)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "togglebutton");
            paComponentController.AddElementType(ComponentController.ElementType.InputRow);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.InputLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(paLabel));
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctSETReceiptLayout);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Name, paName);            
            paComponentController.AddElementType(ComponentController.ElementType.Button);
            RenderToggleButton(paComponentController);

            paComponentController.RenderEndTag();
        }

        private void RenderNumberInputBoxRow(ComponentController paComponentController, String paColumnName, String paName, String paLabel, String paType, int paMaxLength, int paLowerBound, int paUpperBound)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "numberinputrow");
            paComponentController.AddElementType(ComponentController.ElementType.InputRow);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.InputLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(paLabel));
            paComponentController.RenderEndTag();


            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "unit");
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(paType == "pixel" ? "px" : "%");
            paComponentController.RenderEndTag();

            paComponentController.AddElementType(ComponentController.ElementType.InputBox);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, paColumnName);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Name, paName);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_InputMode, "number");
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, paType);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_UpperBound, paUpperBound.ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_LowerBound, paLowerBound.ToString());
            paComponentController.AddAttribute(HtmlAttribute.Maxlength, paMaxLength.ToString());
            paComponentController.AddAttribute(HtmlAttribute.Type, "text");
            paComponentController.RenderBeginTag(HtmlTag.Input);

            

            paComponentController.RenderEndTag();

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

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSLogoImageDiv);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "logoimage");
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctSETReceiptCustomization);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Name, ctKEYLogoName);            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctTYPLogoName);                        
            paComponentController.RenderBeginTag(HtmlTag.Img);            
            paComponentController.RenderEndTag();


            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, "");
            paComponentController.AddElementType(ComponentController.ElementType.Button);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.ellipsis_h));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderReceiptOutput(ComponentController paComponentController)
        {
            SubControlPOSReceiptOutput lcSubControlPOSReceiptOutput;

            lcSubControlPOSReceiptOutput = new SubControlPOSReceiptOutput();

            lcSubControlPOSReceiptOutput.RenderChildMode(paComponentController);
        }

        private void RenderReceiptLayoutPanel(ComponentController paComponentController)
        {                        
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctBLKReceiptLayout);
            paComponentController.AddElementType(ComponentController.ElementType.Block);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSettingGroup);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderSectionHeader(paComponentController, ctDYTReceiptLayoutTitle);
            RenderNumberInputBoxRow(paComponentController,ctSETReceiptLayout, ctKEYWidth, ctDYTReceiptWidth,"pixel", ctNumberBoxMaxLength,-1,-1);
            RenderNumberInputBoxRow(paComponentController,ctSETPrimaryPrinterSetting, ctKEYDarkness, ctDYTDarkness,"percent", ctNumberBoxMaxLength,1,100);
            RenderToggleButtonRow(paComponentController, ctKEYNumberMode, ctDYTLocalNumberMode);

            paComponentController.RenderEndTag();            
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

            RenderReceiptLayoutPanel(paComponentController);
            RenderHeaderTextPanel(paComponentController);
     //       RenderReceiptLogoPanel(paComponentController);
            RenderFootNotePanel(paComponentController);           

            paComponentController.RenderEndTag();
        }

        private void RenderExternalComponents(ComponentController paComponentController)
        {            
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
            String lcBase64ReceiptLayout;
            String lcBase64PrimaryPrinterSetting;

            lcBase64ReceiptCustomization    = General.Base64Encode(clSettingManager.GetSettingValue(ctSETReceiptCustomization));
            lcBase64ReceiptLayout           = General.Base64Encode(clSettingManager.GetSettingValue(ctSETReceiptLayout));
            lcBase64PrimaryPrinterSetting   = General.Base64Encode(clSettingManager.GetSettingValue(ctSETPrimaryPrinterSetting));

            IncludeExternalLinkFiles(paComponentController);

            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddElementAttribute(ctSETReceiptCustomization, lcBase64ReceiptCustomization);
            paComponentController.AddElementAttribute(ctSETReceiptLayout, lcBase64ReceiptLayout);
            paComponentController.AddElementAttribute(ctSETPrimaryPrinterSetting, lcBase64PrimaryPrinterSetting);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Template, clTestPrintTemplate);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSReceiptLayoutSetting);
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

