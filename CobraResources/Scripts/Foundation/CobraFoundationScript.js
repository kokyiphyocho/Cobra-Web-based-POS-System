String.prototype.trim = function () {    
    return this.replace('\0','').replace(/(^\s*)|(\s*$)/g, '');
};

String.prototype.ltrim = function () {
    return this.replace('\0', '').replace(/^\s*/g, '');
};

String.prototype.rtrim = function () {
    return this.replace('\0', '').replace(/\s*$/g, '');
};

function ConvertToThousandSeparatorStr(paValue) {    
    paValue = paValue || 0;

    var lcNumParts = paValue.toString().split(".");
    lcNumParts[0] = lcNumParts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return lcNumParts.join(".");
};

function CastInteger(paValue, paDefaultValue)
{
    return(isNaN(paValue) ? (paDefaultValue || 0) : parseInt(Math.round(Number(paValue))));
}

function CastDecimal(paValue, paDefaultValue)
{
    return (isNaN(paValue) ? (paDefaultValue || 0) : Number(paValue));
}

function CastIntArray(paArrayStr, paDefaultValue) {
    lcArray = (paArrayStr.length === 0) ? new Array() : paArrayStr.replace(/, +/g, ",").split(",").map(Number);
    
    for (lcCount = 0; lcCount < lcArray.length; lcCount++)
        lcArray[lcCount] = isNaN(lcArray[lcCount]) ? paDefaultValue || 0 : lcArray[lcCount];

    return (lcArray);
}

//String.prototype.ToUnicode = function() {
//    var lcUnicodeString = '';

//    for (var i = 0; i < this.length; i++) {
//        var lcUnicode = this.charCodeAt(i).toString(16).toUpperCase();
//        while (lcUnicode.length < 4) {
//            lcUnicode = '0' + lcUnicode;
//        }
//        lcUnicode = '\\u' + lcUnicode;
//        lcUnicodeString += lcUnicode;
//    }
//    return lcUnicodeString;
//}

String.prototype.StartWith = function(paSubstring)
{    
    return (this.indexOf(paSubstring) === 0);    
}

String.prototype.NormalizeNumber = function()
{
    return (FormManager.NormalizeNumber(this));
}

String.prototype.ToLocalNumber = function ()
{
    return (FormManager.ToLocalNumber(this));
}

String.prototype.ConvertToFormLanguage = function () {
    return (FormManager.ConvertToFormLanguage(this));
}

//String.prototype.NormalizeNumber = function()
//{
//    lcForm        = FormManager.GetForm();
//    lcLocalDigits = lcForm.attr('ea-localdigits');
        
//    var lcStrArray = this.split('');
    
//    if (lcLocalDigits && lcLocalDigits.length == 10) {
//        for (i = 0; i < lcStrArray.length; i++) {
//            var lcIndex = lcLocalDigits.indexOf(lcStrArray[i]);
//            if (lcIndex >= 0) lcStrArray[i] = lcIndex.toString();
//        }
//    }
    
//    return lcStrArray.join('');    
//}

//String.prototype.ToLocalNumber = function () {
//    lcForm = FormManager.GetForm();
//    lcLocalDigits = lcForm.attr('ea-localdigits');

//    var lcEnglishDigit = '0123456789';
    
//    var lcStrArray = this.split('');

//    if (lcLocalDigits && lcLocalDigits.length == 10) {
//        for (i = 0; i < lcStrArray.length; i++) {
//            var lcIndex = lcEnglishDigit.indexOf(lcStrArray[i]);
//            if (lcIndex >= 0) lcStrArray[i] = lcLocalDigits[lcIndex];
//        }
//    }

//    return lcStrArray.join('');
//}

String.prototype.ParseMomentDate = function()
{    
    if (moment) {
        var lcValue = this.NormalizeNumber().trim();        
        return (moment(lcValue, ['DD/MM/YYYY', 'D/M/YYYY', 'D/M/YY', 'DD/MM/YY', 'DD-MM-YYYY', 'D-M-YYYY', 'D-M-YY', 'DD-MM-YY','YYYY-MM-DD'], true));
    }
    else return (null);
}

