$(document).ready(function () {        
    POSItemListManager.Init();
});

var POSItemListManager = (function () {
    var clForm;
    var clToolBar;
    var clControl;
    var clContainer;

    return {
        Init: function () {
            clForm       = FormManager.GetForm();
            clToolBar    = ToolBarManager.GetToolBar();
            clControl    = clForm.find('[sa-elementtype=control].WidControlPOSItemList');
            clContainer  = clControl.find('[sa-elementtype=container]');

            POSItemListManager.BindEvents();

            POSItemListManager.LoadContent().done(function ()
            {
                POSItemListManager.RestoreBlockState();
            });
        },
        BindEvents: function () {
            clToolBar.find('a[ea-command="@cmd%editmode"],a[ea-command="@cmd%deletemode"]').unbind('click');
            clToolBar.find('a[ea-command="@cmd%editmode"],a[ea-command="@cmd%deletemode"]').click(POSItemListManager.HandlerOnClick);

            clToolBar.find('a[ea-command^="@cmd%add"]').unbind('click');
            clToolBar.find('a[ea-command^="@cmd%add"]').click(POSItemListManager.HandlerOnClick);
        },
        BindElementEvents : function()
        {         
            clControl.find('div[sa-elementtype=row][ea-command="@cmd%showcategory"]').unbind('click');
            clControl.find('div[sa-elementtype=row][ea-command="@cmd%showcategory"]').click(POSItemListManager.HandlerOnClick);
                        
            clControl.find('div[sa-elementtype=title] div[ea-command="@cmd%upcategory"]').unbind('click');
            clControl.find('div[sa-elementtype=title] div[ea-command="@cmd%upcategory"]').click(POSItemListManager.HandlerOnClick);

            clControl.find('div[sa-elementtype=title] div[ea-command="@cmd%rootcategory"]').unbind('click');
            clControl.find('div[sa-elementtype=title] div[ea-command="@cmd%rootcategory"]').click(POSItemListManager.HandlerOnClick);

            clControl.find('img[ea-command="@cmd%delete"]').unbind('click');
            clControl.find('img[ea-command="@cmd%delete"]').click(POSItemListManager.HandlerOnClick);
            
            clControl.find('img[ea-command="@cmd%edit"]').unbind('click');
            clControl.find('img[ea-command="@cmd%edit"]').click(POSItemListManager.HandlerOnClick);
        },
        LoadContent : function()
        {            
            var lcDeferred = $.Deferred();
            
            var lcAjaxRequestManager = new AjaxRequestManager('getupdatedcontrol', null, null, 'ajax_loading');

            lcAjaxRequestManager.AddAjaxParam('Parameter', 'itemlistcontent');

            lcAjaxRequestManager.SetCompleteHandler(function (paSuccess, paResponseStruct) {                
                if (paSuccess) {
                    clContainer.empty();
                    clContainer.html(paResponseStruct.ResponseData.RSP_HTML);

                    POSItemListManager.BindElementEvents();
                    lcDeferred.resolve(true);
                }
                else {
                    lcDeferred.resolve(false)
                }
            });

            lcAjaxRequestManager.Execute();

            return (lcDeferred);
        },
        SetControlMode : function(paMode)
        {
            if (paMode) {
                var lcToggleState   = clControl.attr('fa-mode');
                var lcTargetElement = clToolBar.find('a[ea-command="@cmd%' + paMode + '"]');

                clToolBar.find('[fa-active]').removeAttr('fa-active');
                
                if (lcToggleState != paMode)
                {
                    lcTargetElement.attr('fa-active', 'true');
                    clControl.attr('fa-mode', paMode);
                }
                else
                {
                    clControl.removeAttr('fa-mode');
                }
            }
        },
        OpenElementForm : function(paMode, paItemID)
        {
            var lcIndex = -1;

            paItemID = paItemID || '-1'

            switch (paMode) {
                case "item": lcIndex = 0; break;
                case "category": lcIndex = 1; break;
                case "service": lcIndex = 2; break;
                case "staticitem": lcIndex = 3; break;
            }            
            
            if (lcIndex >= 0) {                
                var lcLinkTemplate = clControl.attr('ea-template').split("||")[lcIndex];
                
                var lcActiveBlock = clControl.find('[sa-elementtype=block][fa-position=middle]');
                if (lcActiveBlock.length == 0) lcActiveBlock = clControl.find('[sa-elementtype=block][ea-group="0"]');

                var lcCategory = lcActiveBlock.attr('ea-group');
                var lcLink = lcLinkTemplate.replace('$CATEGORY', lcCategory).replace('$ITEMID', paItemID);
                var lcBlockStateInfo = POSItemListManager.SaveBlockState(clControl);
                
                FormManager.RedirectStatefulTextLink(lcLink, lcBlockStateInfo);
            }
        },   
        SaveBlockState : function()
        {            
            var lcElementBlock = clControl.find('[sa-elementtype=block]');
            var lcBlockStateList = [];

            lcElementBlock.each(function () {
                var lcBlockState = {}
                var lcItemList = $(this).find('[sa-elementtype=list]');

                lcBlockState['ea-group'] = $(this).attr('ea-group');
                lcBlockState['fa-position'] = $(this).attr('fa-position');
                lcBlockState['scrolltop'] = lcItemList.scrollTop();

                lcBlockStateList.push(lcBlockState);
            });

            return(Base64.encode(JSON.stringify(lcBlockStateList)));    
        },
        RestoreBlockState : function () {            
            var lcElementBlockList = clControl.find('[sa-elementtype=block]');
            var lcSavedState = GetUrlParameter('_formsavedstate');

            if (lcSavedState != '') {

                lcSavedState = Base64.decode(lcSavedState);
                
                var lcBlockStateInfo = JSON.parse(lcSavedState);

                $.each(lcBlockStateInfo, function (paIndex, paBlockData) {
                    var lcElementBlock = clControl.find('[sa-elementtype=block][ea-group="' + paBlockData['ea-group'] + '"]');
                    var lcPosition = paBlockData['fa-position'];
                    var lcScrollTop = paBlockData['scrolltop'];
                    var lcItemList = lcElementBlock.find('[sa-elementtype=list]');

                    if (lcElementBlock) {
                        if (lcPosition) {
                            lcElementBlock.addClass('NoTransition');
                            lcElementBlock.attr('fa-position', lcPosition);
                            setTimeout(function () { lcElementBlock.removeClass('NoTransition'); }, 500);
                        }
                        if (lcScrollTop) lcItemList.scrollTop(lcScrollTop);
                    }
                });
            }
        },
        ShowCategoryBlock : function(paDataID)
        {
            if (paDataID) {
                var lcActiveBlock = clControl.find('[sa-elementtype=block][fa-position=middle]');
                if (lcActiveBlock.length == 0) lcActiveBlock = clControl.find('[sa-elementtype=block][ea-group="0"]');

                var lcCategoryBlock = clControl.find('[sa-elementtype=block][ea-group="' + paDataID + '"]');
                lcActiveBlock.attr('fa-position', 'left');
                lcCategoryBlock.attr('fa-position', 'middle');

                clControl.removeAttr('fa-mode');
            }
        },
        UpCategoryBlock : function()
        {                
            var lcActiveBlock       = clControl.find('[sa-elementtype=block][fa-position=middle]');
            var lcParent            = lcActiveBlock.attr('ea-parent');

            if (lcParent)
            {
                var lcParentBlock = clControl.find('[sa-elementtype=block][ea-group="' + lcParent + '"]');
                if (lcParentBlock) 
                {
                    lcParentBlock.attr('fa-position', 'middle');
                    lcActiveBlock.removeAttr('fa-position');
                    clControl.removeAttr('fa-mode');
                }
            }
        },
        RootCategoryBlock : function()
        {
            var lcActiveBlock = clControl.find('[sa-elementtype=block][fa-position=middle]');                
            var lcLeftBlocks = clControl.find('[sa-elementtype=block][fa-position=left]').not('[ea-group="0"]');
            var lcRootBlock = clControl.find('[sa-elementtype=block][ea-group="0"]');

            lcLeftBlocks.hide();
            lcLeftBlocks.removeAttr('fa-position');
            lcRootBlock.removeAttr('fa-position');

            lcActiveBlock.removeAttr('fa-position');

            setTimeout(function () { lcLeftBlocks.show() }, 500);

            clControl.removeAttr('fa-mode');
        },
        DeleteRecord : function(paItemRow)
        {       
            var lcDataID    = paItemRow.attr('ea-dataid');
            var lcItemName = paItemRow.text();

            var lcAjaxRequestManager = new AjaxRequestManager('executenonquery', 'info_successdeleteobject', 'err_faildeleteobject', 'ajax_deleting');

            lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.deleteitemrecord');
            lcAjaxRequestManager.AddObjectDataBlock('datablock', { FPM_ITEMID : lcDataID }, true);
            lcAjaxRequestManager.AddMessagePlaceHolder('$OBJECT', lcItemName);

            lcAjaxRequestManager.SetResponseDictionaryParsingHandler(function (paMessagePlaceHolderList, paJSONString) {
                var lcDictionary = JSON.parse(paJSONString);

                if (lcDictionary.quantity) {                    
                        var lcUnitRelationship = (paItemRow.attr('gpos-unitrelationship') || '').ForceConvertToInteger();
                        var lcMajorUnitName = paItemRow.attr('gpos-majorunitname') || '';
                        var lcMinorUnitName = paItemRow.attr('gpos-minorunitname') || '';
                        var lcQuantity = (lcDictionary.quantity || '').ForceConvertToInteger();
                        var lcMajorQuantity = Math.floor(lcQuantity / (lcUnitRelationship > 1 ? lcUnitRelationship : 1));
                        var lcMinorQuantity = lcQuantity % (lcUnitRelationship > 1 ? lcUnitRelationship : 1);
                        var lcQuantityText;

                        if (lcUnitRelationship > 1) {
                            lcQuantityText = (lcMajorQuantity > 0 ? lcMajorQuantity + ' ' + lcMajorUnitName + ' ' : '') + (lcMinorQuantity > 0 ? lcMinorQuantity + ' ' + lcMinorUnitName + ' ' : '');
                        }
                        else lcQuantityText = (lcMajorQuantity > 0 ? lcMajorQuantity + ' ' + lcMajorUnitName + ' ' : '');

                        paMessagePlaceHolderList['$ITEMNAME'] = paItemRow.attr('gpos-itemtext');
                        paMessagePlaceHolderList['$QUANTITYTEXT'] = FormManager.ConvertToFormLanguage(lcQuantityText);
                }
            });

            lcAjaxRequestManager.SetCompleteHandler(function (paSuccess) {
                if (paSuccess) {
                    paItemRow.remove();
                }
            });

            lcAjaxRequestManager.ExecuteOnConfirm('confirm_deleteobject');
        },

        HandlerOnClick: function (paEvent)
        {            
            paEvent.preventDefault();
            
            var lcCommand = $(this).attr('ea-command');
            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);

            switch(lcCommand)
            {
                case 'editmode':
                case 'deletemode':
                    {
                        POSItemListManager.SetControlMode(lcCommand);
                        break;
                    }

                case 'additem':
                case 'addstaticitem':
                case 'addcategory':
                case 'addservice':
                    {
                        var lcMode = lcCommand.substring(3);
                        
                        if (lcMode) POSItemListManager.OpenElementForm(lcMode);
                        break;
                    }

                case 'showcategory':
                    {
                        var lcDataID = $(this).attr('ea-dataid');
                        var lcType = $(this).attr('ea-type');

                        if (lcType == 'category') POSItemListManager.ShowCategoryBlock(lcDataID);

                        break;
                    }

                case 'upcategory':
                    {                        
                        POSItemListManager.UpCategoryBlock();
                        break;
                    }

                case 'rootcategory':
                    {                        
                        POSItemListManager.RootCategoryBlock();
                        break;
                    }

                case 'edit':
                    {
                        paEvent.stopPropagation();                        
                        
                        var lcItemRow   = $(this).closest('[sa-elementtype=row]');
                        var lcMode      = lcItemRow.attr('ea-type');
                        var lcItemID    = lcItemRow.attr('ea-dataid');

                        POSItemListManager.OpenElementForm(lcMode, lcItemID);

                        break;
                    }

                case 'delete':
                    {
                        paEvent.stopPropagation();

                        var lcItemRow = $(this).closest('[sa-elementtype=row]');

                        POSItemListManager.DeleteRecord(lcItemRow);
                        break;
                    }
            }            
        },       
    }
})();
    