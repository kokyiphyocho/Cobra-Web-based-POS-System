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
    public class WidControlPOSLanguageSetting : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlPOSLanguageSettingStyle      = "WidControlPOSLanguageSetting.css";
        protected const String ctWidControlPOSLanguageSettingScript     = "WidControlPOSLanguageSetting.js";

        const String ctCLSWidControlPOSLanguageSetting                  = "WidControlPOSLanguageSetting";
        const String ctCLSContainer                                     = "Container";        
        
        const String ctCLSSettingGroup                                  = "SettingGroup";
        const String ctCLSSectionHeader                                 = "SectionHeader";
        const String ctCLSFlag                                          = "Flag";
        const String ctCLSLocalName                                     = "LocalName";
        const String ctCLSApplicationTitleRow                           = "ApplicationTitleRow";
        const String ctCLSTitleText                                     = "TitleText";
        
        const String ctDYTLanguageTitle                                 = "@@POS.LanguageSetting.LanguageTitle";
        const String ctDYTApplicationTitle                              = "@@POS.LanguageSetting.ApplicationTitle";
               
        const String ctBLKLanguage                                      = "language";
        const String ctBLKApplicationTitle                              = "applicationtitle";

        const String ctSETRegionalConfig                                = "_REGIONALCONFIG";
        const String ctSETApplicationTitle                              = "_APPLICATIONTITLE"; 
        const String ctSETLanguageOption                                = "_LANGUAGEOPTIONS";

        const String ctTYPLanguage                                      = "language";
        const String ctTYPApplicationTitle                              = "applicationtitle";

        const String ctKEYDefault                                       = "*";
        const String ctKEYLanguage                                      = "language";

        const String ctCMDLanguage                                      = "@cmd%language";

        const int    ctMaxTitleLength                                   = 150;
        const String ctSeparator                                        = ",";

        public CompositeFormInterface SCI_ParentForm { get; set; }

        LanguageManager clLanguageManager;
        SettingManager clSettingManager;

        Dictionary<String, String>  clApplicationTitle;        
        String[]                    clLanaguageOptionArray;
        DataTable                   clLanaguageList;
        String                      clDefaultSubtitleText;

        public WidControlPOSLanguageSetting()
        {
            clLanguageManager   = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSettingManager    = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;

            clApplicationTitle      = General.JSONDeserialize<Dictionary<String, String>>(clSettingManager.GetSettingValue(ctSETApplicationTitle));            
            clLanaguageOptionArray  = clSettingManager.GetSettingValue(ctSETLanguageOption).Split(ctSeparator[0]);
            clLanaguageList         = clLanguageManager.GetLanguageOptionList();
            clDefaultSubtitleText   = ApplicationFrame.GetInstance().ActiveFormInfoManager.TranslateString(clApplicationTitle.GetData(ctKEYDefault, ""));
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager         lcCSSStyleManager;
            JavaScriptManager       lcJavaScriptmanager;
            
            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSLanguageSettingStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSLanguageSettingScript));
        }

        private LanguageRow GetLanguageRow(String paLanaguage)
        {
            LanguageRow         lcLanguageRow;

            lcLanguageRow = new LanguageRow(null);

            foreach (DataRow lcRow in clLanaguageList.Rows)
            {
                lcLanguageRow.Row = lcRow;
                if (lcLanguageRow.Language == paLanaguage) return (lcLanguageRow);
            }

            return (null);            
        }       

        private void RenderSectionHeader(ComponentController paComponentController, String paHeadingText)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSectionHeader);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.Write(clLanguageManager.GetText(paHeadingText));

            paComponentController.RenderEndTag();
        }       

        private void RenderLanaguageRow(ComponentController paComponentController, String paLanaguage)
        {
            LanguageRow     lcLanguageRow;

            if ((lcLanguageRow = GetLanguageRow(paLanaguage)) != null)
            {
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDLanguage);
                paComponentController.AddAttribute(HtmlAttribute.Value, lcLanguageRow.Language);
                paComponentController.AddElementType(ComponentController.ElementType.InputRow);
                paComponentController.RenderBeginTag(HtmlTag.Div);

                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctTYPLanguage);
                paComponentController.AddElementType(ComponentController.ElementType.Item);
                paComponentController.RenderBeginTag(HtmlTag.Div);

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSFlag);
                paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetLanguageImageUrl(lcLanguageRow.FlagIcon));
                paComponentController.RenderBeginTag(HtmlTag.Img);
                paComponentController.RenderEndTag();

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSLocalName);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(lcLanguageRow.LocalName);
                paComponentController.RenderEndTag();

                paComponentController.RenderEndTag();

                paComponentController.RenderEndTag();
            }
        }

        private void RenderLanguagePanel(ComponentController paComponentController)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctBLKLanguage);
            paComponentController.AddElementType(ComponentController.ElementType.Block);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctSETRegionalConfig);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Name, ctKEYLanguage);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_OriginalValue, clSettingManager.Language);
            paComponentController.AddAttribute(HtmlAttribute.Value, clSettingManager.Language);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSettingGroup);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderSectionHeader(paComponentController, ctDYTLanguageTitle);

            for (int lcCount = 0; lcCount < clLanaguageOptionArray.Length; lcCount++)
            {
                RenderLanaguageRow(paComponentController, clLanaguageOptionArray[lcCount]);
            }                

            paComponentController.RenderEndTag();
        }

        private void RenderApplicationTitleRow(ComponentController paComponentController, String paLanaguage)
        {
            LanguageRow lcLanguageRow;
            String      lcValue;
            
            if ((lcLanguageRow = GetLanguageRow(paLanaguage)) != null)
            {
                lcValue = clApplicationTitle.GetData(lcLanguageRow.Language.ToLower(), clDefaultSubtitleText);

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSApplicationTitleRow);
                paComponentController.AddAttribute(HtmlAttribute.Value, lcLanguageRow.Language);
                paComponentController.AddElementType(ComponentController.ElementType.InputRow);
                paComponentController.RenderBeginTag(HtmlTag.Div);

                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctTYPApplicationTitle);
                paComponentController.AddElementType(ComponentController.ElementType.Item);
                paComponentController.RenderBeginTag(HtmlTag.Div);

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSFlag);
                paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetLanguageImageUrl(lcLanguageRow.FlagIcon));
                paComponentController.RenderBeginTag(HtmlTag.Img);
                paComponentController.RenderEndTag();

                paComponentController.AddElementType(ComponentController.ElementType.InputBox);
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTitleText);
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Dynamic, "true");
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctSETApplicationTitle);
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Name, lcLanguageRow.Language.ToLower());                
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_OriginalValue,  lcValue);                
                paComponentController.AddAttribute(HtmlAttribute.Maxlength, ctMaxTitleLength.ToString());
                paComponentController.AddAttribute(HtmlAttribute.Value, lcValue);
                paComponentController.AddAttribute(HtmlAttribute.Type, "text");
                paComponentController.RenderBeginTag(HtmlTag.Input);
                paComponentController.RenderEndTag();

                paComponentController.RenderEndTag();

                paComponentController.RenderEndTag();
            }
        }

        private void RenderApplicationTitlePanel(ComponentController paComponentController)
        {
            String lcDefaultTitle;
            
            lcDefaultTitle = clApplicationTitle.GetData(ctKEYDefault, "");

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctBLKApplicationTitle);
            paComponentController.AddElementType(ComponentController.ElementType.Block);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctSETApplicationTitle);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Name, ctKEYDefault);            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_OriginalValue, lcDefaultTitle);
            paComponentController.AddAttribute(HtmlAttribute.Value, lcDefaultTitle);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSettingGroup);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderSectionHeader(paComponentController, ctDYTApplicationTitle);

            for (int lcCount = 0; lcCount < clLanaguageOptionArray.Length; lcCount++)
            {
                RenderApplicationTitleRow(paComponentController, clLanaguageOptionArray[lcCount]);
            }

            paComponentController.RenderEndTag();
        }
        
        private void RenderContainer(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSContainer);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderLanguagePanel(paComponentController);
            RenderApplicationTitlePanel(paComponentController);
            
            paComponentController.RenderEndTag();
        }

        private void RenderBrowserMode(ComponentController paComponentController)
        {
            String      lcBase64RegionalConfig;

            IncludeExternalLinkFiles(paComponentController);

            lcBase64RegionalConfig = General.Base64Encode(clSettingManager.RegionalConfigStr);

            paComponentController.AddBareAttribute(ctSETRegionalConfig, lcBase64RegionalConfig);
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSLanguageSetting);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderContainer(paComponentController);
                        
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

