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
    public class WidControlPOSStaticCostList : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlPOSStaticCostListStyle = "WidControlPOSStaticCostList.css";
        protected const String ctWidControlPOSStaticCostListScript = "WidControlPOSStaticCostList.js";
        
        const String ctCLSWidControlPOSStaticCostList = "WidControlPOSStaticCostList";
        const String ctCLSContainer             = "Container";
        const String ctCLSCategoryBlock         = "CategoryBlock";
                
        const String ctCLSCategoryTitle         = "CategoryTitle";        
        const String ctCLSTitleText             = "TitleText";
        const String ctCLSHomeButtonDiv         = "HomeButtonDiv";
        const String ctCLSUpButtonDiv           = "UpButtonDiv";

        const String ctCLSItemListBlock         = "ItemListBlock";        
        const String ctCLSItemRow               = "ItemRow";
        
        const String ctCMDRootCategory          = "@cmd%rootcategory";
        const String ctCMDUpCategory            = "@cmd%upcategory";
        const String ctCMDShowCategory          = "@cmd%showcategory";
        const String ctCMDSetFocus              = "@cmd%setfocus";
        
        const String ctCMDEdit                  = "@cmd%edit";
        const String ctCMDDelete                = "@cmd%delete";

        //const String ctSETAllowStaffInventoryFeature   = "POS.AllowStaffInventoryFeature";
        //const String ctSETSystemItemCodeMode    = "POS.SystemItemCodeMode";

        const String ctSETStaffPermissionSetting    = "POS.StaffPermissionSetting";

        const String ctKEYItemCodeMode              = "itemcodemode";
        const String ctKEYAllowInventoryFeature     = "allowinventoryfeature";

        const String ctCLSButtonPanel           = "ButtonPanel";
        const String ctCLSEditButtonDiv         = "EditButtonDiv";
        const String ctCLSDeleteButtonDiv       = "DeleteButtonDiv";
             
        const String ctICOEditButton             = "edit_pencil.png";
        const String ctICODeleteButton           = "cross_button.png";

        const String ctCOLEntryType              = "EntryType";
        const String ctCOLCategory               = "Category";
        const String ctCOLCost                   = "Cost";

        const String ctENTCategory               = "CATEGORY";

        const String ctDash                      = "-";

        const String ctTPLStaticCostEditor      = "FormPOSStaticCostEditor,FPM_ITEMID::$ITEMID";

        const String ctTXTRootCategoryName       = "@@POS.ItemList.RootCategoryName";
        
        public CompositeFormInterface SCI_ParentForm    { get; set; }    
                
        DataTable   clItemList;
        bool        clAdminUser;
        bool        clAllowStaffInventoryFeature;
        bool        clSystemItemCodeMode;
        
        LanguageManager                 clLanguageManager;
        SettingManager                  clSettingManager;
        Dictionary<String, String>      clStaffPermissionSetting;
        
        public WidControlPOSStaticCostList()
        {
            clItemList                  = null;

            clLanguageManager           = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSettingManager             = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;            
            clAdminUser                 = ApplicationFrame.GetInstance().ActiveSessionController.User.IsAdminUser();
            clStaffPermissionSetting    = General.JSONDeserialize<Dictionary<String, String>>(clSettingManager.GetSettingValue(ctSETStaffPermissionSetting));

            clAllowStaffInventoryFeature    = General.ParseBoolean(clStaffPermissionSetting.GetData(ctKEYAllowInventoryFeature), false);            
            clSystemItemCodeMode            = General.ParseBoolean(clSettingManager.SystemConfig.GetData(ctKEYItemCodeMode),false);            
        }
       
        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSStaticCostListStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSStaticCostListScript));
        }        

        private void RenderButtonPanel(ComponentController paComponentController)
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
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICODeleteButton));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderItemRow(ComponentController paComponentController, POSItemCatalogueRow paItemRow)
        {
            Decimal         lcCost;

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItemRow);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, paItemRow.EntryAttribute.ToLower().Trim() + paItemRow.EntryType.ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DataID, paItemRow.ItemID.ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, paItemRow.EntryType == ctENTCategory ? ctCMDShowCategory : ctCMDSetFocus);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_UnitRelationship, paItemRow.UnitRelationship.ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_MajorUnitName, paItemRow.MajorUnitName);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_MinorUnitName, paItemRow.MinorUnitName);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_ItemText, paItemRow.ItemName);
            paComponentController.AddElementType(ComponentController.ElementType.Row);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "itemname");
            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(paItemRow.ItemName);
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "cost");
            paComponentController.RenderBeginTag(HtmlTag.Span);

            if ((lcCost = Convert.ToDecimal(paItemRow.Row[ctCOLCost])) > 0)
                paComponentController.Write(clLanguageManager.ConvertNumber(lcCost.ToString(clSettingManager.CurrencyFormatString)));
            else
                paComponentController.Write(ctDash);

            paComponentController.RenderEndTag();

            if ((clAllowStaffInventoryFeature) || (clAdminUser)) 
                RenderButtonPanel(paComponentController);

            paComponentController.RenderEndTag();
        }

        private void RenderItemList(ComponentController paComponentController, POSItemCatalogueRow paCategoryRow)
        {
            POSItemCatalogueRow lcItemCatalogueRow;
            DataRow[] lcItemList;

            lcItemList = GetItemList(paCategoryRow == null ? 0 : paCategoryRow.ItemID);

            paComponentController.AddElementType(ComponentController.ElementType.List);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItemListBlock);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            
            lcItemCatalogueRow = new POSItemCatalogueRow(null);

            for (int lcCount = 0; lcCount < lcItemList.Length; lcCount++)
            {
                lcItemCatalogueRow.Row = lcItemList[lcCount];
                RenderItemRow(paComponentController, lcItemCatalogueRow);
            }
            

            paComponentController.RenderEndTag();
        }
      
        private void RenderCategoryTitle(ComponentController paComponentController, POSItemCatalogueRow paCategoryRow)
        {            
            if (paCategoryRow != null)
            {
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCategoryTitle);
                paComponentController.AddElementType(ComponentController.ElementType.Title);                                
                paComponentController.RenderBeginTag(HtmlTag.Div);

                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDRootCategory);                
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSHomeButtonDiv);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.home));
                paComponentController.RenderEndTag();

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTitleText);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(paCategoryRow.ItemName);
                paComponentController.RenderEndTag();

                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDUpCategory);                
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSUpButtonDiv);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.level_up));
                paComponentController.RenderEndTag();
                
                paComponentController.RenderEndTag();                
            }
            else
            {
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCategoryTitle);
                paComponentController.AddElementType(ComponentController.ElementType.Title);                            
                paComponentController.RenderBeginTag(HtmlTag.Div);

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTitleText);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(clLanguageManager.GetText(ctTXTRootCategoryName));
                paComponentController.RenderEndTag();
                
                paComponentController.RenderEndTag();
            }
        }

        private void RenderCategoryBlock(ComponentController paComponentController, POSItemCatalogueRow paCategoryRow)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Block);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCategoryBlock);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Group, paCategoryRow != null ? paCategoryRow.ItemID.ToString() : "0");
            
            if (paCategoryRow != null)
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Parent, paCategoryRow.Category.ToString());

            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderCategoryTitle(paComponentController, paCategoryRow);
            RenderItemList(paComponentController, paCategoryRow);

            paComponentController.RenderEndTag();            
        }

        private DataRow[] GetCategoryList()        
        {
            return(clItemList.AsEnumerable().Where(r => r.Field<String>(ctCOLEntryType) == "CATEGORY").ToArray());
        }

        private DataRow[] GetItemList(int paCategory)
        {
            return (clItemList.AsEnumerable().Where(r => r.Field<int>(ctCOLCategory) == paCategory).OrderBy(r => r.Field<String>(ctCOLEntryType)).ToArray());
        }

        private void RenderContainerContent(ComponentController paComponentController)
        {
            DataRow[]           lcCategoryRows;
            POSItemCatalogueRow lcItemCatalogueRow;

            lcCategoryRows = GetCategoryList();

            RenderCategoryBlock(paComponentController, null);

            lcItemCatalogueRow = new POSItemCatalogueRow(null);

            for (int lcCount = 0; lcCount < lcCategoryRows.Length; lcCount++)
            {
                lcItemCatalogueRow.Row = lcCategoryRows[lcCount];
                RenderCategoryBlock(paComponentController, lcItemCatalogueRow);
            }
        }

        private void RenderContainer(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSContainer);
            paComponentController.RenderBeginTag(HtmlTag.Div);

        //    RenderContainerContent(paComponentController);
                
            paComponentController.RenderEndTag(); // Container
        }

        protected void RenderBrowserMode(ComponentController paComponentController)
        {
            IncludeExternalLinkFiles(paComponentController);

            if ((clAllowStaffInventoryFeature) || (clAdminUser))            
                SCI_ParentForm.RenderToolBar(paComponentController);
            else paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Attribute, "readonly");

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Template, ctTPLStaticCostEditor);
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSStaticCostList);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            
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
            if (paRenderMode == null) RenderBrowserMode(paComponentController);
            if (paRenderMode == "itemlistcontent")
            {                
                clItemList = ApplicationFrame.GetInstance().ActiveFormInfoManager.RunRetrieveQuery();
                RenderContainerContent(paComponentController);
            }
        }

        protected override void Render(HtmlTextWriter paHtmlTextWriter)
        {
            if (!DesignMode) RenderBrowserMode(new ComponentController(paHtmlTextWriter));
            else RenderDesignMode(new ComponentController(paHtmlTextWriter));
        }
    }
}

