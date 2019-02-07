using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using CobraFrame;
using CobraResources;

namespace CobraWebFrame
{  
    public class ExternalLinkManager
    {
        public static ExternalLinkManager clExternalLinkManager;

        ArrayList clExternalLinkList;

        public static void InitializeInstance()
        {
            clExternalLinkManager = new ExternalLinkManager();
        }

        public static ExternalLinkManager GetInstance()
        {
            return (clExternalLinkManager);
        }

        private ExternalLinkManager()
        {
            clExternalLinkList = new ArrayList();
        }

        public bool RegisterExternalLink(String paLinkName)
        {
            if (!clExternalLinkList.Contains(paLinkName))
            {
                clExternalLinkList.Add(paLinkName);
                return (true);
            }          
            else return (false);
        }
    }

    public class JavaScriptManager
    {
        private const String ctDocumentReadyJavaScript = "<script type=\"text/javascript\"> $(document).ready(function () { $STATEMENT }); </script>";
        private const String ctDelayDocumentReadyJavaScript = "<script type=\"text/javascript\"> $(document).ready(setTimeout(function () { $STATEMENT },$DELAYTIME)); </script>";
        private const String ctExternalJavaScriptTemplate = "<script type=\"text/javascript\" src=\"$FILENAME\" defer></script>";

        private const string ctCobraFoundationScriptResourceName = "CobraResources.Scripts.Foundation.CobraFoundationScript.js";
        private const string ctCobraUIFunctionsScriptResourceName = "CobraResources.Scripts.Foundation.CobraUIFunctions.js";
        private const string ctJQueryResourceName = "CobraResources.Scripts.Foundation.jquery-3.1.0.min.js";
        private const string ctJQueryMD5ResourceName = "CobraResources.Scripts.Foundation.jquery.md5.min.js";        
        private const string ctJQueryBase64ResourceName = "CobraResources.Scripts.Foundation.Base64.js";
        private const string ctModernizrResourceName = "CobraResources.Scripts.Foundation.modernizr.js";
        private const string ctMomentResourceName = "CobraResources.Scripts.Foundation.moment.min.js";
        
        ComponentController clComponentController;

        public JavaScriptManager(ComponentController paComponentController)
        {
            clComponentController = paComponentController;
        }

        public static void IncludeStandardJavaScript()
        {
            IncludeJavaScript(ctJQueryResourceName);
            IncludeJavaScript(ctJQueryMD5ResourceName);
            IncludeJavaScript(ctJQueryBase64ResourceName);
            IncludeJavaScript(ctModernizrResourceName);
            IncludeJavaScript(ctMomentResourceName);
            IncludeJavaScript(ctCobraFoundationScriptResourceName);
            IncludeJavaScript(ctCobraUIFunctionsScriptResourceName);            
        }

        public static String GenerateDocumentReadyJavaScript(String paStatement)
        {
            return (ctDocumentReadyJavaScript.Replace("$STATEMENT", paStatement));
        }

        public static String GenerateDocumentReadyJavaScript(String paStatement, int paDelayTime)
        {
            return (ctDelayDocumentReadyJavaScript.Replace("$STATEMENT", paStatement).Replace("$DELAYTIME", paDelayTime.ToString()));
        }

        private static void IncludeJavaScript(string paResourceName)
        {
            InsertJavaScriptLink(ResourceManager.GetInstance().GetResourceUrl(paResourceName));         
        }

        private static void InsertJavaScriptLink(String paResourceUrl)
        {
            HtmlGenericControl lcJavaScriptControl;

            lcJavaScriptControl = new HtmlGenericControl("script");                        
            lcJavaScriptControl.Attributes.Add("type", "text/javascript");
            lcJavaScriptControl.Attributes.Add("src", paResourceUrl);
            lcJavaScriptControl.Attributes.Add("defer", "defer");            

            ApplicationFrame.GetInstance().ActivePage.Header.Controls.Add(lcJavaScriptControl);
        }

        public void IncludeExternalJavaScript(String paFileName)
        {            
            if (ExternalLinkManager.GetInstance().RegisterExternalLink(paFileName))
               clComponentController.Write(ctExternalJavaScriptTemplate.Replace("$FILENAME", paFileName));           
        }
    }

    public class CSSStyleManager
    {
        const String ctStyleTemplate = "<style type=\"text/css\">$CLASSNAME{$STYLE}</style>";
        const String ctExternalStyleSheetTemplate = "<link rel=\"stylesheet\" type=\"text/css\" href=\"$FILENAME\"/>";
        const String ctExtractClassRegEx = "(.*)(?i)class=['|\"]([^'\"]*)['|\"]\\s*(;|$)(.*)";

