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
    public class SubControlPOSItemPanel : WebControl, WidgetControlInterface
    {
        const String ctSubControlPOSItemPanelStyle      = "SubControlPOSItemPanel.css";
        const String ctSubControlPOSItemPanelScript     = "SubControlPOSItemPanel.js";

        const String ctCLSSubControlPOSItemPanelComposite = "SubControlPOSItemPanelComposite";
        const String ctCLSSubControlPOSItemPanel        = "SubControlPOSItemPanel";
        const String ctCLSHeaderBar                     = "HeaderBar";
        const String ctCLSComponentContainer            = "ComponentContainer";
        const String ctCLSCategoryTitle                 = "CategoryTitle";
        const String ctCLSTitleText                     = "TitleText";
        const String ctCLSFavouriteTitle                = "FavouriteTitle";
        const String ctCLSBlockContainer                = "BlockContainer";
        const String ctCLSTerminatingBar                = "TerminatingBar";

        const String ctCLSItemName                      = "ItemName";
        const String ctCLSItemInfo                      = "ItemInfo";
        const String ctCLSItemCode                      = "ItemCode";
        const String ctCLSMajorPrice                    = "MajorPrice";
        const String ctCLSMinorPrice                    = "MinorPrice";

        const String ctCMDList                          = "@cmd%list";
        const String ctCMDFavourite                     = "@cmd%favourite";        
        const String ctCMDEditFavourite                 = "@cmd%editfavourite";
        const String ctCMDSwitchView                    = "@cmd%switchview";
        const String ctCMDClose                         = "@cmd%close";

        const String ctCMDRootCategory                  = "@cmd%rootcategory";
        const String ctCMDParentCategory                = "@cmd%parentcategory";

        const String ctCOLEntryType                     = "EntryType";
        const String ctCOLStatus                        = "Status";

        // const String ctSETItemPanelDisplayMode          = "POS.ItemPanelDisplayMode";
        const String ctSETTransactionSetting            = "POS.TransactionSetting";

        const String ctKEYItemPanelDisplayMode          = "itempaneldisplaymode";
        const String ctKEYItemCodeMode                  = "itemcodemode";

        const String ctTXTRootCategory                  = "@@POS.Transaction.RootCategoryText";                
        const String ctTXTFavouriteTitle                = "@@POS.Transaction.FavouriteTitle";
                
        const String ctDQYRetrieveItemList              = "EPOS.RetrieveItemList";

        // const String ctSETItemCodeMode                  = "POS.SystemItemCodeMode";

        public enum EntryType { Category, Item, Service, Cancel };
        public enum PriceMode { PriceShow, PriceHide }
        
        DataTable                       clItemList;        
        LanguageManager                 clLanguageManager;
        SettingManager                  clSettingManager;
        Dictionary<String, String>      clTransactionSetting;
        PriceMode                       clPriceMode;      
        bool                            clItemCodeMode;
        bool                            clStockOnlyMode;

        public CompositeFormInterface SCI_ParentForm { get; set; }

        public SubControlPOSItemPanel(PriceMode paPriceMode, bool paStockOnlyMode)
        {
            clPriceMode             = paPriceMode;
            clItemList              = null;
            clStockOnlyMode         = paStockOnlyMode;

            clLanguageManager       = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSettingManager        = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;
            clItemCodeMode          = General.ParseBoolean(clSettingManager.SystemConfig.GetData(ctKEYItemCodeMode), false);
            clTransactionSetting    = General.JSONDeserialize<Dictionary<String, String>>(clSettingManager.GetSettingValue(ctSETTransactionSetting));
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctSubControlPOSItemPanelStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctSubControlPOSItemPanelScript));
        }             
        
        private DataRow[] GetEntryList(EntryType paEntryType)
        {
            if (paEntryType == EntryType.Cancel)
                return (clItemList.AsEnumerable().Where(r => (r.Field<String>(ctCOLEntryType) == "ITEM") && (r.Field<String>(ctCOLStatus) == "CANCEL")).ToArray());
            else
                return (clItemList.AsEnumerable().Where(r => (r.Field<String>(ctCOLEntryType) == paEntryType.ToString().ToUpper()) && (r.Field<String>(ctCOLStatus) == "ACTIVE")).ToArray());
        }

        private void RenderContentList(ComponentController paComponentController, DataRow[] paDataRowList, EntryType paEntryType)
        {
            POSItemCatalogueRow  lcPOSItemCatalogueRow;

            lcPOSItemCatalogueRow = new POSItemCatalogueRow(null);

            for (int lcCount = 0; lcCount < paDataRowList.Length; lcCount++)
            {
                lcPOSItemCatalogueRow.Row = paDataRowList[lcCount];

                if ((paEntryType == EntryType.Category) || (!clStockOnlyMode) || (lcPOSItemCatalogueRow.EntryAttribute != "STATIC"))
                {
                    paComponentController.AddElementType(ComponentController.ElementType.Element);
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_EntryType, lcPOSItemCatalogueRow.EntryType.ToLower());
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_EntryAttribute, lcPOSItemCatalogueRow.EntryAttribute.ToLower());
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_ItemID, lcPOSItemCatalogueRow.ItemID.ToString());
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_Category, lcPOSItemCatalogueRow.Category.ToString());
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_Attribute, lcPOSItemCatalogueRow.Favourite ? "favourite" : "");
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_ItemText, lcPOSItemCatalogueRow.ItemName);

                    if (paEntryType != EntryType.Category)
                    {
                        paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_ItemCode, lcPOSItemCatalogueRow.ItemCode.ToUpper());
                        paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_MajorUnitName, lcPOSItemCatalogueRow.MajorUnitName);
                        paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_MinorUnitName, lcPOSItemCatalogueRow.MinorUnitName);
                        paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_UnitRelationship, lcPOSItemCatalogueRow.UnitRelationship.ToString());
                        paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_ItemStatus, lcPOSItemCatalogueRow.Status.ToLower());

                        if (clPriceMode == PriceMode.PriceShow)
                        {
                            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_MajorPrice, lcPOSItemCatalogueRow.MajorPrice.ToString(clSettingManager.BareCurrencyFormatString));
                            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_MinorPrice, lcPOSItemCatalogueRow.MinorPrice.ToString(clSettingManager.BareCurrencyFormatString));
                            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_MajorMSP, lcPOSItemCatalogueRow.MajorMSP.ToString(clSettingManager.BareCurrencyFormatString));
                            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_MinorMSP, lcPOSItemCatalogueRow.MinorMSP.ToString(clSettingManager.BareCurrencyFormatString));
                        }
                    }
                    paComponentController.RenderBeginTag(HtmlTag.A);

                    paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItemName);
                    paComponentController.RenderBeginTag(HtmlTag.Span);
                    paComponentController.Write(lcPOSItemCatalogueRow.ItemName);
                    paComponentController.RenderEndTag();

                    if (paEntryType != EntryType.Category)
                    {
                        paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItemInfo);
                        paComponentController.RenderBeginTag(HtmlTag.Span);

                        if ((clItemCodeMode) && (!String.IsNullOrWhiteSpace(lcPOSItemCatalogueRow.ItemCode)))
                        {
                            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItemCode);
                            paComponentController.RenderBeginTag(HtmlTag.Span);
                            paComponentController.Write(clLanguageManager.ConvertNumber(lcPOSItemCatalogueRow.ItemCode));
                            paComponentController.RenderEndTag();
                        }

                        if (clPriceMode == PriceMode.PriceShow)
                        {
                            if (lcPOSItemCatalogueRow.MajorPrice > 0)
                            {
                                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSMajorPrice);
                                paComponentController.RenderBeginTag(HtmlTag.Span);
                                paComponentController.Write(clLanguageManager.ConvertNumber(lcPOSItemCatalogueRow.MajorPrice.ToString(clSettingManager.CurrencyFormatString)));
                                paComponentController.RenderEndTag();
                            }


                            if (lcPOSItemCatalogueRow.MinorPrice > 0)
                            {
                                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSMinorPrice);
                                paComponentController.RenderBeginTag(HtmlTag.Span);
                                paComponentController.Write(clLanguageManager.ConvertNumber(lcPOSItemCatalogueRow.MinorPrice.ToString(clSettingManager.CurrencyFormatString)));
                                paComponentController.RenderEndTag();
                            }
                        }

                        paComponentController.RenderEndTag();
                    }

                    paComponentController.RenderEndTag();
                }
            }            
        }

        private void RenderBlock(ComponentController paComponentController, EntryType paEntryType)
        {
            DataRow[] lcDataRowList;

            lcDataRowList = GetEntryList(paEntryType);

            paComponentController.AddElementType(ComponentController.ElementType.Block);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, paEntryType.ToString().ToLower());            
            paComponentController.RenderBeginTag(HtmlTag.Div);
                        
            RenderContentList(paComponentController, lcDataRowList, paEntryType);

            paComponentController.RenderEndTag();
        }

        private void RenderListContainer(ComponentController paComponentController)
        {            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSBlockContainer);
            paComponentController.RenderBeginTag(HtmlTag.Div);
                        
            RenderBlock(paComponentController, EntryType.Category);
            RenderBlock(paComponentController, EntryType.Item);
            if (!clStockOnlyMode) RenderBlock(paComponentController, EntryType.Service);            
                
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTerminatingBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderCategoryTitle(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Title);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCategoryTitle);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderButton(paComponentController, ctCMDRootCategory, Fontawesome.home);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTitleText);
            paComponentController.RenderBeginTag(HtmlTag.Span);            
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSFavouriteTitle);
            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(clLanguageManager.GetText(ctTXTFavouriteTitle));
            paComponentController.RenderEndTag();

            RenderButton(paComponentController, ctCMDParentCategory, Fontawesome.level_up);

            paComponentController.RenderEndTag();
        }

        private void RenderComponentContainer(ComponentController paComponentController)
        {            
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSComponentContainer);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            if (clItemList != null)
            {
                RenderCategoryTitle(paComponentController);
                RenderListContainer(paComponentController);
            }

            paComponentController.RenderEndTag();
        }

        private void RenderButton(ComponentController paComponentController, String paCommand,Fontawesome paFontAwesome)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, paCommand);            
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(ComponentController.UnicodeStr((int)paFontAwesome));
            paComponentController.RenderEndTag();
        }

        private void RenderHeaderBar(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSHeaderBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderButton(paComponentController, ctCMDList, Fontawesome.list);
            RenderButton(paComponentController, ctCMDFavourite, Fontawesome.star);
            RenderButton(paComponentController, ctCMDEditFavourite, Fontawesome.star);
            RenderButton(paComponentController, ctCMDClose, Fontawesome.remove);

            if (clItemCodeMode) RenderButton(paComponentController, ctCMDSwitchView, Fontawesome.space);            

            paComponentController.RenderEndTag();
        }

        private void RenderBrowserMode(ComponentController paComponentController)
        {            
            IncludeExternalLinkFiles(paComponentController);
            
            clItemList = DynamicQueryManager.GetInstance().GetDataTableResult(ctDQYRetrieveItemList);
            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSubControlPOSItemPanelComposite);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "itempanel");
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Mode, clPriceMode.ToString().ToLower());
            // paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Edition, clEdition.ToString().ToLower());
            paComponentController.AddElementType(ComponentController.ElementType.Composite);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DisplayMode, clTransactionSetting.GetData(ctKEYItemPanelDisplayMode, "price").ToLower());
            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Mode, "list");
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Root, clLanguageManager.GetText(ctTXTRootCategory));
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSubControlPOSItemPanel);                  
            paComponentController.AddElementType(ComponentController.ElementType.Panel);            
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderHeaderBar(paComponentController);
            RenderComponentContainer(paComponentController);            

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
            if (paRenderMode == "cancelitemblock")
            {
                clItemList = DynamicQueryManager.GetInstance().GetDataTableResult(ctDQYRetrieveItemList);
                RenderBlock(paComponentController, EntryType.Cancel);
            }
            else RenderBrowserMode(paComponentController);           
        }

        protected override void Render(HtmlTextWriter paHtmlTextWriter)
        {
            if (!DesignMode) RenderBrowserMode(new ComponentController(paHtmlTextWriter));
            else RenderDesignMode(new ComponentController(paHtmlTextWriter));
        }
    }
}

