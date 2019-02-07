using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using CobraFrame;
using CobraResources;
using CobraWebFrame;

namespace CobraWebControls
{
    [ToolboxData("<{0}:CompositeAjaxLoaderPopUp runat=server></{0}:CompositeAjaxLoaderPopUp>")]
    public class CompositeAjaxLoaderPopUp : WebControl
    {
        protected const String ctCLSCompositeAjaxLoaderPopUp = "CompositeAjaxLoaderPopUp";
        protected const String ctCLSPopUp                    = "PopUp";
        protected const String ctCLSTextDiv                  = "TextDiv";
        protected const String ctCLSAjaxImageDiv             = "AjaxImageDiv";
        protected const String ctCLSAjaxImage                = "AjaxImage";

        protected const String ctDEFStatusText = "Loading.............";

        public AjaxManager.AjaxLoaderImage SC_AjaxLoaderImage { get; set; }
        public String SC_DefaultStatusText { get; set; }
        
        public CompositeAjaxLoaderPopUp()
        {
            SC_AjaxLoaderImage = AjaxManager.AjaxLoaderImage.IndicatorBigCircle;
            SC_DefaultStatusText = ctDEFStatusText;
        }        

        protected virtual void RenderBrowserMode(ComponentController paComponentController)
        {               
            paComponentController.AddElementType(ComponentController.ElementType.AjaxLoaderPopUp);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Default, SC_DefaultStatusText);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCompositeAjaxLoaderPopUp);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPopUp);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTextDiv);            
            paComponentController.AddElementType(ComponentController.ElementType.AjaxLoaderStatusDisplay);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write("Testing....");
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSAjaxImageDiv);                
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSAjaxImage);                            
            paComponentController.AddAttribute(HtmlAttribute.Src, AjaxManager.GetInstance().GetAjaxLoaderImageUrl(SC_AjaxLoaderImage));
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_GifDisplayMode, "inline-block");
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        protected virtual void RenderDesignMode(ComponentController paComponentController)
        {
            paComponentController.AddStyle(CSSStyle.Border, "2px Solid Black");
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(this.GetType().ToString());
            paComponentController.RenderEndTag();
        }

        public virtual void RenderChildMode(ComponentController paComponentController)
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
