$(document).ready(function () {
    POSAppearanceManager.Init();
});

var POSAppearanceManager = (function () {

    var clControl;
    var clForm;
    var clToolBar;
    var clPopUpOverlay;    

    return {
        Init: function () {
            clControl                   = $('[sa-elementtype=control].WidControlPOSAppearanceSetting');     
            clForm                      = clControl.closest('[sa-elementtype=form]');
            clToolBar                   = clForm.find("[sa-elementtype=toolbar].ToolBar");
            clPopUpOverlay              = clForm.find('[sa-elementtype=overlay][ea-type=imagepopup]');                            
                        
            POSAppearanceManager.BindEvents();            
        },
        BindEvents: function () {
            clControl.find('[ea-command^="@cmd%"]').unbind('click');
            clControl.find('[ea-command^="@cmd%"]').click(POSAppearanceManager.HandlerOnClick);

            clToolBar.find('[ea-command^="@cmd%"]').unbind('click');
            clToolBar.find('[ea-command^="@cmd%"]').click(POSAppearanceManager.HandlerOnClick);

            clPopUpOverlay.find('[ea-command^="@cmd%"]').unbind('click');
            clPopUpOverlay.find('[ea-command^="@cmd%"]').click(POSAppearanceManager.HandlerOnClick);
        },
        UpdateAppSetting: function () {
            var lcMasterBlock = {};
            var lcSettingData = POSAppearanceManager.GetSettingData();

            if (!jQuery.isEmptyObject(lcSettingData)) {
                lcMasterBlock['settingdata'] = Base64.encode(JSON.stringify(lcSettingData));
                var lcAjaxRequestManager = new AjaxRequestManager('executescalarquery', null, 'err_failupdate', 'ajax_updating');

                lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.updatesetting');
                lcAjaxRequestManager.AddObjectDataBlock('DataBlock', lcMasterBlock, true);

                lcAjaxRequestManager.SetCompleteHandler(function (paSuccess) {
                    if (paSuccess) {
                        POSAppearanceManager.UpdateOriginalValues();
                    }
                });

                lcAjaxRequestManager.Execute();            
            }
         //   else FormManager.CloseForm();
        },               
        GetControlValue : function(paElement)
        {
            if (paElement)
            {
                if (paElement.is('input[type=text]')) return(paElement.val());
                else if (paElement.is('img'))
                {                    
                    if (paElement.attr('ea-desktopbackgroundcss')) return (paElement.attr('ea-desktopbackgroundcss'));
                    else return (paElement.attr('src'));
                }
                else return (paElement.attr('value'));
            }
            return(null);
        },
        IsControlValueChanged: function (paColumnName) {

            var lcControlList = clControl.find('[ea-columnname="' + paColumnName + '"]');
            var lcValueChanged = false;

            lcControlList.each(function () {
                
                var lcControlValue  = POSAppearanceManager.GetControlValue($(this)) || '';
                var lcOriginalValue = $(this).attr('ea-originalvalue') || '';

                lcValueChanged = lcControlValue.trim() != lcOriginalValue.trim();

                if (lcValueChanged == true) return (false);
            });

            return (lcValueChanged);
        },
        UpdateOriginalValues : function()
        {
            var lcControlList = clControl.find('[ea-columnname]');

            lcControlList.each(function () {
                var lcElement = $(this);

                if (lcElement.is('input[type=text]')) lcElement.attr('ea-originalvalue',lcElement.val())
                else if (lcElement.is('img')) {
                    if (lcElement.attr('ea-desktopbackgroundcss')) lcElement.attr('ea-originalvalue', lcElement.attr('ea-desktopbackgroundcss'));
                    else lcElement.attr('ea-originalvalue', lcElement.attr('src'));
                }
                else lcElement.attr('ea-originalvalue',lcElement.attr('value'));
            });
        },
        //CompileSettingKey: function (paColumnName, paMultiDataMode)
        //{
        //    if (paColumnName)
        //    {
        //        var lcSettingData = {};
        //        var lcControlList = clControl.find('[ea-columnname="' + paColumnName + '"]');
        //        var lcValue;

        //        if (paMultiDataMode)
        //        {
        //            var lcValueObject = {};

        //            lcControlList.each(function () {
        //                var lcName = $(this).attr('ea-name');
        //                var lcResult = POSAppearanceManager.GetControlValue($(this));
                        
        //                if (lcResult != null)
        //                {
        //                    lcValueObject[lcName] = lcResult;
        //                }
        //            });
                    
        //            lcValue = JSON.stringify(lcValueObject);                    
        //        }
        //        else lcValue = POSAppearanceManager.GetControlValue(lcControlList);
                                
        //        if (lcValue != null) lcSettingData[paColumnName] = lcValue;

        //        return (lcSettingData);
        //    }
        //},
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
                        var lcResult = POSAppearanceManager.GetControlValue($(this));

                        if (lcResult != null) {
                            if (lcKeyValue) {
                                lcValueObject[lcName] = paIndex == 0 ? '' : lcValueObject[lcName];
                                if ((lcResult == 'true') && (lcValueObject[lcName].indexOf(lcKeyValue) == -1)) lcValueObject[lcName] += (lcValueObject[lcName].length == 0 ? "" : ",") + lcKeyValue;
                            }
                            else lcValueObject[lcName] = lcResult;
                        }
                    });


                    lcValue = JSON.stringify(lcValueObject);
                }
                else lcValue = POSAppearanceManager.GetControlValue(lcControlList);

                if (lcValue != null) lcSettingData[paColumnName] = lcValue;

                return (lcSettingData);
            }
        },
        GetColumnNameList : function()
        {
            var lcColumnNameList = {};
            
            clControl.find('[ea-columnname]').each(function () {
                if (!lcColumnNameList[$(this).attr('ea-columnname')])
                    lcColumnNameList[$(this).attr('ea-columnname')] = $(this).attr('ea-name') ? true : false;
            });            
            return (lcColumnNameList);
        },
        GetSettingData: function () {
            var lcDataBlock         = {};
            var lcColumnNameList    = POSAppearanceManager.GetColumnNameList();            

            for (var lcColumn in lcColumnNameList)
            {
                if (POSAppearanceManager.IsControlValueChanged(lcColumn))
                    lcDataBlock = $.extend(lcDataBlock, POSAppearanceManager.CompileSettingKey(lcColumn, lcColumnNameList[lcColumn]));
            }            
            return (lcDataBlock);
        },              
        SetActiveImage: function (paControl) {
            var lcImages = clPopUpOverlay.find('[sa-elementtype=list].ImageListContainer img');

            lcImages.removeAttr('fa-active');
            paControl.attr('fa-active', 'true');
        },
        SelectActiveImage : function()
        {
            var lcName          = clForm.attr('fa-showpopup');            
            var lcImageControl  = clControl.find('[sa-elementtype=inputrow][ea-type=image] img[ea-name="' + lcName + '"]');
            var lcActiveImage   = clPopUpOverlay.find('[sa-elementtype=list].ImageListContainer img[fa-active]');
            var lcBackgroundCSS = lcActiveImage.attr('ea-desktopbackgroundcss');

            if ((lcName) && (lcActiveImage))
            {
                if (lcBackgroundCSS) lcImageControl.attr('ea-desktopbackgroundcss', lcBackgroundCSS);
                else lcImageControl.removeAttr('ea-desktopbackgroundcss');
                lcImageControl.attr('src',lcActiveImage.attr('src'));
            }
            clForm.removeAttr('fa-showpopup');
        },
        ActionOnToggle : function(paControl) {
            if (paControl)
            {
                var lcSwitchContainer = paControl.closest(".ToggleSwitchContainer");
                

                if (paControl.attr('value') == 'true') paControl.attr('value', 'false');
                else (paControl.attr('value', 'true'));
                
                if (lcSwitchContainer.length > 0) {                                        
                    var lcOptionList = lcSwitchContainer.attr('ea-optionlist').split(',');
                    lcSwitchContainer.attr('value', paControl.attr('value'));
                    paControl.attr('ea-value', lcOptionList[(paControl.attr('value')=='true' ? 1 : 0)]);
                }
            }
        },                
        ActionOnShowPopUp : function(paControl)
        {
            var lcImage         = paControl.find('img'); 
            var lcName          = lcImage.attr('ea-name');
            var lcImagePath     = lcImage.attr('src');                        
            var lcBackgroundCSS = lcImage.attr('ea-desktopbackgroundcss');
            var lcActiveImage   = clPopUpOverlay.find('[sa-elementtype=list].ImageListContainer img[src="' + lcImagePath + '"]');

            if (lcBackgroundCSS)
                lcActiveImage = clPopUpOverlay.find('[sa-elementtype=list].ImageListContainer img[src="' + lcImagePath + '"][ea-desktopbackgroundcss=' + lcBackgroundCSS + ']');
            
            POSAppearanceManager.SetActiveImage(lcActiveImage);
            lcActiveImage[0].scrollIntoView();
            clForm.attr('fa-showpopup', lcName);            
        },
        HandlerOnFocus : function(paEvent) {
            $(this).select();
        },
        HandlerOnClick: function (paEvent) {
            paEvent.preventDefault();
            
            var lcCommand = $(this).attr('ea-command');
            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);

            switch (lcCommand) {
                case 'toggle':
                    {
                        POSAppearanceManager.ActionOnToggle($(this));
                        break;
                    }

                case 'save':
                    {                        
                        POSAppearanceManager.UpdateAppSetting()
                        break;
                    }
                case 'showpopup':
                    {
                        POSAppearanceManager.ActionOnShowPopUp($(this));                        
                        break;
                    }

                case 'popup.close':
                case 'popup.cancel':
                    {
                        clForm.removeAttr('fa-showpopup');
                        break;
                    }

                case 'popup.choose':
                    {
                        POSAppearanceManager.SelectActiveImage();
                        break;
                    }
                case 'popup.imageclick' :
                    {
                        POSAppearanceManager.SetActiveImage($(this));
                        break;
                    }
            }

        }
    }
})();
