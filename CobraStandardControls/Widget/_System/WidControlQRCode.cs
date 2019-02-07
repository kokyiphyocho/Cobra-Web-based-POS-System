using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CobraFrame;
using CobraFoundation;
using CobraWebFrame;
using CobraResources;

namespace CobraStandardControls
{
    [ToolboxData("<{0}:WidControlQRCode runat=server></{0}:WidControlQRCode>")]
    public class WidControlQRCode : WebControl, WidgetControlInterface 
    {
        private const String ctWidControlQRCodeStyle       = "WidControlQRCode.css";
        private const String ctWidControlQRCodeScript      = "WidControlQRCode.js";

        private const String ctCLSWidControlQRCode         = "WidControlQRCode";
        private const String ctCLSElementDiv               = "ElementDiv";
        private const String ctCLSTextDiv                  = "TextDiv";
        private const String ctCLSImage                    = "Image";

        private const String ctDEFAndriodFrontEndText       = "Andriod Front-End QR Code";
        private const String ctDEFAndriodBackEndText        = "Andriod Back-End QR Code";
        private const String ctDEFiOSFrontEndText           = "iOS Front-End QR Code";
        private const String ctDEFiOSBackEndText            = "iOS Back-End QR Code";
                
        public enum QRCodeType  { andriodfrontend, andriodbackend, iosfrontend, iosbackend }
        
        public CompositeFormInterface SCI_ParentForm { get; set; }

        public String SC_AndriodFrontEndText           { get; set; }
        public String SC_AndriodBackEndText            { get; set; }
        public String SC_iOSFrontEndText               { get; set; }
        public String SC_iOSBackEndText                { get; set; }

        public WidControlQRCode()
        {
            SC_AndriodFrontEndText  = ctDEFAndriodFrontEndText;
            SC_AndriodBackEndText   = ctDEFAndriodBackEndText;
            SC_iOSFrontEndText      = ctDEFiOSFrontEndText;
            SC_iOSBackEndText       = ctDEFiOSBackEndText;        
        }              
       
        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;
            
            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.System, ctWidControlQRCodeStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.System, ctWidControlQRCodeScript));
        }

        private void RenderQRCode(ComponentController paComponentController, QRCodeType paQRCodeType, String paText, String paImageSource)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, paQRCodeType.ToString());
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSElementDiv);
            paComponentController.AddElementType(ComponentController.ElementType.Element);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTextDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(paText);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSImage);
            paComponentController.AddAttribute(HtmlAttribute.Src, paImageSource);
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();


        }

        private void RenderBrowserMode(ComponentController paComponentController)
        {
            IncludeExternalLinkFiles(paComponentController);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Attribute, QRCodeType.andriodfrontend.ToString());
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlQRCode);            
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderQRCode(paComponentController, QRCodeType.andriodfrontend, SC_AndriodFrontEndText, ApplicationFrame.GetInstance().ActiveSubscription.GetSubscriptionUrl(SubscriptionManager.UrlType.AndriodFEQRCode));
            RenderQRCode(paComponentController, QRCodeType.iosfrontend, SC_iOSFrontEndText, ApplicationFrame.GetInstance().ActiveSubscription.GetSubscriptionUrl(SubscriptionManager.UrlType.iOSFEQRCode));

            if (ApplicationFrame.GetInstance().ActiveSubscription.ActiveMode == SubscriptionManager.Mode.BackEnd)
            {
                RenderQRCode(paComponentController, QRCodeType.andriodbackend, SC_AndriodBackEndText, ApplicationFrame.GetInstance().ActiveSubscription.GetSubscriptionUrl(SubscriptionManager.UrlType.AndriodBEQRCode));
                RenderQRCode(paComponentController, QRCodeType.iosbackend, SC_iOSBackEndText, ApplicationFrame.GetInstance().ActiveSubscription.GetSubscriptionUrl(SubscriptionManager.UrlType.iOSBEQRCode));
            }
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
