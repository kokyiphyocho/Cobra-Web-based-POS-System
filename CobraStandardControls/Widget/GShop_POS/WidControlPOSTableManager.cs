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
using System.Collections;

namespace CobraStandardControls
{
    public class WidControlPOSTableManager: WebControl, WidgetControlInterface
    {
        protected const String ctWidControlPOSTableManagerStyle     = "WidControlPOSTableManager.css";
        protected const String ctWidControlPOSTableManagerScript    = "WidControlPOSTableManager.js";
        protected const String ctJQueryTouchSwipeScript             = "jquery.touchSwipe.min.js";

        const String ctCLSWidControlPOSTableManager = "WidControlPOSTableManager";
        const String ctCLSContainer                 = "Container";
        const String ctCLSTableGroupBlock           = "TableGroupBlock";

        const String ctCLSTableGroupTitle           = "TableGroupTitle";
        const String ctCLSRefreshButtonDiv          = "RefreshButtonDiv";
        const String ctCLSTitleText                 = "TitleText";
        
        const String ctCLSTableListBlock            = "TableListBlock";
        const String ctCLSTableElement              = "TableElement";
        const String ctCLSInnerArea                 = "InnerArea";
        const String ctCLSTableElementTitle         = "TableElementTitle";
        const String ctCLSTableElementContent       = "TableElementContent";

        const String ctCLSButtonPanel               = "ButtonPanel";        
        
        const String ctCLSNavigationBar             = "NavigationBar";

        const String ctICOSettlementButton      = "cash.png";
        const String ctICODeleteButton          = "recycle_bin.png";
        const String ctICORefresh               = "refresh.png";

        const String ctCOLEntryType             = "EntryType";
        const String ctCOLGroupID               = "GroupID";
        const String ctCOLReference             = "Reference";
                
        const String ctCMDAddOrder              = "@cmd%addorder";
        const String ctCMDMove                  = "@cmd%move";
        const String ctCMDChangeGroup           = "@cmd%changeblock";
        const String ctCMDDelete                = "@cmd%deletebill";
        const String ctCMDSettle                = "@cmd%settlebill";
        const String ctCMDToggleActive          = "@cmd%toggleactive";
        const String ctCMDRefresh               = "@cmd%refresh";

        const String ctTableReferencePrefix     = "TABLE#";
                
        const String ctTPLFormTransaction       = "FormPOSTransaction,FPM_ReceiptType::sale;;FPM_TransactionState::$TRANSACTIONSTATE;;FPM_ReceiptID::$RECEIPTID;;FPM_Reference::$REFERENCE;;FPM_FormTitle::$FORMTITLE";        
                
        const String ctDYQPendingReceiptList    = "EPOS.RetrievePendingReceiptList";

        public CompositeFormInterface SCI_ParentForm { get; set; }
                
        DataTable       clTableList;
        DataTable       clPendingReceiptList;
        
        LanguageManager clLanguageManager;
        SettingManager  clSettingManger;

