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
    public class WidControlPOSReceiptList : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlPOSReceiptListStyle = "WidControlPOSReceiptList.css";
        protected const String ctWidControlPOSReceiptListScript = "WidControlPOSReceiptList.js";
        
        const String ctCLSWidControlPOSReceiptList      = "WidControlPOSReceiptList";
        const String ctCLSTitleBar                      = "TitleBar";
        const String ctCLSHeaderBar                     = "HeaderBar";
        const String ctCLSReceiptList                   = "ReceiptList";
        const String ctCLSElement                       = "Element";
        const String ctCLSButtonPanel                   = "ButtonPanel";
        const String ctCLSSummary                       = "Summary";
        const String ctCLSNoReceiptDiv                  = "NoReceiptDiv";

        const String ctTIDSearchReceiptInfo             = "searchreceiptinfo";

        const String ctIIGSearchReceiptInfo             = "POSPopUpSearchReceiptInfo";

        //const String ctSETAllowStaffAdjustReceipt       = "POS.AllowStaffAdjustReceipt";
        //const String ctSETAllowStaffCancelReceipt       = "POS.AllowStaffCancelReceipt";  

        //const String ctSETSystemReceiptActionLimitDays  = "POS.SystemReceiptActionLimitDays";
        //const String ctSETStaffReceiptAdjustLimitDays   = "POS.StaffReceiptAdjustLimitDays";
        //const String ctSETStaffReceiptCancelLimitDays   = "POS.StaffReceiptCancelLimitDays";        

        const String ctSETStaffPermissionSetting          = "POS.StaffPermissionSetting";

        const String ctKEYAllowAdjustReceipt              = "allowadjustreceipt";
        const String ctKEYAllowCancelReceipt              = "allowcancelreceipt";
        const String ctKEYReceiptAdjustLimitDays          = "receiptadjustlimitdays";
        const String ctKEYReceiptCancelLimitDays          = "receiptcancellimitdays";
        const String ctKEYReceiptActionLimitDays          = "receiptactionlimitdays";
        
        const String ctCMDFormClose                       = "@cmd%formclose";
        const String ctCMDPrevDay                         = "@cmd%prevday";
        const String ctCMDNextDay                         = "@cmd%nextday";
        const String ctCMDSearch                          = "@cmd%search";
        const String ctCMDShowCalendar                    = "@cmd%showcalendar";
        const String ctCMDSelectReceipt                   = "@cmd%selectreceipt";
        const String ctCMDEditReceipt                     = "@cmd%editreceipt";
        const String ctCMDDeleteReceipt                   = "@cmd%deletereceipt";
        const String ctCMDReprintReceipt                  = "@cmd%reprintreceipt";
        const String ctCMDPrinterStatus                   = "@cmd%printerstatus";

        const String ctTPLEditReceipt                       = "FormPOSTransaction,FPM_ReceiptID::$RECEIPTID;;FPM_ReceiptType::$RECEIPTTYPE";

        const String ctCOLReceiptID                         = "receiptid";
        const String ctCOLReceiptNo                         = "receiptno";
        const String ctCOLReceiptType                       = "receipttype";
        const String ctCOLReceiptDate                       = "receiptdate";
        const String ctCOLReceiptAmount                     = "receiptamount";
        const String ctCOLCodeNo                            = "codeno";
        const String ctCOLName                              = "name";
        
        const String ctICORecycleBin                        = "recycle_bin.png";
        const String ctICOEditPencil                        = "edit_pencil.png";
        const String ctICOReprint                           = "printericon.png";

        const String ctICOPrinter                           = "printericon.png";
        const String ctAJAXSquares                          = "AJAX_Squares.gif";

        const String ctMSGNoReceiptRecord                   = "status_noreceiptrecord";
        const String ctMSGSearchReceipt                     = "status_searchreceipt";

        const String ctCTNExternalComponent                 = "externalcomponent";

        const String ctDYTReceiptListTitle                  = "@@POS.ReceiptList.TitleText";
        const String ctDYTTotalQuantityText                 = "@@POS.Transaction.ReceiptListQuantityText";                
        
        const String ctFPMDate                              = "FPM_DATE";

        const String ctISODateFormat                        = "yyyy-MM-dd";

        const String ctReceiptNoPrefix                      = "#";
        const String ctDefaultType                          = "sale";
                                
        private LanguageManager         clLanguageManager;
        private SettingManager          clSettingManager;

        DataTable                       clReceiptList;        
        private bool                    clAdminUser;        
        private Edition                 clEdition;
        private int                     clStaffAdjustLimitDays;
        private int                     clStaffCancelLimitDays;
        private int                     clCurrentDayDelta;
        private bool                    clStaffAdjustable;
        private bool                    clStaffCancelable;
        Dictionary<String, String>      clStaffPermissionSetting;
        
        public CompositeFormInterface SCI_ParentForm { get; set; }

        public WidControlPOSReceiptList()
        {   
            DateTime  lcLocalDate;
            DateTime  lcReceiptDate;            

            clEdition                   = ApplicationFrame.GetInstance().ActiveSubscription.GetEdition();

            clLanguageManager           = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSettingManager            = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;
            clAdminUser                 = ApplicationFrame.GetInstance().ActiveSessionController.User.IsAdminUser();
            
            clStaffPermissionSetting = General.JSONDeserialize<Dictionary<String, String>>(clSettingManager.GetSettingValue(ctSETStaffPermissionSetting, "{}"));
            
            lcLocalDate                 = General.GetCurrentSystemLocalTime();
            lcReceiptDate               = General.ParseDate(ApplicationFrame.GetInstance().ActiveFormInfoManager.GetFormParam(ctFPMDate), ctISODateFormat, new DateTime());
            clCurrentDayDelta           = Convert.ToInt32((lcLocalDate.Date - lcReceiptDate.Date).TotalDays);

            clStaffAdjustLimitDays      = General.ParseInt(clStaffPermissionSetting.GetData(ctKEYReceiptAdjustLimitDays), 0);
            clStaffCancelLimitDays      = General.ParseInt(clStaffPermissionSetting.GetData(ctKEYReceiptCancelLimitDays), 0);
            
            clStaffAdjustable           = General.ParseBoolean(clStaffPermissionSetting.GetData(ctKEYAllowAdjustReceipt), false) && (clStaffAdjustLimitDays > clCurrentDayDelta);
            clStaffCancelable           = General.ParseBoolean(clStaffPermissionSetting.GetData(ctKEYAllowCancelReceipt), false) && (clStaffCancelLimitDays > clCurrentDayDelta);
        }
        
        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSReceiptListStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSReceiptListScript));
        }

        private void RenderTitleBar(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Title);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTitleBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);

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
            

            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(clLanguageManager.GetText(ctDYTReceiptListTitle));
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

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDSearch);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.search));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private int GetLowerBoundDays()
        {
            int lcDays;

            if (clAdminUser)
                lcDays = General.ParseInt(clSettingManager.SystemConfig.GetData(ctKEYReceiptActionLimitDays), 0);
            else lcDays = Math.Max(clStaffAdjustLimitDays, clStaffCancelLimitDays);

            return (lcDays);
        }

        private int GetUpperBoundDays()
        {
            return (0);
        }

        private void RenderButtonPanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            if (clAdminUser || clStaffCancelable)
            {
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDDeleteReceipt);
                paComponentController.RenderBeginTag(HtmlTag.A);

                paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICORecycleBin));
                paComponentController.RenderBeginTag(HtmlTag.Img);
                paComponentController.RenderEndTag();

                paComponentController.RenderEndTag();
            }

            if (clAdminUser || clStaffAdjustable)
            {
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDEditReceipt);
                paComponentController.RenderBeginTag(HtmlTag.A);

                paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOEditPencil));
                paComponentController.RenderBeginTag(HtmlTag.Img);
                paComponentController.RenderEndTag();

                paComponentController.RenderEndTag();
            }

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDReprintReceipt);
            paComponentController.RenderBeginTag(HtmlTag.A);

            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOReprint));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();


            paComponentController.RenderEndTag();
        }

        private void RenderReceiptEntry(ComponentController paComponentController, POSReceiptListRow paReceiptListRow)
        {            
            paComponentController.AddElementType(ComponentController.ElementType.Element);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDSelectReceipt);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DataID, paReceiptListRow.ReceiptID.ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, paReceiptListRow.ReceiptType.ToLower());
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSElement);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctCOLReceiptNo);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.ConvertNumber(ctReceiptNoPrefix + paReceiptListRow.ReceiptNo.ToString().PadLeft(6,'0')));
            paComponentController.RenderEndTag(); 

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctCOLReceiptDate);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.ConvertNumber(paReceiptListRow.ReceiptDate.ToString(clSettingManager.DateFormatString)));
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctCOLName);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(paReceiptListRow.Name);
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctCOLReceiptAmount);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_CurrencyCode, clSettingManager.CurrencyCode);
            paComponentController.AddAttribute(HtmlAttribute.Value, paReceiptListRow.ReceiptAmount.ToString(clSettingManager.BareCurrencyFormatString));
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.ConvertNumber(paReceiptListRow.ReceiptAmount.ToString(clSettingManager.CurrencyFormatString)));
            paComponentController.RenderEndTag();

            RenderButtonPanel(paComponentController);

            paComponentController.RenderEndTag();            
        }       
        
        protected void RenderReceiptListEntries(ComponentController paComponentController)
        {            
            POSReceiptListRow   lcReceiptListRow;

            if (clReceiptList != null)
            {                
                lcReceiptListRow    = new POSReceiptListRow(null);                

                for (int lcCount = 0; lcCount < clReceiptList.Rows.Count; lcCount++)
                {
                    lcReceiptListRow.Row = clReceiptList.Rows[lcCount];
                    
                    RenderReceiptEntry(paComponentController, lcReceiptListRow);                    
                }
            }
        }

        protected void RenderReceiptContainer(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.List);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSReceiptList);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            
            paComponentController.RenderEndTag();
        }        

        protected void RenderSummaryBar(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Summary);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSummary);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "quantity");
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.Quantity);
            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.RenderEndTag();
                        
            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(clLanguageManager.GetText(ctDYTTotalQuantityText));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "total");
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_CurrencyCode, clSettingManager.CurrencyCode);
            paComponentController.AddElementType(ComponentController.ElementType.Total);
            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        protected void RenderNoReceiptDiv(ComponentController paComponentController)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_MessageCode, ctMSGNoReceiptRecord);
            paComponentController.AddElementType(ComponentController.ElementType.MessageDisplay);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSNoReceiptDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.RenderEndTag();
        }        

        protected void RenderCalendar(ComponentController paComponentController)
        {
            SubControlCalendar lcCanlendar;

            lcCanlendar = new SubControlCalendar();

            lcCanlendar.RenderChildMode(paComponentController);
        }

        private void RenderSearchPopUp(ComponentController paComponentController)
        {
            SubControlPopUpInput lcSubControlPopUpInput;
                        
            lcSubControlPopUpInput = new SubControlPopUpInput(ctTIDSearchReceiptInfo, ctIIGSearchReceiptInfo, null, true);
            lcSubControlPopUpInput.SC_MessageCode = ctMSGSearchReceipt;

            lcSubControlPopUpInput.RenderChildMode(paComponentController);
        }

        private void RenderReceiptOutput(ComponentController paComponentController)
        {
            SubControlPOSReceiptOutput lcSubControlPOSReceiptOutput;

            lcSubControlPOSReceiptOutput = new SubControlPOSReceiptOutput();            
            lcSubControlPOSReceiptOutput.RenderChildMode(paComponentController);
        }

        protected void RenderExternalComponents(ComponentController paComponentController)
        {
            RenderCalendar(paComponentController);
            RenderSearchPopUp(paComponentController);
            RenderReceiptOutput(paComponentController);
        }

        protected void RenderExternalComponentsContainer(ComponentController paComponentController)
        {            
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctCTNExternalComponent);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.RenderEndTag();
        }

        protected void RenderBrowserMode(ComponentController paComponentController)
        {
            IncludeExternalLinkFiles(paComponentController);

            clReceiptList = ApplicationFrame.GetInstance().ActiveFormInfoManager.RunRetrieveQuery();
                                    
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Template, ctTPLEditReceipt);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_LowerBound, GetLowerBoundDays().ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_UpperBound, GetUpperBoundDays().ToString());            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Edition, clEdition.ToString().ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Filter, ctDefaultType);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSReceiptList);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderTitleBar(paComponentController);
            RenderHeaderBar(paComponentController);
            SCI_ParentForm.RenderToolBar(paComponentController);
            RenderReceiptContainer(paComponentController);
            RenderSummaryBar(paComponentController);
            RenderNoReceiptDiv(paComponentController);

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
            else if (paRenderMode == "receiptlistcontent")
            {
                clReceiptList = ApplicationFrame.GetInstance().ActiveFormInfoManager.RunRetrieveQuery();
                RenderReceiptListEntries(paComponentController);
            }
            else 
            {
                clReceiptList = DynamicQueryManager.GetInstance().GetDataTableResult(paRenderMode);
                RenderReceiptListEntries(paComponentController);
            }
        }

        protected override void Render(HtmlTextWriter paHtmlTextWriter)
        {
            if (!DesignMode) RenderBrowserMode(new ComponentController(paHtmlTextWriter));
            else RenderDesignMode(new ComponentController(paHtmlTextWriter));
        }
    }
}
