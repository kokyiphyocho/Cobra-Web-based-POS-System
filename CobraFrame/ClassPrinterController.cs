using CobraFoundation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobraFrame
{
    public class PrinterController
    {
        public enum KeyName { DisplayName, DefaultIP, DefaultPort, DefaultDeviceID, ScriptFileList };

        String[] clPrinterControllerScriptList = new String[] { "PrinterManager.js", "epos-interface.js", "star-interface.js" };
        
        const String ctSETPrinterList       = "POS.PrinterList";
        const char ctDelimiter             = ',';                

        public String                       clPrinterListBase64JSON;        
        public Dictionary<String,dynamic>   clPrinterList;

        public String PrinterListBase64JSON { get { return (clPrinterListBase64JSON); } }

        public enum PrinterType { PrimaryPrinter, KitchenPrinter }

        public static PrinterController clPrinterController;

        public static PrinterController GetInstance()
        {
            if (clPrinterController == null) clPrinterController = new PrinterController();
            return (clPrinterController);
        }

        private PrinterController()
        {
            String lcPrinterListJSON;

            lcPrinterListJSON       = ApplicationFrame.GetInstance().ActiveSubscription.ActiveSetting.GetSettingValue(ctSETPrinterList, "{}");
            clPrinterListBase64JSON = General.Base64Encode(lcPrinterListJSON);

            clPrinterList = General.JSONDeserialize<Dictionary<String, dynamic>>(lcPrinterListJSON);           
        }

        public Dictionary<String, String> GetPrinterNameDictionary()
        {            
            return(clPrinterList.ToDictionary(x=> x.Key,x => x.Key));
        }

        public JObject GetPrinterBlock(String paPrinterName)
        {
            return (clPrinterList[paPrinterName]);
        }
        
        public String GetValue(String paPrinterName, KeyName paKey)
        {
            JObject lcJObject;
            
            if ((lcJObject = clPrinterList[paPrinterName]) != null)
            {
                return(General.GetJValueStr((JValue)lcJObject[paKey.ToString()]));                
            }

            return(String.Empty);
        }

        public String[] GetPrinterControllerScriptList()
        {
            return (clPrinterControllerScriptList);
        }

        public String[] GetDeviceScriptList(String paPrinterName)
        {
            if (!String.IsNullOrEmpty(paPrinterName))
            {
                return (GetValue(paPrinterName, KeyName.ScriptFileList).Split(ctDelimiter));
            }
            else return (null);            
        }

        public String[] GetAllDeviceScriptList()
        {
            String[]  lcAllDeviceList;

            lcAllDeviceList = null;

            foreach (String lcKey in clPrinterList.Keys)
            {
                if (lcAllDeviceList == null) lcAllDeviceList = GetDeviceScriptList(lcKey);
                else lcAllDeviceList = lcAllDeviceList.Concat(GetDeviceScriptList(lcKey)).ToArray();                
            }

            return(lcAllDeviceList);
        }

        //public JObject GetPrinterSetting(Dictionary<String, dynamic> paPrinterSetting, PrinterType paPrinterType)
        //{
        //    if (paPrinterSetting != null)
        //    {
        //        return (paPrinterSetting.GetData(paPrinterType.ToString(), null));
        //    }
        //    else return (null);
        //}

        //public String GetPrinterSettingValue(JObject paPrinterSetting, String paKey)
        //{
        //    JValue  lcJValue = null;

        //    if ((!String.IsNullOrEmpty(paKey)) && (paPrinterSetting != null))
        //    {
        //        if (((lcJValue = (JValue)paPrinterSetting[paKey]) != null) && (lcJValue.Value != null))
        //            return(lcJValue.Value.ToString());
        //    }

        //    return (String.Empty);
        //}

        //public String[] GetDeviceScriptList(JObject paPrinterSetting)
        //{
        //    if (paPrinterSetting != null)
        //    {
        //        paPrinterSetting[ctPrinterName]
        //    }
        //    if (!String.IsNullOrEmpty(paPrinterName))
        //    {
        //        return (GetMetaDataValue(paPrinterName, ctMETAScriptFileList).Split(ctDelimiter));
        //    }
        //    else return (null);
        //}
    }
}
