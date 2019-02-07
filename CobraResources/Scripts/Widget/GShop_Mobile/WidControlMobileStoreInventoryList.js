$(document).ready(function () {
    $('[sa-elementtype=control].WidControlMobileStoreInventoryList').MobileStoreInventoryListEvents();
    $('[sa-elementtype=control].WidControlMobileStoreInventoryList').RefreshActiveGroup();
});

$.fn.MobileStoreInventoryListEvents = function () {
    var lcForm = $(this).closest('[sa-elementtype=form]');
    //var lcTextBoxes = $(this).find('[sa-elementtype=container] [sa-elementtype=element] input[type=text]');
    // var lcFilterControl = ;

    //lcTextBoxes.ForceIntegerInput();
    //// No need to Unbind Blur as ForceIntegerInput already unbind.
    //lcTextBoxes.blur(function (paEvent) {
    //    var lcNewValue = Number($(this).val());
    //    var lcOriginalValue = Number($(this).attr('ea-originalvalue'));
    //    var lcFieldRow = $(this).closest('[sa-elementtype=element]');

    //    lcFieldRow.RefreshValueChangeStatus();

    //    lcFieldRow.removeAttr('fa-focus');
    //});

    //lcTextBoxes.focus(function (paEvent) {
    //    var lcFieldRow = $(this).closest('[sa-elementtype=element]');
    //    var lcControl = lcFieldRow.closest('[sa-elementtype=control]');
    //    var lcContainer = lcFieldRow.closest('[sa-elementtype=container]');
    //    var lcFieldRowList = lcContainer.find('[sa-elementtype=element]');

    //    $(this).select();
    //    lcFieldRowList.removeAttr('fa-focus');
    //    lcFieldRow.attr('fa-focus', 'true');
    //    lcFieldRowList.not('[fa-focus]').removeAttr('fa-showusualprice');
    //});

    $(this).find('[sa-elementtype=filtercontrol]').change(function (paEvent) {
        $('[sa-elementtype=control].WidControlMobileStoreInventoryList').RefreshActiveGroup();
    });

    lcForm.find('a[href="@cmd%editmode"]').unbind('click');
    lcForm.find('a[href="@cmd%editmode"]').click(function (paEvent) {
        paEvent.preventDefault();
        var lcForm = $(this).closest('[sa-elementtype=form]');
        var lcControl = lcForm.find('[sa-elementtype=control].WidControlMobileStoreInventoryList');
        
        if (lcControl.attr('fa-controlmode') == 'editmode')
            lcControl.removeAttr('fa-controlmode');
        else
        {
            lcControl.removeAttr('fa-controlmode');
            setTimeout(function () {
                lcControl.attr('fa-controlmode', 'editmode');
            }, 100);
        }
    });

    lcForm.find('a[href="@cmd%deletemode"]').unbind('click');
    lcForm.find('a[href="@cmd%deletemode"]').click(function (paEvent) {
        paEvent.preventDefault();
        var lcForm = $(this).closest('[sa-elementtype=form]');
        var lcControl = lcForm.find('[sa-elementtype=control].WidControlMobileStoreInventoryList');

        if (lcControl.attr('fa-controlmode') == 'deletemode')
            lcControl.removeAttr('fa-controlmode');
        else {
            lcControl.removeAttr('fa-controlmode');
            setTimeout(function () {                
                lcControl.attr('fa-controlmode', 'deletemode');
            }, 100);
        }
    });

    lcForm.find('a[href="@cmd%add"]').unbind('click');
    lcForm.find('a[href="@cmd%add"]').click(function (paEvent) {
        paEvent.preventDefault();
        paEvent.stopPropagation();

        var lcForm = $(this).closest('[sa-elementtype=form]');
        var lcControl = lcForm.find('[sa-elementtype=control].WidControlMobileStoreInventoryList');
        var lcTemplate = lcControl.attr('ea-template');               

        var lcLink = lcTemplate.replace('$ENTRYID', '-1');
        
        if (lcLink) {
            var lcFormStack = lcForm.attr('ea-formstack');

            if (!lcFormStack) lcFormStack = Base64.decode(lcForm.attr('ea-encodedformname'));
            else lcFormStack = Base64.decode(lcFormStack) + '||' + Base64.decode(lcForm.attr('ea-encodedformname'));

            lcLink = "?_f=" + encodeURIComponent(Base64.encode(lcLink)) + '&_s=' + encodeURIComponent(Base64.encode(lcFormStack));
            RedirectPage(lcLink, false);
        }
    });

    $(this).find('img[href="@cmd%edititem"]').unbind('click');
    $(this).find('img[href="@cmd%edititem"]').click(function (paEvent) {
        paEvent.preventDefault();
        paEvent.stopPropagation();

        var lcFieldRow  = $(this).closest('[sa-elementtype=element]');        
        var lcControl   = lcFieldRow.closest('[sa-elementtype=control]');
        var lcForm      = lcFieldRow.closest('[sa-elementtype=form]');
        var lcTemplate = lcControl.attr('ea-template');
        
        var lcLink = lcTemplate.replace('$ENTRYID', lcFieldRow.attr('ea-dataid'));
        
        if (lcLink) {
            var lcFormStack = lcForm.attr('ea-formstack');

            if (!lcFormStack) lcFormStack = Base64.decode(lcForm.attr('ea-encodedformname'));
            else lcFormStack = Base64.decode(lcFormStack) + '||' + Base64.decode(lcForm.attr('ea-encodedformname'));

            lcLink = "?_f=" + encodeURIComponent(Base64.encode(lcLink)) + '&_s=' + encodeURIComponent(Base64.encode(lcFormStack));
            RedirectPage(lcLink, false);
        }
    });

    $(this).find('img[href="@cmd%deleteitem"]').unbind('click');
    $(this).find('img[href="@cmd%deleteitem"]').click(function (paEvent) {
        paEvent.preventDefault();
        var lcFieldRow = $(this).closest('[sa-elementtype=element]');
        var lcControl = lcFieldRow.closest('[sa-elementtype=control]');
        var lcDataID = lcFieldRow.attr('ea-dataid');

        lcForm.ShowFormMessage(0, "Are you sure you want to delete ?%%Yes%%No").then(function (paResult, paControl) {
            if ((paResult) && (paResult.toLowerCase() == "yes")) {                
                lcFieldRow.DeleteInventoryItem(lcDataID);
            }
         });
    });

    //$(this).find('a[href="@cmd%update"]').unbind('click');
    //$(this).find('a[href="@cmd%update"]').click(function (paEvent) {
    //    paEvent.preventDefault();
    //    var lcControl = $(this).closest('[sa-elementtype=control]');
    //    var lcContainer = lcControl.find('[sa-elementtype=container]');
    //    var lcAmendedRows = lcControl.find('[sa-elementtype=container] [sa-elementtype=element][fa-valuechanged]');

    //    if (lcAmendedRows.length > 0) {
    //        lcContainer.scrollTop(0);
    //        lcControl.attr('ea-controlmode', 'confirmation');

    //    }
    //    else
    //        ShowErrorMessage("Unable to update. No amendment was found.")
    //});

    //$(this).find('a[href="@cmd%back"]').unbind('click');
    //$(this).find('a[href="@cmd%back"]').click(function (paEvent) {
    //    paEvent.preventDefault();
    //    var lcControl = $(this).closest('[sa-elementtype=control]');
    //    var lcContainer = lcControl.find('[sa-elementtype=container]');
    //    lcControl.attr('ea-controlmode', 'standard');
    //    lcControl.find('[sa-elementtype=element][fa-valuechanged=false]').removeAttr('fa-valuechanged');
    //    lcContainer.scrollTop(0);
    //});

    //$(this).find('a[href="@cmd%confirm"]').unbind('click');
    //$(this).find('a[href="@cmd%confirm"]').click(function (paEvent) {
    //    paEvent.preventDefault();

    //    var lcForm = $(this).closest('[sa-elementtype=form]');

    //    lcForm.UpdatePriceList();
    //});
}

