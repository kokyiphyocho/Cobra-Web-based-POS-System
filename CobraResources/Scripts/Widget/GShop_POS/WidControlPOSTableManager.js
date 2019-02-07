$(document).ready(function () {
    POSTableManager.Init();
});

var POSTableManager = (function () {
    var clForm;    
    var clControl;
    var clContainer;
    var clControlBar;


    return {
        Init: function () {
            clForm = FormManager.GetForm();            
            clControl = clForm.find('[sa-elementtype=control].WidControlPOSTableManager');
            clContainer = clControl.find('[sa-elementtype=container]');
            clControlBar = clControl.find('[sa-elementtype=controlbar]');

            POSTableManager.LoadContent().done(function () {
                POSTableManager.RestoreBlockState();

                POSTableManager.WaitForDependencies().done(function () {
                    clContainer.find('[sa-elementtype=block][ea-group].TableGroupBlock').swipe('destroy');
                    clContainer.find('[sa-elementtype=block][ea-group].TableGroupBlock').swipe({ swipe: POSTableManager.HandlerOnSwipe, allowPageScroll: "vertical" })
                });
            });

            
        },
        WaitForDependencies: function () {
            var lcDeferred = $.Deferred();

            var lcWaitTimer = setInterval(function () {
                
                if (typeof $.fn.swipe !== 'undefined') {                    
                    if (lcDeferred.state() == 'pending') {
                        lcDeferred.resolve();
                        clearInterval(lcWaitTimer);
                    }
                }
            }, 200);

            return (lcDeferred);
        }, 
        BindElementEvents: function () {            
            clControl.find('[ea-command]').unbind('click');
            clControl.find('[ea-command]').click(POSTableManager.HandlerOnClick);
        },
        LoadContent: function () {
            var lcDeferred = $.Deferred();

            var lcAjaxRequestManager = new AjaxRequestManager('getupdatedcontrol', null, null, 'ajax_loading');

            lcAjaxRequestManager.AddAjaxParam('Parameter', 'controlcontent');

            lcAjaxRequestManager.SetCompleteHandler(function (paSuccess, paResponseStruct) {
                if (paSuccess) {
                    var lcActiveGroup = POSTableManager.GetActiveGroup();
                    
                    clControl.empty();
                    clControl.html(paResponseStruct.ResponseData.RSP_HTML);

                    clContainer = clControl.find('[sa-elementtype=container]');
                    clControlBar = clControl.find('[sa-elementtype=controlbar]');

                    POSTableManager.SetActiveGroup(lcActiveGroup);

                    POSTableManager.BindElementEvents();
                    lcDeferred.resolve(true);
                }
                else {
                    lcDeferred.resolve(false)
                }
            });

            lcAjaxRequestManager.Execute();

            return (lcDeferred);
        },        
        SaveBlockState: function () {
            var lcElementBlock = clControl.find('[sa-elementtype=block]');
            var lcBlockStateList = [];

            lcElementBlock.each(function () {
                var lcBlockState = {}
                var lcItemList = $(this).find('[sa-elementtype=list]');

                lcBlockState['ea-group'] = $(this).attr('ea-group');                
                lcBlockState['scrolltop'] = lcItemList.scrollTop();
                lcBlockState['fa-position'] = $(this).attr('fa-position');

                lcBlockStateList.push(lcBlockState);
            });

            return (Base64.encode(JSON.stringify(lcBlockStateList)));
        },
        RestoreBlockState: function () {
            var lcElementBlockList = clControl.find('[sa-elementtype=block]');
            var lcSavedState = GetUrlParameter('_formsavedstate');

            if (lcSavedState != '') {

                lcSavedState = Base64.decode(lcSavedState);

                var lcBlockStateInfo = JSON.parse(lcSavedState);

                $.each(lcBlockStateInfo, function (paIndex, paBlockData) {
                    var lcElementBlock = clControl.find('[sa-elementtype=block][ea-group="' + paBlockData['ea-group'] + '"]');                    
                    var lcScrollTop = paBlockData['scrolltop'];
                    var lcPosition = paBlockData['fa-position'];
                    var lcItemList = lcElementBlock.find('[sa-elementtype=list]');

                    if (lcElementBlock) {
                        lcElementBlock.attr('fa-position', lcPosition);                        
                        if (lcScrollTop) lcItemList.scrollTop(lcScrollTop);
                    }
                });
                clControl.attr('fa-noanimation', 'true');
                POSTableManager.SetActiveGroup(POSTableManager.GetActiveGroup());
                setTimeout(function () { clControl.removeAttr('fa-noanimation') }, 100);
            }
        },                
        OpenReceipt: function (paElement, paTransactionState) {
            if (paElement) {
                
                var lcReceiptID         = paElement.attr('gpos-receiptid') || -1;
                var lcReference         = paElement.attr('gpos-reference');
                var lcDisplayName       = paElement.attr('gpos-displayname');
                var paTransactionState  = paTransactionState || 'pending';
               
                var lcLink = clControl.attr('ea-template');
                lcLink = lcLink.replace('$RECEIPTID', lcReceiptID);
                lcLink = lcLink.replace('$REFERENCE', lcReference.toUpperCase());
                lcLink = lcLink.replace('$TRANSACTIONSTATE', paTransactionState);
                lcLink = lcLink.replace('$FORMTITLE', lcDisplayName);

                var lcBlockStateInfo = POSTableManager.SaveBlockState();

                FormManager.RedirectStatefulTextLink(lcLink, lcBlockStateInfo);
           }           
        },
        DeleteRecept: function (paElement) {

            if (paElement) {
                var lcReceiptID    = paElement.attr('gpos-receiptid');
                var lcDisplayName  = paElement.attr('gpos-displayname');
                var lcLastModified = paElement.attr('ea-lastmodified');

                var lcAjaxRequestManager = new AjaxRequestManager('executenonquery', 'info_successdeleteentry', 'err_faildeleteentry', 'ajax_deleting');

                lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.deletependingreceipt');
                lcAjaxRequestManager.AddObjectDataBlock('datablock', { ReceiptID : lcReceiptID, LastModified : lcLastModified }, true);
                lcAjaxRequestManager.AddMessagePlaceHolder('$TABLENAME', lcDisplayName);

                lcAjaxRequestManager.SetCompleteHandler(function (paSuccess) {
                    if (paSuccess) {
                        POSTableManager.LoadContent();
                    }
                });

                lcAjaxRequestManager.ExecuteOnConfirm('confirm_deletetablereceipt');
            }                       
        },
        SwapTable: function (paSourceElement, paTargetElement) {

            if ((paSourceElement) && (paTargetElement)) {
                var lcSourceReceiptID    = paSourceElement.attr('gpos-receiptid');
                var lcSourceReference    = paSourceElement.attr('gpos-reference');
                var lcSourceLastModified = paSourceElement.attr('ea-lastmodified');
                var lcSourceTableName    = paSourceElement.attr('gpos-displayname');
                var lcTargetReceiptID    = paTargetElement.attr('gpos-receiptid')  || -1;
                var lcTargetReference    = paTargetElement.attr('gpos-reference')  || '';
                var lcTargetLastModified = paTargetElement.attr('ea-lastmodified') || moment().format('YYYY-MM-DD');
                var lcTargetTableName    = paTargetElement.attr('gpos-displayname');
                
                var lcAjaxRequestManager = new AjaxRequestManager('executenonquery', null, 'err_failrequest', 'ajax_updating');

                lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.swaptable');
                lcAjaxRequestManager.AddObjectDataBlock('datablock', {
                    SourceReceiptID    : lcSourceReceiptID,
                    SourceReference    : lcSourceReference,
                    SourceLastModified : lcSourceLastModified,
                    TargetReceiptID    : lcTargetReceiptID,
                    TargetReference    : lcTargetReference,
                    TargetLastModified : lcTargetLastModified
                }, true);

                lcAjaxRequestManager.AddMessagePlaceHolder('$SOURCETABLE', lcSourceTableName);
                lcAjaxRequestManager.AddMessagePlaceHolder('$TARGETTABLE', lcTargetTableName);                

                lcAjaxRequestManager.SetCompleteHandler(function (paSuccess) {
                    if (paSuccess) {
                        
                        POSTableManager.LoadContent();
                    }                    
                });

                lcAjaxRequestManager.SetConfirmationResultHandler(function (paResult) {
                        var lcElements = clContainer.find('[sa-elementtype=element].TableElement');

                        clControl.removeAttr('fa-movemode');
                        lcElements.removeAttr('fa-movesource');
                    }
                );

                lcAjaxRequestManager.ExecuteOnConfirm('confirm_movetable');
            }
        },
        SwipeGroup : function(paDirection)
        {
            var lcActiveBlock = clContainer.find('[sa-elementtype=block][fa-position=middle]');
            
            if ((lcActiveBlock.length > 0) && ((paDirection == 'left') || (paDirection == 'right')))
            {
                
                var lcTagetBlock = paDirection == 'left' ? lcActiveBlock.next() : lcActiveBlock.prev();

                if (lcTagetBlock.length > 0) POSTableManager.SetActiveGroup(lcTagetBlock.attr('ea-group'));                
            }
        },
        GetActiveGroup : function()
        {
            var lcActiveBlock = clContainer.find('[sa-elementtype=block][fa-position=middle]');
            
            if (lcActiveBlock.length == 0) lcActiveBlock = clContainer.children().first();
            
            return (lcActiveBlock.attr('ea-group'));
        },
        SetActiveGroup : function(paGroupID)
        {            
            paGroupID = paGroupID || POSTableManager.GetActiveGroup();
            
            if (paGroupID) {
                var lcTargetBlock   = clContainer.find('[sa-elementtype=block][ea-group="' + paGroupID + '"]');
                var lcGroupButtons  = clControlBar.find('[sa-elementtype=button]');
                var lcActiveButton  = clControlBar.find('[sa-elementtype=button][ea-group="' + paGroupID + '"]');
                
                if ((lcTargetBlock.length > 0)) {
                    lcTargetBlock.prevAll().attr('fa-position', 'left');
                    lcTargetBlock.nextAll().attr('fa-position', 'right');
                    lcTargetBlock.attr('fa-position','middle')
                    //clContainer.find('[sa-elementtype=block]').removeAttr('ea-appearance');
                    //lcTargetBlock.attr('ea-appearance', 'active');
                    
                    lcGroupButtons.removeAttr('fa-active');
                    if (lcActiveButton) lcActiveButton.attr('fa-active', 'true');

                    return (true);
                }
            }
            return (false);
        },        
        ToggleActiveElement : function(paElement)
        {
            if (paElement)
            {
                var lcActive   = paElement.attr('fa-active') == 'true';
                var lcHasBill  = paElement.attr('gpos-receiptid');

                var lcElements = clContainer.find('[sa-elementtype=element].TableElement');

                lcElements.removeAttr('fa-active');

                if ((!lcActive) && (lcHasBill))
                    paElement.attr('fa-active', 'true');
                
            }
        },
        HandlerOnSwipe: function (paEvent, paDirection, paDistance, paDuration, paFingerCount) {
            POSTableManager.SwipeGroup(paDirection);
        },        
        HandlerOnClick: function (paEvent) {
            paEvent.preventDefault();

            var lcCommand = $(this).attr('ea-command');
            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);
            
            switch (lcCommand) {
                case 'changeblock':
                    {
                        paEvent.stopPropagation();

                        var lcGroupID = $(this).attr('ea-group');
                        POSTableManager.SetActiveGroup(lcGroupID);
                        break;
                    }

                case 'addorder':
                    {                        
                        paEvent.stopPropagation();
                        var lcElement = $(this).closest('[sa-elementtype=element]');                        
                        POSTableManager.OpenReceipt(lcElement,'pending');
                        break;
                    }

                case 'settlebill':
                    {
                        paEvent.stopPropagation();
                        var lcElement = $(this).closest('[sa-elementtype=element]');
                        POSTableManager.OpenReceipt(lcElement, 'settlement');
                        break;
                    }

                case 'deletebill':
                    {
                        paEvent.stopPropagation();
                        var lcElement = $(this).closest('[sa-elementtype=element]');
                        POSTableManager.DeleteRecept(lcElement);
                        break;
                    }

                case 'toggleactive':
                    {
                        var lcElement       = $(this).closest('[sa-elementtype=element]');
                        var lcMoveMode      = clControl.attr('fa-movemode');
                        var lcMoveSource    = lcElement.attr('fa-movesource');
                        
                        if (lcMoveMode == 'true')
                        {
                            if (lcMoveSource != 'true') 
                            {
                                var lcSourceElement = clContainer.find('[sa-elementtype=element][fa-movesource=true]');
                                var lcTargetElement = lcElement;

                                POSTableManager.SwapTable(lcSourceElement, lcTargetElement);
                            }
                            else
                            {
                                clControl.removeAttr('fa-movemode');
                                lcElement.removeAttr('fa-movesource');
                            }
                        }
                        else
                        {
                            paEvent.stopPropagation();
                            POSTableManager.ToggleActiveElement(lcElement);
                        }
                        break;
                    }

                case 'refresh':
                    {                        
                        paEvent.stopPropagation();                        
                        POSTableManager.LoadContent();
                        break;
                    }

                case 'move':
                    {
                        paEvent.stopPropagation();
                        var lcElement = $(this).closest('[sa-elementtype=element]');
                        var lcElements = clContainer.find('[sa-elementtype=element].TableElement');

                        clControl.attr('fa-movemode', 'true');

                        lcElements.removeAttr('fa-movesource');
                        lcElement.attr('fa-movesource', 'true');

                        POSTableManager.ToggleActiveElement(lcElement);
                        break;
                    }                
            }
        },
    }
})();
