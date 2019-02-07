$(document).ready(function () {
    POSUpdateSubscriberInfoManager.Init();
});

var POSUpdateSubscriberInfoManager = (function () {

    var clControl;
    var clPhoneTextBoxList;
    var clEmailTextBoxList;    
    var clButtonPanel;    

    return {
        Init: function () {
            clControl                   = $('[sa-elementtype=control].WidControlPOSUpdateSubscriberInfo');
            clPhoneTextBoxList          = clControl.find('input[ea-inputmode=phoneno]');
            clEmailTextBoxList          = clControl.find('input[ea-inputmode=email]');            
            clButtonPanel               = clControl.find('.ButtonPanel');

            clPhoneTextBoxList.ForcePhoneNoInput();
            POSUpdateSubscriberInfoManager.BindEvents();
            
        },
        BindEvents: function () {            
            clButtonPanel.find('[ea-command^="@cmd%"]').unbind('click');
            clButtonPanel.find('[ea-command^="@cmd%"]').click(POSUpdateSubscriberInfoManager.HandlerOnClick);
        },
        VerifyInputs: function () {
            var lcMandatoryField = clControl.find('[ea-columnname][ea-mandatory=true]');
            var lcSuccess = true;
            
            lcMandatoryField.each(function () {
                if ($(this).val().trim() == '') {
                    var lcFieldName = $(this).attr('ea-name');
                    MessageHandler.ShowDynamicMessage('err_requirefieldmissing', { '$FIELD': lcFieldName }, $(this));
                    lcSuccess = false;
                    return (false);
                }

            });

            return (lcSuccess);
        },
        UpdateInfo: function () {                
            
            if (POSUpdateSubscriberInfoManager.VerifyInputs()) {                
                var lcMasterBlock = {};
                var lcSubscriberInfo = POSUpdateSubscriberInfoManager.GetSubscriberInfoBlock();

                lcMasterBlock['subscriberinfo'] = Base64.encode(JSON.stringify(lcSubscriberInfo));
                var lcAjaxRequestManager = new AjaxRequestManager('executescalarquery', null, 'err_failupdate', 'ajax_updating');

                lcAjaxRequestManager.AddAjaxParam('Parameter', 'epos.updatesubscriberinfo');
                lcAjaxRequestManager.AddObjectDataBlock('DataBlock', lcMasterBlock, true);

                lcAjaxRequestManager.SetCompleteHandler(function (paSuccess) {
                    if (paSuccess) {
                        FormManager.CloseForm();
                    }
                });

                lcAjaxRequestManager.Execute();      
            }
        },        
        GetSubscriberInfoBlock: function () {
            var lcDataBlock = {};
            var lcElements = clControl.find('[ea-columnname]');

            lcDataBlock['SubscriptionID'] = clControl.attr('ea-dataid');

            lcElements.each(function () {
                var lcName = $(this).attr('ea-columnname');
                lcDataBlock[lcName] = $(this).val();                
            });

            return (lcDataBlock);
        },
        HandlerOnClick: function (paEvent) {
            paEvent.preventDefault();
            
            var lcCommand = $(this).attr('ea-command');
            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);
            
            switch (lcCommand) {
                case 'update':
                    {                        
                        POSUpdateSubscriberInfoManager.UpdateInfo()
                        break;
                    }
                case 'close':
                    {
                        FormManager.CloseForm();
                        break;
                    }
            }

        }
    }
})();
