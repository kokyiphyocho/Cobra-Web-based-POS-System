using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using CobraFoundation;
using CobraFrame;

namespace CobraApplicationFrame
{
    public class DataRecordUpdate
    {
        private const String ctKEYKeyColumnList         = "#KEYCOLUMNLIST";
        private const String ctKEYDataColumnList        = "#DATACOLUMNLIST";
        private const String ctKEYIdentifierColumnList  = "#IDENTIFIERCOLUMNLIST";
        private const String ctComma                    = ",";
        private const String ctPeriod                   = "."; 
        
        Dictionary<String, String>      clDataList;
        Dictionary<String, String>      clCompiledDataList;
        bool                            clMetaMode;        
        String                          clUpdateQuery;
        QueryClass.ConnectionMode       clConnectionMode;

        public static DataRecordUpdate CreateInstance(String paJSONData)
        {
            FormInfoManager lcFormInfoManager;
            // JavaScriptSerializer lcJavaScriptSerializer;    
            Dictionary<String,String> lcDataList;
            String lcUpdateQuery;            

            lcFormInfoManager = ApplicationFrame.GetInstance().ActiveFormInfoManager;

            if ((!String.IsNullOrEmpty(paJSONData)) && (lcFormInfoManager != null) && (!String.IsNullOrEmpty(lcUpdateQuery = lcFormInfoManager.ActiveRow.UpdateQuery)))
            {
                // lcJavaScriptSerializer = new JavaScriptSerializer();
                if (((lcDataList = General.JSONDeserialize<Dictionary<String,String>>(paJSONData)) != null) && (lcDataList.Count > 0))
                {                    
                    return (new DataRecordUpdate(lcDataList, lcUpdateQuery));
                }

            }
            return (null);
        }

        public DataRecordUpdate(Dictionary<String, String> paDataList, String paUpdateQuery)
        {
            clDataList = paDataList;

            clUpdateQuery = paUpdateQuery;
            clConnectionMode = ApplicationFrame.GetInstance().ActiveFormInfoManager.ConnectionMode;
            clMetaMode = ApplicationFrame.GetInstance().ActiveFormInfoManager.IsAttributeSet(FormInfoManager.FormAttribute.MetaMode);
        }

        private String BuildSQLColumnValueString(Dictionary<String,String> paActiveDataList, String paKeyName, String paDelimiter)
        { 
            Dictionary<String, String>   lcDictionary;

            if ((!String.IsNullOrWhiteSpace(paKeyName)) && (paActiveDataList.ContainsKey(paKeyName)))
            {
                lcDictionary = paActiveDataList.GetSubsetList(paActiveDataList[paKeyName]);
                if ((lcDictionary != null) && (lcDictionary.Count > 0))
                    return ("N\'" + String.Join(paDelimiter, lcDictionary.AsEnumerable().Select(e => e.Key + " = N''" + e.Value + "''").ToArray()) + "\'");
            }                
            return ("NULL");            
        }

        private String BuildSQLValueString(Dictionary<String,String> paActiveDataList, String paKeyName, String paDelimiter)
        {
            if ((!String.IsNullOrWhiteSpace(paKeyName)) && (paActiveDataList.ContainsKey(paKeyName)))
            {
                return ("N\'(" + String.Join(ctComma, paActiveDataList.GetUniCodeValueList(paActiveDataList[paKeyName])) + ")\'");                
            }                
            return ("NULL");            
        }  

        public bool UpdateData()
        {
            if (clMetaMode) return (UpdateMetaMode());
            return (UpdatDataRecord(clDataList));
        }

        private bool UpdateMetaMode()
        {
            MetaDataRow                 lcMetaDataRow;
            String                      lcDataColumnList;
            
            lcMetaDataRow = new MetaDataRow(clDataList, ApplicationFrame.GetInstance().ActiveFormInfoManager.RunRetrieveRow());
            clCompiledDataList = lcMetaDataRow.DictGetCompiledDataList();

            if ((lcDataColumnList = clCompiledDataList.GetData(ctKEYDataColumnList)) != null)
                clCompiledDataList[ctKEYDataColumnList] = String.Join(",", lcDataColumnList.Split(new[] { ctComma.ToString() }, StringSplitOptions.RemoveEmptyEntries)
                                                         .Select(part => part.Split(new[] { ctPeriod.ToString() }, StringSplitOptions.RemoveEmptyEntries)).Select(k => k[0]).Distinct().ToArray());

            

            return (UpdatDataRecord(clCompiledDataList));
        }

