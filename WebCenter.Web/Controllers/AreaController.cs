using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;

namespace WebCenter.Web.Controllers
{
    public class AreaController : BaseController
    {
        public AreaController(IUnitOfWork UOF)
            : base(UOF)
        {
        }

        public ActionResult List()
        {
            var list = Uof.IareaService.GetAll().Select(a => new
            {
                id = a.id,
                name = a.name,
                description = a.description
            }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}