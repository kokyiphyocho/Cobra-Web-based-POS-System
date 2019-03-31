using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using CobraWebFrame;
using System.Web.Script.Serialization;
using CobraFoundation;
using CobraFrame;
using CobraWebControls;
using CobraStandardControls;
using System.Data;
using CobraBusinessFrame;

namespace CobraApplicationFrame
{
    public class ServiceResponse
    {
        private struct ResponseStruct
        {
            public bool Success;
            public Dictionary<String, object> ResponseData;
        }

        ResponseStruct clResponseStruct;

        public bool Success
        {
            get { return (clResponseStruct.Success); }
            set { clResponseStruct.Success = value; }
        }

        public ServiceResponse()
        {
            clResponseStruct.Success = false;
            clResponseStruct.ResponseData = new Dictionary<string, object>();
        }

        public bool AddResponse(String paName, object paValue)
        {
            if (!String.IsNullOrWhiteSpace(paName))
            {
                clResponseStruct.ResponseData.Add(paName, paValue);
                return (true);
            }
            else return (false);
        }

        public void AddResponse<TKey, TValue>(Dictionary<TKey, TValue> paDictionary)
        {
            foreach (var lcKey in paDictionary.Keys)
                clResponseStruct.ResponseData.Add(lcKey.ToString(), paDictionary[lcKey]);
        }

        public String GetSerializedResponse()
        {
          //  JavaScriptSerializer lcJavaScriptSerializer;

            // lcJavaScriptSerializer = new JavaScriptSerializer();
            return (General.JSONSerialize(clResponseStruct));
        }
    }

    public class CobraAjaxServiceController
    {
        const String ctARGCobraAjaxRequest = "CobraAjaxRequest";
                
        const String ctARGLogInID           = "LogInID";
        const String ctARGPassword          = "Password";
        const String ctARGDataBlock         = "DataBlock";
        const String ctARGParamBlock        = "ParamBlock";
        const String ctARGParameter         = "Parameter";
        const String ctARGSecondParameter   = "SecondParameter";   
        const String ctARGItemList          = "ItemList";
        const String ctARGDeliveryInfo      = "DeliveryInfo";

        const String ctARGMasterBlock       = "ARG_MasterBlock";
        const String ctARGDetailListBlock   = "ARG_DetailListBlock";

        const String ctError                = "[#ERROR]";
        const char   ctErrorSplitter        = ';';

        const String ctUnexpectedError = "Unexpected error occured.";
        const String ctArgumentError = "Argument error occured.";

        public enum CobraAjaxRequest { none, LogIn, LogOutSystem, UpdateDataList, UpdateDataRecord, UploadFile, ExecuteScalarQuery, ExecuteNonQuery,GetDataRowQuery, 
                                       ResetPassword, GetUpdatedControl, VerifyPassword, LazyLoadItemList, ChangePinCode }

        public enum CobraServiceResponse { RSP_Result, RSP_Serial, RSP_ID, RSP_KeyDetect, RSP_HTML, RSP_ErrorCode, RSP_ErrorMessage, RSP_Dictionary };

        private CobraAjaxRequest      clCobraAjaxRequest;
        private ServiceResponse       clServiceResponse;

        static public CobraAjaxServiceController CreateInstance()
        {
            CobraAjaxRequest lcCobraAjaxRequest;

            if ((lcCobraAjaxRequest = GetCobraAjaxRequest()) != CobraAjaxRequest.none)
                return (new CobraAjaxServiceController(lcCobraAjaxRequest));
            else return (null);
        }

        static private CobraAjaxRequest GetCobraAjaxRequest()
        {
            String lcCobraAjaxRequestStr;
            CobraAjaxRequest lcCobraAjaxRequest;

            if ((lcCobraAjaxRequestStr = ApplicationFrame.GetParameter(ctARGCobraAjaxRequest)) != null)
            {
                if (Enum.TryParse<CobraAjaxRequest>(lcCobraAjaxRequestStr, true, out lcCobraAjaxRequest))
                    return (lcCobraAjaxRequest);
            }
            return (CobraAjaxRequest.none);
        }

        private CobraAjaxServiceController(CobraAjaxRequest paCobraAjaxRequest)
        {
            clCobraAjaxRequest = paCobraAjaxRequest;
            clServiceResponse = new ServiceResponse();
        }

