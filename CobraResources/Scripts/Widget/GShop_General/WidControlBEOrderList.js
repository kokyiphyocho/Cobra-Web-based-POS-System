$(document).ready(function () {
    $('[sa-elementtype=control].WidControlBEOrderList').BindBEOrderListEvents();
    $('[sa-elementtype=control].WidControlBEOrderList').BindBEOrderListOrderDivEvents();
});

$.fn.BindBEOrderListEvents = function () {
    lcForm = $(this).closest('[sa-elementtype=form]');
    RefreshTimerObject.StartTimer();

    $(this).find('a[href="@cmd%refresh"]').unbind('click');
    $(this).find('a[href="@cmd%refresh"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcControl       = $(this).closest('[sa-elementtype=control]');
        var lcRefreshBar    = $(this).closest('.RefreshBar');
        var lcTimer         = lcRefreshBar.find('[sa-elementtype=timer]');
        var lcTimeOut       = $(this).attr('ea-timeout');
                
        if (lcTimeOut == 0) RefreshTimerObject.TimerTick('true');
        else  {
                lcControl.attr('ea-timeout', lcTimeOut);
                lcTimer.attr('value', lcTimeOut * 60);
            }
    });

    $(this).find('a[href="@cmd%filter"]').unbind('click');
    $(this).find('a[href="@cmd%filter"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcControl = $(this).closest('[sa-elementtype=control]');
        var lcStatus = $(this).attr('ea-status');

        if (lcStatus != lcControl.attr('ea-status')) {
            lcControl.attr('ea-status', lcStatus);
            lcControl.RemoveFocus();
        }
    });   

    $(this).find('a[href="@cmd%rejectcancel"]').unbind('click');
    $(this).find('a[href="@cmd%rejectcancel"]').click(function (paEvent) {
        paEvent.preventDefault();
                
        var lcControl = $(this).closest('[sa-elementtype=control]');
        var lcFocusedElements = lcControl.find('[sa-elementtype=container] [sa-elementtype=element] [sa-elementtype=row][fa-focus]');
        lcFocusedElements.removeAttr('fa-focus');
        lcControl.removeAttr('fa-popup');
    });

    $(this).find('a[href="@cmd%rejectproceed"]').unbind('click');
    $(this).find('a[href="@cmd%rejectproceed"]').click(function (paEvent) {
        paEvent.preventDefault();

        if (!IsDemoMode()) {
            var lcForm = $(this).closest('[sa-elementtype=form]');
            var lcControl = lcForm.find('[sa-elementtype=control].WidControlBEOrderList');
            var lcOrderDiv = lcControl.find('.OrderDiv[fa-focus]');
            var lcRejectReasonButton = $(this).closest('[sa-elementtype=popup]').find('input:checked');
            var lcRejectReason = '';

            if (lcRejectReasonButton.length > 0) lcRejectReason = lcRejectReasonButton.attr('value');

            lcControl.removeAttr('fa-popup');

            lcOrderDiv.CancelOrder(lcRejectReason);
        }
    });   
}

