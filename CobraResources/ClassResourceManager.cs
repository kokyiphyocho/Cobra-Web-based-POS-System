using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CobraFrame;

namespace CobraResources
{
    public class ResourceManager
    {
        const String ctFoundationStyleResourceTemplate  = "CobraResources.StyleSheets.Foundation.$FILENAME";
        const String ctWidgetStyleResourceTemplate      = "CobraResources.StyleSheets.Widget.$CATEGORY.$FILENAME";
        const String ctToolBarStyleResourceTemplate     = "CobraResources.StyleSheets.ToolBar.$FILENAME";
        const String ctPolymorphicStyleResourceTemplate = "CobraResources.StyleSheets.Polymorphic.$FILENAME";
        
        const String ctFoundationScriptResourceTemplate = "CobraResources.Scripts.Foundation.$FILENAME";
        const String ctWidgetScriptResourceTemplate     = "CobraResources.Scripts.Widget.$CATEGORY.$FILENAME";
        const String ctToolBarScriptResourceTemplate    = "CobraResources.Scripts.ToolBar.$FILENAME";

        const String ctImageResourceTemplate            = "CobraResources.Images.$FILENAME";
        const String ctAjaxLoaderImageResourceTemplate  = "CobraResources.Images.AjaxLoader.$FILENAME";
        const String ctFoundationIconResourceTemplate   = "CobraResources.Icons.Foundation.$FILENAME";
        const String ctToolBarImageResourceTemplate     = "CobraResources.Images.ToolBar.$FILENAME";
        const String ctSystemImageResourceTemplate      = "CobraResources.Images.System.$FILENAME";
        const String ctWidgetImageResourceTemplate      = "CobraResources.Images.Widget.$FILENAME";
        const String ctLanguageImageResourceTemplate    = "CobraResources.Images.Language.$FILENAME";

        public enum WidgetCategory {  System = 0, GShop_General, GShop_Mobile, GShop_POS }

        private String[] clWidgetCategoryFolderList = { "_System", "GShop_General", "GShop_Mobile", "GShop_POS" };

        static ResourceManager      clResourceManager;        
        Type                        clType;

        static public ResourceManager GetInstance()
        {
            if (clResourceManager == null) clResourceManager = new ResourceManager();
            return (clResourceManager);
        }

        private ResourceManager()
        {
            clType = this.GetType();
        }

        public String GetResourceUrl(String paFileName)
        {
            return (ApplicationFrame.GetInstance().ClientScriptManager.GetWebResourceUrl(clType, paFileName));
        }

        public String GetFoundationStyleSheetUrl(String paFileName)
        {
            return (ApplicationFrame.GetInstance().ClientScriptManager.GetWebResourceUrl(clType, ctFoundationStyleResourceTemplate.Replace("$FILENAME", paFileName)));
        }

        public String GetWidgetStyleSheetUrl(WidgetCategory paWidgetCategory, String paFileName)
        {
            String lcResourceName;

            lcResourceName = ctWidgetStyleResourceTemplate.Replace("$FILENAME", paFileName);
            lcResourceName = lcResourceName.Replace("$CATEGORY", clWidgetCategoryFolderList[(int)paWidgetCategory]);

            return (ApplicationFrame.GetInstance().ClientScriptManager.GetWebResourceUrl(clType, lcResourceName));
        }

        public String GetToolBarStyleSheetUrl(String paFileName)
        {
            return (ApplicationFrame.GetInstance().ClientScriptManager.GetWebResourceUrl(clType, ctToolBarStyleResourceTemplate.Replace("$FILENAME", paFileName)));
        }

        public String GetPolymorphicStyleSheetUrl(String paFileName)
        {
            return (ApplicationFrame.GetInstance().ClientScriptManager.GetWebResourceUrl(clType, ctPolymorphicStyleResourceTemplate.Replace("$FILENAME", paFileName)));
        }        

        public String GetFoundationScriptUrl(String paFileName)
        {
            return (ApplicationFrame.GetInstance().ClientScriptManager.GetWebResourceUrl(clType, ctFoundationScriptResourceTemplate.Replace("$FILENAME", paFileName)));
        }

