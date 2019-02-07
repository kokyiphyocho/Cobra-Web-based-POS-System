var KeypadManager = function (paKeypadComposite) {
    var clKeypadComposite = paKeypadComposite;
    var clInstance;
    var clLCDScreen;
    var clLanguage;
    var clKeypadType;
    
    return {
                Init            :   function()
                                    {
                                        clInstance = this;                                        
                                        clLCDScreen = clKeypadComposite.find('.LCDScreen');
                                        clLanguage = clKeypadComposite.attr('ea-language');
                                        clKeypadType = clKeypadComposite.attr('ea-type');
                                        this.BindEvents();
                                    
                                        if (clKeypadType == 'poskeypad') this.SetUnitNameKeys();
                                        clKeypadComposite.trigger('ev-initialized');                                    
                                    },
                BindEvents      :   function()
                                    {                    
                                        clKeypadComposite.find('a[ea-command^="@poskey%"]').unbind('click');
                                        clKeypadComposite.find('a[ea-command^="@poskey%"]').click(this.HandlerOnClick);
                                        //clKeypadComposite.find('a[ea-command^="@poskey%"]').click(function (paEvent) {
                                        //    paEvent.preventDefault();
                                        //    var lcKey     = $(this);
                                        //    var lcKeyName = lcKey.attr('ea-command').substring(8);
                                        //    var lcDisabled = lcKey.attr('fa-disabled') 
                        
                                        //    if ((lcDisabled != 'true') && (!lcKey.is('.Disable')))
                                        //        clInstance.ProcessKey(lcKeyName);
                                        //});
                                    },
                ProcessKey      : function(paKeyName)
                                  {                                     
                                     switch (paKeyName) {
                                        case '0': case '1': case '2': case '3': case '4': case '5':
                                        case '6': case '7': case '8': case '9': case '00': this.InsertChar(paKeyName); break;

                                        case 'decimal': this.InsertDecimalPoint(); break;

                                        case 'backspace': this.BackSpaceKey(); break;

                                        case 'clear': this.ClearScreen(); break;
                                        case 'lakh' : this.RightZeroPadding(6); break;
                                        case 'tenthousand': this.RightZeroPadding(5); break;
                                        case 'thousand': this.RightZeroPadding(4); break;
                                        case 'hundred': this.RightZeroPadding(3); break;
                                        case 'togglekeypad': this.ToggleKeyPad(); break;

                                        //case 'item':
                                        //case 'majorunit':
                                        //case 'minorunit':
                                        //case 'unitprice':
                                        //case 'subtotal':
                                        //case 'discountpercent':
                                        //case 'discount':
                                        //case 'enter':
                                        //case 'void':                                        
                                        //case 'toparrow':
                                        //case 'bottomarrow':
                                        //case 'customer':
                                        //case 'save':
                                        default :
                                            {                                                
                                                clKeypadComposite.trigger('ev-keyaction', [paKeyName, clLCDScreen.text().NormalizeNumber()]);
                                                break;
                                             }
                                     }
                                  },
        
                BackSpaceKey      : function() 
                                    {
                                        var lcExistingText = clLCDScreen.text();

                                        if (lcExistingText.length > 0) {
                                            clLCDScreen.text(lcExistingText.substring(0, lcExistingText.length - 1));
                                            this.RefreshScreenFormat();
                                        }
                                    },

                ClearScreen     :   function() 
                                    {
                                        var lcExistingText = clLCDScreen.text();

                                        if (lcExistingText.length > 0) {
                                            clLCDScreen.text('');
                                            this.RefreshScreenFormat();
                                        }
                                    },                
                SetEnableState :    function(paKeyName, paEnable)
                                    {                    
                                        if ((paKeyName) && (clKeypadComposite))
                                        {
                                            var lcKey = clKeypadComposite.find('a[ea-command="@poskey%' + paKeyName + '"]');                        
                                            if (lcKey.length > 0)
                                            {
                                                if (!paEnable)
                                                    lcKey.attr('fa-disabled', 'true');
                                                else
                                                    lcKey.removeAttr('fa-disabled');
                                            }
                                        }
                                    },
                DisableUnusedKeys  : function ()
                                    {
                                        var clUnusedKeys = clKeypadComposite.find('a[ea-command^="@poskey%reserved"],a[ea-command^="@poskey%unused"]');

                                        clUnusedKeys.each(function (paIndex)
                                        {
                                            var lcCommand = $(this).attr('ea-command')
                                            var lcKeyName = lcCommand.substring(lcCommand.indexOf('%') + 1);
                                            clInstance.SetEnableState(lcKeyName, false);
                                        });
                                    },
                ModifyKeyAttribute : function (paKeyName, paNewKeyName, paDisplayText)
                                    {
                                        if ((paKeyName) && (clKeypadComposite)) {
                                            var lcKey = clKeypadComposite.find('a[ea-command="@poskey%' + paKeyName + '"]');
                                            if (lcKey.length > 0)
                                            {
                                                lcKey.text(paDisplayText);
                                                lcKey.attr('ea-command', '@poskey%' + paNewKeyName);
                                            }
                                        }
                                    },
                ResetKeyPad    :    function()
                                    {
                                        this.ClearScreen();
                                        this.SetUnitNameKeys(null);
                                    },

                InsertChar      :   function(paKeyName) 
                                    {
                                        var lcExistingText = clLCDScreen.text();
                                        
                                        clLCDScreen.text(lcExistingText.concat(paKeyName));
                                        this.RefreshScreenFormat();
                                    },

       InsertDecimalPoint      :   function() 
                                   {
                                        var lcExistingText = clLCDScreen.text();
                                        
                                        if (lcExistingText.indexOf('.') == -1) {
                                            clLCDScreen.text(lcExistingText.concat('.'));
                                            this.RefreshScreenFormat();
                                        }
                                    },

       RightZeroPadding         :   function (paCount) 
                                    {
                                       var lcExistingText = clLCDScreen.text().NormalizeNumber();
                                       var lcNumberArray = lcExistingText.split('.');
                                       
                                       if (Number(lcNumberArray[0]) > 0)
                                       {
                                           lcNumber = lcNumberArray[0];

                                            while ((lcNumber[lcNumber.length - 1] == '0'))
                                                lcNumber = lcNumber.substring(0, lcNumber.length - 1);

                                            if (lcNumber.length <= paCount)
                                            {
                                                while (lcNumber.length < paCount)
                                                    lcNumber = lcNumber.concat('0');

                                                lcNumberArray[0] = lcNumber;
                                            }
                                    
                                            clLCDScreen.text(FormManager.ConvertToFormLanguage(lcNumberArray.join('.')));
                                       }
                                    },

     RefreshScreenFormat       :   function() 
                                   {
                                        var lcExistingText = clLCDScreen.text().NormalizeNumber();
                                        
                                        if (lcExistingText.length > 0)
                                        {
                                            while ((lcExistingText[0] == '0') && (lcExistingText.length > 1) && (lcExistingText[1] != '.'))                                            
                                                lcExistingText = lcExistingText.substring(1);                                            
                                        }

                                        if (lcExistingText.length > 8) lcExistingText = lcExistingText.substring(0, 8);

                                        if ((lcExistingText.length > 0) && (lcExistingText[0] == '.'))
                                            lcExistingText = '0'.concat(lcExistingText);
                                                                               
                                        clLCDScreen.text(FormManager.ConvertToFormLanguage(lcExistingText));
                                        
                                        if (clLCDScreen.text().length >= 12) clLCDScreen.attr('fa-contentsize', '12');
                                        else if (clLCDScreen.text().length >= 9) clLCDScreen.attr('fa-contentsize', '9');
                                        else clLCDScreen.removeAttr('fa-contentsize');
                                   },

     ToggleKeyPad               :  function ()
                                   {
                                        if (clKeypadComposite.attr('fa-hidekeypad') != 'true')
                                            {
                                                clKeypadComposite.attr('fa-hidekeypad', 'true');
                                                clKeypadComposite.trigger('ev-keypadsizechanged', 'minimal');
                                            }
                                        else
                                        {
                                            clKeypadComposite.removeAttr('fa-hidekeypad');
                                            clKeypadComposite.trigger('ev-keypadsizechanged', 'full');
                                        }
                                   },     
     SetUnitNameKeys            : function(paItemInfo)
                                  {
                                      if (clKeypadComposite)
                                          {
                                            var lcMajorKey = clKeypadComposite.find('a[ea-command="@poskey%majorunit"]');
                                            var lcMinorKey = clKeypadComposite.find('a[ea-command="@poskey%minorunit"]');
                                            
                                            if ((paItemInfo) && (paItemInfo.attr('gpos-itemstatus') != 'cancel'))
                                            {                                                
                                                var lcMajorUnitName = paItemInfo.attr('gpos-majorunitname') || '';
                                                var lcMinorUnitName = paItemInfo.attr('gpos-minorunitname') || '';

                                                if (lcMajorUnitName == '')
                                                    lcMajorKey.attr('fa-disabled', 'true');
                                                else
                                                    lcMajorKey.removeAttr('fa-disabled');

                                                if (lcMinorUnitName == '')
                                                    lcMinorKey.attr('fa-disabled', 'true');
                                                else
                                                    lcMinorKey.removeAttr('fa-disabled');

                                                lcMajorKey.text(lcMajorUnitName == '' ? '-' : lcMajorUnitName);
                                                lcMinorKey.text(lcMinorUnitName == '' ? '-' : lcMinorUnitName);
                                            }
                                            else
                                            {                                             
                                                lcMajorKey.attr('fa-disabled', 'true');
                                                lcMinorKey.attr('fa-disabled', 'true');

                                                lcMajorKey.text('-');
                                                lcMinorKey.text('-');
                                            }
                                          }
                                  },
     HandlerOnClick             : function(paEvent)
                                  {
                                     paEvent.preventDefault();
                                     var lcKey = $(this);
                                     var lcCommand = lcKey.attr('ea-command')
                                     var lcKeyName = lcCommand.substring(lcCommand.indexOf('%') + 1);
                                     var lcDisabled = lcKey.attr('fa-disabled')
                                     
                                     if ((lcDisabled != 'true') && (!lcKey.is('.Disable')))
                                         clInstance.ProcessKey(lcKeyName);
                                  },

    }
};


