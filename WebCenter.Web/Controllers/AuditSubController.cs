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
    public class AuditSubController : BaseController
    {
        public AuditSubController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        [HttpPost]
        public ActionResult Add(sub_audit subAudit)
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

            var custId = Uof.IauditService.GetAll(a => a.id == subAudit.master_id).Select(a => a.customer_id).FirstOrDefault();

            var userId = 0;
            var organization_id = 0;
            int.TryParse(arrs[0], out userId);
            int.TryParse(arrs[2], out organization_id);

            subAudit.status = 0;
            subAudit.review_status = -1;
            subAudit.creator_id = userId;
            subAudit.customer_id = custId;

            var newAbroad = Uof.Isub_auditService.AddEntity(subAudit);
            if (newAbroad == null)
            {
                return Json(new { success = false, message = "添加失败" }, JsonRequestBehavior.AllowGet);
            }

            var timelines = new List<timeline>();
            timelines.Add(new timeline
            {
                source_id = subAudit.master_id,
                source_name = "audit",
                title = "新增账期",
                content = string.Format("{0}新增了账期", arrs[3])
            });

            timelines.Add(new timeline
            {
                source_id = subAudit.id,
                source_name = "sub_audit",
                title = "新增账期",
                content = string.Format("{0}新增了账期", arrs[3])
            });
            try
            {
                Uof.ItimelineService.AddEntities(timelines);
            }
            catch (Exception)
            {
            }
            return Json(new { id = newAbroad.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get(int id)
        {
            var sub = Uof.Isub_auditService.GetAll(s => s.id == id).Select(a => new SubAudit
            {
                id = a.id,
                master_id = a.master_id,
                customer_id = a.customer_id,
                customer_name = a.customer.name,
                turnover_currency = a.turnover_currency,
                account_period = a.account_period,
                account_period2 = a.account_period2,
                date_year_end = a.date_year_end,
                turnover = a.turnover,
                amount_bank = a.amount_bank,
                bill_number = a.bill_number,
                accounting_standard = a.accounting_standard,
                cost_accounting = a.cost_accounting,
                progress = a.progress,
                date_transaction = a.date_transaction,
                amount_transaction = a.amount_transaction,
                date_finish = a.date_finish,
                currency = a.currency,
                rate = a.rate,
                salesman_id = a.salesman_id,
                salesman = a.member4.name,
                accountant_id = a.accountant_id,
                accountant_name = a.member.name,
                manager_id = a.manager_id,
                manager_name = a.member3.name,
                assistant_id = a.assistant_id,
            }).FirstOrDefault();

            return Json(sub, JsonRequestBehavior.AllowGet);
        }

    }
}