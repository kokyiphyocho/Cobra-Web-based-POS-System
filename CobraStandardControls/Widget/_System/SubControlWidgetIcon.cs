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
using System.Text.RegularExpressions;

namespace CobraStandardControls
{
    [ToolboxData("<{0}:SubCompositeWidget runat=server></{0}:SubCompositeWidget>")]
    public class SubControlWidgetIcon : WebControl
    {
        protected const String ctSubControlWidgetIconStyle = "SubControlWidgetIcon.css";
        protected const String ctSubControlWidgetIconScript = "SubControlWidgetIcon.js";

        const String ctDefaultIcon          = "/images/Widget_Default.png";
        const String ctRegExFormLink        = "^[[](?<FormName>[^]]*)[]]$";
        const String ctGRPFormName          = "FormName";
        const String ctFormLinkTemplate     = "@cmd%formlink?_f=$FORMNAME";
        const String ctOtherLinkTemplate    = "@cmd%link?$LINK";

        const String ctCLSSubControlWidgetIcon  = "SubControlWidgetIcon";
        const String ctCLSWidgetIcon            = "WidgetIcon";
        const String ctCLSWidgetLabel           = "WidgetLabel";

        public String SC_WidgetIcon         { get; set; }
        public String SC_WidgetLabel        { get; set; }
        public String SC_WidgetLink         { get; set; }
        public String SC_WidgetCategory     { get; set; }

        LanguageManager clLanguageManager;

        public SubControlWidgetIcon()
        {
            SC_WidgetIcon = String.Empty;
            SC_WidgetLabel = String.Empty;
            SC_WidgetLink = String.Empty;
            SC_WidgetCategory = String.Empty;

            clLanguageManager = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.System, ctSubControlWidgetIconStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.System, ctSubControlWidgetIconScript));
        }

        protected String GetHyperLink()
        {
            Match lcMatch;

            if ((lcMatch = Regex.Match(SC_WidgetLink, ctRegExFormLink)).Success)
                return (ctFormLinkTemplate.Replace("$FORMNAME", HttpUtility.UrlEncode(General.Base64Encode(lcMatch.Groups[ctGRPFormName].Value))));
            else return (ctOtherLinkTemplate.Replace("$LINK", SC_WidgetLink));
        }

        protected void RenderWidgetIcon(ComponentController paComponentController)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Mode, SC_WidgetCategory.ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, GetHyperLink());
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.WidgetIcon);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidgetIcon);
            paComponentController.AddAttribute(HtmlAttribute.Src, SC_WidgetIcon);
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        protected void RenderWidgetLabel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidgetLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(SC_WidgetLabel));            
            paComponentController.RenderEndTag();
        }

        protected void RenderBrowserMode(ComponentController paComponentController)
        {
            IncludeExternalLinkFiles(paComponentController);

            paComponentController.AddElementType(ComponentController.ElementType.Element);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSubControlWidgetIcon);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            RenderWidgetIcon(paComponentController);
            RenderWidgetLabel(paComponentController); 
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

        public void RenderChildMode(ComponentController paComponentController)
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
