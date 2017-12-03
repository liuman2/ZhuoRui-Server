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
                name = m.name
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
                name = m.name
            }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Add(string name)
        {
            var r = Uof.IsupplierService.AddEntity(new supplier()
            {
                name = name,
            });

            return SuccessResult;
        }
    }
}