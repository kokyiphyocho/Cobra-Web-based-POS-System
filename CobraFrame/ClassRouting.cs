using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;
using CobraFoundation;

namespace CobraFrame
{
    public class RoutingManager
    {
        const String ctFileNameRegEx        = @"(?<Path>.*[/])(?<FileName>[a-z_]+[.]aspx)\s*$";
        const String ctGRPPath              = "Path";
        const String ctGRPFileName          = "FileName";
        const String ctMobilePrefix         = "Mobile_";
        const String ctMobileFilePrefix     = "_Mobile_";

        DataTable               clRoutingTable;
        static RoutingManager   clRoutingManager;

        public static RoutingManager CreateInstance()
        {
            if (clRoutingManager == null) clRoutingManager = new RoutingManager();
            return (clRoutingManager);
        }

        private RoutingManager()
        {
            try { clRoutingTable = RetrieveRoutingTable(); }
            catch { }
        }

        private DataTable RetrieveRoutingTable()
        {
            QueryClass lcQuery;

            lcQuery = new QueryClass(QueryClass.QueryType.RetrieveRoutingTable);            
            return (lcQuery.RunQuery());
        }     

        public void RegisterRoutes(RouteCollection paRoutes)
        {
            RoutingRow lcRoutingRow;            

            paRoutes.RouteExistingFiles = true;

            paRoutes.Ignore("{resource}.axd/{*pathInfo}");            

            if (clRoutingTable != null)
            {
                lcRoutingRow = new RoutingRow(null);

                for (int lcCount = 0; lcCount < clRoutingTable.Rows.Count; lcCount++)
                {
                    lcRoutingRow.Row = clRoutingTable.Rows[lcCount];
                    paRoutes.MapPageRoute(lcRoutingRow.RouteName, lcRoutingRow.UrlPattern, lcRoutingRow.PhysicalFile);
                }
            }
        }
    }

    public class UrlRewritingManager
    {
        const String ctWWWPrefix            = "www.";
        const String ctPathDelimiter        = "/";
        const String ctExtensionDelimiter   = ".";
        const String ctPlaceHolderRegEx     = "[{]([^}]+)[}]";
        const String ctCOLPath              = "Path";
        const String ctCOLHostName          = "HostName";
        const String ctCOLRequestHostName   = "RequestHostName";

        String[] ctSystemFolderList = new String[] { "webresource.axd","upload/","upload_images/", "images/", "fonts/", "commonimages/", "scripts/", "styles/" };

        DataTable                       clUrlRewritingTable;        
        static UrlRewritingManager      clUrlRewritingManager;

        public static UrlRewritingManager CreateInsatnce()
        {
            if (clUrlRewritingManager == null) clUrlRewritingManager = new UrlRewritingManager();
            return (clUrlRewritingManager);
        }

        private UrlRewritingManager()
        {
            try
            {                
                clUrlRewritingTable = RetrieveUrlRewritingTable();
            }
            catch 
            {

            }
        }        

        private DataTable RetrieveUrlRewritingTable()
        {
            QueryClass lcQuery;

            lcQuery = new QueryClass(QueryClass.QueryType.RetrieveUrlRewriteTable);            
            return (lcQuery.RunQuery());
        }

        private String GetCorrespondingUrlPath(HttpContext paHttpContext, RouteValueDictionary paRouteValueDictionary)
        {
            String          lcUrlPath;
            UrlRewriteRow   lcUrlRewriteRow;
            String          lcQueryString;
            
            lcUrlRewriteRow = new UrlRewriteRow(clUrlRewritingTable.Rows[0]);

            lcUrlPath = lcUrlRewriteRow.Path.Trim();

            if (paRouteValueDictionary != null)
            {
                foreach (String lcKey in paRouteValueDictionary.Keys)
                    lcUrlPath = lcUrlPath.Replace("$" + lcKey.ToUpper(), Convert.ToString(paRouteValueDictionary[lcKey]));
            }

            if (!String.IsNullOrEmpty(paHttpContext.Request.Url.Query))
                lcQueryString = "&" + paHttpContext.Request.Url.Query.Substring(1);
            else lcQueryString = String.Empty;

            lcUrlPath += lcQueryString;

            return (lcUrlPath);
        }

        private bool IsSystemRoutes(ref String paPath)
        {
            String lcPathLowerCase;
            int lcIndex;

            lcPathLowerCase = paPath.ToLower();

            for (int lcCount = 0; lcCount < ctSystemFolderList.Length; lcCount++)
                if ((lcIndex = lcPathLowerCase.IndexOf(ctSystemFolderList[lcCount])) != -1)
                {
                    paPath = paPath.Substring(lcIndex);
                    return (true);
                }

            return (false);
        }
        

        public void RewriteUrl(HttpContext paHttpContext)
        {            
            String                  lcPath;                                              
            RouteValueDictionary    lcRouteValueDictionary;
            RouteData               lcRouteData;            

            lcPath = paHttpContext.Request.Url.LocalPath.Trim().Trim('/');

            if (IsSystemRoutes(ref lcPath))
            {
                paHttpContext.RewritePath("/" + lcPath);
                return;
            }

            if ((!String.IsNullOrEmpty(lcPath)) && (!lcPath.Contains("/")) && (!lcPath.Contains(".aspx")))
            {
                if ((lcRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(paHttpContext))) != null)
                    lcRouteValueDictionary = lcRouteData.Values;
                else
                    lcRouteValueDictionary = null;


                paHttpContext.RewritePath("/" + GetCorrespondingUrlPath(paHttpContext, lcRouteValueDictionary));
            }
            UrlCompression.PerformCompression(paHttpContext);
        }            
    }

    public static class UrlCompression
    {
        public static void PerformCompression(HttpContext paHttpContext)
        {
            String lcAllowEncodings;

            lcAllowEncodings = paHttpContext.Request.Headers.Get("Accept-Encoding");

            if (lcAllowEncodings != null)
            {
                lcAllowEncodings = lcAllowEncodings.ToLower();

                if (lcAllowEncodings.Contains("gzip"))
                {
                    paHttpContext.Response.Filter = new GZipStream(paHttpContext.Response.Filter, CompressionMode.Compress);
                    paHttpContext.Response.AppendHeader("Content-Encoding", "gzip");
                }
                else if (lcAllowEncodings.Contains("deflate"))
                {
                    paHttpContext.Response.Filter = new DeflateStream(paHttpContext.Response.Filter, CompressionMode.Compress);
                    paHttpContext.Response.AppendHeader("Content-Encoding", "deflate");
                }
                
            }
        }
    }
}
