$(document).ready(function () {
    $('[sa-elementtype=control].WidControlMobileStoreFront [sa-elementtype=container].ItemContainer [sa-elementtype=grid]').BindMobileStoreFrontGridItemEvents();    
});

$.fn.BindMobileStoreFrontGridItemEvents = function () {
    
    $(this).find('a[href="@cmd%showdetailinfo"]').unbind('click');
    $(this).find('a[href="@cmd%showdetailinfo"]').click(function (paEvent) {
        paEvent.preventDefault();
        
        var lcGridContainer = $(this).closest('[sa-elementtype=container]');
        var lcGridItem = $(this).closest('[sa-elementtype=griditem]');
        var lcDetailPopUp = lcGridItem.find('[sa-elementtype=popup]');
        var lcDetailInfoOverlay = lcGridContainer.find('[sa-elementtype=overlay]');

        lcDetailInfoOverlay.html(lcDetailPopUp.clone(true));

        lcDetailInfoOverlay.attr('fa-showdetailinfo', 'true');

    });

    $(this).find('a[href="@cmd%closedetailinfo"]').unbind('click');
    $(this).find('a[href="@cmd%closedetailinfo"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcDetailInfoOverlay = $(this).closest('[sa-elementtype=overlay]');

        lcDetailInfoOverlay.removeAttr('fa-showdetailinfo');

    });


}
