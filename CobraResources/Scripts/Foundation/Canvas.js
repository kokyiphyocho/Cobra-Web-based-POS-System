var CanvasPainter = function (paCanvasElement) {
    var     clContext;
    var     clCanvasElement = paCanvasElement;
    var     clYCord;
    var     clSavedStateCount;
    var     clYMarginBefore;
    var     clYMarginAfter;
    var     clLeftMargin;
    var     clTopMargin;
    var     clTextPaddingRatio;
    var     clFontHeightFactor;
    var     clDrawMode;    

    return {
                Init : function()
                {                                     
                    clSavedStateCount       = 0;
                    clYMarginBefore         = 0;
                    clYMarginAfter          = 0;
                    clDrawMode              = true;                    
                },
                GetCanvas : function()
                {
                    return (clCanvasElement[0]);
                },
                CreateCanvas : function(paWidth, paLeftMargin, paTopMargin)
                {
                    clCanvasElement[0].width        = (paWidth || CastInteger(clCanvasElement.attr('ea-originalvalue'))) + (paLeftMargin || 0);
                    
                    clContext                       = clCanvasElement[0].getContext('2d');
                    clContext.imageSmoothingEnabled = true;
                    clContext.textBaseline          = 'top';

                    clLeftMargin                = paLeftMargin || 0;
                    clTopMargin                 = paTopMargin || 0;
                    clYCord                     = clTopMargin;                    
                },
                //ScaleCanvas : function(paScaleFactor)
                //{
                //    paScaleFactor = CastDecimal(paScaleFactor, 1);

                //    if (paScaleFactor < 1)
                //    {   
                //        clCanvasElement[0].width = clCanvasElement[0].width * paScaleFactor;
                //        clCanvasElement[0].height = clCanvasElement[0].height * paScaleFactor;
                //    }
                //},
                //GetScaledCanvas : function(paScaleFactor)
                //{                    
                //    paScaleFactor = paScaleFactor || 1;

                //    if (paScaleFactor < 2) {

                //        if (clScaledCanvas == null) clScaledCanvas = document.createElement("canvas");

                //        clScaledCanvas.width = Math.floor(clCanvasElement[0].width * paScaleFactor);
                //        clScaledCanvas.height = Math.floor(clCanvasElement[0].height * paScaleFactor);

                //        var lcScaledCanvasContext = clScaledCanvas.getContext('2d');
                //        lcScaledCanvasContext.imageSmoothingEnabled = true;
                //        lcScaledCanvasContext.clearRect(0, 0, clScaledCanvas.width, clScaledCanvas.height);
                //        //lcScaledCanvasContext.fillStyle = "white";
                //        //lcScaledCanvasContext.fillRect(0, 0, lcScaledCanvas.width, lcScaledCanvas.height);

                //        lcScaledCanvasContext.drawImage(clCanvasElement[0], 0, 0, clCanvasElement[0].width, clCanvasElement[0].height, 0, 0, clScaledCanvas.width, clScaledCanvas.height);
                //        //  $(lcScaledCanvas).css('display', 'none');
                //        //$(clScaledCanvas).css('position', 'fixed');
                //        //$(clScaledCanvas).css('top', '0');

                //        //$('body').append($(clScaledCanvas));

                //        return (clScaledCanvas);
                //    }
                //    else return (clCanvasElement[0]);                    
                //},
                SetDrawMode : function(paDrawMode)
                {
                    clDrawMode = paDrawMode;
                },
                Clear : function()
                {
                    if (clDrawMode)
                    {
                        clContext.clearRect(0, 0, clCanvasElement[0].width, clCanvasElement[0].height);
                        clSavedStateCount = 0;
                    }

                    clYCord = clTopMargin;                    
                },
                CurrentY : function()
                {
                    return (clYCord);
                },
                SetContextParam: function (paTextBaseLine, paTextPaddingRatio, paFontHeightFactor)
                {
                    clContext.textBaseline  = paTextBaseLine;
                    clFontHeightFactor      = paFontHeightFactor || 1;
                    clTextPaddingRatio      = paTextPaddingRatio || 0;
                },
                SetLineFeed: function (paBefore, paAfter)
                {                 
                    clYMarginBefore         = paBefore;
                    clYMarginAfter          = paAfter;
                },
                LineFeed : function(paLineCount)
                {
                    paLineCount = paLineCount || clYMarginAfter;
                    clYCord += paLineCount;
                },
                LineFeedBefore : function()
                {
                    clYCord += clYMarginBefore;
                },
                LineFeedAfter  : function(paHeight, paAddMargin)
                {
                    paHeight = paHeight || 0;
                    
                    clYCord += paHeight + (paAddMargin ? clYMarginAfter : 0);                    
                },                
                DrawImage : function(paParameter)
                {
                    paParameter.Image           = paParameter.Image || ''
                    paParameter.Alignment       = paParameter.Alignment || '';
                    paParameter.X               = CastInteger(paParameter.X) + clLeftMargin;
                    paParameter.ImageWidth      = CastInteger(paParameter.ImageWidth);
                    paParameter.ImageHeight     = CastInteger(paParameter.ImageHeight);
                    paParameter.MarginBefore    = CastInteger(paParameter.MarginBefore);
                    paParameter.MarginAfter     = CastInteger(paParameter.MarginAfter);
                          
                    if (paParameter.MarginBefore) this.LineFeed(paParameter.MarginBefore);

                    this.SetClipRectangle(paParameter.ClipRectangle);
                    
                    if (paParameter.Alignment != '') 
                        paParameter.X = this.GetShapeAlignX(paParameter.Alignment, paParameter.ImageWidth, clCanvasElement[0].width);                    
                       
                    this.LineFeedBefore();                                        
                    if (clDrawMode) clContext.drawImage(paParameter.Image, paParameter.X, clYCord, paParameter.ImageWidth, paParameter.ImageHeight);
                    this.LineFeedAfter(paParameter.ImageHeight, false);

                    if (paParameter.MarginAfter) this.LineFeed(paParameter.MarginAfter);
                    
                    this.RestoreContext();
                },
                MeasureHeight : function(paFont)
                {
                    var lcHeight;                    
                    var lcMatch;
                    
                    if ((paFont) && (lcMatch = paFont.match(/\d{2}px/)))
                    {
                        lcHeight = parseInt(lcMatch[0]);
                        return(isNaN(lcHeight) ? 0 : lcHeight * clFontHeightFactor);
                    }
                    else return(0);
                },
                MeasureWidth : function(paText)
                {
                    if (paText)  {                        
                        return (clContext.measureText(paText).width);
                    }
                    else return (0);
                },
                GetShapeAlignX  : function(paAlignment, paShapeWidth, paContainerWidth)
                {
                    paShapeWidth     = CastInteger(paShapeWidth);
                    paContainerWidth = CastInteger(paContainerWidth);
                    paAlignment      = paAlignment || 'left';

                    switch(paAlignment)
                    {
                        case 'left'     : return (0 + clLeftMargin);
                        case 'center'   : return (((paContainerWidth / 2) - (paShapeWidth / 2)) + (clLeftMargin / 2));
                        case 'right'    : return ((paContainerWidth - paShapeWidth) + clLeftMargin);
                    }
                },
                GetTextAlignedX : function(paAlignment, paText, paClipRectangle)
                {
                    var lcRectWidth;
                    var lcTextWidth;
                    
                    if ((paClipRectangle) && (typeof paClipRectangle.Width !== 'undefined'))
                        lcRectWidth = paClipRectangle.Width;
                    else 
                        lcRectWidth = clCanvasElement[0].width;
                                        
                    lcTextWidth = Math.round(this.MeasureWidth(paText)) || 0;
                    
                    switch(paAlignment)
                    {
                        case 'left'     : return ((paClipRectangle ? paClipRectangle.X : 0) + clLeftMargin);
                        case 'center'   : return (((lcRectWidth / 2) - (lcTextWidth / 2)) + (clLeftMargin / 2));
                        case 'right'    : return (((paClipRectangle ? paClipRectangle.X : 0) + clLeftMargin + (lcRectWidth - lcTextWidth)));
                    }
                },
                GetAbsoluteWidth: function (paWidthPercent) {
                    if (paWidthPercent) return (clCanvasElement[0].width * paWidthPercent / 100);
                    else return (0);
                },
                DrawRectangle : function(paRectangleInfo)
                {
                    var lcPrevLineWidth;
                    
                    if (paRectangleInfo)
                    {                        
                        paRectangleInfo.X       = (paRectangleInfo.X || 0) + clLeftMargin;
                        paRectangleInfo.Y       = paRectangleInfo.Y || 0;
                        paRectangleInfo.Width   = paRectangleInfo.Width || clCanvasElement[0].width;
                        paRectangleInfo.Height  = paRectangleInfo.Height || clCanvasElement[0].height;
                        paRectangleInfo.Fill    = paRectangleInfo.Fill || false;
                        
                        if (paRectangleInfo.Fill) clContext.fillRect(paRectangleInfo.X, paRectangleInfo.Y, paRectangleInfo.Width, paRectangleInfo.Height);
                        else
                        {
                            lcPrevLineWidth         = clContext.lineWidth;
                            clContext.lineWidth     = paRectangleInfo.LineWidth || 1;

                            clContext.rect(paRectangleInfo.X, paRectangleInfo.Y, paRectangleInfo.Width, paRectangleInfo.Height);
                            clContext.stroke();

                            clContext.lineWidth = lcPrevLineWidth;
                        }
                    }
                },
                DrawLine : function(paLineInfo)
                {
                    var lcPrevLineWidth;
                    var lcDashStyle;

                    if (paLineInfo)
                    {
                        lcPrevLineWidth = clContext.lineWidth;
                        clContext.lineWidth = paLineInfo.LineWidth || 1;
                        
                        lcDashStyle = paLineInfo.DashStyle || [];
                        
                        clContext.setLineDash(lcDashStyle);
                        
                        clContext.beginPath();
                        clContext.moveTo(paLineInfo.X1 + clLeftMargin || 0, paLineInfo.Y1 || 0);
                        clContext.lineTo(paLineInfo.X2 + clLeftMargin || clCanvasElement[0].width, paLineInfo.Y2 || clCanvasElement[0].height);
                        clContext.stroke();

                        clContext.lineWidth = lcPrevLineWidth;
                        clContext.setLineDash([]);
                    }
                },
                DrawSeparatorLine : function(paStyle)
                {                    
                    if (paStyle)
                    {
                        if (paStyle[0]) this.LineFeed(paStyle[0]);

                        if (clDrawMode) this.DrawLine({ X1: this.GetAbsoluteWidth(paStyle[3] || 0), 
                                                        Y1: clYCord,
                                                        X2: this.GetAbsoluteWidth(paStyle[4] || 100),
                                                        Y2: clYCord,
                                                        LineWidth: paStyle[2],
                                                        DashStyle : paStyle.slice(5)
                                                     });
                    }
                    this.LineFeed(paStyle[1]);
                },                
                DrawText : function(paParameter)
                {
                    paParameter.Text            = typeof paParameter.Text === 'undefined' ? '' : paParameter.Text;                    
                    paParameter.Font            = paParameter.Font || '12px cobrasystemfont';
                    paParameter.Alignment       = paParameter.Alignment || '';
                    paParameter.X               = CastInteger(paParameter.X) + clLeftMargin;                    
                    paParameter.LineSpacing     = CastDecimal(paParameter.LineSpacing, 1);
                              
                    clContext.font = paParameter.Font;
                    
                    // this.DrawRectangle(paParameter.ClipRectangle);
                    
                    this.SetClipRectangle(paParameter.ClipRectangle);

                    if (paParameter.Alignment != '')
                        paParameter.X = this.GetTextAlignedX(paParameter.Alignment, paParameter.Text, paParameter.ClipRectangle);                                                          
                    
                    if (clDrawMode) clContext.fillText(paParameter.Text, paParameter.X, clYCord + (this.MeasureHeight(paParameter.Font) * clTextPaddingRatio));

                    if (paParameter.LineFeed)                        
                        this.LineFeedAfter(CastInteger(this.MeasureHeight(paParameter.Font) * paParameter.LineSpacing));                    

                    this.RestoreContext();
                },
                DrawTextLn : function(paParameter)
                {
                    paParameter.LineFeed = true;
                    this.DrawText(paParameter);                    
                },
                SetClipRectangle : function(paClipRectangle)
                {
                    if ((paClipRectangle) &&
                        (typeof paClipRectangle.X !== 'undefined') &&
                        (typeof paClipRectangle.Y !== 'undefined') &&
                        (typeof paClipRectangle.Width !== 'undefined') &&
                        (typeof paClipRectangle.Height !== 'undefined')) {
                        this.SaveContext();
                        clContext.beginPath();
                        clContext.rect(paClipRectangle.X + clLeftMargin, paClipRectangle.Y, paClipRectangle.Width, paClipRectangle.Height);
                        clContext.clip();
                    }                    
                },
                SaveContext : function()
                {
                    if (clDrawMode)
                    {
                        clContext.save();
                        clSavedStateCount++;
                    }
                },
                RestoreContext : function()
                {
                    if ((clDrawMode) && (clSavedStateCount > 0)) {
                        clContext.restore();
                        clSavedStateCount--;
                    }
                }

                

                


        //AddAjaxParam: function (paKey, paValue) {
        //    clAjaxData[paKey] = paValue;
        //},
        //AddObjectDataBlock: function (paKey, paValue, paAppendSysInfo) {
        //    if (paValue) 
        //    {
        //        if (paAppendSysInfo)
        //            paValue['accessinfo'] = window.__SYSVAR_CurrentGeoLocation || '';

        //        clAjaxData[paKey] = Base64.encode(JSON.stringify(paValue));
        //    }
        //},
        //AddStringDataBlock: function (paKey, paValue) {
        //    if (paValue) {
        //        clAjaxData[paKey] = Base64.encode(paValue);
        //    }
        //},
        //AddMessagePlaceHolder : function(paKey, paValue)
        //{
        //    clMessagePlaceHolderList[paKey] = paValue;
        //},
        //SetCompleteHandler : function(paCompleteHandler)
        //{
        //    clCompleteHandler = paCompleteHandler;
        //},
        //SetConfirmationResultHandler : function(paConfirmationResultHandler)
        //{
        //    clConfirmationResultHandler = paConfirmationResultHandler;
        //},
        //SetResponseDictionaryParsingHandler : function(paResponseDictionaryParsingHandler)
        //{
        //    clResponseDictionaryParsingHandler = paResponseDictionaryParsingHandler;
        //},
        //ShowCompleteMessage : function(paSuccess, paMessageCode, paResponseStruct)
        //{
        //    if (paMessageCode)
        //    {                        
        //        MessageHandler.ShowMessage(paMessageCode, function (paOption) { 
        //            $.each(clMessagePlaceHolderList, function(paName, paValue){
        //                paOption['message'] = paOption['message'].replace(paName, paValue); 
        //            });                            
        //        }).done(function (paButtonAction) {                            
        //            if (clCompleteHandler) clCompleteHandler(paSuccess, paResponseStruct, paButtonAction);
        //        });  
        //    }
        //    else if (clCompleteHandler) clCompleteHandler(paSuccess, paResponseStruct);
        //},
        //ExecuteOnConfirm : function(paMessageCode)
        //{
        //    var lcActiveObject = this;

        //    if (paMessageCode)
        //    {                        
        //        MessageHandler.ShowMessage(paMessageCode, function (paOption) { 
        //            $.each(clMessagePlaceHolderList, function(paName, paValue){
        //                paOption['message'] = paOption['message'].replace(paName, paValue); 
        //            });                            
        //        }).done(function (paResult) {

        //            if (clConfirmationResultHandler) clConfirmationResultHandler(paResult);

        //            if (paResult == 'yes')
        //            {                                
        //                lcActiveObject.Execute();
        //            }
        //        });
        //    }
        //    else lcActiveObject.Execute();
        //},
        //Execute : function()
        //{
        //    var lcActiveObject = this;

                    
        //    GlobalAjaxHandler.SetAjaxLoaderStatusText(MessageHandler.GetMessage(clAjaxLoaderStatusCode));
                    
        //    DoPostBack(clAjaxData, function (paResponseData) {                        
        //        var lcRespondStruct = jQuery.parseJSON(paResponseData);
        //        if (lcRespondStruct.Success) {                            
        //            lcActiveObject.ShowCompleteMessage(true, clSuccessMessageCode, lcRespondStruct);
        //        }
        //        else {
                            
        //            if ((lcRespondStruct.ResponseData.RSP_Dictionary) && (clResponseDictionaryParsingHandler))                                 
        //                clResponseDictionaryParsingHandler(clMessagePlaceHolderList, lcRespondStruct.ResponseData.RSP_Dictionary);                            
                            
        //            lcActiveObject.ShowCompleteMessage(false, lcRespondStruct.ResponseData.RSP_ErrorCode || clFailMessageCode, lcRespondStruct);
        //        }
        //    });
        //}
    }
};