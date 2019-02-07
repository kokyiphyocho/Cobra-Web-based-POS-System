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
using System.IO;

namespace CobraStandardControls
{
    public class WidControlPOSAppearanceSetting : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlPOSAppearanceSettingStyle        = "WidControlPOSAppearanceSetting.css";
        protected const String ctWidControlPOSAppearanceSettingScript       = "WidControlPOSAppearanceSetting.js";

        const String ctCLSWidControlPOSAppearanceSetting                    = "WidControlPOSAppearanceSetting";
        const String ctCLSContainer                                         = "Container";
        const String ctCLSButtonPanel                                       = "ButtonPanel";        

        const String ctCLSSettingGroup                                      = "SettingGroup";
        const String ctCLSSectionHeader                                     = "SectionHeader";        
        const String ctCLSImageContainer                                    = "ImageContainer";
        const String ctCLSToggleSwitch                                      = "ToggleSwitch";

        const String ctCLSPopUpOverlay                                      = "PopUpOverlay";
        const String ctCLSPopUpTitle                                        = "PopUpTitle";
        const String ctCLSImageListContainer                                = "ImageListContainer";
                
        const String ctDYTChooseButtonText                                  = "@@POS.Button.Choose";
        const String ctDYTCancelButtonText                                  = "@@POS.Button.Cancel";
                
        const String ctDYTAppearanceTitle                                   = "@@POS.Appearance.AppearanceTitle";                
        const String ctDYTLoginBackground                                   = "@@POS.Appearance.Appearance.LoginBackground";
        const String ctDYTDesktopBackground                                 = "@@POS.Appearance.Appearance.DesktopBackground";
        const String ctDYTAppGroupingTitle                                  = "@@POS.Appearance.AppGroupingTitle";
        const String ctDYTAppFeatureGroup                                   = "@@POS.Appearance.AppGrouping.AppFeature";
        const String ctDYTAppAdminGroup                                     = "@@POS.Appearance.AppGrouping.AppAdmin";
        const String ctDYTReportGroup                                       = "@@POS.Appearance.AppGrouping.Report";
        const String ctDYTSystemSettingGroup                                = "@@POS.Appearance.AppGrouping.SystemSetting";

        const String ctDYTPopUpTitle                                        = "@@POS.Appearance.ImagePopUp.PopUpTitle";        
                
        const String ctBLKAppearance                                        = "appearance";
        const String ctBLKAppGrouping                                       = "appgrouping";

        const String ctGRPAppFeatureGroup                                   = "appfeature";
        const String ctGRPAppAdminGroup                                     = "appadmin";
        const String ctGRPReportGroup                                       = "report";
        const String ctGRPSystemSettingGroup                                = "systemsetting";
                                
        const String ctSETCustomConfig                                      = "_CUSTOMCONFIG";
        const String ctSETWallPaper                                         = "_WALLPAPER";        
        const String ctSETBackgroundCSSList                                 = "_BACKGROUNDCSSLIST";

        const String ctKEYAppGrouping                                       = "appgrouping";
        
        const String ctFRMDesktop                                           = "formdesktop";
        const String ctFRMLogIn                                             = "formposlogin";
                
        const String ctCMDToggle                                            = "@cmd%toggle";
        const String ctCMDShowPopUp                                         = "@cmd%showpopup";

        const String ctCMDPopUpClose                                        = "@cmd%popup.close";
        const String ctCMDPopUpChoose                                       = "@cmd%popup.choose";
        const String ctCMDPopUpCancel                                       = "@cmd%popup.cancel";
        const String ctCMDPopUpImageClick                                   = "@cmd%popup.imageclick";

        const String ctPTHImageServerPath                                   = "/images/background/";
        
        const String ctTransparentImage                                     = "/images/background/transparent.png";
        const String ctImagePattern                                         = "*.jpg";
        const String ctSeparator                                            = ",";

