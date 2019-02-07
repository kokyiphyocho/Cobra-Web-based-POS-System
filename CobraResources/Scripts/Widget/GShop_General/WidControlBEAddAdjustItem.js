$(document).ready(function () {
    $('[sa-elementtype=control].WidControlBEAddAdjustItem').BindBEAddAdjustItemEvents();
    $('[sa-elementtype=form]').ReplaceMessagePlaceHolders();
});

$.fn.BindBEAddAdjustItemEvents = function () {

    var lcInputBlock = $(this).find('[sa-elementtype=container] [sa-elementtype=inputblock]');
    var lcNumberBoxes = lcInputBlock.find('input[type=text][ea-inputmode=number]');

    lcNumberBoxes.ForceIntegerInput();
        
    $(this).find('input[type=text][ea-mirrorcolumn]').each(function () {
        var lcControl = $(this).closest('[sa-elementtype=control]');
        var lcContainer = lcControl.find('[sa-elementtype=container]');
        var lcMirrorColumnName = $(this).attr('ea-mirrorcolumn');
        var lcControl = $(this);

        lcContainer.find('[ea-columnname=' + lcMirrorColumnName + ']').unbind('change');
        lcContainer.find('[ea-columnname=' + lcMirrorColumnName + ']').change(function (paEvent) {
            lcControl.val($(this).val());
        });

    });   

    $(this).find('a[href="@cmd%update"]').unbind('click');
    $(this).find('a[href="@cmd%update"]').click(function (paEvent) {
        paEvent.preventDefault();
        var lcForm = $(this).closest('[sa-elementtype=form]');
        var lcControl = $(this).closest('[sa-elementtype=control]');
        var lcContainer = lcControl.find('[sa-elementtype=container]');

        if (lcContainer.VerifyInputs()) {
            var lcData = { CobraAjaxRequest: "updatedatarecord", DataBlock: lcControl.GetSerializedData() };
            GlobalAjaxHandler.SetAjaxLoaderStatusText('Updating .....');

            DoPostBack(lcData, function (paResponseData) {
                var lcRespondStruct = jQuery.parseJSON(paResponseData);
                if (lcRespondStruct.Success) {                    
                    lcForm.ShowFormMessage(0, 'Successfully Updated',false).done(function (paResult, paControl) {
                            lcForm.CloseForm();
                    });                    
                }
                else {                    
                    lcForm.ShowFormMessage(1, 'Error in Updating Data.',true);
                }
            });
        }
    });

    $(this).find('a[href="@cmd%close"]').unbind('click');
    $(this).find('a[href="@cmd%close"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcForm = $(this).closest('[sa-elementtype=form]');

        lcForm.CloseForm();
    });    
}

$.fn.VerifyInputs = function () {
    var lcSuccess = true;    

    var lcMandtoryFields = $(this).find('[ea-mandatory=true]');
    

    lcMandtoryFields.each(function (paIndex) {

        if ($(this).val().trim() == '') {
            $(this).ShowControlMessage(0, 'Require Field Missing.', true);
            lcSuccess = false;
            return (false);
        }
    });    

    return (lcSuccess);
}

$.fn.GetSerializedData = function () {
    var lcDataBlock = {};
    var lcDataColumnList = [];
    var lcKeyColumnList = [];
    var lcIdentifierColumnList = [];

    var lcDataControls = $(this).find('[ea-columnname]');
    var lcKeyControls = $(this).find('[ea-columnname][ea-keyfield]');
    var lcIdentifierControls = $(this).find('[ea-columnname][ea-identifiercolumn]');

    lcKeyControls.each(function (paIndex) {
        lcKeyColumnList.push($(this).attr('ea-columnname'));
    });

    lcIdentifierControls.each(function (paIndex) {
        lcIdentifierColumnList.push($(this).attr('ea-columnname'));
    });

    lcDataControls.each(function (paIndex) {

        if ($(this).attr('ea-inputmode') == 'number') lcDataBlock[$(this).attr('ea-columnname')] = $(this).val().NormalizeNumber().trim();
        else lcDataBlock[$(this).attr('ea-columnname')] = $(this).val().trim();
        
        if ($(this).attr('ea-identifiercolumn') != 'true')
            lcDataColumnList.push($(this).attr('ea-columnname'));
        
    });

    lcDataBlock['#KEYCOLUMNLIST'] = lcKeyColumnList.join(',');
    lcDataBlock['#DATACOLUMNLIST'] = lcDataColumnList.join(',');
    lcDataBlock['#IDENTIFIERCOLUMNLIST'] = lcIdentifierColumnList.join(',');
    
    return (Base64.encode(JSON.stringify(lcDataBlock)));
}

$.fn.RestoreOriginalValue = function () {
    var lcColumns = $(this).find('[ea-columnname]');
    
    lcColumns.each(function (paIndex) {
        var lcOriginalValue = $(this).attr('ea-originalvalue');
        $(this).val(lcOriginalValue);
    });
}

$.fn.SetColumnValue= function (paColumnName, paValue) {
    var lcColumnControl = $(this).find('[ea-columnname=' + paColumnName + ']');

    if (lcColumnControl)
    {
        lcColumnControl.val(paValue);
        
        return (true);
    }
    
    return (false);    
}

$.fn.ReplaceMessagePlaceHolders = function () {
    var lcColumns = $(this).find('[ea-columnname]');
    var lcMessages = $(this).attr('ea-messages');
    
    if (lcMessages)
    {
        lcColumns.each(function (paIndex) {
            var lcValue = $(this).attr('ea-originalvalue');
            var lcColumnName = $(this).attr('ea-columnname');
            var lcPlaceHolder = "$" + lcColumnName.toUpperCase();
            
            lcMessages = lcMessages.replace(lcPlaceHolder, lcValue);
        });
        
        $(this).attr('ea-messages', lcMessages);
    }
}
