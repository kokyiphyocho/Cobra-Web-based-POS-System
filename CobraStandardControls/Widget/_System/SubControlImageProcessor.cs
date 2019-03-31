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
using System.Text.RegularExpressions;

namespace CobraStandardControls
{
    public class SubControlImageProcessor : WebControl, WidgetControlInterface
    {
        const String ctSubControlImageProcessorStyle                    = "SubControlImageProcessor.css";
        const String ctSubControlImageProcessorScript                   = "SubControlImageProcessor.js";        

        const String ctCLSSubControlImageProcessorComposite             = "SubControlImageProcessorComposite";
        const String ctCLSSubControlImageProcessor                      = "SubControlImageProcessor";
        const String ctCLSTitleBar                                      = "TitleBar";

        const String ctCLSButtonBar                                     = "ButtonBar";
        const String ctCLSButtonIcon                                    = "ButtonIcon";
        const String ctCLSButtonIconImage                               = "ButtonIconImage";
        const String ctCLSButtonPanel                                   = "ButtonPanel";
        const String ctCLSHiddenPanel                                   = "HiddenPanel";

        const String ctCLSCanvasPanel                                   = "CanvasPanel";
        const String ctCLSResetDiv                                      = "ResetDiv";
        const String ctCLSMoveUpDiv                                     = "MoveUpDiv";
        const String ctCLSMoveLeftDiv                                   = "MoveLeftDiv";
        const String ctCLSMoveRightDiv                                  = "MoveRightDiv";
        const String ctCLSMoveDownDiv                                   = "MoveDownDiv";
        const String ctCLSZoomOutDiv                                    = "ZoomOutDiv";
        const String ctCLSZoomInDiv                                     = "ZoomInDiv";
        const String ctCLSCanvasDiv                                     = "CanvasDiv";

//        const String ctDYTImageUploaderTitle                            = "@@POS.ImageUploader.Title";

        const String ctCMDPopUpClose                                    = "@popupcmd%popupclose";
        const String ctCMDChooseImage                                   = "@popupcmd%chooseimage";

        const String ctCMDOk                                            = "@popupcmd%ok";
        const String ctCMDCancel                                        = "@popupcmd%cancel";

        const String ctCMDReset                                         = "@popupcmd%reset";
        const String ctCMDMoveUp                                        = "@popupcmd%moveup";
        const String ctCMDMoveLeft                                      = "@popupcmd%moveleft";
        const String ctCMDMoveRight                                     = "@popupcmd%moveright";
        const String ctCMDMoveDown                                      = "@popupcmd%movedown";
        const String ctCMDZoomOut                                       = "@popupcmd%zoomout";
        const String ctCMDZoomIn                                        = "@popupcmd%zoomin";

        const String ctICOUploadImage                                   = "ToolBar_ImageUpload.png";      
  
        const String ctPNLHidden                                = "hiddenpanel";
        const String ctPNLButton                                = "buttonpanel";
        const String ctPNLCanvas                                = "canvaspanel";

        const String ctImageUploader                            = "imageuploader";

        const int ctCanvasWidth                                 = 300;
        const int ctCanvasHeight                                = 100;

        LanguageManager         clLanguageManager;
        SettingManager          clSettingManager;
        String                  clTitle;

        public CompositeFormInterface SCI_ParentForm { get; set; }
        
        public SubControlImageProcessor(String paTitle)
        {
            clTitle = paTitle;

            clSettingManager = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;
            clLanguageManager = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;            
        }        

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.System, ctSubControlImageProcessorStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.System, ctSubControlImageProcessorScript));            
        }

        private void RenderTitleBar(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Title);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTitleBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(clTitle);
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDPopUpClose);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.remove));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        public void RenderToolBar(ComponentController paComponentController)
        {            
            paComponentController.AddElementType(ComponentController.ElementType.ControlBar);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonIcon);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDChooseImage);
            paComponentController.RenderBeginTag(HtmlTag.A);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonIconImage);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetToolBarImageUrl(ctICOUploadImage));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderButtonPanel(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Panel);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctPNLButton);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDCancel);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.remove));
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDOk);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.ok));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }


        private void RenderFileInputPanel(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Panel);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctPNLHidden);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSHiddenPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Type, "file");
            paComponentController.RenderBeginTag(HtmlTag.Input);                      
            paComponentController.RenderEndTag();

            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderMoveButton(ComponentController paComponentController, String paClassName, String paCommand, Fontawesome paIcon)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, paClassName);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, paCommand);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(ComponentController.UnicodeStr((int) paIcon));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }
        private void RenderCanvasPanel(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Panel);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctPNLCanvas);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCanvasPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderMoveButton(paComponentController, ctCLSMoveUpDiv, ctCMDMoveUp, Fontawesome.angle_up);
            RenderMoveButton(paComponentController, ctCLSResetDiv, ctCMDReset, Fontawesome.undo);
            RenderMoveButton(paComponentController, ctCLSMoveLeftDiv, ctCMDMoveLeft, Fontawesome.angle_left);
            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCanvasDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Width, ctCanvasWidth.ToString());
            paComponentController.AddAttribute(HtmlAttribute.Height, ctCanvasHeight.ToString());
            paComponentController.RenderBeginTag(HtmlTag.Canvas);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            RenderMoveButton(paComponentController, ctCLSMoveRightDiv, ctCMDMoveRight, Fontawesome.angle_right);
            RenderMoveButton(paComponentController, ctCLSZoomOutDiv, ctCMDZoomOut, Fontawesome.zoom_out);
            RenderMoveButton(paComponentController, ctCLSMoveDownDiv, ctCMDMoveDown, Fontawesome.angle_down);
            RenderMoveButton(paComponentController, ctCLSZoomInDiv, ctCMDZoomIn, Fontawesome.zoom_in);

            paComponentController.RenderEndTag();
        }        

        private void RenderBrowserMode(ComponentController paComponentController)
        {            
            IncludeExternalLinkFiles(paComponentController);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSubControlImageProcessorComposite);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctImageUploader);                        
            paComponentController.AddElementType(ComponentController.ElementType.Composite);            
            paComponentController.RenderBeginTag(HtmlTag.Div);

            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSubControlImageProcessor);
            paComponentController.AddElementType(ComponentController.ElementType.Panel);          
            paComponentController.RenderBeginTag(HtmlTag.Div);    
           
            RenderTitleBar(paComponentController);
            RenderToolBar(paComponentController);
            RenderCanvasPanel(paComponentController);
            RenderButtonPanel(paComponentController);
            RenderFileInputPanel(paComponentController);
            
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



