using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using System.IO;
using System.IO.Ports;
using System.Configuration;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Security.Cryptography;
using System.Web;
using System.Linq;
using System.Drawing;
using System.Globalization;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CobraFoundation
{

    //*********************
    //* ConfigClass Class *
    //*********************
    public class ConfigClass
    {        
        // Primary : User:gizmo-success19kpc, pwd:yX9C&!vw^<AQBbrF        
        // Remote : User:giz20x18s-mict19bz, Pwd:6uKE-6#~4LERynmn
        // <!HostName::localhost;;UserName::gizmo18;;Password::ZQseKQfQaJC7cqA2;;DatabaseName::vdata_pos!>
        //  const String ctLiveConnectionString     = @"Data Source=$HOSTNAME;Initial Catalog=$INITIALCATALOG;User ID=cba1802ps-gxxxxxa2;Password=hwM*(VqX[@J5Fns~;";
        const String ctBase64LiveConnectionString   = @"RGF0YSBTb3VyY2U9JEhPU1ROQU1FO0luaXRpYWwgQ2F0YWxvZz0kSU5JVElBTENBVEFMT0c7VXNlciBJRD1jYmExODAycHMtZ3h4eHh4YTI7UGFzc3dvcmQ9aHdNKihWcVhbQEo1Rm5zfjs=";
        const String ctLocalConnectionString        = @"Data Source=$HOSTNAME;Initial Catalog=$INITIALCATALOG;User ID=$USERNAME;Password=$PASSWORD;"; 
        // const String ctRemoteConnectionString    = @"Data Source=$HOSTNAME;Initial Catalog=$DATABASENAME;User ID=$USERNAME;Password=$PASSWORD;"; 
        const String ctBase64RemoteConnectionString = @"RGF0YSBTb3VyY2U9JEhPU1ROQU1FO0luaXRpYWwgQ2F0YWxvZz0kREFUQUJBU0VOQU1FO1VzZXIgSUQ9JFVTRVJOQU1FO1Bhc3N3b3JkPSRQQVNTV09SRDs=";

        const String ctVARConfigClassInstance       = "__ConfigClassInstance";

        const String ctConfigFileName = "Web.Config";
   //     private static ConfigClass clConfigClass;

        int     clCommandTimeOut;
        String  clDefaultConnectionStr;
        String  clEServiceRemoteConnectionStr;
        String  clExceptionLogFile;        

        String  clStandardXUnit;
        String  clStandardYUnit;

        bool    clLocalMode;
        String  clConnectionTemplate;        
        String  clRemoteConnectionTemplate;
        
        public String DefaultConnectionStr { get { return (clDefaultConnectionStr); } }
        // AUTO RESET FOR CONTEXT USING ResetConfiguration;
        public String EServiceRemoteConnectionStr { get { return (clEServiceRemoteConnectionStr == null ? DefaultConnectionStr : clEServiceRemoteConnectionStr); } }        
        public int CommandTimeOut { get { return (clCommandTimeOut); } }
        public String ExceptionLogFile { get { return (clExceptionLogFile); } }

        public String StandardXUnit { get { return (clStandardXUnit); } }
        public String StandardYUnit { get { return (clStandardYUnit); } }

        static public ConfigClass GetInstance()
        {
            if (HttpContext.Current.Items[ctVARConfigClassInstance] == null)
                HttpContext.Current.Items.Add(ctVARConfigClassInstance, new ConfigClass());

            return ((ConfigClass)HttpContext.Current.Items[ctVARConfigClassInstance]);            
        }

        private ConfigClass()
        {
            RetrieveConfigurationSettings();
        }

        // To Reset Context Specific Information
        public void ResetConfiguration()
        {
            ResetEServiceRemoteConnection();
        }

        private void RetrieveConfigurationSettings()
        {
            clLocalMode                 = HttpContext.Current.Request.IsLocal;
            clConnectionTemplate        = clLocalMode ? ctLocalConnectionString : General.Base64Decode(ctBase64LiveConnectionString);
            clRemoteConnectionTemplate  = General.Base64Decode(ctBase64RemoteConnectionString);

            clDefaultConnectionStr = CompileConnectionString();
            clEServiceRemoteConnectionStr = String.Empty;
            clCommandTimeOut = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeOut"]);
            clExceptionLogFile = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ExceptionLogFile"]).Replace("$MMYY", DateTime.Now.ToString("MMyy"));
        }

        private void ResetEServiceRemoteConnection()
        {
            clEServiceRemoteConnectionStr = String.Empty;
        }

        public void SetEServiceRemoteConnection(String paHostName, String paDatabaseName, String paUserName, String paPassword)
        {
            String lcConnectionString;

            lcConnectionString = clRemoteConnectionTemplate;
            
            lcConnectionString = lcConnectionString.Replace("$HOSTNAME", paHostName);
            lcConnectionString = lcConnectionString.Replace("$DATABASENAME", paDatabaseName);            
            lcConnectionString = lcConnectionString.Replace("$USERNAME", paUserName);            
            lcConnectionString = lcConnectionString.Replace("$PASSWORD", paPassword);

            clEServiceRemoteConnectionStr = lcConnectionString;
        }

        private String CompileConnectionString()
        {
            String lcConnectionString;
            String lcHostName;
            String lcInitialCatalogue;
            String lcUserName;
            String lcPassword;

            lcHostName          = ConfigurationManager.AppSettings["HostName"];
            lcInitialCatalogue  = ConfigurationManager.AppSettings["InitialCatalog"];
            
            lcConnectionString = clConnectionTemplate;

            if (lcInitialCatalogue != null)
                lcConnectionString = lcConnectionString.Replace("$INITIALCATALOG", lcInitialCatalogue);

            if (lcHostName != null)
                lcConnectionString = lcConnectionString.Replace("$HOSTNAME", lcHostName);

            if (clLocalMode)
            {
                lcUserName = ConfigurationManager.AppSettings["UserName"];
                lcPassword = ConfigurationManager.AppSettings["Password"];

                if (lcUserName != null)
                    lcConnectionString = lcConnectionString.Replace("$USERNAME", lcUserName);

                if (lcPassword != null)
                    lcConnectionString = lcConnectionString.Replace("$PASSWORD", lcPassword);
            }

            return (lcConnectionString);
        }

        public void SetStandardUnit(bool paMobileMode)
        {
            clStandardXUnit = paMobileMode ? "vw" : "px";
            clStandardYUnit = paMobileMode ? "vh" : "px";
        }

    } // ConfigClass


    public static class General
    {
        const String ctREGSmartBase64   = @"BASE64:(?<Base64>[^~]+)[~]{2}";
        const String ctCRLF             = "\r\n";
        const String ctComma            = ",";
        const String ctSingleQuote      = "'";
        const String ctVARLocalTimeZone = "__LocalTimeZone";

        public static object                clFileLock = null;
                
        public static String ContrastColor(String paColor)
        {
            Color   lcColor;
            Double  lcColorValue;

            try
            {
                if (!String.IsNullOrEmpty(paColor))
                {
                    lcColor = ColorTranslator.FromHtml(paColor);
                    lcColorValue = (lcColor.R * 0.299) + (lcColor.G * 0.587) + (lcColor.B * 0.114);

                    if (lcColorValue > 186) paColor = "#000000";
                    else paColor = "#ffffff";
                }
            }
            catch { }

            return (paColor);            
        }

        public static bool TryChangeType(object paValue, Type paType, out object paOutputValue)
        {
            paOutputValue = null;

            try 
            {
                if (paType.IsEnum) paOutputValue = Enum.Parse(paType,Convert.ToString(paValue));
                else paOutputValue = Convert.ChangeType(paValue, paType);
                return (true);
            }
            catch { }
            return (false);
        }

        public static T ParseEnum<T>(String paString, T paDefaultValue) where T : struct
        {
            T lcOutput;

            if (Enum.TryParse<T>(paString, true, out lcOutput)) return (lcOutput);
            else return (paDefaultValue);
        }

        public static int ParseInt(String paIntegerStr, int paDefaultValue)
        {
            int lcInteger;

            if (int.TryParse(paIntegerStr, out lcInteger)) return (lcInteger);
            else return (paDefaultValue);
        }

        public static Decimal ParseDecimal(String paDecimalStr, Decimal paDefaultValue)
        {
            Decimal lcDecimal;

            if (Decimal.TryParse(paDecimalStr, out lcDecimal)) return (lcDecimal);
            else return (paDefaultValue);
        }

        public static bool ParseBoolean(String paBooleanStr, bool paDefaultValue)
        {
            bool lcBoolean;

            if (Boolean.TryParse(paBooleanStr, out lcBoolean)) return (lcBoolean);
            else return (paDefaultValue);
        }

        public static Single ParseSingle(String paSingle, Single paDefaultValue)
        {
            Single lcSingle;

            if (Single.TryParse(paSingle, out lcSingle)) return (lcSingle);
            else return (paDefaultValue);
        }

        public static Double ParseDouble(String paDouble, Double paDefaultValue)
        {
            Double lcDouble;

            if (Double.TryParse(paDouble, out lcDouble)) return (lcDouble);
            else return (paDefaultValue);
        }

        public static DateTime ParseDate(String paDateStr, String paFormatString, DateTime paDefaultValue)
        {
            DateTime lcDate;

            if (DateTime.TryParseExact(paDateStr, paFormatString, CultureInfo.InvariantCulture, DateTimeStyles.None, out lcDate)) return (lcDate);
            else return (paDefaultValue);
        }

        public static String GetJValueStr(JValue paJValue, String paDefaultValue = "")
        {            
            if ((paJValue != null) && (paJValue.Value != null))
            {
                return (paJValue.Value.ToString());
            }

            return(paDefaultValue);
        }

        public static string Base64Encode(string paTextString)
        {
            byte[] lcBytes;

            if (paTextString != null)
            {
                lcBytes = System.Text.Encoding.UTF8.GetBytes(paTextString);

                return (System.Convert.ToBase64String(lcBytes));
            }
            else return (null);
        }

        public static string SmartBase64Encode(string paTextString)
        {
            byte[] lcBytes;

            lcBytes = System.Text.Encoding.UTF8.GetBytes(paTextString);

            return ("BASE64:" + System.Convert.ToBase64String(lcBytes) + "~~");
        }


        public static string Base64Decode(string paBase64EncodedData, bool paUrlDecode = false)
        {
            byte[] lcBytes;

            try
            {
                if (!String.IsNullOrWhiteSpace(paBase64EncodedData))
                {
                    if (paUrlDecode) paBase64EncodedData = HttpUtility.UrlDecode(paBase64EncodedData);
                    lcBytes = System.Convert.FromBase64String(paBase64EncodedData);
                    return (System.Text.Encoding.UTF8.GetString(lcBytes));
                }
                return (String.Empty);
            }
            catch
            {
                return (String.Empty);
            }
        }

        public static string SmartBase64Decode(string paTextString, bool paUrlDecode = false)
        {
            byte[] lcBytes;
            Match lcMatch;
            String lcTempStr;

            if ((lcMatch = Regex.Match(paTextString, ctREGSmartBase64)).Success)
            {
                lcTempStr = lcMatch.Groups["Base64"].Value;
                if (paUrlDecode) paTextString = HttpUtility.UrlDecode(lcTempStr);
                lcBytes = System.Convert.FromBase64String(lcTempStr);
                lcTempStr = System.Text.Encoding.UTF8.GetString(lcBytes);
                return (paTextString.Replace(lcMatch.Value, lcTempStr));
            }
            else return (paTextString);
        }

        public static String JSONSerialize(object paObject, String paDefault = null)
        {
            try
            {
                if (paObject != null)
                {
                    return(JsonConvert.SerializeObject(paObject));                    
                }
            }
            catch
            {
                /* Do Nothing */
            }

            return (paDefault);
        }

        public static T JSONDeserialize<T>(String paJOSNString) where T : new()
        {            
            try
            {
                if (!String.IsNullOrEmpty(paJOSNString))
                {                    
                    return (JsonConvert.DeserializeObject<T>(paJOSNString));                    
                }
            }
            catch
            {
                /* Do Nothing */
            }

            return (new T());
        }

        public static Dictionary<String, object> GetDataRowDictionary(DataRow paDataRow)
        {
            Dictionary<String, object> lcRowDictionary;

            if (paDataRow != null)
            {
                lcRowDictionary = new Dictionary<String, object>(StringComparer.InvariantCultureIgnoreCase);
                foreach (DataColumn lcColumn in paDataRow.Table.Columns)
                    lcRowDictionary.Add(lcColumn.ColumnName, paDataRow[lcColumn]);

                return (lcRowDictionary);
            }
            else return (null);

        }

        public static Dictionary<String, String>  GetFormattedDataRowDictionary(DataRow paDataRow)
        {
            String                          lcValue;
            Dictionary<String, String>      lcDictionary;

            if (paDataRow != null)
            {
                lcDictionary = new Dictionary<String, String>();

                foreach (DataColumn lcColumn in paDataRow.Table.Columns)
                {                    
                    if (paDataRow[lcColumn.ColumnName].GetType() != typeof(DBNull))
                        lcValue = General.GetFormattedDBData(paDataRow[lcColumn.ColumnName]);
                    else lcValue = String.Empty;                 
                    
                    lcDictionary.Add(lcColumn.ColumnName.ToLower(), lcValue);
                }

                return (lcDictionary);
            }
            return (null);
        }

        static public List<String> GetDistinctColumnValue(DataTable paDataTable, String paColumnName)
        {
            if ((!String.IsNullOrEmpty(paColumnName)) && (paDataTable != null) && (paDataTable.Rows.Count > 0) && (paDataTable.Columns.Contains(paColumnName)))
                return(paDataTable.AsEnumerable().Select(x=>x.Field<String>(paColumnName)).Distinct().ToList());
            else 
                return(new List<String>());
        }

        static public List<String> GetDistinctColumnValue(DataTable paDataTable, int paColumnIndex)
        {
            if ((paColumnIndex != -1) && (paDataTable != null) && (paDataTable.Rows.Count > 0) && (paDataTable.Columns.Count > paColumnIndex))
                return (paDataTable.AsEnumerable().Select(x => x.Field<String>(paDataTable.Columns[paColumnIndex].ColumnName)).Distinct().ToList());
            else
                return (new List<String>());
        }

        static public T GetTableData<T>(DataRow paDataRow, String paColumnName, T paDefault)
        {
            if ((paDataRow != null) && (!String.IsNullOrWhiteSpace(paColumnName)) &&
                (paDataRow.Table.Columns.Contains(paColumnName)) && (paDataRow[paColumnName].GetType() != typeof(System.DBNull)))
                return ((T)Convert.ChangeType(paDataRow[paColumnName], typeof(T)));
            else return (paDefault);
        }

        static public string GetMd5Hash(String paInputString)
        {
            MD5 lcMD5Hash;
            byte[] lcData;
            StringBuilder lcStringBuilder;

            lcMD5Hash = MD5.Create();
            lcStringBuilder = new StringBuilder();

            lcData = lcMD5Hash.ComputeHash(Encoding.UTF8.GetBytes(paInputString));

            for (int lcCount = 0; lcCount < lcData.Length; lcCount++)
                lcStringBuilder.Append(lcData[lcCount].ToString("X2"));

            return (lcStringBuilder.ToString());
        }

        public static void WriteExceptionLog(Exception paException, String paAdditionalInfo)
        {
            String lcExceptionInfo;

            if (clFileLock == null) clFileLock = new object();

            lock (clFileLock)
            {
                lcExceptionInfo = RetrieveExceptionInfo(paException, paAdditionalInfo);
                try { File.AppendAllText(ConfigClass.GetInstance().ExceptionLogFile, lcExceptionInfo); }
                catch { }
            }
        }

        public static String RetrieveExceptionInfo(Exception paException, String paAdditionalInfo)
        {
            String lcExceptionInfo;
            
            lcExceptionInfo = "Exception Message : " + paException.Message + ctCRLF;
            lcExceptionInfo += "Event Time        : " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + ctCRLF;
            lcExceptionInfo += "Module            : " + paException.TargetSite.Module + ctCRLF;
            lcExceptionInfo += "Class             : " + paException.TargetSite.DeclaringType.FullName + ctCRLF;
            lcExceptionInfo += "Target Function   : " + paException.TargetSite.ToString() + ctCRLF;
            lcExceptionInfo += "Constructor       : " + paException.TargetSite.IsConstructor.ToString() + ctCRLF;
            lcExceptionInfo += "Static            : " + paException.TargetSite.IsStatic.ToString() + ctCRLF;

            if (paAdditionalInfo != null)
                lcExceptionInfo += "ADDITIONAL INFO   : " + ctCRLF + paAdditionalInfo + ctCRLF;

            lcExceptionInfo += "STACK TRACE       : " + ctCRLF + paException.StackTrace + ctCRLF;
            lcExceptionInfo += "".PadLeft(20, '-') + ctCRLF;

            if (paException.InnerException != null)
            {
                lcExceptionInfo += ctCRLF + "INNER EXCEPTION : " + ctCRLF;
                lcExceptionInfo += RetrieveExceptionInfo(paException.InnerException, "");
            }
            else lcExceptionInfo += "".PadLeft(40, '=') + ctCRLF + ctCRLF;

            return (lcExceptionInfo);
        }
        
        public static void SetTimeZoneInfo(TimeZoneInfo paTimeZoneInfo)
        {
            HttpContext.Current.Items[ctVARLocalTimeZone] = paTimeZoneInfo;
        }

        public static DateTime GetCurrentSystemLocalTime()
        {
            return (ConvertUTCToSystemLocalTime(DateTime.UtcNow));
        }

        public static DateTime ConvertUTCToSystemLocalTime(DateTime paDateTime)
        {
            TimeZoneInfo lcTimeZoneInfo;

            if ((lcTimeZoneInfo = HttpContext.Current.Items[ctVARLocalTimeZone] as TimeZoneInfo) != null)
                return (TimeZoneInfo.ConvertTimeFromUtc(paDateTime, lcTimeZoneInfo));
            else return (paDateTime);
        }

        public static String GetFormattedDBData(object paValue, String paFormatString = "")
        {
            try
            {
                if (paValue != null)
                {
                    switch (paValue.GetType().ToString())
                    {
                        case "System.DateTime":
                            {
                                paFormatString = String.IsNullOrEmpty(paFormatString) ? "dd/MM/yyyy" : paFormatString;
                                return (Convert.ToDateTime(paValue).ToString(paFormatString));                                    
                            }

                        case "System.Decimal":
                            {
                                paFormatString = String.IsNullOrEmpty(paFormatString) ? "F0" : paFormatString;
                                return (((Decimal)paValue).ToString(paFormatString));
                            }

                        case "System.Single":
                            {                                
                                paFormatString = String.IsNullOrEmpty(paFormatString) ? "F0" : paFormatString;
                                return (((Single)paValue).ToString(paFormatString));
                            }

                        case "System.Double":
                            {
                                paFormatString = String.IsNullOrEmpty(paFormatString) ? "F0" : paFormatString;
                                return (((Double)paValue).ToString(paFormatString));
                            }

                        case "System.Boolean": return ((bool)paValue).ToString();                        
                        
                        case "System.DBNull": return (String.Empty);

                        default: return (paValue.ToString());
                    }
                }
            }
            catch { }

            return (String.Empty);
        }

        public static String[] SingleDimensionStringArray(DataRow[] paDataRows, String paColumn)
        {
            String[] lcStrArray;

            if ((paDataRows != null) && (paDataRows.Length > 0))
            {
                lcStrArray = new String[paDataRows.Length];

                for (int lcCount = 0; lcCount < paDataRows.Length; lcCount++)
                    if (paDataRows[lcCount][paColumn].GetType().ToString() != "System.DBNull")
                        lcStrArray[lcCount] = paDataRows[lcCount][paColumn].ToString();
                    else lcStrArray[lcCount] = "";

                return (lcStrArray);
            }
            else return (null);
        }

        public static String[,] MultiDimensionStringArray(DataTable paDataTable)
        {
            String[,] lcStringArray;

            if ((paDataTable != null) && (paDataTable.Rows.Count > 0))
            {
                lcStringArray = new String[paDataTable.Rows.Count, paDataTable.Columns.Count];

                for (int lcRowCounter = 0; lcRowCounter < paDataTable.Rows.Count; lcRowCounter++)
                {
                    for (int lcColCounter = 0; lcColCounter < paDataTable.Columns.Count; lcColCounter++)
                    {
                        lcStringArray[lcRowCounter, lcColCounter] = Convert.ToString(paDataTable.Rows[lcRowCounter][lcColCounter]);
                    }
                }

                return (lcStringArray);
            }
            else return (null);

        }

        public static Type FindType(String paFullyQualifiedName)
        {
            Type lcType;

            foreach (var lcAssembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                lcType = lcAssembly.GetType(paFullyQualifiedName);
                if (lcType != null) return (lcType);
            }

            return (null);
        }

        public static int CountTableRow(DataTable paDataTable, String paColumnName, String paFilter = "")
        {
            object lcResult;

            if ((paDataTable != null) && (!String.IsNullOrWhiteSpace(paColumnName)))
                if ((lcResult = paDataTable.Compute("Count(" + paColumnName + ")", paFilter)).GetType() != typeof(DBNull))
                    return (Convert.ToInt32(lcResult));
            
            return (0);
        }

        public static int SumInteger(DataTable paDataTable, String paColumnName, String paFilter = "")
        {
            object lcResult;

            if ((paDataTable != null) && (!String.IsNullOrWhiteSpace(paColumnName)))
                if ((lcResult = paDataTable.Compute("Sum(" + paColumnName + ")", paFilter)).GetType() != typeof(DBNull))
                    return (Convert.ToInt32(lcResult));
            return (0);
        }

        public static Decimal SumDecimal(DataTable paDataTable, String paColumnName, String paFilter = "")
        {
            object lcResult;

            if ((paDataTable != null) && (!String.IsNullOrWhiteSpace(paColumnName)))
            {
                if ((lcResult = paDataTable.Compute("Sum(" + paColumnName + ")", paFilter)).GetType() != typeof(DBNull))
                    return (Convert.ToDecimal(lcResult));                
            }
            return (0M);
        }

        public static String[] SplitString(String paString, bool paRemoveEmptyEntry = false, String paDelimiter = ",")
        {            
            if (!String.IsNullOrEmpty(paString))
                return(paString.Split(new String[] { paDelimiter }, paRemoveEmptyEntry ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None));
            else
                return(new String[0]);
        }

        public static String CompileSqlConditionListStr(String paString)
        {
            String[]    lcSplittedStr;
            String      lcOutputStr;

            if (!String.IsNullOrEmpty(paString))
            {
                lcOutputStr     = String.Empty;
                lcSplittedStr   = paString.Split(ctComma[0]);

                for (int lcCount = 0; lcCount < lcSplittedStr.Length; lcCount++)
                {
                    lcOutputStr = lcOutputStr + ctSingleQuote + lcSplittedStr[lcCount] + ctSingleQuote + ctComma;
                }

                lcOutputStr = lcOutputStr.Trim(new char[] { ctComma[0] });

                return (lcOutputStr);
            }
            else return (ctSingleQuote + String.Empty + ctSingleQuote);
        }
    }

    //********************
    //* QueryClass Class *
    //********************
    public class QueryClass
    {
        const String ctGetSchema      = "Select " +
                                            "* " +
                                        "From " +
                                            "$TABLENAME " +
                                        "Where " +
                                            "1=0; "; //No Row, Just Schema

        const String ctGetLanguageRow   =  "Select " +
                                                "* " +
                                           "From " +
                                                "Admin_Language " +
                                           "Where " +
                                                "Language = $LANGUAGE; ";

        const String ctRetrieveRoutingTable = "Select " +
                                                  "* " +
                                             "From " +
                                                  "Admin_Routing " +
                                             "Order By " +
                                                  "RouteOrder; ";

        const String ctRetrieveUrlRewriteTable = "Select " +
                                                        "* " +
                                                   "From " +
                                                        "Admin_UrlRewrite; ";

        const String ctGetFormInfoRow = "Select " +
                                              "* " +
                                         "From " +
                                              "Admin_Forminfo " +
                                          "Where " +
                                              "FormName = $FORMNAME; ";

        const String ctRetrieveFieldInfo           = "Select " +
                                                         "* " +
                                                     "From " +
                                                         "Admin_FieldInfo " +
                                                     "Where " +
                                                          "(EServiceID = '*' Or EServiceID = $ESERVICEID) And " +
                                                          "DataGroup = $DATAGROUP " +                                                          
                                                     "Order By " +
                                                          "PageIndex,RowIndex, ColumnIndex; ";

        const String ctRetrieveInputInfo            = "Select " +
                                                         "* " +
                                                      "From " +
                                                         "Admin_InputInfo " +
                                                      "Where " +
                                                          "(EServiceID = '*' Or EServiceID = $ESERVICEID) And " +
                                                          "InputGroup = $INPUTGROUP " +                                                          
                                                      "Order By " +
                                                          "PageIndex,RowIndex, ColumnIndex; ";

        const String ctGetUserRow         = "Select " +
                                                "* " +
                                            "From " +
                                                "Admin_User " +
                                            "Where " +
                                                "(SubscriptionID = '*' Or SubscriptionID = $SUBSCRIPTIONID) And " +
                                                "LoginID = $LOGINID; ";

        //const String ctGetAdminUserRow = "Select " +
        //                                       "* " +
        //                                 "From " +
        //                                       "Admin_User " +
        //                                 "Where " +
        //                                       "SubscriptionID = $SUBSCRIPTIONID And " +
        //                                       "LoginID = $LOGINID And " +
        //                                       "UserRole Like '%ADMIN%';";

        const String ctGetEServiceRow         = "Select " +
                                                    "* " +
                                                "From " +
                                                    "Data_EService " +
                                                "Where " +
                                                    "EServiceID =$ESERVICEID; ";


        const String ctGetSubscriptionRow = "Select " +
                                                "* " +
                                            "From " +
                                                "Data_SubScription " +
                                            "Where " +
                                                "Replace(SubscriptionID,'-','') = $SUBSCRIPTIONID; ";

        const String ctGetSubscriptionRowByFrontEndPath = "Select " +
                                                                "* " +
                                                          "From " +
                                                               "Data_SubScription " +
                                                          "Where " +
                                                                "FrontEndPath = $FRONTENDPATH; ";


        const String ctInsertServiceRequestLog = "Insert Into " +
                                                        "Track_ServiceRequestLog " +
                                                            "(SubscriptionID, RequestTime, RequestIP, RequestUrl) " +
                                                            "Output INSERTED.ID into @IDTable " +
                                                        "Values " +
                                                            "($SUBSCRIPTIONID, $REQUESTTIME, $REQUESTIP, $REQUESTURL); ";

        const String ctUpdateServiceRequestLog = "Update " +
                                                    "Track_ServiceRequestLog " +
                                                        "Set " +
                                                            "CompleteTime = $COMPLETETIME " +                                                            
                                                    "Where " +
                                                        "ID = $ID ";

        const String ctGetSessionRow             = "Select " +
                                                         "* " +
                                                    "From " +
                                                         "Admin_Session " +
                                                    "Where " +
                                                         "SubscriptionID = $SUBSCRIPTIONID And " +
                                                         "LoginID = $LOGINID;";

        const String ctGetSessionRowBySessionKey = "Select " +
                                                        "* " +
                                                   "From " +
                                                        "Admin_Session " +
                                                   "Where " +
                                                        "SessionKey = $SESSIONKEY";

        const String ctUpdateSessionAccessInfo    = "Update " +
                                                        "Admin_Session " +
                                                    "Set " +                                                    
                                                         "LastAccessTime = $LASTACCESSTIME, " +
                                                         "AccessCount = AccessCount + 1 " +
                                                    "Where " +
                                                         "SubscriptionID = $SUBSCRIPTIONID And "  +
                                                         "LoginID = $LOGINID;";


        //const String ctInsertSessionRow = "Merge Into " +
        //                                        "Admin_Session " +
        //                                              "(SubscriptionID, SessionID, SessionOwnerIP, LoginID, Password, StartTime, ExpiryTime) " +
        //                                        "Values " +
        //                                              "($SUBSCRIPTIONID, $SESSIONID, $SESSIONOWNERIP, $LOGINID, $PASSWORD, $STARTTIME, $EXPIRYTIME); ";


        const String ctInsertSessionRow = "Merge Into " +
                                               "Admin_Session With (HoldLock) As Target " +
                                               "Using (Select $SUBSCRIPTIONID, $LOGINID, $PASSWORD, $SESSIONOWNERIP, $SESSIONKEY, $STARTTIME, $EXPIRYTIME, $FIRSTACCESSTIME, $LASTACCESSTIME, $ACCESSCOUNT) " +
                                               "As Source (SubscriptionID, LogInID, Password, SessionOwnerIP, SessionKey, StartTime, ExpiryTime, FirstAccessTime, LastAccessTime, AccessCount) " +
                                          "On " +
                                               "(Target.SubscriptionID = Source.SubscriptionID And Target.LoginID = Source.LoginID) " +
                                          "When Matched Then " +
                                              "Update Set " +
                                                  "Password = Source.Password, " +
                                                  "SessionOwnerIP = Source.SessionOwnerIP, " +
                                                  "SessionKey = Source.SessionKey, " +
                                                  "StartTime = Source.StartTime, " +
                                                  "ExpiryTime = Source.ExpiryTime, " +                                                  
                                                  "LastAccessTime = Source.LastAccessTime, " +
                                                  "AccessCount = Target.AccessCount + 1 " +
                                          "When Not Matched Then " +
                                                 "Insert " +
                                                     "(SubscriptionID, LogInID, Password, SessionOwnerIP, SessionKey, StartTime, ExpiryTime, FirstAccessTime, LastAccessTime, AccessCount) " +
                                                 "Values " +
                                                     "($SUBSCRIPTIONID, $LOGINID, $PASSWORD, $SESSIONOWNERIP, $SESSIONKEY, $STARTTIME, $EXPIRYTIME, $FIRSTACCESSTIME, $LASTACCESSTIME, $ACCESSCOUNT); ";
        

        const String ctRetrieveWidgetList = "Select " +
                                                "* " +
                                            "From " +
                                                "Admin_Widget " +
                                            "Where " +
                                                "WidgetName in ($WIDGETNAMELIST); ";

        const String ctRetrieveWidgetSubscriptionList = "Select " +
                                                            "wd.WidgetName, wd. Grouping, wd.Icon, wd.IconLabel, wd.Type, wd.Category, wd.Link, " +
                                                            "wd.DisplayName, wd.BriefDescription, wd.DetailDescription, wd.LicenseMode, wd.RequireRole, " +
                                                            "cs.ServiceKey, cs.Status, cs.ClassificationCode, cs.SortPriority, cs.ActivationDate, cs.ExpiryDate  " +
                                                        "From " +
                                                            "Admin_Widget as wd, Data_ComponentSubscription as cs " +
                                                        "Where " +
                                                            "wd.WidgetName = cs.ComponentName And " +
                                                            "(ServiceKey = Left($ESERVICEID,4) Or ServiceKey = Left($ESERVICEID,5) Or ServiceKey = $ESERVICEID or ServiceKey = $SUBSCRIPTIONID) And " +
                                                            "((LTRIM(RTRIM(RequireRole)) = '') Or (RequireRole = '#ADMIN' and $USERTYPE ='SUPERADMIN')) " +
                                                        "Order By " +
                                                            "ClassificationCode, SortPriority, WidgetName, len(ServiceKey) desc; ";

        //const String ctRetrieveWidgetSubscriptionList = "Select " +
        //                                                    "* " +                                                            
        //                                                "From  " +

        //                                                    "View_WidgetSubscription " +
        //                                                "Where " +
        //                                                    "(ServiceKey = Left($ESERVICEID,4) Or ServiceKey = Left($ESERVICEID,5) Or ServiceKey = $ESERVICEID or ServiceKey = $SUBSCRIPTIONID) And " +
        //                                                    "((LTRIM(RTRIM(RequireRole)) = '') Or (RequireRole = '#ADMIN' and $USERTYPE ='SUPERADMIN')) " +
        //                                                "Order By " +
        //                                                    "Classificationcode asc, SortPriority asc;";

        const String ctGetWidgetRow                   = "Select " +
                                                            "* " +
                                                        "From  " +
                                                            "Admin_Widget " +
                                                        "Where " +
                                                            "WidgetName = $WIDGETNAME; ";

        const String ctGetPaymentRow                 = "Select " +
                                                            "* " +
                                                        "From  " +
                                                            "Data_Payment " +
                                                        "Where " +
                                                            "TransactionID = $TRANSACTIONID; ";

        const string ctRetrieveDynamicQueryTable   = "Select " +
                                                         "* " +
                                                      "From " +
                                                          "Data_DynamicQuery; ";                                                      

        const String ctRetrieveToolBarList = "Select " +
                                                  "* " +
                                             "From " +
                                                  "Admin_ToolBar " +
                                             "Where " +
                                                   "ToolBarName = $TOOLBARNAME  " +
                                             "Order By " +
                                                   "ItemOrder; ";

        const String ctRetrieveKeyPadList  = "Select " +
                                                  "* " +
                                             "From " +
                                                  "Admin_KeyPad " +
                                             "Where " +
                                                   "KeyPadName = $KEYPADNAME And " +
                                                   "Language = $LANGUAGE And " +
                                                   "(Edition = '*' Or Edition = $EDITION) " +                                                   
                                             "Order By " +
                                                   "PanelName, KeyOrder; ";


        const String ctDeclareIdentityTable  = "Declare @IDTable table (OrderNo int); ";

        const String ctReturnIdentity       = "Select OrderNo From @IDTable ;";

        const String ctInsertOrderInfoRecord = "Insert Into " +
                                                        "EData_OrderInfo " +
                                                            "(OrderDate, OrderStatus, ShippingCharge, SubscriptionID, LogInID, " +
                                                            "Name, ContactNo, BuildingNo, Floor, RoomNo, Street, Quarter, " +
                                                            "AddressInfo, Township, City, Postal, Country, OrderRemark) " +
                                                        "Output INSERTED.OrderNo into @IDTable " +
                                                        "Values " +
                                                            "($ORDERDATE, $ORDERSTATUS, $SHIPPINGCHARGE, $SUBSCRIPTIONID, $LOGINID, " +
                                                            " $NAME, $CONTACTNO, $BUILDINGNO, $FLOOR, $ROOMNO, $STREET, $QUARTER, " +
                                                            " $ADDRESSINFO, $TOWNSHIP, $CITY, $POSTAL, $COUNTRY, $ORDERREMARK); ";

        const String ctInsertOrderDetailRecord    = "Insert Into " +
                                                        "EData_OrderDetail " +
                                                            "(OrderNo, Status, ItemID, ItemName, Quantity, UnitPrice, Remark) " +
                                                       "Values " +
                                                            "($ORDERNO, $STATUS, $ITEMID, $ITEMNAME, $QUANTITY, $UNITPRICE, $REMARK);";

        const String ctUpdateOrderDetailRecord = "Update OrderDetail " +
                                                        "Set " +
                                                            "OrderDetail.Status = Iif($STATUS = 0, OrderDetail.Status, $STATUS), OrderDetail.Remark = $REMARK, OrderDetail.Quantity = $QUANTITY " +
                                                         "From " +
                                                            "EData_OrderDetail as OrderDetail " +
                                                         "INNER JOIN " +
                                                            "EData_OrderInfo as OrderInfo " +
                                                         "On " +
                                                            "OrderInfo.OrderNo = OrderDetail.OrderNo " +
                                                         "Where " +
                                                            "OrderInfo.SubscriptionID   = $SUBSCRIPTIONID And " +
                                                            "OrderInfo.LogInID          = $LOGINID And " +
                                                            "OrderInfo.OrderNo          = $ORDERNO And " +
                                                            "OrderDetail.ItemID         = $ITEMID; ";

        const String ctUpdateOrderInfoRemark = "Update Edata_OrderInfo " +
                                                    "Set " +
                                                        "OrderRemark = $ORDERREMARK " +
                                                    "Where " +
                                                        "Edata_OrderInfo.SubscriptionID = $SUBSCRIPTIONID And " +
                                                        "Edata_OrderInfo.OrderNo = $ORDERNO; ";

        const String ctGetAppManifestRow    = "Select " +
                                                    "* " +
                                              "From " +
                                                "Data_AppManifest " +
                                              "Where " +
                                                "SubscriptionID = $SUBSCRIPTIONID; ";

        const String ctSPRCreateSubscription = "Declare @SubscriptionID varchar(200); " +
                                                 "EXEC CRS_AddNewSubscription " +
                                                     "@ServiceProviderId        = $SERVICEPROVIDERID, " +
                                                     "@SErviceProviderLoginID   = $SERVICEPROVIDERLOGINID, " +
                                                     "@SErviceProviderPassword  = $SERVICEPROVIDERPASSWORD, " +
                                                     "@EServiceId               = $ESERVICEID, " +
                                                     "@ItemList                 = $ITEMLIST, " +
                                                     "@InvoiceNo                = $INVOICENO, " +
                                                     "@ServiceFee               = $SERVICEFEE, " +
                                                     "@Discount                 = $DISCOUNT, " +
                                                     "@Remark                   = $REMARK, " +
                                                     "@FrontEndPath             = $FRONTENDPATH, " +
                                                     "@MobileNo                 = $MOBILENO, " +
                                                     "@BusinessName             = $BUSINESSNAME, " +
                                                     "@AppName                  = $APPNAME, " +
                                                     "@BackgroundColor          = $BACKGROUNDCOLOR, " +
                                                     "@FrontEndIcon             = $FRONTENDICON, " +
                                                     "@BackEndIcon              = $BACKENDICON, " +
                                                     "@ReturnSubscriptionID     = @SubscriptionID Output; " +
                                               "Select  @SubscriptionID;";

        const String ctRetrieveSettingList = "Select " +
                                                 "* " +
                                             "From " +
                                                 "Admin_Setting " +
                                             "Where " +
                                                 "SettingGroup = '*' Or " +                                                 
                                                 "SettingGroup = Left($ESERVICEID,4) Or " +
                                                 "SettingGroup = Left($ESERVICEID,5) Or " +
                                                 "SettingGroup = $ESERVICEID Or " +
                                                 "SettingGroup = $SUBSCRIPTIONID;";

        const String ctRetrieveMessageList = "Select " +
                                                "* " +
                                             "From " +
                                                "Admin_Message " +
                                             "Where " +
                                                "Language = $LANGUAGE And " +
                                                "(GroupName = '*' Or " +
                                                " GroupName = $FORMNAME Or + " +
                                                " GroupName = Left($ESERVICEID,4) Or + " +
                                                " GroupName = $ESERVICEID Or + " +
                                                " GroupName = $SUBSCRIPTIONID); ";

        const String ctRetrieveTextList      = "Select " +
                                                    "Upper(Concat('@@',GroupName,'.',TextKey)) As TextKey, Text " +
                                                 "From " +
                                                    "Admin_Text " +
                                                 "Where " +
                                                    "Language = $LANGUAGE; ";

        const String ctGetLanguageOptionList = "Select " +
                                                    "* " +
                                               "From " +
                                                    "Admin_Language " +
                                               "Where " +
                                                    "Language in ($LANGUAGEOPTIONLIST)";

        const String ctGetTimeZoneList = "Select " +
                                             "* " +
                                         "From " +
                                             "Admin_TimeZone " +
                                         "Order By " +
                                             "DisplayOrder, TimeZoneID; ";
        
        public enum QueryType : int
        {
            GetSchema,
            RetrieveRoutingTable, RetrieveUrlRewriteTable,

            RetrieveFieldInfo,
            RetrieveInputInfo,
            GetFormInfoRow,  
            GetLanguageRow,
            GetLanguageOptionList,
             // GetPublicUserRow,
            GetUserRow,            
            GetEServiceRow,
            GetSubscriptionRow,
            GetSubscriptionRowByFrontEndPath,
            InsertServiceRequestLog,
            UpdateServiceRequestLog,
            
            GetSessionRow,
            GetSessionRowBySessionKey,
            UpdateSessionAccessInfo,

            InsertSessionRow,
            RetrieveWidgetList,
            RetrieveWidgetSubscriptionList,

            GetWidgetRow,
            GetPaymentRow,

            RetrieveDynamicQueryTable,

            RetrieveToolBarList,
            RetrieveKeyPadList,

            InsertOrderInfoRecord,
            InsertOrderDetailRecord,
            UpdateOrderDetailRecord,

            UpdateOrderInfoRemark,

            GetAppManifestRow,    
        
            RetrieveSettingList,

            RetrieveMessageList,
                       
            RetrieveTextList,

            SPRCreateSubscription,

            GetTimeZoneList,
            
        };

        public enum ConnectionMode { None, Primary, EService }

        const int ctQAAQueryType = 0;

        const int ctPlaceHolder = 0;
        const int ctColumnName = 1;

        const String ctPlaceHolderPrefix = "$";
        const char ctValueListSplitter = '#';
        const String ctPlaceHolderRegEx = @"\$[^\s,;)]+";
        const String ctPlaceHolderFixRegEx = "$& ";

        String clQueryString;
        String clOriginalQuery;
        String clValueTemplate;
        String clTrailingQuery;
        QueryClass clInnerQuery;

        ConnectionMode clConnectionMode;

        public QueryClass(QueryType paQueryType, ConnectionMode paConnectionMode = ConnectionMode.Primary)
        {
            clConnectionMode = paConnectionMode;
            ChooseQuery(paQueryType);
        }

        public QueryClass(String paQueryString, ConnectionMode paConnectionMode, bool paDecode = false)
        {
            clConnectionMode = paConnectionMode;

            if (paDecode) clQueryString = DecodeQuery(paQueryString);
            else clQueryString = paQueryString;

            clQueryString = clQueryString.Replace("$GETSYSTEMLOCALDATETIME", "'" + General.GetCurrentSystemLocalTime().ToString("yyyy-MM-dd HH:mm:ss") + "'");
            clQueryString = clQueryString.Replace("$GETSYSTEMLOCALDATE", "'" + General.GetCurrentSystemLocalTime().ToString("yyyy-MM-dd") + "'");

            clQueryString = Regex.Replace(clQueryString, ctPlaceHolderRegEx, ctPlaceHolderFixRegEx);
            if (clValueTemplate != null) clValueTemplate = Regex.Replace(clValueTemplate, ctPlaceHolderRegEx, ctPlaceHolderFixRegEx);

            clOriginalQuery = clQueryString;
        }

        private void ChooseQuery(QueryType paQueryType)
        {
            switch (paQueryType)
            {
                case QueryType.GetSchema                        : clQueryString = ctGetSchema; break;
                case QueryType.RetrieveRoutingTable             : clQueryString = ctRetrieveRoutingTable; break;
                case QueryType.RetrieveUrlRewriteTable          : clQueryString = ctRetrieveUrlRewriteTable; break;

                case QueryType.RetrieveFieldInfo                : clQueryString = ctRetrieveFieldInfo; break;
                case QueryType.RetrieveInputInfo                : clQueryString = ctRetrieveInputInfo; break;
                case QueryType.GetFormInfoRow                   : clQueryString = ctGetFormInfoRow; break;                                
                case QueryType.GetUserRow                       : clQueryString = ctGetUserRow; break;
                case QueryType.GetLanguageRow                   : clQueryString = ctGetLanguageRow; break;
                case QueryType.GetEServiceRow                   : clQueryString = ctGetEServiceRow; break;
                case QueryType.GetSubscriptionRow               : clQueryString = ctGetSubscriptionRow; break;
                case QueryType.GetSubscriptionRowByFrontEndPath : clQueryString = ctGetSubscriptionRowByFrontEndPath; break;

                case QueryType.InsertServiceRequestLog          : clQueryString = ctDeclareIdentityTable + ctInsertServiceRequestLog + ctReturnIdentity; break;
                case QueryType.UpdateServiceRequestLog          : clQueryString = ctUpdateServiceRequestLog; break;

                case QueryType.GetSessionRow                    : clQueryString = ctGetSessionRow; break;                
                case QueryType.GetSessionRowBySessionKey        : clQueryString = ctGetSessionRowBySessionKey; break;                
                case QueryType.InsertSessionRow                 : clQueryString = ctInsertSessionRow; break;
                case QueryType.UpdateSessionAccessInfo          : clQueryString = ctUpdateSessionAccessInfo; break;

                case QueryType.RetrieveWidgetList               : clQueryString = ctRetrieveWidgetList; break;
                case QueryType.RetrieveWidgetSubscriptionList   : clQueryString = ctRetrieveWidgetSubscriptionList; break;

                case QueryType.GetWidgetRow                     : clQueryString = ctGetWidgetRow; break;
                case QueryType.GetPaymentRow                    : clQueryString = ctGetPaymentRow; break;

                case QueryType.RetrieveDynamicQueryTable        : clQueryString = ctRetrieveDynamicQueryTable; break;
                case QueryType.RetrieveToolBarList              : clQueryString = ctRetrieveToolBarList; break;

                case QueryType.RetrieveKeyPadList               : clQueryString = ctRetrieveKeyPadList; break;

                case QueryType.InsertOrderInfoRecord            : clQueryString = ctDeclareIdentityTable + ctInsertOrderInfoRecord + ctReturnIdentity; break;
                case QueryType.InsertOrderDetailRecord          : clQueryString = ctInsertOrderDetailRecord; break;
                case QueryType.UpdateOrderDetailRecord          : clQueryString = ctUpdateOrderDetailRecord; break;

                case QueryType.UpdateOrderInfoRemark            : clQueryString = ctUpdateOrderInfoRemark; break;

                case QueryType.GetAppManifestRow                : clQueryString = ctGetAppManifestRow; break;

                case QueryType.RetrieveSettingList              : clQueryString = ctRetrieveSettingList; break;

                case QueryType.RetrieveMessageList              : clQueryString = ctRetrieveMessageList; break;

                case QueryType.RetrieveTextList                 : clQueryString = ctRetrieveTextList; break;

                case QueryType.SPRCreateSubscription            : clQueryString = ctSPRCreateSubscription; break;

                case QueryType.GetLanguageOptionList            : clQueryString = ctGetLanguageOptionList; break;

                case QueryType.GetTimeZoneList                  : clQueryString = ctGetTimeZoneList; break;
            }

            clQueryString = Regex.Replace(clQueryString, ctPlaceHolderRegEx, ctPlaceHolderFixRegEx);
            if (clValueTemplate != null) clValueTemplate = Regex.Replace(clValueTemplate, ctPlaceHolderRegEx, ctPlaceHolderFixRegEx);

            clQueryString = clQueryString.Replace("$GETSYSTEMLOCALDATETIME", "'" + General.GetCurrentSystemLocalTime().ToString("yyyy-MM-dd HH:mm:ss") + "'");
            clQueryString = clQueryString.Replace("$GETSYSTEMLOCALDATE", "'" + General.GetCurrentSystemLocalTime().ToString("yyyy-MM-dd") + "'");

            clOriginalQuery = clQueryString;
        }

        private String DecodeQuery(String paQuery)
        {
            String[] lcQueryArray;

            // Split betweeen Repeat data template string and Query statement.
            lcQueryArray = paQuery.Split(ctValueListSplitter);

            if (lcQueryArray.Length > 1)
                clValueTemplate = lcQueryArray[1];

            if (lcQueryArray.Length > 2) clTrailingQuery = lcQueryArray[2];
            else clTrailingQuery = ";";

            return (lcQueryArray[0]);
        }

        public String GetQueryString()
        {
            return clQueryString;
        }

        public String GetQueryChain()
        {
            return (clQueryString + (clInnerQuery != null ? clInnerQuery.GetQueryChain() : ""));
        }

        public void ResetQuery()
        {
            clQueryString = clOriginalQuery;
        }

        public void ReplacePlaceHolder(String paPlaceHolder, String paValue, bool paQuoteNeed)
        {
            if (paValue != null)
            {              
                paValue = (paQuoteNeed ? "N'" + paValue.Replace(@"'", @"''") + "'" : paValue);                
                clQueryString = clQueryString.Replace(paPlaceHolder, paValue);
            }
        }

        private String GetString(object paValue, bool paDefaultQuote = true)
        {
            switch (paValue.GetType().ToString())
            {
                case "System.DateTime": return ("'" + ((DateTime)paValue).ToString("yyyy-MM-dd HH:mm:ss") + "'");
                case "System.Decimal": return (((Decimal)paValue).ToString("F4"));
                case "System.Boolean": return ((bool)paValue).ToString();
                case "System.Double": return ((Double)paValue).ToString();
                case "System.Int32": return ((Int32)paValue).ToString();
                case "System.DBNull": return ("NULL");
                default:
                    {
                        if (paDefaultQuote)
                            return (("N'" + paValue.ToString().Replace(@"'", @"''") + "'"));
                        else
                            return (paValue.ToString());
                    }
            }
        }

        public void ReplaceRowPlaceHolder(DataRow paDataRow)
        {
            object lcValue;
            String lcPlaceHolder;

            for (int lcCount = 0; lcCount < paDataRow.Table.Columns.Count; lcCount++)
            {
                lcValue = paDataRow[paDataRow.Table.Columns[lcCount].ColumnName];
                lcPlaceHolder = "$" + paDataRow.Table.Columns[lcCount].ColumnName.ToUpper() + " ";
                ReplacePlaceHolder(lcPlaceHolder, GetString(lcValue), false);
            }
        }

        public void ReplaceDictionaryPlaceHolder(Dictionary<String,String> paDictionary, bool paPlaceHolderSpacing = true)
        {
            object lcValue;
            String lcPlaceHolder;
            String lcSpace;

            if (paDictionary != null)
            {
                lcSpace = paPlaceHolderSpacing ? " " : "";

                foreach (String lcKey in paDictionary.Keys)
                {
                    lcValue = paDictionary[lcKey];
                    lcPlaceHolder = "$" + lcKey.ToUpper() + lcSpace;
                    ReplacePlaceHolder(lcPlaceHolder, GetString(lcValue, !lcKey.StartsWith("FPMNQ_")), false);
                }
            }
        }

        public void ReplaceMultipleRowPlaceHolder(DataTable paDataTable)
        {
            DataRow lcDataRow;
            String lcValueStr;
            String lcPlaceHolder;

            for (int lcCount = 0; lcCount < paDataTable.Rows.Count; lcCount++)
            {
                lcValueStr = clValueTemplate;
                lcDataRow = paDataTable.Rows[lcCount];

                if (lcDataRow.RowState != DataRowState.Deleted)
                {
                    foreach (DataColumn lcColumn in paDataTable.Columns)
                    {
                        lcPlaceHolder = "$" + lcColumn.ColumnName.ToUpper() + " ";
                        lcValueStr = lcValueStr.Replace(lcPlaceHolder, GetString(lcDataRow[lcColumn.ColumnName]));
                    }
                    clQueryString += lcValueStr + ((lcCount < paDataTable.Rows.Count - 1) ? "," : clTrailingQuery);
                }
            }
        }

        public void ReplaceDynamicPlaceHolder(dynamic paDynamicData)
        {
            String lcValueStr;
            String lcPlaceHolder;

            for (int lcCount = 0; lcCount < paDynamicData.Length; lcCount++)
            {
                lcValueStr = clValueTemplate;
                
                foreach (dynamic lcKey in paDynamicData.Keys)
                {
                    lcPlaceHolder = "$" + lcKey.ToUpper() + " ";
                    lcValueStr = lcValueStr.Replace(lcPlaceHolder, "N'" + Convert.ToString(paDynamicData[lcCount][lcKey]) + "'");
                }
                clQueryString += lcValueStr + ((lcCount < paDynamicData.Length - 1) ? "," : clTrailingQuery);                
            }
        }



        public void ReplaceDictionaryPlaceHolder(Dictionary<String, object> paDictionary)
        {
            object lcValue;
            String lcPlaceHolder;

            foreach (String lcKey in paDictionary.Keys)
            {
                lcValue = paDictionary[lcKey];
                lcPlaceHolder = "$" + lcKey.ToUpper() + " ";
                ReplacePlaceHolder(lcPlaceHolder, GetString(lcValue), false);
            }
        }


        public QueryClass SetInnerQuery(QueryClass paQueryClass)
        {
            if (clInnerQuery == null) clInnerQuery = paQueryClass;
            else clInnerQuery.SetInnerQuery(paQueryClass);

            return (this);
        }

        public DataTable RunQuery()
        {
            return (DatabaseInterface.GetInstance().RunQuery(clQueryString, clConnectionMode));
        }

        public DataSet RunQueryEx()
        {
            return (DatabaseInterface.GetInstance().RunQueryEx(clQueryString, clConnectionMode));
        }

        public object GetResult()
        {
            return (DatabaseInterface.GetInstance().ExecuteScalar(clQueryString, clConnectionMode));
        }

        public int ExecuteNonQuery()
        {
            return (DatabaseInterface.GetInstance().ExecuteNonQuery(clQueryString, clConnectionMode));
        }

        public object ExecuteFeedBackQuery(String paFeedBackVar)
        {
            return (DatabaseInterface.GetInstance().ExecuteFeedBackQuery(clQueryString, paFeedBackVar, clConnectionMode));
        }

        public void ExecuteNonQueryChain()
        {
            DatabaseInterface.GetInstance().ExecuteNonQuery(GetQueryChain(), clConnectionMode);
        }

        public object ExecuteFeedBackQueryChain(String paFeedBackVar)
        {
            return (DatabaseInterface.GetInstance().ExecuteFeedBackQuery(GetQueryChain(), paFeedBackVar, clConnectionMode));
        }
    } // QueryClass    
}
