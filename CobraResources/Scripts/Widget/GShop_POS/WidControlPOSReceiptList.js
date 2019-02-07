$(document).ready(function () {        
    POSReceiptListManager.Init($('[sa-elementtype=control].WidControlPOSReceiptList'));    
});

var POSReceiptListManager = (function () {
    var clPOSReceiptListControl;
    var clCalendar;
    var clReceiptList;
    var clToolBar;
    var clActiveDate;
    var clLowerBoundDays;
    var clUpperBoundDays;    
    var clLowerBoundDate;
    var clUpperBoundDate;
    var clTitleBar;
    var clHeaderBar;
    var clDateBox;
    var clPreviousButton;
    var clNextButton;    
    var clSummaryBar;
    var clQuantityBox;
    var clTotalAmountBox;
    var clSearchPopUp;
    var clExternalComponentContainer;
    var clPrinterStatusControl;
    
    return {
        Init: function (paPOSReceiptListControl) {            
            clPOSReceiptListControl = paPOSReceiptListControl;            
            clReceiptList = clPOSReceiptListControl.find('[sa-elementtype=list].ReceiptList');
            clToolBar = clPOSReceiptListControl.find('[sa-elementtype=toolbar]');
            clTitleBar = clPOSReceiptListControl.find('[sa-elementtype=title]');
            clHeaderBar = clPOSReceiptListControl.find('[sa-elementtype=controlbar]');
            clDateBox = clHeaderBar.find('[sa-elementtype=datebox]');
            clPreviousButton = clHeaderBar.find('a[ea-command="@cmd%prevday"]');
            clNextButton = clHeaderBar.find('a[ea-command="@cmd%nextday"]');
            clSummaryBar = clPOSReceiptListControl.find('[sa-elementtype=summary]');
            clQuantityBox = clSummaryBar.find('span[sa-elementtype=quantity]');
            clTotalAmountBox = clSummaryBar.find('span[sa-elementtype=total]');
            clExternalComponentContainer = $('[sa-elementtype=container][ea-type=externalcomponent]');
            clPrinterStatusControl = clPOSReceiptListControl.find('[sa-elementtype=title] div[sa-elementtype=statuscontrol]');
            
            if ((clActiveDate = POSReceiptListManager.GetStateDate()) == null)  clActiveDate = moment(clDateBox.text, "DD/MM/YYYY");
            if (!clActiveDate.isValid()) clActiveDate = moment();            

            clDateBox.text(clActiveDate.format('DD/MM/YYYY'));
            
            clLowerBoundDays = Number(clPOSReceiptListControl.attr('ea-lowerbound') || '7');
            clUpperBoundDays = Number(clPOSReceiptListControl.attr('ea-upperbound') || '0');            
            
            clLowerBoundDate = moment().add((!isNaN(clLowerBoundDays) ? -1 * clLowerBoundDays : -7), 'days');
            clUpperBoundDate = moment().add((!isNaN(clUpperBoundDays) ? clUpperBoundDays : 0), 'days');

            POSReceiptListManager.LoadExternalComponents().done(function () {
                POSReceiptListManager.WaitForDependencies().done(function () {
                    POSReceiptListManager.RetrieveContentByDate(clActiveDate).done(function (paSuccess) {
                        if (paSuccess) {
                            POSReceiptListManager.RestoreBlockState();
                        }

                        clCalendar = new CalendarComposite('receiptdate', clLowerBoundDate, clUpperBoundDate);
                        clCalendar.Init();

                        POSReceiptListManager.RefreshNavigationButtonAppearance();
                        POSReceiptListManager.SetFilter(clPOSReceiptListControl.attr('ea-filter'));
                        POSReceiptListManager.RefreshBadges();

                        clSearchPopUp = new InputPopUpController('searchreceiptinfo');
                        clSearchPopUp.Init();
                        clSearchPopUp.SetMessageBarText(500);

                        POSReceiptPrintingManager.Init(clPOSReceiptListControl);
                        POSReceiptPrintingManager.ConnectPrinter();

                        POSReceiptListManager.BindEvents();
                    });
                });
            });
        },
        WaitForDependencies: function ()
        {
            var lcDeferred = $.Deferred();
            
            var lcWaitTimer = setInterval(function () {

                if ((typeof InputPopUpController !== 'undefined') && (typeof CalendarComposite !== 'undefined') &&
                    (typeof POSReceiptPrintingManager !== 'undefined'))
                {
                    if (lcDeferred.state() == 'pending') {
                        lcDeferred.resolve();
                        clearInterval(lcWaitTimer);
                    }
                }                
               
            }, 200);

            return (lcDeferred);
        },
        LoadExternalComponents: function () {
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
        BindEvents: function () {            
            clTitleBar.find('div[ea-command]').unbind('click');
            clTitleBar.find('div[ea-command]').click(POSReceiptListManager.HandlerOnClick);

            clHeaderBar.find('[ea-command]').unbind('click');
            clHeaderBar.find('[ea-command]').click(POSReceiptListManager.HandlerOnClick);
            
            //clHeaderBar.find('span[ea-command]').unbind('click');
            //clHeaderBar.find('span[ea-command]').click(POSReceiptListManager.HandlerOnClick);

            clToolBar.find('[ea-command]').unbind('click');
            clToolBar.find('[ea-command]').click(POSReceiptListManager.HandlerOnClick);

            clPOSReceiptListControl.unbind('ev-printernotification');
            clPOSReceiptListControl.bind('ev-printernotification', POSReceiptListManager.HandlerOnPrinterNotification);

            clSearchPopUp.SetHandler('ev-commitcomplete', POSReceiptListManager.HandlerOnCommitComplete);

            clCalendar.SetHandler('ev-datechanged',POSReceiptListManager.HandlerOnDateChanged);

            POSReceiptListManager.BindElementEvents();       
        },
        BindElementEvents : function()
        {
            var lcElements = clReceiptList.find('[sa-elementtype=element]');

            lcElements.unbind('click');
            lcElements.click(POSReceiptListManager.HandlerOnClick);

            lcElements.find('[ea-command]').unbind('click');
            lcElements.find('[ea-command]').click(POSReceiptListManager.HandlerOnClick);

            //lcElements.find('a[ea-command="@cmd%deletereceipt"]').unbind('click');
            //lcElements.find('a[ea-command="@cmd%deletereceipt"]').click(POSReceiptListManager.HandlerOnClick);

            //lcElements.find('a[ea-command="@cmd%editreceipt"]').unbind('click');
            //lcElements.find('a[ea-command="@cmd%editreceipt"]').click(POSReceiptListManager.HandlerOnClick);
        },
        RefreshNavigationButtonAppearance : function()
        {
            if (Number(clActiveDate.format('YYYYMMDD')) <= Number(clLowerBoundDate.format('YYYYMMDD')))
                clPreviousButton.attr('fa-disable', 'true');
            else
                clPreviousButton.removeAttr('fa-disable');

            if (Number(clActiveDate.format('YYYYMMDD')) >= Number(clUpperBoundDate.format('YYYYMMDD')))
                clNextButton.attr('fa-disable', 'true');
            else
                clNextButton.removeAttr('fa-disable');
        },
        RefreshBadge  : function(paType)
        {
            if (paType) {
                var lcCount = clReceiptList.find('[sa-elementtype=element][ea-type=' + paType + ']').length;
                var lcToolButton = clToolBar.find('a[ea-command="@toolcmd%' + paType + '"]');

                if (lcCount > 0) lcToolButton.attr('fa-badge', FormManager.ConvertToFormLanguage(lcCount));
                else lcToolButton.removeAttr('fa-badge');
            }
        },
        RefreshBadges : function()
        {
            POSReceiptListManager.RefreshBadge('sale');
            POSReceiptListManager.RefreshBadge('purchase');
            POSReceiptListManager.RefreshBadge('stockin');
            POSReceiptListManager.RefreshBadge('stockout');
        },
        RefreshItemCount : function()
        {
            var lcCount = clReceiptList.find('[sa-elementtype=element]').length;
            var lcFilter = clPOSReceiptListControl.attr('ea-filter');
            var lcVisibleCount = clReceiptList.find('[ea-type="' + lcFilter + '"]').length;

            if (lcVisibleCount > 0) clPOSReceiptListControl.attr('fa-visiblecount', lcVisibleCount);
            else clPOSReceiptListControl.removeAttr('fa-visiblecount');

            if (lcCount > 0) clPOSReceiptListControl.attr('fa-count', lcCount);
            else clPOSReceiptListControl.removeAttr('fa-count');
        },
        RetrieveContentByDate: function (paDate) {

            var lcDeferred = $.Deferred();

            if ((paDate) && (paDate.isValid))
            {
                var lcAjaxRequestManager = new AjaxRequestManager('getupdatedcontrol', null, null, 'ajax_loading');
                
                lcAjaxRequestManager.AddAjaxParam('Parameter', 'receiptlistcontent');
                lcAjaxRequestManager.AddObjectDataBlock('paramblock', { FPM_DATE : paDate.format('YYYY-MM-DD') });                    

                lcAjaxRequestManager.SetCompleteHandler(function (paSuccess, paResponseStruct) {
                    if (paSuccess) {
                        clActiveDate = paDate;
                        clDateBox.text(clActiveDate.format('DD/MM/YYYY'));
                        POSReceiptListManager.RefreshNavigationButtonAppearance();
                        clPOSReceiptListControl.removeAttr('fa-searchresult');

                        clReceiptList.empty();
                        clReceiptList.html(paResponseStruct.ResponseData.RSP_HTML);
                        POSReceiptListManager.BindElementEvents();
                        POSReceiptListManager.RefreshItemCount();
                        POSReceiptListManager.RefreshBadges();
                        POSReceiptListManager.RefreshSummary();

                        lcDeferred.resolve(true);
                    }
                    else lcDeferred.resolve(false);
                });
                
                lcAjaxRequestManager.Execute();
            }
            else
            {
                setTimeout(function () { lcDeferred.resolve(false); }, 500);
            }

            return (lcDeferred);            
        },
        NavigateDate : function(paDirection)
        {            
            var lcDataDate = moment(clActiveDate.format('YYYY-MM-DD'), 'YYYY-MM-DD');
            
            switch(paDirection)
            {
                case "previous":
                    {                     
                        if (Number(lcDataDate.format('YYYYMMDD')) > Number(clLowerBoundDate.format('YYYYMMDD'))) {
                            lcDataDate.add(-1, 'days');
                            POSReceiptListManager.RetrieveContentByDate(lcDataDate);
                        }
                        break;
                    }

                case "next":
                    {                        
                        if (Number(lcDataDate.format('YYYYMMDD')) < Number(clUpperBoundDate.format('YYYYMMDD'))) {                            
                            lcDataDate.add(1, 'days');
                            POSReceiptListManager.RetrieveContentByDate(lcDataDate);
                        }
                        break;
                    }
            }
        },
        SetFilter : function(paFilter)
        {
            var lcTriggerControl = clToolBar.find('a[ea-command="@toolcmd%' + paFilter + '"]');
            clPOSReceiptListControl.attr('ea-filter', paFilter);
            clPOSReceiptListControl.attr('fa-filterdatacount',clReceiptList.find('[sa-elementtype=element][ea-type=' + paFilter + ']').length);
            clToolBar.find('a[ea-command]').removeAttr('fa-current');
            lcTriggerControl.attr('fa-current', true);        
            POSReceiptListManager.RefreshSummary();
            POSReceiptListManager.RefreshItemCount();
        },
        RefreshSummary : function()
        {
            var lcElementList = clReceiptList.find('[sa-elementtype=element][ea-type=' + clPOSReceiptListControl.attr('ea-filter') + ']');
            var lcCount = lcElementList.length;
            var lcTotalAmount = 0;
            
            lcElementList.each(function () {
                var lcAmountBox = $(this).find('div[ea-columnname=receiptamount]');
                var lcAmount = Number(lcAmountBox.attr('value').replace(',','') || '0');                
                lcTotalAmount += lcAmount;
            });

            clQuantityBox.attr('value', lcCount);
            clQuantityBox.text(FormManager.ConvertToFormLanguage(lcCount));

            clTotalAmountBox.attr('value', lcTotalAmount);
            clTotalAmountBox.text(FormManager.ConvertToFormLanguage(ConvertToThousandSeparatorStr(lcTotalAmount)));
           
        },
        SelectElement  : function(paElement)
        {
            if (paElement) 
            {
                if (paElement.attr('fa-active') != 'true') {
                    clReceiptList.find('[sa-elementtype=element][fa-active]').removeAttr('fa-active');
                    paElement.attr('fa-active', 'true');
                }
                else paElement.removeAttr('fa-active');
            }
        },
        SaveBlockState: function () {
            var lcFilter = clPOSReceiptListControl.attr('ea-filter');
            var lcDate = clActiveDate.format('YYYY-MM-DD');
            var lcBlockState = { Date : lcDate, Filter : lcFilter, ScrollTop: clReceiptList.scrollTop() };

            return (Base64.encode(JSON.stringify(lcBlockState)));
        },
        GetStateDate : function()
        {
            var lcSavedState = GetUrlParameter('_formsavedstate');
            if (lcSavedState != '') {
                lcSavedState = Base64.decode(lcSavedState);

                var lcBlockStateInfo = JSON.parse(lcSavedState);

                if (lcBlockStateInfo.Date) return (moment(lcBlockStateInfo.Date, 'YYYY-MM-DD'));
                else return(null);
            }
        },
        RestoreBlockState: function () {
            var lcSavedState = GetUrlParameter('_formsavedstate');
            
            if (lcSavedState != '') {

                lcSavedState = Base64.decode(lcSavedState);

                var lcBlockStateInfo = JSON.parse(lcSavedState);
                if (lcBlockStateInfo.Filter) POSReceiptListManager.SetFilter(lcBlockStateInfo.Filter);
                if (lcBlockStateInfo.ScrollTop) clReceiptList.scrollTop(lcBlockStateInfo.ScrollTop);
            }
        },
        ReprintReceipt: function (paElement) {
            if (paElement) {
                var lcReceiptID = paElement.attr('ea-dataid');
                var lcReceiptType = paElement.attr('ea-type');

                POSReceiptPrintingManager.PreviewReceipt(lcReceiptType, lcReceiptID);               
            }
        },
        EditReceipt : function(paElement)
        {            
            if (paElement) {
                var lcReceiptID     = paElement.attr('ea-dataid');
                var lcReceiptType   = paElement.attr('ea-type');
                
                if (lcReceiptID) {
                    var lcLinkTemplate = clPOSReceiptListControl.attr('ea-template');
                   
                    var lcLink = lcLinkTemplate.replace('$RECEIPTID', lcReceiptID).replace('$RECEIPTTYPE', lcReceiptType.toUpperCase());                    
                    var lcBlockStateInfo = POSReceiptListManager.SaveBlockState();
                    
                    FormManager.RedirectStatefulTextLink(lcLink, lcBlockStateInfo);
                }
            }
        },
        DeleteReceipt : function(paElement)
        {
            if (paElement)
            {                
                var lcDataID = paElement.attr('ea-dataid');
                var lcReceiptNo = paElement.find('div[ea-columnname=receiptno]').text();
                var lcAjaxRequestManager = new AjaxRequestManager('executenonquery', 'info_successdeletereceipt', 'err_faildeletereceipt', 'ajax_deleting');
                
                lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.deletereceiptrecord');
                lcAjaxRequestManager.AddObjectDataBlock('datablock', { FPM_ReceiptID: lcDataID, FPM_Action: 'DELETE' }, true);
                lcAjaxRequestManager.AddMessagePlaceHolder('$RECEIPTNO', lcReceiptNo);

                lcAjaxRequestManager.SetCompleteHandler(function (paSuccess) {
                    if (paSuccess)
                    {
                        paElement.remove();
                        POSReceiptListManager.RefreshItemCount();
                        POSReceiptListManager.RefreshBadges();
                        POSReceiptListManager.RefreshSummary();
                    }
                });
                
                lcAjaxRequestManager.ExecuteOnConfirm('confirm_deletereceipt');
            }
        },
        SetPrinterStatusControlAttribute: function (paStatus, paErrorCode, paErrorParam) {
            clPrinterStatusControl.attr('fa-status', paStatus);

            if ((paStatus != 'none') && (paErrorCode)) clPrinterStatusControl.attr('fa-errorcode', paErrorCode);
            else clPrinterStatusControl.removeAttr('fa-errorcode');

            if (paErrorParam) clPrinterStatusControl.attr('fa-errorparam', paErrorParam);
            else clPrinterStatusControl.removeAttr('fa-errorparam');
        },
        SetPrinterStatus: function (paErrorCode, paErrorParam) {

            switch (paErrorCode) {
                case 'none':
                case 'success':
                case 'connecting':
                    {
                        if (paErrorCode == 'none') paErrorCode = 'success';
                        POSReceiptListManager.SetPrinterStatusControlAttribute(paErrorCode, '');
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
                        POSReceiptListManager.SetPrinterStatusControlAttribute('error', paErrorCode, paErrorParam);
                        break;
                    }

                case 'err_printer_autocuttererror':
                case 'err_printer_receiptnearend':
                    {
                        POSReceiptListManager.SetPrinterStatusControlAttribute('warning', paErrorCode, paErrorParam);
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
        HandlerOnDateChanged : function(paEvent, paEventInfo)
        {
            if (paEventInfo.eventdata)
            {
                var lcDate = paEventInfo.eventdata;
                var lcDateValue = Number(lcDate.format('YYYYMMDD'));

                if ((lcDateValue >= Number(clLowerBoundDate.format('YYYYMMDD'))) && (lcDateValue <= Number(clUpperBoundDate.format('YYYYMMDD'))))
                {                    
                    POSReceiptListManager.RetrieveContentByDate(lcDate);
                    return;
                }
            }

            paEventInfo.defaultaction = false;
        },
        HandlerOnCommitComplete : function(paEvent, paEventInfo)
        {
            var lcSearchKey = clSearchPopUp.GetData('searchkey');
            var lcRenderMode = 'epos.searchreceiptbycustomer';

            if (lcSearchKey)
            {
                if (lcSearchKey.StartWith('#'))
                {
                    lcSearchKey = lcSearchKey.substring(1).NormalizeNumber();
                    lcRenderMode = 'epos.searchreceiptbyreceiptno';
                }                

                var lcAjaxRequestManager = new AjaxRequestManager('getupdatedcontrol', null, null, 'ajax_loading');

                lcAjaxRequestManager.AddAjaxParam('Parameter', lcRenderMode);
                lcAjaxRequestManager.AddObjectDataBlock('paramblock', { FPMNQ_SEARCHKEY: lcSearchKey }, true);                

                lcAjaxRequestManager.SetCompleteHandler(function (paSuccess, paResponseStruct) {
                    if (paSuccess) {

                        clPOSReceiptListControl.attr('fa-searchresult', 'true');

                        clReceiptList.empty();
                        clReceiptList.html(paResponseStruct.ResponseData.RSP_HTML);
                        POSReceiptListManager.BindElementEvents();

                        POSReceiptListManager.RefreshBadges();
                        POSReceiptListManager.RefreshSummary();
                    }
                });

                lcAjaxRequestManager.Execute();
            }
        },
        HandlerOnPrinterNotification: function (paEventArg, paNotificationInfo) {

            if (paNotificationInfo) {
                switch (paNotificationInfo.event) {
                    case 'connecting':
                        {
                            POSReceiptListManager.SetPrinterStatus('connecting');
                        }
                    case 'initfail':
                        {
                            POSReceiptListManager.SetPrinterStatus(paNotificationInfo.errorcode, paNotificationInfo.errorparam);
                            break;
                        }

                    case 'initsuccess':
                        {
                            POSReceiptListManager.SetPrinterStatus('success');
                            break;
                        }

                    case 'printfail':
                        {
                            Messagehandler.ShowMessage(paNotificationInfo.errorcode);
                            break;
                        }

                    case 'statuschanged':
                        {
                            POSReceiptListManager.SetPrinterStatus(paNotificationInfo.errorcode, paNotificationInfo.errorparam);
                            break;
                        }
                }
            }
        },
        HandlerOnClick : function(paEvent)
        {
            paEvent.preventDefault();
            
            var lcCommand = $(this).attr('ea-command');
            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);
            
            switch (lcCommand) {
                case "prevday":
                    {                        
                        POSReceiptListManager.NavigateDate('previous');
                        break;
                    }
                case "nextday":
                    {
                        POSReceiptListManager.NavigateDate('next');
                        break;
                    }
                case "showcalendar":
                    {
                        clCalendar.SetDate(clActiveDate);
                        clCalendar.Show();
                        break;
                    }
                case "sale":        
                case "purchase":
                case "stockout":                    
                case "stockin":
                    {
                        POSReceiptListManager.SetFilter(lcCommand);
                        break;
                    }

                case "selectreceipt" :
                    {                        
                        POSReceiptListManager.SelectElement($(this));
                        break;
                    }

                case "deletereceipt" :
                    {
                        paEvent.stopPropagation();
                        var lcElement = $(this).closest('[sa-elementtype=element]');
                        POSReceiptListManager.DeleteReceipt(lcElement);
                        break;
                    }

                case "editreceipt":
                    {                        
                        paEvent.stopPropagation();
                        var lcElement = $(this).closest('[sa-elementtype=element]');                        
                        POSReceiptListManager.EditReceipt(lcElement);
                        break;
                    }

                case "reprintreceipt":
                    {
                        paEvent.stopPropagation();                        
                        var lcElement = $(this).closest('[sa-elementtype=element]');
                        POSReceiptListManager.ReprintReceipt(lcElement);                        
                        break;
                    }

                case "search":
                    {
                        clSearchPopUp.Show();
                        break;
                    }

                case "printerstatus":
                    {                        
                        POSReceiptListManager.ShowPrinterStatus();
                        break;
                    }
            }
        }
    }
})();

