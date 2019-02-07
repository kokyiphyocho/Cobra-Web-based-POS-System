using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using CobraFoundation;

namespace CobraFrame
{
    public class FormInfoManager
    {
        private const String ctColumnPlaceHolder = "@[$COLUMNNAME]";
        
        private const String ctESCCommand = "#";

        private const char   ctParamDelimiter = ',';
        private const char   ctGroupDelimiter = ',';
        private const String ctFirstLevelDelimiter = "::";
        private const String ctSecondLevelDelimiter = ";;";
                
        public enum FormAttribute { Desktop, MetaMode, NoSession, NoTitle, NoToolBar, PrinterStatus }

        String                      clEncodedFormName;
        FormInfoRow                 clFormInfoRow;
        FieldInfoManager            clFieldInfoManager;        
        Dictionary<String,String>   clFormParam;
        QueryClass.ConnectionMode   clConnectionMode;
        MetaDataBlock               clToolBarConfig;
        String                      clFieldInfoDataGroup;
        String                      clInputInfoDataGroup;

        public FormInfoRow ActiveRow                    { get { return (clFormInfoRow); } }
        public FieldInfoManager FieldInfoManager        { get { return (clFieldInfoManager); } }     
        public QueryClass.ConnectionMode ConnectionMode { get { return (clConnectionMode); } }
        public String EncodedFormName                   { get { return (clEncodedFormName); } }
        public String FieldInfoDataGroup                { get { return (clFieldInfoDataGroup); } }
        public String InputInfoDataGroup                { get { return (clInputInfoDataGroup); } }
        
        static public FormInfoManager CreateInstance(String paEncodedFormName)
        {
            String                      lcFormName;
            FormInfoRow                 lcFormInfoRow;
            Dictionary<String, String>  lcFormParam;

            if (!String.IsNullOrWhiteSpace(paEncodedFormName))
            {                
                DecodeFormName(paEncodedFormName, out lcFormName, out lcFormParam);
                if ((lcFormInfoRow = GetFormInfoRow(lcFormName)) != null) return (new FormInfoManager(lcFormInfoRow, paEncodedFormName, lcFormParam));
                else
                {
                    if ((lcFormInfoRow = GetFormInfoRow(ApplicationFrame.GetInstance().ActiveEservice.GetPrimaryFormName())) != null) 
                        return (new FormInfoManager(lcFormInfoRow, paEncodedFormName, lcFormParam));
                }
                
            }

            return (null);
        }

        static public void DecodeFormName(String paEncodedFormName, out String paFormName, out Dictionary<String, String> paFormParam)
        {
            String[] lcStrArray;

            paFormParam = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase); // Make Sure Param is not null.
            paFormName  = String.Empty;

