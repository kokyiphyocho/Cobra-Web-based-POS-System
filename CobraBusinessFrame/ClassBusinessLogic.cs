using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using CobraFoundation;
using CobraFrame;

namespace CobraBusinessFrame
{
    public class OrderInfoManager
    {
        public struct Struct_OrderDetail
        {
            public int ItemID { get; set; }
            public String Action { get; set; }
            public String ItemName { get; set; }
            public int Quantity { get; set; }
            public Decimal UnitPrice { get; set; }
            public String Remark { get; set; }
        }
        const int ctSTASubmitted = 0;

        int clOrderNo;
        String clSubscriptionID;
        String clLogInID;

        Dictionary<String, String> clDeliveryInfo;
        List<Struct_OrderDetail> clOrderDetail;

        public enum OrderUniversalStatus { Reject = -2, Cancel = -1, Submitted = 0, Requested = 1, Processing = 2, Shipping = 3, Delivered = 4, Confirmed = 9 }

        public static OrderInfoManager CreateInstance(int paOrderNo, String paSubScriptionID, String paLogInID)
        {
            if ((paOrderNo > 0) && (!String.IsNullOrEmpty(paSubScriptionID)) && (!String.IsNullOrEmpty(paLogInID)))
                return (new OrderInfoManager(paOrderNo, paSubScriptionID, paLogInID));
            else return (null);
        }

        public static OrderInfoManager CreateInstance(String paJSONAddressInfo, String paJSONOrderDetailList)
        {
            // JavaScriptSerializer lcJavaScriptSerializer;
            Dictionary<String, String> lcAddressInfo;
            List<Struct_OrderDetail> lcOrderDetail;

            if ((!String.IsNullOrEmpty(paJSONAddressInfo)) && (!String.IsNullOrEmpty(paJSONOrderDetailList)))
            {
               //  lcJavaScriptSerializer = new JavaScriptSerializer();

                lcAddressInfo = General.JSONDeserialize<Dictionary<String, String>>(paJSONAddressInfo);
                lcOrderDetail = General.JSONDeserialize<List<Struct_OrderDetail>>(paJSONOrderDetailList);

                return (new OrderInfoManager(lcAddressInfo, lcOrderDetail));
            }
            else return (null);
        }

        public static OrderInfoManager CreateInstance(int paOrderNo, String paJSONOrderDetailList)
        {
            // JavaScriptSerializer lcJavaScriptSerializer;
            List<Struct_OrderDetail> lcOrderDetail;

            if ((!String.IsNullOrEmpty(paJSONOrderDetailList)) && (paOrderNo > 0))
            {
               // lcJavaScriptSerializer = new JavaScriptSerializer();

                lcOrderDetail = General.JSONDeserialize<List<Struct_OrderDetail>>(paJSONOrderDetailList);

                return (new OrderInfoManager(paOrderNo, lcOrderDetail));
            }
            else return (null);
        }

        private OrderInfoManager(int paOrderNo, String paSubScriptionID, String paLogInID)
        {
            clOrderNo = paOrderNo;
            clSubscriptionID = paSubScriptionID;
            clLogInID = paLogInID;
        }

        private OrderInfoManager(Dictionary<String, String> paDeliveryInfo, List<Struct_OrderDetail> paOrderDetail)
        {
            clDeliveryInfo = paDeliveryInfo;
            clOrderDetail = paOrderDetail;
        }

        private OrderInfoManager(int paOrderNo, List<Struct_OrderDetail> paOrderDetail)
        {
            clOrderNo = paOrderNo;
            clOrderDetail = paOrderDetail;
        }

        public int SubmitNewOrder()
        {
            OrderInfoRow lcOrderInfoRow;
            OrderDetailRow lcOrderDetailRow;
            int lcOrderNo;

            if (clOrderDetail != null)
            {
                lcOrderInfoRow = CreateNewOrderInfoRow();
                if ((lcOrderNo = InsertOrderInfoRecord(lcOrderInfoRow)) > 0)
                {
                    for (int lcCount = 0; lcCount < clOrderDetail.Count; lcCount++)
                    {
                        lcOrderDetailRow = CreateNewOrderDetailRow(lcOrderNo, clOrderDetail[lcCount]);
                        InsertOrderDetailRecord(lcOrderDetailRow);
                    }

                    return (lcOrderNo);
                }
            }

            return (-1);
        }

