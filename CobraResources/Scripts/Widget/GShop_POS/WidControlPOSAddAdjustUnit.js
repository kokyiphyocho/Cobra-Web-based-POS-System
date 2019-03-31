$(document).ready(function () {
    POSUnitEditingManager.Init();
});

var POSUnitEditingManager = (function () {

    var clControl;
    var clToolBar;
    var clAllSlideControls;
    var clMajorUnitName;
    var clMinorUnitName;
    var clUnitRelationship;
    var clUnitName;
    var clRelationshipTextLabel;
    var clControlMode;
    var clSelectionPopUpList;
    return {
        Init: function () {
            clControl           = $('[sa-elementtype=control].WidControlPOSAddAdjustUnit');
            clControlMode       = clControl.attr('ea-controlmode');
            clAllSlideControls  = clControl.find('[sa-elementtype=slideselectioncontrol]');
            clSelectionPopUpList = clControl.find('[sa-elementtype=popup].SubControlSelectionPanel');

            clMajorUnitName = clControl.find('input[ea-columnname=majorunitname]');
            clMinorUnitName = clControl.find('input[ea-columnname=minorunitname]');
            clUnitRelationship = clControl.find('input[ea-columnname=unitrelationship]');       
            clUnitName = clControl.find('input[ea-columnname="unitname"]');
            clRelationshipTextLabel = clControl.find('[ea-columnname="#unitrelationshiptext"]').parent().find('[sa-elementtype=inputlabel]');

            POSUnitEditingManager.WaitForDependencies().done(function () {
                clToolBar = ToolBarManager.GetToolBar();

                POSUnitEditingManager.SetInputBehaviour();
                POSUnitEditingManager.BindEvents();

                clSelectionPopUpList.each(function () {                    
                    $(this).data(new SelectionPanelController($(this), clControl));
                    $(this).data().Init();
                });
            });
        },
        SetInputBehaviour: function () {
            var lcInputBlock = clControl.find('[sa-elementtype=container] [sa-elementtype=inputblock]');
            var lcNumberBoxes = lcInputBlock.find('input[type=text][ea-inputmode=number]');
            var lcSignedNumberBoxes = lcInputBlock.find('input[type=text][ea-inputmode=signednumber]');

            lcNumberBoxes.ForceIntegerInput();
            lcSignedNumberBoxes.ForceSignedIntegerInput();
        },
        WaitForDependencies: function () {
            var lcDeferred = $.Deferred();
            
            var lcWaitTimer = setInterval(function () {
                if (((clAllSlideControls.length == 0) || (typeof SelectionPanelController !== 'undefined')) && (typeof ToolBarManager !== 'undefined')) {
                    if (lcDeferred.state() == 'pending') {
                        lcDeferred.resolve();
                        clearInterval(lcWaitTimer);
                    }
                }

            }, 200);

            return (lcDeferred);
        },
        BindEvents: function () {

            clControl.unbind('ev-selectionpanelevent');
            clControl.bind('ev-selectionpanelevent', POSUnitEditingManager.HandlerOnSelectionPanelEvent);

            clControl.find('[ea-command]').unbind('click');
            clControl.find('[ea-command]').click(POSUnitEditingManager.HandlerOnClick);

            clControl.find('input[ea-columnname=unitrelationship]').unbind('keyup');
            clControl.find('input[ea-columnname=unitrelationship]').keyup(POSUnitEditingManager.HandlerOnKeyUp);

            clToolBar.find('[ea-command^="@cmd%"]').unbind('click');
            clToolBar.find('[ea-command^="@cmd%"]').click(POSUnitEditingManager.HandlerOnClick);

            //clControl.find('[sa-elementtype=popup] [sa-elementtype=overlay] [sa-elementtype=panel]').unbind('change');
            //clControl.find('[sa-elementtype=popup] [sa-elementtype=overlay] [sa-elementtype=panel]').change(POSUnitEditingManager.HandlerOnChange);
            
            //clControl.find('a[ea-command="@cmd%update"]').unbind('click');
            //clControl.find('a[ea-command="@cmd%update"]').click(POSUnitEditingManager.HandlerOnClick);

            //clControl.find('a[ea-command="@cmd%close"]').unbind('click');
            //clControl.find('a[ea-command="@cmd%close"]').click(POSUnitEditingManager.HandlerOnClick);
        },
        //OpenSlideControl: function (paSlideControl) {
        //    var lcType = paSlideControl.attr('ea-type');
        //    var lcPopUp = clControl.find('[sa-elementtype=popup][ea-type="' + lcType + '"]');
        //    var lcInputControl = paSlideControl.find('input[type=text]');
        //    var lcList = lcPopUp.find('[sa-elementtype=panel] [sa-elementtype=list]');
        //    var lcListItems = lcList.find('a');
        //    var lcActiveControl = lcList.find('a[ea-text="' + lcInputControl.val().trim() + '"]');

        //    if (paSlideControl.attr('fa-disable') != 'true') {
        //        lcListItems.removeAttr('fa-selected');
        //        lcActiveControl.attr('fa-selected', 'true');

        //        clAllSlideControls.removeAttr('fa-active');
        //        paSlideControl.attr('fa-active', true);
        //        clControl.attr('fa-showpanel', lcType);
        //    }
        //},       
        RefreshUnitRelationshipText: function () {
            var lcRelationshipValue = clUnitRelationship.val().ForceConvertToInteger();
            var lcTextTemplate = clControl.attr('ea-statustext');

            if ((lcRelationshipValue != 0) &&
                (clMajorUnitName.val().trim().length > 0) &&
                (clMinorUnitName.val().trim().length > 0)) {
                lcTextTemplate = lcTextTemplate.replace('$MAJORUNITCOUNT', FormManager.ConvertToFormLanguage('1'));
                lcTextTemplate = lcTextTemplate.replace('$MAJORUNITNAME', clMajorUnitName.val().trim());
                lcTextTemplate = lcTextTemplate.replace('$MINORUNITCOUNT', FormManager.ConvertToFormLanguage(lcRelationshipValue));
                lcTextTemplate = lcTextTemplate.replace('$MINORUNITNAME', clMinorUnitName.val().trim());

                clRelationshipTextLabel.text(lcTextTemplate);
                clRelationshipTextLabel.show();
            }
            else {

                clRelationshipTextLabel.text('');
                clRelationshipTextLabel.hide();
            }
        },
        RefreshUnitName : function()
        {
            var lcRelationshipValue = clUnitRelationship.val().ForceConvertToInteger(0);

            if ((clMajorUnitName.val().trim().length > 0) && (clMinorUnitName.val().trim().length > 0)) {

                if ( lcRelationshipValue > 0)        
                    clUnitName.val(clMajorUnitName.val().trim() + '/' + clMinorUnitName.val().trim() + ' (' + lcRelationshipValue + ')');
                else clUnitName.val(clMajorUnitName.val().trim() + '/' + clMinorUnitName.val().trim());
            }
        },
        Refresh : function()
        {            
            if (clControlMode != 'base') {
                POSUnitEditingManager.RefreshUnitName();
                POSUnitEditingManager.RefreshUnitRelationshipText();
            }            
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

            var lcMandtoryFields = clControl.find('[ea-mandatory=true]');
            
            lcMandtoryFields.each(function (paIndex) {

                if ($(this).val().trim() == '') {                    
                    POSUnitEditingManager.ShowControlError($(this), 'err_requirefieldmissing');
                    lcSuccess = false;
                    return (false);
                }
            });

            if ((lcSuccess) && (clControlMode != 'base')) {
                var lcRelationshipValue = clUnitRelationship.val().ForceConvertToInteger();
                
                if (lcRelationshipValue <= 1) {
                    lcSuccess = false;
                    POSUnitEditingManager.ShowControlError(clUnitRelationship, 'err_unitrelationshipvalue');
                }                
            }

            return (lcSuccess);
        },
        UpdateRecord: function () {
            var lcContainer = clControl.find('[sa-elementtype=container]');
            var lcUnitIDBox = clControl.find('[ea-columnname=unitid]');            
            var lcInsertMode = false;

            if (Number(lcUnitIDBox.val()) == -1) lcInsertMode = true;

            if (POSUnitEditingManager.VerifyInputs()) {
                var lcAjaxRequestManager = new AjaxRequestManager('executenonquery', null, 'err_failaddunit', 'ajax_updating');

                lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.updateunitrecord');
                lcAjaxRequestManager.AddAjaxParam('datablock', POSUnitEditingManager.GetSerializedData());
                
                lcAjaxRequestManager.SetCompleteHandler(function (paSuccess) {
                    if (paSuccess) {
                        if (lcInsertMode) POSUnitEditingManager.ResetControls();
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
                if (($(this).attr('ea-inputmode') == 'number') || ($(this).attr('ea-inputmode') == 'signednumber')) lcDataBlock[$(this).attr('ea-columnname')] = $(this).val().ForceConvertToInteger();
                else if ($(this).attr('ea-inputmode') == 'accessinfo') lcDataBlock[$(this).attr('ea-columnname')] = window.__SYSVAR_CurrentGeoLocation || '';
                else lcDataBlock[$(this).attr('ea-columnname')] = $(this).val().trim();

                if (($(this).attr('ea-identifiercolumn') != 'true') && ($(this).attr('ea-columnname')[0] != '#'))
                    lcDataColumnList.push($(this).attr('ea-columnname'));

            });

            lcDataBlock['#KEYCOLUMNLIST'] = lcKeyColumnList.join(',');
            lcDataBlock['#DATACOLUMNLIST'] = lcDataColumnList.join(',');
            lcDataBlock['#IDENTIFIERCOLUMNLIST'] = lcIdentifierColumnList.join(',');
            

            return (Base64.encode(JSON.stringify(lcDataBlock)));
        },
        //SetSelectionBehaviour: function (paSlideControl, paParameter) {
        //    var lcDataArray = paParameter.split(';;');

        //    if (lcDataArray.length == 1) {
        //        POSItemEditingManager.SetStandAloneUnitBehaviour(paSlideControl, lcDataArray[0]);
        //    }
        //    else if (lcDataArray.length == 4) {
        //        POSItemEditingManager.SetPresetUnitBehaviour(lcDataArray);
        //    }
        //},
        //RefreshUnitRelationshipText: function () {
        //    var lcRelationshipValue = clUnitRelationship.val().ForceConvertToInteger();
        //    var lcTextTemplate = clControl.attr('ea-statustext');

        //    if ((lcRelationshipValue != 0) &&
        //        (clMajorUnitName.val().trim().length > 0) &&
        //        (clMinorUnitName.val().trim().length > 0)) {
        //        lcTextTemplate = lcTextTemplate.replace('$MAJORUNITCOUNT', FormManager.ConvertToFormLanguage('1'));
        //        lcTextTemplate = lcTextTemplate.replace('$MAJORUNITNAME', clMajorUnitName.val().trim());
        //        lcTextTemplate = lcTextTemplate.replace('$MINORUNITCOUNT', FormManager.ConvertToFormLanguage(lcRelationshipValue));
        //        lcTextTemplate = lcTextTemplate.replace('$MINORUNITNAME', clMinorUnitName.val().trim());

        //        clRelationshipTextLabel.text(lcTextTemplate);
        //        clRelationshipTextLabel.show();
        //    }
        //    else {

        //        clRelationshipTextLabel.text('');
        //        clRelationshipTextLabel.hide();
        //    }
        //},
        ResetControls: function (paItemMode) {
            clControl.find('input[ea-columnname],textarea[ea-columnname]').each(function () {
                $(this).val($(this).attr('ea-originalvalue') || '');
            });          

            POSUnitEditingManager.RefreshUnitRelationshipText();

            clControl.find('input[type=text]').filter(':visible:first').focus();
        },
        HandlerOnKeyUp : function(paEvent)
        {
            POSUnitEditingManager.Refresh();
        },
        HandlerOnBlur: function (paEvent) {
            POSUnitEditingManager.RefreshUnitRelationshipText();
        },
        //HandlerOnChange: function (paEvent) {
        //    var lcPopUp = $(this).closest('[sa-elementtype=popup]');
        //    var lcList = lcPopUp.find('[sa-elementtype=panel] [sa-elementtype=list]');
        //    var lcActiveItem = lcList.find('a[fa-selected]');
        //    var lcColumnName = lcPopUp.attr('ea-type');
        //    var lcSelectionControl = clControl.find('[sa-elementtype=slideselectioncontrol][fa-active]');
        //    var lcInputControl = lcSelectionControl.find('input[type=text]');

        //    if (lcActiveItem.length > 0) {                
        //        lcInputControl.val(lcActiveItem.first().text());
        //        POSUnitEditingManager.Refresh();
        //    }
        //    else
        //        lcInputControl.val('');
        //},
        HandlerOnSelectionPanelEvent: function (paEvent, paEventInfo) {            
            if (paEventInfo) {
                switch (paEventInfo.event) {
                    case 'openpopup': break;

                    case 'closepopup':
                        {
                            clAllSlideControls.removeAttr('fa-active');
                            break;
                        }

                    case 'selectionchoosed':
                        {
                            var lcSelectionControl = clControl.find('[sa-elementtype=slideselectioncontrol][fa-active]');

                            if (lcSelectionControl.length == 1) {
                                var lcInputControl = lcSelectionControl.find('input[type=text]');
                                // POSUnitEditingManager.SetSelectionBehaviour(lcSelectionControl, paEventInfo.selectedvalue);
                                lcInputControl.val(paEventInfo.selectedtext);
                                POSUnitEditingManager.RefreshUnitRelationshipText();
                                clAllSlideControls.removeAttr('fa-active');
                            }
                        }
                        break;
                }
            }
        },
        HandlerOnClick: function (paEvent) {
            paEvent.preventDefault();

            var lcCommand = $(this).attr('ea-command');
            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);

            switch (lcCommand) {
                case 'openslide':
                    {
                        var lcDisabled = $(this).attr('fa-disable');

                        if (lcDisabled != 'true') {
                            var lcType = $(this).attr('ea-type');
                            var lcPopUp = clControl.find('.SubControlSelectionPanel[sa-elementtype=popup][ea-type="' + lcType + '"]');

                            $(this).attr('fa-active', true);
                            $(this).siblings().removeAttr('fa-active');

                            lcPopUp.data().OpenPopUp();
                        }
                        break;
                    }

                case 'save':
                    {                        
                        POSUnitEditingManager.UpdateRecord();
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
