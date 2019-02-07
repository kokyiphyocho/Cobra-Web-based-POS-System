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
    public class WidControlPOSGeneralSetting : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlPOSGeneralSettingStyle           = "WidControlPOSGeneralSetting.css";
        protected const String ctWidControlPOSGeneralSettingScript          = "WidControlPOSGeneralSetting.js";

        const String ctCLSWidControlPOSGeneralSetting                       = "WidControlPOSGeneralSetting";
        const String ctCLSContainer                                         = "Container";
        const String ctCLSSettingGroup                                      = "SettingGroup";
        const String ctCLSSectionHeader                                     = "SectionHeader";
        const String ctCLSToggleSwitch                                      = "ToggleSwitch";
        const String ctCLSImageContainer                                    = "ImageContainer";
        const String ctCLSSlideSelectionContainer                           = "SlideSelectionContainer";
        const String ctCLSPopUpOverlay                                      = "PopUpOverlay";
        const String ctCLSPopUpTitle                                        = "PopUpTitle";
        const String ctCLSImageListContainer                                = "ImageListContainer";
        const String ctCLSButtonPanel                                       = "ButtonPanel";

        const String ctDYTChooseButtonText                                  = "@@POS.Button.Choose";
        const String ctDYTCancelButtonText                                  = "@@POS.Button.Cancel";
        
        const String ctDYTInstallerSettingTitle                             = "@@POS.GeneralSetting.InstallerSettingTitle";
        const String ctDYTApplicationFullName                               = "@@POS.GeneralSetting.ApplicationFullName";
        const String ctDYTApplicationShortName                              = "@@POS.GeneralSetting.ApplicationShortName";
        const String ctDYTApplicationIcon                                   = "@@POS.GeneralSetting.ApplicationIcon";
        const String ctDYTRegionalSettigTitle                               = "@@POS.GeneralSetting.RegionalSettingTitle";
        const String ctDYTTimeZone                                          = "@@POS.GeneralSetting.TimeZone";
        const String ctDYTDateFormat                                        = "@@POS.GeneralSetting.DateFormat";
        const String ctDYTLocalNumberMode                                   = "@@POS.GeneralSetting.LocalNumberMode";
        const String ctDYTThousandSeparator                                 = "@@POS.GeneralSetting.ThousandSeparator";
        const String ctDYTTimeZoneListTitle                                 = "@@POS.GeneralSetting.TimeZoneListTitle";
        const String ctDYTDateFormatListTitle                               = "@@POS.GeneralSetting.DateFormatListTitle";

        const String ctDYTPopUpTitle                                        = "@@POS.GeneralSetting.ImagePopUp.PopUpTitle";    
        
        const String ctBLKInstallerSetting                                  = "installersetting";
        const String ctBLKRegionalSetting                                   = "regionalsetting";

        const String ctSETRegionalConfig                                    = "_REGIONALCONFIG";
        
        const String ctTBLAppManifest                                       = "[appmanifest]";

        const String ctCTNContent                                           = "content";
        const String ctCTNExternalComponent                                 = "externalcomponent";
        
        const String ctKEYDateFormat                                        = "dateformat";
        const String ctKEYLocalTimeZoneID                                   = "localtimezoneid";
        const String ctKEYLocalTimeOffset                                   = "localtimeoffset";
        const String ctKEYLocalNumberMode                                   = "localnumbermode";
        const String ctKEYThousandSeparator                                 = "thousandseparator";
        const String ctKEYAppName                                           = "appname";
        const String ctKEYShortName                                         = "shortname";
        const String ctKEYBackEndIcon                                       = "backendicon";
        
        const String ctCMDToggle                                            = "@cmd%toggle";
        const String ctCMDShowPopUp                                         = "@cmd%showpopup";
        const String ctCMDPopUpClose                                        = "@cmd%popup.close";
        const String ctCMDPopUpChoose                                       = "@cmd%popup.choose";
        const String ctCMDPopUpCancel                                       = "@cmd%popup.cancel";
        const String ctCMDPopUpImageClick                                   = "@cmd%popup.imageclick";

        const String ctPNTTimeZone                                          = "timezonelist";
        const String ctPNTDateFormat                                        = "dateformatlist";

        const String ctPanelAppearance                                      = "wide";

        const int    ctAppFullNameMaxLength                                 = 100;
        const int    ctAppShortNameMaxLength                                = 20;

        const String ctPTHImageServerPath                                   = "/images/appicons/";

        const String ctImagePattern                                         = "*.png";
        
        public CompositeFormInterface SCI_ParentForm { get; set; }

        LanguageManager             clLanguageManager;
        SettingManager              clSettingManager;
        Dictionary<String, String>  clAppManifestDictionary;
        
        public WidControlPOSGeneralSetting()
        {
            AppManifestRow lcAppManifestRow;

            clLanguageManager   = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSettingManager    = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;
            
            if ((lcAppManifestRow = AppManifestManager.GetAppManifestRow()) != null)
            {
                clAppManifestDictionary = lcAppManifestRow.Row.Table.Columns.Cast<DataColumn>().ToDictionary(c => c.ColumnName.ToLower(), c => lcAppManifestRow.Row[c].ToString());
            }
            else clAppManifestDictionary = new Dictionary<string, string>();            
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSGeneralSettingStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSGeneralSettingScript));
        }      

        protected void RenderToggleButton(ComponentController paComponentController)
        {            
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

        private void RenderToggleButtonRow(ComponentController paComponentController, String paName, String paLabel)
        {            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "togglebutton");
            paComponentController.AddElementType(ComponentController.ElementType.InputRow);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.InputLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(paLabel));
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctSETRegionalConfig);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Name, paName.ToLower());            
            paComponentController.AddElementType(ComponentController.ElementType.Button);
            RenderToggleButton(paComponentController);

            paComponentController.RenderEndTag();
        }

        private void RenderInputBoxRow(ComponentController paComponentController, String paColumnName, String paName, String paLabel, int paMaxLength)
        {
            if (paMaxLength <= 0)
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Hidden, "true");
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "input");
            paComponentController.AddElementType(ComponentController.ElementType.InputRow);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.InputLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(paLabel));
            paComponentController.RenderEndTag();

            paComponentController.AddElementType(ComponentController.ElementType.InputBox);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, paColumnName);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Name, paName.ToLower());
            paComponentController.AddAttribute(HtmlAttribute.Maxlength, paMaxLength.ToString());            
            paComponentController.AddAttribute(HtmlAttribute.Type, "text");
            paComponentController.RenderBeginTag(HtmlTag.Input);

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderImageSelectionRow(ComponentController paComponentController, String paColumnName, String paName, String paLabel)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "image");
            paComponentController.AddElementType(ComponentController.ElementType.InputRow);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.InputLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(paLabel));
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDShowPopUp);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSImageContainer);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, paColumnName);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Name, paName.ToLower());
            paComponentController.RenderBeginTag(HtmlTag.Img);

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderSlidSelectionRow(ComponentController paComponentController, String paColumnName, String paName, String paLabel)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "slideselection");
            paComponentController.AddElementType(ComponentController.ElementType.InputRow);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.InputLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(paLabel));
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, paColumnName);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Name, paName.ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDShowPopUp);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSlideSelectionContainer);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.ReadOnly, "readonly");
            paComponentController.AddAttribute(HtmlAttribute.Type, "text");
            paComponentController.RenderBeginTag(HtmlTag.Input);

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

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
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.remove));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderImageListContainer(ComponentController paComponentController)
        {
            String[] lcImageFileList;            

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSImageListContainer);
            paComponentController.AddElementType(ComponentController.ElementType.List);
            paComponentController.RenderBeginTag(HtmlTag.Div);

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
            try { return (Directory.GetFiles(HttpContext.Current.Server.MapPath(ctPTHImageServerPath), ctImagePattern)); }
            catch { return (null); }
        }

        private void RenderSelectionPanel(ComponentController paComponentController, String paPanelType, String paTitle, Dictionary<String,String> paItemList)
        {
            SubControlSelectionPanel lcSubControlSelectionPanel;

            lcSubControlSelectionPanel = new SubControlSelectionPanel(paPanelType, ctPanelAppearance, paTitle, paItemList);

            lcSubControlSelectionPanel.RenderChildMode(paComponentController);
        }


        private void RenderInstallerSettingPanel(ComponentController paComponentController)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctBLKInstallerSetting);
            paComponentController.AddElementType(ComponentController.ElementType.Block);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSettingGroup);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderSectionHeader(paComponentController, ctDYTInstallerSettingTitle);

            RenderInputBoxRow(paComponentController, ctTBLAppManifest, ctKEYAppName, ctDYTApplicationFullName, ctAppFullNameMaxLength);
            RenderInputBoxRow(paComponentController, ctTBLAppManifest, ctKEYShortName, ctDYTApplicationShortName, ctAppShortNameMaxLength);
            RenderImageSelectionRow(paComponentController, ctTBLAppManifest, ctKEYBackEndIcon, ctDYTApplicationIcon);

            paComponentController.RenderEndTag();
        }

        private void RenderRegionalSettingPanel(ComponentController paComponentController)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctBLKRegionalSetting);
            paComponentController.AddElementType(ComponentController.ElementType.Block);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSettingGroup);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderSectionHeader(paComponentController, ctDYTRegionalSettigTitle);
            RenderSlidSelectionRow(paComponentController, ctSETRegionalConfig, ctKEYLocalTimeZoneID, ctDYTTimeZone);
            RenderInputBoxRow(paComponentController, ctSETRegionalConfig, ctKEYLocalTimeOffset, null, 0);
            RenderSlidSelectionRow(paComponentController, ctSETRegionalConfig, ctKEYDateFormat, ctDYTDateFormat);
            RenderToggleButtonRow(paComponentController, ctKEYLocalNumberMode, ctDYTLocalNumberMode);
            RenderToggleButtonRow(paComponentController, ctKEYThousandSeparator, ctDYTThousandSeparator);

            paComponentController.RenderEndTag();
        }

        private void RenderExternalComponents(ComponentController paComponentController)
        {
            RenderSelectionPanel(paComponentController, ctPNTTimeZone, ctDYTTimeZoneListTitle, TimeZoneManager.GetInstance().GetTimeZoneDictionary());
            RenderSelectionPanel(paComponentController, ctPNTDateFormat, ctDYTDateFormatListTitle, clSettingManager.GetDateFormatOptionDictionary());
            RenderImagePopUp(paComponentController);
        }

        private void RenderExternalComponentsContainer(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctCTNExternalComponent);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.RenderEndTag();
        }

        private void RenderContainer(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSContainer);
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctCTNContent);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderInstallerSettingPanel(paComponentController);
            RenderRegionalSettingPanel(paComponentController);

            paComponentController.RenderEndTag();
        }
       
        private void RenderBrowserMode(ComponentController paComponentController)
        {
            String  lcBase64RegionalConfig;
            String  lcBase64AppManifestConfig;
            String  lcBase64JSONTable;

            lcBase64RegionalConfig      = General.Base64Encode(clSettingManager.RegionalConfigStr);
            lcBase64AppManifestConfig   = General.Base64Encode(General.JSONSerialize(clAppManifestDictionary));            
            lcBase64JSONTable           = General.Base64Encode(TimeZoneManager.GetInstance().GetJSONTable());

            IncludeExternalLinkFiles(paComponentController);           

            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddBareAttribute(ctSETRegionalConfig, lcBase64RegionalConfig);
            paComponentController.AddBareAttribute(ctTBLAppManifest, lcBase64AppManifestConfig);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_OptionList, lcBase64JSONTable);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSGeneralSetting);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderContainer(paComponentController);
                        
            paComponentController.RenderEndTag();

            RenderExternalComponentsContainer(paComponentController);
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
            if (paRenderMode == null) RenderBrowserMode(paComponentController);
            else if (paRenderMode == "externalcomponent") RenderExternalComponents(paComponentController);
        }

        protected override void Render(HtmlTextWriter paHtmlTextWriter)
        {
            if (!DesignMode) RenderBrowserMode(new ComponentController(paHtmlTextWriter));
            else RenderDesignMode(new ComponentController(paHtmlTextWriter));
        }
    }
}


