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
    public class AuditSubController : BaseController
    {
        public AuditSubController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        [HttpPost]
        public ActionResult Add(sub_audit subAudit)
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

            var custId = Uof.IauditService.GetAll(a => a.id == subAudit.master_id).Select(a => a.customer_id).FirstOrDefault();

            var userId = 0;
            var organization_id = 0;
            int.TryParse(arrs[0], out userId);
            int.TryParse(arrs[2], out organization_id);

            subAudit.status = 0;
            subAudit.review_status = -1;
            subAudit.creator_id = userId;
            subAudit.customer_id = custId;

            if (subAudit.customer_id != null)
            {
                var salesman_id = Uof.IcustomerService.GetAll(c => c.id == subAudit.customer_id).Select(c => c.salesman_id).FirstOrDefault();
                if (salesman_id != null)
                {
                    subAudit.salesman_id = salesman_id;
                }
            }

            var newAbroad = Uof.Isub_auditService.AddEntity(subAudit);
            if (newAbroad == null)
            {
                return Json(new { success = false, message = "添加失败" }, JsonRequestBehavior.AllowGet);
            }

            var timelines = new List<timeline>();
            timelines.Add(new timeline
            {
                source_id = subAudit.master_id,
                source_name = "audit",
                title = "新增账期",
                is_system = 1,
                content = string.Format("{0}新增了账期", arrs[3])
            });

            timelines.Add(new timeline
            {
                source_id = subAudit.id,
                source_name = "sub_audit",
                title = "新增账期",
                is_system = 1,
                content = string.Format("{0}新增了账期", arrs[3])
            });
            try
            {
                Uof.ItimelineService.AddEntities(timelines);
            }
            catch (Exception)
            {
            }
            return Json(new { id = newAbroad.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get(int id)
        {
            var sub = Uof.Isub_auditService.GetAll(s => s.id == id).Select(a => new SubAudit
            {
                id = a.id,
                master_id = a.master_id,
                customer_id = a.customer_id,
                customer_name = a.customer.name,
                turnover_currency = a.turnover_currency,
                account_period = a.account_period,
                account_period2 = a.account_period2,
                date_year_end = a.date_year_end,
                turnover = a.turnover,
                amount_bank = a.amount_bank,
                bill_number = a.bill_number,
                accounting_standard = a.accounting_standard,
                cost_accounting = a.cost_accounting,
                progress = a.progress,
                date_transaction = a.date_transaction,
                amount_transaction = a.amount_transaction,
                date_finish = a.date_finish,
                currency = a.currency,
                rate = a.rate,
                //salesman_id = a.salesman_id,
                //salesman = a.member4.name,

                salesman_id = a.customer.salesman_id,
                salesman = a.customer.member1.name,

                accountant_id = a.accountant_id,
                accountant_name = a.member.name,
                manager_id = a.manager_id,
                manager_name = a.member3.name,
                assistant_id = a.assistant_id,
                status = a.status,
                review_status = a.review_status,
                description = a.description,
                finance_review_moment = a.finance_review_moment,
                submit_review_moment = a.submit_review_moment,

                trader_id = a.trader_id,
                trader_name = a.customer1.name,
                creator = a.member1.name,

            }).FirstOrDefault();

            return Json(sub, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(sub_audit _audit)
        {
            var dbAudit = Uof.Isub_auditService.GetById(_audit.id);

            //if (_audit.account_period == dbAudit.account_period &&
            //    _audit.account_period2 == dbAudit.account_period2 &&
            //    _audit.date_year_end == dbAudit.date_year_end &&
            //    _audit.turnover == dbAudit.turnover &&
            //    _audit.amount_bank == dbAudit.amount_bank &&
            //    _audit.bill_number == dbAudit.bill_number &&
            //    _audit.accounting_standard == dbAudit.accounting_standard &&
            //    _audit.cost_accounting == dbAudit.cost_accounting &&
            //    _audit.progress == dbAudit.progress &&
            //    _audit.date_transaction == dbAudit.date_transaction &&
            //    _audit.amount_transaction == dbAudit.amount_transaction &&
            //    _audit.accountant_id == dbAudit.accountant_id &&
            //    _audit.turnover_currency == dbAudit.turnover_currency &&
            //    _audit.manager_id == dbAudit.manager_id &&
            //    _audit.salesman_id == dbAudit.salesman_id &&
            //    _audit.description == dbAudit.description &&
            //    _audit.currency == dbAudit.currency &&
            //    _audit.rate == dbAudit.rate &&
            //    _audit.assistant_id == dbAudit.assistant_id
            //    )
            //{
            //    return Json(new { id = _audit.id }, JsonRequestBehavior.AllowGet);
            //}

            var identityName = HttpContext.User.Identity.Name;
            var arrs = identityName.Split('|');
            if (arrs.Length == 0)
            {
                return new HttpUnauthorizedResult();
            }

            var isChangeCurrency = _audit.currency != dbAudit.currency || _audit.rate != dbAudit.rate;

            dbAudit.account_period = _audit.account_period;
            dbAudit.account_period2 = _audit.account_period2;
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
            dbAudit.rate = _audit.rate;
            dbAudit.date_updated = DateTime.Now;
            dbAudit.turnover_currency = _audit.turnover_currency;
            dbAudit.assistant_id = _audit.assistant_id;

            dbAudit.trader_id = _audit.trader_id;

            var r = Uof.Isub_auditService.UpdateEntity(dbAudit);

            if (r)
            {
                //if (isChangeCurrency)
                //{
                //    var list = Uof.IincomeService.GetAll(i => i.source_id == _audit.id && i.source_name == "sub_audit").ToList();
                //    if (list.Count() > 0)
                //    {
                //        foreach (var item in list)
                //        {
                //            item.currency = _audit.currency;
                //            item.rate = _audit.rate;
                //        }

                //        Uof.IincomeService.UpdateEntities(list);
                //    }
                //}

                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbAudit.id,
                    source_name = "sub_audit",
                    title = "修改账期",
                    is_system = 1,
                    content = string.Format("{0}修改了账期", arrs[3])
                });
            }
            return Json(new { success = r, id = _audit.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Submit(int id)
        {
            var dbAudit = Uof.Isub_auditService.GetById(id);
            if (dbAudit == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }

            dbAudit.status = 1;
            dbAudit.review_status = -1;
            dbAudit.date_updated = DateTime.Now;

            var r = Uof.Isub_auditService.UpdateEntity(dbAudit);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbAudit.id,
                    source_name = "sub_audit",
                    title = "提交审核",
                    is_system = 1,
                    content = string.Format("提交给财务审核")
                });

                var auditor_id = GetAuditorByKey("CW_ID");
                if (auditor_id != null)
                {
                    var waitdeals = new List<waitdeal>();
                    waitdeals.Add(new waitdeal
                    {
                        source = "sub_audit",
                        source_id = dbAudit.master_id,
                        user_id = auditor_id,
                        router = "audit_view",
                        content = "您有审计账期需要财务审核",
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

            var dbAudit = Uof.Isub_auditService.GetById(id);
            if (dbAudit == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }

            var t = "";
            var waitdeals = new List<waitdeal>();
            if (dbAudit.status == 1)
            {
                dbAudit.status = 2;
                dbAudit.review_status = 1;
                dbAudit.finance_reviewer_id = userId;
                dbAudit.finance_review_date = DateTime.Now;
                dbAudit.finance_review_moment = "";

                t = "财务审核";
                waitdeals.Add(new waitdeal
                {
                    source = "sub_audit",
                    source_id = dbAudit.master_id,
                    user_id = dbAudit.salesman_id,
                    router = "audit_view",
                    content = "您的审计账期已通过财务审核",
                    read_status = 0
                });

                if (dbAudit.assistant_id != null && dbAudit.assistant_id != dbAudit.salesman_id)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "sub_audit",
                        source_id = dbAudit.master_id,
                        user_id = dbAudit.assistant_id,
                        router = "audit_view",
                        content = "您的审计账期已通过财务审核",
                        read_status = 0
                    });
                }
                // TODO:
                var key = "JWSJ_ID"; // dbAudit.type == "境外" ? "JWSJ_ID" : "GNSJ_ID";
                var jwId = GetSubmitMemberByKey(key);
                if (jwId != null && jwId > 0)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "sub_audit",
                        source_id = dbAudit.master_id,
                        user_id = jwId,
                        router = "audit_view",
                        content = "您有审计账期需要提交审核",
                        read_status = 0
                    });
                }                
            }
            else
            {
                dbAudit.status = 3;
                dbAudit.review_status = 1;
                dbAudit.submit_reviewer_id = userId;
                dbAudit.submit_review_date = DateTime.Now;
                dbAudit.submit_review_moment = "";

                t = "提交的审核";
                waitdeals.Add(new waitdeal
                {
                    source = "sub_audit",
                    source_id = dbAudit.master_id,
                    user_id = dbAudit.salesman_id,
                    router = "audit_view",
                    content = "您的审计账期已通过提交审核",
                    read_status = 0
                });

                if (dbAudit.assistant_id != null && dbAudit.assistant_id != dbAudit.salesman_id)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "sub_audit",
                        source_id = dbAudit.master_id,
                        user_id = dbAudit.assistant_id,
                        router = "audit_view",
                        content = "您的审计订单已通过提交审核",
                        read_status = 0
                    });
                }
            }

            dbAudit.date_updated = DateTime.Now;

            var r = Uof.Isub_auditService.UpdateEntity(dbAudit);

            if (r)
            {
                Uof.IwaitdealService.AddEntities(waitdeals);

                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbAudit.id,
                    source_name = "sub_audit",
                    title = "通过审核",
                    is_system = 1,
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

            var dbAudit = Uof.Isub_auditService.GetById(id);
            if (dbAudit == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }
            var t = "";
            var waitdeals = new List<waitdeal>();
            if (dbAudit.status == 1)
            {
                dbAudit.status = 0;
                dbAudit.review_status = 0;
                dbAudit.finance_reviewer_id = userId;
                dbAudit.finance_review_date = DateTime.Now;
                dbAudit.finance_review_moment = description;

                t = "驳回了财务审核";
                waitdeals.Add(new waitdeal
                {
                    source = "sub_audit",
                    source_id = dbAudit.master_id,
                    user_id = dbAudit.salesman_id,
                    router = "audit_view",
                    content = "您的审计账期未通过财务审核",
                    read_status = 0
                });

                if (dbAudit.assistant_id != null && dbAudit.assistant_id != dbAudit.salesman_id)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "sub_audit",
                        source_id = dbAudit.master_id,
                        user_id = dbAudit.assistant_id,
                        router = "audit_view",
                        content = "您的审计账期未通过财务审核",
                        read_status = 0
                    });
                }
            }
            else
            {
                dbAudit.status = 0;
                dbAudit.review_status = 0;
                dbAudit.submit_reviewer_id = userId;
                dbAudit.submit_review_date = DateTime.Now;
                dbAudit.submit_review_moment = description;

                t = "驳回了提交的审核";
                waitdeals.Add(new waitdeal
                {
                    source = "sub_audit",
                    source_id = dbAudit.master_id,
                    user_id = dbAudit.salesman_id,
                    router = "audit_view",
                    content = "您的审计账期未通过提交审核",
                    read_status = 0
                });
                if (dbAudit.assistant_id != null && dbAudit.assistant_id != dbAudit.salesman_id)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "sub_audit",
                        source_id = dbAudit.master_id,
                        user_id = dbAudit.assistant_id,
                        router = "audit_view",
                        content = "您的审计账期未通过提交审核",
                        read_status = 0
                    });
                }
            }

            dbAudit.date_updated = DateTime.Now;

            var r = Uof.Isub_auditService.UpdateEntity(dbAudit);

            if (r)
            {
                Uof.IwaitdealService.AddEntities(waitdeals);

                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbAudit.id,
                    source_name = "sub_audit",
                    title = "驳回审核",
                    is_system = 1,
                    content = string.Format("{0}{1}, 驳回理由: {2}", arrs[3], t, description)
                });
            }

            return Json(new { success = r, message = r ? "" : "审核失败" }, JsonRequestBehavior.AllowGet);
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

            var dbAudit = Uof.Isub_auditService.GetById(request.id);
            if (dbAudit == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }

            if (request.progress_type != "p")
            {
                dbAudit.status = 4;
                dbAudit.date_updated = DateTime.Now;
                if (dbAudit.date_finish == null)
                {
                    dbAudit.date_finish = request.date_finish ?? DateTime.Today;
                }
            }
            else
            {
                if (dbAudit.progress == request.progress && dbAudit.date_finish == request.date_finish && dbAudit.accountant_id == request.accountant_id)
                {
                    return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);
                }
                dbAudit.status = 4;
                dbAudit.date_finish = request.date_finish;
                dbAudit.progress = request.progress;
                dbAudit.accountant_id = request.accountant_id;
            }

            var r = Uof.Isub_auditService.UpdateEntity(dbAudit);

            if (r)
            {
                if (request.progress_type != "p")
                {
                    Uof.ItimelineService.AddEntity(new timeline()
                    {
                        source_id = dbAudit.id,
                        source_name = "sub_audit",
                        title = "完善了审计资料",
                        is_system = 1,
                        content = string.Format("{0}完善了注册资料", arrs[3])
                    });

                    var waitdeals = new List<waitdeal>();
                    waitdeals.Add(new waitdeal
                    {
                        source = "sub_audit",
                        source_id = dbAudit.master_id,
                        user_id = dbAudit.salesman_id,
                        router = "audit_view",
                        content = "您的审计订单已完成",
                        read_status = 0
                    });
                    if (dbAudit.assistant_id != null && dbAudit.assistant_id != dbAudit.salesman_id)
                    {
                        waitdeals.Add(new waitdeal
                        {
                            source = "sub_audit",
                            source_id = dbAudit.master_id,
                            user_id = dbAudit.assistant_id,
                            router = "audit_view",
                            content = "您的审计订单已完成",
                            read_status = 0
                        });
                    }

                    Uof.IwaitdealService.AddEntities(waitdeals);
                }
                else
                {
                    Uof.ItimelineService.AddEntity(new timeline()
                    {
                        source_id = dbAudit.id,
                        source_name = "sub_audit",
                        title = "更新了订单进度",
                        is_system = 1,
                        content = string.Format("{0}更新了进度: {1} 预计完成日期 {2}", arrs[3], dbAudit.progress, dbAudit.date_finish.Value.ToString("yyyy-MM-dd"))
                    });

                    var waitdeals = new List<waitdeal>();
                    waitdeals.Add(new waitdeal
                    {
                        source = "sub_audit",
                        source_id = dbAudit.master_id,
                        user_id = dbAudit.salesman_id,
                        router = "audit_view",
                        content = "您的审计订单更新了进度",
                        read_status = 0
                    });
                    if (dbAudit.assistant_id != null && dbAudit.assistant_id != dbAudit.salesman_id)
                    {
                        waitdeals.Add(new waitdeal
                        {
                            source = "sub_audit",
                            source_id = dbAudit.master_id,
                            user_id = dbAudit.assistant_id,
                            router = "audit_view",
                            content = "您的审计订单更新了进度",
                            read_status = 0
                        });
                    }

                    Uof.IwaitdealService.AddEntities(waitdeals);

                }
            }

            return Json(new { success = r, message = r ? "" : "更新失败" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProgress(int id)
        {
            var p = Uof.Isub_auditService.GetAll(r => r.id == id).Select(r => new
            {
                id = r.id,
                customer_id = r.customer_id,
                is_done = r.status == 4 ? 1 : 0,
                date_finish = r.date_finish,
                progress = r.progress,
                accountant_id = r.accountant_id,
                accountant_name = r.member.name

            }).FirstOrDefault();

            return Json(p, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetIncomes(int id)
        {
            var reg = Uof.Isub_auditService.GetAll(s => s.id == id).Select(s=> new
            {
                amount_transaction = s.amount_transaction,
                currency = s.currency,
                rate = s.rate ?? 1,

            }).FirstOrDefault();

            var list = Uof.IincomeService.GetAll(i => i.source_id == id && i.source_name == "sub_audit").Select(i => new {
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

            var balance = (reg.amount_transaction * reg.rate) - total;
            var incomes = new
            {
                items = list,
                total = total,
                balance = balance,
                rate = reg.rate,
                amount = (float)Math.Round((double)(reg.amount_transaction * reg.rate ?? 0), 2),

                //local_total = (float)Math.Round((double)(total * reg.rate ?? 0), 2),
                //local_balance = (float)Math.Round((double)(balance * reg.rate ?? 0), 2)
            };

            return Json(incomes, JsonRequestBehavior.AllowGet);
        }
    }
}