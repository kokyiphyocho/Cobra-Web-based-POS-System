$(document).ready(function () {
    POSSubscriptionInfoManager.Init();
});

var POSSubscriptionInfoManager = (function () {

    var clControl;    
    var clButtonPanel;

    return {
        Init: function () {
            clControl = $('[sa-elementtype=control].WidControlPOSSubscriptionInfo');            
            clButtonPanel = clControl.find('.ButtonPanel');
            
            POSSubscriptionInfoManager.BindEvents();

        },
        BindEvents: function () {
            clButtonPanel.find('[ea-command^="@cmd%"]').unbind('click');
            clButtonPanel.find('[ea-command^="@cmd%"]').click(POSSubscriptionInfoManager.HandlerOnClick);
        },        
        HandlerOnClick: function (paEvent) {
            paEvent.preventDefault();

            var lcCommand = $(this).attr('ea-command');
            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);

            switch (lcCommand) {
                case 'close':
                    {
                        FormManager.CloseForm();
                        break;
                    }
            }

        }
    }
})();
