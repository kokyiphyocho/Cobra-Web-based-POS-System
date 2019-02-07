$(document).ready(function () {
    $('[sa-elementtype=control].WidControlFEOrderList').BindFEOrderListEvents();
});

$.fn.BindFEOrderListEvents = function () {
    var lcForm = $(this).closest('[sa-elementtype=form]');
    
    RefreshTimerObject.StartTimer();

    $(this).find('input[ea-inputmode=number]').ForceIntegerInput();  

    $(this).find('img[href="@cmd%addquantity"]').unbind('click');
    $(this).find('img[href="@cmd%addquantity"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcButtonPanel = $(this).parent();
        var lcItemBlock = lcButtonPanel.closest('[sa-elementtype=element]');
        var lcComposite = lcItemBlock.closest('[sa-elementtype=composite]');
        var lcQuantityBox = lcButtonPanel.find("input[type=text]");
        
        var lcQuantity = isNaN(lcQuantityBox.val()) || lcQuantityBox.val().trim() == '' ? 0 : parseInt(lcQuantityBox.val());

        if (lcQuantity < 99) {

            if (lcQuantity < 0) lcQuantity = 0;

            lcQuantity = lcQuantity + 1;
            lcQuantityBox.val(lcQuantity);

            lcItemBlock.removeAttr('fa-cancelled');
            lcItemBlock.RefreshSubTotalPrice();
        
            lcComposite.RefreshTotalPrice();
        }
    });


    $(this).find('a[href="@cmd%refresh"]').unbind('click');
    $(this).find('a[href="@cmd%refresh"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcControl = $(this).closest('[sa-elementtype=control]');
        var lcRefreshBar = $(this).closest('.RefreshBar');
        var lcTimer = lcRefreshBar.find('[sa-elementtype=timer]');
        var lcTimeOut = $(this).attr('ea-timeout');

        if (lcTimeOut == 0) RefreshTimerObject.TimerTick('true');
        else {
            lcControl.attr('ea-timeout', lcTimeOut);
            lcTimer.attr('value', lcTimeOut * 60);
        }
    });

    $(this).find('img[href="@cmd%subquantity"]').unbind('click');
    $(this).find('img[href="@cmd%subquantity"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcButtonPanel = $(this).parent();
        var lcItemBlock = lcButtonPanel.closest('[sa-elementtype=element]');
        var lcComposite = lcItemBlock.closest('[sa-elementtype=composite]');
        var lcQuantityBox = lcButtonPanel.find("input[type=text]");        

        var lcQuantity = isNaN(lcQuantityBox.val()) ? 0 : parseInt(lcQuantityBox.val());

        if (lcQuantity > 0) {
            lcQuantity = lcQuantity - 1;
            lcQuantityBox.val(lcQuantity);
            lcItemBlock.RefreshSubTotalPrice();

            lcComposite.RefreshTotalPrice();

            if (lcQuantity <= 0)
            {
                lcItemBlock.attr('fa-cancelled', 'true');
            }
        }
    });

    $(this).find('img[href="@cmd%editorder"]').unbind('click');
    $(this).find('img[href="@cmd%editorder"]').click(function (paEvent) {
        paEvent.preventDefault();
        
        var lcActiveOrder = $(this).closest('[sa-elementtype=element]');
        var lcControl = $(this).closest('[sa-elementtype=control]');
        var lcForm = $(this).closest('[sa-elementtype=form]');

        lcActiveOrder.attr('fa-editing', 'true');
        lcControl.attr('fa-editing', 'true');
        lcForm.find('[sa-elementtype=toolbar]').hide();        
    });

    $(this).find('img[href="@cmd%showemark"]').unbind('click');
    $(this).find('img[href="@cmd%showremark"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcItemBlock = $(this).closest('[sa-elementtype=element]');

        lcItemBlock.attr('fa-remarkopen', 'true');
    });

    $(this).find('img[href="@cmd%hideremark"]').unbind('click');
    $(this).find('img[href="@cmd%hideremark"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcItemBlock = $(this).closest('[sa-elementtype=element]');

        lcItemBlock.removeAttr('fa-remarkopen');
    });
   

    //$(this).find('a[href="@cmd%placeorder"]').unbind('click');
    //$(this).find('a[href="@cmd%placeorder"]').click(function (paEvent) {
    //    paEvent.preventDefault();

    //    var lcForm = $(this).closest('[sa-elementtype=form]');
    //    var lcControl = $(this).closest('[sa-elementtype=control]');
    //    var lcAddressControl = lcControl.find('[sa-elementtype=container][ea-type=deliveryinfo] [sa-elementtype=control]');
    //    var lcOrderRemark = lcControl.find('.OrderRemarkPanel input[type=text]').val();

    //    var lcItemList = lcControl.GetSerializedOrderData();
    //    var lcDeliveryInfo = lcAddressControl.GetSerializedAddressData({ OrderRemark: lcOrderRemark });

    //    var lcData = { CobraAjaxRequest: "placeorder", ItemList: lcItemList, DeliveryInfo: lcDeliveryInfo };
    //    GlobalAjaxHandler.SetAjaxLoaderStatusText('Submitting .....');

    //    DoPostBack(lcData, function (paResponseData) {
    //        var lcRespondStruct = jQuery.parseJSON(paResponseData);
    //        if (lcRespondStruct.Success) {
    //            lcForm.ShowFormMessage(3, 'Successfully Updated', false).done(function (paResult, paControl) {
    //                lcForm.ResetForm();
    //            });
    //        }
    //        else {
    //            lcForm.ShowFormMessage(1, 'Error in Updating Data.', true);
    //        }
    //    });
    //});

    $(this).find('img[href="@cmd%back"]').unbind('click');
    $(this).find('img[href="@cmd%back"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcForm = $(this).closest('[sa-elementtype=form]');
        var lcControl = lcForm.find('[sa-elementtype=control].WidControlFEOrderList');
        
        lcControl.removeAttr('fa-editing');
        lcControl.find('[sa-elementtype=element][fa-editing]').removeAttr('fa-editing');
        lcForm.find('[sa-elementtype=toolbar]').show();
    });

    $(this).find('img[href="@cmd%deleteorder"]').unbind('click');
    $(this).find('img[href="@cmd%deleteorder"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcForm = $(this).closest('[sa-elementtype=form]');
        var lcControl = lcForm.find('[sa-elementtype=control].WidControlFEOrderList');
        var lcComposite = $(this).closest('[sa-elementtype=composite]');
        var lcOrderDiv = $(this).closest('[sa-elementtype=element]');
        var lcOrderNo = lcComposite.attr('ea-dataid');
        var lcDataBlock = { OrderNo: lcOrderNo };

        lcForm.ShowFormMessage(5, 'Do you want to cancel order ?', false).done(function (paResult, paControl) {
            if (paResult == 'yes') {

                var lcParameter = {
                    QueryName: 'CancelOrder',
                    SuccessMessageIndex: 2,
                    DefaultSuccessMessage: "Successfully Cancelled.",
                    ActionFailMessageIndex: 3,
                    DefaultActionFailMessage: "Unable to Cancel Order.",
                    ServerFailMessageIndex: 1,
                    DefaultServerFailMessage: "Unable to Connect Server.",
                };

                lcComposite.CancelOrder(lcParameter,Base64.encode(JSON.stringify(lcDataBlock))).done(function(paResult)
                {
                    if (paResult)
                    {
                        lcControl.removeAttr('fa-editing');
                        lcControl.find('[sa-elementtype=element][fa-editing]').removeAttr('fa-editing');
                        lcForm.find('[sa-elementtype=toolbar]').show();
                        lcOrderDiv.find('.OrderHeadingDiv').attr('ea-appearance', 'cancel');
                        lcOrderDiv.find('.BriefDescriptionDiv .EntryRow').attr('ea-appearance', 'cancel');
                        lcOrderDiv.find('.BriefDescriptionDiv .TotalQuantity').text('0');
                        lcOrderDiv.find('.BriefDescriptionDiv .TotalPrice').text('0');
                    }
                });
                

            }
        });
    });

    $(this).find('img[href="@cmd%updateorder"]').unbind('click');
    $(this).find('img[href="@cmd%updateorder"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcForm = $(this).closest('[sa-elementtype=form]');
        var lcControl = lcForm.find('[sa-elementtype=control].WidControlFEOrderList');
        var lcComposite = $(this).closest('[sa-elementtype=composite]');
        var lcOrderNo = lcComposite.attr('ea-dataid');
        var lcRemarkText = lcComposite.find('.OrderRemarkPanel .OrderRemarkBox').val();
        
        lcForm.ShowFormMessage(4, 'Do you want to update order information ?', false).done(function (paResult, paControl) {
            if (paResult == 'yes') {
                
                var lcParameter = {
                    OrderNo: lcOrderNo,
                    OrderRemark : lcRemarkText,
                    SuccessMessageIndex: 0,
                    DefaultSuccessMessage: "Successfully Updated the Order Information.",                    
                    ServerFailMessageIndex: 1,
                    DefaultServerFailMessage: "Unable to Connect Server.",
                };
                
                lcComposite.CheckOrderStatus().done(function (paVerified) {
                
                    if (paVerified) {                        
                        lcComposite.UpdateOrder(lcParameter, lcComposite.GetSerializedOrderData()).done(function (paResult) {
                            if (paResult) {
                                location.reload();
                            }

                        });
                    }
                    else {
                        location.reload();
                    }
                });

                
            }
        });        
    });
}

