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
            var oldList = Uof.ItimelineService.GetAll(t => t.source_name == "annual").OrderBy(t=>t.id).ToPagedList(1, 500).ToList();

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

        public ActionResult GetTimelines(int source_id, string source_name, string name)
        {
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

            if (!string.IsNullOrEmpty(name))
            {
                nameQuery = c => (c.title.IndexOf(name) > -1 || c.content.IndexOf(name) > -1);
            }

            var list = Uof.ItimelineService.GetAll(t => t.source_id == source_id && t.source_name == source_name).Where(nameQuery).OrderByDescending(c => c.date_created).ToList();

            var result = new
            {
                items = list
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Add(timeline timeLine)
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

            if (timeLine.source_name == "annual")
            {
                var annualExam = Uof.Iannual_examService.GetAll(a => a.id == timeLine.source_id).Select(a => new
                {
                    id = a.id,
                    type = a.type,
                    order_id = a.order_id,
                    order_code = a.order_code
                }).FirstOrDefault();

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

            var r = Uof.ItimelineService.AddEntity(timeLine);

            if (r != null && r.log_type == 1)
            {
                switch (r.source_name)
                {
                    case "reg_abroad":
                        var dbAbroad = Uof.Ireg_abroadService.GetAll(a => a.id == r.source_id).FirstOrDefault();
                        dbAbroad.date_last = r.date_business;
                        dbAbroad.title_last = r.content;
                        Uof.Ireg_abroadService.UpdateEntity(dbAbroad);
                        break;
                    case "reg_internal":
                        var dbInternal = Uof.Ireg_internalService.GetAll(a => a.id == r.source_id).FirstOrDefault();
                        dbInternal.date_last = r.date_business;
                        dbInternal.title_last = r.content;
                        Uof.Ireg_internalService.UpdateEntity(dbInternal);
                        break;
                    case "trademark":
                        var dbTrademark = Uof.ItrademarkService.GetAll(a => a.id == r.source_id).FirstOrDefault();
                        dbTrademark.date_last = r.date_business;
                        dbTrademark.title_last = r.content;
                        Uof.ItrademarkService.UpdateEntity(dbTrademark);
                        break;
                    case "patent":
                        var dbPatent = Uof.IpatentService.GetAll(a => a.id == r.source_id).FirstOrDefault();
                        dbPatent.date_last = r.date_business;
                        dbPatent.title_last = r.content;
                        Uof.IpatentService.UpdateEntity(dbPatent);
                        break;
                    default:
                        break;
                }
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
                var titleLast = "";
                if (preTimeline != null)
                {
                    dateLast = preTimeline.date_business;
                    titleLast = preTimeline.content;
                }

                switch (timeline.source_name)
                {
                    case "reg_abroad":
                        var dbAbroad = Uof.Ireg_abroadService.GetAll(a => a.id == timeline.source_id).FirstOrDefault();
                        dbAbroad.date_last  = dateLast;
                        dbAbroad.title_last = titleLast;
                        Uof.Ireg_abroadService.UpdateEntity(dbAbroad);
                        break;
                    case "reg_internal":
                        var dbInternal = Uof.Ireg_internalService.GetAll(a => a.id == timeline.source_id).FirstOrDefault();
                        dbInternal.date_last  = dateLast;
                        dbInternal.title_last = titleLast;
                        Uof.Ireg_internalService.UpdateEntity(dbInternal);
                        break;
                    case "trademark":
                        var dbTrademark = Uof.ItrademarkService.GetAll(a => a.id == timeline.source_id).FirstOrDefault();
                        dbTrademark.date_last = dateLast;
                        dbTrademark.title_last = titleLast;
                        Uof.ItrademarkService.UpdateEntity(dbTrademark);
                        break;
                    case "patent":
                        var dbPatent = Uof.IpatentService.GetAll(a => a.id == timeline.source_id).FirstOrDefault();
                        dbPatent.date_last = dateLast;
                        dbPatent.title_last = titleLast;
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
                        dbAbroad.date_last = timeline.date_business;
                        dbAbroad.title_last = timeline.content;
                        Uof.Ireg_abroadService.UpdateEntity(dbAbroad);
                        break;
                    case "reg_internal":
                        var dbInternal = Uof.Ireg_internalService.GetAll(a => a.id == timeline.source_id).FirstOrDefault();
                        dbInternal.date_last = timeline.date_business;
                        dbInternal.title_last = timeline.content;
                        Uof.Ireg_internalService.UpdateEntity(dbInternal);
                        break;
                    case "trademark":
                        var dbTrademark = Uof.ItrademarkService.GetAll(a => a.id == timeline.source_id).FirstOrDefault();
                        dbTrademark.date_last = timeline.date_business;
                        dbTrademark.title_last = timeline.content;
                        Uof.ItrademarkService.UpdateEntity(dbTrademark);
                        break;
                    case "patent":
                        var dbPatent = Uof.IpatentService.GetAll(a => a.id == timeline.source_id).FirstOrDefault();
                        dbPatent.date_last = timeline.date_business;
                        dbPatent.title_last = timeline.content;
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
                var titleLast = "";
                if (preTimeline != null)
                {
                    dateLast = preTimeline.date_business;
                    titleLast = preTimeline.content;
                }

                switch (timeline.source_name)
                {
                    case "reg_abroad":
                        var dbAbroad = Uof.Ireg_abroadService.GetAll(a => a.id == timeline.source_id).FirstOrDefault();
                        dbAbroad.date_last = dateLast;
                        dbAbroad.title_last = titleLast;
                        Uof.Ireg_abroadService.UpdateEntity(dbAbroad);
                        break;
                    case "reg_internal":
                        var dbInternal = Uof.Ireg_internalService.GetAll(a => a.id == timeline.source_id).FirstOrDefault();
                        dbInternal.date_last = dateLast;
                        dbInternal.title_last = titleLast;
                        Uof.Ireg_internalService.UpdateEntity(dbInternal);
                        break;
                    case "trademark":
                        var dbTrademark = Uof.ItrademarkService.GetAll(a => a.id == timeline.source_id).FirstOrDefault();
                        dbTrademark.date_last = dateLast;
                        dbTrademark.title_last = titleLast;
                        Uof.ItrademarkService.UpdateEntity(dbTrademark);
                        break;
                    case "patent":
                        var dbPatent = Uof.IpatentService.GetAll(a => a.id == timeline.source_id).FirstOrDefault();
                        dbPatent.date_last = dateLast;
                        dbPatent.title_last = titleLast;
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
            var c = Uof.ItimelineService.GetById(id);
            if (c == null)
            {
                return ErrorResult;
            }

            return Json(c, JsonRequestBehavior.AllowGet);
        }
    }
}