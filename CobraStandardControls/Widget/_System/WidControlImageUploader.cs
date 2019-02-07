using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using CobraFrame;
using CobraResources;
using CobraWebFrame;

namespace CobraStandardControls
{
    public class WidControlImagerUploader : WebControl, WidgetControlInterface
    {
        protected const String ctWidControlImageUploaderScript = "WidControlImageUploader.js";
        protected const String ctWidControlImageUploaderStyle  = "WidControlImageUploader.css";

        protected const String ctCLSWidControlImagerUploader    = "WidControlImagerUploader";
        protected const String ctCLSImageContainer              = "ImageContainer";
        protected const String ctCLSImage                       = "Image";
        protected const String ctCLSButtonPanel                 = "ButtonPanel";
        protected const String ctCLSSelectButton                = "SelectButton";
        protected const String ctCLSUploadButton                = "UploadButton";
        protected const String ctCLSDeleteButton                = "DeleteButton";
        protected const String ctCLSFileInput                   = "FileInput";

        protected const String ctDEFSelectButtonText            = "Select";
        protected const String ctDEFUploadButtonText            = "Upload";
        protected const String ctDEFDeleteButtonText            = "Delete";
        protected const long   ctDEFMaxFileSize                 = 2L * (1024L * 1024L); // 2MB
        protected const int    ctDEFMaxFileCount                = 1;
        protected const String ctDEFExtensionList               = "jpg,jpeg,png,bmp,gif";        
                
        protected const String ctDEFExceedMaxFileCount          = "Maximum upload count is reached. Delete some file to upload.";
        protected const String ctDEFNoFileSelected              = "No Image is Selected.";
        protected const String ctDEFInvalidFileType             = "Invalid File Type.";
        protected const String ctDEFFileTooLarge                = "File size too larege. (Max : $MAXFILESIZE)";
        protected const String ctDEFUploadSuccess               = "File is successfully uploaded.";
        protected const String ctDEFUploadFail                  = "Error in uploading file.";
        protected const String ctDEFDeleteConfirmation          = "Do you want to delete the selected image ? ";
        protected const String ctDEFDeleteSuccess               = "Delete the file successfully.";
        protected const String ctDEFDeleteFail                  = "Error in deleting file.";

        protected const String ctCMDSelect                      = "@cmd%select";        
        protected const String ctCMDUpload                      = "@cmd%upload";        
        protected const String ctCMDDelete                      = "@cmd%delete";

        public CompositeFormInterface SCI_ParentForm  { get; set; }

        public long     SC_MaxFileSize                { get; set; }
        public int      SC_MaxFileCount               { get; set; }

        public String   SCMSG_ExceedMaxFileCount      { get; set; }
        public String   SCMSG_NoFileSelected          { get; set; }
        public String   SCMSG_InvalidFileType         { get; set; }
        public String   SCMSG_FileTooLarge            { get; set; }
        public String   SCMSG_UploadSuccess           { get; set; }
        public String   SCMSG_UploadFail              { get; set; }
        public String   SCMSG_DeleteConfirmation      { get; set; }
        public String   SCMSG_DeleteSuccess           { get; set; }
        public String   SCMSG_DeleteFail              { get; set; }

        public String   SC_SelectButtonText           { get; set; }  
        public String   SC_UploadButtonText           { get; set; }        
        public String   SC_DeleteButtonText           { get; set; }  
 
        public String   SC_ImageName                  { get; set; }
     
        public WidControlImagerUploader()
        {
            SC_MaxFileSize                  = ctDEFMaxFileSize;
            SC_MaxFileCount                 = ctDEFMaxFileCount;

            SCMSG_ExceedMaxFileCount        = ctDEFExceedMaxFileCount;
            SCMSG_NoFileSelected            = ctDEFNoFileSelected;
            SCMSG_InvalidFileType           = ctDEFInvalidFileType;
            SCMSG_FileTooLarge              = ctDEFFileTooLarge;
            SCMSG_UploadSuccess             = ctDEFUploadSuccess;
            SCMSG_UploadFail                = ctDEFUploadFail;
            SCMSG_DeleteConfirmation        = ctDEFDeleteConfirmation;
            SCMSG_DeleteSuccess             = ctDEFDeleteSuccess;
            SCMSG_DeleteFail                = ctDEFDeleteFail;

            SC_SelectButtonText             = ctDEFSelectButtonText;
            SC_UploadButtonText             = ctDEFUploadButtonText;                        
            SC_DeleteButtonText             = ctDEFDeleteButtonText;            
        }

