﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Routing;
using CobraApplicationFrame;
using CobraFrame;
using System.IO.Compression;

namespace Cobra
{
    public class Global : System.Web.HttpApplication
    {

        void Application_Start(object sender, EventArgs e)
        {
           RoutingManager.CreateInstance().RegisterRoutes(RouteTable.Routes);
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown
        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Application_BeginRequest(object paSender, EventArgs paEventArgs)
        {
            HttpApplication lcApplication;
            UrlRewritingManager lcUrlRewritingManager;            

            lcApplication = (HttpApplication)paSender;            

            lcUrlRewritingManager = UrlRewritingManager.CreateInsatnce();
            lcUrlRewritingManager.RewriteUrl(lcApplication.Context);
        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started

        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }


    }
}