        public String ExecuteService()
        {
            switch (clCobraAjaxRequest)
            {
                case CobraAjaxRequest.UpdateDataList        : UpdateDataList(); break;
                case CobraAjaxRequest.UpdateDataRecord      : UpdateDataRecord(); break;                
                case CobraAjaxRequest.LogIn                 : CreateLogInSession(); break;           
                case CobraAjaxRequest.LogOutSystem          : LogOutSystem(); break;
                case CobraAjaxRequest.ExecuteScalarQuery    : ExecuteScalarQuery(); break;
                case CobraAjaxRequest.ExecuteNonQuery       : ExecuteNonQuery(); break;
                case CobraAjaxRequest.GetDataRowQuery       : GetDataRowQuery(); break;
                case CobraAjaxRequest.ResetPassword         : ResetPassword(); break;
                case CobraAjaxRequest.GetUpdatedControl     : GetUpdatedControl(); break;
                case CobraAjaxRequest.VerifyPassword        : VerifyPassword(); break;
         //       case CobraAjaxRequest.LazyLoadItemList    : RetrieveLazyLoadItemList(); break;
            }

            return (clServiceResponse.GetSerializedResponse());
        }

        private void TranslateException(Exception paException)
        {
            String[] lcErrorMessage;

            if (paException != null)
            {
                if (paException.Message.StartsWith(ctError))
                {
                    lcErrorMessage = paException.Message.Split(ctErrorSplitter);
                    if (lcErrorMessage.Length >= 3)
                    {
                        clServiceResponse.AddResponse(CobraServiceResponse.RSP_ErrorCode.ToString(), lcErrorMessage[1]);
                        clServiceResponse.AddResponse(CobraServiceResponse.RSP_ErrorMessage.ToString(), lcErrorMessage[2]);

                        if (lcErrorMessage.Length > 3)
                            clServiceResponse.AddResponse(CobraServiceResponse.RSP_Dictionary.ToString(), lcErrorMessage[3]);
                    }
                }
            }
        }

        private void CreateLogInSession()
        {
            String lcErrorMessage;
            String lcUserName;
            String lcPassword;
            
            lcErrorMessage = String.Empty;

            if ((!String.IsNullOrWhiteSpace(lcUserName = ApplicationFrame.GetParameter(ctARGLogInID))) &&
                (!String.IsNullOrWhiteSpace(lcPassword = ApplicationFrame.GetParameter(ctARGPassword))))
            {
                if (ApplicationFrame.GetInstance().ActiveSessionController.LogIn(lcUserName, lcPassword))
                {
                    clServiceResponse.Success = true;
                }
                else lcErrorMessage = ApplicationFrame.GetInstance().ActiveSessionController.User.GetLastResultStr();
            }

            if (!clServiceResponse.Success)
                clServiceResponse.AddResponse("ErrorMessage", String.IsNullOrWhiteSpace(lcErrorMessage) ? ctUnexpectedError : lcErrorMessage);
        }

        private void LogOutSystem()
        {
            if (ApplicationFrame.GetInstance().ActiveSessionController.User.IsLoggedIn)
                ApplicationFrame.GetInstance().ActiveSessionController.LogOut();

            clServiceResponse.Success = true;
        }

        private void UpdateDataList()
        {            
            String lcDataBlock;
            DataListUpdate lcDataListUpdate;

            if (!String.IsNullOrWhiteSpace(lcDataBlock = ApplicationFrame.GetParameter(ctARGDataBlock)))                
            {
                if ((lcDataListUpdate = DataListUpdate.CreateInstance(General.Base64Decode(lcDataBlock))) != null)
                {
                    if (lcDataListUpdate.UpdateData()) clServiceResponse.Success = true;                    
                }
            }
        }

        private void UpdateDataRecord()
        {
            String              lcDataBlock;
            DataRecordUpdate    lcDataRecordUpdate;

            if (!String.IsNullOrWhiteSpace(lcDataBlock = ApplicationFrame.GetParameter(ctARGDataBlock)))
            {
                if ((lcDataRecordUpdate = DataRecordUpdate.CreateInstance(General.Base64Decode(lcDataBlock))) != null)
                {
                    if (lcDataRecordUpdate.UpdateData()) clServiceResponse.Success = true;
                }
            }
        }

