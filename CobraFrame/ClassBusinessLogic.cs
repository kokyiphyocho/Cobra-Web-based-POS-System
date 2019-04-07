using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CobraFoundation;
using System.Web;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;

namespace CobraFrame
{
    public class LanguageManager
    {
        const   char    ctFontListDelimiter     = ';';
        public  enum    Language                { English, Myanmar }
        const   String  ctSystemDigits          = "0123456789";
        const   String  ctLocal                 = "local";

        const String ctSETLanguageOption        = "_LANGUAGEOPTIONS";
        
        String          clLanguage;
        LanguageRow     clLanguageRow;
        String          clDigitList;
        TextManager     clTextManager;
        
        public LanguageRow  ActiveRow           { get { return (clLanguageRow); } }
        public String       LocalDigits         { get { return (clDigitList); } }

        public LanguageManager(String paLanguage)
        {
            clLanguage = !String.IsNullOrWhiteSpace(paLanguage) ? paLanguage : Language.English.ToString();

            if (!String.IsNullOrEmpty(clLanguage))
                clLanguageRow = RetrieveLanguageRow(clLanguage.Trim());
                        
            if ((ApplicationFrame.GetInstance().ActiveSubscription.Status == SubscriptionManager.SubscriptionStatus.Valid) && 
                (ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting.LocalNumberMode))
            {
                if (clLanguageRow.DigitList.Trim().Length == 10) clDigitList = clLanguageRow.DigitList.Trim();
                else clDigitList = ctSystemDigits;
            }
            else clDigitList = ctSystemDigits;

            clTextManager = new TextManager(clLanguage);
        }        

        private LanguageRow RetrieveLanguageRow(String paLanguage)
        {
            DataTable  lcDataTable;
            QueryClass lcQuery;

            lcQuery = new QueryClass(QueryClass.QueryType.GetLanguageRow);
            lcQuery.ReplacePlaceHolder("$LANGUAGE", paLanguage, true);

            if (((lcDataTable = lcQuery.RunQuery()) != null) && (lcDataTable.Rows.Count > 0)) return (new LanguageRow(lcDataTable.Rows[0]));
            else return (null);
        }

        public String[] GetEmbeddedFontList()
        {
            if (clLanguageRow != null) return (clLanguageRow.EmbeddedFontList.Split(ctFontListDelimiter));
            else return (null);
        }

        public static Language ParseLanguage(String paLanguage)
        {
            return (General.ParseEnum<Language>(paLanguage, Language.English));
        }

        public String ConvertNumber(String paTextStr)
        {
            if ((clDigitList != null) && (!String.IsNullOrEmpty(paTextStr)))
            {
                for (int lcNumber = 0; lcNumber <= 9; lcNumber++)
                    paTextStr = paTextStr.Replace(lcNumber.ToString(), clDigitList[lcNumber].ToString());                
            }
            return (paTextStr);
        }

        public String GetText(String paTextKey, String paDefaultValue = "")
        {
            if (!String.IsNullOrWhiteSpace(paTextKey))
            {
                return (clTextManager.GetText(paTextKey.ToUpper(), paDefaultValue));
            }
            return (paDefaultValue);
        }

        public String GetLanguageAwareText(String paLanguageStr)
        {
            MetaDataBlockCollection lcMetaDataBlockCollection;
            MetaDataBlock           lcMetaDataBlock;           

            if (MetaDataBlockCollection.IsMetaBlockString(paLanguageStr))
            {
                lcMetaDataBlockCollection = new MetaDataBlockCollection(paLanguageStr);
                if ((lcMetaDataBlock = lcMetaDataBlockCollection[0]) != null)
                {
                    return(lcMetaDataBlock.GetData(clLanguage, String.Empty));
                }

                return (String.Empty);
            }
            else return (paLanguageStr);
        }

        public DataTable GetLanguageOptionList()
        {
            String      lcLanguageOption;

            lcLanguageOption = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting.GetSettingValue(ctSETLanguageOption);

            return(RetrieveLanguateOptionList(General.CompileSqlConditionListStr(lcLanguageOption)));       
        }

        private DataTable RetrieveLanguateOptionList(String paLanguageOptionList)
        {
            QueryClass lcQuery;

            lcQuery = new QueryClass(QueryClass.QueryType.GetLanguageOptionList);
            lcQuery.ReplacePlaceHolder("$LANGUAGEOPTIONLIST", paLanguageOptionList, false);                                        

            return (lcQuery.RunQuery());
        }
    }
    
    public class TimeZoneManager
    {
        const String ctCOLTimeZoneID    = "TimeZoneID";
        const String ctCOLDescription   = "Description";

        private static TimeZoneManager clTimeZoneManager;

        private DataTable clTimeZoneList;

        public static TimeZoneManager GetInstance()
        {
            if (clTimeZoneManager == null) clTimeZoneManager = new TimeZoneManager();
            return (clTimeZoneManager);
        }

        public String GetJSONTable()
        {   
            if (clTimeZoneList != null) return(JsonConvert.SerializeObject(clTimeZoneList, new JsonSerializerSettings { ContractResolver = new LowercaseContractResolver() }));
            else return ("{}");
        }

        public Dictionary<String,String> GetTimeZoneDictionary()
        {
            if (clTimeZoneList != null)
            {
                return (clTimeZoneList.AsEnumerable().ToDictionary<DataRow, String, String>(key => key.Field<String>(ctCOLTimeZoneID), value => value.Field<String>(ctCOLDescription)));
            }
            else return(new Dictionary<String, String>());
        }

        private TimeZoneManager()
        {
            clTimeZoneList = RetriveTimeZoneList();            
        }

        private DataTable RetriveTimeZoneList()
        {
            QueryClass  lcQuery;

            lcQuery = new QueryClass(QueryClass.QueryType.GetTimeZoneList);

            return(lcQuery.RunQuery());
        }
    }   

    public class TextManager
    {
        const String ctCOLTextKey   = "TextKey";
        const String ctCOLText      = "Text";
        
        String                      clLanguage;
        Dictionary<String, String>  clTextDictionary;

        public TextManager(String paLanguage)
        {            
            clLanguage = paLanguage;
            clTextDictionary = GetTextDictionary(RetrieveTextList(paLanguage));
        }

        private Dictionary<String, String> GetTextDictionary(DataTable paDatatable)
        {
            if (paDatatable != null)
            {
                return (paDatatable.AsEnumerable().ToDictionary<DataRow, String, String>(row => row.Field<String>(ctCOLTextKey), row => row.Field<String>(ctCOLText)));
            }
            else return (new Dictionary<string,string>());
        }

        private DataTable RetrieveTextList(String paLanguage)
        {            
            QueryClass lcQuery;

            lcQuery = new QueryClass(QueryClass.QueryType.RetrieveTextList);
            lcQuery.ReplacePlaceHolder("$LANGUAGE", paLanguage, true);

            return(lcQuery.RunQuery());
        }

        public String GetText(String paTextKey, String paDefaultValue)
        {                          
            if (clTextDictionary.ContainsKey(paTextKey)) return(clTextDictionary[paTextKey]);
            else  return(paDefaultValue);
        }
    }

    public class EServiceManager
    {
        public  enum EServiceAttribute { SessionMode, FixLoginIDMode, PinCodeMode }
        private enum DBParameters { HostName, DatabaseName, UserName, Password}
        
        EServiceRow     clEserviceRow;
        
        public EServiceRow ActiveRow { get { return (clEserviceRow); } }

        public bool IsMandatorySession          { get { return (IsAttributeSet(EServiceAttribute.SessionMode)); } }
        public bool IsFixLoginIDMode            { get { return (IsAttributeSet(EServiceAttribute.FixLoginIDMode)); } }
        public bool IsPinCodeMode               { get { return (IsAttributeSet(EServiceAttribute.PinCodeMode)); } }
                
        public static EServiceManager CreateInstance(String paEServiceID)
        {
            EServiceRow lcEServiceRow;

            if ((!String.IsNullOrEmpty(paEServiceID)) && ((lcEServiceRow = RetrieveEServiceRow(paEServiceID.Trim())) != null))
            {
                return (new EServiceManager(lcEServiceRow));
            }
            else return (null);
        }

