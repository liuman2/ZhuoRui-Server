﻿using System;
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

        public ActionResult Warning(int? customer_id, int? waiter_id)
        {
            var items = new List<AnnualWarning>();
            var nowYear = DateTime.Now.Year;
            var Month1 = DateTime.Now.AddMonths(-13).Month;

            #region 境外注册
            Expression<Func<reg_abroad, bool>> condition1 = c => c.status == 4 && (c.submit_review_date.Value.Month - Month1 == 1 && nowYear > c.submit_review_date.Value.Year && (c.annual_year == null || (c.annual_year != null && c.annual_year < nowYear)));
            Expression<Func<reg_abroad, bool>> customerQuery1 = c => true;
            if (customer_id != null && customer_id.Value > 0)
            {
                customerQuery1 = c => (c.customer_id == customer_id);
            }
            Expression<Func<reg_abroad, bool>> waiteQuery1 = c => true;
            if (waiter_id != null && waiter_id.Value > 0)
            {
                waiteQuery1 = c => (c.waiter_id == waiter_id);
            }

            var abroads = Uof.Ireg_abroadService.GetAll(condition1).Where(customerQuery1).Where(waiteQuery1).Select(a => new AnnualWarning
            {
                id = a.id,
                customer_id = a.customer_id,
                customer_name = a.customer.name,
                customer_code = a.customer.code,
                order_code = a.code,
                order_name = a.name_cn,
                order_type = "reg_abroad",
                order_type_name = "境外注册",
                submit_review_date = a.submit_review_date,
                date_finish = a.date_finish
            }).ToList();

            if (abroads.Count() > 0)
            {
                items.AddRange(abroads);
            }
            #endregion

            #region 国内注册
            var internalMonth = DateTime.Now.Month;
            if (internalMonth >= 3 && internalMonth <= 6)
            {
                Expression<Func<reg_internal, bool>> condition2 = c => c.status == 4 && nowYear > c.submit_review_date.Value.Year && (c.annual_year == null || (c.annual_year != null && c.annual_year < nowYear));
                Expression<Func<reg_internal, bool>> customerQuery2 = c => true;

                if (customer_id != null && customer_id.Value > 0)
                {
                    customerQuery2 = c => (c.customer_id == customer_id);
                }
                Expression<Func<reg_internal, bool>> waiteQuery2 = c => true;
                if (waiter_id != null && waiter_id.Value > 0)
                {
                    waiteQuery2 = c => (c.waiter_id == waiter_id);
                }

                var internas = Uof.Ireg_internalService.GetAll(condition2).Where(customerQuery2).Where(waiteQuery2).Select(a => new AnnualWarning
                {
                    id = a.id,
                    customer_id = a.customer_id,
                    customer_name = a.customer.name,
                    customer_code = a.customer.code,
                    order_code = a.code,
                    order_name = a.name_cn,
                    order_type = "reg_internal",
                    order_type_name = "境内注册",
                    submit_review_date = a.submit_review_date,
                    date_finish = a.date_finish
                }).ToList();

                if (internas.Count() > 0)
                {
                    items.AddRange(internas);
                }
            }
            #endregion

            #region 商标注册
            Expression<Func<trademark, bool>> condition3 = c => c.status == 4 && c.submit_review_date.Value.Month - Month1 == 1 && nowYear > c.submit_review_date.Value.Year && (c.annual_year == null || (c.annual_year != null && c.annual_year < nowYear));
            Expression<Func<trademark, bool>> customerQuery3 = c => true;
            if (customer_id != null && customer_id.Value > 0)
            {
                customerQuery3 = c => (c.customer_id == customer_id);
            }
            Expression<Func<trademark, bool>> waiteQuery3 = c => true;
            if (waiter_id != null && waiter_id.Value > 0)
            {
                waiteQuery3 = c => (c.waiter_id == waiter_id);
            }

            var trademarks = Uof.ItrademarkService.GetAll(condition3).Where(customerQuery3).Where(waiteQuery3).Select(a => new AnnualWarning
            {
                id = a.id,
                customer_id = a.customer_id,
                customer_name = a.customer.name,
                customer_code = a.customer.code,
                order_code = a.code,
                order_name = a.name,
                order_type = "trademark",
                order_type_name = "商标注册",
                submit_review_date = a.submit_review_date,
                date_finish = a.date_finish
            }).ToList();

            if (abroads.Count() > 0)
            {
                items.AddRange(trademarks);
            }
            #endregion

            #region 专利注册
            Expression<Func<patent, bool>> condition4 = c => c.status == 4 && c.submit_review_date.Value.Month - Month1 == 1 && nowYear > c.submit_review_date.Value.Year && (c.annual_year == null || (c.annual_year != null && c.annual_year < nowYear));
            Expression<Func<patent, bool>> customerQuery4 = c => true;
            if (customer_id != null && customer_id.Value > 0)
            {
                customerQuery4 = c => (c.customer_id == customer_id);
            }
            Expression<Func<patent, bool>> waiteQuery4 = c => true;
            if (waiter_id != null && waiter_id.Value > 0)
            {
                waiteQuery4 = c => (c.waiter_id == waiter_id);
            }

            
            var patents = Uof.IpatentService.GetAll(condition4).Where(customerQuery4).Where(waiteQuery4).Select(a => new AnnualWarning
            {
                id = a.id,
                customer_id = a.customer_id,
                customer_name = a.customer.name,
                customer_code = a.customer.code,
                order_code = a.code,
                order_name = a.name,
                order_type = "patent",
                order_type_name = "专利注册",
                submit_review_date = a.submit_review_date,
                date_finish = a.date_finish
            }).ToList();

            if (abroads.Count() > 0)
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
                        order_type_name = "海外注册",
                        order_name_cn = a.name_cn,
                        order_name_en = a.name_en,

                        customer_id = a.customer_id,
                        customer_code = a.customer.code,
                        customer_name = a.customer.name
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

            exam.code = GetNextOrderCode("NJ");
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
                    Uof.Ireg_abroadService.UpdateEntity(dbRegAbroad);
                    break;
                default:
                    break;
            }

            Uof.ItimelineService.AddEntity(new timeline()
            {
                source_id = newExam.id,
                source_name = "annual",
                title = "新建年检",
                content = string.Format("{0}新建了年检订单, 单号{1}", arrs[3], newExam.code)
            });

            return Json(new { id = newExam.id }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Search(OrderSearchRequest request)
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

            Expression<Func<annual_exam, bool>> condition = c => c.salesman_id == userId;

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
                    name_cn = "",
                    name_en = "",
                    status = c.status,
                    review_status = c.review_status,
                    date_transaction = c.date_transaction,
                    amount_transaction = c.amount_transaction,
                    amount_income = 0,
                    amount_unreceive = 0,
                    progress = c.progress,
                    salesman_id = c.salesman_id,
                    salesman_name = c.member3.name,

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

    }
}