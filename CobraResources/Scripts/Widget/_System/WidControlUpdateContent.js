$(document).ready(function () {
    var lcControl = $('[sa-elementtype=control].WidControlUpdateContent');
    var lcSelectionControl = lcControl.find('[sa-elementtype=slideselectioncontrol][ea-columnname]');

    lcControl.BindUpdateContentEvents();
    lcSelectionControl.SetSelectionControlGroup();
});

$.fn.BindUpdateContentEvents = function () {
    var lcForm = $(this).closest('[sa-elementtype=form]');
    var lcInputBlock =  $(this).find('[sa-elementtype=container] [sa-elementtype=inputblock]');
    var lcDecimalInputBoxes = lcInputBlock.find('input[type=text][ea-inputmode=decimal]');
    
    lcDecimalInputBoxes.ForceDecimalInput();
    
    lcForm.find('a[href="@cmd%geolocation"]').unbind('click');
    lcForm.find('a[href="@cmd%geolocation"]').click(function (paEvent) {
        paEvent.preventDefault();
        var lcForm             = $(this).closest('[sa-elementtype=form]');
        var lcControl          = lcForm.find('[sa-elementtype=control].WidControlUpdateContent');
        var lcLatitudeControl  = lcControl.find('input[type=text][ea-columnname=Latitude]');
        var lcLongitudeControl = lcControl.find('input[type=text][ea-columnname=Longitude]');        
        lcForm.GeoLocationReader(lcLatitudeControl, lcLongitudeControl);
    });

    $(this).find('a[href="@cmd%cancel"]').unbind('click');
    $(this).find('a[href="@cmd%cancel"]').click(function (paEvent) {
        paEvent.preventDefault();
        var lcForm = $(this).closest('[sa-elementtype=form]');

        lcForm.CloseForm();
    });

    $(this).find('[sa-elementtype=slideselectioncontrol],[sa-elementtype=colorslideselectioncontrol],[sa-elementtype=imageslideselectioncontrol]').unbind('click');
    $(this).find('[sa-elementtype=slideselectioncontrol],[sa-elementtype=colorslideselectioncontrol],[sa-elementtype=imageslideselectioncontrol]').click(function (paEvent)
    {        
        var lcControl = $(this).closest('[sa-elementtype=control]');
        var lcColumnName = $(this).attr('ea-columnname');
        var lcPopUp = lcControl.find('[sa-elementtype=popup][ea-type="' + lcColumnName + '"]');        
        var lcLinkColumn = lcPopUp.attr('ea-linkcolumn');
        var lcInputControl = $(this).find('input[type=text]');        
        var lcList = lcPopUp.find('[sa-elementtype=panel] [sa-elementtype=list]');
        var lcListItems = lcList.find('a');
        var lcActiveControl = lcList.find('a[value="' + lcInputControl.val().trim() + '"]');
        
        if (lcLinkColumn) {            
            var lcFilter = lcControl.attr('da-' + lcLinkColumn);
            if (lcFilter)
            {
                lcListItems.removeAttr('fa-hide');
                lcListItems.not('[ea-group="' + lcFilter + '"]').attr('fa-hide', 'true');
            }
        }        
        
        lcListItems.removeAttr('fa-selected');
        lcActiveControl.attr('fa-selected', 'true');

        lcControl.attr('fa-showpanel', lcColumnName);
    });

    $(this).find('[sa-elementtype=popup] [sa-elementtype=overlay] [sa-elementtype=panel]').unbind('change');
    $(this).find('[sa-elementtype=popup] [sa-elementtype=overlay] [sa-elementtype=panel]').change(function () {
        var lcControl = $(this).closest('[sa-elementtype=control]');
        var lcPopUp = $(this).closest('[sa-elementtype=popup]');        
        var lcList = lcPopUp.find('[sa-elementtype=panel] [sa-elementtype=list]');
        var lcActiveItem = lcList.find('a[fa-selected]');
        var lcColumnName        = lcPopUp.attr('ea-type');        
        
        var lcMode = 'Default';
        var lcSelectionControl = lcControl.find('[sa-elementtype=slideselectioncontrol][ea-columnname="' + lcColumnName + '"]');

        if (lcSelectionControl.length == 0)
        {
            lcMode = 'Color';
            lcSelectionControl = lcControl.find('[sa-elementtype=colorslideselectioncontrol][ea-columnname="' + lcColumnName + '"]');

            if (lcSelectionControl.length == 0)
            {
                lcMode = 'Image';
                lcSelectionControl = lcControl.find('[sa-elementtype=imageslideselectioncontrol][ea-columnname="' + lcColumnName + '"]');
            }
        }        

        var lcInputControl = lcSelectionControl.find('input[type=text]');
        var lcImageControl = lcSelectionControl.find('img');
                
        
        if (lcActiveItem.length == 0) lcActiveItem = lcList.find('a').first();
                

        if (lcActiveItem.length > 0) {
            if (lcImageControl.length > 0) {
                lcImageControl.attr('src', lcActiveItem.first().attr('value'));
            }

            if (lcSelectionControl.attr('sa-elementtype') == 'colorslideselectioncontrol')                 
                lcInputControl.attr("style", "background:" + lcActiveItem.first().attr('value') + ";color:" + InvertColor(lcActiveItem.first().attr('value'),true));

            lcInputControl.val(lcActiveItem.first().attr('value'));
        }
        else {
            if (lcSelectionControl.length == 0) lcInputControl.removeAttr('style');
            lcInputControl.val('');
        }

        lcSelectionControl.SetSelectionControlGroup();
    });    

    $(this).find('a[href="@cmd%update"]').unbind('click');
    $(this).find('a[href="@cmd%update"]').click(function (paEvent) {        
        paEvent.preventDefault();
        
        var lcForm = $(this).closest('[sa-elementtype=form]');
        var lcControl = $(this).closest('[sa-elementtype=control]');
        var lcData = { CobraAjaxRequest: "updatedatarecord", DataBlock: lcControl.GetSerializedData() };

        if (lcControl.VerifyInputs()) {
            GlobalAjaxHandler.SetAjaxLoaderStatusText('Updating .....');

            DoPostBack(lcData, function (paResponseData) {
                var lcRespondStruct = jQuery.parseJSON(paResponseData);
                if (lcRespondStruct.Success) {
                    lcForm.ShowFormMessage(0, 'Successfully Updated.', false).done(function () {
                        lcForm.CloseForm();
                    });
                }
                else {
                    lcForm.ShowFormMessage(1, 'Unable to update data.', true);
                }
            });
        }
    });
}

