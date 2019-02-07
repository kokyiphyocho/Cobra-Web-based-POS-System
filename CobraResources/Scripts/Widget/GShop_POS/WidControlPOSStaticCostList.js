$(document).ready(function () {
    POSStaticCostListManager.Init();
});

var POSStaticCostListManager = (function () {
    var clForm;
    var clToolBar;
    var clControl;
    var clContainer;

    return {
        Init: function () {
            clForm = FormManager.GetForm();
            clToolBar = ToolBarManager.GetToolBar();
            clControl = clForm.find('[sa-elementtype=control].WidControlPOSStaticCostList');
            clContainer = clControl.find('[sa-elementtype=container]');            
            POSStaticCostListManager.BindEvents();

            POSStaticCostListManager.LoadContent().done(function () {
                POSStaticCostListManager.RestoreBlockState();
            });
        },
        BindEvents: function () {
            clToolBar.find('a[ea-command="@cmd%editmode"],a[ea-command="@cmd%deletemode"]').unbind('click');
            clToolBar.find('a[ea-command="@cmd%editmode"],a[ea-command="@cmd%deletemode"]').click(POSStaticCostListManager.HandlerOnClick);

            clToolBar.find('a[ea-command^="@cmd%add"]').unbind('click');
            clToolBar.find('a[ea-command^="@cmd%add"]').click(POSStaticCostListManager.HandlerOnClick);
        },
        BindElementEvents: function () {
            clControl.find('div[sa-elementtype=row][ea-command]').unbind('click');
            clControl.find('div[sa-elementtype=row][ea-command]').click(POSStaticCostListManager.HandlerOnClick);

            clControl.find('div[sa-elementtype=title] div[ea-command="@cmd%upcategory"]').unbind('click');
            clControl.find('div[sa-elementtype=title] div[ea-command="@cmd%upcategory"]').click(POSStaticCostListManager.HandlerOnClick);

            clControl.find('div[sa-elementtype=title] div[ea-command="@cmd%rootcategory"]').unbind('click');
            clControl.find('div[sa-elementtype=title] div[ea-command="@cmd%rootcategory"]').click(POSStaticCostListManager.HandlerOnClick);

            clControl.find('img[ea-command="@cmd%delete"]').unbind('click');
            clControl.find('img[ea-command="@cmd%delete"]').click(POSStaticCostListManager.HandlerOnClick);

            clControl.find('img[ea-command="@cmd%edit"]').unbind('click');
            clControl.find('img[ea-command="@cmd%edit"]').click(POSStaticCostListManager.HandlerOnClick);
        },
        LoadContent: function () {
            var lcDeferred = $.Deferred();

            var lcAjaxRequestManager = new AjaxRequestManager('getupdatedcontrol', null, null, 'ajax_loading');

            lcAjaxRequestManager.AddAjaxParam('Parameter', 'itemlistcontent');

            lcAjaxRequestManager.SetCompleteHandler(function (paSuccess, paResponseStruct) {
                if (paSuccess) {
                    clContainer.empty();
                    clContainer.html(paResponseStruct.ResponseData.RSP_HTML);

                    POSStaticCostListManager.BindElementEvents();
                    lcDeferred.resolve(true);
                }
                else {
                    lcDeferred.resolve(false)
                }
            });

            lcAjaxRequestManager.Execute();

            return (lcDeferred);
        },
        SetControlMode: function (paMode) {
            if (paMode) {
                var lcToggleState = clControl.attr('fa-mode');
                var lcTargetElement = clToolBar.find('a[ea-command="@cmd%' + paMode + '"]');

                clToolBar.find('[fa-active]').removeAttr('fa-active');

                if (lcToggleState != paMode) {
                    lcTargetElement.attr('fa-active', 'true');
                    clControl.attr('fa-mode', paMode);
                }
                else {
                    clControl.removeAttr('fa-mode');
                }
            }
        },
        OpenCostEditorForm: function (paItemID) {
            
            var lcLinkTemplate = clControl.attr('ea-template');
                                   
            var lcLink = lcLinkTemplate.replace('$ITEMID', paItemID);
            var lcBlockStateInfo = POSStaticCostListManager.SaveBlockState(clControl);

            FormManager.RedirectStatefulTextLink(lcLink, lcBlockStateInfo);
            
        },
        SaveBlockState: function () {
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
        ShowCategoryBlock: function (paDataID) {
            if (paDataID) {
                var lcActiveBlock = clControl.find('[sa-elementtype=block][fa-position=middle]');
                if (lcActiveBlock.length == 0) lcActiveBlock = clControl.find('[sa-elementtype=block][ea-group="0"]');

                var lcCategoryBlock = clControl.find('[sa-elementtype=block][ea-group="' + paDataID + '"]');
                lcActiveBlock.attr('fa-position', 'left');
                lcCategoryBlock.attr('fa-position', 'middle');

                clControl.removeAttr('fa-mode');
            }
        },
        UpCategoryBlock: function () {
            var lcActiveBlock = clControl.find('[sa-elementtype=block][fa-position=middle]');
            var lcParent = lcActiveBlock.attr('ea-parent');

            if (lcParent) {
                var lcParentBlock = clControl.find('[sa-elementtype=block][ea-group="' + lcParent + '"]');
                if (lcParentBlock) {
                    lcParentBlock.attr('fa-position', 'middle');
                    lcActiveBlock.removeAttr('fa-position');
                    clControl.removeAttr('fa-mode');
                }
            }
        },
        RootCategoryBlock: function () {
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
        SetFocus : function(paControl)
        {            
            if (paControl)
            {
                if (paControl.attr('fa-focus') != 'true')
                {
                    clContainer.find('[sa-elementtype=row].ItemRow').removeAttr('fa-focus');
                    paControl.attr('fa-focus', 'true');
                    return;
                }                
            }            
            
            clContainer.find('[sa-elementtype=row].ItemRow').removeAttr('fa-focus');            
        },
        HandlerOnClick: function (paEvent) {
            paEvent.preventDefault();

            var lcCommand = $(this).attr('ea-command');
            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);

            switch (lcCommand) {
                case 'editmode':
                case 'deletemode':
                    {
                        POSStaticCostListManager.SetControlMode(lcCommand);
                        break;
                    }

                case 'additem':
                case 'addstaticitem':
                case 'addcategory':
                case 'addservice':
                    {
                        var lcMode = lcCommand.substring(3);

                        if (lcMode) POSStaticCostListManager.OpenElementForm(lcMode);
                        break;
                    }

                case 'showcategory':
                    {
                        var lcDataID = $(this).attr('ea-dataid');
                        var lcType   = $(this).attr('ea-type');

                        POSStaticCostListManager.SetFocus(null);
                        if (lcType == 'category') POSStaticCostListManager.ShowCategoryBlock(lcDataID);

                        break;
                    }

                case 'setfocus' : 
                    {
                        POSStaticCostListManager.SetFocus($(this));
                        break;
                    }

                case 'upcategory':
                    {
                        POSStaticCostListManager.UpCategoryBlock();
                        break;
                    }

                case 'rootcategory':
                    {
                        POSStaticCostListManager.RootCategoryBlock();
                        break;
                    }

                case 'edit':
                    {
                        paEvent.stopPropagation();

                        var lcItemRow = $(this).closest('[sa-elementtype=row]');                        
                        var lcItemID = lcItemRow.attr('ea-dataid');

                        POSStaticCostListManager.OpenCostEditorForm(lcItemID);

                        break;
                    }               
            }
        },
    }
})();
