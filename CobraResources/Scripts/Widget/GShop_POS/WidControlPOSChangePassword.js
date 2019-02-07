$(document).ready(function () {
    POSPasswordManager.Init();
});

var POSPasswordManager = (function () {

    var clControl;
    var clContainer;
    var clLogInIDBox;
    var clPasswordBox;
    var clConfirmPasswordBox;
    var clButtonPanel;
    var clUserID;

    return {
        Init: function () {
            clControl = $('[sa-elementtype=control].WidControlPOSChangePassword');
            clContainer = clControl.find('[sa-elementtype=container]');
            clLogInIDBox = clContainer.find('.LogInID input[type=text]');
            clPasswordBox = clContainer.find('.Password input[type=password]');
            clConfirmPasswordBox = clContainer.find('.ConfirmPassword input[type=password]');
            clButtonPanel = clControl.find('.ButtonPanel');
            clUserID = (clControl.attr('ea-dataid') || '').ForceConvertToInteger(-1);

            POSPasswordManager.BindEvents();
        },
        BindEvents: function () {
            clLogInIDBox.ForceLogInInput();
            clPasswordBox.ForceLogInInput();
            clConfirmPasswordBox.ForceLogInInput();

            clButtonPanel.find('a[ea-command^="@cmd%"]').unbind('click');
            clButtonPanel.find('a[ea-command^="@cmd%"]').click(POSPasswordManager.HandlerOnClick);
        },
        IsValidLength: function (paControl) {
            if (paControl)
                if ((paControl.val().length >= 3) && (paControl.val().length <= 15)) return (true);

            MessageHandler.ShowMessage('err_loginlength', null, paControl);


            return (false);
        },
        VerifyInputs: function () {            
                    if ((POSPasswordManager.IsValidLength(clPasswordBox)) &&
                        (POSPasswordManager.IsValidLength(clConfirmPasswordBox))) {
                        if (clPasswordBox.val() == clConfirmPasswordBox.val()) return (true);
                        else {
                            MessageHandler.ShowMessage('err_passwordmismatch', null, clPasswordBox);
                        }
                    }
                    return (false);
        },
        UpdateRecord: function () {

            if (POSPasswordManager.VerifyInputs()) {
                var lcAjaxRequestManager = new AjaxRequestManager('executenonquery', null, 'err_failupdate', 'ajax_updating');

                lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.updateuserrecord');
                lcAjaxRequestManager.AddAjaxParam('datablock', POSPasswordManager.GetSerializedData());

                lcAjaxRequestManager.SetCompleteHandler(function (paSuccess) {
                    if (paSuccess) {
                        FormManager.CloseForm();
                    }
                });

                lcAjaxRequestManager.Execute();
            }
        },
        GetSerializedData: function () {
            var lcDataBlock = {};

            lcDataBlock['fpm_userid'] = clUserID;
            lcDataBlock['fpm_loginid'] = clLogInIDBox.val();
            lcDataBlock['password'] = $.md5(clPasswordBox.val()).toUpperCase();
            lcDataBlock['accessinfo'] = window.__SYSVAR_CurrentGeoLocation || '';

            return (Base64.encode(JSON.stringify(lcDataBlock)));
        },
        HandlerOnClick: function (paEvent) {
            paEvent.preventDefault();

            var lcCommand = $(this).attr('ea-command');
            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);

            switch (lcCommand) {
                case 'update':
                    {
                        POSPasswordManager.UpdateRecord();
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
