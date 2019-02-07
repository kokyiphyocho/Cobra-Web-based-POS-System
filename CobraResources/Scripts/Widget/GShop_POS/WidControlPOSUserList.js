$(document).ready(function () {
    //$('[sa-elementtype=control].WidControlPOSUserList').BindPOSUserListEvents();
    //$('[sa-elementtype=control].WidControlPOSUserList').RestoreBlockState();
    POSUserListManager.Init();
});

var POSUserListManager = (function () {
    var clForm;
    var clToolBar;
    var clControl;

    return {
        Init: function () {
            clForm = FormManager.GetForm();
            clToolBar = ToolBarManager.GetToolBar();
            clControl = clForm.find('[sa-elementtype=control].WidControlPOSUserList');
            POSUserListManager.BindEvents();
            POSUserListManager.BindElementEvents();

            POSUserListManager.RestoreBlockState();
        },
        BindEvents: function () {
            clToolBar.find('a[ea-command="@cmd%editmode"],a[ea-command="@cmd%deletemode"]').unbind('click');
            clToolBar.find('a[ea-command="@cmd%editmode"],a[ea-command="@cmd%deletemode"]').click(POSUserListManager.HandlerOnClick);

            clToolBar.find('a[ea-command="@cmd%adduser"]').unbind('click');
            clToolBar.find('a[ea-command="@cmd%adduser"]').click(POSUserListManager.HandlerOnClick);

            clToolBar.find('a[ea-command="@cmd%addbaseuser"]').unbind('click');
            clToolBar.find('a[ea-command="@cmd%addbaseuser"]').click(POSUserListManager.HandlerOnClick);
        },
        BindElementEvents: function () {
            clControl.find('img[ea-command="@cmd%delete"]').unbind('click');
            clControl.find('img[ea-command="@cmd%delete"]').click(POSUserListManager.HandlerOnClick);

            clControl.find('img[ea-command="@cmd%edit"]').unbind('click');
            clControl.find('img[ea-command="@cmd%edit"]').click(POSUserListManager.HandlerOnClick);
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
        OpenUserForm: function (paUserID) {
            paUserID = paUserID || '-1'

            var lcLinkTemplate = clControl.attr('ea-template');
            var lcLink = lcLinkTemplate.replace("$USERID", paUserID);
            var lcBlockStateInfo = POSUserListManager.SaveBlockState();

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
            var lcuserName = paItemRow.text();

            var lcAjaxRequestManager = new AjaxRequestManager('executenonquery', 'info_successdeleteobject', 'err_faildeleteobject', 'ajax_deleting');

            lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.deleteuserrecord');
            lcAjaxRequestManager.AddObjectDataBlock('datablock', { FPM_userID: lcDataID }, true);
            lcAjaxRequestManager.AddMessagePlaceHolder('$OBJECT', lcuserName);

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
                        POSUserListManager.SetControlMode(lcCommand);
                        break;
                    }

                case 'adduser':
                    {
                        POSUserListManager.OpenUserForm();
                        break;
                    }
                case 'edit':
                    {
                        paEvent.stopPropagation();

                        var lcuserRow = $(this).closest('[sa-elementtype=row]');
                        var lcuserID = lcuserRow.attr('ea-dataid');
                        var lcType = lcuserRow.attr('ea-type');

                        POSUserListManager.OpenUserForm(lcuserID);

                        break;
                    }
                case 'delete':
                    {
                        paEvent.stopPropagation();

                        var lcItemRow = $(this).closest('[sa-elementtype=row]');

                        POSUserListManager.DeleteRecord(lcItemRow);
                        break;
                    }
            }
        },
    }
})();
