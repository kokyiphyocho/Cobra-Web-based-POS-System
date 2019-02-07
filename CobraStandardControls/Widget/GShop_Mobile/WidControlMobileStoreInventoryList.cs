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
    public class WidControlMobileStoreInventoryList : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlMobileStoreInventoryListStyle     = "WidControlMobileStoreInventoryList.css";
        protected const String ctWidControlMobileStoreInventoryListScript    = "WidControlMobileStoreInventoryList.js";

        protected const String ctJQueryUIScript          = "jquery-ui.min.js";
        protected const String ctJQueryUIStyle           = "jquery-ui.min.css";

        protected const String ctSumoSelectScript        = "jquery.sumoselect.min.js";
        protected const String ctSumoSelectStyle         = "sumoselect.css";

        protected const String ctJQueryUIFunctionScript  = "jquery-ui.functions.js";

        const String ctCLSWidControlMobileStoreInventoryList     = "WidControlMobileStoreInventoryList";
        const String ctCLSFilterSelection               = "FilterSelection";
        const String ctCLSFieldContainer                = "FieldContainer";
        const String ctCLSDescriptionDiv                = "DescriptionDiv";
        const String ctCLSField                         = "Field";
        const String ctCLSLabel                         = "Label";        
        const String ctCLSButtonDiv                     = "ButtonDiv";

        const String ctCMDDeleteItem                    = "@cmd%deleteitem";
        const String ctCMDEditItem                      = "@cmd%edititem";

        const String ctICOEditPencil                    = "edit_pencil.png";
        const String ctICOCrossButton                   = "cross_button.png";

        const String ctPRMChildForm                     = "FormMobileStoreAddAdjustInventory,EntryID::$ENTRYID;;";

        public CompositeFormInterface SCI_ParentForm { get; set; }

        public String SC_GroupLabel             { get; set; }
        public String SC_ActiveGroup            { get; set; }  
        
        public String SC_GroupingColumnName     { get; set; }
        public String SC_KeyColumnName          { get; set; }
        public String SC_DescriptionColumnName  { get; set; }
        
        public String SC_UpdateButtonText       { get; set; }
        public String SC_BackButtonText         { get; set; }
        public String SC_ConfirmButtonText      { get; set; }
        
        
        public DataTable SC_DataTable           { get; set; }
        
        public WidControlMobileStoreInventoryList()
        {
            SC_GroupLabel               = String.Empty;
            SC_ActiveGroup              = String.Empty;            

            SC_GroupingColumnName       = String.Empty;
            SC_KeyColumnName            = String.Empty;
            SC_DescriptionColumnName    = String.Empty;
            

            SC_DataTable                = null;
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

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_Mobile, ctWidControlMobileStoreInventoryListStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_Mobile, ctWidControlMobileStoreInventoryListScript));
        }

        private void RenderFilterBox(ComponentController paComponentController)
        {
            List<String>    lcGroupOptions;
            
            if ((SC_GroupingColumnName != null) && ((lcGroupOptions = General.GetDistinctColumnValue(SC_DataTable, SC_GroupingColumnName)).Count > 0))
            {
                lcGroupOptions = lcGroupOptions.OrderBy(l => l).ToList();

                if ((String.IsNullOrWhiteSpace(SC_ActiveGroup)) || (!lcGroupOptions.Contains(SC_ActiveGroup, StringComparer.OrdinalIgnoreCase)))
                    SC_ActiveGroup = lcGroupOptions[0];

                paComponentController.AddElementType(ComponentController.ElementType.FilterControl);
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_PluginMode, "sumoselect");
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, SC_GroupingColumnName);

                paComponentController.RenderBeginTag(HtmlTag.Select);

                SC_ActiveGroup = SC_ActiveGroup.Trim();

                for (int lcCount = 0; lcCount < lcGroupOptions.Count; lcCount++)
                {
                    lcGroupOptions[lcCount] = lcGroupOptions[lcCount].Trim();
                    
                    if (String.Equals(lcGroupOptions[lcCount], SC_ActiveGroup, StringComparison.OrdinalIgnoreCase)) 
                        paComponentController.AddAttribute(HtmlAttribute.Selected, "true");

                    paComponentController.AddAttribute(HtmlAttribute.Value, lcGroupOptions[lcCount]);
                    paComponentController.RenderBeginTag(HtmlTag.Option);
                    paComponentController.Write(lcGroupOptions[lcCount]);
                    paComponentController.RenderEndTag();
                }

                paComponentController.RenderEndTag();
            }
        }

        private void RenderFilterDiv(ComponentController paComponentController)
        {            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSFilterSelection);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            RenderFilterBox(paComponentController);
            paComponentController.RenderEndTag();
        }       

        private void RenderButtonDiv(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonDiv);
            
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDEditItem);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOEditPencil));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDDeleteItem);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOCrossButton));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderDescriptionDiv(ComponentController paComponentController, MetaDataRow paMetaDataRow)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSDescriptionDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(paMetaDataRow.ActiveData.GetMultiColumnData(SC_DescriptionColumnName, String.Empty));
            paComponentController.RenderEndTag();
         
            paComponentController.RenderEndTag();
        }

        private void RenderField(ComponentController paComponentController, MetaDataRow paMetaDataRow)
        {            
            String lcGrouping;

            if (SC_GroupingColumnName != null)
            {
                lcGrouping = paMetaDataRow.ActiveData.GetData(SC_GroupingColumnName, String.Empty);
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Group, lcGrouping);
            }           
            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DataID, General.GetFormattedDBData(paMetaDataRow.ActiveData.GetData(SC_KeyColumnName, String.Empty)));
            paComponentController.AddElementType(ComponentController.ElementType.Element);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSField);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderDescriptionDiv(paComponentController, paMetaDataRow);
            RenderButtonDiv(paComponentController);
                        
            paComponentController.RenderEndTag();
        }

        protected void RenderFieldContainer(ComponentController paComponentController)
        {
            MetaDataRow         lcMetaDataRow;

            if ((SC_DataTable != null) && (SC_KeyColumnName != null) && (SC_DescriptionColumnName != null))
            {
                paComponentController.AddElementType(ComponentController.ElementType.Container);
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSFieldContainer);
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_KeyColumnName, SC_KeyColumnName);                
                paComponentController.RenderBeginTag(HtmlTag.Div);

                for (int lcCount = 0; lcCount < SC_DataTable.Rows.Count; lcCount++)
                {
                    lcMetaDataRow = new MetaDataRow(SC_DataTable.Rows[lcCount]);
                    RenderField(paComponentController, lcMetaDataRow);
                }
                
                paComponentController.RenderEndTag();
            }
        }        

        private void VerifyColumnNames()
        {
            if (SC_DataTable != null)
            {
                SC_GroupingColumnName       = SC_DataTable.Columns.Contains(SC_GroupingColumnName) ? SC_GroupingColumnName : null;
                SC_KeyColumnName            = SC_DataTable.Columns.Contains(SC_KeyColumnName) ? SC_KeyColumnName : null;            
            }            
        }
        
        protected void RenderBrowserMode(ComponentController paComponentController)
        {
            IncludeExternalLinkFiles(paComponentController);
            VerifyColumnNames();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Template, ctPRMChildForm);  
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ControlMode, "standard");
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlMobileStoreInventoryList);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            RenderFilterDiv(paComponentController);            
            RenderFieldContainer(paComponentController);                        
            paComponentController.RenderEndTag();
        }

        protected void RenderDesignMode(ComponentController paComponentController, bool paContentOnly = false)
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
