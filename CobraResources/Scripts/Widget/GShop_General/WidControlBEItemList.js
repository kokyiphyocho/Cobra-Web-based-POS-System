$(document).ready(function () {
    $('[sa-elementtype=control].WidControlBEItemList').BindBEItemListEvents();
});

$.fn.BindBEItemListEvents = function () {
    var lcForm = $(this).closest('[sa-elementtype=form]');
    
    lcForm.find('a[href="@cmd%editmode"]').unbind('click');
    lcForm.find('a[href="@cmd%editmode"]').click(function (paEvent) {
        paEvent.preventDefault();
        
        var lcControl = $('[sa-elementtype=control].WidControlBEItemList');
        var lcToolBar = $(this).closest('[sa-elementtype=toolbar]');
        var lcToggleState = lcControl.attr('fa-editmode');
        
        lcToolBar.find('[fa-active]').removeAttr('fa-active');

        if (!lcToggleState)
        {
            $(this).attr('fa-active', 'true');

            lcControl.removeAttr('fa-deletemode');
            lcControl.attr('fa-editmode', 'true');
        }
        else
        {
            lcControl.removeAttr('fa-editmode');
        }
    });

    lcForm.find('a[href="@cmd%deletemode"]').unbind('click');
    lcForm.find('a[href="@cmd%deletemode"]').click(function (paEvent) {
        paEvent.preventDefault();

        var lcControl = $('[sa-elementtype=control].WidControlBEItemList');
        var lcToolBar = $(this).closest('[sa-elementtype=toolbar]');
        var lcToggleState = lcControl.attr('fa-deletemode');

        lcToolBar.find('[fa-active]').removeAttr('fa-active');

        if (!lcToggleState) {
            $(this).attr('fa-active', 'true');

            lcControl.removeAttr('fa-editmode');
            lcControl.attr('fa-deletemode', 'true');
        }
        else
        {
            lcControl.removeAttr('fa-deletemode');
        }
    });


    lcForm.find('a[href="@cmd%add"]').unbind('click');
    lcForm.find('a[href="@cmd%add"]').click(function (paEvent) {
        paEvent.preventDefault();
        paEvent.stopPropagation();

        var lcForm = $(this).closest('[sa-elementtype=form]');
        var lcControl = lcForm.find('[sa-elementtype=control].WidControlBEItemList');
        var lcLinkTemplate = lcControl.attr('ea-template').split("<!!>")[1];
        var lcCategoryCode = lcControl.attr('ea-group');
                
        var lcToolBar = $(this).closest('[sa-elementtype=toolbar]');

        var lcLink = lcLinkTemplate.replace("$CATEGORYCODE", lcCategoryCode);

        if (lcLink) {
            var lcFormStack = lcForm.attr('ea-formstack');

            if (!lcFormStack) lcFormStack = Base64.decode(lcForm.attr('ea-encodedformname'));
            else lcFormStack = Base64.decode(lcFormStack) + '||' + Base64.decode(lcForm.attr('ea-encodedformname'));

            lcLink = "?_f=" + encodeURIComponent(Base64.encode(lcLink)) + '&_s=' + encodeURIComponent(Base64.encode(lcFormStack));
            RedirectPage(lcLink, false);
        }
    });

    $(this).find('img[href="@cmd%edititem"]').unbind('click');
    $(this).find('img[href="@cmd%edititem"]').click(function (paEvent) {
        paEvent.preventDefault();
        paEvent.stopPropagation();

        var lcForm          = $(this).closest('[sa-elementtype=form]');
        var lcControl       = $(this).closest('[sa-elementtype=control]');
        var lcItemBlock     = $(this).closest('[sa-elementtype=element]');        
        var lcLinkTemplate  = lcControl.attr('ea-template').split("<!!>")[0];
        var lcItemID        = lcItemBlock.attr('ea-dataid');
        
        var lcLink = lcLinkTemplate.replace("$ITEMID", lcItemID);

        if (lcLink) {
            var lcFormStack = lcForm.attr('ea-formstack');

            if (!lcFormStack) lcFormStack = Base64.decode(lcForm.attr('ea-encodedformname'));
            else lcFormStack = Base64.decode(lcFormStack) + '||' + Base64.decode(lcForm.attr('ea-encodedformname'));

            lcLink = "?_f=" + encodeURIComponent(Base64.encode(lcLink)) + '&_s=' + encodeURIComponent(Base64.encode(lcFormStack));
            RedirectPage(lcLink, false);
        }
    });


    $(this).find('img[href="@cmd%deleteitem"]').unbind('click');
    $(this).find('img[href="@cmd%deleteitem"]').click(function (paEvent) {
        paEvent.preventDefault();
        paEvent.stopPropagation();
                
        var lcControl = $(this).closest('[sa-elementtype=control]');
        var lcItemBlock = $(this).closest('[sa-elementtype=element]');        
        
        lcItemBlock.DeleteRecord();
       
    });

    $(this).find('img[href="@cmd%showchild"]').unbind('click');
    $(this).find('img[href="@cmd%showchild"]').click(function (paEvent) {
        paEvent.preventDefault();
        paEvent.stopPropagation();

        var lcForm = $(this).closest('[sa-elementtype=form]');
        var lcControl = $(this).closest('[sa-elementtype=control]');
        var lcItemBlock = $(this).closest('[sa-elementtype=element]');
        var lcItemName = lcItemBlock.find('.ItemName');
        var lcLinkTemplate = lcControl.attr('ea-template').split("<!!>")[2];
        var lcItemID = lcItemBlock.attr('ea-dataid');
        
        var lcLink = lcLinkTemplate.replace("$CATEGORYCODE", lcItemID).replace("$CATEGORYNAME", lcItemName.text());            
        
        if (lcLink) {
            var lcFormStack = lcForm.attr('ea-formstack');

            if (!lcFormStack) lcFormStack = Base64.decode(lcForm.attr('ea-encodedformname'));
            else lcFormStack = Base64.decode(lcFormStack) + '||' + Base64.decode(lcForm.attr('ea-encodedformname'));

            lcLink = "?_f=" + encodeURIComponent(Base64.encode(lcLink)) + '&_s=' + encodeURIComponent(Base64.encode(lcFormStack));
            RedirectPage(lcLink, false);
        }
    });
    

    //$(this).find('a[href="@cmd%payment"]').unbind('click');
    //$(this).find('a[href="@cmd%payment"]').click(function (paEvent) {
    //    paEvent.preventDefault();
    //    var lcForm = $(this).closest('[sa-elementtype=form]');
    //    var lcLink = $(this).attr('ea-parameter');

    //    if (lcLink) {
    //        var lcFormStack = lcForm.attr('ea-formstack');

    //        if (!lcFormStack) lcFormStack = lcForm.attr('ea-formname');
    //        else lcFormStack = lcFormStack + ';' + lcForm.attr('ea-formname');

    //        lcLink = "?_f=" + encodeURIComponent(Base64.encode(lcLink)) + '&_s=' + encodeURIComponent(Base64.encode(lcFormStack));
    //        RedirectPage(lcLink, false);
    //    }
    //});
}

