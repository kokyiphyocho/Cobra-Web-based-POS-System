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
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using CobraBusinessFrame;
using Newtonsoft.Json.Linq;

namespace CobraStandardControls
{
    public class SubControlPOSReceiptOutput : WebControl, WidgetControlInterface
    {
        const String ctSubControlPOSReceiptOutputStyle      = "SubControlPOSReceiptOutput.css";
        const String ctSubControlPOSReceiptOutputScript     = "SubControlPOSReceiptOutput.js";
        const String ctCanvasScript                         = "Canvas.js";
       // const String ctPrinterManagerScript                 = "PrinterManager.js";

        const String ctCLSSubControlPOSReceiptOutput        = "SubControlPOSReceiptOutput";

        const String ctCLSTitleBar                          = "TitleBar";
        const String ctCLSButtonBar                         = "ButtonBar";
        const String ctCLSButtonIcon                        = "ButtonIcon";
        const String ctCLSButtonIconImage                   = "ButtonIconImage";

        const String ctSETReceiptLayout                     = "POS.ReceiptLayoutInfo.Layout"; // Virutal Setting.
        const String ctSETReceiptLayoutSetting              = "POS.ReceiptLayoutInfo.LayoutSetting";
        const String ctSETReceiptCustomization              = "POS.ReceiptLayoutInfo.Customization";
        const String ctSETPrimaryPrinterSetting             = "POS.PrimaryPrinterSetting";

        const String ctDYTPreviewTitle                      = "@@POS.ReceiptOutput.PreviewTitle";

        const String ctKEYLayoutName                        = "LayoutName";
        const String ctKEYReceiptWidth                      = "ReceiptWidth";
        const String ctKEYPrinterWidth                      = "PrinterWidth";

        const String ctDynamicTextPrefix                    = "@@";
        
        const String ctPrinterName                          = "PrinterName";

        const String ctTYPReceiptBaseInfo                   = "receiptbaseinfo";
        const String ctTYPReceiptCanvas                     = "receiptcanvas";
        const String ctTYPPreviewPanel                      = "preview";
        const String ctTYPReceiptLogo                       = "receiptlogo";
        
        const String ctICOPrinter                           = "ToolBar_Printer.png";

        const String ctCMDPopUpClose                        = "@cmd%popupclose";
        const String ctCMDPrint                             = "@cmd%print";
                
        const int ctDEFCanvasWidth                          = 500;
        const int ctDEFCanvasHeight                         = 1000;
 
        LanguageManager                         clLanguageManager;
        SettingManager                          clSetting;        
        Dictionary<String, dynamic>             clReceiptLayoutSetting;
        Dictionary<String, dynamic>             clReceiptActiveLayout;
        Dictionary<String, dynamic>             clReceiptCustomization;
        Dictionary<String, dynamic>             clPrimaryPrinterSetting;        
        String                                  clPrimaryPrinterName;
        
        public CompositeFormInterface SCI_ParentForm { get; set; }

        public SubControlPOSReceiptOutput()
        {   
            clLanguageManager   = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clSetting           = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting;

            LoadLayoutDictionary();            
        }

        private void LoadLayoutDictionary()
        {
            List<String>            lcKeyList;            
            
            clReceiptLayoutSetting  = General.JSONDeserialize<Dictionary<String,dynamic>>(clSetting.GetSettingValue(ctSETReceiptLayoutSetting, "{}"));
            clReceiptCustomization  = General.JSONDeserialize<Dictionary<String,dynamic>>(clSetting.GetSettingValue(ctSETReceiptCustomization, "{}"));
            clPrimaryPrinterSetting = General.JSONDeserialize<Dictionary<String, dynamic>>(clSetting.GetSettingValue(ctSETPrimaryPrinterSetting, "{}"));
            clPrimaryPrinterName    = clPrimaryPrinterSetting.GetData(ctPrinterName, String.Empty);
            
            if (clReceiptLayoutSetting.Keys.Contains(ctKEYLayoutName))            
                clReceiptActiveLayout = General.JSONDeserialize<Dictionary<String, dynamic>>(clSetting.GetSettingValue(clReceiptLayoutSetting[ctKEYLayoutName], "{}"));            
            else clReceiptActiveLayout = new Dictionary<String, dynamic>();

            lcKeyList = clReceiptActiveLayout.Keys.ToList();

            foreach (String lcKey in lcKeyList)
            {
                if (clReceiptActiveLayout[lcKey].GetType() == typeof(JObject)) ReplaceDynamicText(clReceiptActiveLayout[lcKey]);
                else
                {
                    if (clReceiptActiveLayout[lcKey].ToString().StartsWith(ctDynamicTextPrefix))
                        clReceiptActiveLayout[lcKey] = clLanguageManager.GetText(clReceiptActiveLayout[lcKey].ToString());
                }
            }

            lcKeyList = clReceiptCustomization.Keys.ToList();

            foreach(String lcKey in lcKeyList)
            {
               // if (clReceiptCustomization[lcKey].ToString().StartsWith(ctUploadPathPlaceHolder))
              clReceiptCustomization[lcKey] = UploadManager.GetInstance().ReplaceUploadPath(clReceiptCustomization[lcKey].ToString());
            }
        }