String.prototype.ParseMomentDateTime = function () {
    if (moment) {
        var lcValue = this.NormalizeNumber().trim();
        return (moment(lcValue, ['YYYY-MM-DDTHH:mm:ss',
                                 'YYYY-MM-DD HH:mm:ss',
                                 'DD/MM/YYYY HH:mm:ss',                                 
                                 'D/M/YYYY HH:mm:ss'], true));
    }
    else return (null);
}

//String.prototype.ConvertToFormLanguage = function (paString)
//{    
//    lcForm          = FormManager.GetForm();
//    lcLanguage      = lcForm.attr('ea-language');    
//    lcNumberMode    = lcForm.attr('ea-numbermode');

//    if ((paString == 0) || (paString))
//    {
//        if (lcNumberMode == 'local') return (paString.toString().ToLocalNumber());
//        else return (paString.toString().NormalizeNumber());
//    }
//    return (paString);
//}

String.prototype.ForceConvertToInteger = function (paValueIfError) {
    var lcValue = this.NormalizeNumber();
    
    if (!paValueIfError) paValueIfError = 0;
    var lcValue = isNaN(lcValue) || lcValue.trim() == '' ? paValueIfError : parseInt(lcValue);
    
    return (lcValue);
}

String.prototype.ForceConvertToDecimal = function(paValueIfError)
{
    var lcValue = this.NormalizeNumber();
    
    if (!paValueIfError) paValueIfError = 0;
    var lcValue = isNaN(lcValue) || lcValue.trim() == '' ? paValueIfError : Number(lcValue);
    
    return (lcValue);
}

function PadChar(paString, paPadChar, paLength) {    
    var lcPadStr = new Array(paLength).join(paPadChar);
    return (lcPadStr + paString).slice(-paLength);
}

function InvertColor(paHex, paBlackAndWhite) {
    if (paHex.indexOf('#') === 0) {
        paHex = paHex.slice(1);
    }
    // convert 3-digit hex to 6-digits.
    if (paHex.length === 3) {
        paHex = paHex[0] + paHex[0] + paHex[1] + paHex[1] + paHex[2] + paHex[2];
    }

    var r = parseInt(paHex.slice(0, 2), 16),
        g = parseInt(paHex.slice(2, 4), 16),
        b = parseInt(paHex.slice(4, 6), 16);

    if (paBlackAndWhite) {     
        return (r * 0.299 + g * 0.587 + b * 0.114) > 186
            ? '#000000'
            : '#FFFFFF';
    }
    // invert color components
    r = (255 - r).toString(16);
    g = (255 - g).toString(16);
    b = (255 - b).toString(16);
    // pad each with zeros and return
    return "#" + PadChar(r, '0', 2) + PadChar(g, '0', 2) + PadChar(b, '0', 2);
}

jQuery.fn.reverse = [].reverse;

function GetWebState(paAddPrefix) {
    if ($('#__WEBSTATE').length > 0) {
        var lcWebState = $('#__WEBSTATE').val();
        lcWebState = lcWebState.substring(0, 4) + encodeURIComponent(lcWebState.substring(4));
        return ((paAddPrefix == true ? '&' : '') + lcWebState);
    }
    else return ('');
}

function AppendPostBackWebState(paData) {
    if ($('#__WEBSTATE').length > 0) {
        var lcWebState = $('#__WEBSTATE').val();
        paData[lcWebState.substring(0, 3)] = lcWebState.substring(4);
    }    
}

function AppendFormPostBackWebState(paFormData) {
    if ($('#__WEBSTATE').length > 0) {
        var lcWebState = $('#__WEBSTATE').val();
        paFormData.append(lcWebState.substring(0, 3),lcWebState.substring(4));
    }
}

function DoPostBack(paData, paResponseHandler) {    
    if (paData instanceof FormData) AppendFormPostBackWebState(paData);
    else AppendPostBackWebState(paData);

    if (paData instanceof FormData)
    {        
        $.ajax({
            url: document.URL,
            data: paData,
            cache: false,
            contentType: false,
            processData: false,
            type: 'POST',
            success: paResponseHandler
        });
    }
    else $.post(document.URL, paData, paResponseHandler);
        
}