        const String ctCobraStyleSheet      = "CobraStyle.css";
        const String ctCobraUIStyleSheet    = "CobraUIStyle.css";        

        const String ctFontFaceTemplate = "@font-face" +
                                             "{ " +
                                                 "font-family: '$ALIAS'; " +
                                                 "src: url('/fonts/$FONTNAME.eot'); " +
                                                 "src: local('$FONTNAME')," +                                                      
                                                      "url('/fonts/$FONTNAME.eot?#iefix') format('embedded-opentype'), " +                                                      
                                                      "url('/fonts/$FONTNAME.svg#$FONTNAME') format('svg'), " +
                                                      "url('/fonts/$FONTNAME.ttf') format('truetype'), " +
                                                      "url('/fonts/$FONTNAME.woff') format('woff'), " +
                                                      "url('/fonts/$FONTNAME.woff2') format('woff2'); " +
                                                 "font-weight: normal; " +
                                                 "font-style: normal; " +
                                             "} ";


        //const String ctFontFaceTemplate = "@font-face" +
        //                                     "{ " +
        //                                         "font-family: '$FONTNAME'; " +
        //                                         "src: url('../fonts/$FONTNAME.eot'); " +
        //                                         "src: local('$FONTNAME')," +
        //                                              "url('../fonts/$FONTNAME.ttf') format('truetype'), " +
        //                                              "url('../fonts/$FONTNAME.eot?#iefix') format('embedded-opentype'), " +
        //                                              "url('../fonts/$FONTNAME.woff') format('woff'), " +
        //                                              "url('../fonts/$FONTNAME.svg#$FONTNAME') format('svg'); " +
        //                                         "font-weight: normal; " +
        //                                         "font-style: normal; " +
        //                                     "} ";

        ArrayList clStyleList;
        ArrayList clIncludedStyleSheetList;
        ComponentController clComponentController;

        public CSSStyleManager(ComponentController paComponentController)
        {
            clStyleList = new ArrayList();
            clIncludedStyleSheetList = new ArrayList();
            clComponentController = paComponentController;
        }

        static public void AppendStyleString(ref String paStyleString, CSSStyle paStyle, String paValue)
        {
            if (!String.IsNullOrEmpty(paValue))
                paStyleString += paStyle.ToString().Replace("_", "-").ToLower() + ":" + paValue + ";";
        }

        public void AddStyle(CSSStyle paStyle, String paValue)
        {
            clStyleList.Add(paStyle.ToString().Replace("_", "-").ToLower() + " : " + paValue);
        }

        public void AddStyle(String paStyle, String paValue)
        {
            clStyleList.Add(paStyle.ToLower() + " : " + paValue);
        }

        public void AddStyle(String paCSSList)
        {
            if (!String.IsNullOrEmpty(paCSSList))
                clStyleList.Add(paCSSList);
        }

        public void WriteCSSClass(String paClassName)
        {
            String lcStyleListStr;

            lcStyleListStr = "";

            if (clStyleList.Count > 0)
            {
                for (int lcCount = 0; lcCount < clStyleList.Count; lcCount++)
                    lcStyleListStr += clStyleList[lcCount].ToString() + ";";

                clComponentController.Write(ctStyleTemplate.Replace("$CLASSNAME", paClassName).Replace("$STYLE", lcStyleListStr));

                clStyleList.Clear();
            }
        }

        private static void InsertStyleSheetLink(String paResourceUrl)
        {
            HtmlLink lcCssLink;

            lcCssLink = new HtmlLink();
            lcCssLink.Href = paResourceUrl;
            lcCssLink.Attributes.Add("rel", "stylesheet");
            lcCssLink.Attributes.Add("type", "text/css");

            ApplicationFrame.GetInstance().ActivePage.Header.Controls.Add(lcCssLink);

        }

        public static void InsertCSSStyleBlock(String paCSSSTyle)
        {
            HtmlGenericControl lcHtmlGenericControl;

            lcHtmlGenericControl = new HtmlGenericControl("style");
            lcHtmlGenericControl.Attributes.Add("type", "text/css");
            lcHtmlGenericControl.InnerHtml = paCSSSTyle;

            ApplicationFrame.GetInstance().ActivePage.Header.Controls.Add(lcHtmlGenericControl);
        }

