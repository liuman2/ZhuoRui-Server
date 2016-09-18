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
            var docType = Request.Params["DocType"];
            var folder = "image";
            var isThumbnail = false;
            var size = new Size(80, 80);
            if (!string.IsNullOrEmpty(docType))
            {
                switch(docType.ToLower())
                {
                    case "profile":
                        folder = "photo";
                        isThumbnail = true;
                        break;
                    case "image":
                        folder = "image";
                        isThumbnail = false;
                        break;
                    default:
                        folder = "doc";
                        isThumbnail = false;
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


        public ActionResult GetPrintData(int id, string name)
        {
            var printData = new PrintData();

            switch (name)
            {
                case "abroad":
                    printData = Uof.Ireg_abroadService.GetAll(a => a.id == id).Select(a => new PrintData
                    {
                        id = a.id,
                        accounter = "",
                        amount = a.amount_transaction,
                        attachments = 0,
                        auditor = a.member1.name,
                        balance = null,
                        code = a.code,
                        company = a.customer.name,
                        creator = a.member.name,
                        date_transaction = a.date_transaction,
                        date = "",
                        mode = "",
                        ordername = a.name_en,
                        others = "",
                        payer = "",
                        pay_info = "",
                        pay_way = "",
                        project = "",
                        reason = "境外注册",
                        received = 0,
                        saleman = a.member4.name,
                        type = "",
                        currency = a.currency,
                        area = a.member4.area.name,
                        rate = a.rate
                    }).FirstOrDefault();

                    printData.date = printData.date_transaction.Value.ToString("yyyy年MM月dd日");
                    printData.project = string.Format("{0}提交", printData.area);

                    getPrintDataIncome(printData, "reg_abroad");
                    break;
                default:
                    break;
            }

            return Json(printData, JsonRequestBehavior.AllowGet);
        }

        private void getPrintDataIncome(PrintData pd, string source_name)
        {
            var list = Uof.IincomeService.GetAll(i => i.source_id == pd.id && i.source_name == source_name).Select(i => new {
                id = i.id,
                payer = i.payer,
                pay_way = i.pay_way,
                account = i.account,
                amount = i.amount,
                date_pay = i.date_pay,
                attachment_url = i.attachment_url,
                description = i.description,
                bank = i.bank
            }).OrderByDescending(i => i.id).ToList();

            var total = 0f;
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    total += item.amount.Value;
                }

                pd.received = total;
                pd.balance = pd.amount - total;
                pd.payer = list[0].payer ?? "";

                var acc = list[0].account ?? "";
                if (list[0].account.Length > 4)
                {
                    acc = list[0].account.Substring(list[0].account.Length - 4, 4);
                }
                pd.pay_info = string.Format("{0} {1}", list[0].bank, acc);
                pd.pay_way = list[0].pay_way;

                pd.attachments = list.Where(l => !string.IsNullOrEmpty(l.attachment_url)).Count();

            }
        }
    }
}
