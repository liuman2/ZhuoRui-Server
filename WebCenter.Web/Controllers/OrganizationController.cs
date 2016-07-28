using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;
using System.Linq.Expressions;
using System;

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

        public ActionResult Search(int index = 1, int size = 10, string name = "")
        {
            Expression<Func<organization, bool>> condition = m => true;
            if (!string.IsNullOrEmpty(name))
            {
                Expression<Func<organization, bool>> tmp = m => (m.name.IndexOf(name) > -1);
                condition = tmp;
            }

            var list = Uof.IorganizationService.GetAll(condition).OrderBy(item => item.id).Select(m => new
            {
                id = m.id,
                name = m.name
            }).ToPagedList(index, size).ToList();

            var totalRecord = Uof.IorganizationService.GetAll(condition).Count();

            var totalPages = 0;
            if (totalRecord > 0)
            {
                totalPages = (totalRecord + size - 1) / size;
            }
            var page = new
            {
                current_index = index,
                current_size = size,
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