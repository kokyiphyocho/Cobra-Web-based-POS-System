$(document).ready(function () {    
    //$('[sa-elementtype=toolbar]').BindToolBarEvents();
    //$('[sa-elementtype=toolbar]').SetCurrentItem();    
    ToolBarManager.Init();
});


var ToolBarManager = (function () {
    var     clToolBar;

    return  {
                Init : function()
                {                    
                    clToolBar = $('[sa-elementtype=toolbar]');
                 
                    ToolBarManager.BindEvents();
                },
                GetToolBar : function()
                {                 
                    if (!clToolBar) clToolBar = $('[sa-elementtype=toolbar]'); 
                    return (clToolBar);
                },
                BindEvents : function()
                {                    
                    clToolBar.find('a[ea-command^="@cmd%toollink"]').unbind('click');
                    clToolBar.find('a[ea-command^="@cmd%toollink"]').click(function (paEvent) {
                            paEvent.preventDefault();
                            
                            var lcToolIcon  = $(this);                            
                            var lcCurrent   = lcToolIcon.attr('fa-current');
                            var lcMode      = lcToolIcon.attr('ea-mode');

                            if (lcCurrent != 'true') {
                                if (lcMode == 'strict') {
                                    SecurityController.ShowPasswordPopUp().done(function (paResult) {
                                        if (paResult) {
                                            setTimeout(function () { lcToolIcon.TakeToolLinkFormAction() }, 200);
                                        }
                                    });
                                }
                                else lcToolIcon.TakeToolLinkFormAction();
                            }
                    });
                },
                SetCurrentItem : function()
                {
                    var lcFormName = FormManager.GetForm().attr('ea-formname');                  
                    lcToolBar.find('a[ea-command^="@cmd%toollink"][ea-formname=' + lcFormName + ']').attr('fa-current', 'true');
                },
                RunToolLink : function(paToolIcon) {                
                        var lcLink = paToolIcon.attr('href').substring(13);                        
                        var lcPopUpMode = lcToolIcon.attr('ea-action');
    
                        if (lcPopUpMode == 'popup') {
                            FormManager.RedirectStatefulBase64Link(lcLink);
                        }
                        else FormManger.RedirectPage(lcLink, false);
                }
            
            } // return
})();


//$.fn.BindToolBarEvents = function () {
    
//    $(this).find('a[href^="@cmd%toollink"]').unbind('click');
//    $(this).find('a[href^="@cmd%toollink"]').click(function (paEvent) {
//        paEvent.preventDefault();

//        var lcToolIcon = $(this);        
//        var lcForm = $(this).closest('[sa-elementtype=form]');
//        var lcCurrent = lcToolIcon.attr('fa-current');
//        var lcMode = lcToolIcon.attr('ea-mode');

//        if (lcCurrent != 'true')
//        {
//            if (lcMode == 'strict') {
//                SecurityController.ShowPasswordPopUp(lcForm).done(function (paResult) {
//                    if (paResult) {
//                        setTimeout(function () { lcToolIcon.TakeToolLinkFormAction() }, 200);
//                    }
//                });
//            }
//            else lcToolIcon.TakeToolLinkFormAction();
//        }            
//    });
//}

//$.fn.TakeToolLinkFormAction = function () {
//    var lcToolIcon = $(this);
//    var lcForm = $(this).closest('[sa-elementtype=form]');
//    var lcLink = $(this).attr('href').substring(13);
//    var lcFormStack = lcForm.attr('ea-formstack');
//    var lcPopUpMode = lcToolIcon.attr('ea-action');
    
//    if (lcPopUpMode == 'popup') {
//        if (!lcFormStack) lcFormStack = Base64.decode(lcForm.attr('ea-encodedformname'));
//        else lcFormStack = Base64.decode(lcFormStack) + '||' + Base64.decode(lcForm.attr('ea-encodedformname'));        
//        lcLink = lcLink + '&_s=' + encodeURIComponent(Base64.encode(lcFormStack));
//    }

//    RedirectPage(lcLink, false);
//}

//$.fn.SetCurrentItem = function()
//{
//    var lcToolBar = $(this);
//    var lcForm = $(this).closest('[sa-elementtype=form]');
//    var lcFormName = lcForm.attr('ea-formname');
    
//    lcToolBar.find('a[href^="@cmd%toollink"][ea-formname=' + lcFormName + ']').attr('fa-current', 'true');
//}

