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
    public class WidControlPOSDailyReceipt : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlPOSDailyReceiptStyle = "WidControlPOSDailyReceipt.css";
        protected const String ctWidControlPOSDailyReceiptScript = "WidControlPOSDailyReceipt.js";

        const String ctCLSWidControlPOSDailyReceipt     = "WidControlPOSDailyReceipt";
        const String ctCLSTitleBar                      = "TitleBar";
        const String ctCLSHeaderBar                     = "HeaderBar";
        const String ctCLSSummaryContainer              = "SummaryContainer";
        const String ctCLSSummaryBlock                  = "SummaryBlock";
        const String ctCLSSummaryBlockTitle             = "SummaryBlockTitle";
        const String ctCLSButtonPanel                   = "ButtonPanel";

        //const String ctSETSystemReceiptActionLimitDays  = "POS.SystemReceiptActionLimitDays";
        //const String ctSETStaffReportViewLimitDays      = "POS.StaffReportViewLimitDays";
        //const String ctSETSystemProfitLossView          = "POS.SystemProfitLossView";        
        //const String ctSETAllowStaffProfitLossView      = "POS.AllowStaffProfitLossView";  

        const String ctSETStaffPermissionSetting            = "POS.StaffPermissionSetting";
      
        const String ctKEYAllowProfitLossView               = "allowprofitlossview";
        const String ctKEYReportViewLimitDays               = "reportviewlimitdays";
        const String ctKEYReceiptActionLimitDays            = "receiptactionlimitdays";
        
        const String ctCMDFormClose                         = "@cmd%formclose";
        const String ctCMDPrevDay                           = "@cmd%prevday";
        const String ctCMDNextDay                           = "@cmd%nextday";        
        const String ctCMDShowCalendar                      = "@cmd%showcalendar";
        const String ctCMDOpenPopUp                         = "@cmd%openpopup";
        const String ctCMDClosePopUp                        = "@cmd%closepopup";
        
        const String ctICOOpenPopUp                         = "openpopup.png";
        const String ctICOOpenPopUpGray                     = "openpopup_gray.png";
        
        const String ctTIDSale                              = "sale";
        const String ctTIDShortSell                         = "shortsell";
        const String ctTIDPurchase                          = "purchase";
        const String ctTIDStockIn                           = "stockin";
        const String ctTIDStockOut                          = "stockout";
        const String ctTIDCancel                            = "cancel";
                                 

        public enum SummaryValue { SaleCount, SaleAmount, SaleCost, SaleProfit, ShortSellCount, PurchaseCount, PurchaseAmount, 
                                   StockInCount, StockInValue, StockOutCount, StockOutValue,
                                   CancelCount, CancelSaleCount, CancelPurchaseCount, CancelStockInCount, CancelStockOutCount, 
                                   ReceiptAmount, ReceiptCost, ReceiptProfit, ItemAmount, ItemCost, ItemProfit }

        const String ctFPMDate                          = "FPM_DATE";
        const String ctDateFormat                       = "yyyy-MM-dd";

        const String ctCOLTotalAmount                   = "TotalAmount";
        const String ctCOLTotalCost                     = "TotalCost";
        const String ctCOLReceiptID                     = "ReceiptID";
        
        const String ctFLTSales                         = "ReceiptType = 'SALE' And Status = 'ACTIVE'";
        const String ctFLTShortSell                     = "ReceiptType = 'SALE' And Status = 'ACTIVE' And RelativeBalance + Quantity < 0 AND ENTRYATTRIBUTE <> 'STATIC'";
        const String ctFLTPurchase                      = "ReceiptType = 'PURCHASE' And Status = 'ACTIVE'";
        const String ctFLTStockIn                       = "ReceiptType = 'STOCKIN' And Status = 'ACTIVE'";
        const String ctFLTStockOut                      = "ReceiptType = 'STOCKOUT' And Status = 'ACTIVE'";

        const String ctFLTCancelReceipts                = "Status = 'CANCEL'";
        const String ctFLTCancelSales                   = "ReceiptType = 'SALE' And Status = 'CANCEL'";
        const String ctFLTCancelPurchase                = "ReceiptType = 'PURCHASE' And Status = 'CANCEL'";
        const String ctFLTCancelStockIn                 = "ReceiptType = 'STOCKIN' And Status = 'CANCEL'";
        const String ctFLTCancelStockOut                = "ReceiptType = 'STOCKOUT' And Status = 'CANCEL'";
        
        const String ctDYTSaleSummaryTitle              = "@@POS.DailyReceipt.SaleSummaryTitle";
        const String ctDYTShortSellSummaryTitle         = "@@POS.DailyReceipt.ShortSellSummaryTitle";
        const String ctDYTPurchaseSummaryTitle          = "@@POS.DailyReceipt.PurchaseSummaryTitle";
        const String ctDYTStockInSummaryTitle           = "@@POS.DailyReceipt.StockInSummaryTitle";
        const String ctDYTStockOutSummaryTitle          = "@@POS.DailyReceipt.StockOutSummaryTitle";
        const String ctDYTCancelSummaryTitle            = "@@POS.DailyReceipt.CancelSummaryTitle";

        const String ctDYTSaleDetailTitle               = "@@POS.DailyReceipt.SaleDetailTitle";
        const String ctDYTShortSellDetailTitle          = "@@POS.DailyReceipt.ShortSellDetailTitle";
        const String ctDYTPurchaseDetailTitle           = "@@POS.DailyReceipt.PurchaseDetailTitle";
        const String ctDYTStockInDetailTitle            = "@@POS.DailyReceipt.StockInDetailTitle";
        const String ctDYTStockOutDetailTitle           = "@@POS.DailyReceipt.StockOutDetailTitle";
        const String ctDYTCancelDetailTitle             = "@@POS.DailyReceipt.CancelDetailTitle";

        const String ctDYTTitle                         = "@@POS.DailyReceipt.Title";
                
        object[,] clDSISalesSummaryProfitLoss = new object[,] {
                                                                    {"@@POS.DailyReceipt.TotalSaleCount",   SummaryValue.SaleCount, "count",    false},
                                                                    {"@@POS.DailyReceipt.TotalSaleAmount",  SummaryValue.SaleAmount, "amount",  true},
                                                                    {"@@POS.DailyReceipt.TotalSaleCost",    SummaryValue.SaleCost, "cost",      true},
                                                                    {"@@POS.DailyReceipt.TotalSaleProfit",  SummaryValue.SaleProfit, "profit",  true},
                                                              };

        object[,] clDSISalesSummary = new object[,] {
                                                        {"@@POS.DailyReceipt.TotalSaleCount",   SummaryValue.SaleCount, "count",    false},
                                                        {"@@POS.DailyReceipt.TotalSaleAmount",  SummaryValue.SaleAmount, "amount",  true},                                                        
                                                    };

        object[,] clDSIShortSellSummary = new object[,] {
                                                           {"@@POS.DailyReceipt.TotalShortSellCount",   SummaryValue.ShortSellCount, "count",    false},                                                                
                                                        };

        object[,] clDSIPurchaseSummary = new object[,] {
                                                          {"@@POS.DailyReceipt.TotalPurchaseCount", SummaryValue.PurchaseCount, "count",   false},                                                              
                                                          {"@@POS.DailyReceipt.TotalPurchaseCost",  SummaryValue.PurchaseAmount, "amount", true},
                                                       };

        object[,] clDSIStockInSummary = new object[,] {
                                                          {"@@POS.DailyReceipt.TotalStockInCount",  SummaryValue.StockInCount, "count",  false},                                                              
                                                          {"@@POS.DailyReceipt.TotalStockInAmount", SummaryValue.StockInValue, "amount", true},
                                                      };

        object[,] clDSIStockOutSummary = new object[,] {
                                                          {"@@POS.DailyReceipt.TotalStockOutCount", SummaryValue.StockOutCount, "count",   false},                                                              
                                                          {"@@POS.DailyReceipt.TotalStockOutAmount", SummaryValue.StockOutValue, "amount", true},
                                                       };

        object[,] clDSICancelSummary = new object[,]   {
                                                            {"@@POS.DailyReceipt.CancelSale", SummaryValue.CancelSaleCount, "count",         false},                                                              
                                                            {"@@POS.DailyReceipt.CancelPurchase", SummaryValue.CancelPurchaseCount, "count", false},
                                                            {"@@POS.DailyReceipt.CancelStockIn", SummaryValue.CancelStockInCount, "count",   false},
                                                            {"@@POS.DailyReceipt.CancelStockOut", SummaryValue.CancelStockOutCount, "count", false},
                                                       };

        object[,] clDSICancelSummaryCashRegister = new object[,]   {
                                                                        {"@@POS.DailyReceipt.CancelSale", SummaryValue.CancelSaleCount, "count",         false},                                                                                                                                   
                                                                   };
        
        public CompositeFormInterface SCI_ParentForm { get; set; }



        private LanguageManager             clLanguageManager;
        private SettingManager              clSettingManager;        
        private Dictionary<String,String>   clStaffPermissionSetting;
        private DateTime                    clDate;
                       
        private DataTable clReceiptTable;
        private DataTable clSaleReceiptTable;
        private DataTable clShortSellTable;
        private DataTable clPurchaseReceiptTable;
        private DataTable clStockInReceiptTable;
        private DataTable clStockOutReceiptTable;
        private DataTable clCancelReceiptTable;
        
        

        private bool      clAllowProfitLossView;
        
        private bool      clAdminUser;       
        

        public WidControlPOSDailyReceipt()
        { 
            clLanguageManager           = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSettingManager            = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;
            clAdminUser                 = ApplicationFrame.GetInstance().ActiveSessionController.User.IsAdminUser();
            clDate                      = General.ParseDate(ApplicationFrame.GetInstance().ActiveFormInfoManager.GetFormParam(ctFPMDate), ctDateFormat, new DateTime());
            clStaffPermissionSetting    = General.JSONDeserialize<Dictionary<String,String>>(clSettingManager.GetSettingValue(ctSETStaffPermissionSetting));

            clAllowProfitLossView       = General.ParseBoolean(clStaffPermissionSetting.GetData(ctKEYAllowProfitLossView), false) || clAdminUser;            
        }
        
        private int GetIntegerSummary(SummaryValue paSummaryValue)
        {
            switch(paSummaryValue)
            {
                case SummaryValue.SaleCount                 : return (clSaleReceiptTable.AsEnumerable().Select(row=>row.Field<int>(ctCOLReceiptID)).Distinct().Count());
                case SummaryValue.ShortSellCount            : return (clShortSellTable.Rows.Count);
                case SummaryValue.PurchaseCount             : return (clPurchaseReceiptTable.AsEnumerable().Select(row => row.Field<int>(ctCOLReceiptID)).Distinct().Count());
                case SummaryValue.StockInCount              : return (clStockInReceiptTable.AsEnumerable().Select(row => row.Field<int>(ctCOLReceiptID)).Distinct().Count());
                case SummaryValue.StockOutCount             : return (clStockOutReceiptTable.AsEnumerable().Select(row => row.Field<int>(ctCOLReceiptID)).Distinct().Count());
                case SummaryValue.CancelCount               : return (clCancelReceiptTable.AsEnumerable().Select(row => row.Field<int>(ctCOLReceiptID)).Distinct().Count());
                case SummaryValue.CancelSaleCount           : return (General.CountTableRow(clCancelReceiptTable, ctCOLReceiptID, ctFLTCancelSales));
                case SummaryValue.CancelPurchaseCount       : return (General.CountTableRow(clCancelReceiptTable, ctCOLReceiptID, ctFLTCancelPurchase));
                case SummaryValue.CancelStockInCount        : return (General.CountTableRow(clCancelReceiptTable, ctCOLReceiptID, ctFLTCancelStockIn));
                case SummaryValue.CancelStockOutCount       : return (General.CountTableRow(clCancelReceiptTable, ctCOLReceiptID, ctFLTCancelStockOut));
                default: return (0);
            }
        }

        private decimal GetDecimalSummary(SummaryValue paSummaryValue)
        {
            switch (paSummaryValue)
            {
                case SummaryValue.SaleAmount                : return (General.SumDecimal(clSaleReceiptTable,ctCOLTotalAmount));
                case SummaryValue.SaleCost                  : return (General.SumDecimal(clSaleReceiptTable, ctCOLTotalCost));
                case SummaryValue.SaleProfit                : return (GetDecimalSummary(SummaryValue.SaleAmount) + GetDecimalSummary(SummaryValue.SaleCost));                
                case SummaryValue.PurchaseAmount            : return (General.SumDecimal(clPurchaseReceiptTable, ctCOLTotalAmount));
                default: return (0);
            }
        }

        private decimal GetDecimalSummary(SummaryValue paSummaryValue, DataTable paDataTable, int paReceiptNo, int paEntryID = 0)
        {
            switch (paSummaryValue)
            {
                case SummaryValue.ReceiptAmount          : return (General.SumDecimal(paDataTable,ctCOLTotalAmount));
                case SummaryValue.ReceiptCost            : return (General.SumDecimal(paDataTable, ctCOLTotalCost));
                case SummaryValue.ReceiptProfit          : return (GetDecimalSummary(SummaryValue.ReceiptAmount, paDataTable, paReceiptNo, paEntryID) + GetDecimalSummary(SummaryValue.ReceiptCost, paDataTable, paReceiptNo, paEntryID));
                case SummaryValue.ItemAmount             : return (General.SumDecimal(paDataTable,ctCOLTotalAmount));
                case SummaryValue.ItemCost               : return (General.SumDecimal(paDataTable, ctCOLTotalCost));
                case SummaryValue.ItemProfit             : return (GetDecimalSummary(SummaryValue.ItemAmount, paDataTable, paReceiptNo, paEntryID) + GetDecimalSummary(SummaryValue.ItemCost, paDataTable, paReceiptNo, paEntryID));
                default: return (0);
            }
        }        

        private void RetrieveData()
        {
            clReceiptTable = ApplicationFrame.GetInstance().ActiveFormInfoManager.RunRetrieveQuery();
                    
            clSaleReceiptTable = (new DataView(clReceiptTable,ctFLTSales,ctCOLReceiptID,DataViewRowState.CurrentRows)).ToTable();
            clShortSellTable = (new DataView(clReceiptTable, ctFLTShortSell, ctCOLReceiptID, DataViewRowState.CurrentRows)).ToTable();
            clPurchaseReceiptTable = (new DataView(clReceiptTable,ctFLTPurchase,ctCOLReceiptID,DataViewRowState.CurrentRows)).ToTable();
            clStockInReceiptTable = (new DataView(clReceiptTable,ctFLTStockIn,ctCOLReceiptID,DataViewRowState.CurrentRows)).ToTable();
            clStockOutReceiptTable = (new DataView(clReceiptTable,ctFLTStockOut,ctCOLReceiptID,DataViewRowState.CurrentRows)).ToTable();
            clCancelReceiptTable = (new DataView(clReceiptTable,ctFLTCancelReceipts,ctCOLReceiptID,DataViewRowState.CurrentRows)).ToTable();            
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSDailyReceiptStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSDailyReceiptScript));
        }

        private void RenderTitleBar(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Title);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTitleBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);
        
            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(clLanguageManager.GetText(ctDYTTitle));
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

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDPrevDay);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.caret_left));
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDShowCalendar);
            paComponentController.AddElementType(ComponentController.ElementType.DateBox);
            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(General.GetCurrentSystemLocalTime().ToString(clSettingManager.DateFormatString));
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDNextDay);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.caret_right));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private int GetLowerBoundDays()
        {
            int lcDays;

            if (clAdminUser)
                lcDays = General.ParseInt(clSettingManager.SystemConfig.GetData(ctKEYReceiptActionLimitDays), 0);
            else
                lcDays = General.ParseInt(clStaffPermissionSetting.GetData(ctKEYReportViewLimitDays),0);

            return (lcDays);
        }

        private int GetUpperBoundDays()
        {
            return (0);
        }

        private void RenderButtonPanel(ComponentController paComponentController, String paCommand, Fontawesome paButtonText)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, paCommand);
            paComponentController.RenderBeginTag(HtmlTag.A);

            paComponentController.Write(ComponentController.UnicodeStr((int)paButtonText));

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }  

        protected void RenderSummaryBlockHeader(ComponentController paComponentController, String paBlockTitle)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSummaryBlockTitle);
            paComponentController.AddElementType(ComponentController.ElementType.Title);            
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(clLanguageManager.GetText(paBlockTitle));
            paComponentController.RenderEndTag();

            RenderButtonPanel(paComponentController, ctCMDOpenPopUp, Fontawesome.external_link);

            paComponentController.RenderEndTag();
        }

        protected void RenderSummaryInfo(ComponentController paComponentController, String paInfoLabel, SummaryValue paSummaryValue, String paType, bool paDecimal)
        {
            dynamic lcValue;

            if (paDecimal)
                lcValue = GetDecimalSummary(paSummaryValue);
            else
                lcValue = GetIntegerSummary(paSummaryValue);

            if (lcValue < 0) paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Appearance, "negative");

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, paType);
            paComponentController.AddElementType(ComponentController.ElementType.Row);            
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.Label);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(paInfoLabel));            
            paComponentController.RenderEndTag();

            paComponentController.AddElementType(ComponentController.ElementType.Figure);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            if (paDecimal)
                paComponentController.Write(clLanguageManager.ConvertNumber(lcValue.ToString(clSettingManager.CurrencyFormatString)));
            else
                paComponentController.Write(clLanguageManager.ConvertNumber(lcValue.ToString()));
                        
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        protected void RenderSummaryBlock(ComponentController paComponentController, String paBlockTitle, object[,] paInfoBlockRow, String paBlockType, String paPopUpTitle, DataTable paDataTable)
        {
            if (paDataTable.Rows.Count > 0) paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_AdditionalData, "true");

            paComponentController.AddElementType(ComponentController.ElementType.Block);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, paBlockType);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSummaryBlock);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderSummaryBlockHeader(paComponentController, paBlockTitle);

            for (int lcCount = 0; lcCount < paInfoBlockRow.GetLength(0); lcCount++)
                RenderSummaryInfo(paComponentController, Convert.ToString(paInfoBlockRow[lcCount, 0]), 
                                  (SummaryValue)paInfoBlockRow[lcCount, 1],
                                  Convert.ToString(paInfoBlockRow[lcCount, 2]), 
                                  Convert.ToBoolean(paInfoBlockRow[lcCount, 3]));


            RenderDetailPopUp(paComponentController, clDate, paPopUpTitle, paBlockType, paDataTable);
            
            paComponentController.RenderEndTag();
        }

        protected void RenderDailyReceiptContainerContent(ComponentController paComponentController)
        {
            if (clAllowProfitLossView)
                RenderSummaryBlock(paComponentController, ctDYTSaleSummaryTitle, clDSISalesSummaryProfitLoss, ctTIDSale, ctDYTSaleDetailTitle, clSaleReceiptTable);
            else
                RenderSummaryBlock(paComponentController, ctDYTSaleSummaryTitle, clDSISalesSummary, ctTIDSale, ctDYTSaleDetailTitle, clSaleReceiptTable);

            if ((clShortSellTable.Rows.Count > 0) && (clSettingManager.Edition != SettingManager.EditionType.Cash_Register))
                RenderSummaryBlock(paComponentController, ctDYTShortSellSummaryTitle, clDSIShortSellSummary, ctTIDShortSell, ctDYTShortSellDetailTitle, clShortSellTable);

            if ((clSettingManager.Edition != SettingManager.EditionType.Cash_Register))
            {
                RenderSummaryBlock(paComponentController, ctDYTPurchaseSummaryTitle, clDSIPurchaseSummary, ctTIDPurchase, ctDYTPurchaseDetailTitle, clPurchaseReceiptTable);
                RenderSummaryBlock(paComponentController, ctDYTStockInSummaryTitle, clDSIStockInSummary, ctTIDStockIn, ctDYTStockInDetailTitle, clStockInReceiptTable);
                RenderSummaryBlock(paComponentController, ctDYTStockOutSummaryTitle, clDSIStockOutSummary, ctTIDStockOut, ctDYTStockOutDetailTitle, clStockOutReceiptTable);
                RenderSummaryBlock(paComponentController, ctDYTCancelSummaryTitle, clDSICancelSummary, ctTIDCancel, ctDYTCancelDetailTitle, clCancelReceiptTable);
            }
            else RenderSummaryBlock(paComponentController, ctDYTCancelSummaryTitle, clDSICancelSummaryCashRegister, ctTIDCancel, ctDYTCancelDetailTitle, clCancelReceiptTable);
        }

        protected void RenderDailyReceiptContainer(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSummaryContainer);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            
            

            paComponentController.RenderEndTag();
        }        

        protected void RenderCalendar(ComponentController paComponentController)
        {
            SubControlCalendar lcCanlendar;

            lcCanlendar = new SubControlCalendar();

            lcCanlendar.RenderChildMode(paComponentController);
        }

        protected void RenderDetailPopUp(ComponentController paComponentController, DateTime paDate, String paTitle, String paTypeID, DataTable paDataTable)
        {
            SubControlPOSPopUpReceiptDetail lcPopUpReceiptDetail;

            lcPopUpReceiptDetail = new SubControlPOSPopUpReceiptDetail(paDate, paTitle, paTypeID, paDataTable, clAllowProfitLossView);

            lcPopUpReceiptDetail.RenderChildMode(paComponentController);
        }

        protected void RenderBrowserMode(ComponentController paComponentController)
        {
            IncludeExternalLinkFiles(paComponentController);
           
            paComponentController.AddElementType(ComponentController.ElementType.Control);            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_LowerBound, GetLowerBoundDays().ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_UpperBound, GetUpperBoundDays().ToString());

            if (clAllowProfitLossView)
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Mode, "profitloss");

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSDailyReceipt);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderTitleBar(paComponentController);
            RenderHeaderBar(paComponentController);
            RenderDailyReceiptContainer(paComponentController);

            paComponentController.RenderEndTag();
            
            RenderCalendar(paComponentController);         
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
            else if (paRenderMode == "DailyReceiptinfo")
            {                
                RetrieveData();
                RenderDailyReceiptContainerContent(paComponentController);                
            }            
        }

        protected override void Render(HtmlTextWriter paHtmlTextWriter)
        {
            if (!DesignMode) RenderBrowserMode(new ComponentController(paHtmlTextWriter));
            else RenderDesignMode(new ComponentController(paHtmlTextWriter));
        }
    }
}
