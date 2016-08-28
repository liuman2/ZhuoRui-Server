﻿using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;

namespace WebCenter.Web.Controllers
{
    public class AuditController : BaseController
    {
        public AuditController(IUnitOfWork UOF)
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

            Expression<Func<audit, bool>> condition = c => c.salesman_id == userId;

            // 客户id
            Expression<Func<audit, bool>> customerQuery = c => true;
            if (request.customer_id != null && request.customer_id.Value > 0)
            {
                customerQuery = c => (c.customer_id == request.customer_id);
            }

            // 订单状态
            Expression<Func<audit, bool>> statusQuery = c => true;
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
            Expression<Func<audit, bool>> date1Query = c => true;
            Expression<Func<audit, bool>> date2Query = c => true;
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

            var list = Uof.IauditService.GetAll(condition)
                .Where(customerQuery)
                .Where(statusQuery)
                .Where(date1Query)
                .Where(date2Query)
                .OrderByDescending(item => item.id).Select(c => new
            {
                id = c.id,
                code = c.code,
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
                progress = c.progress,
                salesman_id = c.salesman_id,
                salesman_name = c.member3.name,
                finance_review_moment = c.finance_review_moment,
                submit_review_moment = c.submit_review_moment

                }).ToPagedList(request.index, request.size).ToList();

            var totalRecord = Uof.IauditService.GetAll(condition).Count();

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

