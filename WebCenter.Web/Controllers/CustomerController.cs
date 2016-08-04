using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;
using System.Linq.Expressions;
using System;

namespace WebCenter.Web.Controllers
{
    public class CustomerController : BaseController
    {
        public CustomerController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        public ActionResult Introducers(int index = 1, int size = 10, string name = "")
        {
            Expression<Func<customer, bool>> condition = c => true;
            if (!string.IsNullOrEmpty(name))
            {
                Expression<Func<customer, bool>> tmp = c => (c.name.IndexOf(name) > -1);
                condition = tmp;
            }

            var list = Uof.IcustomerService.GetAll(condition).OrderBy(item => item.id).Select(c => new
            {
                id = c.id,
                name = c.name
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

        public ActionResult Search(int index = 1, int size = 10, string name = "")
        {
            // TODO: 查询权限
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

            Expression<Func<customer, bool>> condition = c => c.status == 0 && c.salesman_id == userId;
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
            var organization_id = 0;
            int.TryParse(arrs[0], out userId);
            int.TryParse(arrs[2], out organization_id);

            c.salesman_id = userId;
            c.organization_id = organization_id;
            c.status = 1;

            var _c = Uof.IcustomerService.AddEntity(c);

            if (_c !=null)
            {
                return SuccessResult;
            }

            return ErrorResult;
        }

        public ActionResult Update(customer c)
        {
            var _c = Uof.IcustomerService.GetById(c.id);

            if (_c.name == c.name &&
                _c.address == c.address &&
                _c.city == c.city &&
                _c.contact == c.contact &&
                _c.county == c.county &&
                _c.description == c.description &&
                _c.email == c.email &&
                _c.fax == c.fax &&
                _c.industry == c.industry &&
                _c.mobile == c.mobile &&
                _c.province == c.province &&
                _c.QQ == c.QQ &&
                _c.source == c.source &&
                _c.tel == c.tel &&
                _c.wechat == c.wechat
                )
            {
                return ErrorResult;
            }

            _c.name = c.name;
            _c.address = c.address;
            _c.city = c.city;
            _c.contact = c.contact;
            _c.county = c.county;
            _c.description = c.description;
            _c.email = c.email;
            _c.fax = c.fax;
            _c.industry = c.industry;
            _c.mobile = c.mobile;
            _c.province = c.province;
            _c.QQ = c.QQ;
            _c.source = c.source;
            _c.tel = c.tel;
            _c.wechat = c.wechat;
            _c.date_updated = DateTime.Now;

            var r = Uof.IcustomerService.UpdateEntity(c);

            if (!r)
            {
                return SuccessResult;
            }

            return ErrorResult;
        }

        public ActionResult Get(int id)
        {
            var reserve = Uof.IcustomerService.GetById(id);

            return Json(reserve, JsonRequestBehavior.AllowGet);
        }
    }
}