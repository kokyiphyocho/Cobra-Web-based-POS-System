$(document).ready(function () {
    POSUserManager.Init();
});

var POSUserManager = (function () {
    var clForm;
    var clToolBar;
    var clControl;
    var clContainer;
    var clLogInIDBox;
    var clPasswordBox;
    var clConfirmPasswordBox;
   // var clButtonPanel;
    var clUserID;
        
    return {
        Init: function () {
            clForm                  = FormManager.GetForm();
            clToolBar               = ToolBarManager.GetToolBar();
            clControl               = clForm.find('[sa-elementtype=control].WidControlPOSAddAdjustUser');
            clContainer             = clControl.find('[sa-elementtype=container]');
            clLogInIDBox            = clContainer.find('.LogInID input[type=text]');
            clPasswordBox           = clContainer.find('.Password input[type=password]');
            clConfirmPasswordBox    = clContainer.find('.ConfirmPassword input[type=password]');
         //   clButtonPanel           = clControl.find('.ButtonPanel');
            clUserID                = (clControl.attr('ea-dataid') || '').ForceConvertToInteger(-1);
            
            POSUserManager.BindEvents();
        },        
        BindEvents: function () {
            clLogInIDBox.ForceLogInInput();
            clPasswordBox.ForceLogInInput();
            clConfirmPasswordBox.ForceLogInInput();
            
            clToolBar.find('[ea-command^="@cmd%"]').unbind('click');
            clToolBar.find('[ea-command^="@cmd%"]').click(POSUserManager.HandlerOnClick);

            //clButtonPanel.find('a[ea-command^="@cmd%"]').unbind('click');
            //clButtonPanel.find('a[ea-command^="@cmd%"]').click(POSUserManager.HandlerOnClick);
        },
        IsValidLength : function(paControl)
        {
            if (paControl)            
                if ((paControl.val().length >= 3) && (paControl.val().length <= 15)) return (true);
            
            MessageHandler.ShowMessage('err_loginlength',null,paControl);
            

            return (false);            
        },
        VerifyInputs : function () {
            
            if (POSUserManager.IsValidLength(clLogInIDBox))
            {
                if ((clUserID == -1) || (clPasswordBox.val().length != 0)) {
                    if ((POSUserManager.IsValidLength(clPasswordBox)) &&
                        (POSUserManager.IsValidLength(clConfirmPasswordBox))) {
                        if (clPasswordBox.val() == clConfirmPasswordBox.val()) return (true);
                        else {
                            MessageHandler.ShowMessage('err_passwordmismatch', null, clPasswordBox);
                        }
                    }
                }
                else return (true);              
            }
            return (false);
        },
        UpdateRecord: function () {

            if (POSUserManager.VerifyInputs()) {                
                var lcAjaxRequestManager = new AjaxRequestManager('executenonquery', null, 'err_failupdate', 'ajax_updating');

                lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.updateuserrecord');
                lcAjaxRequestManager.AddAjaxParam('datablock', POSUserManager.GetSerializedData());

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

            if (clPasswordBox.val().trim().length > 0)
                lcDataBlock['password'] = $.md5(clPasswordBox.val()).toUpperCase();
            else
                lcDataBlock['password'] = '';

            lcDataBlock['accessinfo'] = window.__SYSVAR_CurrentGeoLocation || '';

            return(Base64.encode(JSON.stringify(lcDataBlock)));
        },
        HandlerOnClick: function (paEvent) {
            paEvent.preventDefault();
            
            var lcCommand = $(this).attr('ea-command');
            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);

            switch (lcCommand) {
                case 'save':
                    {
                        POSUserManager.UpdateRecord();
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
