$(document).ready(function () {       
 //    POSItemPanelManager.Init($('[sa-elementtype=composite].SubControlPOSItemPanelComposite'));   
});


var POSItemPanelManager = (function () {
    var clForm;
    var clPOSItemComposite;
    var clPOSItemPanel;
    var clBlockContainer;
    var clItemBlock;
    var clServiceBlock;
    var clHeaderBar;
    var clTitle;
    var clTitleText;
    var clStack = [];
    var clDeferred;

    return {
        Init: function (paPOSItemComposite) {            
            clForm = paPOSItemComposite.closest('[sa-elementtype=form]');
            clPOSItemComposite = paPOSItemComposite;            
            clPOSItemPanel      = paPOSItemComposite.find('[sa-elementtype=panel].SubControlPOSItemPanel');
            clHeaderBar         = clPOSItemPanel.find('.HeaderBar');
            clTitle             = clPOSItemPanel.find('[sa-elementtype=container] [sa-elementtype=title]');
            clTitleText         = clTitle.find('span.TitleText');
            clBlockContainer    = clPOSItemPanel.find('[sa-elementtype=container] .BlockContainer');       
            clItemBlock         = clBlockContainer.find('[sa-elementtype=block][ea-type=item]');
            clServiceBlock      = clBlockContainer.find('[sa-elementtype=block][ea-type=service]');
            
            POSItemPanelManager.SetFavouriteFlags();

            POSItemPanelManager.BindFunctionClickEvent();            

            POSItemPanelManager.SetActiveCategory(0);
        },

        SetFavouriteFlags : function()
        {
            var lcFavouriteItems    = clBlockContainer.find('[gpos-attribute=favourite]');
            lcFavouriteItems.attr('fa-favourite','true');
        },
        BindFunctionClickEvent : function()
        {
            var lcHeaderBarButtons = clHeaderBar.find('a[ea-command^="@cmd%"]');
            var lcTitleButtons = clTitle.find('a[ea-command^="@cmd%"]');
            
            lcHeaderBarButtons.unbind('click');
            lcHeaderBarButtons.click(POSItemPanelManager.HandlerOnFunctionClick);

            lcTitleButtons.unbind('click');
            lcTitleButtons.click(POSItemPanelManager.HandlerOnFunctionClick);
        },
        BindItemClickEvent : function(paItemList)
        {            
            clUnbindedItems = paItemList.not('[fa-eventbinded]');

            if (clUnbindedItems.length > 0) {
                clUnbindedItems.unbind('click');
                clUnbindedItems.click(POSItemPanelManager.HandlerOnItemClick);
                clUnbindedItems.attr('fa-eventbinded', 'true');
            }
        },

        SetActiveCategory: function (paCategory) {

            var lcActiveItemList    = clBlockContainer.find('a[gpos-category="' + paCategory + '"]');
            var lcShowedItems   = clBlockContainer.find('[fa-show=true]');
            
            paCategory = Number(paCategory);

            clPOSItemPanel.attr('ea-group', paCategory);
            lcShowedItems.removeAttr('fa-show');            
            lcActiveItemList.attr('fa-show', true);

            POSItemPanelManager.BindItemClickEvent(lcActiveItemList);

            clTitleText.text(POSItemPanelManager.GetCategoryName(paCategory));
        },
        SwitchToMode : function(paMode)
        {
            if (paMode == 'favourite') {
                if (clPOSItemPanel.attr('ea-mode') != 'favourite') {
                    var lcFavouriteItems = clBlockContainer.find('a[gpos-attribute=favourite]');
                    POSItemPanelManager.BindItemClickEvent(lcFavouriteItems);

                    clPOSItemPanel.attr('ea-mode', 'favourite');
                }
            }
            else {
                if (clPOSItemPanel.attr('ea-mode') != 'list') {
                    clPOSItemPanel.attr('ea-mode', 'list');                    
                }
            }
        },
        SetFavouriteEditingMode : function(paEditMode)
        {            
            if (paEditMode)
            {
                clPOSItemPanel.attr('fa-editing', 'true');
            }
            else 
            {
                clBlockContainer.find('a[fa-favouritechanged]').removeAttr('fa-favouritechanged');
                clBlockContainer.find('a[fa-favourite]').removeAttr('fa-favourite');
                POSItemPanelManager.SetFavouriteFlags();
                clPOSItemPanel.removeAttr('fa-editing');
            }
        },
        GetParentCategory : function(paCategory)
        {
            var lcItem = clBlockContainer.find('a[gpos-itemid="' + paCategory + '"]');

            if (lcItem.length > 0) {
                return (lcItem.attr('gpos-category'));
            }
            else return (0);            
        },
        GetCategoryName : function(paCategory)
        {
            paCategory = Number(paCategory);

            if (paCategory == 0) return (clPOSItemPanel.attr('ea-root') || '-');
            else return (clBlockContainer.find('a[gpos-itemid="' + paCategory + '"]').attr('gpos-itemtext'));            
        },
        PushCategory: function() {
            if (clPOSItemPanel.attr('ea-mode') == 'favourite') clStack.push('favourite');
            else clStack.push(clPOSItemPanel.attr('ea-group'));
        },
        PopCategory: function() {            
                var clPreviousCategory = clStack.pop();
                if (clPreviousCategory == 'favourite') POSItemPanelManager.SwitchToMode('favourite');
                else POSItemPanelManager.SetActiveCategory(clPreviousCategory);            
        },
        GetPOSItem : function(paItemID)
        {
            if (paItemID)
            {                
                var lcPOSItem = clBlockContainer.find('a[sa-elementtype=element][gpos-itemid="' + paItemID + '"]');

                if (lcPOSItem.length > 0) return (lcPOSItem);
                else return (null);
            }
            return (null);
        },
        HandlerOnItemClick : function()
        {
            var lcItem = $(this);
            var lcType = lcItem.attr('gpos-entrytype');            
            var lcItemID = lcItem.attr('gpos-itemid');
            var lcPanel = $(this).closest('[sa-elementtype=panel]');
            var lcEditingMode = lcPanel.attr('fa-editing');
            var lcFavourite = lcItem.attr('fa-favourite');
            
            if (lcEditingMode == 'true') {
                if (lcFavourite != 'true') {
                    lcItem.attr('fa-favourite', 'true');
                    if (lcItem.attr('gpos-attribute') != 'favourite') lcItem.attr('fa-favouritechanged', 'true');
                    else lcItem.removeAttr('fa-favouritechanged');
                }
                else {                    
                    lcItem.attr('fa-favourite', 'false');
                    if (lcItem.attr('gpos-attribute') == 'favourite') lcItem.attr('fa-favouritechanged', 'true');
                    else lcItem.removeAttr('fa-favouritechanged');
                }                
            }
            else {
                if (lcType == 'category') {
                    POSItemPanelManager.PushCategory();
                    POSItemPanelManager.SetActiveCategory(lcItemID);
                    POSItemPanelManager.SwitchToMode('list');
                }
                else
                {
                    POSItemPanelManager.ClosePOSItemPanel($(this));
                }
            }
        },
        ApplyFavouriteChanges : function()
        {
            var lcAddItemIDArray = [];
            var lcRemoveItemIDArray = []; 
            
            lcAddItemIDArray.push(0);// Prevent Empty Error.
            lcRemoveItemIDArray.push(0); // Prevent Empty Error.

            var lcAddItemList = clBlockContainer.find('a[fa-favouritechanged=true][fa-favourite=true]');
            var lcRemoveItemList = clBlockContainer.find('a[fa-favouritechanged=true][fa-favourite=false]');

            lcAddItemList.each(function () {
                var lcItemID = Number($(this).attr('gpos-itemid'));
                lcAddItemIDArray.push(lcItemID);                
            });

            lcRemoveItemList.each(function () {
                var lcItemID = Number($(this).attr('gpos-itemid'));
                lcRemoveItemIDArray.push(lcItemID);
            });

            if ((lcAddItemList.length > 0) || (lcRemoveItemList.length > 0)) {
                var lcDataBlock = { FPMNQ_AddItems: lcAddItemIDArray.join(','), FPMNQ_RemoveItems: lcRemoveItemIDArray.join(',') };
                var lcAjaxRequestManager = new AjaxRequestManager('executescalarquery', null, 'err_failupdate', 'ajax_updating');

                lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.updatefavouritestatus');
                lcAjaxRequestManager.AddObjectDataBlock('datablock', lcDataBlock, true);                

                lcAjaxRequestManager.SetCompleteHandler(function (paSuccess) {
                    if (paSuccess) {
                        lcRemoveItemList.each(function () {
                            $(this).removeAttr('gpos-attribute');
                        });

                        lcAddItemList.each(function () {
                            $(this).attr('gpos-attribute', 'favourite');
                        });

                        POSItemPanelManager.SetFavouriteEditingMode(false);
                    }
                });

                lcAjaxRequestManager.Execute();

            }
            else POSItemPanelManager.SetFavouriteEditingMode(false);
        },
        ShowPOSItemPanel : function()
        {
            clDeferred = $.Deferred();

            if (clPOSItemComposite.attr('fa-show') != 'true') {                
                clPOSItemComposite.attr('fa-show', 'true');
                return (clDeferred);
            }
        },
        ClosePOSItemPanel: function (paItemInfo) {
            if (clPOSItemComposite.attr('fa-show') == 'true') {
                clPOSItemComposite.removeAttr('fa-show');
                if (clDeferred)
                {
                    clDeferred.resolve(paItemInfo);
                }
            }
        },
        GetItemInfo: function(paItemCode)
        {   
            if (paItemCode)            
            {
                var lcItem = clBlockContainer.find('a[gpos-itemcode="' + paItemCode + '"]');
                if (lcItem.length > 0)
                {
                   return(lcItem);                    
                }                
            }
            return(null);            
        },
        HandlerOnFunctionClick: function (paEvent)
        {
            paEvent.preventDefault();
            
            var lcButton    = $(this);            
            var lcEditingMode = clPOSItemPanel.attr('fa-editing');
            var lcCommand = lcButton.attr('ea-command').substring('5');
                      
            switch (lcCommand) {
                case 'rootcategory':
                    {
                        if (lcEditingMode != 'true')  POSItemPanelManager.SetActiveCategory(0);
                        break;
                    }
                case 'parentcategory':
                    {
                        if (lcEditingMode != 'true') {
                            POSItemPanelManager.PopCategory();                            
                        }
                        break;
                    }
                case 'favourite':
                    {
                        if (lcEditingMode != 'true') {
                            POSItemPanelManager.SwitchToMode('favourite');
                        }
                        break;
                    }

                case 'list':
                    {
                        if (lcEditingMode != 'true') {
                            clPOSItemPanel.attr('ea-mode', 'list');
                        }
                        break;
                    }
                case 'editfavourite':
                    {
                        if (lcEditingMode == 'true')
                            POSItemPanelManager.ApplyFavouriteChanges();
                        else POSItemPanelManager.SetFavouriteEditingMode(true);
                        break;
                    }

                case 'switchview':
                    {
                        if (lcEditingMode != 'true') {
                            if (clPOSItemPanel.attr('ea-displaymode') == 'itemcode')
                                clPOSItemPanel.attr('ea-displaymode', 'price');
                            else
                                clPOSItemPanel.attr('ea-displaymode', 'itemcode');
                        }
                        break;
                    }

                case 'close':
                    {
                        if (lcEditingMode== 'true') {
                            POSItemPanelManager.SetFavouriteEditingMode(false);
                        }
                        else
                        {
                            POSItemPanelManager.ClosePOSItemPanel(null);
                        }
                        break;
                    }
            }            
        }
    }
})();
