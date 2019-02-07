$(document).ready(function () {    
    $('[sa-elementtype=control].WidControlUpdatePriceList').BindUpdatePriceListEvents();
    $('[sa-elementtype=control].WidControlUpdatePriceList').RefreshActiveGroup();
});

$.fn.BindUpdatePriceListEvents = function () {
        
    var lcTextBoxes = $(this).find('[sa-elementtype=container] [sa-elementtype=element] input[type=text]');    
    var lcFilterControl = $(this).find('[sa-elementtype=filtercontrol]');

    lcTextBoxes.ForceIntegerInput();
    // No need to Unbind Blur as ForceIntegerInput already unbind.
    lcTextBoxes.blur(function (paEvent) {
        var lcNewValue = Number($(this).val());
        var lcOriginalValue = Number($(this).attr('ea-originalvalue'));
        var lcFieldRow = $(this).closest('[sa-elementtype=element]');

        lcFieldRow.RefreshValueChangeStatus();        

        lcFieldRow.removeAttr('fa-focus');
    });

    lcTextBoxes.focus(function (paEvent) {        
        var lcFieldRow = $(this).closest('[sa-elementtype=element]');
        var lcControl = lcFieldRow.closest('[sa-elementtype=control]');
        var lcContainer = lcFieldRow.closest('[sa-elementtype=container]');
        var lcFieldRowList = lcContainer.find('[sa-elementtype=element]');
               
        $(this).select();
        lcFieldRowList.removeAttr('fa-focus');
        lcFieldRow.attr('fa-focus', 'true');
        lcFieldRowList.not('[fa-focus]').removeAttr('fa-showusualprice');
    });
    
    lcFilterControl.change(function (paEvent) {        
        $('[sa-elementtype=control].WidControlUpdatePriceList').RefreshActiveGroup();
    });
    
    $(this).find('img[href="@cmd%showusualprice"]').unbind('click');
    $(this).find('img[href="@cmd%showusualprice"]').click(function (paEvent) {
        paEvent.preventDefault();
        var lcFieldRow = $(this).closest('[sa-elementtype=element]');
        var lcControl = lcFieldRow.closest('[sa-elementtype=control]');
        var lcContainer = lcFieldRow.closest('[sa-elementtype=container]');
        var lcFieldRowList = lcContainer.find('[sa-elementtype=element]');
        var lcUsualPriceInput = lcFieldRow.find('input[type=text][ea-columnname=usualprice]');
        
        lcFieldRowList.removeAttr('fa-showusualprice');
        lcFieldRow.attr('fa-showusualprice', 'true');
        lcUsualPriceInput.focus();
    });

    $(this).find('img[href="@cmd%hideusualprice"]').unbind('click');
    $(this).find('img[href="@cmd%hideusualprice"]').click(function (paEvent) {
        paEvent.preventDefault();        
        var lcFieldRow = $(this).closest('[sa-elementtype=element]');
        var lcControl = lcFieldRow.closest('[sa-elementtype=control]');
        var lcContainer = lcFieldRow.closest('[sa-elementtype=container]');        
        var lcFieldRowList = lcContainer.find('[sa-elementtype=element]');
                
        lcFieldRowList.removeAttr('fa-showusualprice');
    });

    $(this).find('a[href="@cmd%update"]').unbind('click');
    $(this).find('a[href="@cmd%update"]').click(function (paEvent) {
        paEvent.preventDefault();
        var lcControl = $(this).closest('[sa-elementtype=control]');
        var lcContainer = lcControl.find('[sa-elementtype=container]');
        var lcAmendedRows = lcControl.find('[sa-elementtype=container] [sa-elementtype=element][fa-valuechanged]');

        if (lcAmendedRows.length > 0)
        {
            lcContainer.scrollTop(0);
            lcControl.attr('ea-controlmode', 'confirmation');

        }
        else
            ShowErrorMessage("Unable to update. No amendment was found.")
    });

    $(this).find('a[href="@cmd%back"]').unbind('click');
    $(this).find('a[href="@cmd%back"]').click(function (paEvent) {
        paEvent.preventDefault();
        var lcControl = $(this).closest('[sa-elementtype=control]');
        var lcContainer = lcControl.find('[sa-elementtype=container]');
        lcControl.attr('ea-controlmode', 'standard');        
        lcControl.find('[sa-elementtype=element][fa-valuechanged=false]').removeAttr('fa-valuechanged');
        lcContainer.scrollTop(0);
    });

    $(this).find('a[href="@cmd%confirm"]').unbind('click');
    $(this).find('a[href="@cmd%confirm"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcForm = $(this).closest('[sa-elementtype=form]');

        lcForm.UpdatePriceList();
    });
}

