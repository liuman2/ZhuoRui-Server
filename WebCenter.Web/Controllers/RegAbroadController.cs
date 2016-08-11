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

            // TODO: 添加log

            return Json(new { id = newAbroad.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get(int id)
        {
            var reg = GetById(id);

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
                reg.description == dbReg.description
                )
            {
                return SuccessResult;
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

            if (reg.is_open_bank == 0)
            {
                dbReg.bank_id = null;
            }

            dbReg.date_updated = DateTime.Now;

            var r = Uof.Ireg_abroadService.UpdateEntity(dbReg);

            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
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
            return Json(new { success = r, message = r ? "" : "更新失败" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PassAudit(int id)
        {
            //          `finance_reviewer_id` int(11) DEFAULT NULL COMMENT '财务审核人员ID',
            //`finance_review_date` datetime DEFAULT NULL COMMENT '财务审核日期',
            //`finance_review_moment` varchar(100) DEFAULT NULL COMMENT '财务审核意见',

            //`submit_reviewer_id` int(11) DEFAULT NULL COMMENT '提交审核人员ID',
            //`submit_review_date` datetime DEFAULT NULL COMMENT '提交审核日期',
            //`submit_review_moment` varchar(100) DEFAULT NULL COMMENT '提交审核意见',

            //`review_status` int(11) DEFAULT NULL COMMENT '审核状体 未审核：-1；未通过：0；已通过：1',

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
            if (dbReg.status == 1)
            {
                dbReg.status = 2;
                dbReg.review_status = 1;
                dbReg.finance_reviewer_id = userId;
                dbReg.finance_review_date = DateTime.Now;
                dbReg.finance_review_moment = "";

                // TODO
            }
            else
            {
                dbReg.status = 3;
                dbReg.review_status = 1;
                dbReg.submit_reviewer_id = userId;
                dbReg.submit_review_date = DateTime.Now;
                dbReg.submit_review_moment = "";
            }

            dbReg.date_updated = DateTime.Now;

            var r = Uof.Ireg_abroadService.UpdateEntity(dbReg);
            return Json(new { success = r, message = r ? "" : "审核失败" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RefuseAudit(int id, string moment)
        {
            return ErrorResult;
        }

        private object GetById(int id)
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

            return reg;
        }
    }
}