using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Data;

namespace CobraFrame
{
    public class MetaDataBlockCollection
    {
        const String ctMetaBlockRegEx = "<!([[](?<blockname>[^]]+)[]]){0,1}(?<content>((?!!>).)*)!>";

        const String ctGRPBlockName = "blockname";
        const String ctGRPContent = "content";

        protected const String ctSeparator = ".";

        List<MetaDataBlock> clMetaDataBlockCollection;

        public int MetaDataBlockCount { get { return (clMetaDataBlockCollection == null ? 0 : clMetaDataBlockCollection.Count); } }

        public MetaDataBlock this[int paBlockIndex]     { get { return (GetMetaDataBlock(paBlockIndex)); } }
        public MetaDataBlock this[String paBlockName]   { get { return (GetMetaDataBlock(paBlockName)); } }

        public List<MetaDataBlock> MetaDataBlockList    { get { return (clMetaDataBlockCollection); } }

        public MetaDataBlockCollection()
        {
            clMetaDataBlockCollection = new List<MetaDataBlock>();
        }

        static public bool IsMetaBlockString(String paMetaBlockStr)
        {
            if (!String.IsNullOrEmpty(paMetaBlockStr))
                return (Regex.Match(paMetaBlockStr, ctMetaBlockRegEx).Success);
            else return (false);
        }

        public MetaDataBlockCollection(String paMetaData)
        {
            MatchCollection     lcMatches;
            String              lcBlockName;

            clMetaDataBlockCollection = new List<MetaDataBlock>();

            if (paMetaData == null) paMetaData = String.Empty;

            if ((lcMatches = Regex.Matches(paMetaData, ctMetaBlockRegEx)).Count > 0)
                foreach (Match lcMatch in lcMatches)
                {
                    if (lcMatch.Groups[ctGRPBlockName].Success) lcBlockName = lcMatch.Groups[ctGRPBlockName].Value;
                    else lcBlockName = null;
                    clMetaDataBlockCollection.Add(new MetaDataBlock(lcBlockName, lcMatch.Groups[ctGRPContent].Value));
                }
        }        

        public String GetCompiledMetaDatatBlockCollectionString()
        {
            String lcMetaDataBlockStr;

            lcMetaDataBlockStr = String.Empty;

            foreach (MetaDataBlock lcMetaDataBlock in clMetaDataBlockCollection)
                lcMetaDataBlockStr += lcMetaDataBlock.GetCompiledMetaDataBlockString();

            return (lcMetaDataBlockStr);
        }

        public void AppendMetaDataBlock(MetaDataBlock paMetaDataBlock)
        {
            if (paMetaDataBlock != null)
            {                
                clMetaDataBlockCollection.Add(paMetaDataBlock);
            }
        }

        public void InsertMetaDataBlock(int paIndex, MetaDataBlock paMetaDataBlock)
        {
            if (paMetaDataBlock != null)
            {
                clMetaDataBlockCollection.Insert(paIndex, paMetaDataBlock);
            }
        }

        public void ReplaceMetaDataBlock(MetaDataBlock paMetaDataBlock)
        {            
            if (paMetaDataBlock != null)
            {
                if (!String.IsNullOrWhiteSpace(paMetaDataBlock.MetaDataBlockName)) RemoveMetaDataBlock(paMetaDataBlock.MetaDataBlockName);
                clMetaDataBlockCollection.Add(paMetaDataBlock);
            }
        }

        public void AppendMetaDataBlockCollection(MetaDataBlockCollection paOtherMetaBlockCollection)
        {
            for (int lcCount = 0; lcCount < paOtherMetaBlockCollection.MetaDataBlockCount; lcCount++)
            {
                clMetaDataBlockCollection.Add(paOtherMetaBlockCollection[lcCount]);
            }
        }

        public MetaDataBlock GetMetaDataBlock(int paBlockIndex)
        {
            if (paBlockIndex < clMetaDataBlockCollection.Count) return (clMetaDataBlockCollection[paBlockIndex]);
            else return (null);
        }

        public MetaDataBlock GetMetaDataBlock(String paBlockName)
        {
            int lcBlockIndex;

            foreach (MetaDataBlock lcMetaDataBlock in clMetaDataBlockCollection)
                if ((lcMetaDataBlock.MetaDataBlockName != null) && (lcMetaDataBlock.MetaDataBlockName.Trim().ToUpper() == paBlockName.Trim().ToUpper())) return (lcMetaDataBlock);

            if (int.TryParse(paBlockName, out lcBlockIndex)) return (GetMetaDataBlock(lcBlockIndex));
            else return (null);
        }        