        public void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.System, ctWidControlImageUploaderStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.System, ctWidControlImageUploaderScript));
        }

        protected void AddControlStyle(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSWidControlImagerUploader);
            paComponentController.AddElementType(ComponentController.ElementType.Control);                                   
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_MaxFileSize, SC_MaxFileSize.ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_MaxFileCount, SC_MaxFileCount.ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ExtensionList, ctDEFExtensionList);            
            
            paComponentController.AddMessageTemplate(ComponentController.ElementMessageTemplate.ma_ExceedMaxFileCount, SCMSG_ExceedMaxFileCount);
            paComponentController.AddMessageTemplate(ComponentController.ElementMessageTemplate.ma_NoFileSelected, SCMSG_NoFileSelected);
            paComponentController.AddMessageTemplate(ComponentController.ElementMessageTemplate.ma_InvalidFileType, SCMSG_InvalidFileType);
            paComponentController.AddMessageTemplate(ComponentController.ElementMessageTemplate.ma_FileTooLarge, SCMSG_FileTooLarge);
            paComponentController.AddMessageTemplate(ComponentController.ElementMessageTemplate.ma_UploadSuccessful, SCMSG_UploadSuccess);
            paComponentController.AddMessageTemplate(ComponentController.ElementMessageTemplate.ma_UploadFail, SCMSG_UploadFail);
            paComponentController.AddMessageTemplate(ComponentController.ElementMessageTemplate.ma_DeleteConfirmation, SCMSG_DeleteConfirmation);
            paComponentController.AddMessageTemplate(ComponentController.ElementMessageTemplate.ma_DeleteSuccessful, SCMSG_DeleteSuccess);
            paComponentController.AddMessageTemplate(ComponentController.ElementMessageTemplate.ma_DeleteFail, SCMSG_DeleteFail);            
        }

        protected virtual void RenderImageDiv(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Element);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSImageContainer);   
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSImage);   
            paComponentController.AddAttribute(HtmlAttribute.Src, SC_ImageName);   
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();
            paComponentController.RenderEndTag();            
        }

        protected void RenderButtonPanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);   
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSelectButton);
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDSelect);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(SC_SelectButtonText);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSUploadButton);               
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDUpload);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(SC_UploadButtonText);
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSDeleteButton);               
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDDelete);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(SC_DeleteButtonText);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();            
        }       

        protected void RenderFileInputElement(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSFileInput);
            paComponentController.AddAttribute(HtmlAttribute.Type, "file");
            paComponentController.AddAttribute(HtmlAttribute.Accept, "image/*");
            paComponentController.RenderBeginTag(HtmlTag.Input);
            paComponentController.RenderEndTag();
        }

        protected void RenderBrowserMode(ComponentController paComponentController)
        {
            IncludeExternalLinkFiles(paComponentController);

            AddControlStyle(paComponentController);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            RenderImageDiv(paComponentController);
            RenderFileInputElement(paComponentController);
            RenderButtonPanel(paComponentController);
            paComponentController.RenderEndTag();
        }

        protected void RenderDesignMode(ComponentController paComponentController)
        {
            paComponentController.AddStyle(CSSStyle.Border, "2px Solid Black");
            paComponentController.AddStyle(CSSStyle.Height, this.Height.ToString());
            paComponentController.AddStyle(CSSStyle.Width, this.Width.ToString());
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(this.GetType().ToString());
            paComponentController.RenderEndTag();
        }

        public virtual void RenderChildMode(ComponentController paComponentController, String paRenderMode = null)
        {
            RenderBrowserMode(paComponentController);
        }
    }    
}
