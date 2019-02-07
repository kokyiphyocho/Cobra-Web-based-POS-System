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


namespace CobraStandardControls
{
    public class SubControlMobileStoreFrontGridItem : WebControl, WidgetControlInterface
    {
        const String ctSubControlMobileStoreFrontGridItemStyle   = "SubControlMobileStoreFrontGridItem.css";
        const String ctSubControlMobileStoreFrontGridItemScript  = "SubControlMobileStoreFrontGridItem.js";
                
        const String ctCLSGridItem                    = "GridItem";
        const String ctCLSImageDiv                    = "ImageDiv";
        const String ctCLSImage                       = "Image";
        const String ctCLSInfoDiv                     = "InfoDiv";
        const String ctCLSNetworkDiv                  = "NetworkDiv";
        const String ctCLSDescriptionDiv              = "DescriptionDiv";
        const String ctCLSSeparatorDiv                = "SeparatorDiv";
        const String ctCLSSeparator                   = "Separator";
        const String ctCLSPriceDiv                    = "PriceDiv";
        const String ctCLSPriceFigure                 = "PriceFigure";
        const String ctCLSUsualPriceDiv               = "UsualPriceDiv";        

        const String ctCLSDetailInfoButton            = "DetailInfoButton";        
        const String ctCLSDetailInfoPopUpDiv          = "DetailInfoPopUpDiv";

        const String ctCLSPopUpTitleBar               = "PopUpTitleBar";
        const String ctCLSPopUpHeader                 = "PopUpHeader";
        const String ctCLSCloseButton                 = "CloseButton";

        const String ctCLSInfoContainer               = "InfoContainer";
        const String ctCLSInfoImage                   = "InfoImage";
        const String ctCLSInfoGroup                   = "InfoGroup"; 
        const String ctCLSInfoGroupHeading            = "InfoGroupHeading";
        const String ctCLSInfoRowContainer            = "InfoRowContainer";   
        const String ctCLSInfoRow                     = "InfoRow";
        const String ctCLSInfoLabel                   = "InfoLabel";
        const String clCLSInfoData                    = "InfoData";

        const String ctMETAProductFullName            = "U_ProductFullName";
        const String ctMETAUniversalProductUID        = "U_ProductUID";
        const String ctMETAKind                       = "Kind";
        const String ctMETADescription1               = "Description.Description1";
        const String ctMETADescription2               = "Description.Description2";
        const String ctMETAPrice                      = "Price";
        const String ctMETAUsualPrice                 = "UsualPrice";
        const String ctMETALocalImage                 = "Images.Image";
        const String ctMETAUniversalImage             = "U_ProductImages.Images.ThumbNail";

        const String ctImagePlaceHolder               = "NoPhoneImage.jpg";
        const String ctDashImage                      = "dash.png";

        const String ctICOEllipseOrange               = "ellipseorange.png";

        const String ctCMDShowDetailInfo              = "@cmd%showdetailinfo";
        const String ctCMDCloseDetailInfo             = "@cmd%closedetailinfo";

        const String ctDEFCurrency                    = "MMK";
        const String ctPathDelimiter                  = "/";
                
        private String[,] clNetworkInfoTemplate         = new String[,] { { "Technology", "U_TechnicalInfo.Network.Technology"}, 
                                                                          { "2G Bands", "U_TechnicalInfo.Network.2G"}, 
                                                                          { "3G Bands", "U_TechnicalInfo.Network.3G"},
                                                                          { "4G Bands", "U_TechnicalInfo.Network.4G"},
                                                                          { "Speed", "U_TechnicalInfo.Network.Speed"},
                                                                          { "GPRS", "U_TechnicalInfo.Network.GPRS"},
                                                                          {  "EDGE", "U_TechnicalInfo.Network.EDGE"} };


        private String[,] clBodyInfoTemplate            = new String[,] { { "Dimensions", "U_AppearanceInfo.Body.Dimensions"}, 
                                                                          { "Weight", "U_AppearanceInfo.Body.Weight"}, 
                                                                          { "Build", "U_AppearanceInfo.Body.Build"}, 
                                                                          { "SIM", "U_AppearanceInfo.Body.SIM"},
                                                                          { "Other", "U_AppearanceInfo.Body.Other"} };

