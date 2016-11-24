using System;
using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using Common;
using System.Web.Security;
using System.Linq.Expressions;
using WebCenter.Entities;
using System.Collections.Generic;

namespace WebCenter.Web.Controllers
{
    public class AnnualController : BaseController
    {
        public AnnualController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        public ActionResult GetSourceForAudit(int customer_id, string type)
        {
            var year = DateTime.Now.Year;

            switch (type)
            {
                case "境内":
                    var internals = Uof.Ireg_internalService.GetAll(i => i.customer_id == customer_id && (i.annual_year == null || i.annual_year < year)).Select(i => new
                    {
                        id = i.id,
                        name_cn = i.name_cn,
                        name_en = "",
                        code = i.code,
                        customer_name = i.customer.name,
                        date_finish = i.date_finish,
                        date_setup = i.date_setup,
                        address = i.address,
                        salesman = i.member5.name,
                    }).ToList();

                    return Json(internals, JsonRequestBehavior.AllowGet);
                case "境外":
                    var abroads = Uof.Ireg_abroadService.GetAll(i => i.customer_id == customer_id && (i.annual_year == null || i.annual_year < year)).Select(i => new
                    {
                        id = i.id,
                        name_cn = i.name_cn,
                        name_en = i.name_en,
                        code = i.code,
                        customer_name = i.customer.name,
                        date_finish = i.date_finish,
                        date_setup = i.date_setup,
                        address = i.address,
                        salesman = i.member4.name
                    }).ToList();

                    return Json(abroads, JsonRequestBehavior.AllowGet);
            }

            return SuccessResult;
        }

        public ActionResult Warning(int? customer_id, int? waiter_id, int? salesman_id, string name, string area)
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

            var ops = arrs[4].Split(',');
            var hasInspect = ops.Where(o => o == "5").FirstOrDefault();
            var hasCompany = ops.Where(o => o == "1").FirstOrDefault();
            var hasDepart = ops.Where(o => o == "2").FirstOrDefault();

            var items = new List<AnnualWarning>();
            var nowYear = DateTime.Now.Year;
            var Month1 = DateTime.Now.AddMonths(2).Month; // DateTime.Now.AddMonths(-13).Month;
            var Month2 = DateTime.Now.Month;
            var Month3 = DateTime.Now.AddMonths(1).Month;

            #region 境外注册
            Expression<Func<reg_abroad, bool>> condition1 = c => c.status == 4 &&
            ((c.annual_date == null && c.annual_year.Value < nowYear && (Month1 == (c.date_setup.Value.Month) || Month2 >= (c.date_setup.Value.Month) || Month3 == (c.date_setup.Value.Month)) && nowYear >= c.date_setup.Value.Year) ||
            (c.annual_date != null && c.annual_year.Value < nowYear && (Month1 == (c.date_setup.Value.Month) || Month2 >= (c.date_setup.Value.Month) || Month3 == (c.date_setup.Value.Month)) && nowYear >= c.annual_date.Value.Year) ||
            (c.annual_year == null && (Month1 == (c.date_setup.Value.Month) || Month3 == (c.date_setup.Value.Month)) && nowYear >= c.date_setup.Value.Year) ||
            (c.annual_year != null && c.annual_year == nowYear && (Month1 == (c.date_setup.Value.Month) || Month3 == (c.date_setup.Value.Month)) && nowYear >= c.date_setup.Value.Year) ||
            (c.is_annual == 1 && ((c.annual_year == null) || (c.annual_year != null && c.annual_year.Value < nowYear))));
            Expression<Func<reg_abroad, bool>> customerQuery1 = c => true;
            if (customer_id != null && customer_id.Value > 0)
            {
                customerQuery1 = c => (c.customer_id == customer_id);
            }

            //Expression<Func<reg_abroad, bool>> userQuery1 = c => true;
            //if (hasInspect == null)
            //{
            //    if (hasCompany == null)
            //    {
            //        if (hasDepart != null)
            //        {
            //            userQuery1 = c => c.organization_id == deptId;
            //        }
            //        else
            //        {
            //            userQuery1 = c => (c.salesman_id == userId || c.assistant_id == userId);
            //        }
            //    }
            //}
            //else
            //{
            //    if (hasCompany == null)
            //    {
            //        if (hasDepart != null)
            //        {
            //            userQuery1 = c => c.organization_id == deptId;
            //        }
            //        else
            //        {
            //            userQuery1 = c => c.waiter_id == userId;
            //        }
            //    }
            //}

            //int? waiter_id, int? salesman_id, string name
            Expression<Func<reg_abroad, bool>> waiterQuery1 = c => true;
            if (waiter_id != null)
            {
                waiterQuery1 = c => c.waiter_id == waiter_id;
            }
            Expression<Func<reg_abroad, bool>> salesmanQuery1 = c => true;
            if (salesman_id != null)
            {
                salesmanQuery1 = c => c.salesman_id == salesman_id;
            }
            Expression<Func<reg_abroad, bool>> nameQuery1 = c => true;
            if (!string.IsNullOrEmpty(name))
            {
                nameQuery1 = c => (c.name_cn.Contains(name) || c.name_en.Contains(name));
            }
            Expression<Func<reg_abroad, bool>> areaQuery1 = c => true;
            if (!string.IsNullOrEmpty(area))
            {
                areaQuery1 = c => c.code.Contains(area);
            }

