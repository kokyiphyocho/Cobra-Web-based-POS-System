using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using CobraFoundation;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.UI.HtmlControls;

namespace CobraFrame
{
    public class AppManifestManager
    {
        private const String ctAndriodManifest  = "templates/manifest.json";
        private const String ctIOSWebClip       = "templates/webclip.xml";        
        private const String ctOrganizationID  = "GSHOP CLASSIC";
        private const String ctOrganization    = "GIZMO INTEGRATED SOLUTIONS";

                
        public enum Mode { Auto, FrontEnd, BackEnd }

        Mode                clMode;
        AppManifestRow      clActiveRow;

        public AppManifestRow ActiveRow { get { return (clActiveRow); } }

        public AppManifestManager(Mode paMode)
        {
            clMode            = paMode;            

            if (ApplicationFrame.GetInstance().ActiveSubscription.Status == SubscriptionManager.SubscriptionStatus.Valid)
                clActiveRow = GetAppManifestRow(ApplicationFrame.GetInstance().ActiveSubscription.ActiveRow.SubscriptionID);
        }

        public static AppManifestRow GetAppManifestRow()
        {
            AppManifestManager lcAppManifestManager;
            
            lcAppManifestManager = new AppManifestManager(Mode.Auto);

            return (lcAppManifestManager.ActiveRow);
        }
                
        public static void AddAndriodManifestLink()
        {
            HtmlLink lcManifestLink;

            lcManifestLink = new HtmlLink();
            lcManifestLink.Href = GetAndriodManfiestUrl();
            lcManifestLink.Attributes.Add("rel", "MANIFEST");
            ApplicationFrame.GetInstance().ActivePage.Header.Controls.Add(lcManifestLink);
        }

        public static void ResponseAndriodManifest(HttpContext paHttpContext, Mode paMode = Mode.Auto)
        {
            AppManifestManager lcAppManifestManager;


            lcAppManifestManager = new AppManifestManager(paMode);

            paHttpContext.Response.AddHeader("Content-Disposition", "attachment; filename=manifest.json");
            paHttpContext.Response.ContentType = "text/plain";
            paHttpContext.Response.Write(lcAppManifestManager.GetAndriodManifestContent());
            paHttpContext.Response.End();
        }

        public static void ResponseiOSManifest(HttpContext paHttpContext, Mode paMode = Mode.Auto)
        {
            AppManifestManager lcAppManifestManager;

            
            lcAppManifestManager = new AppManifestManager(paMode);

            paHttpContext.Response.AddHeader("Content-Disposition", "attachment; filename=webclip.mobileconfig");
            paHttpContext.Response.ContentType = "text/plain";
            paHttpContext.Response.Write(lcAppManifestManager.GetiOSManifestContent());
            paHttpContext.Response.End();                        
        }

        public static String GetAndriodManfiestUrl()
        {
            String   lcManifestUrl;

            if (ApplicationFrame.GetInstance().Status == ApplicationFrame.InitializationStatus.Success)
            {
                lcManifestUrl = ApplicationFrame.GetInstance().ActiveSubscription.GetSubscriptionUrl(SubscriptionManager.UrlType.AndriodManifest);

                return (lcManifestUrl);
            }
            else return (String.Empty);            
        }

        private AppManifestRow GetAppManifestRow(String paSubscriptionID)
        {
            DataTable lcDataTable;
            QueryClass lcQuery;

            lcQuery = new QueryClass(QueryClass.QueryType.GetAppManifestRow);
            lcQuery.ReplacePlaceHolder("$SUBSCRIPTIONID", paSubscriptionID, true);

            if (((lcDataTable = lcQuery.RunQuery()) != null) && (lcDataTable.Rows.Count > 0))
                return (new AppManifestRow(lcDataTable.Rows[0]));
            else return (null);
        }
        

        public String GetiOSManifestContent()
        {
            String          lcManifestData;

            try
            {
                lcManifestData = File.ReadAllText(HttpContext.Current.Server.MapPath(ctIOSWebClip));

                return (ReplaceIOSManifestPlaceHolder(lcManifestData));                    
            }
            catch { }

            return (String.Empty);
        }

        public String GetAndriodManifestContent()
        {
            String lcManifestData;

            try
            {
                lcManifestData = File.ReadAllText(HttpContext.Current.Server.MapPath(ctAndriodManifest));

                return(ReplaceAndriodManifestPlaceHolder(lcManifestData));                    
            }
            catch {                
            }

            return (String.Empty);
        }


