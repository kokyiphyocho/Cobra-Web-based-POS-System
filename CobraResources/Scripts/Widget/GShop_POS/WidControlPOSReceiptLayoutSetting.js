$(document).ready(function () {
    POSReceiptLayoutSettingManager.Init();
});

var POSReceiptLayoutSettingManager = (function () {

    var clControl;
    var clForm;
    var clToolBar;
    var clContentContainer;
    var clExternalComponentContainer;
    var clReceiptLayoutBlock;
    var clHeaderTextBlock;
    var clFootNoteBlock;
    var clReceiptLayoutConfig;
    var clPrimaryPrinterConfig;
    var clCustomizationConfig;    

    return {
        Init: function () {
            clControl               = $('[sa-elementtype=control].WidControlPOSReceiptLayoutSetting');
            clForm                  = clControl.closest('[sa-elementtype=form]');
            clToolBar               = clForm.find("[sa-elementtype=toolbar].ToolBar");
            clReceiptLayoutBlock    = clControl.find('[sa-elementtype=block][ea-type=receiptlayout]');
            clHeaderTextBlock       = clControl.find('[sa-elementtype=block][ea-type=headertext]');
            clFootNoteBlock         = clControl.find('[sa-elementtype=block][ea-type=footnote]');
            clPrinterStatusControl = clForm.find('.FormTitleBar div[sa-elementtype=statuscontrol]');
            
            clContentContainer              = clControl.find('[sa-elementtype=container][ea-type=content].ContentContainer');
            clExternalComponentContainer    = clForm.find('[sa-elementtype=container][ea-type=externalcomponent]');

            clReceiptLayoutConfig   = JSON.parse(Base64.decode(clControl.attr('pos.receiptlayoutinfo.layout') || 'e30='));
            clCustomizationConfig   = JSON.parse(Base64.decode(clControl.attr('pos.receiptlayoutinfo.customization') || 'e30='));
            clPrimaryPrinterConfig  = JSON.parse(Base64.decode(clControl.attr('pos.primaryprintersetting') || 'e30='));
                        
            POSReceiptLayoutSettingManager.LoadExternalComponents().done(function (paSuccess) {
                if (paSuccess) {
                    POSReceiptLayoutSettingManager.WaitForDependencies().done(function () {

                        POSReceiptLayoutSettingManager.RetreiveConfig();
                        POSReceiptLayoutSettingManager.BindEvents();

                        POSReceiptPrintingManager.Init(clControl);
                        POSReceiptPrintingManager.ConnectPrinter();
                    });
                }
            });
        },
        WaitForDependencies: function () {
            var lcDeferred = $.Deferred();

            var lcWaitTimer = setInterval(function () {
                if (typeof POSReceiptPrintingManager !== 'undefined') {
                    if (lcDeferred.state() == 'pending') {
                        lcDeferred.resolve();
                        clearInterval(lcWaitTimer);
                    }
                }

            }, 200);

            return (lcDeferred);
        },
        LoadExternalComponents: function () {
            var lcDeferred = $.Deferred();

            var lcAjaxRequestManager = new AjaxRequestManager('getupdatedcontrol', null, null, 'ajax_loading');

            lcAjaxRequestManager.AddAjaxParam('Parameter', 'externalcomponent');

            lcAjaxRequestManager.SetCompleteHandler(function (paSuccess, paResponseStruct) {
                if (paSuccess) {
                    clExternalComponentContainer.empty();
                    clExternalComponentContainer.html(paResponseStruct.ResponseData.RSP_HTML);
                    lcDeferred.resolve(true);
                }
                else {
                    lcDeferred.resolve(false)
                }
            });

            lcAjaxRequestManager.Execute();

            return (lcDeferred);
        },
        BindEvents: function () {

            clControl.find('input[type=text][ea-inputmode=number]').ForceIntegerInput();
            clControl.find('input[type=text][ea-inputmode=number]').ForceNumberBoundLimit();
            
            clControl.find('[ea-command^="@cmd%"]').unbind('click');
            clControl.find('[ea-command^="@cmd%"]').click(POSReceiptLayoutSettingManager.HandlerOnClick);

            clToolBar.find('[ea-command^="@cmd%"]').unbind('click');
            clToolBar.find('[ea-command^="@cmd%"]').click(POSReceiptLayoutSettingManager.HandlerOnClick);

            clPrinterStatusControl.unbind('click');
            clPrinterStatusControl.click(POSReceiptLayoutSettingManager.HandlerOnClick);

            clControl.unbind('ev-printernotification');
            clControl.bind('ev-printernotification', POSReceiptLayoutSettingManager.HandlerOnPrinterNotification);
        },
        RetreiveConfig: function () {
            var lcToggleButtons = clReceiptLayoutBlock.find('[ea-columnname][ea-name][sa-elementtype=button]');
            var lcInputBoxes = clControl.find('input[type=text][ea-columnname][ea-name],textarea[ea-columnname][ea-name]');
            
            clControl.attr('notransition', 'true');

            lcToggleButtons.each(function () {            
                var lcValue = (clReceiptLayoutConfig[$(this).attr('ea-name')] || 'false').toLowerCase();                
                $(this).attr('value', lcValue == 'true' ? 'true' : 'false');
            });

            lcInputBoxes.each(function () {
                var lcValue = clCustomizationConfig[$(this).attr('ea-name')] || clReceiptLayoutConfig[$(this).attr('ea-name')] || clPrimaryPrinterConfig[$(this).attr('ea-name')] || '';
                $(this).attr('value', lcValue);
                if ($(this).attr('ea-inputmode') == 'number')
                    $(this).val(FormManager.ConvertToFormLanguage(lcValue));
                else
                    $(this).val(lcValue);
            });

            POSReceiptLayoutSettingManager.UpdateOriginalValues();

            setTimeout(function () { clControl.removeAttr('notransition'); }, 1000);
        },
        UpdateAppSetting: function () {
            var lcMasterBlock = {};
            var lcSettingData = POSReceiptLayoutSettingManager.GetSettingData();

            if (!jQuery.isEmptyObject(lcSettingData)) {
                lcMasterBlock['settingdata'] = Base64.encode(JSON.stringify(lcSettingData));
                var lcAjaxRequestManager = new AjaxRequestManager('executescalarquery', null, 'err_failupdate', 'ajax_updating');

                lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.updatesetting');
                lcAjaxRequestManager.AddObjectDataBlock('DataBlock', lcMasterBlock, true);

                lcAjaxRequestManager.SetCompleteHandler(function (paSuccess) {
                    if (paSuccess) {
                        POSReceiptLayoutSettingManager.UpdateOriginalValues();
                    }
                });

                lcAjaxRequestManager.Execute();
            }
         //   else FormManager.CloseForm();
        },
        GetControlValue: function (paElement) {
            if (paElement) {
                if (paElement.is('input[type=text],textarea')) {
                    var lcInputMode = paElement.attr('ea-inputmode');
                    
                    if (lcInputMode == 'number') return(CastInteger(FormManager.NormalizeNumber(paElement.val()), paElement.attr('ea-originalvalue') || 0));
                    else if (lcInputMode == 'decimal') return (CastDecimal(FormManager.NormalizeNumber(paElement.val()), paElement.attr('ea-originalvalue') || 0));
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

                var lcControlValue = POSReceiptLayoutSettingManager.GetControlValue($(this)) || '';
                var lcOriginalValue = $(this).attr('ea-originalvalue') || '';

                lcValueChanged = ((lcControlValue || '').toString().trim()) != lcOriginalValue.trim();

                if (lcValueChanged == true) return (false);
            });

            return (lcValueChanged);
        },
        UpdateOriginalValues: function () {
            var lcControlList = clControl.find('[ea-columnname]');

            lcControlList.each(function () {
                var lcElement = $(this);

                if (lcElement.is('input[type=text],textarea')) {
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
        CompileSettingKey: function (paColumnName, paMultiDataMode, paObjectMode) {
            if (paColumnName) {
                var lcSettingData = {};
                var lcControlList = clControl.find('[ea-columnname="' + paColumnName + '"]');
                var lcValue;

                if (paMultiDataMode) {
                    var lcValueObject = JSON.parse(Base64.decode(clControl.attr(paColumnName) || 'e30='));
                    
                    lcControlList.each(function () {
                        var lcName = $(this).attr('ea-name');
                        var lcResult = POSReceiptLayoutSettingManager.GetControlValue($(this));

                        if (lcResult != null) {
                            lcValueObject[lcName] = lcResult;
                        }
                    });

                    if (paObjectMode) lcValue = lcValueObject;
                    else lcValue = JSON.stringify(lcValueObject);
                }
                else lcValue = POSReceiptLayoutSettingManager.GetControlValue(lcControlList);

                if (lcValue != null)                
                    lcSettingData[paColumnName] = lcValue;

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
            var lcColumnNameList = POSReceiptLayoutSettingManager.GetColumnNameList();

            for (var lcColumn in lcColumnNameList) {
                if (POSReceiptLayoutSettingManager.IsControlValueChanged(lcColumn))
                    lcDataBlock = $.extend(lcDataBlock, POSReceiptLayoutSettingManager.CompileSettingKey(lcColumn, lcColumnNameList[lcColumn]));
            }          
            return (lcDataBlock);
        },
        SetReceiptChangesForPrinting : function()
        {
            var lcDataBlock = {};
            var lcColumnNameList = POSReceiptLayoutSettingManager.GetColumnNameList();
            
            for (var lcColumn in lcColumnNameList) {                
                lcDataBlock = $.extend(lcDataBlock, POSReceiptLayoutSettingManager.CompileSettingKey(lcColumn, lcColumnNameList[lcColumn], true));                
            }

            POSReceiptPrintingManager.SetReceiptWidth(lcDataBlock['POS.ReceiptLayoutInfo.Layout'].Width);
            POSReceiptPrintingManager.SetReceiptLayoutParameter('LocalNumberMode', lcDataBlock['POS.ReceiptLayoutInfo.Layout'].LocalNumberMode == 'true' ? true : false);
            POSReceiptPrintingManager.SetReceiptCustomizationParameter('BusinessName', lcDataBlock['POS.ReceiptLayoutInfo.Customization'].BusinessName);
            POSReceiptPrintingManager.SetReceiptCustomizationParameter('Address', (lcDataBlock['POS.ReceiptLayoutInfo.Customization'].Address || '').split('\n'));
            POSReceiptPrintingManager.SetReceiptCustomizationParameter('FootNote', (lcDataBlock['POS.ReceiptLayoutInfo.Customization'].FootNote || '').split('\n'));
            PrinterManager.SetPrinterDarkness(lcDataBlock['POS.PrimaryPrinterSetting'].Darkness);
        },
        SetInputBoxEnableState: function (paName, paState) {
            if ((paState) && (paName)) {
                var lcInputBox = clStaffRestrictionBlock.find('input[type=text][ea-name="' + paName + '"]');
                var lcInputRow = lcInputBox.closest('[sa-elementtype=inputrow]');

                if ((lcInputBox) && (lcInputRow)) {
                    if (paState == 'false') {
                        lcInputBox.attr('disabled', 'true');
                        lcInputRow.attr('disabled', 'true');
                    }
                    else {
                        lcInputBox.removeAttr('disabled');
                        lcInputRow.removeAttr('disabled');
                    }
                }
            }
        },
        PrintTestPage: function () {            
            MessageHandler.ShowMessage('confirm_printtestpage').done(function (paResult) {
                if (paResult == 'yes') {

                    var lcTestPrintTemplate = JSON.parse(Base64.decode(clControl.attr('ea-template') || 'e30='));

                    POSReceiptLayoutSettingManager.SetReceiptChangesForPrinting();                    
                    POSReceiptPrintingManager.PrinteJSONReceipt(lcTestPrintTemplate);
                }
            });
        },
        SetPrinterStatusControlAttribute: function (paStatus, paErrorCode, paErrorParam) {
            clPrinterStatusControl.attr('fa-status', paStatus);

            if ((paStatus != 'none') && (paErrorCode)) clPrinterStatusControl.attr('fa-errorcode', paErrorCode);
            else clPrinterStatusControl.removeAttr('fa-errorcode');

            if (paErrorParam) clPrinterStatusControl.attr('fa-errorparam', paErrorParam);
            else clPrinterStatusControl.removeAttr('fa-errorparam');
        },
        SetPrinterStatus: function (paErrorCode, paErrorParam) {
            
            switch (paErrorCode) {
                case 'none'       :
                case 'success'    :
                case 'connecting' :
                    {
                        if (paErrorCode == 'none') paErrorCode = 'success';
                        POSReceiptLayoutSettingManager.SetPrinterStatusControlAttribute(paErrorCode, '');
                        break;
                    }

                case 'err_printer_connectionfail':
                case 'err_printer_initfail':
                case 'err_printer_noresponse':
                case 'err_printer_offline':
                case 'err_printer_coveropen':
                case 'err_printer_mechanicalerror':
                case 'err_printer_unrecoverableerror':
                case 'err_printer_receiptend':
                    {
                        POSReceiptLayoutSettingManager.SetPrinterStatusControlAttribute('error', paErrorCode, paErrorParam);
                        break;
                    }

                case 'err_printer_autocuttererror':
                case 'err_printer_receiptnearend':
                    {
                        POSReceiptLayoutSettingManager.SetPrinterStatusControlAttribute('warning', paErrorCode, paErrorParam);
                        break;
                    }
            }
        },
        ShowPrinterStatus: function () {
            var lcErrorCode = clPrinterStatusControl.attr('fa-errorcode');
            var lcErrorParam = clPrinterStatusControl.attr('fa-errorparam')

            if (lcErrorCode) {
                MessageHandler.ShowMessage(lcErrorCode, function (paOption) {
                    if (lcErrorParam) {
                        paOption.message = "<br/>" + lcErrorParam;
                    }
                });
            }
        },
        ActionOnToggle: function (paToggleSwitch) {
            if (paToggleSwitch) {
                var lcLinkColumnName = paToggleSwitch.attr('ea-linkcolumn');

                if (paToggleSwitch.attr('value') == 'true') paToggleSwitch.attr('value', 'false');
                else (paToggleSwitch.attr('value', 'true'));

                POSReceiptLayoutSettingManager.SetInputBoxEnableState(lcLinkColumnName, paToggleSwitch.attr('value'));
            }
        },
        HandlerOnPrinterNotification: function (paEventArg, paNotificationInfo) {
            
            if (paNotificationInfo) {
                switch (paNotificationInfo.event) {
                    case 'connecting':
                        {
                            POSReceiptLayoutSettingManager.SetPrinterStatus('connecting');
                        }
                    case 'initfail':
                        {
                            POSReceiptLayoutSettingManager.SetPrinterStatus(paNotificationInfo.errorcode, paNotificationInfo.errorparam);
                            break;
                        }

                    case 'initsuccess':
                        {
                            POSReceiptLayoutSettingManager.SetPrinterStatus('success');
                            break;
                        }

                    case 'printfail':
                        {
                            Messagehandler.ShowMessage(paNotificationInfo.errorcode);
                            break;
                        }

                    case 'statuschanged':
                        {
                            POSReceiptLayoutSettingManager.SetPrinterStatus(paNotificationInfo.errorcode, paNotificationInfo.errorparam);
                            break;
                        }
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
                        POSReceiptLayoutSettingManager.ActionOnToggle($(this));
                        break;
                    }

                case 'save':
                    {
                        POSReceiptLayoutSettingManager.UpdateAppSetting()
                        break;
                    }
                case 'language':
                    {
                        POSReceiptLayoutSettingManager.SetLanguage($(this).attr('value'));
                        break;
                    }

                case 'printtestpage':
                    {
                        POSReceiptLayoutSettingManager.PrintTestPage();
                        break;
                    }

                case 'printerstatus':
                    {
                        POSReceiptLayoutSettingManager.ShowPrinterStatus();
                        break;
                    }
            }

        }
    }
})();