        private EServiceManager(EServiceRow paEServiceRow)
        {
            if ((clEserviceRow = paEServiceRow) != null) ConfigureEServiceRemoteConnection();                   
        }        

        private static EServiceRow RetrieveEServiceRow(String paEServiceID)
        {
            DataTable  lcDataTable;
            QueryClass lcQuery;

            lcQuery = new QueryClass(QueryClass.QueryType.GetEServiceRow);
            lcQuery.ReplacePlaceHolder("$ESERVICEID", paEServiceID, true);

            if ((lcDataTable = lcQuery.RunQuery()) != null) return (new EServiceRow(lcDataTable.Rows[0]));
            else return (null);
        }

        private void ConfigureEServiceRemoteConnection()
        {
            MetaDataBlockCollection lcMetaDataBlockCollection;
            String lcHostName;
            String lcDatabaseName;
            String lcUserName;
            String lcPassword;

            lcMetaDataBlockCollection = new MetaDataBlockCollection(clEserviceRow.DBParameters);

            if ((lcMetaDataBlockCollection != null) && (lcMetaDataBlockCollection.MetaDataBlockCount > 0))
            {
                lcHostName = lcMetaDataBlockCollection[0].GetData(DBParameters.HostName.ToString(), String.Empty);
                lcDatabaseName = lcMetaDataBlockCollection[0].GetData(DBParameters.DatabaseName.ToString(), String.Empty);
                lcUserName = lcMetaDataBlockCollection[0].GetData(DBParameters.UserName.ToString(), String.Empty);
                lcPassword = lcMetaDataBlockCollection[0].GetData(DBParameters.Password.ToString(), String.Empty);
                ConfigClass.GetInstance().SetEServiceRemoteConnection(lcHostName, lcDatabaseName, lcUserName, lcPassword);                    
            }            
        }

        public bool IsAttributeSet(EServiceAttribute paEServiceAttribute)
        {
            return (ActiveRow.Attribute.Contains("#" + paEServiceAttribute.ToString().ToUpper()));
        }

        public String GetEncodedLogInFormName()
        {         
            if (ApplicationFrame.GetInstance().ActiveSubscription.ActiveMode == SubscriptionManager.Mode.FrontEnd)
                return (HttpUtility.UrlDecode(General.Base64Encode(ActiveRow.FrontEndLogInFormName)));
            else
                return (HttpUtility.UrlDecode(General.Base64Encode(ActiveRow.BackEndLogInFormName)));
        }

        public String GetPrimaryFormName()
        {
            if (ApplicationFrame.GetInstance().ActiveSubscription.ActiveMode == SubscriptionManager.Mode.FrontEnd)
                return (ActiveRow.FrontEndFormName);
            else
                return (ActiveRow.BackEndFormName);
        }    

        public String GetEncodedPrimaryFormName()
        {            
            if (ApplicationFrame.GetInstance().ActiveSubscription.ActiveMode == SubscriptionManager.Mode.FrontEnd)
                return (HttpUtility.UrlDecode(General.Base64Encode(ActiveRow.FrontEndFormName)));
            else
                return (HttpUtility.UrlDecode(General.Base64Encode(ActiveRow.BackEndFormName)));
        }        
    }
  
    public class SubscriptionManager
    {
        public enum SubscriptionAttribute { DemoMode, DynamicDate }

        private const int       ctServiceRequestTokenLength   = 64;
        private const int       ctMaxFrontEndPathLength       = 20;
        private const String    ctMETAWallPaper               = "WALLPAPER";
        private const String    ctImageUrlTemplate            = "url($IMAGENAME) no-repeat center center fixed";
        private const String    ctSeparator                   = ";;";

        private const String ctIOSManifestFE                  = "_sysreq=manifest.iosfrontend";
        private const String ctIOSManifestBE                  = "_sysreq=manifest.iosbackend";
        private const String ctAndriodManifestFE              = "_sysreq=manifest.andriodfrontend";
        private const String ctAndriodManifestBE              = "_sysreq=manifest.andriodbackend";
        private const String ctIOSQRCodeFE                    = "_sysreq=qrcode.iosfrontend";
        private const String ctIOSQRCodeBE                    = "_sysreq=qrcode.iosbackend";
        private const String ctAndriodQRCodeFE                = "_sysreq=qrcode.andriodfrontend";
        private const String ctAndriodQRCodeBE                = "_sysreq=qrcode.andriodbackend";
        
        public enum SubscriptionStatus { Valid, Suspend, Expired, Invalid }
        public enum ServiceStatus      { Active, Suspend, Cancel, Invalid }
        public enum Mode               { FrontEnd, BackEnd }
        public enum UrlType { ContextUrl, FrontEnd, BackEnd,
                              AndriodManifest, AndriodFEManifest, AndriodBEManifest, iOSFEManifest, iOSBEManifest, 
                              AndriodFEQRCode, AndriodBEQRCode, iOSFEQRCode, iOSBEQRCode  }
        
        String              clSubscriptionID;
        SubscriptionRow     clSubscriptionRow;        
        SubscriptionStatus  clStatus;
        EServiceManager     clEServiceManager;
        LanguageManager     clLanguageManager;
        WidgetSubscriptionController clWidgetSubscriptionController;      
        // MetaDataBlock       clWallPaper;
    //    MetaDataBlock       clToolBarConfig;      
        Mode                clMode;
        String              clServiceRequestToken;
        SettingManager      clSettingManager;
        

        public SubscriptionRow      ActiveRow      { get { return (clSubscriptionRow); } }
        public SubscriptionStatus   Status         { get { return (clStatus); } }
        public EServiceManager      ActiveEService { get { return (clEServiceManager); } }
        public LanguageManager      ActiveLanguage { get { return (GetLanguageManager()); } }
        public Mode                 ActiveMode     { get { return (clMode); } }
        public WidgetSubscriptionController ActiveWidgetSubscription { get { return (clWidgetSubscriptionController); } }
        public SettingManager       ActiveSetting  { get { return (GetSettingManager()); } }

        public static SubscriptionManager CreateInstance(String paServiceRequestToken, String paLanguage)
        {
            String  lcSubscriptionID;
            Mode    lcMode;

            if (!String.IsNullOrEmpty(paServiceRequestToken))
            {
                if (!String.IsNullOrEmpty(lcSubscriptionID = ParseToken(paServiceRequestToken, out lcMode)))                
                    return(new SubscriptionManager(paServiceRequestToken, lcSubscriptionID, lcMode, paLanguage));                
            }
            return(null);
        }

        private static String ParseToken(String paEServiceToken, out Mode paMode)
        {
            String lcDecodedString;
            String lcHashCode;
            String lcSubscriptionID;

            paMode = Mode.BackEnd;

            try
            {
                if (paEServiceToken.Length < ctMaxFrontEndPathLength)
                {
                    paMode = Mode.FrontEnd;
                    return (paEServiceToken);
                }
                else
                {   
                    //Error Fixing for Demo QR CODe /* REMOVE */
                    paEServiceToken = paEServiceToken.Split(new String[]{ "https" },StringSplitOptions.RemoveEmptyEntries)[0]; // To Remove
                    lcDecodedString = General.Base64Decode(paEServiceToken);
                    if (lcDecodedString.Length == ctServiceRequestTokenLength)
                    {
                        lcHashCode = lcDecodedString.Substring(0, ctServiceRequestTokenLength / 2);
                        lcSubscriptionID = lcDecodedString.Substring(ctServiceRequestTokenLength / 2, ctServiceRequestTokenLength / 2);
                        if (General.GetMd5Hash(lcSubscriptionID) == lcHashCode)
                            return (lcSubscriptionID);
                    }
                }
            }
            catch { }

            return (null);
        }