$.fn.RefreshActiveGroup = function () {
    var lcContainer = $(this).find('[sa-elementtype=container]');
    var lcActiveOption = $(this).find('[sa-elementtype=filtercontrol] option:selected');
    var lcActiveGroup = lcActiveOption.attr('value');

    lcContainer.find('[fa-show]').removeAttr('fa-show');
    lcContainer.find('[ea-group="' + lcActiveGroup + '"]').attr('fa-show', 'true');
}

$.fn.DeleteInventoryItem = function (paDataID) {
    var lcFieldRow = $(this);
    var lcControl = $(this).closest('[sa-elementtype=control]');
    var lcForm = lcControl.closest('[sa-elementtype=form]');
    var lcDataBlock = JSON.stringify({ EntryID: paDataID });
        
    var lcData = { CobraAjaxRequest: "executescalarquery", Parameter : 'deleteinventoryitem', DataBlock: Base64.encode(lcDataBlock) };    
    GlobalAjaxHandler.SetAjaxLoaderStatusText('Deleting .....');
    
    DoPostBack(lcData, function (paResponseData) {
        var lcRespondStruct = jQuery.parseJSON(paResponseData);
        if (lcRespondStruct.Success) {
            if (Number(lcRespondStruct.ResponseData.RSP_Result) == 1) {
                lcForm.ShowFormMessage(0, 'Successfully Deleted.');
                lcFieldRow.remove();
            }
            else lcForm.ShowFormMessage(1, 'Unable to Delete Data.');
        }
        else {            
            lcForm.ShowFormMessage(1, 'Unable to Delete Data.');
        }
    });
}


