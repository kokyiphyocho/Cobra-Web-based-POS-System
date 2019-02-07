$(document).ready(function () {    
    $('[sa-elementtype=control].WidControlMobileStoreFront').BindMobileStoreFrontEvents();    
});

$.fn.BindMobileStoreFrontEvents = function () {
    var clLastScrollYPosition = $('[sa-elementtype=container]').scrollTop();

    $(this).find('a[href="@cmd%closeform"]').unbind('click');
    $(this).find('a[href="@cmd%closeform"]').click(function (paEvent) {
        paEvent.preventDefault();

        lcForm = $(this).closest('[sa-elementtype=form]');

        lcForm.CloseForm();
    });

    $(this).find('a[href="@cmd%openfilterpopup"]').unbind('click');
    $(this).find('a[href="@cmd%openfilterpopup"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcControl = $(this).closest('[sa-elementtype=control]');

        lcControl.attr('fa-showpopup', 'filter');

    });

    $(this).find('a[href="@cmd%opensortpopup"]').unbind('click');
    $(this).find('a[href="@cmd%opensortpopup"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcControl = $(this).closest('[sa-elementtype=control]');

        lcControl.attr('fa-showpopup', 'sort')

    });

    $(this).find('a[href="@cmd%openstoreinfopopup"]').unbind('click');
    $(this).find('a[href="@cmd%openstoreinfopopup"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcControl = $(this).closest('[sa-elementtype=control]');

        
        lcControl.attr('fa-showpopup', 'storeinfo')

    });

    $(this).find('[sa-elementtype=overlay]').on('touchmove', function (paEvent) {                 
        if (!$(this).has($(paEvent.target)).length)
            paEvent.preventDefault();
    });

    $('[sa-elementtype=container]').unbind('scroll');
    $('[sa-elementtype=container]').scroll(function () {
        var lcCurrentY = $(this).scrollTop();

        if (lcCurrentY > clLastScrollYPosition) {
            if ($(this).scrollTop() + $(this).height() >= $(this)[0].scrollHeight - 1000) {
                var lcGrid = $(this).find('[sa-elementtype=grid]');
                lcGrid.LazyLoadGridItems();
            }         
        }        
        clLastScrollYPosition = lcCurrentY;
    });

    $(this).find('a[href="@cmd%searchkeyword"]').unbind('click');
    $(this).find('a[href="@cmd%searchkeyword"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcControl = $(this).closest('[sa-elementtype=control]');
        var lcItemGrid = lcControl.find('[sa-elementtype=grid]');
        var lcSearchBox = $(this).closest('[sa-elementtype=searchbox]');
        var lcInputBox = lcSearchBox.find('input[type=text]');

        if (lcInputBox.val().length > 0) {            
            var lcSearchFilter = { Search: lcInputBox.val() };            
            lcControl.attr('fa-filteroption', encodeURIComponent(Base64.encode(JSON.stringify(lcSearchFilter))));
            lcItemGrid.ReloadGridItems();            
        }
    });  
}

$.fn.RefreshStatus = function()
{
    var lcItemGrid = $(this);
    var lcForm = lcItemGrid.closest('[sa-elementtype=form]');
    var lcStatusBar = lcForm.find('[sa-elementtype=statusbar]');
    var lcStatusTextTemplate = lcStatusBar.attr('ea-statustext')
    var lcDataCount = Number(lcItemGrid.attr('ea-totalrows'));
  
    if (lcDataCount) {
        if (lcDataCount <= 0) lcStatusBar.text(lcStatusTextTemplate.replace('$DATACOUNT', 'No'));
        else lcStatusBar.text(lcStatusTextTemplate.replace('$DATACOUNT', lcDataCount));
    }
    else lcStatusBar.text('');
}


//$.fn.CompileFilterTemplate = function()
//{
//    var lcFilterTemplate = $(this).attr('sa-filtertemplate');
//    var lcInputBox = $(this).find('input[type=text]');
//    var lcKeyword = lcInputBox.val();

