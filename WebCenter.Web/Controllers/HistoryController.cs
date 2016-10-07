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
    public class HistoryController : BaseController
    {
        public HistoryController(IUnitOfWork UOF)
            : base(UOF)
        {

        }


        public ActionResult Add(history _history, oldRequest oldRequest)
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

            _history.status = 0;
            _history.review_status = -1;
            _history.creator_id = userId;

            var nowYear = DateTime.Now.Year;
            if (oldRequest.is_old == 0)
            {
            }
            else
            {
                _history.status = 4;
                _history.review_status = 1;
            }

            var newHistory = Uof.IhistoryService.AddEntity(_history);
            if (newHistory == null)
            {
                return Json(new { success = false, message = "添加失败" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { id = newHistory.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult List(int source_id, string source, int index, int size)
        {
            var list = Uof.IhistoryService.GetAll(c=>c.source == source && c.source_id == source_id)
                .OrderByDescending(item => item.id).Select(c => new
                {
                    id = c.id,
                    source = c.source,
                    source_id = c.source_id,
                    order_code = c.order_code,
                    customer_id = c.customer_id,
                    value = c.value,

                    status = c.status,
                    review_status = c.review_status,
                    date_transaction = c.date_transaction,
                    amount_transaction = c.amount_transaction,
                    amount_income = 0,
                    amount_unreceive = 0,
                    progress = c.progress,
                    salesman_id = c.salesman_id,
                    salesman_name = c.member2.name,
                    finance_review_moment = c.finance_review_moment,
                    submit_review_moment = c.submit_review_moment

                }).ToPagedList(index, size).ToList();

            var totalRecord = Uof.IhistoryService.GetAll(c => c.source == source && c.source_id == source_id).Count();

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

        public ActionResult GetView(int id)
        {
            var reg = Uof.IhistoryService.GetAll(c => c.id == id).Select(c => new
            {
                id = c.id,
                source = c.source,
                source_id = c.source_id,
                customer_id = c.customer_id,
                order_code = c.order_code,
                value = c.value,
                status = c.status,
                review_status = c.review_status,
                date_transaction = c.date_transaction,
                rate = c.rate,
                currency = c.currency,
                amount_transaction = c.amount_transaction,
                amount_income = 0,
                amount_unreceive = 0,
                progress = c.progress,
                salesman_id = c.salesman_id,
                salesman = c.member2.name,
                finance_review_moment = c.finance_review_moment,
                submit_review_moment = c.submit_review_moment

            }).FirstOrDefault();

            var list = Uof.IincomeService.GetAll(i => i.source_id == reg.id && i.source_name == "history").Select(i => new {
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

            var balance = reg.amount_transaction - total;
            var incomes = new
            {
                items = list,
                total = total,
                balance = balance,

                rate = reg.rate,
                local_amount = reg.amount_transaction * reg.rate,
                local_total = total * reg.rate,
                local_balance = balance * reg.rate
            };

            return Json(new { order = reg, incomes = incomes }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get(int id)
        {
            var reg = Uof.IhistoryService.GetAll(c => c.id == id).Select(c => new
            {
                id = c.id,
                source = c.source,
                source_id = c.source_id,
                customer_id = c.customer_id,
                order_code = c.order_code,
                value = c.value,
                status = c.status,
                review_status = c.review_status,
                date_transaction = c.date_transaction,
                rate = c.rate,
                currency = c.currency,
                amount_transaction = c.amount_transaction,
                amount_income = 0,
                amount_unreceive = 0,
                progress = c.progress,
                salesman_id = c.salesman_id,
                salesman = c.member2.name,
                finance_review_moment = c.finance_review_moment,
                submit_review_moment = c.submit_review_moment

            }).FirstOrDefault();

            
            return Json(reg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Submit(int id)
        {
            var dbReg = Uof.IhistoryService.GetById(id);
            if (dbReg == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }

            dbReg.status = 1;
            dbReg.review_status = -1;
            dbReg.date_updated = DateTime.Now;

            var r = Uof.IhistoryService.UpdateEntity(dbReg);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbReg.id,
                    source_name = "history",
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
                            source = "history",
                            source_id = dbReg.id,
                            user_id = item,
                            router = "history_view",
                            content = "您有数据变更订单需要财务审核",
                            read_status = 0
                        });
                    }

                    Uof.IwaitdealService.AddEntities(waitdeals);
                }
            }
            return Json(new { success = r, message = r ? "" : "更新失败" }, JsonRequestBehavior.AllowGet);
        }
    }
}