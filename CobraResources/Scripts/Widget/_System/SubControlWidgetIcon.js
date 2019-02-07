$(document).ready(function () {
    // $('[sa-elementtype=element].SubControlWidgetIcon').BindWidgetIconEvents();
    WidgetIconManager.Init();
});


var WidgetIconManager = (function () {
    var clControl;
    var clForm;
    var clDemoMode;

    return {
                Init : function()
                {                   
                    clControl   = $('[sa-elementtype=element].SubControlWidgetIcon');
                    clForm      = clControl.closest('[sa-elementtype=form]');
                    clDemoMode  = clForm.attr('ea-demomode') == 'true';
                    WidgetIconManager.BindEvents();
                },

                BindEvents : function()
                {                    
                    var lcFormLink = clControl.find('div[ea-command^="@cmd%formlink"]');
                    var lcHyperLink = clControl.find('div[ea-command^="@cmd%link"]');

                    lcFormLink.unbind('click');
                    lcFormLink.click(function (paEvent) {
                        paEvent.preventDefault();
                        paEvent.stopPropagation();

                        var lcWidget = $(this);                        
                        var lcMode = lcWidget.attr('ea-mode');
                        
                        if ((!clDemoMode) && (lcMode == 'strict')) {
                            SecurityController.ShowPasswordPopUp().done(function (paResult) {
                                if (paResult) {
                                    setTimeout(function () { WidgetIconManager.RunWidget(lcWidget); }, 200);
                                }
                            });
                        }
                        else WidgetIconManager.RunWidget(lcWidget);
                    });

                    lcHyperLink.unbind('click');
                    lcHyperLink.click(function (paEvent) {
                        paEvent.preventDefault();
                        paEvent.stopPropagation();
                        window.location = $(this).attr('ea-command').substring(10);
                    });
                },

                RunWidget : function(paWidget)
                {                    
                    var lcLink = paWidget.attr('ea-command').substring(13);                    
                    FormManager.RedirectStatefulBase64Link(lcLink);
                }       
           }
})();

//$.fn.BindWidgetIconEvents = function () {

//    var lcForm = $(this).closest('[sa-elementtype=form]');
//    var lcFormLink = $(this).find('a[href^="@cmd%formlink"]');
//    var lcHyperLink = $(this).find('a[href^="@cmd%link"]');
    
//    lcFormLink.unbind('click');
//    lcFormLink.click(function (paEvent) {
//        paEvent.preventDefault();

//        var lcWidget = $(this);
//        var lcForm = $(this).closest('[sa-elementtype=form]');
//        var lcMode = $(this).attr('ea-mode');

//        if (lcMode == 'strict') {
//            SecurityController.ShowPasswordPopUp(lcForm).done(function (paResult) {
//                if (paResult) {
//                    setTimeout(function () { lcWidget.TakeWidgetFormAction() }, 200);
//                }
//            });
//        }
//        else lcWidget.TakeWidgetFormAction();
              
//    });

//    lcHyperLink.unbind('click');
//    lcHyperLink.click(function (paEvent) {
//        paEvent.preventDefault();
//        window.location =$(this).attr('href').substring(10);
//    });
//}

//$.fn.TakeWidgetFormAction = function()
//{
//    var lcForm = $(this).closest('[sa-elementtype=form]');
//    var lcLink = $(this).attr('href').substring(13);
//    var lcFormStack = lcForm.attr('ea-formstack');    


//    if (!lcFormStack) lcFormStack = Base64.decode(lcForm.attr('ea-encodedformname'));
//    else lcFormStack = Base64.decode(lcFormStack) + '||' + Base64.decode(lcForm.attr('ea-encodedformname'));

//    lcLink = lcLink + '&_s=' + encodeURIComponent(Base64.encode(lcFormStack));
//    RedirectPage(lcLink, false);
//}