        public bool RemoveMetaDataBlock(String paBlockName)
        {
            MetaDataBlock lcMetaDataBlock;

            if (!String.IsNullOrEmpty(paBlockName))
            {
                if ((lcMetaDataBlock = GetMetaDataBlock(paBlockName)) != null)
                {
                    clMetaDataBlockCollection.Remove(lcMetaDataBlock);
                    return (true);
                }                
            }

            return (false);
        }

        private MetaDataElement FindElementExact(String paElementName)
        {
            String[] lcSplittedStr;
            MetaDataBlock lcMetaDataBlock;

            lcSplittedStr = paElementName.Split(new String[] { ctSeparator }, StringSplitOptions.None);

            if (lcSplittedStr.Length > 1)
            {
                if (String.IsNullOrWhiteSpace(lcSplittedStr[0])) return (FindElement(lcSplittedStr[1]));
                else
                {
                    if ((lcMetaDataBlock = lcMetaDataBlock = this[lcSplittedStr[0]]) != null)
                        return (lcMetaDataBlock[lcSplittedStr[1]]);
                }
            }
            return (null);
        }

        public MetaDataElement FindElement(String paElementName)
        {
            if (paElementName.Contains(ctSeparator)) return (FindElementExact(paElementName));
            else
            {
                foreach (MetaDataBlock lcMetaDataBlock in clMetaDataBlockCollection)
                {
                    if ((lcMetaDataBlock != null) && (lcMetaDataBlock[paElementName] != null))
                        return (lcMetaDataBlock[paElementName]);
                }
            }
            return (null);
        }

        public String FindElementValue(String paElementName, String paDefaultValue = "")
        {
            MetaDataBlock lcMetaDataBlock;
            MetaDataElement lcMetaDataElement;
            String lcBlockName;
            int lcSeparatorIndex;

            if (!String.IsNullOrEmpty(paElementName))
            {
                if ((lcSeparatorIndex = paElementName.IndexOf(ctSeparator)) > 0)
                {
                    lcBlockName = paElementName.Substring(0, lcSeparatorIndex);
                    paElementName = paElementName.Substring(lcSeparatorIndex + 1);

                    if (((lcMetaDataBlock = GetMetaDataBlock(lcBlockName)) != null) && ((lcMetaDataElement = lcMetaDataBlock[paElementName]) != null))
                        return (lcMetaDataElement[0]);
                    else return (paDefaultValue);
                }
                else if ((lcMetaDataElement = FindElement(paElementName)) != null)
                    return (lcMetaDataElement[0]);
            }
            return (paDefaultValue);
        }

        public String FindElementValueExt(String paElementName, String paCombinationDelimiter = "\r\n", String paDefaultValue = "")
        {
            MetaDataBlock lcMetaDataBlock;
            MetaDataElement lcMetaDataElement;
            String lcBlockName;
            int lcSeparatorIndex;

            if (!String.IsNullOrEmpty(paElementName))
            {
                if ((lcSeparatorIndex = paElementName.IndexOf(ctSeparator)) > 0)
                {
                    lcBlockName = paElementName.Substring(0, lcSeparatorIndex);
                    paElementName = paElementName.Substring(lcSeparatorIndex + 1);

                    if (((lcMetaDataBlock = GetMetaDataBlock(lcBlockName)) != null) && ((lcMetaDataElement = lcMetaDataBlock[paElementName]) != null))
                    {
                        if (paCombinationDelimiter != null) return (lcMetaDataElement.GetCombinedValue(paCombinationDelimiter));
                        else return (lcMetaDataElement.GetCombinedValue());
                    }
                    else return (paDefaultValue);
                }
                else if ((lcMetaDataElement = FindElement(paElementName)) != null)
                {
                    if (paCombinationDelimiter != null) return (lcMetaDataElement.GetCombinedValue(paCombinationDelimiter));
                    else return (lcMetaDataElement.GetCombinedValue());
                }
            }
            return (paDefaultValue);
        }

        public String Peep(String paFullyQualifiedElementName, String paDefaultValue = null)
        {
            int lcBlockSeparatorIndex;
            String lcBlockName;
            MetaDataBlock lcMetaDataBlock;

            lcBlockSeparatorIndex = paFullyQualifiedElementName.Contains(ctSeparator) ? paFullyQualifiedElementName.IndexOf(ctSeparator) : paFullyQualifiedElementName.Length;

            lcBlockName = paFullyQualifiedElementName.Substring(0, lcBlockSeparatorIndex);
            paFullyQualifiedElementName = paFullyQualifiedElementName.Substring(lcBlockSeparatorIndex + 1);

            if ((lcMetaDataBlock = GetMetaDataBlock(lcBlockName)) != null)
                return (lcMetaDataBlock.Peep(paFullyQualifiedElementName));
            else return (paDefaultValue);
        }

