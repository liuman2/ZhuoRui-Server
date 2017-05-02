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
    public class RegInternalController : BaseController
    {
        public RegInternalController(IUnitOfWork UOF)
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

            Expression<Func<reg_internal, bool>> condition = c => true; // c.salesman_id == userId;
            var ops = arrs[4].Split(',');
            if (ops.Count() == 0)
            {
                condition = c => (c.salesman_id == userId || c.assistant_id == userId || c.waiter_id == userId);
            }
            else
            {
                var hasCompany = ops.Where(o => o == "1").FirstOrDefault();
                var hasDepart = ops.Where(o => o == "2").FirstOrDefault();
                if (hasCompany == null)
                {
                    if (hasDepart == null)
                    {
                        condition = c => (c.salesman_id == userId || c.assistant_id == userId);
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
            Expression<Func<reg_internal, bool>> customerQuery = c => true;
            if (request.customer_id != null && request.customer_id.Value > 0)
            {
                customerQuery = c => (c.customer_id == request.customer_id);
            }

            // 订单状态
            Expression<Func<reg_internal, bool>> statusQuery = c => true;
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
            Expression<Func<reg_internal, bool>> date1Query = c => true;
            Expression<Func<reg_internal, bool>> date2Query = c => true;
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
            Expression<Func<reg_internal, bool>> date1Created = c => true;
            Expression<Func<reg_internal, bool>> date2Created = c => true;
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

            Expression<Func<reg_internal, bool>> nameQuery = c => true;
            if (!string.IsNullOrEmpty(request.name))
            {
                nameQuery = c => (c.name_cn.ToLower().Contains(request.name.ToLower()));
            }

            Expression<Func<reg_internal, bool>> codeQuery = c => true;
            if (!string.IsNullOrEmpty(request.code))
            {
                codeQuery = c => c.code.ToLower().Contains(request.code.ToLower());
            }

            var list = Uof.Ireg_internalService.GetAll(condition)
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
                    name_cn = c.name_cn,
                    address = c.address,
                    legal = c.legal,
                    status = c.status,
                    review_status = c.review_status,
                    date_transaction = c.date_transaction,
                    amount_transaction = c.amount_transaction,
                    amount_income = 0,
                    amount_unreceive = 0,
                    progress = c.progress,
                    salesman_id = c.salesman_id,
                    salesman_name = c.member5.name,

                    assistant_id = c.assistant_id,
                    assistant_name = c.member.name,

                    finance_review_moment = c.finance_review_moment,
                    submit_review_moment = c.submit_review_moment,
                    date_setup = c.date_setup,
                    date_created = c.date_created

                }).ToPagedList(request.index, request.size).ToList();

            var totalRecord = Uof.Ireg_internalService.GetAll(condition)
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

        public ActionResult Add(reg_internal reginternal, oldRequest oldRequest, List<reg_internal_items> items)
        {
            //if (reginternal.customer_id == null)
            //{
            //    return Json(new { success = false, message = "请选择客户" }, JsonRequestBehavior.AllowGet);
            //}
            //if (string.IsNullOrEmpty(reginternal.name_cn))
            //{
            //    return Json(new { success = false, message = "请填写公司中文名称" }, JsonRequestBehavior.AllowGet);
            //}
            //if (reginternal.date_setup == null)
            //{
            //    return Json(new { success = false, message = "请填写公司成立日期" }, JsonRequestBehavior.AllowGet);
            //}
            //if (string.IsNullOrEmpty(reginternal.reg_no))
            //{
            //    return Json(new { success = false, message = "请填写统一信用编号" }, JsonRequestBehavior.AllowGet);
            //}
            //if (string.IsNullOrEmpty(reginternal.address))
            //{
            //    return Json(new { success = false, message = "请填写公司注册地址" }, JsonRequestBehavior.AllowGet);
            //}
            //if (reginternal.date_transaction == null)
            //{
            //    return Json(new { success = false, message = "请填写成交日期" }, JsonRequestBehavior.AllowGet);
            //}
            //if (reginternal.amount_transaction == null)
            //{
            //    return Json(new { success = false, message = "请填写成交金额" }, JsonRequestBehavior.AllowGet);
            //}
            //if (reginternal.salesman_id == null)
            //{
            //    return Json(new { success = false, message = "请选择业务员" }, JsonRequestBehavior.AllowGet);
            //}
            //if (reginternal.waiter_id == null)
            //{
            //    return Json(new { success = false, message = "请选择年检客服" }, JsonRequestBehavior.AllowGet);
            //}

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

            reginternal.status = 0;
            reginternal.review_status = -1;
            reginternal.creator_id = userId;
            //reginternal.salesman_id = userId;
            reginternal.organization_id = GetOrgIdByUserId(userId); // organization_id;

            var nowYear = DateTime.Now.Year;
            if (oldRequest.is_old == 0)
            {
                // 新单据
                reginternal.code = GetNextOrderCode(reginternal.salesman_id.Value, "ZN");

                // 需要马上年检的 步骤审核流程
                if (reginternal.is_annual == 1)
                {
                    reginternal.date_finish = DateTime.Now;
                    reginternal.status = 4;
                    reginternal.review_status = 1;
                }
            }
            else
            {
                // 旧单据
                reginternal.status = 4;
                reginternal.review_status = 1;

                if (oldRequest.is_already_annual == 1)
                {
                    // 已年检
                    reginternal.annual_year = nowYear;
                }
                else
                {
                    // 未年检
                    reginternal.annual_year = nowYear - 1;
                }
            }

            if (reginternal.is_customs == 0)
            {
                reginternal.customs_name = null;
                reginternal.customs_address = null;
            }

            if (reginternal.is_bookkeeping == 0)
            {
                reginternal.amount_bookkeeping = null;
            }

            var newInternal = Uof.Ireg_internalService.AddEntity(reginternal);
            if (newInternal == null)
            {
                return Json(new { success = false, message = "添加失败" }, JsonRequestBehavior.AllowGet);
            }

            if (items.Count() > 0)
            {
                foreach (var item in items)
                {
                    item.status = 0;
                    item.master_id = newInternal.id;
                }
                Uof.Ireg_internal_itemsService.AddEntities(items);
            }

            Uof.ItimelineService.AddEntity(new timeline()
            {
                source_id = newInternal.id,
                source_name = "reg_internal",
                title = "新建订单",
                is_system = 1,
                content = string.Format("{0}新建了订单, 档案号{1}", arrs[3], newInternal.code)
            });

            return Json(new { id = newInternal.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get(int id)
        {
            var reg = Uof.Ireg_internalService.GetAll(a => a.id == id).Select(a => new
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
                name_cn = a.name_cn,
                date_setup = a.date_setup,
                reg_no = a.reg_no,
                address = a.address,
                legal = a.legal,
                date_transaction = a.date_transaction,
                amount_transaction = a.amount_transaction,
                director = a.director,
                bank_id = a.bank_id,
                bank_name = a.bank_account.bank,
                holder = a.bank_account.holder,
                account = a.bank_account.account,
                taxpayer = a.taxpayer,
                is_customs = a.is_customs,
                customs_name = a.customs_name,
                customs_address = a.customs_address,
                is_bookkeeping = a.is_bookkeeping,
                amount_bookkeeping = a.amount_bookkeeping,
                date_finish = a.date_finish,
                currency = a.currency,
                rate = a.rate,
                progress = a.progress,

                invoice_name = a.invoice_name,
                invoice_tax = a.invoice_tax,
                invoice_address = a.invoice_address,
                invoice_tel = a.invoice_tel,
                invoice_bank = a.invoice_bank,
                invoice_account = a.invoice_account,

                salesman_id = a.salesman_id,
                salesman = a.member5.name,
                waiter_id = a.waiter_id,
                waiter_name = a.member7.name,
                manager_id = a.manager_id,
                manager_name = a.member3.name,
                outworker_id = a.outworker_id,
                outworker_name = a.member4.name,

                assistant_id = a.assistant_id,
                assistant_name = a.member.name,

                status = a.status,
                review_status = a.review_status,
                description = a.description,
                is_annual = a.is_annual ?? 0,
                shareholder = a.shareholder,
                names = a.names,
                shareholders = a.shareholders,
                card_no = a.card_no,
                scope = a.scope,
                pay_mode = a.pay_mode,
                biz_address = a.biz_address,
                director_card_no = a.director_card_no,
                capital = a.capital,
            }).FirstOrDefault();

            //if (reg == null)
            //{
            //    return Json(null, JsonRequestBehavior.AllowGet);
            //}

            var items = Uof.Ireg_internal_itemsService.GetAll(r => r.master_id == reg.id).ToList();

            return Json(new { order = reg, items = items }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetView(int id)
        {
            var reg = Uof.Ireg_internalService.GetAll(a => a.id == id).Select(a => new
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
                name_cn = a.name_cn,
                date_setup = a.date_setup,
                reg_no = a.reg_no,
                address = a.address,
                legal = a.legal,
                date_transaction = a.date_transaction,
                amount_transaction = a.amount_transaction,
                director = a.director,
                bank_id = a.bank_id,
                bank_name = a.bank_account.bank,
                holder = a.bank_account.holder,
                account = a.bank_account.account,
                taxpayer = a.taxpayer,
                is_customs = a.is_customs,
                customs_name = a.customs_name,
                customs_address = a.customs_address,
                is_bookkeeping = a.is_bookkeeping,
                amount_bookkeeping = a.amount_bookkeeping,
                date_finish = a.date_finish,
                currency = a.currency,
                rate = a.rate,
                progress = a.progress,

                invoice_name = a.invoice_name,
                invoice_tax = a.invoice_tax,
                invoice_address = a.invoice_address,
                invoice_tel = a.invoice_tel,
                invoice_bank = a.invoice_bank,
                invoice_account = a.invoice_account,

                salesman_id = a.salesman_id,
                salesman = a.member5.name,
                waiter_id = a.waiter_id,
                waiter_name = a.member7.name,
                manager_id = a.manager_id,
                manager_name = a.member3.name,
                outworker_id = a.outworker_id,
                outworker_name = a.member4.name,

                assistant_id = a.assistant_id,
                assistant_name = a.member.name,

                status = a.status,
                review_status = a.review_status,
                finance_review_moment = a.finance_review_moment,
                submit_review_moment = a.submit_review_moment,
                description = a.description,
                is_annual = a.is_annual ?? 0,
                shareholder = a.shareholder,

                names = a.names,
                shareholders = a.shareholders,
                card_no = a.card_no,
                scope = a.scope,
                pay_mode = a.pay_mode,

                biz_address = a.biz_address,
                director_card_no = a.director_card_no,
                capital = a.capital,

            }).FirstOrDefault();

            var list = Uof.IincomeService.GetAll(i => i.source_id == reg.id && i.source_name == "reg_internal").Select(i => new {
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
                local_amount = (float)Math.Round((double)(reg.amount_transaction * reg.rate ?? 0), 2),
                local_total = (float)Math.Round((double)(total * reg.rate ?? 0), 2),
                local_balance = (float)Math.Round((double)(balance * reg.rate ?? 0), 2)
            };

            var items = Uof.Ireg_internal_itemsService.GetAll(r => r.master_id == reg.id).ToList();

            return Json(new { order = reg, incomes = incomes, items = items }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(reg_internal reginternal, List<reg_internal_items> items)
        {
            var dbReg = Uof.Ireg_internalService.GetById(reginternal.id);

            var identityName = HttpContext.User.Identity.Name;
            var arrs = identityName.Split('|');
            if (arrs.Length == 0)
            {
                return new HttpUnauthorizedResult();
            }

            var isChangeCurrency = reginternal.currency != dbReg.currency || reginternal.rate != dbReg.rate;

            dbReg.customer_id = reginternal.customer_id;
            dbReg.name_cn = reginternal.name_cn;
            dbReg.amount_bookkeeping = reginternal.amount_bookkeeping;
            dbReg.customs_address = reginternal.customs_address;
            dbReg.customs_name = reginternal.customs_name;
            dbReg.is_bookkeeping = reginternal.is_bookkeeping;
            dbReg.is_customs = reginternal.is_customs;
            dbReg.legal = reginternal.legal;
            dbReg.outworker_id = reginternal.outworker_id;
            dbReg.taxpayer = reginternal.taxpayer;
            dbReg.date_setup = reginternal.date_setup;
            dbReg.reg_no = reginternal.reg_no;

            dbReg.currency = reginternal.currency;
            dbReg.rate = reginternal.rate;

            dbReg.address = reginternal.address;
            dbReg.director = reginternal.director;

            dbReg.bank_id = reginternal.bank_id;
            dbReg.date_transaction = reginternal.date_transaction;
            dbReg.amount_transaction = reginternal.amount_transaction;
            dbReg.invoice_name = reginternal.invoice_name;
            dbReg.invoice_tax = reginternal.invoice_tax;
            dbReg.invoice_address = reginternal.invoice_address;
            dbReg.invoice_tel = reginternal.invoice_tel;
            dbReg.invoice_bank = reginternal.invoice_bank;
            dbReg.invoice_account = reginternal.invoice_account;
            dbReg.waiter_id = reginternal.waiter_id;
            dbReg.salesman_id = reginternal.salesman_id;
            dbReg.manager_id = reginternal.manager_id;
            dbReg.description = reginternal.description;
            dbReg.assistant_id = reginternal.assistant_id;

            dbReg.date_updated = DateTime.Now;
            dbReg.shareholder = reginternal.shareholder;

            dbReg.card_no = reginternal.card_no;
            dbReg.scope = reginternal.scope;
            dbReg.pay_mode = reginternal.pay_mode;
            dbReg.names = reginternal.names;
            dbReg.shareholders = reginternal.shareholders;
            dbReg.biz_address = reginternal.biz_address;
            dbReg.director_card_no = reginternal.director_card_no;
            dbReg.capital = reginternal.capital;

            var r = Uof.Ireg_internalService.UpdateEntity(dbReg);

            if (r)
            {
                var dbItems = Uof.Ireg_internal_itemsService.GetAll(i => i.master_id == dbReg.id).ToList();
                var ids = items.Select(i => i.id).ToList();
                var dbIds = new List<int>();

                var deleteItems = new List<reg_internal_items>();
                var updateItems = new List<reg_internal_items>();
                var newItems = new List<reg_internal_items>();
                if (dbItems.Count() > 0)
                {
                    dbIds = dbItems.Select(i => i.id).ToList();
                    foreach (var dbItem in dbItems)
                    {
                        var isExist = ids.Contains(dbItem.id);
                        if (isExist)
                        {
                            var existItem = items.Where(i => i.id == dbItem.id).FirstOrDefault();
                            updateItems.Add(existItem);
                        }
                        else
                        {
                            deleteItems.Add(dbItem);
                        }
                    }
                }

                if (dbIds.Count() <= 0)
                {
                    foreach (var item in items)
                    {
                        item.master_id = dbReg.id;
                    }

                    newItems = items;
                }
                else
                {
                    foreach (var item in items)
                    {
                        var isExist = dbIds.Contains(item.id);
                        if (!isExist)
                        {
                            item.status = 0;
                            item.master_id = dbReg.id;
                            newItems.Add(item);
                        }
                    }
                }

                if (deleteItems.Count() > 0)
                {
                    foreach (var item in deleteItems)
                    {
                        Uof.Ireg_internal_itemsService.DeleteEntity(item);
                    }
                }

                if (updateItems.Count > 0)
                {
                    var toUpdateItems = new List<reg_internal_items>();
                    foreach (var item in updateItems)
                    {
                        var dbItem = dbItems.Where(d => d.id == item.id).FirstOrDefault();
                        dbItem.name = item.name;
                        dbItem.material = item.material;
                        dbItem.memo = item.memo;
                        dbItem.price = item.price;
                        dbItem.spend = item.spend;
                        dbItem.date_updated = DateTime.Now;
                        toUpdateItems.Add(dbItem);
                    }

                    Uof.Ireg_internal_itemsService.UpdateEntities(toUpdateItems);
                }

                if (newItems.Count() > 0)
                {
                    Uof.Ireg_internal_itemsService.AddEntities(newItems);
                }

                if (isChangeCurrency)
                {
                    var list = Uof.IincomeService.GetAll(i => i.source_id == reginternal.id && i.source_name == "reg_internal").ToList();
                    if (list.Count() > 0)
                    {
                        foreach (var item in list)
                        {
                            item.currency = reginternal.currency;
                            item.rate = reginternal.rate;
                        }

                        Uof.IincomeService.UpdateEntities(list);
                    }
                }

                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbReg.id,
                    source_name = "reg_internal",
                    title = "修改订单资料",
                    is_system = 1,
                    content = string.Format("{0}修改了订单资料", arrs[3])
                });
            }

            return Json(new { success = r, id = reginternal.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Submit(int id)
        {
            var dbReg = Uof.Ireg_internalService.GetById(id);
            if (dbReg == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }

            dbReg.status = 1;
            dbReg.review_status = -1;
            dbReg.date_updated = DateTime.Now;

            var r = Uof.Ireg_internalService.UpdateEntity(dbReg);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbReg.id,
                    source_name = "reg_internal",
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
                        source = "reg_internal",
                        source_id = dbReg.id,
                        user_id = auditor_id,
                        router = "internal_view",
                        content = "您有国内注册订单需要财务审核",
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

            var dbReg = Uof.Ireg_internalService.GetById(id);
            if (dbReg == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }
            var t = "";
            var waitdeals = new List<waitdeal>();
            if (dbReg.status == 1)
            {
                dbReg.status = 2;
                dbReg.review_status = 1;
                dbReg.finance_reviewer_id = userId;
                dbReg.finance_review_date = DateTime.Now;
                dbReg.finance_review_moment = "";

                t = "财务审核";
                waitdeals.Add(new waitdeal
                {
                    source = "reg_internal",
                    source_id = dbReg.id,
                    user_id = dbReg.salesman_id,
                    router = "internal_view",
                    content = "您的国内注册订单已通过财务审核",
                    read_status = 0
                });

                //var ids = GetSubmitMembers();
                var jwId = GetSubmitMemberByKey("GN_ID");
                if (jwId != null && jwId > 0)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "reg_internal",
                        source_id = dbReg.id,
                        user_id = jwId,
                        router = "internal_view",
                        content = "您有国内注册订单需要提交审核",
                        read_status = 0
                    });
                }

                //if (ids.Count() > 0)
                //{
                //    foreach (var item in ids)
                //    {
                //        waitdeals.Add(new waitdeal
                //        {
                //            source = "reg_internal",
                //            source_id = dbReg.id,
                //            user_id = item,
                //            router = "internal_view",
                //            content = "您有国内注册订单需要提交审核",
                //            read_status = 0
                //        });
                //    }
                //}
            }
            else
            {
                dbReg.waiter_id = waiter_id;

                dbReg.status = 3;
                dbReg.review_status = 1;
                dbReg.submit_reviewer_id = userId;
                dbReg.submit_review_date = DateTime.Now;
                dbReg.submit_review_moment = "";

                t = "提交的审核";
                waitdeals.Add(new waitdeal
                {
                    source = "reg_internal",
                    source_id = dbReg.id,
                    user_id = dbReg.salesman_id,
                    router = "internal_view",
                    content = "您的国内注册订单已通过提交审核",
                    read_status = 0
                });

                if (dbReg.assistant_id != null && dbReg.assistant_id != dbReg.salesman_id)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "reg_internal",
                        source_id = dbReg.id,
                        user_id = dbReg.assistant_id,
                        router = "internal_view",
                        content = "您的国内注册订单已通过提交审核",
                        read_status = 0
                    });
                }
            }

            dbReg.date_updated = DateTime.Now;

            var r = Uof.Ireg_internalService.UpdateEntity(dbReg);

            if (r)
            {
                Uof.IwaitdealService.AddEntities(waitdeals);

                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbReg.id,
                    source_name = "reg_internal",
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

            var dbReg = Uof.Ireg_internalService.GetById(id);
            if (dbReg == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }
            var t = "";
            var waitdeals = new List<waitdeal>();
            if (dbReg.status == 1)
            {
                dbReg.status = 0;
                dbReg.review_status = 0;
                dbReg.finance_reviewer_id = userId;
                dbReg.finance_review_date = DateTime.Now;
                dbReg.finance_review_moment = description;

                t = "驳回了财务审核";
                waitdeals.Add(new waitdeal
                {
                    source = "reg_internal",
                    source_id = dbReg.id,
                    user_id = dbReg.salesman_id,
                    router = "internal_view",
                    content = "您的国内注册订单未通过财务审核",
                    read_status = 0
                });
                if (dbReg.assistant_id != null && dbReg.assistant_id != dbReg.salesman_id)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "reg_internal",
                        source_id = dbReg.id,
                        user_id = dbReg.assistant_id,
                        router = "internal_view",
                        content = "您的国内注册订单未通过财务审核",
                        read_status = 0
                    });
                }
            }
            else
            {
                dbReg.status = 0;
                dbReg.review_status = 0;
                dbReg.submit_reviewer_id = userId;
                dbReg.submit_review_date = DateTime.Now;
                dbReg.submit_review_moment = description;

                t = "驳回了提交的审核";
                waitdeals.Add(new waitdeal
                {
                    source = "reg_internal",
                    source_id = dbReg.id,
                    user_id = dbReg.salesman_id,
                    router = "internal_view",
                    content = "您的国内注册订单未通过提交审核",
                    read_status = 0
                });
                if (dbReg.assistant_id != null && dbReg.assistant_id != dbReg.salesman_id)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "reg_internal",
                        source_id = dbReg.id,
                        user_id = dbReg.assistant_id,
                        router = "internal_view",
                        content = "您的国内注册订单未通过提交审核",
                        read_status = 0
                    });
                }
            }

            dbReg.date_updated = DateTime.Now;

            var r = Uof.Ireg_internalService.UpdateEntity(dbReg);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbReg.id,
                    source_name = "reg_internal",
                    title = "驳回审核",
                    is_system = 1,
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

            var dbReg = Uof.Ireg_internalService.GetById(id);
            if (dbReg == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }
            dbReg.status = 4;
            dbReg.date_updated = DateTime.Now;
            dbReg.date_finish = date_finish;
            dbReg.progress = "已完成";

            var r = Uof.Ireg_internalService.UpdateEntity(dbReg);

            if (r)
            {
                var h = new reg_internal_history()
                {
                    reg_id = dbReg.id,
                    name_cn = dbReg.name_cn,
                    address = dbReg.address,
                    date_setup = dbReg.date_setup,
                    director = dbReg.director,
                    reg_no = dbReg.reg_no,
                    legal = dbReg.legal
                };

                Uof.Ireg_internal_historyService.AddEntity(h);

                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbReg.id,
                    source_name = "reg_internal",
                    title = "完成订单",
                    is_system = 1,
                    content = string.Format("{0}完成了订单，完成日期为：{1}", arrs[3], date_finish.ToString("yyyy-MM-dd"))
                });

                // TODO 通知 业务员
            }

            return Json(new { success = r, message = r ? "" : "保存失败" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult History(int id, int index = 1, int size = 10)
        {
            var totalRecord = Uof.Ireg_internal_historyService.GetAll(h => h.reg_id == id).Count();

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

            if (totalRecord <= 1)
            {
                return Json(new { page = page, items = new List<reg_internal_history>() }, JsonRequestBehavior.AllowGet);
            }

            var list = Uof.Ireg_internal_historyService.GetAll(h => h.reg_id == id).OrderByDescending(item => item.date_created).ToPagedList(index, size).ToList();

            var result = new
            {
                page = page,
                items = list
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHistoryTop(int id)
        {
            var last = Uof.Ireg_internal_historyService.GetAll(h => h.reg_id == id).OrderByDescending(h => h.id).FirstOrDefault();

            return Json(last, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddHistory(reg_internal_history history)
        {
            if (history.reg_id == null)
            {
                return Json(new { success = false, message = "参数reg_id不可为空" }, JsonRequestBehavior.AllowGet);
            }

            var dbReg = Uof.Ireg_internalService.GetAll(a=>a.id == history.reg_id).FirstOrDefault();
            if (dbReg == null)
            {
                return Json(new { success = false, message = "找不到订单" }, JsonRequestBehavior.AllowGet);
            }

            if (history.address == dbReg.address &&
                history.date_setup == dbReg.date_setup &&
                history.director == dbReg.director &&
                history.name_cn == dbReg.name_cn &&
                history.legal == dbReg.legal &&
                history.others == dbReg.description &&
                history.reg_no == dbReg.reg_no)
            {
                return Json(new { success = false, message = "您没做任何修改" }, JsonRequestBehavior.AllowGet);
            }

            dbReg.address = history.address ?? dbReg.address;
            dbReg.date_setup = history.date_setup ?? dbReg.date_setup;
            dbReg.director = history.director ?? dbReg.director;
            dbReg.name_cn = history.name_cn ?? dbReg.name_cn;
            dbReg.legal = history.legal ?? dbReg.legal;
            dbReg.reg_no = history.reg_no ?? dbReg.reg_no;
            dbReg.description = history.others ?? dbReg.description;

            dbReg.date_updated = DateTime.Now;

            var r = Uof.Ireg_internalService.UpdateEntity(dbReg);

            if (r)
            {
                Uof.Ireg_internal_historyService.AddEntity(history);

                return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, message = "更新失败" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateHistory(reg_internal_history history)
        {
            if (history.reg_id == null)
            {
                return Json(new { success = false, message = "参数reg_id不可为空" }, JsonRequestBehavior.AllowGet);
            }

            var dbReg = Uof.Ireg_internalService.GetAll(a => a.id == history.reg_id).FirstOrDefault();
            if (dbReg == null)
            {
                return Json(new { success = false, message = "找不到订单" }, JsonRequestBehavior.AllowGet);
            }

            if (history.address == dbReg.address &&
                history.date_setup == dbReg.date_setup &&
                history.director == dbReg.director &&
                history.name_cn == dbReg.name_cn &&
                history.legal == dbReg.legal &&
                history.others == dbReg.description &&
                history.reg_no == dbReg.reg_no)
            {
                return Json(new { success = false, message = "您没做任何修改" }, JsonRequestBehavior.AllowGet);
            }

            dbReg.address = history.address ?? dbReg.address;
            dbReg.date_setup = history.date_setup ?? dbReg.date_setup;
            dbReg.director = history.director ?? dbReg.director;
            dbReg.name_cn = history.name_cn ?? dbReg.name_cn;
            dbReg.legal = history.legal ?? dbReg.legal;
            dbReg.reg_no = history.reg_no ?? dbReg.reg_no;
            dbReg.description = history.others ?? dbReg.description;

            dbReg.date_updated = DateTime.Now;

            var r = Uof.Ireg_internalService.UpdateEntity(dbReg);

            if (r)
            {
                var dbHistory = Uof.Ireg_internal_historyService.GetById(history.id);
                dbHistory.address = history.address;
                dbHistory.date_setup = history.date_setup;
                dbHistory.director = history.director;
                dbHistory.name_cn = history.name_cn;
                dbHistory.legal = history.legal;
                dbHistory.others = history.others;
                dbHistory.reg_no = history.reg_no;


                Uof.Ireg_internal_historyService.UpdateEntity(dbHistory);

                return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, message = "更新失败" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProgress(int id)
        {
            var p = Uof.Ireg_internalService.GetAll(r => r.id == id).Select(r => new
            {
                id = r.id,
                customer_id = r.customer_id,
                is_done = r.status == 4 ? 1 : 0,
                progress = r.progress,

                address = r.address,
                date_setup = r.date_setup,
                reg_no = r.reg_no,

                is_customs = r.is_customs,
                customs_name = r.customs_name,
                customs_address = r.customs_address,

                taxpayer = r.taxpayer,
                bank_id = r.bank_id,
                is_bookkeeping = r.is_bookkeeping,
                amount_bookkeeping = r.amount_bookkeeping

            }).FirstOrDefault();

            return Json(p, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateProgress(RegInternalProgressRequest request)
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

            var dbInternal = Uof.Ireg_internalService.GetById(request.id);
            if (dbInternal == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }

            if (request.progress_type != "p")
            {
                dbInternal.status = 4;
                dbInternal.date_updated = DateTime.Now;
                dbInternal.date_setup = request.date_setup;
                dbInternal.reg_no = request.reg_no;
                dbInternal.taxpayer = request.taxpayer;
                dbInternal.is_bookkeeping = request.is_bookkeeping;
                dbInternal.amount_bookkeeping = request.amount_bookkeeping;
                dbInternal.bank_id = request.bank_id;

                dbInternal.is_customs = request.is_customs;

                if (dbInternal.date_finish == null)
                {
                    dbInternal.date_finish = request.date_finish ?? DateTime.Today;
                }

                if (request.is_customs == 1)
                {
                    dbInternal.customs_name = request.customs_name;
                    dbInternal.customs_address = request.customs_address;
                }
            }
            else
            {
                if (dbInternal.progress == request.progress && dbInternal.date_finish == request.date_finish)
                {
                    return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);
                }
                dbInternal.status = 4;
                dbInternal.date_finish = request.date_finish;
                dbInternal.progress = request.progress;
            }

            var r = Uof.Ireg_internalService.UpdateEntity(dbInternal);

            if (r)
            {
                if (request.progress_type != "p")
                {
                    Uof.ItimelineService.AddEntity(new timeline()
                    {
                        source_id = dbInternal.id,
                        source_name = "reg_internal",
                        title = "完善了注册资料",
                        is_system = 1,
                        content = string.Format("{0}完善了注册资料", arrs[3])
                    });

                    var waitdeals = new List<waitdeal>();
                    waitdeals.Add(new waitdeal
                    {
                        source = "reg_internal",
                        source_id = dbInternal.id,
                        user_id = dbInternal.salesman_id,
                        router = "internal_view",
                        content = "您的国内注册订单已完成",
                        read_status = 0
                    });
                    if (dbInternal.assistant_id != null && dbInternal.assistant_id != dbInternal.salesman_id)
                    {
                        waitdeals.Add(new waitdeal
                        {
                            source = "reg_internal",
                            source_id = dbInternal.id,
                            user_id = dbInternal.salesman_id,
                            router = "internal_view",
                            content = "您的国内注册订单已完成",
                            read_status = 0
                        });
                    }
                    Uof.IwaitdealService.AddEntities(waitdeals);
                }
                else
                {
                    Uof.ItimelineService.AddEntity(new timeline()
                    {
                        source_id = dbInternal.id,
                        source_name = "reg_internal",
                        title = "更新了订单进度",
                        is_system = 1,
                        content = string.Format("{0}更新了进度: {1} 预计完成日期 {2}", arrs[3], dbInternal.progress, dbInternal.date_finish.Value.ToString("yyyy-MM-dd"))
                    });

                    var waitdeals = new List<waitdeal>();
                    waitdeals.Add(new waitdeal
                    {
                        source = "reg_internal",
                        source_id = dbInternal.id,
                        user_id = dbInternal.salesman_id,
                        router = "internal_view",
                        content = "您的国内注册订单更新了进度",
                        read_status = 0
                    });
                    if (dbInternal.assistant_id != null && dbInternal.assistant_id != dbInternal.salesman_id)
                    {
                        waitdeals.Add(new waitdeal
                        {
                            source = "reg_internal",
                            source_id = dbInternal.id,
                            user_id = dbInternal.assistant_id,
                            router = "internal_view",
                            content = "您的国内注册订单更新了进度",
                            read_status = 0
                        });
                    }
                    Uof.IwaitdealService.AddEntities(waitdeals);
                }
            }

            return Json(new { success = r, message = r ? "" : "更新失败" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddRegItem(RegItemRequest request)
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

            var dbInternal = Uof.Ireg_internalService.GetAll(c => c.id == request.id).FirstOrDefault();
            if (dbInternal == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }

            //dbInternal.prices = request.items;
            dbInternal.amount_transaction = request.amount_transaction;
            dbInternal.date_updated = DateTime.Now;

            Uof.Ireg_internalService.UpdateEntity(dbInternal);

            request.item.master_id = request.id;
            var newItem = Uof.Ireg_internal_itemsService.AddEntity(request.item);

            try
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbInternal.id,
                    source_name = "reg_internal",
                    title = "新增委托事项",
                    is_system = 1,
                    content = string.Format("{0}新增委托事项{1}, 档案号{2}", arrs[3], request.item.name, dbInternal.code)
                });
            }
            catch (Exception)
            {

            }


            return Json(new { success = true, id = newItem.id, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult FinishItem(reg_internal_items item)
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

            var dbItem = Uof.Ireg_internal_itemsService.GetById(item.id);
            dbItem.finisher = item.finisher;
            dbItem.date_finished = item.date_finished;
            dbItem.date_started = item.date_started;
            dbItem.date_updated = DateTime.Now;
            if (dbItem.date_finished != null)
            {
                dbItem.status = 1;
            }
            

            Uof.Ireg_internal_itemsService.UpdateEntity(dbItem);

            try
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbItem.master_id,
                    source_name = "reg_internal",
                    title = "进度反馈",
                    is_system = 1,
                    content = string.Format("{0}反馈进度: {1}", arrs[3], dbItem.name)
                });
            }
            catch (Exception)
            {

            }

            return SuccessResult;
        }

        [HttpPost]
        public ActionResult FinishBaseItem(int id, string items)
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

            var dbItem = Uof.Ireg_internal_itemsService.GetById(id);
            dbItem.sub_items = items;
            dbItem.date_updated = DateTime.Now;

            Uof.Ireg_internal_itemsService.UpdateEntity(dbItem);

            try
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbItem.master_id,
                    source_name = "reg_internal",
                    title = "进度反馈",
                    is_system = 1,
                    content = string.Format("{0}反馈进度: {1}", arrs[3], dbItem.name)
                });
            }
            catch (Exception)
            {

            }

            return SuccessResult;
        }

        [HttpPost]
        public ActionResult SureName(int id, string name, string items)
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

            var dbItem = Uof.Ireg_internal_itemsService.GetById(id);
            dbItem.sub_items = items;
            dbItem.date_updated = DateTime.Now;

            Uof.Ireg_internal_itemsService.UpdateEntity(dbItem);

            try
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbItem.master_id,
                    source_name = "reg_internal",
                    title = "进度反馈",
                    is_system = 1,
                    content = string.Format("{0}反馈进度: {1}", arrs[3], dbItem.name)
                });
            }
            catch (Exception)
            {
            }


            var dbReg = Uof.Ireg_internalService.GetAll(r1 => r1.id == dbItem.master_id).FirstOrDefault();
            dbReg.name_cn = name;
            dbReg.date_updated = DateTime.Now;

            Uof.Ireg_internalService.UpdateEntity(dbReg);


            return SuccessResult;
        }
    }
}