        public bool UpdateOrderInfoRemark(String paOrderRemark)
        {
            QueryClass lcQueryClass;

            lcQueryClass = new QueryClass(QueryClass.QueryType.UpdateOrderInfoRemark);
            lcQueryClass.ReplacePlaceHolder("$ORDERNO", clOrderNo.ToString(), false);
            lcQueryClass.ReplacePlaceHolder("$ORDERREMARK", paOrderRemark, true);
            lcQueryClass.ReplacePlaceHolder("$SUBSCRIPTIONID", ApplicationFrame.GetInstance().ActiveSubscription.ActiveRow.SubscriptionID, true);

            lcQueryClass.ExecuteNonQuery();

            return (true);
        }

        public bool UpdateOrderDetail()
        {
            OrderDetailRow lcOrderDetailRow;

            if (clOrderDetail != null)
            {
                for (int lcCount = 0; lcCount < clOrderDetail.Count; lcCount++)
                {
                    lcOrderDetailRow = CreateNewOrderDetailRow(clOrderNo, clOrderDetail[lcCount]);

                    ApplyAction(lcOrderDetailRow, clOrderDetail[lcCount].Action);
                    UpdateOrderDetailRecord(lcOrderDetailRow);
                }
            }

            return (true);
        }

        private void ApplyAction(OrderDetailRow paOrdetailRow, String paAction)
        {
            switch (paAction)
            {
                case "CANCEL": paOrdetailRow.Status = -1; break;
                case "REJECT": paOrdetailRow.Status = -2; break;
                case "REQUESTED": paOrdetailRow.Status = 1; break;
                case "PROCESSING": paOrdetailRow.Status = 2; break;
                case "SHIPPING": paOrdetailRow.Status = 3; break;
                case "DELIVERED": paOrdetailRow.Status = 4; break;
            }
        }

        private OrderInfoRow CreateNewOrderInfoRow()
        {
            OrderInfoRow lcOrderInfoRow;

            lcOrderInfoRow = new OrderInfoRow(EServiceTableManager.GetInstance().GetNewRow(EServiceTableManager.TableType.OrderInfo, true));

            lcOrderInfoRow.OrderDate = General.GetCurrentSystemLocalTime();
            lcOrderInfoRow.OrderStatus = ctSTASubmitted;
            lcOrderInfoRow.SubscriptionID = ApplicationFrame.GetInstance().ActiveSubscription.ActiveRow.SubscriptionID;
            lcOrderInfoRow.LoginID = ApplicationFrame.GetInstance().ActiveSessionController.User.ActiveRow.LoginID;
            lcOrderInfoRow.Name = clDeliveryInfo["Name"];
            lcOrderInfoRow.ContactNo = clDeliveryInfo["ContactNo"];
            lcOrderInfoRow.BuildingNo = clDeliveryInfo["BuildingNo"];
            lcOrderInfoRow.Floor = clDeliveryInfo["Floor"];
            lcOrderInfoRow.RoomNo = clDeliveryInfo["RoomNo"];
            lcOrderInfoRow.Street = clDeliveryInfo["Street"];
            lcOrderInfoRow.Quarter = clDeliveryInfo["Quarter"];
            lcOrderInfoRow.AddressInfo = clDeliveryInfo["AddressInfo"];
            lcOrderInfoRow.Township = clDeliveryInfo["township"];
            lcOrderInfoRow.City = clDeliveryInfo["city"];
            lcOrderInfoRow.OrderRemark = clDeliveryInfo["OrderRemark"];

            return (lcOrderInfoRow);
        }