        protected String GetBase64IconImage()
        {   
            String       lcImageString;
            String       lcPhysicalPath;

            lcImageString = String.Empty;

            if (!String.IsNullOrEmpty(lcPhysicalPath = GetEffectiveIconFile()))
            {
                lcPhysicalPath = HttpContext.Current.Server.MapPath(lcPhysicalPath);

                if (File.Exists(lcPhysicalPath))
                {
                    using (MemoryStream lcMemoryStream = new MemoryStream())
                    {
                        using (Image lcImage = Image.FromFile(lcPhysicalPath))
                        {
                            lcImage.Save(lcMemoryStream, ImageFormat.Png);
                            lcImageString = Convert.ToBase64String(lcMemoryStream.ToArray());
                        }
                    }
                }
            }

            return (lcImageString);
        }

        public String GetEffectiveURL()
        {
            if (ApplicationFrame.GetInstance().Status == ApplicationFrame.InitializationStatus.Success)
            {
                switch (clMode)
                {
                    case Mode.Auto      : return (ApplicationFrame.GetInstance().ActiveSubscription.GetSubscriptionUrl(SubscriptionManager.UrlType.ContextUrl)); 
                    case Mode.FrontEnd  : return (ApplicationFrame.GetInstance().ActiveSubscription.GetSubscriptionUrl(SubscriptionManager.UrlType.FrontEnd)); 
                    case Mode.BackEnd   : return (ApplicationFrame.GetInstance().ActiveSubscription.GetSubscriptionUrl(SubscriptionManager.UrlType.BackEnd)); 
                }
            }
            return (String.Empty);
        }

        public String GetEffectiveIconFile()
        {
            switch (clMode)
            {
                case Mode.Auto: return (ApplicationFrame.GetInstance().ActiveSubscription.ActiveMode == SubscriptionManager.Mode.FrontEnd ? clActiveRow.FrontEndIcon : clActiveRow.BackEndIcon);
                case Mode.FrontEnd: return (clActiveRow.FrontEndIcon);
                case Mode.BackEnd: return (clActiveRow.BackEndIcon);
            }

            return (String.Empty);
        }


        public String ReplaceAndriodManifestPlaceHolder(String paManifestData)
        {
            if ((clActiveRow != null) && (!String.IsNullOrEmpty(paManifestData)))
            {
                paManifestData = paManifestData.Replace("$APPNAME", clActiveRow.AppName);
                paManifestData = paManifestData.Replace("$SHORTNAME", clActiveRow.ShortName);
                paManifestData = paManifestData.Replace("$STARTURL", GetEffectiveURL()); 
                paManifestData = paManifestData.Replace("$DESCRIPTION", clActiveRow.Description);
                paManifestData = paManifestData.Replace("$THEMECOLOR", clActiveRow.BackgroundColor);
                paManifestData = paManifestData.Replace("$BACKGROUNDCOLOR", clActiveRow.BackgroundColor);
                paManifestData = paManifestData.Replace("$ICON", GetEffectiveIconFile());                

                return (paManifestData);
            }
            else return (String.Empty);
        }


        public String ReplaceIOSManifestPlaceHolder(String paManifestData)
        {            
            if ((clActiveRow != null) && (!String.IsNullOrEmpty(paManifestData)))
            {                
                paManifestData = paManifestData.Replace("$ICON", GetBase64IconImage());
                paManifestData = paManifestData.Replace("$SHORTNAME", clActiveRow.ShortName);
                paManifestData = paManifestData.Replace("$PAYLOADID", clActiveRow.SubscriptionID + "-" + ApplicationFrame.GetInstance().ActiveSubscription.ActiveMode.ToString().ToUpper());
                paManifestData = paManifestData.Replace("$PAYLOADUUID", clActiveRow.SubscriptionID + "-" + ApplicationFrame.GetInstance().ActiveSubscription.ActiveMode.ToString().ToUpper());
                paManifestData = paManifestData.Replace("$URL", GetEffectiveURL());
                paManifestData = paManifestData.Replace("$PAYLOADDESCRIPTION", clActiveRow.Description);
                paManifestData = paManifestData.Replace("$PAYLOADDISPLAYNAME", clActiveRow.AppName);
                paManifestData = paManifestData.Replace("$PAYLOADORGANIZATIONID", ApplicationFrame.GetInstance().ActiveEservice.ActiveRow.OrganizationID);
                paManifestData = paManifestData.Replace("$PAYLOADORGANIZATIONNAME", ApplicationFrame.GetInstance().ActiveEservice.ActiveRow.OrganizationName);

                return (paManifestData);
            }
            else return (String.Empty);
        }



        //if (ApplicationFrame.GetParameter("_sysreq") == "manifest.ios")
        //{
        //    if (ApplicationFrame.GetInstance().Status == ApplicationFrame.InitializationStatus.Success)
        //    {
        //        paHttpContext.Response.AddHeader("Content-Disposition", "attachment; filename=myfile.json");
        //        paHttpContext.Response.ContentType = "text/plain";
        //        paHttpContext.Response.Write("aaaa");
        //        paHttpContext.Response.End();

        //        ApplicationFrame

        //    }
        //    else paHttpContext.Response.Write("Fail"); 
        //}


    }
}
