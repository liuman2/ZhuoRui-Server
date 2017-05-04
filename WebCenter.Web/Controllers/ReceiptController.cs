using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;

namespace WebCenter.Web.Controllers
{
    public class ReceiptController : BaseController
    {
        public ReceiptController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        public ActionResult Add(receipt _receipt)
        {
            var r = HttpContext.User.Identity.IsAuthenticated;
            if (!r)
            {
                return new HttpUnauthorizedResult();
            }

            var identityName = HttpContext.User.Identity.Name;
            var arrs = identityName.Split('|');
            if (arrs.Length == 0)
            {
                return new HttpUnauthorizedResult();
            }

            var userId = 0;
            int.TryParse(arrs[0], out userId);

            _receipt.date_created = DateTime.Today;
            _receipt.creator_id = GetAuditorByKey("SJ_ID"); // userId;
            var code = GetCodeByDate(DateTime.Today);
            _receipt.code = code;

            var dbReceipt = Uof.IreceiptService.AddEntity(_receipt);
            if (dbReceipt == null)
            {
                return Json(new { success = false, message = "保存失败" }, JsonRequestBehavior.AllowGet);
            }

            return SuccessResult;
        }

        public ActionResult Update(receipt _receipt)
        {
            var dbReceipt = Uof.IreceiptService.GetAll(i => i.id == _receipt.id).FirstOrDefault();

            if (dbReceipt.memo == _receipt.memo)
            {
                return SuccessResult;
            }
            var identityName = HttpContext.User.Identity.Name;
            var arrs = identityName.Split('|');
            if (arrs.Length == 0)
            {
                return new HttpUnauthorizedResult();
            }
            dbReceipt.memo = _receipt.memo;
            var r = Uof.IreceiptService.UpdateEntity(dbReceipt);
            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get(int? order_id, string name)
        {
            var dbReceipt = Uof.IreceiptService.GetAll(r => r.order_source == name && r.order_id == order_id).Select(r=> new
            {
                id = r.id,
                order_source = r.order_source,
                order_id = r.order_id,
                memo = r.memo
            }).FirstOrDefault();
            return Json(dbReceipt, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintReceiptData(int order_id, string name)
        {
            var printData = new ReceiptPrintData();            
            switch (name)
            {
                case "reg_abroad":
                    #region 境外注册
                    printData = Uof.Ireg_abroadService.GetAll(a => a.id == order_id).Select(a => new ReceiptPrintData
                    {
                        print_type = "abroad",
                        id = a.id,
                        customer_name = a.name_cn ?? a.name_en,
                        saleman = a.member4.english_name,
                        finance_reviewer = a.member1.english_name,
                        rate = a.rate ?? 1,
                    }).FirstOrDefault();
                                        
                    GetPrintDataIncome(printData, "reg_abroad");
                    #endregion
                    break;
                case "annual":
                    #region 年检                    
                    printData = Uof.Iannual_examService.GetAll(a => a.id == order_id).Select(a => new ReceiptPrintData
                    {
                        print_type = "annual",
                        id = a.id,
                        customer_name = a.name_cn ?? a.name_en,
                        saleman = a.member4.english_name,
                        finance_reviewer = a.member2.english_name,
                        rate = a.rate ?? 1,
                    }).FirstOrDefault();
                    
                    GetPrintDataIncome(printData, "annual");
                    #endregion
                    break;
                case "audit":
                    #region 审计                   
                    printData = Uof.IauditService.GetAll(a => a.id == order_id).Select(a => new ReceiptPrintData
                    {
                        print_type = "audit",
                        id = a.id,
                        customer_name = a.name_cn ?? a.name_en,
                        saleman = a.member4.english_name,
                        finance_reviewer = a.member2.english_name,
                        rate = a.rate ?? 1,
                    }).FirstOrDefault();
                    
                    GetPrintDataIncome(printData, "audit");
                    #endregion
                    break;
                case "internal":
                    #region 国内注册
                    printData = Uof.Ireg_internalService.GetAll(a => a.id == order_id).Select(a => new ReceiptPrintData
                    {
                        print_type = "internal",
                        id = a.id,
                        customer_name = a.name_cn,
                        saleman = a.member5.english_name,
                        finance_reviewer = a.member2.english_name,
                        rate = a.rate ?? 1,
                    }).FirstOrDefault();
                    
                    GetPrintDataIncome(printData, "internal");
                    #endregion
                    break;
                case "patent":
                    #region 专利
                    printData = Uof.IpatentService.GetAll(a => a.id == order_id).Select(a => new ReceiptPrintData
                    {
                        print_type = "patent",
                        id = a.id,
                        customer_name = a.customer.name,
                        saleman = a.member4.english_name,
                        finance_reviewer = a.member2.english_name,
                        rate = a.rate ?? 1,
                    }).FirstOrDefault();
                    
                    GetPrintDataIncome(printData, "patent");
                    #endregion
                    break;
                case "trademark":
                    #region 商标
                    printData = Uof.ItrademarkService.GetAll(a => a.id == order_id).Select(a => new ReceiptPrintData
                    {
                        print_type = "trademark",
                        id = a.id,
                        customer_name = a.customer.name,
                        saleman = a.member4.english_name,
                        finance_reviewer = a.member2.english_name,
                        rate = a.rate ?? 1,
                    }).FirstOrDefault();
                    
                    GetPrintDataIncome(printData, "trademark");
                    #endregion
                    break;
                case "history":
                    #region 变更
                    var dbHistory = Uof.IhistoryService.GetAll(a => a.id == order_id).FirstOrDefault();
                    
                    printData.print_type = "history";
                    printData.id = dbHistory.id;
                    printData.customer_name = "";
                    printData.saleman = dbHistory.member2.english_name;
                    printData.finance_reviewer = dbHistory.member1.english_name;
                    printData.rate = dbHistory.rate ?? 1;
                    
                    GetHistorySource(dbHistory, printData);

                    GetPrintDataIncome(printData, "history");
                    #endregion
                    break;
                case "sub_audit":
                    #region 审计账期
                    printData = Uof.Isub_auditService.GetAll(a => a.id == order_id).Select(a => new ReceiptPrintData
                    {
                        print_type = "sub_audit",
                        id = a.id,
                        customer_name = a.audit.name_cn ?? a.audit.name_en,
                        saleman = a.member4.english_name,
                        finance_reviewer = a.member2.english_name,
                        rate = a.rate ?? 1,
                    }).FirstOrDefault();

                    GetPrintDataIncome(printData, "sub_audit");
                    #endregion
                    break;
                case "accounting":
                    #region 境外注册
                    printData = Uof.IaccountingService.GetAll(a => a.id == order_id).Select(a => new ReceiptPrintData
                    {
                        print_type = "accounting",
                        id = a.id,
                        customer_name = a.name,
                        saleman = a.member5.english_name,
                        finance_reviewer = a.member2.english_name,
                        rate = a.rate ?? 1,
                    }).FirstOrDefault();

                    GetPrintDataIncome(printData, "accounting");
                    #endregion
                    break;
                default:
                    break;
            }

            printData.date = DateTime.Today.ToString("yyyy年MM月dd日");
            var dbReceipt = Uof.IreceiptService.GetAll(r => r.order_id == order_id && r.order_source == name).FirstOrDefault();
            printData.no = dbReceipt.date_created.Value.ToString("yyyyMMdd") + dbReceipt.code;

            printData.memo = dbReceipt.memo;
            var creator = Uof.ImemberService.GetAll(m => m.id == dbReceipt.creator_id).Select(m => m.english_name).FirstOrDefault();
            printData.creator = creator;

            return Json(printData, JsonRequestBehavior.AllowGet);
        }

        private string GetCodeByDate(DateTime today)
        {
            var code = Uof.IreceiptService.GetAll(r => r.date_created == today)
                 .OrderByDescending(r => r.code)
                 .Select(r => r.code)
                 .FirstOrDefault();

            if (string.IsNullOrEmpty(code))
            {
                return "001";
            }

            var index = 0;
            int.TryParse(code, out index);

            return string.Format("{0}", (index + 1).ToString().PadLeft(3, '0'));
        }

        private void GetPrintDataIncome(ReceiptPrintData pd, string source_name)
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
                
                pd.received = (float)Math.Round((double)(total * pd.rate), 2);

                var acc = list[0].account ?? "";
                if (list[0].account.Length > 4)
                {
                    acc = list[0].account.Substring(list[0].account.Length - 4, 4);
                }
                pd.pay_way = list[0].pay_way;
            }
            else
            {
                pd.pay_way = "先提交,未付款";
                pd.received = 0;
            }
        }

        private void GetHistorySource(history _history, ReceiptPrintData rpd)
        {
            switch (_history.source)
            {
                case "reg_abroad":
                    var abroad = Uof.Ireg_abroadService.GetAll(r => r.id == _history.source_id).Select(r => new 
                    {
                        name_cn = r.name_cn,
                        name_en = r.name_en
                    }).FirstOrDefault();

                    rpd.customer_name = abroad.name_cn ?? abroad.name_en;
                    break;
                case "reg_internal":
                    var internalName = Uof.Ireg_internalService.GetAll(r => r.id == _history.source_id).Select(r => r.name_cn).FirstOrDefault();
                    rpd.customer_name = internalName;
                    break;
                case "patent":
                    var customerName1 = Uof.IcustomerService.GetAll(c => c.id == _history.customer_id).Select(c => c.name).FirstOrDefault();
                    rpd.customer_name = customerName1;
                    break;
                case "trademark":
                    var customerName2 = Uof.IcustomerService.GetAll(c => c.id == _history.customer_id).Select(c => c.name).FirstOrDefault();
                    rpd.customer_name = customerName2;
                    break;
                default:
                    break;
            }
        }
    }
}