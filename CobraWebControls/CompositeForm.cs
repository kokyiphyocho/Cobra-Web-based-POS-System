using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Web.UI.WebControls;
using CobraFoundation;
using CobraFrame;
using CobraWebFrame;
using CobraResources;
using CobraWidgetFrame;

namespace CobraWebControls
{
    [ToolboxData("<{0}:CompositeForm runat=server></{0}:CompositeForm>")]
    public class CompositeForm : WebControl, CompositeFormInterface
    {
        protected const String ctToolBarScript              = "ToolBarScript.js";
        protected const String ctCLSPanel                   = "FormPanel";
        protected const String ctCLSFormTitleBar            = "FormTitleBar";
        protected const String ctCLSElementContainer        = "ElementContainer";        
        protected const String ctCLSFormCloseButtonDiv      = "FormCloseButtonDiv";
        protected const String ctCLSFormCloseButton         = "FormCloseButton";
        protected const String ctCLSLogOutButton            = "LogOutButton";
        protected const String ctCLSBufferDiv               = "BufferDiv";
        protected const String ctCLSMessageRepositoryDiv    = "MessageRepositoryDiv";
        protected const String ctCLSPasswordDiv             = "PasswordDiv";
        protected const String ctCLSLabelDiv                = "LabelDiv";
        protected const String ctCLSPasswordInputDiv        = "PasswordInputDiv";
        protected const String ctCLSStatusDiv               = "StatusDiv";
        protected const String ctCLSCloseButtonDiv          = "CloseButtonDiv";
        protected const String ctCLSVerifyButtonDiv         = "VerifyButtonDiv";
        protected const String ctCLSLoadingOverlayDiv       = "LoadingOverlayDiv";
        
        protected const String ctCMDFormClose               = "@cmd%formclose";
        protected const String ctCMDClosePassword           = "@cmd%closepassword";
        protected const String ctCMDVerifyPassword          = "@cmd%verifypassword";
        protected const String ctCMDLogOut                  = "@cmd%logout";
        protected const String ctCMDPrinterStatus           = "@cmd%printerstatus";

        protected const String ctICORightChevron            = "rtchevronplain.png";
        protected const String ctICOLogOut                  = "logout.png";
        protected const String ctICOPrinter                 = "printericon.png";

        protected const String ctAJAXIndicatorBig           = "AJAX_IndicatorBigCircle.gif";
        protected const String ctAJAXSquares                = "AJAX_Squares.gif";
        
        
        protected const String ctIMGAjaxLoadingImage        = "Images/AjaxLoading.Gif";

        protected const String ctWallPaperPath              = "/images/background";
        protected const String ctDefaultApplicationTitle    = "*";

        protected const int ctFixComponentHeight            = 100;

        protected const String ctDYTPasswordMessage         = "@@*.Form.PasswordMessage";
        
        public String   SC_FormName                         { get; set; }        
                               
        FormInfoManager clFormInfoManager;
        LanguageManager clLanguageManager;
        SettingManager  clSettingManager;
                        
        public CompositeForm()
        {
            SC_FormName         = String.Empty;

            if (ApplicationFrame.GetInstance().ActiveSubscription != null)
            {
                clLanguageManager = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
                clSettingManager = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;
            }
        }

        public CompositeForm(FormInfoManager paFormInfoManager)
        {
            clFormInfoManager   = paFormInfoManager;            
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {            
            JavaScriptManager lcJavaScriptmanager;
         
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);
         
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetToolBarScriptUrl(ctToolBarScript));
        }
        
