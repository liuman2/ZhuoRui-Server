using System;
using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;
using System.Linq.Expressions;
using System.Collections;

namespace WebCenter.Web.Controllers
{
    public class MemberController : BaseController
    {
        public MemberController(IUnitOfWork UOF)
            : base(UOF)
        {
        }

        public ActionResult List(int pageIndex = 1, int pageSize = 10, string name = "")
        {
            Expression<Func<member, bool>> condition = m => true;
            if (!string.IsNullOrEmpty(name))
            {
                Expression<Func<member, bool>> tmp = m => (m.name.IndexOf(name) > -1);
                condition = tmp;
            }

            var list = Uof.ImemberService.GetAll(condition).OrderByDescending(item => item.id).Select(m => new
            {
                id = m.id,
                name = m.name,
                english_name = m.english_name,
                username = m.username,
                department = m.organization.name,
                area = m.area.name,
                position = m.position.name,
                status = m.status
            }).ToPagedList(pageIndex, pageSize).ToList();

            var totalRecord = Uof.ImemberService.GetAll(condition).Count();

            var totalPages = 0;
            if (totalRecord > 0)
            {
                totalPages = (totalRecord + pageSize - 1) / pageSize;
            }
            var page = new
            {
                current_index = pageIndex,
                current_size = pageSize,
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
            var _area = Uof.IareaService.GetAll(a => a.id == id).FirstOrDefault();
            if (_area == null)
            {
                return ErrorResult;
            }

            return Json(_area, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Add(string name, string description)
        {
            var r = Uof.IareaService.AddEntity(new area()
            {
                name = name,
                description = description
            });

            return SuccessResult;
        }

        [HttpPost]
        public ActionResult Update(int id, string name, string description)
        {
            var _area = Uof.IareaService.GetAll(a=>a.id == id).FirstOrDefault();
            if (_area == null)
            {
                return ErrorResult;
            }
            if (_area.name == name && _area.description == description)
            {
                return SuccessResult;
            }
            _area.name = name;
            _area.description = description;

            var r = Uof.IareaService.UpdateEntity(_area);
            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            var _area = Uof.IareaService.GetAll(a => a.id == id).FirstOrDefault();
            if (_area == null)
            {
                return ErrorResult;
            }

            var r = Uof.IareaService.DeleteEntity(_area);
            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }
    }
}