$.fn.BindBEOrderListOrderDivEvents = function () {

    $(this).find('img[href="@cmd%showbuttonbar"]').unbind('click');
    $(this).find('img[href="@cmd%showbuttonbar"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcOrderDiv = $(this).closest('[sa-elementtype=element]');
        var lcControl = lcOrderDiv.closest('[sa-elementtype=control]');

        lcControl.find('[sa-elementtype=element]').removeAttr('fa-floatbuttonbar');
        lcControl.find('[sa-elementtype=element]').removeAttr('fa-fullinfo');

        lcOrderDiv.attr('fa-floatbuttonbar', 'true');

        lcOrderDiv.attr('fa-focus', 'true');
        lcControl.attr('fa-focus', 'true');

    });

    $(this).find('img[href="@cmd%hidebuttonbar"]').unbind('click');
    $(this).find('img[href="@cmd%hidebuttonbar"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcOrderDiv = $(this).closest('[sa-elementtype=element]');
        lcOrderDiv.HideButtonBar();
    });

    $(this).find('img[href="@cmd%showdetail"]').unbind('click');
    $(this).find('img[href="@cmd%showdetail"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcOrderDiv = $(this).closest('[sa-elementtype=element]');
        var lcControl = lcOrderDiv.closest('[sa-elementtype=control]');

        lcControl.find('[sa-elementtype=element]').removeAttr('fa-floatbuttonbar');
        lcControl.find('[sa-elementtype=element]').removeAttr('fa-fullinfo');

        lcOrderDiv.attr('fa-fullinfo', 'true');

        lcOrderDiv.attr('fa-focus', 'true');
        lcControl.attr('fa-focus', 'true');
    });

    $(this).find('img[href="@cmd%hidedetail"]').unbind('click');
    $(this).find('img[href="@cmd%hidedetail"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcOrderDiv = $(this).closest('[sa-elementtype=element]');
        lcOrderDiv.HideDetail();

    });

    $(this).find('[sa-elementtype=element].OrderDiv').unbind('click');
    $(this).find('[sa-elementtype=element].OrderDiv').click(function (paEvent) {
        paEvent.preventDefault();

        var lcControl = $(this).closest('[sa-elementtype=control]');

        if ($(this).attr('fa-focus') != 'true') {
            if (lcControl.attr('fa-focus') == 'true') {
                var lcOrderDivList = lcControl.find('[sa-elementtype=element]');
                lcControl.removeAttr('fa-focus');
                lcOrderDivList.removeAttr('fa-fullinfo');
                lcOrderDivList.removeAttr('fa-floatbuttonbar');
                lcOrderDivList.removeAttr('fa-focus');
            }
        }
    });


    $(this).find('img[href="@cmd%rejectorder"]').unbind('click');
    $(this).find('img[href="@cmd%rejectorder"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcOrderDiv = $(this).closest('[sa-elementtype=element]');
        var lcControl = lcOrderDiv.closest('[sa-elementtype=control]');

        lcControl.attr('fa-popup', 'reject');
    });

    $(this).find('img[href="@cmd%rejectitem"]').unbind('click');
    $(this).find('img[href="@cmd%rejectitem"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcOrderDiv = $(this).closest('[sa-elementtype=element]');
        var lcControl = lcOrderDiv.closest('[sa-elementtype=control]');
        var lcEntryRow = $(this).closest('[sa-elementtype=row]');
        var lcFocusedEntryRow = lcOrderDiv.find('[sa-elementtype=row][fa-focus]');

        lcFocusedEntryRow.removeAttr('fa-focus');
        lcEntryRow.attr('fa-focus', 'true');
        lcControl.attr('fa-popup', 'reject');
    });

    $(this).find('img[href="@cmd%changeorderstate"]').unbind('click');
    $(this).find('img[href="@cmd%changeorderstate"]').click(function (paEvent) {
        var lcOrderDiv = $(this).closest('[sa-elementtype=element]');
        var lcOrderNo = lcOrderDiv.attr('ea-dataid');
        var lcOrderStatus = lcOrderDiv.attr('ea-status');
        var lcNewOrderStatus = $(this).attr('ea-status');
        
        if (!IsDemoMode()) {
            if (lcOrderStatus != lcNewOrderStatus) {
                lcOrderDiv.HideButtonBar();

                lcOrderDiv.attr('fa-ajaxrunning', 'true');

                var lcDataBlock = { OrderNo: lcOrderNo, NewOrderStatus: lcNewOrderStatus, OrderStatus: lcOrderStatus };

                var lcParameter = {
                    QueryName: 'UpdateOrder',
                    ActionFailMessageIndex: 0,
                    DefaultActionFailMessage: "Unable to Update Order.",
                    ServerFailMessageIndex: 1,
                    DefaultServerFailMessage: "Unable to Connect Server.",
                };

                lcOrderDiv.UpdateOrder(lcParameter, Base64.encode(JSON.stringify(lcDataBlock)), -2).done(function (paStatus) {
                    if (paStatus != 'AJAX_ERROR') {
                        if (paStatus == lcNewOrderStatus) {
                            lcOrderDiv.attr('fa-statuschanging', true);
                            setTimeout(function () {
                                lcOrderDiv.ChangeStatus(paStatus);
                            }, 3000);
                        }
                        else {
                            lcForm.ShowFormMessage(lcParameter.ActionFailMessageIndex, lcParameter.DefaultActionFilMessage, false).done(function () {
                                if (paStatus != lcOrderStatus) {
                                    lcOrderDiv.attr('fa-statuschanging', true);
                                    setTimeout(function () {
                                        lcOrderDiv.ChangeStatus(paStatus);
                                    }, 3000);
                                }
                            });

                        }
                    }
                    lcOrderDiv.removeAttr('fa-ajaxrunning');
                });
            }
        }
    });
}