//$.fn.GetSerializedData = function () {
//    var lcMainBlock = [];

//    var lcAmendedRows = $(this).find('[sa-elementtype=container] [sa-elementtype=element][fa-valuechanged=true]');
//    var lcKeyColumnName = $(this).find('[sa-elementtype=container]').attr('ea-keycolumnname');
//    alert(lcAmendedRows.length);
//    lcAmendedRows.each(function (paIndex) {
//        var lcFieldBlock = {};
//        var lcFieldRow = $(this);
//        var lcKeyValue = lcFieldRow.attr('ea-keyvalue');

//        lcFieldRow.find('input[type=text][ea-columnname]').each(function () {
//            var lcValue = isNaN($(this).val()) || $(this).val().trim().length == 0 ? 0 : parseInt($(this).val());
//            var lcColumnName = $(this).attr('ea-columnname');
//            lcFieldBlock[lcColumnName] = lcValue;
//        });
//        lcFieldBlock[lcKeyColumnName] = lcKeyValue;
//        lcMainBlock.push(lcFieldBlock);
//    });
//    alert(JSON.stringify(lcMainBlock));
//    return (Base64.encode(JSON.stringify(lcMainBlock)));
//}

//$.fn.UpdatePriceList = function () {
//    var lcForm = $(this);
//    var lcControl = $(this).find('[sa-elementtype=control].WidControlUpdatePriceList');
//    var lcData = { CobraAjaxRequest: "updatedatalist", DataBlock: lcControl.GetSerializedData() };
//    GlobalAjaxHandler.SetAjaxLoaderStatusText('Updating .....');

//    DoPostBack(lcData, function (paResponseData) {
//        var lcRespondStruct = jQuery.parseJSON(paResponseData);
//        if (lcRespondStruct.Success) {
//            ShowInfoMessage('Successfully Updated.').done(function (paResult, paControl) {
//                lcForm.CloseForm();
//            });
//        }
//        else {
//            ShowErrorMessage('Error in Updating Data.');
//        }
//    });
//}
