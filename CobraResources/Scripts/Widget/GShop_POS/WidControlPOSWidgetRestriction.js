$(document).ready(function () {
    POSWidgetRestrictionManager.Init();
});

var POSWidgetRestrictionManager = (function () {

    var clControl;
    var clForm;
    var clPopUpOverlay;
    var clPopUpTitle;
    var clPopUpTitleText;
    var clButtonPanel;
    var clWidgetRows;

    return {
        Init: function () {
            clControl           = $('[sa-elementtype=control].WidControlPOSWidgetRestriction');
            clForm              = clControl.closest('[sa-elementtype=form]');
            clPopUpOverlay      = clForm.find('[sa-elementtype=overlay][ea-type=imagepopup]');
            clPopUpTitle        = clPopUpOverlay.find('[sa-elementtype=popup] [sa-elementtype=title]');
            clPopUpTitleText    = clPopUpTitle.find('span');
            clButtonPanel       = clControl.find('.ButtonPanel');            
            clWidgetRows        = clPopUpOverlay.find('[sa-elementtype=popup] [sa-elementtype=list] [sa-elementtype=row]:not([ea-type="#admin"])');
            
            POSWidgetRestrictionManager.BindEvents();
        },
        BindEvents: function () {
            clControl.find('[ea-command^="@cmd%"]').unbind('click');
            clControl.find('[ea-command^="@cmd%"]').click(POSWidgetRestrictionManager.HandlerOnClick);

            clPopUpOverlay.find('[ea-command^="@cmd%"]').unbind('click');
            clPopUpOverlay.find('[ea-command^="@cmd%"]').click(POSWidgetRestrictionManager.HandlerOnClick);
        },
        UpdateSetting: function () {            
            var lcMasterBlock = {};
            var lcSettingData = POSWidgetRestrictionManager.GetSettingData();
            var lcKeyColumnName = clPopUpOverlay.attr('ea-keycolumnname');
            var lcUserRow = clControl.find('[ea-keycolumnname="' + lcKeyColumnName + '"]');            

            if (!jQuery.isEmptyObject(lcSettingData)) {
                lcMasterBlock['settingdata'] = Base64.encode(JSON.stringify(lcSettingData));
                var lcAjaxRequestManager = new AjaxRequestManager('executescalarquery', null, 'err_failupdate', 'ajax_updating');

                lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.updatesetting');
                lcAjaxRequestManager.AddObjectDataBlock('DataBlock', lcMasterBlock, true);

                lcAjaxRequestManager.SetCompleteHandler(function (paSuccess) {
                    if (paSuccess) {
                        clForm.removeAttr('fa-showpopup');
                        lcUserRow.attr('ea-keyvalue', lcSettingData[lcKeyColumnName]);
                    }
                });

                lcAjaxRequestManager.Execute();
            }
            else FormManager.CloseForm();
        },        
        GetSettingData: function () {
            var lcSettingData           = {}
            var lcRestrictedWidgets     = '';
            var lcKeyColumnName         = clPopUpOverlay.attr('ea-keycolumnname');            

            clWidgetRows.each(function () {
                if ($(this).attr('value') == 'false') {
                    lcRestrictedWidgets += (lcRestrictedWidgets.length > 0 ? ',' : '') + '[' + $(this).attr('ea-name') + ']';
                }                
            });
            
            lcSettingData[lcKeyColumnName] = lcRestrictedWidgets;
            
            return (lcSettingData);            
        },
        InitializePopUp : function(paControl)
        {            
            var lcLoginID               = paControl.find('span').text();
            var lcTitleTemplate         = clPopUpTitle.attr('ea-template');
            var lcRestrictedWidgets     = (paControl.attr('ea-keyvalue') || '');
            var lcKeyColumnName         = paControl.attr('ea-keycolumnname');

            clPopUpTitleText.text(lcTitleTemplate.replace('$LOGINID', lcLoginID));
            clPopUpOverlay.attr('ea-keycolumnname', lcKeyColumnName);

            clWidgetRows.each(function () {
                if ((lcRestrictedWidgets.indexOf($(this).attr('ea-name'))) == -1) {
                    $(this).attr('value', 'true');
                }
                else $(this).attr('value', 'false');
            });
            
        },        
        ActionOnToggle: function (paControl) {
            if (paControl) {                
                var lcRow = paControl.closest('[sa-elementtype=row]');
                var lcType = lcRow.attr('ea-type');

                if (lcType != '#admin') {
                    if (lcRow.attr('value') == 'true') lcRow.attr('value', 'false');
                    else (lcRow.attr('value', 'true'));
                }
            }
        },
        ActionOnShowPopUp: function (paControl) {            
            var lcUserID = paControl.attr('ea-dataid');

            POSWidgetRestrictionManager.InitializePopUp(paControl);            
            clForm.attr('fa-showpopup', lcUserID);
        },
        HandlerOnClick: function (paEvent) {
            paEvent.preventDefault();

            var lcCommand = $(this).attr('ea-command');
            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);

            switch (lcCommand) {
                case 'popup.toggle':
                    {                        
                        POSWidgetRestrictionManager.ActionOnToggle($(this));
                        break;
                    }

                case 'popup.update':
                    {
                        POSWidgetRestrictionManager.UpdateSetting()
                        break;
                    }

                case 'close':
                    {
                        FormManager.CloseForm();
                        break;
                    }
                case 'showpopup':
                    {
                        POSWidgetRestrictionManager.ActionOnShowPopUp($(this));
                        break;
                    }

                case 'popup.close':
                case 'popup.cancel':
                    {
                        clForm.removeAttr('fa-showpopup');
                        break;
                    }
            }

        }
    }
})();