        public CompositeFormInterface       SCI_ParentForm          { get; set; }
                
        LanguageManager                     clLanguageManager;
        SettingManager                      clSettingManager;

        //Dictionary<String, String>          clAppGrouping;
        //Dictionary<String, String>          clWallPaper;
                        
        public WidControlPOSAppearanceSetting()
        {
            clLanguageManager               = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSettingManager                = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;

            //clAppGrouping = General.JSONDeserialize<Dictionary<String, String>>(clSettingManager.GetSettingValue(ctSETApplicationGrouping));
            //clWallPaper   = General.JSONDeserialize<Dictionary<String, String>>(clSettingManager.GetSettingValue(ctSETWallPaper));
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager       = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager     = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSAppearanceSettingStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSAppearanceSettingScript));
        }        

        protected void RenderToggleButton(ComponentController paComponentController, String paType = "")
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, paType);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSToggleSwitch);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDToggle);
            paComponentController.AddElementType(ComponentController.ElementType.Button);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "leftbar");
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "key");
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderSectionHeader(ComponentController paComponentController, String paHeadingText)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSectionHeader);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.Write(clLanguageManager.GetText(paHeadingText));

            paComponentController.RenderEndTag();
        }

        private void RenderToggleButtonRow(ComponentController paComponentController, String paName, String paKeyValue, String paLabel)
        {
            String lcSettingValue;

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "togglebutton");
            paComponentController.AddElementType(ComponentController.ElementType.InputRow);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.InputLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(paLabel));
            paComponentController.RenderEndTag();

            lcSettingValue = clSettingManager.AppGrouping.Contains(paKeyValue.ToLower()).ToString();
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctSETCustomConfig);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Name, paName.ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_KeyValue, paKeyValue);
            paComponentController.AddAttribute(HtmlAttribute.Value, lcSettingValue.ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_OriginalValue, lcSettingValue.ToString().ToLower());
            paComponentController.AddElementType(ComponentController.ElementType.Box);
            RenderToggleButton(paComponentController);

            paComponentController.RenderEndTag();
        }

        private void RenderImageSelectionRow(ComponentController paComponentController, String paColumnName, String paName, String paLabel)
        {
            String lcValue;

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "image");
            paComponentController.AddElementType(ComponentController.ElementType.InputRow);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.InputLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(paLabel));
            paComponentController.RenderEndTag();

            lcValue = clSettingManager.WallPaper.GetData(paName);
                        
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDShowPopUp);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSImageContainer);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, paColumnName);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Name, paName.ToLower());            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_OriginalValue, lcValue);

            if ((!String.IsNullOrEmpty(lcValue)) && (lcValue.ToLower().Contains(ctPTHImageServerPath)))
            {
                paComponentController.AddAttribute(HtmlAttribute.Src, lcValue);
            }
            else
            {
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_desktopbackgroundcss, lcValue);
                paComponentController.AddAttribute(HtmlAttribute.Src, ctTransparentImage);
            }
            paComponentController.RenderBeginTag(HtmlTag.Img);

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }        

        private void RenderAppearancePanel(ComponentController paComponentController)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctBLKAppearance);
            paComponentController.AddElementType(ComponentController.ElementType.Block);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSettingGroup);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderSectionHeader(paComponentController, ctDYTAppearanceTitle);

            RenderImageSelectionRow(paComponentController, ctSETWallPaper, ctFRMDesktop, ctDYTDesktopBackground);
            RenderImageSelectionRow(paComponentController, ctSETWallPaper, ctFRMLogIn, ctDYTLoginBackground);            

            paComponentController.RenderEndTag();
        }

        private void RenderAppGroupingPanel(ComponentController paComponentController)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctBLKAppGrouping);
            paComponentController.AddElementType(ComponentController.ElementType.Block);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSettingGroup);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderSectionHeader(paComponentController, ctDYTAppGroupingTitle);

            RenderToggleButtonRow(paComponentController, ctKEYAppGrouping, ctGRPAppFeatureGroup, ctDYTAppFeatureGroup);
            RenderToggleButtonRow(paComponentController, ctKEYAppGrouping, ctGRPAppAdminGroup, ctDYTAppAdminGroup);
            RenderToggleButtonRow(paComponentController, ctKEYAppGrouping, ctGRPReportGroup, ctDYTReportGroup);
            RenderToggleButtonRow(paComponentController, ctKEYAppGrouping, ctGRPSystemSettingGroup, ctDYTSystemSettingGroup);

            paComponentController.RenderEndTag();
        }       

        private void RenderContainer(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSContainer);
            paComponentController.RenderBeginTag(HtmlTag.Div);
         
            RenderAppearancePanel(paComponentController);
            RenderAppGroupingPanel(paComponentController);

            paComponentController.RenderEndTag();
        }

        private void RenderImagePopUpTitle(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPopUpTitle);
            paComponentController.AddElementType(ComponentController.ElementType.Title);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(clLanguageManager.GetText(ctDYTPopUpTitle));
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDPopUpClose);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(ComponentController.UnicodeStr((int) Fontawesome.remove));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderImageListContainer(ComponentController paComponentController)
        {
            String[] lcImageFileList;
            String[] lcBackgroundCSSList;

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSImageListContainer);
            paComponentController.AddElementType(ComponentController.ElementType.List);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            
            if ((lcBackgroundCSSList = GetBackgroundCSSList()).Length > 0)
            {
                for (int lcCount = 0; lcCount < lcBackgroundCSSList.Length; lcCount++)
                {
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDPopUpImageClick);
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_desktopbackgroundcss, lcBackgroundCSSList[lcCount]);
                    paComponentController.AddAttribute(HtmlAttribute.Src, ctTransparentImage.ToLower());
                    paComponentController.RenderBeginTag(HtmlTag.Img);
                    paComponentController.RenderEndTag();
                }
            }

            if ((lcImageFileList = GetImageFileList()) != null)
            {
                for (int lcCount = 0; lcCount < lcImageFileList.Length; lcCount++)
                {
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDPopUpImageClick);
                    paComponentController.AddAttribute(HtmlAttribute.Src, (ctPTHImageServerPath + Path.GetFileName(lcImageFileList[lcCount])).ToLower());
                    paComponentController.RenderBeginTag(HtmlTag.Img);
                    paComponentController.RenderEndTag();
                }
            }
            
            paComponentController.RenderEndTag();
        }

        private void RenderImagePopUpButtonPanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);            
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDPopUpChoose);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(clLanguageManager.GetText(ctDYTChooseButtonText));
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDPopUpCancel);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(clLanguageManager.GetText(ctDYTCancelButtonText));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderImagePopUp(ComponentController paComponentController)
        {            
            paComponentController.AddElementType(ComponentController.ElementType.Overlay);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPopUpOverlay);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "imagepopup");
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.PopUp);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderImagePopUpTitle(paComponentController);
            RenderImageListContainer(paComponentController);
            RenderImagePopUpButtonPanel(paComponentController);

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();           
        }

        private String[] GetImageFileList()
        {
            try { return(Directory.GetFiles(HttpContext.Current.Server.MapPath(ctPTHImageServerPath), ctImagePattern)); }
            catch { return(null);}
        }

        private String[] GetBackgroundCSSList()
        {
            try { return (clSettingManager.GetSettingValue(ctSETBackgroundCSSList).Split(new String[] { ctSeparator }, StringSplitOptions.RemoveEmptyEntries)); }
            catch { return (null); }
        }      

        private void RenderBrowserMode(ComponentController paComponentController)
        {
            IncludeExternalLinkFiles(paComponentController);
            
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSAppearanceSetting);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderContainer(paComponentController);                        
            paComponentController.RenderEndTag();
            RenderImagePopUp(paComponentController);
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