            if (!String.IsNullOrWhiteSpace(paEncodedFormName))
            {
                paFormName = HttpUtility.UrlDecode(General.Base64Decode(paEncodedFormName));

                lcStrArray = paFormName.Split(new char[] { ctParamDelimiter }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (lcStrArray.Length > 1)
                {
                    paFormParam  = lcStrArray[1].Split(new[] { ctSecondLevelDelimiter }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(part => part.Split(new[] { ctFirstLevelDelimiter },StringSplitOptions.None))
                                                .ToDictionary(split => split[0], split => split[1]);
                }

                paFormName = lcStrArray.Length > 0 ? lcStrArray[0] : paFormName;
            }           
        }        

        static private FormInfoRow GetFormInfoRow(String paFormName)
        {
            QueryClass lcQuery;
            DataTable lcDataTable;

            lcQuery = new QueryClass(QueryClass.QueryType.GetFormInfoRow);
            lcQuery.ReplacePlaceHolder("$FORMNAME", paFormName, true);

            if (((lcDataTable = lcQuery.RunQuery()) != null) && (lcDataTable.Rows.Count > 0))
                return (new FormInfoRow(lcDataTable.Rows[0]));
            else return (null);
        }

        private FormInfoManager(FormInfoRow paFormInfoRow, String paEncodedFormName, Dictionary<String, String> paFormParam)
        {
            clFormInfoRow = paFormInfoRow;
            ParseDataGroup();
            clEncodedFormName = paEncodedFormName;
            clConnectionMode = General.ParseEnum<QueryClass.ConnectionMode>(clFormInfoRow.ConnectionMode, QueryClass.ConnectionMode.None);
            
            clFieldInfoManager = new FieldInfoManager(this);
                        
            clFormParam = paFormParam;
            clFormParam.MergeDictionary(paFormInfoRow.DefaultParameters.Split(new[] { ctSecondLevelDelimiter }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(part => part.Split(new[] { ctFirstLevelDelimiter }, StringSplitOptions.None))
                                                .ToDictionary(split => split[0], split => split[1]));


           TranslateDictionary(clFormParam);
           clToolBarConfig = new MetaDataBlockCollection(ActiveRow.ToolBarConfig)[0];
        }

        private void ParseDataGroup()
        {
            String[] lcDataGroupList;

            lcDataGroupList = clFormInfoRow.DataGroup.Split(ctGroupDelimiter);

            clFieldInfoDataGroup = lcDataGroupList[0];

            if (lcDataGroupList.Length > 1) clInputInfoDataGroup = lcDataGroupList[1];
            else clInputInfoDataGroup = lcDataGroupList[0];
        }

        private DataTable RetrieveData()
        {
            QueryClass lcQuery;

            if (!String.IsNullOrEmpty(clFormInfoRow.RetrieveQuery))
            {
                try
                {                    
                    lcQuery = new QueryClass(clFormInfoRow.RetrieveQuery, clConnectionMode);
                    lcQuery.ReplaceDictionaryPlaceHolder(clFormParam);                                  
                    return (lcQuery.RunQuery());                    
                }
                catch(Exception paException)
                {
                    General.WriteExceptionLog(paException, "FORMINFOMANAGER.RETRIEVEDATA");
                }
            }
            return (null);
        }

        public void AddFormParam(String paJSONParamStr, bool paOverwrite)
        {
            // JavaScriptSerializer    lcJavascriptSerializer;
            Dictionary<String, String>  lcDictionary;

            // lcJavascriptSerializer = new JavaScriptSerializer();
            lcDictionary =  General.JSONDeserialize<Dictionary<String, String>>(paJSONParamStr);

            clFormParam.MergeDictionary(lcDictionary, paOverwrite);
        }

        private void TranslateDictionary(Dictionary<String, String> paDictionary)
        {            
            if (paDictionary != null)
            {
                for (int lcCount = 0; lcCount < paDictionary.Keys.Count; lcCount++) 
                {
                    var lcItem = paDictionary.ElementAt(lcCount);
                    paDictionary[lcItem.Key] = TranslateStringExStr(paDictionary[lcItem.Key]);                    
                }
            }                
        }

        public String GetFormParam(String paKey, String paDefaultValue = null)
        {
            if (clFormParam.Keys.Contains(paKey)) return (clFormParam[paKey]);
            else return (paDefaultValue);
        }

        private String GetFormattedData(object paData, String paFormatString)
        {
            if (paData != null)
                return (General.GetFormattedDBData(paData, paFormatString));
            return (String.Empty);
        }

        public bool IsAttributeSet(FormAttribute paAttribute)
        {
            return (ActiveRow.FormAttribute.Contains("#" + paAttribute.ToString().ToUpper()));
        } 
       
        private String ReplaceParameterPlaceHolder(String paString)
        {
            String lcPlaceHolder;

            if (clFormParam != null)
            {
                foreach (String paKey in clFormParam.Keys)
                {
                    lcPlaceHolder = ctColumnPlaceHolder.Replace("$COLUMNNAME", paKey.ToUpper());
                    paString = paString.Replace(lcPlaceHolder, clFormParam[paKey]);
                }                
            }

            return (paString);
        }

        private String ReplaceDataContentPlaceHolder(DataRow paDataRow, String paString)
        {
            String lcPlaceHolder;
            String lcColumnName;

            if (paDataRow != null)
            {
                for (int lcCount = 0; lcCount < paDataRow.Table.Columns.Count; lcCount++)
                {
                    lcColumnName = paDataRow.Table.Columns[lcCount].ColumnName.ToUpper();
                    lcPlaceHolder = ctColumnPlaceHolder.Replace("$COLUMNNAME", lcColumnName);
                    paString = paString.Replace(lcPlaceHolder, GetFormattedData(paDataRow[lcColumnName], String.Empty));
                }
                return (paString);
            }

            return (paString);
        }        

        private String ReplaceServiceDataPlaceHolder(String paString)
        {
            paString = ReplaceDataContentPlaceHolder(ApplicationFrame.GetInstance().ActiveSubscription.ActiveRow.Row, paString);
            paString = ReplaceDataContentPlaceHolder(ApplicationFrame.GetInstance().ActiveEservice.ActiveRow.Row, paString);
            
            if (ApplicationFrame.GetInstance().ActiveSessionController.User.ActiveRow != null)
                paString = ReplaceDataContentPlaceHolder(ApplicationFrame.GetInstance().ActiveSessionController.User.ActiveRow.Row, paString);

            paString = ReplaceParameterPlaceHolder(paString);

            return (paString);
        }

        public void ReplaceQueryPlaceHolder(QueryClass paQueryClass)
        {
            UserRow lcUserRow;
            if (paQueryClass != null)
            {               
                if (ApplicationFrame.GetInstance().ActiveSessionController.User.IsLoggedIn)
                    paQueryClass.ReplacePlaceHolder("$USERID", ApplicationFrame.GetInstance().ActiveSessionController.User.ActiveRow.UserID.ToString(), true);

                paQueryClass.ReplaceRowPlaceHolder(ApplicationFrame.GetInstance().ActiveSubscription.ActiveRow.Row);
                paQueryClass.ReplaceRowPlaceHolder(ApplicationFrame.GetInstance().ActiveEservice.ActiveRow.Row);                

                if ((lcUserRow = ApplicationFrame.GetInstance().ActiveSessionController.User.ActiveRow) == null) 
                    lcUserRow = new UserRow(TableManager.GetInstance().GetNewRow(TableManager.TableType.User, true));

                paQueryClass.ReplaceRowPlaceHolder(lcUserRow.Row);

                paQueryClass.ReplacePlaceHolder("$ACTIVELANGUAGE", ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting.Language, true);

                paQueryClass.ReplaceDictionaryPlaceHolder(clFormParam, false);
            }
        }

        public String TranslateString(String paString)
        {
            return (ReplaceServiceDataPlaceHolder(paString));
        }

        public String TranslateStringExStr(String paString)
        {
            if (!String.IsNullOrEmpty(paString))
            {
                if (paString.Trim().StartsWith(ctESCCommand)) return (TranslateCommandStr(paString.Trim().ToUpper()));
                else return (ReplaceServiceDataPlaceHolder(paString));
            }
            else return (null);
        }

        public object TranslateStringEx(String paString)
        {
            if (!String.IsNullOrEmpty(paString))
            {
                if (paString.Trim().StartsWith(ctESCCommand)) return (TranslateCommand(paString.Trim().ToUpper()));
                else return (ReplaceServiceDataPlaceHolder(paString));
            }
            else return (null);
        }

        private String TranslateCommandStr(String paString)
        {
            switch (paString)
            {
                case "#TODAY": return (General.ConvertUTCToSystemLocalTime(DateTime.UtcNow).ToString("yyyy-MM-dd"));             
                default: return (null);
            }
        }

        private object TranslateCommand(String paString)
        {
            String[]  lcCommand;
            String    lcParameter;

            if (paString != null)
            {
                lcCommand = paString.Split(ctParamDelimiter);

                if (lcCommand.Length > 1) lcParameter = lcCommand[1];
                else lcParameter = String.Empty;

                switch (lcCommand[0])
                {
                    case "#RETRIEVEQUERY"       : return (RunRetrieveQuery());
                    case "#RETRIEVEROW"         : return (RunRetrieveRow());
                    case "#WIDGETROW"           : return (WidgetManager.GetWidgetRow(clFormParam[paString]));                    
                    case "#DYNAMICQUERYROW"     : return (DynamicQueryManager.GetInstance().GetDataRowResult(lcParameter));
                    case "#DYNAMICQUERYTABLE"   : return (DynamicQueryManager.GetInstance().GetDataTableResult(lcParameter));
                    default: return (null);
                }
            }
            else return (null);
        }

        public DataRow RunRetrieveRow()
        {
            DataTable lcDataTable;

            if (((lcDataTable = RunRetrieveQuery()) != null) && (lcDataTable.Rows.Count > 0)) return (lcDataTable.Rows[0]);
            else return (null);
        }

        public DataTable RunRetrieveQuery()
        {
            QueryClass   lcQuery;

            if (!String.IsNullOrEmpty(ActiveRow.RetrieveQuery))
            {
                lcQuery = new QueryClass(ActiveRow.RetrieveQuery, clConnectionMode);              
                ReplaceQueryPlaceHolder(lcQuery);                
                lcQuery.ReplaceDictionaryPlaceHolder(clFormParam);                
                return (lcQuery.RunQuery());
            }
            else return (null);
        }

        public DataSet RunRetrieveQueryEx()
        {
            QueryClass lcQuery;

            if (!String.IsNullOrEmpty(ActiveRow.RetrieveQuery))
            {
                lcQuery = new QueryClass(ActiveRow.RetrieveQuery, clConnectionMode);
                ReplaceQueryPlaceHolder(lcQuery);
                lcQuery.ReplaceDictionaryPlaceHolder(clFormParam);
                return (lcQuery.RunQueryEx());
            }
            else return (null);
        }

        public String GetCustomToolBarLink(String paItemName)
        {
            MetaDataElement lcMetaDataElement;

            if ((!String.IsNullOrEmpty(paItemName)) && (clToolBarConfig != null) &&
                ((lcMetaDataElement = clToolBarConfig[paItemName]) != null))
            {
                return (lcMetaDataElement[0]);
            }

            return (null);
        }
    }

    public class FieldInfoManager
    {
        DataTable   clFieldInfoList;        
        String      clEServiceID;
        String      clDataGroup;
        FormInfoManager  clFormInfoManager;
        InputInfoManager clInputInfoManager;

        protected const String ctFILRowIndex    = "RowIndex = $ROWINDEX";
        protected const String ctFILColumnName  = "ColumnName = '$COLUMNNAME'"; 
        protected const String ctCOLRowIndex    = "RowIndex";

        public DataTable FieldInfoList { get { return (clFieldInfoList); } }
        public InputInfoManager ActiveInputInfoManager { get { return (GetInputInfoManager()); } }
        public FormInfoManager ActiveFormInfoManager { get { return (clFormInfoManager); } }        
        
        internal FieldInfoManager(FormInfoManager paFormInfoManager)
        {
            clFormInfoManager   = paFormInfoManager;            
            clEServiceID        = ApplicationFrame.GetInstance().ActiveEservice.ActiveRow.EServiceID;
            clDataGroup         = paFormInfoManager.FieldInfoDataGroup;
            clFieldInfoList     = RetrieveFieldInfoList(clDataGroup);            
        }

        private DataTable RetrieveFieldInfoList(String paDataGroup)
        {
            QueryClass lcQuery;

            lcQuery = new QueryClass(QueryClass.QueryType.RetrieveFieldInfo);
            lcQuery.ReplacePlaceHolder("$ESERVICEID", clEServiceID, true);
            lcQuery.ReplacePlaceHolder("$DATAGROUP", paDataGroup, true);            

            return (lcQuery.RunQuery());
        }

        private InputInfoManager GetInputInfoManager()
        {
            if (clInputInfoManager == null) clInputInfoManager = new InputInfoManager();
            return (clInputInfoManager);
        }

        public int LowerRowIndexBound()
        {            
            return (FieldInfoList.AsEnumerable().Select(row => row.Field<int>(ctCOLRowIndex)).Min());
        }

        public int UpperRowIndexBound()
        {
            return (FieldInfoList.AsEnumerable().Select(row => row.Field<int>(ctCOLRowIndex)).Max());
        }

        public DataRow[] GetFieldInfoListByRow(int paRowIndex)
        {
            return (FieldInfoList.Select(ctFILRowIndex.Replace("$ROWINDEX", paRowIndex.ToString())));
        }

        public FieldInfoRow FetchFieldInfoRow(String paColumnName)
        {            
            DataRow[] lcDataRows;

            if (!String.IsNullOrEmpty(paColumnName))
            {
                lcDataRows = FieldInfoList.Select(ctFILColumnName.Replace("$COLUMNNAME", paColumnName));
                if ((lcDataRows != null) && (lcDataRows.Length > 0)) return(new FieldInfoRow(lcDataRows[0]));
            }

            return (null);            
        }

        static public String GetDefaultValue(FieldInfoRow paFieldInfoRow)
        {
            switch (paFieldInfoRow.DefaultValue)
            {
                default: return (paFieldInfoRow.DefaultValue);
            }
        }

    }

    public class InputInfoManager
    {
        public enum InputInfoAttribute  { Title, Mandatory, Hide, KeyField, Identifier, HideBlock, ReadOnly, Password, DataMirror, DelimitedList, PopUpBlock, DynamicNumber, Persist }
        public enum ControlType         { None, Label, TextBox, TextArea, SelectBox, SlideSelection, ColorSlideSelection, ImageSlideSelection, Calendar}

        public delegate void DelegateCustomComponentRenderer(ComponentController paComponentController, InputInfoRow lcInputInfoRow, String paActiveValue);

        DataTable           clInputInfoList;        
        String              clInputGroup;
        String              clEServiceID;        
        LanguageManager     clLanguageManager;
        Dictionary<String, String> clFormatStringList;
        bool                clForceReadOnlyMode;
        
        protected const String ctFILTitleList       = "Attribute Like '%#TITLE%'";    
        protected const String ctFILSubGroup        = "SubGroup = '$SUBGROUP'";
        
        protected const String ctCOLSubGroup        = "SubGroup";
        protected const String ctCOLAttribute       = "Attribute";
        protected const String ctCOLInputName       = "InputName";
        protected const String ctCOLFormatString    = "FormatString";

        protected const String ctVirtualColumnPrefix = "#";
        protected const String ctCSSSeparator        = ":";

        protected const String ctCR                 = "&#13;";
        protected const String ctMultilineDelimiter = ";;";

        const String ctCMDOpenSlide                 = "@cmd%openslide";
        const String ctCMDOpenCalendar              = "@cmd%opencalendar";
        
        public DataTable    InputInfoList       { get { return (clInputInfoList); } }
        
        public event DelegateCustomComponentRenderer CustomComponentRenderer
        {
            add { clCustomComponentRendererEvent += value; }
            remove { clCustomComponentRendererEvent -= value;  }
        }

        private DelegateCustomComponentRenderer clCustomComponentRendererEvent;

        public InputInfoManager(String paInputInfoGroup = null)
        {            
            clLanguageManager   = ApplicationFrame.GetInstance().ActiveSubscription.ActiveLanguage;
            clEServiceID        = ApplicationFrame.GetInstance().ActiveEservice.ActiveRow.EServiceID;            

            if (String.IsNullOrEmpty(paInputInfoGroup))
                clInputGroup        = ApplicationFrame.GetInstance().ActiveFormInfoManager.InputInfoDataGroup;
            else
                clInputGroup = paInputInfoGroup;

            clInputInfoList     = RetrieveInputInfoList(clInputGroup);            
            clFormatStringList = clInputInfoList.AsEnumerable().Where(row => !row.Field<String>(ctCOLInputName).StartsWith("#"))
                                                                .ToDictionary<DataRow, String, String>(row => row.Field<String>(ctCOLInputName), row => row.Field<String>(ctCOLFormatString));

            clForceReadOnlyMode = false;
        }

        private DataTable RetrieveInputInfoList(String paDataGroup)
        {
            QueryClass lcQuery;

            lcQuery = new QueryClass(QueryClass.QueryType.RetrieveInputInfo);
            lcQuery.ReplacePlaceHolder("$ESERVICEID", clEServiceID, true);
            lcQuery.ReplacePlaceHolder("$INPUTGROUP", paDataGroup, true);            

            return (lcQuery.RunQuery());
        }

        public void SetForceReadOnlyMode(bool paForceReadOnlyMode)
        {
            clForceReadOnlyMode = paForceReadOnlyMode;
        }

        public bool IsAttributeSet(InputInfoRow paInputInfoRow, InputInfoAttribute paInputInfoAttribute)
        {
            return (paInputInfoRow.Attribute.Contains("#" + paInputInfoAttribute.ToString().ToUpper()));
        }

        public DataRow[] GetInputInfoListBySubGroup(String paSubGroup)
        {            
            return(InputInfoList.AsEnumerable().Where(r => r.Field<String>(ctCOLSubGroup).ToString().ToUpper() == paSubGroup.ToString().ToUpper()).ToArray());          
        }

        public DataRow GetSubGroupTitle(String paSubGroup)
        {
            return(InputInfoList.AsEnumerable().Where(r => r.Field<String>(ctCOLAttribute).ToString().ToUpper().Contains(InputInfoAttribute.Title.ToString().ToUpper())).FirstOrDefault());            
        }    

        public DataRow[] GetInputInfoTitleList()
        {
            return (InputInfoList.Select(ctFILTitleList));
        }

        public List<String> GetSubGroupList()
        {
            return (General.GetDistinctColumnValue(InputInfoList, ctCOLSubGroup));            
        }

        public void RenderAllSubGroups(ComponentController paComponentController, MetaDataRow paMetaDataRow)
        {
            List<String> lcSubGroupList;

            lcSubGroupList = GetSubGroupList();

            for (int lcCount = 0; lcCount < lcSubGroupList.Count; lcCount++)
            {
                RenderSubGroup(paComponentController, null, lcSubGroupList[lcCount], paMetaDataRow);
            }
        }           

        public void RenderSubGroup(ComponentController paComponentController, String paPrimaryClass, String paSubGroup, MetaDataRow paMetaDataRow)
        {
            DataRow[]       lcDataRow;            
            InputInfoRow    lcInputInfoRow;
            String          lcActiveValue;
            
            if ((lcDataRow = GetInputInfoListBySubGroup(paSubGroup)).Length > 0)
            {
                lcInputInfoRow = new InputInfoRow(lcDataRow[0]);

                if (!String.IsNullOrEmpty(lcInputInfoRow.ElementCss))
                {
                    if (!lcInputInfoRow.ElementCss.Contains(ctCSSSeparator)) paPrimaryClass = (paPrimaryClass + " " + lcInputInfoRow.ElementCss).Trim();                
                }

                if (IsAttributeSet(lcInputInfoRow, InputInfoAttribute.PopUpBlock))
                {
                    for (int lcCount = 0; lcCount < lcDataRow.Length; lcCount++)
                    {
                        lcInputInfoRow.Row = lcDataRow[lcCount];
                        if (!IsAttributeSet(lcInputInfoRow, InputInfoAttribute.Title))
                            RenderElementComponent(paComponentController, lcInputInfoRow, String.Empty);
                    }
                }
                else
                {
                    if (IsAttributeSet(lcInputInfoRow, InputInfoAttribute.HideBlock)) paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Appearance, "hidden");
                    paComponentController.AddElementType(ComponentController.ElementType.InputBlock);
                    paComponentController.AddAttribute(HtmlAttribute.Class, paPrimaryClass);
                    paComponentController.RenderBeginTag(HtmlTag.Div);

                    for (int lcCount = 0; lcCount < lcDataRow.Length; lcCount++)
                    {
                        lcInputInfoRow.Row = lcDataRow[lcCount];
                        if (IsAttributeSet(lcInputInfoRow, InputInfoAttribute.Title)) RenderTitle(paComponentController, lcInputInfoRow);
                        else
                        {
                            if ((paMetaDataRow != null) && (paMetaDataRow.HasPreloadedData)) lcActiveValue = paMetaDataRow.ActiveData.GetData(lcInputInfoRow.InputName, String.Empty);
                            else lcActiveValue =  ApplicationFrame.GetInstance().ActiveFormInfoManager.TranslateStringExStr(lcInputInfoRow.DefaultValue);
                            RenderRow(paComponentController, lcInputInfoRow, lcActiveValue);
                        }
                    }

                    paComponentController.RenderEndTag();
                }
            }
        }

        private void RenderTitle(ComponentController paComponentController, InputInfoRow lcInputInfoRow)
        {
            paComponentController.AddElementType(ComponentController.ElementType.InputTitle);
            paComponentController.AddAttribute(HtmlAttribute.Style, lcInputInfoRow.ElementCss);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(lcInputInfoRow.DefaultValue));
            paComponentController.RenderEndTag();
        }

