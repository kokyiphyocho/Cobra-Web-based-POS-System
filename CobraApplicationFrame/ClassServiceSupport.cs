using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace CobraApplicationFrame
{
    public class FtpManager
    {
        const String ctFtpServerIP = "maroon.mysitehosted.com";
        const String ctFtpUserName = "mobilecentre1720";
        const String ctFtpPassword = "@pple@!123ABC";
        const int ctBufferSize = 4 * 1024;

        //String clFTPHostName;
        //String clFTPUserName;
        //String clFTPPassword;
        //String clFTPPort;

        byte[] clBuffer;
        
        private FtpManager()
        {
            clBuffer = new byte[ctBufferSize];            
        }

        public bool UploadFile(String paFileName)
        {
            FileInfo lcFileInfo;
            FtpWebRequest lcWebRequest;
            FileStream lcFileStream;
            Stream lcStream;
            int lcReadCount;

            if ((!String.IsNullOrEmpty(paFileName)) && (File.Exists(paFileName)))
            {
                lcFileInfo = new FileInfo(paFileName);

                lcWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ctFtpServerIP + "/" + Path.GetFileName(paFileName)));
                lcWebRequest.Credentials = new NetworkCredential(ctFtpUserName, ctFtpPassword);
                lcWebRequest.KeepAlive = false;
                lcWebRequest.UseBinary = true;
                lcWebRequest.ContentLength = lcFileInfo.Length;
                lcWebRequest.Method = WebRequestMethods.Ftp.UploadFile;

                lcFileStream = lcFileInfo.OpenRead();

                try
                {
                    lcStream = lcWebRequest.GetRequestStream();

                    while ((lcReadCount = lcFileStream.Read(clBuffer, 0, ctBufferSize)) != 0)
                    {
                        lcStream.Write(clBuffer, 0, lcReadCount);
                    }

                    lcStream.Close();
                    lcFileStream.Close();

                    return (true);
                }
                catch (Exception paException)
                {
                    throw paException;
                }
            }
            else return (false);
        }
    }
}