$.fn.SetSelectionControlGroup = function () {

    var lcSelectionControl = $(this);    
    var lcControl = lcSelectionControl.closest('[sa-elementtype=control]');
    var lcInputControl = lcSelectionControl.find('input[type=text]');

    lcControl.attr('da-' + lcSelectionControl.attr('ea-columnname'), lcInputControl.val());    
    lcSelectionControl.ValidateChildSelectionControlGroup();
}

$.fn.ValidateChildSelectionControlGroup = function () {

    var lcSelectionControl = $(this);
    var lcControl = lcSelectionControl.closest('[sa-elementtype=control]');
    var lcColumnName = lcSelectionControl.attr('ea-columnname');
    var lcChildSelectionControl = lcControl.find('[sa-elementtype=slideselectioncontrol][ea-linkcolumn="' + lcColumnName + '"]');
    var lcPopUp = lcControl.find('[sa-elementtype=popup][ea-type="' + lcChildSelectionControl.attr('ea-columnname') + '"]');
    var lcList = lcPopUp.find('[sa-elementtype=panel] [sa-elementtype=list]');
    var lcChildInputControl = lcChildSelectionControl.find('input[type=text]');
    var lcInputControl = lcSelectionControl.find('input[type=text]');

    var lcItemRecord = lcList.find('[ea-group="' + lcInputControl.val() +'"][value="' + lcChildInputControl.val() + '"]');

    if (lcItemRecord.length == 0)
    {
        var lcFirstValidControl = lcList.find('[ea-group="' + lcInputControl.val() + '"]').first();
        if (lcFirstValidControl) {
            lcChildInputControl.val(lcFirstValidControl.attr('value'));
        }
        else lcChildInputControl.val('');
    }
        

}

