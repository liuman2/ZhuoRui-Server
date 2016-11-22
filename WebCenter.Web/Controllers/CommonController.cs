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
            var _fileName = files[0].FileName;

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
                fileName = _name + "." + fileArr[fileArr.Length - 1];
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

            return Json(new { result = true, url = photoUrl, name = _fileName.Length > 20 ? _fileName.Substring(0, 20) : _fileName }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetPrintData(int id, string name)
        {
            var printData = new PrintData();

            switch (name)
            {
                
                case "abroad":
                    #region 境外注册
                    printData = Uof.Ireg_abroadService.GetAll(a => a.id == id).Select(a => new PrintData
                    {
                        print_type = "abroad",
                        id = a.id,
                        accounter = a.member5.name, // 会计
                        cashier = a.member1.name, // 出纳
                        amount = a.amount_transaction,
                        balance = a.amount_transaction,
                        attachments = 0,
                        auditor = a.member1.name,
                        code = a.code,
                        customer_name = a.customer.name,
                        company_cn = a.name_cn,
                        company_en = a.name_en,
                        creator = a.member.name,
                        date_transaction = a.date_transaction,
                        date = "",
                        mode = "",
                        ordername = a.name_en,
                        others = a.description,
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
                        rate = a.rate,
                        region = a.region
                    }).FirstOrDefault();

                    if (!string.IsNullOrEmpty(printData.region))
                    {
                        printData.others = string.Format("{0}  注册地区:{1}", printData.others, printData.region);
                    }
                    
                    printData.date = printData.date_transaction != null ? printData.date_transaction.Value.ToString("yyyy年MM月dd日") : DateTime.Today.ToString("yyyy年MM月dd日");
                    printData.project = string.Format("{0}提交", printData.area);

                    getPrintDataIncome(printData, "reg_abroad");
                    #endregion
                    break;
                case "annual":
                    #region 年检
                    printData = Uof.Iannual_examService.GetAll(a => a.id == id).Select(a => new PrintData
                    {
                        print_type = "annual",
                        id = a.id,
                        accounter = a.member5.name,
                        cashier = a.member2.name, // 出纳
                        amount = a.amount_transaction,
                        attachments = 0,
                        auditor = a.member2.name,
                        balance = a.amount_transaction,
                        code = a.code,
                        customer_name = a.customer.name,
                        company_cn = a.name_cn,
                        company_en = a.name_en,
                        creator = a.member1.name,
                        date_transaction = a.date_transaction,
                        date = "",
                        mode = "",
                        ordername = a.name_cn,
                        others =  a.description,
                        payer = "",
                        pay_info = "",
                        pay_way = "",
                        project = "",
                        reason = "年检",
                        received = 0,
                        saleman = a.member4.name,
                        type = "",
                        currency = a.currency,
                        area = a.member4.area.name,
                        rate = a.rate
                    }).FirstOrDefault();

                    printData.date = printData.date_transaction != null ? printData.date_transaction.Value.ToString("yyyy年MM月dd日") : DateTime.Today.ToString("yyyy年MM月dd日");

                    printData.project = string.Format("{0}年报", printData.area);

                    getPrintDataIncome(printData, "annual");
                    #endregion
                    break;
                case "audit":
                    #region 审计
                    printData = Uof.IauditService.GetAll(a => a.id == id).Select(a => new PrintData
                    {
                        print_type = "audit",
                        id = a.id,
                        accounter = a.member5.name,
                        cashier = a.member2.name, // 出纳
                        amount = a.amount_transaction * a.rate,
                        attachments = 0,
                        auditor = a.member2.name,
                        balance = a.amount_transaction * a.rate,
                        code = a.code,
                        customer_name = a.customer.name,
                        company_cn = a.name_cn,
                        company_en = a.name_en,
                        creator = a.member1.name,
                        date_transaction = a.date_transaction,
                        date = "",
                        mode = "",
                        ordername = a.name_cn,
                        others = a.description,
                        payer = "",
                        pay_info = "",
                        pay_way = "",
                        project = "",
                        reason = "审计",
                        received = 0,
                        saleman = a.member4.name,
                        type = "",
                        currency = a.currency,
                        area = a.member4.area.name,
                        rate = a.rate
                    }).FirstOrDefault();

                    printData.date = printData.date_transaction != null ? printData.date_transaction.Value.ToString("yyyy年MM月dd日") : DateTime.Today.ToString("yyyy年MM月dd日");
                    printData.project = string.Format("{0}审计", printData.area);

                    getPrintDataIncome(printData, "audit");
                    #endregion
                    break;
                case "internal":
                    #region 国内注册
                    printData = Uof.Ireg_internalService.GetAll(a => a.id == id).Select(a => new PrintData
                    {
                        print_type = "internal",
                        id = a.id,
                        accounter = a.member6.name,
                        cashier = a.member2.name, // 出纳
                        amount = a.amount_transaction * a.rate,
                        attachments = 0,
                        auditor = a.member2.name,
                        balance = a.amount_transaction * a.rate,
                        code = a.code,
                        customer_name = a.customer.name,
                        company_cn = a.name_cn,
                        company_en = "",
                        creator = a.member1.name,
                        date_transaction = a.date_transaction,
                        date = "",
                        mode = "",
                        ordername = a.name_cn,
                        others = a.description,
                        payer = "",
                        pay_info = "",
                        pay_way = "",
                        project = "",
                        reason = "国内注册",
                        received = 0,
                        saleman = a.member5.name,
                        type = "",
                        currency = a.currency,
                        area = a.member5.area.name,
                        rate = a.rate
                    }).FirstOrDefault();

                    printData.date = printData.date_transaction!=null ? printData.date_transaction.Value.ToString("yyyy年MM月dd日") : DateTime.Today.ToString("yyyy年MM月dd日");
                    printData.project = string.Format("{0}提交", printData.area);

                    getPrintDataIncome(printData, "reg_internal");
                    #endregion
                    break;
                case "patent":
                    #region 专利
                    printData = Uof.IpatentService.GetAll(a => a.id == id).Select(a => new PrintData
                    {
                        print_type = "patent",
                        id = a.id,
                        accounter = a.member5.name,
                        cashier = a.member2.name, // 出纳
                        amount = a.amount_transaction * a.rate,
                        attachments = 0,
                        auditor = a.member2.name,
                        balance = a.amount_transaction * a.rate,
                        code = a.code,
                        customer_name = a.customer.name,
                        company_cn = a.applicant,
                        company_en = "",
                        creator = a.member1.name,
                        date_transaction = a.date_transaction,
                        date = "",
                        mode = a.reg_mode,
                        ordername = a.name,
                        others = a.description,
                        payer = "",
                        pay_info = "",
                        pay_way = "",
                        project = "",
                        reason = "专利注册",
                        received = 0,
                        saleman = a.member4.name,
                        type = a.patent_type,
                        currency = a.currency,
                        area = a.member4.area.name,
                        rate = a.rate
                    }).FirstOrDefault();

                    printData.date = printData.date_transaction != null ? printData.date_transaction.Value.ToString("yyyy年MM月dd日") : DateTime.Today.ToString("yyyy年MM月dd日");
                    printData.project = string.Format("{0}提交", printData.area);

                    getPrintDataIncome(printData, "patent");
                    #endregion
                    break;
                case "trademark":
                    #region 商标
                    printData = Uof.ItrademarkService.GetAll(a => a.id == id).Select(a => new PrintData
                    {
                        print_type = "trademark",
                        id = a.id,
                        accounter = a.member5.name,
                        cashier = a.member2.name, // 出纳
                        amount = a.amount_transaction * a.rate,
                        attachments = 0,
                        auditor = a.member2.name,
                        balance = a.amount_transaction * a.rate,
                        code = a.code,
                        customer_name = a.customer.name,
                        company_cn = a.applicant,
                        company_en = "",
                        creator = a.member1.name,
                        date_transaction = a.date_transaction,
                        date = "",
                        mode = a.reg_mode,
                        ordername = a.name,
                        others = a.description,
                        payer = "",
                        pay_info = "",
                        pay_way = "",
                        project = "",
                        reason = "商标注册",
                        received = 0,
                        saleman = a.member4.name,
                        type = a.trademark_type,
                        currency = a.currency,
                        area = a.member4.area.name,
                        rate = a.rate,
                        region = a.region
                    }).FirstOrDefault();

                    if (!string.IsNullOrEmpty(printData.region))
                    {
                        printData.others = string.Format("{0}  注册地区:{1}", printData.others, printData.region);
                    }

                    printData.date = printData.date_transaction != null ? printData.date_transaction.Value.ToString("yyyy年MM月dd日") : DateTime.Today.ToString("yyyy年MM月dd日");
                    printData.project = string.Format("{0}提交", printData.area);

                    getPrintDataIncome(printData, "trademark");
                    #endregion
                    break;
                case "history":
                    #region 变更
                    printData = Uof.IhistoryService.GetAll(a => a.id == id).Select(a => new PrintData
                    {
                        print_type = "history",
                        id = a.id,
                        accounter = a.member3.name,
                        cashier = a.member1.name, // 出纳
                        source = a.source,
                        source_id = a.source_id,

                        amount = a.amount_transaction * a.rate,
                        attachments = 0,
                        auditor = a.member1.name,
                        balance = a.amount_transaction * a.rate,
                        code = a.order_code,
                        customer_name = "",
                        company_cn = "",
                        company_en = "",
                        creator = a.member.name,
                        date_transaction = a.date_transaction,
                        date = "",
                        mode = "",
                        ordername = "",
                        others = "",
                        payer = "",
                        pay_info = "",
                        pay_way = "",
                        project = "",
                        reason = "注册变更",
                        received = 0,
                        saleman = a.member3.name,
                        type = "",
                        currency = a.currency,
                        area = a.member3.area.name,
                        rate = a.rate
                    }).FirstOrDefault();

                    printData.date = printData.date_transaction != null ? printData.date_transaction.Value.ToString("yyyy年MM月dd日") : DateTime.Today.ToString("yyyy年MM月dd日");
                    printData.project = string.Format("{0}提交", printData.area);

                    getCompanyName(printData);

                    getPrintDataIncome(printData, "history");
                    #endregion

                    break;
                default:
                    break;
            }

            return Json(printData, JsonRequestBehavior.AllowGet);
        }

        private void getCompanyName(PrintData printData)
        {
            switch (printData.source)
            {
                case "reg_abroad":
                    var abroad = Uof.Ireg_abroadService.GetAll(a => a.id == printData.source_id).Select(a=> new
                    {
                        id = a.id,
                        name_cn = a.name_cn,
                        name_en = a.name_en,

                    }).FirstOrDefault();
                    if (abroad != null)
                    {
                        printData.company_cn = abroad.name_cn;
                        printData.company_en = abroad.name_en;
                    }
                    break;
                case "reg_internal":
                    var inter = Uof.Ireg_internalService.GetAll(a => a.id == printData.source_id).Select(a => new
                    {
                        id = a.id,
                        name_cn = a.name_cn,
                        name_en = "",

                    }).FirstOrDefault();
                    if (inter != null)
                    {
                        printData.company_cn = inter.name_cn;
                        printData.company_en = inter.name_en;
                    }
                    break;
                case "patent":
                    var paten = Uof.IpatentService.GetAll(a => a.id == printData.source_id).Select(a => new
                    {
                        id = a.id,
                        name_cn = a.applicant,
                        name_en = "",

                    }).FirstOrDefault();
                    if (paten != null)
                    {
                        printData.company_cn = paten.name_cn;
                        printData.company_en = paten.name_en;
                    }
                    break;
                case "trademark":
                    var trademar = Uof.ItrademarkService.GetAll(a => a.id == printData.source_id).Select(a => new
                    {
                        id = a.id,
                        name_cn = a.applicant,
                        name_en = "",

                    }).FirstOrDefault();
                    if (trademar != null)
                    {
                        printData.company_cn = trademar.name_cn;
                        printData.company_en = trademar.name_en;
                    }
                    break;
                default:
                    break;
            }
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

                pd.received = total * pd.rate;
                pd.balance = pd.amount - total * pd.rate;
                pd.payer = list[0].payer ?? "";

                var acc = list[0].account ?? "";
                if (list[0].account.Length > 4)
                {
                    acc = list[0].account.Substring(list[0].account.Length - 4, 4);
                }
                pd.pay_info = string.Format("{0} {1}", list[0].bank, acc);
                pd.pay_way = list[0].pay_way;

                pd.attachments = list.Where(l => !string.IsNullOrEmpty(l.attachment_url)).Count();

            } else
            {
                pd.pay_way = "先提交,未付款";
                pd.received = 0;
            }
        }
    }
}
