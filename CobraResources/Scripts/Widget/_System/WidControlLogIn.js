$(document).ready(function () {    
    // $('[sa-elementtype=control].WidControlLogIn').each(function () { $(this).BindLogInEvents(); });    
    LogInManager.Init();
});

var LogInManager = (function () {
    var clControl;
    var clPasswordTextBox;
    var clUserNameTextBox;
    var clLoginButton;
    var clStatusBar;

    return {
                Init : function()
                {
                    clControl = $('[sa-elementtype=control].WidControlLogIn');
                    clPasswordTextBox = clControl.find('[sa-elementtype=password]');
                    clUserNameTextBox = clControl.find('[sa-elementtype=username]');
                    clLoginButton = clControl.find('[sa-elementtype=loginbutton]');
                    clStatusBar = clControl.find('[sa-elementtype=statusbar]');

                    LogInManager.BindEvents();
                },
                BindEvents : function()
                {
                    clUserNameTextBox.unbind("keydown");
                    clUserNameTextBox.keydown(function (paEvent) {
                        var lcKeyCode = (paEvent.keyCode ? paEvent.keyCode : paEvent.which);
                        if (lcKeyCode == '13') {                            
                            lcPasswordTextBox.focus();
                        }
                    });

                    clPasswordTextBox.unbind("keydown");
                    clPasswordTextBox.keydown(function (paEvent) {
                        var lcKeyCode = (paEvent.keyCode ? paEvent.keyCode : paEvent.which);
                        if (lcKeyCode == '13') {                            
                            clLoginButton.trigger("click");
                        }
                    });

                    clLoginButton.unbind("click");
                    clLoginButton.click(function (paEvent) {

                        if (!clUserNameTextBox.val().trim()) {                            
                            clStatusBar.text(MessageHandler.GetMessage('err_nousername'));
                            setTimeout(function () { clStatusBar.text(""); }, 5000);
                            clUserNameTextBox.focus();
                        }

                        else if (!clPasswordTextBox.val().trim()) {
                            clStatusBar.text(MessageHandler.GetMessage('err_nopincode'));
                            setTimeout(function () { clStatusBar.text(""); }, 5000);
                            clPasswordTextBox.focus();
                        }
                        else {
                            var lcAjaxRequestManager = new AjaxRequestManager('login', null, 'err_login', 'ajax_requesting');
                            lcAjaxRequestManager.AddAjaxParam('UserName', clUserNameTextBox.val().NormalizeNumber().trim());
                            lcAjaxRequestManager.AddAjaxParam('Password',  $.md5(clPasswordTextBox.val().NormalizeNumber().trim()));

                            lcAjaxRequestManager.SetCompleteHandler(function (paSuccess, paResponseStruct) {
                                if (paSuccess) {
                                    var lcLandingPage = clControl.attr('ea-landingpage');
                                    setTimeout(function () { FormManager.RedirectPage(lcLandingPage, false); }, 10);
                                }
                            });

                            lcAjaxRequestManager.Execute();
                        }
                    });
                    

                    clControl.find('a[href="@cmd%createaccount"]').unbind("click");
                    clControl.find('a[href="@cmd%createaccount"]').click(function (paEvent) {
                        paEvent.preventDefault();
                        
                        var lcLink = $(this).attr('ea-parameter');
                        FormManager.RedirectStatefulBase64Link(lcLink);
                    });
                }
           }
})();

//$.fn.BindLogInEvents = function () {    
//    $(this).find('[sa-elementtype=username]').unbind("keydown");
//    $(this).find('[sa-elementtype=username]').keydown(function (paEvent) {
//        var lcKeyCode = (paEvent.keyCode ? paEvent.keyCode : paEvent.which);
//        if (lcKeyCode == '13') {
//            var lcPasswordTextBox = $(this).closest('[sa-elementtype=form]').find('[sa-elementtype=password]');
//            lcPasswordTextBox.focus();
//        }
//    });

//    $(this).find('[sa-elementtype=password]').unbind("keydown");
//    $(this).find('[sa-elementtype=password]').keydown(function (paEvent) {
//        var lcKeyCode = (paEvent.keyCode ? paEvent.keyCode : paEvent.which);
//        if (lcKeyCode == '13') {
//            var lcLogInButton = $(this).closest('[sa-elementtype=form]').find('[sa-elementtype=loginbutton]');
//            lcLogInButton.trigger("click");
//        }
//    });

//    $(this).find('button[type=button][sa-elementtype=loginbutton]').unbind("click");
//    $(this).find('button[type=button][sa-elementtype=loginbutton]').click(function (paEvent) {
//        var lcForm = $(this).closest('[sa-elementtype=form]');
//        var lcControl = $(this).closest('[sa-elementtype=control]');
//        var lcLogInButton = $(this);
//        var lcUserNameTextBox = lcForm.find('[sa-elementtype=username]');
//        var lcPasswordTextBox = lcForm.find('[sa-elementtype=password]');
//        var lcStatusBar = lcForm.find('[sa-elementtype=statusbar]');
        
//        if (!lcUserNameTextBox.val().trim()) {
//            lcStatusBar.text(lcUserNameTextBox.attr('sa-message'));
//            setTimeout(function () { lcStatusBar.text(""); }, 5000);
//            lcUserNameTextBox.focus();
//        }

//        else if (!lcPasswordTextBox.val().trim()) {
//            lcStatusBar.text(lcPasswordTextBox.attr('sa-message'));
//            setTimeout(function () { lcStatusBar.text(""); }, 5000);
//            lcPasswordTextBox.focus();            
//        }
//        else {            
//            GlobalAjaxHandler.SetAjaxLoaderStatusText('Logging in.....');
//            /* _e : lcForm.attr('ea-servicerequesttoken'), */
//            var lcData = { CobraAjaxRequest: "login", UserName: lcUserNameTextBox.val().NormalizeNumber().trim(), Password: $.md5(lcPasswordTextBox.val().NormalizeNumber().trim()) };
//            DoPostBack(lcData, function (paResponseData) {
//                var lcRespondStruct = jQuery.parseJSON(paResponseData);
//                if (lcRespondStruct.Success) {                                        
//                    var lcLandingPage = lcControl.attr('ea-landingpage');
                    
//                    /* lcLandingPage = lcLandingPage.replace('$SERVICETOKEN', lcForm.attr('ea-servicerequesttoken')); */
//                    setTimeout(function () { RedirectPage(lcLandingPage, false); }, 10);
//                }
//                else {
//                    lcStatusBar.text("Invalid user name or password");
//                    setTimeout(function () { lcStatusBar.text(""); }, 5000);
//                }
//            });
//        }
//    });

//    $(this).find('a[href="@cmd%createaccount"]').unbind("click");
//    $(this).find('a[href="@cmd%createaccount"]').click(function (paEvent) {
//        paEvent.preventDefault();
//        var lcForm = $(this).closest('[sa-elementtype=form]');
//        var lcLink = $(this).attr('ea-parameter');
        
//        if (lcLink) {
//            var lcFormStack = lcForm.attr('ea-formstack');

//            if (!lcFormStack) lcFormStack = Base64.decode(lcForm.attr('ea-encodedformname'));
//            else lcFormStack = Base64.decode(lcFormStack) + '||' + Base64.decode(lcForm.attr('ea-encodedformname'));

//            lcLink = "?_f=" + encodeURIComponent(Base64.encode(lcLink)) + '&_s=' + encodeURIComponent(Base64.encode(lcFormStack));
//            RedirectPage(lcLink, false);
//        }
//    });
    
//}