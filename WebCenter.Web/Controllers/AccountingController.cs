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
    public class AccountingController : BaseController
    {
        public AccountingController(IUnitOfWork UOF)
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

            Expression<Func<accounting, bool>> condition = c => true; // c.salesman_id == userId;
            var ops = arrs[4].Split(',');
            if (ops.Count() == 0)
            {
                condition = c => (c.salesman_id == userId || c.assistant_id == userId || c.accountant_id == userId);
            }
            else
            {
                var hasCompany = ops.Where(o => o == "1").FirstOrDefault();
                var hasDepart = ops.Where(o => o == "2").FirstOrDefault();
                if (hasCompany == null)
                {
                    if (hasDepart == null)
                    {
                        condition = c => (c.salesman_id == userId || c.assistant_id == userId || c.accountant_id == userId);
                    }
                    else
                    {
                        var ids = GetChildrenDept(deptId);
                        if (ids.Count > 0)
                        {
                            condition = c => c.organization_id == deptId;
                        }
                        else
                        {
                            condition = c => ids.Contains(c.organization_id.Value);
                        }
                    }
                }
            }

            // 客户id
            Expression<Func<accounting, bool>> customerQuery = c => true;
            if (request.customer_id != null && request.customer_id.Value > 0)
            {
                customerQuery = c => (c.customer_id == request.customer_id);
            }

            // 订单状态
            Expression<Func<accounting, bool>> statusQuery = c => true;
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
            Expression<Func<accounting, bool>> date1Query = c => true;
            Expression<Func<accounting, bool>> date2Query = c => true;
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

            // 录入开始日期
            Expression<Func<accounting, bool>> date1Created = c => true;
            Expression<Func<accounting, bool>> date2Created = c => true;
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

            Expression<Func<accounting, bool>> nameQuery = c => true;
            if (!string.IsNullOrEmpty(request.name))
            {
                nameQuery = c => (c.name.ToLower().Contains(request.name.ToLower()));
            }

            Expression<Func<accounting, bool>> codeQuery = c => true;
            if (!string.IsNullOrEmpty(request.code))
            {
                codeQuery = c => c.code.ToLower().Contains(request.code.ToLower());
            }

            var list = Uof.IaccountingService.GetAll(condition)
                .Where(customerQuery)
                .Where(statusQuery)
                .Where(date1Query)
                .Where(date2Query)
                .Where(date1Created)
                .Where(date2Created)
                .Where(nameQuery)
                .Where(codeQuery)
                .OrderByDescending(item => item.code).Select(c => new
                {
                    id = c.id,
                    code = c.code,
                    customer_id = c.customer_id,
                    customer_name = c.customer.name,
                    name = c.name,
                    address = c.address,
                    legal = c.legal,
                    status = c.status,
                    review_status = c.review_status,
                    date_transaction = c.date_transaction,
                    amount_transaction = c.amount_transaction,
                    amount_income = 0,
                    amount_unreceive = 0,
                    salesman_id = c.salesman_id,
                    salesman_name = c.member5.name,

                    assistant_id = c.assistant_id,
                    assistant_name = c.member.name,

                    finance_review_moment = c.finance_review_moment,
                    submit_review_moment = c.submit_review_moment,
                    date_created = c.date_created

                }).ToPagedList(request.index, request.size).ToList();

            var totalRecord = Uof.IaccountingService.GetAll(condition)
                .Where(customerQuery)
                .Where(statusQuery)
                .Where(date1Query)
                .Where(date2Query)
                .Where(date1Created)
                .Where(date2Created)
                .Where(nameQuery)
                .Where(codeQuery)
                .Count();

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

        public ActionResult Add(accounting acc, oldRequest oldRequest)
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

            acc.status = 0;
            acc.review_status = -1;
            acc.creator_id = userId;
            //reginternal.salesman_id = userId;
            acc.organization_id = GetOrgIdByUserId(userId); // organization_id;

            var nowYear = DateTime.Now.Year;
            if (oldRequest.is_old == 0)
            {
                // 新单据
                acc.code = GetNextOrderCode(acc.salesman_id.Value, "JZ");
            }
            else
            {
                // 旧单据
                acc.status = 4;
                acc.review_status = 1;
            }

            if (acc.source_type == 1)
            {
                acc.source_code = "";
                acc.source_id = null;
            }

            acc.rate = 1;
            acc.currency = "人民币";
            var newAcc = Uof.IaccountingService.AddEntity(acc);
            if (newAcc == null)
            {
                return Json(new { success = false, message = "添加失败" }, JsonRequestBehavior.AllowGet);
            }

            Uof.ItimelineService.AddEntity(new timeline()
            {
                source_id = newAcc.id,
                source_name = "accounting",
                title = "新建记账订单",
                is_system = 1,
                content = string.Format("{0}新建了订单, 档案号{1}", arrs[3], newAcc.code)
            });

            return Json(new { id = newAcc.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(accounting acc)
        {
            var dbAcc = Uof.IaccountingService.GetById(acc.id);

            var identityName = HttpContext.User.Identity.Name;
            var arrs = identityName.Split('|');
            if (arrs.Length == 0)
            {
                return new HttpUnauthorizedResult();
            }

            dbAcc.customer_id = acc.customer_id;
            dbAcc.name = acc.name;
            dbAcc.source_type = acc.source_type;
            dbAcc.source_id = acc.source_id;
            dbAcc.source_code = acc.source_code;
            dbAcc.legal = acc.legal;
            dbAcc.address = acc.address;
            dbAcc.bank_id = acc.bank_id;
            dbAcc.amount_transaction = acc.amount_transaction;
            dbAcc.date_transaction = acc.date_transaction;
            dbAcc.date_updated = DateTime.Now;

            dbAcc.tax = acc.tax;
            if (acc.tax == 0)
            {
                dbAcc.invoice_name = acc.invoice_name;
                dbAcc.invoice_tax = acc.invoice_tax;
                dbAcc.invoice_address = acc.invoice_address;
                dbAcc.invoice_tel = acc.invoice_tel;
                dbAcc.invoice_bank = acc.invoice_bank;
                dbAcc.invoice_account = acc.invoice_account;
            }
            else
            {
                dbAcc.invoice_name = null;
                dbAcc.invoice_tax = null;
                dbAcc.invoice_address = null;
                dbAcc.invoice_tel = null;
                dbAcc.invoice_bank = null;
                dbAcc.invoice_account = null;
            }

            if (acc.source_type == 1)
            {
                dbAcc.source_code = "";
                dbAcc.source_id = null;
            }

            var r = Uof.IaccountingService.UpdateEntity(dbAcc);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbAcc.id,
                    source_name = "accounting",
                    title = "修改订单资料",
                    is_system = 1,
                    content = string.Format("{0}修改了订单资料", arrs[3])
                });
            }

            return Json(new { success = r, id = dbAcc.id }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult Get(int id)
        {
            var acc = Uof.IaccountingService.GetAll(a => a.id == id).Select(a => new
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
                name = a.name,
                address = a.address,
                legal = a.legal,
                date_transaction = a.date_transaction,
                amount_transaction = a.amount_transaction,               
                bank_id = a.bank_id,
                bank_name = a.bank_account.bank,
                holder = a.bank_account.holder,
                account = a.bank_account.account,
                
                currency = a.currency,
                rate = a.rate,

                source_type = a.source_type,
                source_id = a.source_id,
                source_code = a.source_code,

                salesman_id = a.salesman_id,
                salesman = a.member5.name,               
                manager_id = a.manager_id,
                manager_name = a.member3.name,
                finance_reviewer_id = a.finance_reviewer_id,
                finance_reviewer = a.member2.name,
                finance_review_moment = a.finance_review_moment,
                submit_reviewer_id = a.submit_reviewer_id,
                submit_reviewer = a.member6.name,
                submit_review_moment = a.submit_review_moment,
                review_status = a.review_status,
                creator_id = a.creator_id,
                creator_name = a.member1.name,
                accountant_id = a.accountant_id,
                accountan_name = a.member.name,
                assistant_id = a.assistant_id,
                assistant_name = a.member4.name,
                status = a.status,
                tax = a.tax,
                invoice_name = a.invoice_name,
                invoice_tax = a.invoice_tax,
                invoice_address = a.invoice_address,
                invoice_tel = a.invoice_tel,
                invoice_bank = a.invoice_bank,
                invoice_account = a.invoice_account,

            }).FirstOrDefault();

            //creator_id member1
            //accountant_id member
            //finance_reviewer_id member2
            //mamager_id  3
            //assistant_id  4
            //salesman  5
            //submit  6

            //if (reg == null)
            //{
            //    return Json(null, JsonRequestBehavior.AllowGet);
            //}
                        
            return Json(new { order = acc }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetView(int id)
        {
            var acc = Uof.IaccountingService.GetAll(a => a.id == id).Select(a => new
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
                name = a.name,
                address = a.address,
                legal = a.legal,
                date_transaction = a.date_transaction,
                amount_transaction = a.amount_transaction,
                bank_id = a.bank_id,
                bank_name = a.bank_account.bank,
                holder = a.bank_account.holder,
                account = a.bank_account.account,

                currency = a.currency,
                rate = a.rate,

                source_type = a.source_type,
                source_id = a.source_id,
                source_code = a.source_code,

                salesman_id = a.salesman_id,
                salesman = a.member5.name,
                manager_id = a.manager_id,
                manager_name = a.member3.name,
                finance_reviewer_id = a.finance_reviewer_id,
                finance_reviewer = a.member2.name,
                finance_review_moment = a.finance_review_moment,
                submit_reviewer_id = a.submit_reviewer_id,
                submit_reviewer = a.member6.name,
                submit_review_moment = a.submit_review_moment,
                review_status = a.review_status,
                creator_id = a.creator_id,
                creator_name = a.member1.name,
                accountant_id = a.accountant_id,
                accountan_name = a.member.name,
                assistant_id = a.assistant_id,
                assistant_name = a.member4.name,
                status = a.status,

                tax = a.tax,
                invoice_name = a.invoice_name,
                invoice_tax = a.invoice_tax,
                invoice_address = a.invoice_address,
                invoice_tel = a.invoice_tel,
                invoice_bank = a.invoice_bank,
                invoice_account = a.invoice_account,
            }).FirstOrDefault();


            var items = Uof.Iaccounting_itemService.GetAll(a => a.master_id == acc.id).ToList();

            var list = Uof.IincomeService.GetAll(i => i.source_id == acc.id && i.source_name == "accounting").Select(i => new {
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

            var balance = acc.amount_transaction - total;
            var incomes = new
            {
                items = list,
                total = total,
                balance = balance,

                rate = acc.rate,
                local_amount = (float)Math.Round((double)(acc.amount_transaction * acc.rate ?? 0), 2),
                local_total = (float)Math.Round((double)(total * acc.rate ?? 0), 2),
                local_balance = (float)Math.Round((double)(balance * acc.rate ?? 0), 2)
            };

            return Json(new { order = acc, incomes = incomes, items = items }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddItem(accounting_item item)
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

            item.status = 0;

            var dbItem = Uof.Iaccounting_itemService.AddEntity(item);

            var timelines = new List<timeline>();
            timelines.Add(new timeline
            {
                source_id = dbItem.master_id,
                source_name = "accounting",
                title = "新增账期",
                is_system = 1,
                content = string.Format("{0}新增了账期", arrs[3])
            });

            timelines.Add(new timeline
            {
                source_id = dbItem.id,
                source_name = "accounting_item",
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
            return Json(new { id = dbItem.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteItem(int id)
        {
            var b = Uof.Iaccounting_itemService.DeleteEntity(id);
            return b ? SuccessResult : ErrorResult;
        }

        [HttpPost]
        public ActionResult UpdateItem(accounting_item item)
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

            var dbItem = Uof.Iaccounting_itemService.GetById(item.id);

            dbItem.date_start = item.date_start;
            dbItem.date_end = item.date_end;
            dbItem.date_updated = DateTime.Now;

            Uof.Iaccounting_itemService.UpdateEntity(dbItem);

            var timelines = new List<timeline>();
            timelines.Add(new timeline
            {
                source_id = dbItem.id,
                source_name = "accounting_item",
                title = "修改了账期",
                is_system = 1,
                content = string.Format("{0}修改了账期", arrs[3])
            });
            try
            {
                Uof.ItimelineService.AddEntities(timelines);
            }
            catch (Exception)
            {
            }
            return Json(new { id = dbItem.id }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult Submit(int id)
        {
            var dbAcc = Uof.IaccountingService.GetById(id);
            if (dbAcc == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }

            dbAcc.status = 1;
            dbAcc.review_status = -1;
            dbAcc.date_updated = DateTime.Now;

            var r = Uof.IaccountingService.UpdateEntity(dbAcc);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbAcc.id,
                    source_name = "accounting",
                    title = "提交审核",
                    is_system = 1,
                    content = string.Format("提交给财务审核")
                });

                //var ids = GetFinanceMembers();
                var auditor_id = GetAuditorByKey("CW_ID");
                if (auditor_id != null)
                {
                    var waitdeals = new List<waitdeal>();
                    waitdeals.Add(new waitdeal
                    {
                        source = "accounting",
                        source_id = dbAcc.id,
                        user_id = auditor_id,
                        router = "account_view",
                        content = "您有记账订单需要财务审核",
                        read_status = 0
                    });

                    Uof.IwaitdealService.AddEntities(waitdeals);
                }
            }
            return Json(new { success = r, message = r ? "" : "更新失败" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PassAudit(int id, int waiter_id)
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

            var dbAcc = Uof.IaccountingService.GetById(id);
            if (dbAcc == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }
            var t = "";
            var waitdeals = new List<waitdeal>();
            if (dbAcc.status == 1)
            {
                dbAcc.status = 2;
                dbAcc.review_status = 1;
                dbAcc.finance_reviewer_id = userId;
                dbAcc.finance_review_date = DateTime.Now;
                dbAcc.finance_review_moment = "";

                t = "财务审核";
                waitdeals.Add(new waitdeal
                {
                    source = "accounting",
                    source_id = dbAcc.id,
                    user_id = dbAcc.salesman_id,
                    router = "account_view",
                    content = "您的记账订单已通过财务审核",
                    read_status = 0
                });

                //var ids = GetSubmitMembers();
                var jwId = GetSubmitMemberByKey("JZ_ID");
                if (jwId != null && jwId > 0)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "accounting",
                        source_id = dbAcc.id,
                        user_id = jwId,
                        router = "account_view",
                        content = "您有记账订单需要提交审核",
                        read_status = 0
                    });
                }
            }
            else
            {
                dbAcc.accountant_id = waiter_id;
                dbAcc.status = 3;
                dbAcc.review_status = 1;
                dbAcc.submit_reviewer_id = userId;
                dbAcc.submit_review_date = DateTime.Now;
                dbAcc.submit_review_moment = "";

                t = "提交的审核";
                waitdeals.Add(new waitdeal
                {
                    source = "accounting",
                    source_id = dbAcc.id,
                    user_id = dbAcc.salesman_id,
                    router = "account_view",
                    content = "您的记账订单已通过提交审核",
                    read_status = 0
                });

                if (dbAcc.assistant_id != null && dbAcc.assistant_id != dbAcc.salesman_id)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "accounting",
                        source_id = dbAcc.id,
                        user_id = dbAcc.assistant_id,
                        router = "account_view",
                        content = "您的记账订单已通过提交审核",
                        read_status = 0
                    });
                }
            }

            dbAcc.date_updated = DateTime.Now;

            var r = Uof.IaccountingService.UpdateEntity(dbAcc);

            if (r)
            {
                Uof.IwaitdealService.AddEntities(waitdeals);

                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbAcc.id,
                    source_name = "accounting",
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

            var dbAcc = Uof.IaccountingService.GetById(id);
            if (dbAcc == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }
            var t = "";
            var waitdeals = new List<waitdeal>();
            if (dbAcc.status == 1)
            {
                dbAcc.status = 0;
                dbAcc.review_status = 0;
                dbAcc.finance_reviewer_id = userId;
                dbAcc.finance_review_date = DateTime.Now;
                dbAcc.finance_review_moment = description;

                t = "驳回了财务审核";
                waitdeals.Add(new waitdeal
                {
                    source = "accounting",
                    source_id = dbAcc.id,
                    user_id = dbAcc.salesman_id,
                    router = "account_view",
                    content = "您的记账订单未通过财务审核",
                    read_status = 0
                });
                if (dbAcc.assistant_id != null && dbAcc.assistant_id != dbAcc.salesman_id)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "accounting",
                        source_id = dbAcc.id,
                        user_id = dbAcc.assistant_id,
                        router = "account_view",
                        content = "您的记账订单未通过财务审核",
                        read_status = 0
                    });
                }
            }
            else
            {
                dbAcc.status = 0;
                dbAcc.review_status = 0;
                dbAcc.submit_reviewer_id = userId;
                dbAcc.submit_review_date = DateTime.Now;
                dbAcc.submit_review_moment = description;

                t = "驳回了提交的审核";
                waitdeals.Add(new waitdeal
                {
                    source = "accounting",
                    source_id = dbAcc.id,
                    user_id = dbAcc.salesman_id,
                    router = "account_view",
                    content = "您的记账订单未通过提交审核",
                    read_status = 0
                });
                if (dbAcc.assistant_id != null && dbAcc.assistant_id != dbAcc.salesman_id)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "accounting",
                        source_id = dbAcc.id,
                        user_id = dbAcc.assistant_id,
                        router = "account_view",
                        content = "您的记账订单未通过提交审核",
                        read_status = 0
                    });
                }
            }

            dbAcc.date_updated = DateTime.Now;

            var r = Uof.IaccountingService.UpdateEntity(dbAcc);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbAcc.id,
                    source_name = "accounting",
                    title = "驳回审核",
                    is_system = 1,
                    content = string.Format("{0}{1}, 驳回理由: {2}", arrs[3], t, description)
                });
            }

            return Json(new { success = r, message = r ? "" : "审核失败" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPeriodProgress(int id)
        {
            var items = Uof.Iaccounting_progressService.GetAll(p => p.master_id == id).ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProgress(int id)
        {
            var p = Uof.Iaccounting_progressService.GetById(id);

            return Json(p, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddProress(accounting_progress progress)
        {
            Uof.Iaccounting_progressService.AddEntity(progress);
            return SuccessResult;
        }

        [HttpPost]
        public ActionResult UpdateProress(accounting_progress progress)
        {
            var p = Uof.Iaccounting_progressService.GetById(progress.id);
            p.progress = progress.progress;
            p.attachment = progress.attachment;
            p.date_start = progress.date_start;

            Uof.Iaccounting_progressService.UpdateEntity(p);

            return SuccessResult;
        }
        
    }
}