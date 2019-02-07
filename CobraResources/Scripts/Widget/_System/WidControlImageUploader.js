$(document).ready(function () {
    $('[sa-elementtype=control].WidControlImagerUploader').BindImageUploaderEvents();    
});

$.fn.BindImageUploaderEvents = function () {
    $('a[href^="@cmd%select"]').unbind("click");
    $('a[href^="@cmd%select"]').click(function (paEvent) {
        paEvent.preventDefault();
        var lcImageUploadControl = $(this).closest('[sa-elementtype=control]');        
        var lcFileInputControl = lcImageUploadControl.find('input[type=file]');
                
        if (lcFileInputControl.length > 0) {            
            lcFileInputControl.trigger("click");
        }    
    });

    $('a[href^="@cmd%delete"]').unbind("click");
    $('a[href^="@cmd%delete"]').click(function (paEvent) {
        paEvent.preventDefault();
        var lcImageUploadControl = $(this).closest('[sa-elementtype=control]');
        var lcImageTag = lcImageUploadControl.find('[sa-elementtype=element] img');
        var lcFileInputControl = lcImageUploadControl.find('input[type=file]');
        
        lcFileInputControl.wrap('<form>').closest('form').get(0).reset();
        lcFileInputControl.unwrap();

        lcImageTag.removeAttr('src');
        lcImageTag.hide();

        lcImageUploadControl.removeAttr('fa-imageselected'); 
    });

    $('a[href^="@cmd%upload"]').unbind("click");
    $('a[href^="@cmd%upload"]').click(function (paEvent) {
        paEvent.preventDefault();
        var lcImageUploadControl = $(this).closest('[sa-elementtype=control]');
        var lcImageContainer     = lcImageUploadControl.find('[sa-elementtype=element]');
        var lcImageTag           = lcImageContainer.find('img');
        var lcFileInputControl   = lcImageUploadControl.find('input[type=file]');

        if ((lcFileInputControl.files.length > 0) && (lcFileInputControl.files[0])) {
            var lcFormData = new FormData();
            lcFormData.append('image', lcFileInputControl.files[0]);
            lcFormData.append('CobraAjaxRequest', 'UploadFile');

            DoWASPPostBackFormData(lcFormData, function (paData) {
                    var lcRespondStruct = jQuery.parseJSON(paData);
                    if (lcRespondStruct.Success) {
                        lcImageContainer.attr('value', lcRespondStruct.ResponseData.FileName);                        
                    }
                    else { if (RespondStruct.ResponseData.ErrorMessage) { ShowErrorMessage(RespondStruct.ResponseData.ErrorMessage); } }
            });
        }        
    });

    $(this).find('input[type=file]').unbind("change");
    $(this).find('input[type=file]').change(function (paEvent) {
        paEvent.preventDefault();
        var lcImageUploadControl = $(this).closest('[sa-elementtype=control]');
        var lcImageContainer     = lcImageUploadControl.find('[sa-elementtype=element]');
        var lcMaxFileSize       = Number(lcImageUploadControl.attr('ea-maxfilesize'));
        
        if ((this.files.length > 0) && (this.files[0])) {
            if (!isImage(this.files[0])) ShowErrorMessage("Invalid Image File.");
            else if (this.files[0].size > lcMaxFileSize) ShowErrorMessage("Exceed Maximum File Size Limit (" + lcMaxFileSize + ") Bytes");
            else {
                lcImageUploadControl.attr('fa-imageselected', 'true');
                ShowImageUploaderPreviewImage(this.files[0], lcImageContainer);
                return;          
            }
        }
        lcImageUploadControl.removeAttr('fa-imageselected');
    });
}

function ShowImageUploaderPreviewImage(paFile, paImageContainer) {
    if (paFile) {
        var lcReader = new FileReader();
        lcReader.onloadend = function (paEvent) {            
            var lcImageTag = paImageContainer.find('img');            
            lcImageTag.attr('src', paEvent.target.result);            
            lcImageTag.show();
        };
        lcReader.readAsDataURL(paFile);
    }
}