        private OrderDetailRow CreateNewOrderDetailRow(int paOrderNo, Struct_OrderDetail paOrderDetail)
        {
            OrderDetailRow lcOrderDetailRow;

            lcOrderDetailRow = new OrderDetailRow(EServiceTableManager.GetInstance().GetNewRow(EServiceTableManager.TableType.OrderDetail, true));

            lcOrderDetailRow.OrderNo = paOrderNo;
            lcOrderDetailRow.ItemID = paOrderDetail.ItemID;
            lcOrderDetailRow.ItemName = paOrderDetail.ItemName;
            lcOrderDetailRow.UnitPrice = paOrderDetail.UnitPrice;
            lcOrderDetailRow.Quantity = paOrderDetail.Quantity;
            lcOrderDetailRow.Remark = paOrderDetail.Remark;

            return (lcOrderDetailRow);
        }

        private int InsertOrderInfoRecord(OrderInfoRow paOrderInfoRow)
        {
            object lcResult;
            int lcOrderNo;
            QueryClass lcQuery;

            lcQuery = new QueryClass(QueryClass.QueryType.InsertOrderInfoRecord);
            lcQuery.ReplaceRowPlaceHolder(paOrderInfoRow.Row);

            if (((lcResult = lcQuery.GetResult()) != null) && ((lcOrderNo = Convert.ToInt32(lcResult)) > 0)) return (lcOrderNo);
            else return (-1);
        }

        private void InsertOrderDetailRecord(OrderDetailRow paOrderDetailRow)
        {
            QueryClass lcQuery;

            lcQuery = new QueryClass(QueryClass.QueryType.InsertOrderDetailRecord);
            lcQuery.ReplaceRowPlaceHolder(paOrderDetailRow.Row);

            lcQuery.ExecuteNonQuery();
        }

        private void UpdateOrderDetailRecord(OrderDetailRow paOrderDetailRow)
        {
            QueryClass lcQuery;

            lcQuery = new QueryClass(QueryClass.QueryType.UpdateOrderDetailRecord);
            lcQuery.ReplaceRowPlaceHolder(paOrderDetailRow.Row);
            ApplicationFrame.GetInstance().ActiveFormInfoManager.ReplaceQueryPlaceHolder(lcQuery);

            lcQuery.ExecuteNonQuery();
        }
    }

    public class POSReceiptManager
    {
        public struct Struct_StockIO
        {            
            public String   AccessInfo      { get; set; }
            public String   ReceiptID       { get; set; }            
            public int      Serial          { get; set; }
            public int      ItemID          { get; set; }
            public decimal  Quantity        { get; set; }            
            public String   UnitMode        { get; set; }
            public Decimal  UnitPrice       { get; set; }
            public Decimal  Discount        { get; set; }            
        }

        public enum ReceiptType { Sale, Purchase, StockIn, StockOut, Invalid }
        public enum Status  { Active, Cancel };

        const String    ctQUYRetrieveOutgoingDetail =  "epos.retrieveoutgoingreceiptdetail";
        const String    ctQUYRetrieveIncomingDetail =  "epos.retrieveincomingreceiptdetail";

        const String    ctSETTaxInclusive           = "POS.TaxInclusive";
        const String    ctSETTaxPercent             = "POS.TaxPercent";
        const String    ctSETTaxApplicable          = "POS.TaxApplicable";

        POSReceiptRow           clPOSReceiptRow;
        POSStakeHolderManager   clPOSStakeHolderManager;
        DataTable               clReceiptDetailList;
        
        public  POSReceiptRow           ActiveRow           { get { return(clPOSReceiptRow); } }
        public  POSStakeHolderManager   ActiveStakeHolder   { get { return(GetStakeHolder()); } }
        public  DataTable               ReceiptDetailList   { get { return (GetReceiptDetailList()); } }
        
        public static POSReceiptManager CreateInstance(int paReceiptID)
        {   
            POSReceiptRow lcPOSReceiptRow;

            if (paReceiptID != -1)
            {
                if ((lcPOSReceiptRow = GetPOSReceiptRow(paReceiptID)) != null)
                    return(new POSReceiptManager(lcPOSReceiptRow));                
            }
            return (null);            
        }

