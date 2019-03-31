$(document).ready(function () {

    var lcImageUploadPanel = $('[sa-elementtype=composite][ea-type=imageuploader]');
    var lcA = new ImageUploadController(lcImageUploadPanel, null);
    lcA.Init();
    
   // PaymentManager.Init();
});