//$(document).ready(function () {  
//    POSTransactionListManager.Init($('[sa-elementtype=composite].SubControlPOSTransactionList'));
//});

var POSTransactionListManager = (function () {
    var clControl;
    var clTransactionListComposite;
    var clElementTemplate;
    var clItemListContainer;
    var clMessageBar;
    var clSummaryBar;    
    var clList = [];    
     
    return {
        Init: function (paTransactionListComposite, paControl, paNewList) {
            clControl = paControl;
            clTransactionListComposite = paTransactionListComposite;
            clItemListContainer = clTransactionListComposite.find('[sa-elementtype=list]');
            clElementTemplate = clTransactionListComposite.find('[sa-elementtype=item][ea-type=template]');
            clMessageBar = clTransactionListComposite.find('[sa-elementtype=messagebar]');
            clSummaryBar = clTransactionListComposite.find('[sa-elementtype=summary]');

            POSSummaryBarController.Init(paTransactionListComposite, clControl);
            if (paNewList) POSTransactionListManager.AppendNewElement();

            POSTransactionListManager.BindEvents();
        },
        SetExistingReceiptInfo : function(paMasterBlock, paListContent)
        {
            if ((paMasterBlock) && (paListContent))
            {                
                $(paListContent).find('[sa-elementtype=item]').each(function () {                    
                    POSTransactionListManager.AppendExistingElement($(this));
                });

                clTransactionListComposite.attr('fa-entrycount', POSTransactionListManager.GetEntryCount());

                POSSummaryBarController.SetDiscountValue(POSTransactionManager.GetMasterBlockElementValue('discount'));
                POSSummaryBarController.SetSubtotal(POSTransactionListManager.SumEnlistedPrice());

                POSTransactionListManager.SetActiveElement(clList.length - 1);
            }
        },
        BindEvents: function () {
            clSummaryBar.unbind('click');
            clSummaryBar.click(POSTransactionListManager.HandlerOnItemClick);
        },
        SetFullScreenMode : function(paFullScreen)
        {
            if (paFullScreen) {
                if (clTransactionListComposite.attr('fa-fullscreenmode') != 'true') {
                    clTransactionListComposite.attr('fa-fullscreenmode', true);
                    clTransactionListComposite.trigger('ev-fullscreenmodechanged', ['true']);
                    setTimeout(POSTransactionListManager.AdjustActiveElementScroll, 600);                    
                }
            }
            else {

                if (clTransactionListComposite.attr('fa-fullscreenmode') == 'true') {
                    clTransactionListComposite.removeAttr('fa-fullscreenmode');
                    clTransactionListComposite.trigger('fullscreenmodechanged', ['false']);
                    setTimeout(POSTransactionListManager.AdjustActiveElementScroll, 600);
                }                
            }
        },
        CloneElementTemplate: function () {
            var lcItemElement = clElementTemplate.clone();

            lcItemElement.removeAttr('ea-type');
            lcItemElement.attr('fa-unconfirm', 'true');
            lcItemElement.POSItemInfo = null;            

            return (lcItemElement);
        },
        AppendNewElement: function () {            
            if (clItemListContainer.find('[sa-elementtype=item][fa-unconfirm=true]').length == 0) {
                var lcItemElement = POSTransactionListManager.CloneElementTemplate();

                clItemListContainer.append(lcItemElement);
                clList.push(lcItemElement);

                lcItemElement.unbind('click');
                lcItemElement.click(POSTransactionListManager.HandlerOnItemClick);                

                POSTransactionListManager.SetActiveElement(clList.length - 1);
                POSTransactionListManager.GotoElement('last');
                return (lcItemElement);
            }
            else POSTransactionListManager.SetActiveElement(clList.length - 1);
        },
        AppendExistingElement : function(paItemElement)
        {
            var lcItemElementController;

            if (paItemElement) {
                clItemListContainer.append(paItemElement);
                clList.push(paItemElement);

                paItemElement.unbind('click');
                paItemElement.click(POSTransactionListManager.HandlerOnItemClick);

                lcItemElementController = new POSItemElementController(paItemElement);
                lcItemElementController.AccomodateItem();

                return (paItemElement);
            }
        },
        SetActiveElement : function(paIndex)
        {
            paIndex = Number(paIndex);
            
            $.each(clList, function (paIndex, paItemElement) {
                paItemElement.removeAttr('fa-active');
            });

            if ((paIndex >= 0) && (paIndex < clList.length))
            {               
                clList[paIndex].attr('fa-active', 'true');
                POSSummaryBarController.SetActiveMode(false);
                POSTransactionListManager.AdjustScroll(clList[paIndex]);

                // var lcPOSItemInfo = clList[paIndex].POSItemInfo;
                var lcElementType = clList[paIndex].attr('sa-elementtype');

                clTransactionListComposite.trigger('ev-activeelementchanged', [lcElementType, clList[paIndex]]);
            }
            else if (paIndex == -1)
            {
                POSSummaryBarController.SetActiveMode(true);

                var lcElementType = clSummaryBar.attr('sa-elementtype');
                clTransactionListComposite.trigger('ev-activeelementchanged', [lcElementType]);
            }
        },
        AdjustScroll: function(paElement)
        {
            var lcTop = clItemListContainer.scrollTop();
            var lcBottom = lcTop + clItemListContainer.height();

            if (paElement)
            {
                POSTransactionListManager.ScrollIntoView(paElement, clItemListContainer);
                //if (paElement.offset().top < lcTop) {
                //    clItemListContainer.scrollTop(paElement.offset().top - clItemListContainer.height);
                //}
                //if (paElement.offset().top < lcTop) paElement[0].scrollIntoView(true);
                //else if (paElement.offset().top + paElement.height() > lcBottom) paElement[0].scrollIntoView(false);
            }           
        },
        ScrollIntoView: function (paItem, paParent) 
        {
            var lcBorderTop     = parseFloat($.css(paParent[0], 'borderTopWidth')) || 0;
            var lcPaddingTop    = parseFloat($.css(paParent[0], 'paddingTop')) || 0;
            var lcOffset        = paItem.offset().top - paParent.offset().top - lcBorderTop - lcPaddingTop;
            var lcScroll        = paParent.scrollTop();
            var lcElementHeight = paParent.height();
            var lcItemHeight    = paItem.outerHeight();

            if (lcOffset < 0) 
            {
                paParent.scrollTop(lcScroll + lcOffset);
            } else if (lcOffset + lcItemHeight > lcElementHeight) {
                paParent.scrollTop(lcScroll + lcOffset - lcElementHeight + lcItemHeight);
            }
        },
        AdjustActiveElementScroll : function()
        {
            var lcIndex = POSTransactionListManager.GetActiveElementIndex();

            if (lcIndex >= 0)
            {
                POSTransactionListManager.AdjustScroll(clList[lcIndex]);
            }
        },
        GetActiveElement : function()
        {
            var lcItemElementController;

            lcItemElementController = null;

            $.each(clList, function (paIndex, paItemElement) {
                if (paItemElement.attr('fa-active') == 'true') {                    
                    lcItemElementController = new POSItemElementController(paItemElement); // START HERE                              
                    return (false);
                }
            });

            return (lcItemElementController);
        },       
        GetActiveElementIndex : function()
        {
            var lcIndex;

            lcIndex = -1;

            $.each(clList, function (paIndex, paItemElement) {
                if (paItemElement.attr('fa-active') == 'true') {
                    lcIndex = paIndex;
                    return (false);
                }
            });

            return (lcIndex);
        },
        GotoElement : function(paParameter)
        {
            if (paParameter)
            {
                if (paParameter == 'first') POSTransactionListManager.SetActiveElement(0);
                else if (paParameter == 'last') {                    
                    POSTransactionListManager.SetActiveElement(clList.length - 1);
                }
                else SetActiveElement(paParameter)
            }
        },
        GetEntryCount : function()
        {
            var lcEntryCount = 0;

            $.each(clList, function (paIndex, paItemElement) {
                if (!paItemElement.attr('fa-unconfirm')) {                    
                    lcEntryCount += 1;
                }
            });

            return (lcEntryCount);
        },
        SetItemInfo: function (paItemInfo) {
            var lcItemElementController = POSTransactionListManager.GetActiveElement();
            lcItemElementController.SetPOSItemInfo(paItemInfo);
            lcItemElementController.RefreshDisplayInfo();
            POSTransactionListManager.AdjustActiveElementScroll();
        },
        SetQuantity: function (paQuantity, paMajorUnit)
        {            
            var lcItemElementController = POSTransactionListManager.GetActiveElement();
            
            if (lcItemElementController) {
                if (paQuantity > 0) {
                    lcItemElementController.SetQuantity(paQuantity, paMajorUnit);
                    lcItemElementController.RefreshDisplayInfo();
                    POSSummaryBarController.SetSubtotal(POSTransactionListManager.SumEnlistedPrice());
                    POSTransactionListManager.AdjustActiveElementScroll();
                }
            }
        },
        SetUnitPrice: function (paPrice) {
            var lcItemElementController = POSTransactionListManager.GetActiveElement();

            if (lcItemElementController) {                
                lcItemElementController.SetUnitPrice(paPrice);
                lcItemElementController.RefreshDisplayInfo();
                POSSummaryBarController.SetSubtotal(POSTransactionListManager.SumEnlistedPrice());
                POSTransactionListManager.AdjustActiveElementScroll();
            }
        },
        SetDiscountPercent: function (paPercent) {
            var lcItemElementController = POSTransactionListManager.GetActiveElement();

            if (lcItemElementController) {                
                lcItemElementController.SetDiscountPercent(paPercent);
                lcItemElementController.RefreshDisplayInfo();
                POSSummaryBarController.SetSubtotal(POSTransactionListManager.SumEnlistedPrice());
                POSTransactionListManager.AdjustActiveElementScroll();
            }
            else if (POSSummaryBarController.IsActive())
            {
                POSSummaryBarController.SetDiscountPercent(paPercent);                
            }
        },
        SetDiscountValue: function (paValue) {
            var lcItemElementController = POSTransactionListManager.GetActiveElement();
            
            if (lcItemElementController) {                
                lcItemElementController.SetDiscountValue(paValue);
                lcItemElementController.RefreshDisplayInfo();
                POSSummaryBarController.SetSubtotal(POSTransactionListManager.SumEnlistedPrice());
                POSTransactionListManager.AdjustActiveElementScroll();
            }
            else if (POSSummaryBarController.IsActive())
            {                
                POSSummaryBarController.SetDiscountValue(paValue);                
            }
        },
        SetSubTotalAmount: function (paSubTotal) {
            var lcItemElementController = POSTransactionListManager.GetActiveElement();

            if (lcItemElementController) {                
                lcItemElementController.SetSubTotalAmount(paSubTotal);
                lcItemElementController.RefreshDisplayInfo();
                POSSummaryBarController.SetSubtotal(POSTransactionListManager.SumEnlistedPrice());
                POSTransactionListManager.AdjustActiveElementScroll();
            }
            else if (POSSummaryBarController.IsActive())
            {
                POSSummaryBarController.SetTotal(paSubTotal);
            }
        },
        RefreshItemElement : function(paItemElement)
        {            
            var lcItemElementController = new POSItemElementController(paItemElement);
            lcItemElementController.RefreshDisplayInfo();
        },
        ResetList : function() // For New Form
        {
            clList = [];
            clItemListContainer.empty();            

            POSTransactionListManager.AppendNewElement();
            clTransactionListComposite.removeAttr('fa-entrycount');
            POSSummaryBarController.SetSubtotal(0);
            POSSummaryBarController.SetDiscountValue(0);
        },
        DeleteActiveEntry: function () {
            var lcIndex = POSTransactionListManager.GetActiveElementIndex();
            var lcActiveElement = clList[lcIndex];            

            if (lcActiveElement) {
                var lcSerialNo = FormManager.ConvertToFormLanguage((lcActiveElement.attr('fa-serial') || '').ForceConvertToInteger(0));
                
                if (!lcActiveElement.attr('fa-unconfirm')) {
                    MessageHandler.ShowMessage('confirm_deleterecord', function (paOption) { paOption['message'] = paOption['message'].replace('$SERIAL', lcSerialNo.toString()); }).done(function (paResult) {
                        if (paResult == 'yes') {
                            lcActiveElement.remove();
                            clList.splice(lcIndex, 1);
                            if (clList.length == 0) POSTransactionListManager.AppendNewElement();
                            if (lcIndex >= clList.length) lcIndex = clList.length - 1;
                            POSTransactionListManager.SetActiveElement(lcIndex);

                            var lcEntryCount = POSTransactionListManager.GetEntryCount();
                            if (lcEntryCount > 0) clTransactionListComposite.attr('fa-entrycount', lcEntryCount);
                            else clTransactionListComposite.removeAttr('fa-entrycount');
                            POSSummaryBarController.SetSubtotal(POSTransactionListManager.SumEnlistedPrice());
                        }
                    });
                }
                else
                {                    
                    lcActiveElement.remove();
                    clList.splice(lcIndex, 1);
                    if (clList.length == 0) POSTransactionListManager.AppendNewElement();
                    if (lcIndex >= clList.length) lcIndex = clList.length - 1;
                    POSTransactionListManager.SetActiveElement(lcIndex);

                    var lcEntryCount = POSTransactionListManager.GetEntryCount();
                    if (lcEntryCount > 0) clTransactionListComposite.attr('fa-entrycount', lcEntryCount);
                    else clTransactionListComposite.removeAttr('fa-entrycount');
                    POSSummaryBarController.SetSubtotal(POSTransactionListManager.SumEnlistedPrice());
                }              
            }
            else if (POSSummaryBarController.IsActive())
            {
                MessageHandler.ShowMessage('confirm_deleteallrecord').done(function (paResult) {
                    if (paResult == 'yes') {
                        var lcCount;
                        for (lcCount = 0; lcCount < clList.length; lcCount++)
                        {
                            clList[lcCount].remove();                            
                        }
                        clList = [];

                        POSTransactionListManager.AppendNewElement();                        

                        var lcEntryCount = POSTransactionListManager.GetEntryCount();
                        if (lcEntryCount > 0) clTransactionListComposite.attr('fa-entrycount', lcEntryCount);
                        else clTransactionListComposite.removeAttr('fa-entrycount');
                        POSSummaryBarController.SetSubtotal(POSTransactionListManager.SumEnlistedPrice());
                    }
                });
            }
        },
        EnlistItem : function()
        {
            var lcIndex = POSTransactionListManager.GetActiveElementIndex();
            var lcActiveElement = clList[lcIndex];

            if ((lcActiveElement) && (lcActiveElement.attr('fa-unconfirm'))) {
                var lcItemElementController = new POSItemElementController(lcActiveElement);
                var lcNewEmptyElement = lcItemElementController.IsNewEmptyElement();
                var lcErrorCode = lcItemElementController.VerifyEntry();
                if (lcErrorCode == 0) {
                    lcItemElementController.SetSerialNo(lcIndex + 1);
                    lcActiveElement.removeAttr('fa-unconfirm');
                    clTransactionListComposite.attr('fa-entrycount', POSTransactionListManager.GetEntryCount());

                    POSSummaryBarController.SetSubtotal(POSTransactionListManager.SumEnlistedPrice());
                    return (true);
                }
                else {
                    if ((lcNewEmptyElement) && (clList.length > 1)) {
                        lcActiveElement.remove();
                        clList.pop();
                        POSTransactionListManager.SetActiveElement(-1);
                        return (false);
                    }   
                    else {
                        POSTransactionListManager.ShowMessageBar(lcErrorCode);
                        return (false);
                    }
                }
            }
            else 
            {
                if (!lcActiveElement)
                {
                    if (POSSummaryBarController.IsActive())
                    {
                        clTransactionListComposite.trigger('ev-save');
                        return(false);
                    }
                }
            }

            return (true);
        },       
        SumEnlistedPrice  : function()
        {
            var lcSumSubtotal = 0;

            $.each(clList, function (paIndex, paItemElement) {
                var lcItemElementController = new POSItemElementController(paItemElement);
                var lcSubtotal = lcItemElementController.GetSubTotal();
                
                if (lcSubtotal == -1)
                {
                    lcSumSubtotal = -1;
                    return(false);
                }
                else lcSumSubtotal += lcSubtotal;                
            });

            return (lcSumSubtotal);
        },
        ShowMessageBar : function(paMessageCode)
        {
            var lcMessage;

            if ((paMessageCode) && (lcMessage = MessageHandler.GetMessage(paMessageCode)))
            {
                clMessageBar.text(lcMessage);
                clTransactionListComposite.attr('fa-showmessage', 'true');
                setTimeout(function () { clTransactionListComposite.removeAttr('fa-showmessage') }, 5000);
            }            
        },
        HandlerOnItemClick : function(paEvent)
        {
            paEvent.preventDefault();
            
            var lcElementType = $(this).attr('sa-elementtype');

            if (lcElementType == 'item') {
                var lcIndex = $(this).index();
                var lcPOSItemInfo = clList[lcIndex].POSItemInfo;

                POSTransactionListManager.SetActiveElement(lcIndex);               
            }
            else if (lcElementType == 'summary')
            {
                POSTransactionListManager.SetActiveElement(-1);             
            }
        },
        VerifyTransactionList : function()
        {
            var lcReturnCode = true;;
                        
            if (clList.length == 0) 
            {            
                MessageHandler.ShowMessage('err_transactionlistempty');                
                lcReturnCode = false;
            }
            else 
            {
                $.each(clList, function (paIndex, paItemElement) {
                    if (paItemElement.attr('fa-unconfirm'))
                    {             
                        MessageHandler.ShowMessage('err_unconfirmiteminlist');
                        lcReturnCode = false;
                        return (false)
                    }
                });
            }

            return (lcReturnCode);
        },
        GetTransactionListArray : function()
        {
            var lcItemElementController;
            var lcTransactionListArray = [];
            var lcDataArray;
                        
            $.each(clList, function (paIndex, paItemElement) {
                if (!paItemElement.attr('fa-unconfirm')) {
                    lcItemElementController = new POSItemElementController(paItemElement);

                    lcDataArray = lcItemElementController.GetDataArray();
                    if (lcDataArray) lcTransactionListArray.push(lcDataArray);
                }
            });
            
            return(lcTransactionListArray);
        }
    }
})();


