using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using CobraFoundation;

namespace CobraFrame
{
    public static class UILogic
    {
        const String ctBuildingNoPrefix = "တိုက္အမွတ္";
        const String ctHomeNoPrefix     = "အမွတ္";
        const String ctFloorSuffix      = "ထပ္";
        const String ctMiniFloorSuffix  = "လႊာ";
        const String ctRoomNoPrefix     = "အခန္း";
        const String ctStreetSuffix     = "လမ္း";
        const String ctTownshipSuffix   = "ၿမိဳ႕နယ္";

        const String ctMinorSeparator   = "၊ ";
        const String ctMajorSeparator   = "။ ";
        const String ctLineBreak        = "<BR/>";

        public static String CompileAddress(DataRow paDataRow)
        {
            UserRow     lcUserRow;
            String      lcBuildingNo;
            String      lcFloor;
            String      lcRoomNo;
            String      lcStreet;
            String      lcQuarter;            
            String      lcTownship;
            String      lcAddressInfo;
            String      lcCompiledString;

            lcUserRow       = new UserRow(paDataRow);
            lcBuildingNo    = lcUserRow.BuildingNo.Trim();
            lcFloor         = lcUserRow.Floor.Trim();
            lcRoomNo        = lcUserRow.RoomNo.Trim();
            lcStreet        = lcUserRow.Street.Trim();
            lcQuarter       = lcUserRow.Quarter.Trim();
            lcTownship      = lcUserRow.Township.Trim();
            lcAddressInfo   = lcUserRow.AddressInfo.Trim();

            if (lcFloor.Length > 0) 
            {
                lcBuildingNo = ctBuildingNoPrefix + " " + lcBuildingNo + ctMinorSeparator;
                if (lcFloor.Length < 2) lcFloor = lcFloor + " " + ctMiniFloorSuffix + ctMinorSeparator;
                else lcFloor = lcFloor + " " + ctFloorSuffix + ctMinorSeparator;
            }
            else lcBuildingNo = ctHomeNoPrefix + " " + lcBuildingNo + ctMinorSeparator;

            if (lcRoomNo.Length > 0) lcRoomNo = ctRoomNoPrefix + " " + lcRoomNo + ctMinorSeparator;

            if (lcStreet.Length > 0) lcStreet = lcStreet + " " + ctStreetSuffix + ctMinorSeparator;

            if (lcQuarter.Length > 0) lcQuarter = lcQuarter + ctMinorSeparator;

            if (lcTownship.Length > 0) lcTownship = lcTownship + ctTownshipSuffix + ctMajorSeparator;

            if (lcAddressInfo.Length > 0) lcAddressInfo = "(" + lcAddressInfo + ")";
                        
            if (lcFloor.Length > 0 || lcRoomNo.Length > 0)
                lcCompiledString = lcBuildingNo + lcFloor + lcRoomNo + ctLineBreak + lcStreet + lcQuarter + ctLineBreak + lcTownship + ctLineBreak + lcAddressInfo;
            else
               lcCompiledString = lcBuildingNo +  lcFloor + lcRoomNo + lcStreet + ctLineBreak + lcQuarter + lcTownship + ctLineBreak + lcAddressInfo;

            return (lcCompiledString);
        }

        public static String CompileShortAddress(DataRow paDataRow)
        {
            UserRow             lcUserRow;
            String              lcBuildingNo;
            String              lcFloor;
            String              lcStreet;
            String              lcCompiledString;
            
            lcUserRow           = new UserRow(paDataRow);
            lcBuildingNo        = lcUserRow.BuildingNo.Trim();
            lcFloor             = lcUserRow.Floor.Trim();
            lcStreet            = lcUserRow.Street.Trim();
            
            if (lcFloor.Length > 0) 
            {
                lcBuildingNo = ctBuildingNoPrefix + " " + lcBuildingNo + ctMinorSeparator;
                if (lcFloor.Length < 2) lcFloor = lcFloor + " " + ctMiniFloorSuffix + ctMinorSeparator;
                else lcFloor = lcFloor + " " + ctFloorSuffix + ctMinorSeparator;
            }
            else lcBuildingNo = ctHomeNoPrefix + " " + lcBuildingNo + ctMinorSeparator;
            
            if (lcStreet.Length > 0) lcStreet = lcStreet + " " + ctStreetSuffix + ctMinorSeparator;

            lcCompiledString = lcBuildingNo + lcStreet;

            return (lcCompiledString);
        }
    }

    public class GridFilterController
    {
        const String ctFilterSplitter       = ".";
        const String ctWildChar             = "*";
        const String ctParamPlaceHolder     = "$PARAM";
        const String ctSortMetaBlockName    = "Sort";
        const String ctDefaultSortOption    = "Default";

        MetaDataBlockCollection clMetaDataBlockCollection;

        public GridFilterController(String paFilterMetaData)
        {
            clMetaDataBlockCollection = new MetaDataBlockCollection(paFilterMetaData);
        }

