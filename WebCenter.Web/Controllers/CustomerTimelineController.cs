using System;
using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using System.Linq.Expressions;

namespace WebCenter.Web.Controllers
{
    public class CustomerTimelineController : BaseController
    {
        public CustomerTimelineController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        public ActionResult GetTimelines(int id)
        {
            var list = Uof.Icustomer_timelineService.GetAll(t => t.customer_id == id).OrderByDescending(c => c.date_created).ToList();

            var _customer = Uof.IcustomerService.GetAll(c => c.id == id).Select(c => new {
                id = c.id,
                name = c.name
            }).FirstOrDefault();

            var result = new
            {
                items = list,
                customer = _customer
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Add(customer_timeline t)
        {
            t.is_system = 0;
            if (t.date_business == null)
            {
                t.date_business = DateTime.Now;
            }

            var r = Uof.Icustomer_timelineService.AddEntity(t);
            return SuccessResult;
        }

        public ActionResult Update(customer_timeline tl)
        {
            var timeline = Uof.Icustomer_timelineService.GetAll(t => t.id == tl.id).FirstOrDefault();

            if (timeline == null)
            {
                return ErrorResult;
            }

            if (timeline.title == tl.title && timeline.content == tl.content && timeline.date_business == tl.date_business)
            {
                return SuccessResult;
            }

            timeline.title = tl.title;
            timeline.content = tl.content;
            timeline.date_business = tl.date_business;
            timeline.date_updated = DateTime.Now;

            var r = Uof.Icustomer_timelineService.UpdateEntity(timeline);

            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            var timeline = Uof.Icustomer_timelineService.GetById(id);

            if (timeline == null)
            {
                return ErrorResult;
            }

            var r = Uof.Icustomer_timelineService.DeleteEntity(timeline);
            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get(int id)
        {
            var c = Uof.Icustomer_timelineService.GetById(id);
            if (c == null)
            {
                return ErrorResult;
            }

            return Json(c, JsonRequestBehavior.AllowGet);
        }
    }
}