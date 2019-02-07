$(document).ready(function () {
    POSMonthlyTransactionManager.Init($('[sa-elementtype=control].WidControlPOSMonthlyTransaction'));
});

var POSMonthlyTransactionManager = (function () {
    
    var     clControl;
    var     clForm;
    var     clContentContainer;
    var     clMonthListPopUp;
    var     clMonthListContainer;
    var     clMonthBar;
    var     clActiveDate;    

    return {
        Init: function (paPOSMonthlyTransactionControl) {
            clControl           = paPOSMonthlyTransactionControl;
            clForm              = clControl.closest('[sa-elementtype=form]');
            clMonthListPopUp    = clForm.find('[sa-elementtype=overlay][ea-type=monthlist]');
            clMonthBar          = clControl.find('[sa-elementtype=datebox].MonthBar');
            clMonthListContainer = clMonthListPopUp.find('[sa-elementtype=list].MonthListContainer');
            clContentContainer  = clControl.find('[sa-elementtype=container][ea-type=content]');
            clActiveDate = moment(clMonthBar.attr('value'), 'YYYY-MM-DD');
            POSMonthlyTransactionManager.RefreshGrid(clActiveDate);

            POSMonthlyTransactionManager.BindEvents();            
        },

        BindEvents: function () {
            clControl.find('[ea-command]').unbind('click');
            clControl.find('[ea-command]').click(POSMonthlyTransactionManager.HandlerOnClick);

            clMonthListPopUp.find('[ea-command]').unbind('click');
            clMonthListPopUp.find('[ea-command]').click(POSMonthlyTransactionManager.HandlerOnClick);
        },

        RefreshGrid : function(paDate)
        {
            var lcDeferred = $.Deferred();

            if ((paDate) && (paDate.isValid)) {
                var lcAjaxRequestManager = new AjaxRequestManager('getupdatedcontrol', null, null, 'ajax_loading');

                lcAjaxRequestManager.AddAjaxParam('Parameter', 'monthlytransactioncontent');
                lcAjaxRequestManager.AddObjectDataBlock('paramblock', { FPM_DATE: paDate.format('YYYY-MM-DD') });

                lcAjaxRequestManager.SetCompleteHandler(function (paSuccess, paResponseStruct) {
                    if (paSuccess) {                        
                        clActiveDate = paDate;
                        clMonthBar.text(clMonthListContainer.find('a[value="' + clActiveDate.format('YYYY-MM-DD') + '"]').attr('ea-text'));
                        clMonthBar.attr('value', clActiveDate.format('YYYY-MM-DD'));

                        clContentContainer.html(paResponseStruct.ResponseData.RSP_HTML);                        

                        lcDeferred.resolve(true);
                    }
                    else lcDeferred.resolve(false);
                });

                lcAjaxRequestManager.Execute();
            }
        },
        HandlerOnClick: function (paEvent) {
            paEvent.preventDefault();

            var lcCommand   = $(this).attr('ea-command');
            lcCommand       = lcCommand.substring(lcCommand.indexOf('%') + 1);

            switch (lcCommand) {

                case "showpopup":                
                    {                        
                        var lcItems         = clMonthListContainer.find('[sa-elementtype=item]');
                        var lcActiveItem    = clMonthListContainer.find('[sa-elementtype=item][value="' + clMonthBar.attr('value') + '"]');

                        lcItems.removeAttr('fa-selected');
                        lcActiveItem.attr('fa-selected', 'true');

                        clForm.attr('fa-showpopup', 'true');
                        break;
                    }

                case 'popup.close'  :
                case 'popup.cancel' :
                    {
                        clForm.removeAttr('fa-showpopup');
                        break;
                    }

                case 'popup.choose' :
                    {
                        var lcItem = clMonthListContainer.find('a[fa-selected]');

                        if (lcItem.length > 0)
                        {
                            POSMonthlyTransactionManager.RefreshGrid(moment(lcItem.attr('value'), 'YYYY-MM-DD'));
                        }
                        clForm.removeAttr('fa-showpopup');
                        
                        break;
                    }

                case 'monthselect' :
                    {                        
                        var lcItems = clMonthListContainer.find('[sa-elementtype=item]');
                        lcItems.removeAttr('fa-selected');

                        $(this).attr('fa-selected', 'true');
                    }

            }
        }
    }
})();