$.fn.VerifyInputs = function () {
    var lcSuccess = true;
    var lcMandtoryFields = $(this).find('[ea-mandatory=true]');    
    var lcPasswordFields = $(this).find('input[type=password]');

    lcMandtoryFields.each(function (paIndex) {

        if ($(this).val().trim() == '') {
            $(this).ShowControlMessage(0, 'Require Field Missing.', true);
            lcSuccess = false;
            return (false);
        }
    });    

    if ((lcSuccess) && (lcPasswordFields.length == 2))
    {
        var lcOldPassword = Base64.decode(lcPasswordFields.eq(0).attr('ea-originalvalue'));
        var lcNewPassword = $.md5(lcPasswordFields.eq(0).val());
        
        if (lcNewPassword == lcOldPassword)
        {
            lcPasswordFields.eq(0).ShowControlMessage(2, 'New PIN Cannot same as Old PIN Code.', true);
            lcSuccess = false;
        }
        else
        if ((lcPasswordFields.eq(0).val().length < 6))
        {
            lcPasswordFields.eq(0).ShowControlMessage(1, 'PIN Code must have at least 6 digits', true);
            lcSuccess = false;
        }
        else if (lcPasswordFields.eq(0).val() != lcPasswordFields.eq(1).val())
        {
            lcPasswordFields.eq(1).ShowControlMessage(1, 'PIN Codes does not match.', true);
            lcSuccess = false;
        }
    }

    return (lcSuccess);
}

$.fn.GetDelimitedValue = function()
{
    var lcOutputString = '';
    var lcStringArray = $(this).val().split('\n');

    $.each(lcStringArray, function () {
        lcOutputString += $.trim(this) + ';;';
    });

    return lcOutputString.substring(0,lcOutputString.length - 2);
}

$.fn.GetSerializedData = function () {
    var lcDataBlock = {};
    var lcDataColumnList = [];
    var lcKeyColumnList = [];
    var lcIdentifierColumnList = [];
    
    var lcDataControls = $(this).find('[ea-columnname]').not('div');
    var lcKeyControls = $(this).find('[ea-columnname][ea-keyfield]');
    var lcIdentifierControls = $(this).find('[ea-columnname][ea-identifiercolumn]');

    lcKeyControls.each(function (paIndex) {
        lcKeyColumnList.push($(this).attr('ea-columnname'));
    });

    lcIdentifierControls.each(function (paIndex) {
        lcIdentifierColumnList.push($(this).attr('ea-columnname'));
    });

    lcDataControls.each(function (paIndex) {
        if ($(this).attr('type') == 'password')
        {
            lcDataBlock[$(this).attr('ea-columnname')] = $.md5($(this).val().NormalizeNumber().trim());
        }
        else if ($(this).attr('ea-mode') == 'delimited') 
            lcDataBlock[$(this).attr('ea-columnname')] = $(this).GetDelimitedValue();
        else if ($(this).attr('ea-inputmode') == 'number')
            lcDataBlock[$(this).attr('ea-columnname')] = $(this).val().NormalizeNumber().trim();            
        else
            lcDataBlock[$(this).attr('ea-columnname')] = $(this).val().trim();
        
        lcDataColumnList.push($(this).attr('ea-columnname'));
    });
        
    lcDataBlock['#KEYCOLUMNLIST'] = lcKeyColumnList.join(',');
    lcDataBlock['#DATACOLUMNLIST'] = lcDataColumnList.join(',');
    lcDataBlock['#IDENTIFIERCOLUMNLIST'] = lcIdentifierColumnList.join(',');

    return (Base64.encode(JSON.stringify(lcDataBlock)));
}
