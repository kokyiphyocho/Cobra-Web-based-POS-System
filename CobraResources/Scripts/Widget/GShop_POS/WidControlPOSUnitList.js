$(document).ready(function () {
    //$('[sa-elementtype=control].WidControlPOSUnitList').BindPOSUnitListEvents();
    //$('[sa-elementtype=control].WidControlPOSUnitList').RestoreBlockState();
    POSUnitListManager.Init();
});

var POSUnitListManager = (function () {
    var clForm;
    var clToolBar;
    var clControl;

    return {
                Init: function () {
                    clForm = FormManager.GetForm();
                    clToolBar = ToolBarManager.GetToolBar();
                    clControl = clForm.find('[sa-elementtype=control].WidControlPOSUnitList');
                    POSUnitListManager.BindEvents();
                    POSUnitListManager.BindElementEvents();

                    POSUnitListManager.RestoreBlockState();
                },
                BindEvents: function () {
                    clToolBar.find('a[ea-command="@cmd%editmode"],a[ea-command="@cmd%deletemode"]').unbind('click');
                    clToolBar.find('a[ea-command="@cmd%editmode"],a[ea-command="@cmd%deletemode"]').click(POSUnitListManager.HandlerOnClick);

                    clToolBar.find('a[ea-command="@cmd%addunit"]').unbind('click');
                    clToolBar.find('a[ea-command="@cmd%addunit"]').click(POSUnitListManager.HandlerOnClick);

                    clToolBar.find('a[ea-command="@cmd%addbaseunit"]').unbind('click');
                    clToolBar.find('a[ea-command="@cmd%addbaseunit"]').click(POSUnitListManager.HandlerOnClick);
                },
                BindElementEvents: function () {
                    clControl.find('img[ea-command="@cmd%delete"]').unbind('click');
                    clControl.find('img[ea-command="@cmd%delete"]').click(POSUnitListManager.HandlerOnClick);

                    clControl.find('img[ea-command="@cmd%edit"]').unbind('click');
                    clControl.find('img[ea-command="@cmd%edit"]').click(POSUnitListManager.HandlerOnClick);
                },
                SetControlMode: function (paMode) {
                    if (paMode) {
                        var lcToggleState = clControl.attr('fa-mode');
                        var lcTargetElement = clToolBar.find('a[ea-command="@cmd%' + paMode + '"]');

                        clToolBar.find('[fa-active]').removeAttr('fa-active');

                        if (lcToggleState != paMode) {
                            lcTargetElement.attr('fa-active', 'true');
                            clControl.attr('fa-mode', paMode);
                        }
                        else {
                            clControl.removeAttr('fa-mode');
                        }
                    }
                },
                OpenUnitForm: function (paUnitID) {                    
                    paUnitID = paUnitID || '-1'
                    
                    var lcLinkTemplate = clControl.attr('ea-template').split('||')[1];                    
                    var lcLink = lcLinkTemplate.replace("$UNITID", paUnitID);
                    var lcBlockStateInfo = POSUnitListManager.SaveBlockState();

                    FormManager.RedirectStatefulTextLink(lcLink, lcBlockStateInfo);
                },
                OpenBaseUnitForm: function (paUnitID) {
                    paUnitID = paUnitID || '-1'

                    var lcLinkTemplate = clControl.attr('ea-template').split('||')[0];
                    var lcLink = lcLinkTemplate.replace("$UNITID", paUnitID);
                    var lcBlockStateInfo = POSUnitListManager.SaveBlockState();

                    FormManager.RedirectStatefulTextLink(lcLink, lcBlockStateInfo);
                },
                SaveBlockState: function () {                    
                    var lcBlockState = { ScrollTop: clControl.find('[sa-elementtype=list]').scrollTop() };

                    return (Base64.encode(JSON.stringify(lcBlockState)));
                },
                RestoreBlockState: function () {                    
                    var lcElementBlockList = clControl.find('[sa-elementtype=block]');
                    var lcSavedState = GetUrlParameter('_formsavedstate');
                    var lcItemList = clControl.find('[sa-elementtype=list]');

                    if (lcSavedState != '') {

                        lcSavedState = Base64.decode(lcSavedState);

                        var lcBlockStateInfo = JSON.parse(lcSavedState);

                        if (lcBlockStateInfo.ScrollTop) lcItemList.scrollTop(lcBlockStateInfo.ScrollTop);
                    }
                },        
                DeleteRecord: function (paItemRow) {
                    var lcDataID = paItemRow.attr('ea-dataid');
                    var lcUnitName = paItemRow.text();

                    var lcAjaxRequestManager = new AjaxRequestManager('executenonquery', 'info_successdeleteobject', 'err_faildeleteobject', 'ajax_deleting');

                    lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.deleteunitrecord');
                    lcAjaxRequestManager.AddObjectDataBlock('datablock', { FPM_UnitID: lcDataID }, true);
                    lcAjaxRequestManager.AddMessagePlaceHolder('$OBJECT', lcUnitName);

                    lcAjaxRequestManager.SetCompleteHandler(function (paSuccess) {
                        if (paSuccess) {
                            paItemRow.remove();
                        }
                    });

                    lcAjaxRequestManager.ExecuteOnConfirm('confirm_deleteobject');
                },
                HandlerOnClick: function (paEvent) {
                    paEvent.preventDefault();
                    
                    var lcCommand = $(this).attr('ea-command');
                    lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);

                    switch (lcCommand) {
                        case 'editmode':
                        case 'deletemode':
                            {                                
                                POSUnitListManager.SetControlMode(lcCommand);
                                break;
                            }

                        case 'addunit':                        
                            {
                                POSUnitListManager.OpenUnitForm();
                                break;
                            }

                        case 'addbaseunit':
                            {
                                POSUnitListManager.OpenBaseUnitForm();
                                break;
                            }

                        case 'edit':
                            {
                                paEvent.stopPropagation();

                                var lcUnitRow   = $(this).closest('[sa-elementtype=row]');                                
                                var lcUnitID    = lcUnitRow.attr('ea-dataid');
                                var lcType      = lcUnitRow.attr('ea-type');
                                
                                if (lcType == 'base')
                                    POSUnitListManager.OpenBaseUnitForm(lcUnitID);
                                else
                                    POSUnitListManager.OpenUnitForm(lcUnitID);

                                break;
                            }
                        case 'delete':
                            {
                                paEvent.stopPropagation();

                                var lcItemRow = $(this).closest('[sa-elementtype=row]');

                                POSUnitListManager.DeleteRecord(lcItemRow);
                                break;
                            }
                    }
                },
    }
})();