        public void Push(String paFullyQualifiedElementName, String paValue)
        {
            int lcBlockSeparatorIndex;
            String lcBlockName;
            MetaDataBlock lcMetaDataBlock;

            lcBlockSeparatorIndex = paFullyQualifiedElementName.Contains(ctSeparator) ? paFullyQualifiedElementName.IndexOf(ctSeparator) : paFullyQualifiedElementName.Length;

            lcBlockName = paFullyQualifiedElementName.Substring(0, lcBlockSeparatorIndex);
            paFullyQualifiedElementName = paFullyQualifiedElementName.Substring(lcBlockSeparatorIndex + 1);

            if ((lcMetaDataBlock = GetMetaDataBlock(lcBlockName)) == null)
            {
                lcMetaDataBlock = new MetaDataBlock(lcBlockName.ToUpper());
                AppendMetaDataBlock(lcMetaDataBlock);
            }

            lcMetaDataBlock.Push(paFullyQualifiedElementName, paValue);
        }
    }

    public class MetaDataBlock
    {
        const String ctElementDelimiter = ";;";

        const String ctMetaBlockRegEx = "<!([[](?<blockname>[^]]+)[]]){0,1}(?<content>((?!!>).)*)!>";

        const String ctGRPBlockName = "blockname";
        const String ctGRPContent = "content";

        const String ctSeparator                = ".";
        const String ctMultiColumnSplitter      = ";;";
        const String ctMetaNameRegex            = "@[[](?<MetaElementName>[^]]*)[]]";
        const String ctGRPMetaElementName       = "MetaElementName";

        String clMetaDataBlockName;
        List<MetaDataElement> clMetaDataElementCollection;

        public int MetaDataElementCount { get { return (clMetaDataElementCollection == null ? 0 : clMetaDataElementCollection.Count); } }
        public String MetaDataBlockName { get { return (clMetaDataBlockName); } }

        public MetaDataElement this[int paMetaElementIndex] { get { return (GetMetaDataElement(paMetaElementIndex)); } }
        public MetaDataElement this[String paMetaElementName] { get { return (GetMetaDataElement(paMetaElementName)); } }

        public MetaDataBlock(String paMetaDataBlockName = null)
        {
            clMetaDataBlockName = paMetaDataBlockName;
            clMetaDataElementCollection = new List<MetaDataElement>();
        }

        public MetaDataBlock(String paMetaDataBlockName, String paMetaDataBlockStr)
        {
            String[] lcElementList;
            Match lcMatch;
            MetaDataElement lcMetaDataElement;

            clMetaDataBlockName = paMetaDataBlockName;
            clMetaDataElementCollection = new List<MetaDataElement>();

            if ((lcMatch = Regex.Match(paMetaDataBlockStr, ctMetaBlockRegEx)).Success)
            {
                paMetaDataBlockStr = lcMatch.Groups[ctGRPContent].Value;
                if ((lcMatch.Groups[ctGRPBlockName].Success) && String.IsNullOrWhiteSpace(clMetaDataBlockName))
                    clMetaDataBlockName = lcMatch.Groups[ctGRPBlockName].Value;
            }

            lcElementList = paMetaDataBlockStr.Split(new String[] { ctElementDelimiter }, StringSplitOptions.None);

            foreach (String lcElementStr in lcElementList)
                if ((lcMetaDataElement = MetaDataElement.CreateMetaDataElement(lcElementStr)) != null)
                    clMetaDataElementCollection.Add(lcMetaDataElement);
        }

        public void AppendMetaBlock(String paMetaData, String paIdentifierPrefix)
        {
            MatchCollection     lcMatches;
            String              lcPrefix;            
            String[]            lcElementList;
            MetaDataElement     lcMetaDataElement;

            if (!String.IsNullOrWhiteSpace(paMetaData))
            {
                if ((lcMatches = Regex.Matches(paMetaData, ctMetaBlockRegEx)).Count > 0)
                    foreach (Match lcMatch in lcMatches)
                    {
                        if (lcMatch.Groups[ctGRPBlockName].Success) lcPrefix = paIdentifierPrefix + ctSeparator + lcMatch.Groups[ctGRPBlockName].Value + ctSeparator;
                        else lcPrefix = paIdentifierPrefix + ctSeparator;

                        lcElementList = lcMatch.Groups[ctGRPContent].Value.Split(new String[] { ctElementDelimiter }, StringSplitOptions.None);

                        foreach (String lcElementStr in lcElementList)
                            if ((lcMetaDataElement = MetaDataElement.CreateMetaDataElement(lcPrefix + lcElementStr)) != null)
                                clMetaDataElementCollection.Add(lcMetaDataElement);
                    }
            }
        }       

