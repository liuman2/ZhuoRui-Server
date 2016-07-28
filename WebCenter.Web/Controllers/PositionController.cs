using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;
using System.Linq.Expressions;
using System;

namespace WebCenter.Web.Controllers
{
    public class PositionController : BaseController
    {
        public PositionController(IUnitOfWork UOF)
            : base(UOF)
        {
        }

        public ActionResult List()
        {
            var list = Uof.IpositionService.GetAll().Select(a => new
            {
                id = a.id,
                name = a.name,
                description = a.description
            }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search(int index = 1, int size = 10, string name = "")
        {
            Expression<Func<position, bool>> condition = m => true;
            if (!string.IsNullOrEmpty(name))
            {
                Expression<Func<position, bool>> tmp = m => (m.name.IndexOf(name) > -1);
                condition = tmp;
            }

            var list = Uof.IpositionService.GetAll(condition).OrderBy(item => item.id).Select(m => new
            {
                id = m.id,
                name = m.name
            }).ToPagedList(index, size).ToList();

            var totalRecord = Uof.IpositionService.GetAll(condition).Count();

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
            var _area = Uof.IpositionService.GetAll(a => a.id == id).FirstOrDefault();
            if (_area == null)
            {
                return ErrorResult;
            }

            return Json(_area, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Add(string name, string description)
        {
            var r = Uof.IpositionService.AddEntity(new position()
            {
                name = name,
                description = description
            });

            return SuccessResult;
        }

        [HttpPost]
        public ActionResult Update(int id, string name, string description)
        {
            var _position = Uof.IpositionService.GetAll(a=>a.id == id).FirstOrDefault();
            if (_position == null)
            {
                return ErrorResult;
            }
            if (_position.name == name && _position.description == description)
            {
                return SuccessResult;
            }
            _position.name = name;
            _position.description = description;

            var r = Uof.IpositionService.UpdateEntity(_position);
            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            var _position = Uof.IpositionService.GetAll(a => a.id == id).FirstOrDefault();
            if (_position == null)
            {
                return ErrorResult;
            }

            var r = Uof.IpositionService.DeleteEntity(_position);
            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }
    }
}