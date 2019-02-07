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
    public class WidControlPOSPrinterSetting : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlPOSPrinterSettingStyle   = "WidControlPOSPrinterSetting.css";
        protected const String ctWidControlPOSPrinterSettingScript  = "WidControlPOSPrinterSetting.js";

        const String ctCLSWidControlPOSPrinterSetting               = "WidControlPOSPrinterSetting";
        const String ctCLSTitleBar                                  = "TitleBar";
        const String ctCLSContentContainer                          = "ContentContainer";
        const String ctCLSRow                                       = "Row";
        const String ctCLSInputDiv                                  = "InputDiv";
        const String ctCLSConnectionOverlay                         = "ConnectionOverlay";
        
        const String ctDYTPrinterSettingTitle                       = "@@POS.PrinterSetting.Title";        
        const String ctDYTPrinterNameLabel                          = "@@POS.PrinterSetting.PrinterName";
        const String ctDYTIPAddressLabel                            = "@@POS.PrinterSetting.IPAddress";
        const String ctDYTPortLabel                                 = "@@POS.PrinterSetting.Port";
        const String ctDYTDeviceIDLabel                             = "@@POS.PrinterSetting.DeviceID";
        const String ctDYTDarknessLabel                             = "@@POS.PrinterSetting.Darkness";
        const String ctDYTMonitorInterval                           = "@@POS.PrinterSetting.MonitorInterval";
        const String ctDYTReconnectInterval                         = "@@POS.PrinterSetting.ReconnectInterval";
        const String ctDYTPanelTitle                                = "@@POS.PrinterSetting.PrinterListTitle";
        const String ctDYTPrinterConnecting                         = "@@POS.PrinterSetting.PrinterConnecting";
        
        const String ctSETPrimaryPrinterSetting                     = "POS.PrimaryPrinterSetting";
        const String ctSETTestPrintReceipt                          = "POS.TestPrintReceipt";

        const String ctAJAXBigCircle                                = "AJAX_IndicatorBigCircle.gif";

        const String ctCTNContent                                   = "content";
        const String ctCTNExternalComponent                         = "externalcomponent";
        
        const String ctINPIPAddress                                 = "ipaddress";
        const String ctINPNumber                                    = "number";

        const String ctPanelType                                    = "printerlist";
        const String ctPanelAppearance                              = "wide";

        const String ctCMDPrinterList                               = "@cmd%printerlist";

        private enum PrinterSettingKey { PrinterName, IPAddress, Port, DeviceID, Darkness, MonitorInterval, ReconnectInterval };
        
        public CompositeFormInterface SCI_ParentForm { get; set; }

        LanguageManager clLanguageManager;
        SettingManager  clSettingManager;
        String          clPrimaryPrinterSetting;
        String          clTestPrintTemplate;

        public WidControlPOSPrinterSetting()
        {
            clLanguageManager           = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSettingManager            = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;
            clPrimaryPrinterSetting     = General.Base64Encode(clSettingManager.GetSettingValue(ctSETPrimaryPrinterSetting, "{}"));
            clTestPrintTemplate         = General.Base64Encode(clSettingManager.GetSettingValue(ctSETTestPrintReceipt, "{}"));
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSPrinterSettingStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSPrinterSettingScript));
        }        

        private void RenderTitle(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Title);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTitleBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(ctDYTPrinterSettingTitle));
            paComponentController.RenderEndTag();
        }

        private void RenderStatusOverlay(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSConnectionOverlay);
            paComponentController.AddElementType(ComponentController.ElementType.Overlay);            
            paComponentController.RenderBeginTag(HtmlTag.Div);
                        
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetAjaxLoaderImageUrl(ctAJAXBigCircle));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.AddElementType(ComponentController.ElementType.Label);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(ctDYTPrinterConnecting));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }
        
        private void RenderSelectionPanel(ComponentController paComponentController)
        {
            SubControlSelectionPanel    lcSubControlSelectionPanel;

            lcSubControlSelectionPanel = new SubControlSelectionPanel(ctPanelType, ctPanelAppearance, ctDYTPanelTitle, PrinterController.GetInstance().GetPrinterNameDictionary());

            lcSubControlSelectionPanel.RenderChildMode(paComponentController);
        }

        private void RenderReceiptOutput(ComponentController paComponentController)
        {
            SubControlPOSReceiptOutput  lcSubControlPOSReceiptOutput;

            lcSubControlPOSReceiptOutput = new SubControlPOSReceiptOutput();

            lcSubControlPOSReceiptOutput.RenderChildMode(paComponentController);
        }

        private void RenderRow(ComponentController paComponentController, String paLabel, PrinterSettingKey paPrinterSettingKey,  String paInputMode, int paMaxLength, String paCommand = null)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, paPrinterSettingKey.ToString());   
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSRow);            
            paComponentController.AddElementType(ComponentController.ElementType.Row);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.InputLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(paLabel));
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInputDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_InputMode, paInputMode);
            
            if (paMaxLength > 0) paComponentController.AddAttribute(HtmlAttribute.Maxlength, paMaxLength.ToString());
            else paComponentController.AddAttribute(HtmlAttribute.ReadOnly, HtmlAttribute.ReadOnly.ToString());

            paComponentController.AddElementType(ComponentController.ElementType.InputBox);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, paCommand);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctSETPrimaryPrinterSetting);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Name, paPrinterSettingKey.ToString());                     
            paComponentController.AddAttribute(HtmlAttribute.Type, "text");            
            paComponentController.RenderBeginTag(HtmlTag.Input);
            paComponentController.RenderEndTag();

            if (paMaxLength == 0)
            {
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, paCommand);            
                paComponentController.AddElementType(ComponentController.ElementType.Button);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(ComponentController.UnicodeStr((int) Fontawesome.ellipsis_h));
                paComponentController.RenderEndTag();
            }

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderContentContainer(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSContentContainer);
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctCTNContent);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderRow(paComponentController, ctDYTPrinterNameLabel, PrinterSettingKey.PrinterName, String.Empty, 0, ctCMDPrinterList);
            RenderRow(paComponentController, ctDYTIPAddressLabel, PrinterSettingKey.IPAddress, ctINPIPAddress, 15);
            RenderRow(paComponentController, ctDYTPortLabel, PrinterSettingKey.Port, ctINPNumber, 5);
            RenderRow(paComponentController, ctDYTDeviceIDLabel, PrinterSettingKey.DeviceID, String.Empty, 20);
//             RenderRow(paComponentController, ctDYTDarknessLabel, PrinterSettingKey.Darkness, ctINPNumber, 3);
            RenderRow(paComponentController, ctDYTMonitorInterval, PrinterSettingKey.MonitorInterval,ctINPNumber, 5);
            RenderRow(paComponentController, ctDYTReconnectInterval, PrinterSettingKey.ReconnectInterval, ctINPNumber, 5);

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
            IncludeExternalLinkFiles(paComponentController);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_AdditionalData, PrinterController.GetInstance().PrinterListBase64JSON);
            paComponentController.AddElementAttribute(ctSETPrimaryPrinterSetting, clPrimaryPrinterSetting);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Template, clTestPrintTemplate);
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSPrinterSetting);
            paComponentController.RenderBeginTag(HtmlTag.Div);
             
            RenderTitle(paComponentController);
            RenderContentContainer(paComponentController);
            RenderStatusOverlay(paComponentController);
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

