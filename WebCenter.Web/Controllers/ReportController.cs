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
    public class ReportController : BaseController
    {
        public ReportController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        [HttpPost]
        public ActionResult OrderSummary(OrderSummaryRequest request)
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

            var items = new List<FinanceCheck>();

            DateTime? end = null;
            if (request.end_time != null)
            {
                end = request.end_time.Value.AddDays(1);
            }

            #region 境外注册
            if (string.IsNullOrEmpty(request.order_type) || request.order_type == "reg_abroad")
            {
                Expression<Func<reg_abroad, bool>> customerQuery1 = c => true;
                if (request.customer_id != null && request.customer_id.Value > 0)
                {
                    customerQuery1 = c => (c.customer_id == request.customer_id);
                }

                Expression<Func<reg_abroad, bool>> salesmanQuery1 = c => true;
                if (request.salesman_id != null && request.salesman_id.Value > 0)
                {
                    salesmanQuery1 = c => (c.salesman_id == request.salesman_id);
                }
                else
                {
                    if (request.range == 1)
                    {
                        salesmanQuery1 = c => (c.organization_id == deptId);
                    }
                    else if (request.range == 2)
                    {
                        salesmanQuery1 = c => (c.salesman_id == userId);
                    }
                }

                Expression<Func<reg_abroad, bool>> orderTypeQuery1 = c => true;
                switch (request.order_status)
                {
                    case 0:
                        // 未提交审核
                        orderTypeQuery1 = c => c.status == 0 && c.review_status == -1;
                        break;
                    case 1:
                        // 未审核
                        orderTypeQuery1 = c => c.status == 1;
                        break;
                    case 2:
                        // 审核中
                        orderTypeQuery1 = c => c.status == 2;
                        break;
                    case 3:
                        // 已提交
                        orderTypeQuery1 = c => c.status == 3 && c.review_status == 1;
                        break;
                    case 4:
                        // 已完成
                        orderTypeQuery1 = c => c.status == 4;
                        break;
                    default:
                        break;
                }

                Expression<Func<reg_abroad, bool>> startDateQuery1 = c => true;
                if (request.start_time != null)
                {
                    startDateQuery1 = c => c.date_transaction.Value >= request.start_time;
                }
                Expression<Func<reg_abroad, bool>> endDateQuery1 = c => true;
                if (request.end_time != null)
                {
                    endDateQuery1 = c => c.date_transaction.Value < end;
                }

                // 录入开始日期
                Expression<Func<reg_abroad, bool>> date1Created = c => true;
                Expression<Func<reg_abroad, bool>> date2Created = c => true;
                if (request.start_create != null)
                {
                    date1Created = c => (c.date_created >= request.start_create.Value);
                }
                // 录入结束日期
                if (request.end_create != null)
                {
                    var endTime = request.end_create.Value.AddDays(1);
                    date2Created = c => (c.date_created < endTime);
                }

                Expression<Func<reg_abroad, bool>> nameQuery = c => true;
                if (!string.IsNullOrEmpty(request.name))
                {
                    nameQuery = c => (c.code.ToLower().Contains(request.name.ToLower()) || c.name_cn.ToLower().Contains(request.name.ToLower()) || c.name_en.ToLower().Contains(request.name.ToLower()));
                }

                var abroads = Uof.Ireg_abroadService.GetAll()
                    .Where(customerQuery1)
                    .Where(salesmanQuery1)
                    .Where(orderTypeQuery1)
                    .Where(startDateQuery1)
                    .Where(endDateQuery1)
                    .Where(date1Created)
                    .Where(date2Created)
                    .Where(nameQuery)
                    .Select(a => new FinanceCheck
                    {
                        id = a.id,
                        customer_id = a.customer_id,
                        customer_name = a.customer.name,
                        customer_code = a.customer.code,
                        order_code = a.code,
                        order_name = a.name_cn ?? a.name_en,
                        order_name_en = a.name_en,
                        order_type = "reg_abroad",
                        order_type_name = "境外注册",
                        review_status = a.review_status,
                        status = a.status,
                        salesman = a.member4.name,
                        waitor = a.member6.name,
                        amount_transaction = a.amount_transaction,
                        date_transaction = a.date_transaction,
                        date_created = a.date_created,

                    }).ToList();

                if (abroads.Count() > 0)
                {
                    items.AddRange(abroads);
                }
            }

            #endregion

            #region 国内注册
            if (string.IsNullOrEmpty(request.order_type) || request.order_type == "reg_internal")
            {
                Expression<Func<reg_internal, bool>> customerQuery1 = c => true;
                if (request.customer_id != null && request.customer_id.Value > 0)
                {
                    customerQuery1 = c => (c.customer_id == request.customer_id);
                }

                Expression<Func<reg_internal, bool>> salesmanQuery1 = c => true;
                if (request.salesman_id != null && request.salesman_id.Value > 0)
                {
                    salesmanQuery1 = c => (c.salesman_id == request.salesman_id);
                }
                else
                {
                    if (request.range == 1)
                    {
                        salesmanQuery1 = c => (c.organization_id == deptId);
                    }
                    else if (request.range == 2)
                    {
                        salesmanQuery1 = c => (c.salesman_id == userId);
                    }
                }

                Expression<Func<reg_internal, bool>> orderTypeQuery1 = c => true;
                switch (request.order_status)
                {
                    case 0:
                        // 未提交审核
                        orderTypeQuery1 = c => c.status == 0 && c.review_status == -1;
                        break;
                    case 1:
                        // 未审核
                        orderTypeQuery1 = c => c.status == 1;
                        break;
                    case 2:
                        // 审核中
                        orderTypeQuery1 = c => c.status == 2;
                        break;
                    case 3:
                        // 已提交
                        orderTypeQuery1 = c => c.status == 3 && c.review_status == 1;
                        break;
                    case 4:
                        // 已完成
                        orderTypeQuery1 = c => c.status == 4;
                        break;
                    default:
                        break;
                }

                Expression<Func<reg_internal, bool>> startDateQuery1 = c => true;
                if (request.start_time != null)
                {
                    startDateQuery1 = c => c.date_transaction.Value >= request.start_time;
                }
                Expression<Func<reg_internal, bool>> endDateQuery1 = c => true;
                if (request.end_time != null)
                {
                    endDateQuery1 = c => c.date_transaction.Value < end;
                }

                // 录入开始日期
                Expression<Func<reg_internal, bool>> date1Created = c => true;
                Expression<Func<reg_internal, bool>> date2Created = c => true;
                if (request.start_create != null)
                {
                    date1Created = c => (c.date_created >= request.start_create.Value);
                }
                // 录入结束日期
                if (request.end_create != null)
                {
                    var endTime = request.end_create.Value.AddDays(1);
                    date2Created = c => (c.date_created < endTime);
                }

                Expression<Func<reg_internal, bool>> nameQuery = c => true;
                if (!string.IsNullOrEmpty(request.name))
                {
                    nameQuery = c => (c.code.ToLower().Contains(request.name.ToLower()) || c.name_cn.ToLower().Contains(request.name.ToLower()));
                }

                var internas = Uof.Ireg_internalService.GetAll()
                    .Where(customerQuery1)
                    .Where(salesmanQuery1)
                    .Where(orderTypeQuery1)
                    .Where(startDateQuery1)
                    .Where(endDateQuery1)
                    .Where(date1Created)
                    .Where(date2Created)
                    .Where(nameQuery)
                    .Select(a => new FinanceCheck
                    {
                        id = a.id,
                        customer_id = a.customer_id,
                        customer_name = a.customer.name,
                        customer_code = a.customer.code,
                        order_code = a.code,
                        order_name = a.name_cn,
                        order_type = "reg_internal",
                        order_type_name = "境内注册",
                        review_status = a.review_status,
                        status = a.status,
                        salesman = a.member4.name,
                        waitor = a.member6.name,
                        amount_transaction = a.amount_transaction,
                        date_transaction = a.date_transaction,
                        date_created = a.date_created,

                    }).ToList();

                if (internas.Count() > 0)
                {
                    items.AddRange(internas);
                }
            }
            #endregion

            #region 商标注册
            if (string.IsNullOrEmpty(request.order_type) || request.order_type == "trademark")
            {
                Expression<Func<trademark, bool>> customerQuery1 = c => true;
                if (request.customer_id != null && request.customer_id.Value > 0)
                {
                    customerQuery1 = c => (c.customer_id == request.customer_id);
                }

                Expression<Func<trademark, bool>> salesmanQuery1 = c => true;
                if (request.salesman_id != null && request.salesman_id.Value > 0)
                {
                    salesmanQuery1 = c => (c.salesman_id == request.salesman_id);
                }
                else
                {
                    if (request.range == 1)
                    {
                        salesmanQuery1 = c => (c.organization_id == deptId);
                    }
                    else if (request.range == 2)
                    {
                        salesmanQuery1 = c => (c.salesman_id == userId);
                    }
                }

                Expression<Func<trademark, bool>> orderTypeQuery1 = c => true;
                switch (request.order_status)
                {
                    case 0:
                        // 未提交审核
                        orderTypeQuery1 = c => c.status == 0 && c.review_status == -1;
                        break;
                    case 1:
                        // 未审核
                        orderTypeQuery1 = c => c.status == 1;
                        break;
                    case 2:
                        // 审核中
                        orderTypeQuery1 = c => c.status == 2;
                        break;
                    case 3:
                        // 已提交
                        orderTypeQuery1 = c => c.status == 3 && c.review_status == 1;
                        break;
                    case 4:
                        // 已完成
                        orderTypeQuery1 = c => c.status == 4;
                        break;
                    default:
                        break;
                }

                Expression<Func<trademark, bool>> startDateQuery1 = c => true;
                if (request.start_time != null)
                {
                    startDateQuery1 = c => c.date_transaction.Value >= request.start_time;
                }
                Expression<Func<trademark, bool>> endDateQuery1 = c => true;
                if (request.end_time != null)
                {
                    endDateQuery1 = c => c.date_transaction.Value < end;
                }

                // 录入开始日期
                Expression<Func<trademark, bool>> date1Created = c => true;
                Expression<Func<trademark, bool>> date2Created = c => true;
                if (request.start_create != null)
                {
                    date1Created = c => (c.date_created >= request.start_create.Value);
                }
                // 录入结束日期
                if (request.end_create != null)
                {
                    var endTime = request.end_create.Value.AddDays(1);
                    date2Created = c => (c.date_created < endTime);
                }

                Expression<Func<trademark, bool>> nameQuery = c => true;
                if (!string.IsNullOrEmpty(request.name))
                {
                    nameQuery = c => (c.code.ToLower().Contains(request.name.ToLower()) || c.name.ToLower().Contains(request.name.ToLower()));
                }

                var test = Uof.ItrademarkService.GetAll().ToList();
                var trademarks = Uof.ItrademarkService.GetAll()
                    .Where(customerQuery1)
                    .Where(salesmanQuery1)
                    .Where(orderTypeQuery1)
                    .Where(startDateQuery1)
                    .Where(endDateQuery1)
                    .Where(date1Created)
                    .Where(date2Created)
                    .Where(nameQuery)
                    .Select(a => new FinanceCheck
                    {
                        id = a.id,
                        customer_id = a.customer_id,
                        customer_name = a.customer.name,
                        customer_code = a.customer.code,
                        order_code = a.code,
                        order_name = a.name,
                        order_type = "trademark",
                        order_type_name = "商标",
                        review_status = a.review_status,
                        status = a.status,
                        salesman = a.member3.name,
                        waitor = a.member5.name,
                        amount_transaction = a.amount_transaction,
                        date_transaction = a.date_transaction,
                        date_created = a.date_created,

                    }).ToList();

                if (trademarks.Count() > 0)
                {
                    items.AddRange(trademarks);
                }
            }
            #endregion

            #region 专利注册
            if (string.IsNullOrEmpty(request.order_type) || request.order_type == "patent")
            {
                Expression<Func<patent, bool>> customerQuery1 = c => true;
                if (request.customer_id != null && request.customer_id.Value > 0)
                {
                    customerQuery1 = c => (c.customer_id == request.customer_id);
                }

                Expression<Func<patent, bool>> salesmanQuery1 = c => true;
                if (request.salesman_id != null && request.salesman_id.Value > 0)
                {
                    salesmanQuery1 = c => (c.salesman_id == request.salesman_id);
                }
                else
                {
                    if (request.range == 1)
                    {
                        salesmanQuery1 = c => (c.organization_id == deptId);
                    }
                    else if (request.range == 2)
                    {
                        salesmanQuery1 = c => (c.salesman_id == userId);
                    }
                }

                Expression<Func<patent, bool>> orderTypeQuery1 = c => true;
                switch (request.order_status)
                {
                    case 0:
                        // 未提交审核
                        orderTypeQuery1 = c => c.status == 0 && c.review_status == -1;
                        break;
                    case 1:
                        // 未审核
                        orderTypeQuery1 = c => c.status == 1;
                        break;
                    case 2:
                        // 审核中
                        orderTypeQuery1 = c => c.status == 2;
                        break;
                    case 3:
                        // 已提交
                        orderTypeQuery1 = c => c.status == 3 && c.review_status == 1;
                        break;
                    case 4:
                        // 已完成
                        orderTypeQuery1 = c => c.status == 4;
                        break;
                    default:
                        break;
                }

                Expression<Func<patent, bool>> startDateQuery1 = c => true;
                if (request.start_time != null)
                {
                    startDateQuery1 = c => c.date_transaction.Value >= request.start_time;
                }
                Expression<Func<patent, bool>> endDateQuery1 = c => true;
                if (request.end_time != null)
                {
                    endDateQuery1 = c => c.date_transaction.Value < end;
                }

                // 录入开始日期
                Expression<Func<patent, bool>> date1Created = c => true;
                Expression<Func<patent, bool>> date2Created = c => true;
                if (request.start_create != null)
                {
                    date1Created = c => (c.date_created >= request.start_create.Value);
                }
                // 录入结束日期
                if (request.end_create != null)
                {
                    var endTime = request.end_create.Value.AddDays(1);
                    date2Created = c => (c.date_created < endTime);
                }

                Expression<Func<patent, bool>> nameQuery = c => true;
                if (!string.IsNullOrEmpty(request.name))
                {
                    nameQuery = c => (c.code.ToLower().Contains(request.name.ToLower()) || c.name.ToLower().Contains(request.name.ToLower()));
                }

                var patents = Uof.IpatentService.GetAll()
                    .Where(customerQuery1)
                    .Where(salesmanQuery1)
                    .Where(orderTypeQuery1)
                    .Where(startDateQuery1)
                    .Where(endDateQuery1)
                    .Where(date1Created)
                    .Where(date2Created)
                    .Where(nameQuery)
                    .Select(a => new FinanceCheck
                    {
                        id = a.id,
                        customer_id = a.customer_id,
                        customer_name = a.customer.name,
                        customer_code = a.customer.code,
                        order_code = a.code,
                        order_name = a.name,
                        order_name_en = "",
                        order_type = "patent",
                        order_type_name = "专利",
                        review_status = a.review_status,
                        status = a.status,
                        salesman = a.member3.name,
                        waitor = a.member5.name,
                        amount_transaction = a.amount_transaction,
                        date_transaction = a.date_transaction,
                        date_created = a.date_created,

                    }).ToList();

                if (patents.Count() > 0)
                {
                    items.AddRange(patents);
                }
            }
            #endregion

            #region 审计
            if (string.IsNullOrEmpty(request.order_type) || request.order_type == "audit")
            {
                Expression<Func<audit, bool>> customerQuery1 = c => true;
                if (request.customer_id != null && request.customer_id.Value > 0)
                {
                    customerQuery1 = c => (c.customer_id == request.customer_id);
                }

                Expression<Func<audit, bool>> salesmanQuery1 = c => true;
                if (request.salesman_id != null && request.salesman_id.Value > 0)
                {
                    salesmanQuery1 = c => (c.salesman_id == request.salesman_id);
                }
                else
                {
                    if (request.range == 1)
                    {
                        salesmanQuery1 = c => (c.organization_id == deptId);
                    }
                    else if (request.range == 2)
                    {
                        salesmanQuery1 = c => (c.salesman_id == userId);
                    }
                }


                Expression<Func<audit, bool>> orderTypeQuery1 = c => true;
                switch (request.order_status)
                {
                    case 0:
                        // 未提交审核
                        orderTypeQuery1 = c => c.status == 0 && c.review_status == -1;
                        break;
                    case 1:
                        // 未审核
                        orderTypeQuery1 = c => c.status == 1;
                        break;
                    case 2:
                        // 审核中
                        orderTypeQuery1 = c => c.status == 2;
                        break;
                    case 3:
                        // 已提交
                        orderTypeQuery1 = c => c.status == 3 && c.review_status == 1;
                        break;
                    case 4:
                        // 已完成
                        orderTypeQuery1 = c => c.status == 4;
                        break;
                    default:
                        break;
                }

                Expression<Func<audit, bool>> startDateQuery1 = c => true;
                if (request.start_time != null)
                {
                    startDateQuery1 = c => c.date_transaction.Value >= request.start_time;
                }
                Expression<Func<audit, bool>> endDateQuery1 = c => true;
                if (request.end_time != null)
                {
                    endDateQuery1 = c => c.date_transaction.Value < end;
                }

                // 录入开始日期
                Expression<Func<audit, bool>> date1Created = c => true;
                Expression<Func<audit, bool>> date2Created = c => true;
                if (request.start_create != null)
                {
                    date1Created = c => (c.date_created >= request.start_create.Value);
                }
                // 录入结束日期
                if (request.end_create != null)
                {
                    var endTime = request.end_create.Value.AddDays(1);
                    date2Created = c => (c.date_created < endTime);
                }

                Expression<Func<audit, bool>> nameQuery = c => true;
                if (!string.IsNullOrEmpty(request.name))
                {
                    nameQuery = c => (c.code.ToLower().Contains(request.name.ToLower()) || c.name_cn.ToLower().Contains(request.name.ToLower()) || c.name_en.ToLower().Contains(request.name.ToLower()));
                }

                var audits = Uof.IauditService.GetAll()
                    .Where(customerQuery1)
                    .Where(salesmanQuery1)
                    .Where(orderTypeQuery1)
                    .Where(startDateQuery1)
                    .Where(endDateQuery1)
                    .Where(date1Created)
                    .Where(date2Created)
                    .Where(nameQuery)
                    .Select(a => new FinanceCheck
                    {
                        id = a.id,
                        customer_id = a.customer_id,
                        customer_name = a.customer.name,
                        customer_code = a.customer.code,
                        order_code = a.code,
                        order_name = a.name_cn,
                        order_type = "audit",
                        order_type_name = "审计",
                        review_status = a.review_status,
                        status = a.status,
                        salesman = a.member4.name,
                        waitor = "-",
                        amount_transaction = a.amount_transaction,
                        date_transaction = a.date_transaction,
                        date_created = a.date_created,

                    }).ToList();

                if (audits.Count() > 0)
                {
                    items.AddRange(audits);
                }
            }
            #endregion

            #region 年检
            if (string.IsNullOrEmpty(request.order_type) || request.order_type == "annual_exam")
            {
                Expression<Func<annual_exam, bool>> customerQuery1 = c => true;
                if (request.customer_id != null && request.customer_id.Value > 0)
                {
                    customerQuery1 = c => (c.customer_id == request.customer_id);
                }

                Expression<Func<annual_exam, bool>> salesmanQuery1 = c => true;
                if (request.salesman_id != null && request.salesman_id.Value > 0)
                {
                    salesmanQuery1 = c => (c.waiter_id == request.salesman_id);
                }
                else
                {
                    if (request.range == 1)
                    {
                        salesmanQuery1 = c => (c.organization_id == deptId);
                    }
                    else if (request.range == 2)
                    {
                        salesmanQuery1 = c => (c.salesman_id == userId);
                    }
                }

                Expression<Func<annual_exam, bool>> orderTypeQuery1 = c => true;
                switch (request.order_status)
                {
                    case 0:
                        // 未提交审核
                        orderTypeQuery1 = c => c.status == 0 && c.review_status == -1;
                        break;
                    case 1:
                        // 未审核
                        orderTypeQuery1 = c => c.status == 1;
                        break;
                    case 2:
                        // 审核中
                        orderTypeQuery1 = c => c.status == 2;
                        break;
                    case 3:
                        // 已提交
                        orderTypeQuery1 = c => c.status == 3 && c.review_status == 1;
                        break;
                    case 4:
                        // 已完成
                        orderTypeQuery1 = c => c.status == 4;
                        break;
                    default:
                        break;
                }

                Expression<Func<annual_exam, bool>> startDateQuery1 = c => true;
                if (request.start_time != null)
                {
                    startDateQuery1 = c => c.date_transaction.Value >= request.start_time;
                }
                Expression<Func<annual_exam, bool>> endDateQuery1 = c => true;
                if (request.end_time != null)
                {
                    endDateQuery1 = c => c.date_transaction.Value < end;
                }

                // 录入开始日期
                Expression<Func<annual_exam, bool>> date1Created = c => true;
                Expression<Func<annual_exam, bool>> date2Created = c => true;
                if (request.start_create != null)
                {
                    date1Created = c => (c.date_created >= request.start_create.Value);
                }
                // 录入结束日期
                if (request.end_create != null)
                {
                    var endTime = request.end_create.Value.AddDays(1);
                    date2Created = c => (c.date_created < endTime);
                }

                Expression<Func<annual_exam, bool>> nameQuery = c => true;
                if (!string.IsNullOrEmpty(request.name))
                {
                    nameQuery = c => (c.code.ToLower().Contains(request.name.ToLower()) || c.name_cn.ToLower().Contains(request.name.ToLower()) || c.name_en.ToLower().Contains(request.name.ToLower()));
                }

                var audits = Uof.Iannual_examService.GetAll()
                    .Where(customerQuery1)
                    .Where(salesmanQuery1)
                    .Where(orderTypeQuery1)
                    .Where(startDateQuery1)
                    .Where(endDateQuery1)
                    .Where(date1Created)
                    .Where(date2Created)
                    .Where(nameQuery)
                    .Select(a => new FinanceCheck
                    {
                        id = a.id,
                        customer_id = a.customer_id,
                        customer_name = a.customer.name,
                        customer_code = a.customer.code,
                        order_code = "", //a.code,
                        order_name = a.name_cn ?? a.name_en,
                        order_name_en = a.name_en,
                        order_type = "annual_exam",
                        order_type_name = "年检",
                        review_status = a.review_status,
                        status = a.status,
                        salesman = "-",
                        waitor = a.member6.name,
                        amount_transaction = a.amount_transaction,
                        date_transaction = a.date_transaction,
                        date_created = a.date_created,

                    }).ToList();

                if (audits.Count() > 0)
                {
                    items.AddRange(audits);
                }
            }
            #endregion


            var result = new
            {
                items = items
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}