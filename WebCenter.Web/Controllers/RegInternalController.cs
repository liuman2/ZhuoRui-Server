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

            Expression<Func<reg_internal, bool>> condition = c => c.salesman_id == userId;

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

            var list = Uof.Ireg_internalService.GetAll(condition)
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
                    salesman_name = c.member3.name,
                    finance_review_moment = c.finance_review_moment,
                    submit_review_moment = c.submit_review_moment

                }).ToPagedList(request.index, request.size).ToList();

            var totalRecord = Uof.Ireg_internalService.GetAll(condition).Count();

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

        public ActionResult Add(reg_internal reginternal)
        {
            if (reginternal.customer_id == null)
            {
                return Json(new { success = false, message = "请选择客户" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(reginternal.name_cn))
            {
                return Json(new { success = false, message = "请填写公司中文名称" }, JsonRequestBehavior.AllowGet);
            }
            //if (reginternal.date_setup == null)
            //{
            //    return Json(new { success = false, message = "请填写公司成立日期" }, JsonRequestBehavior.AllowGet);
            //}
            //if (string.IsNullOrEmpty(reginternal.reg_no))
            //{
            //    return Json(new { success = false, message = "请填写统一信用编号" }, JsonRequestBehavior.AllowGet);
            //}
            if (string.IsNullOrEmpty(reginternal.address))
            {
                return Json(new { success = false, message = "请填写公司注册地址" }, JsonRequestBehavior.AllowGet);
            }
            if (reginternal.date_transaction == null)
            {
                return Json(new { success = false, message = "请填写成交日期" }, JsonRequestBehavior.AllowGet);
            }
            if (reginternal.amount_transaction == null)
            {
                return Json(new { success = false, message = "请填写成交金额" }, JsonRequestBehavior.AllowGet);
            }
            if (reginternal.waiter_id == null)
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
            
            reginternal.code = GetNextOrderCode("ZN");

            reginternal.status = 0;
            reginternal.review_status = -1;
            reginternal.creator_id = userId;
            //reginternal.salesman_id = userId;
            reginternal.organization_id = organization_id;

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

            Uof.ItimelineService.AddEntity(new timeline()
            {
                source_id = newInternal.id,
                source_name = "reg_internal",
                title = "新建订单",
                content = string.Format("{0}新建了订单, 单号{1}", arrs[3], newInternal.code)
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
                progress = a.progress,

                invoice_name = a.invoice_name,
                invoice_tax = a.invoice_tax,
                invoice_address = a.invoice_address,
                invoice_tel = a.invoice_tel,
                invoice_bank = a.invoice_bank,
                invoice_account = a.invoice_account,

                salesman_id = a.salesman_id,
                salesman = a.member4.name,
                waiter_id = a.waiter_id,
                waiter_name = a.member6.name,
                manager_id = a.manager_id,
                manager_name = a.member2.name,
                outworker_id = a.outworker_id,
                outworker_name = a.member3.name,

                status = a.status,
                review_status = a.review_status

            }).FirstOrDefault();

            return Json(reg, JsonRequestBehavior.AllowGet);
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
                progress = a.progress,

                invoice_name = a.invoice_name,
                invoice_tax = a.invoice_tax,
                invoice_address = a.invoice_address,
                invoice_tel = a.invoice_tel,
                invoice_bank = a.invoice_bank,
                invoice_account = a.invoice_account,

                salesman_id = a.salesman_id,
                salesman = a.member4.name,
                waiter_id = a.waiter_id,
                waiter_name = a.member6.name,
                manager_id = a.manager_id,
                manager_name = a.member2.name,
                outworker_id = a.outworker_id,
                outworker_name = a.member3.name,

                status = a.status,
                review_status = a.review_status,
                finance_review_moment = a.finance_review_moment,
                submit_review_moment = a.submit_review_moment

            }).FirstOrDefault();

            var list = Uof.IincomeService.GetAll(i => i.source_id == reg.id && i.customer_id == i.customer_id && i.source_name == "reg_internal").ToList();

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

        public ActionResult Update(reg_internal reg)
        {
            var dbReg = Uof.Ireg_internalService.GetById(reg.id);

            if (reg.customer_id == dbReg.customer_id &&
                reg.name_cn == dbReg.name_cn &&
                //reg.date_setup == dbReg.date_setup &&
                //reg.reg_no == dbReg.reg_no &&
                reg.address == dbReg.address &&
                //reg.amount_bookkeeping == dbReg.amount_bookkeeping &&
                //reg.is_bookkeeping == dbReg.is_bookkeeping &&
                //reg.is_customs == dbReg.is_customs &&
                //reg.taxpayer == dbReg.taxpayer &&
                //reg.customs_address == dbReg.customs_address && 
                //reg.customs_name == dbReg.customs_name &&
                reg.legal == dbReg.legal &&
                reg.director == dbReg.director &&
                //reg.bank_id == dbReg.bank_id &&
                reg.date_transaction == dbReg.date_transaction &&
                reg.amount_transaction == dbReg.amount_transaction &&
                reg.invoice_name == dbReg.invoice_name &&
                reg.invoice_tax == dbReg.invoice_tax &&
                reg.invoice_address == dbReg.invoice_address &&
                reg.invoice_tel == dbReg.invoice_tel &&
                reg.invoice_bank == dbReg.invoice_bank &&
                reg.invoice_account == dbReg.invoice_account &&
                reg.salesman_id == dbReg.salesman_id &&
                reg.waiter_id == dbReg.waiter_id &&
                reg.manager_id == dbReg.manager_id &&
                reg.outworker_id == dbReg.outworker_id &&
                reg.description == dbReg.description &&
                reg.currency == dbReg.currency
                )
            {
                return Json(new { success = true, id = reg.id }, JsonRequestBehavior.AllowGet);
            }

            var identityName = HttpContext.User.Identity.Name;
            var arrs = identityName.Split('|');
            if (arrs.Length == 0)
            {
                return new HttpUnauthorizedResult();
            }

            dbReg.customer_id = reg.customer_id;
            dbReg.name_cn = reg.name_cn;
            dbReg.amount_bookkeeping = reg.amount_bookkeeping;
            dbReg.customs_address = reg.customs_address;
            dbReg.customs_name = reg.customs_name;
            dbReg.is_bookkeeping = reg.is_bookkeeping;
            dbReg.is_customs = reg.is_customs;
            dbReg.legal = reg.legal;
            dbReg.outworker_id = reg.outworker_id;
            dbReg.taxpayer = reg.taxpayer;
            dbReg.date_setup = reg.date_setup;
            dbReg.reg_no = reg.reg_no;
            
            dbReg.address = reg.address;
            dbReg.director = reg.director;
           
            dbReg.bank_id = reg.bank_id;
            dbReg.date_transaction = reg.date_transaction;
            dbReg.amount_transaction = reg.amount_transaction;
            dbReg.invoice_name = reg.invoice_name;
            dbReg.invoice_tax = reg.invoice_tax;
            dbReg.invoice_address = reg.invoice_address;
            dbReg.invoice_tel = reg.invoice_tel;
            dbReg.invoice_bank = reg.invoice_bank;
            dbReg.invoice_account = reg.invoice_account;
            dbReg.waiter_id = reg.waiter_id;
            dbReg.salesman_id = reg.salesman_id;
            dbReg.manager_id = reg.manager_id;
            dbReg.description = reg.description;

            dbReg.date_updated = DateTime.Now;

            var r = Uof.Ireg_internalService.UpdateEntity(dbReg);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbReg.id,
                    source_name = "reg_internal",
                    title = "修改订单资料",
                    content = string.Format("{0}修改了订单资料", arrs[3])
                });
            }
            
            return Json(new { success = r, id = reg.id }, JsonRequestBehavior.AllowGet);
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

            var dbReg = Uof.Ireg_internalService.GetById(id);
            if (dbReg == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }
            var t = "";
            if (dbReg.status == 1)
            {
                dbReg.status = 2;
                dbReg.review_status = 1;
                dbReg.finance_reviewer_id = userId;
                dbReg.finance_review_date = DateTime.Now;
                dbReg.finance_review_moment = "";

                t = "财务审核";
                // TODO 通知 提交人，业务员
            }
            else
            {
                dbReg.status = 3;
                dbReg.review_status = 1;
                dbReg.submit_reviewer_id = userId;
                dbReg.submit_review_date = DateTime.Now;
                dbReg.submit_review_moment = "";

                t = "提交的审核";
                // TODO 通知 业务员
            }

            dbReg.date_updated = DateTime.Now;

            var r = Uof.Ireg_internalService.UpdateEntity(dbReg);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbReg.id,
                    source_name = "reg_internal",
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

            var dbReg = Uof.Ireg_internalService.GetById(id);
            if (dbReg == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }
            var t = "";
            if (dbReg.status == 1)
            {
                dbReg.status = 0;
                dbReg.review_status = 0;
                dbReg.finance_reviewer_id = userId;
                dbReg.finance_review_date = DateTime.Now;
                dbReg.finance_review_moment = description;

                t = "驳回了财务审核";
                // TODO 通知 业务员
            }
            else
            {
                dbReg.status = 0;
                dbReg.review_status = 0;
                dbReg.submit_reviewer_id = userId;
                dbReg.submit_review_date = DateTime.Now;
                dbReg.submit_review_moment = description;

                t = "驳回了提交的审核";
                // TODO 通知 业务员
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

            dbReg.date_updated = DateTime.Now;

            var r = Uof.Ireg_internalService.UpdateEntity(dbReg);

            if (r)
            {
                Uof.Ireg_internal_historyService.AddEntity(history);

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

            if (request.is_done == 1)
            {
                dbInternal.status = 4;
                dbInternal.date_updated = DateTime.Now;
                dbInternal.date_finish = request.date_finish;
                dbInternal.date_setup = request.date_setup;
                dbInternal.reg_no = request.reg_no;
                //dbInternal.address = request.address;
                dbInternal.taxpayer = request.taxpayer;
                dbInternal.is_bookkeeping = request.is_bookkeeping;
                dbInternal.amount_bookkeeping = request.amount_bookkeeping;
                dbInternal.bank_id = request.bank_id;

                dbInternal.is_customs = request.is_customs;
                dbInternal.progress = request.progress ?? "已完成";

                if (request.is_customs == 1)
                {
                    dbInternal.customs_name = request.customs_name;
                    dbInternal.customs_address = request.customs_address;
                }
            }
            else
            {
                if (dbInternal.progress == request.progress)
                {
                    return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);
                }

                dbInternal.progress = request.progress;
            }

            var r = Uof.Ireg_internalService.UpdateEntity(dbInternal);

            if (r)
            {
                if (request.is_done == 1)
                {
                    var h = new reg_internal_history()
                    {
                        reg_id = dbInternal.id,
                        name_cn = dbInternal.name_cn,
                        address = dbInternal.address,
                        date_setup = dbInternal.date_setup,
                        director = dbInternal.director,
                        reg_no = dbInternal.reg_no,
                        legal = dbInternal.legal
                    };

                    Uof.Ireg_internal_historyService.AddEntity(h);

                    Uof.ItimelineService.AddEntity(new timeline()
                    {
                        source_id = dbInternal.id,
                        source_name = "reg_internal",
                        title = "完成订单",
                        content = string.Format("{0}完成了订单，完成日期为：{1}", arrs[3], dbInternal.date_finish.Value.ToString("yyyy-MM-dd"))
                    });
                    // TODO 通知 业务员
                }
                else
                {
                    Uof.ItimelineService.AddEntity(new timeline()
                    {
                        source_id = dbInternal.id,
                        source_name = "reg_internal",
                        title = "更新了订单进度",
                        content = string.Format("{0}更新了进度: {1}", arrs[3], dbInternal.progress)
                    });
                }
            }

            return Json(new { success = r, message = r ? "" : "更新失败" }, JsonRequestBehavior.AllowGet);
        }
    }
}