        private void RenderLabel(ComponentController paComponentController, InputInfoRow lcInputInfoRow)
        {
            paComponentController.AddAttribute(HtmlAttribute.Style, lcInputInfoRow.LabelCss);
            paComponentController.AddElementType(ComponentController.ElementType.InputLabel);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(clLanguageManager.GetText(lcInputInfoRow.InputLabel));
            paComponentController.RenderEndTag();
        }

        private void RenderInputBoxStatic(ComponentController paComponentController, InputInfoRow lcInputInfoRow, String paActiveValue)
        {
            paComponentController.AddAttribute(HtmlAttribute.Style, lcInputInfoRow.InputCss);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, lcInputInfoRow.InputName.ToLower());
            paComponentController.AddElementType(ComponentController.ElementType.InputBox);
            paComponentController.RenderBeginTag(HtmlTag.Div);
            paComponentController.Write(paActiveValue);                
            paComponentController.RenderEndTag();
        }

        private void RenderInputBoxTextBox(ComponentController paComponentController, InputInfoRow paInputInfoRow, String paActiveValue, bool paMultiline)
        {
            String  lcInputType;

            if (paInputInfoRow.MaxLimit > 0) paComponentController.AddAttribute(HtmlAttribute.Maxlength, paInputInfoRow.MaxLimit.ToString());
            if (!paInputInfoRow.InputName.StartsWith(ctVirtualColumnPrefix))
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, paInputInfoRow.InputName.ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_InputMode, paInputInfoRow.InputMode.Trim());

