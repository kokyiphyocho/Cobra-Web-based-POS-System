var POSReceiptPrintingManager = (function () {
    var     clControl;
    var     clComposite;    
    var     clCanvas;
    var     clPreviewPanel;
    var     clPreviewImage;
    var     clReceiptRenderer;
    var     clPrinterSetting;

    return {
        Init: function (paControl) {            
            clControl        = paControl;
            clComposite      = $('.SubControlPOSReceiptOutput[sa-elementtype=composite]');            
            clPrinterSetting = JSON.parse(Base64.decode(clComposite.attr('pos.primaryprintersetting') || '') || "{}");
            clCanvas         = clComposite.find('canvas');
            clPreviewPanel   = clComposite.find('[sa-elementtype=panel][ea-type=preview]');
            clPreviewImage   = clPreviewPanel.find('[sa-elementtype=container] img');
            
            POSReceiptPrintingManager.WaitForDependicies().done(function () {            
                PrinterManager.Init(clControl);                
            });            
            
            clReceiptRenderer = new RenderingController(clComposite);
            clReceiptRenderer.Init(clPrinterSetting);

            POSReceiptPrintingManager.BindEvents();
        },

        ConnectPrinter: function (paPrinterSetting, paSuppressReconnect)
        {
            if (paPrinterSetting)
            {                
                clPrinterSetting = paPrinterSetting;
                clReceiptRenderer.Init(clPrinterSetting);
            }            

            POSReceiptPrintingManager.WaitForDependicies().done(function () {
            //    paPrinterSetting = paPrinterSetting || clPrinterSetting;
                PrinterManager.Connect(clPrinterSetting, paSuppressReconnect);
            });
        },
        
        WaitForDependicies: function ()
        {
            var lcDeferred = $.Deferred();            

            var lcWaitTimer = setInterval(function () {
                    if ((typeof PrinterManager !== 'undefined') && (lcDeferred.state() == 'pending')) 
                    {                    
                        lcDeferred.resolve();
                        clearInterval(lcWaitTimer);                        
                    }
            }, 200);

            return (lcDeferred);
        },
        BindEvents : function()
        {
            clPreviewPanel.find('[ea-command]').unbind('click');
            clPreviewPanel.find('[ea-command]').click(POSReceiptPrintingManager.HandlerOnClick);
        },
        OpenPreviewWindow: function(paReceiptImage)
        {            
            if (paReceiptImage) {
                clPreviewImage.attr('src', paReceiptImage);
                clPreviewPanel.attr('fa-show', 'true');
            }
        },        
        ClosePreviewWindow : function()
        {
            clPreviewPanel.removeAttr('fa-show');
        },
        PrintOutPreviewReceipt : function ()
        {
            clReceiptRenderer.PrintCanvasImage();
        },
        PrintLoadedReceipt : function(paReceiptInfo)
        {
            if (paReceiptInfo)
            {                
                var lcReceiptDataManager = new ReceiptDataManager(clComposite, paReceiptInfo);
                lcReceiptDataManager.Init();
                clReceiptRenderer.PrintReceipt(lcReceiptDataManager);
            }
        },
        PreviewReceipt: function (paReceiptType, paReceiptID) {            
            POSReceiptPrintingManager.LoadReceipt(paReceiptType, paReceiptID).done(function (paReceiptInfoStr) {
                if (paReceiptInfoStr) {
                    var lcReceiptInfo = JSON.parse(paReceiptInfoStr);
                    var lcReceiptDataManager = new ReceiptDataManager(clComposite, lcReceiptInfo);
                    lcReceiptDataManager.Init();
                    POSReceiptPrintingManager.OpenPreviewWindow(clReceiptRenderer.GetRenderedReceiptImage(lcReceiptDataManager));
                }
            });
        },

        PrintReceipt : function(paReceiptType, paReceiptID)
        {            
            POSReceiptPrintingManager.LoadReceipt(paReceiptType, paReceiptID).done(function (paReceiptInfoStr) {
                if (paReceiptInfoStr)
                {
                    var lcReceiptInfo           = JSON.parse(paReceiptInfoStr); 
                    var lcReceiptDataManager    = new ReceiptDataManager(clComposite,lcReceiptInfo);
                    lcReceiptDataManager.Init();

                    clReceiptRenderer.PrintReceipt(lcReceiptDataManager);
                }                
            });
        },

        PrinteJSONReceipt: function (paReceiptInfo)
        {
            var lcReceiptDataManager = new ReceiptDataManager(clComposite, paReceiptInfo);
            lcReceiptDataManager.Init();

            clReceiptRenderer.PrintReceipt(lcReceiptDataManager);
        },

        LoadReceipt : function (paReceiptType, paReceiptID)
        {
            var lcDeferred = $.Deferred();

            var lcAjaxRequestManager = new AjaxRequestManager('executescalarquery', null, 'err_failrequest', 'ajax_loading');

            lcAjaxRequestManager.AddAjaxParam('parameter', 'epos.getprintinginfo');
            lcAjaxRequestManager.AddAjaxParam('datablock', Base64.encode(JSON.stringify({ ReceiptType: paReceiptType, ReceiptID: paReceiptID })) );
            
            lcAjaxRequestManager.SetCompleteHandler(function (paSuccess, paResponseStruct) {
                if (paSuccess)
                {
                    var lcReceiptInfo = paResponseStruct.ResponseData.RSP_Result;                    
                    lcDeferred.resolve(lcReceiptInfo);                    
                }
                else
                {
                    lcDeferred.resolve(null);
                }
            });

            lcAjaxRequestManager.Execute();

            return (lcDeferred);
        },

        SetReceiptWidth: function (paNewWidth)
        {
            clReceiptRenderer.SetReceiptWidth(paNewWidth);            
        },

        SetReceiptLayoutParameter: function (paKey, paValue)
        {
            clReceiptRenderer.SetReceiptLayoutParameter(paKey, paValue);
        },

        SetReceiptCustomizationParameter: function (paKey, paValue)
        {
            clReceiptRenderer.SetReceiptCustomizationParameter(paKey, paValue);
        },
        HandlerOnClick : function(paEvent)
        {
            paEvent.preventDefault();

            var lcCommand = $(this).attr('ea-command');
            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);
            
            switch(lcCommand)
            {
                case 'popupclose' :
                    {
                        POSReceiptPrintingManager.ClosePreviewWindow();
                        break;
                    }

                case 'print' :
                    {
                        POSReceiptPrintingManager.PrintOutPreviewReceipt();
                        break;
                    }
            }
        }       
    }
})();

