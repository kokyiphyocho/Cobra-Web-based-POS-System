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
    [ToolboxData("<{0}:WidControlPOSQRCode runat=server></{0}:WidControlPOSQRCode>")]
    public class WidControlPOSQRCode : WebControl, WidgetControlInterface 
    {
        private const String ctWidControlPOSQRCodeStyle    = "WidControlPOSQRCode.css";
        private const String ctWidControlPOSQRCodeScript   = "WidControlPOSQRCode.js";

        private const String ctCLSWidControlPOSQRCode      = "WidControlPOSQRCode";
        private const String ctCLSElementDiv               = "ElementDiv";
        private const String ctCLSTextDiv                  = "TextDiv";
        private const String ctCLSImage                    = "Image";

        private const String ctDYTAndriodQRCodeText        = "@@POS.QRCode.Andriod";
        private const String ctDYTiOSQRCodeText            = "@@POS.QRCode.iOS";
                
        public enum QRCodeType  { andriod, ios }
        
        public CompositeFormInterface SCI_ParentForm { get; set; }

        LanguageManager     clLanguageManager;
        SettingManager      clSettingManager;
        
        public WidControlPOSQRCode()
        {
            clLanguageManager = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSettingManager = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;
        }              
       
        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;
            
            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSQRCodeStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSQRCodeScript));
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

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Attribute, QRCodeType.andriod.ToString());
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSQRCode);            
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderQRCode(paComponentController, QRCodeType.andriod, clLanguageManager.GetText(ctDYTAndriodQRCodeText), ApplicationFrame.GetInstance().ActiveSubscription.GetSubscriptionUrl(SubscriptionManager.UrlType.AndriodBEQRCode));
            RenderQRCode(paComponentController, QRCodeType.ios, clLanguageManager.GetText(ctDYTiOSQRCodeText), ApplicationFrame.GetInstance().ActiveSubscription.GetSubscriptionUrl(SubscriptionManager.UrlType.iOSBEQRCode));
            
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

