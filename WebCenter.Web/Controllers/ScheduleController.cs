using System;
using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using Common;
using System.Web.Security;
using System.Collections.Generic;
using WebCenter.Entities;
using System.IO;
using System.Drawing;
using System.Web;

namespace WebCenter.Web.Controllers
{
    public class ScheduleController : BaseController
    {
        public ScheduleController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        [HttpPost]
        public ActionResult Add(schedule _schedule)
        {
            var auth = HttpContext.User.Identity.IsAuthenticated;
            if (!auth)
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

            _schedule.created_id = userId;
            _schedule.date_created = DateTime.Now;

            var dbSchedule = Uof.IscheduleService.AddEntity(_schedule);

            return Json(dbSchedule, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update(schedule _schedule)
        {
            var auth = HttpContext.User.Identity.IsAuthenticated;
            if (!auth)
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

            var dbSchedule = Uof.IscheduleService.GetAll(s => s.id == _schedule.id).FirstOrDefault();
            if (dbSchedule == null)
            {
                return Json(new { success = false, message = "找不到该日程" }, JsonRequestBehavior.AllowGet);
            }

            dbSchedule.date_updated = DateTime.Now;
            dbSchedule.updated_id = userId;

            dbSchedule.title = _schedule.title;
            dbSchedule.color = _schedule.color;
            dbSchedule.end = _schedule.end;
            dbSchedule.location = _schedule.location;
            dbSchedule.memo = _schedule.memo;
            dbSchedule.people = _schedule.people;
            dbSchedule.start = _schedule.start;
            dbSchedule.type = _schedule.type;
            dbSchedule.all_day = _schedule.all_day;

            var result = Uof.IscheduleService.UpdateEntity(dbSchedule);
            return Json(new { success = result, message = "更新成功" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            var auth = HttpContext.User.Identity.IsAuthenticated;
            if (!auth)
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

            var dbSchedule = Uof.IscheduleService.GetById(id);
            if (dbSchedule.created_id != userId)
            {
                return Json(new { success = false, message="您没有权限删除该日程" }, JsonRequestBehavior.AllowGet);
            }

            var r = Uof.IscheduleService.DeleteEntity(dbSchedule);
            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetToday()
        {
            var auth = HttpContext.User.Identity.IsAuthenticated;
            if (!auth)
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
            var strUserId = userId.ToString();

            var dt1 = DateTime.Today;
            var dt2 = DateTime.Today.AddDays(1);

            var list = Uof.IscheduleService
                .GetAll(s => s.created_id == userId || s.type == 2 || (s.type == 1 && s.people.Contains(strUserId)))
                .Where(s=>s.start.Value >= dt1 && s.start < dt2)
                .Select(s => new ScheduleEntity
            {
                id = s.id,
                attachment = s.attachment,
                color = s.color,
                created_id = s.created_id,
                date_created = s.date_created,
                date_updated = s.date_updated,
                creator = "",
                end = s.end,
                location = s.location ?? "",
                memo = s.memo ?? "",
                people = s.people,
                start = s.start,
                title = s.title,
                type = s.type,
                updated_id = s.updated_id,
                all_day = s.all_day,
            }).ToList();

            if (list.Count == 0)
            {
                return Json(list, JsonRequestBehavior.AllowGet);
            }

            var newList = new List<ScheduleEntity>();

            var memberList = Uof.ImemberService.GetAll().Select(m => new SimplePeople
            {
                id = m.id,
                name = m.name,
            }).ToList();

            foreach (var item in list)
            {
                item.editable = item.created_id == userId;
                item.allDay = item.all_day == 1;

                if (item.end != null && item.end.Value.ToString("yyyy-MM-dd") == DateTime.Today.ToString("yyyy-MM-dd"))
                {
                    item.timeStr = string.Format("{0}-{1}", item.start.Value.ToString("HH:mm"), item.end.Value.ToString("HH:mm"));
                } else
                {
                    item.timeStr = string.Format("{0}", item.start.Value.ToString("HH:mm"));
                }

                var creator = memberList.Where(m => m.id == item.created_id).FirstOrDefault();
                if (creator != null)
                {
                    item.creator = creator.name;
                }

                if (item.type != 1)
                {
                    newList.Add(item);
                }

                if (item.type == 1)
                {
                    var isIn = CheckPeopleIn(strUserId, item);
                    if (!isIn)
                    {
                        continue;
                    }

                    item.peoples = GetSimplePeopleList(item.people, memberList);
                    newList.Add(item);
                }
            }

            return Json(newList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search()
        {
            var auth = HttpContext.User.Identity.IsAuthenticated;
            if (!auth)
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
            var strUserId = userId.ToString();

            var list = Uof.IscheduleService.GetAll(s => s.created_id == userId || s.type == 2 || (s.type == 1 && s.people.Contains(strUserId))).Select(s=> new ScheduleEntity
            {
                id = s.id,
                attachment = s.attachment,
                color = s.color,
                created_id = s.created_id,
                date_created = s.date_created,
                date_updated = s.date_updated,
                creator = "",
                end = s.end,
                location = s.location ?? "",
                memo = s.memo ?? "",
                people = s.people,
                start = s.start,
                title = s.title,
                type = s.type,
                updated_id = s.updated_id,
                all_day = s.all_day,
            }).ToList();

            if (list.Count == 0)
            {
                return Json(list, JsonRequestBehavior.AllowGet);
            }

            var newList = new List<ScheduleEntity>();

            var memberList = Uof.ImemberService.GetAll().Select(m => new SimplePeople
            {
                id = m.id,
                name = m.name,
            }).ToList();

            foreach (var item in list)
            {
                item.editable = item.created_id == userId;
                item.allDay = item.all_day == 1;

                var creator = memberList.Where(m => m.id == item.created_id).FirstOrDefault();
                if (creator != null)
                {
                    item.creator = creator.name;
                }

                if (item.type != 1)
                {
                    newList.Add(item);
                }

                if (item.type == 1)
                {
                    var isIn = CheckPeopleIn(strUserId, item);
                    if (!isIn)
                    {
                        continue;
                    }

                    item.peoples = GetSimplePeopleList(item.people, memberList);
                    newList.Add(item);
                }
            }

            return Json(newList, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult Get(int id)
        {
            var dbSchedule = Uof.IscheduleService.GetAll(s => s.id == id).Select(s => new ScheduleEntity
            {
                id = s.id,
                attachment = s.attachment,
                color = s.color,
                created_id = s.created_id,
                date_created = s.date_created,
                date_updated = s.date_updated,
                creator = "",
                end = s.end,
                location = s.location,
                memo = s.memo,
                people = s.people,
                start = s.start,
                title = s.title,
                type = s.type,
                updated_id = s.updated_id,
                all_day = s.all_day,

            }).FirstOrDefault();

            if (dbSchedule.type == 1)
            {
                var memberList = Uof.ImemberService.GetAll().Select(m => new SimplePeople
                {
                    id = m.id,
                    name = m.name,
                }).ToList();

                dbSchedule.peoples = GetSimplePeopleList(dbSchedule.people, memberList);
                dbSchedule.creator = memberList.Where(m => m.id == dbSchedule.created_id).Select(m => m.name).FirstOrDefault();
            } else
            {
                var creator = Uof.ImemberService.GetAll(m=>m.id == dbSchedule.created_id).Select(m => new SimplePeople
                {
                    id = m.id,
                    name = m.name,
                }).FirstOrDefault();

                dbSchedule.creator = creator.name;
            }

            return Json(dbSchedule, JsonRequestBehavior.AllowGet);
        } 

        private bool CheckPeopleIn(string userId, ScheduleEntity entity)
        {
            if (string.IsNullOrEmpty(entity.people))
            {
                return false;
            }

            if (entity.created_id.ToString() == userId)
            {
                return true;
            }

            var people = entity.people;
            var hasMore = people.Contains(",");
            if (!hasMore)
            {
                return userId == people;
            }

            var peoples = people.Split(',');
            var hasIn = false;
            foreach (var item in peoples)
            {
                if (item.Trim() == userId)
                {
                    hasIn = true;
                    break;
                }
            }

            return hasIn;
        }

        private List<SimplePeople> GetSimplePeopleList(string people, List<SimplePeople> memberList)
        {
            var peoples = people.Split(',');
            var peopleList = new List<SimplePeople>();
            foreach (var item in peoples)
            {
                var userId = 0;
                int.TryParse(item, out userId);
                var user = memberList.Where(m => m.id == userId).FirstOrDefault();
                if (user != null)
                {
                    peopleList.Add(user);
                }                
            }

            return peopleList;
        }
    }
}