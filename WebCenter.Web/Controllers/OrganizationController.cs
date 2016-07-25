using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;

namespace WebCenter.Web.Controllers
{
    public class OrganizationController : BaseController
    {
        public OrganizationController(IUnitOfWork UOF)
            : base(UOF)
        {
        }

        public ActionResult List()
        {
            var list = Uof.IorganizationService.GetAll().Select(o=>new
            {
                id = o.id,
                parent_id = o.parent_id,
                name = o.name,
                description = o.description
            }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Add(int parent_id, string name)
        {
            var org = new organization()
            {
                parent_id = parent_id,
                name = name,
                description = ""
            };

            var newOrg = Uof.IorganizationService.AddEntity(org);

            if(newOrg == null)
            {
                return ErrorResult;
            }

            return Json(new { success = true, result = newOrg }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update(int id, string name, string description)
        {
            var oldOrg = Uof.IorganizationService.GetAll(o => o.id == id).FirstOrDefault();

            if (oldOrg == null)
            {
                return ErrorResult;
            }

            if (oldOrg.name == name && oldOrg.description == description)
            {
                return SuccessResult;
            }

            oldOrg.name = name;
            oldOrg.description = description;
            var r = Uof.IorganizationService.UpdateEntity(oldOrg);

            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            var oldOrg = Uof.IorganizationService.GetById(id);
            if (oldOrg == null)
            {
                return ErrorResult;
            }

            var r = Uof.IorganizationService.DeleteEntity(oldOrg);
            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }
    }
}