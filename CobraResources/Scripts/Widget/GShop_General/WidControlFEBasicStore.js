$(document).ready(function () {
    $('[sa-elementtype=control].WidControlFEBasicStore').HideEmptyControl();
    $('[sa-elementtype=control].WidControlFEBasicStore').BindFEBasicStoreEvents();    
});

$.fn.HideEmptyControl = function ()
{
    var lcControl = $(this);    
    var lcCategoryItems = $(this).find('[sa-elementtype=container][ea-type=category] [sa-elementtype=element]');
    
    lcCategoryItems.each(function(){
        if (lcControl.find('[sa-elementtype=container][ea-type=item] [sa-elementtype=element][ea-group=' + $(this).attr('ea-dataid') + ']').length == 0)
            $(this).hide();        
    });
    
}

$.fn.BindFEBasicStoreEvents = function () {
    lcForm = $(this).closest('[sa-elementtype=form]');

    $(this).find('input[ea-inputmode=number]').ForceIntegerInput();

    $(this).find('.ItemButtonPanel input[type=text]').each(function () {
        var lcButtonPanel = $(this).parent();
        var lcQuantityBox = $(this);
        
        var lcQuantity = isNaN(lcQuantityBox.val()) ? 0 : parseInt(lcQuantityBox.val());

        if (lcQuantity <= 0) lcButtonPanel.removeAttr('fa_hasquantity');
        else lcButtonPanel.attr('fa_hasquantity', 'true');        
    });

    $(this).find('img[href="@cmd%addquantity"]').unbind('click');
    $(this).find('img[href="@cmd%addquantity"]').click(function (paEvent) {
        paEvent.preventDefault();
        
        var lcButtonPanel = $(this).parent();
        var lcItemBlock = lcButtonPanel.closest('[sa-elementtype=element]');
        var lcControl = lcItemBlock.closest('[sa-elementtype=control]');
        var lcQuantityBox = lcButtonPanel.find("input[type=text]");
        var lcForm = $(this).closest('[sa-elementtype=form]');
        var lcShoppingCartIcon = lcForm.find('a[href="@cmd%shoppingcart"]');

        var lcQuantity = isNaN(lcQuantityBox.val()) || lcQuantityBox.val().trim() == '' ? 0 : parseInt(lcQuantityBox.val());

        if (lcQuantity < 99) {

            if (lcQuantity < 0) lcQuantity = 0;

            lcQuantity = lcQuantity + 1;
            lcQuantityBox.val(lcQuantity);
            lcItemBlock.RefreshSubTotalPrice();
            
            lcButtonPanel.attr('fa-hasquantity', 'true');
            lcItemBlock.attr('fa-hasquantity', 'true');
            lcShoppingCartIcon.attr('fa-active', 'true');

            lcControl.RefreshTotalPrice();
        }
    });

    $(this).find('img[href="@cmd%subquantity"]').unbind('click');
    $(this).find('img[href="@cmd%subquantity"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcButtonPanel = $(this).parent();        
        var lcItemBlock = lcButtonPanel.closest('[sa-elementtype=element]');
        var lcControl = lcItemBlock.closest('[sa-elementtype=control]');
        var lcQuantityBox = lcButtonPanel.find("input[type=text]");
        var lcForm = $(this).closest('[sa-elementtype=form]');
        var lcShoppingCartIcon = lcForm.find('a[href="@cmd%shoppingcart"]');

        var lcQuantity = isNaN(lcQuantityBox.val()) ? 0 : parseInt(lcQuantityBox.val());

        if (lcQuantity > 0) {
            lcQuantity = lcQuantity - 1;
            lcQuantityBox.val(lcQuantity);
            lcItemBlock.RefreshSubTotalPrice();

            if (lcQuantity <= 0) {
                lcButtonPanel.removeAttr('fa-hasquantity');
                lcItemBlock.removeAttr('fa-hasquantity');
                if (lcControl.find('[sa-elementtype=container][ea-type=item] [sa-elementtype=element][fa-hasquantity]').length == 0)
                    lcShoppingCartIcon.removeAttr('fa-active');
            }

            lcControl.RefreshTotalPrice();
        }
    });
    
    $(this).find('img[href="@cmd%showchild"]').unbind('click');
    $(this).find('img[href="@cmd%showchild"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcCategoryBlock = $(this).closest('[sa-elementtype=element]');
        var lcControl = lcCategoryBlock.closest('[sa-elementtype=control]');

        lcCategoryBlock.attr('fa-selected', 'true');
        lcControl.attr('fa-categorylisting', lcCategoryBlock.attr('ea-dataid'));        
        lcControl.find('[sa-elementtype=container][ea-type=item] [sa-elementtype=element][ea-group=' + lcCategoryBlock.attr('ea-dataid') + ']').attr('fa-visible', 'true');
    });

    $(this).find('img[href="@cmd%hidechild"]').unbind('click');
    $(this).find('img[href="@cmd%hidechild"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcCategoryBlock = $(this).closest('[sa-elementtype=element]');
        var lcControl = lcCategoryBlock.closest('[sa-elementtype=control]');

        lcCategoryBlock.removeAttr('fa-selected');
        lcControl.removeAttr('fa-categorylisting');
        lcControl.find('[sa-elementtype=container][ea-type=item] [sa-elementtype=element]').removeAttr('fa-visible');
        if (lcControl.find('[sa-elementtype=container][ea-type=item] [sa-elementtype=element][ea-group=' + lcCategoryBlock.attr('ea-dataid') + '][fa-hasquantity]').length > 0)
            lcCategoryBlock.attr('fa-hasquantity', true);
        else
            lcCategoryBlock.removeAttr('fa-hasquantity');        
    });

    lcForm.find('a[href="@cmd%shoppingcart"]').unbind('click');
    lcForm.find('a[href="@cmd%shoppingcart"]').click(function (paEvent) {
        paEvent.preventDefault();

        if ($(this).attr('fa-active') == 'true') {
            var lcForm = $(this).closest('[sa-elementtype=form]');
            var lcControl = lcForm.find('[sa-elementtype=control].WidControlFEBasicStore');

            lcControl.attr('fa-confirmationmode', 'true');
            lcControl.find('[sa-elementtype=element][fa-hasquantity]').attr('fa-enlisted','true');
            lcForm.find('[sa-elementtype=toolbar]').hide();
        }
    });

    $(this).find('img[href="@cmd%showemark"]').unbind('click');
    $(this).find('img[href="@cmd%showremark"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcItemBlock = $(this).closest('[sa-elementtype=element]');

        lcItemBlock.attr('fa-remarkopen','true');
    });

    $(this).find('img[href="@cmd%hideremark"]').unbind('click');
    $(this).find('img[href="@cmd%hideremark"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcItemBlock = $(this).closest('[sa-elementtype=element]');

        lcItemBlock.removeAttr('fa-remarkopen');
    });

    $(this).find('img[href="@cmd%editaddress"]').unbind('click');
    $(this).find('img[href="@cmd%editaddress"]').click(function (paEvent) {
        paEvent.preventDefault();
        
        var lcContainer = $(this).closest('[sa-elementtype=container]');

        lcContainer.attr('fa-addressediting','true');
        
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

    $(this).find('img[href="@cmd%updateaddress"]').unbind('click');
    $(this).find('img[href="@cmd%updateaddress"]').click(function (paEvent) {
        paEvent.preventDefault();
        
        var lcForm      = $(this).closest('[sa-elementtype=form]');
        var lcContainer = $(this).closest('[sa-elementtype=container]');
        var lcControl   = lcContainer.find('[sa-elementtype=control]');
        
        if (lcControl.VerifyInputs()) {
            var lcData = { CobraAjaxRequest: "updatedatarecord", DataBlock: lcControl.GetSerializedAddressData({ InsertMode: lcControl.attr('ea-controlmode').toUpperCase() }) };
            
            GlobalAjaxHandler.SetAjaxLoaderStatusText('Updating .....');

            DoPostBack(lcData, function (paResponseData) {
                var lcRespondStruct = jQuery.parseJSON(paResponseData);
                if (lcRespondStruct.Success) {
                    lcForm.ShowFormMessage(0, 'Successfully Updated', false).done(function (paResult, paControl) {
                        lcControl.attr('ea-controlmode', 'update');
                        lcContainer.RetrieveDataRow();                        
                        lcForm.CloseForm();
                        lcContainer.removeAttr('fa-addressediting');
                    });
                }
                else {
                    lcForm.ShowFormMessage(1, 'Error in Updating Data.', true);
                }
            });
        }
    });

    $(this).find('img[href="@cmd%canceladdress"]').unbind('click');
    $(this).find('img[href="@cmd%canceladdress"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcContainer = $(this).closest('[sa-elementtype=container]');
        var lcControl = lcContainer.find('[sa-elementtype=control]');

        if (lcControl.IsDirty()) {
                lcForm.ShowFormMessage(2, 'Do you want to close ?', false).done(function (paResult, paControl) {
                    if (paResult == 'yes') lcContainer.removeAttr('fa-addressediting');
                });
        }
        else lcContainer.removeAttr('fa-addressediting');
    });

    $(this).find('a[href="@cmd%placeorder"]').unbind('click');
    $(this).find('a[href="@cmd%placeorder"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcForm = $(this).closest('[sa-elementtype=form]');
        var lcControl = $(this).closest('[sa-elementtype=control]');
        var lcAddressControl = lcControl.find('[sa-elementtype=container][ea-type=deliveryinfo] [sa-elementtype=control]');
        var lcOrderRemark = lcControl.find('.OrderRemarkPanel input[type=text]').val();
        
        var lcItemList = lcControl.GetSerializedOrderData();
        var lcDeliveryInfo = lcAddressControl.GetSerializedAddressData({ OrderRemark : lcOrderRemark });
        
        var lcData = { CobraAjaxRequest: "placeorder", ItemList: lcItemList, DeliveryInfo: lcDeliveryInfo };
        GlobalAjaxHandler.SetAjaxLoaderStatusText('Submitting .....');

        DoPostBack(lcData, function (paResponseData) {
            var lcRespondStruct = jQuery.parseJSON(paResponseData);
            if (lcRespondStruct.Success) {
                lcForm.ShowFormMessage(3, 'Successfully Updated', false).done(function (paResult, paControl) {                                        
                    lcForm.ResetForm();
                });
            }
            else {
                lcForm.ShowFormMessage(1, 'Error in Updating Data.', true);
            }
        });        
    });

    $(this).find('a[href="@cmd%back"]').unbind('click');
    $(this).find('a[href="@cmd%back"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcForm = $(this).closest('[sa-elementtype=form]');
        var lcControl = lcForm.find('[sa-elementtype=control].WidControlFEBasicStore');

        lcControl.removeAttr('fa-confirmationmode');        
        lcControl.find('[sa-elementtype=element][fa-hasquantity]').removeAttr('fa-enlisted');
        lcForm.find('[sa-elementtype=toolbar]').show();
    });
}

