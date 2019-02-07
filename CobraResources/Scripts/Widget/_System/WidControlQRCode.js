$(document).ready(function () {
    var lcForm = $(document).find('[sa-elementtype=form]');
    var lcToolBar = lcForm.find('[sa-elementtype=toolbar]');

    $('[sa-elementtype=control].WidControlQRCode').BindQRCodeEvents();
    lcToolBar.find('a').first().trigger('click');
});

$.fn.BindQRCodeEvents = function () {
    var lcForm = $(this).closest('[sa-elementtype=form]');


    lcForm.find('a[href="@cmd%andriodfrontend"],a[href="@cmd%andriodbackend"],a[href="@cmd%iosfrontend"],a[href="@cmd%iosbackend"]').unbind('click');
    lcForm.find('a[href="@cmd%andriodfrontend"],a[href="@cmd%andriodbackend"],a[href="@cmd%iosfrontend"],a[href="@cmd%iosbackend"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcForm = $(this).closest('[sa-elementtype=form]');
        var lcToolBar = $(this).closest('[sa-elementtype=toolbar]');
        var lcControl = lcForm.find('[sa-elementtype=control]');
        var lcAttribute = $(this).attr('href').substring(5);
                
        $(this).siblings().removeAttr('fa-current');
        $(this).attr('fa-current', 'true');

        lcControl.attr('ea-attribute', lcAttribute);
        
    });

}
