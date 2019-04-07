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
        const String ctCLSInputDiv                                          = "InputDiv";

        const String ctDYTReceiptLayoutTitle                                = "@@POS.ReceiptLayoutSetting.ReceiptLayoutTitle";
        const String ctDYTLayoutName                                        = "@@POS.ReceiptLayoutSetting.LayoutName";
        const String ctDYTReceiptWidth                                      = "@@POS.ReceiptLayoutSetting.ReceiptWidth";
        const String ctDYTTopMagin                                          = "@@POS.ReceiptLayoutSetting.TopMargin";
        const String ctDYTLeftMagin                                         = "@@POS.ReceiptLayoutSetting.LeftMargin";
        const String ctDYTDarkness                                          = "@@POS.ReceiptLayoutSetting.Darkness";
        const String ctDYTLocalNumberMode                                   = "@@POS.ReceiptLayoutSetting.LocalNumberMode";
        const String ctDYTCopies                                            = "@@POS.ReceiptLayoutSetting.Copies";

        const String ctDYTReceiptScaleTitle                                 = "@@POS.ReceiptLayoutSetting.ReceiptScaleTitle";   
        const String ctDYTLayoutScaleX                                      = "@@POS.ReceiptLayoutSetting.LayoutScaleX";
        const String ctDYTLayoutScaleY                                      = "@@POS.ReceiptLayoutSetting.LayoutScaleY";
        const String ctDYTFontScale                                         = "@@POS.ReceiptLayoutSetting.FontScale";

        const String ctDYTPanelTitle                                        = "@@POS.ReceiptLayoutSetting.LayoutListTitle";
        
        const String ctBLKReceiptLayout                                     = "receiptlayout";
        const String ctBLKReceiptScale                                      = "receiptscale";
                
        const String ctSETReceiptLayoutSetting                              = "POS.ReceiptLayoutInfo.LayoutSetting";        
        const String ctSETTestPrintReceipt                                  = "POS.TestPrintReceipt";
        const String ctSETReceiptLayoutList                                 = "POS.ReceiptLayoutInfo.LayoutList";        
        
        const String ctKEYLayoutName                                        = "LayoutName";
        const String ctKEYReceiptWidth                                      = "ReceiptWidth";
        const String ctKEYCopies                                            = "Copies";
        const String ctKEYTopMargin                                         = "TopMargin";
        const String ctKEYLeftMargin                                        = "LeftMargin";
        const String ctKEYDarkness                                          = "Darkness";
        const String ctKEYLocalNumberMode                                   = "LocalNumberMode";
        const String ctKEYLayoutScaleX                                      = "LayoutScaleX";
        const String ctKEYLayoutScaleY                                      = "LayoutScaleY";
        const String ctKEYFontScale                                         = "FontScale";

        const String ctCTNContent                                           = "content";
        const String ctCTNExternalComponent                                 = "externalcomponent";

        const String ctCMDToggle                                            = "@cmd%toggle";
        const String ctCMDLayoutList                                        = "@cmd%layoutlist";

        const String ctPanelType                                            = "layoutlist";
        const String ctPanelAppearance                                      = "wide";

        const int   ctNumberBoxMaxLength                                    = 3;

        public CompositeFormInterface SCI_ParentForm { get; set; }

        LanguageManager     clLanguageManager;
        SettingManager      clSettingManager;
        
        public WidControlPOSReceiptLayoutSetting()
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

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctSETReceiptLayoutSetting);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Name, paName);            
            paComponentController.AddElementType(ComponentController.ElementType.Button);
            RenderToggleButton(paComponentController);

            paComponentController.RenderEndTag();
        }

        private void RenderNumberInputBoxRow(ComponentController paComponentController, String paName, String paLabel, String paInputMode, String paSuffix, int paMaxLength, int paLowerBound, int paUpperBound)
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
            paComponentController.Write(paSuffix);
            paComponentController.RenderEndTag();

            paComponentController.AddElementType(ComponentController.ElementType.InputBox);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctSETReceiptLayoutSetting);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Name, paName);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_InputMode, paInputMode);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, paSuffix);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_UpperBound, paUpperBound.ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_LowerBound, paLowerBound.ToString());
            paComponentController.AddAttribute(HtmlAttribute.Maxlength, paMaxLength.ToString());
            paComponentController.AddAttribute(HtmlAttribute.Type, "text");
            paComponentController.RenderBeginTag(HtmlTag.Input);
            

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderSelectionInputRow(ComponentController paComponentController, String paName, String paLabel, String paCommand = null)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "selectioninputrow");            
            paComponentController.AddElementType(ComponentController.ElementType.InputRow);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.InputLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(paLabel));
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInputDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, paCommand);
            paComponentController.AddElementType(ComponentController.ElementType.Button);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.ellipsis_h));
            paComponentController.RenderEndTag();            

            paComponentController.AddAttribute(HtmlAttribute.ReadOnly, HtmlAttribute.ReadOnly.ToString());
            paComponentController.AddElementType(ComponentController.ElementType.InputBox);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "selection");
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, paCommand);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctSETReceiptLayoutSetting);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Name, paName);
            paComponentController.AddAttribute(HtmlAttribute.Type, "text");
            paComponentController.RenderBeginTag(HtmlTag.Input);
            paComponentController.RenderEndTag();
            
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }       

        private void RenderSelectionPanel(ComponentController paComponentController)
        {
            Dictionary<String, String> lcReceiptLayoutList;
            SubControlSelectionPanel    lcSubControlSelectionPanel;

            lcReceiptLayoutList = General.JSONDeserialize<Dictionary<String, String>>(clSettingManager.GetSettingValue(ctSETReceiptLayoutList, "{}"));

            lcSubControlSelectionPanel = new SubControlSelectionPanel(ctPanelType, ctPanelAppearance, ctDYTPanelTitle, lcReceiptLayoutList);

            lcSubControlSelectionPanel.RenderChildMode(paComponentController);
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
            RenderSelectionInputRow(paComponentController, ctKEYLayoutName,ctDYTLayoutName,ctCMDLayoutList);
            RenderNumberInputBoxRow(paComponentController, ctKEYCopies, ctDYTCopies, "number", "", ctNumberBoxMaxLength, 0, 10);
            RenderNumberInputBoxRow(paComponentController,ctKEYReceiptWidth, ctDYTReceiptWidth,"number", "px", ctNumberBoxMaxLength,-1,-1);
            RenderNumberInputBoxRow(paComponentController, ctKEYTopMargin, ctDYTTopMagin, "number", "px", ctNumberBoxMaxLength, -1, -1);
            RenderNumberInputBoxRow(paComponentController, ctKEYLeftMargin, ctDYTLeftMagin, "number", "px", ctNumberBoxMaxLength, -1, -1);
            RenderNumberInputBoxRow(paComponentController, ctKEYDarkness, ctDYTDarkness, "number", "%", ctNumberBoxMaxLength, 0, 100);
            RenderToggleButtonRow(paComponentController, ctKEYLocalNumberMode, ctDYTLocalNumberMode);

            paComponentController.RenderEndTag();            
        }

        private void RenderReceiptScalePanel(ComponentController paComponentController)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctBLKReceiptScale);
            paComponentController.AddElementType(ComponentController.ElementType.Block);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSettingGroup);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderSectionHeader(paComponentController, ctDYTReceiptScaleTitle);
            RenderNumberInputBoxRow(paComponentController, ctKEYLayoutScaleX, ctDYTLayoutScaleX, "decimal", "", ctNumberBoxMaxLength, 1, 10);
            RenderNumberInputBoxRow(paComponentController, ctKEYLayoutScaleY, ctDYTLayoutScaleY, "decimal", "", ctNumberBoxMaxLength, 1, 10);
            RenderNumberInputBoxRow(paComponentController, ctKEYFontScale, ctDYTFontScale, "decimal", "", ctNumberBoxMaxLength, 1, 10);            

            paComponentController.RenderEndTag();
        }

        private void RenderContainer(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSContainer);
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctCTNContent);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderReceiptLayoutPanel(paComponentController);
            RenderReceiptScalePanel(paComponentController);             

            paComponentController.RenderEndTag();
        }

        private void RenderExternalComponents(ComponentController paComponentController)
        {
            RenderSelectionPanel(paComponentController);
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
            String lcBase64ReceiptLayoutSetting;
            String lcBase64TestPrintTemplate;
            String lcBase64ReceiptLayoutList;

            lcBase64ReceiptLayoutSetting    = General.Base64Encode(clSettingManager.GetSettingValue(ctSETReceiptLayoutSetting, "{}"));
            lcBase64TestPrintTemplate       = General.Base64Encode(clSettingManager.GetSettingValue(ctSETTestPrintReceipt, "{}"));
            lcBase64ReceiptLayoutList       = General.Base64Encode(clSettingManager.GetSettingValue(ctSETReceiptLayoutList, "{}"));
                        
            IncludeExternalLinkFiles(paComponentController);

            paComponentController.AddElementType(ComponentController.ElementType.Control);            
            paComponentController.AddElementAttribute(ctSETReceiptLayoutSetting, lcBase64ReceiptLayoutSetting);
            paComponentController.AddElementAttribute(ctSETReceiptLayoutList, lcBase64ReceiptLayoutList);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Template, lcBase64TestPrintTemplate);            
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

