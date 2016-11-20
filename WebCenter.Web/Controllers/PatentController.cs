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

            Expression<Func<patent, bool>> condition = c => true; // c.salesman_id == userId;
            var ops = arrs[4].Split(',');
            if (ops.Count() == 0)
            {
                condition = c => (c.salesman_id == userId || c.assistant_id == userId);
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
            Expression<Func<patent, bool>> customerQuery = c => true;
            if (request.customer_id != null && request.customer_id.Value > 0)
            {
                customerQuery = c => (c.customer_id == request.customer_id);
            }

            // 订单状态
            Expression<Func<patent, bool>> statusQuery = c => true;
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
            Expression<Func<patent, bool>> date1Query = c => true;
            Expression<Func<patent, bool>> date2Query = c => true;
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

            Expression<Func<patent, bool>> nameQuery = c => true;
            if (!string.IsNullOrEmpty(request.name))
            {
                nameQuery = c => (c.name.Contains(request.name));
            }

            Expression<Func<patent, bool>> applicantQuery = c => true;
            if (!string.IsNullOrEmpty(request.applicant))
            {
                applicantQuery = c => (c.applicant.Contains(request.applicant));
            }

            var list = Uof.IpatentService.GetAll(condition)
                .Where(customerQuery)
                .Where(statusQuery)
                .Where(date1Query)
                .Where(date2Query)
                .Where(nameQuery)
                .Where(applicantQuery)
                .OrderByDescending(item => item.code).Select(c => new
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
                    salesman_name = c.member4.name,

                    assistant_id = c.assistant_id,
                    assistant_name = c.member.name,

                    finance_review_moment = c.finance_review_moment,
                    submit_review_moment = c.submit_review_moment,
                    date_empower = c.date_empower,
                    date_regit = c.date_regit,

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

        public ActionResult Add(patent _patent, oldRequest oldRequest)
        {
            //if (_patent.customer_id == null)
            //{
            //    return Json(new { success = false, message = "请选择客户" }, JsonRequestBehavior.AllowGet);
            //}
            //if (string.IsNullOrEmpty(_patent.name))
            //{
            //    return Json(new { success = false, message = "请填写专利名称" }, JsonRequestBehavior.AllowGet);
            //}
            //if (string.IsNullOrEmpty(_patent.applicant))
            //{
            //    return Json(new { success = false, message = "请填写申请人" }, JsonRequestBehavior.AllowGet);
            //}
            //if (string.IsNullOrEmpty(_patent.address))
            //{
            //    return Json(new { success = false, message = "请填写申请人地址" }, JsonRequestBehavior.AllowGet);
            //}
            //if (_patent.date_transaction == null)
            //{
            //    return Json(new { success = false, message = "请填写成交日期" }, JsonRequestBehavior.AllowGet);
            //}
            //if (_patent.amount_transaction == null)
            //{
            //    return Json(new { success = false, message = "请填写成交金额" }, JsonRequestBehavior.AllowGet);
            //}
            //if (_patent.salesman_id == null)
            //{
            //    return Json(new { success = false, message = "请选择业务员" }, JsonRequestBehavior.AllowGet);
            //}

            //if (_patent.waiter_id == null)
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
                                  
            _patent.status = 0;
            _patent.review_status = -1;
            _patent.creator_id = userId;
            //_patent.salesman_id = userId;
            _patent.organization_id = GetOrgIdByUserId(userId); //organization_id;

            
            var nowYear = DateTime.Now.Year;
            if (oldRequest.is_old == 0)
            {
                // 新单据
                _patent.code = GetNextOrderCode(_patent.salesman_id.Value, "ZL");

                // 需要马上年检的 步骤审核流程
                if (_patent.is_annual == 1)
                {
                    _patent.date_finish = DateTime.Now;
                    _patent.status = 4;
                    _patent.review_status = 1;
                }
            }
            else
            {
                // 旧单据
                _patent.status = 4;
                _patent.review_status = 1;

                if (oldRequest.is_already_annual == 1)
                {
                    _patent.annual_year = nowYear;
                }
                else
                {
                    _patent.annual_year = nowYear - 1;
                }
            }

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
                rate = a.rate,
                
                date_accept = a.date_accept,
                date_empower = a.date_empower,
                date_inspection = a.date_inspection,
                progress = a.progress,

                salesman_id = a.salesman_id,
                salesman = a.member4.name,
                waiter_id = a.waiter_id,
                waiter_name = a.member6.name,
                manager_id = a.manager_id,
                manager_name = a.member3.name,

                assistant_id = a.assistant_id,
                assistant_name = a.member.name,

                status = a.status,
                review_status = a.review_status,
                description = a.description,
                date_regit = a.date_regit

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
                rate = a.rate,

                date_accept = a.date_accept,
                date_empower = a.date_empower,
                date_inspection = a.date_inspection,
                date_finish = a.date_finish,
                progress = a.progress,

                salesman_id = a.salesman_id,
                salesman = a.member4.name,
                waiter_id = a.waiter_id,
                waiter_name = a.member6.name,
                manager_id = a.manager_id,
                manager_name = a.member3.name,

                assistant_id = a.assistant_id,
                assistant_name = a.member.name,

                status = a.status,
                review_status = a.review_status,
                finance_review_moment = a.finance_review_moment,
                submit_review_moment = a.submit_review_moment,
                description = a.description,
                date_regit = a.date_regit

            }).FirstOrDefault();

            var list = Uof.IincomeService.GetAll(i => i.source_id == reg.id && i.source_name == "patent").Select(i => new {
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
                _patent.salesman_id == dbPatent.salesman_id &&
                _patent.manager_id == dbPatent.manager_id && 
                _patent.description == dbPatent.description &&
                _patent.currency == dbPatent.currency &&
                _patent.rate == dbPatent.rate &&
                _patent.date_regit == dbPatent.date_regit &&
                _patent.assistant_id == dbPatent.assistant_id
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

            var isChangeCurrency = _patent.currency != dbPatent.currency || _patent.rate != dbPatent.rate;

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
            dbPatent.rate = _patent.rate;
            
            dbPatent.date_accept = _patent.date_accept;
            dbPatent.date_empower = _patent.date_empower;
            dbPatent.date_inspection = _patent.date_inspection;
            dbPatent.progress = _patent.progress;

            dbPatent.salesman_id = _patent.salesman_id;
            dbPatent.assistant_id = _patent.assistant_id;
            dbPatent.waiter_id = _patent.waiter_id;
            dbPatent.manager_id = _patent.manager_id;
            dbPatent.description = _patent.description;
            dbPatent.date_updated = DateTime.Now;
            dbPatent.date_regit = _patent.date_regit;
            var r = Uof.IpatentService.UpdateEntity(dbPatent);

            if (r)
            {
                if (isChangeCurrency)
                {
                    var list = Uof.IincomeService.GetAll(i => i.source_id == _patent.id && i.source_name == "patent").ToList();
                    if (list.Count() > 0)
                    {
                        foreach (var item in list)
                        {
                            item.currency = _patent.currency;
                            item.rate = _patent.rate;
                        }

                        Uof.IincomeService.UpdateEntities(list);
                    }
                }

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
            dbPatent.review_status = -1;
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

                var ids = GetFinanceMembers();
                if (ids.Count() > 0)
                {
                    var waitdeals = new List<waitdeal>();
                    foreach (var item in ids)
                    {
                        waitdeals.Add(new waitdeal
                        {
                            source = "patent",
                            source_id = dbPatent.id,
                            user_id = item,
                            router = "patent_view",
                            content = "您有专利订单需要财务审核",
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

            var dbPatent = Uof.IpatentService.GetById(id);
            if (dbPatent == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }
            var t = "";
            var waitdeals = new List<waitdeal>();
            if (dbPatent.status == 1)
            {
                dbPatent.status = 2;
                dbPatent.review_status = 1;
                dbPatent.finance_reviewer_id = userId;
                dbPatent.finance_review_date = DateTime.Now;
                dbPatent.finance_review_moment = "";

                t = "财务审核";
                waitdeals.Add(new waitdeal
                {
                    source = "patent",
                    source_id = dbPatent.id,
                    user_id = dbPatent.salesman_id,
                    router = "patent_view",
                    content = "您的专利订单已通过财务审核",
                    read_status = 0
                });
                if (dbPatent.assistant_id != null && dbPatent.assistant_id != dbPatent.salesman_id)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "patent",
                        source_id = dbPatent.id,
                        user_id = dbPatent.assistant_id,
                        router = "patent_view",
                        content = "您的专利订单已通过财务审核",
                        read_status = 0
                    });
                }

                var ids = GetSubmitMembers();
                if (ids.Count() > 0)
                {
                    foreach (var item in ids)
                    {
                        waitdeals.Add(new waitdeal
                        {
                            source = "patent",
                            source_id = dbPatent.id,
                            user_id = item,
                            router = "patent_view",
                            content = "您有专利订单需要提交审核",
                            read_status = 0
                        });
                    }
                }
            }
            else
            {
                dbPatent.status = 3;
                dbPatent.review_status = 1;
                dbPatent.submit_reviewer_id = userId;
                dbPatent.submit_review_date = DateTime.Now;
                dbPatent.submit_review_moment = "";

                t = "提交的审核";
                waitdeals.Add(new waitdeal
                {
                    source = "patent",
                    source_id = dbPatent.id,
                    user_id = dbPatent.salesman_id,
                    router = "patent_view",
                    content = "您的专利订单已通过提交审核",
                    read_status = 0
                });

                if (dbPatent.assistant_id != null && dbPatent.assistant_id != dbPatent.salesman_id)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "patent",
                        source_id = dbPatent.id,
                        user_id = dbPatent.assistant_id,
                        router = "patent_view",
                        content = "您的专利订单已通过提交审核",
                        read_status = 0
                    });
                }
            }

            dbPatent.date_updated = DateTime.Now;

            var r = Uof.IpatentService.UpdateEntity(dbPatent);

            if (r)
            {
                Uof.IwaitdealService.AddEntities(waitdeals);

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
            var waitdeals = new List<waitdeal>();
            if (dbPatent.status == 1)
            {
                dbPatent.status = 0;
                dbPatent.review_status = 0;
                dbPatent.finance_reviewer_id = userId;
                dbPatent.finance_review_date = DateTime.Now;
                dbPatent.finance_review_moment = description;

                t = "驳回了财务审核";

                waitdeals.Add(new waitdeal
                {
                    source = "patent",
                    source_id = dbPatent.id,
                    user_id = dbPatent.salesman_id,
                    router = "patent_view",
                    content = "您的专利订单未通过财务审核",
                    read_status = 0
                });
                if (dbPatent.assistant_id != null && dbPatent.assistant_id != dbPatent.salesman_id)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "patent",
                        source_id = dbPatent.id,
                        user_id = dbPatent.assistant_id,
                        router = "patent_view",
                        content = "您的专利订单未通过财务审核",
                        read_status = 0
                    });
                }
            }
            else
            {
                dbPatent.status = 0;
                dbPatent.review_status = 0;
                dbPatent.submit_reviewer_id = userId;
                dbPatent.submit_review_date = DateTime.Now;
                dbPatent.submit_review_moment = description;

                t = "驳回了提交的审核";
                waitdeals.Add(new waitdeal
                {
                    source = "patent",
                    source_id = dbPatent.id,
                    user_id = dbPatent.salesman_id,
                    router = "patent_view",
                    content = "您的专利订单未通过提交审核",
                    read_status = 0
                });
                if (dbPatent.assistant_id != null && dbPatent.assistant_id != dbPatent.salesman_id)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "patent",
                        source_id = dbPatent.id,
                        user_id = dbPatent.assistant_id,
                        router = "patent_view",
                        content = "您的专利订单未通过提交审核",
                        read_status = 0
                    });
                }
            }

            dbPatent.date_updated = DateTime.Now;

            var r = Uof.IpatentService.UpdateEntity(dbPatent);

            if (r)
            {
                Uof.IwaitdealService.AddEntities(waitdeals);

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

                var waitdeals = new List<waitdeal>();
                waitdeals.Add(new waitdeal
                {
                    source = "patent",
                    source_id = dbPatent.id,
                    user_id = dbPatent.salesman_id,
                    router = "patent_view",
                    content = "您的专利订单已完成",
                    read_status = 0
                });
                if (dbPatent.assistant_id != null && dbPatent.assistant_id != dbPatent.salesman_id)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "patent",
                        source_id = dbPatent.id,
                        user_id = dbPatent.assistant_id,
                        router = "patent_view",
                        content = "您的专利订单已完成",
                        read_status = 0
                    });
                }
                Uof.IwaitdealService.AddEntities(waitdeals);
            }

            return Json(new { success = r, message = r ? "" : "保存失败" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProgress(int id)
        {
            var p = Uof.IpatentService.GetAll(r => r.id == id).Select(r => new
            {
                id = r.id,
                progress = r.progress,
                is_done = r.status == 4 ? 1 : 0,

                date_accept = r.date_accept,
                date_empower = r.date_empower
            }).FirstOrDefault();

            return Json(p, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateProgress(PatentProgressRequest request)
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

            var dbPantent = Uof.IpatentService.GetById(request.id);
            if (dbPantent == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }

            if (request.progress_type != "p")
            {
                dbPantent.status = 4;
                dbPantent.date_updated = DateTime.Now;
                dbPantent.date_accept = request.date_accept;
                dbPantent.date_empower = request.date_empower;
                dbPantent.date_regit = request.date_regit;
                if (dbPantent.date_finish == null)
                {
                    dbPantent.date_finish = request.date_finish ?? DateTime.Today;
                }
            }
            else
            {
                if (dbPantent.progress == request.progress && dbPantent.date_finish == request.date_finish)
                {
                    return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);
                }
                dbPantent.status = 4;
                dbPantent.date_finish = request.date_finish;
                dbPantent.progress = request.progress;
            }

            var r = Uof.IpatentService.UpdateEntity(dbPantent);

            if (r)
            {
                if (request.progress_type != "p")
                {
                    Uof.ItimelineService.AddEntity(new timeline()
                    {
                        source_id = dbPantent.id,
                        source_name = "patent",
                        title = "完善了注册资料",
                        content = string.Format("{0}完善了注册资料", arrs[3])
                    });

                    var waitdeals = new List<waitdeal>();
                    waitdeals.Add(new waitdeal
                    {
                        source = "patent",
                        source_id = dbPantent.id,
                        user_id = dbPantent.salesman_id,
                        router = "patent_view",
                        content = "您的专利订单已完成",
                        read_status = 0
                    });
                    if (dbPantent.assistant_id != null && dbPantent.assistant_id != dbPantent.salesman_id)
                    {
                        waitdeals.Add(new waitdeal
                        {
                            source = "patent",
                            source_id = dbPantent.id,
                            user_id = dbPantent.assistant_id,
                            router = "patent_view",
                            content = "您的专利订单已完成",
                            read_status = 0
                        });
                    }

                    Uof.IwaitdealService.AddEntities(waitdeals);
                }
                else
                {
                    Uof.ItimelineService.AddEntity(new timeline()
                    {
                        source_id = dbPantent.id,
                        source_name = "patent",
                        title = "更新了订单进度",
                        content = string.Format("{0}更新了进度: {1} 预计完成日期 {2}", arrs[3], dbPantent.progress, dbPantent.date_finish.Value.ToString("yyyy-MM-dd"))
                    });

                    var waitdeals = new List<waitdeal>();
                    waitdeals.Add(new waitdeal
                    {
                        source = "patent",
                        source_id = dbPantent.id,
                        user_id = dbPantent.salesman_id,
                        router = "patent_view",
                        content = "您的专利订单更新了进度",
                        read_status = 0
                    });
                    if (dbPantent.assistant_id != null && dbPantent.assistant_id != dbPantent.salesman_id)
                    {
                        waitdeals.Add(new waitdeal
                        {
                            source = "patent",
                            source_id = dbPantent.id,
                            user_id = dbPantent.assistant_id,
                            router = "patent_view",
                            content = "您的专利订单更新了进度",
                            read_status = 0
                        });
                    }

                    Uof.IwaitdealService.AddEntities(waitdeals);
                }
            }
            
            return Json(new { success = r, message = r ? "" : "更新失败" }, JsonRequestBehavior.AllowGet);
        }
    }
}