$(document).ready(function () {
    POSTableListManager.Init();
});

var POSTableListManager = (function () {
    var clForm;
    var clToolBar;
    var clControl;
    var clContainer;
    
    return {
        Init: function () {
            clForm = FormManager.GetForm();
            clToolBar = ToolBarManager.GetToolBar();
            clControl = clForm.find('[sa-elementtype=control].WidControlPOSTableList');
            clContainer = clControl.find('[sa-elementtype=container]');            

            POSTableListManager.BindEvents();

            POSTableListManager.LoadContent().done(function () {                
                POSTableListManager.RestoreBlockState();
            });
        },
        BindEvents: function () {
            clToolBar.find('[ea-command]').unbind('click');
            clToolBar.find('[ea-command]').click(POSTableListManager.HandlerOnClick);

            //clToolBar.find('a[ea-command="@cmd%editmode"],a[ea-command="@cmd%deletemode"]').unbind('click');
            //clToolBar.find('a[ea-command="@cmd%editmode"],a[ea-command="@cmd%deletemode"]').click(POSTableListManager.HandlerOnClick);

            //clToolBar.find('a[ea-command^="@cmd%add"]').unbind('click');
            //clToolBar.find('a[ea-command^="@cmd%add"]').click(POSTableListManager.HandlerOnClick);
        },
        BindElementEvents: function () {
            clControl.find('div[sa-elementtype=row][ea-command="@cmd%showgroup"]').unbind('click');
            clControl.find('div[sa-elementtype=row][ea-command="@cmd%showgroup"]').click(POSTableListManager.HandlerOnClick);

            clControl.find('div[sa-elementtype=title] div[ea-command="@cmd%upgroup"]').unbind('click');
            clControl.find('div[sa-elementtype=title] div[ea-command="@cmd%upgroup"]').click(POSTableListManager.HandlerOnClick);

            clControl.find('div[sa-elementtype=title] div[ea-command="@cmd%rootgroup"]').unbind('click');
            clControl.find('div[sa-elementtype=title] div[ea-command="@cmd%rootgroup"]').click(POSTableListManager.HandlerOnClick);

            clControl.find('img[ea-command="@cmd%delete"]').unbind('click');
            clControl.find('img[ea-command="@cmd%delete"]').click(POSTableListManager.HandlerOnClick);

            clControl.find('img[ea-command="@cmd%edit"]').unbind('click');
            clControl.find('img[ea-command="@cmd%edit"]').click(POSTableListManager.HandlerOnClick);
        },
        LoadContent: function () {
            var lcDeferred = $.Deferred();

            var lcAjaxRequestManager = new AjaxRequestManager('getupdatedcontrol', null, null, 'ajax_loading');

            lcAjaxRequestManager.AddAjaxParam('Parameter', 'tablelistcontent');

            lcAjaxRequestManager.SetCompleteHandler(function (paSuccess, paResponseStruct) {
                if (paSuccess) {
                    clContainer.empty();
                    clContainer.html(paResponseStruct.ResponseData.RSP_HTML);

                    POSTableListManager.BindElementEvents();
                    lcDeferred.resolve(true);
                }
                else {
                    lcDeferred.resolve(false)
                }
            });

            lcAjaxRequestManager.Execute();

            return (lcDeferred);
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
        OpenElementForm: function (paMode, paTableID) {
            var lcIndex = -1;

            paTableID = paTableID || '-1'

            switch (paMode) {
                case "table": lcIndex = 0; break;
                case "tablegroup":
                case "group" : lcIndex = 1; break;
            }

            if (lcIndex >= 0) {
                var lcLinkTemplate = clControl.attr('ea-template').split("||")[lcIndex];

                var lcActiveBlock = clControl.find('[sa-elementtype=block][fa-position=middle]');
                if (lcActiveBlock.length == 0) lcActiveBlock = clControl.find('[sa-elementtype=block][ea-group="0"]');

                var lcGroupID = lcActiveBlock.attr('ea-group');
                var lcLink = lcLinkTemplate.replace('$GROUPID', lcGroupID).replace('$TABLEID', paTableID);
                var lcBlockStateInfo = POSTableListManager.SaveBlockState(clControl);

                FormManager.RedirectStatefulTextLink(lcLink, lcBlockStateInfo);
            }
        },
        SaveBlockState: function () {
            var lcElementBlock = clControl.find('[sa-elementtype=block]');
            var lcBlockStateList = [];

            lcElementBlock.each(function () {
                var lcBlockState = {}
                var lcItemList = $(this).find('[sa-elementtype=list]');

                lcBlockState['ea-group'] = $(this).attr('ea-group');
                lcBlockState['fa-position'] = $(this).attr('fa-position');
                lcBlockState['scrolltop'] = lcItemList.scrollTop();

                lcBlockStateList.push(lcBlockState);
            });

            return (Base64.encode(JSON.stringify(lcBlockStateList)));
        },
        RestoreBlockState: function () {
            var lcElementBlockList = clControl.find('[sa-elementtype=block]');
            var lcSavedState = GetUrlParameter('_formsavedstate');

            if (lcSavedState != '') {

                lcSavedState = Base64.decode(lcSavedState);

                var lcBlockStateInfo = JSON.parse(lcSavedState);

                $.each(lcBlockStateInfo, function (paIndex, paBlockData) {
                    var lcElementBlock = clControl.find('[sa-elementtype=block][ea-group="' + paBlockData['ea-group'] + '"]');
                    var lcPosition = paBlockData['fa-position'];
                    var lcScrollTop = paBlockData['scrolltop'];
                    var lcItemList = lcElementBlock.find('[sa-elementtype=list]');

                    if (lcElementBlock) {
                        if (lcPosition) {
                            lcElementBlock.addClass('NoTransition');
                            lcElementBlock.attr('fa-position', lcPosition);
                            setTimeout(function () { lcElementBlock.removeClass('NoTransition'); }, 500);
                        }
                        if (lcScrollTop) lcItemList.scrollTop(lcScrollTop);
                    }
                });
            }

            POSTableListManager.RefreshToolBarIcon();
        },
        RefreshToolBarIcon : function()
        {
            var lcTableGroupLimit = clControl.attr('gpos-systemtablegrouplimit') || 0;
            var lcActiveBlock = clControl.find('[sa-elementtype=block][fa-position=middle]');
            if (lcActiveBlock.length == 0) lcActiveBlock = clControl.find('[sa-elementtype=block][ea-group="0"]');
            
            if ((lcActiveBlock.attr('ea-group') == '0') && (lcTableGroupLimit > 0))
            {             
                clToolBar.find('a[ea-command="@cmd%addtablegroup"]').removeAttr('fa-hide');
            }
            else
            {                
                clToolBar.find('a[ea-command="@cmd%addtablegroup"]').attr('fa-hide', 'true');
            }
        },
        ShowTableGroupBlock: function (paDataID) {
            if (paDataID) {
                var lcActiveBlock = clControl.find('[sa-elementtype=block][fa-position=middle]');
                if (lcActiveBlock.length == 0) lcActiveBlock = clControl.find('[sa-elementtype=block][ea-group="0"]');

                var lcTableGroupBlock = clControl.find('[sa-elementtype=block][ea-group="' + paDataID + '"]');
                lcActiveBlock.attr('fa-position', 'left');
                lcTableGroupBlock.attr('fa-position', 'middle');

                POSTableListManager.RefreshToolBarIcon();

                clControl.removeAttr('fa-mode');
                
            }
        },
        UpTableGroupBlock: function () {
            var lcActiveBlock = clControl.find('[sa-elementtype=block][fa-position=middle]');
            var lcParent = lcActiveBlock.attr('ea-parent');

            if (lcParent) {
                var lcParentBlock = clControl.find('[sa-elementtype=block][ea-group="' + lcParent + '"]');
                if (lcParentBlock) {
                    lcParentBlock.attr('fa-position', 'middle');
                    lcActiveBlock.removeAttr('fa-position');

                    POSTableListManager.RefreshToolBarIcon();

                    clControl.removeAttr('fa-mode');                    
                }
            }
        },
        RootTableGroupBlock: function () {
            var lcActiveBlock = clControl.find('[sa-elementtype=block][fa-position=middle]');
            var lcLeftBlocks = clControl.find('[sa-elementtype=block][fa-position=left]').not('[ea-group="0"]');
            var lcRootBlock = clControl.find('[sa-elementtype=block][ea-group="0"]');

            lcLeftBlocks.hide();
            lcLeftBlocks.removeAttr('fa-position');
            lcRootBlock.removeAttr('fa-position');

            lcActiveBlock.removeAttr('fa-position');

            setTimeout(function () { lcLeftBlocks.show() }, 500);

            POSTableListManager.RefreshToolBarIcon();

            clControl.removeAttr('fa-mode');
        },
        DeleteRecord: function (paItemRow) {
            var lcDataID = paItemRow.attr('ea-dataid');
            var lcItemName = paItemRow.text();

            var lcAjaxRequestManager = new AjaxRequestManager('executenonquery', 'info_successdeleteobject', 'err_faildeleteobject', 'ajax_deleting');

            lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.deletetablerecord');
            lcAjaxRequestManager.AddObjectDataBlock('datablock', { FPM_TABLEID: lcDataID }, true);
            lcAjaxRequestManager.AddMessagePlaceHolder('$OBJECT', lcItemName);

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
                        POSTableListManager.SetControlMode(lcCommand);
                        break;
                    }

                case 'addtable':
                case 'addtablegroup':                
                    {
                        var lcMode = lcCommand.substring(3);

                        if (lcMode) POSTableListManager.OpenElementForm(lcMode);
                        break;
                    }

                case 'showgroup':
                    {
                        var lcDataID = $(this).attr('ea-dataid');
                        var lcType = $(this).attr('ea-type');

                        if (lcType == 'group') POSTableListManager.ShowTableGroupBlock(lcDataID);

                        break;
                    }

                case 'upgroup':
                    {
                        POSTableListManager.UpTableGroupBlock();
                        break;
                    }

                case 'rootgroup':
                    {
                        POSTableListManager.RootTableGroupBlock();
                        break;
                    }

                case 'edit':
                    {
                        paEvent.stopPropagation();

                        var lcTableListRow = $(this).closest('[sa-elementtype=row]');
                        var lcMode = lcTableListRow.attr('ea-type');
                        var lcTableID = lcTableListRow.attr('ea-dataid');

                        POSTableListManager.OpenElementForm(lcMode, lcTableID);

                        break;
                    }

                case 'delete':
                    {
                        paEvent.stopPropagation();

                        var lcItemRow = $(this).closest('[sa-elementtype=row]');

                        POSTableListManager.DeleteRecord(lcItemRow);
                        break;
                    }
            }
        },
    }
})();