        private Dictionary<String, String> DecodeFilterInfo(String  paEncodedFilterInfo)
        {
         //   JavaScriptSerializer        lcJavaScriptSerializer;
            String                      lcJSONString;            

           //  lcJavaScriptSerializer = new JavaScriptSerializer();            

            try 
            { 
                lcJSONString = General.Base64Decode(paEncodedFilterInfo, true);
                return(General.JSONDeserialize<Dictionary<String, String>>(lcJSONString));
            }
            catch
            {
                return(null);
            }
        }

        public String GetMultiFilterStr(String paEncodedFilterInfo)
        {
            Dictionary<String, String>  lcFilterDictionary;
            String                      lcFilterString;
            String                      lcCompiledFilterString;

            lcFilterString          = String.Empty;
            lcCompiledFilterString  = String.Empty;

            if (!String.IsNullOrEmpty(paEncodedFilterInfo))
            {
                lcFilterDictionary = DecodeFilterInfo(paEncodedFilterInfo);

                if (lcFilterDictionary != null)
                {
                    foreach (String lcKey in lcFilterDictionary.Keys)
                    {
                        if (!String.IsNullOrEmpty(lcFilterString = GetFilterValue(lcKey, lcFilterDictionary[lcKey])))
                        {
                            lcCompiledFilterString +=  " And "  + lcFilterString;
                        }
                    }
                }
            }

            return (lcCompiledFilterString.Trim());
        }

        public String GetMultiSortKeyStr(String paEncodedSortInfo)
        {
            Dictionary<String, String> lcSortDictionary;
            String lcSortKeyString;
            String lcCompiledSortKeyString;

            lcSortKeyString = String.Empty;
            lcCompiledSortKeyString = String.Empty;

            if (!String.IsNullOrEmpty(paEncodedSortInfo))
            {
                lcSortDictionary = DecodeFilterInfo(paEncodedSortInfo);

                if (lcSortDictionary != null)
                {
                    foreach (String lcKey in lcSortDictionary.Keys)
                    {
                        if (!String.IsNullOrEmpty(lcSortKeyString = GetFilterValue(ctSortMetaBlockName, lcSortDictionary[lcKey])))
                        {
                            lcCompiledSortKeyString += (lcCompiledSortKeyString.Length > 0 ? "m" : "") + lcSortKeyString;
                        }
                    }
                }
            }

            if (String.IsNullOrWhiteSpace(lcCompiledSortKeyString))            
                lcCompiledSortKeyString = GetFilterValue(ctSortMetaBlockName, ctDefaultSortOption);

            return (lcCompiledSortKeyString.Trim());
        }

        //public String GetMultiFilterStr(String paFilterInfo)
        //{
        //    String[]   lcFilterList;
        //    String     lcFilterString;

        //    lcFilterString = String.Empty;

        //    if ((paFilterInfo != null) && ((lcFilterList =  paFilterInfo.Split(new String[] { ctFilterSplitter }, 2, StringSplitOptions.RemoveEmptyEntries)).Length > 0))
        //    {
        //        for (int lcCount = 0; lcCount < lcFilterList.Length; lcCount++)
        //        {
        //            lcFilterList[lcCount] = GetFilterStr(lcFilterList[lcCount]);                    
                    
        //            if (!String.IsNullOrWhiteSpace(lcFilterList[lcCount]))
        //                lcFilterString += (lcFilterString.Length > 0 ?  " And " : "") + lcFilterList[lcCount];
        //        }
        //    }

        //    return (lcFilterString.Trim());
        //}

        public String GetFilterStr(String paFilter)
        {
            String[]    lcFilterData;

            if ((paFilter != null) && ((lcFilterData = paFilter.Split(new String[] { ctFilterSplitter }, 2, StringSplitOptions.RemoveEmptyEntries)).Length == 2))            
                return(GetFilterValue(lcFilterData[0], lcFilterData[1]));            
            else return (String.Empty);
        }

        public String GetSortKey(String paSortInfo)
        {
            String lcSortKey;
            
            if (String.IsNullOrWhiteSpace(lcSortKey = GetFilterValue(ctSortMetaBlockName, paSortInfo)))
                lcSortKey = GetFilterValue(ctSortMetaBlockName, ctDefaultSortOption);
            return (lcSortKey);
        }

        private String GetFilterValue(String paBlockName, String paElementName)
        {
            MetaDataBlock       lcMetaDataBlock;
            MetaDataElement     lcMetaDataElement;

            if ((lcMetaDataBlock = clMetaDataBlockCollection[paBlockName]) != null)
            {
                if ((lcMetaDataElement = lcMetaDataBlock[paElementName]) != null) return (lcMetaDataElement[0]);
                else if ((lcMetaDataElement = lcMetaDataBlock[ctWildChar]) != null) return (lcMetaDataElement[0].Replace(ctParamPlaceHolder, paElementName));                    
            }
            return (String.Empty);
        }

    }
}
