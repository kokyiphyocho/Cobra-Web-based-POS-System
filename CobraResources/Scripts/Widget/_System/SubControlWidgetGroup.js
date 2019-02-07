$(document).ready(function () {
    // $('[sa-elementtype=element].SubControlWidgetIcon').BindWidgetIconEvents();
    WidgetGroupManger.Init();
});


var WidgetGroupManger = (function () {
    var clControl;

    return {
        Init: function () {
            clControl = $('[sa-elementtype=element].SubControlWidgetGroup');
            WidgetGroupManger.BindEvents();
        },

        BindEvents: function () {
            var lcWidgetGroupIcon = clControl.find('div[sa-elementtype=widgeticon].WidgetGroupIcon');
            var lcPopUp = clControl.find('div[sa-elementtype=popup].WidgetPopUp');
            

            lcWidgetGroupIcon.unbind('click');
            lcWidgetGroupIcon.click(function (paEvent) {
                paEvent.preventDefault();
                paEvent.stopPropagation();
                
                var lcParentControl = $(this).closest('[sa-elementtype=element].SubControlWidgetGroup');
                lcParentControl.attr('fa-showpopup', 'true');
            });

            lcPopUp.unbind('click');
            lcPopUp.click(function (paEvent) {
                paEvent.preventDefault();
                paEvent.stopPropagation();

                var lcParentControl = $(this).closest('[sa-elementtype=element].SubControlWidgetGroup');
                lcParentControl.removeAttr('fa-showpopup');
            });
        },

        RunWidget: function (paWidget) {
            var lcLink = paWidget.attr('ea-command').substring(13);
            FormManager.RedirectStatefulBase64Link(lcLink);
        }
    }
})();