function DoPostBackReadMode(paData, paResponseHandler) {    
        if (paData instanceof FormData) AppendFormPostBackWebState(paData);
        else AppendPostBackWebState(paData);
        $.post(document.URL, paData, paResponseHandler);    
}

function DoFormPostBack(paData, paResponseHandler) {
    $.post(document.URL, paData, paResponseHandler);
}

//function IsDemoMode()
//{
//    if ($(document).find('[sa-elementtype=form][ea-demomode]').length > 0) {
//        ShowInfoMessage("Unable to Perform Update Operations in Demo Mode.");
//        return (true);
//    }
//    else return (false);
//}

$.fn.ForceIPAddressInput = function () {
    $(this).ForceFilterInput('0123456789၀၁၂၃၄၅၆၇၈၉.', '');
}

$.fn.ForceLogInInput = function () {
    $(this).ForceFilterInput('0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ~!@#$%^&*()_+|}{[]\\?><,./;:-=');
}

$.fn.ForceDecimalInput = function () {
    $(this).ForceFilterInput('0123456789၀၁၂၃၄၅၆၇၈၉.','.');
}
$.fn.ForceIntegerInput = function () {    
    $(this).ForceFilterInput('0123456789၀၁၂၃၄၅၆၇၈၉');    
}

$.fn.ForcePhoneNoInput = function () {
    $(this).ForceFilterInput('0123456789၀၁၂၃၄၅၆၇၈၉+-(),၊ ');
}

$.fn.ForceSignedIntegerInput = function () {
    $(this).ForceFilterInput('0123456789၀၁၂၃၄၅၆၇၈၉-');
}

$.fn.ForceSignedDecimalInput = function () {
    $(this).ForceFilterInput('0123456789၀၁၂၃၄၅၆၇၈၉.-', '.');
}

$.fn.ForceDateInput = function () {
    $(this).ForceFilterInput('၀၁၂၃၄၅၆၇၈၉/-0123456789/-');
}

$.fn.ForceFilterInput = function (paInputChar, paOneTimeChar) {
    //$(this).unbind('keypress keyup');
    //$(this).on("keypress keyup", function (paEvent) {        
    $(this).unbind('keypress');
    $(this).on("keypress", function (paEvent) {        
        var lcKeyCode = paEvent.which || paEvent.keyCode;
        if (paInputChar) {
            if ((paInputChar.indexOf(String.fromCharCode(lcKeyCode)) == -1)) {
                paEvent.preventDefault(); 
               return;
            }            
        }

        if (paOneTimeChar) {                    
            if ((paOneTimeChar.indexOf(String.fromCharCode(lcKeyCode)) != -1) && ($(this).val().indexOf(String.fromCharCode(lcKeyCode)) != -1))
            {
                paEvent.preventDefault();
                return;
            }            
        }
    });
}

$.fn.ForceNumberBoundLimit = function()
{
    this.unbind('blur');
    this.on('blur', function (paEvent) {
        var lcLowerBound = CastDecimal($(this).attr('ea-lowerbound'), 0);
        var lcUpperBound = CastDecimal($(this).attr('ea-upperbound'), 0);
        var lcValue = CastDecimal($(this).val().NormalizeNumber(), 0);
                
        if (lcLowerBound < lcUpperBound) {
            if (lcValue < lcLowerBound) $(this).val(lcLowerBound.toString().ConvertToFormLanguage());
            else if (lcValue > lcUpperBound) $(this).val(lcUpperBound.toString().ConvertToFormLanguage());
        }
    });    
}


$.fn.IsImageOk = function () {
    var lcImage = $(this)[0];
    if (!lcImage.complete) { return false; }
    if (typeof lcImage.naturalWidth != "undefined" && lcImage.naturalWidth == 0) { return false; }

    return true;
}

//$.fn.VerifyDateInputMode = function ()
//{
    
//    $(this).unbind('blur', HandlerOnDateBlur);
//    $(this).bind('blur', HandlerOnDateBlur);
//}

//function HandlerOnDateBlur(paEvent)
//{    
//    if (moment)
//    {
//        var lcValue = $(this).val().NormalizeNumber().trim();