//$.fn.BindFEOrderListOrderDivEvents = function () {

//}

var RefreshTimerObject = {
    TimerID : 0,

    StartTimer: function ()
    {
        RefreshTimerObject.TimerID = setInterval(RefreshTimerObject.TimerTick, 1000);
    },

    StopTimer: function () { clearInterval(RefreshTimerObject.TimerID); },

    CountDownRequire : function (paControl) {
        var lcDate = new Date();
        var lcSecondCount = parseInt(lcDate.getTime() / 1000);
        var lcPrevSecondCount = parseInt(paControl.attr('fa-secondcount'));

        if (lcSecondCount != lcPrevSecondCount) {
            paControl.attr('fa-secondcount', lcSecondCount);
            return (true);
        }
        else return (false);
    },

    TimerTick : function (paForceRefresh) {
        var lcControl = $('[sa-elementtype=control].WidControlFEOrderList');
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

$.fn.RefreshInfo = function () {
    var lcControl = $(this);
    var lcDeferred = $.Deferred();
    var lcForm = lcControl.closest('[sa-elementtype=form]');
    var lcBufferDiv = lcForm.find('[sa-elementtype=buffer]');
    var lcLastUpdateDiv = lcControl.find('.RefreshBar .StatusDiv .LastUpdateDiv');

    lcControl.attr('fa-refreshing', true);

    $(this).RetrieveUpdatedInfo().done(function (paResult) {
        if ((paResult != 'AJAX_ERROR') && (paResult != 'NO_RESPONSE')) {
            lcBufferDiv.html(paResult);

            var lcListPanel = lcBufferDiv.find('[sa-elementtype=container].ListPanel');
            lcBufferDiv.html(lcListPanel.html());

            var lcUpdatedList = lcBufferDiv.find('[sa-elementtype=element].OrderDiv');
            lcControl.ApplyUpdate(lcUpdatedList);
            
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

$.fn.RetrieveUpdatedInfo = function () {
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

$.fn.ApplyUpdate = function (paUpdatedList) {
    lcControl = $(this);
    lcListPanel = lcControl.find('[sa-elementtype=container].ListPanel');
    
    if ((paUpdatedList) && (paUpdatedList.length > 0)) {
        paUpdatedList.reverse().each(function () {
            var lcDataID = $(this).attr('ea-dataid');

            var lcExistingData = lcListPanel.find('[sa-elementtype=element][ea-dataid="' + lcDataID + '"].OrderDiv');            
            if (lcExistingData.length > 0) {
                lcExistingData.UpdateOrderDivContent($(this));
            }
            
        });
    }
}

$.fn.UpdateOrderDivContent = function (paNewData) {
    var lcOrderDiv = $(this);
    var lcCurrentStatus = lcOrderDiv.attr('ea-status');
    var lcNewStatus = paNewData.attr('ea-status');
    var lcComposite = lcOrderDiv.find('[sa-elementtype=composite]');
    var lcNewComposite = paNewData.find('[sa-elementtype=composite]');
    
    if (lcNewStatus != lcCurrentStatus) {
        lcOrderDiv.attr('fa-statuschanging', true);
        setTimeout(function () {
            lcOrderDiv.ChangeStatus(lcNewStatus);
        }, 3000);
    }

    lcOrderDiv.UpdateEntryRowData(paNewData); 
    lcComposite.UpdateItemBlocks(lcNewComposite);
}

$.fn.ChangeStatus = function (paStatus) {
    var lcOrderDiv = $(this);
    var lcControl = $(this).closest('[sa-elementtype=control]');

    lcOrderDiv.SetOrderStatus(paStatus);
    lcOrderDiv.removeAttr('fa-statuschanging');
}

$.fn.SetOrderStatus = function (paStatus) {
    var lcControl = $(this).closest('[sa-elementtype=control]');
    var lcStatusTextControl = $(this).find('.OrderHeadingDiv [ea-type=status].HeadingText');

    $(this).attr('ea-status', paStatus);
    lcStatusTextControl.text(lcControl.GetStatusDictionary()[paStatus]);

    if ((paStatus == -1) || (paStatus == -2)) {
        var lcTotalQuantity = $(this).find('.BriefDescriptionDiv .TotalQuantity');
        var lcTotalPrice = $(this).find('.BriefDescriptionDiv .TotalPrice');

        lcTotalQuantity.text("0");
        lcTotalPrice.text("0");
    }
}

$.fn.GetStatusDictionary = function () {
    lcStatusInfo = $(this).attr('ea-additionaldata');

    lcStatusInfo = lcStatusInfo.substring(2, lcStatusInfo.length - 2);

    var lcStatusDictionary = {};
    var lcStatusTypeArray = lcStatusInfo.split(';;');

    for (lcCount = 0; lcCount < lcStatusTypeArray.length; lcCount++) {
        var lcContent = lcStatusTypeArray[lcCount].split('::');

        if (lcContent.length == 2) lcStatusDictionary[lcContent[0]] = lcContent[1];
    }

    return (lcStatusDictionary);
}


$.fn.UpdateEntryRowData = function (paNewData) {
    var lcOrderDiv = $(this);
    var lcCurrentEntryRowList = lcOrderDiv.find('[sa-elementtype=row].EntryRow');
    var lcSummaryRow = lcOrderDiv.find('[sa-elementtype=summary].TotalSummary');
    var lcNewSummaryRow = paNewData.find('[sa-elementtype=summary].TotalSummary');

    lcCurrentEntryRowList.each(function () {
        var lcDataID = $(this).attr('ea-dataid');

        var lcNewDataRow = paNewData.find('[sa-elementtype=row][ea-dataid="' + lcDataID + '"]');

        if (lcNewDataRow.length > 0) {
            $(this).replaceWith(lcNewDataRow);
        }
    });

    lcSummaryRow.replaceWith(lcNewSummaryRow);
}

$.fn.UpdateItemBlocks = function (paNewComposite) {
    var lcComposite = $(this);
    var lcItemBlockList = lcComposite.find('[sa-elementtype=element].ItemBlock');

    lcItemBlockList.each(function () {
        var lcDataID = $(this).attr('ea-dataid');

        var lcNewItemBlock = paNewComposite.find('[sa-elementtype=element][ea-dataid="' + lcDataID + '"].ItemBlock');

        if (lcNewItemBlock.length == 0) {
            $(this).remove();
        }
    });
}


$.fn.RefreshSubTotalPrice = function () {
    var lcUnitPriceBox = $(this).find('[sa-elementtype=unitprice]');
    var lcTotalBox = $(this).find('[sa-elementtype=total]');
    var lcQuantityBox = $(this).find('[sa-elementtype=quantity]');

    var lcUnitPrice = isNaN(lcUnitPriceBox.text()) ? 0 : parseInt(lcUnitPriceBox.text());
    var lcQuantity = isNaN(lcQuantityBox.val()) ? 0 : parseInt(lcQuantityBox.val());

    lcTotalBox.text(lcUnitPrice * lcQuantity);
}

$.fn.RefreshTotalPrice = function () {

    var lcControlList = $(this).find('[sa-elementtype=element]');
    var lcTotalQuantityBox = $(this).find('[sa-elementtype=summary] [sa-elementtype=quantity]');
    var lcTotalPriceBox = $(this).find('[sa-elementtype=summary] [sa-elementtype=total]');
    
    var lcTotalQuantity = 0;
    var lcTotalPrice = 0;
    
    lcControlList.each(function () {
        var lcTotalBox = $(this).find('[sa-elementtype=total]');
        var lcQuantityBox = $(this).find('[sa-elementtype=quantity]');

        lcTotalQuantity += isNaN(lcQuantityBox.val()) ? 0 : parseInt(lcQuantityBox.val());
        lcTotalPrice += isNaN(lcTotalBox.text()) ? 0 : parseInt(lcTotalBox.text());
    })

    lcTotalQuantityBox.text(lcTotalQuantity);
    lcTotalPriceBox.text(lcTotalPrice);
}

$.fn.CancelOrder = function (paParameter, paDataBlock) {    
    var lcForm = $(this).closest('[sa-elementtype=form]');    
    var lcDeferred = $.Deferred();
    
    var lcData = { CobraAjaxRequest: "executescalarquery", Parameter: paParameter.QueryName, DataBlock: paDataBlock};    
    GlobalAjaxHandler.SetAjaxLoaderStatusText('Updating to System .....');

    DoPostBack(lcData, function (paResponseData) {
        var lcRespondStruct = jQuery.parseJSON(paResponseData);
        if (lcRespondStruct.Success) {
            var lcStatus = Number(lcRespondStruct.ResponseData.RSP_Result);
            if (lcStatus == -1) {
                lcForm.ShowFormMessage(paParameter.SuccessMessageIndex, paParameter.DefaultSuccessMessage, false).done(function () {                    
                    lcDeferred.resolve(true);
                });
            }
            else {
                lcForm.ShowFormMessage(paParameter.ActionFailMessageIndex, paParameter.DefaultActionFilMessage, false).done(function () {
                    lcDeferred.resolve(false);
                });
            }
        }
        else {
                lcForm.ShowFormMessage(paParameter.ServerFailMessageIndex, paParameter.DefaultServerFailMessage, false).done(function () {
                lcDeferred.resolve(false);
            });
        }
    });

    return (lcDeferred);
}

$.fn.UpdateOrder = function (paParameter, paDataBlock) {
    var lcComposite = $(this);
    var lcForm = $(this).closest('[sa-elementtype=form]');
    var lcDeferred = $.Deferred();
    
    var lcData = { CobraAjaxRequest: "updateorder", Parameter: paParameter.OrderNo, SecondParameter : paParameter.OrderRemark, ItemList: paDataBlock };
    GlobalAjaxHandler.SetAjaxLoaderStatusText('Updating to System .....');
    
    DoPostBack(lcData, function (paResponseData) {
        var lcRespondStruct = jQuery.parseJSON(paResponseData);
        if (lcRespondStruct.Success) {
            
            lcForm.ShowFormMessage(paParameter.SuccessMessageIndex, paParameter.DefaultSuccessMessage, false).done(function () {            
                    lcDeferred.resolve(true);
                });                        
        }
        else {
            lcForm.ShowFormMessage(paParameter.ServerFailMessageIndex, paParameter.DefaultServerFailMessage, false).done(function () {
                lcDeferred.resolve(false);
            });
        }
    });

    return (lcDeferred);
}

$.fn.CheckOrderStatus = function () {
    var lcForm = $(this).closest('[sa-elementtype=form]');
    var lcControl = $(this).closest('[sa-elementtype=control]');    
    
    var lcDeferred      = $.Deferred();
    var lcOrderNo       = $(this).attr('ea-dataid');
    var lcDataBlock     = Base64.encode(JSON.stringify({ OrderNo: lcOrderNo }));

    var lcData = { CobraAjaxRequest: "executescalarquery", Parameter: "OrderStatus", DataBlock : lcDataBlock };
    GlobalAjaxHandler.SetAjaxLoaderStatusText('Verifying Order Status .....');
    
    DoPostBackReadMode(lcData, function (paResponseData) {
        var lcRespondStruct = jQuery.parseJSON(paResponseData);
        if (lcRespondStruct.Success) {
            var lcStatus = Number(lcRespondStruct.ResponseData.RSP_Result);
            if ((lcStatus == 0) || (lcStatus == 1))  {                
                lcDeferred.resolve(true);
            }
            else
            {
                lcForm.ShowFormMessage(6, "Order status has changed. Unable to update.").done(function () {
                    lcDeferred.resolve(false);
                });
            }            
        }
        else {
            lcForm.ShowFormMessage(1, "Unable to Connect to Server").done(function () {
                lcDeferred.resolve(false);
            });
        }
    });

    return (lcDeferred);

}


$.fn.GetSerializedOrderData = function () {
    var lcMainBlock = [];
        
    var lcOrderedItem = $(this).find('[sa-elementtype=element]');
    
    lcOrderedItem.each(function (paIndex) {
        var lcFieldBlock = {};
        
        lcFieldBlock['ItemID'] = $(this).attr('ea-dataid');        
        if ($(this).attr('fa-cancelled') == 'true') {
            lcFieldBlock['Action'] = 'CANCEL';
            $(this).find('[sa-elementtype=quantity]').val($(this).find('[sa-elementtype=quantity]').attr('ea-originalvalue'));
            $(this).find('.RemarkBox').val($(this).find('.RemarkBox').attr('ea-originalvalue'));
        }

        $(this).find('[ea-columnname]').each(function () {
            if ($(this).is('div'))
                lcFieldBlock[$(this).attr('ea-columnname')] = $(this).text().trim();
            else
                lcFieldBlock[$(this).attr('ea-columnname')] = $(this).val();
        });

        lcMainBlock.push(lcFieldBlock);
    });

    return (Base64.encode(JSON.stringify(lcMainBlock)));
}


