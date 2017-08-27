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

            dbSchedule.is_done = _schedule.is_done;
            dbSchedule.is_repeat = _schedule.is_repeat;
            dbSchedule.meeting_type = _schedule.meeting_type;
            dbSchedule.presenter_id = _schedule.presenter_id;
            dbSchedule.repeat_type = _schedule.repeat_type;
            dbSchedule.repeat_dow = _schedule.repeat_dow;
            dbSchedule.property = _schedule.property;

            dbSchedule.repeat_end = _schedule.repeat_end;

            if (_schedule.property != 0)
            {
                dbSchedule.meeting_type = null;
                dbSchedule.presenter_id = null;
            }

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
                .Where(s=>(s.start.Value >= dt1 && s.start < dt2) && s.is_repeat == 0)
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

                is_repeat = s.is_repeat,
                repeat_type = s.repeat_type,
                repeat_dow = s.repeat_dow,
                is_done = s.is_done,
                property = s.property,
                meeting_type = s.meeting_type,
                presenter_id = s.presenter_id,
                presenter = "",

                }).ToList();

            var repeatList = Uof.IscheduleService
                .GetAll(s => s.created_id == userId || s.type == 2 || (s.type == 1 && s.people.Contains(strUserId)))
                .Where(s => s.is_repeat == 1 && s.repeat_end >= dt1)
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
                    is_repeat = s.is_repeat,
                    repeat_type = s.repeat_type,
                    repeat_dow = s.repeat_dow,
                    is_done = s.is_done,
                    property = s.property,
                    meeting_type = s.meeting_type,
                    presenter_id = s.presenter_id,
                    presenter = "",

                }).ToList();

            var newRepeatList = new List<ScheduleEntity>();

            if (repeatList.Count() > 0)
            {
                foreach (var item in repeatList)
                {
                    if (item.repeat_type == 1)
                    {                        
                        var todayWeek = (int)DateTime.Today.DayOfWeek;
                        var index = item.repeat_dow.IndexOf(todayWeek.ToString());
                        if (index >  -1)
                        {
                            newRepeatList.Add(item);
                        }
                    }
                    if (item.repeat_type == 2)
                    {
                        int month = DateTime.Today.Month - item.start.Value.Month;
                        if (item.start.Value.AddMonths(month).Date == DateTime.Today)
                        {
                            newRepeatList.Add(item);
                        }
                    }

                    if (item.repeat_type == 3)
                    {
                        int year = DateTime.Today.Year - item.start.Value.Year;
                        if (item.start.Value.AddYears(year).Date == DateTime.Today)
                        {
                            newRepeatList.Add(item);
                        }
                    }
                }
            }

            if (newRepeatList.Count() > 0)
            {
                list.AddRange(newRepeatList);
            }

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

                if (item.presenter_id != null)
                {
                    var presenter = memberList.Where(m => m.id == item.presenter_id).FirstOrDefault();
                    if (presenter != null)
                    {
                        item.presenter = presenter.name;
                    }
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

                is_repeat = s.is_repeat,
                repeat_type = s.repeat_type,
                repeat_dow = s.repeat_dow,
                repeat_end = s.repeat_end,
                is_done = s.is_done,
                property = s.property ?? null,
                meeting_type = s.meeting_type,
                presenter_id = s.presenter_id,
                presenter = "",

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

            var repeatList = new List<ScheduleEntity>();

            foreach (var item in list)
            {
                item.editable = item.created_id == userId;
                item.allDay = item.all_day == 1;

                if (item.repeat_type == 1)
                {
                    var dows = item.repeat_dow.Split(',');
                    if (dows.Count() > 0)
                    {
                        item.dow = new List<int>();
                        foreach (var dow in dows)
                        {
                            int iDow = 0;
                            int.TryParse(dow, out iDow);
                            item.dow.Add(iDow);
                        }
                    }
                }

                var creator = memberList.Where(m => m.id == item.created_id).FirstOrDefault();
                if (creator != null)
                {
                    item.creator = creator.name;
                }

                if (item.presenter_id != null)
                {
                    var presenter = memberList.Where(m => m.id == item.presenter_id).FirstOrDefault();
                    if (presenter != null)
                    {
                        item.presenter = presenter.name;
                    }
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

                if (item.is_repeat == 1)
                {
                    item.repeat_start = item.start;

                    if (item.repeat_type == 2)
                    {
                        int year = item.repeat_end.Value.Year - item.start.Value.Year;
                        int month = item.repeat_end.Value.Month - item.start.Value.Month;
                        int total = year * 12 + month;
                        for (int i = 0; i < total; i++)
                        {
                            var repeatItem = new ScheduleEntity
                            {
                                id = item.id,
                                attachment = item.attachment,
                                color = item.color,
                                created_id = item.created_id,
                                date_created = item.date_created,
                                date_updated = item.date_updated,
                                creator = item.creator,
                                end = item.end,
                                location = item.location ?? "",
                                memo = item.memo ?? "",
                                people = item.people,
                                start = item.start.Value.AddMonths(i + 1),
                                title = item.title,
                                type = item.type,
                                updated_id = item.updated_id,
                                all_day = item.all_day,

                                is_repeat = item.is_repeat,
                                repeat_type = item.repeat_type,
                                repeat_dow = item.repeat_dow,
                                repeat_end = item.repeat_end,
                                is_done = item.is_done,
                                property = item.property ?? null,
                                meeting_type = item.meeting_type,
                                presenter_id = item.presenter_id,
                                presenter = item.presenter,

                                editable = item.editable,
                                allDay = item.allDay,
                                repeat_start = item.start,
                            };
                            repeatList.Add(repeatItem);
                        }
                    }
                    if (item.repeat_type == 3)
                    {
                        int total = item.repeat_end.Value.Year - item.start.Value.Year;
                        for (int i = 0; i < total; i++)
                        {
                            var repeatItem = new ScheduleEntity
                            {
                                id = item.id,
                                attachment = item.attachment,
                                color = item.color,
                                created_id = item.created_id,
                                date_created = item.date_created,
                                date_updated = item.date_updated,
                                creator = item.creator,
                                end = item.end,
                                location = item.location ?? "",
                                memo = item.memo ?? "",
                                people = item.people,
                                start = item.start.Value.AddYears(i + 1),
                                title = item.title,
                                type = item.type,
                                updated_id = item.updated_id,
                                all_day = item.all_day,

                                is_repeat = item.is_repeat,
                                repeat_type = item.repeat_type,
                                repeat_dow = item.repeat_dow,
                                repeat_end = item.repeat_end,
                                is_done = item.is_done,
                                property = item.property ?? null,
                                meeting_type = item.meeting_type,
                                presenter_id = item.presenter_id,
                                presenter = item.presenter,

                                editable = item.editable,
                                allDay = item.allDay,
                                repeat_start = item.start,
                            };
                            repeatList.Add(repeatItem);
                        }
                    }
                }
            }

            if (repeatList.Count() > 0)
            {
                newList.AddRange(repeatList);
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