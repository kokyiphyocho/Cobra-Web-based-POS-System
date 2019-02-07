using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using CobraFoundation;

namespace CobraFrame
{
    public class SessionManager
    {
        SessionRow  clActiveRow;
        bool        clNewRecord;

        public SessionRow ActiveRow { get { return (clActiveRow); } }
        public bool NewRecord { get { return (clNewRecord); } }

        public static SessionManager GetExistingSession(String paSessionID)
        {
            SessionRow lcSessionRow;

            if ((lcSessionRow = GetSessionRow(paSessionID)) != null)
                return (new SessionManager(lcSessionRow, false));
            else return (null);
        }

        public static SessionManager CreateNewSession(String paSesionID)
        {
            if (!String.IsNullOrWhiteSpace(paSesionID))
                return (new SessionManager(CreateNewSessionRow(paSesionID), true));
            else return (null);
        }

        private SessionManager(SessionRow paSessionRow, bool paNewRecord)
        {
            clActiveRow = paSessionRow;
            clNewRecord = paNewRecord;
        }

        protected static SessionRow GetSessionRow(String paSessionKey)
        {
            QueryClass lcQuery;
            DataTable lcDataTable;

            lcQuery = new QueryClass(QueryClass.QueryType.GetSessionRowBySessionKey);            
            lcQuery.ReplacePlaceHolder("$SESSIONKEY", paSessionKey, true);            

            if (((lcDataTable = lcQuery.RunQuery()) != null) && (lcDataTable.Rows.Count > 0))
                return (new SessionRow(lcDataTable.Rows[0]));
            else return (null);
        }

        protected static SessionRow GetSessionRow(String paSubscriptionID, String paLoginID)
        {
            QueryClass lcQuery;
            DataTable lcDataTable;

            lcQuery = new QueryClass(QueryClass.QueryType.GetSessionRow);
            lcQuery.ReplacePlaceHolder("$SUBSCRIPTIONID", paSubscriptionID, true);
            lcQuery.ReplacePlaceHolder("$LOGINID", paLoginID, true);

            if (((lcDataTable = lcQuery.RunQuery()) != null) && (lcDataTable.Rows.Count > 0))
                return (new SessionRow(lcDataTable.Rows[0]));
            else return (null);
        }

        public void UpdateSessionAccessInfo()
        {
            QueryClass lcQuery;

            lcQuery = new QueryClass(QueryClass.QueryType.UpdateSessionAccessInfo);
            lcQuery.ReplacePlaceHolder("$SUBSCRIPTIONID", ActiveRow.SubscriptionID, true);
            lcQuery.ReplacePlaceHolder("$LASTACCESSTIME", General.ConvertUTCToSystemLocalTime(DateTime.UtcNow).ToString("yyyy-MM-dd HH:mm:ss"), true);
            lcQuery.ReplacePlaceHolder("$LOGINID", ActiveRow.LoginID, true);

            lcQuery.ExecuteNonQuery();
        }

        protected static SessionRow CreateNewSessionRow(String paSessionKey)
        {
            SessionRow lcSessionRow;

            lcSessionRow = new SessionRow(TableManager.GetInstance().GetNewRow(TableManager.TableType.Session, true));
            lcSessionRow.SubscriptionID = ApplicationFrame.GetInstance().ActiveSubscription.ActiveRow.SubscriptionID;
            lcSessionRow.SessionKey = paSessionKey;            
            lcSessionRow.SessionOwnerIP = ApplicationFrame.GetRemoteIPAddress();
            lcSessionRow.StartTime = General.ConvertUTCToSystemLocalTime(DateTime.UtcNow);
            lcSessionRow.ExpiryTime = lcSessionRow.StartTime.AddMinutes(ApplicationFrame.GetInstance().SessionTimeOutMinutes);
            lcSessionRow.FirstAccessTime = lcSessionRow.StartTime;
            lcSessionRow.LastAccessTime = lcSessionRow.StartTime;
            lcSessionRow.AccessCount = 1;

            return (lcSessionRow);
        }

        public bool InsertRecord()
        {
            QueryClass lcQuery;

            if (clNewRecord)
            {
                lcQuery = new QueryClass(QueryClass.QueryType.InsertSessionRow);                
                lcQuery.ReplaceRowPlaceHolder(clActiveRow.Row);

                return (lcQuery.ExecuteNonQuery() > 0);
            }
            else return (false);
        }
    }

    public class SessionController
    {
        public enum SessionStatus { Active, NoSession };

        const String ctCKSFrontEndSessionCookiesBlock  = "DinamicV-FrontEnd-InfoBlock";
        const String ctCKSBackEndSessionCookiesBlock   = "DinamicV-BackEnd-InfoBlock";
        const String ctCCLSmartSessionKey              = "SmartSessionKey";

        // For Security Info
        const String ctSubscriptionID               = "SubscriptionID";
        const String ctClientIP                     = "ClientIP";        
        const String ctSessionStartTime             = "SessionStartTime";
        const String ctDEMOAccountID                = "DEMO";
        const String ctDEMOAccountPassword          = "DEMO";
        
        String clSmartSectionID;
        SessionStatus clSessionStatus;
        UserManager clUserManager;
        SmartCookieManager clSmartCookieManager;
        // JavaScriptSerializer clJavaScriptSerializer;        

        public SessionStatus Status { get { return (clSessionStatus); } }
        public UserManager User { get { return (clUserManager); } }        

