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
    public class WidControlBEItemList : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlBEItemListStyle  = "WidControlBEItemList.css";
        protected const String ctWidControlBEItemListScript = "WidControlBEItemList.js";

        public enum ControlMode     {  Item, Category }
        public enum CurrencyPositon {  Prefix, Suffix }        

        const String ctCLSWidControlBEItemList  = "WidControlBEItemList";
        const String ctCLSCategoryTitle         = "CategoryTitle";
        const String ctCLSContainer             = "Container";
        const String ctCLSItemBlock             = "ItemBlock";        
        const String ctCLSItemName              = "ItemName";
        const String ctCLSDescription           = "Description";
        const String ctCLSPrefixSymbol          = "PrefixSymbol";
        const String ctCLSSuffixSymbol          = "SuffixSymbol";
        const String ctCLSPriceDiv              = "PriceDiv";
        const String ctCLSPrice                 = "Price";
        
        const String ctCLSButtonPanel           = "ButtonPanel";
        const String ctCLSButton                = "Button";

        const String ctCLSEditingBar            = "EditingBar";
        const String ctCLSButtonDiv             = "ButtonDiv";

        const String ctICOEditButton            = "edit_pencil.png";
        const String ctICODeleteButton          = "cross_button.png";
        const String ctICORightChevron          = "rtchevron.png"; 
        
        const String ctCMDEdit                  = "@cmd%edititem";
        const String ctCMDDelete                = "@cmd%deleteitem";
        const String ctCMDShowChild             = "@cmd%showchild";        
        const String ctPRMEditItem              = "FormBEAddAdjustItem,ItemID::$ITEMID;;";
        const String ctPRMEditCategory          = "FormBEAddAdjustCategory,ItemID::$ITEMID;;";

        const String ctPRMShowChild             = "FormBEItemListByCategory,Category::$CATEGORYCODE;;CategoryName::$CATEGORYNAME;;";     
        const String ctPRMAddItem               = "FormBEAddAdjustItem,Category::$CATEGORYCODE;;";
        const String ctPRMAddCategory           = "FormBEAddAdjustCategory,Category::$CATEGORYCODE;;";        

        const String ctDEFButtonText            = "Add New";
        const String ctDEFCurrencySymbol        = "MMK";
        const String ctDEFPriceFormatString     = "F0";
        const String ctDEFCategoryTitle         = "@[CATEGORYNAME]";

        const String ctFPRCategory              = "Category";

        public CompositeFormInterface SCI_ParentForm        { get; set; }

        public DataTable        SC_ItemList                 { get; set; }
        public ControlMode      SC_ControlMode              { get; set; }
        public String           SC_CategoryTitle            { get; set; }
        public bool             SC_TitleVisible             { get; set; }
        public bool             SC_DescriptionVisible       { get; set; }
        public bool             SC_PriceVisible             { get; set; }
        public String           SC_PriceFormatString        { get; set; }
        public String           SC_CurrencySymbol           { get; set; }
        public CurrencyPositon  SC_CurrencyPosition         { get; set; }
        public String           SC_ButtonText               { get; set; }
        
        public String           SC_NumberLanguage           
        {
            get { return (clLanguage.ToString()); } 
            set { clLanguage = LanguageManager.ParseLanguage(value); } 
        }

        LanguageManager.Language clLanguage;

        public WidControlBEItemList()
        {            
            SC_ItemList                 = null;
            SC_ControlMode              = ControlMode.Item;
            SC_CategoryTitle            = ctDEFCategoryTitle;
            SC_TitleVisible             = false;
            SC_DescriptionVisible       = false;
            SC_PriceVisible             = true;          
            SC_PriceFormatString        = ctDEFPriceFormatString;
            SC_CurrencySymbol           = ctDEFCurrencySymbol;
            SC_ButtonText               = ctDEFButtonText;
            SC_CurrencyPosition         = CurrencyPositon.Prefix;
            clLanguage                  = LanguageManager.Language.English;
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_General, ctWidControlBEItemListStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_General, ctWidControlBEItemListScript));
        }        

        private void RenderButtonPanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDShowChild);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICORightChevron));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDEdit);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOEditButton));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDDelete);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICODeleteButton));
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

        private void RenderTitle(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCategoryTitle);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(ApplicationFrame.GetInstance().ActiveFormInfoManager.TranslateString(SC_CategoryTitle));
            paComponentController.RenderEndTag();
        }

        private void RenderPricePanel(ComponentController paComponentController, Decimal paPrice)
        {
            if (SC_PriceVisible)
            {
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPriceDiv);                
                paComponentController.RenderBeginTag(HtmlTag.Div);

                if (SC_CurrencyPosition == CurrencyPositon.Prefix)
                {
                    RenderElement(paComponentController, ctCLSPrefixSymbol, SC_CurrencySymbol);
                    RenderElement(paComponentController, ctCLSPrice, paPrice.ToString(SC_PriceFormatString));
                }
                else
                {
                    RenderElement(paComponentController, ctCLSPrice, paPrice.ToString(SC_PriceFormatString));
                    RenderElement(paComponentController, ctCLSSuffixSymbol, SC_CurrencySymbol);
                }

                paComponentController.RenderEndTag();
            }
        }

        

        private void RenderElementBlock(ComponentController paComponentController, StandardItemCatalogueRow paStandardCatalogueRow)
        {
            // String  lcShowChildParam;

            if (paStandardCatalogueRow != null)
            {
                //if (SC_ControlMode == ControlMode.Category)
                //{
                //    paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDShowChild);

                //    lcShowChildParam = ctPRMShowChild.Replace("$CATEGORYCODE",paStandardCatalogueRow.ItemID.ToString());
                //    lcShowChildParam = lcShowChildParam.Replace("$CATEGORYNAME", paStandardCatalogueRow.EntryName);
                //    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Parameter, lcShowChildParam);
                //}

                paComponentController.AddElementType(ComponentController.ElementType.Element);
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DataID, paStandardCatalogueRow.ItemID.ToString());
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, paStandardCatalogueRow.EntryType);
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Group, paStandardCatalogueRow.Category.ToString());
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItemBlock);
                paComponentController.RenderBeginTag(HtmlTag.Div);

                RenderButtonPanel(paComponentController);

                RenderElement(paComponentController, ctCLSItemName, paStandardCatalogueRow.EntryName);

                if ((SC_DescriptionVisible) && (!String.IsNullOrEmpty(paStandardCatalogueRow.Description)))
                   RenderElement(paComponentController, ctCLSDescription, paStandardCatalogueRow.Description);

                RenderPricePanel(paComponentController, paStandardCatalogueRow.Price);
                
                paComponentController.RenderEndTag();
            }
        }       


        //private void RenderButton(ComponentController paComponentController)
        //{
        //    int lcParentCategory;

        //    paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);
        //    paComponentController.RenderBeginTag(HtmlTag.Div);

        //    paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButton);
        //    paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDAddItem);
            
        //    lcParentCategory = General.ParseInt(ApplicationFrame.GetInstance().ActiveFormInfoManager.GetFormParam(ctFPRCategory), 0);

        //    if (SC_ControlMode == ControlMode.Item)
        //        paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Parameter, ctPRMAddItem.Replace("$CATEGORYCODE",lcParentCategory.ToString()));
        //    else
        //        paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Parameter, ctPRMAddCategory.Replace("$CATEGORYCODE",lcParentCategory.ToString()));

        //    paComponentController.RenderBeginTag(HtmlTag.A);
        //    paComponentController.Write(SC_ButtonText);
        //    paComponentController.RenderEndTag();

        //    paComponentController.RenderEndTag();
        //}

        private void RenderContainer(ComponentController paComponentController)
        {
            StandardItemCatalogueRow   lcStandardItemCatalogueRow;

            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSContainer);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            
            if (SC_ItemList != null) 
            {
                lcStandardItemCatalogueRow = new StandardItemCatalogueRow(null);

                for (int lcCount = 0; lcCount < SC_ItemList.Rows.Count; lcCount++)
               {
                   lcStandardItemCatalogueRow.Row = SC_ItemList.Rows[lcCount];
                   RenderElementBlock(paComponentController, lcStandardItemCatalogueRow);
               }
            }

          //  RenderButton(paComponentController);

            paComponentController.RenderEndTag(); // Container
        }
       
        protected void RenderBrowserMode(ComponentController paComponentController)
        {
            int     lcParentCategory;

            IncludeExternalLinkFiles(paComponentController);
            
            if (SC_ControlMode == ControlMode.Category)
            {   
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Template, ctPRMEditCategory + "<!!>" + ctPRMAddCategory + "<!!>" + ctPRMShowChild);                
            }
            else
            {
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Template, ctPRMEditItem + "<!!>" + ctPRMAddItem);                
            }

            lcParentCategory = General.ParseInt(ApplicationFrame.GetInstance().ActiveFormInfoManager.GetFormParam(ctFPRCategory), 0);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Group, lcParentCategory.ToString());

            if (SC_TitleVisible)
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Appearance, "title");

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ControlMode, SC_ControlMode.ToString().ToLower());
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlBEItemList);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            if (SC_TitleVisible)
                RenderTitle(paComponentController);
            

            RenderContainer(paComponentController);                        

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
