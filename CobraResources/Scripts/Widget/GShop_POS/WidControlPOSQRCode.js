$(document).ready(function () {    
    POSQRCodeManager.Init($('[sa-elementtype=control].WidControlPOSQRCode'));    
});

var POSQRCodeManager = (function () {    
    var clToolBar;
    var clControl;
    var clForm;

    return {
                Init: function (paPOSQRCodeControl) {
                            clControl = paPOSQRCodeControl;
                            clForm = clControl.closest('[sa-elementtype=form]');
                            clToolBar = clForm.find('[sa-elementtype=toolbar]');

                            POSQRCodeManager.BindEvents();
                            clToolBar.find('a[ea-command]').first().trigger('click');
                        },

                        BindEvents : function(){
                            clToolBar.find('a[ea-command]').unbind('click');
                            clToolBar.find('a[ea-command]').click(POSQRCodeManager.HandlerOnClick);
                        },
                        HandlerOnClick : function(paEvent)
                        {
                            paEvent.preventDefault();
            
                            var lcCommand = $(this).attr('ea-command');
                            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);
            
                            switch (lcCommand) 
                            {
                                case "andriod":
                                case "ios":
                                    {                        
                                        $(this).siblings().removeAttr('fa-current');
                                        $(this).attr('fa-current', 'true');
                                        clControl.attr('ea-attribute',lcCommand);
                                        break;
                                    }
                              
                            }
                        }
           }
})();

//$.fn.BindQRCodeEvents = function () {
//    var lcForm = $(this).closest('[sa-elementtype=form]');


//    lcForm.find('a[ea-command].unbind('click');
//    lcForm.find('a[href="@cmd%andriodfrontend"],a[href="@cmd%andriodbackend"],a[href="@cmd%iosfrontend"],a[href="@cmd%iosbackend"]').click(function (paEvent) {
//        paEvent.preventDefault();

//        var lcForm = $(this).closest('[sa-elementtype=form]');
//        var lcToolBar = $(this).closest('[sa-elementtype=toolbar]');
//        var lcControl = lcForm.find('[sa-elementtype=control]');
//        var lcAttribute = $(this).attr('href').substring(5);

//        $(this).siblings().removeAttr('fa-current');
//        $(this).attr('fa-current', 'true');

//        lcControl.attr('ea-attribute', lcAttribute);

//    });

//}