        private SubscriptionManager(String paServiceRequestToken, String paSubscriptionID, Mode paMode, String paLanguage)
        {
            clServiceRequestToken = paServiceRequestToken;
            clSubscriptionID      = paSubscriptionID;
            clMode                = paMode;

            if (!String.IsNullOrEmpty(paSubscriptionID))
            {
                paSubscriptionID = paSubscriptionID.Trim();

                if (paSubscriptionID.Length <= ctMaxFrontEndPathLength)
                    clSubscriptionRow = GetSubscriptionRowByFrontEndPath(paSubscriptionID);
                else
                    clSubscriptionRow = GetSubscriptionRow(paSubscriptionID.Trim());
            }

            if (clSubscriptionRow != null)
            {                
                SetCorrespondingStatus();             
               // clWallPaper = new MetaDataBlockCollection(ActiveRow.WallPaper)[0];
              //  clToolBarConfig = new MetaDataBlockCollection(ActiveRow.ToolBarConfig)[0];
             //   clOrderStatusType = new MetaDataBlockCollection(ActiveRow.OrderStatusType)[0];
                clEServiceManager = EServiceManager.CreateInstance(clSubscriptionRow.EServiceID);
                clWidgetSubscriptionController = new WidgetSubscriptionController(this);

                //paLanguage = String.IsNullOrEmpty(paLanguage) ? clEServiceManager.ActiveRow.DefaultLanguage : paLanguage;
                //clLanguageManager = new LanguageManager(paLanguage);
            }
            else clStatus = SubscriptionStatus.Invalid;
        }

        private SubscriptionRow GetSubscriptionRow(String paSubscriptionID)
        {
            DataTable lcDataTable;
            QueryClass lcQuery;

            lcQuery = new QueryClass(QueryClass.QueryType.GetSubscriptionRow);
            lcQuery.ReplacePlaceHolder("$SUBSCRIPTIONID", paSubscriptionID, true);

            if (((lcDataTable = lcQuery.RunQuery()) != null) && (lcDataTable.Rows.Count > 0))
                return (new SubscriptionRow(lcDataTable.Rows[0]));
            else return (null);
        }

        private SubscriptionRow GetSubscriptionRowByFrontEndPath(String paFrontEndPath)
        {
            DataTable lcDataTable;
            QueryClass lcQuery;

            lcQuery = new QueryClass(QueryClass.QueryType.GetSubscriptionRowByFrontEndPath);
            lcQuery.ReplacePlaceHolder("$FRONTENDPATH", paFrontEndPath, true);

            if (((lcDataTable = lcQuery.RunQuery()) != null) && (lcDataTable.Rows.Count > 0))
                return (new SubscriptionRow(lcDataTable.Rows[0]));
            else return (null);
        }

        public bool IsDemoMode()
        {
            return (IsAttributeSet(SubscriptionAttribute.DemoMode));
        }

        public bool IsDynamicDateMode()
        {
            return (IsAttributeSet(SubscriptionAttribute.DynamicDate));
        }

        public bool IsAttributeSet(SubscriptionAttribute paSubscriptionAttribute)
        {
            if (ActiveRow != null)
                return (ActiveRow.SubscriptionAttribute.Contains("#" + paSubscriptionAttribute.ToString().ToUpper()));
            else return (false);
        }

        private SettingManager GetSettingManager()
        {           
            if (clSettingManager == null) clSettingManager = new SettingManager();
            return (clSettingManager);
        }

        private LanguageManager GetLanguageManager()
        {
            if (clLanguageManager == null)            
                clLanguageManager = new LanguageManager(ActiveSetting.Language);

            return (clLanguageManager);            
        }

        private void SetCorrespondingStatus()
        {
            DateTime    lcMMStandardTime;
            ServiceStatus lcServiceStatus;

            lcMMStandardTime = General.ConvertUTCToSystemLocalTime(DateTime.UtcNow);

            try
            {
                if (ActiveRow.ActivationDate.Date > lcMMStandardTime.Date) clStatus = SubscriptionStatus.Invalid;
                else if (ActiveRow.ExpiryDate.Date < lcMMStandardTime.Date) clStatus = SubscriptionStatus.Expired;
                else
                {
                    lcServiceStatus = General.ParseEnum<ServiceStatus>(ActiveRow.Status, ServiceStatus.Invalid);
                    if (lcServiceStatus == ServiceStatus.Suspend) clStatus = SubscriptionStatus.Suspend;
                    else if (lcServiceStatus == ServiceStatus.Active) clStatus = SubscriptionStatus.Valid;
                    else clStatus = SubscriptionStatus.Invalid;
                }
            }
            catch
            {
                clStatus = SubscriptionStatus.Invalid;
            }
        }

        public String GetCustomWallPaper(String paFormName)
        {
            String lcWallPaper;

            if ((!String.IsNullOrEmpty(paFormName)) && ((lcWallPaper = clSettingManager.WallPaper.GetData(paFormName.ToLower())) != null))                 
            {
                if (lcWallPaper.IndexOfAny(new char[] {'/','\\'}) != -1)
                    return (ctImageUrlTemplate.Replace("$IMAGENAME", lcWallPaper));
                else return (lcWallPaper);
            }

            return (null);
        }

        //public String GetCustomToolBarLink(String paItemName)
        //{
        //    MetaDataElement lcMetaDataElement;

        //    if ((!String.IsNullOrEmpty(paItemName)) && (clToolBarConfig != null) &&
        //        ((lcMetaDataElement = clToolBarConfig[paItemName]) != null))
        //    {
        //        return (lcMetaDataElement[0]);
        //    }

        //    return (null);
        //}

        //public String GetOrderStatusText(int paOrderStatusCode)
        //{
        //    MetaDataElement lcMetaDataElement;

        //    if ((clOrderStatusType != null) &&
        //        ((lcMetaDataElement = clOrderStatusType[paOrderStatusCode.ToString()]) != null))
        //    {
        //        return (lcMetaDataElement[0]);
        //    }

        //    return (paOrderStatusCode.ToString());
        //}

        //public String[] GetRejectReasonList()
        //{
        //    return (ActiveRow.RejectReasonList.Split(new String[] { ctSeparator }, StringSplitOptions.RemoveEmptyEntries));
        //}

        public String GetHomeForm()
        {
            if (ActiveMode == Mode.FrontEnd) return(ActiveEService.ActiveRow.FrontEndFormName);
            else return (ActiveEService.ActiveRow.BackEndFormName);
        }

        public String GetLandingPage()
        {
            if (clMode == Mode.FrontEnd) return ("/" + clServiceRequestToken);
            else return ("?_e=" + clServiceRequestToken);
        }        

        //public String GetSubscriptionUrl()
        //{
        //    String lcBaseUrl;
            
        //    lcBaseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            
        //    return (lcBaseUrl + GetLandingPage());
        //}

        private String QueryStringPrefix()
        {
            if (clMode == Mode.FrontEnd) return ("?");
            else return ("&");
        }

        public String GetSubscriptionUrl(UrlType paUrlType)
        {
            String lcBaseUrl;

            lcBaseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            switch(paUrlType)
            {
                case UrlType.ContextUrl         : return (lcBaseUrl + GetLandingPage());
                case UrlType.FrontEnd           : return (lcBaseUrl + "/" + ActiveRow.FrontEndPath);
                case UrlType.BackEnd            : return (lcBaseUrl + "?_e=" + ActiveRow.ServiceRequestToken);
                case UrlType.iOSFEManifest      : return (GetSubscriptionUrl(UrlType.ContextUrl) + QueryStringPrefix() + ctIOSManifestFE);
                case UrlType.iOSBEManifest      : return (GetSubscriptionUrl(UrlType.ContextUrl) + QueryStringPrefix() + ctIOSManifestBE);
                case UrlType.AndriodManifest    : return (GetSubscriptionUrl(UrlType.ContextUrl) + QueryStringPrefix() + (clMode == Mode.FrontEnd ? ctAndriodManifestFE : ctAndriodManifestBE));
                case UrlType.AndriodFEManifest  : return (GetSubscriptionUrl(UrlType.ContextUrl) + QueryStringPrefix() + ctAndriodManifestFE);
                case UrlType.AndriodBEManifest  : return (GetSubscriptionUrl(UrlType.ContextUrl) + QueryStringPrefix() + ctAndriodManifestBE);
                case UrlType.iOSFEQRCode        : return (GetSubscriptionUrl(UrlType.ContextUrl) + QueryStringPrefix() + ctIOSQRCodeFE);
                case UrlType.iOSBEQRCode        : return (GetSubscriptionUrl(UrlType.ContextUrl) + QueryStringPrefix() + ctIOSQRCodeBE);
                case UrlType.AndriodFEQRCode    : return (GetSubscriptionUrl(UrlType.ContextUrl) + QueryStringPrefix() + ctAndriodQRCodeFE);
                case UrlType.AndriodBEQRCode    : return (GetSubscriptionUrl(UrlType.ContextUrl) + QueryStringPrefix() + ctAndriodQRCodeBE);
            }

            return (String.Empty);
        }

