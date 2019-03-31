$(document).ready(function () {
    POSPrinterSettingManager.Init();
});

var POSPrinterSettingManager = (function () {

    var clControl;
    var clContentContainer;
    var clExternalComponentContainer;
    var clPrimaryPrinterSetting;
    var clPrinterList;    
    var clPrinterSelectionPopUp;
    var clPrinterSelectionController;
    var clPrinterNameTextBox;

    return {

        Init: function () {
            clControl                       = $('[sa-elementtype=control].WidControlPOSPrinterSetting');
            clForm                          = clControl.closest('[sa-elementtype=form]');
            clToolBar                       = clForm.find('[sa-elementtype=toolbar].ToolBar');
            clContentContainer              = clControl.find('[sa-elementtype=container][ea-type=content].ContentContainer');
            clExternalComponentContainer    = clForm.find('[sa-elementtype=container][ea-type=externalcomponent]');
            clPrinterNameTextBox            = clContentContainer.find('[ea-type=PrinterName] .InputDiv input[type=text]');            

            clControl.find('[ea-inputmode=decimal]').ForceDecimalInput();
            clControl.find('[ea-inputmode=number]').ForceIntegerInput();
            clControl.find('[ea-inputmode=ipaddress]').ForceIPAddressInput();
            POSPrinterSettingManager.RetrivePrinterInfo();

            POSPrinterSettingManager.LoadExternalComponents().done(function (paSuccess)
            {
                if (paSuccess)
                {
                    POSPrinterSettingManager.WaitForDependencies().done(function () {
                        
                        clPrinterSelectionPopUp = clExternalComponentContainer.find('[sa-elementtype=popup][ea-type=printerlist]');                        
                        clPrinterSelectionController = new SelectionPanelController(clPrinterSelectionPopUp, clControl);
                        clPrinterSelectionController.Init();

                        POSPrinterSettingManager.BindEvents();
                        POSReceiptPrintingManager.Init(clControl);
                    });
                }                
            });            
        },

        WaitForDependencies: function () {
            var lcDeferred = $.Deferred();

            var lcWaitTimer = setInterval(function () {
                if ((typeof SelectionPanelController !== 'undefined') && (typeof POSReceiptPrintingManager !== 'undefined')) {
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

        BindEvents : function()
        {            
            clControl.unbind('ev-selectionpanelevent');
            clControl.bind('ev-selectionpanelevent', POSPrinterSettingManager.HandlerOnSelectionPanelEvent);

            clControl.unbind('ev-printernotification');
            clControl.bind('ev-printernotification', POSPrinterSettingManager.HandlerOnPrinterNotification);

            clContentContainer.find("[ea-command]").unbind('click');
            clContentContainer.find("[ea-command]").click(POSPrinterSettingManager.HandlerOnClick);

            clToolBar.find("[ea-command]").unbind('click');
            clToolBar.find("[ea-command]").click(POSPrinterSettingManager.HandlerOnClick);            
        },

        RetrivePrinterInfo : function()
        {   
            clPrimaryPrinterSetting = JSON.parse(Base64.decode(clControl.attr('POS.PrimaryPrinterSetting') || 'e30='));

            clPrinterList = JSON.parse(Base64.decode(clControl.attr('ea-additionaldata') || 'e30='));
            
            if (clPrimaryPrinterSetting) POSPrinterSettingManager.RetreiveConfig(false);
        },
        RetreiveConfig: function (paUpdateOriginalValue)
        {
            var lcInputBoxes = clControl.find('input[type=text][ea-columnname][ea-name],textarea[ea-columnname][ea-name]');
            var lcDefaultSetting = POSPrinterSettingManager.GetPrinterSetting(clPrimaryPrinterSetting.PrinterName);
            var lcVisibleOptions = (lcDefaultSetting.VisibleOptions || '').split(',');

            clControl.attr('notransition', 'true');
                        
            lcInputBoxes.each(function () {                
                var lcName = $(this).attr('ea-name');
                var lcRow = $(this).closest('[sa-elementtype=row]');
                var lcValue = clPrimaryPrinterSetting[lcName] || '';

                if (lcVisibleOptions.indexOf(lcName) == -1) lcRow.attr('fa-hidden', true);
                else lcRow.removeAttr('fa-hidden');

                $(this).attr('value', lcValue);
                $(this).val(lcValue);
            });

            POSPrinterSettingManager.SetHiddenSetting(clPrimaryPrinterSetting);

            if (paUpdateOriginalValue) POSPrinterSettingManager.UpdateOriginalValues();

            setTimeout(function () { clControl.removeAttr('notransition'); }, 1000);
        },
        SetHiddenSetting: function (paSetting)
        {
            if (paSetting) {
                var lcHiddenOptions = (paSetting.HiddenOptions || '').split(',');
                var lcHiddenConfig = {};

                $.each(lcHiddenOptions, function (paIndex, paValue) {
                    lcHiddenConfig[paValue] = paSetting[paValue];
                });
                clControl.data('POS.PrimaryPrinterSetting', lcHiddenConfig);
            }
            else clControl.data('POS.PrimaryPrinterSetting', {});
        },
        PopulateNewSetting: function (paCurrentSetting, paNewSetting)
        {
            var lcInputBoxes = clControl.find('input[type=text][ea-columnname][ea-name],textarea[ea-columnname][ea-name]');
            var lcVisibleOptions = (paNewSetting.VisibleOptions || '').split(',');
            
            clControl.attr('notransition', 'true');
            
            lcInputBoxes.each(function () {
                var lcRow = $(this).closest('[sa-elementtype=row]');
                var lcName = $(this).attr('ea-name');
                var lcReadOnly = $(this).attr('readonly') || '';

                if (lcVisibleOptions.indexOf(lcName) == -1) lcRow.attr('fa-hidden', true);
                else lcRow.removeAttr('fa-hidden');                

                if (lcReadOnly.length > 0)
                {
                    if (paNewSetting[lcName]) {
                        $(this).val(paNewSetting[lcName]);
                    }
                }
                else
                if ((!paCurrentSetting) || (!paCurrentSetting[lcName]) || (paCurrentSetting[lcName].trim() == $(this).val().trim()))
                {                    
                    if (paNewSetting[lcName])
                    {                 
                        $(this).val(paNewSetting[lcName]);
                    }
                }                                
            });
            POSPrinterSettingManager.SetHiddenSetting(paNewSetting);
            setTimeout(function () { clControl.removeAttr('notransition'); }, 1000);
        },
        GetPrinterSetting : function(paPrinterName)
        {
            return (clPrinterList[paPrinterName] || {});
        },
        SetPrinterSetting : function(paPrinterName)
        {
            var lcCurrentSetting = clPrinterList[clPrinterNameTextBox.val()];
            var lcNewSetting = clPrinterList[paPrinterName];

            if (clPrinterNameTextBox.val().trim() != paPrinterName.trim())
            {                
                POSPrinterSettingManager.PopulateNewSetting(lcCurrentSetting, lcNewSetting);                
            }            
        },
        ResetPrinterSetting : function()
        {
            var lcCurrentSetting = clPrinterList[clPrinterNameTextBox.val()];  
            POSPrinterSettingManager.PopulateNewSetting(null, lcCurrentSetting);                                        
        },
        GetControlValue: function (paElement) {
            if (paElement) {
                if (paElement.is('input[type=text],textarea')) {
                    var lcInputMode = paElement.attr('ea-inputmode');

                    if (lcInputMode == 'number') return (CastInteger(FormManager.NormalizeNumber(paElement.val()), paElement.attr('ea-originalvalue') || 0));
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

                var lcControlValue = POSPrinterSettingManager.GetControlValue($(this)) || '';
                var lcOriginalValue = $(this).attr('ea-originalvalue') || '';                
                lcValueChanged = ((lcControlValue || '').toString().trim()) != lcOriginalValue.trim();

                if (lcValueChanged == true) return (false);
            });

            return (lcValueChanged);
        },
        CompileSettingKey: function (paColumnName, paMultiDataMode, paObjectMode) {
            if (paColumnName) {
                var lcSettingData = {};
                var lcControlList = clControl.find('[ea-columnname="' + paColumnName + '"]');
                var lcValue;
                
                if (paMultiDataMode) {
                    var lcValueObject = clControl.data(paColumnName) || {};
                    
                    lcControlList.each(function () {
                        var lcName = $(this).attr('ea-name');
                        var lcResult = POSPrinterSettingManager.GetControlValue($(this));

                        if (lcResult != null) {
                            lcValueObject[lcName] = lcResult;
                        }
                    });

                    if (paObjectMode) lcValue = lcValueObject;
                    else lcValue = JSON.stringify(lcValueObject);
                }
                else lcValue = POSPrinterSettingManager.GetControlValue(lcControlList);

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
            var lcColumnNameList = POSPrinterSettingManager.GetColumnNameList();

            for (var lcColumn in lcColumnNameList) {
                if (POSPrinterSettingManager.IsControlValueChanged(lcColumn))
                    lcDataBlock = $.extend(lcDataBlock, POSPrinterSettingManager.CompileSettingKey(lcColumn, lcColumnNameList[lcColumn]));
            }           
            return (lcDataBlock);
        },
        GetInterimPrinterSetting : function()
        {
            var lcNewSetting = $.extend({}, clPrimaryPrinterSetting);            
            var lcRows = clContentContainer.find('.Row');

            lcNewSetting = $.extend(lcNewSetting, (clControl.data('POS.PrimaryPrinterSetting') || {}));
            
            lcRows.each(function () {                                
                var lcInputBox = $(this).find('input[type=text]');
                var lcColumnName = lcInputBox.attr('ea-name');                
                lcNewSetting[lcColumnName] = lcInputBox.val();                
            });
            return (lcNewSetting);
        },
        TestPrinter : function()
        {            
            POSReceiptPrintingManager.ConnectPrinter(POSPrinterSettingManager.GetInterimPrinterSetting(), true);            
        },
        ShowPrinterError : function(paErrorCode, paErrorParam)
        {
            if (paErrorCode) {
                
                MessageHandler.ShowMessage(paErrorCode, function (paOption) {
                    if (paErrorParam) {
                        paOption.message = "<br/>" + paErrorParam;
                    }
                });
            }
        },

        PrintTestPage : function()
        {
            MessageHandler.ShowMessage('confirm_printtestpage').done(function (paResult) {
                if (paResult == 'yes')
                {
                    var lcTestPrintTemplate = JSON.parse(Base64.decode(clControl.attr('ea-template') || 'e30='));
                    POSReceiptPrintingManager.PrinteJSONReceipt(lcTestPrintTemplate);
                }
            });
        },
        UpdateAppSetting: function () {
            var lcMasterBlock = {};
            var lcSettingData = POSPrinterSettingManager.GetSettingData();

            if (!jQuery.isEmptyObject(lcSettingData)) {
                lcMasterBlock['settingdata'] = Base64.encode(JSON.stringify(lcSettingData));
                var lcAjaxRequestManager = new AjaxRequestManager('executescalarquery', null, 'err_failupdate', 'ajax_updating');

                lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.updatesetting');
                lcAjaxRequestManager.AddObjectDataBlock('DataBlock', lcMasterBlock, true);

                lcAjaxRequestManager.SetCompleteHandler(function (paSuccess) {
                    if (paSuccess) {
                        POSPrinterSettingManager.UpdateOriginalValues();
                    }
                });

                lcAjaxRequestManager.Execute();
            }
         //   else FormManager.CloseForm();
        },
        UpdateOriginalValues: function () {
            var lcControlList = clControl.find('[ea-columnname][ea-name]');
            
            lcControlList.each(function () {
                var lcElement = $(this);

                if (lcElement.is('input[type=text],textarea')) {
                    var lcInputMode = lcElement.attr('ea-inputmode');

                    if ((lcInputMode == 'number') || (lcInputMode == 'decimal')) lcElement.attr('ea-originalvalue', FormManager.NormalizeNumber(lcElement.val()));
                    else lcElement.attr('ea-originalvalue', lcElement.val());
                }                
                else lcElement.attr('ea-originalvalue', lcElement.attr('value'));
            });
        },       
        HandlerOnSelectionPanelEvent: function (paEvent, paEventInfo)
        {           
            if (paEventInfo)
            {
                switch (paEventInfo.event)
                {
                    case 'openpopup'        : break;

                    case 'closepopup':
                        {
                            clPrinterNameTextBox.focus();
                            break;
                        }

                    case 'selectionchoosed' :
                        {
                            if (paEventInfo.selectedvalue)
                            {
                                POSPrinterSettingManager.SetPrinterSetting(paEventInfo.selectedvalue);                                
                            }
                        }
                        break;
                }
            }
        },

        HandlerOnPrinterNotification : function(paEvent, paEventInfo)
        {            
            if (paEventInfo)
            {
                switch (paEventInfo.event)
                {
                    case 'connecting' :
                    {         
                        clControl.attr('fa-connecting','true');
                        break;
                    }

                    case 'initfail':
                    {
                        clControl.removeAttr('fa-connecting');                        
                        POSPrinterSettingManager.ShowPrinterError(paEventInfo.errorcode, paEventInfo.errorparam);
                        break;
                    }
                    case 'initsuccess':
                        {
                            clControl.removeAttr('fa-connecting');
                            POSPrinterSettingManager.PrintTestPage();
                            break;
                        }
                }                
            }
            
        },

        HandlerOnClick : function(paEvent)
        {            
            paEvent.preventDefault();

            var lcCommand = $(this).attr('ea-command');
            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);
            
            switch(lcCommand)
            {
                case 'printerlist':
                {                    
                    clPrinterSelectionController.OpenPopUp(clPrinterNameTextBox.val().trim());
                    break;
                }

                case 'resetsetting':
                {
                    POSPrinterSettingManager.ResetPrinterSetting();
                    break;
                }

                case 'revertsetting':
                {
                    POSPrinterSettingManager.RetreiveConfig(false);
                    break;
                }

                case 'testconnection':
                {                    
                   POSPrinterSettingManager.TestPrinter();
                   break;
                }

                case 'savesetting':
                {
                    POSPrinterSettingManager.UpdateAppSetting();
                    break;
                }
            }
        }        
    }
})();