$.fn.RefreshValueChangeStatus = function()
{
    var lcFieldRow = $(this);
    var lcControl = lcFieldRow.closest('[sa-elementtype=control]');
    var lcValueChanged = false;

    lcFieldRow.find('input[type=text]').each(function () {
        var lcNewValue = isNaN($(this).val()) || $(this).val().trim().length == 0 ? 0 : parseInt($(this).val());
        var lcOriginalValue = isNaN($(this).attr('ea-originalvalue')) ? 0 : parseInt($(this).attr('ea-originalvalue'));
                
        if (lcNewValue != lcOriginalValue) lcValueChanged = true;
    });

    if (lcValueChanged) lcFieldRow.attr('fa-valuechanged', 'true');
    else {
        if (lcControl.attr('ea-controlmode') == 'confirmation')
            lcFieldRow.attr('fa-valuechanged', 'false');
        else                    
            lcFieldRow.removeAttr('fa-valuechanged');
    }
}

$.fn.RefreshActiveGroup = function ()
{
    var lcContainer = $(this).find('[sa-elementtype=container]');
    var lcActiveOption = $(this).find('[sa-elementtype=filtercontrol] option:selected');    
    var lcActiveGroup = lcActiveOption.attr('value');

    lcContainer.find('[fa-show]').removeAttr('fa-show');
    lcContainer.find('[ea-group="' + lcActiveGroup + '"]').attr('fa-show','true');
}

$.fn.GetSerializedData = function()
{
    var lcMainBlock = [];
    
    var lcAmendedRows = $(this).find('[sa-elementtype=container] [sa-elementtype=element][fa-valuechanged=true]');
    var lcKeyColumnName = $(this).find('[sa-elementtype=container]').attr('ea-keycolumnname');
    
    lcAmendedRows.each(function (paIndex) {
        var lcFieldBlock = {};
        var lcFieldRow = $(this);        
        var lcKeyValue = lcFieldRow.attr('ea-keyvalue');        

        lcFieldRow.find('input[type=text][ea-columnname]').each(function () {
            var lcValue = isNaN($(this).val()) || $(this).val().trim().length == 0 ? 0 : parseInt($(this).val());
            var lcColumnName = $(this).attr('ea-columnname');
            lcFieldBlock[lcColumnName] = lcValue;
        });
        lcFieldBlock[lcKeyColumnName] = lcKeyValue;        
        lcMainBlock.push(lcFieldBlock);
    });
    
    return (Base64.encode(JSON.stringify(lcMainBlock)));    
}

$.fn.UpdatePriceList = function()
{
    var lcForm = $(this);
    var lcControl = $(this).find('[sa-elementtype=control].WidControlUpdatePriceList');
    var lcData = { CobraAjaxRequest: "updatedatalist", DataBlock: lcControl.GetSerializedData() };
    GlobalAjaxHandler.SetAjaxLoaderStatusText('Updating .....');
    
    DoPostBack(lcData, function (paResponseData) {
        var lcRespondStruct = jQuery.parseJSON(paResponseData);
        if (lcRespondStruct.Success) {
            lcForm.ShowFormMessage(0, "Successfully Updated").done(function (paResult, paControl) {
                lcForm.CloseForm();
            });
        }
        else {
            lcForm.ShowFormMessage(1, "Error in Updating Data.");
        }
    });
}