        public Edition GetEdition()
        {
            return (General.ParseEnum<Edition>(ActiveSetting.EditionName, Edition.Classic));
        }

        //public String GetFrontEndUrl()
        //{
        //    String lcBaseUrl;

        //    lcBaseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

        //    return (lcBaseUrl + "/" + ActiveRow.FrontEndPath);
        //}

        //public String GetBackEndUrl()
        //{
        //    String lcBaseUrl;

        //    lcBaseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

        //    return (lcBaseUrl + "?_e=" + ActiveRow.ServiceRequestToken);        
        //}

        //public String GetIOSFrontEndManifestUrl()
        //{
        //    String lcBaseUrl;

        //    lcBaseUrl = GetSubscriptionUrl();

        //    if (clMode == Mode.FrontEnd) return (lcBaseUrl + "?" + ctIOSManifestFE);
        //    else return (lcBaseUrl + "&" + ctIOSManifestBE);
        //}

        //public String GetIOSBackEndManifestUrl()
        //{
        //    String lcBaseUrl;

        //    lcBaseUrl = GetSubscriptionUrl();

        //    if (clMode == Mode.FrontEnd) return (lcBaseUrl + "?" + ctIOSManifestBE);
        //    else return (lcBaseUrl + "&" + ctIOSManifestBE);
        //}

        //public String GetAndriodFrontEndManifestUrl()
        //{
        //    String lcBaseUrl;

        //    lcBaseUrl = GetSubscriptionUrl();

        //    if (clMode == Mode.FrontEnd) return (lcBaseUrl + "?" + ctAndriodManifestFE);
        //    else return (lcBaseUrl + "&" + ctAndriodManifestFE);
        //}

        //public String GetAndriodBackEndManifestUrl()
        //{
        //    String lcBaseUrl;

        //    lcBaseUrl = GetSubscriptionUrl();

        //    if (clMode == Mode.FrontEnd) return (lcBaseUrl + "?" + ctAndriodManifestBE);
        //    else return (lcBaseUrl + "&" + ctAndriodManifestBE);
        //}
    }


    public class UploadManager
    {
        const String ctReceiptLogoFileName      = "ReceiptLogo.png";
        const String ctUploadPathPlaceHolder    = "$UPLOADPATH";
        const String ctPTHUploadPath            = "upload/$SUBSCRIPTIONID";

        const String ctUploadManagerInstance    = "__UploadManagerInstance";       

        public String LastError                     { get; set; }
        public String UploadPath                    { get; set; }
        public String UploadPathPlaceHolder         { get { return (ctUploadPathPlaceHolder); } }
        public String ReceiptLogoFileName           { get { return (ctReceiptLogoFileName); } }

        public static UploadManager GetInstance()
        {
            if (HttpContext.Current.Items[ctUploadManagerInstance] == null) HttpContext.Current.Items[ctUploadManagerInstance] = new UploadManager();
            return ((UploadManager) HttpContext.Current.Items[ctUploadManagerInstance]);
        }

        public UploadManager()
        {
            UploadPath = ctPTHUploadPath.Replace("$SUBSCRIPTIONID", ApplicationFrame.GetInstance().ActiveSubscription.ActiveRow.SubscriptionID);
        }

        public bool UploadFiles()
        {
            if (HttpContext.Current.Request.Files.Count > 0)
            {
                for (int lcCount = 0; lcCount < HttpContext.Current.Request.Files.Count; lcCount++)
                {
                    if (!SaveFile(HttpContext.Current.Request.Files[0]))
                        return(false);
                }
            }

            return (true);
        }

        private bool CreateUploadPath()
        {
            String   lcPhysicalPath;

            lcPhysicalPath = HttpContext.Current.Server.MapPath(UploadPath);

            try
            {
                if (!Directory.Exists(lcPhysicalPath))
                {
                    Directory.CreateDirectory(lcPhysicalPath);
                }
            }
            catch(Exception paException)
            {
                LastError = paException.Message;

                return (false);
            }

            return (true);
        }

        private bool SaveFile(HttpPostedFile paHttpPostedFile)
        {
            try
            {
                LastError = null;

                if ((paHttpPostedFile != null) && (CreateUploadPath()))
                {                    
                    if (!String.IsNullOrEmpty(paHttpPostedFile.FileName))
                    {                        
                        paHttpPostedFile.SaveAs(HttpContext.Current.Server.MapPath(UploadPath + "/" + paHttpPostedFile.FileName));

                        return (true);
                    }
                }
            }
            catch (Exception paException) 
            {
                LastError = paException.Message;
            }            

            return (false);
        }

        public String ReplaceUploadPath(String paFilePath)
        {
            if ((!String.IsNullOrEmpty(paFilePath)) && (paFilePath.StartsWith(ctUploadPathPlaceHolder)))
            {
                return (paFilePath.Replace(ctUploadPathPlaceHolder, ctPTHUploadPath).Replace("$SUBSCRIPTIONID", ApplicationFrame.GetInstance().ActiveSubscription.ActiveRow.SubscriptionID));
            }
            else return (paFilePath);
        }
    }

    public class WidgetManager
    {
        WidgetRow       clWidgetRow;

        public WidgetRow ActiveRow { get { return (clWidgetRow); } }

        public static WidgetManager CreateInstance(String paWidgetName)
        {
            WidgetRow lcWidgetRow;

            if ((!String.IsNullOrWhiteSpace(paWidgetName)) && ((lcWidgetRow = GetWidgetRow(paWidgetName)) != null))            
                return (new WidgetManager(lcWidgetRow));
            
            return (null);
        }

        public static WidgetRow GetWidgetRow(String paWidgetName)
        {
            DataTable lcDataTable;
            QueryClass lcQuery;

            lcQuery = new QueryClass(QueryClass.QueryType.GetWidgetRow);
            lcQuery.ReplacePlaceHolder("$WIDGETNAME", paWidgetName, true);            

            if (((lcDataTable = lcQuery.RunQuery()) != null) && (lcDataTable.Rows.Count > 0))
                return (new WidgetRow(lcDataTable.Rows[0]));
            else return (null);
        }

        public WidgetManager(WidgetRow paWidgetRow)
        {
            clWidgetRow = paWidgetRow;
        }       
    }

    public class WidgetSubscriptionController
    {
        DataTable       clWidgetSubscriptionList;
        SubscriptionManager clSubscriptionManager;

        public DataTable ActiveTable { get { return (clWidgetSubscriptionList == null ? clWidgetSubscriptionList = RetrieveWidgetSubscriptionList() : clWidgetSubscriptionList); } }
        
        public WidgetSubscriptionController(SubscriptionManager paSubScriptionManager)
        {
            clSubscriptionManager = paSubScriptionManager;
        }       

        private  DataTable RetrieveWidgetSubscriptionList()
        {
            QueryClass      lcQuery;

            lcQuery = new QueryClass(QueryClass.QueryType.RetrieveWidgetSubscriptionList);
            lcQuery.ReplacePlaceHolder("$ESERVICEID", clSubscriptionManager.ActiveRow.EServiceID, true);
            lcQuery.ReplacePlaceHolder("$SUBSCRIPTIONID", clSubscriptionManager.ActiveRow.SubscriptionID, true);
            
            if (ApplicationFrame.GetInstance().ActiveSessionController.User.ActiveRow != null)
                lcQuery.ReplacePlaceHolder("$USERTYPE",ApplicationFrame.GetInstance().ActiveSessionController.User.ActiveRow.Type, true);
            else lcQuery.ReplacePlaceHolder("$USERTYPE", String.Empty, true);                

            return (lcQuery.RunQuery());
        }
    }

