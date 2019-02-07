$(document).ready(function () {
    $('.OptionPanel[sa-elementtype=panel]').BindOptionPanelEvents();  
});

$.fn.BindOptionPanelEvents = function () {    
    $(this).find('[sa-elementtype=list] .FilterGroup .GroupHeading a.Chevron').unbind('click');
    $(this).find('[sa-elementtype=list] .FilterGroup .GroupHeading a.Chevron').click(function (paEvent) {
        paEvent.preventDefault();

        var lcFilterGroup = $(this).closest('.FilterGroup');

        if (lcFilterGroup.attr('fa-open') != 'true') {
            lcFilterGroup.siblings('.FilterGroup').removeAttr('fa-open');
            lcFilterGroup.attr('fa-open', 'true');
        }
        else lcFilterGroup.removeAttr('fa-open');
    });

    $(this).find('[sa-elementtype=list] .FilterGroup .ItemContainer a.Item').unbind('click');
    $(this).find('[sa-elementtype=list] .FilterGroup .ItemContainer a.Item').click(function (paEvent) {
        paEvent.preventDefault();

        var lcFilterGroup = $(this).closest('.FilterGroup');
        var lcSelectionDisplay = lcFilterGroup.find('.Selection');

        $(this).siblings('.Item').removeAttr('fa-selected');
        $(this).attr('fa-selected', true);

        lcSelectionDisplay.text($(this).text());
        if ($(this).index() == 0) lcSelectionDisplay.removeAttr('fa-nondefault');
        else lcSelectionDisplay.attr('fa-nondefault', 'true');
    });

    $(this).find('a[href="@cmd%close"]').unbind('click');
    $(this).find('a[href="@cmd%close"]').click(function (paEvent) {

        paEvent.preventDefault();

        var lcControl = $(this).closest('[sa-elementtype=control]');

        lcControl.removeAttr('fa-showpopup');
    });

    $(this).find('a[href="@cmd%reset"]').unbind('click');
    $(this).find('a[href="@cmd%reset"]').click(function (paEvent) {

        paEvent.preventDefault();

        var lcPanel = $(this).closest('[sa-elementtype=panel]');

        lcPanel.find('[sa-elementtype=list] .FilterGroup').each(function () {
            $(this).find('.ItemContainer a.Item').first().trigger('click');
        });

    });

    $(this).find('a[href="@cmd%search"]').unbind('click');
    $(this).find('a[href="@cmd%search"]').click(function (paEvent) {

        paEvent.preventDefault();

        var lcPanel = $(this).closest('[sa-elementtype=panel]');
        lcPanel.GetFilterResult();
    });

    $(this).find('a[href="@cmd%sort"]').unbind('click');
    $(this).find('a[href="@cmd%sort"]').click(function (paEvent) {

        paEvent.preventDefault();

        var lcPanel = $(this).closest('[sa-elementtype=panel]');
        lcPanel.GetFilterResult();
    });

}

$.fn.ResetFilterSetting = function () {
    $(this).find('[ea-type=filter] [sa-elementtype=list] .FilterGroup').each(function () {
        $(this).find('.ItemContainer a.Item').first().trigger('click');
    });
}

$.fn.ResetSortSetting = function () {
    $(this).find('[ea-type=sort] [sa-elementtype=list] .FilterGroup').each(function () {
        $(this).find('.ItemContainer a.Item').first().trigger('click');
    });
}

$.fn.ShowOptionPopUp = function () {
    $(this).attr('fa-showpopup', 'true');
}

$.fn.HideOptionPopUp = function () {
    $(this).removeAttr('fa-showpopup');
}

$.fn.SetOptionSetting = function ()
{
    var lcOptionPanel   = $(this);
    var lcControl       = lcOptionPanel.closest('[sa-elementtype=control]');
    var lcOption        = {};
    var lcOptionGroups  = lcOptionPanel.find('[sa-elementtype=list] .FilterGroup');
    
    lcOptionGroups.each(function () {
        var lcSelectionControl = $(this).find('.GroupHeading .Selection');
        var lcOptionTemplate = $(this).attr('ea-template');
        lcOption[lcOptionTemplate] = lcSelectionControl.text().trim();
    });    
    
    if (lcOptionPanel.attr('ea-type') == 'filter')
        lcControl.attr('fa-filteroption', encodeURIComponent(Base64.encode(JSON.stringify(lcOption))));
    else if (lcOptionPanel.attr('ea-type') == 'sort')
        lcControl.attr('fa-sortoption', encodeURIComponent(Base64.encode(JSON.stringify(lcOption))));
}

