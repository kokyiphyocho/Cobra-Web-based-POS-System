using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using CobraFoundation;

namespace CobraBusinessFrame
{
    //******************************
    //* EServiceTableManager Class *
    //******************************
    public partial class EServiceTableManager
    {
        //array index of Column Name and Column Type
        const int ctTableName = 0;
        const int ctDataTable = 1;

        public enum TableType : int
        {
            Invalid             = -1,
            OrderInfo           = 0,
            OrderDetail         = 1,            
            POSReceipt          = 2,
            POSStakeHolder      = 3,
            POSStockIncoming    = 4,
            POSStockOutgoing    = 5,
        };

        object[,] arTables = {
                                {"EData_OrderInfo",null},
                                {"EData_OrderDetail",null},                              
                                {"EData_Receipt",null},
                                {"EData_StakeHolder",null},
                                {"EData_StockIncoming",null},
                                {"EData_StockOutgoing",null},
                             };

        private static EServiceTableManager clEServiceTableManager;

        //to make TableManager class as singleton
        public static EServiceTableManager GetInstance()
        {
            if (clEServiceTableManager == null) return (clEServiceTableManager = new EServiceTableManager());
            else return (clEServiceTableManager);
        }

        // Singleton Constructor.
        private EServiceTableManager() { }

        //Creating Tables
        private void CreateTable(TableType paTableType)
        {
            QueryClass lcQuery;

            lcQuery = new QueryClass(QueryClass.QueryType.GetSchema, QueryClass.ConnectionMode.EService);
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

    public class StandardItemCatalogueRow
    {
        DataRow clDataRow;

        public StandardItemCatalogueRow(DataRow paDataRow)
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
        public bool IsSubScriptionIDNull { get { return (clDataRow["SubScriptionID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsItemIDNull { get { return (clDataRow["ItemID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsStatusNull { get { return (clDataRow["Status"].GetType().ToString() == "System.DBNull"); } }
        public bool IsEntryTypeNull { get { return (clDataRow["EntryType"].GetType().ToString() == "System.DBNull"); } }
        public bool IsCategoryNull { get { return (clDataRow["Category"].GetType().ToString() == "System.DBNull"); } }
        public bool IsEntryNameNull { get { return (clDataRow["EntryName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDescriptionNull { get { return (clDataRow["Description"].GetType().ToString() == "System.DBNull"); } }
        public bool IsOption1Null { get { return (clDataRow["Option1"].GetType().ToString() == "System.DBNull"); } }
        public bool IsOption2Null { get { return (clDataRow["Option2"].GetType().ToString() == "System.DBNull"); } }
        public bool IsOption3Null { get { return (clDataRow["Option3"].GetType().ToString() == "System.DBNull"); } }
        public bool IsExStockNull { get { return (clDataRow["ExStock"].GetType().ToString() == "System.DBNull"); } }
        public bool IsPriceNull { get { return (clDataRow["Price"].GetType().ToString() == "System.DBNull"); } }
        public bool IsUsualPriceNull { get { return (clDataRow["UsualPrice"].GetType().ToString() == "System.DBNull"); } }
        
        public String SubscriptionID
        {
            get { return (Convert.ToString(clDataRow["SubscriptionID"])); }
            set { clDataRow["SubscriptionID"] = value; }
        }

        public int ItemID
        {
            get { return (Convert.ToInt32(clDataRow["ItemID"])); }
            set { clDataRow["ItemID"] = value; }
        }

        public String Status
        {
            get { return (Convert.ToString(clDataRow["Status"])); }
            set { clDataRow["Status"] = value; }
        }

        public String EntryType
        {
            get { return (Convert.ToString(clDataRow["EntryType"])); }
            set { clDataRow["EntryType"] = value; }
        }
        
        public int Category
        {
            get { return (Convert.ToInt32(clDataRow["Category"])); }
            set { clDataRow["Category"] = value; }
        }

        public String EntryName
        {
            get { return (Convert.ToString(clDataRow["EntryName"])); }
            set { clDataRow["EntryName"] = value; }
        }

        public String Description
        {
            get { return (Convert.ToString(clDataRow["Description"])); }
            set { clDataRow["Description"] = value; }
        }

        public String Option1
        {
            get { return (Convert.ToString(clDataRow["Option1"])); }
            set { clDataRow["Option1"] = value; }
        }

        public String Option2
        {
            get { return (Convert.ToString(clDataRow["Option2"])); }
            set { clDataRow["Option2"] = value; }
        }

        public String Option3
        {
            get { return (Convert.ToString(clDataRow["Option3"])); }
            set { clDataRow["Option3"] = value; }
        }

        public String ExStock
        {
            get { return (Convert.ToString(clDataRow["ExStock"])); }
            set { clDataRow["ExStock"] = value; }
        }

        public Decimal Price
        {
            get { return (Convert.ToDecimal(clDataRow["Price"])); }
            set { clDataRow["Price"] = value; }
        }

        public Decimal UsualPrice
        {
            get { return (Convert.ToDecimal(clDataRow["UsualPrice"])); }
            set { clDataRow["UsualPrice"] = value; }
        }
    }

    public class MobileStoreCatalogueRow
    {
        DataRow clDataRow;

        public MobileStoreCatalogueRow(DataRow paDataRow)
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
        public bool IsSubScriptionIDNull { get { return (clDataRow["SubScriptionID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsEntryIDNull { get { return (clDataRow["EntryID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsEntryStatusNull { get { return (clDataRow["EntryStatus"].GetType().ToString() == "System.DBNull"); } }
        public bool IsStockIDNull { get { return (clDataRow["StockID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsProductUIDNull { get { return (clDataRow["ProductUID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsManufacturerNull { get { return (clDataRow["Manufacturer"].GetType().ToString() == "System.DBNull"); } }
        public bool IsGroupNameNull { get { return (clDataRow["GroupName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsCategoryNull { get { return (clDataRow["Category"].GetType().ToString() == "System.DBNull"); } }
        public bool IsKindNull { get { return (clDataRow["Kind"].GetType().ToString() == "System.DBNull"); } }
        public bool IsPriorityNull { get { return (clDataRow["Priority"].GetType().ToString() == "System.DBNull"); } }
        public bool IsSortKeyNull { get { return (clDataRow["SortKey"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDescriptionNull { get { return (clDataRow["Description"].GetType().ToString() == "System.DBNull"); } }
        public bool IsCurrencyNull { get { return (clDataRow["Currency"].GetType().ToString() == "System.DBNull"); } }
        public bool IsPriceNull { get { return (clDataRow["Price"].GetType().ToString() == "System.DBNull"); } }
        public bool IsUsualPriceNull { get { return (clDataRow["UsualPrice"].GetType().ToString() == "System.DBNull"); } }
        public bool IsStockQuantityNull { get { return (clDataRow["StockQuantity"].GetType().ToString() == "System.DBNull"); } }
        public bool IsStockUnitNull { get { return (clDataRow["StockUnit"].GetType().ToString() == "System.DBNull"); } }
        public bool IsStockInfoNull { get { return (clDataRow["StockInfo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsOptionInfoNull { get { return (clDataRow["OptionInfo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsAdditionalInfoNull { get { return (clDataRow["AdditionalInfo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsImagesNull { get { return (clDataRow["Images"].GetType().ToString() == "System.DBNull"); } }
        public bool IsTagNull { get { return (clDataRow["Images"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsRemarkNull { get { return (clDataRow["Remark"].GetType().ToString() == "System.DBNull"); } }
        public bool IsLastUpdateTimeNull { get { return (clDataRow["LastUpdateTime"].GetType().ToString() == "System.DBNull"); } }

        public String SubscriptionID
        {
            get { return (Convert.ToString(clDataRow["SubscriptionID"])); }
            set { clDataRow["SubscriptionID"] = value; }
        }

        public int EntryID
        {
            get { return (Convert.ToInt32(clDataRow["EntryID"])); }
            set { clDataRow["EntryID"] = value; }
        }

        public String EntryStatus
        {
            get { return (Convert.ToString(clDataRow["EntryStatus"])); }
            set { clDataRow["EntryStatus"] = value; }
        }

        public String StockID
        {
            get { return (Convert.ToString(clDataRow["StockID"])); }
            set { clDataRow["StockID"] = value; }
        }       

        public String ProductUID
        {
            get { return (Convert.ToString(clDataRow["ProductUID"])); }
            set { clDataRow["ProductUID"] = value; }
        }

        public String GroupName
        {
            get { return (Convert.ToString(clDataRow["GroupName"])); }
            set { clDataRow["GroupName"] = value; }
        }

        public String Category
        {
            get { return (Convert.ToString(clDataRow["Category"])); }
            set { clDataRow["Category"] = value; }
        }

        public String Kind
        {
            get { return (Convert.ToString(clDataRow["Kind"])); }
            set { clDataRow["Kind"] = value; }
        }

        public int Priority
        {
            get { return (Convert.ToInt32(clDataRow["Priority"])); }
            set { clDataRow["Priority"] = value; }
        }

        public String SortKey
        {
            get { return (Convert.ToString(clDataRow["SortKey"])); }
            set { clDataRow["SortKey"] = value; }
        }

        public String Description
        {
            get { return (Convert.ToString(clDataRow["Description"])); }
            set { clDataRow["Description"] = value; }
        }

        public String Currency
        {
            get { return (Convert.ToString(clDataRow["Currency"])); }
            set { clDataRow["Currency"] = value; }
        }

        public Decimal Price
        {
            get { return (Convert.ToDecimal(clDataRow["Price"])); }
            set { clDataRow["Price"] = value; }
        }

        public Decimal UsualPrice
        {
            get { return (Convert.ToDecimal(clDataRow["UsualPrice"])); }
            set { clDataRow["UsualPrice"] = value; }
        }

        public int StockQuantity
        {
            get { return (Convert.ToInt32(clDataRow["StockQuantity"])); }
            set { clDataRow["StockQuantity"] = value; }
        }

        public String StockUnit
        {
            get { return (Convert.ToString(clDataRow["StockUnit"])); }
            set { clDataRow["StockUnit"] = value; }
        }

        public String StockInfo
        {
            get { return (Convert.ToString(clDataRow["StockInfo"])); }
            set { clDataRow["StockInfo"] = value; }
        }

        public String OptionInfo
        {
            get { return (Convert.ToString(clDataRow["OptionInfo"])); }
            set { clDataRow["OptionInfo"] = value; }
        }

        public String AdditionalInfo
        {
            get { return (Convert.ToString(clDataRow["AdditionalInfo"])); }
            set { clDataRow["AdditionalInfo"] = value; }
        }

        public String Images
        {
            get { return (Convert.ToString(clDataRow["Images"])); }
            set { clDataRow["Images"] = value; }
        }

        public String Tag
        {
            get { return (Convert.ToString(clDataRow["Tag"])); }
            set { clDataRow["Tag"] = value; }
        }

        public String Remark
        {
            get { return (Convert.ToString(clDataRow["Remark"])); }
            set { clDataRow["Remark"] = value; }
        }

        public DateTime LastUpdateTime
        {
            get { return (Convert.ToDateTime(clDataRow["LastUpdateTime"])); }
            set { clDataRow["LastUpdateTime"] = value; }
        }
    }

    public class OrderInfoRow
    {
        DataRow clDataRow;

        public OrderInfoRow(DataRow paDataRow)
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
        public bool IsOrderNoNull { get { return (clDataRow["OrderNo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsOrderDateNull { get { return (clDataRow["OrderDate"].GetType().ToString() == "System.DBNull"); } }
        public bool IsOrderStatusNull { get { return (clDataRow["OrderStatus"].GetType().ToString() == "System.DBNull"); } }
        public bool IsShippingDateNull { get { return (clDataRow["ShippingDate"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDeliveryDateNull { get { return (clDataRow["DeliveryDate"].GetType().ToString() == "System.DBNull"); } }
        public bool IsConfirmDateNull { get { return (clDataRow["ConfirmDate"].GetType().ToString() == "System.DBNull"); } }
        public bool IsShippingChargeNull { get { return (clDataRow["ShippingCharge"].GetType().ToString() == "System.DBNull"); } }
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
        public bool IsOrderRemarkNull { get { return (clDataRow["OrderRemark"].GetType().ToString() == "System.DBNull"); } }
        public bool IsRejectReasonNull { get { return (clDataRow["RejectReason"].GetType().ToString() == "System.DBNull"); } }

        public int OrderNo
        {
            get { return (Convert.ToInt32(clDataRow["OrderNo"])); }
            set { clDataRow["OrderNo"] = value; }
        }

        public DateTime OrderDate
        {
            get { return (Convert.ToDateTime(clDataRow["OrderDate"])); }
            set { clDataRow["OrderDate"] = value; }
        }

        public int OrderStatus
        {
            get { return (Convert.ToInt32(clDataRow["OrderStatus"])); }
            set { clDataRow["OrderStatus"] = value; }
        }

        public DateTime ShippingDate
        {
            get { return (Convert.ToDateTime(clDataRow["ShippingDate"])); }
            set { clDataRow["ShippingDate"] = value; }
        }

        public DateTime DeliveryDate
        {
            get { return (Convert.ToDateTime(clDataRow["DeliveryDate"])); }
            set { clDataRow["DeliveryDate"] = value; }
        }

        public DateTime ConfirmDate
        {
            get { return (Convert.ToDateTime(clDataRow["ConfirmDate"])); }
            set { clDataRow["ConfirmDate"] = value; }
        }

        public Decimal ShippingCharge
        {
            get { return (Convert.ToDecimal(clDataRow["ShippingCharge"])); }
            set { clDataRow["ShippingCharge"] = value; }
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

        public String OrderRemark
        {
            get { return (Convert.ToString(clDataRow["OrderRemark"])); }
            set { clDataRow["OrderRemark"] = value; }
        }

        public String RejectReason
        {
            get { return (Convert.ToString(clDataRow["RejectReason"])); }
            set { clDataRow["RejectReason"] = value; }
        }
    }

    public class OrderDetailRow
    {
        DataRow clDataRow;

        public OrderDetailRow(DataRow paDataRow)
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
        public bool IsOrderNoNull { get { return (clDataRow["OrderNo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsStatusNull { get { return (clDataRow["Status"].GetType().ToString() == "System.DBNull"); } }
        public bool IsItemIDNull { get { return (clDataRow["ItemID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsItemNameNull { get { return (clDataRow["ItemName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsQuantityNull { get { return (clDataRow["Quantity"].GetType().ToString() == "System.DBNull"); } }
        public bool IsUnitPriceNull { get { return (clDataRow["UnitPrice"].GetType().ToString() == "System.DBNull"); } }
        public bool IsRemarkNull { get { return (clDataRow["Remark"].GetType().ToString() == "System.DBNull"); } }
        public bool IsRejectReasonNull { get { return (clDataRow["RejectReason"].GetType().ToString() == "System.DBNull"); } }

        public int OrderNo
        {
            get { return (Convert.ToInt32(clDataRow["OrderNo"])); }
            set { clDataRow["OrderNo"] = value; }
        }

        public int Status
        {
            get { return (Convert.ToInt32(clDataRow["Status"])); }
            set { clDataRow["Status"] = value; }
        }

        public int ItemID
        {
            get { return (Convert.ToInt32(clDataRow["ItemID"])); }
            set { clDataRow["ItemID"] = value; }
        }

        public String ItemName
        {
            get { return (Convert.ToString(clDataRow["ItemName"])); }
            set { clDataRow["ItemName"] = value; }
        }

        public int Quantity
        {
            get { return (Convert.ToInt32(clDataRow["Quantity"])); }
            set { clDataRow["Quantity"] = value; }
        }

        public Decimal UnitPrice
        {
            get { return (Convert.ToInt32(clDataRow["UnitPrice"])); }
            set { clDataRow["UnitPrice"] = value; }
        }

        public String Remark
        {
            get { return (Convert.ToString(clDataRow["Remark"])); }
            set { clDataRow["Remark"] = value; }
        }

        public String RejectReason
        {
            get { return (Convert.ToString(clDataRow["RejectReason"])); }
            set { clDataRow["RejectReason"] = value; }
        }
    }

    public class POSItemCatalogueRow
    {
        DataRow clDataRow;

        public POSItemCatalogueRow(DataRow paDataRow)
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
        public bool IsSubScriptionIDNull { get { return (clDataRow["SubScriptionID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsLoginIDNull { get { return (clDataRow["LoginID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsAccessInfoNull { get { return (clDataRow["AccessInfo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsItemIDNull { get { return (clDataRow["ItemID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsStatusNull { get { return (clDataRow["Status"].GetType().ToString() == "System.DBNull"); } }
        public bool IsEntryTypeNull { get { return (clDataRow["EntryType"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsCategoryNull { get { return (clDataRow["Category"].GetType().ToString() == "System.DBNull"); } }
        public bool IsSortOrderNull { get { return (clDataRow["SortOrder"].GetType().ToString() == "System.DBNull"); } }
        public bool IsItemCodeNull { get { return (clDataRow["ItemCode"].GetType().ToString() == "System.DBNull"); } }
        public bool IsItemNameNull { get { return (clDataRow["ItemName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDescriptionNull { get { return (clDataRow["Description"].GetType().ToString() == "System.DBNull"); } }
        public bool IsMajorUnitNameNull { get { return (clDataRow["MajorUnitName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsMinorUnitNameNull { get { return (clDataRow["MinorUnitName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsAllowDecimalQuantityNull { get { return (clDataRow["AllowDecimalQuantity"].GetType().ToString() == "System.DBNull"); } }
        public bool IsUnitRelationshipNull { get { return (clDataRow["UnitRelationship"].GetType().ToString() == "System.DBNull"); } }
        public bool IsMajorPriceNull { get { return (clDataRow["MajorPrice"].GetType().ToString() == "System.DBNull"); } }
        public bool IsMinorPriceNull { get { return (clDataRow["MinorPrice"].GetType().ToString() == "System.DBNull"); } }
        public bool IsMajorMSPNull { get { return (clDataRow["MajorMSP"].GetType().ToString() == "System.DBNull"); } }
        public bool IsMinorMSPNull { get { return (clDataRow["MinorMSP"].GetType().ToString() == "System.DBNull"); } }
        public bool IsTaxPercentNull { get { return (clDataRow["TaxPercent"].GetType().ToString() == "System.DBNull"); } }
        public bool IsReOrderQuantityNull { get { return (clDataRow["ReOrderQuantity"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDownLinkIDNull { get { return (clDataRow["DownLinkID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDownLinkRelationshipNull { get { return (clDataRow["DownLinkRelationship"].GetType().ToString() == "System.DBNull"); } }
        public bool IsFavouriteNull { get { return (clDataRow["Favourite"].GetType().ToString() == "System.DBNull"); } }
        public bool IsRemarkNull { get { return (clDataRow["Remark"].GetType().ToString() == "System.DBNull"); } }
        public bool IsTagNull { get { return (clDataRow["Tag"].GetType().ToString() == "System.DBNull"); } }        

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

        public String AccessInfo
        {
            get { return (Convert.ToString(clDataRow["AccessInfo"])); }
            set { clDataRow["AccessInfo"] = value; }
        }

        public int ItemID
        {
            get { return (Convert.ToInt32(clDataRow["ItemID"])); }
            set { clDataRow["ItemID"] = value; }
        }

        public String Status
        {
            get { return (Convert.ToString(clDataRow["Status"])); }
            set { clDataRow["Status"] = value; }
        }

        public String EntryType
        {
            get { return (Convert.ToString(clDataRow["EntryType"])); }
            set { clDataRow["EntryType"] = value; }
        }

        public String EntryAttribute
        {
            get { return (Convert.ToString(clDataRow["EntryAttribute"])); }
            set { clDataRow["EntryAttribute"] = value; }
        }

        public int Category
        {
            get { return (Convert.ToInt32(clDataRow["Category"])); }
            set { clDataRow["Category"] = value; }
        }

        public int SortOrder
        {
            get { return (Convert.ToInt32(clDataRow["SortOrder"])); }
            set { clDataRow["SortOrder"] = value; }
        }        

        public String ItemCode
        {
            get { return (Convert.ToString(clDataRow["ItemCode"])); }
            set { clDataRow["ItemCode"] = value; }
        }

        public String ItemName
        {
            get { return (Convert.ToString(clDataRow["ItemName"])); }
            set { clDataRow["ItemName"] = value; }
        }

        public String Description
        {
            get { return (Convert.ToString(clDataRow["Description"])); }
            set { clDataRow["Description"] = value; }
        }

        public String MajorUnitName
        {
            get { return (Convert.ToString(clDataRow["MajorUnitName"])); }
            set { clDataRow["MajorUnitName"] = value; }
        }

        public String MinorUnitName
        {
            get { return (Convert.ToString(clDataRow["MinorUnitName"])); }
            set { clDataRow["MinorUnitName"] = value; }
        }

        public int AllowDecimalQuantity
        {
            get { return (Convert.ToInt32(clDataRow["AllowDecimalQuantity"])); }
            set { clDataRow["AllowDecimalQuantity"] = value; }
        }

        public int UnitRelationship
        {
            get { return (Convert.ToInt32(clDataRow["UnitRelationship"])); }
            set { clDataRow["UnitRelationship"] = value; }
        }

        public Decimal Cost
        {
            get { return (Convert.ToDecimal(clDataRow["Cost"])); }
            set { clDataRow["Cost"] = value; }
        }

        public Decimal MajorPrice
        {
            get { return (Convert.ToDecimal(clDataRow["MajorPrice"])); }
            set { clDataRow["MajorPrice"] = value; }
        }

        public Decimal MinorPrice
        {
            get { return (Convert.ToDecimal(clDataRow["MinorPrice"])); }
            set { clDataRow["MinorPrice"] = value; }
        }

        public Decimal MajorMSP
        {
            get { return (Convert.ToDecimal(clDataRow["MajorMSP"])); }
            set { clDataRow["MajorMSP"] = value; }
        }

        public Decimal MinorMSP
        {
            get { return (Convert.ToDecimal(clDataRow["MinorMSP"])); }
            set { clDataRow["MinorMSP"] = value; }
        }

        public Decimal TaxPercent
        {
            get { return (Convert.ToDecimal(clDataRow["TaxPercent"])); }
            set { clDataRow["TaxPercent"] = value; }
        }

        public Decimal ReOrderQuantity
        {
            get { return (Convert.ToDecimal(clDataRow["ReOrderQuantity"])); }
            set { clDataRow["ReOrderQuantity"] = value; }
        }

        public int DownLinkID
        {
            get { return (Convert.ToInt32(clDataRow["DownLinkID"])); }
            set { clDataRow["DownLinkID"] = value; }
        }

        public int DownLinkRelationship
        {
            get { return (Convert.ToInt32(clDataRow["DownLinkRelationship"])); }
            set { clDataRow["DownLinkRelationship"] = value; }
        }

        public bool Favourite
        {
            get { return (Convert.ToBoolean(clDataRow["Favourite"])); }
            set { clDataRow["Favourite"] = value; }
        }

        public String Remark
        {
            get { return (Convert.ToString(clDataRow["Remark"])); }
            set { clDataRow["Remark"] = value; }
        }

        public String Tag
        {
            get { return (Convert.ToString(clDataRow["Tag"])); }
            set { clDataRow["Tag"] = value; }
        }
    }

    public class POSUnitRow
    {
        DataRow clDataRow;

        public POSUnitRow(DataRow paDataRow)
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
        public bool IsSubScriptionIDNull { get { return (clDataRow["SubScriptionID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsLoginIDNull { get { return (clDataRow["LoginID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsAccessInfoNull { get { return (clDataRow["AccessInfo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsUnitIDNull { get { return (clDataRow["UnitID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsStatusNull { get { return (clDataRow["Status"].GetType().ToString() == "System.DBNull"); } }
        public bool IsLanguageNull { get { return (clDataRow["Language"].GetType().ToString() == "System.DBNull"); } }
        public bool IsUnitTypeNull { get { return (clDataRow["UnitType"].GetType().ToString() == "System.DBNull"); } }
        public bool IsUnitOrderNull { get { return (clDataRow["UnitOrder"].GetType().ToString() == "System.DBNull"); } }
        public bool IsUnitNameNull { get { return (clDataRow["UnitName"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsMajorUnitNameNull { get { return (clDataRow["MajorUnitName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsMinorUnitNameNull { get { return (clDataRow["MinorUnitName"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsUnitRelationshipNull { get { return (clDataRow["UnitRelationship"].GetType().ToString() == "System.DBNull"); } }
        public bool IsAdjustableRelationshipNull { get { return (clDataRow["AdjustableRelationship"].GetType().ToString() == "System.DBNull"); } }
        public bool IsAllowDecimalQuantityNull { get { return (clDataRow["AllowDecimalQuantity"].GetType().ToString() == "System.DBNull"); } }
        
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

        public String AccessInfo    
        {
            get { return (Convert.ToString(clDataRow["AccessInfo"])); }
            set { clDataRow["AccessInfo"] = value; }
        }

        public int UnitID
        {
            get { return (Convert.ToInt32(clDataRow["UnitID"])); }
            set { clDataRow["UnitID"] = value; }
        }

        public String Status
        {
            get { return (Convert.ToString(clDataRow["Status"])); }
            set { clDataRow["Status"] = value; }
        }

        public String Language
        {
            get { return (Convert.ToString(clDataRow["Language"])); }
            set { clDataRow["Language"] = value; }
        }

        public String UnitType
        {
            get { return (Convert.ToString(clDataRow["UnitType"])); }
            set { clDataRow["UnitType"] = value; }
        }

        public int UnitOrder
        {
            get { return (Convert.ToInt32(clDataRow["UnitOrder"])); }
            set { clDataRow["UnitOrder"] = value; }
        }
        
        public String UnitName
        {
            get { return (Convert.ToString(clDataRow["UnitName"])); }
            set { clDataRow["UnitName"] = value; }
        }

        public String MajorUnitName
        {
            get { return (Convert.ToString(clDataRow["MajorUnitName"])); }
            set { clDataRow["MajorUnitName"] = value; }
        }

        public String MinorUnitName
        {
            get { return (Convert.ToString(clDataRow["MinorUnitName"])); }
            set { clDataRow["MinorUnitName"] = value; }
        }

        public int UnitRelationship
        {
            get { return (Convert.ToInt32(clDataRow["UnitRelationship"])); }
            set { clDataRow["UnitRelationship"] = value; }
        }

        public int AdjustableRelationship
        {
            get { return (Convert.ToInt32(clDataRow["AdjustableRelationship"])); }
            set { clDataRow["AdjustableRelationship"] = value; }
        }

        public int AllowDecimalQuantity
        {
            get { return (Convert.ToInt32(clDataRow["AllowDecimalQuantity"])); }
            set { clDataRow["AllowDecimalQuantity"] = value; }
        }
    }

    public class POSTableListRow
    {
        DataRow clDataRow;

        public POSTableListRow(DataRow paDataRow)
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
        public bool IsSubScriptionIDNull { get { return (clDataRow["SubScriptionID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsUserIDNull { get { return (clDataRow["UserID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsLoginIDNull { get { return (clDataRow["LoginID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsAccessInfoNull { get { return (clDataRow["AccessInfo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsTableIDNull { get { return (clDataRow["TableID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsStatusNull { get { return (clDataRow["Status"].GetType().ToString() == "System.DBNull"); } }
        public bool IsEntryTypeNull { get { return (clDataRow["EntryType"].GetType().ToString() == "System.DBNull"); } }
        public bool IsGroupIDNull { get { return (clDataRow["GroupID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDisplayNameNull { get { return (clDataRow["DisplayName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsCapacityNull { get { return (clDataRow["Capacity"].GetType().ToString() == "System.DBNull"); } }
        public bool IsSortOrderNull { get { return (clDataRow["SortOrder"].GetType().ToString() == "System.DBNull"); } }
        public bool IsTableChargeNull { get { return (clDataRow["TableCharge"].GetType().ToString() == "System.DBNull"); } }
        public bool IsTableChargeModeNull { get { return (clDataRow["TableChargeMode"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDescriptionNull { get { return (clDataRow["Description"].GetType().ToString() == "System.DBNull"); } }
        
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

        public String AccessInfo
        {
            get { return (Convert.ToString(clDataRow["AccessInfo"])); }
            set { clDataRow["AccessInfo"] = value; }
        }

        public int TableID
        {
            get { return (Convert.ToInt32(clDataRow["TableID"])); }
            set { clDataRow["TableID"] = value; }
        }

        public String Status
        {
            get { return (Convert.ToString(clDataRow["Status"])); }
            set { clDataRow["Status"] = value; }
        }

        public String EntryType
        {
            get { return (Convert.ToString(clDataRow["EntryType"])); }
            set { clDataRow["EntryType"] = value; }
        }

        public int GroupID
        {
            get { return (Convert.ToInt32(clDataRow["GroupID"])); }
            set { clDataRow["GroupID"] = value; }
        }

        public String DisplayName
        {
            get { return (Convert.ToString(clDataRow["DisplayName"])); }
            set { clDataRow["DisplayName"] = value; }
        }

        public int Capacity
        {
            get { return (Convert.ToInt32(clDataRow["Capacity"])); }
            set { clDataRow["Capacity"] = value; }
        }

        public int SortOrder
        {
            get { return (Convert.ToInt32(clDataRow["SortOrder"])); }
            set { clDataRow["SortOrder"] = value; }
        }

        public String UnitName
        {
            get { return (Convert.ToString(clDataRow["UnitName"])); }
            set { clDataRow["UnitName"] = value; }
        }

        public Decimal TableCharge
        {
            get { return (Convert.ToDecimal(clDataRow["TableCharge"])); }
            set { clDataRow["TableCharge"] = value; }
        }

        public String TableChargeMode
        {
            get { return (Convert.ToString(clDataRow["TableChargeMode"])); }
            set { clDataRow["TableChargeMode"] = value; }
        }

        public String Description
        {
            get { return (Convert.ToString(clDataRow["Description"])); }
            set { clDataRow["Description"] = value; }
        }
      
    }

    public class POSReceiptRow
    {
        DataRow clDataRow;

        public POSReceiptRow(DataRow paDataRow)
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
        public bool IsSubScriptionIDNull { get { return (clDataRow["SubScriptionID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsLoginIDNull { get { return (clDataRow["LoginID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsAccessInfoNull { get { return (clDataRow["AccessInfo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsReceiptIDNull { get { return (clDataRow["ReceiptID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsEntryTypeNull { get { return (clDataRow["EntryType"].GetType().ToString() == "System.DBNull"); } }
        public bool IsReceiptNoNull { get { return (clDataRow["ReceiptNo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsReceiptDateNull { get { return (clDataRow["ReceiptDate"].GetType().ToString() == "System.DBNull"); } }
        public bool IsTaxInclusiveNull { get { return (clDataRow["TaxInclusive"].GetType().ToString() == "System.DBNull"); } }
        public bool IsStatusNull { get { return (clDataRow["Status"].GetType().ToString() == "System.DBNull"); } }
        public bool IsStakeHolderIDNull { get { return (clDataRow["StakeHolderID"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsReferenceNull { get { return (clDataRow["Reference"].GetType().ToString() == "System.DBNull"); } }
        public bool IsPaymentCashNull { get { return (clDataRow["PaymentCash"].GetType().ToString() == "System.DBNull"); } }
        public bool IsPaymentBankNull { get { return (clDataRow["PaymentBank"].GetType().ToString() == "System.DBNull"); } }
        public bool IsPaymentCreditNull { get { return (clDataRow["PaymentCredit"].GetType().ToString() == "System.DBNull"); } }
        public bool IsPaymentContraNull { get { return (clDataRow["PaymentContra"].GetType().ToString() == "System.DBNull"); } }
        public bool IsLastModifiedNull { get { return (clDataRow["LastModified"].GetType().ToString() == "System.DBNull"); } }        

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

        public String AccessInfo
        {
            get { return (Convert.ToString(clDataRow["AccessInfo"])); }
            set { clDataRow["AccessInfo"] = value; }
        }

        public int ReceiptID
        {
            get { return (Convert.ToInt32(clDataRow["ReceiptID"])); }
            set { clDataRow["ReceiptID"] = value; }
        }

        public String ReceiptType
        {
            get { return (Convert.ToString(clDataRow["ReceiptType"])); }
            set { clDataRow["ReceiptType"] = value; }
        }

        public int ReceiptNo
        {
            get { return (Convert.ToInt32(clDataRow["ReceiptNo"])); }
            set { clDataRow["ReceiptNo"] = value; }
        }

        public DateTime ReceiptDate
        {
            get { return (Convert.ToDateTime(clDataRow["ReceiptDate"])); }
            set { clDataRow["ReceiptDate"] = value; }
        }

        public Decimal TaxPercent
        {
            get { return (Convert.ToDecimal(clDataRow["TaxPercent"])); }
            set { clDataRow["TaxPercent"] = value; }
        }

        public bool TaxInclusive
        {
            get { return (Convert.ToBoolean(clDataRow["TaxInclusive"])); }
            set { clDataRow["TaxInclusive"] = value; }
        }

        public String Status
        {
            get { return (Convert.ToString(clDataRow["Status"])); }
            set { clDataRow["Status"] = value; }
        }

        public int StakeHolderID
        {
            get { return (Convert.ToInt32(clDataRow["StakeHolderID"])); }
            set { clDataRow["StakeHolderID"] = value; }
        }

        public String Reference
        {
            get { return (Convert.ToString(clDataRow["Reference"])); }
            set { clDataRow["Reference"] = value; }
        }

        public Decimal PaymentCash
        {
            get { return (Convert.ToDecimal(clDataRow["PaymentCash"])); }
            set { clDataRow["PaymentCash"] = value; }
        }

        public Decimal PaymentBank
        {
            get { return (Convert.ToDecimal(clDataRow["PaymentBank"])); }
            set { clDataRow["PaymentBank"] = value; }
        }

        public Decimal PaymentCredit
        {
            get { return (Convert.ToDecimal(clDataRow["PaymentCredit"])); }
            set { clDataRow["PaymentCredit"] = value; }
        }

        public Decimal PaymentContra
        {
            get { return (Convert.ToDecimal(clDataRow["PaymentContra"])); }
            set { clDataRow["PaymentContra"] = value; }
        }

        public DateTime LastModified
        {
            get { return (Convert.ToDateTime(clDataRow["LastModified"])); }
            set { clDataRow["LastModified"] = value; }
        }
    }

    public class POSStakeHolderRow
    {
        DataRow clDataRow;

        public POSStakeHolderRow(DataRow paDataRow)
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
        public bool IsSubScriptionIDNull    { get { return (clDataRow["SubScriptionID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsLoginIDNull           { get { return (clDataRow["LoginID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsAccessInfoNull        { get { return (clDataRow["AccessInfo"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsStakeHolderIDNull     { get { return (clDataRow["StakeHolderID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsCodeNoNull            { get { return (clDataRow["CodeNo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsNameNull              { get { return (clDataRow["Name"].GetType().ToString() == "System.DBNull"); } }
        public bool IsContactNoNull         { get { return (clDataRow["ContactNo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsAddressNull           { get { return (clDataRow["Address"].GetType().ToString() == "System.DBNull"); } }        

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

        public String AccessInfo
        {
            get { return (Convert.ToString(clDataRow["AccessInfo"])); }
            set { clDataRow["AccessInfo"] = value; }
        }

        public int StakeHolderID
        {
            get { return (Convert.ToInt32(clDataRow["StakeHolderID"])); }
            set { clDataRow["StakeHolderID"] = value; }
        }

        public String CodeNo
        {
            get { return (Convert.ToString(clDataRow["CodeNo"])); }
            set { clDataRow["CodeNo"] = value; }
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

        public String Address
        {
            get { return (Convert.ToString(clDataRow["Address"])); }
            set { clDataRow["Address"] = value; }
        }
    }

    public class POSStockIORow
    {
        DataRow clDataRow;

        public POSStockIORow(DataRow paDataRow)
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
        public bool IsSubScriptionIDNull { get { return (clDataRow["SubScriptionID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsLoginIDNull { get { return (clDataRow["LoginID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsAccessInfoNull { get { return (clDataRow["AccessInfo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsIncomingIDNull { get { return (clDataRow["IncomingID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsOutgoingIDNull { get { return (clDataRow["OutgoingID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsReceiptIDNull { get { return (clDataRow["ReceiptID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsStatusNull { get { return (clDataRow["Status"].GetType().ToString() == "System.DBNull"); } }        
        public bool IsReferenceNull { get { return (clDataRow["Reference"].GetType().ToString() == "System.DBNull"); } }
        public bool IsItemIDNull { get { return (clDataRow["ItemID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsQuantityNull { get { return (clDataRow["Quantity"].GetType().ToString() == "System.DBNull"); } }
        public bool IsUnitModeNull { get { return (clDataRow["UnitMode"].GetType().ToString() == "System.DBNull"); } }
        public bool IsUnitPriceNull { get { return (clDataRow["UnitPrice"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDiscountNull { get { return (clDataRow["Discount"].GetType().ToString() == "System.DBNull"); } }
        public bool IsRemarkNull { get { return (clDataRow["Remark"].GetType().ToString() == "System.DBNull"); } }

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

        public String AccessInfo
        {
            get { return (Convert.ToString(clDataRow["AccessInfo"])); }
            set { clDataRow["AccessInfo"] = value; }
        }

        public int IncomingID
        {
            get { return (Convert.ToInt32(clDataRow["IncomingID"])); }
            set { clDataRow["IncomingID"] = value; }
        }

        public int ReceiptID
        {
            get { return (Convert.ToInt32(clDataRow["ReceiptID"])); }
            set { clDataRow["ReceiptID"] = value; }
        }     

        public String Status
        {
            get { return (Convert.ToString(clDataRow["Status"])); }
            set { clDataRow["Status"] = value; }
        }

        public String Reference
        {
            get { return (Convert.ToString(clDataRow["Reference"])); }
            set { clDataRow["Reference"] = value; }
        }

        public int ItemID
        {
            get { return (Convert.ToInt32(clDataRow["ItemID"])); }
            set { clDataRow["ItemID"] = value; }
        }

        public Decimal Quantity 
        {
            get { return (Convert.ToDecimal(clDataRow["Quantity"])); }
            set { clDataRow["Quantity"] = value; }
        }

        public Decimal UnitMode
        {
            get { return (Convert.ToDecimal(clDataRow["UnitMode"])); }
            set { clDataRow["UnitMode"] = value; }
        }

        public Decimal UnitPrice
        {
            get { return (Convert.ToDecimal(clDataRow["UnitPrice"])); }
            set { clDataRow["UnitPrice"] = value; }
        }

        public Decimal Discount
        {
            get { return (Convert.ToDecimal(clDataRow["Discount"])); }
            set { clDataRow["Discount"] = value; }
        }

        public String Remark
        {
            get { return (Convert.ToString(clDataRow["Remark"])); }
            set { clDataRow["Remark"] = value; }
        }
    }

    public class POSTransactionListRow
    {
        DataRow clDataRow;

        public POSTransactionListRow(DataRow paDataRow)
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
        public bool IsSerialNull { get { return (clDataRow["Serial"].GetType().ToString() == "System.DBNull"); } }
        public bool IsItemIDNull { get { return (clDataRow["ItemID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsItemStatusNull { get { return (clDataRow["ItemStatus"].GetType().ToString() == "System.DBNull"); } }
        public bool IsItemCodeNull { get { return (clDataRow["ItemCode"].GetType().ToString() == "System.DBNull"); } }
        public bool IsItemNameNull { get { return (clDataRow["ItemName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDescriptionNull { get { return (clDataRow["Description"].GetType().ToString() == "System.DBNull"); } }
        public bool IsUnitModeNull { get { return (clDataRow["UnitMode"].GetType().ToString() == "System.DBNull"); } }
        public bool IsUnitNameNull { get { return (clDataRow["UnitName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsUnitRelationshipNull { get { return (clDataRow["UnitRelationship"].GetType().ToString() == "System.DBNull"); } }
        public bool IsQuantityNull { get { return (clDataRow["Quantity"].GetType().ToString() == "System.DBNull"); } }
        public bool IsUnitPriceNull { get { return (clDataRow["UnitPrice"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDiscountNull { get { return (clDataRow["Discount"].GetType().ToString() == "System.DBNull"); } }
        public bool IsRemarkNull { get { return (clDataRow["Remark"].GetType().ToString() == "System.DBNull"); } }        

        public int Serial
        {
            get { return (Convert.ToInt32(clDataRow["Serial"])); }
            set { clDataRow["Serial"] = value; }
        }

        public int ItemID
        {
            get { return (Convert.ToInt32(clDataRow["ItemID"])); }
            set { clDataRow["ItemID"] = value; }
        }

        public String ItemStatus
        {
            get { return (Convert.ToString(clDataRow["ItemStatus"])); }
            set { clDataRow["ItemStatus"] = value; }
        }

        public String ItemCode
        {
            get { return (Convert.ToString(clDataRow["ItemCode"])); }
            set { clDataRow["ItemCode"] = value; }
        }

        public String ItemName
        {
            get { return (Convert.ToString(clDataRow["ItemName"])); }
            set { clDataRow["ItemName"] = value; }
        } 

        public String Description
        {
            get { return (Convert.ToString(clDataRow["Description"])); }
            set { clDataRow["Description"] = value; }
        }

        public String UnitMode
        {
            get { return (Convert.ToString(clDataRow["UnitMode"])); }
            set { clDataRow["UnitMode"] = value; }
        }

        public String UnitName
        {
            get { return (Convert.ToString(clDataRow["UnitName"])); }
            set { clDataRow["UnitName"] = value; }
        }
        
        public Decimal UnitRelationship
        {
            get { return (Convert.ToDecimal(clDataRow["UnitRelationship"])); }
            set { clDataRow["UnitRelationship"] = value; }
        }

        public Decimal Quantity
        {
            get { return (Convert.ToDecimal(clDataRow["Quantity"])); }
            set { clDataRow["Quantity"] = value; }
        }

        public Decimal UnitPrice
        {
            get { return (Convert.ToDecimal(clDataRow["UnitPrice"])); }
            set { clDataRow["UnitPrice"] = value; }
        }

        public Decimal Discount
        {
            get { return (Convert.ToDecimal(clDataRow["Discount"])); }
            set { clDataRow["Discount"] = value; }
        }

        public String Remark
        {
            get { return (Convert.ToString(clDataRow["Remark"])); }
            set { clDataRow["Remark"] = value; }
        }     
    }

    public class POSReceiptListRow
    {
        DataRow clDataRow;

        public POSReceiptListRow(DataRow paDataRow)
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
        public bool IsReceiptIDNull         { get { return (clDataRow["ReceiptID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsReceiptNoNull         { get { return (clDataRow["ReceiptNo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsReceiptTypeNull       { get { return (clDataRow["ReceiptType"].GetType().ToString() == "System.DBNull"); } }
        public bool IsReceiptDateNull       { get { return (clDataRow["ReceiptDate"].GetType().ToString() == "System.DBNull"); } }
        public bool IsCodeNoNull            { get { return (clDataRow["CodeNo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsNameNull              { get { return (clDataRow["Name"].GetType().ToString() == "System.DBNull"); } }
        public bool IsReceiptAmountNull     { get { return (clDataRow["ReceiptAmount"].GetType().ToString() == "System.DBNull"); } }        

        public int ReceiptID
        {
            get { return (Convert.ToInt32(clDataRow["ReceiptID"])); }
            set { clDataRow["ReceiptID"] = value; }
        }

        public int ReceiptNo
        {
            get { return (Convert.ToInt32(clDataRow["ReceiptNo"])); }
            set { clDataRow["ReceiptNo"] = value; }
        }

        public String ReceiptType
        {
            get { return (Convert.ToString(clDataRow["ReceiptType"])); }
            set { clDataRow["ReceiptType"] = value; }
        }

        public DateTime ReceiptDate
        {
            get { return (Convert.ToDateTime(clDataRow["ReceiptDate"])); }
            set { clDataRow["ReceiptDate"] = value; }
        }

        public String CodeNo
        {
            get { return (Convert.ToString(clDataRow["CodeNo"])); }
            set { clDataRow["CodeNo"] = value; }
        }

        public String Name
        {
            get { return (Convert.ToString(clDataRow["Name"])); }
            set { clDataRow["Name"] = value; }
        }
     
        public Decimal ReceiptAmount
        {
            get { return (Convert.ToDecimal(clDataRow["ReceiptAmount"])); }
            set { clDataRow["ReceiptAmount"] = value; }
        }
    }

    public class POSReceiptDetailRow
    {
        DataRow clDataRow;

        public POSReceiptDetailRow(DataRow paDataRow)
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
        public bool IsReceiptIDNull         { get { return (clDataRow["ReceiptID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsReceiptNoNull         { get { return (clDataRow["ReceiptNo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsReceiptTypeNull       { get { return (clDataRow["ReceiptType"].GetType().ToString() == "System.DBNull"); } }
        public bool IsReceiptDateNull       { get { return (clDataRow["ReceiptDate"].GetType().ToString() == "System.DBNull"); } }
        public bool IsCodeNoNull            { get { return (clDataRow["CodeNo"].GetType().ToString() == "System.DBNull"); } }
        public bool IsNameNull              { get { return (clDataRow["Name"].GetType().ToString() == "System.DBNull"); } }
        public bool IsSerialNull            { get { return (clDataRow["Serial"].GetType().ToString() == "System.DBNull"); } }
        public bool IsItemIDNull            { get { return (clDataRow["ItemID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsEntryAttributeNull    { get { return (clDataRow["EntryAttribute"].GetType().ToString() == "System.DBNull"); } }
        public bool IsQuantityNull          { get { return (clDataRow["Quantity"].GetType().ToString() == "System.DBNull"); } }
        public bool IsUnitModeNull          { get { return (clDataRow["UnitMode"].GetType().ToString() == "System.DBNull"); } }
        public bool IsUnitPriceNull         { get { return (clDataRow["UnitPrice"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDiscountNull          { get { return (clDataRow["Discount"].GetType().ToString() == "System.DBNull"); } }
        public bool IsTaxAmountNull         { get { return (clDataRow["TaxAmount"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDisplayQuantityNull   { get { return (clDataRow["DisplayQuantity"].GetType().ToString() == "System.DBNull"); } }
        public bool IsTotalAmountNull       { get { return (clDataRow["TotalTaxAmount"].GetType().ToString() == "System.DBNull"); } }
        public bool IsUnitCostNull          { get { return (clDataRow["UnitCost"].GetType().ToString() == "System.DBNull"); } }
        public bool IsTotalCostNull         { get { return (clDataRow["TotalCost"].GetType().ToString() == "System.DBNull"); } }
        public bool IsItemNameNull          { get { return (clDataRow["ItemName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsUnitNameNull          { get { return (clDataRow["UnitName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsDescriptionNull       { get { return (clDataRow["Description"].GetType().ToString() == "System.DBNull"); } }
        public bool IsRelativeBalanceNull   { get { return (clDataRow["RelativeBalance"].GetType().ToString() == "System.DBNull"); } }
        
        public int ReceiptID
        {
            get { return (Convert.ToInt32(clDataRow["ReceiptID"])); }
            set { clDataRow["ReceiptID"] = value; }
        }

        public int ReceiptNo
        {
            get { return (Convert.ToInt32(clDataRow["ReceiptNo"])); }
            set { clDataRow["ReceiptNo"] = value; }
        }

        public String ReceiptType
        {
            get { return (Convert.ToString(clDataRow["ReceiptType"])); }
            set { clDataRow["ReceiptType"] = value; }
        }

        public DateTime ReceiptDate
        {
            get { return (Convert.ToDateTime(clDataRow["ReceiptDate"])); }
            set { clDataRow["ReceiptDate"] = value; }
        }

        public String CodeNo
        {
            get { return (Convert.ToString(clDataRow["CodeNo"])); }
            set { clDataRow["CodeNo"] = value; }
        }

        public String Name
        {
            get { return (Convert.ToString(clDataRow["Name"])); }
            set { clDataRow["Name"] = value; }
        }
     
        public int Serial
        {
            get { return (Convert.ToInt32(clDataRow["Serial"])); }
            set { clDataRow["Serial"] = value; }
        }

        public int ItemID
        {
            get { return (Convert.ToInt32(clDataRow["ItemID"])); }
            set { clDataRow["ItemID"] = value; }
        }

        public String EntryAttribute
        {
            get { return (Convert.ToString(clDataRow["EntryAttribute"])); }
            set { clDataRow["EntryAttribute"] = value; }
        }

        public String UnitMode
        {
            get { return (Convert.ToString(clDataRow["UnitMode"])); }
            set { clDataRow["UnitMode"] = value; }
        }

        public int UnitRelationship
        {
            get { return (Convert.ToInt32(clDataRow["UnitRelationship"])); }
            set { clDataRow["UnitRelationship"] = value; }
        }

        public Decimal UnitPrice
        {
            get { return (Convert.ToDecimal(clDataRow["UnitPrice"])); }
            set { clDataRow["UnitPrice"] = value; }
        }

        public Decimal Discount
        {
            get { return (Convert.ToDecimal(clDataRow["Discount"])); }
            set { clDataRow["Discount"] = value; }
        }

        public Decimal TaxAmount
        {
            get { return (Convert.ToDecimal(clDataRow["TaxAmount"])); }
            set { clDataRow["TaxAmount"] = value; }
        }

        public Decimal DisplayQuantity
        {
            get { return (Convert.ToDecimal(clDataRow["DisplayQuantity"])); }
            set { clDataRow["DisplayQuantity"] = value; }
        }

        public Decimal TotalAmount
        {
            get { return (Convert.ToDecimal(clDataRow["TotalAmount"])); }
            set { clDataRow["TotalAmount"] = value; }
        }

        public Decimal UnitCost
        {
            get { return (Convert.ToDecimal(clDataRow["UnitCost"])); }
            set { clDataRow["UnitCost"] = value; }
        }

        public Decimal TotalCost
        {
            get { return (Convert.ToDecimal(clDataRow["TotalCost"])); }
            set { clDataRow["TotalCost"] = value; }
        }

        public String ItemName
        {
            get { return (Convert.ToString(clDataRow["ItemName"])); }
            set { clDataRow["ItemName"] = value; }
        }

        public String UnitName
        {
            get { return (Convert.ToString(clDataRow["UnitName"])); }
            set { clDataRow["UnitName"] = value; }
        }

        public String Description
        {
            get { return (Convert.ToString(clDataRow["Description"])); }
            set { clDataRow["Description"] = value; }
        }

        public Decimal RelativeBlance
        {
            get { return (Convert.ToDecimal(clDataRow["RelativeBalance"])); }
            set { clDataRow["RelativeBalance"] = value; }
        }
    }

    public class POSStockBalanceRow
    {
        DataRow clDataRow;

        public POSStockBalanceRow(DataRow paDataRow)
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
        public bool IsItemIDNull { get { return (clDataRow["ItemID"].GetType().ToString() == "System.DBNull"); } }
        public bool IsEntryTypeNull { get { return (clDataRow["EntryType"].GetType().ToString() == "System.DBNull"); } }
        public bool IsCategoryNull { get { return (clDataRow["Category"].GetType().ToString() == "System.DBNull"); } }
        public bool IsSortOrderNull { get { return (clDataRow["SortOrder"].GetType().ToString() == "System.DBNull"); } }
        public bool IsUnitRelationshipNull { get { return (clDataRow["UnitRelationship"].GetType().ToString() == "System.DBNull"); } }
        public bool IsItemNameNull { get { return (clDataRow["ItemName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsMajorUnitNameNull { get { return (clDataRow["MajorUnitName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsMinorUnitNameNull { get { return (clDataRow["MinorUnitName"].GetType().ToString() == "System.DBNull"); } }
        public bool IsQuantityNull { get { return (clDataRow["Quantity"].GetType().ToString() == "System.DBNull"); } }
        public bool IsUnitCostNull { get { return (clDataRow["UnitMode"].GetType().ToString() == "System.DBNull"); } }
        public bool IsTotalCostNull { get { return (clDataRow["TotalCost"].GetType().ToString() == "System.DBNull"); } }
        public bool IsQuantityTextNull { get { return (clDataRow["QuantityText"].GetType().ToString() == "System.DBNull"); } }

        public int ItemID
        {
            get { return (Convert.ToInt32(clDataRow["ItemID"])); }
            set { clDataRow["ItemID"] = value; }
        }

        public String EntryType
        {
            get { return (Convert.ToString(clDataRow["EntryType"])); }
            set { clDataRow["EntryType"] = value; }
        }

        public int Category
        {
            get { return (Convert.ToInt32(clDataRow["Category"])); }
            set { clDataRow["Category"] = value; }
        }

        public int SortOrder
        {
            get { return (Convert.ToInt32(clDataRow["SortOrder"])); }
            set { clDataRow["SortOrder"] = value; }
        }

        public int UnitRelationship
        {
            get { return (Convert.ToInt32(clDataRow["UnitRelationship"])); }
            set { clDataRow["UnitRelationship"] = value; }
        }

        public String ItemName
        {
            get { return (Convert.ToString(clDataRow["ItemName"])); }
            set { clDataRow["ItemName"] = value; }
        }

        public String MajorUnitName
        {
            get { return (Convert.ToString(clDataRow["MajorUnitName"])); }
            set { clDataRow["MajorUnitName"] = value; }
        }

        public String MinorUnitName
        {
            get { return (Convert.ToString(clDataRow["MinorUnitName"])); }
            set { clDataRow["MinorUnitName"] = value; }
        }

        public Decimal Quantity
        {
            get { return (Convert.ToDecimal(clDataRow["Quantity"])); }
            set { clDataRow["Quantity"] = value; }
        }

        public Decimal UnitCost
        {
            get { return (Convert.ToDecimal(clDataRow["UnitCost"])); }
            set { clDataRow["UnitCost"] = value; }
        }

        public Decimal TotalCost
        {
            get { return (Convert.ToDecimal(clDataRow["TotalCost"])); }
            set { clDataRow["TotalCost"] = value; }
        }

        public String QuantityText
        {
            get { return (Convert.ToString(clDataRow["QuantityText"])); }
            set { clDataRow["QuantityText"] = value; }
        }
    }
}
