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
using System.Text.RegularExpressions;
using CobraBusinessFrame;


namespace CobraStandardControls
{
    public class SubControlPOSTransactionList : WebControl, WidgetControlInterface
    {
        const String ctSubControlPOSTransactionListStyle       = "SubControlPOSTransactionList.css";
        const String ctSubControlPOSTransactionListScript      = "SubControlPOSTransactionList.js";

        const String ctCLSSubControlPOSTransactionList       = "SubControlPOSTransactionList";
        const String ctCLSItemList                           = "ItemList";
        const String ctCLSMessageBar                         = "MessageBar";
        const String ctCLSSummaryBar                         = "SummaryBar";
        const String ctCLSQuantity                           = "Quantity";
        const String ctCLSSubtotal                           = "Subtotal";
        const String ctCLSDiscount                           = "Discount";
        const String ctCLSSubtotal2                          = "Subtotal2";
        const String ctCLSTaxAmount                          = "TaxAmount";
        const String ctCLSTotal                              = "Total";

        const String ctDYTSerialSeparator                    = "@@POS.Transaction.SerialSeparator";
        const String ctDYTSubtotalText                       = "@@POS.Transaction.SubTotalText";
        const String ctDYTDiscountText                       = "@@POS.Transaction.DiscountText";
        const String ctDYTSubTotal2Text                      = "@@POS.Transaction.Subtotal2Text";
        const String ctDYTTaxAmountText                      = "@@POS.Transaction.TaxAmountText";
        const String ctDYTTotalText                          = "@@POS.Transaction.TotalText";
        const String ctDYTTotalTaxIncludeText                = "@@POS.Transaction.TotalTaxIncludeText";

        //const String ctSETTaxPercent                        = "POS.TaxPercent";
        //const String ctSETTaxApplicable                     = "POS.TaxApplicable";

        const String ctUNTMajor                             = "MAJOR";
        const String ctUNTMinor                             = "MINOR";

        const String ctSTACancel                            = "CANCEL";

        LanguageManager     clLanguageManager;
        SettingManager      clSettingManager;

        public enum POSGridItem { Serial, Description, Quantity, UnitPrice, Discount, SubTotal }

        public CompositeFormInterface SCI_ParentForm { get; set; }

        private POSReceiptManager   clReceiptManager;
        private DataTable           clReceiptDetail;
        
