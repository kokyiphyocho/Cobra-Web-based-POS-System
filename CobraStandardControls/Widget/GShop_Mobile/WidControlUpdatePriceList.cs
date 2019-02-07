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
    public class WidControlUpdatePriceList : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlUpdatePriceListStyle     = "WidControlUpdatePriceList.css";
        protected const String ctWidControlUpdatePriceListScript    = "WidControlUpdatePriceList.js";

        protected const String ctJQueryUIScript          = "jquery-ui.min.js";
        protected const String ctJQueryUIStyle           = "jquery-ui.min.css";

        protected const String ctSumoSelectScript        = "jquery.sumoselect.min.js";
        protected const String ctSumoSelectStyle         = "sumoselect.css";

        protected const String ctJQueryUIFunctionScript  = "jquery-ui.functions.js";

        const String ctCLSWidControlUpdatePriceList     = "WidControlUpdatePriceList";
        const String ctCLSFilterSelection               = "FilterSelection";
        const String ctCLSFieldContainer                = "FieldContainer";        
        const String ctCLSField                         = "Field";
        const String ctCLSLabel                         = "Label";
        const String ctCLSInputDiv                      = "InputDiv";
        const String ctCLSPriceDiv                      = "PriceDiv";
        const String ctCLSUsualPriceDiv                 = "UsualPriceDiv";
        const String ctCLSInlineButtonDiv               = "InlineButtonDiv";

        const String ctCLSConfirmationTitle             = "ConfirmationTitle";        
        const String ctCLSButtonPanel                   = "ButtonPanel";
        const String ctCLSUpdateButton                  = "UpdateButton";
        const String ctCLSBackButton                    = "BackButton";
        const String ctCLSConfirmButton                 = "ConfirmButton";

        const String ctCMDShowUsualPrice                = "@cmd%showusualprice";
        const String ctCMDHideUsualPrice                = "@cmd%hideusualprice";

        const String ctCMDUpdate                        = "@cmd%update";
        const String ctCMDBack                          = "@cmd%back";
        const String ctCMDConfirm                       = "@cmd%confirm";

        const String ctICODownChevron                   = "downchevron.png";
        const String ctICOUpChevron                     = "upchevron.png";

        const String ctDEFUsualPrice                    = "Usual Price";
        const String ctDEFConfirmationTitle             = "CONFIRMATION OF CHANGES";
        const String ctDEFUpdateButtonText              = "Update";
        const String ctDEFBackButtonText                = "Back";
        const String ctDEFConfirmButtonText             = "Confirm";        
        const int    ctDEFMaxLength                     = 8;

        public CompositeFormInterface SCI_ParentForm { get; set; }

        public String SC_GroupLabel             { get; set; }
        public String SC_ActiveGroup            { get; set; }  
        
        public String SC_GroupingColumnName     { get; set; }
        public String SC_KeyColumnName          { get; set; }
        public String SC_DescriptionColumnName  { get; set; }
        public String SC_PriceColumnName        { get; set; }
        public String SC_UsualPriceColumnName   { get; set; }

        public String SC_UsualPriceText         { get; set; }
        public String SC_ConfirmationTitleText  { get; set; }
        public String SC_UpdateButtonText       { get; set; }
        public String SC_BackButtonText         { get; set; }
        public String SC_ConfirmButtonText      { get; set; }
        
        public int    SC_MaxLength              { get; set; }

        public DataTable SC_DataTable           { get; set; }
        
        public WidControlUpdatePriceList()
        {
            SC_GroupLabel               = String.Empty;
            SC_ActiveGroup              = String.Empty;

            SC_ConfirmationTitleText    = ctDEFConfirmationTitle;
            SC_UpdateButtonText         = ctDEFUpdateButtonText;
            SC_BackButtonText           = ctDEFBackButtonText;
            SC_ConfirmButtonText        = ctDEFConfirmButtonText;

            SC_UsualPriceText           = ctDEFUsualPrice;

            SC_GroupingColumnName       = String.Empty;
            SC_KeyColumnName            = String.Empty;
            SC_DescriptionColumnName    = String.Empty;
            SC_PriceColumnName          = String.Empty;
            SC_UsualPriceColumnName     = String.Empty;

            SC_DataTable                = null;

            SC_MaxLength                = ctDEFMaxLength;
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

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_Mobile, ctWidControlUpdatePriceListStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_Mobile, ctWidControlUpdatePriceListScript));
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

        private void RenderConfirmationTitle(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSConfirmationTitle);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(SC_ConfirmationTitleText);
            paComponentController.RenderEndTag();            
        }
        
        private void RenderButtonPanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSUpdateButton);
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDUpdate);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(SC_UpdateButtonText);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSBackButton);
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDBack);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(SC_BackButtonText);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSConfirmButton);
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDConfirm);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(SC_ConfirmButtonText);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderFilterDiv(ComponentController paComponentController)
        {            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSFilterSelection);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            RenderFilterBox(paComponentController);
            paComponentController.RenderEndTag();
        }       

        private void RenderInlineButtons(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInlineButtonDiv);
            
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDShowUsualPrice);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICODownChevron));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDHideUsualPrice);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOUpChevron));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderPriceDiv(ComponentController paComponentController, MetaDataRow paMetaDataRow)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPriceDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(paMetaDataRow.ActiveData.GetMultiColumnData(SC_DescriptionColumnName, String.Empty));
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInputDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, SC_PriceColumnName.ToLower());
            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_OriginalValue, paMetaDataRow.ActiveData.GetData(SC_PriceColumnName, String.Empty));
            paComponentController.AddAttribute(HtmlAttribute.Value, paMetaDataRow.ActiveData.GetData(SC_PriceColumnName, String.Empty));
            paComponentController.AddAttribute(HtmlAttribute.Maxlength, SC_MaxLength.ToString());
            paComponentController.AddAttribute(HtmlAttribute.Type, "text");
            paComponentController.RenderBeginTag(HtmlTag.Input);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderUsualPriceDiv(ComponentController paComponentController, MetaDataRow paMetaDataRow)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSUsualPriceDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(paMetaDataRow.ActiveData.GetMultiColumnData(SC_UsualPriceText, String.Empty));
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInputDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, SC_UsualPriceColumnName.ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_OriginalValue, paMetaDataRow.ActiveData.GetData(SC_UsualPriceColumnName, String.Empty));
            paComponentController.AddAttribute(HtmlAttribute.Value, paMetaDataRow.ActiveData.GetData(SC_UsualPriceColumnName, String.Empty));
            paComponentController.AddAttribute(HtmlAttribute.Maxlength, SC_MaxLength.ToString());
            paComponentController.AddAttribute(HtmlAttribute.Type, "text");
            paComponentController.RenderBeginTag(HtmlTag.Input);
            paComponentController.RenderEndTag();

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
            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_KeyValue, General.GetFormattedDBData(paMetaDataRow.ActiveData.GetData(SC_KeyColumnName, String.Empty)));
            paComponentController.AddElementType(ComponentController.ElementType.Element);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSField);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderPriceDiv(paComponentController, paMetaDataRow);
            
            RenderInlineButtons(paComponentController);

            RenderUsualPriceDiv(paComponentController, paMetaDataRow);            

            paComponentController.RenderEndTag();
        }

        protected void RenderFieldContainer(ComponentController paComponentController)
        {
            MetaDataRow         lcMetaDataRow;

            if ((SC_DataTable != null) && (SC_KeyColumnName != null) && (SC_PriceColumnName != null) && (SC_DescriptionColumnName != null) && (SC_UsualPriceColumnName != null))
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
            //    SC_DescriptionColumnName    = SC_DataTable.Columns.Contains(SC_DescriptionColumnName) ? SC_DescriptionColumnName : null;
                SC_PriceColumnName          = SC_DataTable.Columns.Contains(SC_PriceColumnName) ? SC_PriceColumnName : null;
                SC_UsualPriceColumnName     = SC_DataTable.Columns.Contains(SC_UsualPriceColumnName) ? SC_UsualPriceColumnName : null;
            }            
        }
        
        protected void RenderBrowserMode(ComponentController paComponentController)
        {
            IncludeExternalLinkFiles(paComponentController);
            VerifyColumnNames();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ControlMode, "standard");
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlUpdatePriceList);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            RenderFilterDiv(paComponentController);
            RenderConfirmationTitle(paComponentController);
            RenderFieldContainer(paComponentController);            
            RenderButtonPanel(paComponentController);
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
