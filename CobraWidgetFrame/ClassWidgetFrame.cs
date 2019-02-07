using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobraFoundation;
using CobraFrame;
using CobraStandardControls;
using CobraWebFrame;

namespace CobraWidgetFrame
{
    public class WidgetRenderingManager : WidgetRenderingController
    {
        public WidgetRenderingManager(CompositeFormInterface paCompositeForm, FormInfoManager paFormInfoManager, ComponentController paComponentController): base(paCompositeForm, paFormInfoManager, paComponentController)
        {

        }

        public override void RenderWidget(String paRenderMode = null)
        {
            String lcRenderMode;            

            lcRenderMode = clFormInfoManager.ActiveRow.RenderMode.ToUpper();

            switch(lcRenderMode)
            {
                case "STANDARDLOGIN"                        : WidgetRenderingEngine<WidControlLogIn>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "STANDARDICONLIST"                     : RenderStandardIconList(); break;
                case "UPDATEPRICELIST"                      : WidgetRenderingEngine<WidControlUpdatePriceList>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "UPDATECONTENT"                        : WidgetRenderingEngine<WidControlUpdateContent>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "IMAGEUPLOADER"                        : WidgetRenderingEngine<WidControlImagerUploader>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "SUBSCRIPTION"                         : WidgetRenderingEngine<WidControlSubscription>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "CREATEUSER"                           : WidgetRenderingEngine<WidControlCreateUser>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "BEITEMLIST"                           : WidgetRenderingEngine<WidControlBEItemList>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "BEADDADJUSTITEM"                      : WidgetRenderingEngine<WidControlBEAddAdjustItem>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "FEBASICSTORE"                         : WidgetRenderingEngine<WidControlFEBasicStore>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "FEBASICSTORELOCATION"                 : WidgetRenderingEngine<WidControlFEBasicStoreLocation>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "FEORDERLIST"                          : WidgetRenderingEngine<WidControlFEOrderList>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "BEORDERLIST"                          : WidgetRenderingEngine<WidControlBEOrderList>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "MOBILESTOREFRONT"                     : WidgetRenderingEngine<WidControlMobileStoreFront>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "MOBILESTOREADDADJUSTINVENTORY"        : WidgetRenderingEngine<WidControlMobileStoreAddAdjustInventory>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "MOBILESTOREINVENTORYLIST"             : WidgetRenderingEngine<WidControlMobileStoreInventoryList>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "QRCODE"                               : WidgetRenderingEngine<WidControlQRCode>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "POSADDADJUSTITEM"                     : WidgetRenderingEngine<WidControlPOSAddAdjustItem>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "POSITEMLIST"                          : WidgetRenderingEngine<WidControlPOSItemList>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "POSUNITLIST"                          : WidgetRenderingEngine<WidControlPOSUnitList>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "POSADDADJUSTUNIT"                     : WidgetRenderingEngine<WidControlPOSAddAdjustUnit>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;                
                case "POSTRANSACTION"                       : WidgetRenderingEngine<WidControlPOSTransaction>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "POSRECEIPTLIST"                       : WidgetRenderingEngine<WidControlPOSReceiptList>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "POSDAILYSTOCKBALANCE"                 : WidgetRenderingEngine<WidControlPOSDailyStockBalance>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "POSDAILYRECEIPT"                      : WidgetRenderingEngine<WidControlPOSDailyReceipt>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "POSUSERLIST"                          : WidgetRenderingEngine<WidControlPOSUserList>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "POSADDADJUSTUSER"                     : WidgetRenderingEngine<WidControlPOSAddAdjustUser>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "POSLOGIN"                             : WidgetRenderingEngine<WidControlPOSLogIn>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "POSRESETPASSWORD"                     : WidgetRenderingEngine<WidControlPOSResetPassword>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "POSCHANGEPASSWORD"                    : WidgetRenderingEngine<WidControlPOSChangePassword>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;                
                case "POSSUBSCRIPTIONINFO"                  : WidgetRenderingEngine<WidControlPOSSubscriptionInfo>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "POSUPDATESUBSCRIBERINFO"              : WidgetRenderingEngine<WidControlPOSUpdateSubscriberInfo>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;                
                case "POSWIDGETRESTRICTION"                 : WidgetRenderingEngine<WidControlPOSWidgetRestriction>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "POSQRCODE"                            : WidgetRenderingEngine<WidControlPOSQRCode>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "POSMONTHLYTRANSACTION"                : WidgetRenderingEngine<WidControlPOSMonthlyTransaction>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "POSSTATICCOSTLIST"                    : WidgetRenderingEngine<WidControlPOSStaticCostList>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "POSSTATICCOSTEDITOR"                  : WidgetRenderingEngine<WidControlPOSStaticCostEditor>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "POSTABLELIST"                         : WidgetRenderingEngine<WidControlPOSTableList>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "POSADDADJUSTTABLE"                    : WidgetRenderingEngine<WidControlPOSAddAdjustTable>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "POSTABLEMANAGER"                      : WidgetRenderingEngine<WidControlPOSTableManager>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;                
                case "POSCONTROLPANEL"                      : WidgetRenderingEngine<WidControlPOSControlPanel>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;                
                case "POSAPPEARANCESETTING"                 : WidgetRenderingEngine<WidControlPOSAppearanceSetting>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "POSPRINTERSETTING"                    : WidgetRenderingEngine<WidControlPOSPrinterSetting>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "POSLANGUAGESETTING"                   : WidgetRenderingEngine<WidControlPOSLanguageSetting>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "POSSTAFFPERMISSIONSETTING"            : WidgetRenderingEngine<WidControlPOSStaffPermissionSetting>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "POSTRANSACTIONSETTING"                : WidgetRenderingEngine<WidControlPOSTransactionSetting>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "POSRECEIPTLAYOUTSETTING"              : WidgetRenderingEngine<WidControlPOSReceiptLayoutSetting>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
                case "POSGENERALSETTING"                    : WidgetRenderingEngine<WidControlPOSGeneralSetting>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;                
                case "_TESTCOMPOSITE"                       : WidgetRenderingEngine<_TestComposite>.CreateInstance(clCompositeForm, clFormInfoManager, clComponentController).RenderWidget(paRenderMode); break;
            }
        }        

        public void RenderStandardIconList()
        {
            WidControlWidgetPanel       lcWidControlWidgetPanel;
            FieldInfoRow                lcFieldInfoRow;    

            lcWidControlWidgetPanel = new WidControlWidgetPanel();


            if ((lcFieldInfoRow = clFormInfoManager.FieldInfoManager.FetchFieldInfoRow("WidgetIcon")) != null)
            {
                lcWidControlWidgetPanel.SC_WidgetList = ApplicationFrame.GetInstance().ActiveSubscription.ActiveWidgetSubscription.ActiveTable;
                lcWidControlWidgetPanel.SC_EffectiveRole = ApplicationFrame.GetInstance().GetEffectiveRoleList();

                lcWidControlWidgetPanel.RenderChildMode(clComponentController);
            }
        }
    }
    
}
