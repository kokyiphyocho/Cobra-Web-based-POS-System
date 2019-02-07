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
    public class WidControlPOSTransaction : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlPOSTransactionStyle  = "WidControlPOSTransaction.css";
        protected const String ctWidControlPOSTransactionScript = "WidControlPOSTransaction.js";
        
        const String ctCLSWidControlPOSTransaction      = "WidControlPOSTransaction";
        const String ctCLSTitleBar                      = "TitleBar";
        const String ctCLSHeaderBar                     = "HeaderBar";
        const String ctCLSReceiptNoDiv                  = "ReceiptNoDiv";
        const String ctCLSCustomerDiv                   = "CustomerDiv";
        const String ctCLSDateDiv                       = "DateDiv";
        const String ctCLSMasterBlock                   = "MasterBlock";

        const String ctCLSSideButton                    = "SideButton";
        
        //const String ctSETSystemReceiptActionLimitDays  = "POS.SystemReceiptActionLimitDays";
        //const String ctSETStaffReceiptAdjustLimitDays   = "POS.StaffReceiptAdjustLimitDays";
        //const String ctSETAllowStaffAdjustReceipt       = "POS.AllowStaffAdjustReceipt";
        //const String ctSETTaxApplicable                 = "POS.TaxApplicable";        
        //const String ctSETTaxPercent                    = "POS.TaxPercent";
        //const String ctSETAllowShortSell                = "POS.AllowShortSell";
                
        const String ctSETTransactionSetting            = "POS.TransactionSetting";
        const String ctSETStaffPermissionSetting        = "POS.StaffPermissionSetting";
        const String ctSETRegionalConfig                = "_REGIONALCONFIG";
        const String ctSETSytemConfig                   = "_SYSTEMCONFIG";


        //const String ctKEYAllowShortSell                = "allowshortsell";
        //const String ctKEYTaxPercent                    = "taxpercent";
        //const String ctKEYTaxApplicable                 = "taxapplicable";
        //const String ctKEYAllowAdjustReceipt            = "allowadjustreceipt";
        //const String ctKEYReceiptAdjustLimitDays        = "receiptadjustlimitdays";
        //const String ctKEYReceiptActionLimitDays        = "receiptactionlimitdays";
        //const String ctKEYMultiPaymentMode = "MultiPaymentMode";
        //const String ctKEYReceiptPrintMode = "ReceiptPrintMode";

        const String ctKEYShowPaymentForm               = "showpaymentform";
        const String ctKEYReceiptPrintOption            = "receiptprintoption";

        //{"receiptprintoption":"sale,stockout","showpaymentform":"true","paymentoption":"paymentcash,paymentcontra","taxapplicable":"false","taxpercent":"20"}


        const String ctTIDCustomerInfo                  = "customerinfo";
        const String ctTIDReceiptDateInfo               = "receiptdateinfo";
        const String ctTIDPaymentInfo                   = "paymentinfo";

        const String ctTXTPurchaseTitleText             = "@@POS.Transaction.PurchaseTitle";
        const String ctTXTSaleTitleText                 = "@@POS.Transaction.SaleTitle";
        const String ctTXTStockInTitleText              = "@@POS.Transaction.StockInTitle";
        const String ctTXTStockOutTitleText             = "@@POS.Transaction.StockOutTitle";

        const String ctCMDFormClose                     = "@cmd%formclose";
        const String ctCMDCustomerInfo                  = "@cmd%customerinfo";
        const String ctCMDChangeReceiptDate             = "@cmd%changereceiptdate";
        const String ctCMDPrinterStatus                 = "@cmd%printerstatus";

        

        const String ctCOLCodeNo                        = "codeno";
        const String ctCOLName                          = "name";
        const String ctCOLReceiptDate                   = "receiptdate";

        const String ctIIGPOSTransaction                = "POSTransaction";
        const String ctIIGCustomerInfo                  = "POSPopUpCustomerInfo";
        const String ctIIGReceiptDateInfo               = "POSPopUpReceiptDateInfo";
        const String ctIIGPaymentInfo                   = "POSPopUpPaymentInfo";

        const String ctFPMReceiptType                   = "FPM_ReceiptType";
        const String ctFPMFormTitle                     = "FPM_FormTitle";
        const String ctFPMTransactionState              = "FPM_TransactionState";
        const String ctFPMReference                     = "FPM_Reference";

        const String ctCTLPaymentTextBox                = "PAYMENTTEXTBOX";

        const String ctCTNMasterBlock                   = "masterblock";
        const String ctCTNExternalComponent             = "externalcomponent";

        const String ctICOPrinter                       = "printericon.png";

        const String ctAJAXSquares                      = "AJAX_Squares.gif";
        
        const String ctStandardKeypadName               = "POSKeypad";
        
        public enum TransactionState    { Normal, Pending, Settlement };
        
        private LanguageManager         clLanguageManager;
        private SettingManager          clSettingManager;
        
        private POSReceiptManager.ReceiptType       clMode;
        private bool                                clAdminUser;
        private POSReceiptManager                   clReceiptManager;
        private Edition                             clEdition;

        
        //private bool                                clTaxApplicable;        
        //private Decimal                             clTaxPercent;
        private bool                                clReceiptPrintMode;
        private bool                                clShowPaymentForm;

        private Dictionary<String, String>          clTransactionSetting;
        private TransactionState                    clTransactionState;        
        private String                              clFormTitle;

        public CompositeFormInterface SCI_ParentForm { get; set; }
        
        public WidControlPOSTransaction()
        {
            clReceiptManager            = null;
            clMode                      = General.ParseEnum<POSReceiptManager.ReceiptType>(ApplicationFrame.GetInstance().ActiveFormInfoManager.GetFormParam(ctFPMReceiptType), POSReceiptManager.ReceiptType.Sale);
            
            clEdition                   = ApplicationFrame.GetInstance().ActiveSubscription.GetEdition();

            clLanguageManager           = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSettingManager            = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;
            clAdminUser                 = ApplicationFrame.GetInstance().ActiveSessionController.User.IsAdminUser();

            clTransactionSetting        = General.JSONDeserialize<Dictionary<String, String>>(clSettingManager.GetSettingValue(ctSETTransactionSetting));
            clReceiptPrintMode          = clTransactionSetting.GetData(ctKEYReceiptPrintOption, String.Empty).Contains(clMode.ToString().ToLower());
            clShowPaymentForm           = General.ParseBoolean(clTransactionSetting.GetData(ctKEYShowPaymentForm), false);
            //clTaxApplicable             = General.ParseBoolean(clTransactionSetting.GetData(ctKEYTaxApplicable), false);            
            //clTaxPercent                = General.ParseDecimal(clTransactionSetting.GetData(ctKEYTaxApplicable),0);
            //clMultiPaymentMode          = clTransactionOption.GetData(ctKEYMultiPaymentMode, String.Empty).Contains(clMode.ToString().ToLower());
            //clReceiptPrintMode          = clTransactionOption.GetData(ctKEYReceiptPrintMode, String.Empty).Contains(clMode.ToString().ToLower());
            
            clTransactionState          = General.ParseEnum<TransactionState>(ApplicationFrame.GetInstance().ActiveFormInfoManager.GetFormParam(ctFPMTransactionState), TransactionState.Normal);
            clFormTitle                 = ApplicationFrame.GetInstance().ActiveFormInfoManager.GetFormParam(ctFPMFormTitle, String.Empty);
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSTransactionStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSTransactionScript));
        }

        private String GetTitleBarText()
        {
            if (String.IsNullOrEmpty(clFormTitle.Trim()))
            {
                switch (clMode)
                {
                    case POSReceiptManager.ReceiptType.Purchase: return (clLanguageManager.GetText(ctTXTPurchaseTitleText));
                    case POSReceiptManager.ReceiptType.Sale: return (clLanguageManager.GetText(ctTXTSaleTitleText));
                    case POSReceiptManager.ReceiptType.StockIn: return (clLanguageManager.GetText(ctTXTStockInTitleText));
                    case POSReceiptManager.ReceiptType.StockOut: return (clLanguageManager.GetText(ctTXTStockOutTitleText));
                }
            }

            return (clFormTitle);
        }

        private void RenderTitleBar(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Title);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTitleBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            if (clReceiptPrintMode)
            {
                //paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDPrinterStatus);
                //paComponentController.AddElementType(ComponentController.ElementType.StatusControl);
                //paComponentController.RenderBeginTag(HtmlTag.Div);                
                //paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOPrinter));
                //paComponentController.RenderBeginTag(HtmlTag.Img);
                //paComponentController.RenderEndTag();
                //paComponentController.RenderEndTag();

                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDPrinterStatus);
                paComponentController.AddElementType(ComponentController.ElementType.StatusControl);
                paComponentController.RenderBeginTag(HtmlTag.Div);

                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "icon");
                paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOPrinter));
                paComponentController.RenderBeginTag(HtmlTag.Img);
                paComponentController.RenderEndTag();

                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "ajax");
                paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetAjaxLoaderImageUrl(ctAJAXSquares));
                paComponentController.RenderBeginTag(HtmlTag.Img);
                paComponentController.RenderEndTag();

                paComponentController.RenderEndTag();
            }

            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(GetTitleBarText());
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDFormClose);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(ComponentController.UnicodeStr((int) Fontawesome.remove));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderHeaderBar(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.ControlBar);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSHeaderBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSReceiptNoDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage.ConvertNumber(clReceiptManager.ActiveRow.ReceiptNo.ToString().PadLeft(6,'0')));
            paComponentController.RenderEndTag();


            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDCustomerInfo);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCustomerDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctCOLCodeNo);
            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(clReceiptManager.ActiveStakeHolder.ActiveRow.CodeNo);
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctCOLName);
            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(clReceiptManager.ActiveStakeHolder.ActiveRow.Name);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDChangeReceiptDate);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSDateDiv);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctCOLReceiptDate);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage.ConvertNumber(clReceiptManager.ActiveRow.ReceiptDate.ToString("dd/MM/yyyy")));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderItemPanel(ComponentController paComponentController, String paRenderMode = "")
        {
            SubControlPOSItemPanel lcSubControlPOSItemPanel;

            if (clMode == POSReceiptManager.ReceiptType.Sale) lcSubControlPOSItemPanel = new SubControlPOSItemPanel(SubControlPOSItemPanel.PriceMode.PriceShow, false);
            else lcSubControlPOSItemPanel = new SubControlPOSItemPanel(SubControlPOSItemPanel.PriceMode.PriceHide, true);

            lcSubControlPOSItemPanel.RenderChildMode(paComponentController, paRenderMode);
        }

        private void RenderTransactionList(ComponentController paComponentController, String paRenderMode = null)
        {
            SubControlPOSTransactionList    lcSubControlPOSTransactionList;

            lcSubControlPOSTransactionList = new SubControlPOSTransactionList(clReceiptManager);

            lcSubControlPOSTransactionList.RenderChildMode(paComponentController, paRenderMode);
        }

        private void RenderKeyPad(ComponentController paComponentController)
        {
            SubControlPOSKeyPad         lcSubControlPOSKeyPad;

            lcSubControlPOSKeyPad = new SubControlPOSKeyPad(ctStandardKeypadName);

            lcSubControlPOSKeyPad.RenderChildMode(paComponentController);
        }

        private void RenderPopUp(ComponentController paComponentController, String paPopUPID, String paInfoInfoGroup)
        {
            SubControlPopUpInput lcSubControlPopUpInput;

            lcSubControlPopUpInput = new SubControlPopUpInput(paPopUPID, paInfoInfoGroup, null, true);
            lcSubControlPopUpInput.CustomComponentRenderer += CustomComponentRenderer;

            lcSubControlPopUpInput.RenderChildMode(paComponentController);
        }

        private void RenderPaymentPopUp(ComponentController paComponentController)
        {
            SubControlPOSPaymentPanel       lcSubControlPOSPaymentPanel;

            lcSubControlPOSPaymentPanel = new SubControlPOSPaymentPanel();
            lcSubControlPOSPaymentPanel.RenderChildMode(paComponentController);
        }

        private void RenderReceiptOutput(ComponentController paComponentController)
        {
            SubControlPOSReceiptOutput lcSubControlPOSReceiptOutput;

            lcSubControlPOSReceiptOutput = new SubControlPOSReceiptOutput();
            lcSubControlPOSReceiptOutput.RenderChildMode(paComponentController);
        }

        private void RenderCalendarPopUp(ComponentController paComponentController)
        {
            SubControlCalendar lcSubControlCalendar;

            lcSubControlCalendar = new SubControlCalendar();

            lcSubControlCalendar.RenderChildMode(paComponentController);
        }     

        private void CustomComponentRenderer(ComponentController paComponentController, InputInfoRow paInputInfoRow, string paActiveValue)
        {
            if (paInputInfoRow.ControlType == ctCTLPaymentTextBox)
            {
                if (paInputInfoRow.MaxLimit > 0) paComponentController.AddAttribute(HtmlAttribute.Maxlength, paInputInfoRow.MaxLimit.ToString());
                paComponentController.AddAttribute(HtmlAttribute.Style, paInputInfoRow.InputCss);
                paComponentController.AddAttribute(HtmlAttribute.Name, paInputInfoRow.InputName);
                paComponentController.AddAttribute(HtmlAttribute.Type, "text");
                paComponentController.RenderBeginTag(HtmlTag.Input);
                paComponentController.RenderEndTag();

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSideButton);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.circle));
                paComponentController.RenderEndTag();
            }
        }


        //private int GetLowerBoundDays()
        //{            
        //    int      lcDays;

        //    if (clAdminUser)
        //        lcDays = General.ParseInt(clSettingManager.GetSettingValue(ctSETSystemReceiptActionLimitDays), 0);
        //    else
        //    {
        //        if (General.ParseBoolean(clSettingManager.GetSettingValue(ctSETAllowStaffAdjustReceipt), false))
        //            lcDays = General.ParseInt(clSettingManager.GetSettingValue(ctSETStaffReceiptAdjustLimitDays), 0);
        //        else return (0);
                
        //    }

        //    return (lcDays);
        //}

        //private int GetUpperBoundDays()
        //{
        //    return (0);
        //}        

        protected void RenderFields(ComponentController paComponentController)
        {
            InputInfoManager    lcInputInfoManager;
            MetaDataRow         lcMetaDataRow;

            lcInputInfoManager  = new InputInfoManager(ctIIGPOSTransaction);
            lcMetaDataRow       = new MetaDataRow(clReceiptManager.ActiveRow.Row);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSMasterBlock);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctCTNMasterBlock);
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            lcInputInfoManager.RenderAllSubGroups(paComponentController,  lcMetaDataRow);

            paComponentController.RenderEndTag();
        }

        protected void CreateReceiptManager()
        {
            DataRow     lcDataRow;

            if ((lcDataRow = ApplicationFrame.GetInstance().ActiveFormInfoManager.RunRetrieveRow()) != null)
                clReceiptManager = POSReceiptManager.CreateInstance(new POSReceiptRow(lcDataRow));
            else
            {
                clReceiptManager = POSReceiptManager.CreateInstance(clMode);
                clReceiptManager.ActiveRow.Reference = ApplicationFrame.GetInstance().ActiveFormInfoManager.GetFormParam(ctFPMReference);
            }
        }

        protected void RenderExternalComponents(ComponentController paComponentController)
        {
            RenderItemPanel(paComponentController);
            RenderPopUp(paComponentController, ctTIDCustomerInfo, ctIIGCustomerInfo);
            RenderCalendarPopUp(paComponentController);
            
            if (clShowPaymentForm) RenderPaymentPopUp(paComponentController);
            if (clReceiptPrintMode) RenderReceiptOutput(paComponentController);
        }

        protected void RenderExternalComponentsContainer(ComponentController paComponentController)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Mode, clMode.ToString().ToLower());
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctCTNExternalComponent);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.RenderEndTag();
        }

        protected void RenderBrowserMode(ComponentController paComponentController)
        {
            String      lcBase64TransactionSettingStr;
            String      lcBase64SystemConfigStr;
            String      lcBase64StaffPermissionStr;
            String      lcBase64RegionalConfigStr;

            lcBase64SystemConfigStr         = General.Base64Encode(clSettingManager.SystemConfigStr);
            lcBase64RegionalConfigStr       = General.Base64Encode(clSettingManager.RegionalConfigStr);
            lcBase64StaffPermissionStr      = General.Base64Encode(clSettingManager.GetSettingValue(ctSETStaffPermissionSetting,"{}"));
            lcBase64TransactionSettingStr   = General.Base64Encode(clSettingManager.GetSettingValue(ctSETTransactionSetting, "{}"));

            IncludeExternalLinkFiles(paComponentController);
            CreateReceiptManager();                                    

            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddBareAttribute(ctSETSytemConfig, lcBase64SystemConfigStr);
            paComponentController.AddBareAttribute(ctSETRegionalConfig, lcBase64RegionalConfigStr);            
            paComponentController.AddBareAttribute(ctSETTransactionSetting, lcBase64TransactionSettingStr);
            paComponentController.AddBareAttribute(ctSETStaffPermissionSetting, lcBase64StaffPermissionStr);
            
            // paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_TaxApplicable, clTaxApplicable.ToString().ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_TransactionState, clTransactionState.ToString().ToLower());
            // paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_MultiPaymentMode, clMultiPaymentMode.ToString().ToLower());
            // paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_ReceiptPrintMode, clReceiptPrintMode.ToString().ToLower());

            // paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_AllowShortSell, General.ParseBoolean(clSettingManager.SystemConfig.GetData(ctKEYAllowShortSell), false).ToString().ToLower());
            // paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_TaxInclusive, clReceiptManager.ActiveRow.TaxInclusive.ToString().ToLower());
            // paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_TaxPercent, clReceiptManager.ActiveRow.TaxPercent.ToString());
            // paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_LowerBound, GetLowerBoundDays().ToString());
            // paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_UpperBound, GetUpperBoundDays().ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_AdminMode, clAdminUser ? "true" : null);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Mode, clMode.ToString().ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DataID, clReceiptManager.ActiveRow.ReceiptID.ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Edition, clEdition.ToString().ToLower());
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSTransaction);
            paComponentController.RenderBeginTag(HtmlTag.Div);            
            RenderTitleBar(paComponentController);
            RenderHeaderBar(paComponentController);
            RenderFields(paComponentController);

            RenderTransactionList(paComponentController);

            RenderKeyPad(paComponentController);      
      
            paComponentController.RenderEndTag();

            RenderExternalComponentsContainer(paComponentController);


            // RenderPopUp(paComponentController, ctTIDReceiptDateInfo, ctIIGReceiptDateInfo);
            // RenderPopUp(paComponentController, ctTIDPaymentInfo, ctIIGPaymentInfo);            
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
            else if (paRenderMode == "cancelitemblock") RenderItemPanel(paComponentController, paRenderMode);
            else if (paRenderMode == "transactionlistcontent")
            {
                CreateReceiptManager();
                RenderTransactionList(paComponentController, paRenderMode);
            }
        }

        protected override void Render(HtmlTextWriter paHtmlTextWriter)
        {
            if (!DesignMode) RenderBrowserMode(new ComponentController(paHtmlTextWriter));
            else RenderDesignMode(new ComponentController(paHtmlTextWriter));
        }
    }
}