        public String GetWidgetScriptUrl(WidgetCategory paWidgetCategory, String paFileName)
        {
            String lcResourceName;

            lcResourceName = ctWidgetScriptResourceTemplate.Replace("$FILENAME", paFileName);
            lcResourceName = lcResourceName.Replace("$CATEGORY", clWidgetCategoryFolderList[(int)paWidgetCategory]);

            return (ApplicationFrame.GetInstance().ClientScriptManager.GetWebResourceUrl(clType, lcResourceName));
        }

        public String GetToolBarScriptUrl(String paFileName)
        {
            return (ApplicationFrame.GetInstance().ClientScriptManager.GetWebResourceUrl(clType, ctToolBarScriptResourceTemplate.Replace("$FILENAME", paFileName)));
        }

        public String GetImageUrl(String paFileName)
        {
            return (ApplicationFrame.GetInstance().ClientScriptManager.GetWebResourceUrl(clType, ctImageResourceTemplate.Replace("$FILENAME", paFileName)));
        }

        public String GetAjaxLoaderImageUrl(String paFileName)
        {
            return (ApplicationFrame.GetInstance().ClientScriptManager.GetWebResourceUrl(clType, ctAjaxLoaderImageResourceTemplate.Replace("$FILENAME", paFileName)));
        }

        public String GetFoundationIconUrl(String paFileName)
        {
            return (ApplicationFrame.GetInstance().ClientScriptManager.GetWebResourceUrl(clType, ctFoundationIconResourceTemplate.Replace("$FILENAME", paFileName)));
        }

        public String GetToolBarImageUrl(String paFileName)
        {
            return (ApplicationFrame.GetInstance().ClientScriptManager.GetWebResourceUrl(clType, ctToolBarImageResourceTemplate.Replace("$FILENAME", paFileName)));
        }

        public String GetSystemImageUrl(String paFileName)
        {
            return (ApplicationFrame.GetInstance().ClientScriptManager.GetWebResourceUrl(clType, ctSystemImageResourceTemplate.Replace("$FILENAME", paFileName)));
        }

        public String GetWidgetImageUrl(String paFileName)
        {
            return (ApplicationFrame.GetInstance().ClientScriptManager.GetWebResourceUrl(clType, ctWidgetImageResourceTemplate.Replace("$FILENAME", paFileName)));
        }

        public String GetLanguageImageUrl(String paFileName)
        {
            return (ApplicationFrame.GetInstance().ClientScriptManager.GetWebResourceUrl(clType, ctLanguageImageResourceTemplate.Replace("$FILENAME", paFileName)));
        }
    }

    public class AjaxManager
    {
        public enum AjaxLoaderImage : int
        {
            Rotation, Arrows, BarCircle, Bert, BertRound, BigCircleBall, BigRoller, BigSnake,
            BouncingBall, CircleBall, CircleThickBox, Clock, CyclingBall, DarkBar, DripCycle,
            ExpandingCircle, FaceBook, Flower, FlowerBig, Hypnotize, Indicator, IndicatorBig,
            IndicatorBigCircle, IndicatorLite, Kit, LightBar, PacMan, Pik, Pk, Radar, Refresh,
            Roller, ShortBar, SmallWait, Snake, SquareCircle, Squares, Wheel, WheelThrobber
        }