        public MetaDataBlock(MetaDataElement paMetaDataElement)
        {
            clMetaDataElementCollection = new List<MetaDataElement>();
            if (paMetaDataElement != null) clMetaDataElementCollection.Add(paMetaDataElement);
        }

        public MetaDataElement GetMetaDataElement(int paMetaElementIndex)
        {
            if ((clMetaDataElementCollection != null) && (paMetaElementIndex < clMetaDataElementCollection.Count))
                return (clMetaDataElementCollection[paMetaElementIndex]);
            else return (null);
        }

        public MetaDataElement GetMetaDataElement(String paMetaElementName)
        {
            if (!String.IsNullOrEmpty(paMetaElementName))
            {
                foreach (MetaDataElement lcMetaDataElement in clMetaDataElementCollection)
                    if (lcMetaDataElement.Name.ToUpper().Trim() == paMetaElementName.ToUpper().Trim()) return (lcMetaDataElement);
            }
            return (null);
        }

        //public List<MetaDataElement> GetPrefixedMetaDataElementList(String paPrefix)
        //{
        //    List<MetaDataElement>   lcMetaDataElementList;

        //    lcMetaDataElementList = new List<MetaDataElement>();

        //    if (!String.IsNullOrEmpty(paPrefix))
        //    {
        //        foreach (MetaDataElement lcMetaDataElement in clMetaDataElementCollection)
        //        {
        //            if (lcMetaDataElement.Name.ToUpper().Trim().StartsWith(paPrefix.ToUpper().Trim()))
        //            {                        
        //                lcMetaDataElementList.Add(lcMetaDataElement);                        
        //            }
        //        }
        //    }

        //    return (lcMetaDataElementList);
        //}

        public void ChangeMetaBlockName(String paNewBlockName)
        {
            clMetaDataBlockName = paNewBlockName;
        }

        public bool RemoveMetaDataElement(String paMetaElementName)
        {
            MetaDataElement lcMetaDataElement;

            if ((lcMetaDataElement = GetMetaDataElement(paMetaElementName)) != null)
            {
                clMetaDataElementCollection.Remove(lcMetaDataElement);
                return (true);
            }
            else return (false);
        }

        public bool RemoveMetaDataElement(int paIndex)
        {
            if (paIndex < clMetaDataElementCollection.Count)
            {
                clMetaDataElementCollection.RemoveAt(paIndex);
                return (true);
            }
            else return (false);
        }

        public String GetCompiledMetaDataBlockString()
        {
            String lcMetaDataElementStr;
            String lcBlockName;

            lcBlockName = String.IsNullOrWhiteSpace(clMetaDataBlockName) ? "" : "[" + clMetaDataBlockName + "]";
            lcMetaDataElementStr = String.Empty;

            foreach (MetaDataElement lcMetaDataElement in clMetaDataElementCollection)
                lcMetaDataElementStr += lcMetaDataElement.GetCompiledMetaDataElementString() + ctElementDelimiter;

            return ("<!" + lcBlockName + lcMetaDataElementStr + "!>");
        }

        public MetaDataElement AddMetaDataElement(String paMetaElementName, String paValueList, bool paMultiValue = true)
        {
            MetaDataElement lcMetaDataElement;
            MetaDataElement lcExistingDataElement;

            if ((!String.IsNullOrEmpty(paMetaElementName)) && ((lcExistingDataElement = GetMetaDataElement(paMetaElementName)) != null))
                clMetaDataElementCollection.Remove(lcExistingDataElement);

            if ((lcMetaDataElement = MetaDataElement.CreateMetaDataElement(paMetaElementName, paValueList, paMultiValue)) != null)
                clMetaDataElementCollection.Add(lcMetaDataElement);

            return (lcMetaDataElement);
        }

        public MetaDataElement AddMetaDataElement(MetaDataElement paMetaDataElement)
        {
            MetaDataElement lcMetaDataElement;
            MetaDataElement lcExistingDataElement;

            lcMetaDataElement = null;

            if (paMetaDataElement != null)
            {
                if ((!String.IsNullOrEmpty(paMetaDataElement.Name)) && ((lcExistingDataElement = GetMetaDataElement(paMetaDataElement.Name)) != null))
                    clMetaDataElementCollection.Remove(lcExistingDataElement);

                lcMetaDataElement = (MetaDataElement)paMetaDataElement.CopyObject();
                clMetaDataElementCollection.Add(lcMetaDataElement);
            }
            return (lcMetaDataElement);
        }

