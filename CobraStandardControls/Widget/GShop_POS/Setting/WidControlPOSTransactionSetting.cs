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
    public class WidControlPOSTransactionSetting : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlPOSTransactionSettingStyle   = "WidControlPOSTransactionSetting.css";
        protected const String ctWidControlPOSTransactionSettingScript  = "WidControlPOSTransactionSetting.js";

        const String ctCLSWidControlPOSTransactionSetting               = "WidControlPOSTransactionSetting";
        const String ctCLSContainer                                     = "Container";
        const String ctCLSSettingGroup                                  = "SettingGroup";
        const String ctCLSSectionHeader                                 = "SectionHeader";
        const String ctCLSToggleSwitch                                  = "ToggleSwitch";

        const String ctDYTReceiptPrintOptionTitle                       = "@@POS.TransactionSetting.ReceiptPrintOptionTitle";
        const String ctDYTReceiptPrintOptionTextTemplate                = "@@POS.TransactionSetting.ReceiptPrintOption.$RECEIPTMODE";
        const String ctDYTPaymentOptionTitle                            = "@@POS.TransactionSetting.PaymentOptionTitle";
        const String ctDYTShowPaymentForm                               = "@@POS.TransactionSetting.ShowPaymentForm";
        const String ctDYTPaymentModeTemplate                           = "@@POS.TransactionSetting.PaymentMode.$PAYMENTMODE";
        const String ctDYTTaxOptionTitle                                = "@@POS.TransactionSetting.TaxOptionTitle";
        const String ctDYTTaxApplicable                                 = "@@POS.TransactionSetting.TaxApplicable";
        const String ctDYTTaxPercent                                    = "@@POS.TransactionSetting.TaxPercent";
        
        const String ctBLKReceiptPrintOption                            = "receiptprintoption";
        const String ctBLKPaymentOption                                 = "paymentoption";
        const String ctBLKTaxOption                                     = "taxoption";

        const String ctSETTransactionSetting                            = "POS.TransactionSetting";

        const String ctKEYTaxFeature                                    = "taxfeature";

        const String ctKEYShowPaymentForm                               = "showpaymentform";
        const String ctKEYPaymentOption                                 = "paymentoption";
        const String ctKEYReceiptPrintOption                            = "receiptprintoption";
        const String ctKEYTaxApplicable                                 = "taxapplicable";
        const String ctKEYTaxInclusive                                  = "taxinclusive";
        const String ctKEYTaxPercent                                    = "taxpercent";

        const String ctDEFReceiptPrintOption                            = "sale,purchase";
        const String ctDEFPaymentOption                                 = "paymentcash";
        
        const String ctCMDToggle = "@cmd%toggle";

        const int    ctTextBoxMaxLength                                 = 2;

        public CompositeFormInterface SCI_ParentForm { get; set; }

        LanguageManager clLanguageManager;
        SettingManager clSettingManager;

        public WidControlPOSTransactionSetting()
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

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSTransactionSettingStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSTransactionSettingScript));
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

        private void RenderToggleButtonRow(ComponentController paComponentController, String paName, String paKeyValue, String paLabel, String paLinkColumn)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "togglebutton");
            paComponentController.AddElementType(ComponentController.ElementType.InputRow);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.InputLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(paLabel));
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctSETTransactionSetting);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Name, paName.ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_KeyValue, paKeyValue);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_LinkColumn, paLinkColumn);
            paComponentController.AddElementType(ComponentController.ElementType.Button);
            RenderToggleButton(paComponentController);

            paComponentController.RenderEndTag();
        }

        private void RenderInputBoxRow(ComponentController paComponentController, String paName, String paKeyValue, String paLabel, int paMaxLength)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "input");
            paComponentController.AddElementType(ComponentController.ElementType.InputRow);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.InputLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(paLabel));
            paComponentController.RenderEndTag();

            paComponentController.AddElementType(ComponentController.ElementType.InputBox);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_InputMode, "number");
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctSETTransactionSetting);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Name, paName.ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_KeyValue, paKeyValue);
            paComponentController.AddAttribute(HtmlAttribute.Maxlength, paMaxLength.ToString());
            paComponentController.AddAttribute(HtmlAttribute.Type, "text");
            paComponentController.RenderBeginTag(HtmlTag.Input);

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderReceiptPrintOptionPanel(ComponentController paComponentController)
        {
            String[]   lcReceiptPrintOption;
            String     lcKeyValue;

            if ((lcReceiptPrintOption = clSettingManager.GetSystemConfigDataArray(ctKEYReceiptPrintOption, ctDEFReceiptPrintOption)) != null)
            {
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctBLKReceiptPrintOption);
                paComponentController.AddElementType(ComponentController.ElementType.Block);
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSettingGroup);
                paComponentController.RenderBeginTag(HtmlTag.Div);

                RenderSectionHeader(paComponentController, ctDYTReceiptPrintOptionTitle);

                for (int lcCount = 0; lcCount < lcReceiptPrintOption.Length; lcCount++)
                {
                    lcKeyValue = lcReceiptPrintOption[lcCount].Trim();

                    if (!String.IsNullOrEmpty(lcKeyValue))
                        RenderToggleButtonRow(paComponentController, ctKEYReceiptPrintOption, lcKeyValue , ctDYTReceiptPrintOptionTextTemplate.Replace("$RECEIPTMODE", lcKeyValue), String.Empty);
                }               

                paComponentController.RenderEndTag();
            }
        }

        private void RenderPaymentOptionPanel(ComponentController paComponentController)
        {
            String[]    lcPaymentOption;
            String      lcKeyValue;

            if ((lcPaymentOption = clSettingManager.GetSystemConfigDataArray(ctKEYPaymentOption, ctDEFPaymentOption)) != null)
            {
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctBLKPaymentOption);
                paComponentController.AddElementType(ComponentController.ElementType.Block);
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSettingGroup);
                paComponentController.RenderBeginTag(HtmlTag.Div);

                RenderSectionHeader(paComponentController, ctDYTPaymentOptionTitle);
                
                RenderToggleButtonRow(paComponentController, ctKEYShowPaymentForm, null, ctDYTShowPaymentForm, ctKEYPaymentOption);

                for (int lcCount = 0; lcCount < lcPaymentOption.Length; lcCount++)
                {
                    lcKeyValue = lcPaymentOption[lcCount].Trim();

                    if (!String.IsNullOrEmpty(lcKeyValue))
                        RenderToggleButtonRow(paComponentController, ctKEYPaymentOption, lcKeyValue, ctDYTPaymentModeTemplate.Replace("$PAYMENTMODE", lcKeyValue), String.Empty);
                }                

                paComponentController.RenderEndTag();
            }            
        }

        private void RenderTaxOptionPanel(ComponentController paComponentController)
        {
            bool lcSystemTaxFeature;

            lcSystemTaxFeature = General.ParseBoolean(clSettingManager.SystemConfig.GetData(ctKEYTaxFeature, false.ToString()),false);

            if (lcSystemTaxFeature)
            {
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctBLKTaxOption);
                paComponentController.AddElementType(ComponentController.ElementType.Block);
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSettingGroup);
                paComponentController.RenderBeginTag(HtmlTag.Div);

                RenderSectionHeader(paComponentController, ctDYTTaxOptionTitle);
                RenderToggleButtonRow(paComponentController, ctKEYTaxApplicable, null, ctDYTTaxApplicable, ctKEYTaxPercent);
                RenderInputBoxRow(paComponentController, ctKEYTaxPercent, null, ctDYTTaxPercent, ctTextBoxMaxLength);

                paComponentController.RenderEndTag();
            }
        }

        private void RenderContainer(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSContainer);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderReceiptPrintOptionPanel(paComponentController);
            RenderPaymentOptionPanel(paComponentController);
            RenderTaxOptionPanel(paComponentController);
            
            paComponentController.RenderEndTag();
        }

        private void RenderBrowserMode(ComponentController paComponentController)
        {
            String lcBase64TransactionSetting;
            String lcBase64SystemConfig;

            lcBase64TransactionSetting = General.Base64Encode(clSettingManager.GetSettingValue(ctSETTransactionSetting));
            lcBase64SystemConfig = General.Base64Encode(clSettingManager.SystemConfigStr);

            IncludeExternalLinkFiles(paComponentController);

            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddElementAttribute(ctSETTransactionSetting, lcBase64TransactionSetting);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_SystemConfig, lcBase64SystemConfig);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSTransactionSetting);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderContainer(paComponentController);

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

