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
    public class WidControlFEBasicStore : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlWidControlFEBasicStoreStyle = "WidControlFEBasicStore.css";
        protected const String ctWidControlWidControlFEBasicStoreScript = "WidControlFEBasicStore.js";

        public enum ControlMode     {  SingleLevel, DualLevel }        

        const String ctCLSWidControlFEBasicStoreSingleLevel     = "WidControlFEBasicStore";
        const String ctCLSCategoryTitle                         = "CategoryTitle";
        const String ctCLSContainer                             = "Container";
        const String ctCLSItemBlock                             = "ItemBlock";
        const String ctCLSCategoryBlock                         = "CategoryBlock";
        const String ctCLSItemName                              = "ItemName";
        const String ctCLSDescription                           = "Description";
        const String ctCLSPrefixSymbol                          = "PrefixSymbol";
        const String ctCLSSuffixSymbol                          = "SuffixSymbol";
        const String ctCLSPrice                                 = "Price";
        const String ctCLSItemButtonPanel                       = "ItemButtonPanel";
        const String ctCLSCategoryButtonPanel                   = "CategoryButtonPanel";
        const String ctCLSCartIconPanel                         = "CartIconPanel";

        const String ctCLSConfirmationTitle                     = "ConfirmationTitle";
        const String ctCLSPricePanel                            = "PricePanel";
        const String ctCLSUnitPricePanel                        = "UnitPricePanel";
        const String ctCLSTotalPanel                            = "TotalPanel";
                
        const String ctCLSRemarkButton                          = "RemarkButton";
        const String ctCLSRemarkBox                             = "RemarkBox";
        
        const String ctCLSSummaryBar                            = "SummaryBar";
        const String ctCLSTotalQuantityDiv                      = "TotalQuantityDiv";
        const String ctCLSTotalQuantityLabel                    = "TotalQuantityLabel";
        const String ctCLSTotalQuantityBox                      = "TotalQuantityBox";
        const String ctCLSTotalPriceDiv                         = "TotalPriceDiv";
        const String ctCLSTotalPriceLabel                       = "TotalPriceLabel";
        const String ctCLSTotalPriceBox                         = "TotalPriceBox";
        const String ctCLSOrderNote                             = "OrderNote";

        const String ctCLSDeliveryOverlay                       = "DeliveryOverlay";
        const String ctCLSDeliveryInfo                          = "DeliveryInfo";
        const String ctCLSDeliveryTitle                         = "DeliveryTitle";
        const String ctCLSDeliveryInfoButton                    = "DeliveryInfoButton";
        const String ctCLSStaticInfo                            = "StaticInfo";
        const String ctCLSStaticLabel                           = "StaticLabel";
        const String ctCLSStaticData                            = "StaticData";
        const String ctCLSEditPanel                             = "EditPanel";

        const String ctCLSOrderRemarkPanel                      = "OrderRemarkPanel";
        const String ctCLSOrderRemarkTitle                      = "OrderRemarkTitle";
        const String ctCLSOrderRemarkBox                        = "OrderRemarkBox";

        const String ctCLSButtonPanel                           = "ButtonPanel";
        const String ctCLSOrderButton                           = "OrderButton";
        const String ctCLSBackButton                            = "BackButton";

        const String ctCTLSelectionPanel                        = "SELECTIONPANEL";

        const String ctICOPlusButton                            = "plus_button.png";
        const String ctICOMinusButton                           = "minus_button.png";
        const String ctICOEllipsisButton                        = "ellipsis_button.png";
        const String ctICO2UpChevron                            = "2upchevron.png";
        const String ctICOCartIcon                              = "carticon.png";
        const String ctICOUpChevron                             = "upchevron.png";
        const String ctICODownChevron                           = "downchevron.png";
        const String ctICOEditPencil                            = "edit_pencil.png";
        const String ctICOTickButton                            = "tick_button.png";
        const String ctICOCrossButton                           = "cross_button.png";
                
        const String ctCMDAddQuantity                           = "@cmd%addquantity";
        const String ctCMDSubQuantity                           = "@cmd%subquantity";
        
        const String ctCMDShowChild                             = "@cmd%showchild";
        const String ctCMDHideChild                             = "@cmd%hidechild";

        const String ctCMDShowRemark                            = "@cmd%showremark";
        const String ctCMDHideRemark                            = "@cmd%hideremark";

        const String ctCMDEditAddress                           = "@cmd%editaddress";
        const String ctCMDUpdateAddress                         = "@cmd%updateaddress";
        const String ctCMDCancelAddress                         = "@cmd%canceladdress";
        
        const String ctCMDPlaceOrder                            = "@cmd%placeorder";
        const String ctCMDBack                                  = "@cmd%back";

        const String ctSTICartIcon                              = "@sti%carticon";

        const String ctCOLName                                  = "Name";
        const String ctCOLContactNo                             = "ContactNo";
        const String ctCOLCompiledAddress                       = "Vir_CompiledAddress";

        const String ctCOLItemName                              = "ItemName";
        const String ctCOLQuantity                              = "Quantity";
        const String ctCOLUnitPrice                             = "UnitPrice";
        const String ctCOLRemark                                = "Remark";
        
        const String ctDEFCurrencySymbol        = "MMK";
        const String ctDEFPriceFormatString     = "F0";
        const String ctDEFCategoryTitle         = "@[CATEGORYNAME]";
        const int    ctDEFInputLimit            = 9;
        const String ctDEFConfirmationTitle     = "CONFIRMATION";
        const String ctDEFTotalQuantityText     = "Total Quantity";
        const String ctDEFTotalPriceText        = "Total Price";

        const String ctDEFDeliveryTitle         = "Delivery Address";
        const String ctDEFNameLabel             = "Name";
        const String ctDEFContactNoLabel        = "Contact No.";
        const String ctDEFAddressLabel          = "Address";

        const String ctDEFOrderRemarkTitle      = "ORDER REMARK";

        const String ctDEFOrderButtonText       = "Order";
        const String ctDEFBackButtonText        = "Back";
        
        const String ctFLTCategoryList          = "EntryType = 'Category'";
        const String ctFLTItemList              = "EntryType = 'Item'";

        public CompositeFormInterface SCI_ParentForm        { get; set; }

        public DataRow SC_ActiveDataRow                     { get; set; }

        public DataTable        SC_ItemList                 { get; set; }
        public DataRow          SC_DeliveryAddress          { get; set; }
        public DataRow          SC_UserAddress              { get; set; }
        public ControlMode      SC_ControlMode              { get; set; }                
        public String           SC_PriceFormatString        { get; set; }
        public String           SC_CurrencySymbol           { get; set; }
        public CurrencyPosition SC_CurrencyPosition         { get; set; }
        public int              SC_InputLimit               { get; set; }
        
        public String           SC_ConfirmationTitle        { get; set; }
        public String           SC_TotalQuantityText        { get; set; }
        public String           SC_TotalPriceText           { get; set; }

        public String           SC_DeliveryTitle            { get; set; }
        public String           SC_NameLabel                { get; set; }
        public String           SC_ContactNoLabel           { get; set; }
        public String           SC_AddressLabel             { get; set; }

        public String           SC_OrderRemarkTitle         { get; set; }

        public String           SC_OrderButtonText          { get; set; }
        public String           SC_BackButtonText           { get; set; }
        
        public String           SC_NumberLanguage           
        {
            get { return (clLanguage.ToString()); } 
            set { clLanguage = LanguageManager.ParseLanguage(value); } 
        }

        LanguageManager.Language clLanguage;

        public WidControlFEBasicStore()
        {            
            SC_ItemList                 = null;
            SC_DeliveryAddress          = null;
            SC_UserAddress              = null;
            SC_ControlMode              = ControlMode.SingleLevel;            
            SC_PriceFormatString        = ctDEFPriceFormatString;
            SC_CurrencySymbol           = ctDEFCurrencySymbol;
            SC_InputLimit               = ctDEFInputLimit;

            SC_ConfirmationTitle        = ctDEFConfirmationTitle;
            SC_TotalQuantityText        = ctDEFTotalQuantityText;
            SC_TotalPriceText           = ctDEFTotalPriceText;

            SC_DeliveryTitle            = ctDEFDeliveryTitle;
            SC_NameLabel                = ctDEFNameLabel;
            SC_ContactNoLabel           = ctDEFContactNoLabel;
            SC_AddressLabel             = ctDEFAddressLabel;

            SC_OrderRemarkTitle         = ctDEFOrderRemarkTitle;

            SC_OrderButtonText          = ctDEFOrderButtonText;
            SC_BackButtonText           = ctDEFBackButtonText;

            SC_CurrencyPosition         = CurrencyPosition.Prefix;
            clLanguage                  = LanguageManager.Language.English;                        
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_General, ctWidControlWidControlFEBasicStoreStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_General, ctWidControlWidControlFEBasicStoreScript));
        }
       

        private DataRow[] GetCategoryList()
        {
            if (SC_ItemList != null)
                return (SC_ItemList.Select(ctFLTCategoryList));
            else return (null);            
        }

        private DataRow[] GetItemList()
        {
            if (SC_ItemList != null)
                return (SC_ItemList.Select(ctFLTItemList));
            else return (null);
        }

        private void RenderItemButtons(ComponentController paComponentController)
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
            paComponentController.AddAttribute(HtmlAttribute.Maxlength, SC_InputLimit.ToString());
            paComponentController.AddAttribute(HtmlAttribute.ReadOnly, "true");
            paComponentController.AddAttribute(HtmlAttribute.Type, "text");
            paComponentController.AddAttribute(HtmlAttribute.Value, "0");
            paComponentController.RenderBeginTag(HtmlTag.Input);
            paComponentController.RenderEndTag(); 

            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDSubQuantity);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOMinusButton));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderCategoryButtons(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCategoryButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDShowChild);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOEllipsisButton));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDHideChild);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICO2UpChevron));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderCategoryCartIcon(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCartIconPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
                        
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOCartIcon));
            paComponentController.RenderBeginTag(HtmlTag.Img);
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
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPrice);
                paComponentController.AddElementType(paElementType);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(paPrice.ToString(SC_PriceFormatString));
                paComponentController.RenderEndTag();
            }
            else
            {
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, paColumnName);
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

        private void RenderSummaryBar(ComponentController paComponentController)
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
            paComponentController.Write("0");
            paComponentController.RenderEndTag();
            
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTotalPriceDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTotalPriceLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(SC_TotalPriceText);
            paComponentController.RenderEndTag();

            RenderPriceComponent(paComponentController, 0, ctCLSPricePanel, ComponentController.ElementType.Total, String.Empty);            
    
            paComponentController.RenderEndTag();

            // RenderElement(paComponentController, ctCLSOrderNote, ApplicationFrame.GetInstance().ActiveSubscription.ActiveRow.OrderNote);

            paComponentController.RenderEndTag();
        }

        private void RenderItemBlock(ComponentController paComponentController, StandardItemCatalogueRow paStandardCatalogueRow)
        {
            if (paStandardCatalogueRow != null)
            {    
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DataID, paStandardCatalogueRow.ItemID.ToString());
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Group, paStandardCatalogueRow.Category.ToString());
                paComponentController.AddElementType(ComponentController.ElementType.Element);
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItemBlock);
                paComponentController.RenderBeginTag(HtmlTag.Div);

                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctCOLItemName);
                RenderElement(paComponentController, ctCLSItemName, paStandardCatalogueRow.EntryName);

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
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSRemarkBox);
                paComponentController.AddAttribute(HtmlAttribute.Type, "text");
                paComponentController.RenderBeginTag(HtmlTag.Input);
                paComponentController.RenderEndTag();

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPricePanel);                
                paComponentController.RenderBeginTag(HtmlTag.Div);
                RenderPriceComponent(paComponentController, 0, ctCLSTotalPanel, ComponentController.ElementType.Total, String.Empty);
                RenderPriceComponent(paComponentController, paStandardCatalogueRow.Price, ctCLSUnitPricePanel, ComponentController.ElementType.UnitPrice, ctCOLUnitPrice);
                paComponentController.RenderEndTag();

                RenderItemButtons(paComponentController);

                paComponentController.RenderEndTag();
            }
        }

        private void RenderCategoryBlock(ComponentController paComponentController, StandardItemCatalogueRow paStandardCatalogueRow)
        {
            if (paStandardCatalogueRow != null)
            {
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DataID, paStandardCatalogueRow.ItemID.ToString());
                paComponentController.AddElementType(ComponentController.ElementType.Element);
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCategoryBlock);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                
                RenderElement(paComponentController, ctCLSItemName, paStandardCatalogueRow.EntryName);
                RenderCategoryButtons(paComponentController);
                RenderElement(paComponentController, ctCLSDescription, paStandardCatalogueRow.Description);
                RenderCategoryCartIcon(paComponentController);

                paComponentController.RenderEndTag();
            }
        }

        private void RenderButtonPanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSOrderButton);
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDPlaceOrder);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(SC_OrderButtonText);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSBackButton);
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDBack);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(SC_BackButtonText);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderCategoryList(ComponentController paComponentController)
        {
            DataRow[] lcDataRows;
            StandardItemCatalogueRow lcStandardItemCatalogueRow;

            if ((lcDataRows = GetCategoryList()) != null)
            {
                lcStandardItemCatalogueRow = new StandardItemCatalogueRow(null);

                for (int lcCount = 0; lcCount < lcDataRows.Length; lcCount++)
                {
                    lcStandardItemCatalogueRow.Row = lcDataRows[lcCount];
                    RenderCategoryBlock(paComponentController, lcStandardItemCatalogueRow);
                }
            }
        }

        private void RenderItemList(ComponentController paComponentController)
        {
            DataRow[] lcDataRows;
            StandardItemCatalogueRow lcStandardItemCatalogueRow;

            if ((lcDataRows = GetItemList()) != null)
            {
                lcStandardItemCatalogueRow = new StandardItemCatalogueRow(null);

                for (int lcCount = 0; lcCount < lcDataRows.Length; lcCount++)
                {
                    lcStandardItemCatalogueRow.Row = lcDataRows[lcCount];
                    RenderItemBlock(paComponentController, lcStandardItemCatalogueRow);
                }
            }            
        }

        private void RenderContainer(ComponentController paComponentController)
        {   
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSContainer);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "category");
            paComponentController.RenderBeginTag(HtmlTag.Div);
            RenderCategoryList(paComponentController);
            paComponentController.RenderEndTag();

            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSContainer);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "item");
            paComponentController.RenderBeginTag(HtmlTag.Div);
            RenderItemList(paComponentController);            

            paComponentController.RenderEndTag(); // Container
        }

        private void RenderDeliveryButtonPanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSDeliveryInfoButton);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDEditAddress);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOEditPencil));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDUpdateAddress);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOTickButton));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDCancelAddress);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOCrossButton));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();
            
            paComponentController.RenderEndTag();
        }

        private void RenderDeliveryStaticPanel(ComponentController paComponentController, DeliveryAddressRow paDeliveryAddressRow)
        {
            if (paDeliveryAddressRow != null)
            {
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSStaticInfo);
                paComponentController.RenderBeginTag(HtmlTag.Div);

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSStaticLabel);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(SC_NameLabel);
                paComponentController.RenderEndTag();

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSStaticData);
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctCOLName);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(paDeliveryAddressRow.Name);
                paComponentController.RenderEndTag();

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSStaticLabel);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(SC_ContactNoLabel);
                paComponentController.RenderEndTag();

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSStaticData);
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctCOLContactNo);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(paDeliveryAddressRow.ContactNo);
                paComponentController.RenderEndTag();

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSStaticLabel);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(SC_AddressLabel);
                paComponentController.RenderEndTag();

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSStaticData);
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctCOLCompiledAddress);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(paDeliveryAddressRow.Vir_CompiledAddress);
                paComponentController.RenderEndTag();

                paComponentController.RenderEndTag();
            }
        }

        protected void RenderDeliveryInfo(ComponentController paComponentController)
        {
            InputInfoManager        lcInputInfoManager;
            MetaDataRow             lcMetaDataRow;
            DeliveryAddressRow      lcDeliveryAddressRow;
            String                  lcControlMode;

            if (SC_DeliveryAddress == null)
            {
                lcControlMode = "insert";
                lcDeliveryAddressRow = new DeliveryAddressRow(SC_UserAddress);
                lcDeliveryAddressRow.SubscriptionID = ApplicationFrame.GetInstance().ActiveSubscription.ActiveRow.SubscriptionID;
            }
            else
            {
                lcControlMode = "update";
                lcDeliveryAddressRow = new DeliveryAddressRow(SC_DeliveryAddress);
            }

            lcInputInfoManager = ApplicationFrame.GetInstance().ActiveFormInfoManager.FieldInfoManager.ActiveInputInfoManager;
            lcInputInfoManager.CustomComponentRenderer += CustomComponentRendererHandler;
            lcMetaDataRow = new MetaDataRow(lcDeliveryAddressRow.Row);
            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSDeliveryInfo);
            paComponentController.AddElementType(ComponentController.ElementType.Container);            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "deliveryinfo");
            paComponentController.RenderBeginTag(HtmlTag.Div);
            
            // Delivery Info Overlay;
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSDeliveryOverlay);
            paComponentController.AddElementType(ComponentController.ElementType.Overlay);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.RenderEndTag();

            RenderElement(paComponentController, ctCLSDeliveryTitle, SC_DeliveryTitle);
            RenderDeliveryButtonPanel(paComponentController);
            RenderDeliveryStaticPanel(paComponentController, lcDeliveryAddressRow);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ControlMode, lcControlMode);
            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSEditPanel);
            paComponentController.AddElementType(ComponentController.ElementType.Control);   
            paComponentController.RenderBeginTag(HtmlTag.Div);

            
            if (lcInputInfoManager != null)
            {                
                lcInputInfoManager.RenderAllSubGroups(paComponentController, lcMetaDataRow);
            }

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void CustomComponentRendererHandler(ComponentController paComponentController, InputInfoRow paInputInfoRow, String paActiveValue)
        {
            if (paInputInfoRow.ControlType.ToUpper() == ctCTLSelectionPanel)
            {
                SubControlSelectionPanel lcSubControlSelectionPanel;

                lcSubControlSelectionPanel = new SubControlSelectionPanel(paInputInfoRow.InputMode.ToLower(), paInputInfoRow.AdditionalInfo, paInputInfoRow.InputLabel, paInputInfoRow.QueryName, paInputInfoRow.LinkColumn);
                lcSubControlSelectionPanel.RenderChildMode(paComponentController);
            }
        }


        private void RenderRemarkPanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSOrderRemarkPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderElement(paComponentController, ctCLSOrderRemarkTitle, SC_OrderRemarkTitle);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSOrderRemarkBox);
            paComponentController.AddAttribute(HtmlAttribute.Type, "text");
            paComponentController.RenderBeginTag(HtmlTag.Input);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderBrowserMode(ComponentController paComponentController)
        {            
            IncludeExternalLinkFiles(paComponentController);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ControlMode, SC_ControlMode.ToString().ToLower());
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlFEBasicStoreSingleLevel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            
            RenderElement(paComponentController, ctCLSConfirmationTitle, SC_ConfirmationTitle);
                        
            RenderContainer(paComponentController);
            RenderSummaryBar(paComponentController);

            RenderDeliveryInfo(paComponentController);
            RenderRemarkPanel(paComponentController);
            RenderButtonPanel(paComponentController);
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