        public static void IncludeStandardStyleSheet()
        {                          
            InsertStyleSheetLink(ResourceManager.GetInstance().GetFoundationStyleSheetUrl(ctCobraStyleSheet));
            InsertStyleSheetLink(ResourceManager.GetInstance().GetFoundationStyleSheetUrl(ctCobraUIStyleSheet));
        }

        public void IncludeExternalStyleSheet(String paFileName)
        {
            if (ExternalLinkManager.GetInstance().RegisterExternalLink(paFileName))
                clComponentController.Write(ctExternalStyleSheetTemplate.Replace("$FILENAME", paFileName));
        }

        public static void ParseStyle(String paStyleStr, out String paClassName, out String paStyleContent)
        {
            Match lcMatch;

            paClassName = String.Empty;
            paStyleContent = String.Empty;

            if (!String.IsNullOrEmpty(paStyleStr))
            {
                if ((lcMatch = Regex.Match(paStyleStr, ctExtractClassRegEx)).Success)
                {
                    paClassName = lcMatch.Groups[2].Value;
                    paStyleContent = lcMatch.Groups[1].Value + lcMatch.Groups[4].Value;
                }
                else paStyleContent = paStyleStr;
            }
        }

        public static String GetEmbeddedWebFontStyle(String[] paEmbeddedFont)
        {
            String lcFontName;
            String lcAlias;

            if ((paEmbeddedFont != null) && (paEmbeddedFont.Length > 0))
            {
                lcFontName  = paEmbeddedFont[0];
                lcAlias     = paEmbeddedFont.Length > 1 ? paEmbeddedFont[1] : lcFontName;
                return (ctFontFaceTemplate.Replace("$FONTNAME", lcFontName).Replace("$ALIAS", lcAlias));
            } else return (String.Empty);            
        }

        public static void InsertEmbeddedFontStyle(String[][] paEmbeddedFontList)
        {
            String lcEmbeddedFontArray;

            if (paEmbeddedFontList != null)
            {
                lcEmbeddedFontArray = String.Empty;

                for (int lcCount = 0; lcCount < paEmbeddedFontList.Length; lcCount++)
                {
                    lcEmbeddedFontArray += CSSStyleManager.GetEmbeddedWebFontStyle(paEmbeddedFontList[lcCount]);
                }

                CSSStyleManager.InsertCSSStyleBlock(lcEmbeddedFontArray);
            }
        }
    }

    public class CrossBrowserStyleManager
    {
        // Incomaptible Style List
        String[][] clIncomaptibleStyleList = new String[][] { new String[] {"border-radius","-moz-border-radius","-webkit-border-radius","-khtml-border-radius"},
                                                              new String[] {"box-shadow","-moz-box-shadow","-webkit-box-shadow"},
                                                              new String[] {"transition","-webkit-transition","-moz-transition","-ms-transition ","-o-transition"} };

        public enum GradientOrientation : int { Horizontal, Vertical }

        const int ctISLBorderRadius = 0;
        const int ctISLBoxShadow = 1;

        // Gradient Background
        const String ctIEGradient = "progid:DXImageTransform.Microsoft.gradient(gradienttype=$GRADIENTTYPE, startColorstr='$STARTCOLOR', endColorstr='$ENDCOLOR')";
        const String ctWEBKITGradient = "-webkit-linear-gradient($DIRECTIONSTART, $STARTCOLOR, $ENDCOLOR)";
        const String ctMOZGradient = "-moz-linear-gradient($DIRECTIONSTART,  $STARTCOLOR,  $ENDCOLOR)";
        const String ctMSLinearGradient = "-ms-linear-gradient($DIRECTIONSTART, $STARTCOLOR, $ENDCOLOR)";
        const String ctLinearGradient = "linear-gradient(to $DIRECTIONEND, $STARTCOLOR, $ENDCOLOR)";
        const String ctOperaGradient = "-o-linear-gradient($DIRECTIONSTART, $STARTCOLOR, $ENDCOLOR)";

        // Opa
 
        const String ctIE8Opacity = "\"progid:DXImageTransform.Microsoft.Alpha(Opacity=$OPACITY)\";";
        const String ctIEOpacity = "alpha(opacity=$OPACITY);";

        // Box Shadow 
        const String ctBoxShadow1Param = "$SHADOWSTYLE";
        const String ctBoxShadow2Param = "$HORIZSHADOW $VERTSHADOW $SHADOWCOLOR";
        const String ctBoxShadow5Param = "$HORIZSHADOW $VERTSHADOW $BLURDISTANCE $SPREADDISTANCE $SHADOWCOLOR";


