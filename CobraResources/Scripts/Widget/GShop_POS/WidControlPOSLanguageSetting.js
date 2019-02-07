$(document).ready(function () {
    POSLanguageSettingManager.Init();
});

var POSLanguageSettingManager = (function () {

    var clControl;
    var clForm;
    var clToolBar;
    var clLanguageBlock;
    var clApplicationTitleBlock;

    return {
        Init: function () {
            clControl = $('[sa-elementtype=control].WidControlPOSLanguageSetting');
            clForm = clControl.closest('[sa-elementtype=form]');
            clToolBar = clForm.find("[sa-elementtype=toolbar].ToolBar");
            clLanguageBlock = clControl.find('[sa-elementtype=block][ea-type=language]');
            clApplicationTitleBlock = clControl.find('[sa-elementtype=block][ea-type=applicationtitle]');
            
            POSLanguageSettingManager.SetLanguage(clLanguageBlock.attr('value'));

            POSLanguageSettingManager.BindEvents();
        },
        BindEvents: function () {
            clControl.find('[ea-command^="@cmd%"]').unbind('click');
            clControl.find('[ea-command^="@cmd%"]').click(POSLanguageSettingManager.HandlerOnClick);

            clToolBar.find('[ea-command^="@cmd%"]').unbind('click');
            clToolBar.find('[ea-command^="@cmd%"]').click(POSLanguageSettingManager.HandlerOnClick);            
        },
        UpdateAppSetting: function () {
            var lcMasterBlock = {};
            var lcSettingData = POSLanguageSettingManager.GetSettingData();

            if (!jQuery.isEmptyObject(lcSettingData)) {
                lcMasterBlock['settingdata'] = Base64.encode(JSON.stringify(lcSettingData));
                var lcAjaxRequestManager = new AjaxRequestManager('executescalarquery', null, 'err_failupdate', 'ajax_updating');

                lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.updatesetting');
                lcAjaxRequestManager.AddObjectDataBlock('DataBlock', lcMasterBlock, true);

                lcAjaxRequestManager.SetCompleteHandler(function (paSuccess) {
                    if (paSuccess) {
                        POSLanguageSettingManager.UpdateOriginalValues();
                    }
                });

                lcAjaxRequestManager.Execute();
            }
        //    else FormManager.CloseForm();
        },
        SetLanguage: function (paLanguage) {            
            var lcLanguageRow           = clLanguageBlock.find('[sa-elementtype=inputrow][value="' + paLanguage + '"]');
            var lcApplicationTitleRow   = clApplicationTitleBlock.find('[sa-elementtype=inputrow][value="' + paLanguage + '"]');
            
            clLanguageBlock.attr('value', paLanguage);
            lcLanguageRow.siblings().removeAttr('fa-active');
            lcLanguageRow.attr('fa-active', 'true');
            lcApplicationTitleRow.siblings().removeAttr('fa-active');
            lcApplicationTitleRow.attr('fa-active', 'true');
            
        },
        GetControlValue: function (paElement) {
            if (paElement) {
                if (paElement.is('input[type=text]')) return (paElement.val());
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

                var lcControlValue = POSLanguageSettingManager.GetControlValue($(this)) || '';
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

                if (lcElement.is('input[type=text]')) lcElement.attr('ea-originalvalue', lcElement.val())
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

                    lcControlList.each(function () {
                        var lcName      = $(this).attr('ea-name');
                        var lcDynamic    = $(this).attr('ea-dynamic') == 'true';
                        var lcOriginalValue = $(this).attr('ea-originalvalue') || '' ;
                        var lcResult = POSLanguageSettingManager.GetControlValue($(this));

                        if (lcDynamic)
                        {
                            if (lcResult && (lcOriginalValue.trim() != lcResult.trim()))
                            {
                                lcValueObject[lcName] = lcResult;
                            }
                        }
                        else
                        if (lcResult != null) {
                            lcValueObject[lcName] = lcResult;
                        }
                    });

                    lcValue = JSON.stringify(lcValueObject);
                }
                else lcValue = POSLanguageSettingManager.GetControlValue(lcControlList);

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
            var lcColumnNameList = POSLanguageSettingManager.GetColumnNameList();

            for (var lcColumn in lcColumnNameList) {
                if (POSLanguageSettingManager.IsControlValueChanged(lcColumn))
                    lcDataBlock = $.extend(lcDataBlock, POSLanguageSettingManager.CompileSettingKey(lcColumn, lcColumnNameList[lcColumn]));
            }
            
            return (lcDataBlock);
        },      
        HandlerOnClick: function (paEvent) {
            paEvent.preventDefault();

            var lcCommand = $(this).attr('ea-command');
            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);

            switch (lcCommand) {              
                case 'save':
                    {
                        POSLanguageSettingManager.UpdateAppSetting()
                        break;
                    }
                case 'language':
                    {
                        POSLanguageSettingManager.SetLanguage($(this).attr('value'));
                        break;
                    }
            }

        }
    }
})();
