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

    }
}