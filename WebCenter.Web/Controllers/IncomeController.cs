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
    public class IncomeController : BaseController
    {
        public IncomeController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        public ActionResult Add(income _inc)
        {
            if (string.IsNullOrEmpty(_inc.payer))
            {
                return Json(new { success = false, message = "付款人不能为空" }, JsonRequestBehavior.AllowGet);
            }            
            if (string.IsNullOrEmpty(_inc.account))
            {
                return Json(new { success = false, message = "收款账号不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (_inc.amount == null)
            {
                return Json(new { success = false, message = "收款金额不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (_inc.date_pay == null)
            {
                return Json(new { success = false, message = "收款日期额不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (_inc.source_id == null)
            {
                return Json(new { success = false, message = "source_id不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (_inc.customer_id == null)
            {
                return Json(new { success = false, message = "customer_id不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(_inc.source_name))
            {
                return Json(new { success = false, message = "source_name不能为空" }, JsonRequestBehavior.AllowGet);
            }


            var identityName = HttpContext.User.Identity.Name;
            var arrs = identityName.Split('|');
            if (arrs.Length == 0)
            {
                return new HttpUnauthorizedResult();
            }

            var dbInc = Uof.IincomeService.AddEntity(_inc);
            if (dbInc == null)
            {
                return Json(new { success = false, message = "保存失败" }, JsonRequestBehavior.AllowGet);
            }

            Uof.ItimelineService.AddEntity(new timeline()
            {
                source_id = _inc.source_id,
                source_name = _inc.source_name,
                title = "新增收款",
                content = string.Format("{0}新增了收款, 金额{1}", arrs[3], dbInc.amount)
            });

            return SuccessResult;

        }

        public ActionResult Get(int id)
        {
           var dbIncome = Uof.IincomeService.GetAll(i => i.id == id).Select(i=> new
           {
               id = i.id,
               payer = i.payer,
               account = i.account,
               amount = i.amount,
               bank = i.bank,
               date_pay = i.date_pay,
               pay_way = i.pay_way,
               attachment_url = i.attachment_url,
               description = i.description
           }).FirstOrDefault();

            return Json(dbIncome, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(income _income)
        {
            var dbIncome = Uof.IincomeService.GetAll(i => i.id == _income.id).FirstOrDefault();

            if (dbIncome.payer == _income.payer &&
                dbIncome.account == _income.account &&
                dbIncome.bank == _income.bank &&
                dbIncome.amount == _income.amount &&
                dbIncome.date_pay == _income.date_pay &&
                dbIncome.pay_way == _income.pay_way &&
                dbIncome.attachment_url == _income.attachment_url)
            {
                return SuccessResult;
            }


            var identityName = HttpContext.User.Identity.Name;
            var arrs = identityName.Split('|');
            if (arrs.Length == 0)
            {
                return new HttpUnauthorizedResult();
            }

            dbIncome.payer = _income.payer;
            dbIncome.account = _income.account;
            dbIncome.amount = _income.amount;
            dbIncome.date_pay = _income.date_pay;
            dbIncome.pay_way = _income.pay_way;
            dbIncome.bank = _income.bank;
            dbIncome.attachment_url = _income.attachment_url;

            dbIncome.date_updated = DateTime.Now;

            var r = Uof.IincomeService.UpdateEntity(dbIncome);

            if (r)
            {
                var msg = new List<string>();
                if (dbIncome.payer != _income.payer)
                {
                    msg.Add(string.Format("付款人{0}->{1}", dbIncome.payer, _income.payer));
                }
                if (dbIncome.account != _income.account)
                {
                    msg.Add(string.Format("账号{0}->{1}", dbIncome.account, _income.account));
                }
                if (dbIncome.amount != _income.amount)
                {
                    msg.Add(string.Format("金额{0}->{1}", dbIncome.amount, _income.amount));
                }
                if (dbIncome.date_pay != _income.date_pay)
                {
                    msg.Add(string.Format("付款日期{0}->{1}", dbIncome.date_pay.Value.ToString("yyyy-MM-dd"), _income.date_pay.Value.ToString("yyyy-MM-dd")));
                }

                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbIncome.source_id,
                    source_name = dbIncome.source_name,
                    title = "修改收款",
                    content = string.Format("{0}修改了收款, {1}", arrs[3], string.Join(",", msg))
                });
            }

            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            var identityName = HttpContext.User.Identity.Name;
            var arrs = identityName.Split('|');
            if (arrs.Length == 0)
            {
                return new HttpUnauthorizedResult();
            }

            var dbIncome = Uof.IincomeService.GetById(id);

            var r = Uof.IincomeService.DeleteEntity(dbIncome);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbIncome.source_id,
                    source_name = dbIncome.source_name,
                    title = "删除收款",
                    content = string.Format("{0}删除了收款, 金额{1}", arrs[3], dbIncome.amount)
                });
            }

            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }
    }
}