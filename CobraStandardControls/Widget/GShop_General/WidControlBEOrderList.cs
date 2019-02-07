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
    public class WidControlBEOrderList : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlBEOrderListStyle     = "WidControlBEOrderList.css";
        protected const String ctWidControlBEOrderListScript    = "WidControlBEOrderList.js";
        
        const String ctCLSWidControlBEOrderList                 = "WidControlBEOrderList";        
        const String ctCLSListPanel                             = "ListPanel";
        const String ctCLSOrderDiv                              = "OrderDiv";
        const String ctCLSOrderHeadingDiv                       = "OrderHeadingDiv";
        const String ctCLSHeadingText                           = "HeadingText";
        const String ctCLSAjaxImage                             = "AjaxImage";

        const String ctCLSFilterBar                             = "FilterBar";
        const String ctCLSButton                                = "Button";
        const String ctCLSBufferDiv                             = "BufferDiv";

        const String ctCLSRefreshBar                            = "RefreshBar";
        const String ctCLSStatusDiv                             = "StatusDiv";
        const String ctCLSTimerDiv                              = "TimerDiv";
        const String ctCLSLastUpdateDiv                         = "LastUpdateDiv";
        
        const String ctCLSIconDiv                               = "IconDiv";
        const String ctCLSBriefDescriptionDiv                   = "BriefDescriptionDiv";
        const String ctCLSEntryRow                              = "EntryRow";        
        const String ctCLSBullet                                = "Bullet";
        const String ctCLSItemName                              = "ItemName";
        const String ctCLSQuantity                              = "Quantity";
        const String ctCLSPrice                                 = "Price";
        const String ctCLSTotalQuantity                         = "TotalQuantity";
        const String ctCLSTotalPrice                            = "TotalPrice";
        const String ctCLSItemRemark                            = "ItemRemark";
        const String ctCLSRejectReason                          = "RejectReason";
        
        const String ctCLSCustomerInfoPanel                     = "CustomerInfoPanel";
        const String ctCLSBriefInfoDiv                          = "BriefInfoDiv";
        const String ctCLSContactNoDiv                          = "ContactNoDiv";
        const String ctCLSNameDiv                               = "NameDiv";
        const String ctCLSShortAddressDiv                       = "ShortAddressDiv";
        const String ctCLSButtonDiv                             = "ButtonDiv";
        const String ctCLSCompleteInfoDiv                       = "CompleteInfoDiv";
        const String ctCLSAddressDiv                            = "AddressDiv";

        const String ctCLSBaseBarPanel                          = "BaseBarPanel";        
        const String ctCLSFloatButtonBar                        = "FloatButtonBar";

        const String ctCLSOrderRemarkDiv                        = "OrderRemarkDiv";

        const String ctCLSRejectPopUp                           = "RejectPopUp";
        const String ctCLSRejectTitleDiv                        = "RejectTitleDiv";
        const String ctCLSRejectOptionPanel                     = "RejectOptionPanel";
        const String ctCLSRejectButtonPanel                     = "RejectButtonPanel";
        const String ctCLSRejectButton                          = "RejectButton";
        const String ctCLSCancelButton                          = "CancelButton";

        const String ctCLSOverlayDiv                            = "OverlayDiv";

        const String ctRejectReasonName                         = "RejectReason";

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

        const String ctICOUpChevron                             = "upchevron.png";
        const String ctICODownChevron                           = "downchevron.png";
        const String ctICOCross                                 = "cross_button.png";
        const String ctICOProcessing                            = "processing.png";
        const String ctICOShipping                              = "shipping.png";
        const String ctICODelivered                             = "delivered.png";
        const String ctICOBack                                  = "back.png";
        const String ctICOOpenPopUp                             = "openpopup.png";
        const String ctICOOpenPopUpGray                         = "openpopup_gray.png";
        const String ctICOLeftChevron                           = "ltchevron.png";
        const String ctICORightChevron                          = "rtchevron.png";
        const String ctICOMinus                                 = "minus_button.png";

        const String ctICOIconEnvalope                          = "icon_envalope.png";
        const String ctICOIconProcess                           = "icon_process.png";        
        const String ctICOIconShippping                         = "icon_shipping.png";
        const String ctICOIconDelivered                         = "icon_delivered.png";
        const String ctICOIconReject                            = "icon_reject.png";

        const String ctICOButton1_Green                         = "1_green.png";
        const String ctICOButton1_Gray                          = "1_gray.png";
        const String ctICOButton3_Green                         = "3_green.png";
        const String ctICOButton3_Gray                          = "3_gray.png";
        const String ctICOButton5_Green                         = "5_green.png";
        const String ctICOButton5_Gray                          = "5_gray.png";
        const String ctICORefresh                               = "refresh.png";

        const String ctAJAXBigCircle                            = "AJAX_IndicatorBigCircle.gif";

        const String ctCMDFilter                                = "@cmd%filter";
        const String ctCMDRefresh                               = "@cmd%refresh";

        const String ctCMDHideDetail                            = "@cmd%hidedetail";
        const String ctCMDShowDetail                            = "@cmd%showdetail";

        const String ctCMDRejectItem                            = "@cmd%rejectitem";

        const String ctCMDRejectOrder                           = "@cmd%rejectorder";
        const String ctCMDChangeOrderState                      = "@cmd%changeorderstate";

        const String ctCMDBack                                  = "@cmd%back";

        const String ctCMDShowButtonBar                         = "@cmd%showbuttonbar";
        const String ctCMDHideButtonBar                         = "@cmd%hidebuttonbar";

        const String ctCMDRejectProceed                         = "@cmd%rejectproceed";
        const String ctCMDRejectCancel                          = "@cmd%rejectcancel";

        const String ctFLTOrderDetail                           = "OrderNo = $ORDERNO";

        const String ctDEFRejectPopUpTitle                      = "REJECT REASON";
        const String ctDEFRejectPopUpOkButtonText               = "Reject";
        const String ctDEFRejectPopUpCancelButtonText           = "Cancel";
        const int    ctDEFRefreshTimeOut                        = 5;

        public CompositeFormInterface SCI_ParentForm                            { get; set; }

        public DataTable    SC_OrderInfoList                                    { get; set; }
        public DataTable    SC_OrderDetail                                      { get; set; }        
        public String       SC_RejectPopUpTitle                                 { get; set; }
        public String       SC_RejectPopUpOkButtonText                          { get; set; }
        public String       SC_RejectPopUpCancelButtonText                      { get; set; }
        public int          SC_RefreshTimeOut                                   { get; set; }
        
        public String SC_NumberLanguage
        {
            get { return (clLanguage.ToString()); }
            set { clLanguage = LanguageManager.ParseLanguage(value); }
        }

        LanguageManager.Language clLanguage;

        public WidControlBEOrderList()
        {
            SC_OrderInfoList        = null;
            SC_OrderDetail          = null;

            SC_RejectPopUpTitle             = ctDEFRejectPopUpTitle;
            SC_RejectPopUpOkButtonText      = ctDEFRejectPopUpOkButtonText;
            SC_RejectPopUpCancelButtonText  = ctDEFRejectPopUpCancelButtonText;
            SC_RefreshTimeOut               = ctDEFRefreshTimeOut;
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_General, ctWidControlBEOrderListStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_General, ctWidControlBEOrderListScript));
        }

        private DataRow[] RetrieveOrderDetailList(int paOrderNo)
        {
            if (SC_OrderDetail != null)
                return(SC_OrderDetail.Select(ctFLTOrderDetail.Replace("$ORDERNO", paOrderNo.ToString())));            
            else return (null);
        }

        private void RenderRefrehBarButton(ComponentController paComponentController, String paIcon, int paTimeOut, bool paHref)
        {
            if (paHref)  paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDRefresh);
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

            RenderRefrehBarButton(paComponentController, ctICOButton1_Green,1, false);
            RenderRefrehBarButton(paComponentController, ctICOButton1_Gray, 1, true);
            RenderRefrehBarButton(paComponentController, ctICOButton3_Green, 3, false);
            RenderRefrehBarButton(paComponentController, ctICOButton3_Gray, 3, true);
            RenderRefrehBarButton(paComponentController, ctICOButton5_Green, 5, false);
            RenderRefrehBarButton(paComponentController, ctICOButton5_Gray, 5, true);

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

        private void RenderFilterBarButton(ComponentController paComponentController, String paIcon, OrderInfoManager.OrderUniversalStatus paStatus)
        {
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDFilter);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Status, ((int)paStatus).ToString());
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButton);
            paComponentController.RenderBeginTag(HtmlTag.A);            
            
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(paIcon));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();
            
            paComponentController.RenderEndTag();
        }
        
       
        private void RenderFilterBar(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSFilterBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            
            RenderFilterBarButton(paComponentController, ctICOIconEnvalope, OrderInfoManager.OrderUniversalStatus.Requested);
            RenderFilterBarButton(paComponentController, ctICOIconProcess, OrderInfoManager.OrderUniversalStatus.Processing);
            RenderFilterBarButton(paComponentController, ctICOIconShippping, OrderInfoManager.OrderUniversalStatus.Shipping);
            RenderFilterBarButton(paComponentController, ctICOIconDelivered, OrderInfoManager.OrderUniversalStatus.Delivered);
            RenderFilterBarButton(paComponentController, ctICOIconReject, OrderInfoManager.OrderUniversalStatus.Reject);

            paComponentController.RenderEndTag();
        }

        private void RenderOrderHeading(ComponentController paComponentController, OrderInfoRow paOrderInfoRow, int paTotalQuantity)
        {   
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
         //   paComponentController.Write(ApplicationFrame.GetInstance().ActiveSubscription.GetOrderStatusText(paOrderInfoRow.OrderStatus));
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSAjaxImage);            
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetAjaxLoaderImageUrl(ctAJAXBigCircle));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();


            paComponentController.RenderEndTag();
        }

        private void RenderCustomerAddressPanel(ComponentController paComponentController, OrderInfoRow paOrderInfoRow)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCustomerInfoPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSBriefInfoDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSNameDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(paOrderInfoRow.Name);
            paComponentController.RenderEndTag();
            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSContactNoDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(paOrderInfoRow.ContactNo);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSShortAddressDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(UILogic.CompileShortAddress(paOrderInfoRow.Row));
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDShowDetail);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICODownChevron));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDHideDetail);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOUpChevron));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCompleteInfoDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            
            paComponentController.RenderBeginTag(HtmlTag.Span); 
            paComponentController.Write(UILogic.CompileAddress(paOrderInfoRow.Row));
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

                        if ((lcEffectiveStatus >= 0) && (lcOrderDetailRow.Quantity <= 0)) lcEffectiveStatus = (int) OrderInfoManager.OrderUniversalStatus.Cancel;
                        
                     //   paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Appearance, ((OrderInfoManager.OrderUniversalStatus) lcEffectiveStatus).ToString().ToLower());
                        paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DataID, lcOrderDetailRow.ItemID.ToString());
                        paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Status, lcEffectiveStatus.ToString());
                        paComponentController.AddElementType(ComponentController.ElementType.Row);
                        paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSEntryRow);
                        paComponentController.RenderBeginTag(HtmlTag.Div);

                        
                        paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItemName);
                        paComponentController.RenderBeginTag(HtmlTag.Span);
                        paComponentController.Write(lcOrderDetailRow.ItemName);
                        paComponentController.RenderEndTag();

                        paComponentController.AddElementType(ComponentController.ElementType.Quantity);
                        paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSQuantity);
                        paComponentController.RenderBeginTag(HtmlTag.Span);
                        paComponentController.Write(lcOrderDetailRow.Quantity.ToString("F0"));
                        paComponentController.RenderEndTag();

                        paComponentController.AddElementType(ComponentController.ElementType.Total);
                        paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPrice);
                        paComponentController.RenderBeginTag(HtmlTag.Span);
                        paComponentController.Write((lcOrderDetailRow.UnitPrice * lcOrderDetailRow.Quantity).ToString("F0"));
                        paComponentController.RenderEndTag();

                        if (lcEffectiveStatus >= 0)
                        {
                            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonDiv);
                            paComponentController.RenderBeginTag(HtmlTag.Div);

                            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDRejectItem);
                            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOCross));
                            paComponentController.RenderBeginTag(HtmlTag.Img);
                            paComponentController.RenderEndTag();

                            paComponentController.RenderEndTag();
                        }

                        if ((!String.IsNullOrWhiteSpace(lcOrderDetailRow.Remark)) && (lcEffectiveStatus != (int)OrderInfoManager.OrderUniversalStatus.Reject))
                        {
                            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItemRemark);
                            paComponentController.RenderBeginTag(HtmlTag.Span);
                            paComponentController.Write(lcOrderDetailRow.Remark);
                            paComponentController.RenderEndTag();
                        }

                        paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSRejectReason);
                        paComponentController.RenderBeginTag(HtmlTag.Span);
                        paComponentController.Write(lcOrderDetailRow.RejectReason);
                        paComponentController.RenderEndTag();
                        

                        paComponentController.RenderEndTag();   
                    }
                }

                paComponentController.AddElementType(ComponentController.ElementType.Quantity);
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTotalQuantity);
                paComponentController.RenderBeginTag(HtmlTag.Span);
                paComponentController.Write(paTotalQuantity.ToString("F0"));
                paComponentController.RenderEndTag();

                paComponentController.AddElementType(ComponentController.ElementType.Total);
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTotalPrice);
                paComponentController.RenderBeginTag(HtmlTag.Span);
                paComponentController.Write(paTotalAmount.ToString("F0"));
                paComponentController.RenderEndTag();  
            }

            RenderFloatButtonBar(paComponentController);

            paComponentController.RenderEndTag();
        }


        private void RenderFloatButtonBar(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSFloatButtonBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Status, ((int) OrderInfoManager.OrderUniversalStatus.Reject).ToString());
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDRejectOrder);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOCross));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();


            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Status, ((int)OrderInfoManager.OrderUniversalStatus.Processing).ToString());
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDChangeOrderState);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOProcessing));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Status, ((int)OrderInfoManager.OrderUniversalStatus.Shipping).ToString());
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDChangeOrderState);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOShipping));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Status, ((int)OrderInfoManager.OrderUniversalStatus.Delivered).ToString());
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDChangeOrderState);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICODelivered));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderBaseButtonPanel(ComponentController paComponentController, OrderInfoRow paOrderInfoRow)
        {
            String  lcRemarkText;
            
            lcRemarkText        = String.Empty;

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSBaseBarPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDShowButtonBar);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOOpenPopUp));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();
                        
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOOpenPopUpGray));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDHideButtonBar);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOMinus));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
                        
            if ((OrderInfoManager.OrderUniversalStatus)paOrderInfoRow.OrderStatus == OrderInfoManager.OrderUniversalStatus.Reject)
            {
                if (!String.IsNullOrEmpty(paOrderInfoRow.RejectReason))
                {
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_HasValue, "true");
                    lcRemarkText = paOrderInfoRow.RejectReason;
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(paOrderInfoRow.OrderRemark))
                {
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_HasValue, "true");
                    lcRemarkText = paOrderInfoRow.OrderRemark;
                }
            }
            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSOrderRemarkDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(lcRemarkText);            
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        

        private void RenderElement(ComponentController paComponentController, String paClassName, String paText)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, paClassName);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(paText);
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
            String      lcAttribute;

            lcAttribute = String.Empty;

            CalculateOrderDetailSummary(paOrderInfoRow, paOrderDetail, out lcTotalQuantity, out lcTotalAmount);

            if ((paOrderInfoRow.OrderStatus != (int)OrderInfoManager.OrderUniversalStatus.Submitted) &&
                (paOrderInfoRow.OrderStatus != (int)OrderInfoManager.OrderUniversalStatus.Requested))
            {
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ReadOnlyMode, "true");
            }

            if ((lcTotalQuantity <= 0) || (paOrderInfoRow.OrderStatus < 0))
            {
                //if (paOrderInfoRow.OrderStatus == (int)OrderInfoManager.OrderUniversalStatus.Reject)
                //    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Appearance, "reject");
                //else
                //    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Appearance, "cancel");
            }
            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DataID, paOrderInfoRow.OrderNo.ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Status, paOrderInfoRow.OrderStatus.ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Attribute, lcAttribute);
            paComponentController.AddElementType(ComponentController.ElementType.Element);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSOrderDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderOrderHeading(paComponentController, paOrderInfoRow, lcTotalQuantity);
            RenderCustomerAddressPanel(paComponentController, paOrderInfoRow);
            RenderBriefDescriptionDiv(paComponentController, paOrderInfoRow, paOrderDetail, lcTotalQuantity, lcTotalAmount);
            RenderBaseButtonPanel(paComponentController, paOrderInfoRow);
            // RenderEditingPanel(paComponentController, paOrderInfoRow, paOrderDetail, lcTotalQuantity, lcTotalAmount);
            

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

        private void RenderRejectReasonPopUp(ComponentController paComponentController)
        {
            String[]        lcRejectReasonsList;

         //   lcRejectReasonsList = ApplicationFrame.GetInstance().ActiveSubscription.GetRejectReasonList();
            lcRejectReasonsList = null;

            paComponentController.AddElementType(ComponentController.ElementType.PopUp);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSRejectPopUp);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSRejectTitleDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(SC_RejectPopUpTitle);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSRejectOptionPanel);

            paComponentController.RenderBeginTag(HtmlTag.Div);
            for (int lcCount = 0; lcCount < lcRejectReasonsList.Length; lcCount++)
            {
                paComponentController.AddAttribute(HtmlAttribute.Name, ctRejectReasonName);
                paComponentController.AddAttribute(HtmlAttribute.Id, ctRejectReasonName + lcCount.ToString());
                paComponentController.AddAttribute(HtmlAttribute.Type, "radio");
                paComponentController.AddAttribute(HtmlAttribute.Value, lcRejectReasonsList[lcCount]);
                paComponentController.RenderBeginTag(HtmlTag.Input);                
                paComponentController.RenderEndTag();

                paComponentController.AddAttribute(HtmlAttribute.For, ctRejectReasonName + lcCount.ToString());
                paComponentController.RenderBeginTag(HtmlTag.Label);
                paComponentController.Write(lcRejectReasonsList[lcCount]);
                paComponentController.RenderEndTag();
            }
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSRejectButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSRejectButton);
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDRejectProceed);
            paComponentController.RenderBeginTag(HtmlTag.A);

            paComponentController.Write(SC_RejectPopUpOkButtonText);

            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCancelButton);
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDRejectCancel);
            paComponentController.RenderBeginTag(HtmlTag.A);

            paComponentController.Write(SC_RejectPopUpCancelButtonText);

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
            
            paComponentController.RenderEndTag();
        }

        protected void RenderOverlayPanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSOverlayDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.RenderEndTag();
        }

        
        protected void RenderBrowserMode(ComponentController paComponentController)
        {   
            IncludeExternalLinkFiles(paComponentController);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_TimeOut, SC_RefreshTimeOut.ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Status, ((int)OrderInfoManager.OrderUniversalStatus.Requested).ToString());
            // paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_AdditionalData, ApplicationFrame.GetInstance().ActiveSubscription.ActiveRow.OrderStatusType);
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlBEOrderList);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderRefreshBar(paComponentController);
            RenderFilterBar(paComponentController);
            RenderOrderList(paComponentController);
            RenderRejectReasonPopUp(paComponentController);
            RenderOverlayPanel(paComponentController);        

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

