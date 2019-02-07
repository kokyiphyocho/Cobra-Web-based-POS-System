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
    public class WidControlPOSStaticCostEditor : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlPOSStaticCostEditorStyle = "WidControlPOSStaticCostEditor.css";
        protected const String ctWidControlPOSStaticCostEditorScript = "WidControlPOSStaticCostEditor.js";
        
        const String ctCLSWidControlPOSStaticCostEditor = "WidControlPOSStaticCostEditor";
        const String ctCLSContainer             = "Container";

        const String ctCLSCostListBlock         = "CostListBlock";
        const String ctCLSTitle                 = "Title";        

        const String ctCLSEntryList              = "EntryList";        
        const String ctCLSEntryRow               = "EntryRow";
        
        const String ctCLSButtonPanel           = "ButtonPanel";
        const String ctCLSEditButtonDiv         = "EditButtonDiv";
        const String ctCLSDeleteButtonDiv       = "DeleteButtonDiv";
        
        const String ctICOEditButton             = "edit_pencil.png";
        const String ctICODeleteButton           = "recycle_bin.png";
        const String ctICODisableDeleteButton    = "recycle_white.png";
        
        const String ctCMDEdit                  = "@cmd%edit";
        const String ctCMDDelete                = "@cmd%delete";
        const String ctCMDRowClick              = "@cmd%rowclick";

        const String ctDYTTitle                 = "@@POS.StaticCostEditor.Title";
        const String ctDYTInitialCost           = "@@POS.StaticCostEditor.InitialCost";

        //const String ctSETSystemReceiptActionLimitDays  = "POS.SystemReceiptActionLimitDays";
        //const String ctSETStaffReceiptAdjustLimitDays   = "POS.StaffReceiptAdjustLimitDays";
        //const String ctSETAllowStaffAdjustReceipt       = "POS.AllowStaffAdjustReceipt";

        const String ctSETStaffPermissionSetting        = "POS.StaffPermissionSetting";

        const String ctKEYAllowAdjustReceipt            = "allowadjustreceipt";
        const String ctKEYReceiptAdjustLimitDays        = "receiptadjustlimitdays";
        const String ctKEYReceiptActionLimitDays        = "receiptactionlimitdays";

        const String ctCTNExternalComponent     = "externalcomponent";

        const String ctQUYGetItemRecord         = "EPOS.GetItemRecord";

        const String ctTIDStaticCostInfo        = "staticcostinfo";

        const String ctIIGStaticCostInfo        = "POSPopUpStaticCostInfo";

        const String ctCOLIncomingID            = "IncomingID";
        const String ctCOLReceiptDate           = "ReceiptDate";
        const String ctCOLUnitPrice             = "UnitPrice";

        const String ctFPMItemID                = "FPM_ITEMID";

        public CompositeFormInterface SCI_ParentForm { get; set; }
        
        private DataTable               clCostPriceList;

        private bool                        clAdminUser;
        private LanguageManager             clLanguageManager;
        private SettingManager              clSettingManager;
        private Dictionary<String, String>  clStaffPermissionSetting;
        private DateTime                    clInitialCostDate;
        private POSItemCatalogueRow         clItemCatalogueRow;

        public WidControlPOSStaticCostEditor()
        {
            DataRow         lcDataRow;

            clCostPriceList             = null;
            clInitialCostDate           = new DateTime(1900,1,1);

            clLanguageManager           = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSettingManager            = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;
            clAdminUser                 = ApplicationFrame.GetInstance().ActiveSessionController.User.IsAdminUser();
            clStaffPermissionSetting    = General.JSONDeserialize<Dictionary<String, String>>(clSettingManager.GetSettingValue(ctSETStaffPermissionSetting));

            if ((lcDataRow = DynamicQueryManager.GetInstance().GetDataRowResult(ctQUYGetItemRecord)) != null)
            {
                clItemCatalogueRow = new POSItemCatalogueRow(lcDataRow);
            }
            else clItemCatalogueRow = null;
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSStaticCostEditorStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSStaticCostEditorScript));
        }        

        private void RenderButtonPanel(ComponentController paComponentController, bool paDisableDelete)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSEditButtonDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDEdit);            
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOEditButton));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSDeleteButtonDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDDelete);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(paDisableDelete ? ctICODisableDeleteButton : ctICODeleteButton));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderEntryRow(ComponentController paComponentController, DateTime paDate, int paIncomingID, Decimal paCost)
        {
            bool            lcInitialCost;

            lcInitialCost = paDate.Date == clInitialCostDate.Date;

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSEntryRow);
            paComponentController.AddAttribute(HtmlAttribute.Value, paCost.ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDRowClick);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DataID, paIncomingID.ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Group, paDate.ToString("yyyy-MM-dd"));
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Mode, lcInitialCost ? "initialcost" : "cost");
            
            paComponentController.AddElementType(ComponentController.ElementType.Row);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "date");
            paComponentController.RenderBeginTag(HtmlTag.Span);

            if (lcInitialCost)
                paComponentController.Write(clLanguageManager.GetText(ctDYTInitialCost));
            else
                paComponentController.Write(clLanguageManager.ConvertNumber(paDate.ToString(clSettingManager.DateFormatString)));

            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Value, paCost.ToString(clSettingManager.BareCurrencyFormatString));
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "unitprice");
            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(clLanguageManager.ConvertNumber(paCost.ToString(clSettingManager.CurrencyFormatString)));
            paComponentController.RenderEndTag();

            RenderButtonPanel(paComponentController, lcInitialCost);

            paComponentController.RenderEndTag();
        }

        private void RenderEntryList(ComponentController paComponentController)
        {
            DateTime        lcDate;
            int             lcIncomingID;
            Decimal         lcUnitPrice;            

            paComponentController.AddElementType(ComponentController.ElementType.List);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSEntryList);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            
            if ((clCostPriceList != null) && (clCostPriceList.Rows.Count > 0))
            {                
                for (int lcCount = 0; lcCount < clCostPriceList.Rows.Count; lcCount++)
                {
                    lcDate          = Convert.ToDateTime(clCostPriceList.Rows[lcCount][ctCOLReceiptDate]);
                    lcIncomingID    = Convert.ToInt32(clCostPriceList.Rows[lcCount][ctCOLIncomingID]);
                    lcUnitPrice     = Convert.ToDecimal(clCostPriceList.Rows[lcCount][ctCOLUnitPrice]);
                    
                    if ((lcCount == 0) && (lcDate.Date != clInitialCostDate.Date))
                    {
                        RenderEntryRow(paComponentController, clInitialCostDate, 0, 0);
                        RenderEntryRow(paComponentController, lcDate, lcIncomingID, lcUnitPrice);
                    }                    
                    else                    
                    {                        
                        RenderEntryRow(paComponentController, lcDate, lcIncomingID, lcUnitPrice);
                    }
                }
            }
            else RenderEntryRow(paComponentController, clInitialCostDate, 0, 0);

            paComponentController.RenderEndTag();
        }

        private void RenderCostListTitle(ComponentController paComponentController)
        {            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTitle);
            paComponentController.AddElementType(ComponentController.ElementType.Title);                            
            paComponentController.RenderBeginTag(HtmlTag.Div);
                        
            if (clItemCatalogueRow != null)
                paComponentController.Write(clLanguageManager.GetText(ctDYTTitle).Replace("$ITEMNAME",clItemCatalogueRow.ItemName));            
                
            paComponentController.RenderEndTag();            
        }


        private void RenderCostListBlock(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Block);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCostListBlock);            
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderCostListTitle(paComponentController);
            RenderEntryList(paComponentController);

            paComponentController.RenderEndTag();            
        }        

        private void RenderContainer(ComponentController paComponentController)
        {            
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSContainer);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderCostListBlock(paComponentController);

            paComponentController.RenderEndTag(); 
        }

        private void RenderPopUp(ComponentController paComponentController, String paPopUPID, String paInfoInfoGroup)
        {
            SubControlPopUpInput lcSubControlPopUpInput;

            lcSubControlPopUpInput = new SubControlPopUpInput(paPopUPID, paInfoInfoGroup, null, true);            

            lcSubControlPopUpInput.RenderChildMode(paComponentController);
        }

        private void RenderCalendarPopUp(ComponentController paComponentController)
        {
            SubControlCalendar lcSubControlCalendar;

            lcSubControlCalendar = new SubControlCalendar();

            lcSubControlCalendar.RenderChildMode(paComponentController);
        }     

        protected void RenderExternalComponents(ComponentController paComponentController)
        {            
            RenderPopUp(paComponentController, ctTIDStaticCostInfo, ctIIGStaticCostInfo);
            RenderCalendarPopUp(paComponentController);
        }
 
        private void RenderExtrernalComponentContainer(ComponentController paComponentController)
        {            
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctCTNExternalComponent);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.RenderEndTag();
        }

        private int GetLowerBoundDays()
        {
            int lcDays;

            if (clAdminUser)
                lcDays = General.ParseInt(clSettingManager.SystemConfig.GetData(ctKEYReceiptActionLimitDays), 0);
            else
            {
                if (General.ParseBoolean(clStaffPermissionSetting.GetData(ctKEYAllowAdjustReceipt), false))
                    lcDays = General.ParseInt(clStaffPermissionSetting.GetData(ctKEYReceiptAdjustLimitDays), 0);
                else return (0);

            }

            return (lcDays);
        }

        private int GetUpperBoundDays()
        {
            return (0);
        }        


        protected void RenderBrowserMode(ComponentController paComponentController)
        {
            IncludeExternalLinkFiles(paComponentController);

            clCostPriceList = ApplicationFrame.GetInstance().ActiveFormInfoManager.RunRetrieveQuery();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_LowerBound, GetLowerBoundDays().ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_UpperBound, GetUpperBoundDays().ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DataID, ApplicationFrame.GetInstance().ActiveFormInfoManager.GetFormParam(ctFPMItemID));
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSStaticCostEditor);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            
            RenderContainer(paComponentController);                        

            paComponentController.RenderEndTag();

            RenderExtrernalComponentContainer(paComponentController);
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
            else if (paRenderMode == "entrylist") 
            {
                clCostPriceList = ApplicationFrame.GetInstance().ActiveFormInfoManager.RunRetrieveQuery();
                RenderEntryList(paComponentController);
            }
            else if (paRenderMode == "externalcomponent") RenderExternalComponents(paComponentController);
        }

        protected override void Render(HtmlTextWriter paHtmlTextWriter)
        {
            if (!DesignMode) RenderBrowserMode(new ComponentController(paHtmlTextWriter));
            else RenderDesignMode(new ComponentController(paHtmlTextWriter));
        }
    }
}

