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

        [HttpPost]
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
                    router = "audit_leave_view",
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

        public ActionResult Pass(int id)
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

            if (result)
            {
                var waitdeals = new List<waitdeal>();
                waitdeals.Add(new waitdeal
                {
                    source = "leave",
                    source_id = dbLeave.id,
                    user_id = dbLeave.owner_id,
                    router = "my_leave_view",
                    content = "您的假单已批准",
                    read_status = 0
                });

                var owner = Uof.ImemberService.GetAll(m => m.id == dbLeave.owner_id).Select(m => m.name).FirstOrDefault();
                waitdeals.Add(new waitdeal
                {
                    source = "leave",
                    source_id = dbLeave.id,
                    user_id = dbLeave.receiver_id,
                    router = "leave_view",
                    content = string.Format("您被{0}指定了请假区间的工作交接人", owner),
                    read_status = 0
                });

                Uof.IwaitdealService.AddEntities(waitdeals);
            }

            return Json(new { success = result, id = dbLeave.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Refuse(int id, string memo)
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

            if (result)
            {
                Uof.IwaitdealService.AddEntity(new waitdeal
                {
                    source = "leave",
                    source_id = dbLeave.id,
                    user_id = dbLeave.owner_id,
                    router = "my_leave_view",
                    content = "您的假单已被驳回",
                    read_status = 0
                });
            }

            return Json(new { success = result, id = dbLeave.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Abandon(int id)
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

        public ActionResult Get(int id)
        {
            var dbLeave = Uof.IleaveService.GetAll(l => l.id == id).Select(l => new LeaveResponse
            {
                id = l.id,
                auditor_id = l.auditor_id,
                auditor_name = "",
                audit_memo = l.audit_memo,
                date_created = l.date_created,
                date_end = l.date_end,
                date_review = l.date_review,
                date_start = l.date_start,
                memo = l.memo,
                owner_id = l.owner_id,
                owner_name = "",
                reason = l.reason,
                receiver_id = l.receiver_id,
                receiver_name = "",
                status = l.status,
                status_name = "",
                tel = l.tel,
                type = l.type,
                type_name = ""
            }).FirstOrDefault();

            if (dbLeave == null)
            {
                return ErrorResult;
            }
            // LeaveResponse
            //var leaver_ids = list.Select(l => l.owner_id).Distinct();
            var members = Uof.ImemberService.GetAll(m => m.id == dbLeave.receiver_id || m.id == dbLeave.auditor_id || m.id == dbLeave.owner_id).Select(m => new
            {
                id = m.id,
                name = m.name,
                department = m.organization.name,
                position = m.position.name
            }).ToList();
            if (members.Count > 0)
            {
                var m1 = members.Where(a => a.id == dbLeave.receiver_id).FirstOrDefault();
                if (m1 != null)
                {
                    dbLeave.receiver_name = m1.name;
                }

                var m2 = members.Where(a => a.id == dbLeave.auditor_id).FirstOrDefault();
                if (m2 != null)
                {
                    dbLeave.auditor_name = m2.name;
                }

                var m3 = members.Where(a => a.id == dbLeave.owner_id).FirstOrDefault();
                if (m3 != null)
                {
                    dbLeave.owner_name = m3.name;
                    dbLeave.department = m3.department;
                    dbLeave.position = m3.position;
                }
            }

            return Json(dbLeave, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Statistics(LeaveSearchRequest request)
        {
            //member_id: '',
            //type: '',
            //start_time: '',
            //end_time:''

            Expression<Func<leave, bool>> condition = c => true;
            if (request.member_id != null && request.member_id.Value > 0)
            {
                condition = c => (c.owner_id == request.member_id.Value);
            }

            Expression<Func<leave, bool>> typeQuery = c => true;
            if (request.type != null)
            {
                typeQuery = c => (c.type == request.type.Value);
            }

            Expression<Func<leave, bool>> dateQuery = c => true;
            if (request.start_time != null && request.end_time != null)
            {
                var endTime = request.end_time.Value.AddDays(1);
                dateQuery = c => ((c.date_start >= request.start_time.Value && c.date_end < endTime));
            }
            if (request.start_time != null && request.end_time == null)
            {
                dateQuery = c => (c.date_start >= request.start_time.Value);
            }
            if (request.start_time == null && request.end_time != null)
            {
                var endTime = request.end_time.Value.AddDays(1);
                dateQuery = c => (c.date_end < endTime);
            }

            var list = Uof.IleaveService.GetAll(l => l.status >= -1)
                .Where(condition)
                .Where(typeQuery)
                .Where(dateQuery)
                .OrderByDescending(item => item.date_created).Select(c => new LeaveResponse
                {
                    id = c.id,
                    auditor_id = c.auditor_id,
                    auditor_name = "",
                    owner_id = c.owner_id,
                    owner_name = "",
                    date_created = c.date_created,
                    date_start = c.date_start,
                    date_end = c.date_end,
                    date_review = c.date_review,
                    type = c.type,
                    status = c.status

                }).ToPagedList(request.index, request.size).ToList();

            var totalRecord = Uof.IleaveService.GetAll(l => l.status >= -1)
                .Where(condition)
                .Where(typeQuery)
                .Where(dateQuery).Count();

            var totalPages = 0;
            if (totalRecord > 0)
            {
                totalPages = (totalRecord + request.size - 1) / request.size;                
            }

            if (list.Count > 0)
            {
                var memberIds = list.Select(l => l.owner_id).ToList();

                var owners =  Uof.ImemberService.GetAll(m => memberIds.Contains(m.id)).ToList();

                foreach (var item in list)
                {
                    var ds = item.date_end - item.date_start;

                    var name = owners.Where(o => o.id == item.owner_id).Select(m => m.name).FirstOrDefault();
                    item.owner_name = name;
                    item.hours = Math.Round(ds.Value.TotalHours, 2);
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
    }
}