(function ($) {
    var POSItemInfo = null;
})(jQuery);


var POSSummaryBarController = (function () {
    var clControl;
    var clComposite;
    var clSummaryBar;
    var clSubtotalBox;
    var clDiscountBox;
    var clSubtotal2Box;
    var clTaxAmountBox;
    var clQuantityBox;
    var clTotalBox;
    var clTaxApplicable;
    var clTaxPercent;
    var clTaxInclusive;

    return {
                Init : function(paComposite, paControl)
                {
                    clControl       = paControl;
                    clComposite     = paComposite;
                    clSummaryBar    = clComposite.find('[sa-elementtype=summary]');
                    clQuantityBox   = clSummaryBar.find('[sa-elementtype=row].Quantity [sa-elementtype=figure]');
                    clSubtotalBox   = clSummaryBar.find('[sa-elementtype=row].Subtotal [sa-elementtype=figure]');                    
                    clDiscountBox   = clSummaryBar.find('[sa-elementtype=row].Discount [sa-elementtype=figure]');
                    clSubtotal2Box  = clSummaryBar.find('[sa-elementtype=row].Subtotal2 [sa-elementtype=figure]');
                    clTaxAmountBox  = clSummaryBar.find('[sa-elementtype=row].TaxAmount [sa-elementtype=figure]');
                    clTotalBox      = clSummaryBar.find('[sa-elementtype=row].Total [sa-elementtype=figure]');
                    clTaxApplicable     =(clControl.data('transactionsetting').taxapplicable == 'true');
                    clTaxPercent        = (clControl.data('transactionsetting').taxpercent || '').ForceConvertToDecimal(0);
                    clTaxInclusive      = (clControl.data('transactionsetting').taxinclusive == 'true');                    
                },                
                IsActive : function()
                {                    
                    return (clSummaryBar.attr('fa-active') == 'true');
                },
                SetActiveMode : function(paActive)
                {
                    if (paActive) {
                        clSummaryBar.attr('fa-active', 'true');
                    }
                    else clSummaryBar.removeAttr('fa-active');
                },
                SetSubtotal : function(paValue)
                {
                    paValue = (paValue || '').toString().ForceConvertToDecimal(0);
                    var lcDiscountMode = clDiscountBox.attr('fa-discountmode');

                    if (lcDiscountMode == 'auto') POSSummaryBarController.SetDiscountValue(0);

                    clSubtotalBox.attr('value', paValue);                    
                    POSSummaryBarController.RefreshDisplayInfo();
                },
                SetTotal: function (paTotal) {
                    paTotal = (paTotal || '').toString().ForceConvertToDecimal(0);
                    paSubTotal = (clSubtotalBox.attr('value') || '').ForceConvertToDecimal(0);

                    var lcDiscount = paSubTotal - paTotal;

                    if (lcDiscount > 0)
                    {
                        POSSummaryBarController.SetDiscountValue(lcDiscount, 'auto');
                    }

                    POSSummaryBarController.RefreshDisplayInfo();
                },
                SetDiscountPercent : function(paPercent)
                {
                    paPercent = (paPercent || '').toString().ForceConvertToDecimal(0);

                    if (paPercent > 0) {                        
                        clComposite.attr('fa-overalldiscount', 'true');
                        paPercent = paPercent < 100 ? paPercent : 100;
                        clDiscountBox.attr('fa-discountmode', 'percent');
                        clDiscountBox.attr('value', paPercent);
                        clDiscountBox.attr('fa-displaytext', '(' + FormManager.ConvertToFormLanguage(paPercent) + '%)');
                    }
                    else {                        
                        clComposite.removeAttr('fa-overalldiscount');
                        clDiscountBox.removeAttr('fa-discountmode');
                        clDiscountBox.removeAttr('value');
                        clDiscountBox.removeAttr('fa-displaytext');
                    }

                    POSSummaryBarController.RefreshDisplayInfo();
                },
                SetDiscountValue: function (paValue, paDiscountMode) {
                    paValue = (paValue || '').toString().ForceConvertToDecimal(0);
                    paDiscountMode = (paDiscountMode || 'value');

                    if (paValue > 0) {                        
                        clComposite.attr('fa-overalldiscount', 'true');
                        clDiscountBox.attr('fa-discountmode', paDiscountMode);
                        clDiscountBox.attr('value', paValue);
                    }
                    else {                        
                        clComposite.removeAttr('fa-overalldiscount');
                        clDiscountBox.removeAttr('fa-discountmode');
                        clDiscountBox.removeAttr('value');
                    }
                    clDiscountBox.removeAttr('fa-displaytext');

                    POSSummaryBarController.RefreshDisplayInfo();
                },
                RefreshDisplayInfo : function()
                {
                    var lcEventInfo     = {};
                    var lcTotal         = 0;
                    var lcTaxAmount     = 0;

                    var lcSubtotal = (clSubtotalBox.attr('value') || '').ForceConvertToDecimal(0);
                    var lcDiscount = (clDiscountBox.attr('value') || '').ForceConvertToDecimal(0);
                    var lcDiscountMode = (clDiscountBox.attr('fa-discountmode') || 'value');
                    var lcQuantity = (clComposite.attr('fa-entrycount') || '').ForceConvertToInteger(0);

                    if (lcDiscountMode == 'percent') lcDiscount = Math.round(lcSubtotal * lcDiscount) / 100;

                    var lcSubtotal2 = Math.round((lcSubtotal - lcDiscount) * 100) / 100;
                    
                    if (clTaxApplicable)
                    {                        
                        if ((clTaxPercent > 0) && (clTaxInclusive == false))
                            lcTaxAmount = ((lcSubtotal2 * clTaxPercent / 100) * 100) / 100;
                    }
                    
                        
                    lcTotal = lcSubtotal2 + lcTaxAmount;

                    POSTransactionManager.SetMasterBlockElement('discount', lcDiscount);

                    clTotalBox.attr('value', lcTotal);                    
                    clTotalBox.text(lcTotal > 0       ? FormManager.ConvertToFormLanguage(lcTotal) : '');
                    clSubtotalBox.text(lcSubtotal > 0 ? FormManager.ConvertToFormLanguage(lcSubtotal) : '');
                    clDiscountBox.text(lcDiscount > 0 ? FormManager.ConvertToFormLanguage(lcDiscount) : '');
                    clQuantityBox.text(lcQuantity > 0 ? FormManager.ConvertToFormLanguage(lcQuantity) : '');

                    if (clSubtotal2Box) clSubtotal2Box.text(lcSubtotal2 > 0 ? FormManager.ConvertToFormLanguage(lcSubtotal2) : '');
                    if (clTaxAmountBox) clTaxAmountBox.text(lcTaxAmount > 0 ? FormManager.ConvertToFormLanguage(lcTaxAmount) : '');
                    
                    lcEventInfo["subtotal"]         = lcSubtotal;
                    lcEventInfo["discount"]         = lcDiscount;
                    lcEventInfo["subtotal2"]        = lcSubtotal2;
                    lcEventInfo["taxamount"]        = lcTaxAmount;
                    lcEventInfo["total"]            = lcTotal;                    
                    lcEventInfo["defaultaction"]    = true;

                    clComposite.trigger('ev-totalchanged', lcEventInfo);
                }
           };
})();