            var abroads = Uof.Ireg_abroadService.GetAll(condition1).Where(customerQuery1)
                .Where(waiterQuery1)
                .Where(salesmanQuery1)
                .Where(nameQuery1)
                .Where(areaQuery1)
                .Select(a => new AnnualWarning
                {
                    id = a.id,
                    customer_id = a.customer_id,
                    customer_name = a.customer.name,
                    customer_code = a.customer.code,
                    order_code = a.code,
                    order_name = a.name_cn ?? a.name_en,
                    order_type = "reg_abroad",
                    order_type_name = "境外注册",
                    saleman = a.member4.name,
                    waiter = a.member6.name,
                    assistant_name = a.member7.name,
                    submit_review_date = a.submit_review_date,
                    date_finish = a.date_finish,
                    date_setup = a.date_setup,
                    annual_date = a.annual_date,
                    annual_year = a.annual_year,
                    month = DateTime.Today.Month - a.date_setup.Value.Month,
                }).ToList();

            if (abroads.Count() > 0)
            {
                var newList = abroads.Where(a => a.annual_date == null || (a.annual_date != null && a.annual_date.Value.AddMonths(10) <= DateTime.Today)).ToList();
                items.AddRange(newList);
            }
            #endregion

            #region 国内注册
            var internalMonth = DateTime.Now.Month;
            if (internalMonth >= 3 && internalMonth <= 6)
            {
                //Expression<Func<reg_internal, bool>> condition2 = c => c.status == 4 &&
                //((c.annual_date == null && c.annual_year.Value < nowYear && (Month1 == (c.date_setup.Value.Month) || Month2 >= (c.date_setup.Value.Month) || Month3 == (c.date_setup.Value.Month)) && nowYear >= c.date_setup.Value.Year) ||
                //(c.annual_date != null && c.annual_year.Value < nowYear && (Month1 == (c.date_setup.Value.Month) || Month2 >= (c.date_setup.Value.Month) || Month3 == (c.date_setup.Value.Month)) && nowYear >= c.annual_date.Value.Year) ||
                //(c.annual_year == null && (Month1 == (c.date_setup.Value.Month) || Month3 == (c.date_setup.Value.Month)) && nowYear >= c.date_setup.Value.Year) ||
                //(c.annual_year != null && c.annual_year == nowYear && (Month1 == (c.date_setup.Value.Month) || Month3 == (c.date_setup.Value.Month)) && nowYear >= c.date_setup.Value.Year) ||
                //(c.is_annual == 1 && ((c.annual_year == null) || (c.annual_year != null && c.annual_year.Value < nowYear))));

                Expression<Func<reg_internal, bool>> condition2 = c => c.status == 4 &&
               ((c.annual_year == null && nowYear >= c.date_setup.Value.Year) ||
               (c.annual_year != null && c.annual_year.Value < nowYear) ||
               (c.is_annual == 1 && ((c.annual_year == null) || (c.annual_year != null && c.annual_year.Value < nowYear))));

                Expression<Func<reg_internal, bool>> customerQuery2 = c => true;

                if (customer_id != null && customer_id.Value > 0)
                {
                    customerQuery2 = c => (c.customer_id == customer_id);
                }

                //Expression<Func<reg_internal, bool>> userQuery2 = c => true;
                //if (hasInspect == null)
                //{
                //    if (hasCompany == null)
                //    {
                //        if (hasDepart != null)
                //        {
                //            userQuery2 = c => c.organization_id == deptId;
                //        }
                //        else
                //        {
                //            userQuery2 = c => (c.salesman_id == userId || c.assistant_id == userId);
                //        }
                //    }
                //}
                //else
                //{
                //    if (hasCompany == null)
                //    {
                //        if (hasDepart != null)
                //        {
                //            userQuery2 = c => c.organization_id == deptId;
                //        }
                //        else
                //        {
                //            userQuery2 = c => c.waiter_id == userId;
                //        }
                //    }
                //}

                Expression<Func<reg_internal, bool>> waiterQuery2 = c => true;
                if (waiter_id != null)
                {
                    waiterQuery2 = c => c.waiter_id == waiter_id;
                }
                Expression<Func<reg_internal, bool>> salesmanQuery2 = c => true;
                if (salesman_id != null)
                {
                    salesmanQuery2 = c => c.salesman_id == salesman_id;
                }
                Expression<Func<reg_internal, bool>> nameQuery2 = c => true;
                if (!string.IsNullOrEmpty(name))
                {
                    nameQuery2 = c => c.name_cn.Contains(name);
                }
                Expression<Func<reg_internal, bool>> areaQuery2 = c => true;
                if (!string.IsNullOrEmpty(area))
                {
                    areaQuery2 = c => c.code.Contains(area);
                }

                var internas = Uof.Ireg_internalService.GetAll(condition2).Where(customerQuery2)
                    .Where(waiterQuery2)
                    .Where(salesmanQuery2)
                    .Where(nameQuery2)
                    .Where(areaQuery2)
                    .Select(a => new AnnualWarning
                    {
                        id = a.id,
                        customer_id = a.customer_id,
                        customer_name = a.customer.name,
                        customer_code = a.customer.code,
                        order_code = a.code,
                        order_name = a.name_cn,
                        order_type = "reg_internal",
                        order_type_name = "境内注册",
                        saleman = a.member5.name,
                        waiter = a.member7.name,
                        assistant_name = a.member.name,
                        submit_review_date = a.submit_review_date,
                        date_finish = a.date_finish,
                        date_setup = a.date_setup,
                        annual_year = a.annual_year,
                        month = DateTime.Today.Month - a.date_setup.Value.Month,
                    }).ToList();

                if (internas.Count() > 0)
                {
                    items.AddRange(internas);
                }
            }
            #endregion

            #region 商标注册
            // Expression<Func<trademark, bool>> condition3 = c => c.status == 4 && c.submit_review_date.Value.Month - Month1 >= 1 && nowYear > c.submit_review_date.Value.Year && (c.annual_year == null || (c.annual_year != null && c.annual_year < nowYear));

