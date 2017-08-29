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
using System.Reflection;

namespace WebCenter.Web.Controllers
{
    public class CommonController : BaseController
    {

        public CommonController(IUnitOfWork UOF)
            : base(UOF)
        {
        }

        private byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        [HttpPost]
        public ActionResult GetHexFloat(Hex hex)
        {
            var ox = StringToByteArray(hex.ox);
            var ph = StringToByteArray(hex.ph);
            var water = StringToByteArray(hex.water);

            float oxf = System.BitConverter.ToSingle(ox, 0);
            float phf = System.BitConverter.ToSingle(ph, 0);
            float waterf = System.BitConverter.ToSingle(water, 0);

            return Json(new {
                ox = oxf,
                ph = phf,
                water = waterf
            }, JsonRequestBehavior.AllowGet);
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
                        accounter = a.member5.name,     // 提交审核人
                        cashier = a.member1.name,       // 出纳 财务审核人
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
                        area = a.member.area.name, // a.member4.area.name,
                        rate = a.rate ?? 1,
                        region = a.region,
                        trader = a.customer1.name

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
                case "abroad_line":
                    #region 境外注册
                    var abroadLine = Uof.IincomeService.GetAll(l => l.id == id).FirstOrDefault();
                    printData = Uof.Ireg_abroadService.GetAll(a => a.id == abroadLine.source_id).Select(a => new PrintData
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
                        rate = a.rate ?? 1,
                        region = a.region
                    }).FirstOrDefault();

                    if (!string.IsNullOrEmpty(printData.region))
                    {
                        printData.others = string.Format("{0}  注册地区:{1}", printData.others, printData.region);
                    }

                    printData.date = abroadLine.date_pay != null ? abroadLine.date_pay.Value.ToString("yyyy年MM月dd日") : DateTime.Today.ToString("yyyy年MM月dd日");
                    printData.project = string.Format("{0}提交", printData.area);