//    var lcArray = lcKeyword.split(/\s+/, 5);
//    var lcFinalArray = [];
//    var lcFilterObject = {};
//    var lcMaxFilterCount = 9;
        
//    lcFilterObject["FilterOperator"] = 'Or';

//    $.each(lcArray, function (paIndex, paValue) {
//        lcFinalArray.push(lcFilterTemplate.replace('$FILTERPARAM', paValue));
//    });

//    $.each(lcFinalArray, function (paIndex, paValue) {
//        lcFilterObject["Filter" + ((lcMaxFilterCount - paIndex) <= 0 ? '' : (lcMaxFilterCount - paIndex))] = paValue;
//    });

//    return (lcFilterObject);
//}

//$.fn.SearchKeyWord = function()
//{
//    var lcForm = $(this).closest('[sa-elementtype=form]');
//    var lcItemGrid = lcForm.find('[sa-elementtype=container]');
//    var lcOption = lcForm.LoadFilterOption(true);
//    var lcControlParam = lcItemGrid.attr('sa-controlparam');    
//    var lcChunkNoParamName = lcItemGrid.attr('sa-chunknoparamname');
//    var lcAjaxImageDiv = lcItemGrid.find('[sa-elementtype=ajaxloaderelement]');
//    var lcInputBox = $(this).find('input[type=text]');    
//    var lcKeyword = lcInputBox.val();
    
//    if ((lcKeyword) && (lcKeyword.length > 0))
//    {
//        lcForm.attr('fa-filtermode', 'search');

//        lcForm.ResetFilterSetting();
                
//        var lcData = { SmartAjaxRequest: "lazyloaditemgrid", ControlParam: lcControlParam };
//        lcData[lcChunkNoParamName] = 1;        
//        lcData = $.extend(lcData, $(this).CompileFilterTemplate());
//        lcData = $.extend(lcData, lcOption);

//        DoPostBack(lcData, function (paResponseData) {            
//            var lcRespondStruct = jQuery.parseJSON(paResponseData);
//            if (lcRespondStruct.Success) {

//                lcItemGrid.find('[sa-elementtype=griditem]').remove();
//                lcItemGrid.scrollTop(0);

//                if (lcRespondStruct.ResponseData.GridItemList) {
//                    $(lcRespondStruct.ResponseData.GridItemList).insertBefore(lcAjaxImageDiv);
//                }

//                lcItemGrid.attr('sa-chunkno', lcRespondStruct.ResponseData.ChunkNo);
//                lcItemGrid.attr('sa-chunkcount', lcRespondStruct.ResponseData.ChunkCount);
//                lcItemGrid.attr('sa-datacount', lcRespondStruct.ResponseData.DataCount);

//                lcItemGrid.RefreshStatus();

//                if (Number(lcRespondStruct.ResponseData.ChunkNo) >= Number(lcRespondStruct.ResponseData.ChunkCount)) lcAjaxImageDiv.hide();
//                else lcAjaxImageDiv.show();
//            }
//            else {

//            }
//        });
//    }
//}


//DECLARE 
//    @PageSize INT = 10, 
//    @PageNum  INT = 1;

//WITH TempResult AS(
//    SELECT *
//    FROM EData_MobilePhoneCatalogue where Manufacturer = 'APPLE'
//), TempCount AS (
//    SELECT COUNT(*) AS rowscount FROM TempResult
//)
//SELECT *
//FROM TempResult, TempCount
//ORDER BY TempResult.EntryID
//OFFSET (@PageNum-1)*@PageSize ROWS
//FETCH NEXT @PageSize ROWS ONLY

