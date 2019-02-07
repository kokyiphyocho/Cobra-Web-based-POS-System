using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace CobraFrame
{
    public static class CobraExtensions
    {
        const char ctDelimiter = ',';

        public static Dictionary<String, String> GetSubsetList(this IDictionary<String, String> paDictionary, String paKeys, char paDelimiter = ctDelimiter)
        {
            return(paKeys.Split(ctDelimiter).AsEnumerable().Where(e => paDictionary.ContainsKey(e)).ToDictionary(e => e, e => paDictionary[e]));            
        }

        public static Dictionary<String, String> GetMetaSubsetList(this IDictionary<String, String> paDictionary, String paKeys, char paDelimiter = ctDelimiter)
        {
             return(paKeys.Split(new[] { ctDelimiter.ToString() }, StringSplitOptions.RemoveEmptyEntries)
                          .Select(part => part.Split(new[] { "." },StringSplitOptions.RemoveEmptyEntries)).Select(k => k[0]).Distinct().ToDictionary(e => e, e => paDictionary[e]));
        }

        public static String[] GetUniCodeValueList(this IDictionary<String, String> paDictionary, String paKeys, char paDelimiter = ctDelimiter)
        {
            return (paKeys.Split(ctDelimiter).AsEnumerable().Where(e => paDictionary.ContainsKey(e)).Select(e => "N''" + paDictionary[e] + "''").ToArray());
        }

        public static Dictionary<String, String> MergeDictionary(this IDictionary<String, String> paDictionary, Dictionary<String, String> paNewDictionary, bool paOverwrite = false)
        {
            if ((paNewDictionary != null) && (paNewDictionary.Count > 0))
            {
                foreach (String lcKey in paNewDictionary.Keys)
                    if (!paDictionary.ContainsKey(lcKey)) paDictionary.Add(lcKey, paNewDictionary[lcKey]);
                    else if (paOverwrite) paDictionary[lcKey] = paNewDictionary[lcKey];                
            }
            return ((Dictionary<String, String>) paDictionary);
        }

        public static String GetData(this IDictionary<String, String> paDictionary, String paKeyName, String paDefaultData = null)
        {
            if ((!String.IsNullOrEmpty(paKeyName)) && (paDictionary.ContainsKey(paKeyName)))
                return (paDictionary[paKeyName]);
            else return (paDefaultData);
        }

        public static dynamic GetData(this IDictionary<String, dynamic> paDictionary, String paKeyName, dynamic paDefaultData = null)
        {
            if ((!String.IsNullOrEmpty(paKeyName)) && (paDictionary.ContainsKey(paKeyName)))
                return (paDictionary[paKeyName]);
            else return (paDefaultData);
        }
    }


    public class LowercaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower();
        }
    }

}
