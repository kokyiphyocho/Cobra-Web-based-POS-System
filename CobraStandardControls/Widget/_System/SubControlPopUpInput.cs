using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CobraFrame;
using CobraFoundation;
using CobraWebFrame;
using CobraResources;
using CobraBusinessFrame;
using System.Text.RegularExpressions;


namespace CobraStandardControls
{
    public class SubControlPopUpInput : WebControl, WidgetControlInterface
    {
        const String ctSubControlPopUpInputStyle                = "SubControlPopUpInput.css";
        const String ctSubControlPopUpInputScript               = "SubControlPopUpInput.js";

        const String ctCLSSubControlPopUpInputComposite         = "SubControlPopUpInputComposite";
        const String ctCLSSubControlPopUpInput                  = "SubControlPopUpInput";
        const String ctCLSHeaderBar                             = "HeaderBar";
        const String ctCLSInfoContainer                         = "InfoContainer";
        const String ctCLSInfoRow                               = "InfoRow";
        const String ctCLSMessageBar                            = "MessageBar";

        const String ctCLSButtonPanel                           = "ButtonPanel";
        const String ctCLSButton                                = "Button";

        const String ctCMDUpdateInfo                            = "@popupcmd%update";
        const String ctCMDCancelInfo                            = "@popupcmd%cancel";
        
        LanguageManager         clLanguageManager;
        SettingManager          clSettingManager;
        DataRow                 clDataRow;        
        InputInfoManager        clInputInfoManager;
        String                  clTypeID;
        bool                    clMessageBar;


        public CompositeFormInterface SCI_ParentForm { get; set; }
        public String                 SC_MessageCode { get; set; }

        public event InputInfoManager.DelegateCustomComponentRenderer CustomComponentRenderer
        {
            add { clInputInfoManager.CustomComponentRenderer += value; }
            remove { clInputInfoManager.CustomComponentRenderer -= value; }
        }

        public SubControlPopUpInput(String paTypeID, String paInputInfoGroup, DataRow paDataRow, bool paMessageBar)
        {
            clDataRow = paDataRow;
            clInputInfoManager = new InputInfoManager(paInputInfoGroup);
            clTypeID = paTypeID;
            clMessageBar = paMessageBar;
            SC_MessageCode = String.Empty;
                        
            clSettingManager = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;
            clLanguageManager = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
        }
        

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.System, ctSubControlPopUpInputStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.System, ctSubControlPopUpInputScript));
        }   

        private void RenderInfoContainer(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInfoContainer);
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            clInputInfoManager.RenderAllSubGroups(paComponentController, null);
            
            paComponentController.RenderEndTag();
        }

        private void RenderButtonPanel(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonPanel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDCancelInfo);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.remove));
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDUpdateInfo);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.ok));
            paComponentController.RenderEndTag();            

            paComponentController.RenderEndTag();
        }

        private void RenderMessageBar(ComponentController paComponentController)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_MessageCode, SC_MessageCode);
            paComponentController.AddElementType(ComponentController.ElementType.MessageBar);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSMessageBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.RenderEndTag();
        }

        private void RenderBrowserMode(ComponentController paComponentController)
        {            
            IncludeExternalLinkFiles(paComponentController);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSubControlPopUpInputComposite);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Language, clLanguageManager.ActiveRow.Language.ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, clTypeID);
            paComponentController.AddElementType(ComponentController.ElementType.PopUp);            
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSubControlPopUpInput);
            paComponentController.AddElementType(ComponentController.ElementType.Panel);          
            paComponentController.RenderBeginTag(HtmlTag.Div);    
            
            RenderInfoContainer(paComponentController);
            RenderButtonPanel(paComponentController);

            if (clMessageBar) RenderMessageBar(paComponentController);

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderDesignMode(ComponentController paComponentController)
        {
            paComponentController.AddStyle(CSSStyle.Border, "2px Solid Black");
            paComponentController.AddStyle(CSSStyle.Height, this.Height.ToString());
            paComponentController.AddStyle(CSSStyle.Width, this.Width.ToString());
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(this.GetType().ToString());
            paComponentController.RenderEndTag();
        }

        public void RenderChildMode(ComponentController paComponentController, String paRenderMode = null)
        {
            RenderBrowserMode(paComponentController);
        }

        protected override void Render(HtmlTextWriter paHtmlTextWriter)
        {
            if (!DesignMode) RenderBrowserMode(new ComponentController(paHtmlTextWriter));
            else RenderDesignMode(new ComponentController(paHtmlTextWriter));
        }
    }
}


