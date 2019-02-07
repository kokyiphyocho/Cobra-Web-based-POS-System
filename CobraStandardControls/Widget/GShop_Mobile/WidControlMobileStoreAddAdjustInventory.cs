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

namespace CobraStandardControls
{
    public class WidControlMobileStoreAddAdjustInventory : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlMobileStoreAddAdjustInventoryStyle     = "WidControlMobileStoreAddAdjustInventory.css";
        protected const String ctWidControlMobileStoreAddAdjustInventoryScript    = "WidControlMobileStoreAddAdjustInventory.js";

        protected const String ctJQueryUIScript             = "jquery-ui.min.js";
        protected const String ctJQueryUIStyle              = "jquery-ui.min.css";

        protected const String ctSumoSelectScript           = "jquery.sumoselect.min.js";
        protected const String ctSumoSelectStyle            = "sumoselect.css";
        
        protected const String ctJQueryUIFunctionScript     = "jquery-ui.functions.js";

        const String ctCLSWidControlMobileStoreAddAdjustInventory = "WidControlMobileStoreAddAdjustInventory";
        const String ctCLSInputRow                          = "InputRow";
        const String ctCLSLabel                             = "Label";        
        const String ctCLSInputDiv                          = "InputDiv";
        
        const String ctCLSButtonPanel                       = "ButtonPanel";
        const String ctCLSAddButton                         = "AddButton";        
        const String ctCLSCloseButton                       = "CloseButton";

        const String ctDEFAddButtonText                     = "Add";
        const String ctDEFUpdateButtonText                  = "Update";
        const String ctDEFCloseButtonText                   = "Close";

        const String ctCOLEntryID                           = "EntryID";
        const String ctCOLManufacturer                      = "Manufacturer";
        const String ctCOLProductFullName                   = "ProductFullName";
        const String ctCOLProductUID                        = "ProductUID";
        const String ctCOLKind                              = "Kind";
        const String ctCOLCategory                          = "Category";
        const String ctCOLDescription1                      = "Description.Description1";
        const String ctCOLDescription2                      = "Description.Description2";
        const String ctCOLGroupName                         = "GroupName";
        const String ctCOLSortKey                           = "SortKey";
        const String ctCOLTag                               = "Tag";
        const String ctCOLPrice                             = "Price";

        const String ctOther                                = "OTHER";
        const String ctOtherValue                           = "[OTHER]";
        const String ctWildGroup                            = "*";

        const String ctLBLManufacturer                      = "Manufacturer";
        const String ctLBLProductName                       = "Product Name";
        const String ctLBLCategory                          = "Type";
        const String ctLBLNetwork                           = "Network";
        const String ctLBLDescription1                      = "Description Line 1";
        const String ctLBLDescription2                      = "Description Line 2";
        const String ctLBLGroupName                         = "Group Name";
        const String ctLBLSortKey                           = "Sort Key";
        const String ctLBLTag                               = "Tag";
        const String ctLBLPrice                             = "Price";

        const String ctDEFCategorySelection                 = "MOBILE PHONE";
        const String ctDEFManufacturerSelection             = "SAMSUNG";
        const String ctDEFProductNameSelection              = "[OTHER]";
        const String ctDEFNetworkSelection                  = "GSM";

        const String ctCLSSelectionControl                  = "SelectionControl";

        const String ctDEFNetworkTypes                      = "<!NetworkList::GSM%%CDMA 450%%CDMA 800!>";

        const String ctQUEGetUniversalProductList           = "GetUniversalProductList";        

        const String ctCMDUpdate                            = "@cmd%update";
        const String ctCMDClose                             = "@cmd%close";

        public enum SelectionPanelType          { Manufacturer, ProductName, Category, NetworkType }

        public CompositeFormInterface SCI_ParentForm { get; set; }

        public String SC_AddButtonText          { get; set; }
        public String SC_UpdateButtonText       { get; set; }
        public String SC_CloseButtonText        { get; set; }