        public WidControlPOSTableManager()
        {
            clTableList = null;

            clLanguageManager = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSettingManger = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;            
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSTableManagerStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctWidControlPOSTableManagerScript));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetFoundationScriptUrl(ctJQueryTouchSwipeScript));
        }

        private void RenderButtonPanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.Button);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDDelete);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICODeleteButton));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.AddElementType(ComponentController.ElementType.Button);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDSettle);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOSettlementButton));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderTable(ComponentController paComponentController, POSTableListRow paTableListRow)
        {
            POSReceiptRow           lcPOSReceiptRow;
            Decimal                 lcTotalPrice;

            if ((lcPOSReceiptRow = GetPendingReceiptRow(paTableListRow.TableID)) != null)
            {
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_ReceiptID, lcPOSReceiptRow.ReceiptID.ToString());
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Lastmodified, lcPOSReceiptRow.LastModified.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                lcTotalPrice = lcPOSReceiptRow.PaymentCash;
            }
            else lcTotalPrice = 0;

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTableElement);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DataID, paTableListRow.TableID.ToString());            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, paTableListRow.EntryType.ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_Reference, ctTableReferencePrefix + paTableListRow.TableID.ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gpos_DisplayName, paTableListRow.DisplayName);            
            paComponentController.AddElementType(ComponentController.ElementType.Element);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDToggleActive);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInnerArea);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTableElementTitle);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(paTableListRow.DisplayName);
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDAddOrder);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.plus));
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDMove);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.move));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTableElementContent);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.Total);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            if (lcTotalPrice > 0)
                paComponentController.Write(clLanguageManager.ConvertNumber(lcTotalPrice.ToString(clSettingManger.CurrencyFormatString)));

            paComponentController.RenderEndTag();            

            paComponentController.RenderEndTag();            

            RenderButtonPanel(paComponentController);

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderTableList(ComponentController paComponentController, DataRow[] paTableList)
        {
            POSTableListRow     lcTableListRow;
            
            paComponentController.AddElementType(ComponentController.ElementType.List);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTableListBlock);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            lcTableListRow = new POSTableListRow(null);

            for (int lcCount = 0; lcCount < paTableList.Length; lcCount++)
            {
                lcTableListRow.Row = paTableList[lcCount];
                RenderTable(paComponentController, lcTableListRow);
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

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSRefreshButtonDiv);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDRefresh);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.refresh));
                paComponentController.RenderEndTag();

                paComponentController.RenderEndTag();

                
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTitleText);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(paTableGroupRow.DisplayName);
                paComponentController.RenderEndTag();

                paComponentController.RenderEndTag();
            }
            else
            {
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTableGroupTitle);
                paComponentController.AddElementType(ComponentController.ElementType.Title);
                paComponentController.RenderBeginTag(HtmlTag.Div);

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSRefreshButtonDiv);
                paComponentController.RenderBeginTag(HtmlTag.Div);

                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDRefresh);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.refresh));
                paComponentController.RenderEndTag();

                paComponentController.RenderEndTag();

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTitleText);
                paComponentController.RenderBeginTag(HtmlTag.Div);             
                paComponentController.RenderEndTag();

                paComponentController.RenderEndTag();
            }
        }

        private void RenderTableGroupBlock(ComponentController paComponentController, POSTableListRow paTableGroupRow)
        {
            DataRow[]   lcTableList;
            int         lcTableID;
            
            lcTableID = paTableGroupRow == null ? 0 : paTableGroupRow.TableID;
            lcTableList = GetTableList(lcTableID);

            if (lcTableList.Length > 0)
            {
                paComponentController.AddElementType(ComponentController.ElementType.Block);
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTableGroupBlock);
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Group, lcTableID.ToString());
        
                paComponentController.RenderBeginTag(HtmlTag.Div);

                RenderTableGroupTitle(paComponentController, paTableGroupRow);
                RenderTableList(paComponentController, lcTableList);

                paComponentController.RenderEndTag();
            }
            
        }

        private DataRow[] GetTableGroupList()
        {
            return (clTableList.AsEnumerable().Where(r => r.Field<String>(ctCOLEntryType) == "GROUP").ToArray());
        }

        private DataRow[] GetTableList(int paGroupID)
        {
            return (clTableList.AsEnumerable().Where(r => r.Field<int>(ctCOLGroupID) == paGroupID && r.Field<string>(ctCOLEntryType) == "TABLE").OrderBy(r => r.Field<String>(ctCOLEntryType)).ToArray());
        }

        private POSReceiptRow GetPendingReceiptRow(int paTableID)
        {
            DataRow  lcDataRow;

            if ((clPendingReceiptList != null) && (clPendingReceiptList.Rows.Count > 0))
            {
                lcDataRow = clPendingReceiptList.AsEnumerable().Where(r => r.Field<String>(ctCOLReference).ToUpper() == ctTableReferencePrefix + paTableID.ToString()).FirstOrDefault();

                if (lcDataRow != null) return (new POSReceiptRow(lcDataRow));
            }

            return(null);            
        }

        private void RenderContainerContent(ComponentController paComponentController)
        {
            DataRow[]       lcTableGroupList;
            POSTableListRow lcTableListRow;           

            lcTableGroupList = GetTableGroupList();
                  
            lcTableListRow = new POSTableListRow(null);

            if (GetTableList(0).Length > 0) RenderTableGroupBlock(paComponentController, null);

            for (int lcCount = 0; lcCount < lcTableGroupList.Length; lcCount++)
            {
                lcTableListRow.Row = lcTableGroupList[lcCount];
                RenderTableGroupBlock(paComponentController, lcTableListRow);
            }
        }

        private void RenderContainer(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSContainer);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderContainerContent(paComponentController);

            paComponentController.RenderEndTag(); // Container
        }

        private ArrayList GetNavigationButtonList()
        {
            DataRow[]           lcTableGroupList;
            POSTableListRow     lcTableListRow;
            ArrayList           lcButtonList;

            lcButtonList        = new ArrayList();
            lcTableGroupList    = GetTableGroupList();
            lcTableListRow      = new POSTableListRow(null);
                        
            if (GetTableList(0).Length > 0) lcButtonList.Add(new Object[] {0, String.Empty});

            for (int lcCount = 0; lcCount < lcTableGroupList.Length; lcCount++)
            {
                lcTableListRow.Row = lcTableGroupList[lcCount];
                if (GetTableList(lcTableListRow.TableID).Length > 0) lcButtonList.Add(new Object[] {lcTableListRow.TableID, lcTableListRow.DisplayName});                
            }

            return (lcButtonList);
        }

        private void RenderTableGroupBarElement(ComponentController paComponentController, object[] paButtonData)
        {
            if (paButtonData != null)
            {   
                paComponentController.AddElementType(ComponentController.ElementType.Button);
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDChangeGroup);
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Group, paButtonData[0].ToString());
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(paButtonData[1].ToString());
                paComponentController.RenderEndTag();
            }
        }

        private void RenderTableGroupBar(ComponentController paComponentController)
        {
            ArrayList       lcButtonList;

            lcButtonList    =   GetNavigationButtonList();

            paComponentController.AddElementType(ComponentController.ElementType.ControlBar);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSNavigationBar);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Count, lcButtonList.Count.ToString());
            paComponentController.RenderBeginTag(HtmlTag.Div);
                        
            for (int lcCount = 0; lcCount < lcButtonList.Count; lcCount++)                            
                RenderTableGroupBarElement(paComponentController, lcButtonList[lcCount] as object[]);            

            paComponentController.RenderEndTag();
        }
      
        protected void RenderBrowserMode(ComponentController paComponentController)
        {
            IncludeExternalLinkFiles(paComponentController);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Template, ctTPLFormTransaction);
            paComponentController.AddElementType(ComponentController.ElementType.Control);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlPOSTableManager);
            paComponentController.RenderBeginTag(HtmlTag.Div);

           // RenderContainer(paComponentController);
           // RenderTableGroupBar(paComponentController);

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
            else if (paRenderMode == "controlcontent")
            {                
                clTableList = ApplicationFrame.GetInstance().ActiveFormInfoManager.RunRetrieveQuery();
                clPendingReceiptList = DynamicQueryManager.GetInstance().GetDataTableResult(ctDYQPendingReceiptList);

                RenderContainer(paComponentController);
                RenderTableGroupBar(paComponentController);
            }
            else if (paRenderMode == "containercontent")
            {
                clTableList = ApplicationFrame.GetInstance().ActiveFormInfoManager.RunRetrieveQuery();
                clPendingReceiptList = DynamicQueryManager.GetInstance().GetDataTableResult(ctDYQPendingReceiptList);
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