        public void AppendMetaDataBlock(MetaDataBlock paMetaDataBlock)
        {
            if (paMetaDataBlock != null)
            {
                for (int lcCount = 0; lcCount < paMetaDataBlock.MetaDataElementCount; lcCount++)
                    AddMetaDataElement(paMetaDataBlock[lcCount]);
            }
        }

        public String GetElementName(int paMetaElementIndex, String paValueIfError)
        {
            if (paMetaElementIndex < clMetaDataElementCollection.Count)
                return (clMetaDataElementCollection[paMetaElementIndex].Name);
            return (paValueIfError);
        }

        public String GetData(String paMetaElementName, int paValueIndex, String paValueIfError)
        {
            MetaDataElement lcMetaDataElement;

            if ((!String.IsNullOrWhiteSpace(paMetaElementName)) && ((lcMetaDataElement = GetMetaDataElement(paMetaElementName)) != null))
                return (lcMetaDataElement.GetData(paValueIndex, paValueIfError));
            else return (paValueIfError);
        }

        public String GetData(String paMetaElementName, String paValueIfError)
        {
            return (GetData(paMetaElementName, 0, paValueIfError));
        }

        public String GetMultiColumnData(String paComplexMetaName, String paValueIfError)
        {
            String[]  lcElementList;
            Match     lcMatch;
            String    lcCompiledString;

            lcCompiledString = String.Empty;

            if (!String.IsNullOrEmpty(paComplexMetaName))
            {                
                lcElementList = paComplexMetaName.Split(new String[] { ctMultiColumnSplitter },StringSplitOptions.None);

                for (int lcCount = 0; lcCount < lcElementList.Length; lcCount++)
                {
                    if ((lcMatch = Regex.Match(lcElementList[lcCount], ctMetaNameRegex)).Success)
                    {
                        lcCompiledString += GetData(lcMatch.Groups[ctGRPMetaElementName].Value,String.Empty);
                    }
                    else lcCompiledString += lcElementList[lcCount];
                }

            }
            
            return(lcCompiledString);
        }

        public String GetData(int paMetaElementIndex, int paValueIndex, String paValueIfError)
        {
            if (paMetaElementIndex < clMetaDataElementCollection.Count)
                return (clMetaDataElementCollection[paMetaElementIndex].GetData(paValueIndex, paValueIfError));
            return (paValueIfError);
        }

        public String GetData(int paMetaElementIndex, String paValueIfError)
        {
            return (GetData(paMetaElementIndex, 0, paValueIfError));
        }

        public int GetIntData(String paMetaElementName, int paValueIndex, int paValueIfError)
        {
            int lcConvertedValue;
            String lcMetaElementValue;

            if ((lcMetaElementValue = GetData(paMetaElementName, paValueIndex, null)) != null)
                if (int.TryParse(lcMetaElementValue, out lcConvertedValue)) return (lcConvertedValue);

            return (paValueIfError);
        }

        public int GetIntData(String paMetaElementName, int paValueifError)
        {
            return (GetIntData(paMetaElementName, 0, paValueifError));
        }

        public int GetIntData(int paMetaElementIndex, int paValueIndex, int paValueIfError)
        {
            int lcConvertedValue;

            if (paMetaElementIndex < clMetaDataElementCollection.Count)
                if (int.TryParse(clMetaDataElementCollection[paMetaElementIndex].GetData(paValueIndex, null), out lcConvertedValue)) return (lcConvertedValue);

            return (paValueIfError);
        }

        public int GetIntData(int paMetaElementName, int paValueifError)
        {
            return (GetIntData(paMetaElementName, 0, paValueifError));
        }

        public String Peep(String paFullyQualifiedElementName, String paDefaultValue = null)
        {
            int lcSeparatorIndex;
            String lcElementName;
            MetaDataElement lcMetaDataElement;

            lcSeparatorIndex = paFullyQualifiedElementName.Contains(ctSeparator) ? paFullyQualifiedElementName.IndexOf(ctSeparator) : paFullyQualifiedElementName.Length;

            lcElementName = paFullyQualifiedElementName.Substring(0, lcSeparatorIndex);
            paFullyQualifiedElementName = paFullyQualifiedElementName.Substring(lcSeparatorIndex + 1);

            if ((lcMetaDataElement = GetMetaDataElement(lcElementName)) != null)
                return (lcMetaDataElement.Peep(paFullyQualifiedElementName, paDefaultValue));
            else return (paDefaultValue);
        }

