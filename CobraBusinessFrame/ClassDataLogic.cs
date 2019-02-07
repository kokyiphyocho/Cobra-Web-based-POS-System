using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobraFoundation;
using CobraFrame;

namespace CobraBusinessFrame
{
   public class EServiceQueryClass
   {
       const String ctGetPOSReceiptRow    = "Select " +
                                                "* " +
                                            "From " +
                                                "EData_Receipt " +
                                            "Where " +
                                                "SubscriptionID = $SUBSCRIPTIONID And " +
                                                "ReceiptID = $RECEIPTID; ";

       const String ctGetPOSLastReceiptNo   = "Select " +
                                                 "Isnull(Max(ReceiptNo),0)  As NewReceiptNo " +
                                              "From " +
                                                 "EData_Receipt " +
                                              "Where " +
                                                 "SubscriptionID = $SUBSCRIPTIONID And " +
                                                 "ReceiptType = $RECEIPTTYPE;";

       const String ctGetPOSStakeHolderRowByStakeHolderID   = "Select " +
                                                                    "* " +
                                                                "From " +
                                                                    "EData_StakeHolder " +
                                                                "Where " +
                                                                    "SubscriptionID = $SUBSCRIPTIONID And " +
                                                                    "StakeHolderID = $STAKEHOLDERID; ";

       const String ctGetPOSStakeHolderRowByCodeNo    = "Select " +
                                                                    "Top(1) * " +
                                                                "From " +
                                                                    "EData_StakeHolder " +
                                                                "Where " +
                                                                    "SubscriptionID = $SUBSCRIPTIONID And " +
                                                                    "CodeNo = $CODENO " +
                                                                "Order By " +
                                                                    "StakeHolderID Desc; ";

      // const String ctDeclareReturnVal = "Declare @ReturnValue int;";
       const String ctPOSUpdateReceipt = "Exec ES_POSUpdateReceiptRecord " +
                                          "$SUBSCRIPTIONID, $LOGINID, $ACCESSINFO, " +
                                          "$RECEIPTID, $RECEIPTTYPE, $RECEIPTDATE, $CODENO, $NAME, $REFERENCE, " +
                                          "$PAYMENTCASH, $PAYMENTBANK, $PAYMENTCREDIT, $PAYMENTCONTRA, $TRANSACTIONLIST;";    
                                                
       
       public enum QueryType : int
       {
           GetPOSReceiptRow,
           GetPOSLastReceiptNo,

           GetPOSStakeHolderRowByStakeHolderID,
           GetPOSStakeHolderRowByCodeNo,

           POSUpdateReceipt,
       }

       QueryClass.ConnectionMode       clConnectionMode;
       QueryClass                      clQueryClass;

       public QueryClass Query         { get { return (clQueryClass); } }

       public EServiceQueryClass(QueryType paQueryType)
       {
           clConnectionMode = QueryClass.ConnectionMode.EService;
           ChooseQuery(paQueryType);
       }

       private void ChooseQuery(QueryType paQueryType)
       {
           switch (paQueryType)
           {
               case QueryType.GetPOSReceiptRow                      : clQueryClass = new QueryClass(ctGetPOSReceiptRow, clConnectionMode); break;
               case QueryType.GetPOSLastReceiptNo                   : clQueryClass = new QueryClass(ctGetPOSLastReceiptNo, clConnectionMode); break;

               case QueryType.GetPOSStakeHolderRowByStakeHolderID   : clQueryClass = new QueryClass(ctGetPOSStakeHolderRowByStakeHolderID, clConnectionMode); break;
               case QueryType.GetPOSStakeHolderRowByCodeNo          : clQueryClass = new QueryClass(ctGetPOSStakeHolderRowByCodeNo, clConnectionMode); break;

               case QueryType.POSUpdateReceipt                      : clQueryClass = new QueryClass(ctPOSUpdateReceipt, clConnectionMode); break;
               
           }

           clQueryClass.ReplacePlaceHolder("$SUBSCRIPTIONID", ApplicationFrame.GetInstance().ActiveSubscription.ActiveRow.SubscriptionID, true);         
       }
   }
}
