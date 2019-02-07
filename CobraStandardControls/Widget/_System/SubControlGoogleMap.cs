using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using CobraFrame;
using CobraWebFrame;

namespace CobraStandardControls
{
    [ToolboxData("<{0}:CompositeGoogleMap runat=server></{0}:CompositeGoogleMap>")]
    public class SubControlGoogleMap : WebControl
    {
        public enum MapType { RoadMap, Satellite, Hybrid, Terrain }

        protected const String      ctCLSGoogleMapPanel     = "GoogleMap";

        protected const String      ctGoogleMapAPILink      = @"https://maps.googleapis.com/maps/api/js?v=3.exp&key=$APIKEY&callback=Initialize_GoogleMap";
        
        protected const String      ctDSNWidth              = "100px";
        protected const String      ctDSNHeight             = "100px";

        protected const String      ctDEFGoogleMapAPIKey    = "AIzaSyD96pwiILogHMtZR0wQcx3SQGBt_KrccZY";
        protected const Decimal     ctDEFLatitude           = 16.849693M;
        protected const Decimal     ctDEFLongitude          = 96.128917M;
        protected const int         ctDEFZoomLevel          = 16;
        protected const MapType     ctDefaultMapType        = MapType.RoadMap;
        
        public String   SC_GoogleMapAPIKey                  { get; set; }
        public Decimal  SC_Latitude                         { get; set; }
        public Decimal  SC_Longitude                        { get; set; }
        public int      SC_Zoom                             { get; set; }
        public MapType  SC_MapType                          { get; set; }
        public bool     SC_InstantLoad                      { get; set; }
        public bool     SC_ShowMarker                       { get; set; }

        public SubControlGoogleMap()
        {            
            SC_GoogleMapAPIKey  = ctDEFGoogleMapAPIKey;
            SC_Latitude         = ctDEFLatitude;
            SC_Longitude        = ctDEFLongitude;
            SC_Zoom             = ctDEFZoomLevel;
            SC_MapType          = ctDefaultMapType;
            SC_InstantLoad      = true;
            SC_ShowMarker       = true;            
        }
        
        private void IncludeExternalLinkFiles(ComponentController paComponentController)
        {
            JavaScriptManager lcJavaScriptmanager;
                        
            lcJavaScriptmanager = new JavaScriptManager(paComponentController);                        
            lcJavaScriptmanager.IncludeExternalJavaScript(ctGoogleMapAPILink.Replace("$APIKEY", SC_GoogleMapAPIKey));
        }        
        protected virtual void AddGoogleMapParamAttributes(ComponentController paComponentController)
        {
            paComponentController.AddElementType(ComponentController.ElementType.GoogleMap);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gmap_instantload, SC_InstantLoad ? "true" : "false");
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gmap_Latitude, SC_Latitude.ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gmap_Longitude, SC_Longitude.ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gmap_maptype, SC_MapType.ToString().ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gmap_zoom, SC_Zoom.ToString());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.gmap_showmarker, SC_ShowMarker ? "true" : "false");
        }

        protected virtual void RenderBrowserMode(ComponentController paComponentController)
        {            
            IncludeExternalLinkFiles(paComponentController);            
                        
            AddGoogleMapParamAttributes(paComponentController);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.RenderEndTag();
        }

        protected virtual void RenderDesignMode(ComponentController paComponentController)
        {
            this.Width = new Unit(ctDSNWidth);
            this.Height = new Unit(ctDSNHeight);

            paComponentController.AddStyle(CSSStyle.Border, "2px Solid Black");
            paComponentController.AddStyle(CSSStyle.Overflow, "hidden");
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write("Google Map");
            paComponentController.RenderEndTag();
        }

        protected override void Render(HtmlTextWriter paHtmlTextWriter)
        {
            if (!DesignMode) RenderBrowserMode(new ComponentController(paHtmlTextWriter));
            else RenderDesignMode(new ComponentController(paHtmlTextWriter));
        }

        public void RenderChildMode(ComponentController paComponentController)
        {
            RenderBrowserMode(paComponentController);
        }
    }
}
