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
using System.Text.RegularExpressions;


namespace CobraStandardControls
{
    public class SubControlPOSPaymentPanel : WebControl, WidgetControlInterface
    {
        const String ctSubControlPOSPaymentPanelStyle                   = "SubControlPOSPaymentPanel.css";
        const String ctSubControlPOSPaymentPanelScript                  = "SubControlPOSPaymentPanel.js";

        const String ctCLSSubControlPOSPaymentPanelPopUp                = "SubControlPOSPaymentPanelPopUp";
        const String ctCLSSubControlPOSPaymentPanel                     = "SubControlPOSPaymentPanel";
        const String ctCLSTitleBar                                      = "TitleBar";        
        const String ctCLSTotalRow                                      = "TotalRow";
        const String ctCLSPaymentRow                                    = "PaymentRow";
        const String ctCLSChangeRow                                     = "ChangeRow";
        const String ctCLSTotalPaymentRow                               = "TotalPaymentRow";
        const String ctCLSContentConatiner                              = "ContentContainer";
        const String ctCLSPaymentConatiner                              = "PaymentContainer";

        const String ctDYTPaymentTitle                                  = "@@POS.Transaction.Payment.Title";
        const String ctDYTTotalAmount                                   = "@@POS.Transaction.Payment.TotalAmount";
        const String ctDYTTotalPayment                                  = "@@POS.Transaction.Payment.TotalPayment";
        const String ctDYTChange                                        = "@@POS.Transaction.Payment.Change";
        const String ctDYTPaymentType                                   = "@@POS.Transaction.Payment.$PAYMENTTYPE";

        const String ctCMDClosePopUp                                    = "@cmd%closepopup";

        // const String ctSETPaymentTypeList                               = "POS.PaymentTypeList";

        const String ctSETTransactionSetting                            = "POS.TransactionSetting";

        const String ctKEYPaymentOption                                 = "paymentoption";
        
        const String ctCompositeType                                    = "Payment"; 
        const String ctPaymentKeypad                                    = "POSPaymentKeypad";

        const String ctCOLTotalAmount                                   = "_totalamount";
        const String ctCOLTotalPayment                                  = "_totalpayment";
        const String ctCOLChange                                        = "change";
                
        const String ctDEFPaymentType                                   = "paymentcash";
                
        LanguageManager             clLanguageManager;
        SettingManager              clSettingManager;
        Dictionary<String, String>  clTransactionSetting;
        
        public CompositeFormInterface SCI_ParentForm { get; set; }

        public SubControlPOSPaymentPanel()
        {            
            clSettingManager = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;
            clLanguageManager = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;

            clTransactionSetting = General.JSONDeserialize<Dictionary<String, String>>(clSettingManager.GetSettingValue(ctSETTransactionSetting));        
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctSubControlPOSPaymentPanelStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctSubControlPOSPaymentPanelScript));
        }        

        private void RenderRow(ComponentController paComponentController, String paClass, String paColumnName, String paLabel)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, paClass);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, paColumnName);
            paComponentController.AddElementType(ComponentController.ElementType.Row);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.Label);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(paLabel);
            paComponentController.RenderEndTag();

            paComponentController.AddElementType(ComponentController.ElementType.Figure);
            paComponentController.RenderBeginTag(HtmlTag.Div);            
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderTitleBar(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTitleBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(clLanguageManager.GetText(ctDYTPaymentTitle));
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDClosePopUp);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.remove));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderFigureContainer(ComponentController paComponentController)
        {
            String[]      lcPaymentTypeArray;

            lcPaymentTypeArray = General.SplitString(clTransactionSetting.GetData(ctKEYPaymentOption, ctDEFPaymentType), true);
                        
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPaymentConatiner);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            for (int lcCount = 0; lcCount < lcPaymentTypeArray.Length; lcCount++)
            {
                RenderRow(paComponentController, ctCLSPaymentRow, lcPaymentTypeArray[lcCount].ToLower(), clLanguageManager.GetText(ctDYTPaymentType.Replace("$PAYMENTTYPE", lcPaymentTypeArray[lcCount])));
            }

            paComponentController.RenderEndTag();
        }

        private void RenderContentContainer(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSContentConatiner);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderRow(paComponentController, ctCLSTotalRow, ctCOLTotalAmount, clLanguageManager.GetText(ctDYTTotalAmount));

            RenderFigureContainer(paComponentController);

            RenderRow(paComponentController, ctCLSTotalPaymentRow, ctCOLTotalPayment, clLanguageManager.GetText(ctDYTTotalPayment));
            RenderRow(paComponentController, ctCLSChangeRow, ctCOLChange, clLanguageManager.GetText(ctDYTChange));

            paComponentController.RenderEndTag();            
        }

        private void RenderKeyPad(ComponentController paComponentController)
        {
            SubControlPOSKeyPad     lcKeyPad;

            lcKeyPad = new SubControlPOSKeyPad(ctPaymentKeypad);

            lcKeyPad.RenderChildMode(paComponentController);
        }
        
        private void RenderBrowserMode(ComponentController paComponentController)
        {
            IncludeExternalLinkFiles(paComponentController);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSubControlPOSPaymentPanelPopUp);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Language, clLanguageManager.ActiveRow.Language.ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctCompositeType);
            paComponentController.AddElementType(ComponentController.ElementType.PopUp);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSubControlPOSPaymentPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderTitleBar(paComponentController);
            RenderContentContainer(paComponentController);
            RenderKeyPad(paComponentController);

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderDesignMode(ComponentController paComponentController)
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


