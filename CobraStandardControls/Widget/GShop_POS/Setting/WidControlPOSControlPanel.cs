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

namespace CobraStandardControls
{
    public class WidControlPOSControlPanel: WebControl, WidgetControlInterface
    {
        protected const String ctWidControlPOSControlPanelStyle     = "WidControlPOSControlPanel.css";
        protected const String ctWidControlPOSControlPanelScript    = "WidControlPOSControlPanel.js";

        const String ctCLSWidControlPOSControlPanel                 = "WidControlPOSControlPanel";
        const String ctCLSTitle                                     = "Title";
        const String ctCLSSettingList                               = "SettingList";
        const String ctCLSSRow                                      = "Row";
        const String ctCLSIconDiv                                   = "IconDiv";
        const String ctCLSTextDiv                                   = "TextDiv";
                
        const String ctDYTTitle                                     = "@@POS.Setting.Title";

        const String ctSETSettingOptions                            = "_SETTINGOPTIONS";

        const String ctCMDOpenForm                                  = "@cmd%openform";

        const String ctKEYLabel                                     = "Label";
        const String ctKEYForm                                      = "Form";
        const String ctKEYIcon                                      = "Icon";

        //const int ctSLIName                                         = 0;
        //const int ctSLILabel                                        = 1;
        //const int ctSLIFormName                                     = 2;
        //const int ctSLIIcon                                         = 3;

        //String[,] ctSettingList = new String[,] { {"GeneralSetting",                "@@POS.Setting.GeneralSetting",         "FormPOSGeneralSetting",            "Setting_General.png" }, 
        //                                          {"AppearanceSetting",             "@@POS.Setting.AppearanceSetting",      "FormPOSAppearanceSetting",         "Setting_Appearance.png" }, 
        //                                          {"LanguageSetting",               "@@POS.Setting.LanguageSetting",        "FormPOSLanguageSetting",           "Setting_Language.png" }, 
        //                                          {"StaffPermissionSetting",        "@@POS.Setting.StaffPermissionSetting", "FormPOSStaffPermissionSetting",    "Setting_StaffPermission.png" }, 
        //                                          {"TransactionSetting",            "@@POS.Setting.TransactionSetting",     "FormPOSTransactionSetting",        "Setting_Transaction.png" }, 
        //                                          {"PrinterSetting",                "@@POS.Setting.PrinterSetting",         "FormPOSPrinterSetting",            "Setting_Printer.png" }, 
        //                                          {"ReceiptLayoutSetting",          "@@POS.Setting.ReceiptLayoutSetting",   "FormPOSReceiptLayoutSetting",      "Setting_ReceiptLayout.png" }, 
        //                                          {"ReceiptCustomizationSetting",   "@@POS.Setting.ReceiptLayoutSetting",   "FormPOSReceiptLayoutSetting",      "Setting_ReceiptLayout.png" }, 
        //                                          {"UserSetting",                   "@@POS.Setting.UserSetting",            "FormPOSUserList",                  "Setting_User.png" }, 
        //                                          {"WidgetSetting",                 "@@POS.Setting.WidgetSetting",          "FormPOSWidgetRestriction",         "Setting_Widget.png" }};
        
        public CompositeFormInterface       SCI_ParentForm          { get; set; }
                
        LanguageManager                     clLanguageManager;
        SettingManager                      clSettingManager;
        
        Dictionary<String,dynamic>           clSettingOptions;

        public WidControlPOSControlPanel()
        {
            clLanguageManager               = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSettingManager                = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;

            clSettingOptions                = General.JSONDeserialize<Dictionary<String,dynamic>>(clSettingManager.GetSettingValue(ctSETSettingOptions));
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager       = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager     = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSControlPanelStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSControlPanelScript));
        }       

        private void RenderTitle(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTitle);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.Write(clLanguageManager.GetText(ctDYTTitle));

            paComponentController.RenderEndTag();
        }

        private void RenderSettingRow(ComponentController paComponentController, String paIcon, String paText, String paFormName)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDOpenForm);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_FormName, paFormName);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSRow);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSIconDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetWidgetImageUrl(paIcon));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();
            
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTextDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(paText));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }


        //private int GetSettingIndex(String paSettingName)
        //{
        //    for (int lcCount = 0; lcCount < ctSettingList.GetLength(0); lcCount++)
        //        if (string.Equals(ctSettingList[lcCount, ctSLIName], paSettingName, StringComparison.InvariantCultureIgnoreCase)) return (lcCount);

        //    return (-1);
        //}

        private void RenderSettingConatiner(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSettingList);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            foreach(String lcKey in clSettingOptions.Keys)
            {

                RenderSettingRow(paComponentController, clSettingOptions[lcKey][ctKEYIcon].ToString(), clSettingOptions[lcKey][ctKEYLabel].ToString(), clSettingOptions[lcKey][ctKEYForm].ToString());
               // General.GetJValueStr()
            }

            //for (int lcCount = 0; lcCount < clSettingOptions.Length; lcCount++)
            //{
            //    if ((lcIndex = GetSettingIndex(clSettingOptions[lcCount])) != -1)
            //    {
            //        RenderSettingRow(paComponentController, ctSettingList[lcIndex, ctSLIIcon], ctSettingList[lcIndex, ctSLILabel], ctSettingList[lcIndex, ctSLIFormName]);
            //    }                
            //}
                
            paComponentController.RenderEndTag();
        }

        private void RenderBrowserMode(ComponentController paComponentController)
        {
            IncludeExternalLinkFiles(paComponentController);
            
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSControlPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderTitle(paComponentController);
            RenderSettingConatiner(paComponentController);

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

