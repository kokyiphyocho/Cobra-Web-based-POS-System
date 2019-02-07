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
    public class SubControlMobileStoreFrontStoreInfo : WebControl, WidgetControlInterface
    {
        const String ctSubControlMobileStoreFrontStoreInfoStyle     = "SubControlMobileStoreFrontStoreInfo.css";
        const String ctSubControlMobileStoreFrontStoreInfoScript    = "SubControlMobileStoreFrontStoreInfo.js";

        protected const String ctGoogleMapInterfaceScript           = "googlemap.interface.js";

        const String ctCLSSubControlMobileStoreFrontStoreInfo = "SubControlMobileStoreFrontStoreInfo";
        const String ctCLSPanelOverlay                  = "PanelOverlay";
        const String ctCLSStoreInfoPanel                = "StoreInfoPanel";
        const String ctCLSPanelTitleBar                 = "PanelTitleBar";
        const String ctCLSPanelHeader                   = "PanelHeader";
        const String ctCLSCloseButton                   = "CloseButton";
        const String ctCLSContentArea                   = "ContentArea";
        const String ctCLSMapPanel                      = "MapPanel";

        const String ctCLSShopNameDiv                   = "AddButton";
        const String ctCLSAddressDiv                    = "AddressDiv";
        const String ctCLSContactNoDiv                  = "ContactNoDiv";
        const String ctCLSOpeningHourDiv                = "OpeningHourDiv";
        const String ctCLSDetailInfoPanel               = "DetailInfoPanel";
        const String ctCLSInfoIcon                      = "InfoIcon";
        const String ctCLSInfoText                      = "InfoText";

        //const String ctCLSGroupHeading                  = "GroupHeading";
        //const String ctCLSGroupHeader                   = "GroupHeader";
        //const String ctCLSSelection                     = "Selection";
        //const String ctCLSChevron                       = "Chevron";
        //const String ctCLSItemContainer                 = "ItemContainer";
        //const String ctCLSItem                          = "Item";
        
        const String ctCMDClose                         = "@cmd%close";

        public CompositeFormInterface SCI_ParentForm { get; set; }

        SubscriberRow       clSubscriberRow;

        public SubControlMobileStoreFrontStoreInfo(DataRow paSubscriberInfo)
        {
            if (paSubscriberInfo != null)
                clSubscriberRow = new SubscriberRow(paSubscriberInfo);                
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);
            
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetFoundationScriptUrl(ctGoogleMapInterfaceScript));

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_Mobile, ctSubControlMobileStoreFrontStoreInfoStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_Mobile, ctSubControlMobileStoreFrontStoreInfoScript));
        }

        private void RenderMapPanel(ComponentController paComponentController)
        {
            SubControlGoogleMap lcGoogleMap;

            lcGoogleMap = new SubControlGoogleMap();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSMapPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            if ((clSubscriberRow != null) && (clSubscriberRow.Latitude != 0) && (clSubscriberRow.Longitude != 0))
            {
                lcGoogleMap.SC_Longitude = clSubscriberRow.Longitude;
                lcGoogleMap.SC_Latitude = clSubscriberRow.Latitude;
            }

            lcGoogleMap.RenderChildMode(paComponentController);

            paComponentController.RenderEndTag();
        }

        private void RenderDetailInfoPanel(ComponentController paComponentController)
        {
            String lcBusinessName;
            String lcAddress;
            String lcContactNo;
            String lcOperatingHour;

            if (clSubscriberRow != null)
            {
                lcBusinessName = clSubscriberRow.BusinessName;
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

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSDetailInfoPanel);
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
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.envelope));
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

        private void RenderContentArea(ComponentController paComponentController)
        {            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSContentArea);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            RenderMapPanel(paComponentController);
            RenderDetailInfoPanel(paComponentController);
            paComponentController.RenderEndTag();
        }

        private void RenderTitleBar(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPanelTitleBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPanelHeader);
            paComponentController.RenderBeginTag(HtmlTag.Div);


            paComponentController.Write(clSubscriberRow.BusinessName);

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
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "storeinfo");
            paComponentController.AddElementType(ComponentController.ElementType.Panel);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSStoreInfoPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderTitleBar(paComponentController);
            RenderContentArea(paComponentController);            

            paComponentController.RenderEndTag();
        }

        private void RenderBrowserMode(ComponentController paComponentController)
        {            
            IncludeExternalLinkFiles(paComponentController);        
            
            paComponentController.AddElementType(ComponentController.ElementType.PopUp);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSubControlMobileStoreFrontStoreInfo);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.Overlay);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "storeinfo");
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






