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
    public class WidControlPOSSubscriptionInfo : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlPOSSubscriptionInfoStyle     = "WidControlPOSSubscriptionInfo.css";
        protected const String ctWidControlPOSSubscriptionInfoScript    = "WidControlPOSSubscriptionInfo.js";

        const String ctCLSWidControlPOSSubscriptionInfo                 = "WidControlPOSSubscriptionInfo";
        const String ctCLSButtonPanel                                   = "ButtonPanel";
        const String ctCLSSubscribeButton                               = "SubscribeButton";
        const String ctCLSCloseButton                                   = "CloseButton";

        const String ctCLSInfoGroup                                     = "InfoGroup";
        const String ctCLSInfoHeader                                    = "InfoHeader";

        const String ctDYTSubscribeButtonText                           = "@@POS.Button.Subscribe";
        const String ctDYTCloseButtonText                               = "@@POS.Button.Close";

        const String ctDYTServiceTitle                                  = "@@POS.SubscriptionInfo.ServiceTitle";
        const String ctDYTServiceName                                   = "@@POS.SubscriptionInfo.ServiceName";
        const String ctDYTSubscriptionType                              = "@@POS.SubscriptionInfo.SubscriptionType";
        const String ctDYTSubscriptionCode                              = "@@POS.SubscriptionInfo.SubscriptionCode";
        const String ctDYTServiceDurationTitle                          = "@@POS.SubscriptionInfo.ServiceDurationTitle";
        const String ctDYTActivationDate                                = "@@POS.SubscriptionInfo.ActivationDate";
        const String ctDYTExpiryDate                                    = "@@POS.SubscriptionInfo.ExpiryDate";

        const String ctDTTSubscriptionTypeTemplate                      = "@@*.SubscriptionType.$SUBSCRIPTIONTYPE";
        
        const String ctCMDSubscribe             = "@cmd%subscribe";
        const String ctCMDClose                 = "@cmd%close";
        
        const int    ctSubscriptionCodeLength   = 8;

        public CompositeFormInterface SCI_ParentForm { get; set; }

        LanguageManager             clLanguageManager;
        SettingManager              clSettingManager;
        
        public WidControlPOSSubscriptionInfo()
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

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSSubscriptionInfoStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSSubscriptionInfoScript));
        }

        private void RenderButtonPanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSubscribeButton);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDSubscribe);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(clLanguageManager.GetText(ctDYTSubscribeButtonText));
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCloseButton);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDClose);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(clLanguageManager.GetText(ctDYTCloseButtonText));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }        

        private void RenderInfoHeader(ComponentController paComponentController, String paHeadingText)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInfoHeader);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.Write(clLanguageManager.GetText(paHeadingText));

            paComponentController.RenderEndTag();
        }       

        private void RenderInfoRow(ComponentController paComponentController, String paLabel, String paInfoText)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Row);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.Label);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(paLabel));
            paComponentController.RenderEndTag();

            paComponentController.AddElementType(ComponentController.ElementType.Info);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(paInfoText);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderServiceInfoPanel(ComponentController paComponentController)
        {
            String    lcSubscriptionTypeText;
            String    lcSubscriptionCode;

            paComponentController.AddElementType(ComponentController.ElementType.Block);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInfoGroup);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderInfoHeader(paComponentController, ctDYTServiceTitle);
            
            lcSubscriptionCode     = ApplicationFrame.GetInstance().ActiveSubscription.ActiveRow.SubscriptionID;
            lcSubscriptionCode     = lcSubscriptionCode.Substring(lcSubscriptionCode.Length - ctSubscriptionCodeLength);
            lcSubscriptionTypeText = ctDTTSubscriptionTypeTemplate.Replace("$SUBSCRIPTIONTYPE",ApplicationFrame.GetInstance().ActiveSubscription.ActiveRow.Type.ToUpper());

            RenderInfoRow(paComponentController, ctDYTServiceName,      clLanguageManager.GetText(ApplicationFrame.GetInstance().ActiveEservice.ActiveRow.ServiceName));
            RenderInfoRow(paComponentController, ctDYTSubscriptionType, clLanguageManager.GetText(lcSubscriptionTypeText));
            RenderInfoRow(paComponentController, ctDYTSubscriptionCode,  lcSubscriptionCode);

            paComponentController.RenderEndTag();
        }

        private void RenderServiceDurationPanel(ComponentController paComponentController)
        {
            String lcActivationDate;
            String lcExpiryDate;

            paComponentController.AddElementType(ComponentController.ElementType.Block);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInfoGroup);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderInfoHeader(paComponentController, ctDYTServiceDurationTitle);

            lcActivationDate    = ApplicationFrame.GetInstance().ActiveSubscription.ActiveRow.ActivationDate.ToString(clSettingManager.DateFormatString);
            lcExpiryDate = ApplicationFrame.GetInstance().ActiveSubscription.ActiveRow.ExpiryDate.ToString(clSettingManager.DateFormatString);

            RenderInfoRow(paComponentController, ctDYTActivationDate, clLanguageManager.ConvertNumber(lcActivationDate));
            RenderInfoRow(paComponentController, ctDYTExpiryDate, clLanguageManager.ConvertNumber(lcExpiryDate));            

            paComponentController.RenderEndTag();
        }        

        private void RenderBrowserMode(ComponentController paComponentController)
        {
            IncludeExternalLinkFiles(paComponentController);

            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSSubscriptionInfo);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderServiceInfoPanel(paComponentController);
            RenderServiceDurationPanel(paComponentController);

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

