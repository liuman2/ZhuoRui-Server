using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;
using System.Linq.Expressions;
using System;

namespace WebCenter.Web.Controllers
{
    public class RoleController : BaseController
    {
        public RoleController(IUnitOfWork UOF)
            : base(UOF)
        {
        }

        public ActionResult List()
        {
            var list = Uof.IroleService.GetAll().Select(a => new
            {
                id = a.id,
                name = a.name,
                is_system = a.is_system,
                description = a.description
            }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search(int index = 1, int size = 10, string name = "")
        {
            Expression<Func<role, bool>> condition = m => true;
            if (!string.IsNullOrEmpty(name))
            {
                condition = m => (m.name.IndexOf(name) > -1);
            }

            var list = Uof.IroleService.GetAll(condition).OrderBy(item => item.id).Select(m => new
            {
                id = m.id,
                name = m.name
            }).ToPagedList(index, size).ToList();

            var totalRecord = Uof.IroleService.GetAll(condition).Count();

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

        public ActionResult Get(int id)
        {
            var _role = Uof.IroleService.GetAll(a => a.id == id).FirstOrDefault();
            if (_role == null)
            {
                return ErrorResult;
            }

            return Json(_role, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Add(string name, string description)
        {
            var r = Uof.IroleService.AddEntity(new role()
            {
                name = name,
                is_system = 0,
                code = "0",
                description = description
            });

            return SuccessResult;
        }

        [HttpPost]
        public ActionResult Update(int id, string name, string description)
        {
            var _role = Uof.IroleService.GetAll(a=>a.id == id).FirstOrDefault();
            if (_role == null)
            {
                return ErrorResult;
            }
            if (_role.name == name && _role.description == description)
            {
                return SuccessResult;
            }
            _role.name = name;
            _role.description = description;

            var r = Uof.IroleService.UpdateEntity(_role);
            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            var _role = Uof.IroleService.GetAll(a => a.id == id).FirstOrDefault();
            if (_role == null)
            {
                return ErrorResult;
            }

            if (_role.is_system == 1)
            {
                return Json(new { success = false, message = "系统默认角色，不可删除" }, JsonRequestBehavior.AllowGet);
            }

            var r = Uof.IroleService.DeleteEntity(_role);
            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }
    }
}