var ReceiptDataManager  = function(paComposite, paReceiptData) {
    var clComposite     = paComposite;    
    var clReceiptMaster = paReceiptData.receiptmaster[0];
    var clReceiptDetail = paReceiptData.receiptdetail;
    var clCurrencyDecimal;

    return {
                Init : function() 
                {
                    clCurrencyDecimal = clComposite.attr('ea-decimal');

                    this.ParseMasterData();
                    this.ParseDetailData();
                },
                ParseMasterData : function ()
                {                    
                    if (clReceiptMaster)
                    {
                        clReceiptMaster.receiptid               = CastInteger(clReceiptMaster.receiptid, 0);
                        clReceiptMaster.receipttype             = clReceiptMaster.receipttype || 'SALE';
                        clReceiptMaster.receiptno               = '#' + ('0000000' + CastInteger(clReceiptMaster.receiptno, 0)).slice(-7);                        
                        clReceiptMaster.receiptdate             = (clReceiptMaster.receiptdate || '').ParseMomentDateTime();
                        clReceiptMaster.taxpercent              = CastDecimal(clReceiptMaster.taxpercent,0);
                        clReceiptMaster.taxinclusive            = CastInteger(clReceiptMaster.taxinclusive || 0);
                        clReceiptMaster.reference               = clReceiptMaster.reference || '';
                        clReceiptMaster.discount                = CastDecimal(clReceiptMaster.discount,0);
                        clReceiptMaster.servicechargepercent    = CastDecimal(clReceiptMaster.servicechargepercent,0);
                        clReceiptMaster.servicecharge           = CastDecimal(clReceiptMaster.servicecharge,0);
                        clReceiptMaster.paymentcash             = CastDecimal(clReceiptMaster.paymentcash,0);
                        clReceiptMaster.paymentbank             = CastDecimal(clReceiptMaster.paymentbank,0);
                        clReceiptMaster.paymentcredit           = CastDecimal(clReceiptMaster.paymentcredit,0);
                        clReceiptMaster.paymentcontra           = CastDecimal(clReceiptMaster.paymentcontra,0); 
                        clReceiptMaster.paymentetransfer        = CastDecimal(clReceiptMaster.paymentetransfer,0); 
                        clReceiptMaster.paymentcreditcard       = CastDecimal(clReceiptMaster.paymentcreditcard,0);
                        clReceiptMaster.change                  = CastDecimal(clReceiptMaster.change,0);
                        clReceiptMaster.staffname               = clReceiptMaster.staffname || '';
                        clReceiptMaster.customername            = clReceiptMaster.customername || '';
                        clReceiptMaster.tablename               = clReceiptMaster.tablename || '';
                    }   
                },
                ParseDetailData : function ()
                {
                    var lcReceiptDetailEntry;

                    if (clReceiptDetail)
                    {
                        for (lcCount = 0; lcCount < clReceiptDetail; lcCount++)
                        {
                            lcReceiptDetailEntry                = clReceiptDetail[lcCount];
                            lcReceiptDetailEntry.receiptid      = CastInteger(lcReceiptDetailEntry.receiptid, 0);
                            lcReceiptDetailEntry.serial         = CastInteger(lcReceiptDetailEntry.serial, 0);
                            lcReceiptDetailEntry.quantity       = CastDecimal(lcReceiptDetailEntry.quantity,0);
                            lcReceiptDetailEntry.unitprice      = CastDecimal(lcReceiptDetailEntry.unitprice,0);
                            lcReceiptDetailEntry.discount       = CastDecimal(lcReceiptDetailEntry.discount,0);
                            lcReceiptDetailEntry.taxamount      = CastDecimal(lcReceiptDetailEntry.taxamount,0);
                            lcReceiptDetailEntry.item.itemname  = lcReceiptDetailEntry.item.itemname || '#ERR';
                            lcReceiptDetailEntry.item.unitname  = lcReceiptDetailEntry.item.unitname || '#ERR';
                        }
                    }
                },   
                GetMasterData : function(paColumnName)
                {
                    switch(paColumnName)
                    {
                        case 'receiptno'            : return(clReceiptMaster.receiptno);
                        case 'receiptdate'          : return(clReceiptMaster.receiptdate);
                        case 'reference'            : return(clReceiptMaster.reference);
                        case 'customername'         : 
                        case 'suppliername'         : return(clReceiptMaster.customername);
                        case 'tablename'            : return(clReceiptMaster.tablename);
                        case 'staffname'            : return(clReceiptMaster.staffname);
                        case 'servicechargepercent' : return(clReceiptMaster.servicechargepercent);
                        case 'servicecharge'        : return(clReceiptMaster.servicecharge);                            
                        case 'taxpercent'           : return(clReceiptMaster.taxpercent);
                        case 'discount'             : return(clReceiptMaster.discount);
                        case 'tendercash'           : return(clReceiptMaster.paymentcash + clReceiptMaster.change);
                        case 'tenderchange'         : return(clReceiptMaster.change);

                    }                    
                },
                GetReceiptDetail : function()
                {
                    return(clReceiptDetail);
                },
                GetAggregateFigure : function(paAggregateName)
                {
                    switch(paAggregateName)
                    {
                        case 'subtotal':
                            {
                                if (clReceiptDetail)
                                {                                    
                                    var lcSubtotal  = 0;
                                    
                                    for (lcCount = 0; lcCount < clReceiptDetail.length; lcCount++)
                                    {                                        
                                        lcSubtotal += (clReceiptDetail[lcCount].quantity * clReceiptDetail[lcCount].unitprice).toFixed(clCurrencyDecimal) - clReceiptDetail[lcCount].discount;                                        
                                    }
                                    
                                    return (lcSubtotal);
                                }
                                return (0);
                            }

                        case 'taxtotal':
                            {
                                if (clReceiptDetail) {                                    
                                    var lcTaxtotal      = 0;

                                    for (lcCount = 0; lcCount < clReceiptDetail.length; lcCount++)
                                    {                                        
                                        lcTaxtotal += clReceiptDetail[lcCount].taxamount;
                                    }

                                    return (lcTaxtotal);
                                }
                                return (0);
                            }

                        case 'total':
                            {
                                var lcSubtotal          = this.GetAggregateFigure('subtotal');
                                var lcTaxtotal          = this.GetAggregateFigure('taxtotal');
                                var lcDiscount          = this.GetMasterData('discount');
                                var lcServiceCharge = this.GetMasterData('servicecharge');

                                return(lcSubtotal + lcServiceCharge + lcTaxtotal - lcDiscount);
                            }
                    }
                }


    } // Return

}

