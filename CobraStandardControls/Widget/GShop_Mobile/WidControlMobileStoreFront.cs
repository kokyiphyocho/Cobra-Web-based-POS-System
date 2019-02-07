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
    public class WidControlMobileStoreFront : WebControl, WidgetControlInterface, AjaxWidgetControlInterface, AjaxWidgControlPagingInterface
    {
        protected const String ctWidControlMobileStoreFrontStyle   = "WidControlMobileStoreFront.css";
        protected const String ctWidControlMobileStoreFrontScript  = "WidControlMobileStoreFront.js";

        const String ctCLSWidControlMobileStoreFront    = "WidControlMobileStoreFront";        
        const String ctCLSFormTitleArea                 = "FormTitleArea";
        const String ctCLSTitle                         = "Title";
        const String ctCLSSearchBox                     = "SearchBox";
        const String ctCLSInputBox                      = "InputBox";        
        const String ctCLSSearchButton                  = "SearchButton";
        const String ctCLSCloseButton                   = "CloseButton";

        const String ctCLSToolBar                       = "ToolBar";
        const String ctCLSInfoBox                       = "InfoBox";
        const String ctCLSToolButton                    = "ToolButton";

        const String ctCLSItemGridPanel                 = "ItemGridPanel";
        const String ctCLSItemContainer                 = "ItemContainer";

        const String ctCLSAjaxLoaderDiv                 = "AjaxLoaderDiv";
        const String ctCLSAjaxLoaderImage               = "AjaxLoaderImage";
        const String ctCLSAjaxErrorImage                = "AjaxErrorImage";

        const String ctCLSDetailOverlayDiv              = "DetailOverlayDiv";

        const String ctCLSNoDataDiv                     = "NoDataDiv";

        const String ctCMDOpenSortPopUp                 = "@cmd%opensortpopup";
        const String ctCMDOpenFilterPopUp               = "@cmd%openfilterpopup";
        const String ctCMDOpenStoreInfoPopUp            = "@cmd%openstoreinfopopup";
        const String ctCMDSearchKeyWord                 = "@cmd%searchkeyword";
        const String ctCMDCloseForm                     = "@cmd%closeform";

        const String ctDEFSearchPlaceHolder             = "Search Products";
        const String ctDEFImagePlaceHolder              = "{{NoPhoneImage.jpg}}";
        const String ctDEFInfoMessageTemplate           = "$DATACOUNT Item(s) Found.";
        const int    ctDEFPageSize                      = 10;

        const String ctPRMGridFilterInfo                = "GridFilterInfo";
        const String ctPRMGridSortInfo                  = "GridSortInfo";
        const String ctPRMGridPageIndex                 = "GridPageIndex";

        const String ctFPMPreviewMode                   = "PreviewMode";

        const String ctRetrieveGridData                 = "RetrieveGridData";

        const String ctCOLTotalPages                    = "TotalPages";
        const String ctCOLTotalRows                     = "TotalRows";

        const String ctICOError                         = "Error.png";

        const String ctAJAXBigCircle                    = "AJAX_IndicatorBigCircle.gif";

        const String ctDEFNoDataText                    = "No Record Found.";

        public CompositeFormInterface SCI_ParentForm            { get; set; }

        public String SC_Title                                  { get; set; }
        public String SC_SearchPlaceHolder                      { get; set; }
        
        public String SC_InfoMessageTemplate                    { get; set; }

        public int    SC_PageSize                               { get; set; }
        
        public String SC_FilterPanelMetaData                    { get; set; }
        public String SC_SortPanelMetaData                      { get; set; }
        public String SC_FilterMetaData                         { get; set; }
        public String SC_UniversalImagePath                     { get; set; }
        public String SC_LocalImagePath                         { get; set; }

        public DataRow SC_SubscriberInfo                        { get; set; }

        public String SC_NoDataText                             { get; set; }

        private DataTable   clDataTable;
        private int         clTotalRows;
        private int         clTotalPages;
        private int         clPageIndex;
        private int         clFetchedRows;
        private String      clSortInfo;
        private String      clFilterInfo;
        
        public WidControlMobileStoreFront()
        {
            // SC_Title = ApplicationFrame.GetInstance().ActiveSubscription.ActiveRow.SubscriptionTitle;

            SC_SearchPlaceHolder        = ctDEFSearchPlaceHolder;
            SC_InfoMessageTemplate      = ctDEFInfoMessageTemplate;

            SC_PageSize                 = ctDEFPageSize;
            SC_UniversalImagePath       = String.Empty;
            SC_LocalImagePath           = String.Empty;

            SC_NoDataText               = ctDEFNoDataText;

            SC_SubscriberInfo           = null;

            clPageIndex                 = 0;
            clTotalRows                 = 0;
            clTotalPages                = 0;
            clSortInfo                  = null;
            clFilterInfo                = null;     
        } 

        public Dictionary<String, object> GetAjaxPagingInfo()                                          
        {
            Dictionary<String, object> lcDictionary;

            lcDictionary = new Dictionary<string, object>();
            lcDictionary.Add(AjaxSummaryResponse.TotalRows.ToString(), clTotalRows);
            lcDictionary.Add(AjaxSummaryResponse.TotalPages.ToString(), clTotalPages);
            lcDictionary.Add(AjaxSummaryResponse.PageIndex.ToString(), clPageIndex);
            lcDictionary.Add(AjaxSummaryResponse.FetchedRows.ToString(), clFetchedRows);

            return (lcDictionary);
        }

        private DataTable RetrieveData()
        {
            DataTable                   lcDataTable;
            QueryClass                  lcQueryClass;
            GridFilterController        lcGridFilterController;            
            String                      lcFilterOption;
            String                      lcSortOption;
                   
            lcGridFilterController     = new GridFilterController(SC_FilterMetaData);
            lcFilterOption             = lcGridFilterController.GetMultiFilterStr(clFilterInfo);
            lcSortOption               = lcGridFilterController.GetMultiSortKeyStr(clSortInfo);
            
            lcQueryClass               = DynamicQueryManager.GetInstance().GetQueryClass(ctRetrieveGridData);
            lcQueryClass.ReplacePlaceHolder("$FILTEROPTION",lcFilterOption, false);
            lcQueryClass.ReplacePlaceHolder("$SORTOPTION", lcSortOption, false);
            lcQueryClass.ReplacePlaceHolder("$PAGEINDEX", clPageIndex.ToString(), false);
            lcQueryClass.ReplacePlaceHolder("$PAGESIZE", SC_PageSize.ToString(), false);

            lcDataTable = lcQueryClass.RunQuery();

            if (lcDataTable.Rows.Count > 0)
            {
                clTotalRows     = Convert.ToInt32(lcDataTable.Rows[0][ctCOLTotalRows]);
                clTotalPages    = Convert.ToInt32(lcDataTable.Rows[0][ctCOLTotalPages]);
                clFetchedRows   = lcDataTable.Rows.Count;
            }
            else
            {
                clTotalRows     = 0;
                clTotalPages    = 0;
                clFetchedRows   = 0;
            }

            return (lcDataTable);
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_Mobile, ctWidControlMobileStoreFrontStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_Mobile, ctWidControlMobileStoreFrontScript));
        }

        private void RenderToolButton(ComponentController paComponentController, String paButtonCommand, Fontawesome paButtonIcon)
        {
            paComponentController.AddAttribute(HtmlAttribute.Href, paButtonCommand);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSToolButton);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(ComponentController. UnicodeStr((int)paButtonIcon));
            paComponentController.RenderEndTag();            
        }

        private void RenderInfoBox(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.StatusBar);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_StatusText, SC_InfoMessageTemplate);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInfoBox);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(SC_InfoMessageTemplate.Replace("$DATACOUNT", clTotalRows.ToString()));
            paComponentController.RenderEndTag();
        }

        private void RenderToolBar(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSToolBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderInfoBox(paComponentController);
            RenderToolButton(paComponentController, ctCMDOpenFilterPopUp, Fontawesome.filter);
            RenderToolButton(paComponentController, ctCMDOpenSortPopUp, Fontawesome.sort);
            RenderToolButton(paComponentController, ctCMDOpenStoreInfoPopUp, Fontawesome.map_marker);

            paComponentController.RenderEndTag();
        }

        private void RenderTitle(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTitle);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(ApplicationFrame.GetInstance().ActiveFormInfoManager.TranslateString(SC_Title));
            paComponentController.RenderEndTag();

            if (ApplicationFrame.GetInstance().ActiveFormInfoManager.GetFormParam(ctFPMPreviewMode) == "true")
            {
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCloseButton);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDCloseForm);
                paComponentController.RenderBeginTag(HtmlTag.A);
                paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.remove));
                paComponentController.RenderEndTag();
                paComponentController.RenderEndTag();
            }
        }

        private void RenderSearchBox(ComponentController paComponentController)
        {          
            paComponentController.AddElementType(ComponentController.ElementType.SearchBox);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSearchBox);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInputBox);
            paComponentController.AddAttribute(HtmlAttribute.Type, "text");
            paComponentController.AddAttribute(HtmlAttribute.PlaceHolder, SC_SearchPlaceHolder);
            paComponentController.RenderBeginTag(HtmlTag.Input);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDSearchKeyWord);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSearchButton);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.search));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderTitleArea(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSFormTitleArea);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderTitle(paComponentController);
            RenderSearchBox(paComponentController);            

            paComponentController.RenderEndTag();
        }

        private void RenderFilterPanel(ComponentController paComponentController)
        {
            SubControlMobileStoreFrontOptionPanel lcFilterPanel;

            lcFilterPanel = new SubControlMobileStoreFrontOptionPanel(SubControlMobileStoreFrontOptionPanel.PanelMode.Filter);
            lcFilterPanel.SC_PanelMetaData = SC_FilterPanelMetaData;
            lcFilterPanel.RenderChildMode(paComponentController);
        }

        private void RenderSortPanel(ComponentController paComponentController)
        {
            SubControlMobileStoreFrontOptionPanel lcSortPanel;

            lcSortPanel = new SubControlMobileStoreFrontOptionPanel(SubControlMobileStoreFrontOptionPanel.PanelMode.Sort);
            lcSortPanel.SC_PanelMetaData = SC_SortPanelMetaData;
            lcSortPanel.RenderChildMode(paComponentController);
        }

        private void RenderInfoPanel(ComponentController paComponentController)
        {
            SubControlMobileStoreFrontStoreInfo lcInfoPanel;

            lcInfoPanel = new SubControlMobileStoreFrontStoreInfo(SC_SubscriberInfo);
            lcInfoPanel.RenderChildMode(paComponentController);
        }

        private void RenderAjaxLoaderDiv(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSAjaxLoaderDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSAjaxLoaderImage);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetAjaxLoaderImageUrl(ctAJAXBigCircle));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSAjaxErrorImage);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetSystemImageUrl(ctICOError));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

        }

        private void RenderDetailOverlayDiv(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Overlay);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSDetailOverlayDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.RenderEndTag();
        }

        private void RenderItemGrid(ComponentController paComponentController)
        {            
            SubControlMobileStoreFrontGridItem lcGridItem;
            
            lcGridItem = new SubControlMobileStoreFrontGridItem();
            lcGridItem.IncludeExternalLinkFiles(paComponentController);
            lcGridItem.SC_LocalImagePath        = SC_LocalImagePath;
            lcGridItem.SC_UniversalImagePath    = SC_UniversalImagePath;
            
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItemContainer);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_TotalRows, clTotalRows.ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_TotalPages, clTotalPages.ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_PageIndex, "0");
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_PageSize, SC_PageSize.ToString());
            paComponentController.AddElementType(ComponentController.ElementType.Grid);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItemGridPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            foreach (DataRow lcRow in clDataTable.Rows)
            {
                lcGridItem.SetData(lcRow);
                lcGridItem.RenderChildMode(paComponentController);
            }

            paComponentController.RenderEndTag();

            RenderAjaxLoaderDiv(paComponentController);

            RenderDetailOverlayDiv(paComponentController);

            paComponentController.RenderEndTag();
        }

        private void RenderAjaxModeItemGrid(ComponentController paComponentController)
        {
            SubControlMobileStoreFrontGridItem lcGridItem;

            lcGridItem = new SubControlMobileStoreFrontGridItem();
            lcGridItem.IncludeExternalLinkFiles(paComponentController);
            lcGridItem.SC_LocalImagePath = SC_LocalImagePath;
            lcGridItem.SC_UniversalImagePath = SC_UniversalImagePath;
                        
            foreach (DataRow lcRow in clDataTable.Rows)
            {
                lcGridItem.SetData(lcRow);
                lcGridItem.RenderChildMode(paComponentController);
            }
        }

        private void RenderBrowserMode(ComponentController paComponentController)
        {
            clDataTable = RetrieveData();            
            IncludeExternalLinkFiles(paComponentController);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, this.GetType().ToString());
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlMobileStoreFront);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderTitleArea(paComponentController);
            RenderToolBar(paComponentController);
            RenderFilterPanel(paComponentController);
            RenderSortPanel(paComponentController);
            RenderInfoPanel(paComponentController);
            
            RenderItemGrid(paComponentController);

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

        public void RenderNoDataDiv(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSNoDataDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(SC_NoDataText);
            paComponentController.RenderEndTag();
        }
        
        public void RenderAjaxMode(ComponentController paComponentController)
        {
            clPageIndex     = General.ParseInt(ApplicationFrame.GetParameter(ctPRMGridPageIndex), 0);
            clFilterInfo    = ApplicationFrame.GetParameter(ctPRMGridFilterInfo);
            clSortInfo      = ApplicationFrame.GetParameter(ctPRMGridSortInfo);
            
            clDataTable = RetrieveData();
            if (clFetchedRows > 0) RenderAjaxModeItemGrid(paComponentController);
            else RenderNoDataDiv(paComponentController);
        }

        protected override void Render(HtmlTextWriter paHtmlTextWriter)
        {
            if (!DesignMode) RenderBrowserMode(new ComponentController(paHtmlTextWriter));
            else RenderDesignMode(new ComponentController(paHtmlTextWriter));
        }
    }
}
