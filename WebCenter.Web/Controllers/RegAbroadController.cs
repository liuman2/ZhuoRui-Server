using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;
using System.Linq.Expressions;
using System;

namespace WebCenter.Web.Controllers
{
    public class RegAbroadController : BaseController
    {
        public RegAbroadController(IUnitOfWork UOF)
            : base(UOF)
        {

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

            Expression<Func<reg_abroad, bool>> condition = c => c.salesman_id == userId;
            // 客户id
            if (request.customer_id != null && request.customer_id.Value > 0)
            {
                Expression<Func<reg_abroad, bool>> tmp = c => (c.customer_id == request.customer_id);
                condition = tmp;
            }
            // 订单状态
            if (request.status != null)
            {
                if (request.status == 2)
                {
                    Expression<Func<reg_abroad, bool>> tmp = c => (c.status == 2 || c.status == 3);
                    condition = tmp;
                }
                else
                {
                    Expression<Func<reg_abroad, bool>> tmp = c => (c.status == request.status.Value);
                    condition = tmp;
                }                
            }
            // 成交开始日期
            if (request.start_time != null)
            {
                Expression<Func<reg_abroad, bool>> tmp = c => (c.date_transaction >= request.start_time.Value);
                condition = tmp;
            }
            // 成交结束日期
            if (request.end_time != null)
            {
                Expression<Func<reg_abroad, bool>> tmp = c => (c.date_transaction >= request.end_time.Value);
                condition = tmp;
            }


            var list = Uof.Ireg_abroadService.GetAll(condition).OrderBy(item => item.id).Select(c => new
            {
                id = c.id,
                customer_id = c.customer_id,
                customer_name = c.customer.name,
                name_cn = c.name_cn,
                name_en = c.name_en,
                status = c.status,
                review_status = c.review_status,
                date_transaction = c.date_transaction,
                amount_transaction = c.amount_transaction,
                amount_income = 0,
                amount_unreceive = 0,
                salesman_id = c.salesman_id,
                salesman_name = c.member3.name,

            }).ToPagedList(request.index, request.size).ToList();

            var totalRecord = Uof.Ireg_abroadService.GetAll(condition).Count();

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