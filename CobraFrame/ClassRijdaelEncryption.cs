using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace CobraFrame
{
    public class RijdaelEncryption
    {
        // This Class intentended to use for temporary unimportant encryption. Key and Vector May Changed anytime.
        const String ctSmartKey = "Sky2016vma6512xy";
        const String ctVector = "65d#M11s#6r9x37@";
        const int ctIVSize = 16;

        private byte[] clKey;
        private byte[] clVector;
        private RijndaelManaged clRijndaelManaged;

        static private RijdaelEncryption clRijdaelEncryption;

        static public RijdaelEncryption GetInstance()
        {
            if (clRijdaelEncryption == null) clRijdaelEncryption = new RijdaelEncryption();
            return (clRijdaelEncryption);
        }

        private RijdaelEncryption()
        {
            clRijndaelManaged = new RijndaelManaged();
            clKey = Encoding.UTF8.GetBytes(ctSmartKey);
            clVector = Encoding.UTF8.GetBytes(ctVector);
        }

        public String EncryptString(String paSourceString)
        {
            byte[] lcEncryptedData;
            ICryptoTransform lcEncrypter;

            if (paSourceString != null)
            {
                clRijndaelManaged.Key = clKey;
                clRijndaelManaged.IV = clVector;

                lcEncrypter = clRijndaelManaged.CreateEncryptor(clRijndaelManaged.Key, clRijndaelManaged.IV);

                using (MemoryStream lcMemoryStream = new MemoryStream())
                {
                    using (CryptoStream lcCryptoStream = new CryptoStream(lcMemoryStream, lcEncrypter, CryptoStreamMode.Write))
                    {
                        using (StreamWriter lcStreamWriter = new StreamWriter(lcCryptoStream))
                        {
                            lcStreamWriter.Write(paSourceString);
                        }
                        lcEncryptedData = lcMemoryStream.ToArray();
                    }
                }

                return (Convert.ToBase64String(lcEncryptedData));
            }
            else return (null);
        }

        public String DecryptString(String paEncryptedStr)
        {
            String lcDecryptedString;
            byte[] lcEncryptedData;
            ICryptoTransform lcDecryptor;

            if (!String.IsNullOrEmpty(paEncryptedStr))
            {
                clRijndaelManaged.Key = clKey;
                clRijndaelManaged.IV = clVector;
                lcDecryptedString = null;
                lcEncryptedData = Convert.FromBase64String(paEncryptedStr);

                lcDecryptor = clRijndaelManaged.CreateDecryptor(clRijndaelManaged.Key, clRijndaelManaged.IV);

                using (MemoryStream lcMemoryStream = new MemoryStream(lcEncryptedData))
                {
                    using (CryptoStream lcCryptoStream = new CryptoStream(lcMemoryStream, lcDecryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader lcSteramReader = new StreamReader(lcCryptoStream))
                        {
                            lcDecryptedString = lcSteramReader.ReadToEnd();
                        }
                    }
                }

                return (lcDecryptedString);
            }
            else return (null);
        }
    }
}
