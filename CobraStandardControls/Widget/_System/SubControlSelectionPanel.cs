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
    public class SubControlSelectionPanel : WebControl, WidgetControlInterface
    {
        const String ctSubControlSelectionPanelStyle    = "SubControlSelectionPanel.css";
        const String ctSubControlSelectionPanelScript   = "SubControlSelectionPanel.js";

        const String ctCLSSubControlSelectionPanel      = "SubControlSelectionPanel";
        const String ctCLSPanelOverlay                  = "PanelOverlay";
        const String ctCLSSelectionPanel                = "SelectionPanel";
        const String ctCLSPanelTitleBar                 = "PanelTitleBar";
        const String ctCLSPanelHeader                   = "PanelHeader";
        const String ctCLSPanelCloseButton              = "PanelCloseButton";
        const String ctCLSContentArea                   = "ContentArea";

        const String ctCLSItemContainer                 = "ItemContainer";
        const String ctCLSItem                          = "Item";

        const String ctCLSSelectionButtonPanel          = "SelectionButtonPanel";
        const String ctCLSSelectionCancelButton         = "SelectionCancelButton";
        const String ctCLSSelectionChooseButton         = "SelectionChooseButton";

        protected const String ctQuery                  = "#QUERY";

        const String ctDYTCancelButtonText              = "@@POS.Button.Cancel";
        const String ctDYTChooseButtonText              = "@@POS.Button.Choose";

        const String ctCMDItem                          = "@cmd%item";
        const String ctCMDSelectionClose                = "@cmd%selectionclose";
        const String ctCMDSelectionCancel               = "@cmd%selectioncancel";
        const String ctCMDSelectionChoose               = "@cmd%selectionchoose";

        public enum ListMode { DictionaryMode, ArrayMode };
        public enum SelectionMode { Text, Color, Image }

        public CompositeFormInterface SCI_ParentForm { get; set; }

        Dictionary<String, String>    clDictionarySelectionList;
        String[,]                     clArraySelectionList;
        String                        clPanelTitle;
        String                        clPanelType;
        String                        clPanelAppearance;
        String                        clFilterName;
        ListMode                      clListMode;
        SelectionMode                 clSelectionMode;
        LanguageManager               clLanguageManager;

        public SubControlSelectionPanel(String paPanelType, String paPanelAppearance, String paPanelTitle, Dictionary<String, String> paSelectionList, String paFilterName = null)
        {
            clDictionarySelectionList     = paSelectionList;
            clPanelTitle        = paPanelTitle;
            clPanelType         = paPanelType;
            clPanelAppearance   = paPanelAppearance;
            clFilterName        = paFilterName;
            clListMode          = ListMode.DictionaryMode;
            clSelectionMode     = SelectionMode.Text;

            clLanguageManager = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
        }  

        public SubControlSelectionPanel(String paPanelType, String paPanelAppearance, String paPanelTitle, String paDynamicQuery, String paFilterName = null)
        {            
            clArraySelectionList = DynamicQueryManager.GetInstance().GetTableStringArrayResult(paDynamicQuery);            
            clPanelTitle        = paPanelTitle;
            clPanelType         = paPanelType;
            clPanelAppearance   = paPanelAppearance;
            clFilterName        = paFilterName;
            clListMode          = ListMode.ArrayMode;
            clSelectionMode     = SelectionMode.Text;

            clLanguageManager = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
        }

        public void SetSelectionMode(SelectionMode paSelectionMode)
        {
            clSelectionMode = paSelectionMode;
        }        
        
        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.System, ctSubControlSelectionPanelStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.System, ctSubControlSelectionPanelScript));
        }

        private void RenderButtonPanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSelectionButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSelectionCancelButton);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDSelectionCancel);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.remove));
            // paComponentController.Write(clLanguageManager.GetText(ctDYTCancelButtonText));
            paComponentController.RenderEndTag();


            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSelectionChooseButton);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDSelectionChoose);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.ok));
            // paComponentController.Write(clLanguageManager.GetText(ctDYTChooseButtonText));
            paComponentController.RenderEndTag();
           
            paComponentController.RenderEndTag();
        }

        private void RenderDictionaryItemList(ComponentController paComponentController)
        {
            if (clDictionarySelectionList != null)
            {
                foreach (String lcKey in clDictionarySelectionList.Keys)
                {
                    paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItem);
                    paComponentController.AddAttribute(HtmlAttribute.Value, lcKey);
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDItem);
                    paComponentController.RenderBeginTag(HtmlTag.A);
                    paComponentController.Write(Convert.ToString(clDictionarySelectionList[lcKey]));
                    paComponentController.RenderEndTag();
                }
            }          
        }

        private void RenderArrayItemList(ComponentController paComponentController)
        {
            if (clArraySelectionList != null)
            {
                for (int lcCount = 0; lcCount < clArraySelectionList.GetLength(0); lcCount++)
                {
                    paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItem);                    
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Group, clArraySelectionList[lcCount,0]);
                    paComponentController.AddAttribute(HtmlAttribute.Value, clArraySelectionList[lcCount, 2]);
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Text, clArraySelectionList[lcCount, 1]);
                    if (clSelectionMode == SelectionMode.Color)
                        paComponentController.AddAttribute(HtmlAttribute.Style, "background:" + clArraySelectionList[lcCount, 2] + ";color:" + General.ContrastColor(clArraySelectionList[lcCount, 2]));
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDItem);
                    paComponentController.RenderBeginTag(HtmlTag.A);
                    paComponentController.Write(clArraySelectionList[lcCount,1]);
                    paComponentController.RenderEndTag();
                }
            }
        }

        private void RenderImageItemList(ComponentController paComponentController)
        {
            if (clArraySelectionList != null)
            {
                for (int lcCount = 0; lcCount < clArraySelectionList.GetLength(0); lcCount++)
                {
                    paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItem);
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Group, clArraySelectionList[lcCount, 0]);
                    paComponentController.AddAttribute(HtmlAttribute.Value, clArraySelectionList[lcCount, 2]);
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDItem);
                    paComponentController.RenderBeginTag(HtmlTag.A);

                    paComponentController.AddAttribute(HtmlAttribute.Src, clArraySelectionList[lcCount, 2]);
                    paComponentController.RenderBeginTag(HtmlTag.Img);
                    paComponentController.RenderEndTag();

                    paComponentController.RenderEndTag();
                }
            }
        }

        private void RenderContentArea(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.List);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSContentArea);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            if (clListMode == ListMode.DictionaryMode)
                RenderDictionaryItemList(paComponentController);
            else
            {
                if (clSelectionMode == SelectionMode.Image) RenderImageItemList(paComponentController);
                else RenderArrayItemList(paComponentController);
            }

            paComponentController.RenderEndTag();
        }

        private void RenderTitleBar(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPanelTitleBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPanelHeader);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.Write(clLanguageManager.GetText(clPanelTitle));

            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPanelCloseButton);            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDSelectionClose);
            paComponentController.RenderBeginTag(HtmlTag.A);

            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.remove));

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderPanelArea(ComponentController paComponentController)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, clPanelType.ToLower());
            paComponentController.AddElementType(ComponentController.ElementType.Panel);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSelectionPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderTitleBar(paComponentController);
            RenderContentArea(paComponentController);
            RenderButtonPanel(paComponentController);

            paComponentController.RenderEndTag();
        }

        private void RenderBrowserMode(ComponentController paComponentController)
        {            
            IncludeExternalLinkFiles(paComponentController);

            if (!String.IsNullOrEmpty(clFilterName))
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_LinkColumn, clFilterName.ToLower());

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, clPanelType);            
            paComponentController.AddElementType(ComponentController.ElementType.PopUp);            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Appearance, clPanelAppearance);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSubControlSelectionPanel );
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.Overlay);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type,clPanelType.ToLower());
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
