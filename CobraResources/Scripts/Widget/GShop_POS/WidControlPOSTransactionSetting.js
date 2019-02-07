$(document).ready(function () {
    POSTransactionSettingManager.Init();
});

var POSTransactionSettingManager = (function () {

    var clControl;
    var clForm;
    var clToolBar;
    var clReceiptPrintOptionBlock;
    var clPaymentOptionBlock;
    var clTaxOptionBlock;
    var clSystemConfig;
    var clTransactionSettingConfig;
    var clSystemActionLimitDays;

    return {
        Init: function () {
            clControl                       = $('[sa-elementtype=control].WidControlPOSTransactionSetting');
            clForm                          = clControl.closest('[sa-elementtype=form]');
            clToolBar                       = clForm.find("[sa-elementtype=toolbar].ToolBar");
            clReceiptPrintOptionBlock       = clControl.find('[sa-elementtype=block][ea-type=receiptprintoption]');
            clPaymentOptionBlock            = clControl.find('[sa-elementtype=block][ea-type=paymentoption]');
            clTaxOptionBlock                = clControl.find('[sa-elementtype=block][ea-type=taxoption]');
            clTransactionSettingConfig      = JSON.parse(Base64.decode(clControl.attr('pos.transactionsetting') || 'e30='));
            clSystemConfig                  = JSON.parse(Base64.decode(clControl.attr('ea-systemconfig') || 'e30='));
            
            clControl.find('input[type=text][ea-inputmode=number]').ForceIntegerInput();

            POSTransactionSettingManager.RetreiveConfig();

            POSTransactionSettingManager.BindEvents();
        },
        BindEvents: function () {
            clControl.find('[ea-command^="@cmd%"]').unbind('click');
            clControl.find('[ea-command^="@cmd%"]').click(POSTransactionSettingManager.HandlerOnClick);

            clToolBar.find('[ea-command^="@cmd%"]').unbind('click');
            clToolBar.find('[ea-command^="@cmd%"]').click(POSTransactionSettingManager.HandlerOnClick);
        },
        RetreiveConfig: function () {
            var lcToggleButtons = clControl.find('[ea-columnname][ea-name][sa-elementtype=button]');
            var lcInputBoxes = clTaxOptionBlock.find('input[type=text][ea-columnname][ea-name]');

            clControl.attr('notransition', 'true');

            lcToggleButtons.each(function () {
                var lcLinkColumnName = $(this).attr('ea-linkcolumn');
                var lcValue = (clTransactionSettingConfig[$(this).attr('ea-name')] || 'false').toLowerCase();
                var lcKeyValue = $(this).attr('ea-keyvalue');

                if (lcKeyValue) $(this).attr('value', lcValue.indexOf(lcKeyValue) !== -1 ? 'true' : 'false');
                    else $(this).attr('value', lcValue == 'true' ? 'true' : 'false');

                POSTransactionSettingManager.SetLinkedEnableState(lcLinkColumnName, $(this).attr('value'));
            });

            lcInputBoxes.each(function () {
                var lcValue = clTransactionSettingConfig[$(this).attr('ea-name')] || '';
                $(this).attr('value', lcValue);
                $(this).val(FormManager.ConvertToFormLanguage(lcValue));
            });

            POSTransactionSettingManager.UpdateOriginalValues();

            setTimeout(function () { clControl.removeAttr('notransition'); }, 1000);
        },
        UpdateAppSetting: function () {
            var lcMasterBlock = {};
            var lcSettingData = POSTransactionSettingManager.GetSettingData();

            if (!jQuery.isEmptyObject(lcSettingData)) {
                lcMasterBlock['settingdata'] = Base64.encode(JSON.stringify(lcSettingData));
                var lcAjaxRequestManager = new AjaxRequestManager('executescalarquery', null, 'err_failupdate', 'ajax_updating');

                lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.updatesetting');
                lcAjaxRequestManager.AddObjectDataBlock('DataBlock', lcMasterBlock, true);

                lcAjaxRequestManager.SetCompleteHandler(function (paSuccess) {
                    if (paSuccess) {
                        POSTransactionSettingManager.UpdateOriginalValues();
                    }
                });

                lcAjaxRequestManager.Execute();
            }
         //   else FormManager.CloseForm();
        },
        GetControlValue: function (paElement) {
            if (paElement) {
                if (paElement.is('input[type=text]')) {
                    var lcInputMode = paElement.attr('ea-inputmode');

                    if ((lcInputMode == 'number') || (lcInputMode == 'decimal')) return (FormManager.NormalizeNumber(paElement.val()));
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

                var lcControlValue = POSTransactionSettingManager.GetControlValue($(this)) || '';
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

                if (lcElement.is('input[type=text]')) {
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
                    var lcValueObject = JSON.parse(Base64.decode(clControl.attr(paColumnName) || 'e30='));

                    lcControlList.each(function (paIndex) {
                        var lcName = $(this).attr('ea-name');
                        var lcKeyValue = $(this).attr('ea-keyvalue');
                        var lcResult = POSTransactionSettingManager.GetControlValue($(this));
                        
                        if (lcResult != null)
                        {
                            if (lcKeyValue)
                            {
                                lcValueObject[lcName] = paIndex == 0 ? '' : lcValueObject[lcName];
                                if ((lcResult == 'true') && (lcValueObject[lcName].indexOf(lcKeyValue) == -1)) lcValueObject[lcName] += (lcValueObject[lcName].length == 0 ? "" : ",") + lcKeyValue;
                            }
                            else lcValueObject[lcName] = lcResult;
                        }                        
                    });
                    
                    
                    lcValue = JSON.stringify(lcValueObject);
                }
                else lcValue = POSTransactionSettingManager.GetControlValue(lcControlList);

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
            var lcColumnNameList = POSTransactionSettingManager.GetColumnNameList();
            
            for (var lcColumn in lcColumnNameList) {
                if (POSTransactionSettingManager.IsControlValueChanged(lcColumn))
                    lcDataBlock = $.extend(lcDataBlock, POSTransactionSettingManager.CompileSettingKey(lcColumn, lcColumnNameList[lcColumn]));
            }          
            return (lcDataBlock);
        },
        SetLinkedEnableState: function (paName, paState) {
            if ((paState) && (paName)) {
                var lcElement = clControl.find('[ea-name="' + paName + '"]');
                var lcInputRow = lcElement.closest('[sa-elementtype=inputrow]');

                if ((lcElement) && (lcInputRow)) {
                    if (paState == 'false') {
                        lcElement.attr('disabled', 'true');
                        lcInputRow.attr('disabled', 'true');
                    }
                    else {
                        lcElement.removeAttr('disabled');
                        lcInputRow.removeAttr('disabled');
                    }
                }
            }
        },
        ActionOnToggle: function (paToggleSwitch) {
            if (paToggleSwitch) {
                var lcDisabled = paToggleSwitch.attr('disabled');
                var lcLinkColumnName = paToggleSwitch.attr('ea-linkcolumn');

                if (!lcDisabled) {
                    if (paToggleSwitch.attr('value') == 'true') paToggleSwitch.attr('value', 'false');
                    else (paToggleSwitch.attr('value', 'true'));

                    POSTransactionSettingManager.SetLinkedEnableState(lcLinkColumnName, paToggleSwitch.attr('value'));
                }
            }
        },
        HandlerOnClick: function (paEvent) {
            paEvent.preventDefault();

            var lcCommand = $(this).attr('ea-command');
            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);

            switch (lcCommand) {
                case 'toggle':
                    {
                        POSTransactionSettingManager.ActionOnToggle($(this));
                        break;
                    }

                case 'save':
                    {
                        POSTransactionSettingManager.UpdateAppSetting()
                        break;
                    }

                case 'language':
                    {
                        POSTransactionSettingManager.SetLanguage($(this).attr('value'));
                        break;
                    }
            }

        }
    }
})();
