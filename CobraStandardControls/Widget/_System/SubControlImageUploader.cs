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
    public class SubControlImageUploader : WebControl, WidgetControlInterface
    {
        const String ctSubControlImageUploaderStyle                     = "SubControlImageUploader.css";
        const String ctSubControlImageUploaderScript                    = "SubControlImageUploader.js";

        const String ctCLSSubControlImageUploaderComposite              = "SubControlImageUploaderComposite";
        const String ctCLSSubControlImageUploader                       = "SubControlImageUploader";
        const String ctCLSTitleBar                                      = "TitleBar";
        const String ctCLSButtonBar                                     = "ButtonBar";
        const String ctCLSButtonIcon                                    = "ButtonIcon";
        const String ctCLSButtonIconImage                               = "ButtonIconImage";


        const String ctDYTImageUploaderTitle                            = "@@POS.ImageUploader.Title";

        const String ctCMDPopUpClose                                    = "@cmd%popupclose";
        const String ctCMDSelectImage                                   = "@cmd%selectimage";
        const String ctCMDUploadImage                                   = "@cmd%uploadimage";

        const String ctICOSelectImage                               = "ToolBar_POSEditUnit.png";
        const String ctICOUploadImage                               = "ToolBar_POSSale.png";            

        //const String ctCLSCalendarBox                             = "CalendarBox";
        //const String ctCLSMonthBar                                = "MonthBar";        
        //const String ctCLSCalendar                                = "Calendar";
        //const String ctCLSHeaderRow                               = "HeaderRow";
        //const String ctCLSDayRow                                  = "DayRow";
        //const String ctCLSCell                                    = "Cell";        
        

        ////const String ctCLSInfoContainer                         = "InfoConatiner";
        ////const String ctCLSInfoRow                               = "InfoRow";
        ////const String ctCLSMessageBar                            = "MessageBar";

        //const String ctCLSButtonPanel                       = "ButtonPanel";
        //const String ctCLSButton                            = "Button";

        //const String ctCMDCellClick                         = "@popupcmd%cellclick";

        //const String ctCMDPrevMonth                         = "@popupcmd%prevmonth";
        //const String ctCMDNextMonth                         = "@popupcmd%nextmonth";

        //const String ctCMDUpdateInfo                        = "@popupcmd%setdate";
        //const String ctCMDCancelInfo                        = "@popupcmd%cancel";

        //const String ctTXTCalendarTitle                     = "@@*.CalendarTitle";
        //const String ctTXTDayOfWeek                         = "@@*.DayOfWeek";
        //const String ctTXTMonthList                         = "@@*.MonthList";

        //const String ctDEFDayOfWeek                         = "SUN,MON,TUE,WED,THU,FRI,SAT";

        //const String ctDLMComma                             = ",";

        //const int    ctCalendarRowCount                     = 6;
        
        const String ctImageUploader                             = "imageuploader";
        
        //String                  clTitle;
        //String                  clMonthList;
        //String[]                clDayOfWeek;

        LanguageManager         clLanguageManager;
        SettingManager          clSettingManager;

        public CompositeFormInterface SCI_ParentForm { get; set; }
        
        public SubControlImageUploader()
        {
            clSettingManager = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;
            clLanguageManager = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;          
        }        

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.System, ctSubControlImageUploaderStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.System, ctSubControlImageUploaderScript));
        }

        private void RenderTitleBar(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Title);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTitleBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.RenderBeginTag(HtmlTag.Span);
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
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDSelectImage);
            paComponentController.RenderBeginTag(HtmlTag.A);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonIconImage);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetToolBarImageUrl(ctICOSelectImage));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonIcon);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDUploadImage);
            paComponentController.RenderBeginTag(HtmlTag.A);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonIconImage);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetToolBarImageUrl(ctICOUploadImage));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        //private void RenderButtonPanel(ComponentController paComponentController)
        //{
        //    paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);
        //    paComponentController.RenderBeginTag(HtmlTag.Div);

        //    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDCancelInfo);
        //    paComponentController.RenderBeginTag(HtmlTag.A);
        //    paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.remove));
        //    paComponentController.RenderEndTag();

        //    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDUpdateInfo);
        //    paComponentController.RenderBeginTag(HtmlTag.A);
        //    paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.ok));
        //    paComponentController.RenderEndTag();            

        //    paComponentController.RenderEndTag();
        //}

        //private void RenderMonthBar(ComponentController paComponentController)
        //{
        //    paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSMonthBar);
        //    paComponentController.RenderBeginTag(HtmlTag.Div);

        //    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDPrevMonth);
        //    paComponentController.RenderBeginTag(HtmlTag.A);
        //    paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.chevron_left));
        //    paComponentController.RenderEndTag();

        //    paComponentController.RenderBeginTag(HtmlTag.Span);            
        //    paComponentController.RenderEndTag();

        //    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDNextMonth);
        //    paComponentController.RenderBeginTag(HtmlTag.A);
        //    paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.chevron_right));
        //    paComponentController.RenderEndTag();

        //    paComponentController.RenderEndTag();
        //}

        //private void RenderCalendarCell(ComponentController paComponentController, String paTextStr, bool paEnableCommand)
        //{
        //    if (paEnableCommand)
        //       paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDCellClick);

        //    paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCell);
        //    paComponentController.RenderBeginTag(HtmlTag.Div);
        //    paComponentController.Write(paTextStr);
        //    paComponentController.RenderEndTag();
        //}

        //private void RenderCalendarHeaderRow(ComponentController paComponentController)
        //{
        //    paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSHeaderRow);
        //    paComponentController.RenderBeginTag(HtmlTag.Div);

        //    for (int lcCount = 0; lcCount < clDayOfWeek.Length; lcCount++)
        //        RenderCalendarCell(paComponentController, clDayOfWeek[lcCount], false);

        //    paComponentController.RenderEndTag();
        //}

        //private void RenderCalendarDayRow(ComponentController paComponentController)
        //{
        //    paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSDayRow);
        //    paComponentController.RenderBeginTag(HtmlTag.Div);

        //    for (int lcCount = 0; lcCount < clDayOfWeek.Length; lcCount++)            
        //        RenderCalendarCell(paComponentController, String.Empty, true);

        //    paComponentController.RenderEndTag();
        //}

        //private void RenderCalendar(ComponentController paComponentController)
        //{
        //    paComponentController.AddElementType(ComponentController.ElementType.Container);
        //    paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCalendar);
        //    paComponentController.RenderBeginTag(HtmlTag.Div);

        //    RenderCalendarHeaderRow(paComponentController);

        //    for (int lcCount = 0; lcCount < ctCalendarRowCount; lcCount++)
        //        RenderCalendarDayRow(paComponentController);

        //    paComponentController.RenderEndTag();
        //}

        //private void RenderCalendarBox(ComponentController paComponentController)
        //{
        //    paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCalendarBox);            
        //    paComponentController.RenderBeginTag(HtmlTag.Div);
        //    RenderMonthBar(paComponentController);
        //    RenderCalendar(paComponentController);
        //    paComponentController.RenderEndTag();
        //}

        private void RenderBrowserMode(ComponentController paComponentController)
        {            
            IncludeExternalLinkFiles(paComponentController);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSubControlImageUploaderComposite);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctImageUploader);                        
            paComponentController.AddElementType(ComponentController.ElementType.Composite);            
            paComponentController.RenderBeginTag(HtmlTag.Div);

            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSubControlImageUploader);
            paComponentController.AddElementType(ComponentController.ElementType.Panel);          
            paComponentController.RenderBeginTag(HtmlTag.Div);    
           
            RenderTitleBar(paComponentController);
            RenderToolBar(paComponentController);
            
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



