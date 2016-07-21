using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using System.IO;
using Common;
using System.Web.Security;
using System.Text;
using System.Net;
using System.Drawing;

namespace WebCenter.Web.Controllers
{
    public class CommonController : BaseController
    {

        public CommonController(IUnitOfWork UOF)
            : base(UOF)
        {
        }

        [HttpPost]
        public ActionResult Upload()
        {
            HttpFileCollectionBase files = Request.Files;

            if (files.Count <= 0)
            {
                return Json(new { result = true, url = "" }, JsonRequestBehavior.AllowGet);
            }

            var fileName = files[0].FileName;

            byte[] fileData = null;
            using (var binaryReader = new BinaryReader(files[0].InputStream))
            {
                fileData = binaryReader.ReadBytes(files[0].ContentLength);
            }
            var directory = AppDomain.CurrentDomain.BaseDirectory;
            var uploadDir = Path.Combine(directory, "Uploads");
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            // 不同业务的文件 分不同文件夹保存
            var business = Request.Params["DocType"];
            var folder = "image";
            var isThumbnail = false;
            var size = new Size(80, 80);
            if (!string.IsNullOrEmpty(business))
            {
                switch(business.ToLower())
                {
                    case "profile":
                        folder = "photo";
                        isThumbnail = true;
                        break;
                    case "signform":
                        folder = "doc";
                        isThumbnail = true;
                        size.Height = 80;
                        size.Width = 100;
                        break;
                    case "logo":
                        folder = "logo";
                        isThumbnail = true;
                        break;
                    default:
                        folder = "image";
                        isThumbnail = true;
                        break;
                }
            }

            var folderDir = Path.Combine(uploadDir, folder);
            if (!Directory.Exists(folderDir))
            {
                Directory.CreateDirectory(folderDir);
            }

            var uploadFile = Path.Combine(folderDir, fileName);

            // 防止重复
            if(System.IO.File.Exists(uploadFile))
            {
                var fileArr = fileName.Split('.');
                var _name = DateTime.Now.ToString("yyyyMMddHHmmss");
                fileName = _name + "." + fileArr[1];
            }

            uploadFile = Path.Combine(folderDir, fileName);
            using (FileStream fs = new FileStream(uploadFile, FileMode.Create))
            {
                fs.Write(fileData, 0, fileData.Length);
            }

            var photoUrl = string.Format("{0}://{1}:{2}/Uploads/{3}/{4}", Request.Url.Scheme, Request.Url.Host, Request.Url.Port, folder, fileName);

            if (isThumbnail)
            {
                var thumbnailDir = Path.Combine(folderDir, "thumbnail");
                if (!Directory.Exists(thumbnailDir))
                {
                    Directory.CreateDirectory(thumbnailDir);
                }

                var thumbnail = Path.Combine(thumbnailDir, fileName);

                Image image = Image.FromFile(uploadFile);
                Image thumb = image.GetThumbnailImage(size.Width, size.Height, () => false, IntPtr.Zero);
                thumb.Save(thumbnail);

                photoUrl = string.Format("{0}://{1}:{2}/Uploads/{3}/thumbnail/{4}", Request.Url.Scheme, Request.Url.Host, Request.Url.Port, folder, fileName);
            }

            return Json(new { result = true, url = photoUrl }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetVerify(string mobile, string type)
        {
            if (string.IsNullOrEmpty(mobile))
            {
                return Json(new { result = false, message = "mobile is required" }, JsonRequestBehavior.AllowGet);
            }
            //if(!Request.Headers.AllKeys.Contains("KG-Sms"))
            //{
            //    return Json(new { result = false, message = "header KG-Sms is required" }, JsonRequestBehavior.AllowGet);
            //}
            //var credential = ParseAuthHeader(Request.Headers["KG-Sms"]);
            //if (credential == null)
            //{
            //    return Json(new { result = false, message = "header KG-Sms value is required" }, JsonRequestBehavior.AllowGet);
            //}
            //if (credential[0] != mobile && credential[1] != type)
            //{
            //    return Json(new { result = false, message = "mobile is not correct" }, JsonRequestBehavior.AllowGet);
            //}

            var _mobile = Uof.IuserService.GetAll(u => u.mobile == mobile).Select(u => u.mobile).FirstOrDefault();

            if (string.IsNullOrEmpty(_mobile))
            {
                if (type == "retrieve")
                {
                    return Json(new { result = false, message = "您输入的手机号码还未注册" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                if (type == "register")
                {
                    return Json(new { result = false, message = "您输入的手机号码已被注册" }, JsonRequestBehavior.AllowGet);
                }
            }
            

            var code = getRandomCode();

            var apikey = "214fdac0f9d0b211a645636fe7375156";
            var text = string.Format("【今日开工】您的验证码为{0}", code);
            var url_send_sms = "https://sms.yunpian.com/v2/sms/single_send.json";
            string data_send_sms = "apikey=" + apikey + "&mobile=" + mobile + "&text=" + text;

            SmsPost(url_send_sms, data_send_sms);
            Cache.Remove(mobile);
            Cache.Add(mobile, string.Format("{0}", code));
            return base.SuccessResult;
        }


        private string[] ParseAuthHeader(string authHeader)
        {
            if (authHeader == null || authHeader.Length == 0 || !authHeader.StartsWith("KgApp"))
            {
                return null;
            }
            string base64Credentials = authHeader.Substring(6);
            string[] credentials = HttpUtility.UrlDecode(Encoding.ASCII.GetString(Convert.FromBase64String(base64Credentials))).Split(new char[] { ':' });

            if (credentials.Length != 2 || string.IsNullOrEmpty(credentials[0]) || string.IsNullOrEmpty(credentials[1]))
            {
                return null;
            }
            return credentials;
        }

        private int getRandomCode()
        {
            var rm = new Random();
            return rm.Next(1001, 9999);
        }

        private void SmsPost(string Url, string postDataStr)
        {
            byte[] dataArray = Encoding.UTF8.GetBytes(postDataStr);
            // Console.Write(Encoding.UTF8.GetString(dataArray));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = dataArray.Length;
            //request.CookieContainer = cookie;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(dataArray, 0, dataArray.Length);
            dataStream.Close();
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                String res = reader.ReadToEnd();
                reader.Close();
                Console.Write("\nResponse Content:\n" + res + "\n");
            }
            catch (Exception e)
            {
                Console.Write(e.Message + e.ToString());
            }
        }
    }
}
