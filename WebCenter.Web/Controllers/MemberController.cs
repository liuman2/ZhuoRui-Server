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

        public ActionResult List(int index = 1, int size = 10, string name = "")
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
                position = m.position.name
                //status = m.status
            }).ToPagedList(index, size).ToList();

            var totalRecord = Uof.ImemberService.GetAll(condition).Count();

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

        public ActionResult ExistUsername(string username, int? id)
        {
            Expression<Func<member, bool>> condition = m => (m.username == username);
            if (id != null)
            {
                Expression<Func<member, bool>> tmp = m => (m.id == id.Value);
                condition = tmp;
            }

            var _member = Uof.ImemberService.GetAll(condition).Select(m => new {
                id = m.id,
                name = m.name,
                username = m.username
            }).FirstOrDefault();

            if (_member == null)
            {
                return Json(new { ok = "验证成功" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = "用户名已存在" }, JsonRequestBehavior.AllowGet);
        }
    }
}