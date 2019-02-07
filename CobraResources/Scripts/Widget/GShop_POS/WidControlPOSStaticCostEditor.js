$(document).ready(function () {
    //$('[sa-elementtype=control].WidControlPOSStaticCost').BindPOSStaticCostEvents();
    //$('[sa-elementtype=control].WidControlPOSStaticCost').RestoreBlockState();
    POSStaticCostManager.Init();
});

var POSStaticCostManager = (function () {
    var clForm;
    var clToolBar;
    var clControl;
    var clList;    
    var clExternalComponentContainer;
    var clStaticCostInfoPopup;
    var clCalendar;
    var clLowerBoundDays;
    var clUpperBoundDays;
    var clLowerBoundDate;
    var clUpperBoundDate;

    return {
        Init: function () {            
            clForm    = FormManager.GetForm();
            clToolBar = ToolBarManager.GetToolBar();
            clControl = clForm.find('[sa-elementtype=control].WidControlPOSStaticCostEditor');            
            clList = clControl.find('[sa-elementtype=list]');
            clExternalComponentContainer = $('[sa-elementtype=container][ea-type=externalcomponent]');

            clLowerBoundDays = Number(clControl.attr('ea-lowerbound') || '7');
            clUpperBoundDays = Number(clControl.attr('ea-upperbound') || '0');

            clLowerBoundDate = moment().add((!isNaN(clLowerBoundDays) ? -1 * clLowerBoundDays : -7), 'days');
            clUpperBoundDate = moment().add((!isNaN(clUpperBoundDays) ? clUpperBoundDays : 0), 'days');

            POSStaticCostManager.LoadExternalComponents().done(function () {
                POSStaticCostManager.WaitForDependencies().done(function () {
                    clStaticCostInfoPopup = new InputPopUpController('staticcostinfo');
                    clStaticCostInfoPopup.Init();

                    clCalendar = new CalendarComposite('costdate', clLowerBoundDate, clUpperBoundDate);
                    clCalendar.Init();

                    POSStaticCostManager.BindEvents();
                });
            });

                 
        },
        WaitForDependencies: function () {
            var lcDeferred = $.Deferred();

            var lcWaitTimer = setInterval(function () {

                if ((typeof InputPopUpController !== 'undefined') && (typeof CalendarComposite !== 'undefined')) {
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
        RefreshContent : function()
        {
            var lcDeferred = $.Deferred();

            var lcAjaxRequestManager = new AjaxRequestManager('getupdatedcontrol', null, null, 'ajax_loading');

            lcAjaxRequestManager.AddAjaxParam('Parameter', 'entrylist');
            
            lcAjaxRequestManager.SetCompleteHandler(function (paSuccess, paResponseStruct) {
                if (paSuccess) {                                
                    clList.replaceWith(paResponseStruct.ResponseData.RSP_HTML);
                    clList = clControl.find('[sa-elementtype=list]');

                    POSStaticCostManager.BindListElements();
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
            clToolBar.find('a[ea-command]').unbind('click');
            clToolBar.find('a[ea-command]').click(POSStaticCostManager.HandlerOnClick);

            clControl.find('[ea-command]').unbind('click');
            clControl.find('[ea-command]').click(POSStaticCostManager.HandlerOnClick);

            clExternalComponentContainer.find('[ea-command="@popupcmd%cancel"],[ea-command="@popupcmd%update"],[ea-command="@cmd%opencalendar"]').unbind('click');
            clExternalComponentContainer.find('[ea-command="@popupcmd%cancel"],[ea-command="@popupcmd%update"],[ea-command="@cmd%opencalendar"]').click(POSStaticCostManager.HandlerOnClick);

            clStaticCostInfoPopup.SetHandler('ev-datainit', POSStaticCostManager.HandlerOnDataInit);            

            clCalendar.SetHandler('ev-datechanged', POSStaticCostManager.HandlerOnDateChanged);            
        },
        BindListElements : function()
        {            
            clList.find('[ea-command]').unbind('click');
            clList.find('[ea-command]').click(POSStaticCostManager.HandlerOnClick);
        },
        //BindElementEvents: function () {
        //    clControl.find('img[ea-command="@cmd%delete"]').unbind('click');
        //    clControl.find('img[ea-command="@cmd%delete"]').click(POSStaticCostManager.HandlerOnClick);

        //    clControl.find('img[ea-command="@cmd%edit"]').unbind('click');
        //    clControl.find('img[ea-command="@cmd%edit"]').click(POSStaticCostManager.HandlerOnClick);
        //},
        //SetControlMode: function (paMode) {
        //    if (paMode) {
        //        var lcToggleState = clControl.attr('fa-mode');
        //        var lcTargetElement = clToolBar.find('a[ea-command="@cmd%' + paMode + '"]');

        //        clToolBar.find('[fa-active]').removeAttr('fa-active');

        //        if (lcToggleState != paMode) {
        //            lcTargetElement.attr('fa-active', 'true');
        //            clControl.attr('fa-mode', paMode);
        //        }
        //        else {
        //            clControl.removeAttr('fa-mode');
        //        }
        //    }
        //},
        //OpenUnitForm: function (paUnitID) {
        //    paUnitID = paUnitID || '-1'

        //    var lcLinkTemplate = clControl.attr('ea-template').split('||')[1];
        //    var lcLink = lcLinkTemplate.replace("$UNITID", paUnitID);
        //    var lcBlockStateInfo = POSStaticCostManager.SaveBlockState();

        //    FormManager.RedirectStatefulTextLink(lcLink, lcBlockStateInfo);
        //},
        //OpenBaseUnitForm: function (paUnitID) {
        //    paUnitID = paUnitID || '-1'

        //    var lcLinkTemplate = clControl.attr('ea-template').split('||')[0];
        //    var lcLink = lcLinkTemplate.replace("$UNITID", paUnitID);
        //    var lcBlockStateInfo = POSStaticCostManager.SaveBlockState();

        //    FormManager.RedirectStatefulTextLink(lcLink, lcBlockStateInfo);
        //},
        //SaveBlockState: function () {
        //    var lcBlockState = { ScrollTop: clControl.find('[sa-elementtype=list]').scrollTop() };

        //    return (Base64.encode(JSON.stringify(lcBlockState)));
        //},
        //RestoreBlockState: function () {
        //    var lcElementBlockList = clControl.find('[sa-elementtype=block]');
        //    var lcSavedState = GetUrlParameter('_formsavedstate');
        //    var lcItemList = clControl.find('[sa-elementtype=list]');

        //    if (lcSavedState != '') {

        //        lcSavedState = Base64.decode(lcSavedState);

        //        var lcBlockStateInfo = JSON.parse(lcSavedState);

        //        if (lcBlockStateInfo.ScrollTop) lcItemList.scrollTop(lcBlockStateInfo.ScrollTop);
        //    }
        //},
        //DeleteRecord: function (paItemRow) {
        //    var lcDataID = paItemRow.attr('ea-dataid');
        //    var lcUnitName = paItemRow.text();

        //    var lcAjaxRequestManager = new AjaxRequestManager('executenonquery', 'info_successdeleteobject', 'err_faildeleteobject', 'ajax_deleting');

        //    lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.deleteunitrecord');
        //    lcAjaxRequestManager.AddObjectDataBlock('datablock', { FPM_UnitID: lcDataID }, true);
        //    lcAjaxRequestManager.AddMessagePlaceHolder('$OBJECT', lcUnitName);

        //    lcAjaxRequestManager.SetCompleteHandler(function (paSuccess) {
        //        if (paSuccess) {
        //            paItemRow.remove();
        //        }
        //    });

        //    lcAjaxRequestManager.ExecuteOnConfirm('confirm_deleteobject');
        //},
        SetRowFocus : function(paRow)
        {
            var lcRowList = clList.find('[sa-elementtype=row]');

            if (paRow)
            {
                if (paRow.attr('fa-focus') == 'true')
                {
                    lcRowList.removeAttr('fa-focus');
                }
                else
                {
                    lcRowList.removeAttr('fa-focus');
                    paRow.attr('fa-focus', 'true');
                }
            }
        },
        ClearFocus : function()
        {
            clList.find('[sa-elementtype=row]').removeAttr('fa-focus');
        },
        DeleteEntry: function (paEntryRow)
        {
            var lcItemID        = clControl.attr('ea-dataid');
            var lcIncomingID    = paEntryRow.attr('ea-dataid');
            var lcDate          = paEntryRow.find('span[ea-type=date]').text();

            var lcAjaxRequestManager = new AjaxRequestManager('executenonquery', 'info_successdeleteentry', 'err_faildeleteentry', 'ajax_deleting');

            lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.deletestaticcostrecord');
            lcAjaxRequestManager.AddObjectDataBlock('datablock', { FPM_ItemID : lcItemID, FPM_IncomingID : lcIncomingID }, true);
            lcAjaxRequestManager.AddMessagePlaceHolder('$DATE', lcDate);

            lcAjaxRequestManager.SetCompleteHandler(function (paSuccess) {
                if (paSuccess) {
                    paEntryRow.remove();
                }
            });

            lcAjaxRequestManager.ExecuteOnConfirm('confirm_deletecostentry');          
        },
        UpdateEntry : function(paDate, paCost)
        {
            var lcItemID = clControl.attr('ea-dataid');                        

            var lcAjaxRequestManager = new AjaxRequestManager('executenonquery', null, 'err_failupdate', 'ajax_updating');

            lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.updatestaticcostrecord');
            lcAjaxRequestManager.AddObjectDataBlock('datablock', { FPM_ItemID: lcItemID, FPM_Date: paDate.format('YYYY-MM-DD'),FPM_COST : paCost }, true);
            
            lcAjaxRequestManager.SetCompleteHandler(function (paSuccess) {
                if (paSuccess) {
                    POSStaticCostManager.RefreshContent();                    
                }
            });

            lcAjaxRequestManager.Execute();
        },
        HandlerOnDataInit: function (paEvent, paEventInfo) {

            var lcActiveRow = clList.find('[sa-elementtype=row][fa-focus=true]');

            if (paEventInfo.columnname == 'effectivedate') {
                if (lcActiveRow.length > 0)
                {
                    if (lcActiveRow.attr('ea-mode') == 'initialcost') {
                        paEventInfo.target.val(lcActiveRow.find('span[ea-type=date]').text());
                        paEventInfo.target.attr('fa-date', '1900-01-01');
                    }
                    else {
                        var lcDate = (lcActiveRow.attr('ea-group') || moment().format('YYYY-MM-DD')).ParseMomentDate();
                        paEventInfo.target.attr('fa-date', lcDate.format('YYYY-MM-DD'));
                        paEventInfo.target.val(FormManager.ConvertToFormLanguage(lcDate.format('DD/MM/YYYY')));
                    }
                }
                else {
                    paEventInfo.target.removeAttr('fa-date');
                    paEventInfo.target.val(FormManager.ConvertToFormLanguage(moment().format('DD/MM/YYYY')));
                }
            }
            else if (paEventInfo.columnname == 'costamount') {
                if (lcActiveRow.length > 0) {
                    var lcCostAmount = (lcActiveRow.find('span[ea-type=unitprice]').attr('value') || '').ForceConvertToDecimal();
                    paEventInfo.target.val(FormManager.ConvertToFormLanguage(lcCostAmount));
                }
                else
                {
                    paEventInfo.target.val('');
                }
            }
        },
        HandlerOnDateChanged: function (paEvent, paEventInfo) {
            if (paEventInfo.eventdata) {
                var lcDate = paEventInfo.eventdata;
                var lcDateValue = Number(lcDate.format('YYYYMMDD'));

                if ((lcDateValue >= Number(clLowerBoundDate.format('YYYYMMDD'))) && (lcDateValue <= Number(clUpperBoundDate.format('YYYYMMDD')))) {
                    clStaticCostInfoPopup.SetTextBoxData('effectivedate', FormManager.ConvertToFormLanguage(lcDate.format('DD/MM/YYYY')));                    
                    return;
                }
            }
            paEventInfo.defaultaction = false;
        },
        HandlerOnClick: function (paEvent) {
            paEvent.preventDefault();            
            var lcCommand = $(this).attr('ea-command');
            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);

            switch (lcCommand) {

                case 'rowclick':
                    {
                        POSStaticCostManager.SetRowFocus($(this));
                        break;
                    }

                case 'addstaticcost':
                    {
                        var lcCalenderControl = clStaticCostInfoPopup.GetControl('[sa-elementtype=calendarcontrol]');
                        lcCalenderControl.removeAttr('fa-disabled');

                        POSStaticCostManager.ClearFocus();                        
                        clStaticCostInfoPopup.Show();
                        break;
                    }

                case 'opencalendar':
                    {
                        var lcDisabled = $(this).attr('fa-disabled');
                        var lcTextBox = $(this).find('input[type=text]');
                        var lcDate = (lcTextBox.val() || moment().format('YYYY-MM-DD')).ParseMomentDate();

                        if (lcDisabled != 'true') {
                            clCalendar.SetDate(lcDate);
                            clCalendar.Show();
                        }
                        break;
                    }

                case 'cancel':
                    {
                        var lcType = $(this).closest('[sa-elementtype=popup],[sa-elementtype=composite]').attr('ea-type');
                        
                        if (lcType == 'staticcostinfo') clStaticCostInfoPopup.Hide();
                        else if (lcType == 'calendar') clCalendar.Hide();

                        break;
                    }

                case 'delete':
                    {
                        paEvent.stopPropagation();

                        var lcActiveRow = clList.find('[sa-elementtype=row][fa-focus=true]');

                        if (lcActiveRow)
                        {
                            var lcMode = lcActiveRow.attr('ea-mode');

                            if (lcMode != 'initialcost') {
                                POSStaticCostManager.DeleteEntry(lcActiveRow);
                            }
                            else {
                                MessageHandler.ShowMessage('err_cannotdeleteinitialcost');
                            }
                        }
                        
                        break;
                    }

                case 'update':
                    {
                        var lcDateControl = clStaticCostInfoPopup.GetControl('input[type=text][ea-columnname=effectivedate]');
                        var lcDate = (lcDateControl.attr('fa-date') || lcDateControl.val()).ParseMomentDate();
                        var lcCostTextBox = clStaticCostInfoPopup.GetControl('input[type=text][ea-columnname=costamount]');
                        var lcCostLabel = lcCostTextBox.closest('[sa-elementtype=inputrow]').find('[sa-elementtype=inputlabel]').text();
                        var lcCost = lcCostTextBox.val().ForceConvertToDecimal(-1);
                        
                        if (lcCost < 0) {
                            MessageHandler.ShowDynamicMessage('err_invalidamount', { '$FIELD': lcCostLabel }, lcCostTextBox);
                        }
                        else
                        {
                            clStaticCostInfoPopup.Hide();
                            POSStaticCostManager.UpdateEntry(lcDate, lcCost);
                        }                        
                        
                        break;
                    }

                case 'edit':
                    {
                        paEvent.stopPropagation();

                        var lcCalenderControl = clStaticCostInfoPopup.GetControl('[sa-elementtype=calendarcontrol]');
                        lcCalenderControl.attr('fa-disabled', 'true');
                        clStaticCostInfoPopup.Show();
                        break;
                    }

                //case 'editmode':
                //case 'deletemode':
                //    {
                //        POSStaticCostManager.SetControlMode(lcCommand);
                //        break;
                //    }

                //case 'addunit':
                //    {
                //        POSStaticCostManager.OpenUnitForm();
                //        break;
                //    }

                //case 'addbaseunit':
                //    {
                //        POSStaticCostManager.OpenBaseUnitForm();
                //        break;
                //    }

                //case 'edit':
                //    {
                //        paEvent.stopPropagation();

                //        var lcUnitRow = $(this).closest('[sa-elementtype=row]');
                //        var lcUnitID = lcUnitRow.attr('ea-dataid');
                //        var lcType = lcUnitRow.attr('ea-type');

                //        if (lcType == 'base')
                //            POSStaticCostManager.OpenBaseUnitForm(lcUnitID);
                //        else
                //            POSStaticCostManager.OpenUnitForm(lcUnitID);

                //        break;
                    //    }                
                case 'delete':
                    {
                        paEvent.stopPropagation();
                        alert('delete');

                        break;
                    }
            }
        },
    }
})();
