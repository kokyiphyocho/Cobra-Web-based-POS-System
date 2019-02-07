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
    public class WidControlFEBasicStoreLocation : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlFEBasicStoreLocationStyle    = "WidControlFEBasicStoreLocation.css";
        protected const String ctWidControlFEBasicStoreLocationScript   = "WidControlFEBasicStoreLocation.js";

        protected const String ctGoogleMapInterfaceScript               = "googlemap.interface.js";

        const String ctCLSWidControlFEBasicStoreLocation    = "WidControlFEBasicStoreLocation";        
        const String ctCLSMapPanel                          = "MapPanel";
        const String ctCLSShopNameDiv                       = "AddButton";        
        const String ctCLSAddressDiv                        = "AddressDiv";
        const String ctCLSContactNoDiv                      = "ContactNoDiv";
        const String ctCLSOpeningHourDiv                    = "OpeningHourDiv";
        const String ctCLSInfoPanel                         = "InfoPanel";
        const String ctCLSInfoIcon                          = "InfoIcon";
        const String ctCLSInfoText                          = "InfoText";

        //const String ctFASEnvelope                          = "&#xf0e0";
        //const String ctFASClock                             = "&#xf017";
        //const String ctFASPhone                             = "&#xf095";
        //const String ctFASInfoCycle                         = "&#xf05a";
        
        SubscriberRow clSubscriberRow;

        public CompositeFormInterface SCI_ParentForm { get; set; }

        public WidControlFEBasicStoreLocation()
        {
           clSubscriberRow = RetrieveRow();    
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);
                        
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetFoundationScriptUrl(ctGoogleMapInterfaceScript));

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_General, ctWidControlFEBasicStoreLocationStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_General, ctWidControlFEBasicStoreLocationScript));
        }

        private SubscriberRow RetrieveRow()
        {
            DataRow lcDataRow;

            if ((lcDataRow = ApplicationFrame.GetInstance().ActiveFormInfoManager.RunRetrieveRow()) != null) return (new SubscriberRow(lcDataRow));
            else return (null);
        }

        private void RenderMapPanel(ComponentController paComponentController)
        {
            SubControlGoogleMap     lcGoogleMap;

            lcGoogleMap = new SubControlGoogleMap();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSMapPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            if ((clSubscriberRow != null) && (clSubscriberRow.Latitude != 0) && (clSubscriberRow.Longitude != 0))
            {
                lcGoogleMap.SC_Longitude    = clSubscriberRow.Longitude;
                lcGoogleMap.SC_Latitude     = clSubscriberRow.Latitude;
            }

            lcGoogleMap.RenderChildMode(paComponentController);

            paComponentController.RenderEndTag();
        }

        private void RenderInfoPanel(ComponentController paComponentController)
        {
            String lcBusinessName;
            String lcAddress;
            String lcContactNo;
            String lcOperatingHour;

            if (clSubscriberRow != null)
            {
                lcBusinessName =  clSubscriberRow.BusinessName;
                lcAddress = UILogic.CompileAddress(clSubscriberRow.Row);
                lcContactNo = clSubscriberRow.ContactNo;
                lcOperatingHour = clSubscriberRow.OperatingHour;            
            }
            else
            {
                lcBusinessName = String.Empty;
                lcAddress = String.Empty;
                lcContactNo = String.Empty;
                lcOperatingHour = String.Empty;
            }

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInfoPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInfoIcon);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.info_sign));
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInfoText);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(lcBusinessName);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInfoIcon);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(ComponentController.UnicodeStr((int) Fontawesome.envelope));
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInfoText);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(lcAddress);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInfoIcon);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.phone));
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInfoText);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(lcContactNo);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInfoIcon);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.time));
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInfoText);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(lcOperatingHour);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
            
        }
        
    
        protected void RenderBrowserMode(ComponentController paComponentController)
        {        
            MetaDataRow lcMetaDataRow;

            IncludeExternalLinkFiles(paComponentController);
                        
            if (clSubscriberRow != null)
                lcMetaDataRow = new MetaDataRow(clSubscriberRow.Row);
            else
                lcMetaDataRow = null;
                        
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlFEBasicStoreLocation);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderMapPanel(paComponentController);
            RenderInfoPanel(paComponentController);

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

