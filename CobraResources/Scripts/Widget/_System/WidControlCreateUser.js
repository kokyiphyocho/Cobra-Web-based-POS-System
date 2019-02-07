$(document).ready(function () {
    var lcControl           = $('[sa-elementtype=control].WidControlCreateUser');
    var lcSelectionControl  = lcControl.find('[sa-elementtype=slideselectioncontrol][ea-columnname]');

    lcControl.BindCreateUserEvents();
    lcSelectionControl.SetSelectionControlGroup();
});

$.fn.BindCreateUserEvents = function () {

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

    $(this).find('a[href="@cmd%create"]').unbind('click');
    $(this).find('a[href="@cmd%create"]').click(function (paEvent) {        
        paEvent.preventDefault();
        var lcControl = $(this).closest('[sa-elementtype=control]');
        var lcContainer = lcControl.find('[sa-elementtype=container]');
        
        if (lcContainer.VerifyInputs())
        {
            lcContainer.find('[ea-columnname=LogInID]').CheckLogInNameAvailability().done(function (paVerifySuccess) {
                if (paVerifySuccess) {
                    lcControl.attr('ea-controlmode', 'confirmation');
                    lcContainer.find('input[type=password]:not([readonly])').attr('fa-tempreadonly', 'true');
                    lcContainer.find('input[type=password]:not([readonly])').attr('readonly', 'true');

                    lcContainer.find('input[type=text]:not([readonly])').attr('fa-tempreadonly', 'true');
                    lcContainer.find('input[type=text]:not([readonly])').attr('readonly', 'true');
                }
            });
                     
        }
    });

    
    $(this).find('a[href="@cmd%cancel"]').unbind('click');
    $(this).find('a[href="@cmd%cancel"]').click(function (paEvent) {
        paEvent.preventDefault();
        var lcForm = $(this).closest('[sa-elementtype=form]');
        lcForm.CloseForm();
    });

    $(this).find('a[href="@cmd%back"]').unbind('click');
    $(this).find('a[href="@cmd%back"]').click(function (paEvent) {
        paEvent.preventDefault();
        var lcControl = $(this).closest('[sa-elementtype=control]');
        var lcReadonlyControl = lcControl.find('[sa-elementtype=container] [sa-elementtype=inputblock] [sa-elementtype=inputrow] [fa-tempreadonly]');
        lcControl.attr('ea-controlmode', 'standard');
        lcReadonlyControl.removeAttr('readonly');        
    });

    $(this).find('a[href="@cmd%confirm"]').unbind('click');
    $(this).find('a[href="@cmd%confirm"]').click(function (paEvent) {        
        paEvent.preventDefault();
        var lcForm = $(this).closest('[sa-elementtype=form]');
        var lcControl = $(this).closest('[sa-elementtype=control]');
        var lcData = { CobraAjaxRequest: "updatedatarecord", DataBlock: lcControl.GetSerializedData() };
        GlobalAjaxHandler.SetAjaxLoaderStatusText('Updating .....');

        DoPostBack(lcData, function (paResponseData) {
            var lcRespondStruct = jQuery.parseJSON(paResponseData);
            if (lcRespondStruct.Success) {
                ShowInfoMessage('Successfully Created.').done(function (paResult, paControl) {
                    lcForm.CloseForm();
                });
            }
            else {
                ShowErrorMessage('Error in Updating Data.');
            }
        });
    });

    $(this).find('[sa-elementtype=slideselectioncontrol]').unbind('click');
    $(this).find('[sa-elementtype=slideselectioncontrol]').click(function (paEvent) {
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
            if (lcFilter) {
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
        var lcColumnName = lcPopUp.attr('ea-type');
        var lcSelectionControl = lcControl.find('[sa-elementtype=slideselectioncontrol][ea-columnname="' + lcColumnName + '"]');
        var lcInputControl = lcSelectionControl.find('input[type=text]');

        if (lcActiveItem.length == 0) lcActiveItem = lcList.find('a').first();

        if (lcActiveItem.length > 0)
            lcInputControl.val(lcActiveItem.first().attr('value'));
        else
            lcInputControl.val('');

        lcSelectionControl.SetSelectionControlGroup();
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

    var lcItemRecord = lcList.find('[ea-group="' + lcInputControl.val() + '"][value="' + lcChildInputControl.val() + '"]');

    if (lcItemRecord.length == 0) {
        var lcFirstValidControl = lcList.find('[ea-group="' + lcInputControl.val() + '"]').first();
        if (lcFirstValidControl) {
            lcChildInputControl.val(lcFirstValidControl.attr('value'));
        }
        else lcChildInputControl.val('');
    }


}


$.fn.VerifyInputs = function () {
    var lcSuccess = true;
    var lcPinCode = '';

    var lcMandtoryFields = $(this).find('[ea-mandatory=true]');    
    var lcPasswordFields = $(this).find('input[type=password]');
    
    lcMandtoryFields.each(function (paIndex) {

        if ($(this).val().trim() == '') {
            $(this).ShowControlMessage(0, 'Require Field Missing.', true);
            lcSuccess = false;
            return (false);
        }
    });
        
    if (lcSuccess)
    {        
        if ($(lcPasswordFields[0]).val().length < 6)
        {
            $(lcPasswordFields[0]).ShowControlMessage(1, 'PIN Code must be at least 6 digit.', true);
            lcSuccess = false;
        }
        else
        if ($(lcPasswordFields[0]).val() != $(lcPasswordFields[1]).val()) {
            $(lcPasswordFields[1]).ShowControlMessage(1, 'PIN Codes does not match.', true);
            lcSuccess = false;
        }
    }

    return (lcSuccess);
}

$.fn.CheckLogInNameAvailability = function () {
    var lcForm = $(this).closest('[sa-elementtype=form]');
    var lcControl = $(this).closest('[sa-elementtype=control]');
    var lcLogInIDControl = $(this);
    var lcQueryName = lcLogInIDControl.attr('ea-queryname');
    var lcDeferred = $.Deferred();

    var lcData = { CobraAjaxRequest: "executescalarquery", Parameter : lcQueryName, DataBlock: lcControl.GetSerializedData() };
    GlobalAjaxHandler.SetAjaxLoaderStatusText('Checking Login Name .....');

    DoPostBack(lcData, function (paResponseData) {
        var lcRespondStruct = jQuery.parseJSON(paResponseData);
        if (lcRespondStruct.Success) {
            var lcCount = Number(lcRespondStruct.ResponseData.RSP_Result);
            if (lcCount != 0)
            {
                ShowErrorMessage('Mobile Phone No. is already in use.');
                lcDeferred.resolve(false);
            }            
            lcDeferred.resolve(true);
        }
        else {
            ShowErrorMessage('Request Fail. Please try again.');
            lcDeferred.resolve(false);
        }
    });

    return (lcDeferred);

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
        if ($(this).attr('type') == 'password') {
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