//        if ((lcValue.length > 0) && (moment(lcValue, $(this).attr('ea-dateformat') || 'D/M/YYYY', true).isValid())) {
//            $(this).removeAttr('fa-dateformaterror');
//        }
//        else {
//            $(this).attr('fa-dateformaterror','true');
//        }
//    }
//}

$.fn.ShowControlMessage = function (paMessageIndex, paDefaultMessage, paFocusAfterAction) {
    var lcErrorMessage = $(this).attr('ea-messages');
    if (lcErrorMessage) lcErrorMessage = lcErrorMessage.split(';;')[paMessageIndex];
    if (paFocusAfterAction) ShowErrorMessage(lcErrorMessage || paDefaultMessage, $(this));
    else ShowErrorMessage(lcErrorMessage || paDefaultMessage);
}

$.fn.GetFormMessage = function(paMessageIndex)
{
    var lcMessage = $(this).attr('ea-messages');
    if (lcMessage) lcMessage = lcMessage.split(';;')[paMessageIndex];

    return (lcMessage);
}

$.fn.GetFormMessageEx = function (paMessageCode) {
    var lcMessage = $(this).attr('ea-messages');
    if ((lcMessage) && (paMessageCode)) {
        var lcIndex = lcMessage.indexOf(paMessageCode + "::");
        if (lcIndex != -1) {
            lcIndex += paMessageCode.length + 2;
            lcMessage = lcMessage.substring(lcIndex);
            if (lcMessage.length > 0) return (lcMessage.split(';;')[0]);
        }
        return (null);
    }
}

$.fn.ShowFormMessage = function (paMessageIndex, paDefaultMessage, paError) {
    var lcMessage = $(this).attr('ea-messages');
    if (lcMessage) lcMessage = lcMessage.split(';;')[paMessageIndex];
    lcMessage = lcMessage || paDefaultMessage;
    
    if (lcMessage.indexOf('%%') == -1) {
        if (paError)  return (ShowErrorMessage(lcMessage));        
        else return (ShowInfoMessage(lcMessage));
    }
    else {
        var lcMessageComponents = lcMessage.split('%%');
        return (ShowConfirmationMessage(lcMessageComponents[0], lcMessageComponents[1], lcMessageComponents[2]));
    }
}

$.fn.ShowFormMessageEx = function (paMessageCode, paDefaultMessage, paError) {
    var lcMessage = $(this).GetFormMessageEx(paMessageCode);

    if (lcMessage == null) lcMessage = paDefaultMessage;

    if (lcMessage.indexOf('%%') == -1) {
        if (paError) return (ShowErrorMessage(lcMessage));
        else return (ShowInfoMessage(lcMessage));
    }
    else {
        var lcMessageComponents = lcMessage.split('%%');
        return (ShowConfirmationMessage(lcMessageComponents[0], lcMessageComponents[1], lcMessageComponents[2]));
    }
}

$.fn.ShowAjaxErrorMessage = function (paMessageIndex, paDefaultMessage) {
    var lcMessage = $(this).attr('ea-globalajaxerrormessages');
    if (lcMessage) lcMessage = lcMessage.split(';;')[paMessageIndex];
    lcMessage = lcMessage || paDefaultMessage;
   
    ShowErrorMessage(lcMessage);        
}

function GetUrlParameter(paParamName) {
    paParamName = paParamName.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
    var paRegex = new RegExp('[\\?&]' + paParamName + '=([^&#]*)');
    var paResults = paRegex.exec(location.search);
    return paResults === null ? '' : decodeURIComponent(paResults[1].replace(/\+/g, ' '));
};

//$(".allownumericwithdecimal").on("keypress keyup blur",function (event) {
//            //this.value = this.value.replace(/[^0-9\.]/g,'');
//     $(this).val($(this).val().replace(/[^0-9\.]/g,''));
//            if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
//                event.preventDefault();
//            }
//        });

// $(".allownumericwithoutdecimal").on("keypress keyup blur",function (event) {    
//           $(this).val($(this).val().replace(/[^\d].+/, ""));
//            if ((event.which < 48 || event.which > 57)) {
//                event.preventDefault();
//            }
//        });



