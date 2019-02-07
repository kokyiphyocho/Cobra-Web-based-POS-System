$(document).ready(function () {
    //$('[sa-elementtype=control].WidControlPOSAddAdjustItem').BindPOSAddAdjustItemEvents();
    //$('[sa-elementtype=form]').ReplaceMessagePlaceHolders();
    //$('[sa-elementtype=control].WidControlPOSAddAdjustItem').ResetControls();
    POSItemEditingManager.Init();
});


var POSItemEditingManager = (function () {

    var clForm;
    var clToolBar;
    var clControl;
    var clControlMode;
    var clAllSlideControls;
    var clEntryAttribute;
    var clEntryAttributeValue;
    var clMajorUnitName;
    var clMinorUnitName;
    var clUnitRelationship;
    var clMajorPriceInput;
    var clMinorPriceInput;
    var clMajorPriceLabel;
    var clMinorPriceLabel;  
    var clUnitType;
    var clRelationshipTextLabel;
    var clSelectionPopUpList;

    return {
                Init : function()
                {
                    clForm                      = FormManager.GetForm();
                    clToolBar                   = ToolBarManager.GetToolBar();
                    clControl                   = clForm.find('[sa-elementtype=control].WidControlPOSAddAdjustItem');
                    clControlMode               = clControl.attr('ea-controlmode');
                    clAllSlideControls          = clControl.find('[sa-elementtype=slideselectioncontrol]');
                    clSelectionPopUpList        = clControl.find('[sa-elementtype=popup].SubControlSelectionPanel');

                    clEntryAttribute            = clControl.find('input[ea-columnname=entryattribute]');
                    clEntryAttributeValue       = (clEntryAttribute ? clEntryAttribute.attr('value') || '' : '').toLowerCase().trim();
                    clMajorUnitName             = clControl.find('input[ea-columnname=majorunitname]');
                    clMinorUnitName             = clControl.find('input[ea-columnname=minorunitname]');
                    clUnitRelationship          = clControl.find('input[ea-columnname=unitrelationship]');
                    clMajorPriceInput           = clControl.find('input[ea-columnname=majorprice]');
                    clMinorPriceInput           = clControl.find('input[ea-columnname=minorprice]');
                    clMajorPriceLabel           = clMajorPriceInput.parent().find('[sa-elementtype=inputlabel]');
                    clMinorPriceLabel           = clMinorPriceInput.parent().find('[sa-elementtype=inputlabel]');

                    clUnitType                  = clControl.find('input[ea-columnname="unitname"]');
                    clRelationshipTextLabel     = clControl.find('[ea-columnname="#unitrelationshiptext"]').parent().find('[sa-elementtype=inputlabel]');
                    
                    POSItemEditingManager.SetInputBehaviour();
                    POSItemEditingManager.BindEvents();

                    POSItemEditingManager.WaitForDependencies().done(function () {
                        clSelectionPopUpList.each(function () {
                            $(this).data(new SelectionPanelController($(this), clControl));                            
                            $(this).data().Init();
                        });
                    });
                },
                SetInputBehaviour : function()
                {
                    var lcInputBlock        = clControl.find('[sa-elementtype=container] [sa-elementtype=inputblock]');
                    var lcNumberBoxes       = lcInputBlock.find('input[type=text][ea-inputmode=number]');
                    var lcDecimalBoxes      = lcInputBlock.find('input[type=text][ea-inputmode=decimal]');
                    var lcSignedNumberBoxes = lcInputBlock.find('input[type=text][ea-inputmode=signednumber]');
                    
                    lcNumberBoxes.ForceIntegerInput();
                    lcDecimalBoxes.ForceDecimalInput();
                    lcSignedNumberBoxes.ForceSignedIntegerInput();
                    
                    if ((clControlMode == 'item') && (clEntryAttributeValue != 'static')) {
                        POSItemEditingManager.SetControlAppearance(clMajorUnitName.val(), clMajorUnitName, clMajorPriceLabel, clMajorPriceInput);
                        POSItemEditingManager.SetControlAppearance(clMinorUnitName.val(), clMinorUnitName, clMinorPriceLabel, clMinorPriceInput);
                    }
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
                BindEvents : function()
                {                    
                    //clControl.find('[sa-elementtype=slideselectioncontrol][ea-command]').unbind('click');
                    //clControl.find('[sa-elementtype=slideselectioncontrol][ea-command]').click(POSItemEditingManager.HandlerOnClick);

                    //clControl.find('[sa-elementtype=popup] [sa-elementtype=overlay] [sa-elementtype=panel]').unbind('change');
                    //clControl.find('[sa-elementtype=popup] [sa-elementtype=overlay] [sa-elementtype=panel]').change(POSItemEditingManager.HandlerOnChange);

                    clControl.unbind('ev-selectionpanelevent');
                    clControl.bind('ev-selectionpanelevent', POSItemEditingManager.HandlerOnSelectionPanelEvent);

                    clControl.find('input[ea-columnname=unitrelationship]').unbind('blur');
                    clControl.find('input[ea-columnname=unitrelationship]').blur(POSItemEditingManager.HandlerOnBlur);

                    clControl.find('[ea-command]').unbind('click');
                    clControl.find('[ea-command]').click(POSItemEditingManager.HandlerOnClick);

                    clToolBar.find('[ea-command^="@cmd%"]').unbind('click');
                    clToolBar.find('[ea-command^="@cmd%"]').click(POSItemEditingManager.HandlerOnClick);

                    //clControl.find('a[ea-command="@cmd%update"]').unbind('click');
                    //clControl.find('a[ea-command="@cmd%update"]').click(POSItemEditingManager.HandlerOnClick);
  
                    //clControl.find('a[ea-command="@cmd%close"]').unbind('click');
                    //clControl.find('a[ea-command="@cmd%close"]').click(POSItemEditingManager.HandlerOnClick);
                },
                OpenSlideControl : function(paSlideControl)
                {
                    //var lcType = paSlideControl.attr('ea-type');
                    //var lcPopUp = clControl.find('[sa-elementtype=popup][ea-type="' + lcType + '"]');
                    //var lcInputControl = paSlideControl.find('input[type=text]');
                    //var lcList = lcPopUp.find('[sa-elementtype=panel] [sa-elementtype=list]');
                    //var lcListItems = lcList.find('a');
                    //var lcActiveControl = lcList.find('a[ea-text="' + lcInputControl.val().trim() + '"]');
                    
                    //if (paSlideControl.attr('fa-disable') != 'true')
                    //{
                    //    lcListItems.removeAttr('fa-selected');
                    //    lcActiveControl.attr('fa-selected', 'true');

                    //    clAllSlideControls.removeAttr('fa-active');
                    //    paSlideControl.attr('fa-active', true);
                    //    clControl.attr('fa-showpanel', lcType);
                    //}
                },
                SetStandAloneUnitBehaviour :  function(paSlideControl, paParameter)
                {
                    if (paSlideControl.attr('ea-columnname') == 'majorunitname')
                    {
                        if (paParameter.trim().length > 0) {
                            clMajorPriceLabel.text(FormManager.ConvertToFormLanguage('1 ') + paParameter);
                            clMajorPriceInput.removeAttr('readonly');
                        }
                    }
                    else if (paSlideControl.attr('ea-columnname') == 'minorunitname')
                    {
                        if (paParameter.trim().length > 0) {
                            clMinorPriceLabel.text(FormManager.ConvertToFormLanguage('1 ') + paParameter);
                            clMinorPriceInput.removeAttr('readonly');
                        }
                    }                
                },
                SetControlAppearance : function(paData, paUnitNameControl, paPriceLabel, paPriceInput)
                {
                    if (paData == '#ADJUSTABLE') {                        
                        paUnitNameControl.parent().removeAttr('fa-disable');
                        paUnitNameControl.val('');
                        paPriceInput.attr('readonly', 'true');
                        paPriceLabel.text('-');
                    }
                    else
                    {                        
                        paUnitNameControl.parent().attr('fa-disable', 'true');
                        paUnitNameControl.val(paData);
                     
                        if (paData.trim().length > 0) {                     
                            paPriceInput.removeAttr('readonly');
                            paPriceLabel.text(FormManager.ConvertToFormLanguage('1 ') + paData);                            
                        }
                        else {
                            paPriceInput.attr('readonly', 'true');
                            paPriceLabel.text('-');
                        }
                    }
                },
                SetPresetUnitBehaviour :   function(paParamArray)
                {
                    var lcAdjustableRelationship = paParamArray[3].ForceConvertToInteger() == 0 ? true : false;

                    POSItemEditingManager.SetControlAppearance(paParamArray[0], clMajorUnitName, clMajorPriceLabel, clMajorPriceInput);
                    POSItemEditingManager.SetControlAppearance(paParamArray[1], clMinorUnitName, clMinorPriceLabel, clMinorPriceInput);
                    
                    clUnitRelationship.val(FormManager.ConvertToFormLanguage(paParamArray[2].ForceConvertToInteger()));

                    if (lcAdjustableRelationship)
                    {
                        clUnitRelationship.attr('readonly', 'true');
                    }
                    else {
                            clUnitRelationship.removeAttr('readonly');
                         }                    
                },
                SetSelectionBehaviour  : function(paSlideControl, paParameter)
                {                      
                    var lcDataArray = paParameter.split(';;');
    
                    if (lcDataArray.length == 1) 
                    {
                        POSItemEditingManager.SetStandAloneUnitBehaviour(paSlideControl, lcDataArray[0]);
                    }
                    else if (lcDataArray.length == 4)
                    {
                        POSItemEditingManager.SetPresetUnitBehaviour(lcDataArray);
                    }                    
                },
                RefreshUnitRelationshipText : function()
                {   
                    var lcRelationshipValue = clUnitRelationship.val().ForceConvertToInteger();
                    var lcTextTemplate = clControl.attr('ea-statustext');

                    if ((lcRelationshipValue != 0) && 
                        (clMajorUnitName.val().trim().length > 0) &&
                        (clMinorUnitName.val().trim().length > 0))
                    {                        
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
                ShowControlError : function(paControl, paMessageCode)
                {
                    if (paControl)
                    {
                        var lcInputRow      = paControl.closest('[sa-elementtype=inputrow]');
                        var lcLabel         = lcInputRow.find('[sa-elementtype=inputlabel]');

                        MessageHandler.ShowDynamicMessage(paMessageCode, { '$FIELD': lcLabel.text() }, paControl);                        
                    }
                },
                VerifyInputs : function(paItemMode)
                {                    
                    var lcSuccess = true;                    

                    var lcMandatoryFields = clControl.find('[ea-mandatory=true]');
                                        
                    lcMandatoryFields.each(function (paIndex) {
                       
                        if ($(this).val().trim() == '') {
                    
                            POSItemEditingManager.ShowControlError($(this), 'err_requirefieldmissing');                            
                            lcSuccess = false;
                            return (false);
                        }                      
                    });                    
                    

                    if ((paItemMode) && (clEntryAttributeValue != 'static') && (lcSuccess)) {                        
                            var lcRelationshipValue = clUnitRelationship.val().ForceConvertToInteger();

                            if (clUnitType.val().trim().length == 0) {
                                POSItemEditingManager.ShowControlError(clUnitType, 'err_selectionrequire');
                                lcSuccess = false;
                            }
                            else
                                if ((clMinorUnitName.val().trim().length > 0) && (lcRelationshipValue <= 1)) {
                                    lcSuccess = false;
                                    POSItemEditingManager.ShowControlError(clUnitRelationship, 'err_unitrelationshipvalue');
                                }
                                else if (clMajorUnitName.val().trim().length == 0) {
                                    POSItemEditingManager.ShowControlError(clMajorUnitName, 'err_selectionrequire');
                                    lcSuccess = false;
                                }
                                else if ((clMinorUnitName.parent().attr('fa-disable') != 'true') && (clMinorUnitName.val().trim().length == 0)) {
                                    POSItemEditingManager.ShowControlError(clMinorUnitName, 'err_selectionrequire');
                                    lcSuccess = false;
                                }                        
                    }

                    return (lcSuccess);
                },
                UpdateRecord : function()
                {                    
                    var lcContainer     = clControl.find('[sa-elementtype=container]');
                    var lcItemIDBox     = clControl.find('[ea-columnname=itemid]');                    
                    var lcItemNameBox   = clControl.find('[ea-columnname=itemname]');   
                    var lcItemMode      = clControl.attr('ea-controlmode') == 'item' ? true : false;
                    var lcItemLimit     = clControl.attr('gpos-systemitemlimit');
                    var lcInsertMode    = false;
                    
                    if (Number(lcItemIDBox.val()) == -1) lcInsertMode = true;
                    
                    if (POSItemEditingManager.VerifyInputs(lcItemMode)) {
                        var lcAjaxRequestManager = new AjaxRequestManager('executenonquery', null, 'err_failaddobject', 'ajax_updating');

                        lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.updateitemrecord');
                        lcAjaxRequestManager.AddAjaxParam('datablock', POSItemEditingManager.GetSerializedData());
                        lcAjaxRequestManager.AddMessagePlaceHolder('$OBJECT', lcItemNameBox.val());
                        lcAjaxRequestManager.AddMessagePlaceHolder('$ITEMLIMIT',  FormManager.ConvertToFormLanguage(lcItemLimit));                        
                                                
                        lcAjaxRequestManager.SetCompleteHandler(function (paSuccess) {
                            if (paSuccess) {
                                if (lcInsertMode) POSItemEditingManager.ResetControls();
                                else FormManager.CloseForm();
                            }
                        });

                        lcAjaxRequestManager.Execute();
                    }
                },
                GetSerializedData : function()
                {
                    var lcDataBlock             = {};
                    var lcDataColumnList        = [];
                    var lcKeyColumnList         = [];
                    var lcIdentifierColumnList  = [];

                    var lcDataControls          = clControl.find('input[ea-columnname],textarea[ea-columnname]');
                    var lcKeyControls           = clControl.find('[ea-columnname][ea-keyfield]');
                    var lcIdentifierControls    = clControl.find('[ea-columnname][ea-identifiercolumn]');
    
                    lcKeyControls.each(function (paIndex) {
                        lcKeyColumnList.push($(this).attr('ea-columnname'));
                    });

                    lcIdentifierControls.each(function (paIndex) {
                        lcIdentifierColumnList.push($(this).attr('ea-columnname'));
                    });

                    lcDataControls.each(function (paIndex) {
                        if (($(this).attr('ea-inputmode') == 'number') || ($(this).attr('ea-inputmode') == 'signednumber')) lcDataBlock[$(this).attr('ea-columnname')] = $(this).val().ForceConvertToInteger();        
                        else if ($(this).attr('ea-inputmode') == 'accessinfo')  lcDataBlock[$(this).attr('ea-columnname')] = window.__SYSVAR_CurrentGeoLocation || '';
                        else lcDataBlock[$(this).attr('ea-columnname')] = $(this).val().trim();
        
                        if (($(this).attr('ea-identifiercolumn') != 'true') && ($(this).attr('ea-columnname')[0] != '#'))
                            lcDataColumnList.push($(this).attr('ea-columnname'));
        
                    });
    
                    lcDataBlock['#KEYCOLUMNLIST'] = lcKeyColumnList.join(',');
                    lcDataBlock['#DATACOLUMNLIST'] = lcDataColumnList.join(',');
                    lcDataBlock['#IDENTIFIERCOLUMNLIST'] = lcIdentifierColumnList.join(',');

                    lcDataBlock['SystemItemLimit'] = clControl.attr('gpos-systemitemlimit');
                    
                    return (Base64.encode(JSON.stringify(lcDataBlock)));
                },
                ResetControls : function(paItemMode)
                {   
                    clControl.find('input[ea-columnname],textarea[ea-columnname]').each(function () {
                        if ($(this).attr('ea-persist') != 'true')
                            $(this).val($(this).attr('ea-originalvalue') || '');
                    });
                    
                    clUnitRelationship.attr('readonly', 'true');                    

                    if ((clControlMode == 'item') && (clEntryAttributeValue != 'static')) {
                        POSItemEditingManager.SetControlAppearance(clMajorUnitName.val(), clMajorUnitName, clMajorPriceLabel, clMajorPriceInput);
                        POSItemEditingManager.SetControlAppearance(clMinorUnitName.val(), clMinorUnitName, clMinorPriceLabel, clMinorPriceInput);
                    }

                    POSItemEditingManager.RefreshUnitRelationshipText();

                    clControl.find('input[type=text]:not([ea-persist=true])').filter(':visible:first').focus();
                },
                HandlerOnBlur   : function(paEvent)
                {
                    POSItemEditingManager.RefreshUnitRelationshipText();
                },
                //HandlerOnChange : function(paEvent)
                //{                    
                //    var lcPopUp             = $(this).closest('[sa-elementtype=popup]');
                //    var lcList              = lcPopUp.find('[sa-elementtype=panel] [sa-elementtype=list]');
                //    var lcActiveItem        = lcList.find('a[fa-selected]');
                //    var lcColumnName        = lcPopUp.attr('ea-type');
                //    var lcSelectionControl  = clControl.find('[sa-elementtype=slideselectioncontrol][fa-active]');
                //    var lcInputControl      = lcSelectionControl.find('input[type=text]');

                //    if (lcActiveItem.length > 0) {
                //        POSItemEditingManager.SetSelectionBehaviour(lcSelectionControl, lcActiveItem.attr('value'));
                //        lcInputControl.val(lcActiveItem.first().text());
                //        POSItemEditingManager.RefreshUnitRelationshipText();
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
                                        POSItemEditingManager.SetSelectionBehaviour(lcSelectionControl, paEventInfo.selectedvalue);
                                        lcInputControl.val(paEventInfo.selectedtext);
                                        POSItemEditingManager.RefreshUnitRelationshipText();
                                        clAllSlideControls.removeAttr('fa-active');
                                    }                                    
                                }
                                break;
                        }
                    }
                },
                HandlerOnClick : function(paEvent)
                {
                    paEvent.preventDefault();
            
                    var lcCommand = $(this).attr('ea-command');
                    lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);
                    
                    switch (lcCommand)
                    {
                        case 'openslide':
                            {
                                var lcDisabled  = $(this).attr('fa-disable');
                                
                                if (lcDisabled != 'true')
                                {
                                    var lcType = $(this).attr('ea-type');
                                    var lcPopUp = clControl.find('.SubControlSelectionPanel[sa-elementtype=popup][ea-type="' + lcType + '"]');

                                    $(this).attr('fa-active', true);
                                    $(this).siblings().removeAttr('fa-active');

                                    lcPopUp.data().OpenPopUp();                                    
                                }
                                break;
                            }
                        case 'save' :
                            {                                
                                POSItemEditingManager.UpdateRecord();
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