    public class DynamicQueryManager
    {
        private const char   ctSplitter             = '.';
        private const String ctFLTRowFilter         = "QueryGroup = '$QUERYGROUP' And QueryName = '$QUERYNAME'";
        private const String ctCOLCompiledAddress   = "Vir_CompiledAddress";
        private const String ctUpdate               = "UPDATE";
        private const String ctDemoException        = "[#ERROR];err_demoupdateaccess;Cannot use update feature in DEMO mode.";
        private const String ctSystemException      = "System Exception";

        public enum ExecuteMode { Scalar, NonQuery }
        DataTable       clDynamicQueryTable;        

        private static  DynamicQueryManager clDynamicQueryManager;

        public static DynamicQueryManager GetInstance()
        {
            if (clDynamicQueryManager == null) clDynamicQueryManager = new DynamicQueryManager();
            return (clDynamicQueryManager);
        }

        public String GetStringResult(ExecuteMode paExecuteMode, String paQueryName, String paJSONParameter, String paDefaultValue = null)
        {
            String              lcQueryGroupName;
            DynamicQueryRow     lcDynamicQueryRow;
            String              lcResult;

            ParseQueryName(ref paQueryName, out lcQueryGroupName);
            
            if ((lcDynamicQueryRow = GetDynamicQueryRow(lcQueryGroupName, paQueryName)) != null)
            {
                if ((lcResult = GetStringOutput(paExecuteMode, lcDynamicQueryRow, paJSONParameter)) == null) return(paDefaultValue);
                else return (lcResult);
            }
            else return (paDefaultValue);
        }

        public DataRow GetDataRowResult(String paQueryName, String paJSONParamList = null)
        {
            String              lcQueryGroupName;
            DynamicQueryRow     lcDynamicQueryRow;
            
            ParseQueryName(ref paQueryName, out lcQueryGroupName);

            if ((lcDynamicQueryRow = GetDynamicQueryRow(lcQueryGroupName, paQueryName)) != null)
                return (GetDataRowOutput(lcDynamicQueryRow, paJSONParamList));            
            else return (null);
        }

        public DataTable GetDataTableResult(String paQueryName)
        {
            String          lcQueryGroupName;
            DynamicQueryRow lcDynamicQueryRow;

            ParseQueryName(ref paQueryName, out lcQueryGroupName);

            if ((lcDynamicQueryRow = GetDynamicQueryRow(lcQueryGroupName, paQueryName)) != null)
                return (GetDataTableOutput(lcDynamicQueryRow));
            else return (null);
        }

        public String[,] GetTableStringArrayResult(String paQueryName)
        {
            DataTable   lcDataTable;

            if ((lcDataTable = GetDataTableResult(paQueryName)) != null)
            {
                return (General.MultiDimensionStringArray(lcDataTable));
            }
            else return (null);
        }

        public QueryClass GetQueryClass(String paQueryName)
        {
            String lcQueryGroupName;
            DynamicQueryRow lcDynamicQueryRow;

            ParseQueryName(ref paQueryName, out lcQueryGroupName);

            if ((lcDynamicQueryRow = GetDynamicQueryRow(lcQueryGroupName, paQueryName)) != null)
                return (GetQueryClass(lcDynamicQueryRow));
            else return (null);
        }

        public Dictionary<String, String> GetDataRowDictionary(String paQueryName, String paJSONParamList = null)
        {
            return (General.GetFormattedDataRowDictionary(GetDataRowResult(paQueryName, paJSONParamList)));
        }

        private DataTable RetrieveDynamicQueryTable()
        {            
            QueryClass lcQuery;

            lcQuery = new QueryClass(QueryClass.QueryType.RetrieveDynamicQueryTable);
            return (lcQuery.RunQuery());
        }

        private DynamicQueryRow GetDynamicQueryRow(String paQueryGroupName, String paQueryName)
        {
            DataRow[]           lcDataRows;
            DynamicQueryRow     lcDynamicQueryRow;

            if (clDynamicQueryTable != null)
            {
                if ((lcDataRows = clDynamicQueryTable.Select(ctFLTRowFilter.Replace("$QUERYGROUP", paQueryGroupName).Replace("$QUERYNAME", paQueryName))).Length > 0)
                {
                    lcDynamicQueryRow = new DynamicQueryRow(lcDataRows[0]);
                    if ((ApplicationFrame.GetInstance().ActiveSubscription.IsDemoMode()) && (lcDynamicQueryRow.AccessType.ToUpper() == ctUpdate))
                        throw new Exception(ctSystemException, new Exception(ctDemoException));

                    return (lcDynamicQueryRow);
                }
                    
            }
            return (null);
        }

        private void ParseQueryName(ref String paQueryName, out String paQueryGroupName)
        {
            String[]  lcSplittedStr;

            paQueryGroupName = ApplicationFrame.GetInstance().ActiveFormInfoManager.InputInfoDataGroup;
                        
            if (!String.IsNullOrEmpty(paQueryName))
            {
                lcSplittedStr = paQueryName.Split(new char[] { ctSplitter }, 2);

                if (lcSplittedStr.Length > 1)
                {
                    paQueryName = lcSplittedStr[1];
                    paQueryGroupName = lcSplittedStr[0];
                }
                else paQueryName = lcSplittedStr[0];
            }
        }

        private DynamicQueryManager()
        {
            clDynamicQueryTable = RetrieveDynamicQueryTable();         
        }

        private DataRow GetDataRowOutput(DynamicQueryRow paDynamicQueryRow, String paJSONParamList)
        {
            // JavaScriptSerializer lcJavaScriptSerializer;
            Dictionary<String, String> lcParamList;

            lcParamList = null;

            if (!String.IsNullOrEmpty(paJSONParamList))
            {
               // lcJavaScriptSerializer = new JavaScriptSerializer();
                if (!String.IsNullOrEmpty(paJSONParamList))
                    lcParamList = General.JSONDeserialize<Dictionary<String, String>>(paJSONParamList);                    
                  //  lcParamList = lcJavaScriptSerializer.Deserialize<Dictionary<String, String>>(paJSONParamList);                    
            }

            return (GetDataRowOutput(paDynamicQueryRow, lcParamList));            
        }

        private DataRow GetDataRowOutput(DynamicQueryRow paDynamicQueryRow, Dictionary<String, String> paParameter)
        {
            QueryClass lcQuery;
            DataTable  lcDataTable;
            QueryClass.ConnectionMode lcConnectionMode;

            lcConnectionMode = General.ParseEnum<QueryClass.ConnectionMode>(paDynamicQueryRow.ConnectionMode, QueryClass.ConnectionMode.None);
            lcQuery = new QueryClass(paDynamicQueryRow.Query, lcConnectionMode);            
            lcQuery.ReplaceDictionaryPlaceHolder(paParameter);
            ApplicationFrame.GetInstance().ActiveFormInfoManager.ReplaceQueryPlaceHolder(lcQuery);

            if (((lcDataTable = lcQuery.RunQuery()) != null) && (lcDataTable.Rows.Count > 0))
            {
                if (lcDataTable.Columns.Contains(ctCOLCompiledAddress)) 
                    lcDataTable.Rows[0][ctCOLCompiledAddress] = UILogic.CompileAddress(lcDataTable.Rows[0]);                    

                return (lcDataTable.Rows[0]);
            }
            else return (null);
        }

        private DataTable GetDataTableOutput(DynamicQueryRow paDynamicQueryRow)
        {
            QueryClass lcQuery;            
            QueryClass.ConnectionMode lcConnectionMode;

            lcConnectionMode = General.ParseEnum<QueryClass.ConnectionMode>(paDynamicQueryRow.ConnectionMode, QueryClass.ConnectionMode.None);
            lcQuery = new QueryClass(paDynamicQueryRow.Query, lcConnectionMode);

            ApplicationFrame.GetInstance().ActiveFormInfoManager.ReplaceQueryPlaceHolder(lcQuery);

            return (lcQuery.RunQuery());            
        }

