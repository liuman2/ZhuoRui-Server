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

        public ActionResult GetTimelines(int source_id, string source_name)
        {
            var list = Uof.ItimelineService.GetAll(t => t.source_id == source_id && t.source_name == source_name).OrderByDescending(c => c.date_created).ToList();

            var result = new
            {
                items = list
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}