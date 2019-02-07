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
    public class SubControlMobileStoreFrontOptionPanel : WebControl, WidgetControlInterface
    {
        const String ctSubControlMobileStoreFrontOptionPanelStyle     = "SubControlMobileStoreFrontOptionPanel.css";
        const String ctSubControlMobileStoreFrontOptionPanelScript    = "SubControlMobileStoreFrontOptionPanel.js";

        const String ctCLSSubControlMobileStoreFrontOptionPanel         = "SubControlMobileStoreFrontOptionPanel";
        const String ctCLSPanelOverlay                  = "PanelOverlay";
        const String ctCLSOptionPanel                   = "OptionPanel";
        const String ctCLSPanelTitleBar                 = "PanelTitleBar";
        const String ctCLSPanelHeader                   = "PanelHeader";
        const String ctCLSCloseButton                   = "CloseButton";
        const String ctCLSContentArea                   = "ContentArea";
        const String ctCLSFilterGroup                   = "FilterGroup";
        const String ctCLSGroupHeading                  = "GroupHeading";
        const String ctCLSGroupHeader                   = "GroupHeader";
        const String ctCLSSelection                     = "Selection";
        const String ctCLSChevron                       = "Chevron";
        const String ctCLSItemContainer                 = "ItemContainer";
        const String ctCLSItem                          = "Item";

        const String ctCLSButtonPanel                   = "ButtonPanel";
        const String ctCLSResetButton                   = "ResetButton";
        const String ctCLSSearchButton                  = "SearchButton";

        protected const String ctQuery                  = "#QUERY";

        const String ctDEFResetButtonText               = "Reset";
        const String ctDEFSearchButtonText              = "Search";
        const String ctDEFSortButtonText                = "Sort";

        const String ctCMDClose                         = "@cmd%close";
        const String ctCMDReset                         = "@cmd%reset";
        const String ctCMDSearch                        = "@cmd%search";
        const String ctCMDSort                          = "@cmd%sort";

        public enum PanelMode { Filter, Sort }
        
        public CompositeFormInterface SCI_ParentForm    { get; set; }

        public String SC_PanelMetaData                  { get; set; }        

        PanelMode                   clPanelMode;
        MetaDataBlockCollection     clMetaDataBlockCollection;

        public SubControlMobileStoreFrontOptionPanel(PanelMode paPanelMode)
        {
            clMetaDataBlockCollection   = null;           
            clPanelMode                 = paPanelMode;

            SC_PanelMetaData            = null;
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_Mobile, ctSubControlMobileStoreFrontOptionPanelStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_Mobile, ctSubControlMobileStoreFrontOptionPanelScript));
        }

        private void RenderGroupHeading(ComponentController paComponentController, MetaDataBlock paMetaDataBlock)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSGroupHeading);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSGroupHeader);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(paMetaDataBlock.MetaDataBlockName);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSChevron);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSelection);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            if (paMetaDataBlock.MetaDataElementCount > 0) paComponentController.Write(paMetaDataBlock[0].Name);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderButtonPanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSResetButton);
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDReset);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(ctDEFResetButtonText);
            paComponentController.RenderEndTag();


            if (clPanelMode == PanelMode.Sort)
            {
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSearchButton);
                paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDSort);
                paComponentController.RenderBeginTag(HtmlTag.A);
                paComponentController.Write(ctDEFSortButtonText);
                paComponentController.RenderEndTag();
            }
            else
            {
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSearchButton);
                paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDSearch);
                paComponentController.RenderBeginTag(HtmlTag.A);
                paComponentController.Write(ctDEFSearchButtonText);
                paComponentController.RenderEndTag();
            }
            paComponentController.RenderEndTag();
        }

        private void RenderItem(ComponentController paComponentController, String paItemName, String paFilter)
        {
          //  bool lcTemplateMode;

    //        lcTemplateMode = String.IsNullOrEmpty(paFilter);
//         //   paComponentController.AddElementAttribute(ComponentController.ElementAttribute.sa_TemplateMode, lcTemplateMode.ToString());
  //          paComponentController.AddAttribute(HtmlAttribute.Value, lcTemplateMode ? paItemName : paFilter);            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItem);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(paItemName);
            paComponentController.RenderEndTag();
        }

        private void RenderItemList(ComponentController paComponentController, String paQueryString)
        {
            String[] lcItemList;

            if ((lcItemList = RetrieveData(paQueryString)) != null)
            {
                for (int lcCount = 0; lcCount < lcItemList.Length; lcCount++)
                    RenderItem(paComponentController, lcItemList[lcCount], null);
            }
        }

        private String[] RetrieveData(String paQueryString)
        {
            QueryClass lcQueryClass;
            DataTable lcDataTable;

            if (!String.IsNullOrEmpty(paQueryString))
            {
                lcQueryClass = new QueryClass(paQueryString, QueryClass.ConnectionMode.EService);
                ApplicationFrame.GetInstance().ActiveFormInfoManager.ReplaceQueryPlaceHolder(lcQueryClass);

                if (((lcDataTable = lcQueryClass.RunQuery()) != null) && (lcDataTable.Rows.Count > 0))
                    return (General.SingleDimensionStringArray(lcDataTable.Select(), lcDataTable.Columns[0].ColumnName));
            }

            return (null);
        }

        private void RenderContent(ComponentController paComponentController, MetaDataBlock paMetaDataBlock)
        {
            if (paMetaDataBlock != null)
            {
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItemContainer);
                paComponentController.RenderBeginTag(HtmlTag.Div);

                for (int lcCount = 0; lcCount < paMetaDataBlock.MetaDataElementCount; lcCount++)
                {
                    if (paMetaDataBlock[lcCount].Name == ctQuery) RenderItemList(paComponentController, paMetaDataBlock[lcCount][0]);
                    else RenderItem(paComponentController, paMetaDataBlock[lcCount].Name, paMetaDataBlock[lcCount][0]);
                }

                paComponentController.RenderEndTag();
            }
        }

        private void RenderFilterBlock(ComponentController paComponentController, MetaDataBlock paMetaDataBlock)
        {
            if ((paMetaDataBlock != null) && (!String.IsNullOrEmpty(paMetaDataBlock.MetaDataBlockName)))
            {

                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Template, paMetaDataBlock.MetaDataBlockName);
                // paComponentController.AddAttribute(HtmlAttribute.Value, paMetaDataBlock.MetaDataBlockName);
                // paComponentController.AddElementAttribute(ComponentController.ElementAttribute.sa_FilterTemplate, ctDEFFilterTemplate, false);
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSFilterGroup);
                paComponentController.RenderBeginTag(HtmlTag.Div);

                RenderGroupHeading(paComponentController, paMetaDataBlock);
                RenderContent(paComponentController, paMetaDataBlock);

                paComponentController.RenderEndTag();
            }
        }

        private void RenderContentArea(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.List);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSContentArea);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            if (clMetaDataBlockCollection != null)
            {
                for (int lcCount = 0; lcCount < clMetaDataBlockCollection.MetaDataBlockCount; lcCount++)
                    RenderFilterBlock(paComponentController, clMetaDataBlockCollection[lcCount]);
            }

            paComponentController.RenderEndTag();
        }

        private void RenderTitleBar(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPanelTitleBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPanelHeader);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.Write(clPanelMode.ToString());

            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCloseButton);
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDClose);
            paComponentController.RenderBeginTag(HtmlTag.A);

            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.remove));

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderPanelArea(ComponentController paComponentController)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, clPanelMode.ToString().ToLower());
            paComponentController.AddElementType(ComponentController.ElementType.Panel);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSOptionPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderTitleBar(paComponentController);
            RenderContentArea(paComponentController);
            RenderButtonPanel(paComponentController);

            paComponentController.RenderEndTag();
        }

        private void RenderBrowserMode(ComponentController paComponentController)
        {            
            IncludeExternalLinkFiles(paComponentController);

            clMetaDataBlockCollection = new MetaDataBlockCollection(SC_PanelMetaData);
            
            paComponentController.AddElementType(ComponentController.ElementType.PopUp);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSubControlMobileStoreFrontOptionPanel );
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.Overlay);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, clPanelMode.ToString().ToLower());
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPanelOverlay);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderPanelArea(paComponentController);

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
            RenderBrowserMode(paComponentController);
        }

        protected override void Render(HtmlTextWriter paHtmlTextWriter)
        {
            if (!DesignMode) RenderBrowserMode(new ComponentController(paHtmlTextWriter));
            else RenderDesignMode(new ComponentController(paHtmlTextWriter));
        }
    }
}