        private int[,] clAjaxLoaderImageInfo = { {(int) AjaxLoaderImage.Rotation, 18, 15},
                                                 {(int) AjaxLoaderImage.Arrows, 18, 15},
                                                 {(int) AjaxLoaderImage.BarCircle, 48, 48},
                                                 {(int) AjaxLoaderImage.Bert, 128, 15},
                                                 {(int) AjaxLoaderImage.BertRound, 128, 15},
                                                 {(int) AjaxLoaderImage.BigCircleBall, 54, 55},
                                                 {(int) AjaxLoaderImage.BigRoller, 32, 32},
                                                 {(int) AjaxLoaderImage.BigSnake, 32, 32},
                                                 {(int) AjaxLoaderImage.BouncingBall, 16, 16},
                                                 {(int) AjaxLoaderImage.CircleBall, 16, 16},
                                                 {(int) AjaxLoaderImage.CircleThickBox, 100, 100},
                                                 {(int) AjaxLoaderImage.Clock, 50, 50},
                                                 {(int) AjaxLoaderImage.CyclingBall, 16, 16},
                                                 {(int) AjaxLoaderImage.DarkBar, 126, 22},
                                                 {(int) AjaxLoaderImage.DripCycle, 42, 42},
                                                 {(int) AjaxLoaderImage.ExpandingCircle, 32, 16},
                                                 {(int) AjaxLoaderImage.FaceBook, 16, 11},
                                                 {(int) AjaxLoaderImage.Flower, 15, 15},
                                                 {(int) AjaxLoaderImage.FlowerBig, 35, 35},
                                                 {(int) AjaxLoaderImage.Hypnotize, 32, 16},
                                                 {(int) AjaxLoaderImage.Indicator, 16, 16},
                                                 {(int) AjaxLoaderImage.IndicatorBig, 32, 32},
                                                 {(int) AjaxLoaderImage.IndicatorBigCircle, 32, 32},
                                                 {(int) AjaxLoaderImage.IndicatorLite, 24, 24},
                                                 {(int) AjaxLoaderImage.Kit, 16, 16},
                                                 {(int) AjaxLoaderImage.LightBar, 220, 19},
                                                 {(int) AjaxLoaderImage.PacMan, 24, 24},
                                                 {(int) AjaxLoaderImage.Pik, 28, 28},
                                                 {(int) AjaxLoaderImage.Pk, 66, 66},
                                                 {(int) AjaxLoaderImage.Radar, 16, 16},
                                                 {(int) AjaxLoaderImage.Refresh, 16, 16},
                                                 {(int) AjaxLoaderImage.Roller, 16, 16},
                                                 {(int) AjaxLoaderImage.ShortBar, 56, 21},
                                                 {(int) AjaxLoaderImage.SmallWait, 25, 25},
                                                 {(int) AjaxLoaderImage.Snake, 16, 16},
                                                 {(int) AjaxLoaderImage.SquareCircle, 31, 31},
                                                 {(int) AjaxLoaderImage.Squares, 43, 11},
                                                 {(int) AjaxLoaderImage.Wheel, 16, 16},
                                                 {(int) AjaxLoaderImage.WheelThrobber, 32, 32}};

        private const String ctDEFAjaxLoaderImageStyle = "position:absolute;width:$WIDTH;height:$HEIGHT;top:50%;left:50%;margin-top:$TOPMARGIN;margin-left:$LEFTMARGIN";
        private const String ctHTMLAjaxLoaderImage = "<div style = \"$STYLE\"><img src=\"$IMAGEFILE\"/></div>";
        private const String ctImagePrefix = "AJAX_";
        private const String ctGIFExtension = ".gif";

        private static AjaxManager clAjaxManager;

        public static AjaxManager GetInstance()
        {
            if (clAjaxManager == null) clAjaxManager = new AjaxManager();
            return (clAjaxManager);
        }

        public Size GetAjaxLoaderImageSize(AjaxLoaderImage paAjaxLoaderImage)
        {
            for (int lcCount = 0; lcCount < clAjaxLoaderImageInfo.GetLength(0); lcCount++)
                if (clAjaxLoaderImageInfo[lcCount, 0] == (int)paAjaxLoaderImage) return (new Size(clAjaxLoaderImageInfo[lcCount, 1], clAjaxLoaderImageInfo[lcCount, 2]));

            return (new Size(0, 0));
        }

        public String GetAjaxLoaderImageUrl(AjaxLoaderImage paAjaxLoaderImage)
        {            
            return (ResourceManager.GetInstance().GetAjaxLoaderImageUrl(ctImagePrefix + paAjaxLoaderImage.ToString() + ctGIFExtension));
        }        
    }
}
