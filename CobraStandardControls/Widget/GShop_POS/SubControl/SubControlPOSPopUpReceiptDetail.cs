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
    public class SubControlPOSPopUpReceiptDetail : WebControl, WidgetControlInterface
    {
        const String ctSubControlPOSPopUpReceiptDetailStyle          = "SubControlPOSPopUpReceiptDetail.css";
        const String ctSubControlPOSPopUpReceiptDetailScript         = "SubControlPOSPopUpReceiptDetail.js";

        const String ctCLSSubControlPOSPopUpReceiptDetailComposite   = "SubControlPOSPopUpReceiptDetailComposite";
        const String ctCLSSubControlPOSPopUpReceiptDetail            = "SubControlPOSPopUpReceiptDetail";
        const String ctCLSTitle                                      = "Title";        
        const String ctCLSContainer                                  = "Container";

        const String ctCLSReceiptBlock                               = "ReceiptBlock";
        const String ctCLSBlockHeader                                = "BlockHeader";
        const String ctCLSReceiptNoDiv                               = "ReceiptNoDiv";
        const String ctCLSNameDiv                                    = "NameDiv";
        const String ctCLSItemContainer                              = "ItemContainer";
        const String ctCLSEntryRow                                   = "EntryRow";
        const String ctCLSDescription                                = "Description";
        const String ctCLSSellPrice                                  = "SellPrice";
        const String ctCLSCost                                       = "Cost";
        const String ctCLSProfit                                     = "Profit";
        const String ctClSBlockFooter                                = "BlockFooter";        
        
        const String ctCMDClose                                      = "@popupcmd%close";
        const String ctCMDShowDetail                                 = "@popupcmd%showdetail";

        const String ctCOLReceiptID                                  = "ReceiptID";
        const String ctCOLTotalAmount                                = "TotalAmount";        
        const String ctCOLTotalCost                                  = "TotalCost";
        
        const String ctFLTReceiptFilter                              = "ReceiptID = $RECEIPTID";

        const String ctTIDSale                                       = "sale";

        const String ctATTStatic                                     = "STATIC";
        
        LanguageManager         clLanguageManager;
        SettingManager          clSettingManager;
        DataTable               clDataTable;
        String                  clTitle;
        String                  clTypeID;
        DateTime                clDate;
        bool                    clAllowProfitLossView;

        public CompositeFormInterface SCI_ParentForm { get; set; }

        public SubControlPOSPopUpReceiptDetail(DateTime paDate, String paTitle, String paTypeID, DataTable paDataTable, bool paAllowPrfitLossView = false)
        {
            clDataTable             = paDataTable;
            clTitle                 = paTitle;
            clTypeID                = paTypeID;
            clDate                  = paDate;
            clAllowProfitLossView   = paAllowPrfitLossView;
            clSettingManager        = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;
            clLanguageManager       = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
        }
        
        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctSubControlPOSPopUpReceiptDetailStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctSubControlPOSPopUpReceiptDetailScript));
        }   

        private Decimal CalculateSum(int paReceiptID, String paColumnName)
        {
            return (General.SumDecimal(clDataTable, paColumnName, ctFLTReceiptFilter.Replace("$RECEIPTID", paReceiptID.ToString())));
        }       

        private void RenderReceiptBlockHeader(ComponentController paComponentController, POSReceiptDetailRow paReceiptDetailRow)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSBlockHeader);            
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSReceiptNoDiv);     
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write("#" + clLanguageManager.ConvertNumber(paReceiptDetailRow.ReceiptNo.ToString().PadLeft(6,'0')));
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSNameDiv);
            paComponentController.Write(paReceiptDetailRow.Name);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderReceiptBlockFooter(ComponentController paComponentController, int paReceiptID)
        {
            Decimal lcProfit;

            paComponentController.AddAttribute(HtmlAttribute.Class, ctClSBlockFooter);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            
            if ((clTypeID == ctTIDSale) && (clAllowProfitLossView))
            {
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSellPrice);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(clLanguageManager.ConvertNumber(CalculateSum(paReceiptID, ctCOLTotalAmount).ToString(clSettingManager.CurrencyFormatString)));
                paComponentController.RenderEndTag();

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCost);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(clLanguageManager.ConvertNumber(CalculateSum(paReceiptID, ctCOLTotalCost).ToString(clSettingManager.CurrencyFormatString)));
                paComponentController.RenderEndTag();

                lcProfit = CalculateSum(paReceiptID, ctCOLTotalAmount) + CalculateSum(paReceiptID, ctCOLTotalCost);

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSProfit);

                if (lcProfit < 0) paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Status, "negative");
                else if (lcProfit > 0) paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Status, "positive");

                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(clLanguageManager.ConvertNumber(lcProfit.ToString(clSettingManager.CurrencyFormatString)));
                paComponentController.RenderEndTag();
            }
            else
            {
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSProfit);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(clLanguageManager.ConvertNumber(CalculateSum(paReceiptID, ctCOLTotalAmount).ToString(clSettingManager.CurrencyFormatString)));
                paComponentController.RenderEndTag();
            }

            paComponentController.RenderEndTag();
        }

        private void RenderReceiptItem(ComponentController paComponentController, POSReceiptDetailRow paReceiptDetailRow)
        {
            Decimal lcProfit;

            if ((clTypeID == ctTIDSale) && (clSettingManager.Edition != SettingManager.EditionType.Cash_Register))
            {
                if ((paReceiptDetailRow.EntryAttribute != ctATTStatic) && (paReceiptDetailRow.DisplayQuantity > paReceiptDetailRow.RelativeBlance))
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Appearance, "shortsell");
            }

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSEntryRow);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSDescription);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(paReceiptDetailRow.Description);
            paComponentController.RenderEndTag();
            

            if ((clTypeID == ctTIDSale) && (clAllowProfitLossView))
            {
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSellPrice);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(clLanguageManager.ConvertNumber(paReceiptDetailRow.TotalAmount.ToString(clSettingManager.CurrencyFormatString)));
                paComponentController.RenderEndTag();

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCost);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(clLanguageManager.ConvertNumber(paReceiptDetailRow.TotalCost.ToString(clSettingManager.CurrencyFormatString)));
                paComponentController.RenderEndTag();

                lcProfit = paReceiptDetailRow.TotalAmount + paReceiptDetailRow.TotalCost;

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSProfit);

                if (lcProfit < 0) paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Status, "negative");
                else if (lcProfit > 0) paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Status, "positive");

                paComponentController.RenderBeginTag(HtmlTag.Span);
                paComponentController.Write(clLanguageManager.ConvertNumber(lcProfit.ToString(clSettingManager.CurrencyFormatString)));
                paComponentController.RenderEndTag();
            }
            else
            {
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSProfit);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(clLanguageManager.ConvertNumber(paReceiptDetailRow.TotalAmount.ToString(clSettingManager.CurrencyFormatString)));
                paComponentController.RenderEndTag();
            }

            paComponentController.RenderEndTag();
        }

        private void RenderReceiptBlock(ComponentController paComponentController, DataRow[] paReceiptDetailList)
        {
            POSReceiptDetailRow lcReceiptDetailRow;
            int                 lcReceiptID;
            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSReceiptBlock);
            paComponentController.AddElementType(ComponentController.ElementType.Block);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDShowDetail);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            if (paReceiptDetailList.Length > 0)
            {
                lcReceiptDetailRow = new POSReceiptDetailRow(paReceiptDetailList[0]);
                lcReceiptID = lcReceiptDetailRow.ReceiptID;

                RenderReceiptBlockHeader(paComponentController, lcReceiptDetailRow);

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItemContainer);
                paComponentController.RenderBeginTag(HtmlTag.Div);

                for (int lcCount = 0; lcCount < paReceiptDetailList.Length; lcCount++)
                {
                    lcReceiptDetailRow.Row = paReceiptDetailList[lcCount];
                    RenderReceiptItem(paComponentController, lcReceiptDetailRow);
                }

                paComponentController.RenderEndTag();

                RenderReceiptBlockFooter(paComponentController, lcReceiptID);
            }

            paComponentController.RenderEndTag();
        }

        private DataRow[] GetReceiptData(int paReceiptNo)
        {
            return (clDataTable.Select(ctFLTReceiptFilter.Replace("$RECEIPTID",paReceiptNo.ToString())));
        }
       
        private void RenderContainer(ComponentController paComponentController)
        {
            int[]                   lcReceiptList;
            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSContainer);
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            if (clDataTable != null)
            {
                lcReceiptList = clDataTable.AsEnumerable().Select(row => row.Field<int>(ctCOLReceiptID)).Distinct().ToArray();

                for (int lcCount = 0; lcCount < lcReceiptList.Length; lcCount++)
                {
                    RenderReceiptBlock(paComponentController, GetReceiptData(lcReceiptList[lcCount]));                    
                }
            }
            paComponentController.RenderEndTag();
        }

        private void RenderTitleBar(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTitle);
            paComponentController.AddElementType(ComponentController.ElementType.Title);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(clLanguageManager.ConvertNumber(clLanguageManager.GetText(clTitle).Replace("$DATE",clDate.ToString(clSettingManager.StaticDisplayDateFormat))));
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDClose);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.remove));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderBrowserMode(ComponentController paComponentController)
        {            
            IncludeExternalLinkFiles(paComponentController);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSubControlPOSPopUpReceiptDetailComposite);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, clTypeID);
            
            if (clAllowProfitLossView)
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Mode, "profitloss");

            paComponentController.AddElementType(ComponentController.ElementType.PopUp);            
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSubControlPOSPopUpReceiptDetail);
            paComponentController.AddElementType(ComponentController.ElementType.Panel);          
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderTitleBar(paComponentController);
            RenderContainer(paComponentController);         

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



