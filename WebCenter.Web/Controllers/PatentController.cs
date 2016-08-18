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
    public class PatentController : BaseController
    {
        public PatentController(IUnitOfWork UOF)
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

            Expression<Func<patent, bool>> condition = c => c.salesman_id == userId;
            // 客户id
            if (request.customer_id != null && request.customer_id.Value > 0)
            {
                Expression<Func<patent, bool>> tmp = c => (c.customer_id == request.customer_id);
                condition = tmp;
            }
            // 订单状态
            if (request.status != null)
            {
                if (request.status == 2)
                {
                    Expression<Func<patent, bool>> tmp = c => (c.status == 2 || c.status == 3);
                    condition = tmp;
                }
                else
                {
                    Expression<Func<patent, bool>> tmp = c => (c.status == request.status.Value);
                    condition = tmp;
                }                
            }
            // 成交开始日期
            if (request.start_time != null)
            {
                Expression<Func<patent, bool>> tmp = c => (c.date_transaction >= request.start_time.Value);
                condition = tmp;
            }
            // 成交结束日期
            if (request.end_time != null)
            {
                var endTime = request.end_time.Value.AddDays(1);
                Expression<Func<patent, bool>> tmp = c => (c.date_transaction < endTime);
                condition = tmp;
            }

            if (!string.IsNullOrEmpty(request.name))
            {
                Expression<Func<patent, bool>> tmp = c => (c.name.Contains(request.name));
                condition = tmp;
            }

            if (!string.IsNullOrEmpty(request.applicant))
            {
                Expression<Func<patent, bool>> tmp = c => (c.applicant.Contains(request.applicant));
                condition = tmp;
            }

            var list = Uof.IpatentService.GetAll(condition).OrderByDescending(item => item.id).Select(c => new
            {
                id = c.id,
                code = c.code,
                customer_id = c.customer_id,
                customer_name = c.customer.name,
                type = c.type,
                name = c.name,
                applicant = c.applicant,
                address = c.address,
                patent_type = c.patent_type,
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

            var totalRecord = Uof.IpatentService.GetAll(condition).Count();

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

        public ActionResult Add(patent _patent)
        {
            if (_patent.customer_id == null)
            {
                return Json(new { success = false, message = "请选择客户" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(_patent.name))
            {
                return Json(new { success = false, message = "请填写专利名称" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(_patent.applicant))
            {
                return Json(new { success = false, message = "请填写申请人" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(_patent.address))
            {
                return Json(new { success = false, message = "请填写申请人地址" }, JsonRequestBehavior.AllowGet);
            }
            if (_patent.date_transaction == null)
            {
                return Json(new { success = false, message = "请填写成交日期" }, JsonRequestBehavior.AllowGet);
            }
            if (_patent.amount_transaction == null)
            {
                return Json(new { success = false, message = "请填写成交金额" }, JsonRequestBehavior.AllowGet);
            }
            if (_patent.waiter_id == null)
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
                        
            _patent.code = GetNextOrderCode("ZL");

            _patent.status = 0;
            _patent.review_status = -1;
            _patent.creator_id = userId;
            _patent.salesman_id = userId;
            _patent.organization_id = organization_id;

            var newPatent = Uof.IpatentService.AddEntity(_patent);
            if (newPatent == null)
            {
                return Json(new { success = false, message = "添加失败" }, JsonRequestBehavior.AllowGet);
            }

            Uof.ItimelineService.AddEntity(new timeline()
            {
                source_id = newPatent.id,
                source_name = "patent",
                title = "新建订单",
                content = string.Format("{0}新建了订单, 单号{1}", arrs[3], newPatent.code)
            });

            return Json(new { id = newPatent.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get(int id)
        {
            var reg = Uof.IpatentService.GetAll(a => a.id == id).Select(a => new
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
                card_no= a.card_no,
                designer = a.designer,
                patent_type = a.patent_type,
                patent_purpose = a.patent_purpose,

                reg_mode = a.reg_mode,
                date_transaction = a.date_transaction,
                amount_transaction = a.amount_transaction,
                currency = a.currency,
                
                date_accept = a.date_accept,
                date_empower = a.date_empower,
                date_inspection = a.date_inspection,
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
            var reg = Uof.IpatentService.GetAll(a => a.id == id).Select(a => new
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
                card_no = a.card_no,
                designer = a.designer,
                patent_type = a.patent_type,
                patent_purpose = a.patent_purpose,

                reg_mode = a.reg_mode,
                date_transaction = a.date_transaction,
                amount_transaction = a.amount_transaction,
                currency = a.currency,

                date_accept = a.date_accept,
                date_empower = a.date_empower,
                date_inspection = a.date_inspection,
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

        public ActionResult Update(patent _patent)
        {
            var dbPatent = Uof.IpatentService.GetById(_patent.id);

            if (_patent.customer_id == dbPatent.customer_id &&
                _patent.type == dbPatent.type &&
                _patent.name == dbPatent.name &&
                _patent.applicant == dbPatent.applicant &&
                _patent.address == dbPatent.address &&
                _patent.card_no == dbPatent.card_no &&
                _patent.designer == dbPatent.designer &&                
                _patent.patent_type == dbPatent.patent_type &&
                _patent.patent_purpose == dbPatent.patent_purpose &&
                _patent.reg_mode == dbPatent.reg_mode &&
                _patent.date_transaction == dbPatent.date_transaction &&
                _patent.amount_transaction == dbPatent.amount_transaction &&
                _patent.currency == dbPatent.currency &&
                _patent.date_empower == dbPatent.date_empower &&
                _patent.date_accept == dbPatent.date_accept &&                
                _patent.progress == dbPatent.progress &&                
                _patent.waiter_id == dbPatent.waiter_id &&
                _patent.manager_id == dbPatent.manager_id && 
                _patent.description == dbPatent.description &&
                _patent.currency == dbPatent.currency
                )
            {
                return Json(new { id = _patent.id }, JsonRequestBehavior.AllowGet);
            }

            var identityName = HttpContext.User.Identity.Name;
            var arrs = identityName.Split('|');
            if (arrs.Length == 0)
            {
                return new HttpUnauthorizedResult();
            }

            dbPatent.customer_id = _patent.customer_id;
            dbPatent.type = _patent.type;
            dbPatent.name = _patent.name;
            dbPatent.applicant = _patent.applicant;
            dbPatent.address = _patent.address;
            dbPatent.patent_type = _patent.patent_type;
            dbPatent.patent_purpose = _patent.patent_purpose;
            dbPatent.reg_mode = _patent.reg_mode;
            dbPatent.date_transaction = _patent.date_transaction;
            dbPatent.amount_transaction = _patent.amount_transaction;
            dbPatent.currency = _patent.currency;
            
            dbPatent.date_accept = _patent.date_accept;
            dbPatent.date_empower = _patent.date_empower;
            dbPatent.date_inspection = _patent.date_inspection;
            dbPatent.progress = _patent.progress;

            dbPatent.waiter_id = _patent.waiter_id;
            dbPatent.manager_id = _patent.manager_id;
            dbPatent.description = _patent.description;
            dbPatent.date_updated = DateTime.Now;

            var r = Uof.IpatentService.UpdateEntity(dbPatent);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbPatent.id,
                    source_name = "patent",
                    title = "修改订单资料",
                    content = string.Format("{0}修改了订单资料", arrs[3])
                });
            }
            return Json(new { success = r, id = dbPatent.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Submit(int id)
        {
            var dbPatent = Uof.IpatentService.GetById(id);
            if (dbPatent == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }

            dbPatent.status = 1;
            dbPatent.date_updated = DateTime.Now;

            var r = Uof.IpatentService.UpdateEntity(dbPatent);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbPatent.id,
                    source_name = "patent",
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

            var dbPatent = Uof.IpatentService.GetById(id);
            if (dbPatent == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }
            var t = "";
            if (dbPatent.status == 1)
            {
                dbPatent.status = 2;
                dbPatent.review_status = 1;
                dbPatent.finance_reviewer_id = userId;
                dbPatent.finance_review_date = DateTime.Now;
                dbPatent.finance_review_moment = "";

                t = "财务审核";
                // TODO 通知 提交人，业务员
            }
            else
            {
                dbPatent.status = 3;
                dbPatent.review_status = 1;
                dbPatent.submit_reviewer_id = userId;
                dbPatent.submit_review_date = DateTime.Now;
                dbPatent.submit_review_moment = "";

                t = "提交的审核";
                // TODO 通知 业务员
            }

            dbPatent.date_updated = DateTime.Now;

            var r = Uof.IpatentService.UpdateEntity(dbPatent);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbPatent.id,
                    source_name = "patent",
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

            var dbPatent = Uof.IpatentService.GetById(id);
            if (dbPatent == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }
            var t = "";
            if (dbPatent.status == 1)
            {
                dbPatent.status = 2;
                dbPatent.review_status = 0;
                dbPatent.finance_reviewer_id = userId;
                dbPatent.finance_review_date = DateTime.Now;
                dbPatent.finance_review_moment = description;

                t = "驳回了财务审核";
                // TODO 通知 业务员
            }
            else
            {
                dbPatent.status = 3;
                dbPatent.review_status = 0;
                dbPatent.submit_reviewer_id = userId;
                dbPatent.submit_review_date = DateTime.Now;
                dbPatent.submit_review_moment = description;

                t = "驳回了提交的审核";
                // TODO 通知 业务员
            }

            dbPatent.date_updated = DateTime.Now;

            var r = Uof.IpatentService.UpdateEntity(dbPatent);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbPatent.id,
                    source_name = "patent",
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

            var dbPatent = Uof.IpatentService.GetById(id);
            if (dbPatent == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }
            dbPatent.status = 4;
            dbPatent.date_updated = DateTime.Now;
            dbPatent.date_finish = date_finish;
            dbPatent.progress = "已完成";

            var r = Uof.IpatentService.UpdateEntity(dbPatent);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbPatent.id,
                    source_name = "patent",
                    title = "完成订单",
                    content = string.Format("{0}完成了订单，完成日期为：{1}", arrs[3], date_finish.ToString("yyyy-MM-dd"))
                });

                // TODO 通知 业务员
            }

            return Json(new { success = r, message = r ? "" : "保存失败" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProgress(int id)
        {
            var p = Uof.IpatentService.GetAll(r => r.id == id).Select(r => new
            {
                id = r.id,
                name = r.progress,
                date_accept = r.date_accept,
                date_empower = r.date_empower
            });

            return Json(p, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateProgress(PatentProgressRequest request)
        {
            var dbPantent = Uof.IpatentService.GetById(request.id);
            if (dbPantent == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }

            if (dbPantent.progress == request.name &&
                dbPantent.date_accept == request.date_accept &&
                dbPantent.date_empower == request.date_empower)
            {
                return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);
            }

            dbPantent.progress = request.name;
            dbPantent.date_accept = request.date_accept;
            dbPantent.date_empower = request.date_empower;

            var r = Uof.IpatentService.UpdateEntity(dbPantent);

            return Json(new { success = r, message = r ? "" : "更新失败" }, JsonRequestBehavior.AllowGet);
        }
    }
}