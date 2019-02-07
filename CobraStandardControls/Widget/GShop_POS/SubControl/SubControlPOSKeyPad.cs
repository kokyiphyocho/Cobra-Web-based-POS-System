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
using System.Text.RegularExpressions;


namespace CobraStandardControls
{
    public class SubControlPOSKeyPad : WebControl, WidgetControlInterface
    {
        const String ctSubControlPOSKeyPadStyle         = "SubControlPOSKeyPad.css";
        const String ctSubControlPOSKeyPadScript        = "SubControlPOSKeyPad.js";

        const String ctCLSSubControlPOSKeyPad           = "SubControlPOSKeyPad";
        const String ctCLSPrimaryPanel                  = "PrimaryPanel";

        const String ctCLSKeyPadPanel                   = "KeyPadPanel";
        const String ctCLSSideKeyDiv                    = "SideKeyDiv";
        const String ctCLSMiddleKeyDiv                  = "MiddleKeyDiv";

        const String ctCLSLCDPanelBar                   = "LCDPanelBar";
        const String ctCLSLCDPanel                      = "LCDPanel";
        const String ctCLSLCDScreen                     = "LCDScreen";

        const String ctCLSTopKeyPanel                   = "TopKeyPanel";
        const String ctCLSTopKeyDiv                     = "TopKeyDiv";

        const String ctCLSBottomKeyPanel                = "BottomKeyPanel";
        const String ctCLSBottomKeyDiv                  = "BottomKeyDiv";

      //  const String ctKeyPad                           = "keypad";

        const String ctPNLLCDPanel                      = "LCDPanel";
        const String ctPNLLeftPanel                     = "LeftPanel";
        const String ctPNLRightPanel                    = "RightPanel";
        const String ctPNLMiddlePanel                   = "MiddlePanel";
        const String ctPNLTopPanel                      = "TopPanel";
        const String ctPNLBottomPanel                   = "BottomPanel";

        const String    ctRegExImageResource            = "[{][{](?<ResourceName>[^}]*)[}][}]";
        const String    ctGRPResourceName               = "ResourceName";
                
        KeyPadManager       clKeyPadManager;
        LanguageManager     clLanguageManager;
        String              clKeyPadName;

        public CompositeFormInterface SCI_ParentForm { get; set; }


        public SubControlPOSKeyPad(String paKeypadName)
        {
            clKeyPadName = paKeypadName.ToLower();

            clLanguageManager = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctSubControlPOSKeyPadStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctSubControlPOSKeyPadScript));
        }        

        private void RenderKey(ComponentController paComponentController, KeyPadRow paKeyData)
        {
            Match lcMatch;
            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, paKeyData.KeyCommand);
            paComponentController.AddAttribute(HtmlAttribute.Class, paKeyData.CssClass);
            paComponentController.AddAttribute(HtmlAttribute.Style, paKeyData.InlineCss);
            paComponentController.RenderBeginTag(HtmlTag.A);

            if ((lcMatch = Regex.Match(paKeyData.KeyText, ctRegExImageResource)).Success)
            {
                paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(lcMatch.Groups[ctGRPResourceName].Value));
                paComponentController.RenderBeginTag(HtmlTag.Img);
                paComponentController.RenderEndTag();
            }
            else paComponentController.Write(paKeyData.KeyText);
            paComponentController.RenderEndTag();                                
        }

        private void RenderKeySectionDiv(ComponentController paComponentController, DataRow[] paKeyList, String paPanelClass)
        {
            KeyPadRow   lcKeyPadRow;

            paComponentController.AddAttribute(HtmlAttribute.Class, paPanelClass);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            if (paKeyList != null)
            {
                lcKeyPadRow = new KeyPadRow(null);

                for (int lcCount = 0; lcCount < paKeyList.Length; lcCount++)
                {
                    lcKeyPadRow.Row = paKeyList[lcCount];
                    RenderKey(paComponentController, lcKeyPadRow);
                }
            }            

            paComponentController.RenderEndTag();
        }

        private void RenderKeyPad(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSKeyPadPanel);
            paComponentController.AddElementType(ComponentController.ElementType.Panel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderKeySectionDiv(paComponentController, clKeyPadManager.GetPanelKeyList(ctPNLLeftPanel), ctCLSSideKeyDiv);
            RenderKeySectionDiv(paComponentController, clKeyPadManager.GetPanelKeyList(ctPNLMiddlePanel), ctCLSMiddleKeyDiv);
            RenderKeySectionDiv(paComponentController, clKeyPadManager.GetPanelKeyList(ctPNLRightPanel), ctCLSSideKeyDiv);            

            paComponentController.RenderEndTag();
        }

        private void RenderLCDPanelBar(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSLCDPanelBar);
            paComponentController.AddElementType(ComponentController.ElementType.Panel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderKeySectionDiv(paComponentController, clKeyPadManager.GetPanelKeyList(ctPNLLCDPanel), ctCLSLCDPanel);

            paComponentController.RenderEndTag();

        }

        private void RenderTopKeyBar(ComponentController paComponentController)
        {
            DataRow[]     lcTopBarKeyList;

            lcTopBarKeyList = clKeyPadManager.GetPanelKeyList(ctPNLTopPanel);

            if ((lcTopBarKeyList != null) && (lcTopBarKeyList.Length > 0))
            {
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTopKeyPanel);
                paComponentController.AddElementType(ComponentController.ElementType.Panel);
                paComponentController.RenderBeginTag(HtmlTag.Div);

                RenderKeySectionDiv(paComponentController, lcTopBarKeyList, ctCLSTopKeyDiv);

                paComponentController.RenderEndTag();
            }
        }

        private void RenderBottomKeyBar(ComponentController paComponentController)
        {
            DataRow[]     lcBottomBarKeyList;

            lcBottomBarKeyList = clKeyPadManager.GetPanelKeyList(ctPNLBottomPanel);

            if ((lcBottomBarKeyList != null) && (lcBottomBarKeyList.Length > 0))
            {
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSBottomKeyPanel);
                paComponentController.AddElementType(ComponentController.ElementType.Panel);
                paComponentController.RenderBeginTag(HtmlTag.Div);

                RenderKeySectionDiv(paComponentController, lcBottomBarKeyList, ctCLSBottomKeyDiv);

                paComponentController.RenderEndTag();
            }
        }
   
        private void RenderBrowserMode(ComponentController paComponentController)
        {            
            IncludeExternalLinkFiles(paComponentController);
                        
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSubControlPOSKeyPad);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, clKeyPadName);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Language, clLanguageManager.ActiveRow.Language.ToLower());
            paComponentController.AddElementType(ComponentController.ElementType.Composite);            
            paComponentController.RenderBeginTag(HtmlTag.Div);

            if ((clKeyPadManager = new KeyPadManager(clKeyPadName)) != null)
            {
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPrimaryPanel);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                
                RenderTopKeyBar(paComponentController);
                RenderLCDPanelBar(paComponentController);
                RenderKeyPad(paComponentController);
                
                paComponentController.RenderEndTag();

                RenderBottomKeyBar(paComponentController);
            }

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
