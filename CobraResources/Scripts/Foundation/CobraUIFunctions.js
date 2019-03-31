$(document).ready(function () {    
    GeoLocationManager.RefreshGeoLocation();
    MessageHandler.Init();
    FormManager.Init();    
    GlobalAjaxHandler.SetDefaultAjaxLoaderElement();
    jQuery.ajaxSetup(
        {
            async: true,
            beforeSend: function () { GlobalAjaxHandler.DisplayAjaxLoaderImage(); },
            complete: function () { GlobalAjaxHandler.HideAjaxLoaderImage(); },
            error:function() 
            {
                // SERIOUS THREAD SAFETY ISSUE.                
                if (!GlobalAjaxHandler.GetAjaxErrorSuppressState()) {
                    MessageHandler.ShowMessage('err_connectionfail');
                }
                if (GlobalAjaxHandler.GetAjaxErrorHandler()) GlobalAjaxHandler.GetAjaxErrorHandler()();
                GlobalAjaxHandler.SetAjaxErrorHandler(); // Remove Ajax Error Handler.
                GlobalAjaxHandler.SetAjaxErrorSuppressState(false);
            }
        });
});


var GeoLocationManager = (function () {
    return {            
            GeoLocationReader : function (paLatitudeControl, paLongitudeControl) {

                if (!navigator.geolocation) {        
                    Messagehandler.ShowMessage('err_geolocationnotsupported');
                    return; 
                }

                function SuccessHandler(paPosition) {
                    var lcLatitude = paPosition.coords.latitude;
                    var lcLongitude = paPosition.coords.longitude;

                    if (paLatitudeControl) paLatitudeControl.val(lcLatitude);
                    if (paLongitudeControl) paLongitudeControl.val(lcLongitude);
                }

                function ErrorHandler() {
                    Messagehandler.ShowMessage('err_failgeolocation');
                }

                navigator.geolocation.getCurrentPosition(SuccessHandler, ErrorHandler);
            },

            RefreshGeoLocation: function () {
                if (!navigator.geolocation) {
                    window.__SYSVAR_CurrentGeoLocation = '';
                }

                function SuccessHandler(paPosition) {
                    var lcLatitude = paPosition.coords.latitude;
                    var lcLongitude = paPosition.coords.longitude;
                    window.__SYSVAR_CurrentGeoLocation = 'GEOLOCATION:' + lcLatitude + ', ' + lcLongitude + ';';
                }

                function ErrorHandler() {
                    window.__SYSVAR_CurrentGeoLocation = '';
                }

                navigator.geolocation.getCurrentPosition(SuccessHandler, ErrorHandler);
            }
}})();

