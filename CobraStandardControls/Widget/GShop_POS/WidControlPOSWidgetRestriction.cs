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
    public class WidControlPOSWidgetRestriction : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlPOSWidgetRestrictionStyle    = "WidControlPOSWidgetRestriction.css";
        protected const String ctWidControlPOSWidgetRestrictionScript   = "WidControlPOSWidgetRestriction.js";

        const String ctCLSWidControlPOSWidgetRestriction                = "WidControlPOSWidgetRestriction";
        const String ctCLSContainer                                     = "Container";

        const String ctCLSUserListBlock                                 = "UserListBlock";        
        const String ctCLSTitle                                         = "Title";

        const String ctCLSUserList                                      = "UserList";
        const String ctCLSUserRow                                       = "UserRow";

        const String ctCLSPopUpOverlay                                  = "PopUpOverlay";
        const String ctCLSPopUpTitle                                    = "PopUpTitle";        
        const String ctCLSWidgetList                                    = "WidgetList";
        const String ctCLSWidgetRow                                     = "WidgetRow";

        const String ctCLSToggleSwitch                                  = "ToggleSwitch";
        
        const String ctCLSButtonPanel                                   = "ButtonPanel";
        
        const String ctDYTUserListTitle                                 = "@@POS.UserList.Title";       
        const String ctDYTPopUpTitle                                    = "@@POS.WidgetRestriction.WidgetListPopUp.PopUpTitle";
        const String ctDYTUpdateButtonText                              = "@@POS.Button.Update";
        const String ctDYTCancelButtonText                              = "@@POS.Button.Cancel";

        const String ctFLTType                                          = "Type='WIDGET'";
        const String ctCOLSortPriority                                  = "SortPriority";

        const String ctSETRestrictedWidget                              = "POS.User.$USERID.RestrictedWidgets";

        const String ctCMDShowPopUp                                     = "@cmd%showpopup";
        const String ctCMDPopUpClose                                    = "@cmd%popup.close";
        const String ctCMDPopUpUpdate                                   = "@cmd%popup.update";
        const String ctCMDPopUpCancel                                   = "@cmd%popup.cancel";
        const String ctCMDToggle                                        = "@cmd%popup.toggle";
        
        public CompositeFormInterface SCI_ParentForm { get; set; }

        private DataTable clUserList;

        private LanguageManager     clLanguageManager;
        private SettingManager      clSettingManger;        
        
        public WidControlPOSWidgetRestriction()
        {
            clUserList = null;
            
            clLanguageManager = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSettingManger = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSWidgetRestrictionStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSWidgetRestrictionScript));
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

        private void RenderUserRow(ComponentController paComponentController, UserRow paUserRow)
        {
            String      lcSettingKey;
            String      lcRestrictedWidgetList;

            lcSettingKey = ctSETRestrictedWidget.Replace("$USERID", paUserRow.UserID.ToString());
            lcRestrictedWidgetList = clSettingManger.GetSettingValue(lcSettingKey);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSUserRow);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_KeyColumnName, lcSettingKey);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_KeyValue, lcRestrictedWidgetList);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDShowPopUp);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, paUserRow.Type.ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DataID, paUserRow.UserID.ToString());
            paComponentController.AddElementType(ComponentController.ElementType.Row);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(paUserRow.LoginID);
            paComponentController.RenderEndTag();            

            paComponentController.RenderEndTag();
        }

        private void RenderUserList(ComponentController paComponentController)
        {
            UserRow lcUserRow;

            paComponentController.AddElementType(ComponentController.ElementType.List);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSUserList);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            lcUserRow = new UserRow(null);

            if (clUserList != null)
            {
                for (int lcCount = 0; lcCount < clUserList.Rows.Count; lcCount++)
                {
                    lcUserRow.Row = clUserList.Rows[lcCount];
                    RenderUserRow(paComponentController, lcUserRow);
                }
            }

            paComponentController.RenderEndTag();
        }

        private void RenderBlockTitle(ComponentController paComponentController, String paTitle)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTitle);
            paComponentController.AddElementType(ComponentController.ElementType.Title);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.Write(clLanguageManager.GetText(paTitle));

            paComponentController.RenderEndTag();
        }

        private void RenderUserListBlock(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Block);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSUserListBlock);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderBlockTitle(paComponentController, ctDYTUserListTitle);
            RenderUserList(paComponentController);

            paComponentController.RenderEndTag();
        }      

        private void RenderWidgetListPopUpTitle(ComponentController paComponentController)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Template, clLanguageManager.GetText(ctDYTPopUpTitle));
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

        private void RenderWidgetListContainer(ComponentController paComponentController)
        {
            ViewWidgetSubscriptionRow lcWidgetSubscriptionRow;
            DataRow[] lcWidgetRows;

            lcWidgetRows = ApplicationFrame.GetInstance().ActiveSubscription.ActiveWidgetSubscription.ActiveTable.Select(ctFLTType, ctCOLSortPriority);

            paComponentController.AddElementType(ComponentController.ElementType.List);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidgetList);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            lcWidgetSubscriptionRow = new ViewWidgetSubscriptionRow(null);

            if (lcWidgetRows != null)
            {
                for (int lcCount = 0; lcCount < lcWidgetRows.Length; lcCount++)
                {
                    lcWidgetSubscriptionRow.Row = lcWidgetRows[lcCount];
                    RenderWidgetRow(paComponentController, lcWidgetSubscriptionRow);
                }
            }

            paComponentController.RenderEndTag();
        }

        private void RenderWidgetRow(ComponentController paComponentController, ViewWidgetSubscriptionRow paWidgetRow)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidgetRow);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Name, paWidgetRow.WidgetName);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, paWidgetRow.RequireRole.ToLower());                
            paComponentController.AddElementType(ComponentController.ElementType.Row);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Src, paWidgetRow.Icon);
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(clLanguageManager.GetText(paWidgetRow.IconLabel));
            paComponentController.RenderEndTag();

            RenderToggleButton(paComponentController);

            paComponentController.RenderEndTag();
        }

        private void RenderWidgetListButtonPanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDPopUpUpdate);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(clLanguageManager.GetText(ctDYTUpdateButtonText));
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDPopUpCancel);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(clLanguageManager.GetText(ctDYTCancelButtonText));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderWidgetListPopUp(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Overlay);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPopUpOverlay);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "imagepopup");
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.PopUp);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderWidgetListPopUpTitle(paComponentController);
            RenderWidgetListContainer(paComponentController);
            RenderWidgetListButtonPanel(paComponentController);

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderContainer(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSContainer);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderUserListBlock(paComponentController);        

            paComponentController.RenderEndTag();            
        }        

        protected void RenderBrowserMode(ComponentController paComponentController)
        {
            IncludeExternalLinkFiles(paComponentController);

            clUserList = ApplicationFrame.GetInstance().ActiveFormInfoManager.RunRetrieveQuery();

            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSWidgetRestriction);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderContainer(paComponentController);           

            paComponentController.RenderEndTag();

            RenderWidgetListPopUp(paComponentController);
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