        public SessionController()
        {
            clSessionStatus         = SessionStatus.NoSession;
            clUserManager           = new UserManager();
           //  clJavaScriptSerializer = new JavaScriptSerializer();

            if (ApplicationFrame.GetInstance().ActiveSubscription.ActiveMode == SubscriptionManager.Mode.FrontEnd)
                clSmartCookieManager = new SmartCookieManager(ctCKSFrontEndSessionCookiesBlock);
            else
                clSmartCookieManager = new SmartCookieManager(ctCKSBackEndSessionCookiesBlock);
            
            if (ApplicationFrame.GetInstance().ActiveSubscription.IsDemoMode())
            {
                LogIn(ctDEMOAccountID, ctDEMOAccountPassword);
            }
            
            clSmartSectionID        = clSmartCookieManager.GetCookieData(ctCCLSmartSessionKey);
                        
            try { if (VerifySessionKey() && VerifySessionInfo()) { clSessionStatus = SessionStatus.Active; } }
            catch (Exception paException) { Console.WriteLine(paException.Message); }
        }        

        private bool VerifySessionKey()
        {
            String lcSessionSeurityData;

            Dictionary<String, String> lcSecurityInfo;

            if ((!String.IsNullOrWhiteSpace(clSmartSectionID)) && ((lcSessionSeurityData = RijdaelEncryption.GetInstance().DecryptString(clSmartSectionID)) != null))
            {
                try { lcSecurityInfo = General.JSONDeserialize<Dictionary<String, String>>(lcSessionSeurityData); }
                catch { lcSecurityInfo = null; }

                if (lcSecurityInfo != null)
                {
                    if ((lcSecurityInfo[ctSubscriptionID] == ApplicationFrame.GetInstance().ActiveSubscription.ActiveRow.SubscriptionID))
                        return (true);
                }
            }
            return (false);
        }

        private bool VerifySessionInfo()
        {
            SessionManager lcSessionManager;

            if (!String.IsNullOrWhiteSpace(clSmartSectionID))
            {
                if (((lcSessionManager = SessionManager.GetExistingSession(clSmartSectionID)) != null) &&
                    (clUserManager.LogIn(lcSessionManager.ActiveRow.LoginID, lcSessionManager.ActiveRow.Password)))
                {
                    lcSessionManager.UpdateSessionAccessInfo();
                    return (true);
                }
            }
            return (false);
        }

        public bool LogIn(String paLogInName, String paPassword)
        {
            Dictionary<String, String> lcSecurityInfo;

            if (clUserManager.LogIn(paLogInName, paPassword))
            {
                lcSecurityInfo = new Dictionary<String, String>();

                lcSecurityInfo.Add(ctSubscriptionID, ApplicationFrame.GetInstance().ActiveSubscription.ActiveRow.SubscriptionID);
                lcSecurityInfo.Add(ctClientIP, ApplicationFrame.GetRemoteIPAddress());                
                lcSecurityInfo.Add(ctSessionStartTime, DateTime.Now.Ticks.ToString());
                CreateSession(RijdaelEncryption.GetInstance().EncryptString(General.JSONSerialize(lcSecurityInfo)));

                return (true);
            }
            else return (false);
        }

        private void CreateSession(String paSessionID)
        {
            SessionManager lcSessionManager;

            if (clUserManager.IsLoggedIn)
            {
                lcSessionManager = SessionManager.CreateNewSession(paSessionID);
                lcSessionManager.ActiveRow.LoginID = clUserManager.ActiveRow.LoginID;
                lcSessionManager.ActiveRow.Password = clUserManager.ActiveRow.Password;
                lcSessionManager.InsertRecord();
            }
            AddSmartCookies(paSessionID);
        }

        private void AddSmartCookies(String paSessionKey)
        {
            clSmartCookieManager.CreateCookie();
            clSmartCookieManager.AddCookieData(ctCCLSmartSessionKey, paSessionKey);
            clSmartCookieManager.SaveCookie();
        }

        public void LogOut()
        {
            TerminateSession();
        }

        private void TerminateSession()
        {
            if (clSmartCookieManager != null)
                clSmartCookieManager.RemoveCookie();
            if (clUserManager != null)
                clUserManager.LogOut();
        }       
    }

    public class SmartCookieManager
    {
        String clCookieName;
        HttpCookie clHttpCookie;

        bool clActive;

        public bool Active { get { return (clActive); } }

        public SmartCookieManager(String paCookieName)
        {
            clCookieName = paCookieName;
            clActive = (clHttpCookie = HttpContext.Current.Request.Cookies[paCookieName]) != null;
        }

        public String GetCookieData(String paKey)
        {
            if (clHttpCookie != null) return (clHttpCookie[paKey]);
            return (null);
        }

        public void CreateCookie()
        {
            if (clActive) clHttpCookie.Values.Clear();
            else
            {
                clHttpCookie = new HttpCookie(clCookieName);                
                clHttpCookie.Expires = DateTime.Now.AddYears(5);
                clActive = true;
            }
        }

        public bool AddCookieData(String paName, String paValue)
        {
            if (clActive)
            {
                clHttpCookie.Values.Add(paName, paValue);                
                return (true);
            }
            else return (false);

        }

        public bool SaveCookie()
        {
            if (clActive)
            {
                HttpContext.Current.Response.Cookies.Add(clHttpCookie);
                return (true);
            }
            else return (false);
        }

        public bool RemoveCookie()
        {
            if (clActive)
            {
                clHttpCookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(clHttpCookie);
                clHttpCookie = null;
                clActive = false;
                return (true);
            }
            else return (false);
        }
    }
}
