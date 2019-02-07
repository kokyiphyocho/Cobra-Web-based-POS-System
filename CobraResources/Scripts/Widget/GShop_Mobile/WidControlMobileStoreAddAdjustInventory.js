$(document).ready(function () {
    $('[sa-elementtype=control].WidControlMobileStoreAddAdjustInventory').BindMobileStoreAddAdjustInventoryEvents();
    $('[sa-elementtype=control].WidControlMobileStoreAddAdjustInventory').RefreshActiveGroup();
});

$.fn.BindMobileStoreAddAdjustInventoryEvents = function () {
       
    $(this).find('input[ea-inputmode=number]').ForceIntegerInput();

    $(this).find('.SelectionControl').unbind('click');
    $(this).find('.SelectionControl').click(function (paEvent) {
        paEvent.preventDefault();
        var lcControl = $(this).closest('[sa-elementtype=control]');
        var lcPanelType = $(this).attr('ea-type');
        var lcInputBox = $(this).find('input[type=text]');
        var lcPanel = lcControl.find('[sa-elementtype=popup] [sa-elementtype=overlay] [sa-elementtype=panel][ea-type=' + lcPanelType + ']');
                
        lcPanel.find('[sa-elementtype=list] a').removeAttr('fa-selected');
        lcPanel.find('[sa-elementtype=list] a[value="' + $(this).attr('value') + '"]').attr('fa-selected','true');
        
        lcControl.attr('fa-showpanel', lcPanelType);
    });

    $(this).find('[sa-elementtype=popup] [sa-elementtype=overlay] [sa-elementtype=panel]').unbind('change');
    $(this).find('[sa-elementtype=popup] [sa-elementtype=overlay] [sa-elementtype=panel]').on('change', function () {
        var lcControl           = $(this).closest('[sa-elementtype=control]');
        var lcPanelType         = $(this).attr('ea-type');
        var lcSelectionControl  = lcControl.find('.SelectionControl[ea-type=' + lcPanelType + ']');        
        var lcSelectedItem      = $(this).find('[sa-elementtype=list] a[fa-selected]')
      
        lcSelectionControl.SetSelectedItem(lcSelectedItem);        

        lcControl.RefreshActiveGroup();
    });

    $(this).find('input[type=text][ea-columnname="description.description1"]').unbind('click');
    $(this).find('input[type=text][ea-columnname="description.description1"]').focus(function (paEvent) {
        var lcControl = $(this).closest('[sa-elementtype=control]');
        var lcContainer = lcControl.find('[sa-elementtype=container]');

        var lcProductNameSelectionControl = lcContainer.find('.SelectionControl[ea-type=productname]');
        var lcProductNameInputBox = lcProductNameSelectionControl.find('input[type=text]');

        if (($(this).val().trim().length == 0) && (lcProductNameInputBox.val().toLowerCase() != 'other'))
            $(this).val(lcProductNameInputBox.val());

    });


    $(this).find('a[href="@cmd%update"]').unbind('click');
    $(this).find('a[href="@cmd%update"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcControl = $(this).closest('[sa-elementtype=control]');        
        
        if (lcControl.VerifyInputs())
        {
            lcControl.UpdateData();
        }
        
    });

    $(this).find('a[href="@cmd%close"]').unbind('click');
    $(this).find('a[href="@cmd%close"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcForm = $(this).closest('[sa-elementtype=form]');        
        lcForm.CloseForm();
    });
}

$.fn.SetSelectedItem = function(paSelectedItem)
{
    var lcSelectionControl = $(this);
    var lcInputRow         = lcSelectionControl.closest('[sa-elementtype=inputrow]');
    var lcInputBox         = lcSelectionControl.find('input[type=text]');

    if (paSelectedItem) {
        lcSelectionControl.attr('value', paSelectedItem.attr('value'));
        lcInputBox.val(paSelectedItem.text());
    }
}

$.fn.RefreshActiveGroup = function () {
    var lcControl = $(this);    
    var lcCategoryCombo = lcControl.find('.SelectionControl[ea-type=category]');
    var lcManufacturerCombo = lcControl.find('.SelectionControl[ea-type=manufacturer]');
    var lcProductNameCombo = lcControl.find('.SelectionControl[ea-type=productname]');
    var lcGroupName = lcCategoryCombo.find('input[type=text]').val() + "," + lcManufacturerCombo.find('input[type=text]').val();
    var lcProductNamePanel = lcControl.find('[sa-elementtype=popup] [sa-elementtype=overlay] [sa-elementtype=panel][ea-type=productname]');
    var lcProductNameList = lcProductNamePanel.find('[sa-elementtype=list]');

    lcProductNameList.find('a').attr('fa-hide','true');
    lcProductNameList.find('a[value^="' + lcGroupName + '"],a[value^="[OTHER]"]').removeAttr('fa-hide');    
    if (lcProductNameList.find('a[value="' + lcProductNameCombo.attr('value') + '"]').attr('fa-hide') == 'true')
    {        
        lcProductNameList.removeAttr('fa-selected');
        lcProductNameCombo.SetSelectedItem(lcProductNameList.find('a[value="[OTHER]"]'));      
    }
    
    lcControl.attr('fa-activegroup', lcGroupName);
};

