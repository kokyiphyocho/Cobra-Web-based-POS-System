﻿var SelectionPanelController = function (paSelectionPanel, paControl) {

    var clSelectionPanel    = paSelectionPanel;
    var clControl           = paControl;
    var clList;
    var clType;

    return {
        Init: function () {
            clList = clSelectionPanel.find('[sa-elementtype=list]');
            clType = clSelectionPanel.attr('ea-type');
            this.BindSelectionEvents();
        },

        BindSelectionEvents: function ()
        {
            clSelectionPanel.find('a[ea-command^="@cmd%"]').unbind('click');
            clSelectionPanel.find('a[ea-command^="@cmd%"]').click(this, this.HandlerOnClick);
        },

        OpenPopUp : function(paSelectedValue)
        {
            this.SelectItemByValue(paSelectedValue);
            clControl.trigger('ev-selectionpanelevent', { event: 'openpopup', sender: this, type: clType });            
            clSelectionPanel.attr('fa-show', 'true');            
        },

        ClosePopUp : function()
        {
            clControl.trigger('ev-selectionpanelevent', { event: 'closepopup', sender: this, type: clType });
            clSelectionPanel.removeAttr('fa-show');
        },

        SelectItemByValue : function(paValue)
        {
            if (paValue)
            {
                paValue = paValue.toString().trim();
                
                var lcItem = clList.find('.Item[value="' + paValue + '"]');                
                if (lcItem) this.SelectItem(lcItem);
            }
        },

        SelectItem : function(paItem)
        {
            paItem.siblings('.Item').removeAttr('fa-selected');
            paItem.attr('fa-selected', true);
        },

        ConfirmSelection  : function()
        {
            var lcSelectedItem = clList.find('.Item[fa-selected]');            
            clControl.trigger('ev-selectionpanelevent', { event: 'selectionchoosed', sender: this, type: clType, selectedvalue: lcSelectedItem.attr('value'), selectedtext : lcSelectedItem.text() });
            this.ClosePopUp();
            
        },

        HandlerOnClick: function (paEvent) {
            paEvent.preventDefault();
            
            var lcCommand = $(this).attr('ea-command');
            lcCommand = lcCommand.substring(lcCommand.indexOf('%') + 1);
                        
            switch (lcCommand)
            {
                case 'item':
                    {
                        var lcItem = $(this);
                        paEvent.data.SelectItem(lcItem);

                        break;
                    }

                case 'selectionclose'  :
                case 'selectioncancel' :
                    {
                      //  clControl.removeAttr('fa-showselectionpopup');
                        paEvent.data.ClosePopUp();
                        break;
                    }

                case 'selectionchoose' :
                    {                        
                        paEvent.data.ConfirmSelection();
                        break;
                    }
            }
        }
    } // Return

};




