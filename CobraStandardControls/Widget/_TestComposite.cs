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

namespace CobraStandardControls
{
    public class _TestComposite : WebControl, WidgetControlInterface
    {        
        protected const String ctTestCompositeScriptScript = "_TestComposite.js";
                
        //const String ctCLSContainer             = "Container";

        //const String ctCLSUnitListBlock         = "UnitListBlock";
        //const String ctCLSTitle                 = "Title";        

        //const String ctCLSItemList              = "ItemList";        
        //const String ctCLSItemRow               = "ItemRow";
        
        //const String ctCMDRootCategory          = "@cmd%rootcategory";
        //const String ctCMDUpCategory            = "@cmd%upcategory";

        //const String ctCLSButtonPanel           = "ButtonPanel";
        //const String ctCLSEditButtonDiv         = "EditButtonDiv";
        //const String ctCLSDeleteButtonDiv       = "DeleteButtonDiv";
        
        //const String ctICOEditButton             = "edit_pencil.png";
        //const String ctICODeleteButton           = "cross_button.png";
        
        //const String ctCMDEdit                  = "@cmd%edit";
        //const String ctCMDDelete                = "@cmd%delete";
                
        //const String ctTPLAddAdjustUnit         = "FormPOSAddAdjustUnit,FPM_UNITID::$UNITID";        
        
        //const String ctDEFUnitListTitle     =  "Unit Lists";

      //public enum Mode { Classic, Light }

        //public DataTable        SC_ItemList                 { get; set; }
        //public String           SC_UnitListTitle            { get; set; }
        //public Mode             SC_ControlMode              { get; set; }
        //const String ctEPOSScript = "epos-2.9.0.js";
        //const String ctEPOSInterfaceScript = "epos-interface.js";
        public CompositeFormInterface SCI_ParentForm { get; set; }

        public _TestComposite()
        {            
            //SC_ItemList                 = null;
            //SC_UnitListTitle            = ctDEFUnitListTitle;
            //SC_ControlMode              = Mode.Classic;
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            // lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSUnitListStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.System, ctTestCompositeScriptScript));
            //lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetFoundationScriptUrl(ctEPOSScript));
            //lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetFoundationScriptUrl(ctEPOSInterfaceScript));
        }        

        //private void RenderButtonPanel(ComponentController paComponentController)
        //{
        //    paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);
        //    paComponentController.RenderBeginTag(HtmlTag.Div);

        //    paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSEditButtonDiv);
        //    paComponentController.RenderBeginTag(HtmlTag.Div);

        //    paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDEdit);
        //    paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOEditButton));
        //    paComponentController.RenderBeginTag(HtmlTag.Img);
        //    paComponentController.RenderEndTag();

        //    paComponentController.RenderEndTag();

        //    paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSDeleteButtonDiv);
        //    paComponentController.RenderBeginTag(HtmlTag.Div);

        //    paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDDelete);
        //    paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICODeleteButton));
        //    paComponentController.RenderBeginTag(HtmlTag.Img);
        //    paComponentController.RenderEndTag();

        //    paComponentController.RenderEndTag();

        //    paComponentController.RenderEndTag();
        //}

        //private void RenderItemRow(ComponentController paComponentController, POSUnitRow paUnitRow)
        //{
        //    paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItemRow);            
        //    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DataID, paUnitRow.UnitID.ToString());
        //    paComponentController.AddElementType(ComponentController.ElementType.Row);
        //    paComponentController.RenderBeginTag(HtmlTag.Div);
            
        //    paComponentController.RenderBeginTag(HtmlTag.Span);
        //    paComponentController.Write(paUnitRow.UnitName);
        //    paComponentController.RenderEndTag();

        //    RenderButtonPanel(paComponentController);

        //    paComponentController.RenderEndTag();
        //}

        //private void RenderItemList(ComponentController paComponentController)
        //{
        //    POSUnitRow  lcUnitRow;
            
        //    paComponentController.AddElementType(ComponentController.ElementType.List);
        //    paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSItemList);
        //    paComponentController.RenderBeginTag(HtmlTag.Div);

        //    lcUnitRow = new POSUnitRow(null);

        //    if (SC_ItemList != null)
        //    {
        //        for (int lcCount = 0; lcCount < SC_ItemList.Rows.Count; lcCount++)
        //        {
        //            lcUnitRow.Row = SC_ItemList.Rows[lcCount];
        //            RenderItemRow(paComponentController, lcUnitRow);
        //        }
        //    }            

        //    paComponentController.RenderEndTag();
        //}

        //private void RenderUnitListTitle(ComponentController paComponentController)
        //{            
        //    paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTitle);
        //    paComponentController.AddElementType(ComponentController.ElementType.Title);                            
        //    paComponentController.RenderBeginTag(HtmlTag.Div);
                        
        //    paComponentController.Write(SC_UnitListTitle);            
                
        //    paComponentController.RenderEndTag();            
        //}


        //private void RenderUnitListBlock(ComponentController paComponentController)
        //{
        //    paComponentController.AddElementType(ComponentController.ElementType.Block);
        //    paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSUnitListBlock);            
        //    paComponentController.RenderBeginTag(HtmlTag.Div);

        //    RenderUnitListTitle(paComponentController);
        //    RenderItemList(paComponentController);

        //    paComponentController.RenderEndTag();            
        //}        

        private void RenderContainer(ComponentController paComponentController)
        {
            //SubControlPOSItemPanel lcSubControlPOSItemPanel;

            //lcSubControlPOSItemPanel = new SubControlPOSItemPanel();

            //lcSubControlPOSItemPanel.RenderChildMode(paComponentController);
            //SubControlPOSKeyPad lcSubControlPOSKeyPad;
            //SubControlPOSTransactionList lcSubControlPOSTransactionList;

            //lcSubControlPOSKeyPad = new SubControlPOSKeyPad();
            //lcSubControlPOSTransactionList = new SubControlPOSTransactionList();

            //lcSubControlPOSTransactionList.RenderChildMode(paComponentController);
            //lcSubControlPOSKeyPad.RenderChildMode(paComponentController);

//            SubControlPOSReceiptOutput lcReceiptLayout;

         //   lcReceiptLayout = new SubControlPOSReceiptOutput();

         //   lcReceiptLayout.RenderChildMode(paComponentController);

            //SubControlPOSPaymentPanel lcPaymentPanel;

            //lcPaymentPanel = new SubControlPOSPaymentPanel();

            //lcPaymentPanel.RenderChildMode(paComponentController);
           
            //SubControlSelectionPanel lcSelectionPanel;
            //Dictionary<String,String> lcDictionary;

            //lcDictionary = new Dictionary<string,string>();

            //lcDictionary.Add("EPSON","EPSON");
            //lcDictionary.Add("STAR","STAR");
            //lcDictionary.Add("BIXOLON","BIXOLON");

            //lcSelectionPanel = new SubControlSelectionPanel("abc", "wide", "ABC", lcDictionary);
            //lcSelectionPanel.RenderChildMode(paComponentController);
            //lcOptionPanel = new SubControlOptionPanel("abc", "TEST", new String[] { "EPSON", "STAR", "BIXOLON" });
            //lcOptionPanel.RenderChildMode(paComponentController);

            SubControlImageProcessor lcImageUploader;

            lcImageUploader = new SubControlImageProcessor("");
            lcImageUploader.RenderChildMode(paComponentController);
        }        

        protected void RenderBrowserMode(ComponentController paComponentController)
        {
            IncludeExternalLinkFiles(paComponentController);

            
            paComponentController.AddElementType(ComponentController.ElementType.Control);            
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