$.fn.ChangeStatus = function(paStatus)
{
    var lcOrderDiv = $(this);
    var lcControl = $(this).closest('[sa-elementtype=control]');

    lcOrderDiv.removeAttr('fa-statuschanging');
    lcOrderDiv.SetOrderStatus(paStatus);    

    if (lcOrderDiv.attr('fa-focus') == 'true') lcControl.RemoveFocus();
}

$.fn.RemoveFocus = function()
{
    var lcControl = $(this);

    lcControl.removeAttr('fa-focus');
    lcControl.find('[fa-focus]').removeAttr('fa-focus');

    lcControl.find('[sa-elementtype=element]').removeAttr('fa-floatbuttonbar');
    lcControl.find('[sa-elementtype=element]').removeAttr('fa-fullinfo');
}

var RefreshTimerObject = {
    TimerID: 0,

    StartTimer: function () {
        RefreshTimerObject.TimerID = setInterval(RefreshTimerObject.TimerTick, 1000);
    },

    StopTimer: function () { clearInterval(RefreshTimerObject.TimerID); },

    CountDownRequire: function (paControl) {
        var lcDate = new Date();
        var lcSecondCount = parseInt(lcDate.getTime() / 1000);
        var lcPrevSecondCount = parseInt(paControl.attr('fa-secondcount'));

        if (lcSecondCount != lcPrevSecondCount) {
            paControl.attr('fa-secondcount', lcSecondCount);
            return (true);
        }
        else return (false);
    },

    TimerTick: function (paForceRefresh) {
        var lcControl = $('[sa-elementtype=control].WidControlBEOrderList');
        var lcTimerDiv = lcControl.find('[sa-elementtype=timer]');

        if (lcControl.attr('fa-refreshing') != 'true') {
            var lcTimerValue = isNaN(lcTimerDiv.attr('value')) ? lcControl.attr('ea-timeout') * 60 : parseInt(lcTimerDiv.attr('value'));

            if (lcTimerValue <= 0) lcTimerDiv.attr('value', lcControl.attr('ea-timeout') * 60);
            else {
                if (paForceRefresh == 'true') lcTimerValue = 0;
                else {
                    if (RefreshTimerObject.CountDownRequire(lcControl))
                        lcTimerValue = lcTimerValue - 1;
                }

                lcTimerDiv.attr('value', lcTimerValue);

                if (lcTimerValue == 0) {
                    RefreshTimerObject.StopTimer();
                    lcControl.RefreshInfo().done(function () {
                        RefreshTimerObject.StartTimer();
                    });
                }
            }

            var lcMinutes = parseInt(lcTimerValue / 60);
            var lcSeconds = lcTimerValue % 60;

            lcTimerDiv.text(('00' + lcMinutes).slice(-2) + ':' + ('00' + lcSeconds).slice(-2));
        }
    }
}

