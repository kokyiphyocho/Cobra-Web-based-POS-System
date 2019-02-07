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
    public class WidControlPOSDailyStockBalance : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlPOSDailyStockBalanceStyle    = "WidControlPOSDailyStockBalance.css";
        protected const String ctWidControlPOSDailyStockBalanceScript   = "WidControlPOSDailyStockBalance.js";

        const String ctCLSWidControlPOSDailyStockBalance    = "WidControlPOSDailyStockBalance";
        const String ctCLSHeaderBar                     = "HeaderBar";
        const String ctCLSContainer                     = "Container";
        const String ctCLSCategoryBlock                 = "CategoryBlock";
                
        const String ctCLSCategoryTitle                 = "CategoryTitle";        
        const String ctCLSTitleText                     = "TitleText";
        const String ctCLSHomeButtonDiv                 = "HomeButtonDiv";
        const String ctCLSUpButtonDiv                   = "UpButtonDiv";

        const String ctCLSItemListBlock                 = "ItemListBlock";        
        const String ctCLSItemRow                       = "ItemRow";
        const String ctCLSItemName                      = "ItemName";
        const String ctCLSTotalValue                    = "TotalValue";
        const String ctCLSQuantity                      = "Quantity";

        const String ctCLSSummaryBar                    = "SummaryBar";
        const String ctCLSSummaryText                   = "SummaryText";
        
        const String ctCMDRootCategory                  = "@cmd%rootcategory";
        const String ctCMDUpCategory                    = "@cmd%upcategory";
        const String ctCMDShowCategory                  = "@cmd%showcategory";
        
        const String ctCOLEntryType                     = "EntryType";
        const String ctCOLCategory                      = "Category";

        const String ctCMDPrevDay                       = "@cmd%prevday";
        const String ctCMDNextDay                       = "@cmd%nextday";        
        const String ctCMDShowCalendar                  = "@cmd%showcalendar";

        //const String ctSETSystemReceiptActionLimitDays  = "POS.SystemReceiptActionLimitDays";
        //const String ctSETStaffReportViewLimitDays      = "POS.StaffReportViewLimitDays";

        const String ctSETStaffPermissionSetting        = "POS.StaffPermissionSetting";
        
        const String ctKEYReportViewLimitDays           = "reportviewlimitdays";
        const String ctKEYReceiptActionLimitDays        = "receiptactionlimitdays";
                
        const String ctDYTRootCategoryName              = "@@POS.ItemList.RootCategoryName";
        const String ctDYTSummaryText                   = "@@POS.DailyStockBalance.SummaryText";
        const String ctDYTOutOfStock                    = "@@POS.DailyStockBalance.OutOfStock";

        public CompositeFormInterface SCI_ParentForm       { get; set; }
        
       //  Edition     clEdition;
        private DataTable                       clItemList;
        private LanguageManager                 clLanguageManager;
        private SettingManager                  clSettingManager;
        private Dictionary<String, String>      clStaffPermissionSetting;
        private bool                            clAdminUser;

        
        public WidControlPOSDailyStockBalance()
        {
            clItemList                  = null;
            clLanguageManager           = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSettingManager            = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;
            clAdminUser                 = ApplicationFrame.GetInstance().ActiveSessionController.User.IsAdminUser();
            clStaffPermissionSetting    = General.JSONDeserialize<Dictionary<String, String>>(clSettingManager.GetSettingValue(ctSETStaffPermissionSetting));
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSDailyStockBalanceStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSDailyStockBalanceScript));
        }                

        private void RenderItemRow(ComponentController paComponentController, POSStockBalanceRow paItemRow)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItemRow);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Status, (paItemRow.Quantity > 0 ? "ex-stock" : "nostock"));            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, paItemRow.EntryType.ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DataID, paItemRow.ItemID.ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDShowCategory);
            paComponentController.AddElementType(ComponentController.ElementType.Row);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItemName);
            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(paItemRow.ItemName);            
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSQuantity);
            paComponentController.RenderBeginTag(HtmlTag.Span);

            if (paItemRow.Quantity <= 0) paComponentController.Write(clLanguageManager.GetText(ctDYTOutOfStock));
            else paComponentController.Write(clLanguageManager.ConvertNumber(paItemRow.QuantityText));

            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTotalValue);
            paComponentController.AddAttribute(HtmlAttribute.Value, paItemRow.TotalCost.ToString());
            paComponentController.AddElementType(ComponentController.ElementType.Figure);            
            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(clLanguageManager.ConvertNumber(paItemRow.TotalCost.ToString(clSettingManager.CurrencyFormatString)));
            paComponentController.RenderEndTag();
            
            paComponentController.RenderEndTag();
        }

        private void RenderItemList(ComponentController paComponentController, POSStockBalanceRow paCategoryRow)
        {
            POSStockBalanceRow lcItemCatalogueRow;
            DataRow[] lcItemList;

            lcItemList = GetItemList(paCategoryRow == null ? 0 : paCategoryRow.ItemID);

            paComponentController.AddElementType(ComponentController.ElementType.List);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItemListBlock);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            
            lcItemCatalogueRow = new POSStockBalanceRow(null);

            for (int lcCount = 0; lcCount < lcItemList.Length; lcCount++)
            {
                lcItemCatalogueRow.Row = lcItemList[lcCount];
                RenderItemRow(paComponentController, lcItemCatalogueRow);
            }
            

            paComponentController.RenderEndTag();
        }
      
        private void RenderCategoryTitle(ComponentController paComponentController, POSStockBalanceRow paCategoryRow)
        {            
            if (paCategoryRow != null)
            {
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCategoryTitle);
                paComponentController.AddElementType(ComponentController.ElementType.Title);                                
                paComponentController.RenderBeginTag(HtmlTag.Div);

                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDRootCategory);                
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSHomeButtonDiv);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.home));
                paComponentController.RenderEndTag();

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTitleText);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(paCategoryRow.ItemName);
                paComponentController.RenderEndTag();

                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDUpCategory);                
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSUpButtonDiv);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.level_up));
                paComponentController.RenderEndTag();
                
                paComponentController.RenderEndTag();                
            }
            else
            {
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCategoryTitle);
                paComponentController.AddElementType(ComponentController.ElementType.Title);                            
                paComponentController.RenderBeginTag(HtmlTag.Div);

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTitleText);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(clLanguageManager.GetText(ctDYTRootCategoryName));
                paComponentController.RenderEndTag();
                
                paComponentController.RenderEndTag();
            }
        }

        private void RenderCategoryBlock(ComponentController paComponentController, POSStockBalanceRow paCategoryRow)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Block);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCategoryBlock);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Group, paCategoryRow != null ? paCategoryRow.ItemID.ToString() : "0");
            
            if (paCategoryRow != null)
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Parent, paCategoryRow.Category.ToString());

            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderCategoryTitle(paComponentController, paCategoryRow);
            RenderItemList(paComponentController, paCategoryRow);
            RenderCategorySummaryBlock(paComponentController);

            paComponentController.RenderEndTag();            
        }

        private void RenderCategorySummaryBlock(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSummaryBar);
            paComponentController.AddElementType(ComponentController.ElementType.Summary);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSummaryText);
            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(clLanguageManager.GetText(ctDYTSummaryText));
            paComponentController.RenderEndTag();

            paComponentController.AddElementType(ComponentController.ElementType.Total);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTotalValue);            
            paComponentController.RenderBeginTag(HtmlTag.Span);            
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private DataRow[] GetCategoryList()        
        {
            return(clItemList.AsEnumerable().Where(r => r.Field<String>(ctCOLEntryType) == "CATEGORY").ToArray());
        }

        private DataRow[] GetItemList(int paCategory)
        {
            return (clItemList.AsEnumerable().Where(r => r.Field<int>(ctCOLCategory) == paCategory).OrderBy(r => r.Field<String>(ctCOLEntryType)).ToArray());
        }

        private void RenderContainerContent(ComponentController paComponentController)
        {
            DataRow[]           lcCategoryRows;
            POSStockBalanceRow lcItemCatalogueRow;

            lcCategoryRows = GetCategoryList();

            RenderCategoryBlock(paComponentController, null);

            lcItemCatalogueRow = new POSStockBalanceRow(null);

            for (int lcCount = 0; lcCount < lcCategoryRows.Length; lcCount++)
            {
                lcItemCatalogueRow.Row = lcCategoryRows[lcCount];
                RenderCategoryBlock(paComponentController, lcItemCatalogueRow);
            }
        }

        private void RenderContainer(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSContainer);
            paComponentController.RenderBeginTag(HtmlTag.Div);

        //    RenderContainerContent(paComponentController);
                
            paComponentController.RenderEndTag(); // Container
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

        protected void RenderCalendar(ComponentController paComponentController)
        {
            SubControlCalendar lcCanlendar;

            lcCanlendar = new SubControlCalendar();

            lcCanlendar.RenderChildMode(paComponentController);
        }

        private int GetLowerBoundDays()
        {
            int lcDays;

            if (clAdminUser)
                lcDays = General.ParseInt(clSettingManager.SystemConfig.GetData(ctKEYReceiptActionLimitDays), 0);
            else
                lcDays = General.ParseInt(clStaffPermissionSetting.GetData(ctKEYReportViewLimitDays), 0);

            return (lcDays);
        }

        private int GetUpperBoundDays()
        {
            return (0);
        }

        protected void RenderBrowserMode(ComponentController paComponentController)
        {
            IncludeExternalLinkFiles(paComponentController);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_LowerBound, GetLowerBoundDays().ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_UpperBound, GetUpperBoundDays().ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Decimal, clSettingManager.CurrencyDecimalPlace.ToString());
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSDailyStockBalance);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            clItemList = ApplicationFrame.GetInstance().ActiveFormInfoManager.RunRetrieveQuery();

            RenderHeaderBar(paComponentController);
            RenderContainer(paComponentController);                        

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
            if (paRenderMode == "stockbalancelist")
            {                
                clItemList = ApplicationFrame.GetInstance().ActiveFormInfoManager.RunRetrieveQuery();
                RenderContainerContent(paComponentController);
            }
        }

        protected override void Render(HtmlTextWriter paHtmlTextWriter)
        {
            if (!DesignMode) RenderBrowserMode(new ComponentController(paHtmlTextWriter));
            else RenderDesignMode(new ComponentController(paHtmlTextWriter));
        }
    }
}