        public String SC_ManufacturerLabel      { get; set; }
        public String SC_ProductNameLabel       { get; set; }
        public String SC_CategoryLabel          { get; set; }
        public String SC_NetworkLabel           { get; set; }
        public String SC_Description1Label      { get; set; }
        public String SC_Description2Label      { get; set; }
        public String SC_PriceLabel             { get; set; }
        public String SC_GroupNameLabel         { get; set; }
        public String SC_SortKeyLabel           { get; set; }
        public String SC_TagLabel               { get; set; }

        public String SC_NetworkTypes           { get; set; }

        MetaDataRow                 clMetaDataRow;
        DataTable                   clUniversalProductList;

        Dictionary<String, String>  clNetworkTypeList;
        Dictionary<String, String>  clManufacturerList;
        Dictionary<String, String>  clCategoryList;
        Dictionary<String, String>  clProductNameList;
        
        public WidControlMobileStoreAddAdjustInventory()
        {
            SC_AddButtonText        = ctDEFAddButtonText;
            SC_UpdateButtonText     = ctDEFUpdateButtonText;
            SC_CloseButtonText      = ctDEFCloseButtonText;

            SC_ManufacturerLabel        = ctLBLManufacturer;
            SC_ProductNameLabel         = ctLBLProductName;
            SC_CategoryLabel            = ctLBLCategory;
            SC_NetworkLabel             = ctLBLNetwork;
            SC_Description1Label        = ctLBLDescription1;
            SC_Description2Label        = ctLBLDescription2;
            SC_GroupNameLabel           = ctLBLGroupName;
            SC_SortKeyLabel             = ctLBLSortKey;
            SC_TagLabel                 = ctLBLTag;
            SC_PriceLabel               = ctLBLPrice;

            clMetaDataRow   = null;
            clUniversalProductList      = null;            
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetFoundationStyleSheetUrl(ctJQueryUIStyle));
            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetFoundationStyleSheetUrl(ctSumoSelectStyle));

            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetFoundationScriptUrl(ctJQueryUIScript));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetFoundationScriptUrl(ctSumoSelectScript));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetFoundationScriptUrl(ctJQueryUIFunctionScript));

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_Mobile, ctWidControlMobileStoreAddAdjustInventoryStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_Mobile, ctWidControlMobileStoreAddAdjustInventoryScript));
        }

        private void RenderButtonPanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSAddButton);
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDUpdate);
            paComponentController.RenderBeginTag(HtmlTag.A);

            if (clMetaDataRow.ActiveRow == null)
                paComponentController.Write(SC_AddButtonText);
            else
                paComponentController.Write(SC_UpdateButtonText);

            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCloseButton);
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDClose);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(SC_CloseButtonText);
            paComponentController.RenderEndTag();        

            paComponentController.RenderEndTag();
        }

        private Dictionary<String, String> GetNetworkTypeDictionary()
        {
            MetaDataElement             lcNetworkTypes;
            Dictionary<String, String>  lcNetworkTypeDictionary;

            lcNetworkTypeDictionary = new Dictionary<string, string>();

            if (MetaDataBlockCollection.IsMetaBlockString(SC_NetworkTypes))
                lcNetworkTypes = (new MetaDataBlockCollection(SC_NetworkTypes))[0][0];
            else
                lcNetworkTypes = (new MetaDataBlockCollection(ctDEFNetworkTypes))[0][0];

            for (int lcCount = 0; lcCount < lcNetworkTypes.ValueCount; lcCount++)
                lcNetworkTypeDictionary.Add(lcNetworkTypes[lcCount], lcNetworkTypes[lcCount]);

            return (lcNetworkTypeDictionary);
        }

        private void RetrieveData()
        {   
            clMetaDataRow = new MetaDataRow(ApplicationFrame.GetInstance().ActiveFormInfoManager.RunRetrieveRow());   
            
            clUniversalProductList = DynamicQueryManager.GetInstance().GetDataTableResult(ctQUEGetUniversalProductList);

            clManufacturerList  = General.GetDistinctColumnValue(clUniversalProductList, ctCOLManufacturer).ToDictionary(x => x.ToUpper(), x => x.ToUpper());
            clCategoryList      = General.GetDistinctColumnValue(clUniversalProductList, ctCOLCategory).ToDictionary(x => x.ToUpper(), x => x.ToUpper());
            clProductNameList   = clUniversalProductList.AsEnumerable()
                                  .ToDictionary(x => x.Field<String>(ctCOLCategory).ToUpper() + "," + x.Field<String>(ctCOLManufacturer).ToUpper() + ";" + x.Field<String>(ctCOLProductUID).ToUpper(), 
                                                x => x.Field<String>(ctCOLProductFullName));
            clNetworkTypeList  = GetNetworkTypeDictionary();

            clManufacturerList.Add(ctOtherValue, ctOther);
            clCategoryList.Add(ctOtherValue, ctOther);
            clProductNameList.Add(ctOtherValue, ctOther);
            clNetworkTypeList.Add(ctOtherValue, ctOther);
        }

        private void GetActiveValue(SelectionPanelType paSelectionPanelType,out String paActiveValue, out String paActiveData)
        {
            paActiveValue   = String.Empty;
            paActiveData    = String.Empty;            

            switch(paSelectionPanelType)
            {
                case SelectionPanelType.Category :
                    {                        
                        if (clMetaDataRow.ActiveRow != null)
                        {
                            paActiveValue = clMetaDataRow.ActiveData.GetData(ctCOLCategory, String.Empty);
                            paActiveData  = paActiveValue;
                        }
                        else
                        {
                            paActiveValue = ctDEFCategorySelection;
                            paActiveData = paActiveValue;
                        }
                        break;
                    }

                case SelectionPanelType.Manufacturer:
                    {
                        if (clMetaDataRow.ActiveRow != null)
                        {
                            paActiveValue = clMetaDataRow.ActiveData.GetData(ctCOLManufacturer, String.Empty);
                            paActiveData = paActiveValue;
                        }
                        else
                        {
                            paActiveValue = ctDEFManufacturerSelection;
                            paActiveData = paActiveValue;
                        }
                        break;
                    }

                case SelectionPanelType.ProductName:
                    {

                        if (clMetaDataRow.ActiveRow != null)
                        {
                            paActiveValue = clMetaDataRow.ActiveData.GetData(ctCOLProductUID, String.Empty);

                            if (paActiveValue != ctOtherValue)
                                paActiveValue = clMetaDataRow.ActiveData.GetData(ctCOLCategory, String.Empty) + "," + clMetaDataRow.ActiveData.GetData(ctCOLManufacturer, String.Empty) + ";" + paActiveValue;

                            if (clProductNameList.Keys.Contains(paActiveValue))
                                paActiveData = clProductNameList[paActiveValue];
                        }
                        else
                        {
                            paActiveValue = ctDEFProductNameSelection;
                            paActiveData = clProductNameList[ctDEFProductNameSelection];
                        }
                        break;
                    }

                case SelectionPanelType.NetworkType:
                    {
                        if (clMetaDataRow.ActiveRow != null)
                        {
                            paActiveValue = clMetaDataRow.ActiveData.GetData(ctCOLKind, String.Empty);
                            paActiveData = paActiveValue;
                        }
                        else
                        {
                            paActiveValue = ctDEFNetworkSelection;
                            paActiveData = paActiveValue;
                        }
                        break;
                    }
            }
        }

        protected void RenderSelectionControl(ComponentController paComponentController, String paColumnName, Dictionary<String, String> paDictionary, SelectionPanelType paSelectionPanelType)
        {
            String lcActiveValue;
            String lcActiveData;

            GetActiveValue(paSelectionPanelType, out lcActiveValue, out lcActiveData);
            
            paComponentController.AddAttribute(HtmlAttribute.Value, lcActiveValue.ToUpper());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, paSelectionPanelType.ToString().ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, paColumnName.ToLower());
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSelectionControl);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_OriginalValue, lcActiveValue);
            paComponentController.AddAttribute(HtmlAttribute.ReadOnly, "readonly");
            paComponentController.AddAttribute(HtmlAttribute.Value, lcActiveData);
            paComponentController.AddAttribute(HtmlAttribute.Type, "text");
            paComponentController.RenderBeginTag(HtmlTag.Input);
            
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        protected void RenderSelectionControlSection(ComponentController paComponentController, String paLabel, String paColumnName, Dictionary<String, String> paDictionary, SelectionPanelType paSelectionPanelType)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInputRow);
            paComponentController.AddElementType(ComponentController.ElementType.InputRow);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(paLabel);
            paComponentController.RenderEndTag();
            paComponentController.RenderEndTag();

            RenderSelectionControl(paComponentController, paColumnName, paDictionary, paSelectionPanelType);

            paComponentController.RenderEndTag();
            
        }

        protected void RenderOtherInfoSection(ComponentController paComponentController, String paColumnName, String paLabel, bool paMandatory, bool paNumeric)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInputRow);
            paComponentController.AddElementType(ComponentController.ElementType.InputRow);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(paLabel);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();


            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInputDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, paColumnName);
            if (paMandatory) paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Mandatory, "true");
            if (paNumeric) paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_InputMode, "number");
            paComponentController.AddAttribute(HtmlAttribute.Value, clMetaDataRow.ActiveData.GetData(paColumnName, String.Empty));
            paComponentController.AddAttribute(HtmlAttribute.Type, "text");
            paComponentController.RenderBeginTag(HtmlTag.Input);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();            
        }

        protected void RenderContainer(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderSelectionControlSection(paComponentController, SC_CategoryLabel, ctCOLCategory, clCategoryList, SelectionPanelType.Category);
            RenderSelectionControlSection(paComponentController, SC_ManufacturerLabel, ctCOLManufacturer, clManufacturerList, SelectionPanelType.Manufacturer);
            RenderSelectionControlSection(paComponentController, SC_ProductNameLabel, ctCOLProductFullName, clProductNameList, SelectionPanelType.ProductName);
            RenderSelectionControlSection(paComponentController, SC_NetworkLabel, ctCOLKind, clNetworkTypeList, SelectionPanelType.NetworkType);
            
            RenderOtherInfoSection(paComponentController, ctCOLDescription1.ToLower(), SC_Description1Label, true, false);
            RenderOtherInfoSection(paComponentController, ctCOLDescription2.ToLower(), SC_Description2Label, false, false);
            RenderOtherInfoSection(paComponentController, ctCOLGroupName.ToLower(), SC_GroupNameLabel, false, false);
            RenderOtherInfoSection(paComponentController, ctCOLSortKey.ToLower(), SC_SortKeyLabel, false, false);
            RenderOtherInfoSection(paComponentController, ctCOLTag.ToLower(), SC_TagLabel, false, false);
            RenderOtherInfoSection(paComponentController, ctCOLPrice.ToLower(), SC_PriceLabel, true, true);
            

            paComponentController.RenderEndTag();
        }

        protected void RenderSelectionPanel(ComponentController paComponentController, SelectionPanelType paSelectionPanelType, String paMode, String paTitle, Dictionary<String, String> paDictionary)
        {
            SubControlSelectionPanel lcSubControlSelectionPanel;

            lcSubControlSelectionPanel = new SubControlSelectionPanel(paSelectionPanelType.ToString().ToLower(), paMode, paTitle, paDictionary);
            lcSubControlSelectionPanel.RenderChildMode(paComponentController);
        }
    
        protected void RenderBrowserMode(ComponentController paComponentController)
        {  
            RetrieveData();

            IncludeExternalLinkFiles(paComponentController);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DataID, clMetaDataRow.ActiveData.GetData(ctCOLEntryID,"-1"));
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlMobileStoreAddAdjustInventory);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderContainer(paComponentController);

            RenderButtonPanel(paComponentController);
            
            RenderSelectionPanel(paComponentController, SelectionPanelType.Category, "normal", SC_CategoryLabel, clCategoryList);
            RenderSelectionPanel(paComponentController, SelectionPanelType.Manufacturer, "normal", SC_ManufacturerLabel, clManufacturerList);
            RenderSelectionPanel(paComponentController, SelectionPanelType.ProductName, "wide", SC_ProductNameLabel, clProductNameList);
            RenderSelectionPanel(paComponentController, SelectionPanelType.NetworkType, "normal", SC_NetworkLabel, clNetworkTypeList);

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