$.fn.CancelOrder = function(paRejectReason)
{
    var lcOrderDiv = $(this);
    var lcControl = $(this).closest('[sa-elementtype=control]');
    var lcOrderNo = $(this).attr('ea-dataid');
    var lcOrderStatus = $(this).attr('ea-status');
    var lcRejectReasonDiv = $(this).find('.OrderRemarkDiv');
    var lcFocusEntryRow = $(this).find('[sa-elementtype=row][fa-focus]');
    var lcActiveRows = $(this).find('[sa-elementtype=row]').not('[ea-status="-2"],[ea-status="-1"]');    
    var lcItemRejectMode = ((lcFocusEntryRow.length > 0) && (lcActiveRows.length > 1));

    lcOrderDiv.HideButtonBar();
    
    lcOrderDiv.attr('fa-ajaxrunning','true');

    if (lcItemRejectMode) {
        lcOrderStatus = lcFocusEntryRow.attr('ea-status');
        var lcItemID = lcFocusEntryRow.attr('ea-dataid');
        var lcItemRejectReasonDiv = lcFocusEntryRow.find('.RejectReason');

        var lcDataBlock = { OrderNo: lcOrderNo, OrderItemId: lcItemID, RejectReason: paRejectReason, OrderItemStatus: lcOrderStatus };

        var lcParameter = {
            QueryName: 'RejectItem',
            ActionFailMessageIndex: 0,
            DefaultActionFailMessage: "Unable to Reject Order.",
            ServerFailMessageIndex: 1,
            DefaultServerFailMessage: "Unable to Connect Server.",
        };
    }
    else {
        var lcDataBlock = { OrderNo: lcOrderNo, RejectReason: paRejectReason, OrderStatus: lcOrderStatus };

        var lcParameter = {
            QueryName: 'RejectOrder',
            ActionFailMessageIndex: 0,
            DefaultActionFailMessage: "Unable to Reject Order.",
            ServerFailMessageIndex: 1,
            DefaultServerFailMessage: "Unable to Connect Server.",
        };
    }
        
    lcOrderDiv.UpdateOrder(lcParameter, Base64.encode(JSON.stringify(lcDataBlock)), -2).done(function (paStatus)
    {
        if (paStatus != 'AJAX_ERROR') {
            if (paStatus == -2) {
                if (lcItemRejectMode) {
                    lcFocusEntryRow.attr('ea-status', paStatus);
                    lcOrderDiv.RecalculateTotal(paStatus);
                    lcItemRejectReasonDiv.text(paRejectReason);
                }
                else {
                        lcOrderDiv.attr('fa-statuschanging', true);
                        setTimeout(function () {
                            lcOrderDiv.ChangeStatus(paStatus);                                                        
                        }, 3000);

                        if (paRejectReason.length > 0) lcRejectReasonDiv.attr('ea-hasvalue', true);
                        else lcRejectReasonDiv.removeAttr('ea-hasvalue');

                        lcRejectReasonDiv.text(paRejectReason);
                }
            }
            else {
                lcForm.ShowFormMessage(lcParameter.ActionFailMessageIndex, lcParameter.DefaultActionFilMessage, false).done(function () {
                    if (lcItemRejectMode) {
                        lcFocusEntryRow.attr('ea-status', paStatus);
                        lcOrderDiv.RecalculateTotal(paStatus);
                    }
                    else {                        
                            if (paStatus != lcOrderStatus)
                            {
                                lcOrderDiv.attr('fa-statuschanging', true);
                                setTimeout(function () {
                                    lcOrderDiv.ChangeStatus(paStatus);
                                }, 3000);
                            }
                    }
                });
            }
        }
        lcOrderDiv.removeAttr('fa-ajaxrunning');
    });        
}


$.fn.RecalculateTotal = function()
{
    var lcActiveRows = $(this).find('[sa-elementtype=row]').not('[ea-status="-2"],[ea-status="-1"]');
    var lcTotalPriceDiv    = $(this).find('[sa-elementtype=total].TotalPrice');
    var lcTotalQuantityDiv = $(this).find('[sa-elementtype=quantity].TotalQuantity');

    var lcTotalAmount   = 0;
    var lcTotalQuantity = 0;

    lcActiveRows.each(function () {
        var lcSubTotalDiv = $(this).find('[sa-elementtype=total]');
        var lcQuantityDiv = $(this).find('[sa-elementtype=quantity]');
        
        var lcSubStotal = isNaN(lcSubTotalDiv.text()) ? 0 : parseInt(lcSubTotalDiv.text());
        var lcQuantity = isNaN(lcQuantityDiv.text()) ? 0 : parseInt(lcQuantityDiv.text());

        lcTotalAmount += lcSubStotal;
        lcTotalQuantity += lcQuantity;
    });

    lcTotalPriceDiv.text(lcTotalAmount);
    lcTotalQuantityDiv.text(lcTotalQuantity);
}

