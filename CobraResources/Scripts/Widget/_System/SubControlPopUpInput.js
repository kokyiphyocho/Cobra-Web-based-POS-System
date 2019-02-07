var InputPopUpController = function (paTypeID) {
    var clTypeID = paTypeID;
    var clPopUp;
    var clMessageBar;

    return {
        Init: function () {
            clPopUp = $('[sa-elementtype=popup].SubControlPopUpInputComposite[ea-type="' + paTypeID + '"]');
            clMessageBar = clPopUp.find('[sa-elementtype=messagebar]');
            
            clPopUp.find('a[ea-command]').unbind('click');
            clPopUp.find('a[ea-command]').click({ control: this }, this.HandlerOnClick);

            clPopUp.find('input[type=text][ea-columnname]').unbind('change');
            clPopUp.find('input[type=text][ea-columnname]').change({ control: this }, this.HandlerOnChange);

            clPopUp.find('input[type=text][ea-inputmode=decimal]').ForceDecimalInput();
            clPopUp.find('input[type=text][ea-inputmode=number]').ForceIntegerInput();
        },
        Show :function()
        {
            var lcEventInfo = {};

            clPopUp.find('input[type=text][ea-columnname]').each(function()
            {
                lcEventInfo["typeid"] = clTypeID;
                lcEventInfo["columnname"] = $(this).attr('ea-columnname');
                lcEventInfo["eventdata"] = $(this).val();
                lcEventInfo["target"] = $(this);
                lcEventInfo["defaultaction"] = true;

                clPopUp.trigger('ev-datainit', lcEventInfo);
            });

            clPopUp.attr('fa-show','true');
        },
        Hide : function()
        {
            clPopUp.removeAttr('fa-show');
        },
        SetHandler: function (paTriggerName, paFunction) {
            if ((paTriggerName) && (paFunction)) {
                clPopUp.unbind(paTriggerName);
                clPopUp.bind(paTriggerName, paFunction);
            }
        },
        SetMessageBarText : function(paMessageCode)
        {            
            clMessageBar.text(MessageHandler.GetMessage(paMessageCode));
        },
        SetTextBoxData : function(paColumnName, paData)
        {
            var lcTextBox = clPopUp.find('[ea-columnname="' + paColumnName + '"]');

            if (lcTextBox.length > 0)
            {
                lcTextBox.val(paData || '');
            }
        },
        GetControl : function(paSelector)
        {
            return (clPopUp.find(paSelector));
        },
        DefaultChangeAction : function (paEventInfo) {
            var lcElement   = paEventInfo.target;        
            var lcData      = lcElement.val();
            var lcDataBlock = {};
            var lcActiveObject = this;
          
            if ((lcElement) && (lcElement.attr('ea-queryname')))
            {                
                if ((lcData.trim().length > 0)) {
                    var lcQuery = lcElement.attr('ea-queryname');
                    lcDataBlock[lcElement.attr('ea-columnname')] = lcData;
                    lcDataBlock['_local_' + lcElement.attr('ea-columnname')] = FormManager.ConvertToFormLanguage(lcData);
                    lcDataBlock['_system_' + lcElement.attr('ea-columnname')] = FormManager.NormalizeNumber(lcData);
                    
                    var lcAjaxRequestManager = new AjaxRequestManager('getdatarowquery', null, null, 'ajax_loading');

                    lcAjaxRequestManager.AddAjaxParam('Parameter', lcQuery);
                    lcAjaxRequestManager.AddObjectDataBlock('datablock', lcDataBlock);

                    lcAjaxRequestManager.SetCompleteHandler(function (paSuccess, paResponseStruct) {
                        if (paSuccess) {
                            lcActiveObject.PopulateData(lcElement, paResponseStruct.ResponseData);
                        }
                        else {
                            lcActiveObject.PopulateData(lcElement);
                        }
                    });

                    lcAjaxRequestManager.Execute();
                }
                else lcActiveObject.PopulateData(lcElement);
            }            
        },
        PopulateData : function(paElement, paData)
        {            
            var lcTriggerColumnName = paElement.attr('ea-columnname');

            if (!paData) paData = {};
            
            clPopUp.find('[ea-columnname]').each(function () {
                var lcColumnName = $(this).attr('ea-columnname');
                if (lcColumnName != lcTriggerColumnName)                
                    $(this).val(paData[lcColumnName] || paData[lcColumnName] == 0 ? paData[lcColumnName] : '');
            });
            
        },
        VerifyInput : function()
        {
            var lcVerifySuccess;
            var lcTextBoxes = clPopUp.find('input[type=text][ea-columnname]');
            var lcEventInfo = {};

            lcVerifySuccess = true;

            lcTextBoxes.removeAttr('fa-verifyfail');

            lcTextBoxes.each(function () {

                var lcInputMode = $(this).attr('ea-inputmode');
                var lcValue = $(this).val();
                var lcDateFormat = $(this).attr('ea_dateformat') || 'DD/MM/YYYY';            
            
                if (lcInputMode == 'date')
                {
                    var lcDate = lcValue.ParseMomentDate();
                    if ((lcDate == null) || (!lcDate.isValid()))
                    {
                        $(this).attr('fa-verifyfail', 'true');
                        MessageHandler.ShowMessage(50, false, $(this));                    
                        lcVerifySuccess = false;
                        return (false);
                    }

                    lcValue = lcDate;                
                }           

                lcEventInfo["typeid"] = clTypeID;
                lcEventInfo["columnname"] = $(this).attr('ea-columnname');
                lcEventInfo["eventdata"] = lcValue;
                lcEventInfo["target"] = $(this);
                lcEventInfo["defaultaction"] = true;

                clPopUp.trigger('ev-verify', lcEventInfo);

                if (!lcEventInfo.defaultaction) 
                {
                    lcVerifySuccess = false;
                    return (false);
                }
            });

            return (lcVerifySuccess);
        },
        GetData : function(paColumnname)
        {
            if (paColumnname)
            {
                var lcControl = clPopUp.find('input[type=text][ea-columnname=' + paColumnname + ']');

                return (lcControl.val());
            }
        },
        CommitChanges : function()
        {
            var lcEventInfo = {};

            clPopUp.find('input[type=text][ea-columnname]').each(function () {

                var lcInputMode = $(this).attr('ea-inputmode');
                var lcValue = $(this).val();
                var lcDateFormat = $(this).attr('ea_dateformat') || 'DD/MM/YYYY';

                if (lcInputMode == 'date') {
                    var lcDate = lcValue.ParseMomentDate();
                    lcValue = FormManager.ConvertToFormLanguage(lcDate.format('DD/MM/YYYY'));
                }
                
                lcEventInfo["typeid"] = clTypeID;
                lcEventInfo["columnname"] = $(this).attr('ea-columnname');
                lcEventInfo["eventdata"] = lcValue;
                lcEventInfo["target"] = $(this);
                lcEventInfo["defaultaction"] = true;

                clPopUp.trigger('ev-commit', lcEventInfo);
            });

            lcEventInfo = {};
            lcEventInfo["typeid"] = clTypeID;                        
            lcEventInfo["defaultaction"] = true;

            if (lcEventInfo.defaultaction) clPopUp.trigger('ev-commitcomplete', lcEventInfo);
        },
        ActionOnClick : function(paEventInfo)
        {
            clPopUp.trigger('ev-command', paEventInfo);

            if (paEventInfo.defaultaction) {
                switch (paEventInfo.eventdata) {
                    case "update": {
                        if (this.VerifyInput()) {
                            this.CommitChanges();
                            clPopUp.removeAttr('fa-show');
                        }
                        break;
                    }

                    case "cancel": {
                        clPopUp.removeAttr('fa-show');
                        break;
                    }
                }
            }
        },
        ActionOnChange : function(paEventInfo)
        {
            clPopUp.trigger('ev-change', paEventInfo);

            if (paEventInfo.defaultaction) this.DefaultChangeAction(paEventInfo);

        },
        HandlerOnClick: function (paEvent) {            
            var lcEventInfo = {};
            var lcCommand = $(this).attr('ea-command');
            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);
            
            lcEventInfo["typeid"] = clTypeID;
            lcEventInfo["defaultaction"] = true;
            lcEventInfo["target"] = $(this);
            lcEventInfo["eventdata"] = lcCommand;            

            paEvent.data.control.ActionOnClick(lcEventInfo);
          
        },
        HandlerOnChange: function (paEvent) {
            var lcEventInfo = {};

            lcEventInfo["typeid"] = clTypeID;
            lcEventInfo["columnname"] = $(this).attr('ea-columnname');
            lcEventInfo["eventdata"] = $(this).val();
            lcEventInfo["target"] = $(this);
            lcEventInfo["defaultaction"] = true;            

            paEvent.data.control.ActionOnChange(lcEventInfo);            
        }
    }
};

