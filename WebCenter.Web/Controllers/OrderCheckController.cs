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
    public class OrderCheckController : BaseController
    {
        public OrderCheckController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        public ActionResult FinanceCheck(int? customer_id, int? salesman_id, int? order_status, string order_type)
        {
            var items = new List<FinanceCheck>();

            #region 境外注册
            if (string.IsNullOrEmpty(order_type) || order_type == "reg_abroad")
            {
                Expression<Func<reg_abroad, bool>> customerQuery1 = c => true;
                if (customer_id != null && customer_id.Value > 0)
                {
                    customerQuery1 = c => (c.customer_id == customer_id);
                }

                Expression<Func<reg_abroad, bool>> salesmanQuery1 = c => true;
                if (salesman_id != null && salesman_id.Value > 0)
                {
                    salesmanQuery1 = c => (c.salesman_id == salesman_id);
                }

                Expression<Func<reg_abroad, bool>> orderTypeQuery1 = c => c.status > 0;
                if (order_status == 0)
                {
                    // 未审核
                    orderTypeQuery1 = c => c.status == 1;
                }
                else if (order_status == 1)
                {
                    // 已审核
                    orderTypeQuery1 = c => c.status > 1;
                }

                var abroads = Uof.Ireg_abroadService.GetAll().Where(customerQuery1).Where(salesmanQuery1).Where(orderTypeQuery1).Select(a => new FinanceCheck
                {
                    id = a.id,
                    customer_id = a.customer_id,
                    customer_name = a.customer.name,
                    customer_code = a.customer.code,
                    order_code = a.code,
                    order_name = a.name_cn,
                    order_type = "reg_abroad",
                    order_type_name = "境外注册",
                    review_status = a.review_status,
                    status = a.status,
                    salesman = a.member4.name,
                    amount_transaction = a.amount_transaction

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
                Expression<Func<reg_internal, bool>> customerQuery1 = c => true;
                if (customer_id != null && customer_id.Value > 0)
                {
                    customerQuery1 = c => (c.customer_id == customer_id);
                }

                Expression<Func<reg_internal, bool>> salesmanQuery1 = c => true;
                if (salesman_id != null && salesman_id.Value > 0)
                {
                    salesmanQuery1 = c => (c.salesman_id == salesman_id);
                }

                Expression<Func<reg_internal, bool>> orderTypeQuery1 = c => c.status > 0;
                if (order_status == 0)
                {
                    // 未审核
                    orderTypeQuery1 = c => c.status == 1;
                }
                else if (order_status == 1)
                {
                    // 已审核
                    orderTypeQuery1 = c => c.status > 1;
                }

                var internas = Uof.Ireg_internalService.GetAll().Where(customerQuery1).Where(salesmanQuery1).Where(orderTypeQuery1).Select(a => new FinanceCheck
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
                    salesman = a.member5.name,
                    amount_transaction = a.amount_transaction

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
                Expression<Func<trademark, bool>> customerQuery1 = c => true;
                if (customer_id != null && customer_id.Value > 0)
                {
                    customerQuery1 = c => (c.customer_id == customer_id);
                }

                Expression<Func<trademark, bool>> salesmanQuery1 = c => true;
                if (salesman_id != null && salesman_id.Value > 0)
                {
                    salesmanQuery1 = c => (c.salesman_id == salesman_id);
                }

                Expression<Func<trademark, bool>> orderTypeQuery1 = c => c.status > 0;
                if (order_status == 0)
                {
                    // 未审核
                    orderTypeQuery1 = c => c.status == 1;
                }
                else if (order_status == 1)
                {
                    // 已审核
                    orderTypeQuery1 = c => c.status > 1;
                }

                var test = Uof.ItrademarkService.GetAll().ToList();
                var trademarks = Uof.ItrademarkService.GetAll().Where(customerQuery1).Where(salesmanQuery1).Where(orderTypeQuery1).Select(a => new FinanceCheck
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
                    salesman = a.member4.name,
                    amount_transaction = a.amount_transaction

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
                Expression<Func<patent, bool>> customerQuery1 = c => true;
                if (customer_id != null && customer_id.Value > 0)
                {
                    customerQuery1 = c => (c.customer_id == customer_id);
                }

                Expression<Func<patent, bool>> salesmanQuery1 = c => true;
                if (salesman_id != null && salesman_id.Value > 0)
                {
                    salesmanQuery1 = c => (c.salesman_id == salesman_id);
                }

                Expression<Func<patent, bool>> orderTypeQuery1 = c => c.status > 0;
                if (order_status == 0)
                {
                    // 未审核
                    orderTypeQuery1 = c => c.status == 1;
                }
                else if (order_status == 1)
                {
                    // 已审核
                    orderTypeQuery1 = c => c.status > 1;
                }

                var patents = Uof.IpatentService.GetAll().Where(customerQuery1).Where(salesmanQuery1).Where(orderTypeQuery1).Select(a => new FinanceCheck
                {
                    id = a.id,
                    customer_id = a.customer_id,
                    customer_name = a.customer.name,
                    customer_code = a.customer.code,
                    order_code = a.code,
                    order_name = a.name,
                    order_type = "patent",
                    order_type_name = "专利",
                    review_status = a.review_status,
                    status = a.status,
                    salesman = a.member4.name,
                    amount_transaction = a.amount_transaction

                }).ToList();

                if (patents.Count() > 0)
                {
                    items.AddRange(patents);
                }
            }
            #endregion

            #region 审计
            if (string.IsNullOrEmpty(order_type) || order_type == "audit")
            {
                Expression<Func<audit, bool>> customerQuery1 = c => true;
                if (customer_id != null && customer_id.Value > 0)
                {
                    customerQuery1 = c => (c.customer_id == customer_id);
                }

                Expression<Func<audit, bool>> salesmanQuery1 = c => true;
                if (salesman_id != null && salesman_id.Value > 0)
                {
                    salesmanQuery1 = c => (c.salesman_id == salesman_id);
                }

                Expression<Func<audit, bool>> orderTypeQuery1 = c => c.status > 0;
                if (order_status == 0)
                {
                    // 未审核
                    orderTypeQuery1 = c => c.status == 1;
                }
                else if (order_status == 1)
                {
                    // 已审核
                    orderTypeQuery1 = c => c.status > 1;
                }

                var audits = Uof.IauditService.GetAll().Where(customerQuery1).Where(salesmanQuery1).Where(orderTypeQuery1).Select(a => new FinanceCheck
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
                    amount_transaction = a.amount_transaction

                }).ToList();

                if (audits.Count() > 0)
                {
                    items.AddRange(audits);
                }
            }
            #endregion

            #region 年检
            if (string.IsNullOrEmpty(order_type) || order_type == "annual_exam")
            {
                Expression<Func<annual_exam, bool>> customerQuery1 = c => true;
                if (customer_id != null && customer_id.Value > 0)
                {
                    customerQuery1 = c => (c.customer_id == customer_id);
                }

                Expression<Func<annual_exam, bool>> salesmanQuery1 = c => true;
                if (salesman_id != null && salesman_id.Value > 0)
                {
                    salesmanQuery1 = c => (c.waiter_id == salesman_id);
                }

                Expression<Func<annual_exam, bool>> orderTypeQuery1 = c => c.status > 0;
                if (order_status == 0)
                {
                    // 未审核
                    orderTypeQuery1 = c => c.status == 1;
                }
                else if (order_status == 1)
                {
                    // 已审核
                    orderTypeQuery1 = c => c.status > 1;
                }

                var audits = Uof.Iannual_examService.GetAll().Where(customerQuery1).Where(salesmanQuery1).Where(orderTypeQuery1).Select(a => new FinanceCheck
                {
                    id = a.id,
                    customer_id = a.customer_id,
                    customer_name = a.customer.name,
                    customer_code = a.customer.code,
                    order_code = a.code,
                    order_name = a.name_cn,
                    order_type = "annual_exam",
                    order_type_name = "年检",
                    review_status = a.review_status,
                    status = a.status,
                    salesman = a.member4.name,
                    amount_transaction = a.amount_transaction

                }).ToList();

                if (audits.Count() > 0)
                {
                    items.AddRange(audits);
                }
            }
            #endregion

            #region 变更记录
            if (string.IsNullOrEmpty(order_type) || order_type == "history")
            {
                Expression<Func<history, bool>> customerQuery1 = c => true;
                if (customer_id != null && customer_id.Value > 0)
                {
                    customerQuery1 = c => (c.customer_id == customer_id);
                }

                Expression<Func<history, bool>> salesmanQuery1 = c => true;
                if (salesman_id != null && salesman_id.Value > 0)
                {
                    salesmanQuery1 = c => (c.salesman_id == salesman_id);
                }

                Expression<Func<history, bool>> orderTypeQuery1 = c => c.status > 0;
                if (order_status == 0)
                {
                    // 未审核
                    orderTypeQuery1 = c => c.status == 1;
                }
                else if (order_status == 1)
                {
                    // 已审核
                    orderTypeQuery1 = c => c.status > 1;
                }

                var historys = Uof.IhistoryService.GetAll().Where(customerQuery1).Where(salesmanQuery1).Where(orderTypeQuery1).Select(a => new FinanceCheck
                {
                    id = a.id,
                    customer_id = a.customer_id,
                    //customer_name = a.customer.name,
                    //customer_code = a.customer.code,
                    order_code = a.order_code,
                    order_name = "",
                    order_type = "history",
                    order_type_name = "",
                    source = a.source,
                    review_status = a.review_status,
                    status = a.status,
                    salesman = a.member2.name,
                    amount_transaction = a.amount_transaction

                }).ToList();

                if (historys.Count() > 0)
                {
                    foreach (var item in historys)
                    {
                        item.order_type_name = GetHistorySourceName(item.source);
                    }
                    items.AddRange(historys);
                }
            }

            #endregion


            var result = new
            {
                items = items
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SubmitCheck(int? customer_id, int? salesman_id, int? order_status, string order_type, string name)
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
            var userId = arrs[0];
            // int.TryParse(arrs[0], out userId);
            var ops = arrs[4].Split(',');
            var hasCompany = ops.Where(o => o == "1").FirstOrDefault();

            var items = new List<FinanceCheck>();

            var settings = Uof.IsettingService.GetAll(s => s.name == "JW_ID" || s.name == "GN_ID" || s.name == "JWSJ_ID" || s.name == "GNSJ_ID" || s.name == "SB_ID" || s.name == "ZL_ID").ToList();

            #region 境外注册
            var jwId = settings.Where(s => name == "JW_ID").Select(s => s.value).FirstOrDefault();
            if ((string.IsNullOrEmpty(order_type) || order_type == "reg_abroad") && (hasCompany != null || jwId == userId))
            {
                Expression<Func<reg_abroad, bool>> customerQuery1 = c => true;
                if (customer_id != null && customer_id.Value > 0)
                {
                    customerQuery1 = c => (c.customer_id == customer_id);
                }

                Expression<Func<reg_abroad, bool>> salesmanQuery1 = c => true;
                if (salesman_id != null && salesman_id.Value > 0)
                {
                    salesmanQuery1 = c => (c.salesman_id == salesman_id);
                }

                Expression<Func<reg_abroad, bool>> orderTypeQuery1 = c => c.status >= 2;
                if (order_status == 0)
                {
                    // 未审核
                    orderTypeQuery1 = c => c.status == 2 && c.review_status == 1;
                }
                else if (order_status == 1)
                {
                    // 已审核
                    orderTypeQuery1 = c => c.status > 2;
                }

                Expression<Func<reg_abroad, bool>> nameQuery1 = c => true;
                if (!string.IsNullOrEmpty(name))
                {
                    nameQuery1 = c => (c.name_cn.ToLower().Contains(name.ToLower()) || c.name_en.ToLower().Contains(name.ToLower()) || c.code.ToLower().Contains(name.ToLower()));
                }

                var abroads = Uof.Ireg_abroadService.GetAll()
                    .Where(customerQuery1)
                    .Where(salesmanQuery1)
                    .Where(orderTypeQuery1)
                    .Where(nameQuery1)
                    .Select(a => new FinanceCheck
                    {
                        id = a.id,
                        customer_id = a.customer_id,
                        customer_name = a.customer.name,
                        customer_code = a.customer.code,
                        order_code = a.code,
                        order_name = a.name_cn ?? a.name_en,
                        order_type = "reg_abroad",
                        order_type_name = "境外注册",
                        review_status = a.review_status,
                        status = a.status,
                        salesman = a.member4.name,
                        amount_transaction = a.amount_transaction

                    }).ToList();

                if (abroads.Count() > 0)
                {
                    items.AddRange(abroads);
                }
            }

            #endregion

            #region 国内注册
            var gnId = settings.Where(s => name == "GN_ID").Select(s => s.value).FirstOrDefault();
            if ((string.IsNullOrEmpty(order_type) || order_type == "reg_internal") && (hasCompany != null || userId == gnId))
            {
                Expression<Func<reg_internal, bool>> customerQuery2 = c => true;
                if (customer_id != null && customer_id.Value > 0)
                {
                    customerQuery2 = c => (c.customer_id == customer_id);
                }

                Expression<Func<reg_internal, bool>> salesmanQuery2 = c => true;
                if (salesman_id != null && salesman_id.Value > 0)
                {
                    salesmanQuery2 = c => (c.salesman_id == salesman_id);
                }

                Expression<Func<reg_internal, bool>> orderTypeQuery2 = c => c.status >= 2;
                if (order_status == 0)
                {
                    // 未审核
                    orderTypeQuery2 = c => c.status == 2 && c.review_status == 1;
                }
                else if (order_status == 1)
                {
                    // 已审核
                    orderTypeQuery2 = c => c.status > 2;
                }

                Expression<Func<reg_internal, bool>> nameQuery2 = c => true;
                if (!string.IsNullOrEmpty(name))
                {
                    nameQuery2 = c => (c.name_cn.ToLower().Contains(name.ToLower()) || c.code.ToLower().Contains(name.ToLower()));
                }

                var internas = Uof.Ireg_internalService.GetAll()
                    .Where(customerQuery2)
                    .Where(salesmanQuery2)
                    .Where(orderTypeQuery2)
                    .Where(nameQuery2)
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
                        salesman = a.member5.name,
                        amount_transaction = a.amount_transaction

                    }).ToList();

                if (internas.Count() > 0)
                {
                    items.AddRange(internas);
                }
            }
            #endregion

            #region 商标注册
            var sbId = settings.Where(s => name == "SB_ID").Select(s => s.value).FirstOrDefault();
            if ((string.IsNullOrEmpty(order_type) || order_type == "trademark") && (hasCompany != null || userId == sbId))
            {
                Expression<Func<trademark, bool>> customerQuery3 = c => true;
                if (customer_id != null && customer_id.Value > 0)
                {
                    customerQuery3 = c => (c.customer_id == customer_id);
                }

                Expression<Func<trademark, bool>> salesmanQuery3 = c => true;
                if (salesman_id != null && salesman_id.Value > 0)
                {
                    salesmanQuery3 = c => (c.salesman_id == salesman_id);
                }

                Expression<Func<trademark, bool>> orderTypeQuery3 = c => c.status >= 2;
                if (order_status == 0)
                {
                    // 未审核
                    orderTypeQuery3 = c => c.status == 2 && c.review_status == 1;
                }
                else if (order_status == 1)
                {
                    // 已审核
                    orderTypeQuery3 = c => c.status > 2;
                }

                Expression<Func<trademark, bool>> nameQuery3 = c => true;
                if (!string.IsNullOrEmpty(name))
                {
                    nameQuery3 = c => (c.name.ToLower().Contains(name.ToLower()) || c.code.ToLower().Contains(name.ToLower()));
                }

                var test = Uof.ItrademarkService.GetAll().ToList();
                var trademarks = Uof.ItrademarkService.GetAll()
                    .Where(customerQuery3)
                    .Where(salesmanQuery3)
                    .Where(orderTypeQuery3)
                    .Where(nameQuery3)
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
                        salesman = a.member4.name,
                        amount_transaction = a.amount_transaction

                    }).ToList();

                if (trademarks.Count() > 0)
                {
                    items.AddRange(trademarks);
                }
            }
            #endregion

            #region 专利注册
            var zlId = settings.Where(s => name == "ZL_ID").Select(s => s.value).FirstOrDefault();
            if ((string.IsNullOrEmpty(order_type) || order_type == "patent") && (hasCompany != null || userId != zlId))
            {
                Expression<Func<patent, bool>> customerQuery4 = c => true;
                if (customer_id != null && customer_id.Value > 0)
                {
                    customerQuery4 = c => (c.customer_id == customer_id);
                }

                Expression<Func<patent, bool>> salesmanQuery4 = c => true;
                if (salesman_id != null && salesman_id.Value > 0)
                {
                    salesmanQuery4 = c => (c.salesman_id == salesman_id);
                }

                Expression<Func<patent, bool>> orderTypeQuery4 = c => c.status >= 2;
                if (order_status == 0)
                {
                    // 未审核
                    orderTypeQuery4 = c => c.status == 2 && c.review_status == 1;
                }
                else if (order_status == 1)
                {
                    // 已审核
                    orderTypeQuery4 = c => c.status > 2;
                }

                Expression<Func<patent, bool>> nameQuery4 = c => true;
                if (!string.IsNullOrEmpty(name))
                {
                    nameQuery4 = c => (c.name.ToLower().Contains(name.ToLower()) || c.code.ToLower().Contains(name.ToLower()));
                }

                var patents = Uof.IpatentService.GetAll()
                    .Where(customerQuery4)
                    .Where(salesmanQuery4)
                    .Where(orderTypeQuery4)
                    .Where(nameQuery4)
                    .Select(a => new FinanceCheck
                    {
                        id = a.id,
                        customer_id = a.customer_id,
                        customer_name = a.customer.name,
                        customer_code = a.customer.code,
                        order_code = a.code,
                        order_name = a.name,
                        order_type = "patent",
                        order_type_name = "专利",
                        review_status = a.review_status,
                        status = a.status,
                        salesman = a.member4.name,
                        amount_transaction = a.amount_transaction

                    }).ToList();

                if (patents.Count() > 0)
                {
                    items.AddRange(patents);
                }
            }
            #endregion

            #region 审计
            var jwSjId = settings.Where(s => name == "JWSJ_ID").Select(s => s.value).FirstOrDefault();
            var gnSjId = settings.Where(s => name == "GNSJ_ID").Select(s => s.value).FirstOrDefault();

            if ((string.IsNullOrEmpty(order_type) || order_type == "audit") && (hasCompany != null || jwSjId == userId || gnSjId == userId))
            {
                Expression<Func<audit, bool>> typeQuery5 = c => true;
                if (hasCompany == null && jwSjId != gnSjId)
                {
                    if (jwSjId == userId)
                    {
                        typeQuery5 = c => (c.type == "境外");
                    }
                    if (gnSjId == userId)
                    {
                        typeQuery5 = c => (c.type == "境内");
                    }
                }

                Expression<Func<audit, bool>> customerQuery5 = c => true;
                if (customer_id != null && customer_id.Value > 0)
                {
                    customerQuery5 = c => (c.customer_id == customer_id);
                }

                Expression<Func<audit, bool>> salesmanQuery5 = c => true;
                if (salesman_id != null && salesman_id.Value > 0)
                {
                    salesmanQuery5 = c => (c.salesman_id == salesman_id);
                }

                Expression<Func<audit, bool>> orderTypeQuery5 = c => c.status >= 2;
                if (order_status == 0)
                {
                    // 未审核
                    orderTypeQuery5 = c => c.status == 2 && c.review_status == 1;
                }
                else if (order_status == 1)
                {
                    // 已审核
                    orderTypeQuery5 = c => c.status > 2;
                }

                Expression<Func<audit, bool>> nameQuery5 = c => true;
                if (!string.IsNullOrEmpty(name))
                {
                    nameQuery5 = c => (c.name_cn.ToLower().Contains(name.ToLower()) || c.name_en.ToLower().Contains(name.ToLower()) || c.code.ToLower().Contains(name.ToLower()));
                }

                var audits = Uof.IauditService.GetAll()
                    .Where(customerQuery5)
                    .Where(salesmanQuery5)
                    .Where(orderTypeQuery5)
                    .Where(nameQuery5)
                    .Where(typeQuery5)
                    .Select(a => new FinanceCheck
                    {
                        id = a.id,
                        customer_id = a.customer_id,
                        customer_name = a.customer.name,
                        customer_code = a.customer.code,
                        order_code = a.code,
                        order_name = a.name_cn ?? a.name_en,
                        order_type = "audit",
                        order_type_name = "",
                        review_status = a.review_status,
                        type = a.type,
                        status = a.status,
                        salesman = a.member4.name,
                        amount_transaction = a.amount_transaction

                    }).ToList();

                if (audits.Count() > 0)
                {
                    foreach (var item in audits)
                    {
                        item.order_type_name = item.type == "境外" ? "境外审计" : "国内审计";
                    }
                    items.AddRange(audits);
                }
            }
            #endregion

            #region 年检
            if (string.IsNullOrEmpty(order_type) || order_type == "annual_exam")
            {
                Expression<Func<annual_exam, bool>> customerQuery6 = c => true;
                if (customer_id != null && customer_id.Value > 0)
                {
                    customerQuery6 = c => (c.customer_id == customer_id);
                }

                Expression<Func<annual_exam, bool>> salesmanQuery6 = c => true;
                if (salesman_id != null && salesman_id.Value > 0)
                {
                    salesmanQuery6 = c => (c.waiter_id == salesman_id);
                }

                Expression<Func<annual_exam, bool>> orderTypeQuery6 = c => c.status >= 2;
                if (order_status == 0)
                {
                    // 未审核
                    orderTypeQuery6 = c => c.status == 2 && c.review_status == 1;
                }
                else if (order_status == 1)
                {
                    // 已审核
                    orderTypeQuery6 = c => c.status > 2;
                }

                Expression<Func<annual_exam, bool>> nameQuery6 = c => true;
                if (!string.IsNullOrEmpty(name))
                {
                    nameQuery6 = c => (c.name_cn.ToLower().Contains(name.ToLower()) || c.name_en.ToLower().Contains(name.ToLower()) || c.code.ToLower().Contains(name.ToLower()));
                }

                var audits = Uof.Iannual_examService.GetAll()
                    .Where(customerQuery6)
                    .Where(salesmanQuery6)
                    .Where(orderTypeQuery6)
                    .Where(nameQuery6)
                    .Select(a => new FinanceCheck
                    {
                        id = a.id,
                        customer_id = a.customer_id,
                        customer_name = a.customer.name,
                        customer_code = a.customer.code,
                        order_code = a.code,
                        order_name = a.name_cn ?? a.name_en,
                        order_type = "annual_exam",
                        order_type_name = "年检",
                        review_status = a.review_status,
                        status = a.status,
                        salesman = a.member4.name,
                        amount_transaction = a.amount_transaction

                    }).ToList();

                if (audits.Count() > 0)
                {
                    items.AddRange(audits);
                }
            }
            #endregion

            #region 变更记录
            if (string.IsNullOrEmpty(order_type) || order_type == "history")
            {
                Expression<Func<history, bool>> customerQuery1 = c => true;
                if (customer_id != null && customer_id.Value > 0)
                {
                    customerQuery1 = c => (c.customer_id == customer_id);
                }

                Expression<Func<history, bool>> salesmanQuery1 = c => true;
                if (salesman_id != null && salesman_id.Value > 0)
                {
                    salesmanQuery1 = c => (c.salesman_id == salesman_id);
                }

                Expression<Func<history, bool>> orderTypeQuery1 = c => c.status >= 2;
                if (order_status == 0)
                {
                    // 未审核
                    orderTypeQuery1 = c => c.status == 2 && c.review_status == 1;
                }
                else if (order_status == 1)
                {
                    // 已审核
                    orderTypeQuery1 = c => c.status > 2;
                }

                Expression<Func<history, bool>> nameQuery1 = c => true;
                if (!string.IsNullOrEmpty(name))
                {
                    nameQuery1 = c => (c.order_code.ToLower().Contains(name.ToLower()));
                }

                var historys = Uof.IhistoryService.GetAll()
                    .Where(customerQuery1)
                    .Where(salesmanQuery1)
                    .Where(orderTypeQuery1)
                    .Where(nameQuery1)
                    .Select(a => new FinanceCheck
                    {
                        id = a.id,
                        //customer_id = a.customer_id,
                        //customer_name = a.customer.name,
                        //customer_code = a.customer.code,
                        order_code = a.order_code,
                        //order_name = a.name_cn ?? a.name_en,
                        order_type = "history",
                        order_type_name = "",
                        review_status = a.review_status,
                        status = a.status,
                        source = a.source,
                        salesman = a.member2.name,
                        amount_transaction = a.amount_transaction

                    }).ToList();

                if (historys.Count() > 0)
                {
                    foreach (var item in historys)
                    {
                        item.order_type_name = GetHistorySourceName(item.source);
                    }
                    items.AddRange(historys);
                }
            }

            #endregion

            var result = new
            {
                items = items
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private string GetHistorySourceName(string source)
        {
            switch (source)
            {
                case "reg_abroad":
                    return "境外注册变更";
                case "reg_internal":
                    return "境内注册变更";
                case "trademark":
                    return "商标变更";
                case "patent":
                    return "专利变更";
                default:
                    return "";
            }
        }
    }
}