        private QueryClass GetQueryClass(DynamicQueryRow paDynamicQueryRow)
        {
            QueryClass lcQuery;
            QueryClass.ConnectionMode lcConnectionMode;

            lcConnectionMode = General.ParseEnum<QueryClass.ConnectionMode>(paDynamicQueryRow.ConnectionMode, QueryClass.ConnectionMode.None);
            lcQuery = new QueryClass(paDynamicQueryRow.Query, lcConnectionMode);

            ApplicationFrame.GetInstance().ActiveFormInfoManager.ReplaceQueryPlaceHolder(lcQuery);

            return (lcQuery);
        }

        private String GetStringOutput(ExecuteMode paExecuteMode, DynamicQueryRow paDynamicQueryRow, Dictionary<String,String> paParameter)
        {
            QueryClass                  lcQuery;
            QueryClass.ConnectionMode   lcConnectionMode;

            lcConnectionMode = General.ParseEnum<QueryClass.ConnectionMode>(paDynamicQueryRow.ConnectionMode, QueryClass.ConnectionMode.None);
            lcQuery = new QueryClass(paDynamicQueryRow.Query, lcConnectionMode);
            lcQuery.ReplaceDictionaryPlaceHolder(paParameter);
            ApplicationFrame.GetInstance().ActiveFormInfoManager.ReplaceQueryPlaceHolder(lcQuery);

            if (paExecuteMode == ExecuteMode.Scalar)
                return (Convert.ToString(lcQuery.GetResult()));
            else
                return (Convert.ToString(lcQuery.ExecuteNonQuery()));
        }

        private String GetStringOutput(ExecuteMode paExecuteMode, DynamicQueryRow paDynamicQueryRow, String paJSONParamList)
        {
            //JavaScriptSerializer lcJavaScriptSerializer;
            Dictionary<String, String> lcParamList;

            if (!String.IsNullOrEmpty(paJSONParamList))
            {
              //  lcJavaScriptSerializer = new JavaScriptSerializer();
                if (((lcParamList = General.JSONDeserialize<Dictionary<String, String>>(paJSONParamList)) != null) && (lcParamList.Count > 0))
                {
                    return (GetStringOutput(paExecuteMode, paDynamicQueryRow, lcParamList));
                }
                else return (null);
            }
            else
            {
                lcParamList = new Dictionary<String, String>();
                return (GetStringOutput(paExecuteMode, paDynamicQueryRow, lcParamList));
            }
            
        }
    }

    public class UserManager
    {        
        UserRow     clUserRow;
        Result      clLastResult;

        public enum Result  { Success, InvalidUserName, InvalidPassword, AccessDenied }

        public enum UserRole { User, Admin, SuperAdmin, Reseller }
                
        const String ctROLAll = "*";

        const String ctRoleSplitter = ";";

        public UserRow ActiveRow { get { return (clUserRow); } }
        public Result LastResult { get { return (clLastResult); } }
        public bool IsLoggedIn { get { return (clUserRow != null); } }

        public bool LogIn(String paLoginID, String paPassword)
        {
            UserRow     lcUserRow;
            bool        lcSuccess;

            lcSuccess = false;
            clUserRow = null;

            if ((lcUserRow = GetUserRow(ApplicationFrame.GetInstance().ActiveSubscription.ActiveRow.SubscriptionID, paLoginID)) != null)
            {
                if (lcUserRow.Password.ToUpper() == paPassword.ToUpper())
                {
                    clUserRow = lcUserRow;
                    clLastResult = Result.Success;
                    lcSuccess = true;
                }
                else clLastResult = Result.InvalidPassword;
            }
            else clLastResult = Result.InvalidUserName;

            return (lcSuccess);
        }

        public object GetUserRowData(String paColumnName, object paDefaultValue = null)
        {

            if ((!String.IsNullOrEmpty(paColumnName)) && (ActiveRow != null) && (ActiveRow.Row.Table.Columns.Contains(paColumnName)))
                return (ActiveRow.Row[paColumnName]);
            else return (paDefaultValue);
        }

        public UserRow GetUserRow(String paSubscriptionID, String paLoginID)
        {
            DataTable lcDataTable;
            QueryClass lcQuery;

            lcQuery = new QueryClass(QueryClass.QueryType.GetUserRow);
            
            lcQuery.ReplacePlaceHolder("$LOGINID", paLoginID, true);
            lcQuery.ReplacePlaceHolder("$SUBSCRIPTIONID", paSubscriptionID, true);

            if (((lcDataTable = lcQuery.RunQuery()) != null) && (lcDataTable.Rows.Count > 0))
                return (new UserRow(lcDataTable.Rows[0]));
            else return (null);
        }

        public String GetLastResultStr()
        {
            switch (LastResult)
            {
                case Result.Success: return ("Login successful.");
                case Result.InvalidPassword: return ("Invalid Password was entered.");
                case Result.InvalidUserName: return ("Invalid Username was entered.");
                default: return (String.Empty);
            }
        }         
   
        public void LogOut()
        {
            clUserRow = null;
        }

        public bool IsRoleSet(UserRole paUserRole)
        {
            if (clUserRow != null)
                return (clUserRow.UserRole.Contains("#" + paUserRole.ToString().ToUpper()));
            else
                return (false);
        } 

        public bool IsAdminUser()
        {
            return (IsRoleSet(UserRole.Admin));
        }

        public bool VerifyPermission(String paRequireRole)
        {
            String[]  lcRequireRoleList;
            
            if ((IsLoggedIn) && (!String.IsNullOrWhiteSpace(paRequireRole)))
            {
                paRequireRole = paRequireRole.Trim();

                lcRequireRoleList = paRequireRole.Split(new String[] { ctRoleSplitter }, StringSplitOptions.RemoveEmptyEntries);

                for (int lcCount = 0; lcCount < lcRequireRoleList.Length; lcCount++)
                {
                    if ((lcRequireRoleList[lcCount] == ctROLAll) || (ActiveRow.UserRole.Contains(lcRequireRoleList[lcCount])))
                       return(true);
                }
            }

            return (false);
        }
    }

    //public class PaymentManager
    //{
    //    PaymentRow clPaymentRow;

    //    public PaymentRow ActiveRow { get { return (clPaymentRow); } }

    //    public static PaymentManager CreateInstance(int paTransactionID)
    //    {
    //        PaymentRow lcPaymentRow;

    //        if ((lcPaymentRow = GetPaymentRow(paTransactionID)) != null)
    //            return (new PaymentManager(lcPaymentRow));

    //        return (null);
    //    }

    //    public static PaymentRow GetPaymentRow(int paTransactionID)
    //    {
    //        DataTable lcDataTable;
    //        QueryClass lcQuery;

    //        lcQuery = new QueryClass(QueryClass.QueryType.GetPaymentRow);
    //        lcQuery.ReplacePlaceHolder("$TRANSACTIONID", paTransactionID.ToString(), false);

    //        if (((lcDataTable = lcQuery.RunQuery()) != null) && (lcDataTable.Rows.Count > 0))
    //            return (new PaymentRow(lcDataTable.Rows[0]));
    //        else return (null);
    //    }

    //    public PaymentManager(PaymentRow paPaymentRow)
    //    {
    //        clPaymentRow = paPaymentRow;
    //    }
    //}

    public class SettingManager
    {
        const String ctCOLSettingGroup              = "SettingGroup";
        const String ctCOLSettingKey                = "SettingKey";
        const String ctCOLSettingValue              = "SettingValue";

        //const String ctSETCurrencyDecimal           = "__CURRENCYDECIMAL";
        //const String ctSETCurrencyCode              = "__CURRENCYCODE";
        //const String ctSETCurrencySymbol            = "__CURRENCYSYMBOL";
        //const String ctSETDateFormat                = "__DATEFORMAT";
        //const String ctSETStaticDisplayDateFormat   = "__STATICDISPLAYDATEFORMAT";
        
        //const String ctSETThousandSeparator         = "__THOUSANDSEPARATOR";
        //const String ctSETLocalTimeZone             = "__LOCALTIMEZONE";

