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
    public class WidControlPOSMonthlyTransaction: WebControl, WidgetControlInterface
    {
        protected const String ctWidControlPOSMonthlyTransactionStyle  = "WidControlPOSMonthlyTransaction.css";
        protected const String ctWidControlPOSMonthlyTransactionScript = "WidControlPOSMonthlyTransaction.js";
        
        const String ctCLSWidControlPOSMonthlyTransaction   = "WidControlPOSMonthlyTransaction";
        const String ctCLSContainer                         = "Container";

        const String ctCLSMonthBar                          = "MonthBar";     
        const String ctCLSPopUpOverlay                      = "PopUpOverlay";
        const String ctCLSPopUpTitle                        = "PopUpTitle";
        const String ctCLSMonthListContainer                = "MonthListContainer";
        const String ctCLSButtonPanel                       = "ButtonPanel";
        const String ctCLSHeaderRow                         = "HeaderRow";
        const String ctCLSSummaryRow                        = "SummaryRow";
        const String ctCLSRow                               = "Row";
        
        const String ctDYTMonthList                         = "@@*.MonthList";
        const String ctDYTPopUpTitle                        = "@@POS.MonthlyTransaction.MonthListPopUpTitle";
        const String ctDYTRowHeaderDate                     = "@@POS.MonthlyTransaction.RowHeader.Date";
        const String ctDYTRowHeaderTotalAmount              = "@@POS.MonthlyTransaction.RowHeader.TotalAmount";
        const String ctDYTRowHeaderTotalCost                = "@@POS.MonthlyTransaction.RowHeader.TotalCost";
        const String ctDYTRowheaderProfit                   = "@@POS.MonthlyTransaction.RowHeader.Profit";
        const String ctDYTSummaryTotalText                  = "@@POS.MonthlyTransaction.SummaryTotalText";

        //const String ctSETReportViewLimit                   = "POS.SystemReportViewLimitMonths";
        //const String ctSETSystemProfitLossView              = "POS.SystemProfitLossView";
        //const String ctSETAllowStaffProfitLossView          = "POS.AllowStaffProfitLossView"; 

        const String ctSETStaffPermissionSetting            = "POS.StaffPermissionSetting";

        const String ctKEYAllowProfitLossView               = "allowprofitlossview";
        const String ctKEYReportViewLimitMonths             = "reportviewlimitmonths";        
                
        const String ctDYTChooseButtonText                  = "@@POS.Button.Choose";
        const String ctDYTCancelButtonText                  = "@@POS.Button.Cancel";

        const String ctFPMStartDate                         = "FPM_STARTDATE";
        const String ctDateFormat                           = "yyyy-MM-dd";

        const String ctCMDShowPopUp                         = "@cmd%showpopup";
        const String ctCMDMonthSelect                       = "@cmd%monthselect";
        const String ctCMDPopUpChoose                       = "@cmd%popup.choose";
        const String ctCMDPopUpCancel                       = "@cmd%popup.cancel";
        const String ctCMDPopUpClose                        = "@cmd%popup.close";

        const String ctSeparator                            = ",";
        const int    ctDefaultReportViewLimit               = 3;

        const String ctCOLReceiptDate                       = "ReceiptDate";
        const String ctCOLTotalCost                         = "TotalCost";
        const String ctCOLTotalAmount                       = "TotalAmount";
        
        public CompositeFormInterface SCI_ParentForm { get; set; }
                
        private LanguageManager             clLanguageManager;
        private SettingManager              clSettingManager;
        private String                      clMonthListStr;
        private String[]                    clMonthListArray;
        private DateTime                    clDate;
        private int                         clReportViewLimit;             
        private bool                        clAllowProfitLossView;
        private bool                        clAdminUser;
        private Dictionary<String, String>  clStaffPermissionSetting;

        public WidControlPOSMonthlyTransaction()
        {
            clLanguageManager           = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSettingManager            = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;
            clStaffPermissionSetting    = General.JSONDeserialize<Dictionary<String, String>>(clSettingManager.GetSettingValue(ctSETStaffPermissionSetting));

            clDate                      = General.ParseDate(ApplicationFrame.GetInstance().ActiveFormInfoManager.GetFormParam(ctFPMStartDate), ctDateFormat, DateTime.Now);
            clMonthListStr              = clLanguageManager.GetText(ctDYTMonthList);
            clMonthListArray            = clMonthListStr.Split(new String[] { ctSeparator }, StringSplitOptions.None);

            clReportViewLimit           = General.ParseInt(clSettingManager.SystemConfig.GetData(ctKEYReportViewLimitMonths), ctDefaultReportViewLimit);
            clAdminUser                 = ApplicationFrame.GetInstance().ActiveSessionController.User.IsAdminUser();

            clAllowProfitLossView       = General.ParseBoolean(clStaffPermissionSetting.GetData(ctKEYAllowProfitLossView), false) || clAdminUser;                        
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager   = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSMonthlyTransactionStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSMonthlyTransactionScript));            
        }      
       
        private String GetMonthText(DateTime paDate, bool paYearFirst)
        {
            if (clMonthListArray.Length >= paDate.Month)
            {
                if (paYearFirst)
                    return (clLanguageManager.ConvertNumber(paDate.Year.ToString()) + ctSeparator + " " + clMonthListArray[paDate.Month - 1]);
                else
                    return (clMonthListArray[paDate.Month - 1] + ctSeparator + " " + clLanguageManager.ConvertNumber(paDate.Year.ToString()));
            }                                
            else return (String.Empty);
        }

        private void RenderHeaderRow(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSHeaderRow);
            paComponentController.AddElementType(ComponentController.ElementType.Row);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "date");
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(ctDYTRowHeaderDate));
            paComponentController.RenderEndTag();


            if (clAllowProfitLossView)
            {
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "profit");
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(clLanguageManager.GetText(ctDYTRowheaderProfit));
                paComponentController.RenderEndTag();

                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "cost");
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(clLanguageManager.GetText(ctDYTRowHeaderTotalCost));
                paComponentController.RenderEndTag();
            }

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "amount");
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(ctDYTRowHeaderTotalAmount));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderSummaryRow(ComponentController paComponentController, Decimal paGrandTotalAmount, Decimal paGrandTotalCost)
        {          
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSummaryRow);
            paComponentController.AddElementType(ComponentController.ElementType.Row);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "total");
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(ctDYTSummaryTotalText));
            paComponentController.RenderEndTag();

            if (clAllowProfitLossView)
            {
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "profit");
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(clLanguageManager.ConvertNumber((paGrandTotalAmount + paGrandTotalCost).ToString(clSettingManager.CurrencyFormatString)));
                paComponentController.RenderEndTag();

                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "cost");
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(clLanguageManager.ConvertNumber(paGrandTotalCost.ToString(clSettingManager.CurrencyFormatString)));
                paComponentController.RenderEndTag();
            }

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "amount");
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.ConvertNumber(paGrandTotalAmount.ToString(clSettingManager.CurrencyFormatString)));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderEntryRow(ComponentController paComponentController, DateTime paDate, Decimal paTotalAmount, Decimal paTotalCost)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSRow);            
            paComponentController.AddElementType(ComponentController.ElementType.Row);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "date");
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.ConvertNumber(paDate.ToString(clSettingManager.DateFormatString)));
            paComponentController.RenderEndTag();
            
            if (clAllowProfitLossView)
            {
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "profit");
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(clLanguageManager.ConvertNumber((paTotalAmount + paTotalCost).ToString(clSettingManager.CurrencyFormatString)));
                paComponentController.RenderEndTag();

                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "cost");
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(clLanguageManager.ConvertNumber(paTotalCost.ToString(clSettingManager.CurrencyFormatString)));
                paComponentController.RenderEndTag();
            }

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "amount");
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.ConvertNumber(paTotalAmount.ToString(clSettingManager.CurrencyFormatString)));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }


        private void RenderContainerContent(ComponentController paComponentController)
        {
            DateTime    lcDate;
            Decimal     lcTotalAmount;
            Decimal     lcTotalCost;
            Decimal     lcGrandTotalAmount;
            Decimal     lcGrandTotalCost;
            DataTable   lcMonthlyTransaction;

            lcMonthlyTransaction = ApplicationFrame.GetInstance().ActiveFormInfoManager.RunRetrieveQuery();

            RenderHeaderRow(paComponentController);

            paComponentController.AddElementType(ComponentController.ElementType.List);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            for (int lcCount = 0; lcCount < lcMonthlyTransaction.Rows.Count; lcCount++)
            {
                lcDate = Convert.ToDateTime(lcMonthlyTransaction.Rows[lcCount][ctCOLReceiptDate]);
                lcTotalAmount = Convert.ToDecimal(lcMonthlyTransaction.Rows[lcCount][ctCOLTotalAmount]);
                lcTotalCost = Convert.ToDecimal(lcMonthlyTransaction.Rows[lcCount][ctCOLTotalCost]);
                RenderEntryRow(paComponentController, lcDate, lcTotalAmount, lcTotalCost);
            }

            paComponentController.RenderEndTag();

            lcGrandTotalAmount = General.SumDecimal(lcMonthlyTransaction, ctCOLTotalAmount);
            lcGrandTotalCost = General.SumDecimal(lcMonthlyTransaction, ctCOLTotalCost);
            RenderSummaryRow(paComponentController, lcGrandTotalAmount, lcGrandTotalCost);
        }

        private void RenderContainer(ComponentController paComponentController)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "content");
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSContainer);
            paComponentController.RenderBeginTag(HtmlTag.Div);


            paComponentController.RenderEndTag();
        }

        private void RenderMonthBar(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.DateBox);
            paComponentController.AddAttribute(HtmlAttribute.Value, clDate.ToString("yyyy-MM-") + "01");
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDShowPopUp);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSMonthBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.Write(GetMonthText(clDate, false));

            paComponentController.RenderEndTag();
        }

        private void RenderMonthListPopUpTitle(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPopUpTitle);
            paComponentController.AddElementType(ComponentController.ElementType.Title);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(clLanguageManager.GetText(ctDYTPopUpTitle));
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDPopUpClose);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.remove));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderMonthListContainer(ComponentController paComponentController)
        {
            DateTime    lcDate;

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSMonthListContainer);
            paComponentController.AddElementType(ComponentController.ElementType.List);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            lcDate = General.GetCurrentSystemLocalTime();

            for ( int lcCount = 0; lcCount < clReportViewLimit; lcCount++ )
            {
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDMonthSelect);
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Text, GetMonthText(lcDate.AddMonths(-lcCount), false));
                paComponentController.AddAttribute(HtmlAttribute.Value, lcDate.AddMonths(-lcCount).ToString("yyyy-MM-") + "01");
                paComponentController.AddElementType(ComponentController.ElementType.Item);
                paComponentController.RenderBeginTag(HtmlTag.A);
                paComponentController.Write(GetMonthText(lcDate.AddMonths(-lcCount), true));
                paComponentController.RenderEndTag();
            }
            
            paComponentController.RenderEndTag();
        }

        private void RenderMonthListPopUpButtonPanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDPopUpChoose);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(clLanguageManager.GetText(ctDYTChooseButtonText));
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDPopUpCancel);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(clLanguageManager.GetText(ctDYTCancelButtonText));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderMonthListPopUp(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Overlay);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPopUpOverlay);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "monthlist");
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.PopUp);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderMonthListPopUpTitle(paComponentController);
            RenderMonthListContainer(paComponentController);
            RenderMonthListPopUpButtonPanel(paComponentController);

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        protected void RenderBrowserMode(ComponentController paComponentController)
        {
            IncludeExternalLinkFiles(paComponentController);
            
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSMonthlyTransaction);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderMonthBar(paComponentController);
            RenderContainer(paComponentController);                        
            
            paComponentController.RenderEndTag();

            RenderMonthListPopUp(paComponentController);
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
            if (paRenderMode == "monthlytransactioncontent")
            {
                RenderContainerContent(paComponentController);
            }
            else
            {
                RenderBrowserMode(paComponentController);
            }
        }

        protected override void Render(HtmlTextWriter paHtmlTextWriter)
        {
            if (!DesignMode) RenderBrowserMode(new ComponentController(paHtmlTextWriter));
            else RenderDesignMode(new ComponentController(paHtmlTextWriter));
        }
    }
}