        protected void RenderStatusIcon(ComponentController paComponentController)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDPrinterStatus);
            paComponentController.AddElementType(ComponentController.ElementType.StatusControl);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "icon");
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOPrinter));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, "ajax");
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetAjaxLoaderImageUrl(ctAJAXSquares));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private String GetApplicationTitle()
        {
            String  lcApplicationTitle;

            if ((lcApplicationTitle = clSettingManager.ApplicationTitle.GetData(clLanguageManager.ActiveRow.Language.ToLower())) == null)
                return (clFormInfoManager.TranslateString(clSettingManager.ApplicationTitle.GetData(ctDefaultApplicationTitle, String.Empty)));
            else return (lcApplicationTitle);
        }

        protected void RenderTitleBar(ComponentController paComponentController)
        {            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSFormTitleBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            if (clFormInfoManager.IsAttributeSet(FormInfoManager.FormAttribute.PrinterStatus)) RenderStatusIcon(paComponentController);
            
            paComponentController.Write(GetApplicationTitle());            
            RenderCloseButton(paComponentController);

            if (ApplicationFrame.GetInstance().ActiveSessionController.User.IsLoggedIn)
                RenderLogOutButton(paComponentController);   
            paComponentController.RenderEndTag();
        }

        public void RenderToolBar(ComponentController paComponentController)
        {
            CompositeToolBar lcToolBar;
             
            if (!String.IsNullOrEmpty(clFormInfoManager.ActiveRow.ToolBar))
            {
                lcToolBar = new CompositeToolBar(clFormInfoManager, clFormInfoManager.ActiveRow.ToolBar);
                lcToolBar.RenderChildMode(paComponentController);
            }
        }

        protected void RenderCloseButton(ComponentController paComponentController)
        {            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSFormCloseButtonDiv);         
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSFormCloseButton);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDFormClose);            
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(ComponentController.UnicodeStr((int) Fontawesome.remove));
            paComponentController.RenderEndTag();
            paComponentController.RenderEndTag();
        }

        protected void RenderLogOutButton(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSLogOutButton);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDLogOut);                        
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOLogOut));
            paComponentController.RenderBeginTag(HtmlTag.Img);            
            paComponentController.RenderEndTag();
            paComponentController.RenderEndTag();
        }

        protected void RenderElementContainer(ComponentController paComponentController)
        {
            WidgetRenderingManager lcWidgetRenderingManager;

            lcWidgetRenderingManager = new WidgetRenderingManager(this, clFormInfoManager, paComponentController);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSElementContainer);
            paComponentController.AddStyle(CSSStyle.Height, clFormInfoManager.ActiveRow.ContainerHeight.ToString() + ConfigClass.GetInstance().StandardYUnit);            
            paComponentController.RenderBeginTag(HtmlTag.Div);
            lcWidgetRenderingManager.RenderWidget();
            paComponentController.RenderEndTag();
        }       
 
        protected FormInfoManager GetFormInfoManager()
        {
            if (clFormInfoManager == null)
            {
                if (SC_FormName != null)
                {                    
                    clFormInfoManager = FormInfoManager.CreateInstance(ApplicationFrame.GetInstance().ActiveGlobalMetaBlock.TranslateGlobalMetaVariables(SC_FormName));
                }
            }

            return (clFormInfoManager);
        }        
        
        protected void AddFormStyle(ComponentController paComponentController)
        {
            String lcFormCSSClass;
            String lcCustomWallPaper;

            lcFormCSSClass = String.IsNullOrWhiteSpace(clFormInfoManager.ActiveRow.FormCSSClass) ? clFormInfoManager.ActiveRow.RenderMode : clFormInfoManager.ActiveRow.FormCSSClass;
            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPanel + " " + lcFormCSSClass);
            paComponentController.AddElementType(ComponentController.ElementType.Form);

            if (ApplicationFrame.GetInstance().ActiveSubscription.IsDemoMode())
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DemoMode, "true");

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_FormProtocolList, General.Base64Encode(clSettingManager.FormProtocolListStr));
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ServiceRequestToken, ApplicationFrame.GetInstance().ActiveServiceRequestToken);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_FormStack, ApplicationFrame.GetInstance().FormStack);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_FormName, clFormInfoManager.ActiveRow.FormName);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_EncodedFormName, clFormInfoManager.EncodedFormName);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_DataGroupName, clFormInfoManager.ActiveRow.DataGroup);            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_LandingPage, General.Base64Encode(ApplicationFrame.GetInstance().ActiveSubscription.GetLandingPage()));
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Language, ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage.ActiveRow.Language.ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_LocalNumberMode, ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting.LocalNumberMode.ToString().ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_LocalDigits, ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage.LocalDigits);
            paComponentController.AddStyle(CSSStyle.Width, clFormInfoManager.ActiveRow.FormWidth.ToString() + ConfigClass.GetInstance().StandardXUnit);
            paComponentController.AddStyle(CSSStyle.Height, clFormInfoManager.ActiveRow.FormHeight.ToString() + ConfigClass.GetInstance().StandardYUnit);

            if ((lcCustomWallPaper = ApplicationFrame.GetInstance().ActiveSubscription.GetCustomWallPaper(clFormInfoManager.ActiveRow.FormName)) != null)
            {
                if (lcCustomWallPaper.ToLower().Contains(ctWallPaperPath)) SetCustomBackground(paComponentController, lcCustomWallPaper);
                else paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_desktopbackgroundcss, lcCustomWallPaper);
            }
        }

        private void SetCustomBackground(ComponentController paComponentController, String paCustomWallpaper)
        {
            paComponentController.AddStyle(CSSStyle.Background, paCustomWallpaper);            
            paComponentController.AddStyle("-webkit-background-size", "cover");
            paComponentController.AddStyle("-moz-background-size", "cover");
            paComponentController.AddStyle("-o-background-size", "cover");
            paComponentController.AddStyle("background-size", "cover");
        }

        private void RenderBufferDiv(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Buffer);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSBufferDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.RenderEndTag();
        }

        private DataTable RetrieveMessageList()
        {
            QueryClass lcQuery;

            lcQuery = new QueryClass(QueryClass.QueryType.RetrieveMessageList);
            lcQuery.ReplacePlaceHolder("$LANGUAGE", ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage.ActiveRow.Language.ToLower(), true);
            lcQuery.ReplacePlaceHolder("$FORMNAME", clFormInfoManager.ActiveRow.FormName, true);
            clFormInfoManager.ReplaceQueryPlaceHolder(lcQuery);

            return (lcQuery.RunQuery());
        }

        private void RenderMessageRepositoryDiv(ComponentController paComponentController)
        {
            DataTable       lcMessageList;
            MessageRow      lcMessageRow;

            if ((lcMessageList = RetrieveMessageList()) != null)
            {
                lcMessageRow = new MessageRow(null);

                paComponentController.AddElementType(ComponentController.ElementType.MessageRepository);
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSMessageRepositoryDiv);
                paComponentController.RenderBeginTag(HtmlTag.Div);

                for (int lcCount = 0; lcCount < lcMessageList.Rows.Count; lcCount++ )
                {
                    lcMessageRow.Row = lcMessageList.Rows[lcCount];

                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Code, lcMessageRow.MessageCode.ToLower());
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Button1, lcMessageRow.Button1);
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Button2, lcMessageRow.Button2);
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Title, lcMessageRow.Title);
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, lcMessageRow.MessageType.ToLower());
                    paComponentController.RenderBeginTag(HtmlTag.Span);
                    paComponentController.Write(lcMessageRow.Message);
                    paComponentController.RenderEndTag();
                }

                paComponentController.RenderEndTag();
            }            
        }

        protected void RenderPasswordDiv(ComponentController paComponentController)
        {            
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPasswordDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCloseButtonDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDClosePassword);            
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.remove));
            paComponentController.RenderEndTag();
            
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSLabelDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(ctDYTPasswordMessage));
            paComponentController.RenderEndTag();
            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPasswordInputDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_InputMode, "number");
            paComponentController.AddAttribute(HtmlAttribute.Type, "password");
            paComponentController.RenderBeginTag(HtmlTag.Input);
            paComponentController.RenderEndTag();
            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSVerifyButtonDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDVerifyPassword);                        
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICORightChevron));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.AddElementType(ComponentController.ElementType.AjaxLoaderStatusDisplay);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetAjaxLoaderImageUrl(ctAJAXIndicatorBig));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
            
            paComponentController.AddElementType(ComponentController.ElementType.StatusControl);            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSStatusDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);            
            paComponentController.RenderEndTag();
            
            paComponentController.RenderEndTag();
        }        

        protected void RenderBrowserMode(ComponentController paComponentController)
        {
            CSSStyleManager         lcCSSStyleManager;
            
            
            if ((clFormInfoManager = GetFormInfoManager()) != null)
            {
                lcCSSStyleManager = new CSSStyleManager(paComponentController);                

                IncludeExternalLinkFiles(paComponentController);
                AddFormStyle(paComponentController);

                paComponentController.RenderBeginTag(HtmlTag.Div);

                if (!clFormInfoManager.IsAttributeSet(FormInfoManager.FormAttribute.NoTitle))
                    RenderTitleBar(paComponentController);

                if (!clFormInfoManager.IsAttributeSet(FormInfoManager.FormAttribute.NoToolBar))
                    RenderToolBar(paComponentController);

                RenderElementContainer(paComponentController);
                RenderBufferDiv(paComponentController);
                RenderMessageRepositoryDiv(paComponentController);
                RenderPasswordDiv(paComponentController);                

                paComponentController.RenderEndTag();
                
                if (!String.IsNullOrEmpty(clFormInfoManager.ActiveRow.FormCSSFile)) 
                    lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetPolymorphicStyleSheetUrl(clFormInfoManager.ActiveRow.FormCSSFile));
            }
        }

        protected void RenderControlContent(ComponentController paComponentController, String paRenderMode)
        {
            WidgetRenderingManager lcWidgetRenderingManager;

            lcWidgetRenderingManager = new WidgetRenderingManager(this, clFormInfoManager, paComponentController);
            
            lcWidgetRenderingManager.RenderWidget(paRenderMode);   
        }

        protected void RenderDesignMode(ComponentController paComponentController)
        {
            paComponentController.AddStyle(CSSStyle.Border, "2px Solid Black");
            paComponentController.AddStyle(CSSStyle.Width, "100px");
            paComponentController.AddStyle(CSSStyle.Height, "100px");
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write("Design Mode");
            paComponentController.RenderEndTag();
        }

        public String GetSerializedControl(bool paControlContent = false, String paRenderMode = null)
        {
            HtmlTextWriter  lcHtmlTextWriter;
            StringWriter    lcStringWriter;
            StringBuilder   lcStringBuilder;
            ComponentController lcComponentController;

            lcStringBuilder = new StringBuilder();
            lcStringWriter = new StringWriter(lcStringBuilder);
            lcHtmlTextWriter = new HtmlTextWriter(lcStringWriter);
            lcComponentController = new ComponentController(lcHtmlTextWriter);

            if (paControlContent) RenderControlContent(lcComponentController, paRenderMode);
            else RenderBrowserMode(lcComponentController);

            return (lcStringWriter.ToString());
        }  

        protected override void Render(HtmlTextWriter paHtmlTextWriter)
        {
            if (!DesignMode) RenderBrowserMode(new ComponentController(paHtmlTextWriter));
            else RenderDesignMode(new ComponentController(paHtmlTextWriter));
        }
    }
}