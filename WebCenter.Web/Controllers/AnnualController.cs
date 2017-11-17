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
                    var internals = Uof.Ireg_internalService.GetAll(i => i.customer_id == customer_id).Select(i => new
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
                    var abroads = Uof.Ireg_abroadService.GetAll(i => i.customer_id == customer_id).Select(i => new
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

        public ActionResult Warning(int? customer_id, int? waiter_id, int? salesman_id, string name, string area, int index = 1)
        {
            waiter_id = null;
            salesman_id = null;

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

            var abroadWarningMonth = GetSettingByKey("ABROAD_WARN_MONTH");
            var abroadMonth = 2;
            int.TryParse(abroadWarningMonth.value, out abroadMonth);

            var items = new List<AnnualWarning>();
            var nowYear = DateTime.Now.Year;
            var Month1 = DateTime.Now.AddMonths(abroadMonth).Month; // DateTime.Now.AddMonths(-13).Month;
            var Month2 = DateTime.Now.Month;
            var Month3 = DateTime.Now.AddMonths(1).Month;

            #region 境外注册
            Expression<Func<reg_abroad, bool>> condition1 = c => c.status == 4 && c.date_setup != null && c.order_status == 0 && c.need_annual == 1 && 
            ((c.annual_date == null && c.annual_year.Value < nowYear && (Month1 == (c.date_setup.Value.Month) || Month2 >= (c.date_setup.Value.Month) || Month3 == (c.date_setup.Value.Month)) && nowYear >= c.date_setup.Value.Year) ||
            (c.annual_date != null && c.annual_year.Value < nowYear && (Month1 == (c.date_setup.Value.Month) || Month2 >= (c.date_setup.Value.Month) || Month3 == (c.date_setup.Value.Month)) && nowYear >= c.annual_date.Value.Year) ||
            (c.annual_year == null && nowYear >= c.date_setup.Value.Year) ||
            (c.annual_year != null && c.annual_year == nowYear && (Month1 == (c.date_setup.Value.Month) || Month3 == (c.date_setup.Value.Month)) && nowYear >= c.date_setup.Value.Year) ||
            (c.is_annual == 1 && ((c.annual_year == null) || (c.annual_year != null && c.annual_year.Value < nowYear))));
            Expression<Func<reg_abroad, bool>> customerQuery1 = c => true;
            if (customer_id != null && customer_id.Value > 0)
            {
                customerQuery1 = c => (c.customer_id == customer_id);
            }

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
                nameQuery1 = c => (c.name_cn.Contains(name) || c.name_en.Contains(name) || c.code.Contains(name));
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
                    //saleman = a.member4.name,

                    //salesman_id = a.customer.salesman_id,
                    saleman = a.customer.member1.name ?? "",

                    waiter = a.member6.name,
                    assistant_name = a.member7.name,
                    submit_review_date = a.submit_review_date,
                    date_finish = a.date_finish,
                    date_setup = a.date_setup,
                    annual_date = a.annual_date,
                    annual_year = a.annual_year,
                    month = DateTime.Today.Month - a.date_setup.Value.Month,

                    date_last = a.date_last,
                    title_last = a.title_last,
                    date_wait = a.date_wait,
                    region = a.region,
                }).ToList();

            if (abroads.Count() > 0)
            {
                var newList = abroads.Where(a =>
                 //(a.annual_date == null && (a.date_setup.Value.AddMonths(12 - abroadMonth) <= DateTime.Today || (new DateTime(DateTime.Now.Year, a.date_setup.Value.Month, 1)).AddMonths(-abroadMonth) <= DateTime.Today)) ||
                (a.annual_date == null && (a.date_setup.Value.AddMonths(12 - abroadMonth) <= DateTime.Today)) ||
                (a.annual_date != null && a.annual_date.Value.AddMonths(12 - abroadMonth) <= DateTime.Today)).ToList();

                //var _newList = newList.Where(a => (a.start_annual != null && a.start_annual <= nowYear) || a.start_annual == null).ToList();

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

                Expression<Func<reg_internal, bool>> condition2 = c => c.status == 4 && c.order_status == 0 &&
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
                    nameQuery2 = c => (c.name_cn.Contains(name) || c.code.Contains(name));
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
                        //saleman = a.member5.name,                        
                        saleman = a.customer.member1.name ?? "",
                        waiter = a.member7.name,
                        assistant_name = a.member.name,
                        submit_review_date = a.submit_review_date,
                        date_finish = a.date_finish,
                        date_setup = a.date_setup,
                        annual_year = a.annual_year,
                        month = DateTime.Today.Month - a.date_setup.Value.Month,

                        date_last = a.date_last,
                        title_last = a.title_last,
                        date_wait = a.date_wait,
                        region = "",
                    }).ToList();

                if (internas.Count() > 0)
                {
                    items.AddRange(internas);
                }
            }
            #endregion

            #region 商标注册            
            Expression<Func<trademark, bool>> condition3 = c => c.status == 4 && c.order_status == 0 && c.date_regit != null && c.exten_period != null;

            Expression<Func<trademark, bool>> customerQuery3 = c => true;
            if (customer_id != null && customer_id.Value > 0)
            {
                customerQuery3 = c => (c.customer_id == customer_id);
            }
            
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
                nameQuery3 = c => (c.name.Contains(name) || c.code.Contains(name));
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
                    //saleman = a.member4.name,
                    saleman = a.customer.member1.name ?? "",

                    waiter = a.member6.name,
                    assistant_name = a.member.name,
                    submit_review_date = a.submit_review_date,
                    date_finish = a.date_finish,
                    date_setup = a.date_regit,
                    annual_year = a.annual_year,
                    exten_period = a.exten_period,
                    //month = (a.date_regit != null) ? (DateTime.Today.Month - a.date_regit.Value.Month) : 0,

                    date_last = a.date_last,
                    title_last = a.title_last,
                    date_wait = a.date_wait,
                    region = a.region,
                }).ToList();

            if (trademarks.Count() > 0)
            {
                var trademarkWarningMonth = GetSettingByKey("TRADEMARK_WARN_MONTH");
                var tradeWarningMonth = 6;
                int.TryParse(trademarkWarningMonth.value, out tradeWarningMonth);
                
                var _trademarks = trademarks.Where(t => (t.annual_year == null && t.date_setup.Value.Year < DateTime.Today.Year && t.date_setup.Value.AddYears(t.exten_period.Value).AddMonths(-tradeWarningMonth) <= DateTime.Today) ||
                (t.annual_year != null && new DateTime(t.annual_year.Value, t.date_setup.Value.Month, 1).AddYears(t.exten_period.Value).AddMonths(-tradeWarningMonth) <= DateTime.Today)).ToList();
                if (_trademarks.Count() > 0)
                {
                    items.AddRange(_trademarks);
                }               
            }
            #endregion

            #region 专利注册
            var patentPeriodSetting = Uof.IsettingService.GetAll(s => s.name == "PATENT_PERIOD").Select(s => s.value).FirstOrDefault();
            int patentPeriod = 0;
            int.TryParse(patentPeriodSetting, out patentPeriod);

            Expression<Func<patent, bool>> condition4 = c => c.status == 4 && c.order_status == 0 &&
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
                waiterQuery4 = c => c.waiter_id == waiter_id;
            }
            Expression<Func<patent, bool>> salesmanQuery4 = c => true;
            if (salesman_id != null)
            {
                salesmanQuery4 = c => c.salesman_id == salesman_id;
            }
            Expression<Func<patent, bool>> nameQuery4 = c => true;
            if (!string.IsNullOrEmpty(name))
            {
                nameQuery4 = c => (c.name.Contains(name) || c.code.Contains(name));
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
                    //saleman = a.member4.name,
                    saleman = a.customer.member1.name ?? "",

                    waiter = a.member6.name,
                    assistant_name = a.member.name,
                    submit_review_date = a.submit_review_date,
                    date_finish = a.date_finish,
                    date_setup = a.date_regit,
                    annual_year = a.annual_year,
                    month = (a.date_regit != null) ? (DateTime.Today.Month - a.date_regit.Value.Month) : 0,

                    date_last = a.date_last,
                    title_last = a.title_last,
                    date_wait = a.date_wait,
                    region = "",
                }).ToList();

            if (patents.Count() > 0)
            {
                items.AddRange(patents);
            }
            #endregion

            

            if (items.Count > 0)
            {
                foreach (var item in items)
                {
                    item.setup_day = item.date_setup.Value.ToString("MM-dd");
                }
            }

            var size = 50;
            //items = items.OrderBy(i => i.setup_day).ToList();
            var list = items.OrderBy(item => item.setup_day).Skip((index - 1) * size).Take(size).ToList();

            var totalRecord = items.Count();
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
                        salesman_id = a.customer.salesman_id,
                        salesman = a.customer.member1.name,
                        assistant_id = a.assistant_id,
                        assistant_name = a.member7.name,
                        region = a.region,

                        waiter_id = a.waiter_id,
                        waiter_name = a.member6.name,
                        order_owner = a.member.name,

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
                        salesman_id = a.customer.salesman_id,
                        salesman = a.customer.member1.name,
                        assistant_id = a.assistant_id,
                        assistant_name = a.member.name,

                        waiter_id = a.waiter_id,
                        waiter_name = a.member7.name,

                        order_owner = a.member1.name,
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
                        date_setup = a.date_regit,

                        salesman_id = a.customer.salesman_id,
                        salesman = a.customer.member1.name,

                        assistant_id = a.assistant_id,
                        assistant_name = a.member.name,
                        region = a.region,

                        waiter_id = a.waiter_id,
                        waiter_name = a.member6.name,

                        order_owner = a.member1.name,
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
                        date_setup = a.date_regit,

                        salesman_id = a.customer.salesman_id,
                        salesman = a.customer.member1.name,

                        assistant_id = a.assistant_id,
                        assistant_name = a.member.name,

                        waiter_id = a.waiter_id,
                        waiter_name = a.member6.name,

                        order_owner = a.member1.name,
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

                    dbRegAbroad.annual_year = exam.start_annual;
                    if (dbRegAbroad.date_setup != null)
                    {
                        dbRegAbroad.annual_date = new DateTime(exam.start_annual.Value, dbRegAbroad.date_setup.Value.Month, dbRegAbroad.date_setup.Value.Day);
                    } else
                    {
                        dbRegAbroad.annual_date = DateTime.Today;
                    }

                    if (dbRegAbroad.is_annual == 1)
                    {
                        dbRegAbroad.annual_id = newExam.id;
                    }
                    
                    Uof.Ireg_abroadService.UpdateEntity(dbRegAbroad);
                    break;
                case "reg_internal":
                    var dbRegInternal = Uof.Ireg_internalService.GetAll(a => a.id == exam.order_id.Value).FirstOrDefault();
                    dbRegInternal.annual_year = exam.start_annual;

                    if (dbRegInternal.date_setup != null)
                    {
                        dbRegInternal.annual_date = new DateTime(exam.start_annual.Value, dbRegInternal.date_setup.Value.Month, dbRegInternal.date_setup.Value.Day);
                    }
                    else
                    {
                        dbRegInternal.annual_date = DateTime.Today;
                    }

                    Uof.Ireg_internalService.UpdateEntity(dbRegInternal);
                    break;
                case "audit":
                    var dbAudit = Uof.IauditService.GetAll(a => a.id == exam.order_id.Value).FirstOrDefault();
                    dbAudit.annual_year = exam.start_annual;
                    Uof.IauditService.UpdateEntity(dbAudit);
                    break;
                case "patent":
                    var dbPatent = Uof.IpatentService.GetAll(a => a.id == exam.order_id.Value).FirstOrDefault();
                    dbPatent.annual_year = exam.start_annual;
                    dbPatent.annual_date = DateTime.Today;
                    if (dbPatent.date_regit != null)
                    {
                        dbPatent.annual_date = new DateTime(exam.start_annual.Value, dbPatent.date_regit.Value.Month, dbPatent.date_regit.Value.Day);
                    }
                    else
                    {
                        dbPatent.annual_date = DateTime.Today;
                    }

                    Uof.IpatentService.UpdateEntity(dbPatent);
                    break;
                case "trademark":
                    var dbTrademark = Uof.ItrademarkService.GetAll(a => a.id == exam.order_id.Value).FirstOrDefault();
                    dbTrademark.annual_year = exam.start_annual;
                    //dbTrademark.annual_date = DateTime.Today;

                    if (dbTrademark.date_regit != null)
                    {
                        dbTrademark.annual_date = new DateTime(exam.start_annual.Value, dbTrademark.date_regit.Value.Month, dbTrademark.date_regit.Value.Day);
                    }
                    else
                    {
                        dbTrademark.annual_date = DateTime.Today;
                    }

                    Uof.ItrademarkService.UpdateEntity(dbTrademark);
                    break;
                default:
                    break;
            }

            Uof.ItimelineService.AddEntity(new timeline()
            {
                source_id = exam.order_id,
                source_name = exam.type,
                title = "新建年检",
                is_system = 1,
                content = string.Format("{0}新建了{1}年度年检订单, 年检单号{2}", arrs[3], exam.start_annual, newExam.code)
            });

            return Json(new { id = newExam.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(annual_exam exam)
        {
            var dbExam = Uof.Iannual_examService.GetById(exam.id);

            //if (exam.description == dbExam.description &&
            //    exam.date_transaction == dbExam.date_transaction &&
            //    exam.amount_transaction == dbExam.amount_transaction &&
            //    exam.rate == dbExam.rate &&
            //    exam.currency == dbExam.currency &&
            //    exam.salesman_id == dbExam.salesman_id &&
            //    exam.accountant_id == dbExam.accountant_id &&
            //    exam.assistant_id == dbExam.assistant_id
            //    )
            //{
            //    return Json(new { success = true, id = dbExam.id }, JsonRequestBehavior.AllowGet);
            //}

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

            dbExam.start_annual = exam.start_annual;

            var r = Uof.Iannual_examService.UpdateEntity(dbExam);

            if (r)
            {
                switch (dbExam.type)
                {
                    case "reg_abroad":
                        var dbRegAbroad = Uof.Ireg_abroadService.GetAll(a => a.id == dbExam.order_id.Value).FirstOrDefault();

                        dbRegAbroad.annual_year = dbExam.start_annual;
                        if (dbRegAbroad.date_setup != null)
                        {
                            dbRegAbroad.annual_date = new DateTime(dbExam.start_annual.Value, dbRegAbroad.date_setup.Value.Month, dbRegAbroad.date_setup.Value.Day);
                        }
                        else
                        {
                            dbRegAbroad.annual_date = DateTime.Today;
                        }

                        Uof.Ireg_abroadService.UpdateEntity(dbRegAbroad);
                        break;
                    case "reg_internal":
                        var dbRegInternal = Uof.Ireg_internalService.GetAll(a => a.id == dbExam.order_id.Value).FirstOrDefault();
                        dbRegInternal.annual_year = exam.start_annual;

                        if (dbRegInternal.date_setup != null)
                        {
                            dbRegInternal.annual_date = new DateTime(exam.start_annual.Value, dbRegInternal.date_setup.Value.Month, dbRegInternal.date_setup.Value.Day);
                        }
                        else
                        {
                            dbRegInternal.annual_date = DateTime.Today;
                        }

                        Uof.Ireg_internalService.UpdateEntity(dbRegInternal);
                        break;
                    case "audit":
                        var dbAudit = Uof.IauditService.GetAll(a => a.id == dbExam.order_id.Value).FirstOrDefault();
                        dbAudit.annual_year = exam.start_annual;
                        Uof.IauditService.UpdateEntity(dbAudit);
                        break;
                    case "patent":
                        var dbPatent = Uof.IpatentService.GetAll(a => a.id == dbExam.order_id.Value).FirstOrDefault();
                        dbPatent.annual_year = exam.start_annual;
                        dbPatent.annual_date = DateTime.Today;
                        if (dbPatent.date_regit != null)
                        {
                            dbPatent.annual_date = new DateTime(exam.start_annual.Value, dbPatent.date_regit.Value.Month, dbPatent.date_regit.Value.Day);
                        }
                        else
                        {
                            dbPatent.annual_date = DateTime.Today;
                        }

                        Uof.IpatentService.UpdateEntity(dbPatent);
                        break;
                    case "trademark":
                        var dbTrademark = Uof.ItrademarkService.GetAll(a => a.id == dbExam.order_id.Value).FirstOrDefault();
                        dbTrademark.annual_year = exam.start_annual;
                        //dbTrademark.annual_date = DateTime.Today;

                        if (dbTrademark.date_regit != null)
                        {
                            dbTrademark.annual_date = new DateTime(exam.start_annual.Value, dbTrademark.date_regit.Value.Month, dbTrademark.date_regit.Value.Day);
                        }
                        else
                        {
                            dbTrademark.annual_date = DateTime.Today;
                        }

                        Uof.ItrademarkService.UpdateEntity(dbTrademark);
                        break;
                    default:
                        break;
                }

                //if (isChangeCurrency)
                //{
                //    var list = Uof.IincomeService.GetAll(i => i.source_id == exam.id && i.source_name == "annual").ToList();
                //    if (list.Count() > 0)
                //    {
                //        foreach (var item in list)
                //        {
                //            item.currency = exam.currency;
                //            item.rate = exam.rate;
                //        }

                //        Uof.IincomeService.UpdateEntities(list);
                //    }
                //}

                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbExam.order_id,
                    source_name = dbExam.type,
                    title = "修改年检资料",
                    is_system = 1,
                    content = string.Format("{0}修改了年检资料", arrs[3])
                });
            }

            return Json(new { success = r, id = dbExam.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get(int id)
        {
            var annua = Uof.Iannual_examService.GetAll(a => a.id == id).Select(a => new AnnualEntity
            {
                id = a.id,
                code = a.code,
                customer_id = a.customer_id,
                order_id = a.order_id,
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
                review_status = a.review_status,
                start_annual = a.start_annual,

            }).FirstOrDefault();

            annua.order_code = GetOrderCode(annua.type, annua.order_id.Value);

            switch (annua.type)
            {
                case "reg_abroad":
                    var abroad = Uof.Ireg_abroadService.GetAll(a => a.code == annua.order_code).FirstOrDefault();
                    if (abroad != null)
                    {
                        annua.order_owner = abroad.member.name;
                        annua.salesman = abroad.customer.member1.name;
                    }
                    break;
                case "reg_internal":
                    var dbinternal = Uof.Ireg_internalService.GetAll(a => a.code == annua.order_code).FirstOrDefault();
                    if (dbinternal != null)
                    {
                        annua.name_cn = dbinternal.name_cn;

                        annua.order_owner = dbinternal.member.name;
                        annua.salesman = dbinternal.customer.member1.name;
                    }
                    break;
                case "trademark":
                    var dbtrademark = Uof.ItrademarkService.GetAll(a => a.code == annua.order_code).FirstOrDefault();
                    if (dbtrademark != null)
                    {
                        annua.order_owner = dbtrademark.member.name;
                        annua.salesman = dbtrademark.customer.member1.name;
                    }
                    break;
                case "patent":
                    var dbpatent = Uof.IpatentService.GetAll(a => a.code == annua.order_code).FirstOrDefault();
                    if (dbpatent != null)
                    {
                        annua.order_owner = dbpatent.member.name;
                        annua.salesman = dbpatent.customer.member1.name;
                    }
                    break;
                default:
                    break;
            }

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
                condition = c => (c.salesman_id == userId || c.waiter_id == userId || c.creator_id == userId || c.creator_id == userId);
            }
            else
            {
                var hasCompany = ops.Where(o => o == "1").FirstOrDefault();
                var hasDepart = ops.Where(o => o == "2").FirstOrDefault();
                if (hasCompany == null)
                {
                    if (hasDepart == null)
                    {
                        condition = c => (c.salesman_id == userId || c.waiter_id == userId || c.creator_id == userId || c.creator_id == userId);
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
            //Expression<Func<annual_exam, bool>> customerQuery = c => true;
            //if (request.customer_id != null && request.customer_id.Value > 0)
            //{
            //    customerQuery = c => (c.customer_id == request.customer_id);
            //}
            // 订单状态
            //Expression<Func<annual_exam, bool>> statusQuery = c => true;
            //if (request.status != null)
            //{
            //    if (request.status == 2)
            //    {
            //        statusQuery = c => (c.status == 2 || c.status == 3);
            //    }
            //    else
            //    {
            //        statusQuery = c => (c.status == request.status.Value);
            //    }
            //}

            // 成交开始日期
            //Expression<Func<annual_exam, bool>> date1Query = c => true;
            //Expression<Func<annual_exam, bool>> date2Query = c => true;
            //if (request.start_time != null)
            //{
            //    date1Query = c => (c.date_transaction >= request.start_time.Value);
            //}
            //// 成交结束日期
            //if (request.end_time != null)
            //{
            //    var endTime = request.end_time.Value.AddDays(1);
            //    date2Query = c => (c.date_transaction < endTime);
            //}

            Expression<Func<annual_exam, bool>> nameQuery = c => true;
            if (!string.IsNullOrEmpty(request.name))
            {
                nameQuery = c => (c.name_cn.Contains(request.name) || c.name_en.Contains(request.name) || c.order_code.Contains(request.name));
            }

            Expression<Func<annual_exam, bool>> areaQuery = c => true;
            if (!string.IsNullOrEmpty(request.area))
            {
                areaQuery = c => c.order_code.Contains(request.area);
            }

            Expression<Func<annual_exam, bool>> typeQuery = c => true;
            if (!string.IsNullOrEmpty(request.order_type))
            {
                typeQuery = c => c.type == request.order_type;
            }

            var list = Uof.Iannual_examService.GetAll(condition)
                .Where(nameQuery)
                .Where(areaQuery)
                .Where(typeQuery)
                .OrderByDescending(item => item.id).Select(c => new AnnualExamEntity
                {
                    id = c.id,
                    code = c.order_code,
                    customer_id = c.customer_id,
                    order_id = c.order_id,
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
                    submit_review_moment = c.submit_review_moment,
                    receipt_no = "",

                }).ToPagedList(request.index, request.size).ToList();

            var totalRecord = Uof.Iannual_examService
                .GetAll(condition)
                .Where(nameQuery)
                .Where(areaQuery)
                .Where(typeQuery)
                .Count();

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

            if (list.Count() > 0)
            {
                foreach (var item in list)
                {
                    var dbReceipt = Uof.IreceiptService.GetAll(a => a.order_id == item.id && a.order_source == "annual").FirstOrDefault();
                    if (dbReceipt != null)
                    {
                        item.receipt_no = dbReceipt.date_created.Value.ToString("yyyyMMdd") + dbReceipt.code;
                    }

                    if (item.type == "reg_abroad")
                    {
                        var dbAbroad = Uof.Ireg_abroadService.GetAll(a => a.id == item.order_id).FirstOrDefault();
                        if (dbAbroad != null)
                        {
                            item.region = dbAbroad.region;
                        }
                    }
                    if (item.type == "trademark")
                    {
                        var dbTrademark = Uof.ItrademarkService.GetAll(a => a.id == item.order_id).FirstOrDefault();
                        if (dbTrademark != null)
                        {
                            item.region = dbTrademark.region;
                        }
                    }
                }                
            }

            var result = new
            {
                page = page,
                items = list
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetView(int id)
        {
            var annua = Uof.Iannual_examService.GetAll(a => a.id == id).Select(a => new AnnualEntity
            {
                id = a.id,
                code = a.code,
                customer_id = a.customer_id,
                order_id = a.order_id,
                customer_name = a.customer.name,
                customer_code = a.customer.code,
                type = a.type,
                order_code = a.order_code,
                name_cn = a.name_cn,
                name_en = a.name_en,
                date_transaction = a.date_transaction,
                amount_transaction = a.amount_transaction,
                currency = a.currency,
                rate = a.rate ?? 1,
                description = a.description,
                progress = a.progress,

                //salesman_id = a.salesman_id,
                //salesman = a.member4.name,

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
                submit_review_moment = a.submit_review_moment,
                start_annual = a.start_annual,

            }).FirstOrDefault();

            annua.order_code = GetOrderCode(annua.type, annua.order_id.Value);

            switch (annua.type)
            {
                case "reg_abroad":
                    var abroad = Uof.Ireg_abroadService.GetAll(a => a.code == annua.order_code).FirstOrDefault();
                    if (abroad != null)
                    {
                        annua.name_cn = abroad.name_cn;
                        annua.name_en = abroad.name_en;

                        annua.order_owner = abroad.member.name;
                        annua.salesman = abroad.customer.member1.name;
                    }
                    break;
                case "reg_internal":
                    var dbinternal = Uof.Ireg_internalService.GetAll(a => a.code == annua.order_code).FirstOrDefault();
                    if (dbinternal != null)
                    {
                        annua.name_cn = dbinternal.name_cn;

                        annua.order_owner = dbinternal.member.name;
                        annua.salesman = dbinternal.customer.member1.name;
                    }
                    break;
                case "trademark":
                    var dbtrademark = Uof.ItrademarkService.GetAll(a => a.code == annua.order_code).FirstOrDefault();
                    if (dbtrademark != null)
                    {
                        annua.order_owner = dbtrademark.member.name;
                        annua.salesman = dbtrademark.customer.member1.name;
                    }
                    break;
                case "patent":
                    var dbpatent = Uof.IpatentService.GetAll(a => a.code == annua.order_code).FirstOrDefault();
                    if (dbpatent != null)
                    {
                        annua.order_owner = dbpatent.member.name;
                        annua.salesman = dbpatent.customer.member1.name;
                    }
                    break;
                default:
                    break;
            }

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
                bank = i.bank,
                currency = i.currency,
                rate = i.rate ?? 1,
            }).ToList();

            var total = 0f;
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    total += item.amount.Value * item.rate;
                }
            }                       

            var balance = (annua.amount_transaction * annua.rate) - total;
            var incomes = new
            {
                items = list,
                total = Math.Round(total, 2), //total,
                balance = balance == null ? 0 : Math.Round(balance.Value, 2), //balance,
                rate = annua.rate,
                amount = (float)Math.Round((double)(annua.amount_transaction * annua.rate ?? 0), 2),

                //local_total = (float)Math.Round((double)(total * annua.rate ?? 0), 2),
                //local_balance = (float)Math.Round((double)(balance * annua.rate ?? 0), 2)
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
                    source_id = dbAnnual.order_id,
                    source_name = dbAnnual.type,
                    title = "提交审核",
                    is_system = 1,
                    content = string.Format("年检订单提交给财务审核")
                });

                var auditor_id = GetAuditorByKey("CW_ID");
                if (auditor_id != null)
                {
                    var waitdeals = new List<waitdeal>();
                    waitdeals.Add(new waitdeal
                    {
                        source = "annual",
                        source_id = dbAnnual.id,
                        user_id = auditor_id,
                        router = "annual_view",
                        content = "您有年检订单需要财务审核",
                        read_status = 0
                    });

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
                    source_id = dbAnnual.order_id,
                    source_name = dbAnnual.type,
                    title = "通过审核",
                    is_system = 1,
                    content = string.Format("{0}通过了年检{1}", arrs[3], t)
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
                    source_id = dbAnnual.order_id,
                    source_name = dbAnnual.type,
                    title = "驳回年检单审核",
                    is_system = 1,
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
                    source_id = dbAnnual.order_id,
                    source_name = dbAnnual.type,
                    title = "完成年检订单",
                    is_system = 1,
                    content = string.Format("{0}完成了年检订单，完成日期为：{1}", arrs[3], date_finish.ToString("yyyy-MM-dd"))
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
                        source_id = dbAnnual.order_id,
                        source_name = dbAnnual.type,
                        title = "完善了年检资料",
                        is_system = 1,
                        content = string.Format("{0}完善了年检资料", arrs[3])
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
                        source_id = dbAnnual.order_id,
                        source_name = dbAnnual.type,
                        title = "更新了年检订单进度",
                        is_system = 1,
                        content = string.Format("{0}更新了年检进度: {1} 预计完成日期 {2}", arrs[3], dbAnnual.progress, dbAnnual.date_finish.Value.ToString("yyyy-MM-dd"))
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
        [HttpPost]
        public ActionResult SetOrderStatus(string orderType, int orderId, int status)
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

            //orderType: item.order_type,
            //orderId: item.id,
            //status: 2
            //0 正常 1 转出 2 注销

            var title = "";
            switch (status)
            {
                case 0:
                    title = "恢复正常年检";
                    break;
                case 1:
                    title = "订单转出，不再年检";
                    break;
                case 2:
                    title = "订单注销，不再年检";
                    break;
                case 3:
                    title = "订单暂不年检";
                    break;
                case 6:
                    title = "订单除名";
                    break;
                default:
                    break;
            }
            switch (orderType)
            {
                case "reg_abroad":
                    var dbAbroad = Uof.Ireg_abroadService.GetAll(r1 => r1.id == orderId).FirstOrDefault();
                    dbAbroad.order_status = status;
                    dbAbroad.date_updated = DateTime.Now;

                    var s1 = Uof.Ireg_abroadService.UpdateEntity(dbAbroad);
                    if (s1)
                    {
                        
                        Uof.ItimelineService.AddEntity(new timeline()
                        {
                            source_id = dbAbroad.id,
                            source_name = "reg_abroad",
                            title = title,
                            is_system = 1,
                            content = string.Format("{0}{1}, 档案号{2}", arrs[3], title, dbAbroad.code)
                        });
                    }
                    break;
                case "reg_internal":
                    var dbInternal = Uof.Ireg_internalService.GetAll(r2 => r2.id == orderId).FirstOrDefault();
                    dbInternal.order_status = status;
                    dbInternal.date_updated = DateTime.Now;

                    var s2 = Uof.Ireg_internalService.UpdateEntity(dbInternal);
                    if (s2)
                    {

                        Uof.ItimelineService.AddEntity(new timeline()
                        {
                            source_id = dbInternal.id,
                            source_name = "reg_internal",
                            title = title,
                            is_system = 1,
                            content = string.Format("{0}{1}, 档案号{2}", arrs[3], title, dbInternal.code)
                        });
                    }
                    break;
                case "trademark":
                    var dbTrademark = Uof.ItrademarkService.GetAll(r2 => r2.id == orderId).FirstOrDefault();
                    dbTrademark.order_status = status;
                    dbTrademark.date_updated = DateTime.Now;

                    var s3 = Uof.ItrademarkService.UpdateEntity(dbTrademark);
                    if (s3)
                    {

                        Uof.ItimelineService.AddEntity(new timeline()
                        {
                            source_id = dbTrademark.id,
                            source_name = "trademark",
                            title = title,
                            is_system = 1,
                            content = string.Format("{0}{1}, 档案号{2}", arrs[3], title, dbTrademark.code)
                        });
                    }
                    break;
                case "patent":
                    var dbPatent = Uof.IpatentService.GetAll(r2 => r2.id == orderId).FirstOrDefault();
                    dbPatent.order_status = status;
                    dbPatent.date_updated = DateTime.Now;

                    var s4 = Uof.IpatentService.UpdateEntity(dbPatent);
                    if (s4)
                    {

                        Uof.ItimelineService.AddEntity(new timeline()
                        {
                            source_id = dbPatent.id,
                            source_name = "patent",
                            title = title,
                            is_system = 1,
                            content = string.Format("{0}{1}, 档案号{2}", arrs[3], title, dbPatent.code)
                        });
                    }
                    break;
                default:
                    break;
            }

            return SuccessResult;
        }


        public ActionResult OffOrders(string title, int? order_status, string area)
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

            var items = new List<AnnualWarning>();

            string order_type = null;

            #region 境外注册
            if (string.IsNullOrEmpty(order_type) || order_type == "reg_abroad")
            {
                Expression<Func<reg_abroad, bool>> condition1 = c => c.order_status > 0;

                Expression<Func<reg_abroad, bool>> statusQuery1 = c => true;
                if (order_status != null)
                {
                    statusQuery1 = c => c.order_status == order_status;
                }

                Expression<Func<reg_abroad, bool>> nameQuery1 = c => true;
                if (!string.IsNullOrEmpty(title))
                {
                    nameQuery1 = c => (c.name_cn.Contains(title) || c.name_en.Contains(title) || c.code.Contains(title));
                }

                Expression<Func<reg_abroad, bool>> areaQuery1 = c => true;
                if (!string.IsNullOrEmpty(area))
                {
                    areaQuery1 = c => c.code.Contains(area);
                }

                var abroads = Uof.Ireg_abroadService
                    .GetAll(condition1)
                    .Where(nameQuery1)
                    .Where(areaQuery1)
                    .Where(statusQuery1)
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
                        //saleman = a.member4.name,
                        saleman = a.customer.member1.name ?? "",
                        waiter = a.member6.name,
                        //assistant_name = a.member7.name,
                        submit_review_date = a.submit_review_date,
                        date_finish = a.date_finish,
                        date_setup = a.date_setup,
                        annual_date = a.annual_date,
                        order_status = a.order_status,
                        date_last = a.date_last,
                        title_last = a.title_last,
                        annual_year = a.annual_year,
                        month = DateTime.Today.Month - a.date_setup.Value.Month,
                        region = a.region,
                    }).ToList();

                if (abroads.Count() > 0)
                {
                    items.AddRange(abroads);
                }
            }
            #endregion

            #region 国内注册
            if (string.IsNullOrEmpty(order_type) || order_type == "reg_internal")
            {
                Expression<Func<reg_internal, bool>> condition2 = c => c.order_status > 0;

                Expression<Func<reg_internal, bool>> nameQuery2 = c => true;
                if (!string.IsNullOrEmpty(title))
                {
                    nameQuery2 = c => (c.name_cn.Contains(title) || c.code.Contains(title));
                }
                Expression<Func<reg_internal, bool>> areaQuery2 = c => true;
                if (!string.IsNullOrEmpty(area))
                {
                    areaQuery2 = c => c.code.Contains(area);
                }

                Expression<Func<reg_internal, bool>> statusQuery2 = c => true;
                if (order_status != null)
                {
                    statusQuery2 = c => c.order_status == order_status;
                }

                var internas = Uof.Ireg_internalService
                    .GetAll(condition2)
                    .Where(nameQuery2)
                    .Where(areaQuery2)
                    .Where(statusQuery2)
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
                        //saleman = a.member5.name,
                        saleman = a.customer.member1.name ?? "",
                        waiter = a.member7.name,
                        //assistant_name = a.member.name,
                        submit_review_date = a.submit_review_date,
                        date_finish = a.date_finish,
                        date_setup = a.date_setup,
                        annual_date = a.annual_date,
                        order_status = a.order_status,
                        date_last = a.date_last,
                        title_last = a.title_last,
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
            if (string.IsNullOrEmpty(order_type) || order_type == "trademark")
            {
                Expression<Func<trademark, bool>> condition3 = c => c.order_status > 0;
                               
                Expression<Func<trademark, bool>> nameQuery3 = c => true;
                if (!string.IsNullOrEmpty(title))
                {
                    nameQuery3 = c => (c.name.Contains(title) || c.code.Contains(title));
                }
                Expression<Func<trademark, bool>> areaQuery3 = c => true;
                if (!string.IsNullOrEmpty(area))
                {
                    areaQuery3 = c => c.code.Contains(area);
                }

                Expression<Func<trademark, bool>> statusQuery3 = c => true;
                if (order_status != null)
                {
                    statusQuery3 = c => c.order_status == order_status;
                }

                var trademarks = Uof.ItrademarkService
                    .GetAll(condition3)                    
                    .Where(nameQuery3)
                    .Where(areaQuery3)
                    .Where(statusQuery3)
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
                        //saleman = a.member4.name,
                        saleman = a.customer.member1.name ?? "",
                        waiter = a.member6.name,
                        //assistant_name = a.member.name,
                        submit_review_date = a.submit_review_date,
                        date_finish = a.date_finish,
                        date_setup = a.date_regit,
                        annual_date = a.annual_date,
                        exten_period = a.exten_period,
                        order_status = a.order_status,
                        annual_year = a.annual_year,
                        date_last = a.date_last,
                        title_last = a.title_last,
                        region = a.region,

                    }).ToList();

                if (trademarks.Count() > 0)
                {
                    items.AddRange(trademarks);
                }
            }
            #endregion

            #region 专利注册
            if (string.IsNullOrEmpty(order_type) || order_type == "patent")
            {

                Expression<Func<patent, bool>> condition4 = c => c.order_status > 0;
                
                Expression<Func<patent, bool>> nameQuery4 = c => true;
                if (!string.IsNullOrEmpty(title))
                {
                    nameQuery4 = c => (c.name.Contains(title) || c.code.Contains(title));
                }
                Expression<Func<patent, bool>> areaQuery4 = c => true;
                if (!string.IsNullOrEmpty(area))
                {
                    areaQuery4 = c => c.code.Contains(area);
                }

                Expression<Func<patent, bool>> statusQuery4 = c => true;
                if (order_status != null)
                {
                    statusQuery4 = c => c.order_status == order_status;
                }

                var patents = Uof.IpatentService
                    .GetAll(condition4)
                    .Where(nameQuery4)
                    .Where(areaQuery4)
                    .Where(statusQuery4)
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
                        //saleman = a.member4.name,
                        saleman = a.customer.member1.name ?? "",
                        waiter = a.member6.name,
                        assistant_name = a.member.name,
                        submit_review_date = a.submit_review_date,
                        date_finish = a.date_finish,
                        date_setup = a.date_regit,
                        annual_date = a.annual_date,
                        order_status = a.order_status,
                        date_last = a.date_last,
                        title_last = a.title_last,
                        annual_year = a.annual_year,
                        month = (a.date_regit != null) ? (DateTime.Today.Month - a.date_regit.Value.Month) : 0,
                    }).ToList();

                if (patents.Count() > 0)
                {
                    items.AddRange(patents);
                }
            }
            #endregion

            items = items.OrderBy(i => i.date_setup).ToList();
            var result = new
            {
                items = items
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Revert(int id, string type)
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
            //var organization_id = 0;
            int.TryParse(arrs[0], out userId);
            //int.TryParse(arrs[2], out organization_id);
            var isSuccess = false;
            switch (type)
            {
                case "reg_abroad":
                    var a = Uof.Ireg_abroadService.GetAll(o=>o.id == id).FirstOrDefault();
                    a.order_status = 0;
                    isSuccess = Uof.Ireg_abroadService.UpdateEntity(a);
                    if (isSuccess)
                    {
                        Uof.ItimelineService.AddEntity(new timeline()
                        {
                            source_id = id,
                            source_name = "reg_abroad",
                            title = "恢复年检",
                            is_system = 1,
                            content = string.Format("{0}恢复了订单年检, 档案号{1}", arrs[3], a.code)
                        });
                    }
                    break;
                case "reg_internal":
                    var b = Uof.Ireg_internalService.GetAll(o => o.id == id).FirstOrDefault();
                    b.order_status = 0;
                    isSuccess = Uof.Ireg_internalService.UpdateEntity(b);
                    if (isSuccess)
                    {
                        Uof.ItimelineService.AddEntity(new timeline()
                        {
                            source_id = id,
                            source_name = "reg_internal",
                            title = "恢复年检",
                            is_system = 1,
                            content = string.Format("{0}恢复了订单年检, 档案号{1}", arrs[3], b.code)
                        });
                    }
                    break;
                case "trademark":
                    var c = Uof.ItrademarkService.GetAll(o => o.id == id).FirstOrDefault();
                    c.order_status = 0;
                    isSuccess = Uof.ItrademarkService.UpdateEntity(c);
                    if (isSuccess)
                    {
                        Uof.ItimelineService.AddEntity(new timeline()
                        {
                            source_id = id,
                            source_name = "trademark",
                            title = "恢复年检",
                            is_system = 1,
                            content = string.Format("{0}恢复了订单年检, 档案号{1}", arrs[3], c.code)
                        });
                    }
                    break;
                case "patent":
                    var d = Uof.IpatentService.GetAll(o => o.id == id).FirstOrDefault();
                    d.order_status = 0;
                    isSuccess = Uof.IpatentService.UpdateEntity(d);
                    if (isSuccess)
                    {
                        Uof.ItimelineService.AddEntity(new timeline()
                        {
                            source_id = id,
                            source_name = "patent",
                            title = "恢复年检",
                            is_system = 1,
                            content = string.Format("{0}恢复了订单年检, 档案号{1}", arrs[3], d.code)
                        });
                    }
                    break;
                default:
                    break;
            }

            return SuccessResult;
        }
        [HttpPost]
        public ActionResult ForSale(int order_id, string order_type, float resell_price)
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

            switch (order_type)
            {
                case "reg_abroad":
                    var dbAbroad = Uof.Ireg_abroadService.GetAll(a => a.id == order_id).FirstOrDefault();
                    dbAbroad.resell_price = resell_price;
                    dbAbroad.order_status = 4;
                    dbAbroad.date_updated = DateTime.Now;
                    Uof.Ireg_abroadService.UpdateEntity(dbAbroad);

                    Uof.ItimelineService.AddEntity(new timeline()
                    {
                        source_id = dbAbroad.id,
                        source_name = "reg_abroad",
                        title = "转为待售",
                        is_system = 1,
                        content = string.Format("{0}设置为待售状态, 待售价格{1}", arrs[3], resell_price)
                    });

                    break;
                case "reg_internal":
                    var dbInternal = Uof.Ireg_internalService.GetAll(a => a.id == order_id).FirstOrDefault();
                    dbInternal.resell_price = resell_price;
                    dbInternal.order_status = 4;
                    dbInternal.date_updated = DateTime.Now;
                    Uof.Ireg_internalService.UpdateEntity(dbInternal);

                    Uof.ItimelineService.AddEntity(new timeline()
                    {
                        source_id = dbInternal.id,
                        source_name = "reg_internal",
                        title = "转为待售",
                        is_system = 1,
                        content = string.Format("{0}设置为待售状态, 待售价格{1}", arrs[3], resell_price)
                    });
                    break;
                case "trademark":
                    var dbtrademark = Uof.ItrademarkService.GetAll(a => a.id == order_id).FirstOrDefault();
                    dbtrademark.resell_price = resell_price;
                    dbtrademark.order_status = 4;
                    dbtrademark.date_updated = DateTime.Now;
                    Uof.ItrademarkService.UpdateEntity(dbtrademark);

                    Uof.ItimelineService.AddEntity(new timeline()
                    {
                        source_id = dbtrademark.id,
                        source_name = "trademark",
                        title = "转为待售",
                        is_system = 1,
                        content = string.Format("{0}设置为待售状态, 待售价格{1}", arrs[3], resell_price)
                    });
                    break;
                case "patent":
                    var dbpatent = Uof.IpatentService.GetAll(a => a.id == order_id).FirstOrDefault();
                    dbpatent.resell_price = resell_price;
                    dbpatent.order_status = 4;
                    dbpatent.date_updated = DateTime.Now;
                    Uof.IpatentService.UpdateEntity(dbpatent);

                    Uof.ItimelineService.AddEntity(new timeline()
                    {
                        source_id = dbpatent.id,
                        source_name = "patent",
                        title = "转为待售",
                        is_system = 1,
                        content = string.Format("{0}设置为待售状态, 待售价格{1}", arrs[3], resell_price)
                    });
                    break;
                default:
                    break;
            }

            return SuccessResult;
        }

        public ActionResult GetSallOrders(string title)
        {
            var items = new List<AnnualWarning>();

            #region 境外注册
            Expression<Func<reg_abroad, bool>> nameQuery1 = c => true;
            if (!string.IsNullOrEmpty(title))
            {
                nameQuery1 = c => (c.name_cn.Contains(title) || c.name_en.Contains(title) || c.code.Contains(title));
            }

            var abroads = Uof.Ireg_abroadService
                .GetAll(c => c.order_status == 4 || c.order_status == 5)
                .Where(nameQuery1)
                .Select(a => new AnnualWarning
                {
                    id = a.id,
                    customer_id = a.customer_id,
                    customer_name = a.customer.name,
                    customer_code = a.customer.code,
                    order_code = a.code,
                    order_name = a.name_cn,
                    order_name_en = a.name_en,
                    order_type = "reg_abroad",
                    order_type_name = "境外注册",
                    saleman = a.customer.member1.name ?? "",

                    waiter = a.member6.name,
                    assistant_name = a.member7.name,
                    submit_review_date = a.submit_review_date,
                    date_finish = a.date_finish,
                    date_setup = a.date_setup,
                    annual_date = a.annual_date,
                    annual_year = a.annual_year,
                    month = DateTime.Today.Month - a.date_setup.Value.Month,

                    date_last = a.date_last,
                    title_last = a.title_last,
                    date_wait = a.date_wait,
                    region = a.region,
                    order_status = a.order_status,
                    resell_code = a.resell_code ?? "",
                    resell_price = a.resell_price,
                    is_annual = a.is_annual,
                }).ToList();

            if (abroads.Count() > 0)
            {
                foreach (var item in abroads)
                {
                    var count = Uof.Ibusiness_bankService.GetAll(b => b.source_id == item.id && b.source == "reg_abroad").Count();
                    item.bank_count = count;
                }

                items.AddRange(abroads);                
            }
            #endregion

            #region 国内注册
            Expression<Func<reg_internal, bool>> nameQuery2 = c => true;
            if (!string.IsNullOrEmpty(title))
            {
                nameQuery2 = c => (c.name_cn.Contains(title) || c.code.Contains(title));
            }

            var internas = Uof.Ireg_internalService
                .GetAll(c => c.order_status == 4 || c.order_status == 5)
                .Where(nameQuery2)
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
                    //saleman = a.member5.name,                        
                    saleman = a.customer.member1.name ?? "",
                    waiter = a.member7.name,
                    assistant_name = a.member.name,
                    submit_review_date = a.submit_review_date,
                    date_finish = a.date_finish,
                    date_setup = a.date_setup,
                    annual_year = a.annual_year,
                    month = DateTime.Today.Month - a.date_setup.Value.Month,

                    date_last = a.date_last,
                    title_last = a.title_last,
                    date_wait = a.date_wait,
                    region = "",
                    order_status = a.order_status,
                    resell_code = a.resell_code ?? "",
                    resell_price = a.resell_price,
                    is_annual = a.is_annual,
                }).ToList();

            if (internas.Count() > 0)
            {
                items.AddRange(internas);
            }
            #endregion

            #region 商标注册
            Expression<Func<trademark, bool>> nameQuery3 = c => true;
            if (!string.IsNullOrEmpty(title))
            {
                nameQuery3 = c => (c.name.Contains(title) || c.code.Contains(title));
            }
            var trademarks = Uof.ItrademarkService
                .GetAll(c => c.order_status == 4 || c.order_status == 5)
                .Where(nameQuery3)
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
                    saleman = a.customer.member1.name ?? "",
                    waiter = a.member6.name,
                    assistant_name = a.member.name,
                    submit_review_date = a.submit_review_date,
                    date_finish = a.date_finish,
                    date_setup = a.date_regit,
                    annual_year = a.annual_year,
                    exten_period = a.exten_period,
                    date_last = a.date_last,
                    title_last = a.title_last,
                    date_wait = a.date_wait,
                    region = a.region,
                    order_status = a.order_status,
                    resell_code = a.resell_code ?? "",
                    resell_price = a.resell_price,
                    is_annual = a.is_annual,
                }).ToList();

            if (trademarks.Count() > 0)
            {
                items.AddRange(trademarks);
            }
            #endregion

            #region 专利注册
            Expression<Func<patent, bool>> nameQuery4 = c => true;
            if (!string.IsNullOrEmpty(title))
            {
                nameQuery4 = c => (c.name.Contains(title) || c.code.Contains(title));
            }
            var patents = Uof.IpatentService
                .GetAll(c => c.order_status == 4 || c.order_status == 5)
                .Where(nameQuery4)
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
                    //saleman = a.member4.name,
                    saleman = a.customer.member1.name ?? "",

                    waiter = a.member6.name,
                    assistant_name = a.member.name,
                    submit_review_date = a.submit_review_date,
                    date_finish = a.date_finish,
                    date_setup = a.date_regit,
                    annual_year = a.annual_year,
                    month = (a.date_regit != null) ? (DateTime.Today.Month - a.date_regit.Value.Month) : 0,

                    date_last = a.date_last,
                    title_last = a.title_last,
                    date_wait = a.date_wait,
                    order_status = a.order_status,
                    region = "",
                    resell_code = a.resell_code ?? "",
                    resell_price = a.resell_price,
                    is_annual = a.is_annual,
                }).ToList();

            if (patents.Count() > 0)
            {
                items.AddRange(patents);
            }
            #endregion
            
            if (items.Count > 0)
            {
                foreach (var item in items)
                {
                    item.setup_day = item.date_setup.Value.ToString("MM-dd");
                }
            }

            items = items.OrderBy(i => i.setup_day).ToList();

            var result = new
            {
                items = items
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
