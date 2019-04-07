$(document).ready(function () {
    POSGeneralSettingManager.Init();
});

var POSGeneralSettingManager = (function () {

    var clControl;
    var clForm;
    var clToolBar;        
    var clRegionalConfig;
    var clAppManifestConfig;
    var clTimeZoneList;
    var clExternalComponentContainer;
    var clImageListPopUp;    
    var clTimeZoneListPopUp;
    var clDateFormatListPopUp;    
    var clTimeZoneListController;
    var clDateFormatListController;
    var clInstallerSettingBlock;
    var clRegionalSettingBlock;


    return {
        Init: function () {
            clControl                       = $('[sa-elementtype=control].WidControlPOSGeneralSetting');
            clForm                          = FormManager.GetForm();
            clToolBar                       = ToolBarManager.GetToolBar();
            clRegionalConfig                = JSON.parse(Base64.decode(clControl.attr('_REGIONALCONFIG') || 'e30='));
            clAppManifestConfig             = JSON.parse(Base64.decode(clControl.attr('[appmanifest]') || 'e30='));
            clTimeZoneList                  = JSON.parse(Base64.decode(clControl.attr('ea-optionlist') || 'e30='));
            clExternalComponentContainer    = $('[sa-elementtype=container][ea-type=externalcomponent]');
            clInstallerSettingBlock         = clControl.find('[sa-elementtype=block][ea-type=installersetting]');
            clRegionalSettingBlock          = clControl.find('[sa-elementtype=block][ea-type=regionalsetting]');
            
            POSGeneralSettingManager.LoadExternalComponents().done(function (paSuccess) {
                if (paSuccess) {
                    POSGeneralSettingManager.WaitForDependencies().done(function () {

                        clImageListPopUp    = clExternalComponentContainer.find('[sa-elementtype=overlay][ea-type=imagepopup]');

                        clTimeZoneListPopUp = clExternalComponentContainer.find('[sa-elementtype=popup][ea-type=timezonelist]');
                        clTimeZoneListController = new SelectionPanelController(clTimeZoneListPopUp, clControl);
                        clTimeZoneListController.Init();

                        clDateFormatListPopUp = clExternalComponentContainer.find('[sa-elementtype=popup][ea-type=dateformatlist]');
                        clDateFormatListController = new SelectionPanelController(clDateFormatListPopUp, clControl);
                        clDateFormatListController.Init();
                        
                        POSGeneralSettingManager.BindEvents();
                        POSGeneralSettingManager.RetreiveConfig();
                    });
                }
            });
        },
        WaitForDependencies: function () {
            var lcDeferred = $.Deferred();

            var lcWaitTimer = setInterval(function () {
                if (typeof SelectionPanelController !== 'undefined') {
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
            clControl.find('[ea-command^="@cmd%"]').unbind('click');
            clControl.find('[ea-command^="@cmd%"]').click(POSGeneralSettingManager.HandlerOnClick);

            clToolBar.find('[ea-command^="@cmd%"]').unbind('click');
            clToolBar.find('[ea-command^="@cmd%"]').click(POSGeneralSettingManager.HandlerOnClick);
            
            clImageListPopUp.find('[ea-command^="@cmd%"]').unbind('click');
            clImageListPopUp.find('[ea-command^="@cmd%"]').click(POSGeneralSettingManager.HandlerOnClick);

            clControl.unbind('ev-selectionpanelevent');
            clControl.bind('ev-selectionpanelevent', POSGeneralSettingManager.HandlerOnSelectionPanelEvent);

            clRegionalSettingBlock.find('[ea-columnname][ea-name].SlideSelectionContainer').unbind('ev-valuechanged');
            clRegionalSettingBlock.find('[ea-columnname][ea-name].SlideSelectionContainer').bind('ev-valuechanged', POSGeneralSettingManager.HandlerOnValueChanged);
        },
        RetreiveConfig: function () {            
            var lcToggleButtons     = clRegionalSettingBlock.find('[ea-columnname][ea-name][sa-elementtype=button]');
            var lcInputBoxes        = clControl.find('input[type=text][ea-columnname][ea-name]');
            var lcSlideSelections   = clRegionalSettingBlock.find('[ea-columnname][ea-name].SlideSelectionContainer');
            var lcImageBoxes        = clInstallerSettingBlock.find('img[ea-columnname][ea-name]');

            clControl.attr('notransition', 'true');

            lcToggleButtons.each(function () {
                var lcValue = (clRegionalConfig[$(this).attr('ea-name')] || 'false').toLowerCase();
                $(this).attr('value', lcValue == 'true' ? 'true' : 'false');
            });

            lcInputBoxes.each(function () {
                var lcValue = clRegionalConfig[$(this).attr('ea-name')] || clAppManifestConfig[$(this).attr('ea-name')] || '';
                $(this).attr('value', lcValue);
                if ($(this).attr('ea-inputmode') == 'number')
                    $(this).val(FormManager.ConvertToFormLanguage(lcValue));
                else
                    $(this).val(lcValue);
            });

            lcSlideSelections.each(function () {
                var lcValue = clRegionalConfig[$(this).attr('ea-name')] || clAppManifestConfig[$(this).attr('ea-name')] || '';
                $(this).attr('value', lcValue);
                $(this).trigger('ev-valuechanged', [$(this)]);
            });

            lcImageBoxes.each(function () {
                var lcValue = clRegionalConfig[$(this).attr('ea-name')] || clAppManifestConfig[$(this).attr('ea-name')] || '';                
                $(this).attr('src', lcValue);
            });

            POSGeneralSettingManager.UpdateOriginalValues();

            setTimeout(function () { clControl.removeAttr('notransition'); }, 1000);
        },
        UpdateAppSetting: function () {
            var lcMasterBlock = {};
            var lcSettingData = POSGeneralSettingManager.GetSettingData();

            if (!jQuery.isEmptyObject(lcSettingData)) {
                lcMasterBlock['settingdata'] = Base64.encode(JSON.stringify(lcSettingData));
                var lcAjaxRequestManager = new AjaxRequestManager('executescalarquery', null, 'err_failupdate', 'ajax_updating');

                lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.updatesetting');
                lcAjaxRequestManager.AddObjectDataBlock('DataBlock', lcMasterBlock, true);

                lcAjaxRequestManager.SetCompleteHandler(function (paSuccess) {
                    if (paSuccess) {
                        POSGeneralSettingManager.UpdateOriginalValues();
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

                var lcControlValue = POSGeneralSettingManager.GetControlValue($(this)) || '';
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
        CompileSettingKey: function (paColumnName, paMultiDataMode) {
            if (paColumnName) {
                var lcSettingData = {};
                var lcControlList = clControl.find('[ea-columnname="' + paColumnName + '"]');
                var lcValue;

                if (paMultiDataMode) {
                    var lcValueObject = JSON.parse(Base64.decode(clControl.attr(paColumnName) || clControl.attr(paColumnName.toLowerCase()) || 'e30='));;

                    lcControlList.each(function () {
                        var lcName = $(this).attr('ea-name');
                        var lcResult = POSGeneralSettingManager.GetControlValue($(this));

                        if (lcResult != null) {
                            lcValueObject[lcName] = lcResult;
                        }
                    });

                    lcValue = JSON.stringify(lcValueObject);
                }
                else lcValue = POSGeneralSettingManager.GetControlValue(lcControlList);

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
            var lcColumnNameList = POSGeneralSettingManager.GetColumnNameList();

            for (var lcColumn in lcColumnNameList) {
                if (POSGeneralSettingManager.IsControlValueChanged(lcColumn))
                    lcDataBlock = $.extend(lcDataBlock, POSGeneralSettingManager.CompileSettingKey(lcColumn, lcColumnNameList[lcColumn]));
            }
            
            return (lcDataBlock);
        },
        SelectSlideSelectionValue : function(paName, paValue)
        {
            var lcSlideSelectionControl = clRegionalSettingBlock.find('[ea-columnname][ea-name="' + paName + '"].SlideSelectionContainer');

            if ((paValue) && (lcSlideSelectionControl))
            {
                lcSlideSelectionControl.attr('value', paValue);
                lcSlideSelectionControl.trigger('ev-valuechanged', [lcSlideSelectionControl]);
            }
        },
        SetActiveImage: function (paControl) {
            var lcImages = clImageListPopUp.find('[sa-elementtype=list].ImageListContainer img');

            lcImages.removeAttr('fa-active');
            paControl.attr('fa-active', 'true');
        },
        SelectActiveImage: function () {
            var lcName          = clImageListPopUp.attr('fa-showpopup');
            var lcImageControl  = clControl.find('[sa-elementtype=inputrow][ea-type=image] img[ea-name="' + lcName + '"]');
            var lcActiveImage   = clImageListPopUp.find('[sa-elementtype=list].ImageListContainer img[fa-active]');
            
            if ((lcName) && (lcActiveImage)) {            
                lcImageControl.attr('src', lcActiveImage.attr('src'));
            }
            clImageListPopUp.removeAttr('fa-showpopup');
        },
        ShowImagePopUp : function(paControl)
        {
            var lcImage         = paControl.find('img'); 
            var lcName          = lcImage.attr('ea-name');
            var lcImagePath     = lcImage.attr('src');                                    
            var lcActiveImage   = clImageListPopUp.find('[sa-elementtype=list].ImageListContainer img[src="' + lcImagePath + '"]');
            
            POSGeneralSettingManager.SetActiveImage(lcActiveImage);
            if (lcActiveImage.length > 0) lcActiveImage[0].scrollIntoView();
            clImageListPopUp.attr('fa-showpopup', lcName);
        },
        OpenPopUp : function(paControl)
        {             
            var lcName = paControl.attr('ea-name') || paControl.find('[ea-name]').attr('ea-name');

            switch (lcName)
            {
                case 'localtimezoneid':
                    {
                        clTimeZoneListController.OpenPopUp();                        
                        break;
                    }

                case 'dateformat':
                    {                        
                        clDateFormatListController.OpenPopUp();
                        break;
                    }

                case 'backendicon':
                    {
                        POSGeneralSettingManager.ShowImagePopUp(paControl);
                        break;
                    }
            }
        },       
        ActionOnToggle: function (paToggleSwitch) {
            if (paToggleSwitch) {
                if (paToggleSwitch.attr('value') == 'true') paToggleSwitch.attr('value', 'false');
                else (paToggleSwitch.attr('value', 'true'));
            }
        },
        HandlerOnSelectionPanelEvent: function (paEvent, paEventInfo) {
            if (paEventInfo) {
                switch (paEventInfo.event) {
                    case 'openpopup': break;

                    case 'closepopup':
                        {
                            if (paEventInfo.type)
                            {
                                if (paEventInfo.type == 'dateformatlist') clRegionalSettingBlock.find('[ea-columnname][ea-name=dateformat].SlideSelectionContainer').focus();
                                else if (paEventInfo.type == 'timezonelist') clRegionalSettingBlock.find('[ea-columnname][ea-name=localtimezoneid].SlideSelectionContainer').focus();
                            }
                            break;
                        }

                    case 'selectionchoosed':
                        {
                            if (paEventInfo.selectedvalue)
                            {
                                if (paEventInfo.type == 'dateformatlist') POSGeneralSettingManager.SelectSlideSelectionValue('dateformat', paEventInfo.selectedvalue);
                                else if (paEventInfo.type == 'timezonelist') POSGeneralSettingManager.SelectSlideSelectionValue('localtimezoneid', paEventInfo.selectedvalue);
                            }
                        }
                        break;
                }
            }
        },
        HandlerOnValueChanged : function(paEvent, paControl)
        {
            var lcName      = paControl.attr('ea-name');
            var lcValue     = paControl.attr('value');
            var lcInputBox  = paControl.find('input[type=text]');
            
            switch (lcName)
            {
                case 'localtimezoneid':
                    {
                        var lcTimeZoneOffsetBox = clRegionalSettingBlock.find('input[ea-columnname][ea-name=localtimeoffset]');

                        if (clTimeZoneList)
                        {                            
                            $.each(clTimeZoneList, function(paIndex, paTimeZoneInfo)
                            {
                                if (paTimeZoneInfo.timezoneid == lcValue)
                                {
                                    lcInputBox.val(paTimeZoneInfo.description);
                                    lcTimeZoneOffsetBox.val(paTimeZoneInfo.offset);
                                    return (false);
                                }
                            })                            
                        }
                        break;
                    }

                case 'dateformat':
                    {
                        lcInputBox.val(lcValue);
                        break;
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
                        POSGeneralSettingManager.ActionOnToggle($(this));
                        break;
                    }

                case 'save':
                    {
                        POSGeneralSettingManager.UpdateAppSetting()
                        break;
                    }

                case 'showpopup':
                    {
                        POSGeneralSettingManager.OpenPopUp($(this));
                        break;
                    }

                case 'popup.close':
                case 'popup.cancel':
                    {
                        clImageListPopUp.removeAttr('fa-showpopup');
                        break;
                    }

                case 'popup.choose':
                    {
                        POSGeneralSettingManager.SelectActiveImage();
                        break;
                    }
                case 'popup.imageclick':
                    {
                        POSGeneralSettingManager.SetActiveImage($(this));
                        break;
                    }
            }

        }
    }
})();
