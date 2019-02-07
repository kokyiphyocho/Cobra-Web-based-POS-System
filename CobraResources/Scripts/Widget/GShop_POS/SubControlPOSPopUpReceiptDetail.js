var POSPopUpReceiptDetail = function (paPopUp) {
    var clPopUp   = paPopUp;
    var clControl = paPopUp.closest('[sa-elementtype=control]');    
    

    return {
        Init: function () {
            this.BindEvents();
        },
        BindEvents: function () {
            var lcButtonList = clPopUp.find('a[ea-command="@popupcmd%close"]');
            
            lcButtonList.unbind('click');
            lcButtonList.click(this.HandlerOnClick);            
        },
        OpenPopUp: function (paPopUpType) {
            if (paPopUpType) {
                clPOSDailySummaryManager.attr('fa-activepopup', paPopUpType);
            }
        },
        HandlerOnClick: function (paEvent) {
            paEvent.preventDefault();

            var lcCommand = $(this).attr('ea-command');
            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);

            switch (lcCommand) {
                case 'close':
                    {
                        clControl.removeAttr('fa-activepopup');
                    }
            }
        }
    }

};
