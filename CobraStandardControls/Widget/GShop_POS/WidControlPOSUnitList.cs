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
    public class WidControlPOSUnitList : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlPOSUnitListStyle = "WidControlPOSUnitList.css";
        protected const String ctWidControlPOSUnitListScript = "WidControlPOSUnitList.js";
        
        const String ctCLSWidControlPOSUnitList = "WidControlPOSUnitList";
        const String ctCLSContainer             = "Container";

        const String ctCLSUnitListBlock         = "UnitListBlock";
        const String ctCLSTitle                 = "Title";        

        const String ctCLSItemList              = "ItemList";        
        const String ctCLSItemRow               = "ItemRow";
        
        const String ctCMDRootCategory          = "@cmd%rootcategory";
        const String ctCMDUpCategory            = "@cmd%upcategory";

        const String ctCLSButtonPanel           = "ButtonPanel";
        const String ctCLSEditButtonDiv         = "EditButtonDiv";
        const String ctCLSDeleteButtonDiv       = "DeleteButtonDiv";
        
        const String ctICOEditButton             = "edit_pencil.png";
        const String ctICODeleteButton           = "cross_button.png";
        
        const String ctCMDEdit                  = "@cmd%edit";
        const String ctCMDDelete                = "@cmd%delete";

        const String ctTemplateSeparator        = "||";
        const String ctTPLAddAdjustBaseUnit     = "FormPOSAddAdjustBaseUnit,FPM_ControlMode::base;;FPM_UNITID::$UNITID";
        const String ctTPLAddAdjustUnit         = "FormPOSAddAdjustUnit,FPM_ControlMode::custom;;FPM_UNITID::$UNITID";
                
        const String ctDYTUnitListTitle         =  "@@POS.UnitList.Title";

        public enum Mode { Classic, Light }
        
        public CompositeFormInterface SCI_ParentForm { get; set; }
        
        private DataTable       clUnitList;

        private LanguageManager clLanguageManager;
        private SettingManager  clSettingManager;

        public WidControlPOSUnitList()
        {
            clUnitList                  = null;
            clLanguageManager           = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSettingManager            = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSUnitListStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSUnitListScript));
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

        private void RenderItemRow(ComponentController paComponentController, POSUnitRow paUnitRow)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItemRow);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, paUnitRow.UnitType.ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DataID, paUnitRow.UnitID.ToString());
            paComponentController.AddElementType(ComponentController.ElementType.Row);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            
            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(paUnitRow.UnitName);
            paComponentController.RenderEndTag();

            RenderButtonPanel(paComponentController);

            paComponentController.RenderEndTag();
        }

        private void RenderItemList(ComponentController paComponentController)
        {
            POSUnitRow  lcUnitRow;
            
            paComponentController.AddElementType(ComponentController.ElementType.List);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItemList);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            lcUnitRow = new POSUnitRow(null);

            if (clUnitList != null)
            {
                for (int lcCount = 0; lcCount < clUnitList.Rows.Count; lcCount++)
                {
                    lcUnitRow.Row = clUnitList.Rows[lcCount];
                    RenderItemRow(paComponentController, lcUnitRow);
                }
            }            

            paComponentController.RenderEndTag();
        }

        private void RenderUnitListTitle(ComponentController paComponentController)
        {            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTitle);
            paComponentController.AddElementType(ComponentController.ElementType.Title);                            
            paComponentController.RenderBeginTag(HtmlTag.Div);
                        
            paComponentController.Write(clLanguageManager.GetText(ctDYTUnitListTitle));            
                
            paComponentController.RenderEndTag();            
        }


        private void RenderUnitListBlock(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Block);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSUnitListBlock);            
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderUnitListTitle(paComponentController);
            RenderItemList(paComponentController);

            paComponentController.RenderEndTag();            
        }        

        private void RenderContainer(ComponentController paComponentController)
        {            
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSContainer);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderUnitListBlock(paComponentController);

            paComponentController.RenderEndTag(); 
        }        

        protected void RenderBrowserMode(ComponentController paComponentController)
        {
            IncludeExternalLinkFiles(paComponentController);

            clUnitList = ApplicationFrame.GetInstance().ActiveFormInfoManager.RunRetrieveQuery();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Template, ctTPLAddAdjustBaseUnit + ctTemplateSeparator + ctTPLAddAdjustUnit);
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSUnitList);
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