$.fn.VerifyInputs = function () {
    var lcForm = $(this).closest('[fa-elementtype=form]');

    var lcSuccess = true;

    var lcMandtoryFields = $(this).find('[ea-mandatory=true]');
    
    lcMandtoryFields.each(function (paIndex) {
        if ($(this).val().trim() == '') {
            var lcControl = $(this);

            lcForm.ShowFormMessage(2, "Require Field is Missing", true).done(function () {
                lcControl.focus();
            });

            lcSuccess = false;
            return (false);
        }
    });

    return (lcSuccess);
}

$.fn.CompileDefaultSortKey = function()
{
    var lcControl = $(this);

    var lcTypeSelectionControl = lcControl.find('.SelectionControl[ea-type=category]');
    var lcManufacturerSelectionControl = lcControl.find('.SelectionControl[ea-type=manufacturer]');
    var lcProductNameSelectionControl = lcControl.find('.SelectionControl[ea-type=productname]');
    var lcProductNameInputBox = lcProductNameSelectionControl.find('input[type=text]');

    return (lcManufacturerSelectionControl.attr('value') + "_" + lcTypeSelectionControl.attr('value') + "_" + lcProductNameInputBox.val());
}

$.fn.GetSerializedData = function () {

    var lcDataBlock     =  {};
    var lcControl       = $(this);

    var lcTypeSelectionControl = lcControl.find('.SelectionControl[ea-type=category]');
    var lcManufacturerSelectionControl = lcControl.find('.SelectionControl[ea-type=manufacturer]');
    var lcProductNameSelectionControl = lcControl.find('.SelectionControl[ea-type=productname]');
    var lcNetworkTypeSelectionControl = lcControl.find('.SelectionControl[ea-type=networktype]');

    var lcDescriptionLine1 = lcControl.find('[sa-elementtype=inputrow] .InputDiv input[type=text][ea-columnname="description.description1"]');
    var lcDescriptionLine2 = lcControl.find('[sa-elementtype=inputrow] .InputDiv input[type=text][ea-columnname="description.description2"]');
    var lcPrice            = lcControl.find('[sa-elementtype=inputrow] .InputDiv input[type=text][ea-columnname=price]');
    var lcGroupName        = lcControl.find('[sa-elementtype=inputrow] .InputDiv input[type=text][ea-columnname=groupname]');
    var lcSortKey          = lcControl.find('[sa-elementtype=inputrow] .InputDiv input[type=text][ea-columnname=sortkey]');
    var lcTag              = lcControl.find('[sa-elementtype=inputrow] .InputDiv input[type=text][ea-columnname=tag]');

    lcDataBlock["EntryID"] = lcControl.attr('ea-dataid');
    lcDataBlock["Description"] = "<!Description1::" + lcDescriptionLine1.val() + ";;Description2::" + lcDescriptionLine2.val() + ";;!>";    
    lcDataBlock["Price"] = isNaN(lcPrice.val()) || lcPrice.val().trim().length == 0 ? 0 : parseInt(lcPrice.val());
    lcDataBlock["GroupName"] = lcGroupName.val().length == 0 ? lcManufacturerSelectionControl.attr('value').toUpperCase() : lcGroupName.val().toUpperCase();
    lcDataBlock["SortKey"] = lcSortKey.val().length == 0 ? lcControl.CompileDefaultSortKey().toUpperCase() : lcSortKey.val().toUpperCase();
    lcDataBlock["Tag"] = lcTag.val().toUpperCase();
    lcDataBlock["Category"] = lcTypeSelectionControl.attr('value').toUpperCase();
    lcDataBlock["Manufacturer"] = lcManufacturerSelectionControl.attr('value').toUpperCase();

    if (lcProductNameSelectionControl.attr('value') != '[OTHER]') lcDataBlock["ProductUID"] = lcProductNameSelectionControl.attr('value').split(';')[1];
    else lcDataBlock["ProductUID"] = lcProductNameSelectionControl.attr('value');

    lcDataBlock["Kind"] = lcNetworkTypeSelectionControl.attr('value').toUpperCase();
    
    return (Base64.encode(JSON.stringify(lcDataBlock)));
}

$.fn.UpdateData = function () {    
    var lcControl = $(this);
    var lcForm = $(this).closest('[sa-elementtype=form]');
    var lcEntryID =  parseInt(lcControl.attr('ea-dataid'));
    var lcQuery = lcEntryID == -1 ? 'insertnewproduct' : 'updateproductinfo';
    
    var lcData = { CobraAjaxRequest: "executescalarquery", Parameter : lcQuery, DataBlock: lcControl.GetSerializedData() };
    GlobalAjaxHandler.SetAjaxLoaderStatusText('Updating .....');

    DoPostBack(lcData, function (paResponseData) {
        var lcRespondStruct = jQuery.parseJSON(paResponseData);
        if (lcRespondStruct.Success) {
            lcForm.ShowFormMessage(0, "Successfully Updated.", true).done(function () {
                
                if (lcEntryID == -1) {
                    lcControl.find('[sa-elementtype=inputrow] .InputDiv input[type=text]').val('');
                    lcControl.find('[sa-elementtype=inputrow] .InputDiv input[type=text]').first().focus();
                }
                else {                    
                    lcForm.CloseForm();
                }
                
            });
        }
        else
        {
            lcForm.ShowFormMessage(1, "Unable to Update Information.", true);
        }
    });
}