$.fn.ResetForm = function()
{
    var lcForm = $(this);
    var lcControl = lcForm.find('[sa-elementtype=control].WidControlFEBasicStore');
    var lcShoppingCartIcon = lcForm.find('a[href="@cmd%shoppingcart"]');

    lcControl.removeAttr('fa-confirmationmode');
    lcControl.find('[sa-elementtype=element][fa-hasquantity]').removeAttr('fa-enlisted');
    lcControl.find('[sa-elementtype=element][fa-hasquantity], [sa-elementtype=element] [fa-hasquantity]').removeAttr('fa-hasquantity');
    lcControl.find('[sa-elementtype=element] [sa-elementtype=quantity]').val('0');
        
    lcControl.removeAttr('fa-categorylisting');
    lcControl.find('[sa-elementtype=container][ea-type=category] [sa-elementtype=element]').removeAttr('fa-selected');
    lcControl.find('[sa-elementtype=container][ea-type=item] [sa-elementtype=element]').removeAttr('fa-visible');
        
    lcShoppingCartIcon.removeAttr('fa-active');
    lcForm.find('[sa-elementtype=toolbar]').show();
}

$.fn.RefreshSubTotalPrice = function()
{   
    var lcUnitPriceBox = $(this).find('[sa-elementtype=unitprice]');
    var lcTotalBox = $(this).find('[sa-elementtype=total]');
    var lcQuantityBox = $(this).find('[sa-elementtype=quantity]');
    
    var lcUnitPrice = isNaN(lcUnitPriceBox.text()) ? 0 : parseInt(lcUnitPriceBox.text());
    var lcQuantity = isNaN(lcQuantityBox.val()) ? 0 : parseInt(lcQuantityBox.val());

    lcTotalBox.text(lcUnitPrice * lcQuantity);
}