$.fn.SetOrderStatus = function(paStatus)
{
    var lcControl = $(this).closest('[sa-elementtype=control]');
    var lcStatusTextControl = $(this).find('.OrderHeadingDiv [ea-type=status].HeadingText');

    $(this).attr('ea-status', paStatus);    
    lcStatusTextControl.text(lcControl.GetStatusDictionary()[paStatus]);

    if ((paStatus == -1) || (paStatus == -2))
    {
        var lcTotalQuantity = $(this).find('.BriefDescriptionDiv .TotalQuantity');
        var lcTotalPrice = $(this).find('.BriefDescriptionDiv .TotalPrice');

        lcTotalQuantity.text("0");
        lcTotalPrice.text("0");
    }
}

$.fn.GetStatusDictionary = function()
{    
    lcStatusInfo = $(this).attr('ea-additionaldata');

    lcStatusInfo = lcStatusInfo.substring(2, lcStatusInfo.length - 2);

    var lcStatusDictionary = {};
    var lcStatusTypeArray = lcStatusInfo.split(';;');

    for (lcCount = 0; lcCount < lcStatusTypeArray.length; lcCount++) {
        var lcContent = lcStatusTypeArray[lcCount].split('::');

        if (lcContent.length == 2) lcStatusDictionary[lcContent[0]] = lcContent[1];
    }

    return(lcStatusDictionary);
}

$.fn.HideDetail = function () {
        
    var lcControl = $(this).closest('[sa-elementtype=control]');
    $(this).removeAttr('fa-fullinfo');

    $(this).removeAttr('fa-focus');
    lcControl.removeAttr('fa-focus');
}

$.fn.HideButtonBar = function() {
    var lcControl = $(this).closest('[sa-elementtype=control]');

    $(this).removeAttr('fa-floatbuttonbar');

    $(this).removeAttr('fa-focus');
    lcControl.removeAttr('fa-focus');
}

$.fn.UpdateOrder = function (paParameter, paDataBlock) {
    var lcForm = $(this).closest('[sa-elementtype=form]');
    var lcDeferred = $.Deferred();

    var lcData = { CobraAjaxRequest: "executescalarquery", Parameter: paParameter.QueryName, DataBlock: paDataBlock };

    GlobalAjaxHandler.SetAjaxLoaderSuppressState(true);

    GlobalAjaxHandler.SetAjaxErrorHandler(function () {
        lcDeferred.resolve('AJAX_ERROR');
    });

    DoPostBack(lcData, function (paResponseData) {
        var lcRespondStruct = jQuery.parseJSON(paResponseData);
        if (lcRespondStruct.Success) {
            var lcStatus = Number(lcRespondStruct.ResponseData.RSP_Result);            
            lcDeferred.resolve(lcStatus);
        }
        else {
            lcForm.ShowFormMessage(paParameter.ServerFailMessageIndex, paParameter.DefaultServerFailMessage, false).done(function () {
                lcDeferred.resolve('NO_RESPONSE');
            });
        }
        GlobalAjaxHandler.SetAjaxLoaderSuppressState(false);
    });    

    return (lcDeferred);
}

