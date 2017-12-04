using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;

namespace WebCenter.Web.Controllers
{
    public class SupplierController : BaseController
    {
        public SupplierController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        public ActionResult Get(int id)
        {
            var _supplier = Uof.IsupplierService.GetAll(a => a.id == id).Select(a=> new
            {
                id = a.id,
                name = a.name,
                memo = a.memo,
            }).FirstOrDefault();
            if (_supplier == null)
            {
                return ErrorResult;
            }

            return Json(_supplier, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search(int index = 1, int size = 10, string name = "")
        {
            Expression<Func<supplier, bool>> condition = m => true;
            if (!string.IsNullOrEmpty(name))
            {
                Expression<Func<supplier, bool>> tmp = m => (m.name.IndexOf(name) > -1);
                condition = tmp;
            }

            var list = Uof.IsupplierService.GetAll(condition).OrderBy(item => item.id).Select(m => new
            {
                id = m.id,
                name = m.name,
                memo = m.memo,
            }).ToPagedList(index, size).ToList();

            var totalRecord = Uof.IsupplierService.GetAll(condition).Count();

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

        public ActionResult GetAll()
        {
            var list = Uof.IsupplierService.GetAll().Select(m => new
            {
                id = m.id,
                name = m.name,
                memo = m.memo,
            }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Add(string name, string memo)
        {
            var r = Uof.IsupplierService.AddEntity(new supplier()
            {
                name = name,
                memo = memo,
            });

            return SuccessResult;
        }

        [HttpPost]
        public ActionResult Update(int id, string name, string memo)
        {
            var _d = Uof.IsupplierService.GetAll(a => a.id == id).FirstOrDefault();
            if (_d == null)
            {
                return ErrorResult;
            }

            _d.name = name;
            _d.memo = memo;

            var r = Uof.IsupplierService.UpdateEntity(_d);
            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }
    }
}