        public void Push(String paFullyQualifiedElementName, String paValue)
        {
            int lcSeparatorIndex;
            String lcElementName;
            MetaDataElement lcMetaDataElement;

            lcSeparatorIndex = paFullyQualifiedElementName.Contains(ctSeparator) ? paFullyQualifiedElementName.IndexOf(ctSeparator) : paFullyQualifiedElementName.Length;

            lcElementName = paFullyQualifiedElementName.Substring(0, lcSeparatorIndex);

            if (lcSeparatorIndex >= paFullyQualifiedElementName.Length)
            {
                AddMetaDataElement(lcElementName, paValue);
            }
            else
            {
                paFullyQualifiedElementName = paFullyQualifiedElementName.Substring(lcSeparatorIndex + 1);

                if ((lcMetaDataElement = GetMetaDataElement(lcElementName)) == null)
                {
                    lcMetaDataElement = MetaDataElement.CreateMetaDataElement(lcElementName, String.Empty);
                    AddMetaDataElement(lcMetaDataElement);
                }

                lcMetaDataElement.Push(paFullyQualifiedElementName, paValue);
            }
        }
    }

    public class MetaDataElement
    {
        const String ctNameDelimiter = "::";
        const String ctValueDelimiter = "%%";

        String clName;
        ArrayList clValueList;

        public String Name { get { return (clName); } }
        public int ValueCount { get { return (clValueList == null ? 0 : clValueList.Count); } }

        public String this[int paValueIndex]
        {
            get { return (GetData(paValueIndex, null)); }
            set { SetData(paValueIndex, value); }
        }


        static public MetaDataElement CreateMetaDataElement(String paMetaDataElementStr)
        {
            MetaDataElement lcMetaDataElement;

            lcMetaDataElement = new MetaDataElement(paMetaDataElementStr);

            if (!String.IsNullOrWhiteSpace(lcMetaDataElement.Name)) return (lcMetaDataElement);
            else return (null);
        }

        static public MetaDataElement CreateMetaDataElement(String paName, String paValueList, bool paMultiValue = true)
        {
            MetaDataElement lcMetaDataElement;

            lcMetaDataElement = new MetaDataElement(paName, paValueList, paMultiValue);

            if (!String.IsNullOrWhiteSpace(lcMetaDataElement.Name)) return (lcMetaDataElement);
            else return (null);
        }

        private MetaDataElement(String paMetaDataElementStr)
        {
            String[] lcSplittedParts;

            clValueList = new ArrayList();

            if (((lcSplittedParts = paMetaDataElementStr.Split(new String[] { ctNameDelimiter }, StringSplitOptions.None)).Length == 2) &&
                (!String.IsNullOrWhiteSpace(lcSplittedParts[0])))
            {
                clName = lcSplittedParts[0];
                clValueList.AddRange(lcSplittedParts[1].Split(new String[] { ctValueDelimiter }, StringSplitOptions.None));
            }
            else clName = null;
        }

        public void SetElementName(String paElementName)
        {
            clName = paElementName;
        }

        public MetaDataElement CopyObject()
        {
            return (new MetaDataElement(Name, GetValueListStr()));
        }

        public String GetValueListStr()
        {
            String lcValueListStr;

            lcValueListStr = "";

            if (clValueList.Count > 0)
            {
                for (int lcCount = 0; lcCount < clValueList.Count; lcCount++)
                    lcValueListStr += clValueList[lcCount] + ctValueDelimiter;
                lcValueListStr = lcValueListStr.Substring(0, lcValueListStr.Length - 2);
            }

            return (lcValueListStr);
        }

        public String GetCombinedValue(String paSeparator = "\r\n")
        {
            String lcFinalString;

            lcFinalString = String.Empty;

            for (int lcCount = 0; lcCount < clValueList.Count; lcCount++)
                if ((clValueList[lcCount] != null) && (!String.IsNullOrEmpty(clValueList[lcCount].ToString())))
                    lcFinalString += (lcFinalString.Length > 0 ? paSeparator : "") + clValueList[lcCount].ToString();

            return (lcFinalString);
        }

        public String GetCompiledMetaDataElementString()
        {
            String lcMetaDataElementString;

            lcMetaDataElementString = clName + ctNameDelimiter;

            for (int lcCount = 0; lcCount < clValueList.Count; lcCount++)
                lcMetaDataElementString += (lcCount > 0 ? ctValueDelimiter : "") + (String)clValueList[lcCount];

            return (lcMetaDataElementString);
        }

        private MetaDataElement(String paName, String paValueList, bool paMultiValue = true)
        {
            clValueList = new ArrayList();

            if ((!String.IsNullOrWhiteSpace(paName)) && (paValueList != null))
            {
                clName = paName;
                if (paMultiValue) clValueList.AddRange(paValueList.Split(new String[] { ctValueDelimiter }, StringSplitOptions.None));
                else clValueList.Add(paValueList);
            }
            else clName = null;
        }