        private bool UpdatDataRecord(Dictionary<String,String> paActiveDataList)
        {
            QueryClass lcQuery;            

            try
            {
                lcQuery = new QueryClass(clUpdateQuery, clConnectionMode);
                lcQuery.ReplaceDictionaryPlaceHolder(paActiveDataList);
                lcQuery.ReplacePlaceHolder("$COLUMNLIST", "N\'(" + paActiveDataList.GetData(ctKEYDataColumnList) + ")\'", false);
                lcQuery.ReplacePlaceHolder("$VALUELIST", BuildSQLValueString(paActiveDataList, ctKEYDataColumnList, ctComma), false);
                lcQuery.ReplacePlaceHolder("$UPDATEDATASTRING", BuildSQLColumnValueString(paActiveDataList, ctKEYDataColumnList, ctComma), false);
                lcQuery.ReplacePlaceHolder("$CONDITIONSTRING", BuildSQLColumnValueString(paActiveDataList, ctKEYKeyColumnList, " And "), false);            

                return(lcQuery.ExecuteNonQuery() > 0);
            }
            catch (Exception paException)
            {
                General.WriteExceptionLog(paException, "UPDATEDATA : " + ApplicationFrame.GetInstance().ActiveSubscription.ActiveRow.SubscriptionID);
                return (false);
            }
        }        
    }

    public class DataListUpdate
    {
        dynamic     clDataList;
        String      clUpdateQuery;
        QueryClass.ConnectionMode clConnectionMode;

        public static DataListUpdate CreateInstance(String paJSONData)
        {
            FormInfoManager         lcFormInfoManager;
            // JavaScriptSerializer    lcJavaScriptSerializer;
            dynamic                 lcDataList;
            String                  lcUpdateQuery;
            

            lcFormInfoManager = ApplicationFrame.GetInstance().ActiveFormInfoManager;

            if ((!String.IsNullOrEmpty(paJSONData)) && (lcFormInfoManager != null) && (!String.IsNullOrEmpty(lcUpdateQuery = lcFormInfoManager.ActiveRow.UpdateQuery)))
            {
                // lcJavaScriptSerializer = new JavaScriptSerializer();
                if (((lcDataList = General.JSONDeserialize<dynamic>(paJSONData)) != null) && (lcDataList.Length > 0))
                {
                    return (new DataListUpdate(lcDataList, lcUpdateQuery));
                }

            }
            return (null);
        }

        public DataListUpdate(dynamic paDataList, String paUpdateQuery)
        {
            clDataList          = paDataList;
            clUpdateQuery       = paUpdateQuery;
            clConnectionMode    = ApplicationFrame.GetInstance().ActiveFormInfoManager.ConnectionMode;
        }

        public bool UpdateData()
        {
            String          lcPlaceHolder;
            String          lcValue;
            QueryClass      lcMasterQuery;
            QueryClass      lcQueryClass;
            
            lcQueryClass    = null;
            lcMasterQuery   = null;

            try
            {                   
                for (int lcCount = 0; lcCount < clDataList.Length; lcCount++)
                {
                    lcQueryClass = new QueryClass(clUpdateQuery, clConnectionMode);
                    ApplicationFrame.GetInstance().ActiveFormInfoManager.ReplaceQueryPlaceHolder(lcQueryClass);

                    foreach(dynamic lcKey in clDataList[lcCount].Keys)
                    {                        
                        lcPlaceHolder = "$" + lcKey.ToUpper();
                        lcValue = Convert.ToString(clDataList[lcCount][lcKey]);

                        lcQueryClass.ReplacePlaceHolder(lcPlaceHolder, lcValue, true);
                    }
                    
                    if (lcMasterQuery == null) lcMasterQuery = lcQueryClass;
                    else lcMasterQuery.SetInnerQuery(lcQueryClass);
                }

                lcMasterQuery.ExecuteNonQueryChain();
                return (true);
            }
            catch (Exception paException)
            {
                General.WriteExceptionLog(paException, "DATALISTUPDATE : " + ApplicationFrame.GetInstance().ActiveSubscription.ActiveRow.SubscriptionID);
                return (false);
            }
        }
    }    
}
