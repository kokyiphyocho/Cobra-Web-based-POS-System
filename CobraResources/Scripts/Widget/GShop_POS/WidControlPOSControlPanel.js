$(document).ready(function () {
    POSControlPanelManager.Init();
});

var POSControlPanelManager = (function () {
    var clForm;    
    var clControl;
    var clContainer;

    return {
        Init: function () {
            clForm      = FormManager.GetForm();            
            clControl   = clForm.find('[sa-elementtype=control].WidControlPOSControlPanel');
            clContainer = clControl.find('[sa-elementtype=container]');

            POSControlPanelManager.BindEvents();
            
            POSControlPanelManager.RestoreBlockState();
            
        },
        BindEvents: function () {
            clControl.find('[ea-command]').unbind('click');
            clControl.find('[ea-command]').click(POSControlPanelManager.HandlerOnClick);
        },
        //BindElementEvents: function () {
        //    clControl.find('div[sa-elementtype=row][ea-command="@cmd%showgroup"]').unbind('click');
        //    clControl.find('div[sa-elementtype=row][ea-command="@cmd%showgroup"]').click(POSControlPanelManager.HandlerOnClick);

        //    clControl.find('div[sa-elementtype=title] div[ea-command="@cmd%upgroup"]').unbind('click');
        //    clControl.find('div[sa-elementtype=title] div[ea-command="@cmd%upgroup"]').click(POSControlPanelManager.HandlerOnClick);

        //    clControl.find('div[sa-elementtype=title] div[ea-command="@cmd%rootgroup"]').unbind('click');
        //    clControl.find('div[sa-elementtype=title] div[ea-command="@cmd%rootgroup"]').click(POSControlPanelManager.HandlerOnClick);

        //    clControl.find('img[ea-command="@cmd%delete"]').unbind('click');
        //    clControl.find('img[ea-command="@cmd%delete"]').click(POSControlPanelManager.HandlerOnClick);

        //    clControl.find('img[ea-command="@cmd%edit"]').unbind('click');
        //    clControl.find('img[ea-command="@cmd%edit"]').click(POSControlPanelManager.HandlerOnClick);
        //},
        //LoadContent: function () {
        //    var lcDeferred = $.Deferred();

        //    var lcAjaxRequestManager = new AjaxRequestManager('getupdatedcontrol', null, null, 'ajax_loading');

        //    lcAjaxRequestManager.AddAjaxParam('Parameter', 'tablelistcontent');

        //    lcAjaxRequestManager.SetCompleteHandler(function (paSuccess, paResponseStruct) {
        //        if (paSuccess) {
        //            clContainer.empty();
        //            clContainer.html(paResponseStruct.ResponseData.RSP_HTML);

        //            POSControlPanelManager.BindElementEvents();
        //            lcDeferred.resolve(true);
        //        }
        //        else {
        //            lcDeferred.resolve(false)
        //        }
        //    });

        //    lcAjaxRequestManager.Execute();

        //    return (lcDeferred);
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
        OpenForm: function (paFormName) {           
            if (paFormName)
            {
                var lcStateInfo = POSControlPanelManager.SaveBlockState(clControl);

                FormManager.RedirectStatefulTextLink(paFormName, lcStateInfo);
            }
        },
        SaveBlockState: function () {            
            var lcStateInfo = {};
            lcStateInfo['scrolltop'] = clContainer.scrollTop();
            return (Base64.encode(JSON.stringify(lcStateInfo)));
        },
        RestoreBlockState: function () {            
            var lcSavedState = GetUrlParameter('_formsavedstate');

            if (lcSavedState != '') {
                lcSavedState = Base64.decode(lcSavedState);

                var lcStateInfo = JSON.parse(lcSavedState);

                if (lcStateInfo['scrolltop']) clContainer.scrollTop(lcStateInfo['scrolltop']);
            }
        },

        //    POSControlPanelManager.RefreshToolBarIcon();
        //},
        //RefreshToolBarIcon: function () {
        //    var lcTableGroupLimit = clControl.attr('gpos-systemtablegrouplimit') || 0;
        //    var lcActiveBlock = clControl.find('[sa-elementtype=block][fa-position=middle]');
        //    if (lcActiveBlock.length == 0) lcActiveBlock = clControl.find('[sa-elementtype=block][ea-group="0"]');

        //    if ((lcActiveBlock.attr('ea-group') == '0') && (lcTableGroupLimit > 0)) {
        //        clToolBar.find('a[ea-command="@cmd%addtablegroup"]').removeAttr('fa-hide');
        //    }
        //    else {
        //        clToolBar.find('a[ea-command="@cmd%addtablegroup"]').attr('fa-hide', 'true');
        //    }
        //},
        //ShowTableGroupBlock: function (paDataID) {
        //    if (paDataID) {
        //        var lcActiveBlock = clControl.find('[sa-elementtype=block][fa-position=middle]');
        //        if (lcActiveBlock.length == 0) lcActiveBlock = clControl.find('[sa-elementtype=block][ea-group="0"]');

        //        var lcTableGroupBlock = clControl.find('[sa-elementtype=block][ea-group="' + paDataID + '"]');
        //        lcActiveBlock.attr('fa-position', 'left');
        //        lcTableGroupBlock.attr('fa-position', 'middle');

        //        POSControlPanelManager.RefreshToolBarIcon();

        //        clControl.removeAttr('fa-mode');

        //    }
        //},
        //UpTableGroupBlock: function () {
        //    var lcActiveBlock = clControl.find('[sa-elementtype=block][fa-position=middle]');
        //    var lcParent = lcActiveBlock.attr('ea-parent');

        //    if (lcParent) {
        //        var lcParentBlock = clControl.find('[sa-elementtype=block][ea-group="' + lcParent + '"]');
        //        if (lcParentBlock) {
        //            lcParentBlock.attr('fa-position', 'middle');
        //            lcActiveBlock.removeAttr('fa-position');

        //            POSControlPanelManager.RefreshToolBarIcon();

        //            clControl.removeAttr('fa-mode');
        //        }
        //    }
        //},
        //RootTableGroupBlock: function () {
        //    var lcActiveBlock = clControl.find('[sa-elementtype=block][fa-position=middle]');
        //    var lcLeftBlocks = clControl.find('[sa-elementtype=block][fa-position=left]').not('[ea-group="0"]');
        //    var lcRootBlock = clControl.find('[sa-elementtype=block][ea-group="0"]');

        //    lcLeftBlocks.hide();
        //    lcLeftBlocks.removeAttr('fa-position');
        //    lcRootBlock.removeAttr('fa-position');

        //    lcActiveBlock.removeAttr('fa-position');

        //    setTimeout(function () { lcLeftBlocks.show() }, 500);

        //    POSControlPanelManager.RefreshToolBarIcon();

        //    clControl.removeAttr('fa-mode');
        //},
        //DeleteRecord: function (paItemRow) {
        //    var lcDataID = paItemRow.attr('ea-dataid');
        //    var lcItemName = paItemRow.text();

        //    var lcAjaxRequestManager = new AjaxRequestManager('executenonquery', 'info_successdeleteobject', 'err_faildeleteobject', 'ajax_deleting');

        //    lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.deletetablerecord');
        //    lcAjaxRequestManager.AddObjectDataBlock('datablock', { FPM_TABLEID: lcDataID }, true);
        //    lcAjaxRequestManager.AddMessagePlaceHolder('$OBJECT', lcItemName);

        //    lcAjaxRequestManager.SetCompleteHandler(function (paSuccess) {
        //        if (paSuccess) {
        //            paItemRow.remove();
        //        }
        //    });

        //    lcAjaxRequestManager.ExecuteOnConfirm('confirm_deleteobject');
        //},

        HandlerOnClick: function (paEvent) {
            paEvent.preventDefault();

            var lcCommand = $(this).attr('ea-command');
            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);

            switch(lcCommand)
            {
                case 'openform':
                    {
                        var lcFormName = $(this).attr('ea-formname');
                        POSControlPanelManager.OpenForm(lcFormName);
                    }
            }
            
            //switch (lcCommand) {
            //    case 'editmode':
            //    case 'deletemode':
            //        {
            //            POSControlPanelManager.SetControlMode(lcCommand);
            //            break;
            //        }

            //    case 'addtable':
            //    case 'addtablegroup':
            //        {
            //            var lcMode = lcCommand.substring(3);

            //            if (lcMode) POSControlPanelManager.OpenElementForm(lcMode);
            //            break;
            //        }

            //    case 'showgroup':
            //        {
            //            var lcDataID = $(this).attr('ea-dataid');
            //            var lcType = $(this).attr('ea-type');

            //            if (lcType == 'group') POSControlPanelManager.ShowTableGroupBlock(lcDataID);

            //            break;
            //        }

            //    case 'upgroup':
            //        {
            //            POSControlPanelManager.UpTableGroupBlock();
            //            break;
            //        }

            //    case 'rootgroup':
            //        {
            //            POSControlPanelManager.RootTableGroupBlock();
            //            break;
            //        }

            //    case 'edit':
            //        {
            //            paEvent.stopPropagation();

            //            var lcTableListRow = $(this).closest('[sa-elementtype=row]');
            //            var lcMode = lcTableListRow.attr('ea-type');
            //            var lcTableID = lcTableListRow.attr('ea-dataid');

            //            POSControlPanelManager.OpenElementForm(lcMode, lcTableID);

            //            break;
            //        }

            //    case 'delete':
            //        {
            //            paEvent.stopPropagation();

            //            var lcItemRow = $(this).closest('[sa-elementtype=row]');

            //            POSControlPanelManager.DeleteRecord(lcItemRow);
            //            break;
            //        }
            //}
        },
    }
})();
