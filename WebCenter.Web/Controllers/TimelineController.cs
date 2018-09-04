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
    public class TimelineController : BaseController
    {
        public TimelineController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        public ActionResult UpdateOldData()
        {
            var oldList = Uof.ItimelineService.GetAll(t => t.source_name == "annual").OrderBy(t => t.id).ToPagedList(1, 500).ToList();

            var oldTimeList = new List<timeline>();

            if (oldList != null && oldList.Count() > 0)
            {
                foreach (var item in oldList)
                {
                    // select * from annual_exam where id = 2
                    var annualExam = Uof.Iannual_examService.GetAll(a => a.id == item.source_id).FirstOrDefault();
                    if (annualExam != null)
                    {
                        item.source_id = annualExam.order_id;
                        item.source_name = annualExam.type;
                        item.title = item.title + " (年检)";

                        oldTimeList.Add(item);
                    }
                }

                if (oldTimeList != null && oldTimeList.Count > 0)
                {
                    Uof.ItimelineService.UpdateEntities(oldTimeList);
                }
            }

            return SuccessResult;
        }

        public ActionResult GetTimelines(int source_id, string source_name, string name, int? show_type)
        {
            if(show_type == null)
            {
                show_type = 9;
            }

            if (source_name == "annual")
            {
                var annualExam = Uof.Iannual_examService.GetAll(a => a.id == source_id).Select(a => new
                {
                    id = a.id,
                    type = a.type,
                    order_id = a.order_id,
                    order_code = a.order_code
                }).FirstOrDefault();

                if (annualExam != null)
                {
                    source_id = annualExam.order_id.Value;
                    source_name = annualExam.type;
                }
            }

            Expression<Func<timeline, bool>> nameQuery = c => true;

            Expression<Func<timeline, bool>> logType = c => true;

            if (!string.IsNullOrEmpty(name))
            {
                nameQuery = c => (c.title.IndexOf(name) > -1 || c.content.IndexOf(name) > -1);
            }

            if(show_type == 9)
            {
                logType = c => (c.log_type != 1);
            }
            if (show_type == 1)
            {
                logType = c => (c.log_type == 1);
            }

            var list = Uof.ItimelineService
                .GetAll(t => t.source_id == source_id && t.source_name == source_name)
                .Where(nameQuery)
                .Where(logType)
                .OrderByDescending(c => c.date_created)
                .Select(t => new TimeLine
            {
                id = t.id,
                content = t.content,
                creator_id = t.creator_id,
                date_business = t.date_business,
                date_created = t.date_created,
                date_updated = t.date_updated,
                is_system = t.is_system,
                log_type = t.log_type,
                source_id = t.source_id,
                source_name = t.source_name,
                title = t.title,
                creator = t.member.name
            }).ToList();

            var result = new
            {
                items = list
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Add(TimelineEntity timeLine)
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
            var businessCode = "";
            if (timeLine.source_name == "annual")
            {
                var annualExam = Uof.Iannual_examService.GetAll(a => a.id == timeLine.source_id).Select(a => new
                {
                    id = a.id,
                    type = a.type,
                    order_id = a.order_id,
                    order_code = a.order_code
                }).FirstOrDefault();
                businessCode = annualExam.order_code;
                if (annualExam != null)
                {
                    timeLine.title = timeLine.title;
                    timeLine.source_id = annualExam.order_id.Value;
                    timeLine.source_name = annualExam.type;
                }
            }

            timeLine.is_system = 0;
            timeLine.creator_id = userId;
            if (timeLine.date_business == null)
            {
                timeLine.date_business = DateTime.Now;
            }

            var r = Uof.ItimelineService.AddEntity(new timeline
            {
                source_id = timeLine.source_id,
                content = timeLine.content,
                creator_id = timeLine.creator_id,
                date_business = timeLine.date_business,
                is_system = timeLine.is_system,
                log_type = timeLine.log_type,
                source_name = timeLine.source_name,
                title = timeLine.title,
            });

            if (r != null && r.log_type == 1)
            {
                switch (r.source_name)
                {
                    case "reg_abroad":
                        var dbAbroad = Uof.Ireg_abroadService.GetAll(a => a.id == r.source_id).FirstOrDefault();
                        dbAbroad.date_last = r.date_created;
                        dbAbroad.title_last = r.content;
                        dbAbroad.date_wait = r.date_business;

                        Uof.Ireg_abroadService.UpdateEntity(dbAbroad);
                        businessCode = dbAbroad.code;
                        break;
                    case "reg_internal":
                        var dbInternal = Uof.Ireg_internalService.GetAll(a => a.id == r.source_id).FirstOrDefault();
                        dbInternal.date_last = r.date_created;
                        dbInternal.title_last = r.content;
                        dbInternal.date_wait = r.date_business;
                        Uof.Ireg_internalService.UpdateEntity(dbInternal);
                        businessCode = dbInternal.code;
                        break;
                    case "trademark":
                        var dbTrademark = Uof.ItrademarkService.GetAll(a => a.id == r.source_id).FirstOrDefault();
                        dbTrademark.date_last = r.date_created;
                        dbTrademark.title_last = r.content;
                        dbTrademark.date_wait = r.date_business;
                        Uof.ItrademarkService.UpdateEntity(dbTrademark);
                        businessCode = dbTrademark.code;
                        break;
                    case "patent":
                        var dbPatent = Uof.IpatentService.GetAll(a => a.id == r.source_id).FirstOrDefault();
                        dbPatent.date_last = r.date_created;
                        dbPatent.title_last = r.content;
                        dbPatent.date_wait = r.date_business;
                        Uof.IpatentService.UpdateEntity(dbPatent);
                        businessCode = dbPatent.code;
                        break;
                    default:
                        break;
                }
            }

            if(r!= null && timeLine.is_notify)
            {
                var scheduleList = new List<schedule>();
                scheduleList.Add(new schedule
                {
                    all_day = 1,
                    color = "#51b749",
                    title = timeLine.title,
                    memo = timeLine.content,
                    start = timeLine.date_notify,
                    type = 0,
                    created_id = userId,
                    date_created = DateTime.Now,
                    is_repeat = 0,
                    is_done = 0,
                    property = 2,
                    is_notify = 1,
                    source = timeLine.source_name,
                    source_id = timeLine.source_id,
                    router = r.log_type == 1 ? "annual_warning" : timeLine.source_name,
                    dealt_date = timeLine.date_business,
                    timeline_id = r.id,
                    business_code = businessCode
                });

                if (!string.IsNullOrEmpty(timeLine.notifyPeople))
                {
                    var pIds = timeLine.notifyPeople.Split(',');
                    if (pIds.Count() > 0)
                    {
                        foreach (var pId in pIds)
                        {
                            var notifyId = int.Parse(pId); 
                            scheduleList.Add(new schedule
                            {
                                all_day = 1,
                                color = "#51b749",
                                title = timeLine.title,
                                memo = timeLine.content,
                                start = timeLine.date_notify,
                                type = 0,
                                created_id = notifyId,
                                date_created = DateTime.Now,
                                is_repeat = 0,
                                is_done = 0,
                                property = 2,
                                is_notify = 1,
                                source = timeLine.source_name,
                                source_id = timeLine.source_id,
                                router = r.log_type == 1 ? "annual_warning" : timeLine.source_name,
                                dealt_date = timeLine.date_business,
                                timeline_id = r.id,
                                business_code = businessCode
                            });
                        }
                    }
                }

                Uof.IscheduleService.AddEntities(scheduleList);
            }

            return SuccessResult;
        }

        public ActionResult Update(timeline timeLine)
        {
            var timeline = Uof.ItimelineService.GetAll(t => t.id == timeLine.id).FirstOrDefault();

            if (timeline == null)
            {
                return ErrorResult;
            }

            //if (timeline.title == timeLine.title && timeline.content == timeLine.content && timeline.date_business == timeLine.date_business)
            //{
            //    return SuccessResult;
            //}
            #region old data
            var oldType = timeline.log_type;
            if (oldType == 1 && timeLine.log_type != 1)
            {
                var preTimeline = Uof.ItimelineService
                    .GetAll(t => t.source_id == timeLine.source_id &&
                    t.source_name == timeLine.source_name &&
                    t.id != timeLine.id &&
                    t.log_type == 1)
                    .OrderByDescending(t => t.id)
                    .FirstOrDefault();

                DateTime? dateLast = null;
                DateTime? dateWait = null;
                var titleLast = "";
                if (preTimeline != null)
                {
                    dateLast = preTimeline.date_created;
                    titleLast = preTimeline.content;
                    dateWait = preTimeline.date_business;
                }

                switch (timeline.source_name)
                {
                    case "reg_abroad":
                        var dbAbroad = Uof.Ireg_abroadService.GetAll(a => a.id == timeline.source_id).FirstOrDefault();
                        dbAbroad.date_last = dateLast;
                        dbAbroad.title_last = titleLast;
                        dbAbroad.date_wait = dateWait;
                        Uof.Ireg_abroadService.UpdateEntity(dbAbroad);
                        break;
                    case "reg_internal":
                        var dbInternal = Uof.Ireg_internalService.GetAll(a => a.id == timeline.source_id).FirstOrDefault();
                        dbInternal.date_last = dateLast;
                        dbInternal.title_last = titleLast;
                        dbInternal.date_wait = dateWait;
                        Uof.Ireg_internalService.UpdateEntity(dbInternal);
                        break;
                    case "trademark":
                        var dbTrademark = Uof.ItrademarkService.GetAll(a => a.id == timeline.source_id).FirstOrDefault();
                        dbTrademark.date_last = dateLast;
                        dbTrademark.title_last = titleLast;
                        dbTrademark.date_wait = dateWait;
                        Uof.ItrademarkService.UpdateEntity(dbTrademark);
                        break;
                    case "patent":
                        var dbPatent = Uof.IpatentService.GetAll(a => a.id == timeline.source_id).FirstOrDefault();
                        dbPatent.date_last = dateLast;
                        dbPatent.title_last = titleLast;
                        dbPatent.date_wait = dateWait;
                        Uof.IpatentService.UpdateEntity(dbPatent);
                        break;
                    default:
                        break;
                }
            }
            #endregion

            timeline.title = timeLine.title;
            timeline.content = timeLine.content;
            timeline.date_business = timeLine.date_business;
            timeline.log_type = timeLine.log_type;

            timeline.date_updated = DateTime.Now;

            var r = Uof.ItimelineService.UpdateEntity(timeline);

            if (r && timeline.log_type == 1)
            {
                switch (timeline.source_name)
                {
                    case "reg_abroad":
                        var dbAbroad = Uof.Ireg_abroadService.GetAll(a => a.id == timeline.source_id).FirstOrDefault();
                        dbAbroad.date_last = timeline.date_created;
                        dbAbroad.title_last = timeline.content;
                        dbAbroad.date_wait = timeline.date_business;
                        Uof.Ireg_abroadService.UpdateEntity(dbAbroad);
                        break;
                    case "reg_internal":
                        var dbInternal = Uof.Ireg_internalService.GetAll(a => a.id == timeline.source_id).FirstOrDefault();
                        dbInternal.date_last = timeline.date_created;
                        dbInternal.title_last = timeline.content;
                        dbInternal.date_wait = timeline.date_business;
                        Uof.Ireg_internalService.UpdateEntity(dbInternal);
                        break;
                    case "trademark":
                        var dbTrademark = Uof.ItrademarkService.GetAll(a => a.id == timeline.source_id).FirstOrDefault();
                        dbTrademark.date_last = timeline.date_created;
                        dbTrademark.title_last = timeline.content;
                        dbTrademark.date_wait = timeline.date_business;
                        Uof.ItrademarkService.UpdateEntity(dbTrademark);
                        break;
                    case "patent":
                        var dbPatent = Uof.IpatentService.GetAll(a => a.id == timeline.source_id).FirstOrDefault();
                        dbPatent.date_last = timeline.date_created;
                        dbPatent.title_last = timeline.content;
                        dbPatent.date_wait = timeline.date_business;
                        Uof.IpatentService.UpdateEntity(dbPatent);
                        break;
                    default:
                        break;
                }
            }

            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            var timeline = Uof.ItimelineService.GetById(id);

            if (timeline == null)
            {
                return ErrorResult;
            }

            var r = Uof.ItimelineService.DeleteEntity(timeline);

            if (r && timeline.log_type == 1)
            {
                var preTimeline = Uof.ItimelineService
                    .GetAll(t => t.source_id == timeline.source_id &&
                    t.source_name == timeline.source_name &&
                    t.id != timeline.id &&
                    t.log_type == 1)
                    .OrderByDescending(t => t.id)
                    .FirstOrDefault();


                DateTime? dateLast = null;
                DateTime? dateWait = null;
                var titleLast = "";
                if (preTimeline != null)
                {
                    dateLast = preTimeline.date_created;
                    dateWait = preTimeline.date_business;
                    titleLast = preTimeline.content;
                }

                switch (timeline.source_name)
                {
                    case "reg_abroad":
                        var dbAbroad = Uof.Ireg_abroadService.GetAll(a => a.id == timeline.source_id).FirstOrDefault();
                        dbAbroad.date_last = dateLast;
                        dbAbroad.title_last = titleLast;
                        dbAbroad.date_wait = dateWait;
                        Uof.Ireg_abroadService.UpdateEntity(dbAbroad);
                        break;
                    case "reg_internal":
                        var dbInternal = Uof.Ireg_internalService.GetAll(a => a.id == timeline.source_id).FirstOrDefault();
                        dbInternal.date_last = dateLast;
                        dbInternal.title_last = titleLast;
                        dbInternal.date_wait = dateWait;
                        Uof.Ireg_internalService.UpdateEntity(dbInternal);
                        break;
                    case "trademark":
                        var dbTrademark = Uof.ItrademarkService.GetAll(a => a.id == timeline.source_id).FirstOrDefault();
                        dbTrademark.date_last = dateLast;
                        dbTrademark.title_last = titleLast;
                        dbTrademark.date_wait = dateWait;
                        Uof.ItrademarkService.UpdateEntity(dbTrademark);
                        break;
                    case "patent":
                        var dbPatent = Uof.IpatentService.GetAll(a => a.id == timeline.source_id).FirstOrDefault();
                        dbPatent.date_last = dateLast;
                        dbPatent.title_last = titleLast;
                        dbPatent.date_wait = dateWait;
                        Uof.IpatentService.UpdateEntity(dbPatent);
                        break;
                    default:
                        break;
                }

            }
            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get(int id)
        {
            var c = Uof.ItimelineService.GetAll(t => t.id == id).Select(t => new TimeLine
            {
                id = t.id,
                content = t.content,
                creator_id = t.creator_id,
                date_business = t.date_business,
                date_created = t.date_created,
                date_updated = t.date_updated,
                is_system = t.is_system,
                log_type = t.log_type,
                source_id = t.source_id,
                source_name = t.source_name,
                title = t.source_name,
                creator = t.member.name
            }).FirstOrDefault();
            if (c == null)
            {
                return ErrorResult;
            }

            return Json(c, JsonRequestBehavior.AllowGet);
        }
    }
}