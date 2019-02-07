using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using CobraFoundation;
using CobraFrame;

namespace CobraApplicationFrame
{
    public class SubscriptionService
    {
        public enum AutenticationElements { ServiceProviderID, ServiceProviderLogInID, ServiceProviderPassword }
        public enum DataBlockElements     { ServiceRequestType, EserviceID, SubscriptionID, BusinessName, AppName, MobileNo, FrontEndPath, 
                                            InvoiceNo, ItemList, Remark, ServiceFee, Discount }

        public enum ServiceRequestType  { Invalid, CreateSubscription, AdjustSubscription, CancelSubscription, ResetPassword };
        public enum ResponseKey         { Result, Message, AndriodFrontEndLink, AndriodBackEndLink, iOSFrontEndLink, iOSBackEndLink, SubscriptionID }
        public enum Result              { Error, Success }
                
        const String ctERRInvalidParameter      = "Invalid Parameter.";
        const String ctERRInvalidRequestType    = "Invalid Service Request Type.";
        const String ctERRUnexpectedError       = "Unexpected error occur.";

        const String ctDelimiter                = ";";
                
        const String ctSTAActive                = "ACTIVE";

        HttpContext                 clHttpContext;
        Dictionary<String, String>  clAuthenticationBlock;
        Dictionary<String, String>  clDataBlock;
        
  
        public static SubscriptionService CreateInstance(HttpContext paHttpContext, String paAuthenticationBlock, String paDataBlock)
        {
            Dictionary<String, String>  lcAuthenticationBlock;
            Dictionary<String, String>  lcDataBlock;

            if ((!String.IsNullOrEmpty(paAuthenticationBlock)) && (!String.IsNullOrEmpty(paDataBlock)))
            {
                if (((lcAuthenticationBlock = DecryptBlock(paAuthenticationBlock)) != null) && ((lcDataBlock = DecryptBlock(paDataBlock)) != null))
                {
                    return (new SubscriptionService(paHttpContext, lcAuthenticationBlock, lcDataBlock));
                }
            }

            ResponseErrorMessage(paHttpContext, ctERRInvalidParameter);
            return (null);
        }

        private static Dictionary<String, String> DecryptBlock(String paBlock)
        {
            // JavaScriptSerializer lcJavaScriptSerializer;
            Dictionary<String, String> lcDictionary;
            String lcJSONString;

            // lcJavaScriptSerializer = new JavaScriptSerializer();

            try
            {
               // lcJSONString = RijdaelEncryption.GetInstance().DecryptString(paBlock);
                lcJSONString = General.Base64Decode(paBlock);
                lcDictionary = General.JSONDeserialize<Dictionary<String, String>>(lcJSONString);
                lcDictionary = new Dictionary<string, string>(lcDictionary, StringComparer.OrdinalIgnoreCase);

                return (lcDictionary);
            }
            catch
            {
                return (null);
            }
        }

        private static void ResponseErrorMessage(HttpContext paHttpContext, String paErrorMessage)
        {
            // JavaScriptSerializer lcJavascriptSerializer;
            Dictionary<String, String> lcResponse;

            lcResponse = new Dictionary<string, string>();
            // lcJavascriptSerializer = new JavaScriptSerializer();

            lcResponse.Add(ResponseKey.Result.ToString().ToLower(), Result.Error.ToString().ToLower());
            lcResponse.Add(ResponseKey.Message.ToString().ToLower(), paErrorMessage);

            paHttpContext.Response.Write(General.JSONSerialize(lcResponse));
        }        

        private static void ResponseInfoMessage(HttpContext paHttpContext, ResponseKey paResponseKey, String paResponseData)
        {
            // JavaScriptSerializer lcJavascriptSerializer;
            Dictionary<String, String> lcResponse;

            lcResponse = new Dictionary<string, string>();
            // lcJavascriptSerializer = new JavaScriptSerializer();

            lcResponse.Add(ResponseKey.Result.ToString().ToLower(), Result.Success.ToString().ToLower());
            lcResponse.Add(paResponseKey.ToString().ToLower(), paResponseData);

            paHttpContext.Response.Write(General.JSONSerialize(lcResponse));
        }

        private SubscriptionService(HttpContext paHttpContext, Dictionary<String, String> paAuthenticationBlock, Dictionary<String, String> paDataBlock)
        {
            clHttpContext         = paHttpContext;
            clAuthenticationBlock = paAuthenticationBlock;
            clDataBlock           = paDataBlock;            
        }

        private String GetAuthenticationElement(AutenticationElements paAuthenticationElement)
        {
            String lcValue;

            if (!clAuthenticationBlock.TryGetValue(paAuthenticationElement.ToString(), out lcValue)) return (String.Empty);
            else return (lcValue);
        }

        private String GetDataBlockElement(DataBlockElements paDataBlockElement)
        {
            String lcValue;

            if (!clDataBlock.TryGetValue(paDataBlockElement.ToString(), out lcValue)) return (String.Empty);
            else return (lcValue);
        }        

        private String ExceuteCreateSubscriptionQuery()
        {
            QueryClass lcQueryClass;

            lcQueryClass = new QueryClass(QueryClass.QueryType.SPRCreateSubscription);
            lcQueryClass.ReplaceDictionaryPlaceHolder(clAuthenticationBlock);
            lcQueryClass.ReplaceDictionaryPlaceHolder(clDataBlock);

            return(Convert.ToString(lcQueryClass.GetResult()));
        }        

