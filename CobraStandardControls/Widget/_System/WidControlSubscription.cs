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
    public class WidControlSubscription : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlSubscriptionStyle = "WidControlSubscription.css";
        protected const String ctWidControlSubscriptionScript = "WidControlSubscription.js";

        const String ctCLSWidControlSubscription = "WidControlSubscription";
        const String ctCLSTitlePanel        = "TitlePanel";
        const String ctCLSImageContainer    = "ImageContainer";
        const String ctCLSTextContainer     = "TextContainer";
        const String ctCLSImage             = "Image";
        const String ctCLSNameDiv           = "NameDiv";
        const String ctCLSBriefDiv          = "BriefDiv";
        const String ctCLSPricingDiv        = "PricingDiv";
        const String ctCLSPriceLabelDiv     = "PriceLabelDiv";
        const String ctCLSAmountDiv         = "AmountDiv";
        const String ctCLSRemarkDiv         = "RemarkDiv";
        const String ctCLSDescriptionDiv    = "DescriptionDiv";
        const String ctCLSButtonPanel       = "ButtonPanel";
        const String ctCLSButton            = "Button";        

        const String ctCMDSubscribe         = "@cmd%subscribe";

        const String ctCurrencyCode         = "MMK";

        public CompositeFormInterface SCI_ParentForm { get; set; }

        public WidgetRow    SC_WidgetRow                { get; set;}
        public String       SC_ButtonText               { get; set; }
        public String       SC_SetupFeeLabel            { get; set; }
        public String       SC_SubscriptionFeeLabel     { get; set; }
        public String       SC_MonthlyBillingCycleText  { get; set; }
        public String       SC_YearlyBillingCycleText   { get; set; }
        public String       SC_OtherBillingCycleText    { get; set; }
        public String       SC_Remark                   { get; set; }

        public WidControlSubscription()
        {
            SC_WidgetRow                = null;            
            SC_ButtonText               = String.Empty;
            SC_SetupFeeLabel            = String.Empty;
            SC_SubscriptionFeeLabel     = String.Empty;

            SC_SubscriptionFeeLabel     = String.Empty;
            SC_MonthlyBillingCycleText  = String.Empty;
            SC_YearlyBillingCycleText   = String.Empty;
            SC_OtherBillingCycleText    = String.Empty;
            SC_Remark                   = String.Empty;
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.System, ctWidControlSubscriptionStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.System, ctWidControlSubscriptionScript));
        }        

        private void RenderTitlePanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTitlePanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSImageContainer);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSImage);
            paComponentController.AddAttribute(HtmlAttribute.Src, SC_WidgetRow.Icon);
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag(); // Image Container


            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTextContainer);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSNameDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(SC_WidgetRow.DisplayName);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSBriefDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(SC_WidgetRow.BriefDescription);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag(); // Text Container

            paComponentController.RenderEndTag(); // Title Panel
        }

        private void RenderDescriptionDiv(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSDescriptionDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(SC_WidgetRow.DetailDescription);
            paComponentController.RenderEndTag();
        }


        private void RenderPricingDiv(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPricingDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPriceLabelDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(SC_SetupFeeLabel);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSAmountDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(SC_WidgetRow.SetupFee.ToString("F0"));
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPriceLabelDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(SC_SubscriptionFeeLabel);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSAmountDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(SC_WidgetRow.SubscriptionFee.ToString("F0"));
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSRemarkDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(SC_Remark);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderButtonPanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButton);
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDSubscribe);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(SC_ButtonText);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }
       
        protected void RenderBrowserMode(ComponentController paComponentController)
        {
            if (SC_WidgetRow != null)
            {
                IncludeExternalLinkFiles(paComponentController);

                paComponentController.AddElementType(ComponentController.ElementType.Control);
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlSubscription);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                RenderTitlePanel(paComponentController);
                RenderDescriptionDiv(paComponentController);
                RenderPricingDiv(paComponentController);
                RenderButtonPanel(paComponentController);
                paComponentController.RenderEndTag();
            }
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