$.fn.RunScalarQuery = function (paParameter, paDataBlock) {
    var lcForm = $(this).closest('[sa-elementtype=form]');
    var lcDeferred = $.Deferred();

    var lcData = { CobraAjaxRequest: "executescalarquery", Parameter: paParameter.QueryName, DataBlock: paDataBlock };
        
    DoPostBack(lcData, function (paResponseData) {
        var lcRespondStruct = jQuery.parseJSON(paResponseData);
        if (lcRespondStruct.Success) {
            var lcResponse = lcRespondStruct.ResponseData.RSP_Result;
            lcDeferred.resolve(lcResponse);
        }
        else {
                lcForm.ShowFormMessage(paParameter.ActionFailMessageIndex, paParameter.DefaultActionFailMessage, false).done(function () {
                    lcDeferred.resolve('FAIL');
            });
        }        
    });

    return (lcDeferred);
}

$.fn.DeleteRecord = function()
{    
    var lcItemBlock = $(this);
    var lcForm      = $(this).closest('[sa-elementtype=form]');

    var lcItemID    = lcItemBlock.attr('ea-dataid');
    var lcType      = lcItemBlock.attr('ea-type');

    $(this).CheckChildItemCount().done(function (paResult) {
        if (paResult)
        {
            lcForm.ShowFormMessage(0, "Are you sure you want to delete ?%%Yes%%No").then(function (paResult, paControl) {
                if ((paResult) && (paResult.toLowerCase() == "yes")) {
                    var lcDataBlock = { __ItemID: lcItemID };

                    var lcParameter = {
                        QueryName: 'BEItemList.DeleteItem',
                        ActionFailMessageIndex: 1,
                        DefaultActionFailMessage: "Unable to Delete Item.",
                    };

                    lcItemBlock.RunScalarQuery(lcParameter, Base64.encode(JSON.stringify(lcDataBlock))).done(function (paStatus) {
                        if (paStatus == 'CANCEL') {
                            lcItemBlock.remove();
                        }
                        else lcForm.ShowFormMessage(1, "Unable to Delete Item.");
                    });
                }
            });
        }
        else 
        {
            lcForm.ShowFormMessage(2, "Please Delete the Child Items First.");
        }
    });
}

$.fn.CheckChildItemCount = function()
{
    var lcDeferred = $.Deferred();
    var lcItemBlock = $(this);

    var lcItemID = lcItemBlock.attr('ea-dataid');
    var lcType = lcItemBlock.attr('ea-type');
    
    if (lcType.toLowerCase() == 'category') {        
        var lcDataBlock = { __CategoryID: lcItemID };

        var lcParameter = {
            QueryName: 'BEItemList.GetChildItemCount',
            ActionFailMessageIndex: 1,
            DefaultActionFailMessage: "Unable to Delete Item.",
        };

        lcItemBlock.RunScalarQuery(lcParameter, Base64.encode(JSON.stringify(lcDataBlock))).done(function (paCount) {
            if (Number(paCount) == 0) {
                lcDeferred.resolve(true);
            }
            else lcDeferred.resolve(false);
        });
    }
    else lcDeferred.resolve(true);

    return (lcDeferred);
}