        public SubControlPOSTransactionList(POSReceiptManager paReceiptManager)
        {
            clReceiptManager = paReceiptManager;
            clReceiptDetail  = null;

            clLanguageManager = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSettingManager = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;        
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;
            
            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctSubControlPOSTransactionListStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctSubControlPOSTransactionListScript));
        }

        private void RenderTransactionListRow(ComponentController paComponentController, POSTransactionListRow paTransactionListRow)
        {
            if (paTransactionListRow != null)
            {
                //if (paTransactionListRow.ItemStatus.ToUpper().Trim() == ctSTACancel)
                //{                    
                //    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_ItemText, paTransactionListRow.ItemName);
                //    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_UnitName, paTransactionListRow.UnitName);                    
                //    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_UnitRelationship, paTransactionListRow.UnitRelationship.ToString());   
                //}

                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_ItemStatus, paTransactionListRow.ItemStatus.ToLower());
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_ItemID, paTransactionListRow.ItemID.ToString());
                paComponentController.AddElementType(ComponentController.ElementType.Item);
                paComponentController.RenderBeginTag(HtmlTag.Div);

                paComponentController.AddElementType(ComponentController.ElementType.Element);
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, POSGridItem.Serial.ToString().ToLower());
                paComponentController.AddAttribute(HtmlAttribute.Value, paTransactionListRow.Serial.ToString());
                paComponentController.RenderBeginTag(HtmlTag.Div);
                
                paComponentController.RenderEndTag();

                paComponentController.AddElementType(ComponentController.ElementType.Element);
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, POSGridItem.Description.ToString().ToLower());
                paComponentController.RenderBeginTag(HtmlTag.Div);              
                paComponentController.RenderEndTag();

                paComponentController.AddElementType(ComponentController.ElementType.Element);
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_UnitMode, paTransactionListRow.UnitMode.ToLower());
                paComponentController.AddAttribute(HtmlAttribute.Value, paTransactionListRow.Quantity.ToString());
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, POSGridItem.Quantity.ToString().ToLower());
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.RenderEndTag();

                paComponentController.AddElementType(ComponentController.ElementType.Element);
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, POSGridItem.UnitPrice.ToString().ToLower());
                paComponentController.AddAttribute(HtmlAttribute.Value, paTransactionListRow.UnitPrice.ToString(clSettingManager.BareCurrencyFormatString));
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.RenderEndTag();

                paComponentController.AddElementType(ComponentController.ElementType.Element);
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, POSGridItem.Discount.ToString().ToLower());
                paComponentController.AddAttribute(HtmlAttribute.Value, paTransactionListRow.Discount.ToString());
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.RenderEndTag();

                paComponentController.AddElementType(ComponentController.ElementType.Element);
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, POSGridItem.SubTotal.ToString().ToLower());
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.RenderEndTag();                
                paComponentController.RenderEndTag();
            }
        }

        private void RenderGridRowTemplate(ComponentController paComponentController)
        {            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "template");
            paComponentController.AddElementType(ComponentController.ElementType.Item);
            paComponentController.RenderBeginTag(HtmlTag.Div);
                        
            paComponentController.AddElementType(ComponentController.ElementType.Element);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, POSGridItem.Serial.ToString().ToLower());
            paComponentController.RenderBeginTag(HtmlTag.Div);            
            paComponentController.RenderEndTag();

            paComponentController.AddElementType(ComponentController.ElementType.Element);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, POSGridItem.Description.ToString().ToLower());
            paComponentController.RenderBeginTag(HtmlTag.Div);            
            paComponentController.RenderEndTag();

            paComponentController.AddElementType(ComponentController.ElementType.Element);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, POSGridItem.Quantity.ToString().ToLower());
            paComponentController.RenderBeginTag(HtmlTag.Div);            
            paComponentController.RenderEndTag();

            paComponentController.AddElementType(ComponentController.ElementType.Element);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, POSGridItem.UnitPrice.ToString().ToLower());
            paComponentController.RenderBeginTag(HtmlTag.Div);            
            paComponentController.RenderEndTag();            

            paComponentController.AddElementType(ComponentController.ElementType.Element);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, POSGridItem.Discount.ToString().ToLower());
            paComponentController.RenderBeginTag(HtmlTag.Div);            
            paComponentController.RenderEndTag();

            paComponentController.AddElementType(ComponentController.ElementType.Element);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, POSGridItem.SubTotal.ToString().ToLower());
            paComponentController.RenderBeginTag(HtmlTag.Div);            
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderItemListContent(ComponentController paComponentController)
        {
            POSTransactionListRow lcTransactionListRow;

            lcTransactionListRow = new POSTransactionListRow(null);

            if (clReceiptDetail != null)
            {
                for (int lcCount = 0; lcCount < clReceiptDetail.Rows.Count; lcCount++)
                {
                    lcTransactionListRow.Row = clReceiptDetail.Rows[lcCount];
                    RenderTransactionListRow(paComponentController, lcTransactionListRow);
                }
            }
        }

        private void RenderItemList(ComponentController paComponentController, bool paRenderContent = false)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItemList);
            paComponentController.AddElementType(ComponentController.ElementType.List);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            if (paRenderContent) RenderItemListContent(paComponentController);
            
            paComponentController.RenderEndTag();
            RenderGridRowTemplate(paComponentController);
        }

        private void RenderSummaryElement(ComponentController paComponentController, String paClass, String paLabel)
        {   
            paComponentController.AddAttribute(HtmlAttribute.Class, paClass);
            paComponentController.AddElementType(ComponentController.ElementType.Row);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.Label);
            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(paLabel);
            paComponentController.RenderEndTag();

            paComponentController.AddElementType(ComponentController.ElementType.Figure);
            paComponentController.RenderBeginTag(HtmlTag.Span);            
            paComponentController.RenderEndTag();
            
            paComponentController.RenderEndTag();
        }

        private void RenderSummaryBar(ComponentController paComponentController)
        {            
            //bool    lcTaxApplicable;
         
            //lcTaxApplicable = General.ParseBoolean(clSettingManager.GetSettingValue(ctSETTaxApplicable), false);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSummaryBar);
            paComponentController.AddElementType(ComponentController.ElementType.Summary);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderSummaryElement(paComponentController, ctCLSQuantity, ComponentController.UnicodeStr((int) Fontawesome.cubes));
            RenderSummaryElement(paComponentController, ctCLSSubtotal, clLanguageManager.GetText(ctDYTSubtotalText));
            RenderSummaryElement(paComponentController, ctCLSDiscount, clLanguageManager.GetText(ctDYTDiscountText));
            
            if ((clReceiptManager.GetReceiptType() == POSReceiptManager.ReceiptType.Sale) && (clReceiptManager.ActiveRow.TaxPercent > 0))
            {
                if (clReceiptManager.ActiveRow.TaxInclusive)
                    RenderSummaryElement(paComponentController, ctCLSTotal, clLanguageManager.ConvertNumber(clLanguageManager.GetText(ctDYTTotalTaxIncludeText).Replace("$TAXPERCENT", clReceiptManager.ActiveRow.TaxPercent.ToString("0.##"))));
                else
                {
                    RenderSummaryElement(paComponentController, ctCLSSubtotal2, clLanguageManager.GetText(ctDYTSubTotal2Text));
                    RenderSummaryElement(paComponentController, ctCLSTaxAmount, clLanguageManager.ConvertNumber(clLanguageManager.GetText(ctDYTTaxAmountText).Replace("$TAXPERCENT", clReceiptManager.ActiveRow.TaxPercent.ToString("0.##"))));
                    RenderSummaryElement(paComponentController, ctCLSTotal, clLanguageManager.GetText(ctDYTTotalText));
                }                
            }
            else RenderSummaryElement(paComponentController, ctCLSTotal, clLanguageManager.GetText(ctDYTTotalText));

            paComponentController.RenderEndTag();
        }

        private void RenderMessageBar(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSMessageBar);
            paComponentController.AddElementType(ComponentController.ElementType.MessageBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.RenderEndTag();            
        }

        private void RenderBrowserMode(ComponentController paComponentController)
        {            
            IncludeExternalLinkFiles(paComponentController);

            if ((clReceiptManager != null) && (clReceiptManager.ActiveRow.ReceiptID != -1))
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Lastmodified, clReceiptManager.ActiveRow.LastModified.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Separator, clLanguageManager.GetText(ctDYTSerialSeparator));
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSubControlPOSTransactionList);            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "transactionlist");
            paComponentController.AddElementType(ComponentController.ElementType.Composite);            
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderItemList(paComponentController);
            RenderSummaryBar(paComponentController);
            RenderMessageBar(paComponentController);
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
            if (paRenderMode == null) RenderBrowserMode(paComponentController);
            else if (paRenderMode == "transactionlistcontent")
            {
                if (clReceiptManager != null) clReceiptDetail = clReceiptManager.ReceiptDetailList;
                RenderItemList(paComponentController, true);
            }            
        }       

        protected override void Render(HtmlTextWriter paHtmlTextWriter)
        {
            if (!DesignMode) RenderBrowserMode(new ComponentController(paHtmlTextWriter));
            else RenderDesignMode(new ComponentController(paHtmlTextWriter));
        }
    }
}
