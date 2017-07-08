using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;
using System.Linq.Expressions;
using System;

namespace WebCenter.Web.Controllers
{
    public class TimelineController : BaseController
    {
        public TimelineController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        public ActionResult GetTimelines(int source_id, string source_name, string name)
        {
            if (source_name == "annual")
            {
                // TODO
                var annualExam = Uof.Iannual_examService.GetAll(a => a.id == source_id).Select(a=>new
                {
                    id = a.id,
                    type = a.type,
                    order_id = a.order_id,
                    order_code = a.order_code
                }).FirstOrDefault();

                if (annualExam != null)
                {

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
            timeLine.is_system = 0;
            if (timeLine.date_business == null)
            {
                timeLine.date_business = DateTime.Now;
            }

            var r = Uof.ItimelineService.AddEntity(timeLine);
            return SuccessResult;
        }

        public ActionResult Update(timeline timeLine)
        {
            var timeline = Uof.ItimelineService.GetAll(t => t.id == timeLine.id).FirstOrDefault();

            if (timeline == null)
            {
                return ErrorResult;
            }

            if (timeline.title == timeLine.title && timeline.content == timeLine.content && timeline.date_business == timeLine.date_business)
            {
                return SuccessResult;
            }

            timeline.title = timeLine.title;
            timeline.content = timeLine.content;
            timeline.date_business = timeLine.date_business;
            timeline.date_updated = DateTime.Now;

            var r = Uof.ItimelineService.UpdateEntity(timeline);

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