        ComponentController clComponentController;
        CSSStyleManager clCSSStyleManager;

        public CrossBrowserStyleManager(ComponentController paComponentController)
        {
            clComponentController = paComponentController;
        }

        public CrossBrowserStyleManager(CSSStyleManager paCSSStyleManager)
        {
            clCSSStyleManager = paCSSStyleManager;
        }

        public void AddGradientStyle(GradientOrientation paGraditenOrientation, String paStartColor, String paEndColor)
        {
            String lcDirectionStart;
            String lcDirectionEnd;

            lcDirectionStart = paGraditenOrientation == GradientOrientation.Horizontal ? "left" : "top";
            lcDirectionEnd = paGraditenOrientation == GradientOrientation.Horizontal ? "right" : "bottom";

            clComponentController.AddStyle(CSSStyle.Filter, ctIEGradient.Replace("$STARTCOLOR", paStartColor).Replace("$ENDCOLOR", paEndColor).Replace("$GRADIENTTYPE", paGraditenOrientation == GradientOrientation.Horizontal ? "1" : "0"));
            clComponentController.AddStyle(CSSStyle.Background, ctWEBKITGradient.Replace("$STARTCOLOR", paStartColor).Replace("$ENDCOLOR", paEndColor).Replace("$DIRECTIONSTART", lcDirectionStart));
            clComponentController.AddStyle(CSSStyle.Background, ctMOZGradient.Replace("$STARTCOLOR", paStartColor).Replace("$ENDCOLOR", paEndColor).Replace("$DIRECTIONSTART", lcDirectionStart));
            clComponentController.AddStyle(CSSStyle.Background, ctMSLinearGradient.Replace("$STARTCOLOR", paStartColor).Replace("$ENDCOLOR", paEndColor).Replace("$DIRECTIONSTART", lcDirectionStart));
            clComponentController.AddStyle(CSSStyle.Background, ctLinearGradient.Replace("$STARTCOLOR", paStartColor).Replace("$ENDCOLOR", paEndColor).Replace("$DIRECTIONEND", lcDirectionEnd));
            clComponentController.AddStyle(CSSStyle.Background, ctOperaGradient.Replace("$STARTCOLOR", paStartColor).Replace("$ENDCOLOR", paEndColor).Replace("$DIRECTIONSTART", lcDirectionStart));
        }

        public void InsertGradientStyleCSS(GradientOrientation paGraditenOrientation, String paStartColor, String paEndColor)
        {
            String lcDirectionStart;
            String lcDirectionEnd;

            lcDirectionStart = paGraditenOrientation == GradientOrientation.Horizontal ? "left" : "top";
            lcDirectionEnd = paGraditenOrientation == GradientOrientation.Horizontal ? "right" : "bottom";

            clCSSStyleManager.AddStyle(CSSStyle.Filter, ctIEGradient.Replace("$STARTCOLOR", paStartColor).Replace("$ENDCOLOR", paEndColor).Replace("$GRADIENTTYPE", paGraditenOrientation == GradientOrientation.Horizontal ? "1" : "0"));
            clCSSStyleManager.AddStyle(CSSStyle.Background, ctWEBKITGradient.Replace("$STARTCOLOR", paStartColor).Replace("$ENDCOLOR", paEndColor).Replace("$DIRECTIONSTART", lcDirectionStart));
            clCSSStyleManager.AddStyle(CSSStyle.Background, ctMOZGradient.Replace("$STARTCOLOR", paStartColor).Replace("$ENDCOLOR", paEndColor).Replace("$DIRECTIONSTART", lcDirectionStart));
            clCSSStyleManager.AddStyle(CSSStyle.Background, ctMSLinearGradient.Replace("$STARTCOLOR", paStartColor).Replace("$ENDCOLOR", paEndColor).Replace("$DIRECTIONSTART", lcDirectionStart));
            clCSSStyleManager.AddStyle(CSSStyle.Background, ctLinearGradient.Replace("$STARTCOLOR", paStartColor).Replace("$ENDCOLOR", paEndColor).Replace("$DIRECTIONEND", lcDirectionEnd));
            clCSSStyleManager.AddStyle(CSSStyle.Background, ctOperaGradient.Replace("$STARTCOLOR", paStartColor).Replace("$ENDCOLOR", paEndColor).Replace("$DIRECTIONSTART", lcDirectionStart));
        }