        public String GetData(int paIndex, String paValueIfError)
        {
            if (paIndex < clValueList.Count) return (String.IsNullOrWhiteSpace((String)clValueList[paIndex]) ? String.Empty : (String)clValueList[paIndex]);
            else return (paValueIfError);
        }

        public void SetData(int paIndex, String paNewValue)
        {
            if (paIndex < clValueList.Count)
                clValueList[paIndex] = paNewValue == null ? String.Empty : paNewValue;
        }

        public int GetIntData(int paIndex, int paValueIfError)
        {
            int lcConvertedValue;

            if (int.TryParse(GetData(paIndex, null), out lcConvertedValue)) return (lcConvertedValue);
            return (paValueIfError);
        }

        public void AddNewElement(object paNewData)
        {
            clValueList.Add(paNewData);
        }

        public void InsertNewElement(int paIndex, object paNewData)
        {
            clValueList.Insert(paIndex, paNewData);
        }

        public String Peep(String paFullyQualifiedElementName, String paDefaultValue)
        {
            int lcValueIndex;

            if (int.TryParse(paFullyQualifiedElementName, out lcValueIndex))
                return ((String)clValueList[lcValueIndex]);
            else
                if (clValueList.Count > 0) return ((String)clValueList[0]);

            return (paDefaultValue);
        }

        public void Push(String paFullyQualifiedElementName, String paValue)
        {
            int lcValueIndex;

            if (!int.TryParse(paFullyQualifiedElementName, out lcValueIndex))
                lcValueIndex = 0;

            if (clValueList.Count <= lcValueIndex)
                for (int lcCount = 0; lcCount < (lcValueIndex - clValueList.Count) + 1; lcCount++)
                    clValueList.Add(String.Empty);

            clValueList[lcValueIndex] = paValue;
        }
    }

    public class MetaDataRow
    {
        const String ctDelimiter = ".";

        String[] clDateFormatString = { "yyyy-M-d", "dd/MM/yyyy" };

        DataRow                     clDataRow;
        MetaDataBlock               clMetaDataBlock;
        Dictionary<String, String>  clFormatStringList;
        Dictionary<String, String>  clDataBlock;

        public DataRow                  ActiveRow           { get { return (clDataRow); } }
        public MetaDataBlock            ActiveData          { get { return (clMetaDataBlock); } }
        public bool                     HasPreloadedData    { get { return (ActiveRow != null); } }

        public MetaDataRow(DataRow paDataRow, Dictionary<String, String> paFormatStringList = null)
        {
            clDataRow               = paDataRow;
            clFormatStringList      = paFormatStringList == null ? new Dictionary<String, String>() : paFormatStringList;
            clMetaDataBlock         = CompileReadMetadataBlockCollection(paDataRow);
        }

        public MetaDataRow(Dictionary<String, String> paDataBlock, DataRow paDataRow)
        {
            if ((paDataRow != null) && (paDataBlock != null))
            {                
                clDataBlock = paDataBlock;
                clDataRow = paDataRow;                
            }
        }               

        private String GetDisplayString(object paValue)
        {    
            switch (paValue.GetType().ToString())
            {
                case "System.DateTime": return (((DateTime)paValue).ToString("dd/MM/yyyy"));
                case "System.DBNull": return (String.Empty);
                case "System.Decimal": return (Convert.ToDecimal(paValue) % 1 > 0 ? ((Decimal)paValue).ToString("F2") : ((Decimal)paValue).ToString("F0"));                    
                default: return (paValue.ToString());
            }
        }

        private String GetDisplayString(object paValue, String paFormatString)
        {
            if (!String.IsNullOrEmpty(paFormatString))
            {
                try { return(((dynamic)paValue).ToString(paFormatString)); }
                catch {  }
            }
            return (GetDisplayString(paValue));
        }

        private String GetFormatString(String paColumnName)
        {
            String lcValue;

            if (clFormatStringList.TryGetValue(paColumnName, out lcValue)) return (lcValue);
            else return (null);
        }