        public static POSReceiptManager CreateInstance(ReceiptType paReceiptType)
        {
            return (new POSReceiptManager(CreateNewPOSReceiptRow(paReceiptType)));            
        }

        public static POSReceiptManager CreateInstance(POSReceiptRow paPOSReceiptRow)
        {
            if (paPOSReceiptRow != null) return (new POSReceiptManager(paPOSReceiptRow));            
            else return (null);
        }

        private static POSReceiptRow GetPOSReceiptRow(int paReceiptID)
        {
            DataTable lcDataTable;
            EServiceQueryClass  lcQuery;

            lcQuery = new EServiceQueryClass(EServiceQueryClass.QueryType.GetPOSReceiptRow);
            lcQuery.Query.ReplacePlaceHolder("$RECEIPTID", paReceiptID.ToString(),false);
            
            if (((lcDataTable = lcQuery.Query.RunQuery()) != null) && (lcDataTable.Rows.Count > 0))
                return(new POSReceiptRow(lcDataTable.Rows[0]));
            else return(null);
        }

        private static int GetPOSLastReceiptNo(ReceiptType paReceiptType)
        {
            object              lcResult;
            EServiceQueryClass  lcQuery;

            lcQuery = new EServiceQueryClass(EServiceQueryClass.QueryType.GetPOSLastReceiptNo);
            lcQuery.Query.ReplacePlaceHolder("$RECEIPTTYPE", paReceiptType.ToString(), true);

            if ((lcResult = lcQuery.Query.GetResult()) != null) return (Convert.ToInt32(lcResult));
            else return (0);
        }

        private static POSReceiptRow CreateNewPOSReceiptRow(ReceiptType paReceiptType)
        {
            bool          lcTaxApplicable;
            POSReceiptRow lcPOSReceiptRow;

            lcPOSReceiptRow = new POSReceiptRow(EServiceTableManager.GetInstance().GetNewRow(EServiceTableManager.TableType.POSReceipt, true));
            lcTaxApplicable = General.ParseBoolean(ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting.GetSettingValue(ctSETTaxApplicable), false);

            lcPOSReceiptRow.ReceiptID       = -1;
            lcPOSReceiptRow.ReceiptNo       = GetPOSLastReceiptNo(paReceiptType) + 1;
            lcPOSReceiptRow.ReceiptDate     = General.GetCurrentSystemLocalTime();
            
            if ((lcTaxApplicable) && (paReceiptType == ReceiptType.Sale))
                lcPOSReceiptRow.TaxPercent  = General.ParseDecimal(ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting.GetSettingValue(ctSETTaxPercent), 0);
            else lcPOSReceiptRow.TaxPercent = 0;

            lcPOSReceiptRow.TaxInclusive    = General.ParseBoolean(ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting.GetSettingValue(ctSETTaxInclusive), false);
            lcPOSReceiptRow.ReceiptType     = paReceiptType.ToString().ToUpper();
            lcPOSReceiptRow.Status          = Status.Active.ToString().ToUpper();

            return (lcPOSReceiptRow);
        }

        private POSReceiptManager(POSReceiptRow paPOSReceiptRow)
        {
            clPOSReceiptRow = paPOSReceiptRow;            
        }

        public ReceiptType GetReceiptType()
        {
            return(General.ParseEnum<ReceiptType>(ActiveRow.ReceiptType,ReceiptType.Invalid));
        }

        private DataTable RetrieveReceiptDetailList()
        {
            String lcQueryName;
            ReceiptType lcReceiptType;

            lcReceiptType = GetReceiptType();

            lcQueryName = ((lcReceiptType == ReceiptType.Purchase) || (lcReceiptType == ReceiptType.StockIn)) ? ctQUYRetrieveIncomingDetail : ctQUYRetrieveOutgoingDetail;

            return(DynamicQueryManager.GetInstance().GetDataTableResult(lcQueryName));
        }