$.fn.RefreshInfo = function()
{
    var lcControl = $(this);
    var lcDeferred = $.Deferred();
    var lcForm      = lcControl.closest('[sa-elementtype=form]');
    var lcBufferDiv = lcForm.find('[sa-elementtype=buffer]');
    var lcLastUpdateDiv = lcControl.find('.RefreshBar .StatusDiv .LastUpdateDiv');

    lcControl.attr('fa-refreshing',true);

    $(this).RetrieveUpdatedInfo().done(function (paResult) {
        if ((paResult != 'AJAX_ERROR') && (paResult != 'NO_RESPONSE')) {
            lcBufferDiv.html(paResult);

            var lcListPanel = lcBufferDiv.find('[sa-elementtype=container].ListPanel');
            lcBufferDiv.html(lcListPanel.html());

            var lcUpdatedList = lcBufferDiv.find('[sa-elementtype=element].OrderDiv');
            lcControl.ApplyUpdate(lcUpdatedList);

            lcControl.BindBEOrderListOrderDivEvents();
            
            lcControl.removeAttr('fa-refreshfail');

            var lcDate = new Date();
            var lcTime = ("00" + lcDate.getHours()).slice(-2) + ":" + ("00" + lcDate.getMinutes()).slice(-2);
            lcLastUpdateDiv.text(lcTime);
        }
        else lcControl.attr('fa-refreshfail', true);

        
        lcDeferred.resolve();
        lcControl.removeAttr('fa-refreshing');
    });
    return (lcDeferred);
}

$.fn.ApplyUpdate = function(paUpdatedList)
{
    lcControl = $(this);
    lcListPanel = lcControl.find('[sa-elementtype=container].ListPanel');
        
    if ((paUpdatedList) && (paUpdatedList.length > 0))
    {     
        paUpdatedList.reverse().each(function () {
            var lcDataID = $(this).attr('ea-dataid');
            
            var lcExistingData = lcListPanel.find('[sa-elementtype=element][ea-dataid="' + lcDataID + '"].OrderDiv');

            if (lcExistingData.length > 0)
            {
                lcExistingData.UpdateOrderDivContent($(this));
            }
            else 
            {
                lcListPanel.prepend($(this));
            }
        });
    }
}

$.fn.UpdateOrderDivContent = function(paNewData)
{    
    var lcOrderDiv = $(this);
    var lcCurrentStatus = lcOrderDiv.attr('ea-status');
    var lcNewStatus = paNewData.attr('ea-status');        

    if (lcNewStatus != lcCurrentStatus)
    {        
        lcOrderDiv.attr('fa-statuschanging', true);
        setTimeout(function () {
            lcOrderDiv.ChangeStatus(lcNewStatus);            
        }, 3000);
    }

    lcOrderDiv.UpdateEntryRowData(paNewData);

    lcOrderDiv.RecalculateTotal(lcNewStatus);
}

$.fn.UpdateEntryRowData = function (paNewData)
{
    var lcOrderDiv = $(this);
    var lcCurrentEntryRowList = lcOrderDiv.find('[sa-elementtype=row].EntryRow');

    lcCurrentEntryRowList.each(function () {
        var lcDataID = $(this).attr('ea-dataid');

        var lcNewDataRow = paNewData.find('[sa-elementtype=row][ea-dataid="' + lcDataID + '"]');
        
        if (lcNewDataRow.length > 0) {            
            $(this).replaceWith(lcNewDataRow);
        }
    });
}

$.fn.RetrieveUpdatedInfo = function()
{    
    var lcDeferred = $.Deferred();

    var lcData = { CobraAjaxRequest: "getupdatedcontrol" };

    GlobalAjaxHandler.SetAjaxLoaderSuppressState(true);
    GlobalAjaxHandler.SetAjaxErrorSuppressState(true);

    GlobalAjaxHandler.SetAjaxErrorHandler(function () {        
        lcDeferred.resolve('AJAX_ERROR');
    });

    DoPostBackReadMode(lcData, function (paResponseData) {
        var lcRespondStruct = jQuery.parseJSON(paResponseData);
        if (lcRespondStruct.Success) {
            var lcHTML = lcRespondStruct.ResponseData.RSP_HTML;
            lcDeferred.resolve(lcHTML);
        }
        else {            
                lcDeferred.resolve('NO_RESPONSE');            
        }
        GlobalAjaxHandler.SetAjaxLoaderSuppressState(false);
    });

    return (lcDeferred);
}
