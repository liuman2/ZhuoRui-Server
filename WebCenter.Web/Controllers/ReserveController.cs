using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;
using System.Linq.Expressions;
using System;

namespace WebCenter.Web.Controllers
{
    public class ReserveController : BaseController
    {
        public ReserveController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        public ActionResult Search(int index = 1, int size = 10, string name = "")
        {
            var r = HttpContext.User.Identity.IsAuthenticated;
            if (!r)
            {
                return new HttpUnauthorizedResult();
            }

            var identityName = HttpContext.User.Identity.Name;
            var arrs = identityName.Split('|');
            if (arrs.Length == 0)
            {
                return new HttpUnauthorizedResult();
            }

            var userId = 0;
            int.TryParse(arrs[0], out userId);

            Expression<Func<customer, bool>> condition = c => c.salesman_id == userId;
            if (!string.IsNullOrEmpty(name))
            {
                Expression<Func<customer, bool>> tmp = c => (c.name.IndexOf(name) > -1);
                condition = tmp;
            }

            var list = Uof.IcustomerService.GetAll(condition).OrderBy(item => item.id).Select(c => new
            {
                id = c.id,
                name = c.name,
                contact = c.contact,
                mobile = c.mobile,
                tel = c.tel
            }).ToPagedList(index, size).ToList();

            var totalRecord = Uof.IcustomerService.GetAll(condition).Count();

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

        public ActionResult Add(customer c)
        {
            var r = HttpContext.User.Identity.IsAuthenticated;
            if (!r)
            {
                return new HttpUnauthorizedResult();
            }

            var identityName = HttpContext.User.Identity.Name;
            var arrs = identityName.Split('|');
            if (arrs.Length == 0)
            {
                return new HttpUnauthorizedResult();
            }

            var userId = 0;
            int.TryParse(arrs[0], out userId);

            c.salesman_id = userId;
            //c.organization_id =
            Uof.IcustomerService.AddEntity(c);

            return ErrorResult;
        }
    }
}