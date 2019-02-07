$(document).ready(function () {    
    POSTableManager.Init();
});


var POSTableManager = (function () {

    var clForm;
    var clToolBar;
    var clControl;    
    var clControlMode;

    return {
        Init: function () {
            clForm          = FormManager.GetForm();
            clToolBar       = ToolBarManager.GetToolBar();
            clControl       = clForm.find('[sa-elementtype=control].WidControlPOSAddAdjustTable');
            clControlMode   = clControl.attr('ea-controlmode');

            POSTableManager.SetInputBehaviour();
            POSTableManager.BindEvents();
        },
        SetInputBehaviour: function () {
            var lcInputBlock = clControl.find('[sa-elementtype=container] [sa-elementtype=inputblock]');
            var lcNumberBoxes = lcInputBlock.find('input[type=text][ea-inputmode=number]');
            var lcDecimalBoxes = lcInputBlock.find('input[type=text][ea-inputmode=decimal]');
            var lcSignedNumberBoxes = lcInputBlock.find('input[type=text][ea-inputmode=signednumber]');

            lcNumberBoxes.ForceIntegerInput();
            lcDecimalBoxes.ForceDecimalInput();
            lcSignedNumberBoxes.ForceSignedIntegerInput();
        },
        BindEvents: function () {
            clToolBar.find('[ea-command^="@cmd%"]').unbind('click');
            clToolBar.find('[ea-command^="@cmd%"]').click(POSTableManager.HandlerOnClick);

            //clControl.find('a[ea-command="@cmd%update"]').unbind('click');
            //clControl.find('a[ea-command="@cmd%update"]').click(POSTableManager.HandlerOnClick);

            //clControl.find('a[ea-command="@cmd%close"]').unbind('click');
            //clControl.find('a[ea-command="@cmd%close"]').click(POSTableManager.HandlerOnClick);
        },        
        ShowControlError: function (paControl, paMessageCode) {
            if (paControl) {
                var lcInputRow = paControl.closest('[sa-elementtype=inputrow]');
                var lcLabel = lcInputRow.find('[sa-elementtype=inputlabel]');

                MessageHandler.ShowDynamicMessage(paMessageCode, { '$FIELD': lcLabel.text() }, paControl);
            }
        },
        VerifyInputs: function () {
            var lcSuccess = true;

            var lcMandatoryFields = clControl.find('[ea-mandatory=true]');

            lcMandatoryFields.each(function (paIndex) {

                if ($(this).val().trim() == '') {

                    POSTableManager.ShowControlError($(this), 'err_requirefieldmissing');
                    lcSuccess = false;
                    return (false);
                }
            });

            return (lcSuccess);
        },
        UpdateRecord: function () {
            var lcContainer = clControl.find('[sa-elementtype=container]');
            var lcTableIDBox = clControl.find('[ea-columnname=tableid]');
            var lcDisplayNameBox = clControl.find('[ea-columnname=displayname]');            
            var lcTableLimit = clControlMode == 'group' ? clControl.attr('gpos-systemtablegrouplimit') : clControl.attr('gpos-systemtablelimit');
            var lcInsertMode = false;

            if (Number(lcTableIDBox.val()) == -1) lcInsertMode = true;

            if (POSTableManager.VerifyInputs()) {
                var lcAjaxRequestManager = new AjaxRequestManager('executenonquery', null, 'err_failaddobject', 'ajax_updating');

                lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.updatetablerecord');
                lcAjaxRequestManager.AddAjaxParam('datablock', POSTableManager.GetSerializedData());
                lcAjaxRequestManager.AddMessagePlaceHolder('$OBJECT', lcDisplayNameBox.val());
                lcAjaxRequestManager.AddMessagePlaceHolder('$LIMIT', FormManager.ConvertToFormLanguage(lcTableLimit));

                lcAjaxRequestManager.SetCompleteHandler(function (paSuccess) {
                    if (paSuccess) {
                        if (lcInsertMode) POSTableManager.ResetControls();
                        else FormManager.CloseForm();
                    }
                });

                lcAjaxRequestManager.Execute();
            }
        },
        GetSerializedData: function () {
            var lcDataBlock = {};
            var lcDataColumnList = [];
            var lcKeyColumnList = [];
            var lcIdentifierColumnList = [];

            var lcDataControls = clControl.find('input[ea-columnname],textarea[ea-columnname]');
            var lcKeyControls = clControl.find('[ea-columnname][ea-keyfield]');
            var lcIdentifierControls = clControl.find('[ea-columnname][ea-identifiercolumn]');

            lcKeyControls.each(function (paIndex) {
                lcKeyColumnList.push($(this).attr('ea-columnname'));
            });

            lcIdentifierControls.each(function (paIndex) {
                lcIdentifierColumnList.push($(this).attr('ea-columnname'));
            });

            lcDataControls.each(function (paIndex) {
                if (($(this).attr('ea-inputmode') == 'number') || ($(this).attr('ea-inputmode') == 'signednumber')) {
                    if ($(this).attr('ea-columnname') == 'sortorder')
                    {
                        if ($(this).val().trim() == '') $(this).val(lcDataBlock['displayname'].ForceConvertToInteger());
                    }                   
                    lcDataBlock[$(this).attr('ea-columnname')] = $(this).val().ForceConvertToInteger();
                }
                    //   else if ($(this).attr('ea-inputmode') == 'accessinfo') lcDataBlock[$(this).attr('ea-columnname')] = window.__SYSVAR_CurrentGeoLocation || '';
                else lcDataBlock[$(this).attr('ea-columnname')] = $(this).val().trim();

                if (($(this).attr('ea-identifiercolumn') != 'true') && ($(this).attr('ea-columnname')[0] != '#'))
                    lcDataColumnList.push($(this).attr('ea-columnname'));

            });

            lcDataBlock['#KEYCOLUMNLIST'] = lcKeyColumnList.join(',');
            lcDataBlock['#DATACOLUMNLIST'] = lcDataColumnList.join(',');
            lcDataBlock['#IDENTIFIERCOLUMNLIST'] = lcIdentifierColumnList.join(',');

            if (clControlMode == 'table')            
                lcDataBlock['systemlimit'] = clControl.attr('gpos-systemtablelimit') || 0;
            else            
                lcDataBlock['systemlimit'] = clControl.attr('gpos-systemtablegrouplimit') || 0;                            
            
            lcDataBlock['accessinfo'] = window.__SYSVAR_CurrentGeoLocation || '';

            return (Base64.encode(JSON.stringify(lcDataBlock)));
        },
        ResetControls: function (paItemMode) {
            clControl.find('input[ea-columnname],textarea[ea-columnname]').each(function () {
                if ($(this).attr('ea-persist') != 'true')
                    $(this).val($(this).attr('ea-originalvalue') || '');
            });
            
            clControl.find('input[type=text]:not([ea-persist=true])').filter(':visible:first').focus();
        },       
        HandlerOnClick: function (paEvent) {
            paEvent.preventDefault();

            var lcCommand = $(this).attr('ea-command');
            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);

            switch (lcCommand) {
                
                case 'save':
                    {
                        POSTableManager.UpdateRecord();
                        break;
                    }
                case 'close':
                    {
                        FormManager.CloseForm();
                        break;
                    }
            }

        }
    }
})();
