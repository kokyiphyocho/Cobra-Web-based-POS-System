$(document).ready(function () {        
    POSLogInManager.Init();
});

var POSLogInManager = (function () {
    var clControl;
    var clPasswordTextBox;
    var clLogInIDTextBox;
    var clLoginButton;
    var clStatusBar;

    return {
                Init : function()
                {
                    clControl = $('[sa-elementtype=control].WidControlPOSLogIn');
                    clPasswordTextBox = clControl.find('[sa-elementtype=password]');
                    clLogInIDTextBox = clControl.find('[sa-elementtype=loginid]');
                    clLoginButton = clControl.find('[sa-elementtype=loginbutton]');
                    clStatusBar = clControl.find('[sa-elementtype=statusbar]');
                    
                    POSLogInManager.BindEvents();
                },
                BindEvents : function()
                {
                    clLogInIDTextBox.unbind("keydown");
                    clLogInIDTextBox.keydown(POSLogInManager.HandlerOnKeyDown);

                    clPasswordTextBox.unbind("keydown");
                    clPasswordTextBox.keydown(POSLogInManager.HandlerOnKeyDown);

                    clLoginButton.unbind("click");
                    clLoginButton.click(POSLogInManager.HandlerOnClick);
                    
                    clControl.find('a[ea-command="@cmd%resetpassword"]').unbind("click");
                    clControl.find('a[ea-command="@cmd%resetpassword"]').click(POSLogInManager.HandlerOnClick);
                },
                VerifyInputs : function()
                {                    
                    if (!clLogInIDTextBox.val().trim()) {                 
                        clStatusBar.text(MessageHandler.GetMessage('err_nologinid'));
                        setTimeout(function () { clStatusBar.text(""); }, 5000);
                        clLogInIDTextBox.focus();
                        return (false);
                    }
                    else if (!clPasswordTextBox.val().trim()) {
                        clStatusBar.text(MessageHandler.GetMessage('err_nopincode'));
                        setTimeout(function () { clStatusBar.text(""); }, 5000);
                        clPasswordTextBox.focus();
                        return (false);
                    }
                    else return (true);
                },
                LogIn : function()
                {
                    if (POSLogInManager.VerifyInputs()) {
                        var lcAjaxRequestManager = new AjaxRequestManager('login', null, 'err_login', 'ajax_requesting');
                        lcAjaxRequestManager.AddAjaxParam('LogInID', clLogInIDTextBox.val().NormalizeNumber().trim());
                        lcAjaxRequestManager.AddAjaxParam('Password', $.md5(clPasswordTextBox.val().NormalizeNumber().trim()));

                        lcAjaxRequestManager.SetCompleteHandler(function (paSuccess, paResponseStruct) {
                            if (paSuccess) {
                                var lcLandingPage = clControl.attr('ea-landingpage');
                                setTimeout(function () { FormManager.RedirectPage(lcLandingPage, false); }, 10);
                            }
                        });

                        lcAjaxRequestManager.Execute();
                    }
                },
                HandlerOnKeyDown : function(paEvent)
                {
                    var lcElementType = $(this).attr('sa-elementtype');
                    var lcKeyCode     = (paEvent.keyCode ? paEvent.keyCode : paEvent.which);

                    switch(lcElementType)
                    {
                        case 'loginid':
                            {
                                if (lcKeyCode == '13') {
                                    clPasswordTextBox.focus();
                                }

                                break;
                            }

                        case 'password':
                            {
                                if (lcKeyCode == '13') {
                                    clLoginButton.trigger("click");
                                }

                                break;
                            }
                    }
                },
                HandlerOnClick: function (paEvent)
                {
                    paEvent.preventDefault();
                    
                    var lcCommand = $(this).attr('ea-command');
                    lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);

                    switch (lcCommand)
                    {                        
                       case 'login':
                            {
                                POSLogInManager.LogIn();
                                break;
                            }

                        case 'resetpassword':
                            {
                                var lcFormName = $(this).attr('ea-parameter');
                                var lcFormLink = "?_f=" + Base64.encode(lcFormName);

                                FormManager.RedirectStatefulBase64Link(lcFormLink, false);

                                break;
                            }

                    }
                }
           }
})();
