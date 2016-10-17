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
    public class LeaveController : BaseController
    {
        public LeaveController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        public ActionResult Add(leave _leave)
        {
            _leave.status = 0;
            var dbLeave = Uof.IleaveService.AddEntity(_leave);

            if (dbLeave != null)
            {
                Uof.IwaitdealService.AddEntity(new waitdeal
                {
                    source = "leave",
                    source_id = dbLeave.id,
                    user_id = dbLeave.auditor_id,
                    router = "leave_view",
                    content = "您有新的假单需要审批",
                    read_status = 0
                });
            }

            return SuccessResult;
        }

        public ActionResult GetMyLeave(LeaveSearchRequest request)
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

            if (arrs.Length < 5)
            {
                return new HttpUnauthorizedResult();
            }

            var userId = 0;
            int.TryParse(arrs[0], out userId);

            Expression<Func<leave, bool>> condition = l => l.owner_id == userId;

            Expression<Func<leave, bool>> statusQuery = c => true;
            if (request.status != null)
            {
                statusQuery = l => (l.status == request.status);
            }

            Expression<Func<leave, bool>> date1Query = c => true;
            Expression<Func<leave, bool>> date2Query = c => true;
            if (request.start_time != null)
            {
                date1Query = c => (c.date_created >= request.start_time.Value);
            }
            if (request.end_time != null)
            {
                var endTime = request.end_time.Value.AddDays(1);
                date2Query = c => (c.date_created < endTime);
            }

            var list = Uof.IleaveService.GetAll(condition)
                .Where(statusQuery)
                .Where(date1Query)
                .Where(date2Query)
                .OrderByDescending(item => item.id).Select(c => new LeaveResponse
                {
                    id = c.id,
                    owner_id = c.owner_id,
                    type = c.type,
                    date_start = c.date_start,
                    date_end = c.date_end,
                    reason = c.reason,
                    memo = c.memo,
                    receiver_id = c.receiver_id,
                    tel = c.tel,
                    auditor_id = c.auditor_id,
                    audit_memo = c.audit_memo,
                    date_review = c.date_review,
                    status = c.status,
                    date_created = c.date_created
                }).ToPagedList(request.index, request.size).ToList();

            var totalRecord = Uof.IleaveService.GetAll(condition)
                .Where(statusQuery)
                .Where(date1Query)
                .Where(date2Query).Count();

            var totalPages = 0;
            if (totalRecord > 0)
            {
                totalPages = (totalRecord + request.size - 1) / request.size;
            }
            var page = new
            {
                current_index = request.index,
                current_size = request.size,
                total_size = totalRecord,
                total_page = totalPages
            };

            var result = new
            {
                page = page,
                items = list
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMyAuditLeave(LeaveSearchRequest request)
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

            if (arrs.Length < 5)
            {
                return new HttpUnauthorizedResult();
            }

            var userId = 0;
            int.TryParse(arrs[0], out userId);

            Expression<Func<leave, bool>> condition = l => l.auditor_id == userId && l.status != -1;

            Expression<Func<leave, bool>> statusQuery = c => true;
            if (request.status != null)
            {
                statusQuery = l => (l.status == request.status);
            }

            Expression<Func<leave, bool>> date1Query = c => true;
            Expression<Func<leave, bool>> date2Query = c => true;
            if (request.start_time != null)
            {
                date1Query = c => (c.date_created >= request.start_time.Value);
            }
            if (request.end_time != null)
            {
                var endTime = request.end_time.Value.AddDays(1);
                date2Query = c => (c.date_created < endTime);
            }

            var list = Uof.IleaveService.GetAll(condition)
                .Where(statusQuery)
                .Where(date1Query)
                .Where(date2Query)
                .OrderByDescending(item => item.id).Select(c => new LeaveResponse
                {
                    id = c.id,
                    owner_id = c.owner_id,
                    type = c.type,
                    date_start = c.date_start,
                    date_end = c.date_end,
                    reason = c.reason,
                    memo = c.memo,
                    receiver_id = c.receiver_id,
                    tel = c.tel,
                    auditor_id = c.auditor_id,
                    audit_memo = c.audit_memo,
                    date_review = c.date_review,
                    status = c.status,
                    date_created = c.date_created
                }).ToPagedList(request.index, request.size).ToList();

            var totalRecord = Uof.IleaveService.GetAll(condition)
                .Where(statusQuery)
                .Where(date1Query)
                .Where(date2Query).Count();

            

            var totalPages = 0;
            if (totalRecord > 0)
            {
                totalPages = (totalRecord + request.size - 1) / request.size;

                var leaver_ids = list.Select(l => l.owner_id).Distinct();
                var members = Uof.ImemberService.GetAll(m => leaver_ids.Contains(m.id)).Select(m => new { id = m.id, name = m.name }).ToList();
                foreach(var l in list)
                {
                    var m = members.Where(a => a.id == l.owner_id).FirstOrDefault();
                    if (m != null)
                    {
                        l.owner_name = m.name;
                    }
                }
            }

            var page = new
            {
                current_index = request.index,
                current_size = request.size,
                total_size = totalRecord,
                total_page = totalPages
            };

            var result = new
            {
                page = page,
                items = list
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PassLeave(int id)
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

            if (arrs.Length < 5)
            {
                return new HttpUnauthorizedResult();
            }

            var userId = 0;
            int.TryParse(arrs[0], out userId);

            var dbLeave = Uof.IleaveService.GetAll(l => l.id == id && l.auditor_id == userId).FirstOrDefault();
            dbLeave.status = 1;
            dbLeave.date_updated = DateTime.Now;
            dbLeave.date_review = DateTime.Now;

            var result = Uof.IleaveService.UpdateEntity(dbLeave);

            return Json(new { success = result, id = dbLeave.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RefuseLeave(int id, string memo)
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

            if (arrs.Length < 5)
            {
                return new HttpUnauthorizedResult();
            }

            var userId = 0;
            int.TryParse(arrs[0], out userId);

            var dbLeave = Uof.IleaveService.GetAll(l => l.id == id && l.auditor_id == userId).FirstOrDefault();
            dbLeave.status = 2;
            dbLeave.date_updated = DateTime.Now;
            dbLeave.date_review = DateTime.Now;
            dbLeave.audit_memo = memo;

            var result = Uof.IleaveService.UpdateEntity(dbLeave);

            return Json(new { success = result, id = dbLeave.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AbandonLeave(int id)
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

            if (arrs.Length < 5)
            {
                return new HttpUnauthorizedResult();
            }

            var userId = 0;
            int.TryParse(arrs[0], out userId);

            var u = Uof.IleaveService.GetAll(l => l.id == id && l.owner_id == userId).FirstOrDefault();
            if (u == null)
            {
                return ErrorResult;
            }

            u.status = -1;
            u.date_updated = DateTime.Now;

            Uof.IleaveService.UpdateEntity(u);

            return SuccessResult;
        }
    }
}