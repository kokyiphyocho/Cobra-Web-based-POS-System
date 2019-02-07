$(document).ready(function () {    
    POSDailyReceiptManager.Init($('[sa-elementtype=control].WidControlPOSDailyReceipt'));
});

var POSDailyReceiptManager = (function () {

    var clPOSDailyReceiptControl;
    var clSummaryContainer;    
    var clPopUpList = [];
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
        Init: function (paPOSDailyReceiptControl) {
            clPOSDailyReceiptControl = paPOSDailyReceiptControl;
            clSummaryContainer = clPOSDailyReceiptControl.find('[sa-elementtype=container].SummaryContainer');
            clHeaderBar = clPOSDailyReceiptControl.find('[sa-elementtype=controlbar]');
            clDateBox = clHeaderBar.find('[sa-elementtype=datebox]');
            clPreviousButton = clHeaderBar.find('a[ea-command="@cmd%prevday"]');
            clNextButton = clHeaderBar.find('a[ea-command="@cmd%nextday"]');
            
            clActiveDate = moment(clDateBox.text, "DD/MM/YYYY");
            if (!clActiveDate.isValid()) clActiveDate = moment();

            clDateBox.text(clActiveDate.format('DD/MM/YYYY'));

            clLowerBoundDays = Number(clPOSDailyReceiptControl.attr('ea-lowerbound') || '7');
            clUpperBoundDays = Number(clPOSDailyReceiptControl.attr('ea-upperbound') || '0');

            clLowerBoundDate = moment().add((!isNaN(clLowerBoundDays) ? -1 * clLowerBoundDays : -7), 'days');
            clUpperBoundDate = moment().add((!isNaN(clUpperBoundDays) ? clUpperBoundDays : 0), 'days');
            
            POSDailyReceiptManager.RetrieveContentByDate(clActiveDate).done(function (paSuccess) {

                clCalendar = new CalendarComposite('enquirydate', clLowerBoundDate, clUpperBoundDate);
                clCalendar.Init();

                POSDailyReceiptManager.BindEvents();
            });
            
        },
        CreateObject : function()
        {
            var lcPopUpElements = clPOSDailyReceiptControl.find('[sa-elementtype=block] [sa-elementtype=popup]');            
            
            if (POSPopUpReceiptDetail)
            {                
                lcPopUpElements.each(function () {
                    var lcPopUpObject = new POSPopUpReceiptDetail($(this));
                    lcPopUpObject.Init();

                    clPopUpList.push(lcPopUpObject);
                });
            }
            else setTimeout(function () { POSDailyReceiptManager.CreateObject() }, 100);
        },
        BindEvents : function()
        {
            clHeaderBar.find('a[ea-command]').unbind('click');
            clHeaderBar.find('a[ea-command]').click(POSDailyReceiptManager.HandlerOnClick);

            clHeaderBar.find('span[ea-command]').unbind('click');
            clHeaderBar.find('span[ea-command]').click(POSDailyReceiptManager.HandlerOnClick);

            clCalendar.SetHandler('ev-datechanged', POSDailyReceiptManager.HandlerOnDateChanged);
        },
        BindElementEvents : function()
        {
            var lcDetailPopUpButtons = clPOSDailyReceiptControl.find('[sa-elementtype=container] [sa-elementtype=block] a[ea-command="@cmd%openpopup"]');

            lcDetailPopUpButtons.unbind('click');
            lcDetailPopUpButtons.click(POSDailyReceiptManager.HandlerOnClick);
        },
        OpenPopUp : function(paPopUpType)
        {
            if (paPopUpType)
            {
                clPOSDailyReceiptControl.attr('fa-activepopup', paPopUpType);
            }
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

                lcAjaxRequestManager.AddAjaxParam('Parameter', 'DailyReceiptinfo');
                lcAjaxRequestManager.AddObjectDataBlock('paramblock', { FPM_DATE: paDate.format('YYYY-MM-DD') });

                lcAjaxRequestManager.SetCompleteHandler(function (paSuccess, paResponseStruct) {
                    if (paSuccess) {                        
                        clActiveDate = moment(paDate.format('DD/MM/YYYY'), 'DD/MM/YYYY');
                        clDateBox.text(clActiveDate.format('DD/MM/YYYY'));
                        POSDailyReceiptManager.RefreshNavigationButtonAppearance();
                        
                        clSummaryContainer.empty();                        
                        clSummaryContainer.html(paResponseStruct.ResponseData.RSP_HTML);
                    
                        POSDailyReceiptManager.CreateObject();
                        POSDailyReceiptManager.BindElementEvents();

                                                
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
        NavigateDate: function (paDirection) {
            var lcDataDate = moment(clActiveDate.format('YYYY-MM-DD'), 'YYYY-MM-DD');

            switch (paDirection) {
                case "previous":
                    {
                        if (Number(lcDataDate.format('YYYYMMDD')) > Number(clLowerBoundDate.format('YYYYMMDD'))) {
                            lcDataDate.add(-1, 'days');
                            POSDailyReceiptManager.RetrieveContentByDate(lcDataDate);
                        }
                        break;
                    }

                case "next":
                    {
                        if (Number(lcDataDate.format('YYYYMMDD')) < Number(clUpperBoundDate.format('YYYYMMDD'))) {
                            lcDataDate.add(1, 'days');
                            POSDailyReceiptManager.RetrieveContentByDate(lcDataDate);
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
                    POSDailyReceiptManager.RetrieveContentByDate(lcDate);
                    return;
                }
            }

            paEventInfo.defaultaction = false;
        },
        HandlerOnClick : function(paEvent)
        {
            paEvent.preventDefault();
            
            var lcCommand = $(this).attr('ea-command');
            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);
            
            switch(lcCommand)
            {
                case 'openpopup':
                    {
                        var lcBlock = $(this).closest('[sa-elementtype=block]');
                        var lcType = lcBlock.attr('ea-type');
                        
                        POSDailyReceiptManager.OpenPopUp(lcType);
                        break;
                    }
                case "prevday":
                    {
                        POSDailyReceiptManager.NavigateDate('previous');
                        break;
                    }
                case "nextday":
                    {
                        POSDailyReceiptManager.NavigateDate('next');
                        break;
                    }
                case "showcalendar":
                    {
                        clCalendar.SetDate(clActiveDate);
                        clCalendar.Show();
                        break;
                    }
            }
        }
    }
})();