        //const String ctSETLocalNumberMode           = "_LOCALNUMBERMODE";
        //const String ctSETLanguage                  = "_LANGUAGE";          

        const String ctSETFormProtocolList          = "__FORMPROTOCOLLIST";
        const String ctSETDateFormatOptions         = "_DATEFORMATOPTIONS";          
        const String ctSETSystemConfig              = "_SYSTEMCONFIG";
        const String ctSETRegionalConfig            = "_REGIONALCONFIG";
        const String ctSETWallPaper                 = "_WALLPAPER";
        const String ctSETApplicationTitle          = "_APPLICATIONTITLE";
        const String ctSETCustomConfig              = "_CUSTOMCONFIG";

        const String ctKEYEdition                   = "edition";
        const String ctKEYServiceName               = "servicename";
        const String ctKEYCurrencyCode              = "currencycode";
        const String ctKEYCurrencyDecimal           = "currencydecimal";
        const String ctKEYCurrencySymbol            = "currencysymbol";
        const String ctKEYDateFormat                = "dateformat";
        const String ctKEYLocalTimeZoneID           = "localtimezoneid";
        const String ctKEYLocalTimeOffset           = "localtimeoffset";
        const String ctKEYStaticDisplayDateFormat   = "staticdisplaydateformat";
        const String ctKEYThousandSeparator         = "thousandseparator";
        const String ctKEYLocalNumberMode           = "localnumbermode";
        const String ctKEYLanguage                  = "language";
        const String ctKEYAppGrouping               = "appgrouping";
            
        const String ctWildSetting                  = "*";
        const char   ctSeparator                    = ',';

        public enum EditionType { Light, Classic, Cash_Register, Classic_Plus, Unknown }
        
        DataTable                       clSettingList;
        String                          clEServiceID;
        String                          clSubscriptionID;
                                
        public DataTable ActiveList { get { return (clSettingList); } }

        public int          CurrencyDecimalPlace                { get; set; }
        public String       CurrencyFormatString                { get; set; }
        public String       BareCurrencyFormatString            { get; set; } 
        public String       DateFormatString                    { get; set; }
        public String       StaticDisplayDateFormat             { get; set; }

        public String       CurrencyCode                        { get; set; }
        public String       CurrencySymbol                      { get; set; }
        public bool         LocalNumberMode                     { get; set; }
        public bool         ThousandSeparator                   { get; set; }        
        public String       LocalTimeZoneID                     { get; set; }
        public Decimal      LocalTimeOffset                     { get; set; }
        public String       Language                            { get; set; }
        public String       EditionName                         { get; set; }
        public EditionType  Edition                             { get; set; }        
        public String       AppGrouping                         { get; set; }
        public String       FormProtocolListStr                 { get; set; } 
        public String       ServiceName                         { get; set; }
        public String       SystemConfigStr                     { get; set; }
        public String       RegionalConfigStr                   { get; set; }
        public String       CustomConfigStr                     { get; set; }
        public String       ApplicationTitleStr                 { get; set; }
        public String       WallPaperStr                        { get; set; }        
        public Dictionary<String, String> SystemConfig          { get; set; }        
        public Dictionary<String, String> RegionalConfig        { get; set; }
        public Dictionary<String, String> CustomConfig          { get; set; }
        public Dictionary<String, String> ApplicationTitle      { get; set; }
        public Dictionary<String, String> WallPaper             { get; set; }                
                        
        private DataTable RetrieveSettingList()
        {
            QueryClass lcQuery;

            lcQuery = new QueryClass(QueryClass.QueryType.RetrieveSettingList);
            lcQuery.ReplacePlaceHolder("$ESERVICEID", clEServiceID, true);
            lcQuery.ReplacePlaceHolder("$SUBSCRIPTIONID", clSubscriptionID, true);

            return (lcQuery.RunQuery());
        }

        public SettingManager()
        {
            if (ApplicationFrame.GetInstance().ActiveSubscription.Status == SubscriptionManager.SubscriptionStatus.Valid)
            {
                clEServiceID = ApplicationFrame.GetInstance().ActiveSubscription.ActiveRow.EServiceID;
                clSubscriptionID = ApplicationFrame.GetInstance().ActiveSubscription.ActiveRow.SubscriptionID;
                clSettingList = RetrieveSettingList();
                LoadGlobalSettings();
            }
        }

        private EditionType GetEdition()
        {
            String          lcEdition;
            EditionType     lcEditionType;

            lcEdition = EditionName.Trim().Replace(" ", "_");

            if (Enum.TryParse<EditionType>(lcEdition, true, out lcEditionType))
                return (lcEditionType);
            else return (EditionType.Unknown);
        }

        private void LoadGlobalSettings()
        {
            FormProtocolListStr     = GetSettingValue(ctSETFormProtocolList, "{}");
            SystemConfigStr         = GetSettingValue(ctSETSystemConfig);
            RegionalConfigStr       = GetSettingValue(ctSETRegionalConfig);
            ApplicationTitleStr     = GetSettingValue(ctSETApplicationTitle);
            WallPaperStr            = GetSettingValue(ctSETWallPaper);
            CustomConfigStr         = GetSettingValue(ctSETCustomConfig);

            SystemConfig            = General.JSONDeserialize<Dictionary<String, String>>(SystemConfigStr);
            RegionalConfig          = General.JSONDeserialize<Dictionary<String, String>>(RegionalConfigStr);
            ApplicationTitle        = General.JSONDeserialize<Dictionary<String, String>>(ApplicationTitleStr);
            WallPaper               = General.JSONDeserialize<Dictionary<String, String>>(WallPaperStr);
            CustomConfig            = General.JSONDeserialize<Dictionary<String, String>>(CustomConfigStr);

            ThousandSeparator               = General.ParseBoolean(RegionalConfig.GetData(ctKEYThousandSeparator), false);
            CurrencyDecimalPlace            = General.ParseInt(RegionalConfig.GetData(ctKEYCurrencyDecimal), 0);
            CurrencyFormatString            = (ThousandSeparator ? "N" : "F") + CurrencyDecimalPlace.ToString();
            BareCurrencyFormatString        = "F" + CurrencyDecimalPlace.ToString();            
            DateFormatString                = RegionalConfig.GetData(ctKEYDateFormat, "dd/MM/yyyy").Replace("m","M");
            StaticDisplayDateFormat         = RegionalConfig.GetData(ctKEYStaticDisplayDateFormat, "d-M-yyyy").Replace("m", "M");

            CurrencyCode                    = RegionalConfig.GetData(ctKEYCurrencyCode, String.Empty);
            CurrencySymbol                  = RegionalConfig.GetData(ctKEYCurrencySymbol,String.Empty);
            LocalNumberMode                 = General.ParseBoolean(RegionalConfig.GetData(ctKEYLocalNumberMode),true);
            LocalTimeZoneID                 = RegionalConfig.GetData(ctKEYLocalTimeZoneID, String.Empty);
            LocalTimeOffset                 = General.ParseDecimal(RegionalConfig.GetData(ctKEYLocalTimeOffset), 0);

            Language                        = RegionalConfig.GetData(ctKEYLanguage, String.Empty);
                        
            EditionName                     = SystemConfig.GetData(ctKEYEdition,String.Empty);
            ServiceName                     = SystemConfig.GetData(ctKEYServiceName, String.Empty);

            AppGrouping                     = CustomConfig.GetData(ctKEYAppGrouping, String.Empty);

            Edition                         = GetEdition();

            CreateLocalTimeZone();
        }

        private void CreateLocalTimeZone()
        {
            int             lcHourPart;
            int             lcMinutePart;
            TimeZoneInfo    lcTimeZoneInfo;

            lcHourPart      = Convert.ToInt32(Math.Floor(LocalTimeOffset));
            lcMinutePart    = Convert.ToInt32((LocalTimeOffset - Math.Floor(LocalTimeOffset)) * 60);

            lcTimeZoneInfo = TimeZoneInfo.CreateCustomTimeZone("__System__LocalTimeZone", new TimeSpan(lcHourPart, lcMinutePart, 0), "__System__LocalTimeZone", "__System__LocalTimeZone");

            General.SetTimeZoneInfo(lcTimeZoneInfo);
        }

