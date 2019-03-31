var StarPrinterManager = (function () {
    var clControl;
    var clPrinter;
    var clStarWebPrintTrader;
    var clCanvas;
    var clStatus;
    var clCurrentState;
    var clMonitorInterval;
        
    var clPrinterSetting = {};    
    var clSuppressReconnect;

    return {
        Init: function (paControl, paPrinterSetting) {
            var lcDeferred = $.Deferred();            
            clControl           = paControl;
            clPrinter           = null;
            clStatus            = 'pending';
            clSuppressReconnect = false;       
            clCurrentState      = '';
            clMonitorInterval   = 0;

            StarPrinterManager.WaitForDependencies().done(function () {
                
                StarPrinterManager.SetPrinterSetting(paPrinterSetting);                
                lcDeferred.resolve();
            });

            return (lcDeferred);
        },
        Connect: function (paPrinterSetting, paSuppressReconnect) {            
            clSuppressReconnect = paSuppressReconnect || false;
            StarPrinterManager.SetPrinterSetting(paPrinterSetting);            

            clCurrentState = 'initializing';
            clStarWebPrintTrader = new StarWebPrintTrader(StarPrinterManager.GetConnectionParam());
            StarPrinterManager.BindEvents();

            var lcStarWebPrintBuilder = new StarWebPrintBuilder();
            var lcRequest = lcStarWebPrintBuilder.createInitializationElement();            
            StarPrinterManager.NotifyStatus({ event: 'connecting' });            

            clStarWebPrintTrader.sendMessage(lcRequest);
        },
        BindEvents: function () {
            // OnReceiveHandler            
            clStarWebPrintTrader.onReceive = StarPrinterManager.HandlerOnReceive;
            // OnErrorHandler
            clStarWebPrintTrader.onError = StarPrinterManager.HandlerOnError;
            //clPrinter.onreceive = function (paReceiveInfo) {
            //    if ((paReceiveInfo.fail)) {
            //        StarPrinterManager.NotifyStatus({ event: 'printfail', errorcode: 'err_printer_printfail', errorparam: paReceiveInfo.code });
            //    }
            //}

            //// StatusChangedHandler
            //clPrinter.onstatuschange = function (paStatus) {
            //    StarPrinterManager.HandlePrinterStatus(paStatus);
            //}
        },
        WaitForDependencies: function () {
            var lcDeferred = $.Deferred();

            var lcWaitTimer = setInterval(function () {
                if ((typeof StarWebPrintTrader !== 'undefined') && (typeof StarWebPrintBuilder !== 'undefined')) {
                    if (lcDeferred.state() == 'pending') {
                        lcDeferred.resolve();
                        clearInterval(lcWaitTimer);
                    }
                }
            }, 200);

            return (lcDeferred);
        },
        TranslateStatus : function (paTraderStatus) {
            if      (clStarWebPrintTrader.isOffLine                 ({traderStatus:paTraderStatus})) return('err_printer_offline');
            else if (clStarWebPrintTrader.isCoverOpen               ({ traderStatus: paTraderStatus })) return ('err_printer_coveropen');
            else if (clStarWebPrintTrader.isAutoCutterError         ({ traderStatus: paTraderStatus })) return ('err_printer_autocuttererror');
            else if (clStarWebPrintTrader.isHighTemperatureStop     ({ traderStatus: paTraderStatus })) return ('err_printer_hightemperaturestop');
            else if (clStarWebPrintTrader.isNonRecoverableError     ({ traderStatus: paTraderStatus })) return ('err_printer_unrecoverableerror');
            else if (clStarWebPrintTrader.isPaperEnd                ({ traderStatus: paTraderStatus })) return ('err_printer_receiptend');
            else if (clStarWebPrintTrader.isPaperNearEnd            ({ traderStatus: paTraderStatus })) return ('err_printer_receiptnearend');

            return('none');
        },
        SetPrinterSetting: function (paPrinterSetting) {
            $.extend(clPrinterSetting, paPrinterSetting);

            clPrinterSetting.IPAddress = clPrinterSetting.IPAddress || '';            
        },
        GetConnectionParam : function()
        {
            return ({
                        url: "http://" + clPrinterSetting.IPAddress + "/StarWebPRNT/SendMessage",
                        checkedblock: false,
                        papertype : 'normal'
                    });
        },
        HandlerOnReceive : function(paResponse)
        {
            clStatus = 'connected';

            if (clCurrentState == 'initializing') {                                
                StarPrinterManager.NotifyStatus({ event: 'initsuccess' });                
                if (clMonitorInterval == 0) StarPrinterManager.StartMonitor();
            }
            else
            {                
                
                setTimeout(function () {                    
                    var lcResponseStatus = StarPrinterManager.TranslateStatus(paResponse.traderStatus);
             
                    if (lcResponseStatus) {
                        StarPrinterManager.NotifyStatus({ event: 'statuschanged', errorcode: lcResponseStatus });
                    }
                }, 0);                 
            }
            
            clCurrentState = 'idle';            
        },

        HandlerOnError : function(paResponse)
        {         
            if (clCurrentState == 'initializing') {                
                clStatus = 'fail';
                StarPrinterManager.NotifyStatus({ event: 'initfail', errorcode: 'err_printer_connectionfail' });

                if ((!clSuppressReconnect) && (clPrinterSetting.ReconnectInterval))
                    setTimeout(StarPrinterManager.Connect, clPrinterSetting.ReconnectInterval);
            }
            else
            {
                if (clCurrentState == 'probing')
                {
                    clStatus = 'disconected';
                    StarPrinterManager.NotifyStatus({ event: 'statuschanged', errorcode: 'err_printer_offline' });
                }
                else StarPrinterManager.NotifyStatus({ event: 'printfail', errorcode: 'err_printer_connectionfail', errorparam: paResponse.status });
            }

            clCurrentState = 'idle';
        },
        GetPrinterStatus : function()
        {        
            if (clCurrentState == 'idle') {                
                var lcStarWebPrintBuilder = new StarWebPrintBuilder();
                var lcRequest = lcStarWebPrintBuilder.createInitializationElement();
                lcRequest += lcStarWebPrintBuilder.createFeedElement({ line: 2, unit: 8 });

                clCurrentState = 'probing';
                clStarWebPrintTrader.sendMessage(lcRequest);
            }
            
            if (clMonitorInterval)
            {
                setTimeout(StarPrinterManager.GetPrinterStatus, clMonitorInterval);
            }
                
                
        },
        StartMonitor: function () {
            
            if (clPrinterSetting.MonitorInterval) {                
                clMonitorInterval = clPrinterSetting.MonitorInterval;                
                setTimeout(StarPrinterManager.GetPrinterStatus, clMonitorInterval);
            }
        },
        StopMonitor: function () {
            clMonitorInterval = 0;
        },        
        NotifyStatus: function (paEventInfo) {
            clControl.trigger('ev-printernotification', paEventInfo);
        },               
        WaitForConnection: function () {
            var lcDeferred = $.Deferred();

            var lcWaitTimer = setInterval(function () {
                if ((clStatus != 'pending') && (lcDeferred.state() == 'pending')) {
                    lcDeferred.resolve(clStatus);
                    clearInterval(lcWaitTimer);
                }
            }, 200);

            return (lcDeferred);
        },
        Print: function (paCanvas) {
            if (clStatus == 'pending') StarPrinterManager.WaitForConnection().done(function (paStatus) {
                StarPrinterManager.SendToPrinter(paCanvas);
            });
            else {
                StarPrinterManager.SendToPrinter(paCanvas);
            }
        },
        SetPrinterDarkness: function (paDarkness) {
            clPrinterSetting.Darkness = paDarkness;
        },
        SendToPrinter: function (paCanvas) {
            if (clStatus == 'connected') {

                var lcStarWebPrintBuilder = new StarWebPrintBuilder();
                var lcRequest = lcStarWebPrintBuilder.createBitImageElement({ context: paCanvas.getContext('2d'), x: 0, y: 0, width: paCanvas.width, height: paCanvas.height });

                lcRequest += lcStarWebPrintBuilder.createCutPaperElement({ feed: true });

                clStarWebPrintTrader.sendMessage({ request: lcRequest });                
            }
        },

    }
})();