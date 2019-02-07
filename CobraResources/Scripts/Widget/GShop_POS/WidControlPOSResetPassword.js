$(document).ready(function () {
    POSResetPasswordManager.Init();
});

var POSResetPasswordManager = (function () {

    var clControl;
    var clContainer;
    var clLogInIDBox;
    var clPasswordBox;
    var clConfirmPasswordBox;
    var clButtonPanel;
    var clUserID;

    return {
        Init: function () {
            clControl           = $('[sa-elementtype=control].WidControlPOSResetPassword');
            clContainer         = clControl.find('[sa-elementtype=container]');
            clLogInIDBox        = clContainer.find('.LogInID input[type=text]');
            clEmailMobileBox    = clContainer.find('.EmailMobile input[type=text]');            
            clButtonPanel       = clControl.find('.ButtonPanel');            

            POSResetPasswordManager.BindEvents();
        },
        BindEvents: function () {
            clLogInIDBox.ForceLogInInput();

            clButtonPanel.find('a[ea-command^="@cmd%"]').unbind('click');
            clButtonPanel.find('a[ea-command^="@cmd%"]').click(POSResetPasswordManager.HandlerOnClick);
        },
        IsValidLength: function (paControl) {
            if (paControl)
                if ((paControl.val().length >= 3) && (paControl.val().length <= 15)) return (true);

            MessageHandler.ShowMessage('err_invalidinput', null, paControl);


            return (false);
        },
        VerifyInputs: function () {
            if (clLogInIDBox.val().trim().length == 0) MessageHandler.ShowMessage('err_nologinid', null, clLogInIDBox)
            else
            if (clEmailMobileBox.val().trim().length == 0) MessageHandler.ShowMessage('err_noemailmobile', null, clEmailMobileBox)
            else
            if ((POSResetPasswordManager.IsValidLength(clLogInIDBox)) && (POSResetPasswordManager.IsValidLength(clEmailMobileBox)))
            {
                return (true);
            }
            return (false);
        },
        Submit: function () {
            if (POSResetPasswordManager.VerifyInputs()) {
                var lcAjaxRequestManager = new AjaxRequestManager('executescalarquery', 'info_successpasswordreset', 'err_failrequest', 'ajax_updating');

                lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.resetpassword');
                lcAjaxRequestManager.AddAjaxParam('datablock', POSResetPasswordManager.GetSerializedData());

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
                        
            lcDataBlock['fpm_loginid'] = clLogInIDBox.val();
            lcDataBlock['emailmobile'] = clEmailMobileBox.val();
            lcDataBlock['accessinfo'] = window.__SYSVAR_CurrentGeoLocation || '';            

            return (Base64.encode(JSON.stringify(lcDataBlock)));
        },
        HandlerOnClick: function (paEvent) {
            paEvent.preventDefault();

            var lcCommand = $(this).attr('ea-command');
            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);

            switch (lcCommand) {
                case 'submit':
                    {
                        POSResetPasswordManager.Submit();
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