        private String FetchSettingValue(String paSettingGroup, String paSettingKey)
        {
            DataRow[] lcDataRow;

            if ((lcDataRow = clSettingList.AsEnumerable().Where(r => (r.Field<String>(ctCOLSettingGroup).ToLower() == paSettingGroup.ToLower()) && (r.Field<String>(ctCOLSettingKey).ToLower() == paSettingKey.ToLower())).ToArray()).Length > 0)
                    return (Convert.ToString(lcDataRow[0][ctCOLSettingValue]));
            else return (null);            
        }

        public String GetSettingValue(String paSettingKey, String paDefaultValue = "")
        {            
            String      lcSettingValue;

            if (clSettingList != null)
            {
                if (!String.IsNullOrWhiteSpace(paSettingKey))
                {
                    if ((lcSettingValue = FetchSettingValue(clSubscriptionID, paSettingKey)) == null)
                        if ((lcSettingValue = FetchSettingValue(clEServiceID, paSettingKey)) == null)
                            if ((lcSettingValue = FetchSettingValue(clEServiceID.Substring(0, 5), paSettingKey)) == null)
                                if ((lcSettingValue = FetchSettingValue(clEServiceID.Substring(0, 4), paSettingKey)) == null)
                                    if ((lcSettingValue = FetchSettingValue(ctWildSetting, paSettingKey)) == null)
                                        lcSettingValue = paDefaultValue;

                    return (lcSettingValue);
                }
                else return (paDefaultValue);
            }
            else return (paDefaultValue);
        }

        public String[] GetSystemConfigDataArray(String paKey, String paDefaultValue)
        {
            String  lcData;

            if (!String.IsNullOrEmpty(lcData = SystemConfig.GetData(paKey, paDefaultValue)))
            {
                return (lcData.Split(ctSeparator));
            }

            return (null);
        }

        public Dictionary<String,String> GetDateFormatOptionDictionary()
        {
            String      lcDateFormatOptionStr;
            String[]    lcDateFormatOptionArray;

            if (!String.IsNullOrEmpty(lcDateFormatOptionStr = GetSettingValue(ctSETDateFormatOptions)))
            {
                lcDateFormatOptionArray = lcDateFormatOptionStr.Split(ctSeparator);
                return(lcDateFormatOptionArray.ToDictionary(key => key, value => value));
            }
            else return(new Dictionary<string,string>());
        }
    }

    public class ToolBarManager
    {
        public enum ToolItemAttribute { PopUp, Strict };

        DataTable   clToolBarList;

        public DataTable ActiveList { get { return (clToolBarList); } }

        public static ToolBarManager CreateInstance(String paToolBarName)
        {                
            if (!String.IsNullOrEmpty(paToolBarName)) return (new ToolBarManager(paToolBarName));
            else return (null);
        }

        private  DataTable RetrieveToolBarList(String paToolBarName)
        {            
            QueryClass lcQuery;

            lcQuery = new QueryClass(QueryClass.QueryType.RetrieveToolBarList);
            lcQuery.ReplacePlaceHolder("$TOOLBARNAME",  paToolBarName, true);

            return(lcQuery.RunQuery());
        }

        public ToolBarManager(String paToolBarName)
        {
            paToolBarName = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting.GetSettingValue(paToolBarName, paToolBarName);
            clToolBarList = RetrieveToolBarList(paToolBarName);
        }

        public ToolBarRow GetToolBarRow(int paIndex)
        {
            if ((clToolBarList != null) && (paIndex < clToolBarList.Rows.Count))
                return (new ToolBarRow(clToolBarList.Rows[paIndex]));
            else return (null);
        }

        public bool IsAttributeSet(ToolBarRow paToolBarRow, ToolItemAttribute paAttribute)
        {
            return (paToolBarRow.ItemAttribute.Contains("#" + paAttribute.ToString().ToUpper()));
        } 
    }

    public class KeyPadManager
    {
        const String ctCOLPanelName             = "PanelName";
        const String ctCOLEdition               = "Edition";
        const String ctDEFLanguage              = "MYANMAR";
                        
        DataTable   clKeyList;
        String      clLanguage;
        String      clEdition;
        
        public static KeyPadManager CreateInstance(String paKeyPadName)
        {
            if (!String.IsNullOrEmpty(paKeyPadName)) return (new KeyPadManager(paKeyPadName));
            else return (null);
        }

        private DataTable RetrieveKeyPadList(String paKeyPadName)
        {
            QueryClass  lcQuery;            
            
            lcQuery = new QueryClass(QueryClass.QueryType.RetrieveKeyPadList);
            lcQuery.ReplacePlaceHolder("$LANGUAGE", clLanguage, true);
            lcQuery.ReplacePlaceHolder("$EDITION", clEdition, true);
            lcQuery.ReplacePlaceHolder("$KEYPADNAME", paKeyPadName, true);

            return (lcQuery.RunQuery());
        }

        public KeyPadManager(String paKeyPadName)
        {            
            clLanguage = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage.ActiveRow.Language;
            clEdition  = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting.EditionName;
            clKeyList = RetrieveKeyPadList(paKeyPadName);
        }

        public DataRow[] GetPanelKeyList(String paPanelName)
        {
            if ((clKeyList != null) && (!String.IsNullOrWhiteSpace(paPanelName)))            
                return (clKeyList.AsEnumerable().Where(x => x.Field<String>(ctCOLPanelName).ToLower() == paPanelName.ToLower()).ToArray());            
            else return (null);            
        }
    }    

    public class ServiceRequestLogManager
    {
        const String ctUnIdentifiedSubscription = "UNIDENTIFIED SUBSCRIPTION";

        int     clServiceRequestLogID;
        String  clSubscriptionID;

        public ServiceRequestLogManager(String paSubScriptionID)
        {
            try
            {
                clSubscriptionID = paSubScriptionID;

                clServiceRequestLogID = InsertServiceRequestLogRecord(CreateServiceRequestRow());
            }
            catch { }
        }        

        private ServiceRequestLogRow CreateServiceRequestRow()
        {
            ServiceRequestLogRow lcServiceRequestRow;

            lcServiceRequestRow = new ServiceRequestLogRow(TableManager.GetInstance().GetNewRow(TableManager.TableType.ServiceRequestLog, true));

            if (ApplicationFrame.GetInstance().Status == ApplicationFrame.InitializationStatus.Success)
                lcServiceRequestRow.SubscriptionID = ApplicationFrame.GetInstance().ActiveSubscription.ActiveRow.SubscriptionID;
            else
                lcServiceRequestRow.SubscriptionID = ctUnIdentifiedSubscription;

            lcServiceRequestRow.RequestTime = DateTime.Now;
            lcServiceRequestRow.RequestIP = ApplicationFrame.GetRemoteIPAddress();
            lcServiceRequestRow.RequestUrl = HttpContext.Current.Request.Url.ToString();

            return (lcServiceRequestRow);
        }


        private int InsertServiceRequestLogRecord(ServiceRequestLogRow paServiceRequestLogRow)
        {
            object      lcResult;
            int         lcOrderNo;
            QueryClass  lcQuery;

            lcQuery = new QueryClass(QueryClass.QueryType.InsertServiceRequestLog);
            lcQuery.ReplaceRowPlaceHolder(paServiceRequestLogRow.Row);

            if (((lcResult = lcQuery.GetResult()) != null) && ((lcOrderNo = Convert.ToInt32(lcResult)) > 0)) return (lcOrderNo);
            else return (-1);
        }

        public void UpdateServiceRequestLogRecord(DateTime paCompleteTime)
        {
            QueryClass lcQueryClass;
            try
            {
                lcQueryClass = new QueryClass(QueryClass.QueryType.UpdateServiceRequestLog);
                lcQueryClass.ReplacePlaceHolder("$ID", clServiceRequestLogID.ToString(), false);
                lcQueryClass.ReplacePlaceHolder("$COMPLETETIME", paCompleteTime.ToString("yyyy-MM-dd HH:mm:ss"), true);

                lcQueryClass.ExecuteNonQuery();
            }
            catch { }

        }
    }    
}