            var trademarkPeriodSetting = Uof.IsettingService.GetAll(s => s.name == "TRADEMARK_PERIOD").Select(s => s.value).FirstOrDefault();
            int trademarkPeriod = 0;
            int.TryParse(trademarkPeriodSetting, out trademarkPeriod);

            Expression<Func<trademark, bool>> condition3 = c => c.status == 4 &&
            ((c.annual_date == null && c.annual_year.Value < (nowYear - trademarkPeriod) && (Month1 == (c.date_regit.Value.Month) || Month2 >= (c.date_regit.Value.Month) || Month3 == (c.date_regit.Value.Month)) && (nowYear - trademarkPeriod) == c.date_regit.Value.Year) ||
            (c.annual_date != null && c.annual_year.Value < (nowYear - trademarkPeriod) && (Month1 == (c.date_regit.Value.Month) || Month2 >= (c.date_regit.Value.Month) || Month3 == (c.date_regit.Value.Month)) && (nowYear - trademarkPeriod) == c.annual_date.Value.Year) ||
            (c.is_annual == 1 && ((c.annual_year == null) || (c.annual_year != null && c.annual_year.Value < nowYear))));

            Expression<Func<trademark, bool>> customerQuery3 = c => true;
            if (customer_id != null && customer_id.Value > 0)
            {
                customerQuery3 = c => (c.customer_id == customer_id);
            }

            //Expression<Func<trademark, bool>> userQuery3 = c => true;
            //if (hasInspect == null)
            //{
            //    if (hasCompany == null)
            //    {
            //        if (hasDepart != null)
            //        {
            //            userQuery3 = c => c.organization_id == deptId;
            //        }
            //        else
            //        {
            //            userQuery3 = c => (c.salesman_id == userId || c.assistant_id == userId);
            //        }
            //    }
            //}
            //else
            //{
            //    if (hasCompany == null)
            //    {
            //        if (hasDepart != null)
            //        {
            //            userQuery3 = c => c.organization_id == deptId;
            //        }
            //        else
            //        {
            //            userQuery3 = c => c.waiter_id == userId;
            //        }
            //    }
            //}

            Expression<Func<trademark, bool>> waiterQuery3 = c => true;
            if (waiter_id != null)
            {
                waiterQuery3 = c => c.waiter_id == waiter_id;
            }
            Expression<Func<trademark, bool>> salesmanQuery3 = c => true;
            if (salesman_id != null)
            {
                salesmanQuery3 = c => c.salesman_id == salesman_id;
            }
            Expression<Func<trademark, bool>> nameQuery3 = c => true;
            if (!string.IsNullOrEmpty(name))
            {
                nameQuery3 = c => c.name.Contains(name);
            }
            Expression<Func<trademark, bool>> areaQuery3 = c => true;
            if (!string.IsNullOrEmpty(area))
            {
                areaQuery3 = c => c.code.Contains(area);
            }

            var trademarks = Uof.ItrademarkService.GetAll(condition3).Where(customerQuery3)
                .Where(waiterQuery3)
                .Where(salesmanQuery3)
                .Where(nameQuery3)
                .Where(areaQuery3)
                .Select(a => new AnnualWarning
                {
                    id = a.id,
                    customer_id = a.customer_id,
                    customer_name = a.customer.name,
                    customer_code = a.customer.code,
                    order_code = a.code,
                    order_name = a.name,
                    order_type = "trademark",
                    order_type_name = "商标注册",
                    saleman = a.member4.name,
                    waiter = a.member6.name,
                    assistant_name = a.member.name,
                    submit_review_date = a.submit_review_date,
                    date_finish = a.date_finish,
                    date_setup = a.date_regit,
                    annual_year = a.annual_year,
                    month = (a.date_regit != null) ? (DateTime.Today.Month - a.date_regit.Value.Month) : 0,
                }).ToList();

            if (trademarks.Count() > 0)
            {
                items.AddRange(trademarks);
            }
            #endregion

            #region 专利注册            
            var patentPeriodSetting = Uof.IsettingService.GetAll(s => s.name == "PATENT_PERIOD").Select(s => s.value).FirstOrDefault();
            int patentPeriod = 0;
            int.TryParse(patentPeriodSetting, out patentPeriod);

            Expression<Func<patent, bool>> condition4 = c => c.status == 4 &&
            ((c.annual_date == null && c.annual_year.Value < (nowYear - patentPeriod) && (Month1 == (c.date_regit.Value.Month) || Month2 >= (c.date_regit.Value.Month) || Month3 == (c.date_regit.Value.Month)) && (nowYear - patentPeriod) == c.date_regit.Value.Year) ||
            (c.annual_date != null && c.annual_year.Value < (nowYear - patentPeriod) && (Month1 == (c.date_regit.Value.Month) || Month2 >= (c.date_regit.Value.Month) || Month3 == (c.date_regit.Value.Month)) && (nowYear - patentPeriod) == c.annual_date.Value.Year) ||
            (c.is_annual == 1 && ((c.annual_year == null) || (c.annual_year != null && c.annual_year.Value < nowYear))));

            Expression<Func<patent, bool>> customerQuery4 = c => true;
            if (customer_id != null && customer_id.Value > 0)
            {
                customerQuery4 = c => (c.customer_id == customer_id);
            }

            //Expression<Func<patent, bool>> userQuery4 = c => true;
            //if (hasInspect == null)
            //{
            //    if (hasCompany == null)
            //    {
            //        if (hasDepart != null)
            //        {
            //            userQuery3 = c => c.organization_id == deptId;
            //        }
            //        else
            //        {
            //            userQuery3 = c => (c.salesman_id == userId || c.assistant_id == userId);
            //        }
            //    }
            //}
            //else
            //{
            //    if (hasCompany == null)
            //    {
            //        if (hasDepart != null)
            //        {
            //            userQuery3 = c => c.organization_id == deptId;
            //        }
            //        else
            //        {
            //            userQuery3 = c => c.waiter_id == userId;
            //        }
            //    }
            //}

