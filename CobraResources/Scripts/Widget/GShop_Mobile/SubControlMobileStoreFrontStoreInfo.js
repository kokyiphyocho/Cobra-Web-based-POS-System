$(document).ready(function () {
    $('.StoreInfoPanel[sa-elementtype=panel]').BindStoreInfoPanelEvents();    
});

$.fn.BindStoreInfoPanelEvents = function () {
    $(this).find('a[href="@cmd%close"]').unbind('click');
    $(this).find('a[href="@cmd%close"]').click(function (paEvent) {

        paEvent.preventDefault();

        var lcControl = $(this).closest('[sa-elementtype=control]');

        lcControl.removeAttr('fa-showpopup');
    });
}
