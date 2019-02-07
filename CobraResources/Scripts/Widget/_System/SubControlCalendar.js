
//$(document).ready(function () {    
//    var clCalendar = new CalendarController();
//    clCalendar.Init();
//    clCalendar.SetDate(moment());
//});

var CalendarComposite = function (paInstanceName, paLowerBound, paUpperBound) {
    var clComposite;
    var clMonthList;
    var clMonthBox;
    var clCalendarCells;
    var clCalendar;
    var clCurrentDate;

    var clInstanceName = paInstanceName;
    
    var clLowerBound = moment(paLowerBound.format('YYYYMMDD'), 'YYYYMMDD');
    if (!clLowerBound.isValid()) clLowerBound = moment('19000101', 'YYYYMMDD');
    
    var clUpperBound = moment(paUpperBound.format('YYYYMMDD'), 'YYYYMMDD');
    if (!clUpperBound.isValid()) clUpperBound = moment('20991231', 'YYYYMMDD');
    
    return {
        Init: function () {
            clComposite = $('[sa-elementtype=composite].SubControlCalendarComposite[ea-type=calendar]');            
            clMonthBox = clComposite.find('.CalendarBox .MonthBar span');
            clCalendar = clComposite.find('.CalendarBox .Calendar');
            clCalendarCells = clComposite.find('.CalendarBox .Calendar .DayRow .Cell');            

            var lcMonthListStr = clComposite.attr('ea-datalist');            
            if (lcMonthListStr) clMonthList = lcMonthListStr.split(',');

            clComposite.find('a[ea-command]').unbind('click');
            clComposite.find('a[ea-command]').click({ control: this }, this.HandlerOnClick);
            
            clComposite.find('div[ea-command="@popupcmd%cellclick"]').unbind('click');
            clComposite.find('div[ea-command="@popupcmd%cellclick"]').click({ control : this },this.HandlerOnClick);
        },
        SetHandler: function (paTriggerName, paFunction) {
            if ((paTriggerName) && (paFunction)) {
                clComposite.unbind(paTriggerName);
                clComposite.bind(paTriggerName, paFunction);
            }
        },
        BuildCalendar: function ()
        {
            if ((clCurrentDate) && (clCurrentDate.isValid) && (clCurrentDate.isValid())) {
                var lcStartDate = moment(clCurrentDate.format('YYYY-MM-DD'), 'YYYY-MM-DD');
                lcStartDate = lcStartDate.startOf('month').isoWeekday(0);
                
                clCalendarCells.each(function (paIndex) {
                    var lcElement = $(this);
                    var lcDate = moment(lcStartDate.format('YYYY-MM-DD'), 'YYYY-MM-DD');
                    lcDate = lcDate.add(paIndex, 'days');

                    if (lcDate.month() == clCurrentDate.month()) $(this).attr('fa-currentmonth', 'true');
                    else $(this).removeAttr('fa-currentmonth');

                    if (lcDate.format('YYYY-MM-DD') == clCurrentDate.format('YYYY-MM-DD')) $(this).attr('fa-activedate', 'true');
                    else $(this).removeAttr('fa-activedate');

                    $(this).attr('value', lcDate.format('YYYYMMDD'));

                    if (lcDate.isBefore(clLowerBound) || lcDate.isAfter(clUpperBound))
                        $(this).attr('fa-outofbound', 'true');
                    else $(this).removeAttr('fa-outofbound');
                    
                    lcElement.text(FormManager.ConvertToFormLanguage(lcDate.format('D')));
                });
                
            }            
        },
        NavigateMonth : function(paDirection)
        {
            if (clCurrentDate)
            {                
                switch(paDirection)
                {
                    case 'previous':
                        {
                            var lcDate = moment(clCurrentDate.format('YYYY-MM-DD'), 'YYYY-MM-DD');
                            lcDate = lcDate.add(-1, 'months');
                            
                            this.SetDate(lcDate);
                            break;
                        }

                    case 'next':
                        {
                            var lcDate = moment(clCurrentDate.format('YYYY-MM-DD'), 'YYYY-MM-DD');
                            lcDate = lcDate.add(1, 'months');

                            this.SetDate(lcDate);
                            break;
                        }
                }
            }
            
        },
        SetDate : function(paDate)
        {
            
            if ((paDate) && (paDate.isValid) && (paDate.isValid())) {
                if ((paDate.format('YYYYMM') >= clLowerBound.format('YYYYMM')) && (paDate.format('YYYYMM') <= clUpperBound.format('YYYYMM')))
                {
                    if (clMonthList) {
                        if ((!clCurrentDate) || (clCurrentDate.month() != paDate.month())) {                            
                            if (paDate.isBefore(clLowerBound)) clCurrentDate = moment(clLowerBound.format('YYYY-MM-DD'), 'YYYY-MM-DD');
                            else if (paDate.isAfter(clUpperBound)) clCurrentDate = moment(clUpperBound.format('YYYY-MM-DD'), 'YYYY-MM-DD');
                            else clCurrentDate = moment(paDate.format('YYYY-MM-DD'), 'YYYY-MM-DD');

                            var lcMonthStr = clMonthList[Number(paDate.month())] + ', ' + FormManager.ConvertToFormLanguage(paDate.year());
                            clMonthBox.text(lcMonthStr);

                            this.BuildCalendar();
                        }
                        else {
                            if (paDate.isBefore(clLowerBound)) clCurrentDate = moment(clLowerBound.format('YYYY-MM-DD'), 'YYYY-MM-DD');
                            else if (paDate.isAfter(clUpperBound)) clCurrentDate = moment(clUpperBound.format('YYYY-MM-DD'), 'YYYY-MM-DD');
                            else clCurrentDate = moment(paDate.format('YYYY-MM-DD'), 'YYYY-MM-DD');

                            clCalendarCells.removeAttr('fa-activedate');
                            clCalendar.find('[value=' + clCurrentDate.format('YYYYMMDD') + ']').attr('fa-activedate', 'true');

                        }
                    }
                }
            }            
        },
        Show :function()
        {
            var lcEventInfo = {};

            clComposite.attr('fa-show','true');
        },
        Hide : function()
        {
            clComposite.removeAttr('fa-show');
        },
        SetHandler: function (paTriggerName, paFunction) {
            if ((paTriggerName) && (paFunction)) {
                clComposite.unbind(paTriggerName);
                clComposite.bind(paTriggerName, paFunction);
            }
        },
        ActionOnCellClick : function(paElement)
        {
            var lcDateValue = paElement.attr('value');
            var lcDate = moment(lcDateValue, 'YYYY-MM-DD');

            if (lcDate.isValid()) this.SetDate(lcDate);
                
        },
        ActionOnCommand : function(paEventInfo)
        {            
            clComposite.trigger('ev-command', paEventInfo);

            if (paEventInfo.defaultaction) {
                switch (paEventInfo.eventdata) {
                    case "setdate": {
                        var lcEventInfo = jQuery.extend({}, paEventInfo);
                        var lcActiveCell = clCalendar.find('[fa-activedate]');
                        var lcActiveDate = moment(lcActiveCell.attr('value'), 'YYYYMMDD');
                        if (lcActiveDate.isValid()) {
                            lcEventInfo.eventdata = lcActiveDate;
                            lcEventInfo.defaultaction = true;

                            clComposite.trigger('ev-datechanged', lcEventInfo);
                            if (lcEventInfo.defaultaction) clComposite.removeAttr('fa-show');
                        }                        
                        break;
                    }

                    case "cancel": {
                        clComposite.removeAttr('fa-show');
                        break;
                    }

                    case "prevmonth": {
                        this.NavigateMonth('previous');
                        break;
                    }

                    case "nextmonth": {
                        this.NavigateMonth('next');
                        break;
                    }

                    case "cellclick": {                        
                        this.ActionOnCellClick(paEventInfo.target);
                        break;
                    }
                }
            }
        },
        HandlerOnClick: function (paEvent) {            
            var lcEventInfo = {};
            var lcCommand = $(this).attr('ea-command');
            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);

            lcEventInfo["typeid"] = 'calendar';
            lcEventInfo["defaultaction"] = true;
            lcEventInfo["target"] = $(this);
            lcEventInfo["eventdata"] = lcCommand;
            
            paEvent.data.control.ActionOnCommand(lcEventInfo);
        }
    }
};

