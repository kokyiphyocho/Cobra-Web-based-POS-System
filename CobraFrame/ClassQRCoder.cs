using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using QRCoder;


namespace CobraFrame
{
    public class DynamicQRCode
    {        
        const int ctDEFPixelPerModule   = 20;
        const String ctFEQRCode         = "FEQRCode.png";
        const String ctBEQRCode         = "BEQRCode.png";

        private int     clPixelPerModule;
        private String  clUrl;


        public DynamicQRCode(String paUrl, int paPixelPerModule = ctDEFPixelPerModule)
        {
            clPixelPerModule = paPixelPerModule;
            clUrl = paUrl;
        }

        public static void ResponseAndriodFrontEndQRCode(HttpContext paHttpContext)
        {
            DynamicQRCode   lcDynamicQRCode;
            Bitmap          lcCloneBitmap;

            if (ApplicationFrame.GetInstance().Status == ApplicationFrame.InitializationStatus.Success)
            {
                lcDynamicQRCode = new DynamicQRCode(ApplicationFrame.GetInstance().ActiveSubscription.GetSubscriptionUrl(SubscriptionManager.UrlType.FrontEnd));
                try
                {
                    using (Bitmap lcBitmap = lcDynamicQRCode.GenerateQRCode())
                    {
                        using (lcCloneBitmap = lcBitmap.Clone(new Rectangle(0, 0, lcBitmap.Width, lcBitmap.Height), System.Drawing.Imaging.PixelFormat.Format1bppIndexed))
                        {
                            paHttpContext.Response.ContentType = "image/png";
                            paHttpContext.Response.AppendHeader("Content-Disposition", "attachment; filename=" + ctFEQRCode);
                            lcCloneBitmap.Save(paHttpContext.Response.OutputStream, ImageFormat.Png);                         
                        }
                    }
                }
                catch (Exception paException) { String lcStr = paException.Message; }
            }
        }

        public static void ResponseAndriodBackEndQRCode(HttpContext paHttpContext)
        {
            DynamicQRCode lcDynamicQRCode;
            Bitmap lcCloneBitmap;

            if (ApplicationFrame.GetInstance().Status == ApplicationFrame.InitializationStatus.Success)
            {
                lcDynamicQRCode = new DynamicQRCode(ApplicationFrame.GetInstance().ActiveSubscription.GetSubscriptionUrl(SubscriptionManager.UrlType.BackEnd));
                try
                {
                    using (Bitmap lcBitmap = lcDynamicQRCode.GenerateQRCode())
                    {
                        using (lcCloneBitmap = lcBitmap.Clone(new Rectangle(0, 0, lcBitmap.Width, lcBitmap.Height), System.Drawing.Imaging.PixelFormat.Format1bppIndexed))
                        {
                            paHttpContext.Response.ContentType = "image/png";
                            paHttpContext.Response.AppendHeader("Content-Disposition", "attachment; filename=" + ctBEQRCode);
                            lcCloneBitmap.Save(paHttpContext.Response.OutputStream, ImageFormat.Png);                         
                        }
                    }
                }
                catch { }
            }
        }

        public static void ResponseIOSFrontEndQRCode(HttpContext paHttpContext)
        {
            DynamicQRCode lcDynamicQRCode;
            Bitmap lcCloneBitmap;

            if (ApplicationFrame.GetInstance().Status == ApplicationFrame.InitializationStatus.Success)
            {
                lcDynamicQRCode = new DynamicQRCode(ApplicationFrame.GetInstance().ActiveSubscription.GetSubscriptionUrl(SubscriptionManager.UrlType.iOSFEManifest));
                try
                {
                    using (Bitmap lcBitmap = lcDynamicQRCode.GenerateQRCode())
                    {
                        using (lcCloneBitmap = lcBitmap.Clone(new Rectangle(0, 0, lcBitmap.Width, lcBitmap.Height), System.Drawing.Imaging.PixelFormat.Format1bppIndexed))
                        {
                            paHttpContext.Response.ContentType = "image/png";
                            paHttpContext.Response.AppendHeader("Content-Disposition", "attachment; filename=" + ctFEQRCode);
                            lcCloneBitmap.Save(paHttpContext.Response.OutputStream, ImageFormat.Png);                         
                        }
                    }
                }
                catch { }
            }
        }

        public static void ResponseIOSBackEndQRCode(HttpContext paHttpContext)
        {
            DynamicQRCode lcDynamicQRCode;
            Bitmap lcCloneBitmap;

            if (ApplicationFrame.GetInstance().Status == ApplicationFrame.InitializationStatus.Success)
            {
                lcDynamicQRCode = new DynamicQRCode(ApplicationFrame.GetInstance().ActiveSubscription.GetSubscriptionUrl(SubscriptionManager.UrlType.iOSBEManifest));
                try
                {
                    using (Bitmap lcBitmap = lcDynamicQRCode.GenerateQRCode())
                    {
                        using (lcCloneBitmap = lcBitmap.Clone(new Rectangle(0, 0, lcBitmap.Width, lcBitmap.Height), System.Drawing.Imaging.PixelFormat.Format1bppIndexed))
                        {
                            paHttpContext.Response.ContentType = "image/png";
                            paHttpContext.Response.AppendHeader("Content-Disposition", "attachment; filename=" + ctBEQRCode);
                            lcCloneBitmap.Save(paHttpContext.Response.OutputStream, ImageFormat.Png);                         
                        }
                    }
                }
                catch { }
            }
        }

        

        private Bitmap GenerateQRCode()
        {
            PayloadGenerator.Url    lcUrl;
            String                  lcPayLoad;
            QRCodeGenerator         lcQRCodeGenerator;
            QRCodeData              lcQRCodeData;
            QRCode                  lcQRCode;            

            lcUrl       = new PayloadGenerator.Url(clUrl);
            lcPayLoad   = lcUrl.ToString();

            lcQRCodeGenerator   = new QRCodeGenerator();
            lcQRCodeData        = lcQRCodeGenerator.CreateQrCode(lcPayLoad, QRCodeGenerator.ECCLevel.Q);
            lcQRCode            = new QRCode(lcQRCodeData);

            return(lcQRCode.GetGraphic(clPixelPerModule));            
        }        
    }
        
    
}


//Url generator = new Url("https://github.com/codebude/QRCoder/");
//string payload = generator.ToString();

//QRCodeGenerator qrGenerator = new QRCodeGenerator();
//QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
//QRCode qrCode = new QRCode(qrCodeData);
//var qrCodeAsBitmap = qrCode.GetGraphic(20);