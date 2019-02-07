$(document).ready(function () {
    POSStaffPermissionSettingManager.Init();
});

var POSStaffPermissionSettingManager = (function () {

    var clControl;
    var clForm;
    var clToolBar;
    var clStaffPermissionBlock;
    var clStaffRestrictionBlock;
    var clSystemConfig;
    var clStaffPermissionConfig;
    var clSystemActionLimitDays;

    return {
        Init: function () {
            clControl                   = $('[sa-elementtype=control].WidControlPOSStaffPermissionSetting');
            clForm                      = clControl.closest('[sa-elementtype=form]');
            clToolBar                   = clForm.find("[sa-elementtype=toolbar].ToolBar");
            clStaffPermissionBlock      = clControl.find('[sa-elementtype=block][ea-type=staffpermission]');
            clStaffRestrictionBlock     = clControl.find('[sa-elementtype=block][ea-type=staffrestriction]');
            clStaffPermissionConfig     = JSON.parse(Base64.decode(clControl.attr('pos.staffpermissionsetting') || 'e30='));
            clSystemConfig              = JSON.parse(Base64.decode(clControl.attr('ea-systemconfig') || 'e30='));      
            clSystemActionLimitDays     = (clSystemConfig['receiptactionlimitdays'] || '').ForceConvertToInteger(120);
            
            clControl.find('input[type=text][ea-inputmode=number]').ForceIntegerInput();

            POSStaffPermissionSettingManager.RetreiveConfig();
                        
            POSStaffPermissionSettingManager.BindEvents();
        },
        BindEvents: function () {
            clControl.find('[ea-command^="@cmd%"]').unbind('click');
            clControl.find('[ea-command^="@cmd%"]').click(POSStaffPermissionSettingManager.HandlerOnClick);

            clToolBar.find('[ea-command^="@cmd%"]').unbind('click');
            clToolBar.find('[ea-command^="@cmd%"]').click(POSStaffPermissionSettingManager.HandlerOnClick);

            clControl.find('input[type=text]').unbind('blur');
            clControl.find('input[type=text]').blur(POSStaffPermissionSettingManager.HandlerOnBlur);
        },
        RetreiveConfig: function () {
            var lcToggleButtons = clStaffPermissionBlock.find('[ea-columnname][ea-name][sa-elementtype=button]');
            var lcInputBoxes    = clStaffRestrictionBlock.find('input[type=text][ea-columnname][ea-name]');
            
            clControl.attr('notransition', 'true');
            
            lcToggleButtons.each(function () {
                var lcLinkColumnName = $(this).attr('ea-linkcolumn');
                var lcValue = (clStaffPermissionConfig[$(this).attr('ea-name')] || 'false').toLowerCase();

                $(this).attr('value', lcValue == 'true' ? 'true' : 'false');

                POSStaffPermissionSettingManager.SetInputBoxEnableState(lcLinkColumnName, $(this).attr('value'));
            });

            lcInputBoxes.each(function () {
                var lcValue = clStaffPermissionConfig[$(this).attr('ea-name')] || '';
                $(this).attr('value', lcValue);
                $(this).val(FormManager.ConvertToFormLanguage(lcValue));
            });

            POSStaffPermissionSettingManager.UpdateOriginalValues();

            setTimeout(function () { clControl.removeAttr('notransition'); }, 1000);
        },
        UpdateAppSetting: function () {
            var lcMasterBlock = {};
            var lcSettingData = POSStaffPermissionSettingManager.GetSettingData();

            if (!jQuery.isEmptyObject(lcSettingData)) {
                lcMasterBlock['settingdata'] = Base64.encode(JSON.stringify(lcSettingData));
                var lcAjaxRequestManager = new AjaxRequestManager('executescalarquery', null, 'err_failupdate', 'ajax_updating');

                lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.updatesetting');
                lcAjaxRequestManager.AddObjectDataBlock('DataBlock', lcMasterBlock, true);

                lcAjaxRequestManager.SetCompleteHandler(function (paSuccess) {
                    if (paSuccess) {
                        POSStaffPermissionSettingManager.UpdateOriginalValues();
                    }
                });

                lcAjaxRequestManager.Execute();
            }
          //  else FormManager.CloseForm();
        },        
        GetControlValue: function (paElement) {
            if (paElement) {
                if (paElement.is('input[type=text]'))
                {
                    var lcInputMode = paElement.attr('ea-inputmode');

                    if ((lcInputMode == 'number') || (lcInputMode == 'decimal')) return(FormManager.NormalizeNumber(paElement.val()));
                    else return (paElement.val());
                }
                else if (paElement.is('img')) {
                    if (paElement.attr('ea-desktopbackgroundcss')) return (paElement.attr('ea-desktopbackgroundcss'));
                    else return (paElement.attr('src'));
                }
                else return (paElement.attr('value'));
            }
            return (null);
        },
        IsControlValueChanged: function (paColumnName) {

            var lcControlList = clControl.find('[ea-columnname="' + paColumnName + '"]');
            var lcValueChanged = false;

            lcControlList.each(function () {

                var lcControlValue = POSStaffPermissionSettingManager.GetControlValue($(this)) || '';
                var lcOriginalValue = $(this).attr('ea-originalvalue') || '';

                lcValueChanged = lcControlValue.trim() != lcOriginalValue.trim();

                if (lcValueChanged == true) return (false);
            });

            return (lcValueChanged);
        },        
        UpdateOriginalValues: function () {
            var lcControlList = clControl.find('[ea-columnname]');

            lcControlList.each(function () {
                var lcElement = $(this);

                if (lcElement.is('input[type=text]'))
                {                    
                    var lcInputMode = lcElement.attr('ea-inputmode');

                    if ((lcInputMode == 'number') || (lcInputMode == 'decimal')) lcElement.attr('ea-originalvalue', FormManager.NormalizeNumber(lcElement.val()));
                    else lcElement.attr('ea-originalvalue', lcElement.val());
                }
                else if (lcElement.is('img')) {
                    if (lcElement.attr('ea-desktopbackgroundcss')) lcElement.attr('ea-originalvalue', lcElement.attr('ea-desktopbackgroundcss'));
                    else lcElement.attr('ea-originalvalue', lcElement.attr('src'));
                }
                else lcElement.attr('ea-originalvalue', lcElement.attr('value'));
            });
        },
        CompileSettingKey: function (paColumnName, paMultiDataMode) {
            if (paColumnName) {
                var lcSettingData = {};
                var lcControlList = clControl.find('[ea-columnname="' + paColumnName + '"]');
                var lcValue;

                if (paMultiDataMode) {
                    var lcValueObject = {};

                    lcControlList.each(function () {
                        var lcName = $(this).attr('ea-name');
                        var lcResult = POSStaffPermissionSettingManager.GetControlValue($(this));

                        if (lcResult != null) {
                            lcValueObject[lcName] = lcResult;
                        }
                    });

                    lcValue = JSON.stringify(lcValueObject);
                }
                else lcValue = POSStaffPermissionSettingManager.GetControlValue(lcControlList);

                if (lcValue != null) lcSettingData[paColumnName] = lcValue;

                return (lcSettingData);
            }
        },
        GetColumnNameList: function () {
            var lcColumnNameList = {};

            clControl.find('[ea-columnname]').each(function () {
                if (!lcColumnNameList[$(this).attr('ea-columnname')])
                    lcColumnNameList[$(this).attr('ea-columnname')] = $(this).attr('ea-name') ? true : false;
            });
            return (lcColumnNameList);
        },
        GetSettingData: function () {
            var lcDataBlock = {};
            var lcColumnNameList = POSStaffPermissionSettingManager.GetColumnNameList();

            for (var lcColumn in lcColumnNameList)
            {
                if (POSStaffPermissionSettingManager.IsControlValueChanged(lcColumn))
                    lcDataBlock = $.extend(lcDataBlock, POSStaffPermissionSettingManager.CompileSettingKey(lcColumn, lcColumnNameList[lcColumn]));
            }

            return (lcDataBlock);
        },
        SetInputBoxEnableState : function(paName, paState)
        {
            if ((paState) && (paName))
            {
                var lcInputBox = clStaffRestrictionBlock.find('input[type=text][ea-name="' + paName + '"]');
                var lcInputRow = lcInputBox.closest('[sa-elementtype=inputrow]');

                if ((lcInputBox) && (lcInputRow))
                {
                    if (paState == 'false') {
                        lcInputBox.attr('disabled', 'true');
                        lcInputRow.attr('disabled', 'true');
                    }
                    else
                    {
                        lcInputBox.removeAttr('disabled');
                        lcInputRow.removeAttr('disabled');
                    }
                }
            }
        },
        ActionOnToggle: function (paToggleSwitch) {
            if (paToggleSwitch) {
                var lcLinkColumnName = paToggleSwitch.attr('ea-linkcolumn');

                if (paToggleSwitch.attr('value') == 'true') paToggleSwitch.attr('value', 'false');
                else (paToggleSwitch.attr('value', 'true'));
                
                POSStaffPermissionSettingManager.SetInputBoxEnableState(lcLinkColumnName, paToggleSwitch.attr('value'));
            }
        },
        HandlerOnBlur : function(paEvent) {            
            var lcOriginalValue = ($(this).attr('ea-originalvalue') || '').ForceConvertToInteger(0);
            var lcValue         = $(this).val().NormalizeNumber().ForceConvertToInteger(lcOriginalValue);

            var lcLowerBound = 1;
            var lcUpperBound = clSystemActionLimitDays;

            if (lcValue < lcLowerBound) lcValue = lcLowerBound;
            if (lcValue > lcUpperBound) lcValue = lcUpperBound;
            
            $(this).val(lcValue.toString().ToLocalNumber());
        },
        HandlerOnClick: function (paEvent) {
            paEvent.preventDefault();

            var lcCommand = $(this).attr('ea-command');
            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);

            switch (lcCommand) {
                case 'toggle':
                    {
                        POSStaffPermissionSettingManager.ActionOnToggle($(this));
                        break;
                    }

                case 'save':
                    {
                        POSStaffPermissionSettingManager.UpdateAppSetting()
                        break;
                    }
                case 'language':
                    {
                        POSStaffPermissionSettingManager.SetLanguage($(this).attr('value'));
                        break;
                    }
            }

        }
    }
})();
