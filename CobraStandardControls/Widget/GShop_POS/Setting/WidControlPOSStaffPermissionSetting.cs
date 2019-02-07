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
    public class WidControlPOSStaffPermissionSetting : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlPOSStaffPermissionSettingStyle   = "WidControlPOSStaffPermissionSetting.css";
        protected const String ctWidControlPOSStaffPermissionSettingScript  = "WidControlPOSStaffPermissionSetting.js";

        const String ctCLSWidControlPOSStaffPermissionSetting               = "WidControlPOSStaffPermissionSetting";
        const String ctCLSContainer                                         = "Container";
        const String ctCLSSettingGroup                                      = "SettingGroup";
        const String ctCLSSectionHeader                                     = "SectionHeader";
        const String ctCLSToggleSwitch                                      = "ToggleSwitch";

        const String ctDYTStaffPermissionTitle                              = "@@POS.StaffPermissionSetting.StaffPermissionTitle";
        const String ctDYTAllowInventoryFeature                             = "@@POS.StaffPermissionSetting.AllowInventoryFeature";
        const String ctDYTAllowAdjustReceipt                                = "@@POS.StaffPermissionSetting.AllowAdjustReceipt";
        const String ctDYTAllowCancelReceipt                                = "@@POS.StaffPermissionSetting.AllowCancelReceipt";
        const String ctDYTAllowProfitLossView                               = "@@POS.StaffPermissionSetting.AllowProfitLossView";
        const String ctDYTStaffRestrictionTitle                             = "@@POS.StaffPermissionSetting.StaffRestrictionTitle";
        const String ctDYTReceiptAdjustLimitDays                            = "@@POS.StaffPermissionSetting.ReceiptAdjustLimitDays";
        const String ctDYTReceiptCancelLimitDays                            = "@@POS.StaffPermissionSetting.ReceiptCancelLimitDays";
        const String ctDYTReportViewLimitDays                               = "@@POS.StaffPermissionSetting.ReportViewLimitDays";        
        
        const String ctBLKStaffPermission                                   = "staffpermission";
        const String ctBLKStaffRestriction                                  = "staffrestriction";

        const String ctSETStaffPrmissionSetting                             = "POS.StaffPermissionSetting";        

        const String ctKEYAllowAdjustReceipt                                = "allowadjustreceipt";
        const String ctKEYAllowCancelReceipt                                = "allowcancelreceipt";
        const String ctKEYAllowInventoryFeature                             = "allowinventoryfeature";
        const String ctKEYAllowProfitLossView                               = "allowprofitlossview";
        const String ctKEYReceiptAdjustLimitDays                            = "receiptadjustlimitdays";
        const String ctKEYReceiptCancelLimitDays                            = "receiptcancellimitdays";
        const String ctKEYReportViewLimitDays                               = "reportviewlimitdays";
                
        const String ctCMDToggle                                            = "@cmd%toggle";

        const int    ctTextBoxMaxLength                                     = 3;
        
        public CompositeFormInterface SCI_ParentForm { get; set; }

        LanguageManager clLanguageManager;
        SettingManager  clSettingManager;
        
        public WidControlPOSStaffPermissionSetting()
        {
            clLanguageManager   = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSettingManager    = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSStaffPermissionSettingStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSStaffPermissionSettingScript));
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

        private void RenderToggleButtonRow(ComponentController paComponentController, String paName, String paLabel, String paLinkColumn)
        {            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "togglebutton");
            paComponentController.AddElementType(ComponentController.ElementType.InputRow);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.InputLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(paLabel));
            paComponentController.RenderEndTag();
                     
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctSETStaffPrmissionSetting);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Name, paName.ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_LinkColumn, paLinkColumn);
            paComponentController.AddElementType(ComponentController.ElementType.Button);
            RenderToggleButton(paComponentController);

            paComponentController.RenderEndTag();
        }

        private void RenderInputBoxRow(ComponentController paComponentController, String paName, String paLabel, int paMaxLength)
        {            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "input");
            paComponentController.AddElementType(ComponentController.ElementType.InputRow);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.InputLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(paLabel));
            paComponentController.RenderEndTag();

            paComponentController.AddElementType(ComponentController.ElementType.InputBox);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_InputMode, "number");
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, ctSETStaffPrmissionSetting);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Name, paName.ToLower());
            paComponentController.AddAttribute(HtmlAttribute.Maxlength, paMaxLength.ToString());            
            paComponentController.AddAttribute(HtmlAttribute.Type, "text");
            paComponentController.RenderBeginTag(HtmlTag.Input);

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }        

        private void RenderStaffPermissionPanel(ComponentController paComponentController)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctBLKStaffPermission);
            paComponentController.AddElementType(ComponentController.ElementType.Block);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSettingGroup);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderSectionHeader(paComponentController, ctDYTStaffPermissionTitle);

            RenderToggleButtonRow(paComponentController, ctKEYAllowInventoryFeature, ctDYTAllowInventoryFeature, String.Empty);
            RenderToggleButtonRow(paComponentController, ctKEYAllowAdjustReceipt, ctDYTAllowAdjustReceipt, ctKEYReceiptAdjustLimitDays);
            RenderToggleButtonRow(paComponentController, ctKEYAllowCancelReceipt, ctDYTAllowCancelReceipt, ctKEYReceiptCancelLimitDays);                        
            RenderToggleButtonRow(paComponentController, ctKEYAllowProfitLossView, ctDYTAllowProfitLossView, ctKEYReportViewLimitDays);    

            paComponentController.RenderEndTag();
        }

        private void RenderStaffRestrictionPanel(ComponentController paComponentController)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctBLKStaffRestriction);
            paComponentController.AddElementType(ComponentController.ElementType.Block);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSettingGroup);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderSectionHeader(paComponentController, ctDYTStaffRestrictionTitle);

            RenderInputBoxRow(paComponentController, ctKEYReceiptAdjustLimitDays, ctDYTReceiptAdjustLimitDays, ctTextBoxMaxLength);
            RenderInputBoxRow(paComponentController, ctKEYReceiptCancelLimitDays, ctDYTReceiptCancelLimitDays, ctTextBoxMaxLength);
            RenderInputBoxRow(paComponentController, ctKEYReportViewLimitDays, ctDYTReportViewLimitDays, ctTextBoxMaxLength);            

            paComponentController.RenderEndTag();
        }

        private void RenderContainer(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSContainer);
            paComponentController.RenderBeginTag(HtmlTag.Div);
                        
            RenderStaffPermissionPanel(paComponentController);
            RenderStaffRestrictionPanel(paComponentController);

            paComponentController.RenderEndTag();
        }
       
        private void RenderBrowserMode(ComponentController paComponentController)
        {
            String  lcBase64StaffPermission;
            String  lcBase64SystemConfig;

            lcBase64StaffPermission     = General.Base64Encode(clSettingManager.GetSettingValue(ctSETStaffPrmissionSetting));        
            lcBase64SystemConfig        = General.Base64Encode(clSettingManager.SystemConfigStr);

            IncludeExternalLinkFiles(paComponentController);           

            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddElementAttribute(ctSETStaffPrmissionSetting, lcBase64StaffPermission);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_SystemConfig, lcBase64SystemConfig);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSStaffPermissionSetting);
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