        public ActionResult Add(audit _audit)
        {
            if (_audit.customer_id == null)
            {
                return Json(new { success = false, message = "请选择客户" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(_audit.name_cn))
            {
                return Json(new { success = false, message = "请填写公司中文名称" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(_audit.name_en))
            {
                return Json(new { success = false, message = "请填写公司英文名称" }, JsonRequestBehavior.AllowGet);
            }
            if (_audit.date_setup == null)
            {
                return Json(new { success = false, message = "请填写公司成立日期" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(_audit.address))
            {
                return Json(new { success = false, message = "请填写公司注册地址" }, JsonRequestBehavior.AllowGet);
            }
            if (_audit.date_transaction == null)
            {
                return Json(new { success = false, message = "请填写成交日期" }, JsonRequestBehavior.AllowGet);
            }
            if (_audit.amount_transaction == null)
            {
                return Json(new { success = false, message = "请填写成交金额" }, JsonRequestBehavior.AllowGet);
            }
            if (_audit.accountant_id == null)
            {
                return Json(new { success = false, message = "请选择会计" }, JsonRequestBehavior.AllowGet);
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

            _audit.code = GetNextOrderCode("SJ");

            _audit.status = 0;
            _audit.review_status = -1;
            _audit.creator_id = userId;
            //_audit.salesman_id = userId;
            _audit.organization_id = organization_id;
            
            var newAbroad = Uof.IauditService.AddEntity(_audit);
            if (newAbroad == null)
            {
                return Json(new { success = false, message = "添加失败" }, JsonRequestBehavior.AllowGet);
            }

            Uof.ItimelineService.AddEntity(new timeline()
            {
                source_id = newAbroad.id,
                source_name = "audit",
                title = "新建订单",
                content = string.Format("{0}新建了订单, 单号{1}", arrs[3], _audit.code)
            });

            return Json(new { id = newAbroad.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get(int id)
        {
            var reg = Uof.IauditService.GetAll(a => a.id == id).Select(a => new
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
                name_cn = a.name_cn,
                name_en = a.name_en,
                date_setup = a.date_setup,
                business_area = a.business_area,
                trade_mode = a.trade_mode,
                has_parent = a.has_parent,
                account_number = a.account_number,
                account_period = a.account_period,
                date_year_end = a.date_year_end,
                turnover = a.turnover,
                amount_bank = a.amount_bank,
                bill_number = a.bill_number,
                accounting_standard = a.accounting_standard,
                cost_accounting = a.cost_accounting,
                progress = a.progress,
                
                address = a.address,
                date_transaction = a.date_transaction,
                amount_transaction = a.amount_transaction,
                date_finish = a.date_finish,
                currency = a.currency,

                salesman_id = a.salesman_id,
                salesman = a.member4.name,
                accountant_id = a.accountant_id,
                accountant_name = a.member.name,
                manager_id = a.manager_id,
                manager_name = a.member2.name,

                status = a.status,
                review_status = a.review_status

            }).FirstOrDefault();

            return Json(reg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetView(int id)
        {
            var reg = Uof.IauditService.GetAll(a => a.id == id).Select(a => new
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
                name_cn = a.name_cn,
                name_en = a.name_en,
                date_setup = a.date_setup,
                business_area = a.business_area,
                trade_mode = a.trade_mode,
                has_parent = a.has_parent,
                account_number = a.account_number,
                account_period = a.account_period,
                date_year_end = a.date_year_end,
                turnover = a.turnover,
                amount_bank = a.amount_bank,
                bill_number = a.bill_number,
                accounting_standard = a.accounting_standard,
                cost_accounting = a.cost_accounting,
                progress = a.progress,
                
                address = a.address,
                date_transaction = a.date_transaction,
                amount_transaction = a.amount_transaction,
                date_finish = a.date_finish,
                currency = a.currency,

                salesman_id = a.salesman_id,
                salesman = a.member4.name,
                accountant_id = a.accountant_id,
                accountant_name = a.member.name,
                manager_id = a.manager_id,
                manager_name = a.member2.name,

                status = a.status,
                review_status = a.review_status,
                finance_review_moment = a.finance_review_moment,
                submit_review_moment = a.submit_review_moment

            }).FirstOrDefault();

            var list = Uof.IincomeService.GetAll(i => i.source_id == reg.id && i.customer_id == i.customer_id && i.source_name == "audit").ToList();

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

        public ActionResult Update(audit _audit)
        {
            var dbAudit = Uof.IauditService.GetById(_audit.id);

            if (_audit.customer_id == dbAudit.customer_id &&
                _audit.name_cn == dbAudit.name_cn &&
                _audit.name_en == dbAudit.name_en &&
                _audit.date_setup == dbAudit.date_setup &&
                _audit.type == dbAudit.type &&
                _audit.address == dbAudit.address &&
                _audit.business_area == dbAudit.business_area &&
                _audit.trade_mode == dbAudit.trade_mode &&
                _audit.has_parent == dbAudit.has_parent &&
                _audit.account_number == dbAudit.account_number &&
                _audit.account_period == dbAudit.account_period &&
                _audit.date_year_end == dbAudit.date_year_end &&
                _audit.turnover == dbAudit.turnover &&
                _audit.amount_bank == dbAudit.amount_bank &&
                _audit.bill_number == dbAudit.bill_number &&
                _audit.accounting_standard == dbAudit.accounting_standard &&
                _audit.cost_accounting == dbAudit.cost_accounting &&
                _audit.progress == dbAudit.progress &&
                _audit.date_transaction == dbAudit.date_transaction &&
                _audit.amount_transaction == dbAudit.amount_transaction &&
                _audit.accountant_id == dbAudit.accountant_id &&
                _audit.manager_id == dbAudit.manager_id &&
                _audit.salesman_id == dbAudit.salesman_id &&
                _audit.description == dbAudit.description &&
                _audit.currency == dbAudit.currency
                )
            {
                return Json(new { id = _audit.id }, JsonRequestBehavior.AllowGet);
            }

            var identityName = HttpContext.User.Identity.Name;
            var arrs = identityName.Split('|');
            if (arrs.Length == 0)
            {
                return new HttpUnauthorizedResult();
            }

            dbAudit.customer_id = _audit.customer_id;
            dbAudit.name_cn = _audit.name_cn;
            dbAudit.name_en = _audit.name_en;
            dbAudit.date_setup = _audit.date_setup;
            dbAudit.address = _audit.address;
            dbAudit.type = _audit.type;
            dbAudit.business_area = _audit.business_area;
            dbAudit.trade_mode = _audit.trade_mode;
            dbAudit.has_parent = _audit.has_parent;
            dbAudit.account_number = _audit.account_number;
            dbAudit.account_period = _audit.account_period;
            dbAudit.date_year_end = _audit.date_year_end;
            dbAudit.turnover = _audit.turnover;
            dbAudit.amount_bank = _audit.amount_bank;
            dbAudit.bill_number = _audit.bill_number;
            dbAudit.accounting_standard = _audit.accounting_standard;
            dbAudit.cost_accounting = _audit.cost_accounting;
            dbAudit.progress = _audit.progress;
            dbAudit.date_transaction = _audit.date_transaction;
            dbAudit.amount_transaction = _audit.amount_transaction;
            dbAudit.accountant_id = _audit.accountant_id;
            dbAudit.manager_id = _audit.manager_id;
            dbAudit.salesman_id = _audit.salesman_id;
            dbAudit.description = _audit.description;
            dbAudit.currency = _audit.currency;
            dbAudit.date_updated = DateTime.Now;

            var r = Uof.IauditService.UpdateEntity(dbAudit);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbAudit.id,
                    source_name = "audit",
                    title = "修改订单资料",
                    content = string.Format("{0}修改了订单资料", arrs[3])
                });
            }
            return Json(new { success = r, id = _audit.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Submit(int id)
        {
            var dbAudit = Uof.IauditService.GetById(id);
            if (dbAudit == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }

            dbAudit.status = 1;
            dbAudit.review_status = -1;
            dbAudit.date_updated = DateTime.Now;

            var r = Uof.IauditService.UpdateEntity(dbAudit);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbAudit.id,
                    source_name = "audit",
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

            var dbAudit = Uof.IauditService.GetById(id);
            if (dbAudit == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }
            var t = "";
            if (dbAudit.status == 1)
            {
                dbAudit.status = 2;
                dbAudit.review_status = 1;
                dbAudit.finance_reviewer_id = userId;
                dbAudit.finance_review_date = DateTime.Now;
                dbAudit.finance_review_moment = "";

                t = "财务审核";
                // TODO 通知 提交人，业务员
            }
            else
            {
                dbAudit.status = 3;
                dbAudit.review_status = 1;
                dbAudit.submit_reviewer_id = userId;
                dbAudit.submit_review_date = DateTime.Now;
                dbAudit.submit_review_moment = "";

                t = "提交的审核";
                // TODO 通知 业务员
            }

            dbAudit.date_updated = DateTime.Now;

            var r = Uof.IauditService.UpdateEntity(dbAudit);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbAudit.id,
                    source_name = "audit",
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

            var dbAudit = Uof.IauditService.GetById(id);
            if (dbAudit == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }
            var t = "";
            if (dbAudit.status == 1)
            {
                dbAudit.status = 0;
                dbAudit.review_status = 0;
                dbAudit.finance_reviewer_id = userId;
                dbAudit.finance_review_date = DateTime.Now;
                dbAudit.finance_review_moment = description;

                t = "驳回了财务审核";
                // TODO 通知 业务员
            }
            else
            {
                dbAudit.status = 0;
                dbAudit.review_status = 0;
                dbAudit.submit_reviewer_id = userId;
                dbAudit.submit_review_date = DateTime.Now;
                dbAudit.submit_review_moment = description;

                t = "驳回了提交的审核";
                // TODO 通知 业务员
            }

            dbAudit.date_updated = DateTime.Now;

            var r = Uof.IauditService.UpdateEntity(dbAudit);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbAudit.id,
                    source_name = "reg_abroad",
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

            var dbAudit = Uof.IauditService.GetById(id);
            if (dbAudit == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }
            dbAudit.status = 4;
            dbAudit.date_updated = DateTime.Now;
            dbAudit.date_finish = date_finish;
            dbAudit.progress = "已完成";
            var r = Uof.IauditService.UpdateEntity(dbAudit);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbAudit.id,
                    source_name = "audit",
                    title = "完成订单",
                    content = string.Format("{0}完成了订单，完成日期为：{1}", arrs[3], date_finish.ToString("yyyy-MM-dd"))
                });

                // TODO 通知 业务员
            }

            return Json(new { success = r, message = r ? "" : "保存失败" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProgress(int id)
        {
            var p = Uof.IauditService.GetAll(r => r.id == id).Select(r => new
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

            var dbAudit = Uof.IauditService.GetById(request.id);
            if (dbAudit == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }

            if (request.is_done == 1)
            {
                dbAudit.status = 4;
                dbAudit.date_updated = DateTime.Now;
                dbAudit.date_finish = request.date_finish;
                dbAudit.progress = request.progress ?? "已完成";
            }
            else
            {
                if (dbAudit.progress == request.progress)
                {
                    return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);
                }

                dbAudit.progress = request.progress;
            }

            var r = Uof.IauditService.UpdateEntity(dbAudit);

            if (r)
            {
                if (request.is_done == 1)
                {
                    Uof.ItimelineService.AddEntity(new timeline()
                    {
                        source_id = dbAudit.id,
                        source_name = "audit",
                        title = "完成订单",
                        content = string.Format("{0}完成了订单，完成日期为：{1}", arrs[3], dbAudit.date_finish.Value.ToString("yyyy-MM-dd"))
                    });
                    // TODO 通知 业务员
                }
                else
                {
                    Uof.ItimelineService.AddEntity(new timeline()
                    {
                        source_id = dbAudit.id,
                        source_name = "audit",
                        title = "更新了订单进度",
                        content = string.Format("{0}更新了进度: {1}", arrs[3], dbAudit.progress)
                    });
                }
            }
            
            return Json(new { success = r, message = r ? "" : "更新失败" }, JsonRequestBehavior.AllowGet);
        }
    }
}