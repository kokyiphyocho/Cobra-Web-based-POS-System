$(document).ready(function () {
    POSReceiptCustomizationSettingManager.Init();
});

var POSReceiptCustomizationSettingManager = (function () {

    var clControl;
    var clForm;
    var clToolBar;
    var clLogoImage;
    var clImageProcessorPanel;
    var clContentContainer;
    var clImageProcessingController;
    var clExternalComponentContainer;            
    var clCustomizationConfig;

    return {
        Init: function () {
            clControl = $('[sa-elementtype=control].WidControlPOSReceiptCustomizationSetting');
            clForm = clControl.closest('[sa-elementtype=form]');
            clToolBar = clForm.find("[sa-elementtype=toolbar].ToolBar");            
            clPrinterStatusControl = clForm.find('.FormTitleBar div[sa-elementtype=statuscontrol]');
            clLogoImage = clControl.find('[sa-elementtype=inputrow][ea-type=imagerow] div[ea-type=logoimage] img');
            

            clContentContainer = clControl.find('[sa-elementtype=container][ea-type=content].ContentContainer');
            clExternalComponentContainer = clForm.find('[sa-elementtype=container][ea-type=externalcomponent]');
                       
            clCustomizationConfig = JSON.parse(Base64.decode(clControl.attr('pos.receiptlayoutinfo.customization') || 'e30='));
            

            POSReceiptCustomizationSettingManager.LoadExternalComponents().done(function (paSuccess) {
                if (paSuccess) {
                    POSReceiptCustomizationSettingManager.WaitForDependencies().done(function () {
                        clImageProcessorPanel = clExternalComponentContainer.find('[sa-elementtype=composite][ea-type=imageuploader]');
                        
                        clImageProcessingController = new ImageProcessingController(clImageProcessorPanel, clControl);
                        clImageProcessingController.Init();

                        POSReceiptCustomizationSettingManager.RetreiveConfig();
                        POSReceiptCustomizationSettingManager.BindEvents();

                        POSReceiptPrintingManager.Init(clControl);
                        POSReceiptPrintingManager.ConnectPrinter();
                    });
                }
            });
        },
        WaitForDependencies: function () {
            var lcDeferred = $.Deferred();

            var lcWaitTimer = setInterval(function () {
                if ((typeof POSReceiptPrintingManager !== 'undefined') || (typeof ImageProcessingController !== 'undefined')) {
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
            clControl.find('[ea-command^="@cmd%"]').click(POSReceiptCustomizationSettingManager.HandlerOnClick);

            clToolBar.find('[ea-command^="@cmd%"]').unbind('click');
            clToolBar.find('[ea-command^="@cmd%"]').click(POSReceiptCustomizationSettingManager.HandlerOnClick);

            clPrinterStatusControl.unbind('click');
            clPrinterStatusControl.click(POSReceiptCustomizationSettingManager.HandlerOnClick);

            clControl.unbind('ev-printernotification');
            clControl.bind('ev-printernotification', POSReceiptCustomizationSettingManager.HandlerOnPrinterNotification);

            clControl.unbind('ev-imagepanelevent');
            clControl.bind('ev-imagepanelevent', POSReceiptCustomizationSettingManager.HandlerOnImagePanelEvent);

            clLogoImage.unbind('error');
            clLogoImage.bind('error', function () { $(this).closest('[sa-elementtype=panel]').attr('value', 'false'); $(this).removeAttr('src'); });
        },
        SetImage : function(paImageControl, paImageLink)
        {
            if (paImageControl)
            {
                var lcImagePanel = paImageControl.closest('[sa-elementtype=panel]');

                if (paImageLink)
                {                  
                    paImageControl.attr('src', paImageLink);                    
                }
                else 
                {
                    paImageControl.removeAttr('src');                                     
                }
            }            
        },
        RetreiveConfig: function () {            
            var lcInputBoxes = clControl.find('input[type=text][ea-columnname][ea-name],textarea[ea-columnname][ea-name]');
            var lcImageBoxes = clControl.find('img[ea-columnname][ea-name]');
            var lcDivBoxes   = clControl.find('div[ea-columnname][ea-name]');

            clControl.attr('notransition', 'true');

            lcDivBoxes.each(function () {
                var lcValue = clCustomizationConfig[$(this).attr('ea-name')] || '';
                $(this).attr('value', lcValue);
            });

            lcImageBoxes.each(function () {
                var lcImageFile = clCustomizationConfig[$(this).attr('ea-name')] || '';                                

                if (lcImageFile) {
                    var lcFullImageFileName = ($(this).attr('ea-path') || '') + '/' + lcImageFile + "?t=" + moment().format('YYYYMMDDHHmmss');
                    POSReceiptCustomizationSettingManager.SetImage(clLogoImage, lcFullImageFileName)
                }
                else
                {
                    POSReceiptCustomizationSettingManager.SetImage(clLogoImage);
                }
            });

            lcInputBoxes.each(function () {
                var lcValue = clCustomizationConfig[$(this).attr('ea-name')] || '';
                $(this).attr('value', lcValue);
                if ($(this).attr('ea-inputmode') == 'number')
                    $(this).val(FormManager.ConvertToFormLanguage(lcValue));
                else
                    $(this).val(lcValue);
            });

            POSReceiptCustomizationSettingManager.UpdateOriginalValues();

            setTimeout(function () { clControl.removeAttr('notransition'); }, 1000);
        },      
        UpdateAppSetting: function () {
            var lcMasterBlock = {};
            var lcSettingData = POSReceiptCustomizationSettingManager.GetSettingData();
            var lcFileChanged = (clLogoImage.data().blob) && (clLogoImage.closest('[sa-elementtype=panel]').attr('value') == 'true');

            if ((!jQuery.isEmptyObject(lcSettingData)) || lcFileChanged) {
                lcMasterBlock['settingdata'] = Base64.encode(JSON.stringify(lcSettingData));
                var lcAjaxRequestManager = new AjaxRequestManager('executescalarquery', null, 'err_failupdate', 'ajax_updating');
                lcAjaxRequestManager.SwitchFormDataMode();                
                lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.updatesetting');
                lcAjaxRequestManager.AddObjectDataBlock('DataBlock', lcMasterBlock, true);
                                
                if (lcFileChanged)
                {                    
                    lcAjaxRequestManager.AddAjaxParam("UploadFile", clLogoImage.data().blob, clLogoImage.attr('value'));
                }

                lcAjaxRequestManager.SetCompleteHandler(function (paSuccess) {
                    if (paSuccess) {
                        POSReceiptCustomizationSettingManager.UpdateOriginalValues();
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

                    if (lcInputMode == 'number') return (CastInteger(FormManager.NormalizeNumber(paElement.val()), paElement.attr('ea-originalvalue') || 0));
                    else if (lcInputMode == 'decimal') return (CastDecimal(FormManager.NormalizeNumber(paElement.val()), paElement.attr('ea-originalvalue') || 0));
                    else return (paElement.val());
                }                
                else return (paElement.attr('value'));
            }
            return (null);
        },
        IsControlValueChanged: function (paColumnName) {

            var lcControlList = clControl.find('[ea-columnname="' + paColumnName + '"]');
            var lcValueChanged = false;

            lcControlList.each(function () {

                var lcControlValue = POSReceiptCustomizationSettingManager.GetControlValue($(this)) || '';
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
                        var lcResult = POSReceiptCustomizationSettingManager.GetControlValue($(this));

                        if (lcResult != null) {
                            lcValueObject[lcName] = lcResult;
                        }
                    });

                    if (paObjectMode) lcValue = lcValueObject;
                    else lcValue = JSON.stringify(lcValueObject);
                }
                else lcValue = POSReceiptCustomizationSettingManager.GetControlValue(lcControlList);

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
            var lcColumnNameList = POSReceiptCustomizationSettingManager.GetColumnNameList();

            for (var lcColumn in lcColumnNameList) {
                if (POSReceiptCustomizationSettingManager.IsControlValueChanged(lcColumn))
                    lcDataBlock = $.extend(lcDataBlock, POSReceiptCustomizationSettingManager.CompileSettingKey(lcColumn, lcColumnNameList[lcColumn]));
            }
            return (lcDataBlock);
        },
        SetReceiptChangesForPrinting: function () {
            var lcDataBlock = {};
            var lcColumnNameList = POSReceiptCustomizationSettingManager.GetColumnNameList();

            for (var lcColumn in lcColumnNameList) {
                lcDataBlock = $.extend(lcDataBlock, POSReceiptCustomizationSettingManager.CompileSettingKey(lcColumn, lcColumnNameList[lcColumn], true));
            }

            //POSReceiptPrintingManager.SetReceiptWidth(lcDataBlock['POS.ReceiptLayoutInfo.Layout'].Width);
            //POSReceiptPrintingManager.SetReceiptLayoutParameter('LocalNumberMode', lcDataBlock['POS.ReceiptLayoutInfo.Layout'].LocalNumberMode == 'true' ? true : false);
            //POSReceiptPrintingManager.SetReceiptCustomizationParameter('BusinessName', lcDataBlock['POS.ReceiptLayoutInfo.Customization'].BusinessName);
            //POSReceiptPrintingManager.SetReceiptCustomizationParameter('Address', (lcDataBlock['POS.ReceiptLayoutInfo.Customization'].Address || '').split('\n'));
            //POSReceiptPrintingManager.SetReceiptCustomizationParameter('FootNote', (lcDataBlock['POS.ReceiptLayoutInfo.Customization'].FootNote || '').split('\n'));
            //PrinterManager.SetPrinterDarkness(lcDataBlock['POS.PrimaryPrinterSetting'].Darkness);
        },
        PrintTestPage: function () {
            var lcTestPrintTemplate = JSON.parse(Base64.decode(clControl.attr('ea-template') || 'e30='));

            POSReceiptCustomizationSettingManager.SetReceiptChangesForPrinting();
            POSReceiptPrintingManager.PrinteJSONReceipt(lcTestPrintTemplate);
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
                case 'none':
                case 'success':
                case 'connecting':
                    {
                        if (paErrorCode == 'none') paErrorCode = 'success';
                        POSReceiptCustomizationSettingManager.SetPrinterStatusControlAttribute(paErrorCode, '');
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
                        POSReceiptCustomizationSettingManager.SetPrinterStatusControlAttribute('error', paErrorCode, paErrorParam);
                        break;
                    }

                case 'err_printer_autocuttererror':
                case 'err_printer_receiptnearend':
                    {
                        POSReceiptCustomizationSettingManager.SetPrinterStatusControlAttribute('warning', paErrorCode, paErrorParam);
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
        HandlerOnImagePanelEvent : function(paEventArgs, paEventInfo)
        {
            if (paEventInfo)
            {
                switch(paEventInfo.event)
                {
                    case 'setimage':
                        {
                            var lcImagePanel = clLogoImage.closest('[sa-elementtype=panel]');
                            lcImagePanel.attr('value', 'true');                            

                            clLogoImage.data("blob", paEventInfo.blob);
                            clLogoImage.attr('src', paEventInfo.dataurl);
                                                        
                            break;
                        }
                }
            }            
        },
        HandlerOnPrinterNotification: function (paEventArg, paNotificationInfo) {

            if (paNotificationInfo) {
                switch (paNotificationInfo.event) {
                    case 'connecting':
                        {
                            POSReceiptCustomizationSettingManager.SetPrinterStatus('connecting');
                        }
                    case 'initfail':
                        {
                            POSReceiptCustomizationSettingManager.SetPrinterStatus(paNotificationInfo.errorcode, paNotificationInfo.errorparam);
                            break;
                        }

                    case 'initsuccess':
                        {
                            POSReceiptCustomizationSettingManager.SetPrinterStatus('success');
                            break;
                        }

                    case 'printfail':
                        {
                            Messagehandler.ShowMessage(paNotificationInfo.errorcode);
                            break;
                        }

                    case 'statuschanged':
                        {
                            POSReceiptCustomizationSettingManager.SetPrinterStatus(paNotificationInfo.errorcode, paNotificationInfo.errorparam);
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
                case 'openimagepopup':
                    {
                        clImageProcessingController.OpenPopUp();
                        break;
                    }

                case 'suppressimage':
                    {
                        var lcImage         = (clLogoImage.attr('src') || '');
                        var lcImagePanel    = clLogoImage.closest('[sa-elementtype=panel]');

                        if (lcImage)
                        {
                            if (lcImagePanel.attr('value') === 'false') {
                                lcImagePanel.attr('value','true');                                
                            }
                            else
                            {
                                lcImagePanel.attr('value', 'false');
                            }
                        }
                        
                        break;
                    }
                case 'savesetting':
                    {
                        POSReceiptCustomizationSettingManager.UpdateAppSetting()
                        break;
                    }  

                case 'printtestpage':
                    {
                        POSReceiptCustomizationSettingManager.PrintTestPage();
                        break;
                    }

                case 'printerstatus':
                    {
                        POSReceiptCustomizationSettingManager.ShowPrinterStatus();
                        break;
                    }
            }

        }
    }
})();