        private void ReplaceDynamicText(JObject paJObject)
        {
            List<String> lcKeyList;

            lcKeyList = paJObject.Properties().Select(p => p.Name).ToList();

            
            foreach (String lcKey in lcKeyList)
            {
                if (paJObject[lcKey].ToString().StartsWith(ctDynamicTextPrefix))
                    paJObject[lcKey] = clLanguageManager.GetText(paJObject[lcKey].ToString());
            }            
        }

        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetFoundationScriptUrl(ctCanvasScript));

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_POS, ctSubControlPOSReceiptOutputStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_POS, ctSubControlPOSReceiptOutputScript));
            // lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetFoundationScriptUrl(ctPrinterManagerScript));

            IncludePrinterControllerScriptList(paComponentController, lcJavaScriptmanager);
            IncludeDeviceScriptList(paComponentController, lcJavaScriptmanager);
        }

        private void IncludePrinterControllerScriptList(ComponentController paComponentController, JavaScriptManager paJavaScriptManager)
        {
            String[] lcScriptList;

            lcScriptList = PrinterController.GetInstance().GetPrinterControllerScriptList();

            if (lcScriptList != null)
            {
                for (int lcCount = 0; lcCount < lcScriptList.Length; lcCount++)
                {
                    if (!String.IsNullOrEmpty(lcScriptList[lcCount]))
                        paJavaScriptManager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetFoundationScriptUrl(lcScriptList[lcCount]));
                }
            }
        }

        private void IncludeDeviceScriptList(ComponentController paComponentController, JavaScriptManager paJavaScriptManager)
        {
            String[] lcScriptList;
            
            lcScriptList = PrinterController.GetInstance().GetDeviceScriptList(clPrimaryPrinterName);
            
            if (lcScriptList != null)
            {
                for (int lcCount = 0; lcCount < lcScriptList.Length; lcCount++)
                {
                    if (!String.IsNullOrEmpty(lcScriptList[lcCount]))
                        paJavaScriptManager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetFoundationScriptUrl(lcScriptList[lcCount]));
                }
            }
        }       

        private void RenderCanvas(ComponentController paComponentController)
        {
            int  lcReceiptWidth;
            

            if ((lcReceiptWidth = Convert.ToInt32(clReceiptLayoutSetting.GetData(ctKEYReceiptWidth, ctDEFCanvasWidth))) == 0)
            {
                lcReceiptWidth = Convert.ToInt32(clPrimaryPrinterSetting.GetData(ctKEYPrinterWidth, ctDEFCanvasWidth));
            }
            
            paComponentController.AddAttribute(HtmlAttribute.Width, lcReceiptWidth.ToString());           
            paComponentController.AddAttribute(HtmlAttribute.Height, ctDEFCanvasHeight.ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctTYPReceiptCanvas);
            paComponentController.RenderBeginTag(HtmlTag.Canvas);
            paComponentController.RenderEndTag();         
        }

        private void RenderPreviewPanelTitleBar(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.Title);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSTitleBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);
                        
            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(clLanguageManager.GetText(ctDYTPreviewTitle));
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDPopUpClose);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.remove));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        public void RenderPreviewPanelToolBar(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.ControlBar);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonIcon);            
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDPrint);
            paComponentController.RenderBeginTag(HtmlTag.A);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSButtonIconImage);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetToolBarImageUrl(ctICOPrinter));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderPreviewPanel(ComponentController paComponentController)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctTYPPreviewPanel);
            paComponentController.AddElementType(ComponentController.ElementType.Panel);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderPreviewPanelTitleBar(paComponentController);
            RenderPreviewPanelToolBar(paComponentController);
            paComponentController.AddElementType(ComponentController.ElementType.Container);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderBrowserMode(ComponentController paComponentController)
        {   
            String                  lcReceiptLayoutSetting;
            String                  lcReceiptLayout;
            String                  lcReceiptCustomization;

            //if (!String.IsNullOrEmpty(clPrimaryPrinterName))
            //{
            lcReceiptLayoutSetting  = General.Base64Encode(General.JSONSerialize(clReceiptLayoutSetting));
            lcReceiptCustomization  = General.Base64Encode(General.JSONSerialize(clReceiptCustomization));
            lcReceiptLayout         = General.Base64Encode(General.JSONSerialize(clReceiptActiveLayout));

            IncludeExternalLinkFiles(paComponentController);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Decimal, clSetting.CurrencyDecimalPlace.ToString());
            paComponentController.AddElementAttribute(ctSETReceiptLayoutSetting, lcReceiptLayoutSetting);
            paComponentController.AddElementAttribute(ctSETReceiptCustomization, lcReceiptCustomization);
            paComponentController.AddElementAttribute(ctSETReceiptLayout, lcReceiptLayout);
            paComponentController.AddElementAttribute(ctSETPrimaryPrinterSetting, General.Base64Encode(clSetting.GetSettingValue(ctSETPrimaryPrinterSetting, "{}")));
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Path, UploadManager.GetInstance().UploadPath);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, ctTYPReceiptBaseInfo);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSubControlPOSReceiptOutput);
            paComponentController.AddElementType(ComponentController.ElementType.Composite);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderCanvas(paComponentController);
            RenderPreviewPanel(paComponentController);

            paComponentController.RenderEndTag();
           // }
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

