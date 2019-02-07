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
    public class WidControlFEOrderList : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlFEOrderListStyle     = "WidControlFEOrderList.css";
        protected const String ctWidControlFEOrderListScript    = "WidControlFEOrderList.js";

        const String ctCLSWidControlFEOrderList                 = "WidControlFEOrderList";        
        const String ctCLSListPanel                             = "ListPanel";
        const String ctCLSOrderDiv                              = "OrderDiv";
        const String ctCLSOrderHeadingDiv                       = "OrderHeadingDiv";
        const String ctCLSHeadingText                           = "HeadingText";
        const String ctCLSEditButtonPanel                       = "EditButtonPanel";
                        
        const String ctCLSIconDiv                               = "IconDiv";
        const String ctCLSBriefDescriptionDiv                   = "BriefDescriptionDiv";
        const String ctCLSEntryRow                              = "EntryRow";
        const String ctCLSBullet                                = "Bullet";
        const String ctCLSItemName                              = "ItemName";
        const String ctCLSQuantity                              = "Quantity";
        const String ctCLSPrice                                 = "Price";
        const String ctCLSTotalSummary                          = "TotalSummary";
        const String ctCLSTotalQuantity                         = "TotalQuantity";
        const String ctCLSTotalPrice                            = "TotalPrice";
        const String ctCLSItemRemark                            = "ItemRemark";
        const String ctCLSRejectReason                          = "RejectReason";
        const String ctCLSOrderRemark                           = "OrderRemark";

        const String ctCLSOrderEditingPanel                     = "OrderEditingPanel";
        const String ctCLSButtonBar                             = "ButtonBar";
        const String ctCLSEditingButton                         = "EditingButton";
        const String ctCLSEditingButtonText                     = "EditingButtonText";
        const String ctCLSItemContainer                         = "ItemContainer";
        const String ctCLSItemBlock                             = "ItemBlock";
        const String ctCLSRemarkButton                          = "RemarkButton";
        const String ctCLSRemarkBox                             = "RemarkBox";
        const String ctCLSPricePanel                            = "PricePanel";
        const String ctCLSTotalPanel                            = "TotalPanel";
        const String ctCLSUnitPricePanel                        = "UnitPricePanel";

        const String ctCLSOrderRemarkPanel                      = "OrderRemarkPanel";
        const String ctCLSOrderRemarkTitle                      = "OrderRemarkTitle";
        const String ctCLSOrderRemarkBox                        = "OrderRemarkBox";
                        
        const String ctCLSItemButtonPanel                       = "ItemButtonPanel";

        const String ctCLSPrefixSymbol                          = "PrefixSymbol";
        const String ctCLSSuffixSymbol                          = "SuffixSymbol";

        const String ctCLSSummaryBar                            = "SummaryBar";
        const String ctCLSTotalQuantityDiv                      = "TotalQuantityDiv";
        const String ctCLSTotalQuantityLabel                    = "TotalQuantityLabel";
        const String ctCLSTotalQuantityBox                      = "TotalQuantityBox";
        const String ctCLSTotalPriceDiv                         = "TotalPriceDiv";
        const String ctCLSTotalPriceLabel                       = "TotalPriceLabel";
        const String ctCLSTotalPriceBox                         = "TotalPriceBox";
        const String ctCLSOrderNote                             = "OrderNote";

        const String ctCLSAjaxImage                             = "AjaxImage";

        const String ctCLSRefreshBar                            = "RefreshBar";
        const String ctCLSButton                                = "Button";
        const String ctCLSStatusDiv                             = "StatusDiv";
        const String ctCLSTimerDiv                              = "TimerDiv";
        const String ctCLSLastUpdateDiv                         = "LastUpdateDiv";
                        
        const String ctCOLRemark                                = "Remark";
        const String ctCOLItemName                              = "ItemName";
        const String ctCOLUnitPrice                             = "UnitPrice";
        const String ctCOLQuantity                              = "Quantity";
        
        const String ctFASClock                                 = "&#xf017";        
        const String ctFASInfoCycle                             = "&#xf05a";
        const String ctFASRightArrow                            = "&#xf0a9";
        const String ctFASDoubleRight                           = "&#xf101";
        const String ctFASCross                                 = "&#xf00d";
        const String ctFASReject                                = "&#xf05e";
        const String ctFASCrossCycle                            = "&#xf057";

        const String ctICOPlusButton                            = "plus_button.png";
        const String ctICOMinusButton                           = "minus_button.png";
        //const String ctICOEllipsisButton                        = "ellipsis_button.png";
        ////const String ctICO2UpChevron                            = "2upchevron.png";
        ////const String ctICOCartIcon                              = "carticon.png";
        const String ctICOUpChevron                             = "upchevron.png";
        const String ctICODownChevron                           = "downchevron.png";
        const String ctICOEditPencil                            = "edit_pencil.png";
        const String ctICOBack                                  = "back.png";
        const String ctICOBin                                   = "bin.png";
        const String ctICOTickButton                            = "tick_button.png";

        const String ctICORefresh                               = "refresh.png";

        const String ctAJAXBigCircle                            = "AJAX_IndicatorBigCircle.gif";

        const String ctCMDEditOrder                             = "@cmd%editorder";

        const String ctCMDRefresh                               = "@cmd%refresh";

        const String ctCMDShowRemark                            = "@cmd%showremark";
        const String ctCMDHideRemark                            = "@cmd%hideremark";

        const String ctCMDAddQuantity                           = "@cmd%addquantity";
        const String ctCMDSubQuantity                           = "@cmd%subquantity";

        const String ctCMDDeleteOrder                           = "@cmd%deleteorder";
        const String ctCMDUpdateOrder                           = "@cmd%updateorder";
        const String ctCMDBack                                  = "@cmd%back";

        const String ctFLTOrderDetail                           = "OrderNo = $ORDERNO";

        const String ctDEFCurrencySymbol                        = "MMK";
        const String ctDEFPriceFormatString                     = "F0";
        const int    ctDEFInputLimit                            = 9;
        const String ctDEFCloseText                             = "Close";
        const String ctDEFUpdateText                            = "Update";
        const String ctDEFDeleteText                            = "Delete";
        const String ctDEFTotalQuantityText                     = "Total Quantity";
        const String ctDEFTotalPriceText                        = "Total Price";
        const String ctDEFOrderRemarkTitle                      = "ORDER REMARK";
        const int    ctDEFRefreshTimeOut                        = 1;

        public CompositeFormInterface SCI_ParentForm                        { get; set; }

        public DataTable            SC_OrderInfoList                        { get; set; }
        public DataTable            SC_OrderDetail                          { get; set; }
        public String               SC_PriceFormatString                    { get; set; }
        public String               SC_CurrencySymbol                       { get; set; }
        public CurrencyPosition     SC_CurrencyPosition                     { get; set; }
        public int                  SC_InputLimit                           { get; set; }
        public String               SC_CloseText                            { get; set; }
        public String               SC_UpdateText                           { get; set; }
        public String               SC_DeleteText                           { get; set; }
        public String               SC_TotalQuantityText                    { get; set; }
        public String               SC_TotalPriceText                       { get; set; }
        public String               SC_OrderRemarkTitle                     { get; set; }

        public int                  SC_RefreshTimeOut                       { get; set; }

        
        public String SC_NumberLanguage
        {
            get { return (clLanguage.ToString()); }
            set { clLanguage = LanguageManager.ParseLanguage(value); }
        }

        LanguageManager.Language clLanguage;

        public WidControlFEOrderList()
        {
            SC_OrderInfoList        = null;
            SC_OrderDetail          = null;

            SC_PriceFormatString    = ctDEFPriceFormatString;
            SC_CurrencySymbol       = ctDEFCurrencySymbol;
            SC_CurrencyPosition     = CurrencyPosition.Prefix;
            SC_InputLimit           = ctDEFInputLimit;

            SC_CloseText            = ctDEFCloseText;
            SC_UpdateText           = ctDEFUpdateText;
            SC_DeleteText           = ctDEFDeleteText;

            SC_TotalQuantityText    = ctDEFTotalQuantityText;
            SC_TotalPriceText       = ctDEFTotalPriceText;

            SC_OrderRemarkTitle     = ctDEFOrderRemarkTitle;

            SC_RefreshTimeOut       = ctDEFRefreshTimeOut;
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_General, ctWidControlFEOrderListStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_General, ctWidControlFEOrderListScript));
        }

        private DataRow[] RetrieveOrderDetailList(int paOrderNo)
        {
            if (SC_OrderDetail != null)
                return(SC_OrderDetail.Select(ctFLTOrderDetail.Replace("$ORDERNO", paOrderNo.ToString())));            
            else return (null);
        }

        private void RenderRefrehBarButton(ComponentController paComponentController, String paIcon, int paTimeOut, bool paHref)
        {
            if (paHref) paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDRefresh);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_TimeOut, paTimeOut.ToString());

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButton);
            paComponentController.RenderBeginTag(HtmlTag.A);

            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(paIcon));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderRefreshBar(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSRefreshBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSStatusDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTimerDiv);
            paComponentController.AddElementType(ComponentController.ElementType.Timer);
            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(SC_RefreshTimeOut.ToString().PadLeft(2, '0') + ":00");
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSAjaxImage);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetAjaxLoaderImageUrl(ctAJAXBigCircle));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSLastUpdateDiv);
            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(General.GetCurrentSystemLocalTime().ToString("HH:mm"));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            RenderRefrehBarButton(paComponentController, ctICORefresh, 0, true);

            paComponentController.RenderEndTag();
        }
                
        private void RenderEditingButtons(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSEditingButton);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDDeleteOrder);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOBin));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSEditingButtonText);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(SC_DeleteText);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSEditingButton);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDUpdateOrder);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOTickButton));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSEditingButtonText);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(SC_UpdateText);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSEditingButton);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDBack);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOBack));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSEditingButtonText);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(SC_CloseText);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderOrderHeading(ComponentController paComponentController, OrderInfoRow paOrderInfoRow, int paTotalQuantity)
        {            
            if ((paTotalQuantity <= 0) || (paOrderInfoRow.OrderStatus < 0))
            {
                if (paOrderInfoRow.OrderStatus == (int) OrderInfoManager.OrderUniversalStatus.Reject)
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Appearance, "reject");
                else
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Appearance, "cancel");                
            }

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSOrderHeadingDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSHeadingText);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(paOrderInfoRow.OrderNo.ToString().PadLeft(6, '0'));
            paComponentController.RenderEndTag();
                        
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSIconDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(ctFASClock);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSHeadingText);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(paOrderInfoRow.OrderDate.ToString("dd/MM/yyyy"));
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "status");
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSIconDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(ctFASInfoCycle);
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "status");
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSHeadingText);
            paComponentController.RenderBeginTag(HtmlTag.Div);
        //    paComponentController.Write(ApplicationFrame.GetInstance().ActiveSubscription.GetOrderStatusText(paOrderInfoRow.OrderStatus));
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSEditButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDEditOrder);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOEditPencil));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();            

            paComponentController.RenderEndTag();            

            paComponentController.RenderEndTag();
        }

        private void RenderBriefDescriptionDiv(ComponentController paComponentController,OrderInfoRow paOrderInfoRow, DataRow[] paOrderDetailList, int paTotalQuantity, Decimal paTotalAmount)
        {
            OrderDetailRow  lcOrderDetailRow;                        
            int             lcOverrideStatus;
            int             lcEffectiveStatus;
            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSBriefDescriptionDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            
            if (paOrderDetailList != null)
            {
                lcOrderDetailRow    = new OrderDetailRow(null);
                lcOverrideStatus    = paOrderInfoRow.OrderStatus;

                for (int lcCount = 0; lcCount < paOrderDetailList.Length; lcCount++)
                {
                    lcOrderDetailRow.Row = paOrderDetailList[lcCount];
                    if (!String.IsNullOrEmpty(lcOrderDetailRow.ItemName))
                    {
                        if (lcOverrideStatus < 0) lcEffectiveStatus = lcOverrideStatus;
                        else lcEffectiveStatus = lcOrderDetailRow.Status;

                        paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Appearance, ((OrderInfoManager.OrderUniversalStatus) lcEffectiveStatus).ToString().ToLower());

                        paComponentController.AddElementType(ComponentController.ElementType.Row);
                        paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DataID, lcOrderDetailRow.ItemID.ToString());
                        paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSEntryRow);
                        paComponentController.RenderBeginTag(HtmlTag.Div);

                        //paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSBullet);
                        //paComponentController.RenderBeginTag(HtmlTag.Span);
                        //paComponentController.Write(lcBullet);
                        //paComponentController.RenderEndTag();
                        
                        paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItemName);
                        paComponentController.RenderBeginTag(HtmlTag.Span);
                        paComponentController.Write(lcOrderDetailRow.ItemName);
                        paComponentController.RenderEndTag();
                        
                        paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSQuantity);
                        paComponentController.RenderBeginTag(HtmlTag.Span);
                        paComponentController.Write(lcOrderDetailRow.Quantity.ToString("F0"));
                        paComponentController.RenderEndTag();

                        paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPrice);
                        paComponentController.RenderBeginTag(HtmlTag.Span);
                        paComponentController.Write((lcOrderDetailRow.UnitPrice * lcOrderDetailRow.Quantity).ToString("F0"));
                        paComponentController.RenderEndTag();

                        if (lcEffectiveStatus == (int)OrderInfoManager.OrderUniversalStatus.Reject)
                        {
                            if (!String.IsNullOrWhiteSpace(lcOrderDetailRow.RejectReason))
                            {
                                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSRejectReason);
                                paComponentController.RenderBeginTag(HtmlTag.Span);
                                paComponentController.Write(lcOrderDetailRow.RejectReason);
                                paComponentController.RenderEndTag();
                            }
                        }
                        else
                        {
                            if (!String.IsNullOrWhiteSpace(lcOrderDetailRow.Remark))
                            {
                                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItemRemark);
                                paComponentController.RenderBeginTag(HtmlTag.Span);
                                paComponentController.Write(lcOrderDetailRow.Remark);
                                paComponentController.RenderEndTag();
                            }
                        }                            

                        paComponentController.RenderEndTag();   
                    }
                }

                paComponentController.AddElementType(ComponentController.ElementType.Summary);
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTotalSummary);
                paComponentController.RenderBeginTag(HtmlTag.Div);

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTotalQuantity);
                paComponentController.RenderBeginTag(HtmlTag.Span);
                paComponentController.Write(paTotalQuantity.ToString("F0"));
                paComponentController.RenderEndTag();

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTotalPrice);
                paComponentController.RenderBeginTag(HtmlTag.Span);
                paComponentController.Write(paTotalAmount.ToString("F0"));
                paComponentController.RenderEndTag();

                if (paOrderInfoRow.OrderStatus == (int)OrderInfoManager.OrderUniversalStatus.Reject)
                {
                    paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSOrderRemark);
                    paComponentController.RenderBeginTag(HtmlTag.Div);
                    paComponentController.Write(paOrderInfoRow.RejectReason);
                    paComponentController.RenderEndTag();
                }
                else
                {
                    if (!String.IsNullOrEmpty(paOrderInfoRow.OrderRemark))
                    {
                        paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSOrderRemark);
                        paComponentController.RenderBeginTag(HtmlTag.Div);
                        paComponentController.Write(paOrderInfoRow.OrderRemark);
                        paComponentController.RenderEndTag();
                    }
                }

                paComponentController.RenderEndTag();  
            }

            paComponentController.RenderEndTag();
        }

        private void RenderElement(ComponentController paComponentController, String paClassName, String paText)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, paClassName);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(paText);
            paComponentController.RenderEndTag();
        }

        private void RenderItemButtons(ComponentController paComponentController, OrderDetailRow paOrderDetailRow)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItemButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDAddQuantity);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOPlusButton));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_InputMode, "number");
            paComponentController.AddElementType(ComponentController.ElementType.Quantity);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctCOLQuantity);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_OriginalValue, paOrderDetailRow.Quantity.ToString());
            paComponentController.AddAttribute(HtmlAttribute.Maxlength, SC_InputLimit.ToString());
            paComponentController.AddAttribute(HtmlAttribute.ReadOnly, "true");
            paComponentController.AddAttribute(HtmlAttribute.Type, "text");
            paComponentController.AddAttribute(HtmlAttribute.Value, paOrderDetailRow.Quantity.ToString());
            paComponentController.RenderBeginTag(HtmlTag.Input);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDSubQuantity);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOMinusButton));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderPriceComponent(ComponentController paComponentController, Decimal paPrice, String paClassName, ComponentController.ElementType paElementType, String paColumnName)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, paClassName);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            if (SC_CurrencyPosition == CurrencyPosition.Prefix)
            {
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPrefixSymbol);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(SC_CurrencySymbol);
                paComponentController.RenderEndTag();

                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, paColumnName);
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_OriginalValue, paPrice.ToString(SC_PriceFormatString));
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPrice);
                paComponentController.AddElementType(paElementType);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(paPrice.ToString(SC_PriceFormatString));
                paComponentController.RenderEndTag();
            }
            else
            {
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, paColumnName);
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_OriginalValue, paPrice.ToString(SC_PriceFormatString));
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPrice);
                paComponentController.AddElementType(paElementType);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(paPrice.ToString(SC_PriceFormatString));
                paComponentController.RenderEndTag();

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSuffixSymbol);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(SC_CurrencySymbol);
                paComponentController.RenderEndTag();
            }

            paComponentController.RenderEndTag();
        }

        private void RenderItemBlock(ComponentController paComponentController, OrderDetailRow paOrderDetailRow)
        {
            if (paOrderDetailRow != null)
            {   
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DataID, paOrderDetailRow.ItemID.ToString());                
                paComponentController.AddElementType(ComponentController.ElementType.Element);
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItemBlock);
                paComponentController.RenderBeginTag(HtmlTag.Div);

                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctCOLItemName);
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_OriginalValue, paOrderDetailRow.ItemName);
                RenderElement(paComponentController, ctCLSItemName, paOrderDetailRow.ItemName);

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSRemarkButton);
                paComponentController.RenderBeginTag(HtmlTag.Div);

                paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDShowRemark);
                paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICODownChevron));
                paComponentController.RenderBeginTag(HtmlTag.Img);
                paComponentController.RenderEndTag();

                paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDHideRemark);
                paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOUpChevron));
                paComponentController.RenderBeginTag(HtmlTag.Img);
                paComponentController.RenderEndTag();

                paComponentController.RenderEndTag();

                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctCOLRemark);
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_OriginalValue, paOrderDetailRow.Remark);
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSRemarkBox);
                paComponentController.AddAttribute(HtmlAttribute.Value, paOrderDetailRow.Remark);
                paComponentController.AddAttribute(HtmlAttribute.Type, "text");
                paComponentController.RenderBeginTag(HtmlTag.Input);
                paComponentController.RenderEndTag();

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPricePanel);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                RenderPriceComponent(paComponentController, paOrderDetailRow.UnitPrice * paOrderDetailRow.Quantity, ctCLSTotalPanel, ComponentController.ElementType.Total, String.Empty);
                RenderPriceComponent(paComponentController, paOrderDetailRow.UnitPrice, ctCLSUnitPricePanel, ComponentController.ElementType.UnitPrice, ctCOLUnitPrice);
                paComponentController.RenderEndTag();

                RenderItemButtons(paComponentController, paOrderDetailRow);

                paComponentController.RenderEndTag();
            }
        }

        private void RenderItemList(ComponentController paComponentController, DataRow[] paOrderDetailList)
        {            
            OrderDetailRow      lcOrdetailRow;

            if (paOrderDetailList != null)
            {
                lcOrdetailRow = new OrderDetailRow(null);

                for (int lcCount = 0; lcCount < paOrderDetailList.Length; lcCount++)
                {
                    lcOrdetailRow.Row = paOrderDetailList[lcCount];
                    if (lcOrdetailRow.Status >= 0)
                        RenderItemBlock(paComponentController, lcOrdetailRow);
                }
            }
        }

        private void RenderSummaryBar(ComponentController paComponentController, int paTotalQuantity, Decimal paTotalAmount)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Summary);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSummaryBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTotalQuantityDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTotalQuantityLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(SC_TotalQuantityText);
            paComponentController.RenderEndTag();

            paComponentController.AddElementType(ComponentController.ElementType.Quantity);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTotalQuantityBox);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(paTotalQuantity.ToString());
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTotalPriceDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTotalPriceLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(SC_TotalPriceText);
            paComponentController.RenderEndTag();

            RenderPriceComponent(paComponentController, paTotalAmount, ctCLSPricePanel, ComponentController.ElementType.Total, String.Empty);

            paComponentController.RenderEndTag();

            // RenderElement(paComponentController, ctCLSOrderNote, ApplicationFrame.GetInstance().ActiveSubscription.ActiveRow.OrderNote);

            paComponentController.RenderEndTag();
        }

        private void RenderRemarkPanel(ComponentController paComponentController, OrderInfoRow paOrderInfoRow)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSOrderRemarkPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderElement(paComponentController, ctCLSOrderRemarkTitle, SC_OrderRemarkTitle);
                        
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSOrderRemarkBox);
            paComponentController.AddAttribute(HtmlAttribute.Value, paOrderInfoRow.OrderRemark);
            paComponentController.AddAttribute(HtmlAttribute.Type, "text");
            paComponentController.RenderBeginTag(HtmlTag.Input);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderEditingPanel(ComponentController paComponentController, OrderInfoRow paOrderInfoRow, DataRow[] paOrderDetailList, int paTotalQuantity, Decimal paTotalAmount)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DataID, paOrderInfoRow.OrderNo.ToString());
            paComponentController.AddElementType(ComponentController.ElementType.Composite);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSOrderEditingPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            
            RenderEditingButtons(paComponentController);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItemContainer);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            RenderItemList(paComponentController, paOrderDetailList);
            paComponentController.RenderEndTag();

            RenderSummaryBar(paComponentController, paTotalQuantity, paTotalAmount);
            RenderRemarkPanel(paComponentController, paOrderInfoRow);

            paComponentController.RenderEndTag();
        }


        private void CalculateOrderDetailSummary(OrderInfoRow paOrderInfoRow, DataRow[] paOrderDetailList, out int paTotalQuantity, out Decimal paTotalAmount)
        {
            OrderDetailRow  lcOrderDetailRow;
            int             lcEffectiveStatus;

            lcOrderDetailRow = new OrderDetailRow(null);
            paTotalQuantity = 0;
            paTotalAmount = 0M;

            if (paOrderDetailList != null)
            {                
                for (int lcCount = 0; lcCount < paOrderDetailList.Length; lcCount++)
                {
                    lcOrderDetailRow.Row = paOrderDetailList[lcCount];
                    if (paOrderInfoRow.OrderStatus < 0) lcEffectiveStatus = paOrderInfoRow.OrderStatus;
                    else lcEffectiveStatus = lcOrderDetailRow.Status;

                    if ((lcEffectiveStatus != (int)OrderInfoManager.OrderUniversalStatus.Cancel) &&
                        (lcEffectiveStatus != (int)OrderInfoManager.OrderUniversalStatus.Reject))
                    {
                        paTotalQuantity += lcOrderDetailRow.Quantity;
                        paTotalAmount += (lcOrderDetailRow.Quantity * lcOrderDetailRow.UnitPrice);
                    }
                }
            }
        }

        private void RenderOrderEntry(ComponentController paComponentController, OrderInfoRow paOrderInfoRow, DataRow[] paOrderDetail)
        {
            int         lcTotalQuantity;
            Decimal     lcTotalAmount;

            CalculateOrderDetailSummary(paOrderInfoRow, paOrderDetail, out lcTotalQuantity, out lcTotalAmount);

            if ((paOrderInfoRow.OrderStatus != (int)OrderInfoManager.OrderUniversalStatus.Submitted) &&
                (paOrderInfoRow.OrderStatus != (int)OrderInfoManager.OrderUniversalStatus.Requested))
            {
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ReadOnlyMode, "true");
            }

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DataID, paOrderInfoRow.OrderNo.ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Status, paOrderInfoRow.OrderStatus.ToString());
            paComponentController.AddElementType(ComponentController.ElementType.Element);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSOrderDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderOrderHeading(paComponentController, paOrderInfoRow, lcTotalQuantity);
            RenderBriefDescriptionDiv(paComponentController, paOrderInfoRow, paOrderDetail, lcTotalQuantity, lcTotalAmount);
            RenderEditingPanel(paComponentController, paOrderInfoRow, paOrderDetail, lcTotalQuantity, lcTotalAmount);
            

            //paComponentController.AddAttribute(HtmlAttribute.Class, ctHea);
            //paComponentController.RenderBeginTag(HtmlTag.Div);
            //paComponentController.Write(ctFASInfoCycle);
            //paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderOrderList(ComponentController paComponentController)
        {
            OrderInfoRow   lcOrderInfoRow;
            DataRow[]      lcOrderDetailList;

            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSListPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            if (SC_OrderInfoList != null)
            {
                lcOrderInfoRow = new OrderInfoRow(null);
            
                for (int lcCount = 0; lcCount < SC_OrderInfoList.Rows.Count; lcCount++)
                {
                    lcOrderInfoRow.Row = SC_OrderInfoList.Rows[lcCount];
                    lcOrderDetailList = RetrieveOrderDetailList(lcOrderInfoRow.OrderNo);
                    RenderOrderEntry(paComponentController, lcOrderInfoRow, lcOrderDetailList);
                }
            }

            paComponentController.RenderEndTag();
            
        }

        protected void RenderBrowserMode(ComponentController paComponentController)
        {   
            IncludeExternalLinkFiles(paComponentController);

            // paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_AdditionalData, ApplicationFrame.GetInstance().ActiveSubscription.ActiveRow.OrderStatusType);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_TimeOut, SC_RefreshTimeOut.ToString());
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlFEOrderList);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderRefreshBar(paComponentController);

            RenderOrderList(paComponentController);

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

