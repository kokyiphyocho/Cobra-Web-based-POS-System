var PrinterManager = (function () {
    var clControl;    
    var clPrinterController;    

    return {
        Init: function (paControl)
        {
            clControl               = paControl;            
            clPrinterController     = null;            
         ////   PrinterManager.SetPrinterController();
            
         //   if (clPrinterController)
         //   {                
         //       clPrinterController.Init(clControl, paPrinterSetting);
         //   }
        },
        SetPrinterController : function(paPrinterSetting)
        {
            if (paPrinterSetting) {

                lcPrinterName = paPrinterSetting.PrinterName || '';
                
                switch (lcPrinterName) {
                    case 'EPSON':
                        {                            
                            clPrinterController = ePOSPrinterManager;
                            break;
                        }
                }
            }
        },

        Connect: function (paPrinterSetting, paSuppressReconnect)
        {            
            PrinterManager.SetPrinterController(paPrinterSetting);
            
            if (clPrinterController) {                
                clPrinterController.Init(clControl, paPrinterSetting).done(function () {                    
                    clPrinterController.Connect(paPrinterSetting, paSuppressReconnect);
                });                
            }
            else clControl.trigger('ev-printernotification', { event: 'initfail', errorcode: 'err_printer_nodriverscript' });            
        },
        SetPrinterDarkness: function (paDarkness) {
            clPrinterController.SetPrinterDarkness(paDarkness);
        },
        Print: function (paCanvas) {            
            if (clPrinterController) clPrinterController.Print(paCanvas);            
        },
        //WaitForConnection: function () {
        //    var lcDeferred = $.Deferred();            

        //    var lcWaitTimer = setInterval(function () {
        //        if ((clStatus != 'pending') && (lcDeferred.state() == 'pending')) {                    
        //            lcDeferred.resolve(clStatus);
        //            clearInterval(lcWaitTimer);
        //        }
        //    }, 200);

        //    return (lcDeferred);
        //},
    }
})();