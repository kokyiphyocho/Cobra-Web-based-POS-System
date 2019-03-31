var ImageProcessingController = function (paImageProcessorPanel, paControl) {

    var clImageProcessorPanel = paImageProcessorPanel;
    var clControl = paControl;
    var clFileInputControl;
    var clImageControl;
    var clCanvas;
    var clContext;
    var clDrawInfo = {};
    
    return {
        Init: function () {
            clFileInputControl = clImageProcessorPanel.find('[sa-elementtype=panel][ea-type=hiddenpanel] input[type=file]');            
            clImageControl = clImageProcessorPanel.find('[sa-elementtype=panel][ea-type=hiddenpanel] img');
            clCanvas = clImageProcessorPanel.find('[sa-elementtype=panel][ea-type=canvaspanel] canvas');
            clContext = clCanvas[0].getContext('2d');
            
            this.ResetDrawParameters();
            this.BindEvents();            
        },
        BindEvents: function () {

            clImageProcessorPanel.find('a[ea-command^="@popupcmd%"]').unbind('click');
            clImageProcessorPanel.find('a[ea-command^="@popupcmd%"]').click(this, this.HandlerOnClick);
            
            clFileInputControl.unbind('change');
            clFileInputControl.change(this, this.HandlerOnChange);
            
            clImageControl.unbind('load');
            clImageControl.on('load',this,this.HandlerOnLoad);
        },
        ResetDrawParameters: function ()
        {
            $.extend(clDrawInfo, { ScaleFactor: 1, OriginX: 0, OriginY: 0, AspectRatio : 1 });
        },
        OpenPopUp: function () {            
            clControl.trigger('ev-imagepanelevent', { event: 'openpopup', sender: this });
            clImageProcessorPanel.attr('fa-show', 'true');
        },
        LoadImage : function(paFileName)
        {
            if (paFileName) {                
                var lcReader = new FileReader();

                lcReader.onload = function (paEvent) {                    
                    clImageControl.attr('src', paEvent.target.result);                    
                };

                lcReader.readAsDataURL(paFileName);
            }
        },
        IsEmptyImage : function () {
            var lcImage = clContext.getImageData(0, 0, clCanvas[0].width, clCanvas[0].height);            
            var lcImageData = lcImage.data;

            var lcDataLength = lcImageData.length;
            for (i = 0; i < lcDataLength; i += 4) {
                if (lcImageData[i + 3] > 0) {
                    var lcLuma = lcImageData[i] * 0.2126 + lcImageData[i + 1] * 0.7152 + lcImageData[i + 2] * 0.0722;
                    if (lcLuma < 240)
                    {
                        return (false);
                    }
                }               
            }
            return (true);
        },
        MakeGrayScale: function () {
            
            var lcSourceImage = clContext.getImageData(0, 0, clCanvas[0].width, clCanvas[0].height);
            var lcTargetImage = clContext.createImageData(clCanvas[0].width, clCanvas[0].height);
            var lcSourceData = lcSourceImage.data;
            var lcTargetData = lcTargetImage.data;

            var lcDataLength = lcSourceData.length;
            for (i = 0; i < lcDataLength; i += 4)
            {
                var lcLuma = lcSourceData[i] * 0.2126 + lcSourceData[i + 1] * 0.7152 + lcSourceData[i + 2] * 0.0722;
                lcTargetData[i] = lcLuma;
                lcTargetData[i + 1] = lcLuma;
                lcTargetData[i + 2] = lcLuma;
                lcTargetData[i + 3] = lcSourceData[i + 3]
            } 

            clContext.clearRect(0, 0, 300, 100);
            clContext.putImageData(lcTargetImage, 0, 0);
        },
        DrawImage: function (paFileName) {           
            clContext.clearRect(0, 0, 300, 100);

            clContext.drawImage(clImageControl[0], clDrawInfo.OriginX, clDrawInfo.OriginY, 300 * clDrawInfo.ScaleFactor, 300 * clDrawInfo.ScaleFactor / clDrawInfo.AspectRatio);          
        },
        ClosePopUp: function () {
            clControl.trigger('ev-imagepanelevent', { event: 'closepopup', sender: this });
            clImageProcessorPanel.removeAttr('fa-show');
        },
        SetImage: function () {
            this.MakeGrayScale();
            clCanvas[0].toBlob(function (paBlob) {                
                clControl.trigger('ev-imagepanelevent', { event: 'setimage', sender: this, blob: paBlob, dataurl: clCanvas[0].toDataURL() });                
            });
            this.ClosePopUp();
        },
        HandlerOnLoad:function(paEvent)
        {
            paEvent.data.ResetDrawParameters();
            clDrawInfo.AspectRatio = this.width / this.height;
            paEvent.data.DrawImage();
        },
        HandlerOnChange: function (paEvent) {            
            paEvent.data.LoadImage(clFileInputControl[0].files[0]);
        },
        HandlerOnClick: function (paEvent) {
            paEvent.preventDefault();

            var lcCommand = $(this).attr('ea-command');
            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);

            switch (lcCommand) {
                case 'chooseimage':
                    {                        
                        clFileInputControl.trigger('click');

                        break;
                    }

                case 'moveleft':
                    {
                        clDrawInfo.OriginX -= 1;
                        paEvent.data.DrawImage();
                        break;
                    }

                case 'moveright':
                    {
                        clDrawInfo.OriginX += 1;
                        paEvent.data.DrawImage();
                        break;
                    }

                case 'moveup':
                    {
                        clDrawInfo.OriginY -= 1;
                        paEvent.data.DrawImage();
                        break;
                    }

                case 'movedown':
                    {
                        clDrawInfo.OriginY += 1;
                        paEvent.data.DrawImage();
                        break;
                    }

                case 'zoomin':
                    {
                        clDrawInfo.ScaleFactor += 0.01;
                        paEvent.data.DrawImage();
                        break;
                    }

                case 'zoomout':
                    {
                        clDrawInfo.ScaleFactor -= 0.01;
                        paEvent.data.DrawImage();
                        break;
                    }

                case 'reset':
                    {                        
                        paEvent.data.ResetDrawParameters();
                        paEvent.data.DrawImage();
                        break;
                    }

                case 'ok' :
                    {
                        paEvent.data.SetImage();
                        break;
                    }

                case 'cancel':
                case 'popupclose':
                    {
                        paEvent.data.ClosePopUp();
                        break;
                    }
            }
        }
    } // Return

};





