using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;

namespace WebCenter.Web.Controllers
{
    public class TrademarkController : BaseController
    {
        public TrademarkController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        public ActionResult Search(TrademarkRequest request)
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

            Expression<Func<trademark, bool>> condition = c => c.salesman_id == userId;
            // 客户id
            if (request.customer_id != null && request.customer_id.Value > 0)
            {
                Expression<Func<trademark, bool>> tmp = c => (c.customer_id == request.customer_id);
                condition = tmp;
            }
            // 订单状态
            if (request.status != null)
            {
                if (request.status == 2)
                {
                    Expression<Func<trademark, bool>> tmp = c => (c.status == 2 || c.status == 3);
                    condition = tmp;
                }
                else
                {
                    Expression<Func<trademark, bool>> tmp = c => (c.status == request.status.Value);
                    condition = tmp;
                }                
            }
            // 成交开始日期
            if (request.start_time != null)
            {
                Expression<Func<trademark, bool>> tmp = c => (c.date_transaction >= request.start_time.Value);
                condition = tmp;
            }
            // 成交结束日期
            if (request.end_time != null)
            {
                var endTime = request.end_time.Value.AddDays(1);
                Expression<Func<trademark, bool>> tmp = c => (c.date_transaction < endTime);
                condition = tmp;
            }

            if (!string.IsNullOrEmpty(request.name))
            {
                Expression<Func<trademark, bool>> tmp = c => (c.name.Contains(request.name));
                condition = tmp;
            }

            if (!string.IsNullOrEmpty(request.applicant))
            {
                Expression<Func<trademark, bool>> tmp = c => (c.applicant.Contains(request.applicant));
                condition = tmp;
            }

            var list = Uof.ItrademarkService.GetAll(condition).OrderByDescending(item => item.id).Select(c => new
            {
                id = c.id,
                code = c.code,
                customer_id = c.customer_id,
                customer_name = c.customer.name,
                type = c.type,
                name = c.name,
                applicant = c.applicant,
                address = c.address,
                trademark_type = c.trademark_type,
                region = c.region,
                reg_mode = c.reg_mode,
                progress = c.progress,
                status = c.status,
                review_status = c.review_status,
                date_transaction = c.date_transaction,
                amount_transaction = c.amount_transaction,
                amount_income = 0,
                amount_unreceive = 0,
                salesman_id = c.salesman_id,
                salesman_name = c.member3.name,

            }).ToPagedList(request.index, request.size).ToList();

            var totalRecord = Uof.ItrademarkService.GetAll(condition).Count();

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