var FormManager = (function () {
    var clForm;
    var clPasswordDiv;
    var clLocalDigits;
    var clSystemDigits = '0123456789';
    var clLocalNumberMode;
    var clFormProtocolList;

    return {
                Init: function () {
                    clForm        = $(document).find('[sa-elementtype=form]');
                    clPasswordDiv = clForm.find('[sa-elementtype=container].PasswordDiv');
                    clLocalDigits = clForm.attr('ea-localdigits');
                    clLocalNumberMode = clForm.attr('ea-localnumbermode');
                    clFormProtocolList = JSON.parse(Base64.decode(clForm.attr('ea-formprotocollist') || 'e30='));                   
                    
                    FormManager.BindFormEvents();
                    FormManager.InitializeMessageControls();
                },
                InitializeMessageControls : function()
                {
                    var lcMessageDisplayList = clForm.find('[sa-elementtype=messagedisplay][ea-messagecode],[sa-elementtype=messagebar][ea-messagecode]');
                    
                    lcMessageDisplayList.each(function () {
                        var lcControl = $(this);
                        var lcMessageCode = lcControl.attr('ea-messagecode');

                        if (lcMessageCode) 
                        {
                            lcControl.text(MessageHandler.GetMessage(lcMessageCode));
                        }
                    });
                },
                GetForm : function()
                {
                    if (!clForm) clForm = $(document).find('[sa-elementtype=form]');
                    return (clForm);
                },
                NormalizeNumber: function (paString)
                {
                    if ((paString == 0) || (paString)) {

                        var lcStrArray = paString.split('');
                       
                        if (clLocalDigits && clLocalDigits.length == 10) {
                            for (i = 0; i < lcStrArray.length; i++) {
                                var lcIndex = clLocalDigits.indexOf(lcStrArray[i]);
                                if (lcIndex >= 0) lcStrArray[i] = lcIndex.toString();
                            }
                        }
                        return lcStrArray.join('');
                    }
                    else return ('');
                },
                ToLocalNumber: function (paString)
                {
                    if ((paString == 0) || (paString)) {
                        var lcStrArray = paString.split('');
                        
                        if (clLocalDigits && clLocalDigits.length == 10) {
                            for (i = 0; i < lcStrArray.length; i++) {                                
                                var lcIndex = clSystemDigits.indexOf(lcStrArray[i]);
                                if (lcIndex >= 0) lcStrArray[i] = clLocalDigits[lcIndex];
                            }
                        }
                        return(lcStrArray.join(''));
                    }
                    return(lcStrArray.join(''));
                },
                ConvertToFormLanguage : function(paString)
                {
                    if ((paString) || (paString == 0)) {
                        paString = paString.toString();
                        if (clLocalNumberMode == 'true') return (FormManager.ToLocalNumber(paString));
                        else return (FormManager.NormalizeNumber(paString));
                    }
                    else return ('');                    
                },
                LogOutSystem : function ()
                {                    
                    var lcLandingPage = Base64.decode(clForm.attr('ea-landingpage'));

                    var lcAjaxRequestManager = new AjaxRequestManager('logoutsystem', 'info_successlogout', 'info_faillogout', 'ajax_loading');

                    lcAjaxRequestManager.SetCompleteHandler(function (paSuccess, paResponseStruct) {
                        if (paSuccess) {
                            window.location.href = lcLandingPage;
                        }
                    });

                    lcAjaxRequestManager.Execute();
                },                
                VerifyPassword : function()
                {                    
                    var lcStatusDiv         = clPasswordDiv.find('[sa-elementtype=statuscontrol]');
                    var lcPasswordInputBox  = clPasswordDiv.find('input[type=password]');
                    var lcPassword          = lcPasswordInputBox.val();
                            
                    if (lcPassword.length == 0) lcStatusDiv.text(MessageHandler.GetMessage('err_pincodemissing'));
                    else if ((lcPassword.length < 3) || (lcPassword.length > 15)) lcStatusDiv.text(MessageHandler.GetMessage('err_invalidpincode'));
                    else {
                            lcStatusDiv.text('');
                            GlobalAjaxHandler.SetAjaxLoaderSuppressState(true);
                            GlobalAjaxHandler.SetAjaxErrorHandler(function () { clPasswordDiv.removeAttr('fa-ajaxrunning'); });

                            clPasswordDiv.attr('fa-ajaxrunning', 'true');

                            var lcAjaxRequestManager = new AjaxRequestManager('verifypassword', null, 'err_invalidpincode', null);
                            lcAjaxRequestManager.AddAjaxParam('Parameter', $.md5(lcPassword));

                            lcAjaxRequestManager.SetCompleteHandler(function (paSuccess, paResponseStruct) {
                                if (paSuccess) {
                                    SecurityController.Resolve(true);
                                }

                                clPasswordDiv.removeAttr('fa-ajaxrunning');
                            });

                            lcAjaxRequestManager.Execute();

                            GlobalAjaxHandler.SetAjaxLoaderSuppressState(false);
                       }    
                },
                CloseForm : function()
                {       
                    if (clForm.attr('ea-formstack')) {
                        
                        var lcFormStackList = Base64.decode(clForm.attr('ea-formstack')).split('||');
                        var lcLink = '?_f=' + Base64.encode(lcFormStackList[lcFormStackList.length - 1]);
                        var lcFormSavedState = GetUrlParameter('_formsavedstate');
         
                        if (lcFormSavedState) lcLink += '&_formsavedstate=' + lcFormSavedState;
        
                        FormManager.RedirectPage(lcLink);
                    }
                },
                RedirectPage: function (paLink, paNewWindow) {
                    if (paLink) {
                        if (paNewWindow == true) {
                            paLink = paLink.match(/^http[s]*:\/\//) ? paLink : 'http://' + paLink;
                            var lcWindow = window.open(paLink, '_blank');
                            setTimeout(function () { if (lcWindow) lcWindow.focus(); }, 1000);

                        }
                        else {
                            
                            GlobalAjaxHandler.DisplayAjaxLoaderImage('false');
                            if (paLink.indexOf('?') == 0)                                
                                paLink = paLink + GetWebState(true);

                            if (paLink[0] == '?')
                                paLink = FormManager.GetEffectiveURL(FormManager.ExtractBase64FormName(paLink)) + paLink;
                            
                            window.location.href = paLink;
                        }
                    }
                },
                ExtractBase64FormName : function(paBase64Link)
                {
                    if (paBase64Link)
                    {
                        var lcLinkArray = paBase64Link.split('=');

                        if (lcLinkArray.length > 1)
                        {
                            return(FormManager.ExtractFormName(Base64.decode(decodeURIComponent(lcLinkArray[1]))));
                        }
                    }
                    return ('');
                },
                ExtractFormName: function (paLink) {
                    if (paLink) {
                        var lcLinkArray = paLink.split(',');

                        return (lcLinkArray[0].trim());
                    }
                    return ('');
                },
                GetEffectiveURL : function(paFormName)
                {
                    
                    paFormName = paFormName || '*';

                    if (clFormProtocolList)
                    {
                        var lcProtocol = clFormProtocolList['*'] || window.location.protocol;
                        
                        if (clFormProtocolList[paFormName.toLowerCase()]) lcProtocol = clFormProtocolList[paFormName.toLowerCase()];
                        
                        return (lcProtocol + '//' + window.location.host + '/');
                    }

                },
                RedirectStatefulBase64Link: function (paBase64Link, paFormSaveState)
                {
                    if (paBase64Link) {                        
                        var lcFormStack = clForm.attr('ea-formstack');

                        if (!lcFormStack) lcFormStack = Base64.decode(clForm.attr('ea-encodedformname'));
                        else lcFormStack = Base64.decode(lcFormStack) + '||' + Base64.decode(clForm.attr('ea-encodedformname'));

                        paBase64Link += '&_s=' + encodeURIComponent(Base64.encode(lcFormStack));
                        if (paFormSaveState) paBase64Link += '&_formsavedstate=' + encodeURIComponent(paFormSaveState);
                        FormManager.RedirectPage(paBase64Link, false);
                    }
                },
                RedirectStatefulTextLink: function (paLink, paFormSaveState) {
                    if (paLink) {                       
                        paLink = '?_f=' + encodeURIComponent(Base64.encode(paLink))                        
                        FormManager.RedirectStatefulBase64Link(paLink, paFormSaveState);
                    }
                },
                BindFormEvents : function()
                {
                    clForm.find('a[ea-command="@cmd%formclose"]').unbind('click');
                    clForm.find('a[ea-command="@cmd%formclose"]').click(function (paEvent) {                        
                        paEvent.preventDefault();
                        FormManager.CloseForm();                        
                    });

                    clForm.find('a[ea-command="@cmd%logout"]').unbind('click');
                    clForm.find('a[ea-command="@cmd%logout"]').click(function (paEvent) {
                        paEvent.preventDefault();
                        FormManager.LogOutSystem();                        
                    });

                    clPasswordDiv.find('a[ea-command="@cmd%closepassword"]').unbind('click');
                    clPasswordDiv.find('a[ea-command="@cmd%closepassword"]').click(function (paEvent) {
                        paEvent.preventDefault();
                        SecurityController.Resolve(false);
                    });
                    
                    clPasswordDiv.find('.PasswordInputDiv img[ea-command="@cmd%verifypassword"]').unbind('click');
                    clPasswordDiv.find('.PasswordInputDiv img[ea-command="@cmd%verifypassword"]').click(function (paEvent) {                        
                        paEvent.preventDefault();
                        FormManager.VerifyPassword();
                    });

                    clPasswordDiv.find('.PasswordInputDiv input[type=password]').unbind('keydown');
                    clPasswordDiv.find('.PasswordInputDiv input[type=password]').keydown(function (paEvent) {

                        var lcKeyCode = paEvent.keyCode || paEvent.which;

                        if (lcKeyCode == 13) {
                            paEvent.preventDefault();

                            FormManager.VerifyPassword();            
                        }        
                    });
                }
         }
})();

var GlobalAjaxHandler = (function () {

    var clAjaxLoaderElement;
    var clDisplayMode;
    var clDefaultAjaxLoaderElement;
    var clDefaultDisplayMode;
    var clSuppressAjaxLoader;
    var clAjaxErrorHandler;
    var clSuppressAjaxError;

    return {
        SetAjaxErrorSuppressState: function (paSuppressAjaxError) {
            clSuppressAjaxError = paSuppressAjaxError;
        },
        GetAjaxErrorSuppressState: function () {
            return(clSuppressAjaxError);
        },
        SetAjaxErrorHandler : function (paAjaxErrorHandler) {
            clAjaxErrorHandler = paAjaxErrorHandler;            
        },
        GetAjaxErrorHandler: function () {            
            return (clAjaxErrorHandler);
        },
        SetAjaxLoaderSuppressState : function (paSuppressAjaxLoader) {
            clSuppressAjaxLoader = paSuppressAjaxLoader;
        },
        SetAjaxLoader: function (paAjaxLoaderPopUp, paDisplayMode) {
            if (paAjaxLoaderPopUp) {
                SetAjaxLoaderElement(paAjaxLoaderPopUp, paDisplayMode || "block");
                clSuppressAjaxLoader = false;
            }
        },
        SetAjaxLoaderStatusText: function (paStatusText) {
            var lcActiveAjaxLoaderElement = clAjaxLoaderElement || clDefaultAjaxLoaderElement;

            if (lcActiveAjaxLoaderElement)
                lcActiveAjaxLoaderElement.attr('value', paStatusText);
        },
        SetDefaultAjaxLoaderElement: function () {
            clDefaultAjaxLoaderElement = $('[sa-elementtype=ajaxloaderpopup]').first();
            clDefaultDisplayMode = 'block';
            clSuppressAjaxLoader = false;
        },
        DisplayAjaxLoaderImage: function (paShowImage) {            
            var lcActiveAjaxLoaderElement = clAjaxLoaderElement || clDefaultAjaxLoaderElement;

            if (clSuppressAjaxLoader == true) lcActiveAjaxLoaderElement = false;
            
            if (lcActiveAjaxLoaderElement) {                
                var lcStatusText = lcActiveAjaxLoaderElement.attr('value') || lcActiveAjaxLoaderElement.attr('ea-default');
                var lcStatusDisplayControl = lcActiveAjaxLoaderElement.find('[sa-elementtype=ajaxloaderstatusdisplay]');
                                
                if ((paShowImage) && (paShowImage == 'false')) clDefaultAjaxLoaderElement.find(".AjaxImageDiv").hide();
                else clDefaultAjaxLoaderElement.find(".AjaxImageDiv").show();
                
                lcStatusDisplayControl.text(lcStatusText);
                lcActiveAjaxLoaderElement.css('display', clDisplayMode || clDefaultDisplayMode);

                lcActiveAjaxLoaderElement.find("[ea-gifdisplaymode]").each(function () {                    
                    $(this).css('display', $(this).attr('ea-gifdisplaymode'));
                });

            }
        },
        HideAjaxLoaderImage: function () {
            var lcActiveAjaxLoaderElement = clAjaxLoaderElement || clDefaultAjaxLoaderElement;

            if (clSuppressAjaxLoader == true) lcActiveAjaxLoaderElement = false;

            if (lcActiveAjaxLoaderElement) {
                lcActiveAjaxLoaderElement.css('display', 'none');
                lcActiveAjaxLoaderElement.removeAttr('value');
                lcActiveAjaxLoaderElement.find("[sa-gifdisplaymode]").each(function () {
                    $(this).css('display', 'none');
                });

                clAjaxLoaderElement = null;

            }
        },
        IsInlineAjaxMode: function () {
            return (clDisplayMode == "inline-block");
        }
    }

    function SetAjaxLoaderElement(paAjaxLoaderElement, paDisplayMode) {
        clAjaxLoaderElement = paAjaxLoaderElement;
        clDisplayMode = paDisplayMode;
    }
})();


var MessageHandler = (function () {
    var clMessageRepository;

    return {
                Init : function()
                {
                    clMessageRepository = $(document).find('[sa-elementtype=messagerepository]');                    
                },
                GetMessage : function(paMessageCode)
                {
                    if (paMessageCode) {
                        var lcMessageInfo = clMessageRepository.find('span[ea-code="' + paMessageCode + '"]');
                        if (lcMessageInfo.length > 0) return (lcMessageInfo.text());                        
                    }
                    return (null);
                },
                ShowMessage : function(paMessageCode, paCallBack, paFocusControl)
                {
                    if (paMessageCode) {
                        var lcMessageInfo = clMessageRepository.find('span[ea-code="' + paMessageCode + '"]');
                        var lcOption = [];

                        if (lcMessageInfo.length > 0)
                        {
                            var lcMessageType = lcMessageInfo.attr('ea-type');
                            var lcButton1 = (lcMessageInfo.attr('ea-button1') || '').split('::');
                            var lcButton2 = (lcMessageInfo.attr('ea-button2') || '').split('::');

                            lcOption['title'] = lcMessageInfo.attr('ea-title')  ||  '';
                            lcOption['message'] = lcMessageInfo.html();                            
                            
                            var lcButton2     = (lcMessageInfo.attr('ea-button2') || '').split('::');

                            if (lcMessageType == 'confirmation')
                            {
                                lcOption['button1text'] = lcButton1[0];
                                lcOption['button1action'] = lcButton1.length > 1 ? lcButton1[1] : 'yes';
                                lcOption['button2text'] = lcButton2[0];
                                lcOption['button2action'] = lcButton2.length > 1 ? lcButton2[1] : 'no';
                            }
                            else 
                            {
                                lcOption['button1text'] = lcButton1[0] || 'OK';
                                lcOption['button1action'] = lcButton1.length > 1 ? lcButton1[1] : 'close';
                            }

                            if (paCallBack) paCallBack(lcOption);
                            var lcMessageBox = new MessageBoxInstance();
                            return (lcMessageBox.CreateInstance(lcOption, paFocusControl));
                        }
                    }
                },
                ShowDynamicMessage: function (paMessageCode, paPlaceHolderList, paFocusControl)
                {
                    return(MessageHandler.ShowMessage(paMessageCode, function (paOption) {
                        if (paPlaceHolderList) {
                            $.each(paPlaceHolderList, function (paName, paValue) {
                                paOption['message'] = paOption['message'].replace(paName, paValue);
                            });
                        }
                    }, paFocusControl));                    
                }
          }
})();

var ShowMessageBox = function (paOption) {
    if (!paOption) paOption = {};

    if (!paOption["title"]) paOption["title"] = "Message";
    if (!paOption["button1text"]) paOption["button1text"] = "Close";
    if (!paOption["button1action"]) paOption["button1action"] = paOption["button1text"];
    if (!paOption["button2action"]) paOption["button2action"] = paOption["button2text"];
    if (!paOption["message"]) paOption["message"] = "";

    lcMsg = new MessageBoxInstance();
    return (lcMsg.CreateInstance(paOption));
};

var ShowInfoMessage = function (paMessageText, paFocusControl) {

    lcMsg = new MessageBoxInstance();
    return (lcMsg.CreateInstance({ title: "Information", message: paMessageText, button1text: "Ok" }, paFocusControl));
};

var ShowErrorMessage = function (paMessageText, paFocusControl) {

    lcMsg = new MessageBoxInstance();
    return (lcMsg.CreateInstance({ title: "Error", message: paMessageText, button1text: "Ok" }, paFocusControl));
};

var ShowConfirmationMessage = function (paMessageText, paYesText, paNoText, paFocusControl) {    
    lcMsg = new MessageBoxInstance();
    return (lcMsg.CreateInstance({
        title: "Confirmation",
        message: paMessageText,
        button1text: paYesText,
        button1action: "yes",
        button2text: paNoText,
        button2action: "no"
    }, paFocusControl));
    
};

var DefaultMsgBoxCompleteHandler = function (paEvent) {    
    
    paEvent.preventDefault();    
    var lcMessageBoxInstance = $(this).closest("[sa-elementtype=messagebox]");    
    paEvent.data.instance.Deferred.resolve($(this).attr('ea-action') || $(this).text());    
    if (paEvent.data.instance.FocusControl) {
        paEvent.data.instance.FocusControl.select();
        paEvent.data.instance.FocusControl.focus();
    }
    lcMessageBoxInstance.remove();
}

var MessageBoxInstance = function () {

    return {
        CreateInstance: function (paOption, paFocusControl) {
            return (CreateMessageBoxInstance(paOption, paFocusControl));
        },
        Resolve: function (paAction) {
            this.Deferred.resolve(paAction);
        }
    };

    function CreateMessageBoxInstance(paOption, paFocusControl) {
        this.CurrentInstance = this;
        this.MessageBoxOptions = paOption || {};
        this.FocusControl = paFocusControl;        

        var lcMessageBoxTemplate = $(document).find('[sa-elementtype=messageboxtemplate]');
        var lcMessageBoxInstance = lcMessageBoxTemplate.clone();
        var lcMessageBoxInstanceID = "MSGBOX-" + Math.floor((Math.random() * 100000) + 1);

        this.Deferred = $.Deferred();
        this.MessageBoxID = lcMessageBoxInstanceID;

        lcMessageBoxInstance.attr('id', lcMessageBoxInstanceID);
        lcMessageBoxInstance.attr('sa-elementtype', 'messagebox');
        lcMessageBoxInstance.find('a[href="@msgcmd%buttonclick"]').click({ instance: this }, DefaultMsgBoxCompleteHandler);

        SetupMessageBoxApperance(lcMessageBoxInstance);
        $('body').append(lcMessageBoxInstance);
        lcMessageBoxInstance.show();
        return (this.Deferred);
    }

    function SetupMessageBoxApperance(paMessageBoxInstance) {

        var lcTitleBar = paMessageBoxInstance.find('[ea-componentid=msg_title]');
        var lcContentDiv = paMessageBoxInstance.find('[ea-componentid=msg_content]');
        var lcButton1 = paMessageBoxInstance.find('[ea-componentid=msg_button1]');
        var lcButton2 = paMessageBoxInstance.find('[ea-componentid=msg_button2]');

        if (this.MessageBoxOptions["title"]) {
            lcTitleBar.text(MessageBoxOptions["title"]);
        }

        if (this.MessageBoxOptions["message"]) {
            lcContentDiv.html(MessageBoxOptions["message"]);
        }

        if (this.MessageBoxOptions["button1text"]) {
            lcButton1.text(MessageBoxOptions["button1text"]);
            lcButton1.attr('ea-action', MessageBoxOptions["button1action"]);
            lcButton1.css('display', 'inline-block');
        }

        if (this.MessageBoxOptions["button2text"]) {
            lcButton2.text(MessageBoxOptions["button2text"]);
            lcButton2.attr('ea-action', MessageBoxOptions["button2action"]);
            lcButton2.css('display', 'inline-block');
        }
    }
};


var SecurityController = (function () {
    var clDeferred;    

    return {
        ShowPasswordPopUp: function () {
            clDeferred = $.Deferred();
            FormManager.GetForm().attr('fa-verifypassword', 'true');            
            return (clDeferred);
        },
        Resolve: function (paSuccess) {
            clDeferred.resolve(paSuccess);
            FormManager.GetForm().removeAttr('fa-verifypassword');
        }
    }
})();


var AjaxRequestManager = function (paAjaxRequest, paSuccessMessageCode, paFailMessageCode, paAjaxLoaderStatusCode) {
    var clAjaxData                  = { CobraAjaxRequest: paAjaxRequest };
    var clMessagePlaceHolderList    = {};    
    var clAjaxLoaderStatusCode      = paAjaxLoaderStatusCode || 'ajax_loading';
    var clSuccessMessageCode        = paSuccessMessageCode;
    var clFailMessageCode           = paFailMessageCode;    
    var clCompleteHandler;
    var clConfirmationResultHandler;
    var clResponseDictionaryParsingHandler;
    var clFormData                  = new FormData();
    var clAjaxMode                  = 'normal';
    
    return {
                SwitchFormDataMode : function()
                {
                    clAjaxMode = 'formdata';
                    clFormData.append("CobraAjaxRequest", paAjaxRequest);
                },                
                AddAjaxParam: function (paKey, paValue, paFileName) {
                    if (clAjaxMode !== 'formdata') clAjaxData[paKey] = paValue;
                    else
                    {
                        if (paFileName) clFormData.append(paKey, paValue, paFileName);
                        else clFormData.append(paKey, paValue);
                    }
                },
                AddObjectDataBlock: function (paKey, paValue, paAppendSysInfo) {
                    if (paValue) 
                    {
                        if (paAppendSysInfo)
                            paValue['accessinfo'] = window.__SYSVAR_CurrentGeoLocation || '';

                        if (clAjaxMode !== 'formdata') clAjaxData[paKey] = Base64.encode(JSON.stringify(paValue));
                        else clFormData.append(paKey, Base64.encode(JSON.stringify(paValue)));
                    }
                },
                AddStringDataBlock: function (paKey, paValue) {
                    if (paValue) {
                        if (clAjaxMode !== 'formdata') clAjaxData[paKey] = Base64.encode(paValue);
                        clFormData.append(paKey, Base64.encode(paValue));
                    }
                },
                AddMessagePlaceHolder : function(paKey, paValue)
                {
                    clMessagePlaceHolderList[paKey] = paValue;
                },
                SetCompleteHandler : function(paCompleteHandler)
                {
                    clCompleteHandler = paCompleteHandler;
                },
                SetConfirmationResultHandler : function(paConfirmationResultHandler)
                {
                    clConfirmationResultHandler = paConfirmationResultHandler;
                },
                SetResponseDictionaryParsingHandler : function(paResponseDictionaryParsingHandler)
                {
                    clResponseDictionaryParsingHandler = paResponseDictionaryParsingHandler;
                },
                ShowCompleteMessage : function(paSuccess, paMessageCode, paResponseStruct)
                {
                    if (paMessageCode)
                    {                        
                        MessageHandler.ShowMessage(paMessageCode, function (paOption) { 
                            $.each(clMessagePlaceHolderList, function(paName, paValue){
                                paOption['message'] = paOption['message'].replace(paName, paValue); 
                            });                            
                        }).done(function (paButtonAction) {                            
                            if (clCompleteHandler) clCompleteHandler(paSuccess, paResponseStruct, paButtonAction);
                        });  
                    }
                    else if (clCompleteHandler) clCompleteHandler(paSuccess, paResponseStruct);
                },
                ExecuteOnConfirm : function(paMessageCode)
                {
                    var lcActiveObject = this;

                    if (paMessageCode)
                    {                        
                        MessageHandler.ShowMessage(paMessageCode, function (paOption) { 
                                            $.each(clMessagePlaceHolderList, function(paName, paValue){
                                                paOption['message'] = paOption['message'].replace(paName, paValue); 
                                            });                            
                        }).done(function (paResult) {

                            if (clConfirmationResultHandler) clConfirmationResultHandler(paResult);

                            if (paResult == 'yes')
                            {                                
                                lcActiveObject.Execute();
                            }
                        });
                    }
                    else lcActiveObject.Execute();
                },
                Execute : function()
                {
                    var lcActiveObject = this;

                    
                    GlobalAjaxHandler.SetAjaxLoaderStatusText(MessageHandler.GetMessage(clAjaxLoaderStatusCode));
                    
                    DoPostBack(clAjaxMode === 'formdata' ? clFormData : clAjaxData, function (paResponseData) {
                        var lcRespondStruct = jQuery.parseJSON(paResponseData);
                        if (lcRespondStruct.Success) {                            
                            lcActiveObject.ShowCompleteMessage(true, clSuccessMessageCode, lcRespondStruct);
                        }
                        else {
                            
                            if ((lcRespondStruct.ResponseData.RSP_Dictionary) && (clResponseDictionaryParsingHandler))                                 
                                clResponseDictionaryParsingHandler(clMessagePlaceHolderList, lcRespondStruct.ResponseData.RSP_Dictionary);                            
                            
                            lcActiveObject.ShowCompleteMessage(false, lcRespondStruct.ResponseData.RSP_ErrorCode || clFailMessageCode, lcRespondStruct);
                        }
                    });
                }

    }
};