        private void ExecuteScalarQuery()
        {
            String lcDataBlock;
            String lcQueryName;
            String lcResult;
            
            if ((!String.IsNullOrWhiteSpace(lcQueryName = ApplicationFrame.GetParameter(ctARGParameter))) &&     
                (!String.IsNullOrWhiteSpace(lcDataBlock = ApplicationFrame.GetParameter(ctARGDataBlock))))
            {
                try
                {
                    if ((lcResult = DynamicQueryManager.GetInstance().GetStringResult(DynamicQueryManager.ExecuteMode.Scalar, lcQueryName, General.Base64Decode(lcDataBlock))) != null)
                    {
                        if (UploadManager.GetInstance().UploadFiles())
                        {
                            clServiceResponse.AddResponse(CobraServiceResponse.RSP_Result.ToString(), lcResult);
                            clServiceResponse.Success = true;
                        }                        
                    }
                }
                catch (Exception paException)
                {
                    if (paException.InnerException != null) TranslateException(paException.InnerException);
                }
            }
        }

        private void ExecuteNonQuery()
        {
            String lcDataBlock;
            String lcQueryName;
            String lcResult;

            if ((!String.IsNullOrWhiteSpace(lcQueryName = ApplicationFrame.GetParameter(ctARGParameter))) &&
                (!String.IsNullOrWhiteSpace(lcDataBlock = ApplicationFrame.GetParameter(ctARGDataBlock))))
            {
                try
                {
                    if ((lcResult = DynamicQueryManager.GetInstance().GetStringResult(DynamicQueryManager.ExecuteMode.NonQuery, lcQueryName, General.Base64Decode(lcDataBlock))) != null)
                    {
                        clServiceResponse.AddResponse(CobraServiceResponse.RSP_Result.ToString(), lcResult);                        
                        clServiceResponse.Success = true;
                    }
                }
                catch(Exception paException)
                {
                    if (paException.InnerException != null) TranslateException(paException.InnerException);
                }
            }
        }

        private void ResetPassword()
        {
            String lcDataBlock;
            String lcQueryName;
            String lcResult;

            if ((!String.IsNullOrWhiteSpace(lcQueryName = ApplicationFrame.GetParameter(ctARGParameter))) &&
                (!String.IsNullOrWhiteSpace(lcDataBlock = ApplicationFrame.GetParameter(ctARGDataBlock))))
            {
                try
                {
                    if ((lcResult = DynamicQueryManager.GetInstance().GetStringResult(DynamicQueryManager.ExecuteMode.Scalar, lcQueryName, General.Base64Decode(lcDataBlock))) != null)
                    {
                        clServiceResponse.AddResponse(CobraServiceResponse.RSP_Result.ToString(), lcResult);
                        clServiceResponse.Success = true;
                    }
                }
                catch (Exception paException)
                {
                    if (paException.InnerException != null) TranslateException(paException.InnerException);
                }
            }
        }

        private void VerifyPassword()
        {
            String lcPassword;

            if (!String.IsNullOrWhiteSpace(lcPassword = ApplicationFrame.GetParameter(ctARGParameter)))               
            {
                if (lcPassword.ToLower() == ApplicationFrame.GetInstance().ActiveSessionController.User.ActiveRow.Password.ToLower())                
                {                    
                    clServiceResponse.Success = true;
                }
            }
        }

        private void GetDataRowQuery()
        {
            String      lcDataBlock;
            String      lcQueryName;
            Dictionary<String, String>     lcDataRowDictionary;
            
            if ((!String.IsNullOrWhiteSpace(lcQueryName = ApplicationFrame.GetParameter(ctARGParameter))) &&
                (!String.IsNullOrWhiteSpace(lcDataBlock = ApplicationFrame.GetParameter(ctARGDataBlock))))
            {
                if ((lcDataRowDictionary = DynamicQueryManager.GetInstance().GetDataRowDictionary(lcQueryName, General.Base64Decode(lcDataBlock))) != null)
                {
                    clServiceResponse.AddResponse<String, String>(lcDataRowDictionary);
                    clServiceResponse.AddResponse(CobraServiceResponse.RSP_KeyDetect.ToString(), (lcDataRowDictionary != null).ToString().ToLower());
                    clServiceResponse.Success = true;
                }                
            }
        }

        //private void PlaceOrder()
        //{
        //    String lcItemList;
        //    String lcDeliveryInfo;
        //    OrderInfoManager lcOrderInfoManager;
        //    int lcOrderNo;