var POSItemElementController = function (paItemElement) {
    var   clComposite       = paItemElement.closest('[sa-elementtype=composite][ea-type=transactionlist]');
    var   clControl         = clComposite.closest('[sa-elementtype=control]');
    var   clControlMode     = clComposite.closest('[sa-elementtype=control]').attr('ea-mode') || '';
    var   clItemElement     = paItemElement;
    var   clSerialBox       = clItemElement.find('[sa-elementtype=element][ea-type=serial]');
    var   clDescriptionBox  = clItemElement.find('[sa-elementtype=element][ea-type=description]');
    var   clQuantityBox     = clItemElement.find('[sa-elementtype=element][ea-type=quantity]');
    var   clUnitPriceBox    = clItemElement.find('[sa-elementtype=element][ea-type=unitprice]');
    var   clDiscountBox     = clItemElement.find('[sa-elementtype=element][ea-type=discount]');
    var   clSubtotalBox     = clItemElement.find('[sa-elementtype=element][ea-type=subtotal]');
    
    var clGPOSItemID            = '';
    var clGPOSEntryType         = '';
    var clGPOSEntryAttribute    = '';
    var clGPOSItemText          = '';
    var clGPOSMajorPrice        = 0;
    var clGPOSMinorPrice        = 0;
    var clGPOSMajorUnitName     = '';
    var clGPOSMinorUnitName     = '';
    var clGPOSUnitRelationship  = 0;
        
    var clTaxApplicable         = (clControl.data('transactionsetting').taxapplicable || 'false') == 'true';
    var clTaxInclusive          = (clControl.data('transactionsetting').taxinclusive || 'false') == 'true';
    var clTaxPercent            = (clControl.data('transactionsetting').taxpercent || '').ForceConvertToDecimal(0);
    
    if (clItemElement.POSItemInfo)
    {
        clGPOSItemID            = clItemElement.POSItemInfo.attr('gpos-itemid') || '';
        clGPOSEntryType         = clItemElement.POSItemInfo.attr('gpos-entrytype') || 'item';
        clGPOSEntryAttribute    = clItemElement.POSItemInfo.attr('gpos-entryattribute') || '';
        clGPOSItemText          = clItemElement.POSItemInfo.attr('gpos-itemtext') || 'major';
        clGPOSMajorPrice        = (clItemElement.POSItemInfo.attr('gpos-majorprice') || '').ForceConvertToInteger(0);
        clGPOSMinorPrice        = (clItemElement.POSItemInfo.attr('gpos-minorprice') || '').ForceConvertToInteger(0);
        clGPOSMajorUnitName     = clItemElement.POSItemInfo.attr('gpos-majorunitname') || '';
        clGPOSMinorUnitName     = clItemElement.POSItemInfo.attr('gpos-minorunitname') || '';
        clGPOSUnitRelationship  = (clItemElement.POSItemInfo.attr('gpos-unitrelationship') || '').ForceConvertToInteger(0);        
    }

    var clUnitMode      = (clQuantityBox.attr('gpos-unitmode') || '');
    var clQuantity      = (clQuantityBox.attr('value') || '').ForceConvertToDecimal(0);    
    var clUnitPrice     = (clUnitPriceBox.attr('value') || '').ForceConvertToDecimal(0);
    var clDiscount      = (clDiscountBox.attr('value') || '').ForceConvertToDecimal(0);
    var clDiscountMode  = (clDiscountBox.attr('fa-discountmode') || 'value');    
    var clHasUnitPrice  = (clUnitPriceBox.attr('value') || '').length > 0 ? true : false;
    var clSerialNo      = (clSerialBox.attr('value') || '').ForceConvertToDecimal(0);
    var clSubTotal      = 0;
    var clTaxAmount     = 0;
    
    return {
                SetPOSItemInfo : function(paPOSItemInfo)
                {
                    clItemElement.POSItemInfo = paPOSItemInfo;                    

                    if (clItemElement.POSItemInfo) {
                        clGPOSItemID = clItemElement.POSItemInfo.attr('gpos-itemid') || '';
                        clGPOSEntryType = clItemElement.POSItemInfo.attr('gpos-entrytype') || '';
                        clGPOSEntryAttribute = clItemElement.POSItemInfo.attr('gpos-entryattribute') || '';
                        clGPOSItemText = clItemElement.POSItemInfo.attr('gpos-itemtext') || '';
                        clGPOSMajorPrice = (clItemElement.POSItemInfo.attr('gpos-majorprice') || '').ForceConvertToInteger(0);
                        clGPOSMinorPrice = (clItemElement.POSItemInfo.attr('gpos-minorprice') || '').ForceConvertToInteger(0);
                        clGPOSMajorUnitName = clItemElement.POSItemInfo.attr('gpos-majorunitname') || '';
                        clGPOSMinorUnitName = clItemElement.POSItemInfo.attr('gpos-minorunitname') || '';
                        clGPOSUnitRelationship = (clItemElement.POSItemInfo.attr('gpos-unitrelationship') || '').ForceConvertToInteger(0);
                        
                        clItemElement.attr('gpos-itemid', clGPOSItemID);

                        if ((clControlMode == 'purchase') && (clGPOSEntryType == 'service'))
                        {
                            if (clQuantity == 0) this.SetQuantity('1', true);
                        }
                        else if (clControlMode == 'sale')
                        {                            
                            if (clQuantity == 0) this.SetQuantity('1', true);
                          //  if ((clUnitPrice == 0) && (clGPOSMajorPrice > 0)) {                         
                                this.SetUnitPrice(clGPOSMajorPrice.toString());
                           // }
                        }
                        else if ((clControlMode == 'stockin') || (clControlMode == 'stockout'))
                        {
                            if (clQuantity == 0) this.SetQuantity('1', true);
                            this.SetUnitPrice('0');
                        }
                    }
                },        
                RefreshElement : function(){
                    clQuantity      = (clQuantityBox.attr('value') || '').ForceConvertToDecimal(0);
                    clUnitMode      = (clQuantityBox.attr('gpos-unitmode') || 'major');
                    clUnitPrice     = (clUnitPriceBox.attr('value') || '').ForceConvertToDecimal(0);
                    clDiscount      = (clDiscountBox.attr('value') || '').ForceConvertToDecimal(0);
                    clDiscountMode  = (clDiscountBox.attr('fa-discountmode') || 'value');
                    clHasUnitPrice  = (clUnitPriceBox.attr('value') || '').length > 0 ? true : false;
                    clSerialNo      = (clSerialBox.attr('value') || '').ForceConvertToDecimal(0);
                    clSubTotal      = ((clHasUnitPrice) && (clQuantity > 0)) ? this.CalculateSubTotal() : 0;                    
                    
                    if ((clControlMode == 'sale') && (clTaxApplicable)) {
                        if (clTaxInclusive)
                            clTaxAmount = clSubTotal - (Math.round((clSubTotal / (1 + (clTaxPercent / 100))) * 100) / 100);
                        else
                            clTaxAmount = Math.round((clSubTotal * (clTaxPercent / 100)) * 100) / 100;
                    }
                    else clTaxAmount = 0;
                },
                RefreshDisplayInfo : function()
                {
                    this.RefreshElement();                     
                    var lcDescriptionText =  clGPOSItemText + " " + this.GetPOSUnitText();                    
                    var lcQuantity = ((clUnitMode == 'minor') || (clGPOSUnitRelationship <= 1)) ? clQuantity :  clQuantity / clGPOSUnitRelationship;
                    
                    var lcQuantityText = FormManager.ConvertToFormLanguage(lcQuantity);
                    var lcUnitPriceText = FormManager.ConvertToFormLanguage(clUnitPrice);
                    var lcSubTotalText = FormManager.ConvertToFormLanguage(clSubTotal);
                    var lcCalculatedDiscount = this.CalculateDiscount();
                    var lcDiscountText = '(-' + FormManager.ConvertToFormLanguage(lcCalculatedDiscount) + ')';
                    clDescriptionBox.text(lcDescriptionText);                    
                    
                    clQuantityBox.text(lcQuantity > 0 ? lcQuantityText : '');                    
                    clQuantityBox.attr('fa-fontsizemode', clQuantityBox.text().length > 5 ? 'small' : 'standard');

                    clUnitPriceBox.text(clHasUnitPrice ? lcUnitPriceText : '');
                    clUnitPriceBox.attr('fa-fontsizemode', clUnitPriceBox.text().length > 8 ? 'small' : 'standard');

                    clSubtotalBox.text(clHasUnitPrice ? lcSubTotalText : '');
                    if (clSubtotalBox.text().length > 12) {
                        clSubtotalBox.attr('fa-overflow', true);
                        clSubtotalBox.text('');
                        POSTransactionListManager.ShowMessageBar(200);
                    }
                    else {
                        clSubtotalBox.removeAttr('fa-overflow');
                        clSubtotalBox.attr('fa-fontsizemode', clSubtotalBox.text().length > 8 ? 'small' : 'standard');
                    }

                    clDiscountBox.text(lcCalculatedDiscount > 0 ? lcDiscountText : '');
                                        
                    if (clDiscountBox.text().length > 12) {
                        clDiscountBox.attr('fa-overflow', true);
                        clDiscountBox.text('');
                        POSTransactionListManager.ShowMessageBar(200);
                    }
                    else {
                        clDiscountBox.removeAttr('fa-overflow');
                        clDiscountBox.attr('fa-fontsizemode', clDiscountBox.text().length > 8 ? 'small' : 'standard');
                    }
                },
                AccomodateItem : function()
                {
                    var lcItemID = clItemElement.attr('gpos-itemid');

                    if (lcItemID) {
                        var lcPOSItemRecord = POSItemPanelManager.GetPOSItem(lcItemID);
                        
                        if (lcPOSItemRecord)
                        {
                            this.SetPOSItemInfo(lcPOSItemRecord);
                            this.SetSerialNo(clSerialNo);                            
                            if ((clUnitMode == 'minor') || (clGPOSUnitRelationship <= 1)) this.SetQuantity(clQuantity.toString(), (clUnitMode == 'minor' ? false : true));
                            else this.SetQuantity((clQuantity / clGPOSUnitRelationship).toString(), true);                            
                            this.RefreshDisplayInfo();
                        }
                        
                    }
                },
                CalculateDiscount : function()
                {                    
                    var lcFactor = ((clUnitMode == 'minor') || (clGPOSUnitRelationship <= 1)) ? 1 : clGPOSUnitRelationship;
                    var lcSubtotal = Math.round(clUnitPrice / lcFactor * clQuantity * 100) / 100;
                    
                    var lcDiscount = (clDiscount > 0) ? clDiscount : 0;
                    lcDiscount = ((clDiscountMode == 'percent') && (lcDiscount > 100)) ? 100 : lcDiscount;
                    lcDiscount = clDiscountMode == 'percent' ? Math.round(lcSubtotal * lcDiscount) / 100 : lcDiscount;
                                        
                    return (lcDiscount);
                },
                CalculateSubTotal : function ()
                {                 
                    var lcFactor = ((clUnitMode == 'minor') || (clGPOSUnitRelationship <= 1)) ? 1 : clGPOSUnitRelationship;
                    var lcSubtotal = Math.round(clUnitPrice / lcFactor * clQuantity * 100) / 100;
                    
                    var lcDiscount = (clDiscount > 0) ? clDiscount : 0;                    
                    lcDiscount = ((clDiscountMode == 'percent') && (lcDiscount > 100)) ? 100 : lcDiscount;                    
                    lcDiscount = clDiscountMode == 'percent' ? lcSubtotal * lcDiscount / 100 : lcDiscount;
                                        
                    lcSubtotal = lcSubtotal > lcDiscount ? lcSubtotal - lcDiscount : 0;
                    lcSubtotal = Math.round(lcSubtotal * 100) / 100;
                    
                    return (lcSubtotal);
                },
                GetPOSUnitText : function()
                {   
                    if (clQuantity > 0) 
                    {                        
                        if ((clUnitMode == 'minor') || (clGPOSUnitRelationship <= 1))
                        {                     
                            return(FormManager.ConvertToFormLanguage(clQuantity) + " " + (clUnitMode == 'minor' ? clGPOSMinorUnitName : clGPOSMajorUnitName));
                        }
                        else if (clUnitMode == 'major')
                        {
                            var lcMajorQuantity = clQuantity / clGPOSUnitRelationship;
                            return (FormManager.ConvertToFormLanguage(lcMajorQuantity) + " " + clGPOSMajorUnitName);
                        }
                    }
                    return('');
                },

                SetQuantity : function(paQuantity, paMajorUnit)
                {                    
                    paQuantity = (paQuantity || '').ForceConvertToDecimal(0);                    
                    if (paQuantity > 0) {
                                                
                        clQuantityBox.text(paQuantity.toString());

                        if (paMajorUnit) {                            
                            clQuantityBox.attr('gpos-unitmode', 'major');

                            if (clGPOSUnitRelationship <= 1) clQuantityBox.attr('value', paQuantity);
                            else clQuantityBox.attr('value', paQuantity * clGPOSUnitRelationship);

                            if ((clControlMode == 'sale') &&  (clUnitMode != 'major')) this.SetUnitPrice(clGPOSMajorPrice.toString());
                        }
                        else {                                                        
                            clQuantityBox.attr('gpos-unitmode', 'minor');
                            clQuantityBox.attr('value', paQuantity);
                            
                            if ((clControlMode == 'sale') && (clUnitMode != 'minor')) this.SetUnitPrice(clGPOSMinorPrice.toString());                            
                        }
                    }
                    else 
                    {                        
                        clQuantityBox.attr('value', 0);
                        clQuantityBox.attr('gpos-unitmode', '');
                    }
                },
                SetUnitPrice : function(paPrice)
                {
                    paPrice = (paPrice || '').ForceConvertToDecimal(0);

                    if (paPrice > 0) {
                        clUnitPriceBox.attr('value', paPrice);
                        if (clDiscountMode == 'auto') this.SetDiscountValue(0);
                    }
                    else {                        
                        clUnitPriceBox.attr('value', 0);
                    }
                },
                SetDiscountPercent : function (paPercent) {
                    paPercent = (paPercent || '').ForceConvertToDecimal(0);

                    if (paPercent > 0) {
                        paPercent = paPercent < 100 ? paPercent : 100;
                        clDiscountBox.attr('fa-discountmode', 'percent');
                        clItemElement.attr('fa-discountmode', 'percent');
                        clDiscountBox.attr('value', paPercent);
                        clDiscountBox.attr('fa-displaytext', FormManager.ConvertToFormLanguage(paPercent) + "%");
                    }
                    else {
                        clDiscountBox.removeAttr('fa-discountmode');
                        clItemElement.removeAttr('fa-discountmode');
                        clDiscountBox.removeAttr('value');
                        clDiscountBox.removeAttr('fa-displaytext');
                    }
                },
                SetDiscountValue: function (paValue, paDiscountMode) {
                    paValue = (paValue || '').ForceConvertToDecimal(0);
                    paDiscountMode = (paDiscountMode || 'value');

                    if (paValue > 0) {
                        clDiscountBox.attr('fa-discountmode', paDiscountMode);
                        clItemElement.attr('fa-discountmode', paDiscountMode);
                        clDiscountBox.attr('value', paValue);                        
                    }
                    else {
                        clDiscountBox.removeAttr('fa-discountmode');
                        clItemElement.removeAttr('fa-discountmode');
                        clDiscountBox.removeAttr('value');
                    }
                    clDiscountBox.removeAttr('fa-displaytext');
                },
                SetSubTotalAmount : function (paSubTotal) {
                    this.RefreshElement();

                    paSubTotal = (paSubTotal || '').ForceConvertToDecimal(0);
                    
                    if ((paSubTotal > 0) && (clQuantity > 0))
                    {                     
                        var lcPrice = 0;
                        var lcActualSubtotal = 0;
                        var lcFactor = ((clUnitMode == 'minor') || (clGPOSUnitRelationship <= 1)) ? 1 : clGPOSUnitRelationship;

                        if (clUnitMode == 'major') lcPrice = clGPOSMajorPrice;
                        else if (clUnitMode == 'minor') lcPrice = clGPOSMinorPrice;

                        lcPrice = (lcPrice > 0) && (lcPrice * (clQuantity / lcFactor) >= paSubTotal) ? lcPrice : (Math.ceil((paSubTotal / (clQuantity / lcFactor)) * 100) / 100);
                        

                       // lcPrice = (lcPrice > 0) && (lcPrice * clQuantity >= paSubTotal) ? lcPrice : (Math.round(paSubTotal / clQuantity * 100) / 100);
                        
                        lcActualSubtotal = Math.round((lcPrice * (clQuantity / lcFactor)) * 100) / 100;
                        
                        this.SetUnitPrice(lcPrice.toString());
                        if (lcActualSubtotal > paSubTotal) {
                            var lcDiscount = Math.round((lcActualSubtotal - paSubTotal) * 100) / 100;
                            this.SetDiscountValue(lcDiscount.toString(), 'auto');
                        }
                    }                   
                },
                SetSerialNo : function(paSerialNo)
                {
                    if (paSerialNo)
                    {
                        var lcSeparator = clComposite.attr('ea-separator') || '.';
                        clSerialBox.attr('value', paSerialNo);
                        clItemElement.attr('fa-serial', paSerialNo);
                        clSerialBox.text(FormManager.ConvertToFormLanguage(paSerialNo) + lcSeparator);                        
                    }
                },
                VerifyEntry : function()
                {
                    if (clItemElement.POSItemInfo)
                    {
                        if ((clHasUnitPrice) && (clQuantity > 0)) {
                            if ((clSubtotalBox.attr('fa-overflow') != 'true') &&  (clDiscountBox.attr('fa-overflow') != 'true')) return (0);
                            else return (202);
                        }                        
                    }
                    return (201);
                },
                IsEmptyElement : function()
                {
                    if ((!clItemElement.POSItemInfo) && (!clHasUnitPrice) && (clQuantity == 0) && (clDiscount == 0))
                        return (true);
                    else return (false);                    
                },
                IsNewEmptyElement : function () {
                    if ((this.IsEmptyElement()) && (clItemElement.attr('fa-unconfirm') == 'true'))
                        return (true);
                    else return (false);
                },
                GetSubTotal : function()
                {
                    this.RefreshElement();

                    if (clSubtotalBox.attr('fa-overflow') == 'true') return (-1);
                    else if (clItemElement.attr('fa-unconfirm') == 'true') return (0);                        
                    else return (clSubTotal);
                },
                GetDataArray : function() 
                {
                    var lcData = {}

                    if (clItemElement.POSItemInfo) {
                        this.RefreshElement();

                        lcData['serial']            = clSerialNo;
                        lcData['itemid']            = clGPOSItemID;
                        lcData['entrytype']         = clGPOSEntryType;
                        lcData['entryattribute']    = clGPOSEntryAttribute;
                        lcData['quantity']          = clQuantity;
                        lcData['unitmode']          = clUnitMode;
                        lcData['unitrelationship']  = clGPOSUnitRelationship;
                        lcData['unitprice']         = clUnitPrice;                        
                        lcData['taxamount']         = clTaxAmount;
                        lcData['discount']          = this.CalculateDiscount();                        
                        
                        return (lcData);
                    }
                    else return (null);
                }
            }
};
