$(document).ready(function () {
    POSDailyStockBalanceManager.Init();
});

var POSDailyStockBalanceManager = (function () {
    var clForm;    
    var clControl;
    var clContainer;
    var clDecimalPlace;
    var clLowerBoundDays;
    var clUpperBoundDays;
    var clLowerBoundDate;
    var clUpperBoundDate;
    var clHeaderBar;
    var clDateBox;
    var clPreviousButton;
    var clNextButton;
    var clCalendar;


    return {
        Init: function () {
            clForm = FormManager.GetForm();            
            clControl = clForm.find('[sa-elementtype=control].WidControlPOSDailyStockBalance');
            clContainer = clControl.find('[sa-elementtype=container]');
            clDecimalPlace = (clControl.attr('ea-decimal') || '').ForceConvertToInteger(0);
            clHeaderBar = clControl.find('[sa-elementtype=controlbar]');
            clDateBox = clHeaderBar.find('[sa-elementtype=datebox]');
            clPreviousButton = clHeaderBar.find('a[ea-command="@cmd%prevday"]');
            clNextButton = clHeaderBar.find('a[ea-command="@cmd%nextday"]');

            clActiveDate = moment(clDateBox.text, "DD/MM/YYYY");
            if (!clActiveDate.isValid()) clActiveDate = moment();

            clDateBox.text(clActiveDate.format('DD/MM/YYYY'));

            clLowerBoundDays = Number(clControl.attr('ea-lowerbound') || '7');
            clUpperBoundDays = Number(clControl.attr('ea-upperbound') || '0');

            clLowerBoundDate = moment().add((!isNaN(clLowerBoundDays) ? -1 * clLowerBoundDays : -7), 'days');
            clUpperBoundDate = moment().add((!isNaN(clUpperBoundDays) ? clUpperBoundDays : 0), 'days');
                                    
            POSDailyStockBalanceManager.RetrieveContentByDate(clActiveDate).done(function () {

                clCalendar = new CalendarComposite('enquirydate', clLowerBoundDate, clUpperBoundDate);
                clCalendar.Init();

                POSDailyStockBalanceManager.BindEvents();                
            });
        },
        BindEvents: function () {
            clHeaderBar.find('a[ea-command]').unbind('click');
            clHeaderBar.find('a[ea-command]').click(POSDailyStockBalanceManager.HandlerOnClick);

            clHeaderBar.find('span[ea-command]').unbind('click');
            clHeaderBar.find('span[ea-command]').click(POSDailyStockBalanceManager.HandlerOnClick);

            clCalendar.SetHandler('ev-datechanged', POSDailyStockBalanceManager.HandlerOnDateChanged);
        },
        BindElementEvents: function () {
            clControl.find('div[sa-elementtype=row][ea-command="@cmd%showcategory"]').unbind('click');
            clControl.find('div[sa-elementtype=row][ea-command="@cmd%showcategory"]').click(POSDailyStockBalanceManager.HandlerOnClick);

            clControl.find('div[sa-elementtype=title] div[ea-command="@cmd%upcategory"]').unbind('click');
            clControl.find('div[sa-elementtype=title] div[ea-command="@cmd%upcategory"]').click(POSDailyStockBalanceManager.HandlerOnClick);

            clControl.find('div[sa-elementtype=title] div[ea-command="@cmd%rootcategory"]').unbind('click');
            clControl.find('div[sa-elementtype=title] div[ea-command="@cmd%rootcategory"]').click(POSDailyStockBalanceManager.HandlerOnClick);
        },

        RefreshNavigationButtonAppearance: function () {
            if (Number(clActiveDate.format('YYYYMMDD')) <= Number(clLowerBoundDate.format('YYYYMMDD')))
                clPreviousButton.attr('fa-disable', 'true');
            else
                clPreviousButton.removeAttr('fa-disable');

            if (Number(clActiveDate.format('YYYYMMDD')) >= Number(clUpperBoundDate.format('YYYYMMDD')))
                clNextButton.attr('fa-disable', 'true');
            else
                clNextButton.removeAttr('fa-disable');
        },
        RetrieveContentByDate: function (paDate) {

            var lcDeferred = $.Deferred();

            if ((paDate) && (paDate.isValid)) {
                var lcAjaxRequestManager = new AjaxRequestManager('getupdatedcontrol', null, null, 'ajax_loading');

                lcAjaxRequestManager.AddAjaxParam('Parameter', 'stockbalancelist');
                lcAjaxRequestManager.AddObjectDataBlock('paramblock', { FPM_DATE: paDate.format('YYYY-MM-DD') });

                lcAjaxRequestManager.SetCompleteHandler(function (paSuccess, paResponseStruct) {
                    if (paSuccess) {
                        clActiveDate = moment(paDate.format('DD/MM/YYYY'), 'DD/MM/YYYY');
                        clDateBox.text(clActiveDate.format('DD/MM/YYYY'));
                        POSDailyStockBalanceManager.RefreshNavigationButtonAppearance();

                        clContainer.empty();
                        clContainer.html(paResponseStruct.ResponseData.RSP_HTML);

                        POSDailyStockBalanceManager.BindElementEvents();
                        POSDailyStockBalanceManager.CalculateTotalValue();                       


                        lcDeferred.resolve(true);
                    }
                    else lcDeferred.resolve(false);
                });

                lcAjaxRequestManager.Execute();
            }
            else {
                setTimeout(function () { lcDeferred.resolve(false); }, 500);
            }

            return (lcDeferred);
        },
        //LoadContent: function () {
        //    var lcDeferred = $.Deferred();

        //    var lcAjaxRequestManager = new AjaxRequestManager('getupdatedcontrol', null, null, 'ajax_loading');

        //    lcAjaxRequestManager.AddAjaxParam('Parameter', 'stockbalancelist');

        //    lcAjaxRequestManager.SetCompleteHandler(function (paSuccess, paResponseStruct) {
        //        if (paSuccess) {
        //            clContainer.empty();
        //            clContainer.html(paResponseStruct.ResponseData.RSP_HTML);

        //            POSDailyStockBalanceManager.BindElementEvents();
        //            lcDeferred.resolve(true);
        //        }
        //        else {
        //            lcDeferred.resolve(false)
        //        }
        //    });

        //    lcAjaxRequestManager.Execute();

        //    return (lcDeferred);
        //},
        //SetControlMode: function (paMode) {
        //    if (paMode) {
        //        var lcToggleState = clControl.attr('fa-mode');
        //        var lcTargetElement = clToolBar.find('a[ea-command="@cmd%' + paMode + '"]');

        //        clToolBar.find('[fa-active]').removeAttr('fa-active');

        //        if (lcToggleState != paMode) {
        //            lcTargetElement.attr('fa-active', 'true');
        //            clControl.attr('fa-mode', paMode);
        //        }
        //        else {
        //            clControl.removeAttr('fa-mode');
        //        }
        //    }
        //},
        //OpenElementForm: function (paMode, paItemID) {
        //    var lcIndex = -1;

        //    paItemID = paItemID || '-1'

        //    switch (paMode) {
        //        case "item": lcIndex = 0; break;
        //        case "category": lcIndex = 1; break;
        //        case "serviceitem": lcIndex = 2; break;
        //    }

        //    if (lcIndex >= 0) {
        //        var lcLinkTemplate = clControl.attr('ea-template').split("||")[lcIndex];

        //        var lcActiveBlock = clControl.find('[sa-elementtype=block][fa-position=middle]');
        //        if (lcActiveBlock.length == 0) lcActiveBlock = clControl.find('[sa-elementtype=block][ea-group="0"]');

        //        var lcCategory = lcActiveBlock.attr('ea-group');
        //        var lcLink = lcLinkTemplate.replace('$CATEGORY', lcCategory).replace('$ITEMID', paItemID);
        //        var lcBlockStateInfo = POSDailyStockBalanceManager.SaveBlockState(clControl);

        //        FormManager.RedirectStatefulTextLink(lcLink, lcBlockStateInfo);
        //    }
        //},
        CalculateTotalValue : function(paCategoryGroup)
        {
            paCategoryGroup = paCategoryGroup >= 0 ? paCategoryGroup : 0;
            
            var lcBlock             = clContainer.find('[sa-elementtype=block][ea-group="' + paCategoryGroup + '"]');
            var lcItemList          = lcBlock.find('[sa-elementtype=list] .ItemRow');
            var lcSubCategoryList   = lcBlock.find('[sa-elementtype=list] [ea-type=category]');
            var lcSummaryBar        = lcBlock.find('[sa-elementtype=summary]');
            var lcSummaryTotalBox   = lcSummaryBar.find('[sa-elementtype=total]');
            var lcTotal             = 0;
                        
            lcSubCategoryList.each(function () {
                var lcCategoryGroup = $(this).attr('ea-dataid');
                var lcFigureBox = $(this).find('[sa-elementtype=figure]');

                if (lcCategoryGroup) {
                    var lcTotal = POSDailyStockBalanceManager.CalculateTotalValue(lcCategoryGroup);
                    lcFigureBox.attr('value', lcTotal);
                    lcFigureBox.text(ConvertToThousandSeparatorStr(lcTotal.toFixed(clDecimalPlace)).ConvertToFormLanguage());
                }
            });

            lcItemList.each(function () {
                var lcFigureBox = $(this).find('[sa-elementtype=figure]');
                lcTotal += (lcFigureBox.attr('value') || '').ForceConvertToDecimal(0);
            });            
            
            lcSummaryTotalBox.text(ConvertToThousandSeparatorStr(lcTotal.toFixed(clDecimalPlace)).ConvertToFormLanguage());
            lcBlock.attr('value', lcTotal.toFixed(clDecimalPlace));

            return (lcTotal);
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
        //DeleteRecord: function (paItemRow) {
        //    var lcDataID = paItemRow.attr('ea-dataid');
        //    var lcItemName = paItemRow.text();

        //    var lcAjaxRequestManager = new AjaxRequestManager('executenonquery', 'info_successdeleteobject', 'err_faildeleteobject', 'ajax_deleting');

        //    lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.deleteitemrecord');
        //    lcAjaxRequestManager.AddObjectDataBlock('datablock', { FPM_ITEMID: lcDataID }, true);
        //    lcAjaxRequestManager.AddMessagePlaceHolder('$ITEMNAME', lcItemName);

        //    lcAjaxRequestManager.SetCompleteHandler(function (paSuccess) {
        //        if (paSuccess) {
        //            paItemRow.remove();
        //        }
        //    });

        //    lcAjaxRequestManager.ExecuteOnConfirm('confirm_deleteobject');
        //},
        NavigateDate: function (paDirection) {
            var lcDataDate = moment(clActiveDate.format('YYYY-MM-DD'), 'YYYY-MM-DD');

            switch (paDirection) {
                case "previous":
                    {
                        if (Number(lcDataDate.format('YYYYMMDD')) > Number(clLowerBoundDate.format('YYYYMMDD'))) {
                            lcDataDate.add(-1, 'days');
                            POSDailyStockBalanceManager.RetrieveContentByDate(lcDataDate);
                        }
                        break;
                    }

                case "next":
                    {
                        if (Number(lcDataDate.format('YYYYMMDD')) < Number(clUpperBoundDate.format('YYYYMMDD'))) {
                            lcDataDate.add(1, 'days');
                            POSDailyStockBalanceManager.RetrieveContentByDate(lcDataDate);
                        }
                        break;
                    }
            }
        },
        HandlerOnDateChanged: function (paEvent, paEventInfo) {
            if (paEventInfo.eventdata) {
                var lcDate = paEventInfo.eventdata;
                var lcDateValue = Number(lcDate.format('YYYYMMDD'));

                if ((lcDateValue >= Number(clLowerBoundDate.format('YYYYMMDD'))) && (lcDateValue <= Number(clUpperBoundDate.format('YYYYMMDD')))) {
                    POSDailyStockBalanceManager.RetrieveContentByDate(lcDate);
                    return;
                }
            }

            paEventInfo.defaultaction = false;
        },
        HandlerOnClick: function (paEvent) {
            paEvent.preventDefault();

            var lcCommand = $(this).attr('ea-command');
            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);

            switch (lcCommand) {

                case "prevday":
                    {
                        POSDailyStockBalanceManager.NavigateDate('previous');
                        break;
                    }
                case "nextday":
                    {
                        POSDailyStockBalanceManager.NavigateDate('next');
                        break;
                    }
                case "showcalendar":
                    {
                        clCalendar.SetDate(clActiveDate);
                        clCalendar.Show();
                        break;
                    }

                case 'showcategory':
                    {
                        var lcDataID = $(this).attr('ea-dataid');
                        var lcType = $(this).attr('ea-type');

                        if (lcType == 'category') POSDailyStockBalanceManager.ShowCategoryBlock(lcDataID);

                        break;
                    }

                case 'upcategory':
                    {
                        POSDailyStockBalanceManager.UpCategoryBlock();
                        break;
                    }

                case 'rootcategory':
                    {
                        POSDailyStockBalanceManager.RootCategoryBlock();
                        break;
                    }
            }
        },
    }
})();