        private POSStakeHolderManager GetStakeHolder()
        {
            if (clPOSStakeHolderManager == null)
            {
                if ((clPOSReceiptRow != null) && (clPOSReceiptRow.StakeHolderID > 0))
                    clPOSStakeHolderManager = POSStakeHolderManager.CreateInstance(clPOSReceiptRow.StakeHolderID);
                else clPOSStakeHolderManager = POSStakeHolderManager.CreateInstance();
            }

            return (clPOSStakeHolderManager);
        }

        private DataTable GetReceiptDetailList()
        {
            if (ActiveRow.ReceiptID != -1)
            {
                if (clReceiptDetailList == null) clReceiptDetailList = RetrieveReceiptDetailList();
                return(clReceiptDetailList);
            }
            else return(null);
        }
    }

    public class POSStakeHolderManager
    {
        public enum Status { Active, Cancel };

        POSStakeHolderRow clPOSStakeHolderRow;

        public POSStakeHolderRow ActiveRow { get { return (clPOSStakeHolderRow); } }

        public static POSStakeHolderManager CreateInstance(int paStakeHolderID = -1)
        {
            POSStakeHolderRow lcPOSStakeHolderRow;

            if (paStakeHolderID != -1)
            {
                if ((lcPOSStakeHolderRow = GetPOSStakeHolderRow(paStakeHolderID)) != null)
                    return (new POSStakeHolderManager(lcPOSStakeHolderRow));
                else return (new POSStakeHolderManager(CreateNewPOSStakeHolderRow()));
            }
            else return (new POSStakeHolderManager(CreateNewPOSStakeHolderRow()));
        }

        public static POSStakeHolderManager CreateInstance(DataRow paDataRow)
        {
            if (paDataRow != null) return (new POSStakeHolderManager(new POSStakeHolderRow(paDataRow)));
            else return (new POSStakeHolderManager(CreateNewPOSStakeHolderRow()));
        }

        private static POSStakeHolderRow GetPOSStakeHolderRow(int paStakeHolderID)
        {
            DataTable lcDataTable;
            EServiceQueryClass lcQuery;

            lcQuery = new EServiceQueryClass(EServiceQueryClass.QueryType.GetPOSStakeHolderRowByStakeHolderID);
            lcQuery.Query.ReplacePlaceHolder("$STAKEHOLDERID", paStakeHolderID.ToString(), false);

            if (((lcDataTable = lcQuery.Query.RunQuery()) != null) && (lcDataTable.Rows.Count > 0))
                return (new POSStakeHolderRow(lcDataTable.Rows[0]));
            else return (null);
        }

        private static POSStakeHolderRow GetPOSStakeHolderRow(String paCodeNo)
        {
            DataTable lcDataTable;
            EServiceQueryClass lcQuery;

            lcQuery = new EServiceQueryClass(EServiceQueryClass.QueryType.GetPOSStakeHolderRowByCodeNo);
            lcQuery.Query.ReplacePlaceHolder("$CODENO", paCodeNo.ToString(), false);

            if (((lcDataTable = lcQuery.Query.RunQuery()) != null) && (lcDataTable.Rows.Count > 0))
                return (new POSStakeHolderRow(lcDataTable.Rows[0]));
            else return (null);
        }

        private static int GetPOSLastReceiptNo()
        {
            object lcResult;
            EServiceQueryClass lcQuery;

            lcQuery = new EServiceQueryClass(EServiceQueryClass.QueryType.GetPOSLastReceiptNo);

            if ((lcResult = lcQuery.Query.GetResult()) != null) return (Convert.ToInt32(lcResult));
            else return (0);
        }

        private static POSStakeHolderRow CreateNewPOSStakeHolderRow()
        {
            POSStakeHolderRow   lcPOSStakeHolderRow;

            lcPOSStakeHolderRow = new POSStakeHolderRow(EServiceTableManager.GetInstance().GetNewRow(EServiceTableManager.TableType.POSStakeHolder, true));

            lcPOSStakeHolderRow.StakeHolderID = -1;            

            return (lcPOSStakeHolderRow);
        }

        private POSStakeHolderManager(POSStakeHolderRow paPOSStakeHolderRow)
        {
            clPOSStakeHolderRow = paPOSStakeHolderRow;
        }
    }
}