        private String[,] clDisplayInfoTemplate         = new String[,] { { "Type", "U_AppearanceInfo.Display.Type"}, 
                                                                          { "Size", "U_AppearanceInfo.Display.Size"}, 
                                                                          { "Resolution", "U_AppearanceInfo.Display.Resolution"},
                                                                          { "Multitouch", "U_AppearanceInfo.Display.Multitouch"},
                                                                          { "Protection", "U_AppearanceInfo.Display.Protection"},
                                                                          { "Other", "U_AppearanceInfo.Display.Other"} };

        private String[,] clSystemInfoTemplate          = new String[,] { { "OS", "U_TechnicalInfo.Platform.OS"}, 
                                                                          { "Chipset", "U_TechnicalInfo.Platform.Chipset"}, 
                                                                          { "CPU", "U_TechnicalInfo.Platform.CPU"},
                                                                          { "GPU", "U_TechnicalInfo.Platform.GPU"} };


        private String[,] clMemoryInfoTemplate          = new String[,] { { "Card Slot", "U_TechnicalInfo.Memory.CardSlot"}, 
                                                                          { "Internal", "U_TechnicalInfo.Memory.Internal"}, };


        private String[,] clCameraInfoTemplate          = new String[,] { { "Primary", "U_FeatureInfo.Camera.Primary"}, 
                                                                          { "Features", "U_FeatureInfo.Camera.Features"}, 
                                                                          { "Video", "U_FeatureInfo.Camera.Video"},
                                                                          { "Secondary", "U_FeatureInfo.Camera.Secondary"} };

        private String[,] clSoundInfoTemplate           = new String[,] { { "Alert Types", "U_FeatureInfo.Sound.AlertTypes"}, 
                                                                          { "Loudspeakers", "U_FeatureInfo.Sound.Loudspeaker"}, 
                                                                          { "3.5mmJack", "U_FeatureInfo.Sound.3.5mm"},
                                                                          { "Other", "U_FeatureInfo.Sound.Other"} };

        private String[,] clCommunicationInfoTemplate   = new String[,]   { { "WLAN", "U_FeatureInfo.Comms.Wlan"}, 
                                                                            { "Bluetooth", "U_FeatureInfo.Comms.Bluetooth"}, 
                                                                            { "GPS", "U_FeatureInfo.Comms.GPS"},
                                                                            { "NFC", "U_FeatureInfo.Comms.NFC"},
                                                                            { "RADIO", "U_FeatureInfo.Comms.RADIO"},
                                                                            { "USB", "U_FeatureInfo.Comms.USB"} };

        private String[,] clFeaturesInfoTemplate         = new String[,] { { "Sensors", "U_FeatureInfo.Features.Sensors"}, 
                                                                           { "Messaging", "U_FeatureInfo.Features.Messaging"}, 
                                                                           { "Browser", "U_FeatureInfo.Features.Browser"},
                                                                           { "Java", "U_FeatureInfo.Features.Java"},
                                                                           { "Other", "U_FeatureInfo.Features.Other"}};

        private String[,] clBatteryInfoTemplate          = new String[,] { { "Battery", "U_AdditionalInfo.Battery.Battery"}, 
                                                                           { "Stand-By", "U_AdditionalInfo.Battery.Stand-By"}, 
                                                                           { "Talk Time", "U_AdditionalInfo.Battery.TalkTime"},
                                                                           { "Music Play", "U_AdditionalInfo.Battery.MusicPlay"} };

        private String[,] clMiscellaneousInfoTemplate    = new String[,] { { "Colors", "U_AdditionalInfo.Misc.Colors" } };


        public CompositeFormInterface SCI_ParentForm            { get; set; }

        public String SC_Currency                               { get; set; }
        public String SC_UniversalImagePath                     { get; set; }
        public String SC_LocalImagePath                         { get; set; }
                
        MetaDataRow             clMetaDataRow;

        public SubControlMobileStoreFrontGridItem()
        {
            SC_Currency                 = ctDEFCurrency;
            SC_UniversalImagePath       = String.Empty;
            SC_LocalImagePath           = String.Empty;
        }       