        //    if ((!String.IsNullOrWhiteSpace(lcItemList = ApplicationFrame.GetParameter(ctARGItemList))) &&
        //        (!String.IsNullOrWhiteSpace(lcDeliveryInfo = ApplicationFrame.GetParameter(ctARGDeliveryInfo))))
        //    {
        //        if ((lcOrderInfoManager = OrderInfoManager.CreateInstance(General.Base64Decode(lcDeliveryInfo), General.Base64Decode(lcItemList))) != null)                
        //            if((lcOrderNo = lcOrderInfoManager.SubmitNewOrder()) > 0)
        //            {
        //                clServiceResponse.AddResponse(CobraServiceResponse.RSP_Result.ToString(), lcOrderNo);
        //                clServiceResponse.Success = true;           
        //            }                
        //    }
        //}

        //private void UpdateOrder()
        //{
        //    int     lcOrderNo;
        //    String  lcRemark;
        //    String  lcItemList;            
        //    OrderInfoManager lcOrderInfoManager;
            
        //    if ((!String.IsNullOrWhiteSpace(lcItemList = ApplicationFrame.GetParameter(ctARGItemList))) && 
        //        ((lcOrderNo = General.ParseInt(ApplicationFrame.GetParameter(ctARGParameter), -1)) > 0))
        //    {
        //        if ((lcOrderInfoManager = OrderInfoManager.CreateInstance(lcOrderNo, General.Base64Decode(lcItemList))) != null)
        //            if (lcOrderInfoManager.UpdateOrderDetail())
        //            {
        //                lcRemark = ApplicationFrame.GetParameter(ctARGSecondParameter, String.Empty);
        //                lcOrderInfoManager.UpdateOrderInfoRemark(lcRemark);
        //                clServiceResponse.Success = true;
        //            }
        //    }
        //}

        //private void POSUpdateReceiptRecord()
        //{
        //    String              lcMasterBlock;
        //    String              lcDetailListBlock;
        //    int                 lcReceiptNo;
            
        //    if ((!String.IsNullOrWhiteSpace(lcMasterBlock = ApplicationFrame.GetParameter(ctARGMasterBlock))) &&
        //        (!String.IsNullOrWhiteSpace(lcDetailListBlock = ApplicationFrame.GetParameter(ctARGDetailListBlock))))
        //    {
        //        if ((lcReceiptNo = POSReceiptManager.UpdateReceiptRecord(General.Base64Decode(lcMasterBlock), lcDetailListBlock)) > 0)
        //        {
        //            clServiceResponse.AddResponse(CobraServiceResponse.RSP_Serial.ToString(), lcReceiptNo.ToString());
        //            clServiceResponse.Success = true;
        //        }
        //    }
        //}

        private void GetUpdatedControl()
        {
            CompositeForm   lcForm;
            String          lcUpdatedControl;
            String          lcFormParam;
            String          lcRenderMode;
            
            if ((lcFormParam = ApplicationFrame.GetParameter(ctARGParamBlock))  != null)
            {
                lcFormParam = General.Base64Decode(lcFormParam);
                ApplicationFrame.GetInstance().ActiveFormInfoManager.AddFormParam(lcFormParam, true);
            }

            lcForm = new CompositeForm(ApplicationFrame.GetInstance().ActiveFormInfoManager);
            
            lcRenderMode = ApplicationFrame.GetParameter(ctARGParameter);

            lcUpdatedControl = lcForm.GetSerializedControl(true, lcRenderMode);
            
            clServiceResponse.Success = true;
            clServiceResponse.AddResponse(CobraServiceResponse.RSP_HTML.ToString(), lcUpdatedControl);
        }

        //private void RetrieveLazyLoadItemList()
        //{
        //    String    lcWidgetTypeName;
        //    dynamic   lcWidgetControlRenderer;
        //    Type      lcType;
        //    String    lcOutputData;

        //    if ((!String.IsNullOrWhiteSpace(lcWidgetTypeName = ApplicationFrame.GetParameter(ctARGParameter))) && ((lcType = General.FindType(lcWidgetTypeName)) != null))
        //    {
        //        try
        //        {                    
        //            lcWidgetControlRenderer = Activator.CreateInstance(typeof(WidgetAjaxGridModeRenderingController<>).MakeGenericType(lcType));

        //            lcOutputData = lcWidgetControlRenderer.GetSerializedAjaxData();
        //            clServiceResponse.AddResponse(CobraServiceResponse.RSP_HTML.ToString(), lcOutputData);
        //            clServiceResponse.AddResponse<String, object>(lcWidgetControlRenderer.GetAjaxPagingInfo());
        //            clServiceResponse.Success = true;                        
        //        }
        //        catch(Exception paException)
        //        {
        //            clServiceResponse.AddResponse(CobraServiceResponse.RSP_HTML.ToString(), paException.Message);
        //        }
        //    }
        //}        
    }
}
