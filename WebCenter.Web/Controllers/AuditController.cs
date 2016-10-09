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
    public class AuditController : BaseController
    {
        public AuditController(IUnitOfWork UOF)
            : base(UOF)
        {

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

            Expression<Func<audit, bool>> condition = c => true; //c.salesman_id == userId;
            var ops = arrs[4].Split(',');
            if (ops.Count() == 0)
            {
                condition = c => c.salesman_id == userId;
            }
            else
            {
                var hasCompany = ops.Where(o => o == "1").FirstOrDefault();
                var hasDepart = ops.Where(o => o == "2").FirstOrDefault();
                if (hasCompany == null)
                {
                    if (hasDepart == null)
                    {
                        condition = c => c.salesman_id == userId;
                    }
                    else
                    {
                        condition = c => c.organization_id == deptId;
                    }
                }
            }

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
                type = c.type,
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

        public ActionResult Add(audit _audit, oldRequest oldRequest)
        {
            if (_audit.customer_id == null)
            {
                return Json(new { success = false, message = "请选择客户" }, JsonRequestBehavior.AllowGet);
            }
            //if (string.IsNullOrEmpty(_audit.name_cn))
            //{
            //    return Json(new { success = false, message = "请填写公司中文名称" }, JsonRequestBehavior.AllowGet);
            //}
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

            if (_audit.salesman_id == null)
            {
                return Json(new { success = false, message = "请选择业务员" }, JsonRequestBehavior.AllowGet);
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
                       
            _audit.status = 0;
            _audit.review_status = -1;
            _audit.creator_id = userId;
            //_audit.salesman_id = userId;
            _audit.organization_id = GetOrgIdByUserId(userId); //organization_id;
                        
            var nowYear = DateTime.Now.Year;
            if (oldRequest.is_old == 0)
            {
                _audit.code = GetNextOrderCode(_audit.salesman_id.Value, "SJ");

                //if (_audit.is_annual == 1)
                //{
                //    _audit.annual_year = nowYear - 1;
                //}
            }
            else
            {
                _audit.status = 4;
                _audit.review_status = 1;

                //if (oldRequest.is_already_annual == 1)
                //{
                //    _audit.annual_year = nowYear;
                //}
                //else
                //{
                //    _audit.annual_year = nowYear - 1;
                //}
            }

            var newAbroad = Uof.IauditService.AddEntity(_audit);
            if (newAbroad == null)
            {
                return Json(new { success = false, message = "添加失败" }, JsonRequestBehavior.AllowGet);
            }

            switch (_audit.source)
            {
                case "reg_abroad":
                    var abroad = Uof.Ireg_abroadService.GetAll(a => a.id == _audit.source_id).FirstOrDefault();
                    abroad.annual_date = DateTime.Today;
                    abroad.annual_year = DateTime.Today.Year;
                    Uof.Ireg_abroadService.UpdateEntity(abroad);
                    break;
                case "reg_internal":
                    var intern = Uof.Ireg_internalService.GetAll(i => i.id == _audit.source_id).FirstOrDefault();
                    intern.annual_date = DateTime.Today;
                    intern.annual_year = DateTime.Today.Year;
                    Uof.Ireg_internalService.UpdateEntity(intern);
                    break;
                case "patent":
                    var p = Uof.IpatentService.GetAll(i => i.id == _audit.source_id).FirstOrDefault();
                    p.annual_date = DateTime.Today;
                    p.annual_year = DateTime.Today.Year;
                    Uof.IpatentService.UpdateEntity(p);
                    break;
                case "trademark":
                    var t = Uof.ItrademarkService.GetAll(i => i.id == _audit.source_id).FirstOrDefault();
                    t.annual_date = DateTime.Today;
                    t.annual_year = DateTime.Today.Year;
                    Uof.ItrademarkService.UpdateEntity(t);
                    break;
                default:
                    break;
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
                rate = a.rate,

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
                rate = a.rate,

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

            var list = Uof.IincomeService.GetAll(i => i.source_id == reg.id && i.source_name == "audit").Select(i => new {
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

            var banks = Uof.Iaudit_bankService.GetAll(b => b.audit_id == id).Select(b => new
            {
                id = b.id,
                audit_id = b.audit_id,
                bank_id = b.bank_id,
                bank = b.bank_account.bank,
                holder = b.bank_account.holder,
                account = b.bank_account.account
            }).ToList();

            return Json(new { order = reg, incomes = incomes, banks = banks }, JsonRequestBehavior.AllowGet);
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
                _audit.currency == dbAudit.currency &&
                _audit.rate == dbAudit.rate
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

            var isChangeCurrency = _audit.currency != dbAudit.currency || _audit.rate != dbAudit.rate;

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
            dbAudit.rate = _audit.rate;
            dbAudit.date_updated = DateTime.Now;

            var r = Uof.IauditService.UpdateEntity(dbAudit);

            if (r)
            {
                if (isChangeCurrency)
                {
                    var list = Uof.IincomeService.GetAll(i => i.source_id == _audit.id && i.source_name == "audit").ToList();
                    if (list.Count() > 0)
                    {
                        foreach (var item in list)
                        {
                            item.currency = _audit.currency;
                            item.rate = _audit.rate;
                        }

                        Uof.IincomeService.UpdateEntities(list);
                    }
                }

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

                var ids = GetFinanceMembers();
                if (ids.Count() > 0)
                {
                    var waitdeals = new List<waitdeal>();
                    foreach (var item in ids)
                    {
                        waitdeals.Add(new waitdeal
                        {
                            source = "audit",
                            source_id = dbAudit.id,
                            user_id = item,
                            router = "audit_view",
                            content = "您有审计订单需要财务审核",
                            read_status = 0
                        });
                    }
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

            var dbAudit = Uof.IauditService.GetById(id);
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
                    source = "audit",
                    source_id = dbAudit.id,
                    user_id = dbAudit.salesman_id,
                    router = "audit_view",
                    content = "您的审计订单已通过财务审核",
                    read_status = 0
                });

                var ids = GetSubmitMembers();
                if (ids.Count() > 0)
                {
                    foreach (var item in ids)
                    {
                        waitdeals.Add(new waitdeal
                        {
                            source = "audit",
                            source_id = dbAudit.id,
                            user_id = item,
                            router = "audit_view",
                            content = "您有审计订单需要提交审核",
                            read_status = 0
                        });
                    }
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
                    source = "audit",
                    source_id = dbAudit.id,
                    user_id = dbAudit.salesman_id,
                    router = "audit_view",
                    content = "您的审计订单已通过提交审核",
                    read_status = 0
                });
            }

            dbAudit.date_updated = DateTime.Now;

            var r = Uof.IauditService.UpdateEntity(dbAudit);

            if (r)
            {
                Uof.IwaitdealService.AddEntities(waitdeals);

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
                    source = "audit",
                    source_id = dbAudit.id,
                    user_id = dbAudit.salesman_id,
                    router = "audit_view",
                    content = "您的审计订单未通过财务审核",
                    read_status = 0
                });
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
                    source = "audit",
                    source_id = dbAudit.id,
                    user_id = dbAudit.salesman_id,
                    router = "audit_view",
                    content = "您的审计订单未通过提交审核",
                    read_status = 0
                });
            }

            dbAudit.date_updated = DateTime.Now;

            var r = Uof.IauditService.UpdateEntity(dbAudit);

            if (r)
            {
                Uof.IwaitdealService.AddEntities(waitdeals);

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
                if (dbAudit.progress == request.progress && dbAudit.date_finish == request.date_finish)
                {
                    return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);
                }
                dbAudit.status = 4;
                dbAudit.date_finish = request.date_finish;
                dbAudit.progress = request.progress;
            }

            var r = Uof.IauditService.UpdateEntity(dbAudit);

            if (r)
            {
                if (request.progress_type != "p")
                {
                    Uof.ItimelineService.AddEntity(new timeline()
                    {
                        source_id = dbAudit.id,
                        source_name = "audit",
                        title = "完善了注册资料",
                        content = string.Format("{0}完善了注册资料", arrs[3])
                    });
                   
                    Uof.IwaitdealService.AddEntity(new waitdeal
                    {
                        source = "audit",
                        source_id = dbAudit.id,
                        user_id = dbAudit.salesman_id,
                        router = "audit_view",
                        content = "您的审计订单已完成",
                        read_status = 0
                    });
                }
                else
                {
                    Uof.ItimelineService.AddEntity(new timeline()
                    {
                        source_id = dbAudit.id,
                        source_name = "audit",
                        title = "更新了订单进度",
                        content = string.Format("{0}更新了进度: {1} 预计完成日期 {2}", arrs[3], dbAudit.progress, dbAudit.date_finish.Value.ToString("yyyy-MM-dd"))
                    });

                    Uof.IwaitdealService.AddEntity(new waitdeal
                    {
                        source = "audit",
                        source_id = dbAudit.id,
                        user_id = dbAudit.salesman_id,
                        router = "audit_view",
                        content = "您的审计订单更新了进度",
                        read_status = 0
                    });
                }
            }
            
            return Json(new { success = r, message = r ? "" : "更新失败" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddBank(int audit_id, int customer_id, string bank, string holder, string account)
        {
            var _bank = new bank_account
            {
                customer_id = customer_id,
                bank = bank,
                holder = holder,
                account = account
            };

            var b = Uof.Ibank_accountService.AddEntity(_bank);

             Uof.Iaudit_bankService.AddEntity(new audit_bank
            {
                audit_id = audit_id,
                customer_id = customer_id,
                bank_id = b.id
            });

            return SuccessResult;
        }

        public ActionResult DeleteBank(int id)
        {
            var bank = Uof.Iaudit_bankService.GetAll(b => b.id == id).FirstOrDefault();

            Uof.Iaudit_bankService.DeleteEntity(bank);

            return SuccessResult;
        }

        public ActionResult GetAuditBanks(int audit_id, int customer_id, int index = 1, int size = 500, string name = "")
        {

            var bankIds = Uof.Iaudit_bankService.GetAll(b=>b.audit_id == audit_id).Select(m => m.bank_id).ToList();

            Expression<Func<bank_account, bool>> condition = b => true;
            if (!string.IsNullOrEmpty(name))
            {
                condition = b => (b.bank.IndexOf(name) > -1);
            }

            Expression<Func<bank_account, bool>> excludeIds = m => true;
            if (bankIds.Count() > 0)
            {
                excludeIds = m => !bankIds.Contains(m.id);
            }

            var list = Uof.Ibank_accountService.GetAll(condition).Where(excludeIds).OrderByDescending(item => item.id).Select(m => new
            {
                id = m.id,
                customer_id = m.customer_id,
                bank = m.bank,
                account = m.account,
                holder = m.holder
            }).ToPagedList(index, size).ToList();

            var totalRecord = Uof.Ibank_accountService.GetAll(condition).Where(excludeIds).Count();

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

        [HttpPost]
        public ActionResult AddAuditBank(int audit_id, int customer_id, int[] bankIds)
        {

            var oldBanks = Uof.Iaudit_bankService.GetAll(m => m.audit_id == audit_id).ToList();

            var adds = new List<audit_bank>();

            if (oldBanks.Count() == 0)
            {
                foreach (var item in bankIds)
                {
                    adds.Add(new audit_bank()
                    {
                        customer_id = customer_id,
                        audit_id = audit_id,
                        bank_id = item
                    });
                }

                Uof.Iaudit_bankService.AddEntities(adds);
                return SuccessResult;
            }

            foreach (var item in bankIds)
            {
                var exist = oldBanks.Where(o => o.bank_id == item);
                if (exist.Count() == 0)
                {
                    adds.Add(new audit_bank()
                    {
                        bank_id = item,
                        audit_id = audit_id,
                        customer_id = customer_id
                    });
                }
            }

            if (adds.Count() > 0)
            {
                Uof.Iaudit_bankService.AddEntities(adds);
            }

            return SuccessResult;
        }


        public ActionResult GetSourceForAudit(int id, string type)
        {
            var year = DateTime.Now.Year;

            switch (type)
            {
                case "reg_internal":
                    var internals = Uof.Ireg_internalService.GetAll(i => i.id == id).Select(i => new
                    {
                        source = "reg_internal",
                        id = i.id,
                        customer_id = i.customer_id,
                        name_cn = i.name_cn,
                        name_en = "",
                        code = i.code,
                        customer_name = i.customer.name,


                        province = i.customer.province,
                        city = i.customer.city,
                        county = i.customer.county,
                        customer_address = i.customer.address,
                        contact = i.customer.contact,
                        mobile = i.customer.mobile,
                        tel = i.customer.tel,

                        date_finish = i.date_finish,
                        date_setup = i.date_setup,
                        address = i.address,
                        salement = i.member4.name
                    }).FirstOrDefault();

                    return Json(internals, JsonRequestBehavior.AllowGet);
                case "reg_abroad":
                    var abroads = Uof.Ireg_abroadService.GetAll(i => i.id == id).Select(i => new
                    {
                        source = "reg_abroad",
                        id = i.id,
                        customer_id = i.customer_id,
                        name_cn = i.name_cn,
                        name_en = i.name_en,
                        code = i.code,
                        customer_name = i.customer.name,

                        province = i.customer.province,
                        city = i.customer.city,
                        county = i.customer.county,
                        customer_address = i.customer.address,
                        contact = i.customer.contact,
                        mobile = i.customer.mobile,
                        tel = i.customer.tel,

                        date_finish = i.date_finish,
                        date_setup = i.date_setup,
                        address = i.address,
                        salement = i.member4.name
                    }).FirstOrDefault();

                    return Json(abroads, JsonRequestBehavior.AllowGet);
            }

            return SuccessResult;
        }
    }
}