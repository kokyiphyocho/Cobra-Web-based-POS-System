using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using CobraFrame;
using CobraWebFrame;
using CobraResources;

namespace CobraWebControls
{
    [ToolboxData("<{0}:CompositeMessageBox runat=server></{0}:CompositeMessageBox>")]
    public class CompositeMessageBox : WebControl
    {
        protected const String ctCLSCompositeMessageBox     = "CompositeMessageBox";
        protected const String ctCLSPopUp                   = "PopUp";
        protected const String ctCLSTitle                   = "Title";
        protected const String ctCLSMessageContent          = "MessageContent";
        protected const String ctCLSButtonPanel             = "ButtonPanel";
        protected const String ctCLSButton                  = "Button";
        protected const String ctCLSCloseButton             = "CloseButton";

        protected const String ctIDMessageTitle             = "msg_title";
        protected const String ctIDMessageContent           = "msg_content";
        protected const String ctIDMessageButton1           = "msg_button1";
        protected const String ctIDMessageButton2           = "msg_button2";
        protected const String ctIDCloseButton              = "msg_closebutton";

        protected const String ctButtonHyperLink            = "@msgcmd%buttonclick";
        protected const int    ctDeleteSymbol               = 0xf00d;
        protected const String ctCloseAction                = "cancel";
        
        public CompositeMessageBox()
        {
        
        }
        
        protected virtual void RenderMessageDiv(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSMessageContent);             
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ComponentID, ctIDMessageContent);
            paComponentController.AddAttribute(HtmlAttribute.Id, ctIDMessageContent);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.RenderEndTag();
        }

        protected virtual void RenderButtonPanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);                
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButton);    
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ComponentID, ctIDMessageButton1);
            paComponentController.AddAttribute(HtmlAttribute.Href, ctButtonHyperLink);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButton);                
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ComponentID, ctIDMessageButton2);
            paComponentController.AddAttribute(HtmlAttribute.Href, ctButtonHyperLink);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        protected virtual void RenderCloseButton(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCloseButton);              
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ComponentID, ctIDCloseButton);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Action, ctCloseAction);
            paComponentController.AddAttribute(HtmlAttribute.Href, ctButtonHyperLink);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(ComponentController.UnicodeStr(ctDeleteSymbol));
            paComponentController.RenderEndTag();
            paComponentController.RenderEndTag();
        }

        protected virtual void RenderTitleBar(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTitle);                
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ComponentID, ctIDMessageTitle);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.RenderEndTag();
        }

        protected virtual void RenderPopUp(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPopUp);            
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderTitleBar(paComponentController);
            RenderCloseButton(paComponentController);
            RenderMessageDiv(paComponentController);
            RenderButtonPanel(paComponentController);

            paComponentController.RenderEndTag();
        }

        protected virtual void RenderBrowserMode(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCompositeMessageBox);
            paComponentController.AddElementType(ComponentController.ElementType.MessageBoxTemplate);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderPopUp(paComponentController);

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