$.fn.GetFilterResult = function () {
    lcOptionPanel = $(this);
    lcControl     = lcOptionPanel.closest('[sa-elementtype=control]');
    lcItemGrid    = lcControl.find('[sa-elementtype=grid]');
    lcControl.removeAttr('fa-showpopup');
    lcOptionPanel.SetOptionSetting();
    lcItemGrid.ReloadGridItems();

}

//$.fn.LoadFilterOption = function (paSortSettingOnly) {

//    var lcFilter = {};
//    var lcControlList = $(this).find('[sa-elementtype=list] .FilterGroup')

//    if (paSortSettingOnly)
//        $(this).find('[sa-type=sort] [sa-elementtype=list] .FilterGroup')

//    lcControlList.each(function (paIndex) {

//        var lcSelectedControl = $(this).find('.ItemContainer a.Item[fa-selected]');
//        var lcFilterTemplate = $(this).attr('sa-filtertemplate');

//        if (lcSelectedControl.attr('sa-templatemode') == 'true') {
//            lcFilterTemplate = lcFilterTemplate.replace('$FILTERPARAM', lcSelectedControl.attr('value'));
//        }
//        else lcFilterTemplate = lcSelectedControl.attr('value');

//        lcFilterTemplate = $(this).attr('value') + '.' + lcFilterTemplate;

//        paIndex = paIndex;

//        lcFilter["Filter" + (paIndex <= 0 ? '' : paIndex)] = lcFilterTemplate;
//    });

//    return (lcFilter);
//}

//$.fn.GetFilterResult = function () {
//    var lcForm = $(this).closest('[sa-elementtype=form]');
//    var lcItemGrid = lcForm.find('[sa-elementtype=container]');
//    var lcOption = lcForm.LoadFilterOption();
//    var lcOverlay = $(this).closest('[sa-elementtype=overlay]');
//    var lcControlParam = lcItemGrid.attr('sa-controlparam');
//    var lcChunkNoParamName = lcItemGrid.attr('sa-chunknoparamname');
//    var lcAjaxImageDiv = lcItemGrid.find('[sa-elementtype=ajaxloaderelement]');
//    var lcSearchBox = lcForm.find('[sa-elementtype=searchbox]');
//    var lcInputBox = lcSearchBox.find('input[type=text]');

//    if (lcOverlay.attr('sa-type') == 'filter') {
//        lcForm.attr('fa-filtermode', 'filter');
//        lcInputBox.val('');
//    }

//    var lcData = { SmartAjaxRequest: "lazyloaditemgrid", ControlParam: lcControlParam };
//    lcData = $.extend(lcData, lcOption);
//    if (lcOverlay.attr('sa-type') != 'filter')
//        lcData = $.extend(lcData, lcSearchBox.CompileFilterTemplate());

//    lcData[lcChunkNoParamName] = 1;

//    DoPostBack(lcData, function (paResponseData) {
//        var lcRespondStruct = jQuery.parseJSON(paResponseData);
//        if (lcRespondStruct.Success) {

//            lcItemGrid.find('[sa-elementtype=griditem]').remove();
//            lcItemGrid.scrollTop(0);

//            if (lcRespondStruct.ResponseData.GridItemList) {
//                $(lcRespondStruct.ResponseData.GridItemList).insertBefore(lcAjaxImageDiv);
//            }

//            lcItemGrid.attr('sa-chunkno', lcRespondStruct.ResponseData.ChunkNo);
//            lcItemGrid.attr('sa-chunkcount', lcRespondStruct.ResponseData.ChunkCount);
//            lcItemGrid.attr('sa-datacount', lcRespondStruct.ResponseData.DataCount);

//            lcItemGrid.RefreshStatus();

//            if (Number(lcRespondStruct.ResponseData.ChunkNo) >= Number(lcRespondStruct.ResponseData.ChunkCount)) lcAjaxImageDiv.hide();
//            else lcAjaxImageDiv.show();

//            lcOverlay.HideOptionPopUp();
//        }
//        else {

//        }
//    });
//}