                    getLinePrintDataIncome(printData, abroadLine, "reg_abroad");
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
                        rate = a.rate ?? 1
                    }).FirstOrDefault();

                    printData.date = printData.date_transaction != null ? printData.date_transaction.Value.ToString("yyyy年MM月dd日") : DateTime.Today.ToString("yyyy年MM月dd日");

                    printData.project = string.Format("{0}年报", printData.area);

                    getPrintDataIncome(printData, "annual");
                    #endregion
                    break;
                case "annual_line":
                    #region 年检
                    var annualLine = Uof.IincomeService.GetAll(l => l.id == id).FirstOrDefault();
                    printData = Uof.Iannual_examService.GetAll(a => a.id == annualLine.source_id).Select(a => new PrintData
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
                        others = a.description,
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
                        rate = a.rate ?? 1
                    }).FirstOrDefault();

                    printData.date = annualLine.date_pay != null ? annualLine.date_pay.Value.ToString("yyyy年MM月dd日") : DateTime.Today.ToString("yyyy年MM月dd日");

                    printData.project = string.Format("{0}年报", printData.area);
                    getLinePrintDataIncome(printData, annualLine, "annual");
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
                        rate = a.rate ?? 1,
                        trader = a.customer1.name,
                    }).FirstOrDefault();

                    printData.date = printData.date_transaction != null ? printData.date_transaction.Value.ToString("yyyy年MM月dd日") : DateTime.Today.ToString("yyyy年MM月dd日");
                    printData.project = string.Format("{0}审计", printData.area);

                    getPrintDataIncome(printData, "audit");
                    #endregion
                    break;
                case "audit_line":
                    #region 审计
                    var auditLine = Uof.IincomeService.GetAll(l => l.id == id).FirstOrDefault();
                    printData = Uof.IauditService.GetAll(a => a.id == auditLine.source_id).Select(a => new PrintData
                    {
                        print_type = "audit",
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
                        rate = a.rate ?? 1
                    }).FirstOrDefault();

                    printData.date = auditLine.date_pay != null ? auditLine.date_pay.Value.ToString("yyyy年MM月dd日") : DateTime.Today.ToString("yyyy年MM月dd日");
                    printData.project = string.Format("{0}审计", printData.area);
                    
                    getLinePrintDataIncome(printData, auditLine, "audit");
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
                        amount = a.amount_transaction,
                        attachments = 0,
                        auditor = a.member2.name,
                        balance = a.amount_transaction,
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
                        rate = a.rate ?? 1,
                        trader = a.customer1.name,
                    }).FirstOrDefault();

                    printData.date = printData.date_transaction!=null ? printData.date_transaction.Value.ToString("yyyy年MM月dd日") : DateTime.Today.ToString("yyyy年MM月dd日");
                    printData.project = string.Format("{0}提交", printData.area);

                    getPrintDataIncome(printData, "reg_internal");
                    #endregion
                    break;
                case "internal_line":
                    #region 国内注册
                    var internalLine = Uof.IincomeService.GetAll(l => l.id == id).FirstOrDefault();
                    printData = Uof.Ireg_internalService.GetAll(a => a.id == internalLine.source_id).Select(a => new PrintData
                    {
                        print_type = "internal",
                        id = a.id,
                        accounter = a.member6.name,
                        cashier = a.member2.name, // 出纳
                        amount = a.amount_transaction,
                        attachments = 0,
                        auditor = a.member2.name,
                        balance = a.amount_transaction,
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
                        rate = a.rate ?? 1
                    }).FirstOrDefault();

                    printData.date = internalLine.date_pay != null ? internalLine.date_pay.Value.ToString("yyyy年MM月dd日") : DateTime.Today.ToString("yyyy年MM月dd日");
                    printData.project = string.Format("{0}提交", printData.area);

                    getLinePrintDataIncome(printData, internalLine, "reg_internal");
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
                        amount = a.amount_transaction,
                        attachments = 0,
                        auditor = a.member2.name,
                        balance = a.amount_transaction,
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
                        rate = a.rate ?? 1,
                        trader = a.customer1.name,
                    }).FirstOrDefault();

                    printData.date = printData.date_transaction != null ? printData.date_transaction.Value.ToString("yyyy年MM月dd日") : DateTime.Today.ToString("yyyy年MM月dd日");
                    printData.project = string.Format("{0}提交", printData.area);

                    getPrintDataIncome(printData, "patent");
                    #endregion
                    break;
                case "patent_line":
                    #region 专利
                    var patentLine = Uof.IincomeService.GetAll(l => l.id == id).FirstOrDefault();
                    printData = Uof.IpatentService.GetAll(a => a.id == patentLine.source_id).Select(a => new PrintData
                    {
                        print_type = "patent",
                        id = a.id,
                        accounter = a.member5.name,
                        cashier = a.member2.name, // 出纳
                        amount = a.amount_transaction,
                        attachments = 0,
                        auditor = a.member2.name,
                        balance = a.amount_transaction,
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
                        rate = a.rate ?? 1
                    }).FirstOrDefault();

                    printData.date = patentLine.date_pay != null ? patentLine.date_pay.Value.ToString("yyyy年MM月dd日") : DateTime.Today.ToString("yyyy年MM月dd日");
                    printData.project = string.Format("{0}提交", printData.area);

                    getLinePrintDataIncome(printData, patentLine, "patent");
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
                        amount = a.amount_transaction,
                        attachments = 0,
                        auditor = a.member2.name,
                        balance = a.amount_transaction,
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
                        rate = a.rate ?? 1,
                        region = a.region,
                        trader = a.customer1.name,
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
                case "trademark_line":
                    #region 商标
                    var trademarkLine = Uof.IincomeService.GetAll(l => l.id == id).FirstOrDefault();
                    printData = Uof.ItrademarkService.GetAll(a => a.id == trademarkLine.source_id).Select(a => new PrintData
                    {
                        print_type = "trademark",
                        id = a.id,
                        accounter = a.member5.name,
                        cashier = a.member2.name, // 出纳
                        amount = a.amount_transaction,
                        attachments = 0,
                        auditor = a.member2.name,
                        balance = a.amount_transaction,
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
                        rate = a.rate ?? 1,
                        region = a.region
                    }).FirstOrDefault();

                    if (!string.IsNullOrEmpty(printData.region))
                    {
                        printData.others = string.Format("{0}  注册地区:{1}", printData.others, printData.region);
                    }

                    printData.date = trademarkLine.date_pay != null ? trademarkLine.date_pay.Value.ToString("yyyy年MM月dd日") : DateTime.Today.ToString("yyyy年MM月dd日");
                    printData.project = string.Format("{0}提交", printData.area);
                    getLinePrintDataIncome(printData, trademarkLine, "trademark");
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

                        amount = a.amount_transaction,
                        attachments = 0,
                        auditor = a.member1.name,
                        balance = a.amount_transaction,
                        code = a.order_code,
                        customer_name = "",
                        company_cn = "",
                        company_en = "",
                        creator = a.member.name,
                        date_transaction = a.date_transaction,
                        date = "",
                        mode = "",
                        ordername = "",
                        others = a.value ?? "",
                        payer = "",
                        pay_info = "",
                        pay_way = "",
                        project = "",
                        reason = "注册变更",
                        received = 0,
                        saleman = a.member2.name,
                        type = "",
                        currency = a.currency,
                        area = a.member3.area.name,
                        rate = a.rate ?? 1,
                        logoff = a.logoff,
                        logoff_memo = a.logoff_memo,

                    }).FirstOrDefault();

                    if (printData.logoff == 1)
                    {
                        printData.others = "{\"others\":\"注销\"}";
                    }
                    if (printData.logoff == 1 && !string.IsNullOrEmpty(printData.logoff_memo))
                    {
                        var str = string.Format("注销, {0}", printData.logoff_memo);
                        printData.others = "{\"others\":\""+ str + "\"}";
                    }

                    printData.date = printData.date_transaction != null ? printData.date_transaction.Value.ToString("yyyy年MM月dd日") : DateTime.Today.ToString("yyyy年MM月dd日");
                    printData.project = string.Format("{0}其他", printData.area);

                    getCompanyName(printData);

                    getPrintDataIncome(printData, "history");
                    #endregion
                    break;
                case "history_line":
                    #region 变更
                    var historyLine = Uof.IincomeService.GetAll(l => l.id == id).FirstOrDefault();

                    printData = Uof.IhistoryService.GetAll(a => a.id == historyLine.source_id).Select(a => new PrintData
                    {
                        print_type = "history",
                        id = a.id,
                        accounter = a.member3.name,
                        cashier = a.member1.name, // 出纳
                        source = a.source,
                        source_id = a.source_id,

                        amount = a.amount_transaction,
                        attachments = 0,
                        auditor = a.member1.name,
                        balance = a.amount_transaction,
                        code = a.order_code,
                        customer_name = "",
                        company_cn = "",
                        company_en = "",
                        creator = a.member.name,
                        date_transaction = a.date_transaction,
                        date = "",
                        mode = "",
                        ordername = "",
                        others = a.value ?? "",
                        payer = "",
                        pay_info = "",
                        pay_way = "",
                        project = "",
                        reason = "注册变更",
                        received = 0,
                        saleman = a.member2.name,
                        type = "",
                        currency = a.currency,
                        area = a.member3.area.name,
                        rate = a.rate ?? 1
                    }).FirstOrDefault();



                    printData.date = historyLine.date_pay != null ? historyLine.date_pay.Value.ToString("yyyy年MM月dd日") : DateTime.Today.ToString("yyyy年MM月dd日");
                    printData.project = string.Format("{0}其他", printData.area);

                    getCompanyName(printData);

                    getPrintDataIncome(printData, "history");
                    getLinePrintDataIncome(printData, historyLine, "history");
                    #endregion
                    break;
                case "sub_audit":
                    #region 审计
                    printData = Uof.Isub_auditService.GetAll(a => a.id == id).Select(a => new PrintData
                    {
                        print_type = "sub_audit",
                        id = a.id,
                        accounter = a.member5.name,
                        cashier = a.member2.name, // 出纳
                        amount = a.amount_transaction,
                        attachments = 0,
                        auditor = a.member2.name,
                        balance = a.amount_transaction,
                        code = a.audit.code,
                        customer_name = a.customer.name,
                        company_cn = a.audit.name_cn,
                        company_en = a.audit.name_en,
                        creator = a.member1.name,
                        date_transaction = a.date_transaction,
                        date = "",
                        mode = "",
                        ordername = a.audit.name_cn,
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
                        rate = a.rate ?? 1,
                        trader = a.customer1.name,
                    }).FirstOrDefault();

                    printData.date = printData.date_transaction != null ? printData.date_transaction.Value.ToString("yyyy年MM月dd日") : DateTime.Today.ToString("yyyy年MM月dd日");
                    printData.project = string.Format("{0}审计", printData.area);

                    getPrintDataIncome(printData, "sub_audit");
                    #endregion
                    break;
                case "sub_audit_line":
                    #region 审计
                    var subAuditLineLine = Uof.IincomeService.GetAll(l => l.id == id).FirstOrDefault();
                    printData = Uof.Isub_auditService.GetAll(a => a.id == subAuditLineLine.source_id).Select(a => new PrintData
                    {
                        print_type = "sub_audit",
                        id = a.id,
                        accounter = a.member5.name,
                        cashier = a.member2.name, // 出纳
                        amount = a.amount_transaction,
                        attachments = 0,
                        auditor = a.member2.name,
                        balance = a.amount_transaction,
                        code = a.audit.code,
                        customer_name = a.customer.name,
                        company_cn = a.audit.name_cn,
                        company_en = a.audit.name_en,
                        creator = a.member1.name,
                        date_transaction = a.date_transaction,
                        date = "",
                        mode = "",
                        ordername = a.audit.name_cn,
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
                        rate = a.rate ?? 1
                    }).FirstOrDefault();

                    printData.date = subAuditLineLine.date_pay != null ? subAuditLineLine.date_pay.Value.ToString("yyyy年MM月dd日") : DateTime.Today.ToString("yyyy年MM月dd日");
                    printData.project = string.Format("{0}审计", printData.area);

                    //getPrintDataIncome(printData, "sub_audit");
                    getLinePrintDataIncome(printData, subAuditLineLine, "sub_audit");
                    #endregion
                    break;

                case "accounting":
                    #region 记账
                    printData = Uof.Iaccounting_itemService.GetAll(a => a.id == id).Select(a => new PrintData
                    {
                        print_type = "accounting",
                        id = a.id,
                        masterId = a.master_id,
                        accounter = a.member.name,
                        cashier = "", //a.member2.name, // 出纳
                        amount = a.amount_transaction,
                        attachments = 0,
                        auditor = "", //a.member2.name,
                        balance = a.amount_transaction,
                        code = "",
                        customer_name = "",
                        company_cn = "", //a.applicant,
                        company_en = "",
                        creator = a.member2.name,
                        date_transaction = a.date_transaction,
                        date = "",
                        mode = "", //a.reg_mode,
                        ordername = "",
                        others = "", //a.description,
                        payer = "",
                        pay_info = "",
                        pay_way = "",
                        project = "",
                        reason = "代理记账",
                        received = 0,
                        saleman = a.member5.name,
                        type = "", //a.trademark_type,
                        currency = a.currency,
                        area = a.member5.area.name,
                        rate = 1,
                        region = "", //a.region
                        trader = a.customer.name,                        
                    }).FirstOrDefault();

                    //if (!string.IsNullOrEmpty(printData.region))
                    //{
                    //    printData.others = string.Format("{0}  注册地区:{1}", printData.others, printData.region);
                    //}
                    var acc = Uof.IaccountingService.GetAll(a => a.id == printData.masterId).FirstOrDefault();

                    printData.code = acc.code;
                    printData.customer_name = acc.customer.name;
                    printData.company_cn = acc.name;
                    printData.ordername = acc.name;

                    printData.date = printData.date_transaction != null ? printData.date_transaction.Value.ToString("yyyy年MM月dd日") : DateTime.Today.ToString("yyyy年MM月dd日");
                    printData.project = string.Format("{0}提交", printData.area);

                    getPrintDataIncome(printData, "accounting_item");
                    #endregion
                    break;
                case "account_line":
                    #region 记账
                    var accountLine = Uof.IincomeService.GetAll(l => l.id == id).FirstOrDefault();
                    //
                    printData = Uof.Iaccounting_itemService.GetAll(a => a.id == accountLine.source_id).Select(a => new PrintData
                    {
                        print_type = "accounting",
                        id = a.id,
                        masterId = a.master_id,
                        accounter = a.member.name,
                        cashier = "", //a.member2.name, // 出纳
                        amount = a.amount_transaction,
                        attachments = 0,
                        auditor = "", //a.member2.name,
                        balance = a.amount_transaction,
                        code = "",
                        customer_name = "",
                        company_cn = "", //a.applicant,
                        company_en = "",
                        creator = a.member1.name,
                        date_transaction = a.date_transaction,
                        date = "",
                        mode = "", //a.reg_mode,
                        ordername = "",
                        others = "", //a.description,
                        payer = "",
                        pay_info = "",
                        pay_way = "",
                        project = "",
                        reason = "代理记账",
                        received = 0,
                        saleman = a.member5.name,
                        type = "", //a.trademark_type,
                        currency = a.currency,
                        area = a.member5.area.name,
                        rate = 1,
                        region = "", //a.region
                    }).FirstOrDefault();

                    var acct = Uof.IaccountingService.GetAll(a => a.id == printData.masterId).FirstOrDefault();
                    printData.code = acct.code;
                    printData.customer_name = acct.customer.name;
                    printData.company_cn = acct.name;
                    printData.ordername = acct.name;

                    printData.date = accountLine.date_pay != null ? accountLine.date_pay.Value.ToString("yyyy年MM月dd日") : DateTime.Today.ToString("yyyy年MM月dd日");
                    printData.project = string.Format("{0}记账", printData.area);

                    //getPrintDataIncome(printData, "sub_audit");
                    getLinePrintDataIncome(printData, accountLine, "accounting_item");
                    #endregion
                    break;
                default:
                    break;
            }

            printData.amount = (float)Math.Round((double)(printData.amount * (printData.rate ?? 1)), 2);
            printData.balance = (float)Math.Round((double)(printData.balance * (printData.rate ?? 1)), 2);
            return Json(printData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExistCode(string code, string id)
        {
            var existCode = "";
            switch (id)
            {
                case "reg_abroad":
                    existCode = Uof.Ireg_abroadService.GetAll(a => a.code == code).Select(a => a.code).FirstOrDefault();
                    break;
                case "reg_internal":
                    existCode = Uof.Ireg_internalService.GetAll(a => a.code == code).Select(a => a.code).FirstOrDefault();
                    break;
                case "trademark":
                    existCode = Uof.ItrademarkService.GetAll(a => a.code == code).Select(a => a.code).FirstOrDefault();
                    break;
                case "patent":
                    existCode = Uof.IpatentService.GetAll(a => a.code == code).Select(a => a.code).FirstOrDefault();
                    break;
                case "audit":
                    existCode = Uof.IauditService.GetAll(a => a.code == code).Select(a => a.code).FirstOrDefault();
                    break;
                case "account":
                    existCode = Uof.IaccountingService.GetAll(a => a.code == code).Select(a => a.code).FirstOrDefault();
                    break;
                default:
                    break;
            }

            if (string.IsNullOrEmpty(existCode))
            {
                return Json(new { ok = "验证成功" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { error = "档案号已存在" }, JsonRequestBehavior.AllowGet);
        }

        public FileStreamResult ExportExcel(string tableName)
        {
            var r = HttpContext.User.Identity.IsAuthenticated;
            if (!r)
            {
                throw new Exception("您没登录");
            }

            var identityName = HttpContext.User.Identity.Name;
            var arrs = identityName.Split('|');
            if (arrs.Length == 0)
            {
                throw new Exception("您没登录");
            }

            var userId = 0;
            int.TryParse(arrs[0], out userId);

            switch (tableName)
            {
                case "reg_abroad":
                    var abroads = Uof.Ireg_abroadService.GetAll(a => a.salesman_id == userId || a.assistant_id == userId || userId == 1).OrderBy(m => m.code).Select(a => new RegAbroad()
                    {
                        ID = a.id,
                        业务员 = a.member4.name,
                        交易币别 = a.currency,
                        公司中文名称 = a.name_cn,
                        公司英文名称 = a.name_en,
                        //公司股东 = a.shareholder,
                        //公司董事 = a.director,
                        其他事项 = a.description,
                        助理 = a.member7.name,
                        客户ID = a.customer_id,
                        客户名称 = a.customer.name,
                        年检客服 = a.member6.name,
                        //成交日期 = a.date_transaction,
                        成交金额 = a.amount_transaction,
                        成立日期 = a.date_setup,
                        是否开户 = a.is_open_bank,
                        档案号 = a.code,
                        汇率 = a.rate.ToString(),
                        注册地区 = a.region,
                        注册地址 = a.address,
                        注册编号 = a.reg_no,
                        订单归属人ID = a.creator_id,
                        订单归属人 = a.member.name,
                        客户归属ID = a.salesman_id,
                        客户归属 = a.member4.name,
                    }).ToList();

                    var sheet = ExportToExcel(abroads);
                    var fileName = "境外注册.xml";
                    var bytes = GenerateStreamFromString(sheet);
                    return File(bytes, "application/xml", fileName);
            }

            throw new Exception("您没登录");
        }

        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
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
                bank = i.bank,
                currency = i.currency,
                rate = i.rate ?? 1,
            }).OrderByDescending(i => i.id).ToList();

            var total = 0f;
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    total += item.amount.Value * item.rate;
                }
                if (pd.rate == null)
                {
                    pd.rate = 1;
                }
                if (pd.amount == null)
                {
                    pd.amount = 0;
                }
                //pd.received = (float)Math.Round((double)(total * pd.rate), 2);
                //pd.balance = (float)Math.Round((double)(pd.amount * pd.rate - total * pd.rate), 2);

                pd.received = (float)Math.Round((double)(total), 2);
                pd.balance = (float)Math.Round((double)(pd.amount * pd.rate - total), 2);
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

        private void getLinePrintDataIncome(PrintData pd, income lineIncome, string source_name)
        {
            var list = Uof.IincomeService.GetAll(i => i.source_id == pd.id && i.source_name == source_name && i.id <= lineIncome.id).Select(i => new {
                id = i.id,
                payer = i.payer,
                pay_way = i.pay_way,
                account = i.account,
                amount = i.amount,
                date_pay = i.date_pay,
                attachment_url = i.attachment_url,
                description = i.description,
                bank = i.bank,
                currency = i.currency,
                rate = i.rate ?? 1,
            }).OrderByDescending(i => i.id).ToList();

            var total = 0f;
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    total += item.amount.Value * item.rate;
                }

                if (pd.rate == null)
                {
                    pd.rate = 1;
                }
                if (pd.amount == null)
                {
                    pd.amount = 0;
                }

                if (lineIncome.rate == null)
                {
                    lineIncome.rate = 1;
                }
                //pd.received = (float)Math.Round((double)(lineIncome.amount * pd.rate), 2); // (float)Math.Round((double)(total * pd.rate), 2);
                //pd.balance = (float)Math.Round((double)(pd.amount * pd.rate - total * pd.rate), 2);

                pd.received = (float)Math.Round((double)(lineIncome.amount * lineIncome.rate), 2); // (float)Math.Round((double)(total * pd.rate), 2);
                pd.balance = (float)Math.Round((double)(pd.amount * pd.rate - total), 2);
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
            else
            {
                pd.pay_way = "先提交,未付款";
                pd.received = 0;
            }
        }
    }
}
