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
                var endTime = request.end_time.Value.AddDays(1);
                Expression<Func<reg_abroad, bool>> tmp = c => (c.date_transaction < endTime);
                condition = tmp;
            }
            
            var list = Uof.Ireg_abroadService.GetAll(condition).OrderByDescending(item => item.id).Select(c => new
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

        public ActionResult Add(reg_abroad aboad)
        {
            if (aboad.customer_id == null)
            {
                return Json(new { success = false, message = "请选择客户" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(aboad.name_cn))
            {
                return Json(new { success = false, message = "请填写公司中文名称" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(aboad.name_en))
            {
                return Json(new { success = false, message = "请填写公司英文名称" }, JsonRequestBehavior.AllowGet);
            }
            if (aboad.date_setup == null)
            {
                return Json(new { success = false, message = "请填写公司成立日期" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(aboad.reg_no))
            {
                return Json(new { success = false, message = "请填写公司注册编号" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(aboad.region))
            {
                return Json(new { success = false, message = "请填写公司注册地区" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(aboad.address))
            {
                return Json(new { success = false, message = "请填写公司注册地址" }, JsonRequestBehavior.AllowGet);
            }
            if (aboad.date_transaction == null)
            {
                return Json(new { success = false, message = "请填写成交日期" }, JsonRequestBehavior.AllowGet);
            }
            if (aboad.amount_transaction == null)
            {
                return Json(new { success = false, message = "请填写成交金额" }, JsonRequestBehavior.AllowGet);
            }
            if (aboad.waiter_id == null)
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

            // TODO: 自动编码
            aboad.code = "";

            aboad.status = 0;
            aboad.review_status = -1;
            aboad.creator_id = userId;
            aboad.salesman_id = userId;
            aboad.organization_id = organization_id;

            if (aboad.is_open_bank == 0)
            {
                aboad.bank_id = null;
            }

            var newAbroad = Uof.Ireg_abroadService.AddEntity(aboad);
            if (newAbroad == null)
            {
                return Json(new { success = false, message = "添加失败" }, JsonRequestBehavior.AllowGet);
            }

            Uof.ItimelineService.AddEntity(new timeline()
            {
                source_id = newAbroad.id,
                source_name = "reg_abroad",
                title = "新建订单",
                content = string.Format("{0}新建了订单, 单号{1}", arrs[3], aboad.code)
            });

            return Json(new { id = newAbroad.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get(int id)
        {
            var reg = Uof.Ireg_abroadService.GetAll(a => a.id == id).Select(a => new
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
                name_en = a.name_en,
                date_setup = a.date_setup,
                reg_no = a.reg_no,
                region = a.region,
                address = a.address,
                date_transaction = a.date_transaction,
                amount_transaction = a.amount_transaction,
                director = a.director,
                is_open_bank = a.is_open_bank,
                bank_id = a.bank_id,
                bank_name = a.bank_account.bank,
                holder = a.bank_account.holder,
                account = a.bank_account.account,
                date_finish = a.date_finish,
                currency = a.currency,

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

                status = a.status,
                review_status = a.review_status

            }).FirstOrDefault();

            return Json(reg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetView(int id)
        {
            var reg = Uof.Ireg_abroadService.GetAll(a => a.id == id).Select(a => new
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
                name_en = a.name_en,
                date_setup = a.date_setup,
                reg_no = a.reg_no,
                region = a.region,
                address = a.address,
                date_transaction = a.date_transaction,
                amount_transaction = a.amount_transaction,
                director = a.director,
                is_open_bank = a.is_open_bank,
                bank_id = a.bank_id,
                bank_name = a.bank_account.bank,
                holder = a.bank_account.holder,
                account = a.bank_account.account,
                date_finish = a.date_finish,
                currency = a.currency,

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

        public ActionResult Update(reg_abroad reg)
        {
            var dbReg = Uof.Ireg_abroadService.GetById(reg.id);

            if (reg.customer_id == dbReg.customer_id && 
                reg.name_cn == dbReg.name_cn && 
                reg.name_en == dbReg.name_en &&
                reg.date_setup == dbReg.date_setup &&
                reg.reg_no == dbReg.reg_no &&
                reg.region == dbReg.region &&
                reg.address == dbReg.address &&
                reg.director == dbReg.director &&
                reg.is_open_bank == dbReg.is_open_bank &&
                reg.bank_id == dbReg.bank_id &&
                reg.date_transaction == dbReg.date_transaction &&
                reg.amount_transaction == dbReg.amount_transaction &&
                reg.invoice_name == dbReg.invoice_name &&
                reg.invoice_tax == dbReg.invoice_tax &&
                reg.invoice_address == dbReg.invoice_address &&
                reg.invoice_tel == dbReg.invoice_tel &&
                reg.invoice_bank == dbReg.invoice_bank &&
                reg.invoice_account == dbReg.invoice_account &&
                reg.waiter_id == dbReg.waiter_id &&
                reg.manager_id == dbReg.manager_id && 
                reg.description == dbReg.description &&
                reg.currency == dbReg.currency
                )
            {
                return Json(new { id = reg.id }, JsonRequestBehavior.AllowGet);
            }

            var identityName = HttpContext.User.Identity.Name;
            var arrs = identityName.Split('|');
            if (arrs.Length == 0)
            {
                return new HttpUnauthorizedResult();
            }

            dbReg.customer_id = reg.customer_id;
            dbReg.name_cn = reg.name_cn;
            dbReg.name_en = reg.name_en;
            dbReg.date_setup = reg.date_setup;
            dbReg.reg_no = reg.reg_no;
            dbReg.region = reg.region;
            dbReg.address = reg.address;
            dbReg.director = reg.director;
            dbReg.is_open_bank = reg.is_open_bank;
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
            dbReg.manager_id = reg.manager_id;
            dbReg.description = reg.description;
            dbReg.currency = reg.currency;

            if (reg.is_open_bank == 0)
            {
                dbReg.bank_id = null;
            }

            dbReg.date_updated = DateTime.Now;

            var r = Uof.Ireg_abroadService.UpdateEntity(dbReg);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbReg.id,
                    source_name = "reg_abroad",
                    title = "修改订单资料",
                    content = string.Format("{0}修改了订单资料", arrs[3])
                });
            }
            return Json(new { success = r, id = reg.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Submit(int id)
        {
            var dbReg = Uof.Ireg_abroadService.GetById(id);
            if (dbReg == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }

            dbReg.status = 1;
            dbReg.date_updated = DateTime.Now;

            var r = Uof.Ireg_abroadService.UpdateEntity(dbReg);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbReg.id,
                    source_name = "reg_abroad",
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

            var dbReg = Uof.Ireg_abroadService.GetById(id);
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

            var r = Uof.Ireg_abroadService.UpdateEntity(dbReg);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbReg.id,
                    source_name = "reg_abroad",
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

            var dbReg = Uof.Ireg_abroadService.GetById(id);
            if (dbReg == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }
            var t = "";
            if (dbReg.status == 1)
            {
                dbReg.status = 2;
                dbReg.review_status = 0;
                dbReg.finance_reviewer_id = userId;
                dbReg.finance_review_date = DateTime.Now;
                dbReg.finance_review_moment = description;

                t = "驳回了财务审核";
                // TODO 通知 业务员
            }
            else
            {
                dbReg.status = 3;
                dbReg.review_status = 0;
                dbReg.submit_reviewer_id = userId;
                dbReg.submit_review_date = DateTime.Now;
                dbReg.submit_review_moment = description;

                t = "驳回了提交的审核";
                // TODO 通知 业务员
            }

            dbReg.date_updated = DateTime.Now;

            var r = Uof.Ireg_abroadService.UpdateEntity(dbReg);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbReg.id,
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

            var dbReg = Uof.Ireg_abroadService.GetById(id);
            if (dbReg == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }
            dbReg.status = 4;
            dbReg.date_updated = DateTime.Now;
            dbReg.date_finish = date_finish;
            var r = Uof.Ireg_abroadService.UpdateEntity(dbReg);

            if (r)
            {
                var h = new reg_history()
                {
                    reg_id = dbReg.id,
                    name_cn = dbReg.name_cn,
                    name_en = dbReg.name_en,
                    address = dbReg.address,
                    date_setup = dbReg.date_setup,
                    director = dbReg.director,
                    region = dbReg.region,
                    reg_no = dbReg.reg_no
                };

                Uof.Ireg_historyService.AddEntity(h);

                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbReg.id,
                    source_name = "reg_abroad",
                    title = "完成订单",
                    content = string.Format("{0}完成了订单，完成日期为：{1}", arrs[3], date_finish.ToString("yyyy-MM-dd"))
                });

                // TODO 通知 业务员
            }

            return Json(new { success = r, message = r ? "" : "保存失败" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult History(int id, int index = 1, int size = 10)
        {
            var totalRecord = Uof.Ireg_historyService.GetAll(h => h.reg_id == id).Count();

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
                return Json(new { page = page, items = new List<reg_history>() }, JsonRequestBehavior.AllowGet);
            }

            var list = Uof.Ireg_historyService.GetAll(h => h.reg_id == id).OrderByDescending(item => item.date_created).ToPagedList(index, size).ToList();
            
            var result = new
            {
                page = page,
                items = list
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHistoryTop(int id)
        {
            var last = Uof.Ireg_historyService.GetAll(h => h.reg_id == id).OrderByDescending(h => h.id).FirstOrDefault();

            return Json(last, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddHistory(reg_history history)
        {
            if (history.reg_id == null)
            {
                return Json(new { success = false, message = "参数reg_id不可为空" }, JsonRequestBehavior.AllowGet);
            }

            var dbReg = Uof.Ireg_abroadService.GetAll(a=>a.id == history.reg_id).FirstOrDefault();
            if (dbReg == null)
            {
                return Json(new { success = false, message = "找不到订单" }, JsonRequestBehavior.AllowGet);
            }

            if (history.address == dbReg.address && 
                history.date_setup == dbReg.date_setup && 
                history.director == dbReg.director &&
                history.name_cn == dbReg.name_cn &&
                history.name_en == dbReg.name_en && 
                history.region == dbReg.region &&
                history.reg_no == dbReg.reg_no)
            {
                return Json(new { success = false, message = "您没做任何修改" }, JsonRequestBehavior.AllowGet);
            }

            dbReg.address = history.address ?? dbReg.address;
            dbReg.date_setup = history.date_setup ?? dbReg.date_setup;
            dbReg.director = history.director ?? dbReg.director;
            dbReg.name_cn = history.name_cn ?? dbReg.name_cn;
            dbReg.name_en = history.name_en ?? dbReg.name_en;
            dbReg.region = history.region ?? dbReg.region;
            dbReg.reg_no = history.reg_no ?? dbReg.reg_no;

            dbReg.date_updated = DateTime.Now;

            var r = Uof.Ireg_abroadService.UpdateEntity(dbReg);

            if (r)
            {
                Uof.Ireg_historyService.AddEntity(history);

                return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, message = "更新失败" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProgress(int id)
        {
            var p = Uof.Ireg_abroadService.GetAll(r => r.id == id).Select(r => new
            {
                id = r.id,
                name = r.progress
            });

            return Json(p, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateProgress()
    }
}