        private void ResponseSubscriptionInfo(String paSubscriptionInfo)
        {
            String[] lcInfoList;
            String   lcAndriodFrontEndLink;
            String   lcAndriodBackEndLink;
            String   lciOSFrontEndLink;
            String   lciOSBackEndLink;
            // JavaScriptSerializer lcJavascriptSerializer;
            Dictionary<String, String> lcResponseDictionary;
            
            try
            {
                lcResponseDictionary = new Dictionary<string, string>();

                lcInfoList = paSubscriptionInfo.Split(new String[] { ctDelimiter }, StringSplitOptions.RemoveEmptyEntries);
                                
                lcAndriodFrontEndLink = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/" + clDataBlock[DataBlockElements.FrontEndPath.ToString().ToLower()];
                lcAndriodBackEndLink = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "?_e=" + lcInfoList[1];
                lciOSFrontEndLink = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/" + clDataBlock[DataBlockElements.FrontEndPath.ToString().ToLower()] + "?_sysreq=manifest.iosfrontend";
                lciOSBackEndLink = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "?_e=" + lcInfoList[1] + "&_sysreq=manifest.iosbackend";

                lcResponseDictionary.Add(ResponseKey.Result.ToString().ToLower(), Result.Success.ToString().ToLower());
                lcResponseDictionary.Add(ResponseKey.SubscriptionID.ToString().ToLower(), lcInfoList[0]);
                lcResponseDictionary.Add(ResponseKey.AndriodFrontEndLink.ToString().ToLower(), lcAndriodFrontEndLink);
                lcResponseDictionary.Add(ResponseKey.AndriodBackEndLink.ToString().ToLower(), lcAndriodBackEndLink);
                lcResponseDictionary.Add(ResponseKey.iOSFrontEndLink.ToString().ToLower(), lciOSFrontEndLink);
                lcResponseDictionary.Add(ResponseKey.iOSBackEndLink.ToString().ToLower(), lciOSBackEndLink);

               // lcJavascriptSerializer = new JavaScriptSerializer();

                clHttpContext.Response.Write(General.JSONSerialize(lcResponseDictionary));
            }
            catch(Exception paException)
            {
                ResponseErrorMessage(clHttpContext, paException.Message);
            }
        }

        private void CreateSubscription()
        {
            String      lcReturnVal;            

            try
            {
                lcReturnVal = ExceuteCreateSubscriptionQuery();
                ResponseSubscriptionInfo(lcReturnVal);
            }
            catch(Exception paExeption)
            {
                if (paExeption.InnerException != null) ResponseErrorMessage(clHttpContext, paExeption.InnerException.Message);
                else ResponseErrorMessage(clHttpContext, paExeption.Message);
            }
        }

        public void RenderServiceRequest()
        {
            ServiceRequestType  lcServiceRequestType;          

            lcServiceRequestType = General.ParseEnum<ServiceRequestType>(GetDataBlockElement(DataBlockElements.ServiceRequestType), ServiceRequestType.Invalid);

            switch(lcServiceRequestType)
            {
                case ServiceRequestType.CreateSubscription:
                    {

                        CreateSubscription();
                        break;
                    }

                case ServiceRequestType.Invalid             : ResponseErrorMessage(clHttpContext, ctERRInvalidRequestType); break;
            }
        }

        //public static void TestCreation(HttpContext paCurrent)
        //{
        //    JavaScriptSerializer lcJS;

        //    Dictionary<String, String>  lcA;
        //    Dictionary<String, String>  lcD;
        //    String lcAStr;
        //    String lcDStr;

        //    lcA = new Dictionary<string,string>();
        //    lcD = new Dictionary<string,string>();

        //    lcA.Add("ServiceProviderID","D927CBD1-46E0-4A62-9A55-5CD1349075DB");
        //    lcA.Add("ServiceProviderLoginID","msc");
        //    lcA.Add("ServiceProviderPassword","111111");

        //    lcD.Add("ServiceRequestType","CreateSubscription");
        //    lcD.Add("EServiceID", "EAC00003");
        //    lcD.Add("ItemList", "");
        //    lcD.Add("InvoiceNo","");            
        //    lcD.Add("ServiceFee","30000");
        //    lcD.Add("Discount","3000");
        //    lcD.Add("Remark","Test");
        //    lcD.Add("FrontEndPath","Testing22");
        //    lcD.Add("BackgroundColor","#ffffff");
        //    lcD.Add("FrontEndIcon","FEIcon");
        //    lcD.Add("BackEndIcon","BEIcon");
        //    lcD.Add("BusinessName", "BUSINESS NAME");
        //    lcD.Add("MobileNo","633466666");
        //    lcD.Add("APPName", "App Name");

        //    lcJS = new JavaScriptSerializer();

        //    lcDStr = lcJS.Serialize(lcD);
        //    lcAStr = lcJS.Serialize(lcA);

        //    lcDStr = RijdaelEncryption.GetInstance().EncryptString(lcDStr);
        //    lcAStr = RijdaelEncryption.GetInstance().EncryptString(lcAStr);

        //    SubscriptionService.CreateInstance(paCurrent, lcAStr, lcDStr).RenderServiceRequest();
        //}
    }
}
