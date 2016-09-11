using System;
using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using Common;
using System.Web.Security;
using System.Collections.Generic;
using WebCenter.Entities;
using System.IO;
using System.Drawing;
using System.Web;
using System.Linq.Expressions;

namespace WebCenter.Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        public ActionResult Banner()
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

            if (arrs.Length < 5)
            {
                return new HttpUnauthorizedResult();
            }

            var userId = 0;
            var deptId = 0;
            int.TryParse(arrs[0], out userId);
            int.TryParse(arrs[2], out deptId);

            var customerCount = Uof.IcustomerService.GetAll(c => c.salesman_id == userId & c.status == 1).Count();

            var annualCount = GetAnnualCount(userId);

            var res = new
            {
                customers = customerCount,
                annuals = annualCount
            };

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRecentlyCustomer()
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

            if (arrs.Length < 5)
            {
                return new HttpUnauthorizedResult();
            }

            var userId = 0;
            var deptId = 0;
            int.TryParse(arrs[0], out userId);
            int.TryParse(arrs[2], out deptId);

            var customers = Uof.IcustomerService.GetAll(c => c.salesman_id == userId).Select(c => new
            {
                id = c.id,
                code = c.code,
                name = c.name,
                salesman = c.member1.name,
                contact = c.contact,
                mobile = c.mobile
            }).OrderByDescending(c => c.id).ToPagedList(1, 10).ToList();

            return Json(customers, JsonRequestBehavior.AllowGet);
        }

        private int GetAnnualCount(int userId)
        {
            var nowYear = DateTime.Now.Year;
            var Month1 = DateTime.Now.AddMonths(-13).Month;

            #region 境外注册
            Expression<Func<reg_abroad, bool>> condition1 = c => c.status == 4 && c.salesman_id == userId && (c.submit_review_date.Value.Month - Month1 == 1 && nowYear > c.submit_review_date.Value.Year && (c.annual_year == null || (c.annual_year != null && c.annual_year < nowYear)));
            var abroads = Uof.Ireg_abroadService.GetAll(condition1).Count();
            #endregion

            #region 国内注册
            var internalMonth = DateTime.Now.Month;
            var internas = 0;
            if (internalMonth >= 3 && internalMonth <= 6)
            {
                Expression<Func<reg_internal, bool>> condition2 = c => c.status == 4 && c.salesman_id == userId && nowYear > c.submit_review_date.Value.Year && (c.annual_year == null || (c.annual_year != null && c.annual_year < nowYear));
                internas = Uof.Ireg_internalService.GetAll(condition2).Count();
            }
            #endregion

            #region 商标注册
            Expression<Func<trademark, bool>> condition3 = c => c.status == 4 && c.salesman_id == userId && c.submit_review_date.Value.Month - Month1 == 1 && nowYear > c.submit_review_date.Value.Year && (c.annual_year == null || (c.annual_year != null && c.annual_year < nowYear));
            var trademarks = Uof.ItrademarkService.GetAll(condition3).Count();
            #endregion

            #region 专利注册
            Expression<Func<patent, bool>> condition4 = c => c.status == 4 && c.salesman_id == userId && c.submit_review_date.Value.Month - Month1 == 1 && nowYear > c.submit_review_date.Value.Year && (c.annual_year == null || (c.annual_year != null && c.annual_year < nowYear));
            var patents = Uof.IpatentService.GetAll(condition4).Count();
            #endregion

            return abroads + internas + trademarks + patents;
        }
    }
}