        private MetaDataBlock CompileReadMetadataBlockCollection(DataRow paDataRow)
        {   
            String          lcColumnName;
            MetaDataBlock   lcMetaBlock;

            lcMetaBlock = new MetaDataBlock();            

            if (paDataRow != null)
            {
                
                for (int lcCount = 0; lcCount < paDataRow.Table.Columns.Count; lcCount++)
                {
                    lcColumnName = paDataRow.Table.Columns[lcCount].ColumnName;
                    
                    if ((paDataRow.Table.Columns[lcCount].DataType == typeof(String)) && (MetaDataBlockCollection.IsMetaBlockString(paDataRow[lcColumnName].ToString())))
                    {
                        lcMetaBlock.AppendMetaBlock(paDataRow[lcColumnName].ToString(), lcColumnName);                        
                    }
                    else lcMetaBlock.AddMetaDataElement(lcColumnName, GetDisplayString(paDataRow[lcColumnName], GetFormatString(lcColumnName)), false);                    
                }
            }

            return (lcMetaBlock);
        }

        public Dictionary<String,String> DictGetCompiledDataList()
        {
            return(CompileWriteMetadataBlockCollection());
        }

        private Dictionary<String,String> CompileWriteMetadataBlockCollection()
        {
            String[] lcMetaDataKey;
            String[] lcNonMetaDataKey;
            Dictionary<String, String>  lcCompiledDictionary;

            lcCompiledDictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            lcNonMetaDataKey = clDataBlock.Where(e => !e.Key.Contains(ctDelimiter)).OrderBy(e => e.Key).Select(e => e.Key).ToArray();
            lcMetaDataKey    = clDataBlock.Where(e => e.Key.Contains(ctDelimiter)).Select(e => e.Key.Substring(0, e.Key.IndexOf(ctDelimiter))).Distinct().ToArray();

            PopulateNonMetaData(lcNonMetaDataKey, lcCompiledDictionary);

            if (ActiveRow == null) PopulateNewRowMetaData(lcMetaDataKey, lcCompiledDictionary);
            else PopulateExistingRowMetaData(lcMetaDataKey, lcCompiledDictionary);

            return (lcCompiledDictionary);
        }

        private object ConvertData(Type paDataType, String paDataString)
        { 
            try
            {
                switch(Type.GetTypeCode(paDataType))
                {
                    case TypeCode.DateTime: return (DateTime.ParseExact(paDataString, clDateFormatString, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None));
                    default: return(Convert.ChangeType(paDataString, paDataType));
                }
            }
            catch{ return(null); }
        }

        private void PopulateNonMetaData(String[] paNonMetaKey, Dictionary<string, string> paCompiledDictionary)
        {            
            for (int lcCount = 0; lcCount < paNonMetaKey.Length; lcCount++)            
                paCompiledDictionary.Add(paNonMetaKey[lcCount], clDataBlock[paNonMetaKey[lcCount]]);                            
        }

        private void PopulateNewRowMetaData(String[] paMetaKey, Dictionary<string, string> paCompiledDictionary)
        {
            Dictionary<String, String> lcDataList;
            String                     lcMetaDataValue;

            for (int lcCount = 0; lcCount < paMetaKey.Length; lcCount++)
            {
                lcDataList = clDataBlock.Where(e => e.Key.StartsWith(paMetaKey[lcCount] + ctDelimiter)).ToDictionary(e => e.Key.Substring(e.Key.IndexOf(ctDelimiter)), e => e.Value);
                lcMetaDataValue = CompileMetaDataColumnValue(new MetaDataBlockCollection(), lcDataList);
                paCompiledDictionary.Add(paMetaKey[lcCount],lcMetaDataValue);
            }
        }

        private void PopulateExistingRowMetaData(String[] paMetaKey, Dictionary<string, string> paCompiledDictionary)
        {
            Dictionary<String, String>  lcDataList;
            String                      lcMetaDataValue;

            for (int lcCount = 0; lcCount < paMetaKey.Length; lcCount++)
            {
                if (clDataRow.Table.Columns.Contains(paMetaKey[lcCount]))
                {
                    lcDataList = clDataBlock.Where(e=>e.Key.StartsWith(paMetaKey[lcCount] + ctDelimiter)).ToDictionary(e=>e.Key.Substring(e.Key.IndexOf(ctDelimiter)), e=>e.Value);
                    lcMetaDataValue = CompileMetaDataColumnValue(new MetaDataBlockCollection(clDataRow[paMetaKey[lcCount]].ToString()), lcDataList);
                    paCompiledDictionary.Add(paMetaKey[lcCount], lcMetaDataValue);
                }
            }                
        }

        private String CompileMetaDataColumnValue(MetaDataBlockCollection paExistingMetaData, Dictionary<String, String> paDataList)
        {
            foreach(String lcKey in paDataList.Keys)
            {
                paExistingMetaData.Push(lcKey, paDataList[lcKey]);
            }

            return(paExistingMetaData.GetCompiledMetaDatatBlockCollectionString());
        }        
    }
}