        public void AddOpacity(float paOpacity)
        {
            clComponentController.AddStyle(CSSStyle._Ms_Filter, ctIE8Opacity.Replace("$OPACITY", (paOpacity * 10).ToString("F0")));
            clComponentController.AddStyle(CSSStyle.Filter, ctIEOpacity.Replace("$OPACITY", (paOpacity * 10).ToString("F0")));
            clComponentController.AddStyle(CSSStyle._Moz_Opacity, paOpacity.ToString("F2") + ";");
            clComponentController.AddStyle(CSSStyle._Khtml_Opacity, paOpacity.ToString("F2") + ";");
            clComponentController.AddStyle(CSSStyle.Opacity, paOpacity.ToString("F2") + ";");
        }

        public void InsertOpacity(float paOpacity)
        {
            clCSSStyleManager.AddStyle(CSSStyle._Ms_Filter, ctIE8Opacity.Replace("$OPACITY", (paOpacity * 10).ToString("F0")));
            clCSSStyleManager.AddStyle(CSSStyle.Filter, ctIEOpacity.Replace("$OPACITY", (paOpacity * 10).ToString("F0")));
            clCSSStyleManager.AddStyle(CSSStyle._Moz_Opacity, paOpacity.ToString("F2") + ";");
            clCSSStyleManager.AddStyle(CSSStyle._Khtml_Opacity, paOpacity.ToString("F2") + ";");
            clCSSStyleManager.AddStyle(CSSStyle.Opacity, paOpacity.ToString("F2") + ";");
        }

        public void AddBoxShadow(String paShadowStyle)
        {
            AddMultipleStyle(clIncomaptibleStyleList[ctISLBoxShadow], paShadowStyle);
        }

        public void InsertBoxShadow(String paShadowStyle)
        {
            InsertMultipleStyle(clIncomaptibleStyleList[ctISLBoxShadow], paShadowStyle);
        }

        public void AddBoxShadow(String paHorizShadow, String paVertShadow, String paShadowColor)
        {
            String lcShadowParam;

            paHorizShadow = String.IsNullOrEmpty(paHorizShadow) ? "0px" : paHorizShadow;
            paVertShadow = String.IsNullOrEmpty(paVertShadow) ? "0px" : paVertShadow;
            paShadowColor = String.IsNullOrEmpty(paShadowColor) ? "rgba(0,0,0,1)" : paShadowColor;

            lcShadowParam = ctBoxShadow2Param.Replace("$HORIZSHADOW", paHorizShadow).Replace("$VERTSHADOW", paVertShadow).Replace("$SHADOWCOLOR", paShadowColor);
            AddMultipleStyle(clIncomaptibleStyleList[ctISLBoxShadow], lcShadowParam);
        }

        public void InsertBoxShadow(String paHorizShadow, String paVertShadow, String paShadowColor)
        {
            String lcShadowParam;

            paHorizShadow = String.IsNullOrEmpty(paHorizShadow) ? "0px" : paHorizShadow;
            paVertShadow = String.IsNullOrEmpty(paVertShadow) ? "0px" : paVertShadow;
            paShadowColor = String.IsNullOrEmpty(paShadowColor) ? "rgba(0,0,0,1)" : paShadowColor;

            lcShadowParam = ctBoxShadow2Param.Replace("$HORIZSHADOW", paHorizShadow).Replace("$VERTSHADOW", paVertShadow).Replace("$SHADOWCOLOR", paShadowColor);
            InsertMultipleStyle(clIncomaptibleStyleList[ctISLBoxShadow], lcShadowParam);
        }

        public void AddBoxShadow(String paHorizShadow, String paVertShadow, String paBlurDistance, String paSpreadDistance, String paShadowColor)
        {
            String lcShadowParam;

            paHorizShadow = String.IsNullOrEmpty(paHorizShadow) ? "0px" : paHorizShadow;
            paVertShadow = String.IsNullOrEmpty(paVertShadow) ? "0px" : paVertShadow;
            paBlurDistance = String.IsNullOrEmpty(paBlurDistance) ? "0px" : paBlurDistance;
            paSpreadDistance = String.IsNullOrEmpty(paSpreadDistance) ? "0px" : paSpreadDistance;
            paShadowColor = String.IsNullOrEmpty(paShadowColor) ? "rgba(0,0,0,1)" : paShadowColor;

            lcShadowParam = ctBoxShadow5Param.Replace("$HORIZSHADOW", paHorizShadow).Replace("$VERTSHADOW", paVertShadow).Replace("$SHADOWCOLOR", paShadowColor);
            lcShadowParam = lcShadowParam.Replace("$BLURDISTANCE", paBlurDistance).Replace("$SPREADDISTANCE", paSpreadDistance);
            AddMultipleStyle(clIncomaptibleStyleList[ctISLBoxShadow], lcShadowParam);
        }

