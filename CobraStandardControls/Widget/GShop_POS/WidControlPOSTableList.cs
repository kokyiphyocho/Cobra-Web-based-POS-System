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
    public class WidControlPOSTableList : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlPOSTableListStyle = "WidControlPOSTableList.css";
        protected const String ctWidControlPOSTableListScript = "WidControlPOSTableList.js";
        
        const String ctCLSWidControlPOSTableList = "WidControlPOSTableList";
        const String ctCLSContainer              = "Container";
        const String ctCLSTableGroupBlock        = "TableGroupBlock";

        const String ctCLSTableGroupTitle       = "TableGroupTitle";        
        const String ctCLSTitleText             = "TitleText";
        const String ctCLSHomeButtonDiv         = "HomeButtonDiv";
        const String ctCLSUpButtonDiv           = "UpButtonDiv";

        const String ctCLSTableListBlock         = "TableListBlock";        
        const String ctCLSTableRow               = "TableRow";
        
        const String ctCMDRootGroup             = "@cmd%rootgroup";
        const String ctCMDUpGroup               = "@cmd%upgroup";
        const String ctCMDShowGroup             = "@cmd%showgroup";
        
        const String ctCMDEdit                  = "@cmd%edit";
        const String ctCMDDelete                = "@cmd%delete";

        
        const String ctCLSButtonPanel           = "ButtonPanel";
        const String ctCLSEditButtonDiv         = "EditButtonDiv";
        const String ctCLSDeleteButtonDiv       = "DeleteButtonDiv";
             
        const String ctICOEditButton            = "edit_pencil.png";
        const String ctICODeleteButton          = "cross_button.png";

        //const String ctSETTableGroupLimit       = "POS.SystemTableGroupLimit";
        //const String ctSETTableLimit            = "POS.SystemTableLimit";

        const String ctKEYTableGroupLimit       = "tablegrouplimit";
        const String ctKEYTableLimit            = "tablelimit";

        const String ctCOLEntryType              = "EntryType";
        const String ctCOLGroupID                = "GroupID";
        
        const String ctTemplateSeparator         = "||";
        const String ctTPLAddAdjustTable         = "FormPOSAddAdjustTable,FPM_ControlMode::table;;FPM_GroupID::$GROUPID;;FPM_TableID::$TABLEID";
        const String ctTPLAddAdjustTableGroup    = "FormPOSAddAdjustTableGroup,FPM_ControlMode::group;;FPM_GroupID::$GROUPID;;FPM_TableID::$TABLEID";
        
     //   const String ctDYTRootGroupName            = "@@POS.TableList.RootGroupName";

        public CompositeFormInterface SCI_ParentForm    { get; set; }    
        
        Edition     clEdition;
        DataTable   clTableList;
        bool        clAdminUser;
        
        LanguageManager clLanguageManager;
        SettingManager  clSettingManager;
        
        public WidControlPOSTableList()
        {
            clTableList        = null;

            clLanguageManager = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSettingManager  = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;
            clEdition         = ApplicationFrame.GetInstance().ActiveSubscription.GetEdition();
            clAdminUser       = ApplicationFrame.GetInstance().ActiveSessionController.User.IsAdminUser();        
        }
       
        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSTableListStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSTableListScript));
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

        private void RenderTableRow(ComponentController paComponentController, POSTableListRow paTableListRow)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTableRow);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DataID, paTableListRow.TableID.ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDShowGroup);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, paTableListRow.EntryType.ToLower());
            paComponentController.AddElementType(ComponentController.ElementType.Row);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            
            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(paTableListRow.DisplayName);
            paComponentController.RenderEndTag();
                        
            RenderButtonPanel(paComponentController);

            paComponentController.RenderEndTag();
        }

        private void RenderTableList(ComponentController paComponentController, POSTableListRow paTableGroupRow)
        {
            POSTableListRow lcTableListRow;
            DataRow[] lcTableList;

            lcTableList = GetTableList(paTableGroupRow == null ? 0 : paTableGroupRow.TableID);

            paComponentController.AddElementType(ComponentController.ElementType.List);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTableListBlock);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            lcTableListRow = new POSTableListRow(null);

            for (int lcCount = 0; lcCount < lcTableList.Length; lcCount++)
            {
                lcTableListRow.Row = lcTableList[lcCount];
                RenderTableRow(paComponentController, lcTableListRow);
            }
            

            paComponentController.RenderEndTag();
        }
      
        private void RenderTableGroupTitle(ComponentController paComponentController, POSTableListRow paTableGroupRow)
        {            
            if (paTableGroupRow != null)
            {
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTableGroupTitle);
                paComponentController.AddElementType(ComponentController.ElementType.Title);                                
                paComponentController.RenderBeginTag(HtmlTag.Div);

                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDRootGroup);                
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSHomeButtonDiv);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.home));
                paComponentController.RenderEndTag();

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTitleText);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(paTableGroupRow.DisplayName);
                paComponentController.RenderEndTag();

                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDUpGroup);                
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSUpButtonDiv);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.level_up));
                paComponentController.RenderEndTag();
                
                paComponentController.RenderEndTag();                
            }
            else
            {
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTableGroupTitle);
                paComponentController.AddElementType(ComponentController.ElementType.Title);                            
                paComponentController.RenderBeginTag(HtmlTag.Div);

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTitleText);
                paComponentController.RenderBeginTag(HtmlTag.Div);
             //   paComponentController.Write(clLanguageManager.GetText(ctDYTRootGroupName));
                paComponentController.RenderEndTag();
                
                paComponentController.RenderEndTag();
            }
        }

        private void RenderTableGroupBlock(ComponentController paComponentController, POSTableListRow paTableGroupRow)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Block);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTableGroupBlock);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Group, paTableGroupRow != null ? paTableGroupRow.TableID.ToString() : "0");
            
            if (paTableGroupRow != null)
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Parent, paTableGroupRow.GroupID.ToString());

            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderTableGroupTitle(paComponentController, paTableGroupRow);
            RenderTableList(paComponentController, paTableGroupRow);

            paComponentController.RenderEndTag();            
        }

        private DataRow[] GetTableGroupList()        
        {
            return(clTableList.AsEnumerable().Where(r => r.Field<String>(ctCOLEntryType) == "GROUP").ToArray());
        }

        private DataRow[] GetTableList(int paGroupID)
        {
            return (clTableList.AsEnumerable().Where(r => r.Field<int>(ctCOLGroupID) == paGroupID).OrderBy(r => r.Field<String>(ctCOLEntryType)).ToArray());
        }

        private void RenderContainerContent(ComponentController paComponentController)
        {
            DataRow[]           lcCategoryRows;
            POSTableListRow     lcTableListRow;

            lcCategoryRows = GetTableGroupList();

            RenderTableGroupBlock(paComponentController, null);

            lcTableListRow = new POSTableListRow(null);

            for (int lcCount = 0; lcCount < lcCategoryRows.Length; lcCount++)
            {
                lcTableListRow.Row = lcCategoryRows[lcCount];
                RenderTableGroupBlock(paComponentController, lcTableListRow);
            }
        }

        private void RenderContainer(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSContainer);
            paComponentController.RenderBeginTag(HtmlTag.Div);

        //    RenderContainerContent(paComponentController);
                
            paComponentController.RenderEndTag(); // Container
        }
        
        private String GetTemplate()
        {            
            return (ctTPLAddAdjustTable + ctTemplateSeparator + ctTPLAddAdjustTableGroup);                                        
        }

        protected void RenderBrowserMode(ComponentController paComponentController)
        {            
            IncludeExternalLinkFiles(paComponentController);
                             
            SCI_ParentForm.RenderToolBar(paComponentController);
                                    
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_SystemTableLimit, General.ParseInt(clSettingManager.SystemConfig.GetData(ctKEYTableLimit),7).ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_SystemTableGroupLimit, General.ParseInt(clSettingManager.SystemConfig.GetData(ctKEYTableGroupLimit), 0).ToString());            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Template, GetTemplate());
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSTableList);
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
            if (paRenderMode == null) RenderBrowserMode(paComponentController);
            if (paRenderMode == "tablelistcontent")
            {                
                clTableList = ApplicationFrame.GetInstance().ActiveFormInfoManager.RunRetrieveQuery();
                RenderContainerContent(paComponentController);
            }
        }

        protected override void Render(HtmlTextWriter paHtmlTextWriter)
        {
            if (!DesignMode) RenderBrowserMode(new ComponentController(paHtmlTextWriter));
            else RenderDesignMode(new ComponentController(paHtmlTextWriter));
        }
    }
}