            Expression<Func<patent, bool>> waiterQuery4 = c => true;
            if (waiter_id != null)
            {
                waiterQuery3 = c => c.waiter_id == waiter_id;
            }
            Expression<Func<patent, bool>> salesmanQuery4 = c => true;
            if (salesman_id != null)
            {
                salesmanQuery3 = c => c.salesman_id == salesman_id;
            }
            Expression<Func<patent, bool>> nameQuery4 = c => true;
            if (!string.IsNullOrEmpty(name))
            {
                nameQuery3 = c => c.name.Contains(name);
            }
            Expression<Func<patent, bool>> areaQuery4 = c => true;
            if (!string.IsNullOrEmpty(area))
            {
                areaQuery4 = c => c.code.Contains(area);
            }

            var patents = Uof.IpatentService.GetAll(condition4)
                .Where(waiterQuery4)
                .Where(salesmanQuery4)
                .Where(nameQuery4)
                .Where(areaQuery4)
                .Select(a => new AnnualWarning
                {
                    id = a.id,
                    customer_id = a.customer_id,
                    customer_name = a.customer.name,
                    customer_code = a.customer.code,
                    order_code = a.code,
                    order_name = a.name,
                    order_type = "patent",
                    order_type_name = "专利注册",
                    saleman = a.member4.name,
                    waiter = a.member6.name,
                    assistant_name = a.member.name,
                    submit_review_date = a.submit_review_date,
                    date_finish = a.date_finish,
                    date_setup = a.date_regit,
                    annual_year = a.annual_year,
                    month = (a.date_regit != null) ? (DateTime.Today.Month - a.date_regit.Value.Month) : 0,
                }).ToList();

            if (patents.Count() > 0)
            {
                items.AddRange(patents);
            }
            #endregion

