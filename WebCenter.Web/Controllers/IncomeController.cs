using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;
using System.Linq.Expressions;
using System;

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
                return Json(new { success = false, message = "付款账号不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (_inc.amount == null)
            {
                return Json(new { success = false, message = "付款金额不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (_inc.date_pay == null)
            {
                return Json(new { success = false, message = "付款日期额不能为空" }, JsonRequestBehavior.AllowGet);
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
                        
            var dbInc = Uof.IincomeService.AddEntity(_inc);
            if (dbInc == null)
            {
                return Json(new { success = false, message = "保存失败" }, JsonRequestBehavior.AllowGet);
            }

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
               date_pay = i.date_pay,
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
                dbIncome.amount == _income.amount &&
                dbIncome.date_pay == _income.date_pay &&
                dbIncome.attachment_url == _income.attachment_url)
            {
                return SuccessResult;
            }

            dbIncome.payer = _income.payer;
            dbIncome.account = _income.account;
            dbIncome.amount = _income.amount;
            dbIncome.date_pay = _income.date_pay;
            dbIncome.attachment_url = _income.attachment_url;

            dbIncome.date_updated = DateTime.Now;

            var r = Uof.IincomeService.UpdateEntity(dbIncome);

            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            var dbIncome = Uof.IincomeService.GetById(id);

            var r = Uof.IincomeService.DeleteEntity(dbIncome);

            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }
    }
}