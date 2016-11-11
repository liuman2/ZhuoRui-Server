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


            var result = new
            {
                items = items
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SubmitCheck(int? customer_id, int? salesman_id, int? order_status, string order_type)
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

                Expression<Func<reg_internal, bool>> orderTypeQuery1 = c => c.status >= 2;
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

                Expression<Func<trademark, bool>> orderTypeQuery1 = c => c.status >= 2;
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

                Expression<Func<patent, bool>> orderTypeQuery1 = c => c.status >= 2;
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

                Expression<Func<audit, bool>> orderTypeQuery1 = c => c.status >= 2;
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

                Expression<Func<annual_exam, bool>> orderTypeQuery1 = c => c.status >= 2;
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


            var result = new
            {
                items = items
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}