            if (IsAttributeSet(paInputInfoRow, InputInfoAttribute.DelimitedList))
            {
                paActiveValue = paActiveValue.Replace(ctMultilineDelimiter, ctCR);
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Mode, "delimited");
            }

            if (IsAttributeSet(paInputInfoRow, InputInfoAttribute.Mandatory))
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Mandatory, "true");

            if (IsAttributeSet(paInputInfoRow, InputInfoAttribute.Identifier))
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_IdentifierColumn, "true");

            if (IsAttributeSet(paInputInfoRow, InputInfoAttribute.KeyField))
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_KeyField, "true");

            if (IsAttributeSet(paInputInfoRow, InputInfoAttribute.DataMirror))
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_MirrorColumn, paInputInfoRow.LinkColumn);

            if (IsAttributeSet(paInputInfoRow, InputInfoAttribute.Persist))
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Persist, "true");
                        
            if ((clForceReadOnlyMode) || (IsAttributeSet(paInputInfoRow, InputInfoAttribute.ReadOnly)))
                paComponentController.AddAttribute(HtmlAttribute.ReadOnly, "true");

            if (!String.IsNullOrEmpty(paInputInfoRow.QueryName))
            {
                if (paInputInfoRow.QueryName.Contains("."))
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_QueryName, paInputInfoRow.QueryName);
                else
                    paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_QueryName, paInputInfoRow.InputGroup + "." + paInputInfoRow.QueryName);
            }

            paComponentController.AddAttribute(HtmlAttribute.Style, paInputInfoRow.InputCss);
            paComponentController.AddAttribute(HtmlAttribute.Name, paInputInfoRow.InputName);
            paComponentController.AddElementType(ComponentController.ElementType.InputBox);
            
            if (!IsAttributeSet(paInputInfoRow, InputInfoAttribute.Password))
            {
                if (IsAttributeSet(paInputInfoRow, InputInfoAttribute.DynamicNumber)) paActiveValue = clLanguageManager.ConvertNumber(paActiveValue);
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_OriginalValue, paActiveValue);
                paComponentController.AddAttribute(HtmlAttribute.Value, paActiveValue);                
                lcInputType = "text";
            }
            else
            {
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_OriginalValue, General.Base64Encode(paActiveValue));
                lcInputType = "password";
            }

            if (paMultiline)
            {
                paComponentController.RenderBeginTag(HtmlTag.Textarea);
                paComponentController.Write(paActiveValue);
            }
            else
            {
                //if (IsAttributeSet(lcInputInfoRow, InputInfoAttribute.Password)) paComponentController.AddAttribute(HtmlAttribute.Type, "password");
                //else paComponentController.AddAttribute(HtmlAttribute.Type, "text");
                paComponentController.AddAttribute(HtmlAttribute.Type, lcInputType);
                paComponentController.RenderBeginTag(HtmlTag.Input);
            }            
            paComponentController.RenderEndTag();
        }

        private void RenderSelectBox(ComponentController paComponentController, InputInfoRow lcInputInfoRow, String paActiveValue)
        {            
            MetaDataBlock     lcOptionList;
            MetaDataElement   lcOptionItem;

            lcOptionList = (new MetaDataBlockCollection(lcInputInfoRow.AdditionalInfo))[0];

            if (lcOptionList != null)
            {
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_PluginMode, "sumoselect");
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, lcInputInfoRow.InputName.ToLower());
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_OriginalValue, paActiveValue);

                paComponentController.RenderBeginTag(HtmlTag.Select);

                for (int lcCount = 0; lcCount < lcOptionList.MetaDataElementCount; lcCount++)
                {
                    lcOptionItem = lcOptionList[lcCount];

                    if (String.Equals(lcOptionItem[0], paActiveValue, StringComparison.OrdinalIgnoreCase))
                        paComponentController.AddAttribute(HtmlAttribute.Selected, "true");

                    paComponentController.AddAttribute(HtmlAttribute.Value, lcOptionItem[0]);
                    paComponentController.RenderBeginTag(HtmlTag.Option);
                    paComponentController.Write(lcOptionItem.Name);
                    paComponentController.RenderEndTag();
                }

                paComponentController.RenderEndTag();
            }
        }

        protected void RenderSlideSelectionControl(ComponentController paComponentController, InputInfoRow paInputInfoRow, String paActiveValue)
        {              
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_LinkColumn, paInputInfoRow.LinkColumn.ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, paInputInfoRow.InputName.ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, paInputInfoRow.InputMode.ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDOpenSlide);
            paComponentController.AddElementType(ComponentController.ElementType.SlideSelectionControl);
            paComponentController.RenderBeginTag(HtmlTag.Div);
                        
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_OriginalValue, paActiveValue);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, paInputInfoRow.InputName.ToLower());

            if (IsAttributeSet(paInputInfoRow, InputInfoAttribute.Persist))
                paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Persist, "true");

            paComponentController.AddAttribute(HtmlAttribute.ReadOnly, "readonly");
            paComponentController.AddAttribute(HtmlAttribute.Value, paActiveValue);            
            paComponentController.AddAttribute(HtmlAttribute.Type, "text");
            paComponentController.RenderBeginTag(HtmlTag.Input);

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        protected void RenderImageSlideSelectionControl(ComponentController paComponentController, InputInfoRow paInputInfoRow, String paActiveValue)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_LinkColumn, paInputInfoRow.LinkColumn.ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, paInputInfoRow.InputName.ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, paInputInfoRow.InputMode.ToLower());
            paComponentController.AddElementType(ComponentController.ElementType.ImageSlideSelectionControl);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            //paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_OriginalValue, paActiveValue);
            //paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, lcInputInfoRow.InputName.ToLower());            
            //paComponentController.AddAttribute(HtmlAttribute.Value, paActiveValue);
            paComponentController.AddAttribute(HtmlAttribute.Src, paActiveValue);            
            paComponentController.RenderBeginTag(HtmlTag.Img);
            paComponentController.RenderEndTag();

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_OriginalValue, paActiveValue);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, paInputInfoRow.InputName.ToLower());
            paComponentController.AddAttribute(HtmlAttribute.ReadOnly, "readonly");
            paComponentController.AddAttribute(HtmlAttribute.Value, paActiveValue);
            paComponentController.AddAttribute(HtmlAttribute.Type, "text");
            paComponentController.RenderBeginTag(HtmlTag.Input);

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }

        protected void RenderColorSlideSelectionControl(ComponentController paComponentController, InputInfoRow paInputInfoRow, String paActiveValue)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_LinkColumn, paInputInfoRow.LinkColumn.ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, paInputInfoRow.InputName.ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, paInputInfoRow.InputMode.ToLower());
            paComponentController.AddElementType(ComponentController.ElementType.ColorSlideSelectionControl);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_OriginalValue, paActiveValue);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, paInputInfoRow.InputName.ToLower());
            paComponentController.AddAttribute(HtmlAttribute.ReadOnly, "readonly");
            paComponentController.AddAttribute(HtmlAttribute.Style, "background:" + paActiveValue + ";color:" + General.ContrastColor(paActiveValue));
            paComponentController.AddAttribute(HtmlAttribute.Value, paActiveValue);
            paComponentController.AddAttribute(HtmlAttribute.Type, "text");
            paComponentController.RenderBeginTag(HtmlTag.Input);

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }


        protected void RenderCalendarControl(ComponentController paComponentController, InputInfoRow paInputInfoRow, String paActiveValue)
        {
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Command, ctCMDOpenCalendar);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, paInputInfoRow.InputName.ToLower());
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_Type, paInputInfoRow.InputMode.ToLower());
            paComponentController.AddElementType(ComponentController.ElementType.CalendarControl);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_OriginalValue, paActiveValue);
            paComponentController.AddElementAttribute(ComponentController.ElementAttribute.ea_ColumnName, paInputInfoRow.InputName.ToLower());
            paComponentController.AddAttribute(HtmlAttribute.ReadOnly, "readonly");            
            paComponentController.AddAttribute(HtmlAttribute.Value, paActiveValue);
            paComponentController.AddAttribute(HtmlAttribute.Type, "text");
            paComponentController.RenderBeginTag(HtmlTag.Input);

            paComponentController.RenderEndTag();

            paComponentController.RenderEndTag();
        }      
     
        private void RenderElementComponent(ComponentController paComponentController, InputInfoRow paInputInfoRow, String paActiveValue)
        {
            ControlType lcControlType;

            lcControlType = General.ParseEnum<ControlType>(paInputInfoRow.ControlType, ControlType.None);

            switch (lcControlType)
            {
                case ControlType.Label: RenderInputBoxStatic(paComponentController, paInputInfoRow, paActiveValue); break;
                case ControlType.TextBox: RenderInputBoxTextBox(paComponentController, paInputInfoRow, paActiveValue, false); break;
                case ControlType.TextArea: RenderInputBoxTextBox(paComponentController, paInputInfoRow, paActiveValue, true); break;
                case ControlType.SelectBox: RenderSelectBox(paComponentController, paInputInfoRow, paActiveValue); break;
                case ControlType.SlideSelection: RenderSlideSelectionControl(paComponentController, paInputInfoRow, paActiveValue); break;
                case ControlType.ColorSlideSelection: RenderColorSlideSelectionControl(paComponentController, paInputInfoRow, paActiveValue); break;
                case ControlType.ImageSlideSelection: RenderImageSlideSelectionControl(paComponentController, paInputInfoRow, paActiveValue); break;
                case ControlType.Calendar: RenderCalendarControl(paComponentController, paInputInfoRow, paActiveValue); break;
                default: if (clCustomComponentRendererEvent != null) clCustomComponentRendererEvent(paComponentController, paInputInfoRow, paActiveValue); break;
            }
        }

        private void RenderRow(ComponentController paComponentController, InputInfoRow lcInputInfoRow, String paActiveValue)
        {
            if (IsAttributeSet(lcInputInfoRow, InputInfoAttribute.Hide)) paComponentController.AddStyle(CSSStyle.Display, "none");
            paComponentController.AddElementType(ComponentController.ElementType.InputRow);
            paComponentController.RenderBeginTag(HtmlTag.Div);

            RenderLabel(paComponentController, lcInputInfoRow);
            
            RenderElementComponent(paComponentController, lcInputInfoRow, paActiveValue);
            
            paComponentController.RenderEndTag();
        }
    }
}
