using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace CobraFoundation
{
    //**********************
    //* TableManager Class *
    //**********************
    public partial class TableManager 
    {
        //array index of Column Name and Column Type
        const int ctTableName = 0;
        const int ctDataTable = 1;

        public enum TableType : int
        {
            Invalid = -1,
            Session = 0,
            ServiceRequestLog = 1,            
            User = 2,
            Subscription =3,
            Subscriber = 4,
            AppManifest = 5,            
 
        };

        object[,] arTables = {                              
                              {"Admin_Session",null},
                              {"Track_ServiceRequestLog",null},                              
                              {"Admin_User",null},
                              {"Data_Subscription",null},
                              {"Data_Subscriber",null},
                              {"Data_AppManifest",null},                              
                             };

        private static TableManager clTableManager;

        //to make TableManager class as singleton
        public static TableManager GetInstance()
        {
            if (clTableManager == null) return (clTableManager = new TableManager());
            else return (clTableManager);
        }

        // Singleton Constructor.
        private TableManager() { }

        //Creating Tables
        private void CreateTable(TableType paTableType)
        {
            QueryClass lcQuery;

            lcQuery = new QueryClass(QueryClass.QueryType.GetSchema);
            lcQuery.ReplacePlaceHolder("$TABLENAME", arTables[(int)paTableType, ctTableName].ToString(), false);
            arTables[(int)paTableType, ctDataTable] = lcQuery.RunQuery();
        }

        public DataTable GetNewTable(TableType paTableType)
        {
            if (arTables[(int)paTableType, ctDataTable] == null) CreateTable(paTableType);
            return (((DataTable)arTables[(int)paTableType, ctDataTable]).Clone());
        }

        public DataRow InitializeRow(DataRow paDataRow)
        {
            foreach (DataColumn lcColumn in paDataRow.Table.Columns)
            {
                switch (lcColumn.DataType.ToString())
                {
                    case "System.String": paDataRow[lcColumn.ColumnName] = String.Empty; break;

                    case "System.Decimal": paDataRow[lcColumn.ColumnName] = 0M; break;

                    case "System.Single":
                    case "System.Double": paDataRow[lcColumn.ColumnName] = 0f; break;

                    case "System.Int32":
                    case "System.UInt32":
                    case "System.Int16":
                    case "System.UInt16": paDataRow[lcColumn.ColumnName] = 0; break;
                }
            }

            return (paDataRow);
        }

        public DataRow GetNewRow(TableType paTableType, bool paInitialize)
        {
            if (arTables[(int)paTableType, ctDataTable] == null) CreateTable(paTableType);
            if (paInitialize) return (InitializeRow(((DataTable)arTables[(int)paTableType, ctDataTable]).NewRow()));
            else return (((DataTable)arTables[(int)paTableType, ctDataTable]).NewRow());
        }

        public void SetDBNull(DataRow paDataRow, String paColumnName)
        {
            paDataRow[paColumnName] = System.DBNull.Value;
        }
    }

    public class RoutingRow
    {
        DataRow clDataRow;

        public RoutingRow(DataRow paDataRow)
        {
            clDataRow = paDataRow;
        }

        public DataRow Row
        {
            get { return (clDataRow); }
            set { clDataRow = value; }
        }

        public object this[String paColumnName]
        {
            get { return (clDataRow[paColumnName]); }
            set { clDataRow[paColumnName] = value; }
        }

        public bool IsRouteNameNull { get { return (clDataRow["RouteName"].GetType() == typeof(System.DBNull)); } }
        public bool IsRouteGroupNull { get { return (clDataRow["RouteGroup"].GetType() == typeof(System.DBNull)); } }
        public bool IsRouteOrderNull { get { return (clDataRow["RouteOrder"].GetType() == typeof(System.DBNull)); } }
        public bool IsUrlPatternNull { get { return (clDataRow["UrlPattern"].GetType() == typeof(System.DBNull)); } }
        public bool IsPhysicalFileNull { get { return (clDataRow["PhysicalFile"].GetType() == typeof(System.DBNull)); } }
        public bool IsRouteValueListNull { get { return (clDataRow["RouteValueList"].GetType() == typeof(System.DBNull)); } }

        public String RouteName
        {
            get { return (Convert.ToString(clDataRow["RouteName"])); }
            set { clDataRow["RouteName"] = value; }
        }

        public String RouteGroup
        {
            get { return (Convert.ToString(clDataRow["RouteGroup"])); }
            set { clDataRow["RouteGroup"] = value; }
        }

        public int RouteOrder
        {
            get { return (Convert.ToInt32(clDataRow["RouteOrder"])); }
            set { clDataRow["RouteOrder"] = value; }
        }

        public String UrlPattern
        {
            get { return (Convert.ToString(clDataRow["UrlPattern"])); }
            set { clDataRow["UrlPattern"] = value; }
        }

        public String PhysicalFile
        {
            get { return (Convert.ToString(clDataRow["PhysicalFile"])); }
            set { clDataRow["PhysicalFile"] = value; }
        }

        public String PhysicalFileMobile
        {
            get { return (Convert.ToString(clDataRow["PhysicalFileMobile"])); }
            set { clDataRow["PhysicalFileMobile"] = value; }
        }

        public String RouteValueList
        {
            get { return (Convert.ToString(clDataRow["RouteValueList"])); }
            set { clDataRow["RouteValueList"] = value; }
        }
    }

    public class VirtualPathRow
    {
        DataRow clDataRow;

        public VirtualPathRow(DataRow paDataRow)
        {
            clDataRow = paDataRow;
        }

        public DataRow Row
        {
            get { return (clDataRow); }
            set { clDataRow = value; }
        }

        public object this[String paColumnName]
        {
            get { return (clDataRow[paColumnName]); }
            set { clDataRow[paColumnName] = value; }
        }

        public bool IsVirtualPathNull { get { return (clDataRow["VirtualPath"].GetType() == typeof(System.DBNull)); } }
        public bool IsSubscriptionIDNull { get { return (clDataRow["SubScriptionID"].GetType() == typeof(System.DBNull)); } }
        public bool IsModeNull { get { return (clDataRow["Mode"].GetType() == typeof(System.DBNull)); } }        

        public String VirtualPath
        {
            get { return (Convert.ToString(clDataRow["VirtualPath"])); }
            set { clDataRow["VirtualPath"] = value; }
        }

        public String SubscriptionID
        {
            get { return (Convert.ToString(clDataRow["SubscriptionID"])); }
            set { clDataRow["SubscriptionID"] = value; }
        }

        public String Mode
        {
            get { return (Convert.ToString(clDataRow["Mode"])); }
            set { clDataRow["Mode"] = value; }
        }        
    }

    public class UrlRewriteRow
    {
        DataRow clDataRow;

        public UrlRewriteRow(DataRow paDataRow)
        {
            clDataRow = paDataRow;
        }

        public DataRow Row
        {
            get { return (clDataRow); }
            set { clDataRow = value; }
        }

        public object this[String paColumnName]
        {
            get { return (clDataRow[paColumnName]); }
            set { clDataRow[paColumnName] = value; }
        }

        public bool IsHostNameNull { get { return (clDataRow["HostName"].GetType() == typeof(System.DBNull)); } }
        public bool IsPathNull { get { return (clDataRow["Path"].GetType() == typeof(System.DBNull)); } }
        public bool IsGroupNameNull { get { return (clDataRow["GroupName"].GetType() == typeof(System.DBNull)); } }
        public bool IsRewriteLinkNull { get { return (clDataRow["RewriteLink"].GetType() == typeof(System.DBNull)); } }
        public bool IsClusterNameNull { get { return (clDataRow["ClusterName"].GetType() == typeof(System.DBNull)); } }
        public bool IsDebugModeNull { get { return (clDataRow["DebugMode"].GetType() == typeof(System.DBNull)); } }
        public bool IsAdditionalInfoNull { get { return (clDataRow["AdditionalInfo"].GetType() == typeof(System.DBNull)); } }

        public String HostName
        {
            get { return (Convert.ToString(clDataRow["HostName"])); }
            set { clDataRow["HostName"] = value; }
        }

        public String Path
        {
            get { return (Convert.ToString(clDataRow["Path"])); }
            set { clDataRow["Path"] = value; }
        }

        public String GroupName
        {
            get { return (Convert.ToString(clDataRow["GroupName"])); }
            set { clDataRow["GroupName"] = value; }
        }

        public String RewriteLink
        {
            get { return (Convert.ToString(clDataRow["RewriteLink"])); }
            set { clDataRow["RewriteLink"] = value; }
        }

        public String ClusterName
        {
            get { return (Convert.ToString(clDataRow["ClusterName"])); }
            set { clDataRow["ClusterName"] = value; }
        }

        public bool DebugMode
        {
            get { return (Convert.ToBoolean(clDataRow["DebugMode"])); }
            set { clDataRow["DebugMode"] = value; }
        }

        public String AdditionalInfo
        {
            get { return (Convert.ToString(clDataRow["AdditionalInfo"])); }
            set { clDataRow["AdditionalInfo"] = value; }
        }
    }

    public class FieldInfoRow
    {
        DataRow clDataRow;

        public FieldInfoRow(DataRow paDataRow)
        {
            clDataRow = paDataRow;
        }

        public DataRow Row
        {
            get { return (clDataRow); }
            set { clDataRow = value; }
        }

        public object this[String paString]
        {
            get { return (clDataRow[paString]); }
            set { clDataRow[paString] = value; }
        }

        // Null Checking.
        public bool IsEServiceIDNull { get { return (clDataRow["EServiceID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsEServiceTypeNull { get { return (clDataRow["EServiceType"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDataGroupNull { get { return (clDataRow["DataGroup"].GetType().ToString() == "System.DBNull"); } }
    //    public bool IsLanguageNull { get { return (clDataRow["Language"].GetType().ToString() == "System.DBNull"); } }
        public bool IsColumnNameNull { get { return (clDataRow["ColumnName"].GetType().ToString() == "System.DBNull"); } }                
        public bool IsDisplayNameNull { get { return (clDataRow["DisplayName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsPageIndexNull { get { return (clDataRow["PageIndex"].GetType().ToString() == "System.DBNull"); } }
        public bool IsRowIndexNull { get { return (clDataRow["RowIndex"].GetType().ToString() == "System.DBNull"); } }
        public bool IsColumnIndexNull { get { return (clDataRow["ColumnIndex"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsDefaultValueNull { get { return (clDataRow["DefaultValue"].GetType().ToString() == "System.DBNull"); } }
        public bool IsLinkColumnNull { get { return (clDataRow["LinkColumn"].GetType().ToString() == "System.DBNull"); } }
        public bool IsColumnAttributeNull { get { return (clDataRow["ColumnAttribute"].GetType().ToString() == "System.DBNull"); } }


        public String EServiceID
        {
            get { return (Convert.ToString(clDataRow["EServiceID"])); }
            set { clDataRow["EServiceID"] = value; }
        }

        public String EServiceType
        {
            get { return (Convert.ToString(clDataRow["EServiceType"])); }
            set { clDataRow["EServiceType"] = value; }
        }

        public String DataGroup
        {
            get { return (Convert.ToString(clDataRow["DataGroup"])); }
            set { clDataRow["DataGroup"] = value; }
        }

        //public String Language
        //{
        //    get { return (Convert.ToString(clDataRow["Language"])); }
        //    set { clDataRow["Language"] = value; }
        //}

        public String ColumnName
        {
            get { return (Convert.ToString(clDataRow["ColumnName"])); }
            set { clDataRow["ColumnName"] = value; }
        }        

        public String DisplayName
        {
            get { return (Convert.ToString(clDataRow["DisplayName"])); }
            set { clDataRow["DisplayName"] = value; }
        }

        public int PageIndex
        {
            get { return (Convert.ToInt32(clDataRow["PageIndex"])); }
            set { clDataRow["PageIndex"] = value; }
        }

        public int RowIndex
        {
            get { return (Convert.ToInt32(clDataRow["RowIndex"])); }
            set { clDataRow["RowIndex"] = value; }
        }

        public int ColumnIndex
        {
            get { return (Convert.ToInt32(clDataRow["ColumnIndex"])); }
            set { clDataRow["ColumnIndex"] = value; }
        }              

        public String DefaultValue
        {
            get { return (Convert.ToString(clDataRow["DefaultValue"])); }
            set { clDataRow["DefaultValue"] = value; }
        }

        public String LinkColumn
        {
            get { return (Convert.ToString(clDataRow["LinkColumn"])); }
            set { clDataRow["LinkColumn"] = value; }
        }

        public String ColumnAttribute
        {
            get { return (Convert.ToString(clDataRow["ColumnAttribute"])); }
            set { clDataRow["ColumnAttribute"] = value; }
        }        
    }

    public class InputInfoRow
    {
        DataRow clDataRow;

        public InputInfoRow(DataRow paDataRow)
        {
            clDataRow = paDataRow;
        }

        public DataRow Row
        {
            get { return (clDataRow); }
            set { clDataRow = value; }
        }

        public object this[String paString]
        {
            get { return (clDataRow[paString]); }
            set { clDataRow[paString] = value; }
        }

        // Null Checking.
        public bool IsEServiceIDNull { get { return (clDataRow["EServiceID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsInputGroupNull { get { return (clDataRow["InputGroup"].GetType().ToString() == "System.DBNull"); } }
        public bool IsSubGroupNull { get { return (clDataRow["SubGroup"].GetType().ToString() == "System.DBNull"); } }        
         // public bool IsLanguageNull { get { return (clDataRow["Language"].GetType().ToString() == "System.DBNull"); } }
        public bool IsInputNameNull { get { return (clDataRow["InputName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsInputLabelNull { get { return (clDataRow["InputLabel"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDataTypeNull { get { return (clDataRow["DataType"].GetType().ToString() == "System.DBNull"); } }
        public bool IsFormatStringNull { get { return (clDataRow["FormatString"].GetType().ToString() == "System.DBNull"); } }
        public bool IsControlTypeNull { get { return (clDataRow["ControlType"].GetType().ToString() == "System.DBNull"); } }
        public bool IsInputModeNull { get { return (clDataRow["InputMode"].GetType().ToString() == "System.DBNull"); } }
        public bool IsPageIndexNull { get { return (clDataRow["PageIndex"].GetType().ToString() == "System.DBNull"); } }
        public bool IsRowIndexNull { get { return (clDataRow["RowIndex"].GetType().ToString() == "System.DBNull"); } }
        public bool IsColumnIndexNull { get { return (clDataRow["ColumnIndex"].GetType().ToString() == "System.DBNull"); } }
        public bool IsElementCssNull { get { return (clDataRow["ElementCss"].GetType().ToString() == "System.DBNull"); } }
        public bool IsInputCssNull { get { return (clDataRow["InputCss"].GetType().ToString() == "System.DBNull"); } }
        public bool IsLabelCssNull { get { return (clDataRow["LabelCss"].GetType().ToString() == "System.DBNull"); } }
        public bool IsMaxLimitNull { get { return (clDataRow["MaxLimit"].GetType().ToString() == "System.DBNull"); } }
        public bool IsCharFilterNull { get { return (clDataRow["CharFilter"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDefaultValueNull { get { return (clDataRow["DefaultValue"].GetType().ToString() == "System.DBNull"); } }
        public bool IsAttributeNull { get { return (clDataRow["Attribute"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsLinkColumnNull { get { return (clDataRow["LinkColumn"].GetType().ToString() == "System.DBNull"); } }
        public bool IsAdditionalInfoNull { get { return (clDataRow["AdditionalInfo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsQueryNameNull { get { return (clDataRow["QueryName"].GetType().ToString() == "System.DBNull"); } }
        
        
        public String EServiceID
        {
            get { return (Convert.ToString(clDataRow["EServiceID"])); }
            set { clDataRow["EServiceID"] = value; }
        }

        public String InputGroup
        {
            get { return (Convert.ToString(clDataRow["InputGroup"])); }
            set { clDataRow["InputGroup"] = value; }
        }

        public String SubGroup
        {
            get { return (Convert.ToString(clDataRow["SubGroup"])); }
            set { clDataRow["SubGroup"] = value; }
        }

        //public String Language
        //{
        //    get { return (Convert.ToString(clDataRow["Language"])); }
        //    set { clDataRow["Language"] = value; }
        //}

        public String InputName
        {
            get { return (Convert.ToString(clDataRow["InputName"])); }
            set { clDataRow["InputName"] = value; }
        }

        public String InputLabel
        {
            get { return (Convert.ToString(clDataRow["InputLabel"])); }
            set { clDataRow["InputLabel"] = value; }
        }

        public String DataType
        {
            get { return (Convert.ToString(clDataRow["DataType"])); }
            set { clDataRow["DataType"] = value; }
        }

        public String FormatString
        {
            get { return (Convert.ToString(clDataRow["FormatString"])); }
            set { clDataRow["FormatString"] = value; }
        }

        public String ControlType
        {
            get { return (Convert.ToString(clDataRow["ControlType"])); }
            set { clDataRow["ControlType"] = value; }
        }

        public String InputMode
        {
            get { return (Convert.ToString(clDataRow["InputMode"])); }
            set { clDataRow["InputMode"] = value; }
        }

        public int PageIndex
        {
            get { return (Convert.ToInt32(clDataRow["PageIndex"])); }
            set { clDataRow["PageIndex"] = value; }
        }

        public int RowIndex
        {
            get { return (Convert.ToInt32(clDataRow["RowIndex"])); }
            set { clDataRow["RowIndex"] = value; }
        }

        public int ColumnIndex
        {
            get { return (Convert.ToInt32(clDataRow["ColumnIndex"])); }
            set { clDataRow["ColumnIndex"] = value; }
        }

        public String ElementCss
        {
            get { return (Convert.ToString(clDataRow["ElementCss"])); }
            set { clDataRow["ElementCss"] = value; }
        }

        public String InputCss
        {
            get { return (Convert.ToString(clDataRow["InputCss"])); }
            set { clDataRow["InputCss"] = value; }
        }

        public String LabelCss
        {
            get { return (Convert.ToString(clDataRow["LabelCss"])); }
            set { clDataRow["LabelCss"] = value; }
        }

        public int MaxLimit
        {
            get { return (Convert.ToInt32(clDataRow["MaxLimit"])); }
            set { clDataRow["MaxLimit"] = value; }
        }

        public String CharFilter
        {
            get { return (Convert.ToString(clDataRow["CharFilter"])); }
            set { clDataRow["CharFilter"] = value; }
        }

        public String DefaultValue
        {
            get { return (Convert.ToString(clDataRow["DefaultValue"])); }
            set { clDataRow["DefaultValue"] = value; }
        }

        public String Attribute
        {
            get { return (Convert.ToString(clDataRow["Attribute"])); }
            set { clDataRow["Attribute"] = value; }
        }

        public String LinkColumn
        {
            get { return (Convert.ToString(clDataRow["LinkColumn"])); }
            set { clDataRow["LinkColumn"] = value; }
        }

        public String AdditionalInfo
        {
            get { return (Convert.ToString(clDataRow["AdditionalInfo"])); }
            set { clDataRow["AdditionalInfo"] = value; }
        }

        public String QueryName
        {
            get { return (Convert.ToString(clDataRow["QueryName"])); }
            set { clDataRow["QueryName"] = value; }
        }
    }

    public class FormInfoRow
    {
        DataRow clDataRow;

        public FormInfoRow(DataRow paDataRow)
        {
            clDataRow = paDataRow;
        }

        public DataRow Row
        {
            get { return (clDataRow); }
            set { clDataRow = value; }
        }

        public object this[String paString]
        {
            get { return (clDataRow[paString]); }
            set { clDataRow[paString] = value; }
        }

        // Null Checking.
        public bool IsEServiceIDNull { get { return (clDataRow["EServiceID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsFormNameNull { get { return (clDataRow["FormName"].GetType().ToString() == "System.DBNull"); } }     
        public bool IsDataGroupNull { get { return (clDataRow["DataGroup"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsConnectionModeNull { get { return (clDataRow["ConnectionMode"].GetType().ToString() == "System.DBNull"); } }
        public bool IsRenderModeNull { get { return (clDataRow["RenderMode"].GetType().ToString() == "System.DBNull"); } }
        public bool IsFormAttributeNull { get { return (clDataRow["FormAttribute"].GetType().ToString() == "System.DBNull"); } }
        //public bool IsTitleTextNull { get { return (clDataRow["TitleText"].GetType().ToString() == "System.DBNull"); } }
        public bool IsToolBarNull { get { return (clDataRow["ToolBar"].GetType().ToString() == "System.DBNull"); } }
        public bool IsToolBarConfigNull { get { return (clDataRow["ToolBarConfig"].GetType().ToString() == "System.DBNull"); } }
        public bool IsFormWidthNull { get { return (clDataRow["FormWidth"].GetType().ToString() == "System.DBNull"); } }
        public bool IsFormHeightNull { get { return (clDataRow["FormHeight"].GetType().ToString() == "System.DBNull"); } }
        public bool IsContainerHeightNull { get { return (clDataRow["ContainerHeight"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsFormCSSFileNull { get { return (clDataRow["FormCSSFile"].GetType().ToString() == "System.DBNull"); } }
        public bool IsFormCSSClassNull { get { return (clDataRow["FormCSSClass"].GetType().ToString() == "System.DBNull"); } }
        public bool IsFormCSSNull { get { return (clDataRow["FormCSS"].GetType().ToString() == "System.DBNull"); } }
        public bool IsTitleCSSNull { get { return (clDataRow["TitleCSS"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsContainerCSSNull { get { return (clDataRow["ContainerCSS"].GetType().ToString() == "System.DBNull"); } }
        public bool IsAdditionalInfoNull { get { return (clDataRow["AdditionalInfo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDefaultParametersNull { get { return (clDataRow["DefaultParameters"].GetType().ToString() == "System.DBNull"); } }
        public bool IsRetrieveQueryNull { get { return (clDataRow["RetrieveQuery"].GetType().ToString() == "System.DBNull"); } }
        public bool IsUpdateQueryNull { get { return (clDataRow["UpdateQuery"].GetType().ToString() == "System.DBNull"); } }        
        
        public String EServiceID
        {
            get { return (Convert.ToString(clDataRow["EServiceID"])); }
            set { clDataRow["EServiceID"] = value; }
        }

        public String FormName
        {
            get { return (Convert.ToString(clDataRow["FormName"])); }
            set { clDataRow["FormName"] = value; }
        }

        public String DataGroup
        {
            get { return (Convert.ToString(clDataRow["DataGroup"])); }
            set { clDataRow["DataGroup"] = value; }
        }

        public String ConnectionMode
        {
            get { return (Convert.ToString(clDataRow["ConnectionMode"])); }
            set { clDataRow["ConnectionMode"] = value; }
        }

        public String RenderMode
        {
            get { return (Convert.ToString(clDataRow["RenderMode"])); }
            set { clDataRow["RenderMode"] = value; }
        }

        public String FormAttribute
        {
            get { return (Convert.ToString(clDataRow["FormAttribute"])); }
            set { clDataRow["FormAttribute"] = value; }
        }

        //public String TitleText
        //{
        //    get { return (Convert.ToString(clDataRow["TitleText"])); }
        //    set { clDataRow["TitleText"] = value; }
        //}

        public String ToolBar
        {
            get { return (Convert.ToString(clDataRow["ToolBar"])); }
            set { clDataRow["ToolBar"] = value; }
        }

        public String ToolBarConfig
        {
            get { return (Convert.ToString(clDataRow["ToolBarConfig"])); }
            set { clDataRow["ToolBarConfig"] = value; }
        }

        public float FormWidth
        {
            get { return (Convert.ToSingle(clDataRow["FormWidth"])); }
            set { clDataRow["FormWidth"] = value; }
        }

        public float FormHeight
        {
            get { return (Convert.ToSingle(clDataRow["FormHeight"])); }
            set { clDataRow["FormHeight"] = value; }
        }

        public float ContainerHeight
        {
            get { return (Convert.ToSingle(clDataRow["ContainerHeight"])); }
            set { clDataRow["ContainerHeight"] = value; }
        }                

        public String FormCSSFile
        {
            get { return (Convert.ToString(clDataRow["FormCSSFile"])); }
            set { clDataRow["FormCSSFile"] = value; }
        }

        public String FormCSSClass
        {
            get { return (Convert.ToString(clDataRow["FormCSSClass"])); }
            set { clDataRow["FormCSSClass"] = value; }
        }

        public String FormCSS
        {
            get { return (Convert.ToString(clDataRow["FormCSS"])); }
            set { clDataRow["FormCSS"] = value; }
        }

        public String TitleCSS
        {
            get { return (Convert.ToString(clDataRow["TitleCSS"])); }
            set { clDataRow["TitleCSS"] = value; }
        }

        public String ContainerCSS
        {
            get { return (Convert.ToString(clDataRow["ContainerCSS"])); }
            set { clDataRow["ContainerCSS"] = value; }
        }

        public String AdditionalInfo
        {
            get { return (Convert.ToString(clDataRow["AdditionalInfo"])); }
            set { clDataRow["AdditionalInfo"] = value; }
        }

        public String DefaultParameters
        {
            get { return (Convert.ToString(clDataRow["DefaultParameters"])); }
            set { clDataRow["DefaultParameters"] = value; }
        }        

        public String RetrieveQuery
        {
            get { return (Convert.ToString(clDataRow["RetrieveQuery"])); }
            set { clDataRow["RetrieveQuery"] = value; }
        }

        public String UpdateQuery
        {
            get { return (Convert.ToString(clDataRow["UpdateQuery"])); }
            set { clDataRow["UpdateQuery"] = value; }
        }
    }

    public class EServiceRow
    {
        DataRow clDataRow;

        public EServiceRow(DataRow paDataRow)
        {
            clDataRow = paDataRow;
        }

        public DataRow Row
        {
            get { return (clDataRow); }
            set { clDataRow = value; }
        }

        public object this[String paString]
        {
            get { return (clDataRow[paString]); }
            set { clDataRow[paString] = value; }
        }

        // Null Checking.
        public bool IsEServiceIDNull { get { return (clDataRow["EServiceID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsServiceNameNull { get { return (clDataRow["ServiceName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsServiceDescriptionNull { get { return (clDataRow["ServiceDescription"].GetType().ToString() == "System.DBNull"); } }
        public bool IsAttributeNull { get { return (clDataRow["Attribute"].GetType().ToString() == "System.DBNull"); } }  
        public bool IsOrganizationIDNull { get { return (clDataRow["OrganizationID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsOrganizationNameNull { get { return (clDataRow["OrganizationName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsFrontEndLogInFormNameNull { get { return (clDataRow["FrondEndLoginFormName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsFrontEndFormNameNull { get { return (clDataRow["FrontEndFormName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsBackEndLogInFormNameNull { get { return (clDataRow["BackEndLoginFormName"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsBackEndFormNameNull { get { return (clDataRow["BackEndFormName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsRecoveryFormNameNull { get { return (clDataRow["RecoveryFormName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsErrorFormNameNull { get { return (clDataRow["ErrorFormName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDBParametersNull { get { return (clDataRow["DBParameters"].GetType().ToString() == "System.DBNull"); } }
        public bool IsFTPParametersNull { get { return (clDataRow["FTPParameters"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsEServiceRoleNull { get { return (clDataRow["EServiceRole"].GetType().ToString() == "System.DBNull"); } }        
        
        public String EServiceID
        {
            get { return (Convert.ToString(clDataRow["EServiceID"])); }
            set { clDataRow["EServiceID"] = value; }
        }

        public String ServiceName
        {
            get { return (Convert.ToString(clDataRow["ServiceName"])); }
            set { clDataRow["ServiceName"] = value; }
        }

        public String ServiceDescription
        {
            get { return (Convert.ToString(clDataRow["ServiceDescription"])); }
            set { clDataRow["ServiceDescription"] = value; }
        }

        public String Attribute
        {
            get { return (Convert.ToString(clDataRow["Attribute"])); }
            set { clDataRow["Attribute"] = value; }
        }
        
        public String OrganizationID
        {
            get { return (Convert.ToString(clDataRow["OrganizationID"])); }
            set { clDataRow["OrganizationID"] = value; }
        }

        public String OrganizationName
        {
            get { return (Convert.ToString(clDataRow["OrganizationName"])); }
            set { clDataRow["OrganizationName"] = value; }
        }

        public String FrontEndLogInFormName
        {
            get { return (Convert.ToString(clDataRow["FrontEndLogInFormName"])); }
            set { clDataRow["FrontEndLogInFormName"] = value; }
        }

        public String FrontEndFormName
        {
            get { return (Convert.ToString(clDataRow["FrontEndFormName"])); }
            set { clDataRow["FrontEndFormName"] = value; }
        }

        public String BackEndLogInFormName
        {
            get { return (Convert.ToString(clDataRow["BackEndLogInFormName"])); }
            set { clDataRow["BackEndLogInFormName"] = value; }
        }

        public String BackEndFormName
        {
            get { return (Convert.ToString(clDataRow["BackEndFormName"])); }
            set { clDataRow["BackEndFormName"] = value; }
        }

        public String RecoveryFormName
        {
            get { return (Convert.ToString(clDataRow["RecoveryFormName"])); }
            set { clDataRow["RecoveryFormName"] = value; }
        }

        public String ErrorFormName
        {
            get { return (Convert.ToString(clDataRow["ErrorFormName"])); }
            set { clDataRow["ErrorFormName"] = value; }
        }        

        public String DBParameters
        {
            get { return (Convert.ToString(clDataRow["DBParameters"])); }
            set { clDataRow["DBParameters"] = value; }
        }

        public String FTPParameters
        {
            get { return (Convert.ToString(clDataRow["FTPParameters"])); }
            set { clDataRow["FTPParameters"] = value; }
        }

        public String EServiceRole
        {
            get { return (Convert.ToString(clDataRow["EServiceRole"])); }
            set { clDataRow["EServiceRole"] = value; }
        }
    }

    public class SubscriptionRow
    {
        DataRow clDataRow;

        public SubscriptionRow  (DataRow paDataRow)
        {
            clDataRow = paDataRow;
        }

        public DataRow Row
        {
            get { return (clDataRow); }
            set { clDataRow = value; }
        }

        public object this[String paString]
        {
            get { return (clDataRow[paString]); }
            set { clDataRow[paString] = value; }
        }

        // Null Checking.
        public bool IsSubscriptionIDNull { get { return (clDataRow["SubscriptionID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsEServiceIDNull { get { return (clDataRow["EServiceID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsFrontEndPathNull { get { return (clDataRow["FrontEndPath"].GetType().ToString() == "System.DBNull"); } }
        public bool IsSubscriberNameNull { get { return (clDataRow["SubscriberName"].GetType().ToString() == "System.DBNull"); } }
        // public bool IsSubscriptionTitleNull { get { return (clDataRow["SubscriptionTitle"].GetType().ToString() == "System.DBNull"); } }
        public bool IsTypeNull { get { return (clDataRow["Type"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsStatusNull { get { return (clDataRow["Status"].GetType().ToString() == "System.DBNull"); } }        
        // public bool IsWallPaperNull { get { return (clDataRow["WallPaper"].GetType().ToString() == "System.DBNull"); } }
        public bool IsActivationDateNull { get { return (clDataRow["ActivationDate"].GetType().ToString() == "System.DBNull"); } }
        public bool IsExpiryDateNull { get { return (clDataRow["ExpiryDate"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsSubscriptionRoleNull { get { return (clDataRow["SubscriptionRole"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsServiceRequestTokenNull { get { return (clDataRow["ServiceRequestToken"].GetType().ToString() == "System.DBNull"); } }        

        public String SubscriptionID
        {
            get { return (Convert.ToString(clDataRow["SubscriptionID"])); }
            set { clDataRow["SubscriptionID"] = value; }
        }

        public String EServiceID
        {
            get { return (Convert.ToString(clDataRow["EServiceID"])); }
            set { clDataRow["EServiceID"] = value; }
        }

        public String FrontEndPath
        {
            get { return (Convert.ToString(clDataRow["FrontEndPath"])); }
            set { clDataRow["FrontEndPath"] = value; }
        }
        
        public String SubscriberName
        {
            get { return (Convert.ToString(clDataRow["SubscriberName"])); }
            set { clDataRow["SubscriberName"] = value; }
        }

        //public String SubscriptionTitle
        //{
        //    get { return (Convert.ToString(clDataRow["SubscriptionTitle"])); }
        //    set { clDataRow["SubscriptionTitle"] = value; }
        //}

        public String Type
        {
            get { return (Convert.ToString(clDataRow["Type"])); }
            set { clDataRow["Type"] = value; }
        }

        public String Status
        {
            get { return (Convert.ToString(clDataRow["Status"])); }
            set { clDataRow["Status"] = value; }
        }

        //public String WallPaper
        //{
        //    get { return (Convert.ToString(clDataRow["WallPaper"])); }
        //    set { clDataRow["WallPaper"] = value; }
        //}

        public DateTime ActivationDate
        {
            get { return (Convert.ToDateTime(clDataRow["ActivationDate"])); }
            set { clDataRow["ActivationDate"] = value; }
        }

        public DateTime ExpiryDate
        {
            get { return (Convert.ToDateTime(clDataRow["ExpiryDate"])); }
            set { clDataRow["ExpiryDate"] = value; }
        }

        public String SubscriptionRole
        {
            get { return (Convert.ToString(clDataRow["SubscriptionRole"])); }
            set { clDataRow["SubscriptionRole"] = value; }
        }

        public String SubscriptionAttribute
        {
            get { return (Convert.ToString(clDataRow["SubscriptionAttribute"])); }
            set { clDataRow["SubscriptionAttribute"] = value; }
        }

        public String ServiceRequestToken
        {
            get { return (Convert.ToString(clDataRow["ServiceRequestToken"])); }
            set { clDataRow["ServiceRequestToken"] = value; }
        }        
    }

    public class ComponentSubscriptionRow
    {
        DataRow clDataRow;

        public ComponentSubscriptionRow(DataRow paDataRow)
        {
            clDataRow = paDataRow;
        }

        public DataRow Row
        {
            get { return (clDataRow); }
            set { clDataRow = value; }
        }

        public object this[String paString]
        {
            get { return (clDataRow[paString]); }
            set { clDataRow[paString] = value; }
        }

        // Null Checking.
        public bool IsTransactionIDNull { get { return (clDataRow["TransactionID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsServiceKeyNull { get { return (clDataRow["ServiceKey"].GetType().ToString() == "System.DBNull"); } }
        public bool IsStatusNull { get { return (clDataRow["Status"].GetType().ToString() == "System.DBNull"); } }
        public bool IsComponentNameNull { get { return (clDataRow["ComponentName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsClassificationCodeNull { get { return (clDataRow["ClassificationCode"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsSortPriorityeNull { get { return (clDataRow["SortPriority"].GetType().ToString() == "System.DBNull"); } }
        public bool IsActivationDateNull { get { return (clDataRow["ActivationDate"].GetType().ToString() == "System.DBNull"); } }
        public bool IsExpiryDateNull { get { return (clDataRow["ExpiryDate"].GetType().ToString() == "System.DBNull"); } }
        public bool IsPaymentReferenceNull { get { return (clDataRow["PaymentReference"].GetType().ToString() == "System.DBNull"); } }
        public bool IsPaymentDateNull { get { return (clDataRow["PaymentDate"].GetType().ToString() == "System.DBNull"); } }
        
        public int TransactionID
        {
            get { return (Convert.ToInt32(clDataRow["TransactionID"])); }
            set { clDataRow["TransactionID"] = value; }
        }

        public String ServiceKey
        {
            get { return (Convert.ToString(clDataRow["SubscriptionID"])); }
            set { clDataRow["SubscriptionID"] = value; }
        }

        public String Status
        {
            get { return (Convert.ToString(clDataRow["Status"])); }
            set { clDataRow["Status"] = value; }
        }

        public String ComponentName
        {
            get { return (Convert.ToString(clDataRow["AddOnName"])); }
            set { clDataRow["AddOnName"] = value; }
        }

        public int ClassificationCode
        {
            get { return (Convert.ToInt32(clDataRow["ClassificationCode"])); }
            set { clDataRow["ClassificationCode"] = value; }
        }

        public int SortPriority
        {
            get { return (Convert.ToInt32(clDataRow["SortPriority"])); }
            set { clDataRow["SortPriority"] = value; }
        }

        public DateTime ActivationDate
        {
            get { return (Convert.ToDateTime(clDataRow["ActivationDate"])); }
            set { clDataRow["ActivationDate"] = value; }
        }

        public DateTime ExpiryDate
        {
            get { return (Convert.ToDateTime(clDataRow["ExpiryDate"])); }
            set { clDataRow["ExpiryDate"] = value; }
        }

        public String PaymentReference
        {
            get { return (Convert.ToString(clDataRow["PaymentReference"])); }
            set { clDataRow["PaymentReference"] = value; }
        }

        public DateTime PaymentDate
        {
            get { return (Convert.ToDateTime(clDataRow["PaymentDate"])); }
            set { clDataRow["PaymentDate"] = value; }
        }
    }

    public class AppManifestRow
    {
        DataRow clDataRow;

        public AppManifestRow(DataRow paDataRow)
        {
            clDataRow = paDataRow;
        }

        public DataRow Row
        {
            get { return (clDataRow); }
            set { clDataRow = value; }
        }

        public object this[String paString]
        {
            get { return (clDataRow[paString]); }
            set { clDataRow[paString] = value; }
        }

        // Null Checking.
        public bool IsSubscriptionIDNull { get { return (clDataRow["SubscriptionID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsAppNameNull { get { return (clDataRow["AppName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsShortNameNull { get { return (clDataRow["ShortName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsBackgroundColorNull { get { return (clDataRow["BackgroundColor"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsDescriptionNull { get { return (clDataRow["Description"].GetType().ToString() == "System.DBNull"); } }
        public bool IsFrontEndIconNull { get { return (clDataRow["FrontEndIcon"].GetType().ToString() == "System.DBNull"); } }
        public bool IsBackEndIconNull { get { return (clDataRow["BackEndIcon"].GetType().ToString() == "System.DBNull"); } }

        public String SubscriptionID
        {
            get { return (Convert.ToString(clDataRow["SubscriptionID"])); }
            set { clDataRow["SubscriptionID"] = value; }
        }

        public String AppName
        {
            get { return (Convert.ToString(clDataRow["AppName"])); }
            set { clDataRow["AppName"] = value; }
        }

        public String ShortName
        {
            get { return (Convert.ToString(clDataRow["ShortName"])); }
            set { clDataRow["ShortName"] = value; }
        }

        public String BackgroundColor
        {
            get { return (Convert.ToString(clDataRow["BackgroundColor"])); }
            set { clDataRow["BackgroundColor"] = value; }
        }

        public String Description
        {
            get { return (Convert.ToString(clDataRow["Description"])); }
            set { clDataRow["Description"] = value; }
        }

        public String FrontEndIcon
        {
            get { return (Convert.ToString(clDataRow["FrontEndIcon"])); }
            set { clDataRow["FrontEndIcon"] = value; }
        }

        public String BackEndIcon
        {
            get { return (Convert.ToString(clDataRow["BackEndIcon"])); }
            set { clDataRow["BackEndIcon"] = value; }
        }
    }

    public class DynamicQueryRow
    {
        DataRow clDataRow;

        public DynamicQueryRow(DataRow paDataRow)
        {
            clDataRow = paDataRow;
        }

        public DataRow Row
        {
            get { return (clDataRow); }
            set { clDataRow = value; }
        }

        public object this[String paString]
        {
            get { return (clDataRow[paString]); }
            set { clDataRow[paString] = value; }
        }

        // Null Checking.
        public bool IsQueryGroupNull { get { return (clDataRow["QueryGroup"].GetType().ToString() == "System.DBNull"); } }
        public bool IsQueryNameNull { get { return (clDataRow["QueryName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsQueryTypeNull { get { return (clDataRow["QueryType"].GetType().ToString() == "System.DBNull"); } }
        public bool IsAccessTypeNull { get { return (clDataRow["AccessType"].GetType().ToString() == "System.DBNull"); } }
        public bool IsConnectionModeNull { get { return (clDataRow["ConnectionMode"].GetType().ToString() == "System.DBNull"); } }
        public bool IsQueryNull { get { return (clDataRow["Query"].GetType().ToString() == "System.DBNull"); } }
     
        public String QueryGroup
        {
            get { return (Convert.ToString(clDataRow["QueryGroup"])); }
            set { clDataRow["QueryGroup"] = value; }
        }

        public String QueryName
        {
            get { return (Convert.ToString(clDataRow["QueryName"])); }
            set { clDataRow["QueryName"] = value; }
        }

        public String QueryType
        {
            get { return (Convert.ToString(clDataRow["QueryType"])); }
            set { clDataRow["QueryType"] = value; }
        }

        public String AccessType
        {
            get { return (Convert.ToString(clDataRow["AccessType"])); }
            set { clDataRow["AccessType"] = value; }
        }

        public String ConnectionMode
        {
            get { return (Convert.ToString(clDataRow["ConnectionMode"])); }
            set { clDataRow["ConnectionMode"] = value; }
        }

        public String Query
        {
            get { return (Convert.ToString(clDataRow["Query"])); }
            set { clDataRow["Query"] = value; }
        }
    }

    public class UserRow
    {
        DataRow clDataRow;

        public UserRow(DataRow paDataRow)
        {
            clDataRow = paDataRow;
        }

        public DataRow Row
        {
            get { return (clDataRow); }
            set { clDataRow = value; }
        }

        public object this[String paString]
        {
            get { return (clDataRow[paString]); }
            set { clDataRow[paString] = value; }
        }

        // Null Checking.
        public bool IsSubscriptionIDNull { get { return (clDataRow["SubscriptionID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsUserIDNull { get { return (clDataRow["UserID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsLoginIDNull { get { return (clDataRow["LoginID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsTypeNull { get { return (clDataRow["Type"].GetType().ToString() == "System.DBNull"); } }
        public bool IsStatusNull { get { return (clDataRow["Status"].GetType().ToString() == "System.DBNull"); } }
        public bool IsUserRoleNull { get { return (clDataRow["UserRole"].GetType().ToString() == "System.DBNull"); } }
        public bool IsCustomSettingNull { get { return (clDataRow["CustomSetting"].GetType().ToString() == "System.DBNull"); } }
        public bool IsPasswordNull { get { return (clDataRow["Password"].GetType().ToString() == "System.DBNull"); } }
        public bool IsNameNull { get { return (clDataRow["Name"].GetType().ToString() == "System.DBNull"); } }
        public bool IsEmailAddressNull { get { return (clDataRow["EmailAddress"].GetType().ToString() == "System.DBNull"); } }
        public bool IsContactNoNull { get { return (clDataRow["ContactNo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDateOfBirthNull { get { return (clDataRow["DateOfBirth"].GetType().ToString() == "System.DBNull"); } }
        public bool IsGenderNull { get { return (clDataRow["Gender"].GetType().ToString() == "System.DBNull"); } }
        public bool IsBuildingNoNull { get { return (clDataRow["BuildingNo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsFloorNull { get { return (clDataRow["Floor"].GetType().ToString() == "System.DBNull"); } }
        public bool IsRoomNoNull { get { return (clDataRow["RoomNo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsStreetNull { get { return (clDataRow["Street"].GetType().ToString() == "System.DBNull"); } }
        public bool IsQuarterNull { get { return (clDataRow["Quarter"].GetType().ToString() == "System.DBNull"); } }
        public bool IsAddressInfoNull { get { return (clDataRow["AddressInfo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsTownshipNull { get { return (clDataRow["Township"].GetType().ToString() == "System.DBNull"); } }
        public bool IsCityNull { get { return (clDataRow["City"].GetType().ToString() == "System.DBNull"); } }
        public bool IsCountryNull { get { return (clDataRow["Country"].GetType().ToString() == "System.DBNull"); } }
        public bool IsPostalNull { get { return (clDataRow["Postal"].GetType().ToString() == "System.DBNull"); } }
        public bool IsAdditionalInfoNull { get { return (clDataRow["AdditionalInfo"].GetType().ToString() == "System.DBNull"); } }

        public String SubscriptionID
        {
            get { return (Convert.ToString(clDataRow["SubscriptionID"])); }
            set { clDataRow["SubscriptionID"] = value; }
        }

        public int UserID
        {
            get { return (Convert.ToInt32(clDataRow["UserID"])); }
            set { clDataRow["UserID"] = value; }
        }

        public String LoginID
        {
            get { return (Convert.ToString(clDataRow["LoginID"])); }
            set { clDataRow["LoginID"] = value; }
        }

        public String Type
        {
            get { return (Convert.ToString(clDataRow["Type"])); }
            set { clDataRow["Type"] = value; }
        }

        public String Status
        {
            get { return (Convert.ToString(clDataRow["Status"])); }
            set { clDataRow["Status"] = value; }
        }

        public String UserRole
        {
            get { return (Convert.ToString(clDataRow["UserRole"])); }
            set { clDataRow["UserRole"] = value; }
        }

        public String CustomSetting
        {
            get { return (Convert.ToString(clDataRow["CustomSetting"])); }
            set { clDataRow["CustomSetting"] = value; }
        }

        public String Password
        {
            get { return (Convert.ToString(clDataRow["Password"])); }
            set { clDataRow["Password"] = value; }
        }

        public String Name
        {
            get { return (Convert.ToString(clDataRow["Name"])); }
            set { clDataRow["Name"] = value; }
        }

        public String EmailAddress
        {
            get { return (Convert.ToString(clDataRow["EmailAddress"])); }
            set { clDataRow["EmailAddress"] = value; }
        }

        public String ContactNo
        {
            get { return (Convert.ToString(clDataRow["ContactNo"])); }
            set { clDataRow["ContactNo"] = value; }
        }

        public DateTime DateOfBirth
        {
            get { return (Convert.ToDateTime(clDataRow["DateOfBirth"])); }
            set { clDataRow["DateOfBirth"] = value; }
        }

        public String Gender
        {
            get { return (Convert.ToString(clDataRow["Gender"])); }
            set { clDataRow["Gender"] = value; }
        }

        public String BuildingNo
        {
            get { return (Convert.ToString(clDataRow["BuildingNo"])); }
            set { clDataRow["BuildingNo"] = value; }
        }

        public String Floor
        {
            get { return (Convert.ToString(clDataRow["Floor"])); }
            set { clDataRow["Floor"] = value; }
        }

        public String RoomNo
        {
            get { return (Convert.ToString(clDataRow["RoomNo"])); }
            set { clDataRow["RoomNo"] = value; }
        }

        public String Street
        {
            get { return (Convert.ToString(clDataRow["Street"])); }
            set { clDataRow["Street"] = value; }
        }

        public String Quarter
        {
            get { return (Convert.ToString(clDataRow["Quarter"])); }
            set { clDataRow["Quarter"] = value; }
        }

        public String AddressInfo
        {
            get { return (Convert.ToString(clDataRow["AddressInfo"])); }
            set { clDataRow["AddressInfo"] = value; }
        }

        public String Township
        {
            get { return (Convert.ToString(clDataRow["Township"])); }
            set { clDataRow["Township"] = value; }
        }

        public String City
        {
            get { return (Convert.ToString(clDataRow["City"])); }
            set { clDataRow["City"] = value; }
        }

        public String Country
        {
            get { return (Convert.ToString(clDataRow["Country"])); }
            set { clDataRow["Country"] = value; }
        }

        public String Postal
        {
            get { return (Convert.ToString(clDataRow["Postal"])); }
            set { clDataRow["Postal"] = value; }
        }

        public String AdditionalInfo
        {
            get { return (Convert.ToString(clDataRow["AdditionalInfo"])); }
            set { clDataRow["AdditionalInfo"] = value; }
        }

        public String Vir_CompiledAddress
        {
            get { return (Convert.ToString(clDataRow["Vir_CompiledAddress"])); }
            set { clDataRow["Vir_CompiledAddress"] = value; }
        }
    }


    public class LanguageRow
    {
        DataRow clDataRow;

        public LanguageRow(DataRow paDataRow)
        {
            clDataRow = paDataRow;
        }

        public DataRow Row
        {
            get { return (clDataRow); }
            set { clDataRow = value; }
        }

        public object this[String paString]
        {
            get { return (clDataRow[paString]); }
            set { clDataRow[paString] = value; }
        }

        // Null Checking.
        public bool IsLanguageNull { get { return (clDataRow["Language"].GetType().ToString() == "System.DBNull"); } }
        public bool IsLocalNameNull { get { return (clDataRow["LocalName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsLanguageCodeNull { get { return (clDataRow["LanguageCode"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDigitListNull { get { return (clDataRow["DigitList"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsEmbeddedFontListNull { get { return (clDataRow["EmbeddedFontList"].GetType().ToString() == "System.DBNull"); } }
        public bool IsFlagIconNull { get { return (clDataRow["FlagIcon"].GetType().ToString() == "System.DBNull"); } }                
        

        public String Language
        {
            get { return (Convert.ToString(clDataRow["Language"])); }
            set { clDataRow["Language"] = value; }
        }

        public String LocalName
        {
            get { return (Convert.ToString(clDataRow["LocalName"])); }
            set { clDataRow["LocalName"] = value; }
        }

        public String LanguageCode
        {
            get { return (Convert.ToString(clDataRow["LanguageCode"])); }
            set { clDataRow["LanguageCode"] = value; }
        }

        public String DigitList
        {
            get { return (Convert.ToString(clDataRow["DigitList"])); }
            set { clDataRow["DigitList"] = value; }
        }

        public String EmbeddedFontList
        {
            get { return (Convert.ToString(clDataRow["EmbeddedFontList"])); }
            set { clDataRow["EmbeddedFontList"] = value; }
        }

        public String FlagIcon
        {
            get { return (Convert.ToString(clDataRow["FlagIcon"])); }
            set { clDataRow["FlagIcon"] = value; }
        }
    }

    public class TimeZoneRow
    {
        DataRow clDataRow;

        public TimeZoneRow(DataRow paDataRow)
        {
            clDataRow = paDataRow;
        }

        public DataRow Row
        {
            get { return (clDataRow); }
            set { clDataRow = value; }
        }

        public object this[String paString]
        {
            get { return (clDataRow[paString]); }
            set { clDataRow[paString] = value; }
        }

        // Null Checking.
        public bool IsTimezoneIDNull { get { return (clDataRow["TimezoneID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsNameNull { get { return (clDataRow["Name"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDescriptionNull { get { return (clDataRow["Description"].GetType().ToString() == "System.DBNull"); } }
        public bool IsOffsetNull { get { return (clDataRow["Offset"].GetType().ToString() == "System.DBNull"); } }

        public String TimezoneID
        {
            get { return (Convert.ToString(clDataRow["TimezoneID"])); }
            set { clDataRow["TimezoneID"] = value; }
        }

        public String Name
        {
            get { return (Convert.ToString(clDataRow["Name"])); }
            set { clDataRow["Name"] = value; }
        }

        public String Description
        {
            get { return (Convert.ToString(clDataRow["Description"])); }
            set { clDataRow["Description"] = value; }
        }

        public Decimal Offset
        {
            get { return (Convert.ToDecimal(clDataRow["Offset"])); }
            set { clDataRow["Offset"] = value; }
        }
    }


    public class MessageRow
    {
        DataRow clDataRow;

        public MessageRow(DataRow paDataRow)
        {
            clDataRow = paDataRow;
        }

        public DataRow Row
        {
            get { return (clDataRow); }
            set { clDataRow = value; }
        }

        public object this[String paString]
        {
            get { return (clDataRow[paString]); }
            set { clDataRow[paString] = value; }
        }

        // Null Checking.
        public bool IsFormNameNull { get { return (clDataRow["FormName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsMessageCodeNull { get { return (clDataRow["MessageCode"].GetType().ToString() == "System.DBNull"); } }
        public bool IsLanguageNull { get { return (clDataRow["Language"].GetType().ToString() == "System.DBNull"); } }
        public bool IsMessageTypeNull { get { return (clDataRow["MessageType"].GetType().ToString() == "System.DBNull"); } }
        public bool IsTitleNull { get { return (clDataRow["Title"].GetType().ToString() == "System.DBNull"); } }
        public bool IsMessageNull { get { return (clDataRow["Message"].GetType().ToString() == "System.DBNull"); } }
        public bool IsButton1Null { get { return (clDataRow["Button1"].GetType().ToString() == "System.DBNull"); } }
        public bool IsButton2Null { get { return (clDataRow["Button2"].GetType().ToString() == "System.DBNull"); } }

        public String FormName
        {
            get { return (Convert.ToString(clDataRow["FormName"])); }
            set { clDataRow["FormName"] = value; }
        }

        public String MessageCode
        {
            get { return (Convert.ToString(clDataRow["MessageCode"])); }
            set { clDataRow["MessageCode"] = value; }
        }

        public String Language
        {
            get { return (Convert.ToString(clDataRow["Language"])); }
            set { clDataRow["Language"] = value; }
        }

        public String MessageType
        {
            get { return (Convert.ToString(clDataRow["MessageType"])); }
            set { clDataRow["LocalName"] = value; }
        }

        public String Title
        {
            get { return (Convert.ToString(clDataRow["Title"])); }
            set { clDataRow["Title"] = value; }
        }

        public String Message
        {
            get { return (Convert.ToString(clDataRow["Message"])); }
            set { clDataRow["Message"] = value; }
        }

        public String Button1
        {
            get { return (Convert.ToString(clDataRow["Button1"])); }
            set { clDataRow["Button1"] = value; }
        }

        public String Button2
        {
            get { return (Convert.ToString(clDataRow["Button2"])); }
            set { clDataRow["Button2"] = value; }
        }
    }

    public class SessionRow
    {
        DataRow clDataRow;

        public SessionRow(DataRow paDataRow)
        {
            clDataRow = paDataRow;
        }

        public DataRow Row
        {
            get { return (clDataRow); }
            set { clDataRow = value; }
        }

        public object this[String paString]
        {
            get { return (clDataRow[paString]); }
            set { clDataRow[paString] = value; }
        }

        // Null Checking.
        public bool IsSubscriptionIDNull { get { return (clDataRow["SubscriptionID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsLoginIDNull { get { return (clDataRow["LoginID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsPasswordNull { get { return (clDataRow["Password"].GetType().ToString() == "System.DBNull"); } }
        public bool IsSessionOwnerIPNull { get { return (clDataRow["SessionOwnerIP"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsSessionKeyNull { get { return (clDataRow["SessionKey"].GetType().ToString() == "System.DBNull"); } }
        public bool IsStartTimeNull { get { return (clDataRow["StartTime"].GetType().ToString() == "System.DBNull"); } }
        public bool IsExpiryTimeNull { get { return (clDataRow["ExpiryTime"].GetType().ToString() == "System.DBNull"); } }
        public bool IsFirstAccessTimeNull { get { return (clDataRow["FirstAccessTime"].GetType().ToString() == "System.DBNull"); } }
        public bool IsLastAccessTimeNull { get { return (clDataRow["LastAccessTime"].GetType().ToString() == "System.DBNull"); } }
        public bool IsAccessCountNull { get { return (clDataRow["AccessCount"].GetType().ToString() == "System.DBNull"); } }

        public String SubscriptionID
        {
            get { return (Convert.ToString(clDataRow["SubscriptionID"])); }
            set { clDataRow["SubscriptionID"] = value; }
        }

        public String LoginID
        {
            get { return (Convert.ToString(clDataRow["LoginID"])); }
            set { clDataRow["LoginID"] = value; }
        }

        public String Password
        {
            get { return (Convert.ToString(clDataRow["Password"])); }
            set { clDataRow["Password"] = value; }
        }

        public String SessionOwnerIP
        {
            get { return (Convert.ToString(clDataRow["SessionOwnerIP"])); }
            set { clDataRow["SessionOwnerIP"] = value; }
        }

        public String SessionKey
        {
            get { return (Convert.ToString(clDataRow["SessionKey"])); }
            set { clDataRow["SessionKey"] = value; }
        }        

        public DateTime StartTime
        {
            get { return (Convert.ToDateTime(clDataRow["StartTime"])); }
            set { clDataRow["StartTime"] = value; }
        }

        public DateTime ExpiryTime
        {
            get { return (Convert.ToDateTime(clDataRow["ExpiryTime"])); }
            set { clDataRow["ExpiryTime"] = value; }
        }

        public DateTime FirstAccessTime
        {
            get { return (Convert.ToDateTime(clDataRow["FirstAccessTime"])); }
            set { clDataRow["FirstAccessTime"] = value; }
        }

        public DateTime LastAccessTime
        {
            get { return (Convert.ToDateTime(clDataRow["LastAccessTime"])); }
            set { clDataRow["LastAccessTime"] = value; }
        }

        public int AccessCount
        {
            get { return (Convert.ToInt32(clDataRow["AccessCount"])); }
            set { clDataRow["AccessCount"] = value; }
        }
    }

    public class WidgetRow
    {
        DataRow clDataRow;

        public WidgetRow(DataRow paDataRow)
        {
            clDataRow = paDataRow;
        }

        public DataRow Row
        {
            get { return (clDataRow); }
            set { clDataRow = value; }
        }

        public object this[String paString]
        {
            get { return (clDataRow[paString]); }
            set { clDataRow[paString] = value; }
        }

        // Null Checking.
        public bool IsWidgetNameNull { get { return (clDataRow["WidgetName"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsGroupingNull { get { return (clDataRow["Grouping"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsIconNull { get { return (clDataRow["Icon"].GetType().ToString() == "System.DBNull"); } }
        public bool IsIconLabelNull { get { return (clDataRow["IconLabel"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsTypeNull { get { return (clDataRow["Type"].GetType().ToString() == "System.DBNull"); } }
        public bool IsCategoryNull { get { return (clDataRow["Category"].GetType().ToString() == "System.DBNull"); } }
        public bool IsLinkNull { get { return (clDataRow["Link"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDisplayNameNull { get { return (clDataRow["DisplayName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsBriefDescriptionNull { get { return (clDataRow["BriefDescription"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDetailDescriptionNull { get { return (clDataRow["DetailDescription"].GetType().ToString() == "System.DBNull"); } }
        public bool IsLicenseModeNull { get { return (clDataRow["LicenseMode"].GetType().ToString() == "System.DBNull"); } }
        public bool IsSubscriptionQuantumNull { get { return (clDataRow["SubscriptionQuantum"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDiscountStartNull { get { return (clDataRow["DiscountStart"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDiscountExpiryNull { get { return (clDataRow["DiscountExpiry"].GetType().ToString() == "System.DBNull"); } }
        public bool IsSubscriptionFeeNull { get { return (clDataRow["SubscriptionFee"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDiscountedSetupFeeNull { get { return (clDataRow["DiscountedSetupFee"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDiscountedMonthlyFeeNull { get { return (clDataRow["DiscountedMonthlyFee"].GetType().ToString() == "System.DBNull"); } }
        public bool IsRequireRoleNull { get { return (clDataRow["RequireRole"].GetType().ToString() == "System.DBNull"); } }

        public String WidgetName
        {
            get { return (Convert.ToString(clDataRow["WidgetName"])); }
            set { clDataRow["WidgetName"] = value; }
        }

        public String Grouping
        {
            get { return (Convert.ToString(clDataRow["Grouping"])); }
            set { clDataRow["Grouping"] = value; }
        }        

        public String Icon
        {
            get { return (Convert.ToString(clDataRow["Icon"])); }
            set { clDataRow["Icon"] = value; }
        }

        public String IconLabel
        {
            get { return (Convert.ToString(clDataRow["IconLabel"])); }
            set { clDataRow["IconLabel"] = value; }
        }

        public String Type
        {
            get { return (Convert.ToString(clDataRow["Type"])); }
            set { clDataRow["Type"] = value; }
        }

        public String Category
        {
            get { return (Convert.ToString(clDataRow["Category"])); }
            set { clDataRow["Category"] = value; }
        }

        public String Link
        {
            get { return (Convert.ToString(clDataRow["Link"])); }
            set { clDataRow["Link"] = value; }
        }

        public String DisplayName
        {
            get { return (Convert.ToString(clDataRow["DisplayName"])); }
            set { clDataRow["DisplayName"] = value; }
        }

        public String BriefDescription
        {
            get { return (Convert.ToString(clDataRow["BriefDescription"])); }
            set { clDataRow["BriefDescription"] = value; }
        }

        public String DetailDescription
        {
            get { return (Convert.ToString(clDataRow["DetailDescription"])); }
            set { clDataRow["DetailDescription"] = value; }
        }

        public String LicenseMode
        {
            get { return (Convert.ToString(clDataRow["LicenseMode"])); }
            set { clDataRow["LicenseMode"] = value; }
        }

        public int SubscriptionQuantum
        {
            get { return (Convert.ToInt32(clDataRow["SubscriptionQuantum"])); }
            set { clDataRow["SubscriptionQuantum"] = value; }
        }

        public DateTime DiscountStart
        {
            get { return (Convert.ToDateTime(clDataRow["DiscountStart"])); }
            set { clDataRow["DiscountStart"] = value; }
        }

        public DateTime DiscountExpiry
        {
            get { return (Convert.ToDateTime(clDataRow["DiscountExpiry"])); }
            set { clDataRow["DiscountExpiry"] = value; }
        }

        public Decimal SetupFee
        {
            get { return (Convert.ToDecimal(clDataRow["SetupFee"])); }
            set { clDataRow["SetupFee"] = value; }
        }

        public Decimal SubscriptionFee
        {
            get { return (Convert.ToDecimal(clDataRow["SubscriptionFee"])); }
            set { clDataRow["SubscriptionFee"] = value; }
        }

        public Decimal UsualSetupFee
        {
            get { return (Convert.ToDecimal(clDataRow["UsualSetupFee"])); }
            set { clDataRow["UsualSetupFee"] = value; }
        }

        public Decimal UsualMonthlyFee
        {
            get { return (Convert.ToDecimal(clDataRow["UsualMonthlyFee"])); }
            set { clDataRow["UsualMonthlyFee"] = value; }
        }

        public String RequireRole
        {
            get { return (Convert.ToString(clDataRow["RequireRole"])); }
            set { clDataRow["RequireRole"] = value; }
        }
    }

    public class PaymentRow
    {
        DataRow clDataRow;

        public PaymentRow(DataRow paDataRow)
        {
            clDataRow = paDataRow;
        }

        public DataRow Row
        {
            get { return (clDataRow); }
            set { clDataRow = value; }
        }

        public object this[String paString]
        {
            get { return (clDataRow[paString]); }
            set { clDataRow[paString] = value; }
        }

        // Null Checking.
        public bool IsTransactionIDNull { get { return (clDataRow["TransactionID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsTransactionNoNull { get { return (clDataRow["TransactionNo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsPaymentStatusNull { get { return (clDataRow["PaymentStatus"].GetType().ToString() == "System.DBNull"); } }
        public bool IsSubmitDateNull { get { return (clDataRow["SubmitDate"].GetType().ToString() == "System.DBNull"); } }
        public bool IsSubscriptionIDNull { get { return (clDataRow["SubscriptionID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsLoginIDNull { get { return (clDataRow["LoginID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsPayerNameNull { get { return (clDataRow["PayerName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsPayerContactNoNull { get { return (clDataRow["PayerContactNo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsPaymentDateNull { get { return (clDataRow["PaymentDate"].GetType().ToString() == "System.DBNull"); } }
        public bool IsPaymentTypeNull { get { return (clDataRow["PaymentType"].GetType().ToString() == "System.DBNull"); } }
        public bool IsPaymentAmountNull { get { return (clDataRow["PaymentAmount"].GetType().ToString() == "System.DBNull"); } }
        public bool IsPayeeNameNull { get { return (clDataRow["PayeeName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsPayeeAccountNoNull { get { return (clDataRow["PayeeAccountNo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsPayeeBankNull { get { return (clDataRow["PayeeBank"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsNotesNull { get { return (clDataRow["Notes"].GetType().ToString() == "System.DBNull"); } }
        public bool IsTargetReceiverIDNull { get { return (clDataRow["TargetReceiverID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsRemarkNull { get { return (clDataRow["Remark"].GetType().ToString() == "System.DBNull"); } }

        public int TransactionID
        {
            get { return (Convert.ToInt32(clDataRow["TransactionID"])); }
            set { clDataRow["TransactionID"] = value; }
        }

        public String TransactionNo
        {
            get { return (Convert.ToString(clDataRow["TransactionNo"])); }
            set { clDataRow["TransactionNo"] = value; }
        }

        public String PaymentStatus
        {
            get { return (Convert.ToString(clDataRow["PaymentStatus"])); }
            set { clDataRow["PaymentStatus"] = value; }
        }

        public DateTime SubmitDate
        {
            get { return (Convert.ToDateTime(clDataRow["SubmitDate"])); }
            set { clDataRow["SubmitDate"] = value; }
        }

        public String SubscriptionID
        {
            get { return (Convert.ToString(clDataRow["SubscriptionID"])); }
            set { clDataRow["SubscriptionID"] = value; }
        }

        public String LoginID
        {
            get { return (Convert.ToString(clDataRow["LoginID"])); }
            set { clDataRow["LoginID"] = value; }
        }

        public String PayerName
        {
            get { return (Convert.ToString(clDataRow["PayerName"])); }
            set { clDataRow["PayerName"] = value; }
        }

        public String PayerContactNo
        {
            get { return (Convert.ToString(clDataRow["PayerContactNo"])); }
            set { clDataRow["PayerContactNo"] = value; }
        }

        public String PaymentPurpose
        {
            get { return (Convert.ToString(clDataRow["PaymentPurpose"])); }
            set { clDataRow["PaymentPurpose"] = value; }
        }

        public DateTime PaymentDate
        {
            get { return (Convert.ToDateTime(clDataRow["PaymentDate"])); }
            set { clDataRow["PaymentDate"] = value; }
        }

        public String PaymentType
        {
            get { return (Convert.ToString(clDataRow["PaymentType"])); }
            set { clDataRow["PaymentType"] = value; }
        }

        public Decimal PaymentAmount
        {
            get { return (Convert.ToDecimal(clDataRow["PaymentAmount"])); }
            set { clDataRow["PaymentAmount"] = value; }
        }

        public String PayeeName
        {
            get { return (Convert.ToString(clDataRow["PayeeName"])); }
            set { clDataRow["PayeeName"] = value; }
        }

        public String PayeeAccountNo
        {
            get { return (Convert.ToString(clDataRow["PayeeAccountNo"])); }
            set { clDataRow["PayeeAccountNo"] = value; }
        }

        public String PayeeBank
        {
            get { return (Convert.ToString(clDataRow["PayeeBank"])); }
            set { clDataRow["PayeeBank"] = value; }
        }

        public String Note
        {
            get { return (Convert.ToString(clDataRow["Note"])); }
            set { clDataRow["Note"] = value; }
        }

        public String TargetReceiverID
        {
            get { return (Convert.ToString(clDataRow["PayeeBank"])); }
            set { clDataRow["PayeeBank"] = value; }
        }

        public String Remark
        {
            get { return (Convert.ToString(clDataRow["PayeeBank"])); }
            set { clDataRow["PayeeBank"] = value; }
        }
    }

    public class ServiceRequestLogRow
    {
        DataRow clDataRow;

        public ServiceRequestLogRow(DataRow paDataRow)
        {
            clDataRow = paDataRow;
        }

        public DataRow Row
        {
            get { return (clDataRow); }
            set { clDataRow = value; }
        }

        public object this[String paString]
        {
            get { return (clDataRow[paString]); }
            set { clDataRow[paString] = value; }
        }
                
        // Null Checking.
        public bool IsIDNull { get { return (clDataRow["ID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsSubscriptionIDNull { get { return (clDataRow["SubscriptionID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsRequestTimeNull { get { return (clDataRow["RequestTime"].GetType().ToString() == "System.DBNull"); } }
        public bool IsCompleteTimeNull { get { return (clDataRow["CompleteTime"].GetType().ToString() == "System.DBNull"); } }
        public bool IsRequestIPNull { get { return (clDataRow["RequestIP"].GetType().ToString() == "System.DBNull"); } }
        public bool IsRequestUrlNull { get { return (clDataRow["RequestUrl"].GetType().ToString() == "System.DBNull"); } }

        public int ID
        {
            get { return (Convert.ToInt32(clDataRow["ID"])); }
            set { clDataRow["ID"] = value; }
        }

        public String SubscriptionID
        {
            get { return (Convert.ToString(clDataRow["SubscriptionID"])); }
            set { clDataRow["SubscriptionID"] = value; }
        }

        public DateTime RequestTime
        {
            get { return (Convert.ToDateTime(clDataRow["RequestTime"])); }
            set { clDataRow["RequestTime"] = value; }
        }

        public DateTime CompleteTime
        {
            get { return (Convert.ToDateTime(clDataRow["CompleteTime"])); }
            set { clDataRow["CompleteTime"] = value; }
        }

        public String RequestIP
        {
            get { return (Convert.ToString(clDataRow["RequestIP"])); }
            set { clDataRow["RequestIP"] = value; }
        }

        public String RequestUrl
        {
            get { return (Convert.ToString(clDataRow["RequestUrl"])); }
            set { clDataRow["RequestUrl"] = value; }
        }
    }

    // ************* VIEW ROWS
    public class ViewWidgetSubscriptionRow
    {
        DataRow clDataRow;

        public ViewWidgetSubscriptionRow(DataRow paDataRow)
        {
            clDataRow = paDataRow;
        }

        public DataRow Row
        {
            get { return (clDataRow); }
            set { clDataRow = value; }
        }

        public object this[String paString]
        {
            get { return (clDataRow[paString]); }
            set { clDataRow[paString] = value; }
        }

        // Null Checking.
        public bool IsWidgetNameNull { get { return (clDataRow["WidgetName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsGroupingNull { get { return (clDataRow["Grouping"].GetType().ToString() == "System.DBNull"); } }
        public bool IsIconNull { get { return (clDataRow["Icon"].GetType().ToString() == "System.DBNull"); } }
        public bool IsIconLabelNull { get { return (clDataRow["IconLabel"].GetType().ToString() == "System.DBNull"); } }
        public bool IsTypeNull { get { return (clDataRow["Type"].GetType().ToString() == "System.DBNull"); } }
        public bool IsCategoryNull { get { return (clDataRow["Category"].GetType().ToString() == "System.DBNull"); } }
        public bool IsLinkNull { get { return (clDataRow["Link"].GetType().ToString() == "System.DBNull"); } }
        public bool IsFriendlyNameNull { get { return (clDataRow["FriendlyName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDescriptionNull { get { return (clDataRow["Description"].GetType().ToString() == "System.DBNull"); } }
        public bool IsLicenseModeNull { get { return (clDataRow["LicenseMode"].GetType().ToString() == "System.DBNull"); } }
        public bool IsRequireRoleNull { get { return (clDataRow["RequireRole"].GetType().ToString() == "System.DBNull"); } }
        public bool IsServiceKeyNull { get { return (clDataRow["ServiceKey"].GetType().ToString() == "System.DBNull"); } }
        public bool IsStatusNull { get { return (clDataRow["Status"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsClassificationCodeNull { get { return (clDataRow["ClassificationCode"].GetType().ToString() == "System.DBNull"); } }
        public bool IsSortPriorityeNull { get { return (clDataRow["SortPriority"].GetType().ToString() == "System.DBNull"); } }
        public bool IsActivationDateNull { get { return (clDataRow["ActivationDate"].GetType().ToString() == "System.DBNull"); } }
        public bool IsExpiryDateNull { get { return (clDataRow["ExpiryDate"].GetType().ToString() == "System.DBNull"); } }

        public String WidgetName
        {
            get { return (Convert.ToString(clDataRow["WidgetName"])); }
            set { clDataRow["WidgetName"] = value; }
        }

        public String Grouping
        {
            get { return (Convert.ToString(clDataRow["Grouping"])); }
            set { clDataRow["Grouping"] = value; }
        }

        public String Icon
        {
            get { return (Convert.ToString(clDataRow["Icon"])); }
            set { clDataRow["Icon"] = value; }
        }

        public String IconLabel
        {
            get { return (Convert.ToString(clDataRow["IconLabel"])); }
            set { clDataRow["IconLabel"] = value; }
        }

        public String Type
        {
            get { return (Convert.ToString(clDataRow["Type"])); }
            set { clDataRow["Type"] = value; }
        }

        public String Category
        {
            get { return (Convert.ToString(clDataRow["Category"])); }
            set { clDataRow["Category"] = value; }
        }

        public String Link
        {
            get { return (Convert.ToString(clDataRow["Link"])); }
            set { clDataRow["Link"] = value; }
        }

        public String FriendlyName
        {
            get { return (Convert.ToString(clDataRow["FriendlyName"])); }
            set { clDataRow["FriendlyName"] = value; }
        }

        public String Description
        {
            get { return (Convert.ToString(clDataRow["Description"])); }
            set { clDataRow["Description"] = value; }
        }

        public String LicenseMode
        {
            get { return (Convert.ToString(clDataRow["LicenseMode"])); }
            set { clDataRow["LicenseMode"] = value; }
        }

        public String RequireRole
        {
            get { return (Convert.ToString(clDataRow["RequireRole"])); }
            set { clDataRow["RequireRole"] = value; }
        }

        public String ServiceKey
        {
            get { return (Convert.ToString(clDataRow["SubscriptionID"])); }
            set { clDataRow["SubscriptionID"] = value; }
        }

        public String Status
        {
            get { return (Convert.ToString(clDataRow["Status"])); }
            set { clDataRow["Status"] = value; }
        }

        public int ClassificationCode
        {
            get { return (Convert.ToInt32(clDataRow["ClassificationCode"])); }
            set { clDataRow["ClassificationCode"] = value; }
        }

        public int SortPriority
        {
            get { return (Convert.ToInt32(clDataRow["SortPriority"])); }
            set { clDataRow["SortPriority"] = value; }
        }

        public DateTime ActivationDate
        {
            get { return (Convert.ToDateTime(clDataRow["ActivationDate"])); }
            set { clDataRow["ActivationDate"] = value; }
        }

        public DateTime ExpiryDate
        {
            get { return (Convert.ToDateTime(clDataRow["ExpiryDate"])); }
            set { clDataRow["ExpiryDate"] = value; }
        }
    }

    public class ToolBarRow
    {
        DataRow clDataRow;

        public ToolBarRow(DataRow paDataRow)
        {
            clDataRow = paDataRow;
        }

        public DataRow Row
        {
            get { return (clDataRow); }
            set { clDataRow = value; }
        }

        public object this[String paString]
        {
            get { return (clDataRow[paString]); }
            set { clDataRow[paString] = value; }
        }

        // Null Checking.
        public bool IsToolBarNameNull { get { return (clDataRow["ToolBarName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsItemNameNull { get { return (clDataRow["ItemName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsItemIconNull { get { return (clDataRow["ItemIcon"].GetType().ToString() == "System.DBNull"); } }
        public bool IsItemOrderNull { get { return (clDataRow["ItemOrder"].GetType().ToString() == "System.DBNull"); } }
        public bool IsLinkNull { get { return (clDataRow["Link"].GetType().ToString() == "System.DBNull"); } }
        public bool IsItemCssNull { get { return (clDataRow["ItemCss"].GetType().ToString() == "System.DBNull"); } }
        public bool IsItemAttributeNull { get { return (clDataRow["ItemAttribute"].GetType().ToString() == "System.DBNull"); } }
        
        public String ToolBarName
        {
            get { return (Convert.ToString(clDataRow["ToolBarName"])); }
            set { clDataRow["ToolBarName"] = value; }
        }

        public String ItemName
        {
            get { return (Convert.ToString(clDataRow["ItemName"])); }
            set { clDataRow["ItemName"] = value; }
        }

        public String ItemIcon
        {
            get { return (Convert.ToString(clDataRow["ItemIcon"])); }
            set { clDataRow["ItemIcon"] = value; }
        }

        public int ItemOrder
        {
            get { return (Convert.ToInt32(clDataRow["ItemOrder"])); }
            set { clDataRow["ItemOrder"] = value; }
        }

        public String Link
        {
            get { return (Convert.ToString(clDataRow["Link"])); }
            set { clDataRow["Link"] = value; }
        }

        public String ItemCss
        {
            get { return (Convert.ToString(clDataRow["ItemCss"])); }
            set { clDataRow["ItemCss"] = value; }
        }

        public String ItemAttribute
        {
            get { return (Convert.ToString(clDataRow["ItemAttribute"])); }
            set { clDataRow["ItemAttribute"] = value; }
        }
    }

    public class DeliveryAddressRow
    {
        DataRow clDataRow;

        public DeliveryAddressRow(DataRow paDataRow)
        {
            clDataRow = paDataRow;
        }

        public DataRow Row
        {
            get { return (clDataRow); }
            set { clDataRow = value; }
        }

        public object this[String paString]
        {
            get { return (clDataRow[paString]); }
            set { clDataRow[paString] = value; }
        }

        // Null Checking.
        public bool IsSubscriptionIDNull { get { return (clDataRow["SubscriptionID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsLoginIDNull { get { return (clDataRow["LoginID"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsNameNull { get { return (clDataRow["Name"].GetType().ToString() == "System.DBNull"); } }
        public bool IsContactNoNull { get { return (clDataRow["ContactNo"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsBuildingNoNull { get { return (clDataRow["BuildingNo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsFloorNull { get { return (clDataRow["Floor"].GetType().ToString() == "System.DBNull"); } }
        public bool IsRoomNoNull { get { return (clDataRow["RoomNo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsStreetNull { get { return (clDataRow["Street"].GetType().ToString() == "System.DBNull"); } }
        public bool IsQuarterNull { get { return (clDataRow["Quarter"].GetType().ToString() == "System.DBNull"); } }
        public bool IsAddressInfoNull { get { return (clDataRow["AddressInfo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsTownshipNull { get { return (clDataRow["Township"].GetType().ToString() == "System.DBNull"); } }
        public bool IsCityNull { get { return (clDataRow["City"].GetType().ToString() == "System.DBNull"); } }
        public bool IsCountryNull { get { return (clDataRow["Country"].GetType().ToString() == "System.DBNull"); } }
        public bool IsPostalNull { get { return (clDataRow["Postal"].GetType().ToString() == "System.DBNull"); } }        

        public String SubscriptionID
        {
            get { return (Convert.ToString(clDataRow["SubscriptionID"])); }
            set { clDataRow["SubscriptionID"] = value; }
        }

        public String LoginID
        {
            get { return (Convert.ToString(clDataRow["LoginID"])); }
            set { clDataRow["LoginID"] = value; }
        }        

        public String Name
        {
            get { return (Convert.ToString(clDataRow["Name"])); }
            set { clDataRow["Name"] = value; }
        }
        
        public String ContactNo
        {
            get { return (Convert.ToString(clDataRow["ContactNo"])); }
            set { clDataRow["ContactNo"] = value; }
        }

        public String BuildingNo
        {
            get { return (Convert.ToString(clDataRow["BuildingNo"])); }
            set { clDataRow["BuildingNo"] = value; }
        }

        public String Floor
        {
            get { return (Convert.ToString(clDataRow["Floor"])); }
            set { clDataRow["Floor"] = value; }
        }

        public String RoomNo
        {
            get { return (Convert.ToString(clDataRow["RoomNo"])); }
            set { clDataRow["RoomNo"] = value; }
        }

        public String Street
        {
            get { return (Convert.ToString(clDataRow["Street"])); }
            set { clDataRow["Street"] = value; }
        }

        public String Quarter
        {
            get { return (Convert.ToString(clDataRow["Quarter"])); }
            set { clDataRow["Quarter"] = value; }
        }

        public String AddressInfo
        {
            get { return (Convert.ToString(clDataRow["AddressInfo"])); }
            set { clDataRow["AddressInfo"] = value; }
        }

        public String Township
        {
            get { return (Convert.ToString(clDataRow["Township"])); }
            set { clDataRow["Township"] = value; }
        }

        public String City
        {
            get { return (Convert.ToString(clDataRow["City"])); }
            set { clDataRow["City"] = value; }
        }

        public String Country
        {
            get { return (Convert.ToString(clDataRow["Country"])); }
            set { clDataRow["Country"] = value; }
        }

        public String Postal
        {
            get { return (Convert.ToString(clDataRow["Postal"])); }
            set { clDataRow["Postal"] = value; }
        }

        public String Vir_CompiledAddress
        {
            get { return (Convert.ToString(clDataRow["Vir_CompiledAddress"])); }
            set { clDataRow["Vir_CompiledAddress"] = value; }
        }
    }    

    public class SubscriberRow
    {
        DataRow clDataRow;

        public SubscriberRow(DataRow paDataRow)
        {
            clDataRow = paDataRow;
        }

        public DataRow Row
        {
            get { return (clDataRow); }
            set { clDataRow = value; }
        }

        public object this[String paString]
        {
            get { return (clDataRow[paString]); }
            set { clDataRow[paString] = value; }
        }

        // Null Checking.
        public bool IsSubscriptionIDNull { get { return (clDataRow["SubscriptionID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsContactPersonNull { get { return (clDataRow["ContactPerson"].GetType().ToString() == "System.DBNull"); } }
        public bool IsContactPersonMobileNoNull { get { return (clDataRow["ContactPersonMobileNo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsContactPersonEmailNull { get { return (clDataRow["ContactPersonEmail"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsBusinessNameNull { get { return (clDataRow["BusinessName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsIndustryTypeDateNull { get { return (clDataRow["IndustryType"].GetType().ToString() == "System.DBNull"); } }
        public bool IsEmailAddressNull { get { return (clDataRow["DeliveryDate"].GetType().ToString() == "System.DBNull"); } }
        public bool IsContactNoNull { get { return (clDataRow["ContactNo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsBuildingNoNull { get { return (clDataRow["BuildingNo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsFloorNull { get { return (clDataRow["Floor"].GetType().ToString() == "System.DBNull"); } }
        public bool IsRoomNoNull { get { return (clDataRow["RoomNo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsStreetNull { get { return (clDataRow["Street"].GetType().ToString() == "System.DBNull"); } }
        public bool IsQuarterNull { get { return (clDataRow["Quarter"].GetType().ToString() == "System.DBNull"); } }
        public bool IsAddressInfoNull { get { return (clDataRow["AddressInfo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsTownshipNull { get { return (clDataRow["Township"].GetType().ToString() == "System.DBNull"); } }
        public bool IsCityNull { get { return (clDataRow["City"].GetType().ToString() == "System.DBNull"); } }
        public bool IsCountryNull { get { return (clDataRow["Country"].GetType().ToString() == "System.DBNull"); } }
        public bool IsPostalNull { get { return (clDataRow["Postal"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDisplayAddressNull { get { return (clDataRow["DisplayAddress"].GetType().ToString() == "System.DBNull"); } }
        public bool IsLongitudeNull { get { return (clDataRow["Longitude"].GetType().ToString() == "System.DBNull"); } }
        public bool IsLatitudeNull { get { return (clDataRow["Latitude"].GetType().ToString() == "System.DBNull"); } }
        
        public String SubscriptionID
        {
            get { return (Convert.ToString(clDataRow["SubscriptionID"])); }
            set { clDataRow["SubscriptionID"] = value; }
        }

        public String ContactPerson
        {
            get { return (Convert.ToString(clDataRow["ContactPerson"])); }
            set { clDataRow["ContactPerson"] = value; }
        }

        public String ContactPersonMobileNo
        {
            get { return (Convert.ToString(clDataRow["ContactPersonMobileNo"])); }
            set { clDataRow["ContactPersonMobileNo"] = value; }
        }

        public String ContactPersonEmail
        {
            get { return (Convert.ToString(clDataRow["ContactPersonEmail"])); }
            set { clDataRow["ContactPersonEmail"] = value; }
        }

        public String BusinessName
        {
            get { return (Convert.ToString(clDataRow["BusinessName"])); }
            set { clDataRow["BusinessName"] = value; }
        }

        public String IndustryType
        {
            get { return (Convert.ToString(clDataRow["IndustryType"])); }
            set { clDataRow["IndustryType"] = value; }
        }
        
        public String EmailAddress
        {
            get { return (Convert.ToString(clDataRow["EmailAddress"])); }
            set { clDataRow["EmailAddress"] = value; }
        }
        
        public String ContactNo
        {
            get { return (Convert.ToString(clDataRow["ContactNo"])); }
            set { clDataRow["ContactNo"] = value; }
        }

        public String BuildingNo
        {
            get { return (Convert.ToString(clDataRow["BuildingNo"])); }
            set { clDataRow["BuildingNo"] = value; }
        }

        public String Floor
        {
            get { return (Convert.ToString(clDataRow["Floor"])); }
            set { clDataRow["Floor"] = value; }
        }

        public String RoomNo
        {
            get { return (Convert.ToString(clDataRow["RoomNo"])); }
            set { clDataRow["RoomNo"] = value; }
        }

        public String Street
        {
            get { return (Convert.ToString(clDataRow["Street"])); }
            set { clDataRow["Street"] = value; }
        }

        public String Quarter
        {
            get { return (Convert.ToString(clDataRow["Quarter"])); }
            set { clDataRow["Quarter"] = value; }
        }

        public String AddressInfo
        {
            get { return (Convert.ToString(clDataRow["AddressInfo"])); }
            set { clDataRow["AddressInfo"] = value; }
        }

        public String Township
        {
            get { return (Convert.ToString(clDataRow["Township"])); }
            set { clDataRow["Township"] = value; }
        }

        public String City
        {
            get { return (Convert.ToString(clDataRow["City"])); }
            set { clDataRow["City"] = value; }
        }

        public String Country
        {
            get { return (Convert.ToString(clDataRow["Country"])); }
            set { clDataRow["Country"] = value; }
        }

        public String Postal
        {
            get { return (Convert.ToString(clDataRow["Postal"])); }
            set { clDataRow["Postal"] = value; }
        }

        public String DisplayAddress
        {
            get { return (Convert.ToString(clDataRow["DisplayAddress"])); }
            set { clDataRow["DisplayAddress"] = value; }
        }

        public Decimal Longitude
        {
            get { return (Convert.ToDecimal(clDataRow["Longitude"])); }
            set { clDataRow["Longitude"] = value; }
        }

        public Decimal Latitude
        {
            get { return (Convert.ToDecimal(clDataRow["Latitude"])); }
            set { clDataRow["Latitude"] = value; }
        }

        public String OperatingHour
        {
            get { return (Convert.ToString(clDataRow["OperatingHour"])); }
            set { clDataRow["OperatingHour"] = value; }
        }
    }

    public class KeyPadRow
    {
        DataRow clDataRow;

        public KeyPadRow(DataRow paDataRow)
        {
            clDataRow = paDataRow;
        }

        public DataRow Row
        {
            get { return (clDataRow); }
            set { clDataRow = value; }
        }

        public object this[String paString]
        {
            get { return (clDataRow[paString]); }
            set { clDataRow[paString] = value; }
        }

        // Null Checking.
        public bool IsKeyPadNameNull { get { return (clDataRow["KeyPadName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsEditionNull { get { return (clDataRow["Edition"].GetType().ToString() == "System.DBNull"); } }
        public bool IsLanguageNull { get { return (clDataRow["Language"].GetType().ToString() == "System.DBNull"); } }
        public bool IsPanelNameNull { get { return (clDataRow["PanelName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsKeyOrderNull { get { return (clDataRow["KeyOrder"].GetType().ToString() == "System.DBNull"); } }
        public bool IsKeyTextNull { get { return (clDataRow["KeyText"].GetType().ToString() == "System.DBNull"); } }
        public bool IsKeyCommandNull { get { return (clDataRow["KeyCommand"].GetType().ToString() == "System.DBNull"); } }
        public bool IsCssClassNull { get { return (clDataRow["CssClass"].GetType().ToString() == "System.DBNull"); } }
        public bool IsInlineCssNull { get { return (clDataRow["InlineCss"].GetType().ToString() == "System.DBNull"); } }        

        public String KeyPadName
        {
            get { return (Convert.ToString(clDataRow["KeyPadName"])); }
            set { clDataRow["KeyPadName"] = value; }
        }

        public String Edition
        {
            get { return (Convert.ToString(clDataRow["Edition"])); }
            set { clDataRow["Edition"] = value; }
        }

        public String Language
        {
            get { return (Convert.ToString(clDataRow["Language"])); }
            set { clDataRow["Language"] = value; }
        }

        public String PanelName
        {
            get { return (Convert.ToString(clDataRow["PanelName"])); }
            set { clDataRow["PanelName"] = value; }
        }

        public int KeyOrder
        {
            get { return (Convert.ToInt32(clDataRow["KeyOrder"])); }
            set { clDataRow["KeyOrder"] = value; }
        }

        public String KeyText
        {
            get { return (Convert.ToString(clDataRow["KeyText"])); }
            set { clDataRow["KeyText"] = value; }
        }

        public String KeyCommand
        {
            get { return (Convert.ToString(clDataRow["KeyCommand"])); }
            set { clDataRow["KeyCommand"] = value; }
        }

        public String CssClass
        {
            get { return (Convert.ToString(clDataRow["CssClass"])); }
            set { clDataRow["CssClass"] = value; }
        }

        public String InlineCss
        {
            get { return (Convert.ToString(clDataRow["InlineCss"])); }
            set { clDataRow["InlineCss"] = value; }
        }       
    }

    public class SettingRow
    {
        DataRow clDataRow;

        public SettingRow(DataRow paDataRow)
        {
            clDataRow = paDataRow;
        }

        public DataRow Row
        {
            get { return (clDataRow); }
            set { clDataRow = value; }
        }

        public object this[String paString]
        {
            get { return (clDataRow[paString]); }
            set { clDataRow[paString] = value; }
        }

        // Null Checking.
        public bool IsLoginIDNull { get { return (clDataRow["LoginID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsSettingGroupNull { get { return (clDataRow["SettingGroup"].GetType().ToString() == "System.DBNull"); } }
        public bool IsSettingKeyNull { get { return (clDataRow["SettingKey"].GetType().ToString() == "System.DBNull"); } }
        public bool IsSettingValueNull { get { return (clDataRow["SettingValue"].GetType().ToString() == "System.DBNull"); } }        

        public String LoginID
        {
            get { return (Convert.ToString(clDataRow["LoginID"])); }
            set { clDataRow["LoginID"] = value; }
        }

        public String SettingGroup
        {
            get { return (Convert.ToString(clDataRow["SettingGroup"])); }
            set { clDataRow["SettingGroup"] = value; }
        }

        public String SettingKey
        {
            get { return (Convert.ToString(clDataRow["SettingKey"])); }
            set { clDataRow["SettingKey"] = value; }
        }

        public String SettingValue
        {
            get { return (Convert.ToString(clDataRow["SettingValue"])); }
            set { clDataRow["SettingValue"] = value; }
        }     
    }
}
