using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobraWebFrame;
using System.Web.UI;
using System.IO;
using System.Web;
using CobraFrame;
using CobraFoundation;

namespace CobraApplicationFrame
{
    public class CobraPage : System.Web.UI.Page
    {
        const String ctHtmlFormClass    = "HtmlForm";
        const String ctSysRequestError  = "Invalid System Request.";

        public CobraPage()
        {
            ConfigClass.GetInstance().SetStandardUnit(true);
            ApplicationFrame.InitalizeInstance(this);
        }

        protected void InitializePage()
        {
            ApplicationFrame.GetInstance().SetAjaxRequestMode(false);            
            AppManifestManager.AddAndriodManifestLink();
            CSSStyleManager.IncludeStandardStyleSheet();
            JavaScriptManager.IncludeStandardJavaScript();
            CSSStyleManager.InsertEmbeddedFontStyle(ApplicationFrame.GetInstance().GetEmbeddedFontList());
        }

        protected void Page_Load(object paSender, EventArgs paEventArgs)
        {            
            InitializePage();
            OnPageLoadEvent();
        }

        protected virtual void OnPageLoadEvent()
        {
            return; 
        }

        protected override void OnPreRender(EventArgs paEventArgs)
        {
            base.OnPreRender(paEventArgs);            
        }

        protected Control FindMainForm()
        {
            for (int lcCount = 0; lcCount < this.Controls[0].Controls.Count; lcCount++)
                if (this.Controls[0].Controls[lcCount].GetType().Name == ctHtmlFormClass) return (this.Controls[0].Controls[lcCount]);

            return (null);
        }

        protected override void RenderChildren(HtmlTextWriter paHtmlTextWriter)
        {
            Control lcMainForm;

            if ((lcMainForm = FindMainForm()) != null)
            {
                lcMainForm.Controls.Add(ApplicationFrame.GetInstance().ActiveWebStateBlock.WebStateControl());
            }

            base.RenderChildren(paHtmlTextWriter);
        }

        protected override void Render(HtmlTextWriter paHtmlTextWriter)
        {
            base.Render(paHtmlTextWriter);
        }

        protected virtual void AddScript(String paKey, String paScriptFile)
        {
            this.Page.ClientScript.RegisterClientScriptInclude(paKey, paScriptFile);
        }

        private void PerformSystemRequestAction(HttpContext paHttpContext, String paRequest)
        {
            switch(paRequest)
            {
                case "manifest.ios"                 : AppManifestManager.ResponseiOSManifest(paHttpContext); break;
                case "manifest.iosfrontend"         : AppManifestManager.ResponseiOSManifest(paHttpContext, AppManifestManager.Mode.FrontEnd); break;
                case "manifest.iosbackend"          : AppManifestManager.ResponseiOSManifest(paHttpContext, AppManifestManager.Mode.BackEnd); break;
                case "manifest.andriod"             : AppManifestManager.ResponseAndriodManifest(paHttpContext); break;
                case "manifest.andriodfrontend"     : AppManifestManager.ResponseAndriodManifest(paHttpContext, AppManifestManager.Mode.FrontEnd); break;
                case "manifest.andriodbackend"      : AppManifestManager.ResponseAndriodManifest(paHttpContext, AppManifestManager.Mode.BackEnd); break;                
                case "qrcode.andriodfrontend"       : DynamicQRCode.ResponseAndriodFrontEndQRCode(paHttpContext); break;
                case "qrcode.andriodbackend"        : DynamicQRCode.ResponseAndriodBackEndQRCode(paHttpContext); break;
                case "qrcode.iosfrontend"           : DynamicQRCode.ResponseIOSFrontEndQRCode(paHttpContext); break;
                case "qrcode.iosbackend"            : DynamicQRCode.ResponseIOSBackEndQRCode(paHttpContext); break;
                default                             : paHttpContext.Response.Write(ctSysRequestError); break;
            }
        }

        private void PerformServiceProviderRequestAction(HttpContext paHttpContext, String paServiceAuthenticationBlock, String paServiceDataBlock)
        {
            SubscriptionService lcSubscriptionService;

            lcSubscriptionService = SubscriptionService.CreateInstance(paHttpContext, paServiceAuthenticationBlock, paServiceDataBlock);
            lcSubscriptionService.RenderServiceRequest();
        }

        public override void ProcessRequest(HttpContext paHttpContext)
        {
            String lcSysRequest;
            String lcServiceAuthenticationBlock;
            String lcServiceDataBlock;
            CobraAjaxServiceController lcCobraAjaxServiceController;

            ExternalLinkManager.InitializeInstance();

            if ((lcCobraAjaxServiceController = CobraAjaxServiceController.CreateInstance()) != null)
                paHttpContext.Response.Write(lcCobraAjaxServiceController.ExecuteService());
            else
            {
                if (!String.IsNullOrEmpty(lcSysRequest = ApplicationFrame.GetParameter("_sysreq"))) PerformSystemRequestAction(paHttpContext, lcSysRequest);
                else if ((!String.IsNullOrEmpty(lcServiceAuthenticationBlock = ApplicationFrame.GetParameter("_sprauthblock"))) && 
                         (!String.IsNullOrEmpty(lcServiceDataBlock = ApplicationFrame.GetParameter("_sprdatablock"))))
                        PerformServiceProviderRequestAction(paHttpContext, lcServiceAuthenticationBlock, lcServiceDataBlock);
                else base.ProcessRequest(paHttpContext);                
            }

            ApplicationFrame.GetInstance().LogServiceRequestCompleteTime();
        }

    }
}
