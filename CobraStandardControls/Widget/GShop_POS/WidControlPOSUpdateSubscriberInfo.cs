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
    public class WidControlPOSUpdateSubscriberInfo : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlPOSUpdateSubscriberInfoStyle     = "WidControlPOSUpdateSubscriberInfo.css";
        protected const String ctWidControlPOSUpdateSubscriberInfoScript    = "WidControlPOSUpdateSubscriberInfo.js";

        const String ctCLSWidControlPOSUpdateSubscriberInfo     = "WidControlPOSUpdateSubscriberInfo";
        const String ctCLSButtonPanel                           = "ButtonPanel";
        const String ctCLSUpdateButton                          = "UpdateButton";
        const String ctCLSCloseButton                           = "CloseButton";

        const String ctCLSSectionGroup                          = "SectionGroup";
        const String ctCLSSectionHeader                         = "SectionHeader";

        const String ctDYTUpdateButtonText                      = "@@POS.Button.Update";
        const String ctDYTCloseButtonText                       = "@@POS.Button.Close";

        const String ctDYTContactPersonTitle                    = "@@POS.SubscriberInfo.ContactPerson.Title";
        const String ctDYTContactPersonName                     = "@@POS.SubscriberInfo.ContactPerson.Name";
        const String ctDYTContactPersonMobileNo                 = "@@POS.SubscriberInfo.ContactPerson.MobileNo";
        const String ctDYTContactPersonEmail                    = "@@POS.SubscriberInfo.ContactPerson.Email";
        const String ctDYTBusinessInfoTitle                     = "@@POS.SubscriberInfo.BusinessInfo.Title";
        const String ctDYTBusinessName                          = "@@POS.SubscriberInfo.BusinessInfo.BusinessName";
        const String ctDYTIndustryType                          = "@@POS.SubscriberInfo.BusinessInfo.IndustryType";
        const String ctDYTBusinessContactNo                     = "@@POS.SubscriberInfo.BusinessInfo.ContactNo";
        const String ctDYTBusinessEmail                         = "@@POS.SubscriberInfo.BusinessInfo.Email";
        const String ctDYTBusinessAddress                       = "@@POS.SubscriberInfo.BusinessInfo.Address";
        const String ctDYTBusinessTownship                      = "@@POS.SubscriberInfo.BusinessInfo.Township";
        const String ctDYTBusinessCity                          = "@@POS.SubscriberInfo.BusinessInfo.City";
                        
        const String ctCMDUpdate                                = "@cmd%update";
        const String ctCMDClose                                 = "@cmd%close";

        const String ctCOLContactPerson                         = "ContactPerson";
        const String ctCOLContactPersonMobileNo                 = "ContactPersonMobileNo";
        const String ctCOLContactPersonEmail                    = "ContactPersonEmail";

        const String ctCOLBusinessName                          = "BusinessName";
        const String ctCOLIndustryType                          = "IndustryType";
        const String ctCOLEmailAddress                          = "EmailAddress";
        const String ctCOLContactNo                             = "ContactNo";
        const String ctCOLAddressInfo                           = "AddressInfo";
        const String ctCOLTownship                              = "Township";
        const String ctCOLCity                                  = "City";

        const int ctSubscriptionCodeLength                      = 8;

        public CompositeFormInterface SCI_ParentForm { get; set; }

        LanguageManager     clLanguageManager;
        SettingManager      clSettingManager;
        SubscriberRow       clSubscriberRow;

        public WidControlPOSUpdateSubscriberInfo()
        {
            clLanguageManager   = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSettingManager    = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;
            clSubscriberRow     = RetrieveRow();
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSUpdateSubscriberInfoStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSUpdateSubscriberInfoScript));
        }

        private void RenderButtonPanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSUpdateButton);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDUpdate);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(clLanguageManager.GetText(ctDYTUpdateButtonText));
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCloseButton);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDClose);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(clLanguageManager.GetText(ctDYTCloseButtonText));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderInfoHeader(ComponentController paComponentController, String paHeadingText)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSectionHeader);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.Write(clLanguageManager.GetText(paHeadingText));

            paComponentController.RenderEndTag();
        }

        private void RenderInputRow(ComponentController paComponentController, String paColumnName, String paLabel, String paInfoText, String paInputMode, int paMaxLength, bool paMandatory = false)
        {
            String lcLabel;

            lcLabel = clLanguageManager.GetText(paLabel);
            paComponentController.AddElementType(ComponentController.ElementType.InputRow);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.InputLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(lcLabel);
            paComponentController.RenderEndTag();

            paComponentController.AddElementType(ComponentController.ElementType.InputBox);            
            if (paMandatory) paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Mandatory, "true");
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, paColumnName);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Name, lcLabel);            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_InputMode, paInputMode);
            paComponentController.AddAttribute(HtmlAttribute.Maxlength, paMaxLength.ToString());            
            paComponentController.AddAttribute(HtmlAttribute.Value, paInfoText);
            paComponentController.AddAttribute(HtmlAttribute.Type, "text");            
            paComponentController.RenderBeginTag(HtmlTag.Input);                        
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderInputTextAreaRow(ComponentController paComponentController, String paColumnName, String paLabel, String paInfoText, String paInputMode, int paMaxLength)
        {
            String lcLabel;

            lcLabel = clLanguageManager.GetText(paLabel);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Mode, "textarea");
            paComponentController.AddElementType(ComponentController.ElementType.InputRow);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.InputLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(lcLabel);
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, paColumnName);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Name, lcLabel);
            paComponentController.AddElementType(ComponentController.ElementType.InputBox);            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_InputMode, paInputMode);
            paComponentController.AddAttribute(HtmlAttribute.Maxlength, paMaxLength.ToString());                        
            paComponentController.RenderBeginTag(HtmlTag.Textarea);
            paComponentController.Write(paInfoText);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }
        
        
        private void RenderContactPersonPanel(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Block);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSectionGroup);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderInfoHeader(paComponentController, ctDYTContactPersonTitle);
                        
            RenderInputRow(paComponentController, ctCOLContactPerson, ctDYTContactPersonName,  clSubscriberRow.ContactPerson, "text", 100, true);
            RenderInputRow(paComponentController, ctCOLContactPersonMobileNo, ctDYTContactPersonMobileNo,  clSubscriberRow.ContactPersonMobileNo, "phoneno", 100, true);
            RenderInputRow(paComponentController, ctCOLContactPersonEmail,  ctDYTContactPersonEmail, clSubscriberRow.ContactPersonEmail, "email", 100);

            paComponentController.RenderEndTag();
        }

        private void RenderBusinessInfoPanel(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Block);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSectionGroup);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderInfoHeader(paComponentController, ctDYTBusinessInfoTitle);

            RenderInputRow(paComponentController, ctCOLBusinessName, ctDYTBusinessName, clSubscriberRow.BusinessName, "text", 100, true);
            RenderInputRow(paComponentController, ctCOLIndustryType, ctDYTIndustryType, clSubscriberRow.IndustryType, "text", 100);
            RenderInputRow(paComponentController, ctCOLContactNo, ctDYTBusinessContactNo, clSubscriberRow.ContactNo, "phoneno", 100);
            RenderInputRow(paComponentController, ctCOLEmailAddress, ctDYTBusinessEmail, clSubscriberRow.EmailAddress, "email", 100);
            RenderInputTextAreaRow(paComponentController, ctCOLAddressInfo, ctDYTBusinessAddress, clSubscriberRow.AddressInfo, String.Empty, 200);
            RenderInputRow(paComponentController, ctCOLTownship, ctDYTBusinessTownship, clSubscriberRow.Township, String.Empty, 50);
            RenderInputRow(paComponentController, ctCOLCity, ctDYTBusinessCity, clSubscriberRow.City, String.Empty, 50);            
            
            paComponentController.RenderEndTag();
        }

        private SubscriberRow RetrieveRow()
        {
            DataRow lcDataRow;

            if ((lcDataRow = ApplicationFrame.GetInstance().ActiveFormInfoManager.RunRetrieveRow()) != null) return (new SubscriberRow(lcDataRow));
            else return (new SubscriberRow(TableManager.GetInstance().GetNewRow(TableManager.TableType.Subscriber,true)));
        }

        private void RenderBrowserMode(ComponentController paComponentController)
        {
            IncludeExternalLinkFiles(paComponentController);
            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DataID, ApplicationFrame.GetInstance().ActiveSubscription.ActiveRow.SubscriptionID);
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSUpdateSubscriberInfo);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderContactPersonPanel(paComponentController);
            RenderBusinessInfoPanel(paComponentController);

            RenderButtonPanel(paComponentController);
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

