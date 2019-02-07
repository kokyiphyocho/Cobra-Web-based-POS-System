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
    public class WidControlPOSUserList : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlPOSUserListStyle     = "WidControlPOSUserList.css";
        protected const String ctWidControlPOSUserListScript    = "WidControlPOSUserList.js";

        const String ctCLSWidControlPOSUserList                 = "WidControlPOSUserList";
        const String ctCLSContainer                             = "Container";

        const String ctCLSUserListBlock                         = "UserListBlock";
        const String ctCLSTitle                                 = "Title";

        const String ctCLSUserList                              = "UserList";
        const String ctCLSUserRow                               = "UserRow";

        const String ctCLSButtonPanel                           = "ButtonPanel";
        const String ctCLSEditButtonDiv                         = "EditButtonDiv";
        const String ctCLSDeleteButtonDiv                       = "DeleteButtonDiv";

        const String ctICOEditButton                            = "edit_pencil.png";
        const String ctICODeleteButton                          = "cross_button.png";

        const String ctCMDEdit                                  = "@cmd%edit";
        const String ctCMDDelete                                = "@cmd%delete";
        
        const String ctTPLAddAdjustUser                         = "FormPOSAddAdjustUser,FPM_USERID::$USERID";

        const String ctDYTUserListTitle                         = "@@POS.UserList.Title";
                
        public CompositeFormInterface SCI_ParentForm            { get; set; }

        private DataTable clUserList;

        private LanguageManager clLanguageManager;
        private SettingManager  clSettingManger;

        public WidControlPOSUserList()
        {
            clUserList = null;

            clLanguageManager   = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSettingManger     = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSUserListStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSUserListScript));
        }

        private void RenderButtonPanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSEditButtonDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDEdit);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOEditButton));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSDeleteButtonDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDDelete);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICODeleteButton));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderUserRow(ComponentController paComponentController, UserRow paUserRow)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSUserRow);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, paUserRow.Type.ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DataID, paUserRow.UserID.ToString());
            paComponentController.AddElementType(ComponentController.ElementType.Row);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(paUserRow.LoginID);
            paComponentController.RenderEndTag();

            RenderButtonPanel(paComponentController);

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

        private void RenderUserListTitle(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTitle);
            paComponentController.AddElementType(ComponentController.ElementType.Title);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.Write(clLanguageManager.GetText(ctDYTUserListTitle));

            paComponentController.RenderEndTag();
        }

        private void RenderUserListBlock(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Block);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSUserListBlock);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderUserListTitle(paComponentController);
            RenderUserList(paComponentController);

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

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Template, ctTPLAddAdjustUser);
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSUserList);
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

