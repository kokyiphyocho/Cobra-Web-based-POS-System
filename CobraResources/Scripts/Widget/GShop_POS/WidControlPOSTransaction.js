$(document).ready(function () {
    POSTransactionManager.Init($('[sa-elementtype=control].WidControlPOSTransaction'));
});

var POSTransactionManager = (function () {
    var clPOSTransactionControl;
    var clKeypad;
    var clKeypadController;
    var clItemPanel;
    var clTransactionList;    
    var clLowerBoundDays;
    var clUpperBoundDays;
    var clLowerBoundDate;
    var clUpperBoundDate;
    var clCustomerInfoPopUp;
    var clCalendarPopUp;
    var clHeaderBar;
    var clDateBox;
    var clMasterBlockDateBox;
    var clMasterBlock;
    var clMode;
    var clTransactionMode;
    var clShowPaymentForm;
    var clReceiptPrintMode;
    var clExistingRecord;
    var clExternalComponentContainer;
    var clTransactionState;
    var clPrinterStatusControl;
    var clTransactionSettingConfig;
    var clStaffPermissionSettingConfig;
    var clSystemConfig;
    var clRegionalConfig;
    var clAdminMode;

    return {
        Init: function (paPOSTransactionControl) {
            clPOSTransactionControl         = paPOSTransactionControl;
            clKeypad                        = paPOSTransactionControl.find('[sa-elementtype=composite][ea-type=poskeypad]');   
            clTransactionListPanel          = $('[sa-elementtype=composite].SubControlPOSTransactionList');                                                                                        
            clTransactionList               = paPOSTransactionControl.find('[sa-elementtype=composite][ea-type=transactionlist]');                      
            clHeaderBar                     = clPOSTransactionControl.find('[sa-elementtype=controlbar].HeaderBar');
            clDateBox                       = clHeaderBar.find('[ea-columnname=receiptdate]');
            clMasterBlock                   = clPOSTransactionControl.find('.MasterBlock[sa-elementtype=container]');            
            clMasterBlockDateBox            = clMasterBlock.find('[ea-columnname=receiptdate]');            
            clExternalComponentContainer    = $('[sa-elementtype=container][ea-type=externalcomponent]');
            clMode                          = clPOSTransactionControl.attr('ea-mode');
            clTransactionMode               = (clMode != 'stockin') && (clMode != 'stockout');            
            clExistingRecord                = (clPOSTransactionControl.attr('ea-dataid') != -1);            
            clTransactionState              = (clPOSTransactionControl.attr('gpos-transactionstate') || 'normal');
            clPrinterStatusControl          = clPOSTransactionControl.find('[sa-elementtype=title] div[sa-elementtype=statuscontrol]');
            clAdminMode                     = (clPOSTransactionControl.attr('ea-adminmode') || '') == 'true';
            
            POSTransactionManager.RetrieveConfig();

            POSTransactionManager.LoadExternalComponents().done(function () {
                POSTransactionManager.WaitForDependencies().done(function () {

                    clItemPanel = $('[sa-elementtype=composite].SubControlPOSItemPanelComposite');
                    POSItemPanelManager.Init(clItemPanel);

                    POSTransactionListManager.Init(clTransactionListPanel, clPOSTransactionControl, !clExistingRecord);
                                        
                    if (clExistingRecord) {
                        POSTransactionManager.LoadTransactionList().done(function () {
                        });
                    }

                    clCustomerInfoPopUp = new InputPopUpController('customerinfo');
                    clCustomerInfoPopUp.Init();

                    clCalendar = new CalendarComposite('receiptdate', clLowerBoundDate, clUpperBoundDate);
                    clCalendar.Init();

                    POSTransactionManager.BindEvents();
                    clKeypadController = new KeypadManager(clKeypad);
                    clKeypadController.Init(clPOSTransactionControl);

                    if (clShowPaymentForm) POSPaymentManager.Init();                    
                    if (clReceiptPrintMode)
                    {
                        POSReceiptPrintingManager.Init(clPOSTransactionControl);
                        POSReceiptPrintingManager.ConnectPrinter();
                    }
                });
            });            
        },
        RetrieveConfig : function()
        {
            clRegionalConfig =  JSON.parse(Base64.decode(clPOSTransactionControl.attr('_regionalconfig') || 'e30='));
            clSystemConfig = JSON.parse(Base64.decode(clPOSTransactionControl.attr('_systemconfig') || 'e30='));
            clTransactionSettingConfig = JSON.parse(Base64.decode(clPOSTransactionControl.attr('pos.transactionsetting') || 'e30='));
            clStaffPermissionSettingConfig = JSON.parse(Base64.decode(clPOSTransactionControl.attr('pos.staffpermissionsetting') || 'e30='));

            clPOSTransactionControl.data('transactionsetting',clTransactionSettingConfig);

            clShowPaymentForm  = clTransactionSettingConfig.showpaymentform == 'true';
            clReceiptPrintMode = (clTransactionSettingConfig.receiptprintoption || '').indexOf(clMode) != -1;

            if (clAdminMode)
            {
                clLowerBoundDays = Number(clSystemConfig.receiptactionlimitdays || '7');
                clUpperBoundDays = 0;
            }
            else
            {
                if (clStaffPermissionSettingConfig.allowadjustreceipt == 'true')
                {
                    clLowerBoundDays = Number(clStaffPermissionSettingConfig.receiptadjustlimitdays || '1');
                    clUpperBoundDays = 0;
                }
            }

            if (clAdminMode)
            
            clLowerBoundDate = moment().add((!isNaN(clLowerBoundDays) ? -1 * clLowerBoundDays : -7), 'days');
            clUpperBoundDate = moment().add((!isNaN(clUpperBoundDays) ? clUpperBoundDays : 0), 'days');
            
        },
        WaitForDependencies: function () {
            var lcDeferred = $.Deferred();

            var lcWaitTimer = setInterval(function () {
                if ((typeof KeypadManager !== 'undefined') && (typeof InputPopUpController !== 'undefined') && 
                    (typeof POSItemPanelManager !== 'undefined') && (typeof POSTransactionListManager !== 'undefined') &&
                    (typeof CalendarComposite !== 'undefined') &&
                    ((!clShowPaymentForm) || (typeof POSPaymentManager !== 'undefined')) &&
                    ((!clReceiptPrintMode) || (typeof POSReceiptPrintingManager !== 'undefined')))
                    {
                        if (lcDeferred.state() == 'pending') {
                            lcDeferred.resolve();
                            clearInterval(lcWaitTimer);
                    }
                }

            }, 200);

            return (lcDeferred);
        },
        BindEvents: function () {
            clKeypad.unbind('ev-initialized');
            clKeypad.bind('ev-initialized', POSTransactionManager.HandlerOnKeyPadInitialized);

            clKeypad.unbind('ev-keypadsizechanged');
            clKeypad.bind('ev-keypadsizechanged', POSTransactionManager.HandlerOnKeyPadSizeChanged);
                        
            clKeypad.unbind('ev-keyaction');
            clKeypad.bind('ev-keyaction', POSTransactionManager.HandlerOnKeyAction);

            clTransactionList.unbind('ev-activeelementchanged');
            clTransactionList.bind('ev-activeelementchanged', POSTransactionManager.HandlerOnActiveElementChanged);

            clTransactionList.unbind('ev-save');
            clTransactionList.bind('ev-save', POSTransactionManager.HandlerOnSave);

            clTransactionList.unbind('ev-totalchanged');
            clTransactionList.bind('ev-totalchanged', POSTransactionManager.HandlerOnTotalChanged);

            clPrinterStatusControl.unbind('click');
            clPrinterStatusControl.click(POSTransactionManager.HandlerOnClick);

            clPOSTransactionControl.unbind('ev-printernotification');
            clPOSTransactionControl.bind('ev-printernotification', POSTransactionManager.HandlerOnPrinterNotification);

            clHeaderBar.find('div[ea-command="@cmd%customerinfo"]').unbind('click');
            clHeaderBar.find('div[ea-command="@cmd%customerinfo"]').click(POSTransactionManager.HandlerOnClick);

            clHeaderBar.find('div[ea-command="@cmd%changereceiptdate"]').unbind('click');
            clHeaderBar.find('div[ea-command="@cmd%changereceiptdate"]').click(POSTransactionManager.HandlerOnClick);

            clCustomerInfoPopUp.SetHandler('ev-datainit', POSTransactionManager.HandlerOnDataInit);                        
            clCustomerInfoPopUp.SetHandler('ev-commit', POSTransactionManager.HandlerOnCommit);
            
            clCalendar.SetHandler('ev-datechanged', POSTransactionManager.HandlerOnDateChanged);

        },
        LoadTransactionList    : function()
        {
            var lcDeferred = $.Deferred();

            var lcAjaxRequestManager = new AjaxRequestManager('getupdatedcontrol', null, null, 'ajax_loading');

            lcAjaxRequestManager.AddAjaxParam('Parameter', 'transactionlistcontent');

            lcAjaxRequestManager.SetCompleteHandler(function (paSuccess, paResponseStruct) {
                if (paSuccess) {                    
                    if ($(paResponseStruct.ResponseData.RSP_HTML).find('[sa-elementtype=item][gpos-itemstatus=cancel]').length > 0) {
                        POSTransactionManager.LoadCancelItemBlock().done(function () {                            
                            POSTransactionListManager.SetExistingReceiptInfo(clMasterBlock, paResponseStruct.ResponseData.RSP_HTML);
                            lcDeferred.resolve(true);
                        });
                    }
                    else {
                        POSTransactionListManager.SetExistingReceiptInfo(clMasterBlock, paResponseStruct.ResponseData.RSP_HTML);
                        lcDeferred.resolve(true);
                    }
                }
                else {
                    lcDeferred.resolve(false)
                }
            });

            lcAjaxRequestManager.Execute();

            return (lcDeferred);
        },
        LoadCancelItemBlock : function()
        {
            var lcDeferred = $.Deferred();

            var lcAjaxRequestManager = new AjaxRequestManager('getupdatedcontrol', null, null, 'ajax_loading');

            lcAjaxRequestManager.AddAjaxParam('Parameter', 'cancelitemblock');

            lcAjaxRequestManager.SetCompleteHandler(function (paSuccess, paResponseStruct) {
                if (paSuccess) {                    
                    clExternalComponentContainer.find('div[sa-elementtype=composite][ea-type=itempanel] [sa-elementtype=container] .BlockContainer').append(paResponseStruct.ResponseData.RSP_HTML);
                    lcDeferred.resolve(true);
                }
                else {
                    lcDeferred.resolve(false)
                }
            });

            lcAjaxRequestManager.Execute();

            return (lcDeferred);
        },
        LoadExternalComponents : function()
        {
            var lcDeferred = $.Deferred();

            var lcAjaxRequestManager = new AjaxRequestManager('getupdatedcontrol', null, null, 'ajax_loading');

            lcAjaxRequestManager.AddAjaxParam('Parameter', 'externalcomponent');

            lcAjaxRequestManager.SetCompleteHandler(function (paSuccess, paResponseStruct) {
                if (paSuccess) {
                    clExternalComponentContainer.empty();
                    clExternalComponentContainer.html(paResponseStruct.ResponseData.RSP_HTML);                    
                    lcDeferred.resolve(true);
                }
                else {
                    lcDeferred.resolve(false)
                }
            });

            lcAjaxRequestManager.Execute();

            return (lcDeferred);
        },
        SetMasterBlockElement : function(paColumnName, paValue)
        {
            var lcControl = clMasterBlock.find('[ea-columnname=' + paColumnName + ']');            
            lcControl.val(paValue || '');
        },
        SetHeaderBarElement: function (paColumnName, paValue) {
            var lcControl = clHeaderBar.find('[ea-columnname=' + paColumnName + ']');
            lcControl.attr('value', paValue || '');
            lcControl.text(paValue || '');
        },
        GetMasterBlockElementValue: function (paColumnName) {
            var lcControl = clMasterBlock.find('[ea-columnname=' + paColumnName + ']');
            return(lcControl.val());
        },
        RefreshNewReceipt : function(paNewReceiptNo)
        {
            var lcReceiptNo = clHeaderBar.find('.ReceiptNoDiv');
            if (paNewReceiptNo) {
                POSTransactionListManager.ResetList();
                POSTransactionManager.SetMasterBlockElement('codeno', '');
                POSTransactionManager.SetMasterBlockElement('name', '');
                POSTransactionManager.SetHeaderBarElement('codeno', '');
                POSTransactionManager.SetHeaderBarElement('name', '');
                lcReceiptNo.text(FormManager.ConvertToFormLanguage(('000000' + paNewReceiptNo).slice(-6)));
            }
        },
        GetMasterDataBlock : function() {
            var lcMasterElements    = clMasterBlock.find('[ea-columnname]');
            var lcReceiptType       = clPOSTransactionControl.attr('ea-mode');
            var lcAllowShortSell    = (clSystemConfig.allowshortsell == 'true' ? 1 : 0);
            var lcDataLastModified  = clTransactionList.attr('ea-lastmodified') || moment().format('YYYY-MM-DD');
            
            var lcDataBlock = {
                receipttype: lcReceiptType,
                status: (clTransactionState == 'pending' ? 'PENDING' : 'ACTIVE'),
                allowshortsell: lcAllowShortSell,
                receiptprintmode : (clReceiptPrintMode ? 1 : 0),
                datalastmodified: lcDataLastModified
                };
            var lcDataError = false;
            
            lcMasterElements.each(function () {
                if ($(this).attr('ea-inputmode') == 'date') 
                {
                    var lcColumnName = $(this).attr('ea-columnname');

                    var lcDate = $(this).val().ParseMomentDate();
                    
                    if ((lcDate != null) && (lcDate.isValid()))
                    {
                        if (lcDate.format('YYYY-MM-DD') == moment().format('YYYY-MM-DD'))
                            lcDataBlock[$(this).attr('ea-columnname')] = lcDate.format('YYYY-MM-DD') + ' ' + moment().format('HH:mm:ss');
                        else
                            lcDataBlock[$(this).attr('ea-columnname')] = lcDate.format('YYYY-MM-DD');
                    }
                    else
                    {
                        lcDataError = true;
                        MessageHandler.ShowMessage('err_invaliddate', function (paOption) {
                            if ((paOption) && (paOption.message)) {
                                paOption.message = paOption.message.replace('$ERROR', lcColumnName);
                            }
                        });
                        return (false);
                    }                    
                }
                else if (($(this).attr('ea-inputmode') == 'number') || ($(this).attr('ea-inputmode') == 'signednumber')) lcDataBlock[$(this).attr('ea-columnname')] = $(this).val().ForceConvertToInteger();
                else if (($(this).attr('ea-inputmode') == 'decimal') || ($(this).attr('ea-inputmode') == 'signeddecimal')) lcDataBlock[$(this).attr('ea-columnname')] = $(this).val().ForceConvertToDecimal();
                else if ($(this).attr('ea-inputmode') == 'accessinfo') lcDataBlock[$(this).attr('ea-columnname')] = window.__SYSVAR_CurrentGeoLocation || '';
                else lcDataBlock[$(this).attr('ea-columnname')] = $(this).val().trim();
            });                        

            if (!lcDataError) return (lcDataBlock);
            else return (null);
        },
        GetPaymentInfo : function ()
        {
            var lcDeferred      = $.Deferred();
            var lcPaymentCash   = clMasterBlock.find('[ea-columnname=paymentcash]');
            var lcTotalAmount   = CastDecimal(lcPaymentCash.val(), 0);

            if ((clMode == 'sale') && (lcTotalAmount > 0) && (clShowPaymentForm)) {
                POSPaymentManager.ShowPaymentPopUp(lcTotalAmount).done(function (paSuccess, paPaymentInfo) {
                    if ((paSuccess) && (paPaymentInfo)) {                        
                        $.each(paPaymentInfo, function (paKey, paValue) {
                            POSTransactionManager.SetMasterBlockElement(paKey, paValue);
                        });
                        lcDeferred.resolve(true);                        
                    }
                    else lcDeferred.resolve(false);
                });;
            }
            else lcDeferred.resolve(true);

            return (lcDeferred);
            
        },
        SaveReceipt : function()
        {
            if (POSTransactionListManager.VerifyTransactionList()) {

                POSTransactionManager.GetPaymentInfo().done(function (paProceed) {
                    if (paProceed) {
                        var lcMasterBlock = POSTransactionManager.GetMasterDataBlock();
                        var lcTransactionList = POSTransactionListManager.GetTransactionListArray();

                        if ((lcMasterBlock) && (lcTransactionList)) {
                            lcMasterBlock['transactionlist'] = Base64.encode(JSON.stringify(lcTransactionList));

                            var lcAjaxRequestManager = new AjaxRequestManager('executescalarquery', null, 'err_failupdate', 'ajax_updating');

                            lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.updatereceiptrecord');
                            lcAjaxRequestManager.AddObjectDataBlock('datablock', lcMasterBlock, true);

                            lcAjaxRequestManager.SetResponseDictionaryParsingHandler(function (paMessagePlaceHolderList, paJSONString) {
                                var lcDictionary = JSON.parse(paJSONString);

                                if (lcDictionary.itemid) {
                                    var lcPOSItem = POSItemPanelManager.GetPOSItem(lcDictionary.itemid);

                                    if (lcPOSItem) {
                                        var lcUnitRelationship = (lcPOSItem.attr('gpos-unitrelationship') || '').ForceConvertToInteger();
                                        var lcMajorUnitName = lcPOSItem.attr('gpos-majorunitname') || '';
                                        var lcMinorUnitName = lcPOSItem.attr('gpos-minorunitname') || '';
                                        var lcQuantity = (lcDictionary.quantity || '').ForceConvertToInteger();
                                        var lcMajorQuantity = Math.floor(lcQuantity / (lcUnitRelationship > 1 ? lcUnitRelationship : 1));
                                        var lcMinorQuantity = lcQuantity % (lcUnitRelationship > 1 ? lcUnitRelationship : 1);
                                        var lcQuantityText;

                                        if (lcUnitRelationship > 1) {
                                            lcQuantityText = (lcMajorQuantity > 0 ? lcMajorQuantity + ' ' + lcMajorUnitName + ' ' : '') + (lcMinorQuantity > 0 ? lcMinorQuantity + ' ' + lcMinorUnitName + ' ' : '');
                                        }
                                        else lcQuantityText = (lcMajorQuantity > 0 ? lcMajorQuantity + ' ' + lcMajorUnitName + ' ' : '');

                                        paMessagePlaceHolderList['$ITEMNAME'] = lcPOSItem.attr('gpos-itemtext');
                                        paMessagePlaceHolderList['$QUANTITYTEXT'] = FormManager.ConvertToFormLanguage(lcQuantityText);
                                    }
                                }
                            });

                            lcAjaxRequestManager.SetCompleteHandler(function (paSuccess, paResponseStruct, paButtonCommand) {
                                if (paSuccess) {
                                    var lcCurrentReceiptNo;
                                    var lcReceiptPrintingInfo;

                                    if (clReceiptPrintMode) {
                                        lcReceiptPrintingInfo = JSON.parse(paResponseStruct.ResponseData.RSP_Result);
                                        lcCurrentReceiptNo = lcReceiptPrintingInfo.receiptmaster[0].receiptno;
                                        POSReceiptPrintingManager.PrintLoadedReceipt(lcReceiptPrintingInfo);
                                    }
                                    else
                                    {
                                        lcCurrentReceiptNo = parseInt(paResponseStruct.ResponseData.RSP_Result);
                                    }

                                    if ((clExistingRecord) || (clTransactionState != 'normal')) {
                                        FormManager.CloseForm();
                                    }
                                    else POSTransactionManager.RefreshNewReceipt(lcCurrentReceiptNo + 1);
                                }
                                else {
                                    if ((paButtonCommand == 'closeform') && (clTransactionState != 'normal')) {
                                        FormManager.CloseForm();
                                    }
                                }
                            });

                            lcAjaxRequestManager.Execute();
                        }
                    }
                });
            }
        },

        //SetPrinterStatusControlAttribute : function(paErrorState, paErrorCode, paErrorParam)
        //{
        //    clPrinterStatusControl.attr('fa-errorstate', paErrorState);

        //    if ((paErrorState != 'none') && (paErrorCode)) clPrinterStatusControl.attr('fa-errorcode', paErrorCode);
        //    else clPrinterStatusControl.removeAttr('fa-errorcode');

        //    if (paErrorParam) clPrinterStatusControl.attr('fa-errorparam', paErrorParam);
        //    else clPrinterStatusControl.removeAttr('fa-errorparam');
        //},
        SetPrinterStatusControlAttribute: function (paStatus, paErrorCode, paErrorParam) {
            clPrinterStatusControl.attr('fa-status', paStatus);

            if ((paStatus != 'none') && (paErrorCode)) clPrinterStatusControl.attr('fa-errorcode', paErrorCode);
            else clPrinterStatusControl.removeAttr('fa-errorcode');

            if (paErrorParam) clPrinterStatusControl.attr('fa-errorparam', paErrorParam);
            else clPrinterStatusControl.removeAttr('fa-errorparam');
        },
        //SetPrinterStatus : function(paErrorCode, paErrorParam)
        //{
        //    switch(paErrorCode)
        //    {
        //        case 'none':
        //            {
        //                POSTransactionManager.SetPrinterStatusControlAttribute('none', '');
        //                break;
        //            }

        //        case 'err_printer_connectionfail':
        //        case 'err_printer_initfail' :
        //        case 'err_printer_noresponse' : 
        //        case 'err_printer_offline':
        //        case 'err_printer_coveropen':
        //        case 'err_printer_mechanicalerror':                
        //        case 'err_printer_unrecoverableerror':
        //        case 'err_printer_receiptend':
        //            {
        //                POSTransactionManager.SetPrinterStatusControlAttribute('error', paErrorCode, paErrorParam);
        //                break;
        //            }

        //        case 'err_printer_autocuttererror':
        //        case 'err_printer_receiptnearend':
        //            {
        //                POSTransactionManager.SetPrinterStatusControlAttribute('warning', paErrorCode, paErrorParam);
        //                break;
        //            }
        //    }
        //},
        SetPrinterStatus: function (paErrorCode, paErrorParam) {

            switch (paErrorCode) {
                case 'none':
                case 'success':
                case 'connecting':
                    {
                        if (paErrorCode == 'none') paErrorCode = 'success';
                        POSTransactionManager.SetPrinterStatusControlAttribute(paErrorCode, '');
                        break;
                    }

                case 'err_printer_connectionfail':
                case 'err_printer_initfail':
                case 'err_printer_noresponse':
                case 'err_printer_offline':
                case 'err_printer_coveropen':
                case 'err_printer_mechanicalerror':
                case 'err_printer_unrecoverableerror':
                case 'err_printer_receiptend':
                    {
                        POSTransactionManager.SetPrinterStatusControlAttribute('error', paErrorCode, paErrorParam);
                        break;
                    }

                case 'err_printer_autocuttererror':
                case 'err_printer_receiptnearend':
                    {
                        POSTransactionManager.SetPrinterStatusControlAttribute('warning', paErrorCode, paErrorParam);
                        break;
                    }
            }
        },
        ShowPrinterStatus: function () {
            var lcErrorCode = clPrinterStatusControl.attr('fa-errorcode');
            var lcErrorParam = clPrinterStatusControl.attr('fa-errorparam')

            if (lcErrorCode) {
                MessageHandler.ShowMessage(lcErrorCode, function (paOption) {
                    if (lcErrorParam) {
                        paOption.message = "<br/>" + lcErrorParam;
                    }
                });
            }
        },
        //ShowPrinterStatus : function()
        //{
        //    var lcErrorCode     = clPrinterStatusControl.attr('fa-errorcode');
        //    var lcErrorParam    = clPrinterStatusControl.attr('fa-errorparam')

        //    if (lcErrorCode)
        //    {
        //        MessageHandler.ShowMessage(lcErrorCode, function (paOption) {
        //            if (lcErrorParam)
        //            {
        //                paOption.message = "<br/>" + lcErrorParam;
        //            }
        //        });
        //    }
        //},
        HandlerOnKeyPadInitialized: function () {            
            if (!clTransactionMode) {                
                clKeypadController.SetEnableState('unitprice', false);
                clKeypadController.SetEnableState('discount', false);
                clKeypadController.SetEnableState('discountpercent', false);
                clKeypadController.SetEnableState('subtotal', false);                
            }
        },
        HandlerOnKeyPadSizeChanged: function (paEvent, paSizeMode) {
            paEvent.preventDefault();
            if (paSizeMode == 'minimal') POSTransactionListManager.SetFullScreenMode(true);
            else if (paSizeMode == 'full') POSTransactionListManager.SetFullScreenMode(false);
        },
        HandlerOnActiveElementChanged : function(paEvent, paElementType, paItem)
        {            
            if (paElementType == 'summary')
            {         
                clKeypadController.SetEnableState('item', false);                
                clKeypadController.SetEnableState('unitprice', false);

                if (clTransactionMode) {
                    clKeypadController.SetEnableState('subtotal', true);
                    clKeypadController.SetEnableState('discount', true);
                    clKeypadController.SetEnableState('discountpercent', true);
                }
                clKeypadController.SetUnitNameKeys();
            }
            else if (paElementType == 'item')
            {
                if (paItem.attr('gpos-itemstatus') == 'cancel')
                {
                    clKeypadController.SetEnableState('item', false);
                    clKeypadController.SetEnableState('unitprice', false);
                    clKeypadController.SetEnableState('subtotal', false);
                    clKeypadController.SetEnableState('discount', false);
                    clKeypadController.SetEnableState('discountpercent', false);
                    clKeypadController.SetUnitNameKeys();
                }
                else
                {
                    clKeypadController.SetEnableState('item', true);                    
                    clKeypadController.SetUnitNameKeys(paItem.POSItemInfo);
                    
                    if (clTransactionMode) {
                        clKeypadController.SetEnableState('unitprice', true);
                        clKeypadController.SetEnableState('subtotal', true);
                        clKeypadController.SetEnableState('discount', true);
                        clKeypadController.SetEnableState('discountpercent', true);
                    }
                }
            }            
        },
        HandlerOnTotalChanged : function(paEvent, paEventInfo)
        {
            //var lcDiscount = clMasterBlock.find('[ea-columnname=discount]');
            //var lcPaymentCash = clMasterBlock.find('[ea-columnname=paymentcash]');
            
            //lcDiscount.val(paEventInfo.discount);
            //lcPaymentCash.val(paEventInfo.total);

            POSTransactionManager.SetMasterBlockElement('discount', paEventInfo.discount);
            POSTransactionManager.SetMasterBlockElement('paymentcash', paEventInfo.total);
        },
        HandlerOnDateChanged: function (paEvent, paEventInfo) {
            if (paEventInfo.eventdata) {
                var lcDate = paEventInfo.eventdata;
                var lcDateValue = Number(lcDate.format('YYYYMMDD'));

                if ((lcDateValue >= Number(clLowerBoundDate.format('YYYYMMDD'))) && (lcDateValue <= Number(clUpperBoundDate.format('YYYYMMDD')))) {
                    clDateBox.text(FormManager.ConvertToFormLanguage(lcDate.format('DD/MM/YYYY')));
                    clMasterBlockDateBox.val(clDateBox.text());                    
                    return;
                }
            }
            paEventInfo.defaultaction = false;
        },
        HandlerOnSave : function(paEvent)
        {
            POSTransactionManager.SaveReceipt();            
        },
        HandlerOnClick : function(paEvent)
        {
            paEvent.preventDefault();

            var lcCommand = $(this).attr('ea-command').substring(5);
            
            switch(lcCommand)
            {
                case 'customerinfo': clCustomerInfoPopUp.Show(); break;

                case 'changereceiptdate':
                    {
                        if (clTransactionState == 'normal') {
                            var lcActiveDate = moment(clDateBox.text().NormalizeNumber(), 'DD/MM/YYYY');
                            clCalendar.SetDate(lcActiveDate);
                            clCalendar.Show();
                        }
                        break;
                    }
                case 'printerstatus' :
                    {
                        POSTransactionManager.ShowPrinterStatus();
                        break;
                    }
            }            
        },
        HandlerOnDataInit: function (paEvent, paEventInfo)
        {
            var lcElement = clHeaderBar.find('[ea-columnname="' + paEventInfo.columnname + '"]');
            var lcValue = lcElement.length > 0 ? lcElement.text().trim() : '';
                        
            paEventInfo.target.val(lcValue);
        },       
        HandlerOnCommit: function (paEvent, paEventInfo)
        {
            var lcElement = clHeaderBar.find('[ea-columnname="' + paEventInfo.columnname + '"]');
            var lcMasterElement = clMasterBlock.find('[ea-columnname="' + paEventInfo.columnname + '"]');

            if (lcElement.length > 0)
            {
                lcElement.attr('value', paEventInfo.eventdata);
                lcElement.text(paEventInfo.eventdata);
            }

            if (lcMasterElement.length > 0)
            {
                lcMasterElement.val(paEventInfo.eventdata);
                lcMasterElement.text(paEventInfo.eventdata);
            }
        },
        //HandlerOnPrinterNotification : function(paEventArg, paNotificationInfo)
        //{
        //    if (paNotificationInfo)
        //    {
        //        switch (paNotificationInfo.message)
        //        {
        //            case 'initfail':
        //                {
        //                    POSTransactionManager.SetPrinterStatus(paNotificationInfo.errorcode, paNotificationInfo.errorparam);
        //                    break;
        //                }

        //            case 'initsuccess':
        //                {
        //                    POSTransactionManager.SetPrinterStatus('none');
        //                    break;
        //                }

        //            case 'printfail':
        //                {
        //                    Messagehandler.ShowMessage(paNotificationInfo.errorcode);
        //                    break;
        //                }

        //            case 'statuschanged' :
        //                {
        //                    POSTransactionManager.SetPrinterStatus(paNotificationInfo.errorcode, paNotificationInfo.errorparam);
        //                    break;
        //                }
        //        }
        //    }
        //},
        HandlerOnPrinterNotification: function (paEventArg, paNotificationInfo) {

            if (paNotificationInfo) {
                switch (paNotificationInfo.event) {
                    case 'connecting':
                        {
                            POSTransactionManager.SetPrinterStatus('connecting');
                        }
                    case 'initfail':
                        {
                            POSTransactionManager.SetPrinterStatus(paNotificationInfo.errorcode, paNotificationInfo.errorparam);
                            break;
                        }

                    case 'initsuccess':
                        {
                            POSTransactionManager.SetPrinterStatus('success');
                            break;
                        }

                    case 'printfail':
                        {
                            Messagehandler.ShowMessage(paNotificationInfo.errorcode);
                            break;
                        }

                    case 'statuschanged':
                        {
                            POSTransactionManager.SetPrinterStatus(paNotificationInfo.errorcode, paNotificationInfo.errorparam);
                            break;
                        }
                }
            }
        },
        HandlerOnKeyAction: function (paEvent, paKeyName, paKeyData) {
            paEvent.preventDefault();
            
            switch(paKeyName)
            {
                case 'item':
                    {
                        var lcItemInfo = POSItemPanelManager.GetItemInfo(paKeyData);
                        if (lcItemInfo == null) {
                            POSItemPanelManager.ShowPOSItemPanel().done(function (paItemInfo) {
                                if (paItemInfo) {
                                    POSTransactionListManager.SetItemInfo(paItemInfo);
                                    clKeypadController.SetUnitNameKeys(paItemInfo);
                                }
                            });
                        }
                        else 
                        {
                            POSTransactionListManager.SetItemInfo(lcItemInfo);
                            clKeypadController.SetUnitNameKeys(lcItemInfo);
                            clKeypadController.ClearScreen();
                        }

                        break;
                    }

                case 'majorunit':
                    {                        
                        if ((paKeyData == 0) || (paKeyData)) {
                            if (paKeyData.toString().length > 0) {
                                POSTransactionListManager.SetQuantity(paKeyData, true);
                                clKeypadController.ClearScreen();
                            }
                        }
                        break;
                    }

               case 'minorunit':
                   {
                       if ((paKeyData == 0) || (paKeyData)) {
                           if (paKeyData.toString().length > 0) {
                               POSTransactionListManager.SetQuantity(paKeyData, false);
                               clKeypadController.ClearScreen();
                           }
                       }
                        break;
                    }

               case 'unitprice':
                   {
                       if ((paKeyData == 0) || (paKeyData)) {
                           if (paKeyData.toString().length > 0) {                               
                                   POSTransactionListManager.SetUnitPrice(paKeyData);
                                   clKeypadController.ClearScreen();
                               }
                       }
                        break;
                    }

               case 'discountpercent':
                   {
                       if ((paKeyData == 0) || (paKeyData)) {
                           if (paKeyData.toString().length > 0) {
                               POSTransactionListManager.SetDiscountPercent(paKeyData);
                               clKeypadController.ClearScreen();
                           }
                       }
                        break;
                    }

               case 'discount':
                   {
                       if ((paKeyData == 0) || (paKeyData)) {
                           if (paKeyData.toString().length > 0) {
                               POSTransactionListManager.SetDiscountValue(paKeyData);
                               clKeypadController.ClearScreen();
                           }
                       }
                        break;
                   }

              case 'subtotal':
                   {
                       if ((paKeyData == 0) || (paKeyData)) {
                           if (paKeyData.toString().length > 0) {
                               POSTransactionListManager.SetSubTotalAmount(paKeyData);
                               clKeypadController.ClearScreen();
                           }
                       }
                       break;
                   }

                case 'enter':
                    {
                        if (POSTransactionListManager.EnlistItem())
                        {
                            POSTransactionListManager.AppendNewElement();
                            clKeypadController.ResetKeyPad();
                        }
                        break;
                    }
                case 'void':
                    {                      
                        POSTransactionListManager.DeleteActiveEntry();
                        break;
                    }
                case 'toparrow':
                    {
                        POSTransactionListManager.GotoElement('first');
                        break;
                    }

                case 'bottomarrow':
                    {                        
                        POSTransactionListManager.GotoElement('last');
                        break;
                    }

                case 'goto':
                    {
                        POSTransactionListManager.GotoElement(paKeyData);
                        break;
                    }

                case 'customer':
                    {
                        clCustomerInfoPopUp.Show();
                        break;
                    }
                case 'save':
                    {
                        POSTransactionManager.SaveReceipt();
                        break;
                    }
              }            
        },
    }
})();