            var result = new
            {
                items = items
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBusinessOrder(int orderId, string orderType)
        {
            var annualOrigOrder = new AnnualOrigOrder();
            switch (orderType)
            {
                case "reg_abroad":
                    annualOrigOrder = Uof.Ireg_abroadService.GetAll(a => a.id == orderId).Select(a => new AnnualOrigOrder
                    {
                        order_id = a.id,
                        order_code = a.code,
                        order_type_name = "境外注册",
                        name_cn = a.name_cn ?? a.name_en,
                        name_en = a.name_en,

                        customer_id = a.customer_id,
                        customer_code = a.customer.code,
                        customer_name = a.customer.name,
                        date_setup = a.date_setup,
                        salesman_id = a.salesman_id,
                        salesman = a.member4.name,
                        assistant_id = a.assistant_id,
                        assistant_name = a.member7.name
                    }).FirstOrDefault();
                    break;
                case "reg_internal":
                    annualOrigOrder = Uof.Ireg_internalService.GetAll(a => a.id == orderId).Select(a => new AnnualOrigOrder
                    {
                        order_id = a.id,
                        order_code = a.code,
                        order_type_name = "境内注册",
                        name_cn = a.name_cn,
                        name_en = "",
                        customer_id = a.customer_id,
                        customer_code = a.customer.code,
                        customer_name = a.customer.name,
                        date_setup = a.date_setup,
                        salesman_id = a.salesman_id,
                        salesman = a.member5.name,
                        assistant_id = a.assistant_id,
                        assistant_name = a.member.name,
                    }).FirstOrDefault();
                    break;
                case "trademark":
                    annualOrigOrder = Uof.ItrademarkService.GetAll(a => a.id == orderId).Select(a => new AnnualOrigOrder
                    {
                        order_id = a.id,
                        order_code = a.code,
                        order_type_name = "商标注册",
                        name_cn = a.name,
                        name_en = "",
                        customer_id = a.customer_id,
                        customer_code = a.customer.code,
                        customer_name = a.customer.name,
                        date_setup = a.date_trial,

                        salesman_id = a.salesman_id,
                        salesman = a.member4.name,
                        assistant_id = a.assistant_id,
                        assistant_name = a.member.name,
                    }).FirstOrDefault();
                    break;
                case "patent":
                    annualOrigOrder = Uof.IpatentService.GetAll(a => a.id == orderId).Select(a => new AnnualOrigOrder
                    {
                        order_id = a.id,
                        order_code = a.code,
                        order_type_name = "专利注册",
                        name_cn = a.name,
                        name_en = "",
                        customer_id = a.customer_id,
                        customer_code = a.customer.code,
                        customer_name = a.customer.name,
                        date_setup = a.date_empower,

                        salesman_id = a.salesman_id,
                        salesman = a.member4.name,
                        assistant_id = a.assistant_id,
                        assistant_name = a.member.name,
                    }).FirstOrDefault();
                    break;
                default:
                    break;
            }

            return Json(annualOrigOrder, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Add(annual_exam exam)
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

            exam.code = DateTime.Now.ToString("yyyyMMddHHMMss"); // GetNextOrderCode(userId, "NJ");
            exam.status = 0;
            exam.review_status = -1;
            exam.creator_id = userId;
            exam.organization_id = organization_id;

            var newExam = Uof.Iannual_examService.AddEntity(exam);
            if (newExam == null)
            {
                return Json(new { success = false, message = "添加失败" }, JsonRequestBehavior.AllowGet);
            }

            switch (exam.type)
            {
                case "reg_abroad":
                    var dbRegAbroad = Uof.Ireg_abroadService.GetAll(a => a.id == exam.order_id.Value).FirstOrDefault();
                    dbRegAbroad.annual_year = DateTime.Now.Year;
                    dbRegAbroad.annual_date = DateTime.Today;

                    Uof.Ireg_abroadService.UpdateEntity(dbRegAbroad);
                    break;
                case "reg_internal":
                    var dbRegInternal = Uof.Ireg_internalService.GetAll(a => a.id == exam.order_id.Value).FirstOrDefault();
                    dbRegInternal.annual_year = DateTime.Now.Year;
                    dbRegInternal.annual_date = DateTime.Today;

                    Uof.Ireg_internalService.UpdateEntity(dbRegInternal);
                    break;
                case "audit":
                    var dbAudit = Uof.IauditService.GetAll(a => a.id == exam.order_id.Value).FirstOrDefault();
                    dbAudit.annual_year = DateTime.Now.Year;
                    Uof.IauditService.UpdateEntity(dbAudit);
                    break;
                case "patent":
                    var dbPatent = Uof.IpatentService.GetAll(a => a.id == exam.order_id.Value).FirstOrDefault();
                    dbPatent.annual_year = DateTime.Now.Year;
                    dbPatent.annual_date = DateTime.Today;

                    Uof.IpatentService.UpdateEntity(dbPatent);
                    break;
                case "trademark":
                    var dbTrademark = Uof.ItrademarkService.GetAll(a => a.id == exam.order_id.Value).FirstOrDefault();
                    dbTrademark.annual_year = DateTime.Now.Year;
                    dbTrademark.annual_date = DateTime.Today;

                    Uof.ItrademarkService.UpdateEntity(dbTrademark);
                    break;
                default:
                    break;
            }

            Uof.ItimelineService.AddEntity(new timeline()
            {
                source_id = newExam.id,
                source_name = "annual",
                title = "新建年检",
                content = string.Format("{0}新建了{1}年度年检订单, 单号{2}", arrs[3], DateTime.Now.Year, newExam.code)
            });

            return Json(new { id = newExam.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(annual_exam exam)
        {
            var dbExam = Uof.Iannual_examService.GetById(exam.id);

            if (exam.description == dbExam.description &&
                exam.date_transaction == dbExam.date_transaction &&
                exam.amount_transaction == dbExam.amount_transaction &&
                exam.rate == dbExam.rate &&
                exam.currency == dbExam.currency &&
                exam.salesman_id == dbExam.salesman_id &&
                exam.accountant_id == dbExam.accountant_id &&
                exam.assistant_id == dbExam.assistant_id
                )
            {
                return Json(new { success = true, id = dbExam.id }, JsonRequestBehavior.AllowGet);
            }

            var identityName = HttpContext.User.Identity.Name;
            var arrs = identityName.Split('|');
            if (arrs.Length == 0)
            {
                return new HttpUnauthorizedResult();
            }

            var isChangeCurrency = exam.currency != dbExam.currency || exam.rate != dbExam.rate;

            dbExam.description = exam.description;
            dbExam.date_updated = DateTime.Now;
            dbExam.date_transaction = exam.date_transaction;
            dbExam.amount_transaction = exam.amount_transaction;
            dbExam.rate = exam.rate;
            dbExam.currency = exam.currency;
            dbExam.salesman_id = exam.salesman_id;
            dbExam.accountant_id = exam.accountant_id;
            dbExam.assistant_id = exam.assistant_id;

            var r = Uof.Iannual_examService.UpdateEntity(dbExam);

            if (r)
            {
                if (isChangeCurrency)
                {
                    var list = Uof.IincomeService.GetAll(i => i.source_id == exam.id && i.source_name == "annual").ToList();
                    if (list.Count() > 0)
                    {
                        foreach (var item in list)
                        {
                            item.currency = exam.currency;
                            item.rate = exam.rate;
                        }

                        Uof.IincomeService.UpdateEntities(list);
                    }
                }

                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbExam.id,
                    source_name = "annual",
                    title = "修改年检资料",
                    content = string.Format("{0}修改了年检资料", arrs[3])
                });
            }

            return Json(new { success = r, id = dbExam.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get(int id)
        {
            var annua = Uof.Iannual_examService.GetAll(a => a.id == id).Select(a => new
            {
                id = a.id,
                code = a.code,
                customer_id = a.customer_id,
                customer_name = a.customer.name,
                customer_code = a.customer.code,
                type = a.type,
                order_code = a.order_code,
                name_cn = a.name_cn,
                name_en = a.name_en,
                date_transaction = a.date_transaction,
                amount_transaction = a.amount_transaction,
                currency = a.currency,
                rate = a.rate,
                description = a.description,
                progress = a.progress,
                salesman_id = a.salesman_id,
                salesman = a.member4.name,
                waiter_id = a.waiter_id,
                waiter_name = a.member6.name,

                accountant_id = a.accountant_id,
                accountant_name = a.member.name,

                assistant_id = a.assistant_id,
                assistant_name = a.member7.name,

                status = a.status,
                review_status = a.review_status

            }).FirstOrDefault();

            return Json(annua, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search(OrderSearchRequest request)
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

            //Expression<Func<annual_exam, bool>> condition = c => c.salesman_id == userId;
            Expression<Func<annual_exam, bool>> condition = c => true;
            var ops = arrs[4].Split(',');
            if (ops.Count() == 0)
            {
                condition = c => (c.salesman_id == userId || c.waiter_id == userId || c.creator_id == userId);
            }
            else
            {
                var hasCompany = ops.Where(o => o == "1").FirstOrDefault();
                var hasDepart = ops.Where(o => o == "2").FirstOrDefault();
                if (hasCompany == null)
                {
                    if (hasDepart == null)
                    {
                        condition = c => (c.salesman_id == userId || c.waiter_id == userId || c.creator_id == userId);
                    }
                    else
                    {
                        var ids = GetChildrenDept(deptId);
                        if (ids.Count > 0)
                        {
                            condition = c => c.organization_id == deptId;
                        }
                        else
                        {
                            condition = c => ids.Contains(c.organization_id.Value);
                        }
                    }
                }
            }


            // 客户id
            Expression<Func<annual_exam, bool>> customerQuery = c => true;
            if (request.customer_id != null && request.customer_id.Value > 0)
            {
                customerQuery = c => (c.customer_id == request.customer_id);
            }
            // 订单状态
            Expression<Func<annual_exam, bool>> statusQuery = c => true;
            if (request.status != null)
            {
                if (request.status == 2)
                {
                    statusQuery = c => (c.status == 2 || c.status == 3);
                }
                else
                {
                    statusQuery = c => (c.status == request.status.Value);
                }
            }

            // 成交开始日期
            Expression<Func<annual_exam, bool>> date1Query = c => true;
            Expression<Func<annual_exam, bool>> date2Query = c => true;
            if (request.start_time != null)
            {
                date1Query = c => (c.date_transaction >= request.start_time.Value);
            }
            // 成交结束日期
            if (request.end_time != null)
            {
                var endTime = request.end_time.Value.AddDays(1);
                date2Query = c => (c.date_transaction < endTime);
            }

            var list = Uof.Iannual_examService.GetAll(condition)
                .Where(customerQuery)
                .Where(statusQuery)
                .Where(date1Query)
                .Where(date2Query)
                .OrderByDescending(item => item.id).Select(c => new
                {
                    id = c.id,
                    code = c.code,
                    customer_id = c.customer_id,
                    customer_code = c.customer.code,
                    type = c.type,
                    customer_name = c.customer.name,
                    name_cn = c.name_cn ?? c.name_en,
                    name_en = c.name_en,
                    status = c.status,
                    review_status = c.review_status,
                    date_transaction = c.date_transaction,
                    amount_transaction = c.amount_transaction,
                    amount_income = 0,
                    amount_unreceive = 0,
                    progress = c.progress,
                    salesman_id = c.salesman_id,
                    salesman_name = c.member4.name,

                    assistant_id = c.assistant_id,
                    assistant_name = c.member7.name,

                    finance_review_moment = c.finance_review_moment,
                    submit_review_moment = c.submit_review_moment

                }).ToPagedList(request.index, request.size).ToList();

            var totalRecord = Uof.Iannual_examService.GetAll(condition).Count();

            var totalPages = 0;
            if (totalRecord > 0)
            {
                totalPages = (totalRecord + request.size - 1) / request.size;
            }
            var page = new
            {
                current_index = request.index,
                current_size = request.size,
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

        public ActionResult GetView(int id)
        {
            var annua = Uof.Iannual_examService.GetAll(a => a.id == id).Select(a => new
            {
                id = a.id,
                code = a.code,
                customer_id = a.customer_id,
                customer_name = a.customer.name,
                customer_code = a.customer.code,
                type = a.type,
                order_code = a.order_code,
                name_cn = a.name_cn,
                name_en = a.name_en,
                date_transaction = a.date_transaction,
                amount_transaction = a.amount_transaction,
                currency = a.currency,
                rate = a.rate,
                description = a.description,
                progress = a.progress,
                salesman_id = a.salesman_id,
                salesman = a.member4.name,
                waiter_id = a.waiter_id,
                waiter_name = a.member6.name,
                accountant_id = a.accountant_id,
                accountant_name = a.member.name,

                assistant_id = a.assistant_id,
                assistant_name = a.member7.name,

                date_finish = a.date_finish,

                status = a.status,
                review_status = a.review_status,
                finance_review_moment = a.finance_review_moment,
                submit_review_moment = a.submit_review_moment

            }).FirstOrDefault();

            var list = Uof.IincomeService.GetAll(i => i.source_id == annua.id && i.source_name == "annual").Select(i => new {
                id = i.id,
                customer_id = i.customer_id,
                source_id = i.source_id,
                source_name = i.source_name,
                payer = i.payer,
                pay_way = i.pay_way,
                account = i.account,
                amount = i.amount,
                date_pay = i.date_pay,
                attachment_url = i.attachment_url,
                description = i.description,
                bank = i.bank
            }).ToList();

            var total = 0f;
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    total += item.amount.Value;
                }
            }

            var balance = annua.amount_transaction - total;
            var incomes = new
            {
                items = list,
                total = total,
                balance = balance,

                rate = annua.rate,
                local_amount = annua.amount_transaction * annua.rate,
                local_total = total * annua.rate,
                local_balance = balance * annua.rate
            };

            return Json(new { order = annua, incomes = incomes }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Submit(int id)
        {
            var dbAnnual = Uof.Iannual_examService.GetById(id);
            if (dbAnnual == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }

            dbAnnual.status = 1;
            dbAnnual.review_status = -1;
            dbAnnual.date_updated = DateTime.Now;

            var r = Uof.Iannual_examService.UpdateEntity(dbAnnual);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbAnnual.id,
                    source_name = "annual",
                    title = "提交审核",
                    content = string.Format("提交给财务审核")
                });

                var ids = GetFinanceMembers();
                if (ids.Count() > 0)
                {
                    var waitdeals = new List<waitdeal>();
                    foreach (var item in ids)
                    {
                        waitdeals.Add(new waitdeal
                        {
                            source = "annual",
                            source_id = dbAnnual.id,
                            user_id = item,
                            router = "annual_view",
                            content = "您有年检订单需要财务审核",
                            read_status = 0
                        });
                    }

                    Uof.IwaitdealService.AddEntities(waitdeals);
                }
            }
            return Json(new { success = r, message = r ? "" : "更新失败" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PassAudit(int id)
        {
            var u = HttpContext.User.Identity.IsAuthenticated;
            if (!u)
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

            var dbAnnual = Uof.Iannual_examService.GetById(id);
            if (dbAnnual == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }

            var t = "";
            var waitdeals = new List<waitdeal>();
            if (dbAnnual.status == 1)
            {
                dbAnnual.status = 2;
                dbAnnual.review_status = 1;
                dbAnnual.finance_reviewer_id = userId;
                dbAnnual.finance_review_date = DateTime.Now;
                dbAnnual.finance_review_moment = "";

                t = "财务审核";
                waitdeals.Add(new waitdeal
                {
                    source = "annual",
                    source_id = dbAnnual.id,
                    user_id = dbAnnual.salesman_id,
                    router = "annual_view",
                    content = "您的年检订单已通过财务审核",
                    read_status = 0
                });
                if (dbAnnual.assistant_id != null && dbAnnual.assistant_id != dbAnnual.salesman_id)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "annual",
                        source_id = dbAnnual.id,
                        user_id = dbAnnual.assistant_id,
                        router = "annual_view",
                        content = "您的年检订单已通过财务审核",
                        read_status = 0
                    });
                }

                var ids = GetSubmitMembers();
                if (ids.Count() > 0)
                {
                    foreach (var item in ids)
                    {
                        waitdeals.Add(new waitdeal
                        {
                            source = "annual",
                            source_id = dbAnnual.id,
                            user_id = item,
                            router = "annual_view",
                            content = "您有年检订单需要提交审核",
                            read_status = 0
                        });
                    }
                }
            }
            else
            {
                dbAnnual.status = 3;
                dbAnnual.review_status = 1;
                dbAnnual.submit_reviewer_id = userId;
                dbAnnual.submit_review_date = DateTime.Now;
                dbAnnual.submit_review_moment = "";

                t = "提交的审核";
                waitdeals.Add(new waitdeal
                {
                    source = "annual",
                    source_id = dbAnnual.id,
                    user_id = dbAnnual.salesman_id,
                    router = "annual_view",
                    content = "您的年检订单已通过提交审核",
                    read_status = 0
                });

                if (dbAnnual.assistant_id != null && dbAnnual.assistant_id != dbAnnual.salesman_id)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "annual",
                        source_id = dbAnnual.id,
                        user_id = dbAnnual.assistant_id,
                        router = "annual_view",
                        content = "您的年检订单已通过提交审核",
                        read_status = 0
                    });
                }
            }

            dbAnnual.date_updated = DateTime.Now;

            var r = Uof.Iannual_examService.UpdateEntity(dbAnnual);

            if (r)
            {
                Uof.IwaitdealService.AddEntities(waitdeals);

                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbAnnual.id,
                    source_name = "annual",
                    title = "通过审核",
                    content = string.Format("{0}通过了{1}", arrs[3], t)
                });
            }
            return Json(new { success = r, message = r ? "" : "审核失败" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RefuseAudit(int id, string description)
        {
            var u = HttpContext.User.Identity.IsAuthenticated;
            if (!u)
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

            var dbAnnual = Uof.Iannual_examService.GetById(id);
            if (dbAnnual == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }
            var t = "";
            var waitdeals = new List<waitdeal>();
            if (dbAnnual.status == 1)
            {
                dbAnnual.status = 0;
                dbAnnual.review_status = 0;
                dbAnnual.finance_reviewer_id = userId;
                dbAnnual.finance_review_date = DateTime.Now;
                dbAnnual.finance_review_moment = description;

                t = "驳回了财务审核";

                waitdeals.Add(new waitdeal
                {
                    source = "annual",
                    source_id = dbAnnual.id,
                    user_id = dbAnnual.salesman_id,
                    router = "annual_view",
                    content = "您的年检订单未通过财务审核",
                    read_status = 0
                });

                if (dbAnnual.assistant_id != null && dbAnnual.assistant_id != dbAnnual.salesman_id)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "annual",
                        source_id = dbAnnual.id,
                        user_id = dbAnnual.assistant_id,
                        router = "annual_view",
                        content = "您的年检订单未通过财务审核",
                        read_status = 0
                    });
                }
            }
            else
            {
                dbAnnual.status = 0;
                dbAnnual.review_status = 0;
                dbAnnual.submit_reviewer_id = userId;
                dbAnnual.submit_review_date = DateTime.Now;
                dbAnnual.submit_review_moment = description;

                t = "驳回了提交的审核";

                waitdeals.Add(new waitdeal
                {
                    source = "annual",
                    source_id = dbAnnual.id,
                    user_id = dbAnnual.salesman_id,
                    router = "annual_view",
                    content = "您的年检订单未通过提交审核",
                    read_status = 0
                });

                if (dbAnnual.assistant_id != null && dbAnnual.assistant_id != dbAnnual.salesman_id)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "annual",
                        source_id = dbAnnual.id,
                        user_id = dbAnnual.assistant_id,
                        router = "annual_view",
                        content = "您的年检订单未通过提交审核",
                        read_status = 0
                    });
                }
            }

