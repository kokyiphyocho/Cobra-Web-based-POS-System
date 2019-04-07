var ePOSPrinterManager = (function () {
    var clControl;
    var clPrinter;
    var clePOSDevice;
    var clCanvas;    
    var clStatus;
     // var clMessage;
    var clPrinterSetting    = {};
    var clErrorDictionary;
    var clErrorPriority;
    var clSuppressReconnect;

    return {
                Init: function (paControl, paPrinterSetting)
                {
                    var lcDeferred = $.Deferred();

                    clControl   = paControl;
                    clPrinter   = null;
                    clStatus = 'pending';
                    clSuppressReconnect = false;
                    // clMessage = '';                                        
                    
                    ePOSPrinterManager.WaitForDependencies().done(function () {                    
                        ePOSPrinterManager.SetPrinterSetting(paPrinterSetting);
                        clePOSDevice = new epson.ePOSDevice();                        
                        lcDeferred.resolve();
                    });

                    return (lcDeferred);
                },
                Connect : function(paPrinterSetting, paSuppressReconnect)
                {                    
                    clSuppressReconnect = paSuppressReconnect || false;
                    ePOSPrinterManager.SetPrinterSetting(paPrinterSetting);

                    if (clePOSDevice) {
                        ePOSPrinterManager.NotifyStatus({ event: 'connecting' })
                        clePOSDevice.connect(clPrinterSetting.IPAddress, clPrinterSetting.Port, ePOSPrinterManager.HandlerOnConnect);
                    }
                },
                WaitForDependencies : function () {
                    var lcDeferred = $.Deferred();

                    var lcWaitTimer = setInterval(function () {
                        if (typeof epson.ePOSDevice !== 'undefined')
                        {
                            if (lcDeferred.state() == 'pending') {
                                lcDeferred.resolve();
                                clearInterval(lcWaitTimer);
                            }
                        }
                    }, 200);

                    return (lcDeferred);
                },
                CompileErrorDictionary : function()
                {                    
                    clErrorDictionary = {};
                    clErrorDictionary[clPrinter.ASB_RECEIPT_NEAR_END]   = 'err_printer_receiptnearend';
                    clErrorDictionary[clPrinter.ASB_RECEIPT_END]        = 'err_printer_receiptend';
                    clErrorDictionary[clPrinter.ASB_COVER_OPEN]         = 'err_printer_coveropen';
                    clErrorDictionary[clPrinter.ASB_AUTOCUTTER_ERR]     = 'err_printer_autocuttererror';
                    clErrorDictionary[clPrinter.ASB_NO_RESPONSE]        = 'err_printer_noresponse';                    
                    clErrorDictionary[clPrinter.ASB_OFF_LINE]           = 'err_printer_offline';                    
                    clErrorDictionary[clPrinter.ASB_MECHANICAL_ERR]     = 'err_printer_mechanicalerror';                    
                    clErrorDictionary[clPrinter.ASB_UNRECOVER_ERR]      = 'err_printer_unrecoverableerror';

                    clErrorPriority = [
                                        clPrinter.ASB_OFF_LINE,
                                        clPrinter.ASB_COVER_OPEN,                                        
                                        clPrinter.ASB_AUTOCUTTER_ERR,
                                        clPrinter.ASB_NO_RESPONSE,                                       
                                        clPrinter.ASB_MECHANICAL_ERR,
                                        clPrinter.ASB_UNRECOVER_ERR,
                                        clPrinter.ASB_RECEIPT_NEAR_END,
                                        clPrinter.ASB_RECEIPT_END                                       
                                       ]                    
                },
                SetPrinterSetting : function(paPrinterSetting)
                {                    
                    $.extend(clPrinterSetting, paPrinterSetting);

                    clPrinterSetting.IPAddress  = clPrinterSetting.IPAddress || '';
                    clPrinterSetting.Port       = clPrinterSetting.Port || 8008;
                    clPrinterSetting.DeviceID   = clPrinterSetting.DeviceID;
                    clPrinterSetting.Darkness   = clPrinterSetting.Darkness || 10;
                    clPrinterSetting.Crypto     = clPrinterSetting.Crypto ? true : false;
                    clPrinterSetting.Buffer     = clPrinterSetting.Buffer ? true : false;                    
                },
                HandlerOnConnect: function (paData)
                {                    
                    if (paData == 'OK')
                    {                 
                        clePOSDevice.createDevice(clPrinterSetting.DeviceID, clePOSDevice.DEVICE_TYPE_PRINTER, { 'crypto': clPrinterSetting.Crypto, 'buffer': clPrinterSetting.Buffer }, ePOSPrinterManager.HandlerOnCreateDevice);
                    }
                    else
                    {                        
                        clStatus = 'fail';
                    //    clMessage = paData;                        
                        ePOSPrinterManager.NotifyStatus({ event : 'initfail', errorcode: 'err_printer_connectionfail' });

                        if ((!clSuppressReconnect) && (clPrinterSetting.ReconnectInterval))
                            setTimeout(ePOSPrinterManager.Connect, clPrinterSetting.ReconnectInterval);
                    }
                },
                HandlerOnCreateDevice: function (paDeviceObject, paReturnCode)
                {                    
                    if (paReturnCode == 'OK')
                    {                        
                        clPrinter = paDeviceObject;
                        clStatus = 'connected';                        
                        ePOSPrinterManager.NotifyStatus({ event : 'initsuccess' });
                        ePOSPrinterManager.BindEvents();
                        ePOSPrinterManager.StartMonitor();
                    }
                    else
                    {
                        clStatus = 'fail';
                        // clMessage = paReturnCode;                        
                        ePOSPrinterManager.NotifyStatus({ event : 'initfail', errorcode: 'err_printer_initfail', errorparam: paReturnCode });

                        if (clPrinterSetting.ReconnectInterval)
                            setTimeout(ePOSPrinterManager.Connect, clPrinterSetting.ReconnectInterval);
                    }                    
                },
                BindEvents : function()
                {
                    // OnReceiveHandler
                    clPrinter.onreceive  =  function (paReceiveInfo)
                                            {
                                                if ((paReceiveInfo.fail))
                                                {
                                                    ePOSPrinterManager.NotifyStatus({ event: 'printfail', errorcode: 'err_printer_printfail', errorparam: paReceiveInfo.code });
                                                }
                                            }

                    // StatusChangedHandler
                    clPrinter.onstatuschange =  function(paStatus)
                                                {                                                       
                                                    ePOSPrinterManager.HandlePrinterStatus(paStatus);
                                                }                    
                },
                StartMonitor : function()
                {                    
                    if (clPrinterSetting.MonitorInterval)
                    {                        
                        clPrinter.interval = clPrinterSetting.MonitorInterval;
                        clPrinter.startMonitor();                        
                    }
                },
                StopMonitor : function()
                {
                    clPrinter.stopMonitor();
                },
                NotifyStatus : function(paEventInfo)
                {                    
                    clControl.trigger('ev-printernotification', paEventInfo);
                },
                TestErrorCode : function(paStatus, paErrorCode)
                {
                    if ((paStatus & paErrorCode) == paErrorCode) return ({ event: 'statuschanged', errorcode: clErrorDictionary[paErrorCode] });
                    else return (null);
                },
                HandlePrinterStatus : function(paStatus)
                {
                    var lcErrorDetect = false;
                    
                    if ((!clErrorDictionary) || (!clErrorPriority)) ePOSPrinterManager.CompileErrorDictionary();                    
                    
                    $.each(clErrorPriority, function (paIndex, paValue)
                    {
                        var lcErrorInfo;
                        
                        lcErrorInfo = ePOSPrinterManager.TestErrorCode(paStatus, paValue);
                        if (lcErrorInfo) 
                        {                            
                            ePOSPrinterManager.NotifyStatus(lcErrorInfo);
                            lcErrorDetect = true;
                            return (false);
                        }
                    });
                    
                    if (!lcErrorDetect) ePOSPrinterManager.NotifyStatus({ event: 'statuschanged', errorcode: 'none' });
                },
                WaitForConnection: function ()
                {
                    var lcDeferred = $.Deferred();                    

                    var lcWaitTimer = setInterval(function ()
                    {
                        if ((clStatus != 'pending') && (lcDeferred.state() == 'pending'))
                        {                            
                            lcDeferred.resolve(clStatus);
                            clearInterval(lcWaitTimer);
                        }
                    }, 200);

                    return (lcDeferred);
                },
                Print: function (paCanvas)
                {
                    if (clStatus == 'pending') ePOSPrinterManager.WaitForConnection().done(function (paStatus)
                    {                        
                        ePOSPrinterManager.SendToPrinter(paCanvas);
                    });
                    else
                    {                     
                        ePOSPrinterManager.SendToPrinter(paCanvas);
                    }
                },
                SetPrinterDarkness: function (paDarkness)
                {
                    clPrinterSetting.Darkness = paDarkness;
                },
                SendToPrinter: function ( paCanvas)
                {
                    if (clStatus == 'connected')
                    {
                        clPrinter.brightness    = CastDecimal(10 - (clPrinterSetting.Darkness * 0.1) + 0.1, 1);
                        clPrinter.halftone = clPrinter.HALFTONE_DITHER;
                        clPrinter.addImage(paCanvas.getContext('2d'), 0, 0, paCanvas.width, paCanvas.height, clPrinter.COLOR_1, clPrinter.MODE_MONO);                        
                        clPrinter.addCut(clPrinter.CUT_FEED);
                        clPrinter.send();
                    }
                },
               
            }
    })();