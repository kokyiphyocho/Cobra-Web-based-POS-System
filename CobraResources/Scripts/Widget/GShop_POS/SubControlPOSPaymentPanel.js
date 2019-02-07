var POSPaymentManager = (function () {

    var clPaymentPopUp;
    var clPaymentKeypad;
    var clPaymentKeypadController;
    var clTitleBar;
    var clContentContainer;
    var clPaymentRowContainer;    
    var clTotalAmount;
    var clDeferred;

    return {
        Init: function () {
            clPaymentPopUp              = $('.SubControlPOSPaymentPanelPopUp[sa-elementtype=popup]');
            clPaymentKeypad             = clPaymentPopUp.find('.SubControlPOSKeyPad[ea-type=pospaymentkeypad]');
            clTitleBar                  = clPaymentPopUp.find('.TitleBar');
            clContentContainer          = clPaymentPopUp.find('.ContentContainer');
            clPaymentRowContainer       = clContentContainer.find('.PaymentContainer');
            clTotalAmount               = 0;
            
            POSPaymentManager.WaitForDependencies().done(function ()
            {                
                POSPaymentManager.BindEvents();
                clPaymentKeypadController = new KeypadManager(clPaymentKeypad);
                clPaymentKeypadController.Init();                                
            });            
        },
        WaitForDependencies: function () {
            var lcDeferred = $.Deferred();

            var lcWaitTimer = setInterval(function () {

                if (typeof KeypadManager !== 'undefined') {
                    if (lcDeferred.state() == 'pending') {
                        lcDeferred.resolve();
                        clearInterval(lcWaitTimer);
                    }
                }

            }, 200);

            return (lcDeferred);
        },
        SetPaymentKeys : function()
        {
            var lcPaymentTypeRowList;

            lcPaymentTypeRowList = clPaymentRowContainer.find('[ea-columnname]');            
            lcPaymentTypeRowList.each(function (paIndex) {                
                clPaymentKeypadController.ModifyKeyAttribute('reserved' + (paIndex + 1), $(this).attr('ea-columnname'), $(this).text());
            });            
        },
        BindEvents: function () {
            clPaymentKeypad.unbind('ev-initialized');
            clPaymentKeypad.bind('ev-initialized', POSPaymentManager.HandlerOnKeyPadInitialized);

            clPaymentKeypad.unbind('ev-keyaction');
            clPaymentKeypad.bind('ev-keyaction', POSPaymentManager.HandlerOnKeyAction);
            
            clTitleBar.find('a[ea-command="@cmd%closepopup"]').unbind('click');
            clTitleBar.find('a[ea-command="@cmd%closepopup"]').click(POSPaymentManager.HandlerOnClick);
        },
        ShowPaymentPopUp : function(paTotalAmount)
        {
            clDeferred = $.Deferred();

            clTotalAmount = CastDecimal(paTotalAmount, 0);

            if (clTotalAmount > 0) {
                POSPaymentManager.SetPaymentAmount('_totalamount', clTotalAmount);
                clPaymentPopUp.attr('fa-show', 'true');
            }
            else clDeferred.resolve(true, GetPaymentInfo());
            return (clDeferred);
        },
        ClosePaymentPopUp : function()
        {
            clPaymentPopUp.removeAttr('fa-show');
            POSPaymentManager.ResetPaymentAmount();

            if ((clDeferred) && (clDeferred.state() == 'pending'))
            {
                clDeferred.resolve(false, null);
            }
        },
        ResetPaymentAmount : function()
        {
            var lcPaymentTypeRowList; 
            
            lcPaymentTypeRowList = clPaymentRowContainer.find('[ea-columnname]');            
            
            lcPaymentTypeRowList.each(function (paIndex) {
                POSPaymentManager.SetPaymentAmount($(this).attr('ea-columnname'), 0);
            });
            
            POSPaymentManager.RefreshTotalPayment();
            clPaymentKeypadController.ClearScreen();
        },
        GetPaymentAmount : function(paPaymentType)
        {
            var lcPaymentTypeRowList;
            var lcTotalPayment          = 0;
                        
            if (!paPaymentType) {                
                lcPaymentTypeRowList = clPaymentRowContainer.find('[ea-columnname]');
            }
            else
                lcPaymentTypeRowList = clPaymentRowContainer.find('[ea-columnname="' + paPaymentType + '"]');            

            lcPaymentTypeRowList.each(function (paIndex) {                
                lcTotalPayment += CastDecimal($(this).attr('value'), 0);                
            });            

            return(lcTotalPayment);
        },
        GetRemainAmount : function(paPaymentType)
        {
            var lcPaymentAmount = POSPaymentManager.GetPaymentAmount() - POSPaymentManager.GetPaymentAmount(paPaymentType);

            if (lcPaymentAmount < clTotalAmount) 
                return (clTotalAmount - lcPaymentAmount);            
            else return (0);
        },
        RefreshTotalPayment : function()
        {
            var lcTotalPaymentRow       = clContentContainer.find('.TotalPaymentRow');            
            var lcTotalPaymentAmount    = POSPaymentManager.GetPaymentAmount();
            var lcCashPaymentAmount     = POSPaymentManager.GetPaymentAmount('paymentcash');
            
            POSPaymentManager.SetPaymentAmount('_totalpayment', lcTotalPaymentAmount);

            if ((lcCashPaymentAmount == lcTotalPaymentAmount) && (lcCashPaymentAmount >= clTotalAmount))           
                POSPaymentManager.SetPaymentAmount('change', lcCashPaymentAmount - clTotalAmount, true);
            else POSPaymentManager.SetPaymentAmount('change', -1, true);
                        
            if ((lcCashPaymentAmount < lcTotalPaymentAmount) && (lcTotalPaymentAmount > clTotalAmount))
                lcTotalPaymentRow.attr('fa-error', 'true');            
            else
                lcTotalPaymentRow.removeAttr('fa-error');
        },
        SetPaymentAmount : function(paColumnName, paAmount, paChangeMode)
        {
            var lcPaymentRow        = clContentContainer.find('[ea-columnname="' + paColumnName + '"]');
            var lcFigureBox         = lcPaymentRow.find('[sa-elementtype=figure]');
                        
            if (paAmount === '') paAmount = POSPaymentManager.GetRemainAmount(paColumnName);            

            paAmount = CastDecimal(paAmount, 0);
            
            if (lcPaymentRow.length > 0)
            {   

                if ((paAmount > 0) || (paAmount == 0 && paChangeMode)) {
                    lcPaymentRow.attr('value', paAmount);
                    lcFigureBox.text(FormManager.ConvertToFormLanguage(paAmount));
                }
                else {
                    lcPaymentRow.removeAttr('value');
                    lcFigureBox.text('');
                }
            }
        },
        GetPaymentInfo : function()
        {
            var lcPaymentTypeRowList;            
            var lcPaymentInfo = {};            
                       
            lcPaymentTypeRowList = clContentContainer.find('[ea-columnname]');
            
            lcPaymentTypeRowList.each(function (paIndex) {
                var lcAmount = CastDecimal($(this).attr('value'), 0);
                var lcColumnname = $(this).attr('ea-columnname');                
                lcPaymentInfo[lcColumnname] = CastDecimal($(this).attr('value'), 0);
            });

            lcPaymentInfo['paymentcash'] = lcPaymentInfo['paymentcash'] - lcPaymentInfo['change'];

            return(lcPaymentInfo);
        },
        ConfirmPayment : function()
        {
            var lcTotalPaymentAmount = POSPaymentManager.GetPaymentAmount();
            var lcCashPaymentAmount  = POSPaymentManager.GetPaymentAmount('paymentcash');

            if ((lcCashPaymentAmount < lcTotalPaymentAmount) && (lcTotalPaymentAmount > clTotalAmount))
                MessageHandler.ShowMessage('err_exceedpaymentamount');
            else if (lcTotalPaymentAmount < clTotalAmount)
                MessageHandler.ShowMessage('err_invalidpaymentamount');
            else
            {       
                if ((clDeferred) && (clDeferred.state() == 'pending'))
                {
                    clDeferred.resolve(true, POSPaymentManager.GetPaymentInfo());
                    POSPaymentManager.ClosePaymentPopUp();
                }
            }
        },
        HandlerOnKeyPadInitialized : function()
        {            
            POSPaymentManager.SetPaymentKeys();
            clPaymentKeypadController.DisableUnusedKeys();
        },
        HandlerOnKeyAction: function (paEvent, paKeyName, paKeyData, paScreenText)
        {
            paEvent.preventDefault();

            switch (paKeyName) {
                case 'reset':
                    {
                        POSPaymentManager.ResetPaymentAmount();
                        break;
                    }

                case 'enter' :
                    {
                        POSPaymentManager.ConfirmPayment();
                        break;
                    }

                default:
                    {                        
                        POSPaymentManager.SetPaymentAmount(paKeyName, paKeyData);
                        POSPaymentManager.RefreshTotalPayment();
                        clPaymentKeypadController.ClearScreen();
                        break;
                    }
            }
        },
        HandlerOnClick : function(paEvent)
        {
            paEvent.preventDefault();

            var lcCommand = $(this).attr('ea-command');
            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);

            switch (lcCommand) {
                case 'closepopup':
                    {
                        POSPaymentManager.ClosePaymentPopUp();
                        break;
                    }                
            }
        }
        

        //BindEvents: function () {
        //    clKeyPadComposite.find('a[ea-command^="@poskey%"]').unbind('click');
        //    clKeyPadComposite.find('a[ea-command^="@poskey%"]').click(function (paEvent) {
        //        paEvent.preventDefault();
        //        var lcKey = $(this);
        //        var lcKeyName = lcKey.attr('ea-command').substring(8);
        //        var lcDisabled = lcKey.attr('fa-disabled')

        //        if ((lcDisabled != 'true') && (!lcKey.is('.Disable')))
        //            KeyPadManager.ProcessKey(lcKeyName);
        //    });
        //},
        //ProcessKey: function (paKeyName) {
        //    switch (paKeyName) {
        //        case '0': case '1': case '2': case '3': case '4': case '5':
        //        case '6': case '7': case '8': case '9': case '00': KeyPadManager.InsertChar(paKeyName); break;

        //        case 'decimal': KeyPadManager.InsertDecimalPoint(); break;

        //        case 'backspace': KeyPadManager.BackSpaceKey(); break;

        //        case 'clear': KeyPadManager.ClearScreen(); break;
        //        case 'lakh': KeyPadManager.RightZeroPadding(6); break;
        //        case 'tenthousand': KeyPadManager.RightZeroPadding(5); break;
        //        case 'thousand': KeyPadManager.RightZeroPadding(4); break;
        //        case 'hundred': KeyPadManager.RightZeroPadding(3); break;
        //        case 'togglekeypad': KeyPadManager.ToggleKeyPad(); break;

        //        case 'item':
        //        case 'majorunit':
        //        case 'minorunit':
        //        case 'unitprice':
        //        case 'subtotal':
        //        case 'discountpercent':
        //        case 'discount':
        //        case 'enter':
        //        case 'void':
        //        case 'toparrow':
        //        case 'bottomarrow':
        //        case 'customer':
        //        case 'save':
        //            {
        //                clKeyPadComposite.trigger('ev-keyaction', [paKeyName, clLCDScreen.text().NormalizeNumber()]);
        //                break;
        //            }
        //    }
        //},

        //BackSpaceKey: function () {
        //    var lcExistingText = clLCDScreen.text();

        //    if (lcExistingText.length > 0) {
        //        clLCDScreen.text(lcExistingText.substring(0, lcExistingText.length - 1));
        //        KeyPadManager.RefreshScreenFormat();
        //    }
        //},

        //ClearScreen: function () {
        //    var lcExistingText = clLCDScreen.text();

        //    if (lcExistingText.length > 0) {
        //        clLCDScreen.text('');
        //        KeyPadManager.RefreshScreenFormat();
        //    }
        //},
        //SetEnableState: function (paKeyName, paEnable) {
        //    if ((paKeyName) && (clKeyPadComposite)) {
        //        var lcKey = clKeyPadComposite.find('a[ea-command="@poskey%' + paKeyName + '"]');
        //        if (lcKey.length > 0) {
        //            if (!paEnable)
        //                lcKey.attr('fa-disabled', 'true');
        //            else
        //                lcKey.removeAttr('fa-disabled');
        //        }
        //    }
        //},
        //ResetKeyPad: function () {
        //    KeyPadManager.ClearScreen();
        //    KeyPadManager.SetUnitNameKeys(null);
        //},

        //InsertChar: function (paKeyName) {
        //    var lcExistingText = clLCDScreen.text();

        //    clLCDScreen.text(lcExistingText.concat(paKeyName));
        //    KeyPadManager.RefreshScreenFormat();
        //},

        //InsertDecimalPoint: function () {
        //    var lcExistingText = clLCDScreen.text();

        //    if (lcExistingText.indexOf('.') == -1) {
        //        clLCDScreen.text(lcExistingText.concat('.'));
        //        KeyPadManager.RefreshScreenFormat();
        //    }
        //},

        //RightZeroPadding: function (paCount) {
        //    var lcExistingText = clLCDScreen.text().NormalizeNumber();
        //    var lcNumberArray = lcExistingText.split('.');

        //    if (Number(lcNumberArray[0]) > 0) {
        //        lcNumber = lcNumberArray[0];

        //        while ((lcNumber[lcNumber.length - 1] == '0'))
        //            lcNumber = lcNumber.substring(0, lcNumber.length - 1);

        //        if (lcNumber.length <= paCount) {
        //            while (lcNumber.length < paCount)
        //                lcNumber = lcNumber.concat('0');

        //            lcNumberArray[0] = lcNumber;
        //        }

        //        clLCDScreen.text(FormManager.ConvertToFormLanguage(lcNumberArray.join('.')));
        //    }
        //},

        //RefreshScreenFormat: function () {
        //    var lcExistingText = clLCDScreen.text().NormalizeNumber();

        //    if (lcExistingText.length > 0) {
        //        while ((lcExistingText[0] == '0') && (lcExistingText.length > 1) && (lcExistingText[1] != '.'))
        //            lcExistingText = lcExistingText.substring(1);
        //    }

        //    if (lcExistingText.length > 8) lcExistingText = lcExistingText.substring(0, 8);

        //    if ((lcExistingText.length > 0) && (lcExistingText[0] == '.'))
        //        lcExistingText = '0'.concat(lcExistingText);

        //    clLCDScreen.text(FormManager.ConvertToFormLanguage(lcExistingText));

        //    if (clLCDScreen.text().length >= 12) clLCDScreen.attr('fa-contentsize', '12');
        //    else if (clLCDScreen.text().length >= 9) clLCDScreen.attr('fa-contentsize', '9');
        //    else clLCDScreen.removeAttr('fa-contentsize');
        //},

        //ToggleKeyPad: function () {
        //    if (clKeyPadComposite.attr('fa-hidekeypad') != 'true') {
        //        clKeyPadComposite.attr('fa-hidekeypad', 'true');
        //        clKeyPadComposite.trigger('ev-keypadsizechanged', 'minimal');
        //    }
        //    else {
        //        clKeyPadComposite.removeAttr('fa-hidekeypad');
        //        clKeyPadComposite.trigger('ev-keypadsizechanged', 'full');
        //    }
        //},
        //SetUnitNameKeys: function (paItemInfo) {
        //    if (clKeyPadComposite) {
        //        var lcMajorKey = clKeyPadComposite.find('a[ea-command="@poskey%majorunit"]');
        //        var lcMinorKey = clKeyPadComposite.find('a[ea-command="@poskey%minorunit"]');

        //        if ((paItemInfo) && (paItemInfo.attr('gpos-itemstatus') != 'cancel')) {
        //            var lcMajorUnitName = paItemInfo.attr('gpos-majorunitname') || '';
        //            var lcMinorUnitName = paItemInfo.attr('gpos-minorunitname') || '';

        //            if (lcMajorUnitName == '')
        //                lcMajorKey.attr('fa-disabled', 'true');
        //            else
        //                lcMajorKey.removeAttr('fa-disabled');

        //            if (lcMinorUnitName == '')
        //                lcMinorKey.attr('fa-disabled', 'true');
        //            else
        //                lcMinorKey.removeAttr('fa-disabled');

        //            lcMajorKey.text(lcMajorUnitName == '' ? '-' : lcMajorUnitName);
        //            lcMinorKey.text(lcMinorUnitName == '' ? '-' : lcMinorUnitName);
        //        }
        //        else {
        //            lcMajorKey.attr('fa-disabled', 'true');
        //            lcMinorKey.attr('fa-disabled', 'true');

        //            lcMajorKey.text('-');
        //            lcMinorKey.text('-');
        //        }
        //    }
        //},

    }
})();