            dbAnnual.date_updated = DateTime.Now;

            var r = Uof.Iannual_examService.UpdateEntity(dbAnnual);

            if (r)
            {
                Uof.IwaitdealService.AddEntities(waitdeals);

                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbAnnual.id,
                    source_name = "annual",
                    title = "驳回审核",
                    content = string.Format("{0}{1}, 驳回理由: {2}", arrs[3], t, description)
                });
            }

            return Json(new { success = r, message = r ? "" : "审核失败" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Finish(int id, DateTime date_finish)
        {
            var u = HttpContext.User.Identity.IsAuthenticated;
            if (!u)
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

            var dbAnnual = Uof.Iannual_examService.GetById(id);
            if (dbAnnual == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }
            dbAnnual.status = 4;
            dbAnnual.date_updated = DateTime.Now;
            dbAnnual.date_finish = date_finish;
            dbAnnual.progress = "已完成";
            var r = Uof.Iannual_examService.UpdateEntity(dbAnnual);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbAnnual.id,
                    source_name = "annual",
                    title = "完成订单",
                    content = string.Format("{0}完成了订单，完成日期为：{1}", arrs[3], date_finish.ToString("yyyy-MM-dd"))
                });

                var waitdeals = new List<waitdeal>();
                waitdeals.Add(new waitdeal
                {
                    source = "annual",
                    source_id = dbAnnual.id,
                    user_id = dbAnnual.salesman_id,
                    router = "annual_view",
                    content = "您的年检订单已完成",
                    read_status = 0
                });
                if (dbAnnual.assistant_id != null && dbAnnual.assistant_id != dbAnnual.salesman_id)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "annual",
                        source_id = dbAnnual.id,
                        user_id = dbAnnual.assistant_id,
                        router = "annual_view",
                        content = "您的年检订单已完成",
                        read_status = 0
                    });
                }
                Uof.IwaitdealService.AddEntities(waitdeals);
            }

            return Json(new { success = r, message = r ? "" : "保存失败" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProgress(int id)
        {
            var p = Uof.Iannual_examService.GetAll(r => r.id == id).Select(r => new
            {
                id = r.id,
                customer_id = r.customer_id,
                is_done = r.status == 4 ? 1 : 0,
                date_finish = r.date_finish,
                progress = r.progress
            }).FirstOrDefault();

            return Json(p, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateProgress(ProgressRequest request)
        {
            var u = HttpContext.User.Identity.IsAuthenticated;
            if (!u)
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

            var dbAnnual = Uof.Iannual_examService.GetById(request.id);
            if (dbAnnual == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }

            if (request.progress_type != "p")
            {
                dbAnnual.status = 4;
                dbAnnual.date_updated = DateTime.Now;
                if (dbAnnual.date_finish == null)
                {
                    dbAnnual.date_finish = request.date_finish ?? DateTime.Today;
                }
            }
            else
            {
                if (dbAnnual.progress == request.progress && dbAnnual.date_finish == request.date_finish)
                {
                    return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);
                }

                dbAnnual.status = 4;
                dbAnnual.date_finish = request.date_finish;
                dbAnnual.progress = request.progress;
            }

            var r = Uof.Iannual_examService.UpdateEntity(dbAnnual);

            if (r)
            {
                if (request.progress_type != "p")
                {
                    Uof.ItimelineService.AddEntity(new timeline()
                    {
                        source_id = dbAnnual.id,
                        source_name = "annual",
                        title = "完善了注册资料",
                        content = string.Format("{0}完善了注册资料", arrs[3])
                    });

                    Uof.IwaitdealService.AddEntity(new waitdeal
                    {
                        source = "annual",
                        source_id = dbAnnual.id,
                        user_id = dbAnnual.salesman_id,
                        router = "annual_view",
                        content = "您的年检订单已完成",
                        read_status = 0
                    });
                }
                else
                {
                    Uof.ItimelineService.AddEntity(new timeline()
                    {
                        source_id = dbAnnual.id,
                        source_name = "annual",
                        title = "更新了订单进度",
                        content = string.Format("{0}更新了进度: {1} 预计完成日期 {2}", arrs[3], dbAnnual.progress, dbAnnual.date_finish.Value.ToString("yyyy-MM-dd"))
                    });

                    var waitdeals = new List<waitdeal>();
                    waitdeals.Add(new waitdeal
                    {
                        source = "annual",
                        source_id = dbAnnual.id,
                        user_id = dbAnnual.salesman_id,
                        router = "annual_view",
                        content = "您的年检订单更新了进度",
                        read_status = 0
                    });

                    if (dbAnnual.assistant_id != null && dbAnnual.assistant_id != dbAnnual.salesman_id)
                    {
                        waitdeals.Add(new waitdeal
                        {
                            source = "annual",
                            source_id = dbAnnual.id,
                            user_id = dbAnnual.assistant_id,
                            router = "annual_view",
                            content = "您的年检订单更新了进度",
                            read_status = 0
                        });
                    }
                    Uof.IwaitdealService.AddEntities(waitdeals);
                }
            }

            return Json(new { success = r, message = r ? "" : "更新失败" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DoneThisYear(string orderType, int orderId)
        {
            switch (orderType)
            {
                case "reg_abroad":
                    var a = Uof.Ireg_abroadService.GetAll(r => r.id == orderId).FirstOrDefault();
                    a.annual_year = DateTime.Today.Year;
                    Uof.Ireg_abroadService.UpdateEntity(a);
                    break;
                case "reg_internal":
                    var b = Uof.Ireg_internalService.GetAll(r => r.id == orderId).FirstOrDefault();
                    b.annual_year = DateTime.Today.Year;
                    Uof.Ireg_internalService.UpdateEntity(b);
                    break;
                case "trademark":
                    var c = Uof.ItrademarkService.GetAll(r => r.id == orderId).FirstOrDefault();
                    c.annual_year = DateTime.Today.Year;
                    Uof.ItrademarkService.UpdateEntity(c);
                    break;
                case "patent":
                    var d = Uof.IpatentService.GetAll(r => r.id == orderId).FirstOrDefault();
                    d.annual_year = DateTime.Today.Year;
                    Uof.IpatentService.UpdateEntity(d);
                    break;
                default:
                    break;
            }

            return SuccessResult;
        }
    }
}