$.fn.LazyLoadGridItems = function () {
    var lcItemGrid = $(this);
    var lcControl = lcItemGrid.closest('[sa-elementtype=control]');
    var lcType = lcControl.attr('ea-type');
    var lcPageIndex = Number(lcItemGrid.attr('ea-pageindex'));
    var lcTotalPages = Number(lcItemGrid.attr('ea-totalpages'));
    var lcFilterOption = lcControl.attr('fa-filteroption');
    var lcSortOption = lcControl.attr('fa-sortoption');
    
    lcPageIndex = lcPageIndex + 1;
    
    if ((lcPageIndex < lcTotalPages) && (lcControl.attr('fa-lazyloading') != 'true')) {
        lcControl.attr('fa-lazyloading', 'true');
        lcControl.removeAttr('fa-ajaxerror');

        var lcData = {
            CobraAjaxRequest: "lazyloaditemlist", Parameter: lcType,
            GridPageIndex: lcPageIndex, GridFilterInfo: lcFilterOption, GridSortInfo : lcSortOption
        };

        GlobalAjaxHandler.SetAjaxLoaderSuppressState(true);
        GlobalAjaxHandler.SetAjaxErrorSuppressState(true);
        GlobalAjaxHandler.SetAjaxErrorHandler(function ()
        {
            lcControl.attr('fa-ajaxerror', 'true'); lcControl
            lcControl.removeAttr('fa-lazyloading');
        });

        DoPostBackReadMode(lcData, function (paResponseData) {
            var lcRespondStruct = jQuery.parseJSON(paResponseData);

            if (lcRespondStruct.Success) {
                if (lcRespondStruct.ResponseData.RSP_HTML) {
                    if (lcRespondStruct.ResponseData.FetchedRows > 0) {
                        lcItemGrid.append(lcRespondStruct.ResponseData.RSP_HTML);
                        lcItemGrid.attr('ea-pageindex', lcRespondStruct.ResponseData.PageIndex);
                        lcItemGrid.attr('ea-totalpages', lcRespondStruct.ResponseData.TotalPages);
                        lcItemGrid.attr('ea-totalrows', lcRespondStruct.ResponseData.TotalRows);

                        lcItemGrid.BindMobileStoreFrontGridItemEvents();
                    }
                }
            }
            else
            {                
                lcControl.attr('fa-ajaxerror','true');
            }

            lcControl.removeAttr('fa-lazyloading');                   
        });
    }
}


$.fn.ReloadGridItems = function () {
    var lcItemGrid = $(this);
    var lcControl = lcItemGrid.closest('[sa-elementtype=control]');
    var lcForm = lcControl.closest('[sa-elementtype=form]');
    var lcContainer = lcItemGrid.closest('[sa-elementtype=container]');
    var lcType = lcControl.attr('ea-type');        
    var lcFilterOption = lcControl.attr('fa-filteroption');
    var lcSortOption = lcControl.attr('fa-sortoption');
    
    if (lcControl.attr('fa-lazyloading') != 'true') {
        var lcData = {
            CobraAjaxRequest: "lazyloaditemlist", Parameter: lcType,
            GridPageIndex: 0, GridFilterInfo: lcFilterOption, GridSortInfo: lcSortOption
        };

        GlobalAjaxHandler.SetAjaxLoaderSuppressState(false);
        GlobalAjaxHandler.SetAjaxErrorSuppressState(false);
        
        DoPostBackReadMode(lcData, function (paResponseData) {
            var lcRespondStruct = jQuery.parseJSON(paResponseData);

            if (lcRespondStruct.Success) {
                if (lcRespondStruct.ResponseData.RSP_HTML) {
                    lcItemGrid.empty();
                    lcItemGrid.append(lcRespondStruct.ResponseData.RSP_HTML);
                    lcItemGrid.attr('ea-pageindex', lcRespondStruct.ResponseData.PageIndex);
                    lcItemGrid.attr('ea-totalpages', lcRespondStruct.ResponseData.TotalPages);
                    lcItemGrid.attr('ea-totalrows', lcRespondStruct.ResponseData.TotalRows);

                    if (lcRespondStruct.ResponseData.FetchedRows > 0) {
                        lcItemGrid.BindMobileStoreFrontGridItemEvents();
                    }

                    lcItemGrid.RefreshStatus();
                    lcContainer.scrollTop(0);
                }
            }
            else {
                lcForm.ShowFormMessage(0, "Unable to Receive Information from Server", true);
            
            }

            //lcControl.removeAttr('fa-lazyloading');
        });
    }
}