        // Special Case, call by parent.
        public void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            CSSStyleManager lcCSSStyleManager;
            JavaScriptManager lcJavaScriptmanager;

            lcCSSStyleManager = new CSSStyleManager(paComponentController);
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);

            lcCSSStyleManager.IncludeExternalStyleSheet(ResourceManager.GetInstance().GetWidgetStyleSheetUrl(ResourceManager.WidgetCategory.GShop_Mobile, ctSubControlMobileStoreFrontGridItemStyle));
            lcJavaScriptmanager.IncludeExternalJavaScript(ResourceManager.GetInstance().GetWidgetScriptUrl(ResourceManager.WidgetCategory.GShop_Mobile, ctSubControlMobileStoreFrontGridItemScript));
        }

        public void SetData(DataRow paDataRow)
        {   
            clMetaDataRow = new MetaDataRow(paDataRow);
        }

        protected String GetImage()
        {
            String lcImage;

            if (!String.IsNullOrWhiteSpace(lcImage = clMetaDataRow.ActiveData.GetData(ctMETALocalImage, null)))
                return (SC_LocalImagePath + ctPathDelimiter + lcImage);
            else
                if (!String.IsNullOrWhiteSpace(lcImage = clMetaDataRow.ActiveData.GetData(ctMETAUniversalImage, null)))
                    return (SC_UniversalImagePath + ctPathDelimiter + lcImage);
                else return (ResourceManager.GetInstance().GetSystemImageUrl(ctImagePlaceHolder));
        }

        protected void RenderItemImage(ComponentController paComponentController)
        {            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSImageDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
                        
            paComponentController.AddAttribute(HtmlAttribute.Src, GetImage());
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSImage);
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        protected void RenderItemInfo(ComponentController paComponentController)
        {
            int     lcPrice;
            int     lcUsualPrice;

            lcPrice         = clMetaDataRow.ActiveData.GetIntData(ctMETAPrice, 0);
            lcUsualPrice    = clMetaDataRow.ActiveData.GetIntData(ctMETAUsualPrice, 0);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInfoDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSNetworkDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clMetaDataRow.ActiveData.GetData(ctMETAKind, String.Empty));
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSDescriptionDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clMetaDataRow.ActiveData.GetData(ctMETADescription1, String.Empty) + "<br/>" + clMetaDataRow.ActiveData.GetData(ctMETADescription2, String.Empty));
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSUsualPriceDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            if ((lcUsualPrice != 0) && (lcUsualPrice > lcPrice))
                paComponentController.Write(ctDEFCurrency + " " + lcUsualPrice.ToString("N0"));
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSeparatorDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSSeparator);
            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetSystemImageUrl(ctDashImage));
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPriceDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(ctDEFCurrency);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPriceFigure);
            paComponentController.RenderBeginTag(HtmlTag.Span);
            paComponentController.Write(lcPrice.ToString("N0"));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();


            paComponentController.RenderEndTag();
        }

        private void RenderDetailButton(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDShowDetailInfo);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSDetailInfoButton);
            paComponentController.RenderBeginTag(HtmlTag.A);

            paComponentController.AddAttribute(HtmlAttribute.Src, ResourceManager.GetInstance().GetFoundationIconUrl(ctICOEllipseOrange));
            paComponentController.RenderBeginTag(HtmlTag.Img);            
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        private void RenderDetailInfoGroup(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPopUpTitleBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.RenderEndTag();
        }

        private void RenderDetailTitleBar(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPopUpTitleBar);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSPopUpHeader);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clMetaDataRow.ActiveData.GetData(ctMETAProductFullName, String.Empty));
            paComponentController.RenderEndTag();

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSCloseButton);
            paComponentController.AddAttribute(HtmlAttribute.Href, ctCMDCloseDetailInfo);
            paComponentController.RenderBeginTag(HtmlTag.A);
            paComponentController.Write(ComponentController.UnicodeStr((int)Fontawesome.remove));
            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
            
        }        

        private void RenderInfoGroupElement(ComponentController paComponentController, String paInfoLabel, MetaDataElement paMetaDataElement)
        {
            if ((paMetaDataElement != null) && (paMetaDataElement.ValueCount > 0))
            {
                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInfoRow);
                paComponentController.RenderBeginTag(HtmlTag.Div);

                paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInfoLabel);
                paComponentController.RenderBeginTag(HtmlTag.Div);
                paComponentController.Write(paInfoLabel);
                paComponentController.RenderEndTag();

                paComponentController.AddAttribute(HtmlAttribute.Class, clCLSInfoData);
                paComponentController.RenderBeginTag(HtmlTag.Div);

                for (int lcCount = 0; lcCount < paMetaDataElement.ValueCount; lcCount ++)
                {
                    paComponentController.RenderBeginTag(HtmlTag.Span);
                    paComponentController.Write(paMetaDataElement[lcCount].Replace(".", " "));
                    paComponentController.RenderEndTag();
                }
                

                paComponentController.RenderEndTag();

                paComponentController.RenderEndTag();
            }
        }
        
        private void RenderInfoGroup(ComponentController paComponentController, String paGroupHeading, String[,] paInfoGroup)
        {            
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInfoGroup);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInfoGroupHeading);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(paGroupHeading);
            paComponentController.RenderEndTag();
            
            for (int lcCount = 0; lcCount < paInfoGroup.GetLength(0); lcCount++)            
            {                
                RenderInfoGroupElement(paComponentController, paInfoGroup[lcCount, 0], clMetaDataRow.ActiveData[paInfoGroup[lcCount, 1]]);
            }

            paComponentController.RenderEndTag();
        }
        
        private void RenderDetailImageDiv(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInfoImage);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddAttribute(HtmlAttribute.Src, GetImage());
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();
            paComponentController.RenderEndTag();
        }
      
        private void RenderInfoContainer(ComponentController paComponentController)
        {
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSInfoContainer);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderDetailImageDiv(paComponentController);

            RenderInfoGroup(paComponentController, "Network", clNetworkInfoTemplate);
            RenderInfoGroup(paComponentController, "Body", clBodyInfoTemplate);
            RenderInfoGroup(paComponentController, "Display", clDisplayInfoTemplate);
            RenderInfoGroup(paComponentController, "System", clSystemInfoTemplate);
            RenderInfoGroup(paComponentController, "Memory", clMemoryInfoTemplate);
            RenderInfoGroup(paComponentController, "Camera", clCameraInfoTemplate);
            RenderInfoGroup(paComponentController, "Sound", clSoundInfoTemplate);
            RenderInfoGroup(paComponentController, "Communications", clCommunicationInfoTemplate);
            RenderInfoGroup(paComponentController, "Features", clFeaturesInfoTemplate);
            RenderInfoGroup(paComponentController, "Battery", clBatteryInfoTemplate);
            RenderInfoGroup(paComponentController, "Miscellaneous", clMiscellaneousInfoTemplate);

            paComponentController.RenderEndTag();
        }        

        private void RenderDetailDiv(ComponentController paComponentController)
        {
            //paComponentController.AddElementType(ComponentController.ElementType.Overlay);
            //paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSDetailOverlayDiv);
            //paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementType(ComponentController.ElementType.PopUp);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSDetailInfoPopUpDiv);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderDetailTitleBar(paComponentController);
            RenderInfoContainer(paComponentController);

            paComponentController.RenderEndTag();

  //          paComponentController.RenderEndTag();
        }

        private void RenderBrowserMode(ComponentController paComponentController)        
        {   
            paComponentController.AddElementType(ComponentController.ElementType.GridItem);
            paComponentController.AddAttribute(HtmlAttribute.Class, ctCLSGridItem);
            paComponentController.RenderBeginTag(HtmlTag.Div);                        
            RenderItemImage(paComponentController);
            RenderItemInfo(paComponentController);
            
            if (!String.IsNullOrEmpty(clMetaDataRow.ActiveData.GetData(ctMETAUniversalProductUID, null)))
            {
                RenderDetailButton(paComponentController);
                RenderDetailDiv(paComponentController);
            }

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




