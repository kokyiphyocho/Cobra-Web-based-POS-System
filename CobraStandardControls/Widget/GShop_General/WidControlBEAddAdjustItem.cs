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
    public class WidControlBEAddAdjustItem : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlBEAddAdjustItemStyle     = "WidControlBEAddAdjustItem.css";
        protected const String ctWidControlBEAddAdjustItemScript    = "WidControlBEAddAdjustItem.js";

        protected const String ctJQueryUIScript             = "jquery-ui.min.js";
        protected const String ctJQueryUIStyle              = "jquery-ui.min.css";

        protected const String ctSumoSelectScript           = "jquery.sumoselect.min.js";
        protected const String ctSumoSelectStyle            = "sumoselect.css";
        
        protected const String ctJQueryUIFunctionScript     = "jquery-ui.functions.js";

        const String ctCLSWidControlBEAddAdjustItem         = "WidControlBEAddAdjustItem";        
        const String ctCLSButtonPanel                       = "ButtonPanel";
        const String ctCLSAddButton                         = "AddButton";        
        const String ctCLSCloseButton                       = "CloseButton";

        const String ctDEFAddButtonText                     = "Add";
        const String ctDEFUpdateButtonText                  = "Update";
        const String ctDEFCloseButtonText                   = "Close";               

        const String ctCMDUpdate                            = "@cmd%update";
        const String ctCMDClose                             = "@cmd%close";

        public enum ControlMode                 { Item, Category }

        public CompositeFormInterface SCI_ParentForm { get; set; }

        public ControlMode SC_ControlMode       { get; set; }
        public String SC_AddButtonText          { get; set; }
        public String SC_UpdateButtonText       { get; set; }
        public String SC_CloseButtonText        { get; set; }
        
        StandardItemCatalogueRow        clStandardCatalogueRow;
        
        public WidControlBEAddAdjustItem()
        {
            clStandardCatalogueRow = RetrieveRow();

            SC_ControlMode          = ControlMode.Item;
            SC_AddButtonText        = ctDEFAddButtonText;
            SC_UpdateButtonText     = ctDEFUpdateButtonText;
            SC_CloseButtonText      = ctDEFCloseButtonText;
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

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_General, ctWidControlBEAddAdjustItemStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_General, ctWidControlBEAddAdjustItemScript));
        }

        private void RenderButtonPanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSAddButton);
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDUpdate);
            paComponentController.RenderBeginTag(HtmlTag.A);

            if (clStandardCatalogueRow == null)
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

        private StandardItemCatalogueRow RetrieveRow()
        {
            DataRow lcDataRow;

            if ((lcDataRow = ApplicationFrame.GetInstance().ActiveFormInfoManager.RunRetrieveRow()) != null) return (new StandardItemCatalogueRow(lcDataRow));
            else return (null);
        }
    
        protected void RenderBrowserMode(ComponentController paComponentController)
        {
            InputInfoManager lcInputInfoManager;
            MetaDataRow lcMetaDataRow;

            IncludeExternalLinkFiles(paComponentController);

            lcInputInfoManager = ApplicationFrame.GetInstance().ActiveFormInfoManager.FieldInfoManager.ActiveInputInfoManager;

            if (clStandardCatalogueRow != null)
                lcMetaDataRow = new MetaDataRow(clStandardCatalogueRow.Row);
            else
                lcMetaDataRow = null;

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ControlMode, SC_ControlMode.ToString().ToLower());
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlBEAddAdjustItem);
            paComponentController.RenderBeginTag(HtmlTag.Div);
                        
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            if (lcInputInfoManager != null)
            {
                lcInputInfoManager.RenderAllSubGroups(paComponentController, lcMetaDataRow);
            }

            paComponentController.RenderEndTag();

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