$.fn.RefreshTotalPrice = function () {    

    var lcControlList = $(this).find('[sa-elementtype=container][ea-type=item] [sa-elementtype=element][fa-hasquantity]');
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

$.fn.VerifyInputs = function () {
    var lcSuccess = true;
    var lcPinCode = '';

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

$.fn.IsDirty = function () {
    var lcDirty = false;
    var lcPinCode = '';

    var lcDataControls = $(this).find('[ea-columnname]');

    lcDataControls.each(function (paIndex) {

        if ($(this).val().trim() != ($(this).attr('ea-originalvalue') || '')) {            
            lcDirty = true;
            return (false);
        }
    });

    return (lcDirty);
}

$.fn.RetrieveDataRow = function () {
    var lcControl = $(this);
    var lcContainer = $(this).closest('[sa-elementtype=container]');

    var lcData = { CobraAjaxRequest: "getdatarowquery", Parameter: "deliveryaddress", DataBlock: lcControl.GetSerializedAddressData() };
    GlobalAjaxHandler.SetAjaxLoaderStatusText('Verifying .....');
    
    DoPostBack(lcData, function (paResponseData) {
        var lcRespondStruct = jQuery.parseJSON(paResponseData);
        if (lcRespondStruct.Success) {            
            if (lcRespondStruct.ResponseData.RSP_KeyDetect == 'true') {                
                lcContainer.PopulateData(lcRespondStruct.ResponseData);
            }
        }
        else {
            ShowErrorMessage('Unexpected Error.');            
        }
    });
}

$.fn.PopulateData = function (paData) {    
    var lcDataControls = $(this).find('[ea-columnname]');

    lcDataControls.each(function () {
        var lcKey = $(this).attr('ea-columnname').toLowerCase();
        if ($(this).is('div')) {
            if(!$(this).attr('sa-elementtype'))
                $(this).html(paData[lcKey]);
        }
        else {
            $(this).val(paData[lcKey]);
            $(this).attr('ea-originalvalue', paData[lcKey]);
        }
    });
}


$.fn.GetSerializedAddressData = function (paObject) {
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

    $.extend(lcDataBlock, paObject);

    return (Base64.encode(JSON.stringify(lcDataBlock)));
}


$.fn.GetSerializedOrderData = function () {
    var lcMainBlock = [];

    var lcOrderedItem = $(this).find('[sa-elementtype=element][fa-hasquantity]');    

    lcOrderedItem.each(function (paIndex) {
        var lcFieldBlock = {};

        lcFieldBlock['ItemID'] = $(this).attr('ea-dataid');

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