var RenderingController = function (paComposite) {
    var clComposite = paComposite;
    var clCanvasPainter;
    var clCurrencyDecimal;
    var clReceiptLayout             = {};
    var clReceiptLayoutSetting      = {};    
    var clReceiptCustomization      = {};
    var clActivePrinterSetting      = {};
            
    var clImage;

    return {
                Init : function(paPrinterSetting)
                {
                    clActivePrinterSetting = paPrinterSetting;
                    clCurrencyDecimal = clComposite.attr('ea-decimal');
                    
                    clCanvasPainter = new CanvasPainter(clComposite.find('canvas'));
                    clCanvasPainter.InitializeCanvas();                    

                    this.LoadLayoutInfo();

                    clImage                     = this.LoadImage();                                        
                    clCanvasPainter.SetLineFeed(0, clReceiptLayout.LineFeedAfter);
                    clCanvasPainter.SetContextParam(clReceiptLayout.TextBaseLine, clReceiptLayout.TextPaddingRatio, clReceiptLayout.FontHeightFactor);
                },
                ApplyLayoutScaleX : function(paFigure)
                {
                    if (paFigure) { return (CastInteger(paFigure * clReceiptLayoutSetting.LayoutScaleX)); }
                    else return (0);                    
                },
                ApplyLayoutScaleY: function (paFigure) {
                    if (paFigure) { return (CastInteger(paFigure * clReceiptLayoutSetting.LayoutScaleY)); }
                    else return (0);                    
                },
                ApplyFontScale : function(paFontStr)
                {
                    paFontStr = paFontStr || '';
                    
                    var lcMatches = paFontStr.match(/((\d{1,3})px)/igm);

                    for (lcCount = 0; lcCount < lcMatches.length; lcCount++)
                    {
                        var lcScaledSizeStr = CastInteger(parseInt(lcMatches[lcCount]) * clReceiptLayoutSetting.FontScale) + 'px';                         
                        paFontStr = paFontStr.replace(lcMatches[lcCount], lcScaledSizeStr);
                    }

                    return (paFontStr || '');                    
                },
                LoadLayoutInfo : function()
                {                    
                    clReceiptLayout         = JSON.parse(Base64.decode(clComposite.attr('pos.receiptlayoutinfo.layout') || '') || "{}");
                    clReceiptLayoutSetting  = JSON.parse(Base64.decode(clComposite.attr('pos.receiptlayoutinfo.layoutsetting') || '') || "{}");
                    clReceiptCustomization  = JSON.parse(Base64.decode(clComposite.attr('pos.receiptlayoutinfo.customization') || '') || "{}");                    
                                       
                    clReceiptLayoutSetting.LayoutScaleX     = CastDecimal(clReceiptLayoutSetting.LayoutScaleX,1);
                    clReceiptLayoutSetting.LayoutScaleY     = CastDecimal(clReceiptLayoutSetting.LayoutScaleY,1);
                    clReceiptLayoutSetting.FontScale        = CastDecimal(clReceiptLayoutSetting.FontScale, 1);
                    clReceiptLayoutSetting.ReceiptWidth     = CastInteger(clReceiptLayoutSetting.ReceiptWidth || clActivePrinterSetting.PrinterWidth, 500);
                    clReceiptLayoutSetting.LocalNumberMode  = clReceiptLayoutSetting.LocalNumberMode == 'false' ? false : true;
                    clReceiptLayoutSetting.Copies           = CastInteger(clReceiptLayoutSetting.Copies, 2);
                    
                    clReceiptLayout.LineFeedAfter           = this.ApplyLayoutScaleY(CastInteger(clReceiptLayout.LineFeedAfter, 5));
                    clReceiptLayout.TextBaseLine            = clReceiptLayout.TextBaseLine || 'top';
                    clReceiptLayout.TextPaddingRatio        = CastDecimal(clReceiptLayout.TextPaddingRatio,1);                    
                    clReceiptLayout.FontHeightFactor        = CastDecimal(clReceiptLayout.FontHeightFactor, 1);                    
                                        
                    clReceiptCustomization.LogoName         = clReceiptCustomization.LogoName ? (clComposite.attr('ea-path') || '') + '/' + clReceiptCustomization.LogoName : '';
                    clReceiptCustomization.RenderLogo       = clReceiptLayoutSetting.RenderLogo == 'false' ? false : true;
                    clReceiptCustomization.LogoMarginBefore = this.ApplyLayoutScaleY(CastInteger(clReceiptCustomization.LogoMarginBefore, 0));
                    clReceiptCustomization.LogoMarginAfter  = this.ApplyLayoutScaleY(CastInteger(clReceiptCustomization.LogoMarginAfter, 0));
                    clReceiptCustomization.LogoSize         = this.ApplyLayoutScaleX(CastDecimal(clReceiptCustomization.LogoSize, 80));
                    clReceiptCustomization.LogoAspectRatio  = CastDecimal(clReceiptCustomization.LogoAspectRatio, 3);
                    clReceiptCustomization.BusinessName     = (clReceiptCustomization.BusinessName || '');
                    clReceiptCustomization.Address          = (clReceiptCustomization.Address || '').split('\n');
                    clReceiptCustomization.FootNote         = (clReceiptCustomization.FootNote || '').split('\n');

                    clReceiptCustomization.LogoWidth        = this.ApplyLayoutScaleX(clCanvasPainter.GetAbsoluteWidth(CastInteger(clReceiptCustomization.LogoSize)));
                    clReceiptCustomization.LogoHeight       = CastInteger(clReceiptCustomization.LogoWidth / clReceiptCustomization.LogoAspectRatio);
                    
                    clReceiptLayout.HeadingFont             = this.ApplyFontScale(clReceiptLayout.HeadingFont).split(';;');
                    clReceiptLayout.HeadingLineSpacing      = CastDecimal(clReceiptLayout.HeadingLineSpacing, 1);
                    clReceiptLayout.HeadingLineStyle        = CastInteger(CastIntArray(clReceiptLayout.HeadingLineStyle));
                    
                    clReceiptLayout.InfoFont                = this.ApplyFontScale(clReceiptLayout.InfoFont);
                    clReceiptLayout.InfoLineSpacing         = CastDecimal(clReceiptLayout.InfoLineSpacing, 1);
                    clReceiptLayout.InfoLabelWidth          = this.ApplyLayoutScaleX(clCanvasPainter.GetAbsoluteWidth(CastInteger(clReceiptLayout.InfoLabelWidth, 50)));

                    clReceiptLayout.EntryFont               = this.ApplyFontScale(clReceiptLayout.EntryFont);
                    clReceiptLayout.EntryLineSpacing        = CastDecimal(clReceiptLayout.EntryLineSpacing, 1);
                    clReceiptLayout.EntryRowSpacing         = CastInteger(clReceiptLayout.EntryRowSpacing, 3);
                    clReceiptLayout.EntryColumnGap          = this.ApplyLayoutScaleX(clCanvasPainter.GetAbsoluteWidth(CastInteger(clReceiptLayout.EntryColumnGap, 3)));
                    clReceiptLayout.EntryDescriptionWidth   = this.ApplyLayoutScaleX(clCanvasPainter.GetAbsoluteWidth(CastInteger(clReceiptLayout.EntryDescriptionWidth, 0)));
                    clReceiptLayout.EntryQuantityWidth      = this.ApplyLayoutScaleX(clCanvasPainter.GetAbsoluteWidth(CastInteger(clReceiptLayout.EntryQuantityWidth, 0)));
                    clReceiptLayout.EntryUnitPriceWidth     = this.ApplyLayoutScaleX(clCanvasPainter.GetAbsoluteWidth(CastInteger(clReceiptLayout.EntryUnitPriceWidth, 0)));
                    clReceiptLayout.EntryDiscountWidth      = this.ApplyLayoutScaleX(clCanvasPainter.GetAbsoluteWidth(CastInteger(clReceiptLayout.EntryDiscountWidth, 0)));
                    clReceiptLayout.EntrySubtotalWidth      = this.ApplyLayoutScaleX(clCanvasPainter.GetAbsoluteWidth(CastInteger(clReceiptLayout.EntrySubtotalWidth, 0)));

                    clReceiptLayout.SummaryFont             = this.ApplyFontScale(clReceiptLayout.SummaryFont);                    
                    clReceiptLayout.SummaryLineSpacing      = CastDecimal(clReceiptLayout.HeadingLineSpacing, 1);
                    clReceiptLayout.SummaryTotalFont        = clReceiptLayout.SummaryTotalFont || '';
                    clReceiptLayout.SummaryLineStyle        = CastIntArray(clReceiptLayout.SummaryLineStyle);                                          
                    clReceiptLayout.SummaryLabelWidth       = this.ApplyLayoutScaleX(clCanvasPainter.GetAbsoluteWidth(CastInteger(clReceiptLayout.SummaryLabelWidth, 0)));               
                    clReceiptLayout.SummaryColumnGap        = this.ApplyLayoutScaleX(clCanvasPainter.GetAbsoluteWidth(CastInteger(clReceiptLayout.SummaryColumnGap, 0)));

                    clReceiptLayout.SummaryTotalLabel           = clReceiptLayout.SummaryTotalLabel || '';

                    clReceiptLayout.TenderTitleFont             = this.ApplyFontScale(clReceiptLayout.TenderTitleFont);
                    clReceiptLayout.TenderTextFont              = this.ApplyFontScale(clReceiptLayout.TenderTextFont);
                    clReceiptLayout.TenderLabelWidth            = this.ApplyLayoutScaleX(clCanvasPainter.GetAbsoluteWidth(CastInteger(clReceiptLayout.TenderLabelWidth, 0)));
                    clReceiptLayout.TenderFigureWidth           = this.ApplyLayoutScaleX(clCanvasPainter.GetAbsoluteWidth(CastInteger(clReceiptLayout.TenderFigureWidth, 0)));
                    clReceiptLayout.TenderColumnGap             = this.ApplyLayoutScaleX(clCanvasPainter.GetAbsoluteWidth(CastInteger(clReceiptLayout.TenderColumnGap, 3)));
                    clReceiptLayout.TenderLineSpacing           = CastDecimal(clReceiptLayout.TenderLineSpacing, 1);
                    
                    clReceiptLayout.TenderTitleLabel            = clReceiptLayout.TenderTitleLabel || '';

                    clReceiptLayout.FootNoteFont                = this.ApplyFontScale(clReceiptLayout.FootNoteFont).split(';;');
                    clReceiptLayout.FootNoteLineStyle           = CastIntArray(clReceiptLayout.FootNoteLineStyle);
                    clReceiptLayout.FootNoteLineSpacing         = CastDecimal(clReceiptLayout.FootNoteLineSpacing, 1);
                    
                    clReceiptLayout.RenderCommand               = (clReceiptLayout.RenderCommand || '').split(';');
                },
                SetReceiptWidth : function(paNewWidth)
                {
                    if (paNewWidth)
                    {
                        clReceiptLayoutSetting.ReceiptWidth = CastInteger(paNewWidth);
                        clCanvasPainter.GetCanvas().width = clReceiptLayoutSetting.ReceiptWidth;
                    }
                },
                SetReceiptLayoutParameter : function(paKey, paValue)
                {
                    if (clReceiptLayout[paKey])
                        clReceiptLayout[paKey] = paValue;
                },
                SetReceiptCustomizationParameter : function (paKey, paValue) 
                {
                    if (clReceiptCustomization[paKey])
                        clReceiptCustomization[paKey] = paValue;
                },
                LoadImage : function()
                {                    
                    var lcImage = $(document.createElement('img'));

                    if (clReceiptCustomization.RenderLogo) {
                        lcImage.attr('fa-loadstate', 'loading');

                        lcImage.bind('load', function () { $(this).attr('fa-loadstate', 'success'); });
                        lcImage.bind('error', function () { $(this).attr('fa-loadstate', 'fail'); });
             
                        lcImage.attr('src', clReceiptCustomization.LogoName + "?t=" + moment().format('YYYYMMDDHHmmss'));
                    }
                    else lcImage.attr('fa-loadstate', 'suppress');

                    return (lcImage);
                },                
                GetAddressText : function(paIndex)
                {                    
                    if (clReceiptCustomization.Address.length > paIndex) return (clReceiptCustomization.Address[paIndex]);
                    else return('');
                },
                GetAddressFont: function (paIndex)
                {
                    paIndex += 1;
                    if (clReceiptLayout.HeadingFont.length > paIndex) return (clReceiptLayout.HeadingFont[paIndex]);
                    else return (clReceiptLayout.HeadingFont[clReceiptLayout.HeadingFont.length - 1]);
                },
                GetFootNoteText: function (paIndex)
                {
                    if (clReceiptCustomization.FootNote.length > paIndex) return (clReceiptCustomization.FootNote[paIndex]);
                    else return ('');
                },
                GetFootNoteFont: function (paIndex)
                {
                    if (clReceiptLayout.FootNoteFont.length > paIndex) return (clReceiptLayout.FootNoteFont[paIndex]);
                    else return (clReceiptLayout.FootNoteFont[clReceiptLayout.FootNoteFont.length - 1]);
                },
                GetTextLineClipRectangle : function(paX, paWidth, paFont)
                {
                    return ({ X: paX, Y: clCanvasPainter.CurrentY(), Width: paWidth, Height: clCanvasPainter.MeasureHeight(paFont) })
                },                
                ConvertNumber : function(paNumber)
                {
                    paNumber = paNumber || 0;
                    
                    if (clReceiptLayoutSetting.LocalNumberMode)
                        return(FormManager.ConvertToFormLanguage(paNumber));
                    else return(paNumber);
                },
                GetLabelText : function(paLabelName)
                {
                    switch(paLabelName)
                    {
                        case 'receiptno'        : 
                        case 'receiptdate'      : 
                        case 'staffname'        : 
                        case 'customername'     : 
                        case 'suppliername'     : 
                        case 'tablename'        : 
                        case 'reference'        : return (clReceiptLayout.InfoDataSection[paLabelName]);
                        case 'subtotal'         : 
                        case 'taxtotal'         : 
                        case 'discount'         : 
                        case 'servicecharge'    : return (clReceiptLayout.SummaryDataSection[paLabelName]);
                        case 'total'            : return (clReceiptLayout.SummaryTotalLabel);
                        case 'tendercash'       : 
                        case 'tenderchange'     : return (clReceiptLayout.TenderDataSection[paLabelName]);
                            
                    }
                },
                RenderInfoText : function(paLabel, paText)
                {
                    var lcLabelWidth;                    

                    if ((paText) && (paText.length > 0))
                    {                                    
                        lcLabelWidth = clReceiptLayout.InfoLabelWidth;
                        
                        clCanvasPainter.DrawText({  Text          : paLabel,
                                                    Font          : clReceiptLayout.InfoFont,
                                                    Alignment     : 'left',
                                                    LineSpacing   : clReceiptLayout.InfoLineSpacing,
                                                    ClipRectangle: this.GetTextLineClipRectangle(0, lcLabelWidth, clReceiptLayout.InfoFont)
                                                });
                                            
                        clCanvasPainter.DrawTextLn({  Text          : paText,
                                                      Font          : clReceiptLayout.InfoFont,
                                                      Alignment     : 'left',
                                                      LineSpacing   : clReceiptLayout.InfoLineSpacing,
                                                      ClipRectangle: this.GetTextLineClipRectangle(lcLabelWidth, clReceiptLayoutSetting.ReceiptWidth - lcLabelWidth, clReceiptLayout.InfoFont)
                                                   });                                            
                    }
                },
                RenderEntryText : function(paX, paWidth, paText, paAlign, paLineFeed)
                {
                    if (typeof paText !== 'undefined')
                    {                        
                        clCanvasPainter.DrawText({
                                                    Text: paText,
                                                    Font: clReceiptLayout.EntryFont,
                                                    Alignment: paAlign,
                                                    LineSpacing: clReceiptLayout.EntryLineSpacing,
                                                    ClipRectangle: this.GetTextLineClipRectangle(paX, paWidth, clReceiptLayout.EntryFont),
                                                    LineFeed : paLineFeed
                                                });                        
                    }
                },
                RenderSummaryText: function (paLabel, paText) {
                    var lcLabelWidth;
                    var lcFigureWidth;
                    var lcColumnGap;

                    if ((paText) && (paText.length > 0))
                    {                        
                        lcLabelWidth    = clReceiptLayout.SummaryLabelWidth;
                        lcFigureWidth = clReceiptLayoutSetting.ReceiptWidth - (clReceiptLayout.SummaryLabelWidth + clReceiptLayout.SummaryColumnGap);
                        
                        lcColumnGap     = clReceiptLayout.SummaryColumnGap;

                        clCanvasPainter.DrawText({
                            Text: paLabel,
                            Font: clReceiptLayout.SummaryFont,
                            Alignment: 'right',
                            LineSpacing: clReceiptLayout.SummaryLineSpacing,
                            ClipRectangle: this.GetTextLineClipRectangle(0, lcLabelWidth, clReceiptLayout.SummaryFont)
                        });

                        clCanvasPainter.DrawTextLn({
                            Text: paText,
                            Font: clReceiptLayout.SummaryFont,
                            Alignment: 'right',
                            LineSpacing: clReceiptLayout.SummaryLineSpacing,
                            ClipRectangle: this.GetTextLineClipRectangle(lcLabelWidth + lcColumnGap, lcFigureWidth, clReceiptLayout.SummaryFont)
                        });
                    }
                },
                RenderTotalText: function (paLabel, paText) {
                    var lcLabelWidth;
                    var lcFigureWidth;
                    var lcColumnGap;

                    if ((paText) && (paText.length > 0)) {                        
                        lcLabelWidth    = clReceiptLayout.SummaryLabelWidth;
                        lcFigureWidth   = clReceiptLayoutSetting.ReceiptWidth - (clReceiptLayout.SummaryLabelWidth + clReceiptLayout.SummaryColumnGap);
                        lcColumnGap     = clReceiptLayout.SummaryColumnGap;

                        clCanvasPainter.DrawText({
                            Text: paLabel,
                            Font: clReceiptLayout.SummaryTotalFont,
                            Alignment: 'right',
                            LineSpacing: clReceiptLayout.SummaryLineSpacing,
                            ClipRectangle: this.GetTextLineClipRectangle(0, lcLabelWidth, clReceiptLayout.SummaryTotalFont)
                        });

                        clCanvasPainter.DrawTextLn({
                            Text: paText,
                            Font: clReceiptLayout.SummaryTotalFont,
                            Alignment: 'right',
                            LineSpacing: clReceiptLayout.SummaryLineSpacing,
                            ClipRectangle: this.GetTextLineClipRectangle(lcLabelWidth + lcColumnGap, lcFigureWidth, clReceiptLayout.SummaryTotalFont)
                        });
                    }
                },
                RenderTenderText: function (paLabel, paText) {
                    var lcLabelWidth;
                    var lcFigureWidth;
                    var lcColumnGap;

                    if ((paText) && (paText.length > 0)) {
                        lcLabelWidth = clReceiptLayout.TenderLabelWidth;
                        lcFigureWidth = clReceiptLayout.TenderFigureWidth;
                        lcColumnGap = clReceiptLayout.TenderColumnGap;
                        
                        clCanvasPainter.DrawText({
                            Text: paLabel,
                            Font: clReceiptLayout.TenderTextFont,
                            Alignment: 'left',
                            LineSpacing: clReceiptLayout.TenderLineSpacing,
                            ClipRectangle: this.GetTextLineClipRectangle(0, lcLabelWidth, clReceiptLayout.TenderTextFont)
                        });

                        clCanvasPainter.DrawTextLn({
                            Text: paText,
                            Font: clReceiptLayout.TenderTextFont,
                            Alignment: 'right',
                            LineSpacing: clReceiptLayout.TenderLineSpacing,
                            ClipRectangle: this.GetTextLineClipRectangle(lcLabelWidth + lcColumnGap, lcFigureWidth, clReceiptLayout.TenderTextFont)
                        });
                    }
                },
                RenderEntryRow : function(paReceiptEntry)
                {
                    var     lcDescription;
                    var     lcQuantity;
                    var     lcUnitPrice;
                    var     lcDiscount;
                    var     lcX;

                    if (paReceiptEntry)
                    {
                        lcX         = 0;
                        lcQuantity  = CastDecimal(paReceiptEntry.quantity, 0);
                        lcUnitPrice = CastDecimal(paReceiptEntry.unitprice, 0).toFixed(clCurrencyDecimal);
                        lcDiscount = CastDecimal(paReceiptEntry.discount, 0).toFixed(clCurrencyDecimal);
                        lcSubtotal  = (lcQuantity * lcUnitPrice).toFixed(clCurrencyDecimal) - lcDiscount;
                        
                        lcDescription = paReceiptEntry.item[0].itemname + ' ' + this.ConvertNumber(lcQuantity) + ' ' + paReceiptEntry.item[0].unitname;

                        this.RenderEntryText(lcX, clReceiptLayout.EntryDescriptionWidth, lcDescription, 'left', true);
                       
                        this.RenderEntryText(lcX, clReceiptLayout.EntryQuantityWidth, this.ConvertNumber(lcQuantity) + ' ' + String.fromCharCode(0xD7), 'right', false);
                        lcX += clReceiptLayout.EntryQuantityWidth + clReceiptLayout.EntryColumnGap;
                        
                        this.RenderEntryText(lcX, clReceiptLayout.EntryUnitPriceWidth, this.ConvertNumber(lcUnitPrice), 'right', false);
                        lcX += clReceiptLayout.EntryUnitPriceWidth + clReceiptLayout.EntryColumnGap;

                        if (lcDiscount > 0)                            
                            this.RenderEntryText(lcX, clReceiptLayout.EntryDiscountWidth, '(-' + this.ConvertNumber(lcDiscount) + ')', 'right', false);
                        lcX += clReceiptLayout.EntryDiscountWidth + clReceiptLayout.EntryColumnGap;
                        
                        this.RenderEntryText(lcX, clReceiptLayout.EntrySubtotalWidth, this.ConvertNumber(lcSubtotal), 'right', true);
                    }
                },
                RenderElement : function(paElement, paParameter)
                {
                    switch(paElement)
                    {
                        case 'receiptlogo':
                            {

                                if (clImage.attr('fa-loadstate') == 'success')
                                {
                                        clCanvasPainter.DrawImage({ Image: clImage[0],
                                                                    Alignment: 'center',
                                                                    ImageWidth  : clReceiptCustomization.LogoWidth,
                                                                    ImageHeight : clReceiptCustomization.LogoHeight,
                                                                    MarginBefore: clReceiptCustomization.LogoMarginBefore,
                                                                    MarginAfter : clReceiptCustomization.LogoMarginAfter,
                                        });
                                }
                                break;
                            }
                                                    
                        case 'headertext':
                            {
                                var     lcText;
                                var     lcFont;

                                clCanvasPainter.DrawTextLn({
                                    Text: clReceiptCustomization.BusinessName,
                                    Font: clReceiptLayout.HeadingFont[0],
                                    Alignment: 'center',
                                    LineSpacing: clReceiptLayout.HeadingLineSpacing
                                });
                                
                                for (lcCount = 0; lcCount < clReceiptCustomization.Address.length; lcCount++)
                                {
                                    
                                    lcText = this.GetAddressText(lcCount) || '';
                                    lcFont = this.GetAddressFont(lcCount);
                                    
                                    if (lcText.length > 0)
                                    {                                        
                                        clCanvasPainter.DrawTextLn({ Text: lcText,
                                                                     Font: lcFont, 
                                                                     Alignment: 'center',                                                                   
                                                                     LineSpacing: clReceiptLayout.HeadingLineSpacing});
                                    }
                                }
                                break;
                                
                            }

                        case 'footnote':
                            {
                                var lcText;
                                var lcFont;

                                for (lcCount = 0; lcCount < clReceiptCustomization.FootNote.length; lcCount++) {

                                    lcText = this.GetFootNoteText(lcCount) || '';
                                    lcFont = this.GetFootNoteFont(lcCount);

                                    if (lcText.length > 0) {
                                        clCanvasPainter.DrawTextLn({
                                            Text: lcText,
                                            Font: lcFont,
                                            Alignment: 'center',
                                            LineSpacing: clReceiptLayout.FootNoteLineSpacing
                                        });
                                    }
                                }
                                break;

                            }

                        case 'headingseparatorline':
                            {
                                clCanvasPainter.DrawSeparatorLine(clReceiptLayout.HeadingLineStyle);
                                break;
                            }

                        case 'summaryseparatorline':
                            {
                                clCanvasPainter.DrawSeparatorLine(clReceiptLayout.SummaryLineStyle);
                                break;
                            }

                        case 'footnoteseparatorline':
                            {
                                clCanvasPainter.DrawSeparatorLine(clReceiptLayout.FootNoteLineStyle);
                                break;
                            }

                        case 'receiptno'    :
                        case 'reference'    :
                        case 'staffname'    :
                        case 'customername' :
                        case 'suppliername' :
                        case 'tablename'    :
                            {         
                                var lcData = paParameter.GetMasterData(paElement);

                                if (lcData)
                                {
                                    lcData     = paElement == 'receiptno' ? this.ConvertNumber(lcData) : lcData;                                                                  
                                    this.RenderInfoText(this.GetLabelText(paElement), lcData);
                                }                                
                                break;
                            }

                        case 'receiptdate':
                            {                                
                                var lcDateStr;
                                var lcReceiptDate = paParameter.GetMasterData('receiptdate');

                                if (lcReceiptDate)
                                {
                                    if ((lcReceiptDate != null) && (lcReceiptDate.isValid())) {
                                        if (lcReceiptDate.format('HH:mm:ss') == '00:00:00') lcDateStr = lcReceiptDate.format('D/M/YYYY');
                                        else lcDateStr = lcReceiptDate.format('D/M/YYYY - HH:mm:ss');

                                        this.RenderInfoText(this.GetLabelText(paElement), this.ConvertNumber(lcDateStr));
                                    }
                                }
                                break;
                            }                       

                        case 'linefeed':
                            {                                
                                clCanvasPainter.LineFeed(CastInteger(paParameter,0));
                                break;
                            }

                        case 'receiptentry':
                            {                       
                                var lcReceiptDetail;

                                lcReceiptDetail = paParameter.GetReceiptDetail();

                                for (lcCount = 0; lcCount < lcReceiptDetail.length; lcCount++)
                                {
                                    this.RenderEntryRow(lcReceiptDetail[lcCount]);                                    
                                    clCanvasPainter.LineFeed(clReceiptLayout.EntryMargin);
                                }                                

                                break;
                            }

                        case 'subtotal' :                        
                            {
                                var lcSubtotal = paParameter.GetAggregateFigure(paElement);                                
                                this.RenderSummaryText(this.GetLabelText(paElement), this.ConvertNumber(lcSubtotal.toString()));
                                break;
                            }

                        case 'taxtotal' :                        
                            {
                                var lcTotalTax      = paParameter.GetAggregateFigure(paElement);
                                var lcTaxPercent    = paParameter.GetMasterData('taxpercent');
                                
                                if ((lcTotalTax) && (lcTotalTax > 0))
                                {
                                    lcTaxPercent = this.ConvertNumber(lcTaxPercent > 0 ? " (" + lcTaxPercent + "%)" : '');
                                    this.RenderSummaryText(this.GetLabelText(paElement) + lcTaxPercent, this.ConvertNumber(lcTotalTax.toString()));
                                }
                                break;
                            }

                        case 'discount'      :
                            {
                                var lcDiscount = paParameter.GetMasterData(paElement);

                                if ((lcDiscount) && (lcDiscount > 0)) {
                                    this.RenderSummaryText(this.GetLabelText(paElement), this.ConvertNumber('(-' + lcDiscount.toString() + ')'));
                                }
                                break;
                            }

                        case 'servicecharge' :                        
                            {                                
                                var lcServiceCharge = paParameter.GetMasterData(paElement);
                                var lcServiceChargePercent = paParameter.GetMasterData('servicechargepercent');

                                if ((lcServiceCharge) && (lcServiceCharge > 0))
                                {
                                    lcServiceChargePercent =  this.ConvertNumber(lcServiceChargePercent > 0 ? " (" + lcServiceChargePercent + "%)" : '');
                                    this.RenderSummaryText(this.GetLabelText(paElement) + lcServiceChargePercent, this.ConvertNumber(lcServiceCharge.toString()));
                                }
                                break;
                            }

                        case 'total':
                            {                             
                                var lcTotal = paParameter.GetAggregateFigure('total');                                
                                this.RenderTotalText(this.GetLabelText(paElement), this.ConvertNumber(lcTotal.toString()));                                
                                break;
                            }

                        case 'tendercash':
                        case 'tenderchange':
                            {
                                var lcValue = paParameter.GetMasterData(paElement);                                
                                this.RenderTenderText(this.GetLabelText(paElement), this.ConvertNumber(lcValue.toString()));
                                break;
                            }

                        case 'infodata':
                            {
                                for (var lcProperty in clReceiptLayout.InfoDataSection)
                                {
                                    this.RenderElement(lcProperty, paParameter);
                                }                                
                                break;
                            }

                        case 'summarydata':
                            {
                                for (var lcProperty in clReceiptLayout.SummaryDataSection) {                                    
                                    this.RenderElement(lcProperty, paParameter);
                                }
                                break;
                            }

                        case 'tenderdata':
                            {
                                for (var lcProperty in clReceiptLayout.TenderDataSection) {                                    
                                    this.RenderElement(lcProperty, paParameter);
                                }
                                break;
                            }

                        case 'tendertitle':
                            {
                                var lcText;
                                var lcFont;

                                lcText = clReceiptLayout.TenderTitle || '';
                                lcFont = clReceiptLayout.TenderTitleFont;
                                
                                if (lcText.length > 0) 
                                {
                                    clCanvasPainter.DrawTextLn({
                                        Text: lcText,
                                        Font: lcFont,
                                        Alignment: 'left',
                                        LineSpacing: 1
                                    });
                                }                                
                                break;
                            }
                    }
                },                
                GetRenderedReceiptImage: function (paReceiptDataManager)
                {
                    this.RenderReceipt(paReceiptDataManager, false);
                    clCanvasPainter.GetCanvas().height = clCanvasPainter.CurrentY();
                    this.RenderReceipt(paReceiptDataManager, true);
                    return (clCanvasPainter.GetCanvas().toDataURL());
                },
                PrintCanvasImage : function()
                {
                    for (lcCount = 0; lcCount < clReceiptLayoutSetting.Copies; lcCount++)
                    {                        
                        PrinterManager.Print(clCanvasPainter.GetCanvas());
                    }
                },                
                PrintReceipt : function(paReceiptDataManager)
                {                    
                    this.RenderReceipt(paReceiptDataManager, false);
                    clCanvasPainter.GetCanvas().height = clCanvasPainter.CurrentY();
                    this.RenderReceipt(paReceiptDataManager, true);
                    this.PrintCanvasImage();
                },
                RenderReceipt: function (paReceiptDataManager, paDrawMode) {
                    var lcRenderData;

                    clCanvasPainter.SetDrawMode(paDrawMode);
                    clCanvasPainter.Clear();
                    
                    for (lcIndex = 0; lcIndex < clReceiptLayout.RenderCommand.length; lcIndex++)
                    {                        
                        lcRenderData = clReceiptLayout.RenderCommand[lcIndex].split(',');
                        this.RenderElement(lcRenderData[0], lcRenderData.length > 1 ? lcRenderData[1] : paReceiptDataManager);                        
                    }                                     
                },

    };
};