        public void InsertBoxShadow(String paHorizShadow, String paVertShadow, String paBlurDistance, String paSpreadDistance, String paShadowColor)
        {
            String lcShadowParam;

            paHorizShadow = String.IsNullOrEmpty(paHorizShadow) ? "0px" : paHorizShadow;
            paVertShadow = String.IsNullOrEmpty(paVertShadow) ? "0px" : paVertShadow;
            paBlurDistance = String.IsNullOrEmpty(paBlurDistance) ? "0px" : paBlurDistance;
            paSpreadDistance = String.IsNullOrEmpty(paSpreadDistance) ? "0px" : paSpreadDistance;
            paShadowColor = String.IsNullOrEmpty(paShadowColor) ? "rgba(0,0,0,1)" : paShadowColor;

            lcShadowParam = ctBoxShadow5Param.Replace("$HORIZSHADOW", paHorizShadow).Replace("$VERTSHADOW", paVertShadow).Replace("$SHADOWCOLOR", paShadowColor);
            lcShadowParam = lcShadowParam.Replace("$BLURDISTANCE", paBlurDistance).Replace("$SPREADDISTANCE", paSpreadDistance);
            InsertMultipleStyle(clIncomaptibleStyleList[ctISLBoxShadow], lcShadowParam);
        }

        private void AddMultipleStyle(String[] paStyleList, String paValue)
        {
            for (int lcCount = 0; lcCount < paStyleList.Length; lcCount++)
                clComponentController.AddStyle(paStyleList[lcCount], paValue);
        }

        private void InsertMultipleStyle(String[] paStyleList, String paValue)
        {
            for (int lcCount = 0; lcCount < paStyleList.Length; lcCount++)
                clCSSStyleManager.AddStyle(paStyleList[lcCount], paValue);
        }

        private int GetStyleListIndex(CSSStyle paStyle)
        {
            for (int lcCount = 0; lcCount < clIncomaptibleStyleList.GetLength(0); lcCount++)
                if (clIncomaptibleStyleList[lcCount][0].ToUpper() == paStyle.ToString().Replace("_", "-").ToUpper()) return (lcCount);
            return (-1);
        }

        public void AddStyle(CSSStyle paStyle, String paValue)
        {
            int lcListIndex;

            if ((lcListIndex = GetStyleListIndex(paStyle)) != -1) AddMultipleStyle(clIncomaptibleStyleList[lcListIndex], paValue);
            else clComponentController.AddStyle(paStyle, paValue);
        }

        public void InsertStyle(CSSStyle paStyle, String paValue)
        {
            int lcListIndex;

            if ((lcListIndex = GetStyleListIndex(paStyle)) != -1) InsertMultipleStyle(clIncomaptibleStyleList[lcListIndex], paValue);
            else clCSSStyleManager.AddStyle(paStyle, paValue);
        }
    }

    public class CSSParser
    {
        const String ctExtractClassRegEx = @"((?i)class=['|""](?<classname>[^'""]*)['|""]\s*(;|$)){0,1}(?<stylegroup2>.*)";

        const String ctGRPClassName = "classname";
        const String ctGRPStyleGroup1 = "stylegroup1";
        const String ctGRPStyleGroup2 = "stylegroup2";

        public String ClassName { get; set; }
        public String Style { get; set; }

        public static CSSParser ParseCSSString(String paStyleStr)
        {
            return (new CSSParser(paStyleStr));
        }

        private CSSParser(String paStyleStr)
        {
            Match lcMatch;

            ClassName = String.Empty;
            Style = String.Empty;

            if (!String.IsNullOrWhiteSpace(paStyleStr))
            {
                if ((lcMatch = Regex.Match(paStyleStr, ctExtractClassRegEx)).Success)
                {
                    ClassName = lcMatch.Groups[ctGRPClassName].Value;
                    Style = lcMatch.Groups[ctGRPStyleGroup1].Value + lcMatch.Groups[ctGRPStyleGroup2].Value;
                }
            }
        }
    }
}