        public ActionResult Add(trademark trade)
        {
            if (trade.customer_id == null)
            {
                return Json(new { success = false, message = "请选择客户" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(trade.name))
            {
                return Json(new { success = false, message = "请填写商标名称" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(trade.applicant))
            {
                return Json(new { success = false, message = "请填写申请人" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(trade.region))
            {
                return Json(new { success = false, message = "请填写商标地区" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(trade.address))
            {
                return Json(new { success = false, message = "请填写申请人地址" }, JsonRequestBehavior.AllowGet);
            }
            if (trade.date_transaction == null)
            {
                return Json(new { success = false, message = "请填写成交日期" }, JsonRequestBehavior.AllowGet);
            }
            if (trade.amount_transaction == null)
            {
                return Json(new { success = false, message = "请填写成交金额" }, JsonRequestBehavior.AllowGet);
            }
            if (trade.waiter_id == null)
            {
                return Json(new { success = false, message = "请选择年检客服" }, JsonRequestBehavior.AllowGet);
            }

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

            trade.code = GetNextOrderCode("SB");

            trade.status = 0;
            trade.review_status = -1;
            trade.creator_id = userId;
            //trade.salesman_id = userId;
            trade.organization_id = organization_id;

            var newTrade = Uof.ItrademarkService.AddEntity(trade);
            if (newTrade == null)
            {
                return Json(new { success = false, message = "添加失败" }, JsonRequestBehavior.AllowGet);
            }

            Uof.ItimelineService.AddEntity(new timeline()
            {
                source_id = newTrade.id,
                source_name = "reg_abroad",
                title = "新建订单",
                content = string.Format("{0}新建了订单, 单号{1}", arrs[3], newTrade.code)
            });

            return Json(new { id = newTrade.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get(int id)
        {
            var reg = Uof.ItrademarkService.GetAll(a => a.id == id).Select(a => new
            {
                id = a.id,
                customer_id = a.customer_id,
                customer_name = a.customer.name,
                industry = a.customer.industry,
                province = a.customer.province,
                city = a.customer.city,
                county = a.customer.county,
                customer_address = a.customer.address,
                contact = a.customer.contact,
                mobile = a.customer.mobile,
                tel = a.customer.tel,

                code = a.code,
                type = a.type,
                name = a.name,
                applicant = a.applicant,
                address = a.address,
                trademark_type = a.trademark_type,
                region = a.region,
                reg_mode = a.reg_mode,
                date_transaction = a.date_transaction,
                amount_transaction = a.amount_transaction,
                currency = a.currency,

                date_receipt = a.date_receipt,
                date_accept = a.date_accept,
                date_trial = a.date_trial,
                date_regit = a.date_regit,
                date_exten = a.date_exten,
                progress = a.progress,

                salesman_id = a.salesman_id,
                salesman = a.member4.name,
                waiter_id = a.waiter_id,
                waiter_name = a.member5.name,
                manager_id = a.manager_id,
                manager_name = a.member2.name,

                status = a.status,
                review_status = a.review_status

            }).FirstOrDefault();

            return Json(reg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetView(int id)
        {
            var reg = Uof.ItrademarkService.GetAll(a => a.id == id).Select(a => new
            {
                id = a.id,
                customer_id = a.customer_id,
                customer_name = a.customer.name,
                industry = a.customer.industry,
                province = a.customer.province,
                city = a.customer.city,
                county = a.customer.county,
                customer_address = a.customer.address,
                contact = a.customer.contact,
                mobile = a.customer.mobile,
                tel = a.customer.tel,

                code = a.code,
                type = a.type,
                name = a.name,
                applicant = a.applicant,
                address = a.address,
                trademark_type = a.trademark_type,
                region = a.region,
                reg_mode = a.reg_mode,
                date_transaction = a.date_transaction,
                amount_transaction = a.amount_transaction,
                currency = a.currency,

                date_receipt = a.date_receipt,
                date_accept = a.date_receipt,
                date_trial = a.date_trial,
                date_regit = a.date_regit,
                date_exten = a.date_exten,
                progress = a.progress,
                date_finish = a.date_finish,

                salesman_id = a.salesman_id,
                salesman = a.member4.name,
                waiter_id = a.waiter_id,
                waiter_name = a.member5.name,
                manager_id = a.manager_id,
                manager_name = a.member2.name,

                status = a.status,
                review_status = a.review_status

            }).FirstOrDefault();

            var list = Uof.IincomeService.GetAll(i => i.source_id == reg.id && i.customer_id == i.customer_id && i.source_name == "reg_abroad").ToList();

            var total = 0f;
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    total += item.amount.Value;
                }
            }

            var incomes = new
            {
                items = list,
                total = total,
                balance = reg.amount_transaction - total
            };

            return Json(new { order = reg, incomes = incomes }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(trademark trade)
        {
            var dbTrade = Uof.ItrademarkService.GetById(trade.id);

            if (trade.customer_id == dbTrade.customer_id &&
                trade.type == dbTrade.type &&
                trade.name == dbTrade.name &&
                trade.applicant == dbTrade.applicant &&
                trade.address == dbTrade.address &&
                trade.trademark_type == dbTrade.trademark_type &&
                trade.region == dbTrade.region &&
                trade.reg_mode == dbTrade.reg_mode &&
                trade.date_transaction == dbTrade.date_transaction &&
                trade.amount_transaction == dbTrade.amount_transaction &&
                trade.currency == dbTrade.currency &&
                trade.date_receipt == dbTrade.date_receipt &&
                trade.date_accept == dbTrade.date_accept &&
                trade.date_trial == dbTrade.date_trial &&
                trade.date_regit == dbTrade.date_regit &&
                trade.date_exten == dbTrade.date_exten &&
                trade.progress == dbTrade.progress &&

                trade.salesman_id == dbTrade.salesman_id &&
                trade.waiter_id == dbTrade.waiter_id &&
                trade.manager_id == dbTrade.manager_id && 
                trade.description == dbTrade.description &&
                trade.currency == dbTrade.currency
                )
            {
                return Json(new { id = trade.id }, JsonRequestBehavior.AllowGet);
            }

            var identityName = HttpContext.User.Identity.Name;
            var arrs = identityName.Split('|');
            if (arrs.Length == 0)
            {
                return new HttpUnauthorizedResult();
            }

            dbTrade.customer_id = trade.customer_id;
            dbTrade.type = trade.type;
            dbTrade.name = trade.name;
            dbTrade.applicant = trade.applicant;
            dbTrade.address = trade.address;
            dbTrade.trademark_type = trade.trademark_type;
            dbTrade.region = trade.region;
            dbTrade.reg_mode = trade.reg_mode;
            dbTrade.date_transaction = trade.date_transaction;
            dbTrade.amount_transaction = trade.amount_transaction;
            dbTrade.currency = trade.currency;
            dbTrade.date_receipt = trade.date_receipt;
            dbTrade.date_accept = trade.date_accept;
            dbTrade.date_trial = trade.date_trial;
            dbTrade.date_regit = trade.date_regit;
            dbTrade.date_exten = trade.date_exten;
            dbTrade.progress = trade.progress;

            dbTrade.salesman_id = trade.salesman_id;
            dbTrade.waiter_id = trade.waiter_id;
            dbTrade.manager_id = trade.manager_id;
            dbTrade.description = trade.description;


            dbTrade.date_updated = DateTime.Now;

            var r = Uof.ItrademarkService.UpdateEntity(dbTrade);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbTrade.id,
                    source_name = "trademark",
                    title = "修改订单资料",
                    content = string.Format("{0}修改了订单资料", arrs[3])
                });
            }
            return Json(new { success = r, id = dbTrade.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Submit(int id)
        {
            var dbTrade = Uof.ItrademarkService.GetById(id);
            if (dbTrade == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }

            dbTrade.status = 1;
            dbTrade.date_updated = DateTime.Now;

            var r = Uof.ItrademarkService.UpdateEntity(dbTrade);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbTrade.id,
                    source_name = "trademark",
                    title = "提交审核",
                    content = string.Format("提交给财务审核")
                });

                // TODO 通知 财务人员
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

            var dbTrade = Uof.ItrademarkService.GetById(id);
            if (dbTrade == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }
            var t = "";
            if (dbTrade.status == 1)
            {
                dbTrade.status = 2;
                dbTrade.review_status = 1;
                dbTrade.finance_reviewer_id = userId;
                dbTrade.finance_review_date = DateTime.Now;
                dbTrade.finance_review_moment = "";

                t = "财务审核";
                // TODO 通知 提交人，业务员
            }
            else
            {
                dbTrade.status = 3;
                dbTrade.review_status = 1;
                dbTrade.submit_reviewer_id = userId;
                dbTrade.submit_review_date = DateTime.Now;
                dbTrade.submit_review_moment = "";

                t = "提交的审核";
                // TODO 通知 业务员
            }

            dbTrade.date_updated = DateTime.Now;

            var r = Uof.ItrademarkService.UpdateEntity(dbTrade);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbTrade.id,
                    source_name = "trademark",
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

            var dbTrade = Uof.ItrademarkService.GetById(id);
            if (dbTrade == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }
            var t = "";
            if (dbTrade.status == 1)
            {
                dbTrade.status = 2;
                dbTrade.review_status = 0;
                dbTrade.finance_reviewer_id = userId;
                dbTrade.finance_review_date = DateTime.Now;
                dbTrade.finance_review_moment = description;

                t = "驳回了财务审核";
                // TODO 通知 业务员
            }
            else
            {
                dbTrade.status = 3;
                dbTrade.review_status = 0;
                dbTrade.submit_reviewer_id = userId;
                dbTrade.submit_review_date = DateTime.Now;
                dbTrade.submit_review_moment = description;

                t = "驳回了提交的审核";
                // TODO 通知 业务员
            }

            dbTrade.date_updated = DateTime.Now;

            var r = Uof.ItrademarkService.UpdateEntity(dbTrade);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbTrade.id,
                    source_name = "trademark",
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

            var dbReg = Uof.ItrademarkService.GetById(id);
            if (dbReg == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }
            dbReg.status = 4;
            dbReg.date_updated = DateTime.Now;
            dbReg.date_finish = date_finish;
            dbReg.progress = "已完成";
            var r = Uof.ItrademarkService.UpdateEntity(dbReg);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbReg.id,
                    source_name = "trademark",
                    title = "完成订单",
                    content = string.Format("{0}完成了订单，完成日期为：{1}", arrs[3], date_finish.ToString("yyyy-MM-dd"))
                });

                // TODO 通知 业务员
            }

            return Json(new { success = r, message = r ? "" : "保存失败" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProgress(int id)
        {
            var p = Uof.ItrademarkService.GetAll(r => r.id == id).Select(r => new
            {
                id = r.id,
                name = r.progress,
                date_accept = r.date_accept,
                date_receipt = r.date_receipt,
                date_trial = r.date_trial,
                date_regit = r.date_regit,
                date_exten = r.date_exten
            });

            return Json(p, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateProgress(TtrademarkProgressRequest request)
        {
            var dbtrademark = Uof.ItrademarkService.GetById(request.id);
            if (dbtrademark == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }

            if (dbtrademark.progress == request.name &&
                dbtrademark.date_accept == request.date_accept &&
                dbtrademark.date_receipt == request.date_receipt &&
                dbtrademark.date_trial == request.date_trial &&
                dbtrademark.date_regit == request.date_regit &&
                dbtrademark.date_exten == request.date_exten)
            {
                return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);
            }

            dbtrademark.progress = request.name;
            dbtrademark.date_accept = request.date_accept;
            dbtrademark.date_receipt = request.date_receipt;
            dbtrademark.date_trial = request.date_trial;
            dbtrademark.date_regit = request.date_regit;
            dbtrademark.date_exten = request.date_exten;

            var r = Uof.ItrademarkService.UpdateEntity(dbtrademark);

            return Json(new { success = r, message = r ? "" : "更新失败" }, JsonRequestBehavior.AllowGet);
        }
    }
}