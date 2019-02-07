using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using CobraFoundation;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace CobraFrame
{
    public class WebStateBlock
    {
        const String ctWebStateParam = "__W";
        const String ctWebStateControl = "<input id='__WEBSTATE' type='hidden' value='__W=$WEBSTATE'/>";

        MetaDataBlock clMetaDataBlock;

        public bool HasData { get { return (clMetaDataBlock.MetaDataElementCount > 0); } }

        public WebStateBlock()
        {
            String lcWebStateBlock;
            MetaDataBlockCollection lcMetaBlockCollection;

            if (!String.IsNullOrEmpty(lcWebStateBlock = ApplicationFrame.GetParameter(ctWebStateParam)))
            {
                lcWebStateBlock = General.Base64Decode(lcWebStateBlock, true);
                if ((lcMetaBlockCollection = new MetaDataBlockCollection(lcWebStateBlock)) != null)
                    clMetaDataBlock = lcMetaBlockCollection[0];
                else
                    clMetaDataBlock = new MetaDataBlock();
            }
            else clMetaDataBlock = new MetaDataBlock();
        }

        private String GetCompliedWebStateBlock()
        {
            if (clMetaDataBlock != null)
                return (clMetaDataBlock.GetCompiledMetaDataBlockString());
            else return (null);
        }

        public void StoreWebStateVariable(String paVariableName, String paValue)
        {
            clMetaDataBlock.AddMetaDataElement(paVariableName, paValue);
        }

        public String GetData(String paVariableName)
        {
            MetaDataElement lcMetaDataElement;

            if ((lcMetaDataElement = clMetaDataBlock[paVariableName]) != null)
                return (lcMetaDataElement[0]);
            else return (null);
        }

        public bool RemoveData(String paVariableName)
        {
            return (clMetaDataBlock.RemoveMetaDataElement(paVariableName));
        }

        public LiteralControl WebStateControl()
        {
            return (new LiteralControl(ctWebStateControl.Replace("$WEBSTATE", HttpUtility.UrlEncode(General.Base64Encode(GetCompliedWebStateBlock())))));
        }
    }

    public class GlobalMetaBlock
    {
        protected const String ctREGEXGlobalMetaData = @"@[[]GLOBALMETA(::|[|][|])(?<MetaName>\w+)(::|[|][|])(?<Index>\d+)[]]";
        protected const String ctGRPMetaName = "MetaName";
        protected const String ctGRPIndex = "Index";

        MetaDataBlock clMetaDataBlock;

        public GlobalMetaBlock()
        {
            clMetaDataBlock = new MetaDataBlock();
        }

        public bool AddMetaDataElement(String paVariableName, MetaDataElement paMetaDataElement)
        {
            MetaDataElement lcMetaDataElement;


            if (paMetaDataElement != null)
            {
                lcMetaDataElement = paMetaDataElement.CopyObject();
                lcMetaDataElement.InsertNewElement(0, paMetaDataElement.Name);
                lcMetaDataElement.SetElementName(paVariableName);
                clMetaDataBlock.AddMetaDataElement(lcMetaDataElement);
                return (true);
            }
            else return (false);
        }

        public bool AddMetaDataElement(MetaDataElement paMetaDataElement)
        {
            MetaDataElement lcMetaDataElement;

            if (paMetaDataElement != null)
            {
                lcMetaDataElement = (MetaDataElement)paMetaDataElement.CopyObject();
                clMetaDataBlock.AddMetaDataElement(lcMetaDataElement);
                return (true);
            }
            else return (false);
        }

        public bool AddMetaDataElement(String paVariableName, String paMetaDataValue)
        {
            MetaDataElement lcMetaDataElement;

            if ((lcMetaDataElement = MetaDataElement.CreateMetaDataElement(paVariableName, paMetaDataValue)) != null)
            {
                clMetaDataBlock.AddMetaDataElement(lcMetaDataElement);
                return (true);
            }
            else return (false);
        }

        public bool AppendMetaDataBlock(MetaDataBlock paMetaDataBlock)
        {
            if (paMetaDataBlock != null)
            {
                clMetaDataBlock.AppendMetaDataBlock(paMetaDataBlock);
                return (true);
            }
            else return (false);
        }

        public String GetData(String paGlobalMetaElementName, int paValueIndex = 0, bool paSmartBase64Mode = true)
        {
            MetaDataElement lcMetaDataElement;

            if ((lcMetaDataElement = clMetaDataBlock[paGlobalMetaElementName]) != null)
                return (paSmartBase64Mode ? General.SmartBase64Decode(lcMetaDataElement[paValueIndex]) : lcMetaDataElement[paValueIndex]);
            else return (String.Empty);
        }

        public String TranslateGlobalMetaVariables(String paTextString)
        {
            int lcValueIndex;
            MatchCollection lcMatches;

            if (!String.IsNullOrEmpty(paTextString))
            {
                lcMatches = Regex.Matches(paTextString, ctREGEXGlobalMetaData);

                foreach (Match lcMatch in lcMatches)
                {
                    lcValueIndex = General.ParseInt(lcMatch.Groups[ctGRPIndex].Value, 0);
                    paTextString = paTextString.Replace(lcMatch.Value, GetData(lcMatch.Groups[ctGRPMetaName].Value, lcValueIndex));
                }
            }
            return (paTextString);
        }
    }

    public class ApplicationFrame
    {
        public enum InitializationStatus { Success, Fail };

        const char      ctDelimiter = ';';
        const string    ctFormStackDelimiter = "||";

        const String ctVARApplicationFrameInstance = "__ApplicationFrameInstance";        
        const String ctPRMServiceRequestToken      = "_e";
        const String ctPRMLanguage                 = "_l";
        const String ctPRMFormName                 = "_f";
        const String ctPRMFormStack                = "_s";

        const String ctMETAFormName                = "__FormName";
        const String ctDYQUpdateDemoDynamicDate    = "epos.updatedemodynamicdate";

        const int    ctSessionTimeOutMinutes       = 52594920; // 100 Years

        Page                        clPage;
        InitializationStatus        clInitializationStatus;
        ServiceRequestLogManager    clServiceRequestLogManager;
        SubscriptionManager         clSubscriptionManager;
        GlobalMetaBlock             clActiveGlobalMetaBlock;
        ClientScriptManager         clClientScriptManager;
        SessionController           clSessionController;
        WebStateBlock               clWebStateBlock;        
        String                      clServiceRequestToken;
        FormInfoManager             clFormInfoManager;        
        String                      clFormStack;
        bool                        clAjaxMode;
        
        public int SessionTimeOutMinutes                    { get { return (ctSessionTimeOutMinutes); } }

        public Page ActivePage                              { get { return (clPage); } }
        public InitializationStatus Status                  { get { return (clInitializationStatus); } }
        public SubscriptionManager ActiveSubscription       { get { return (clSubscriptionManager); } }
        public EServiceManager ActiveEservice               { get { return (clSubscriptionManager.ActiveEService); } }
        public GlobalMetaBlock ActiveGlobalMetaBlock        { get { return (clActiveGlobalMetaBlock); } }
        public ClientScriptManager ClientScriptManager      { get { return (clClientScriptManager); } }

        public ServiceRequestLogManager ActiveServiceRequestLogManager { get { return (clServiceRequestLogManager); } }

        public SessionController ActiveSessionController    { get { return (clSessionController); } }

        public WebStateBlock ActiveWebStateBlock            { get { return (clWebStateBlock); } }
                
        public String ActiveLanguageName                    { get { return (ActiveSubscription.ActiveLanguage.ActiveRow.Language); } }     

        public String ActiveServiceRequestToken             { get { return (clServiceRequestToken); } }

        public FormInfoManager ActiveFormInfoManager        { get { return (clFormInfoManager); } }

        public String FormStack                             { get { return (clFormStack); } }

        public bool IsAjaxMode                              { get { return (clAjaxMode); } }
        
        public static void InitalizeInstance(Page paPage)
        {
            new ApplicationFrame(paPage);                      
        }        

        public static ApplicationFrame GetInstance()
        {
            return ((ApplicationFrame) HttpContext.Current.Items[ctVARApplicationFrameInstance]);
        }
        
        public void ResponseManifest()
        {
            // To Response Manifest
        }

        private ApplicationFrame(Page paPage)
        {                  
            String      lcFormName;
                        
            HttpContext.Current.Items.Add(ctVARApplicationFrameInstance, this);

            clPage                  = paPage;
            clAjaxMode              = true;
            clClientScriptManager   = paPage.ClientScript;
            clWebStateBlock         = new WebStateBlock();
            clActiveGlobalMetaBlock = new GlobalMetaBlock();
            ConfigClass.GetInstance().ResetConfiguration();            

            if (!String.IsNullOrEmpty(clServiceRequestToken = GetStateParameter(ctPRMServiceRequestToken)))
            {
                if ((clSubscriptionManager = SubscriptionManager.CreateInstance(clServiceRequestToken, GetParameter(ctPRMLanguage))) != null)
                {
                    if (clSubscriptionManager.Status == SubscriptionManager.SubscriptionStatus.Valid)
                    { 
                        clServiceRequestLogManager = new ServiceRequestLogManager(clSubscriptionManager.ActiveRow.SubscriptionID);

                    clSessionController = new SessionController();

                   // if (clSubscriptionManager.Status == SubscriptionManager.SubscriptionStatus.Valid)
                   // {
                        /*    clSessionController.LogIn("AMT", "81DC9BDB52D04DC20036DBD8313ED055"); */

                        if (String.IsNullOrEmpty(lcFormName = GetParameter(ctPRMFormName)))
                            lcFormName = clSubscriptionManager.ActiveEService.GetEncodedPrimaryFormName();

                        clFormInfoManager = FormInfoManager.CreateInstance(lcFormName);

                        if (((clFormInfoManager == null) || (!clFormInfoManager.IsAttributeSet(FormInfoManager.FormAttribute.NoSession))) &&
                            (ActiveEservice.IsMandatorySession) && (ActiveSessionController.Status == SessionController.SessionStatus.NoSession))
                        {
                            clSessionController.LogOut();
                            lcFormName = clSubscriptionManager.ActiveEService.GetEncodedLogInFormName();
                            clFormInfoManager = FormInfoManager.CreateInstance(lcFormName);
                        }
                        //if ((ActiveEservice.IsMandatorySession) && (ActiveSessionController.Status == SessionController.SessionStatus.NoSession))
                        //    lcFormName = clSubscriptionManager.ActiveEService.GetEncodedLogInFormName();
                        //else if (String.IsNullOrEmpty(lcFormName = GetParameter(ctPRMFormName))) 
                        //        lcFormName = clSubscriptionManager.ActiveEService.GetEncodedPrimaryFormName();
                        // clFormInfoManager = FormInfoManager.CreateInstance(lcFormName);
                        
                        clFormStack = GetFormStack();
                        ActiveGlobalMetaBlock.AddMetaDataElement(MetaDataElement.CreateMetaDataElement(ctMETAFormName, lcFormName));
                        
                        if ((ApplicationFrame.GetInstance().ActiveSubscription.IsDemoMode()) && (ApplicationFrame.GetInstance().ActiveSubscription.IsDynamicDateMode()))
                        {
                            DynamicQueryManager.GetInstance().GetStringResult(DynamicQueryManager.ExecuteMode.NonQuery, ctDYQUpdateDemoDynamicDate, null);
                        }

                        clInitializationStatus = InitializationStatus.Success;
                    }
                    
                    //else
                    //{
                    //    lcFormName = clSubscriptionManager.ActiveEService.GetEncodedLogInFormName();
                    //    clFormInfoManager = FormInfoManager.CreateInstance(lcFormName);
                    //    clFormStack = GetFormStack();
                    //    ActiveGlobalMetaBlock.AddMetaDataElement(MetaDataElement.CreateMetaDataElement(ctMETAFormName, lcFormName));
                    //}

                    
                    return;
                }
            }
            else clServiceRequestLogManager = new ServiceRequestLogManager(String.Empty);

            clInitializationStatus = InitializationStatus.Fail;
        }             
        
        public void SetAjaxRequestMode(bool paAjaxMode)
        {
            clAjaxMode = paAjaxMode;
        }

        public void LogServiceRequestCompleteTime()
        {
            if (clServiceRequestLogManager != null)
                clServiceRequestLogManager.UpdateServiceRequestLogRecord(DateTime.Now);
        }
        
        private String GetFormStack()
        {
            String lcFormStackStr;
            String[] lcFormStack;
            String lcNewFromStack;
            String lcLastFormName;

            if ((ActiveFormInfoManager != null) && (!String.IsNullOrEmpty(lcFormStackStr = General.Base64Decode(GetStateParameter(ctPRMFormStack)))))
            {
                if (!clFormInfoManager.IsAttributeSet(FormInfoManager.FormAttribute.Desktop))
                {
                    lcFormStack = lcFormStackStr.Split(new String[] { ctFormStackDelimiter.ToString() }, StringSplitOptions.RemoveEmptyEntries);

                    if (lcFormStack.Length > 0)
                    {
                        lcLastFormName = lcFormStack[lcFormStack.Length - 1].Trim().Split(',')[0];
                        if (lcLastFormName.Trim() == ActiveFormInfoManager.ActiveRow.FormName.Trim())
                            Array.Resize(ref lcFormStack, lcFormStack.Length - 1);
                    }
                    //if ((lcFormStack.Length > 0) && (lcFormStack[lcFormStack.Length - 1].Trim() == ActiveFormInfoManager.ActiveRow.FormName.Trim()))
                    //    Array.Resize(ref lcFormStack, lcFormStack.Length - 1);

                    lcNewFromStack = String.Join(ctFormStackDelimiter.ToString(), lcFormStack);
                }
                else lcNewFromStack = String.Empty;

                lcNewFromStack = General.Base64Encode(lcNewFromStack);
                ActiveWebStateBlock.StoreWebStateVariable(ctPRMFormStack, lcNewFromStack);

                return(lcNewFromStack);                
            }
            return (String.Empty);            
        }

        public String[][] GetEmbeddedFontList()
        {
            if ((ActiveSubscription != null) && (ActiveSubscription.ActiveLanguage != null))
            {
                return (ActiveSubscription.ActiveLanguage.ActiveRow.EmbeddedFontList.Split(new String[] { ";;" }, StringSplitOptions.None)
                       .Select(s => s.Split(new String[] { "::" }, StringSplitOptions.None)).ToArray());
                //return (ActiveSubscription.ActiveLanguage.ActiveRow.EmbeddedFontList.Split(ctDelimiter));
            }
                
            else return (null);
        }

        public static String GetDecodedParameter(String paName)
        {
            String   lcParamValue;

            if (!String.IsNullOrWhiteSpace(lcParamValue = GetParameter(paName)))
            {
                lcParamValue = HttpUtility.UrlDecode(lcParamValue);
                lcParamValue = General.Base64Decode(lcParamValue);                
            }

            return (lcParamValue);
        }

        public static String GetParameter(String paName, String paDefault = null)
        {
            String  lcParamValue;

            lcParamValue = HttpContext.Current.Request.Form[paName] ?? HttpContext.Current.Request.QueryString[paName];
            if (String.IsNullOrEmpty(lcParamValue)) lcParamValue = paDefault;

            return (lcParamValue);
        }

        public String GetStateParameter(String paName)
        {
            String lcValue;

            if ((!String.IsNullOrEmpty(paName)) && (!String.IsNullOrEmpty(lcValue = GetParameter(paName))))
            {
                ActiveWebStateBlock.StoreWebStateVariable(paName, lcValue);
                return (lcValue);
            }
            else return (ActiveWebStateBlock.GetData(paName));
        }

        public static String GetRemoteIPAddress()
        {
            String lcRemoteIPAddress;

            lcRemoteIPAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (String.IsNullOrEmpty(lcRemoteIPAddress))
                lcRemoteIPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            if (string.IsNullOrEmpty(lcRemoteIPAddress))
                lcRemoteIPAddress = HttpContext.Current.Request.UserHostAddress;

            if (lcRemoteIPAddress == "::1") lcRemoteIPAddress = "127.0.0.1";

            return (lcRemoteIPAddress);
        }

        public String GetRequestHostName()
        {
            return (HttpContext.Current.Request.Url.Host);
        }

        public String GetUserAgent()
        {
            return (HttpContext.Current.Request.UserAgent);
        }

        public String[] GetEffectiveRoleList()
        {
            String lcEffectiveRoleList;
            String lcDelimiter;

            lcDelimiter = ctDelimiter.ToString();

            lcEffectiveRoleList = ActiveEservice.ActiveRow.EServiceRole + lcDelimiter + ActiveSubscription.ActiveRow.SubscriptionRole + lcDelimiter;
            
            if ((ActiveSessionController.Status == SessionController.SessionStatus.Active) && (ActiveSessionController.User != null))
                lcEffectiveRoleList = ActiveSessionController.User.ActiveRow.UserRole;

            return (lcEffectiveRoleList.Split(new String[] { lcDelimiter }, StringSplitOptions.RemoveEmptyEntries));            
        }
       
    }
}
