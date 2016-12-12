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
    public class TrademarkController : BaseController
    {
        public TrademarkController(IUnitOfWork UOF)
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

            Expression<Func<trademark, bool>> condition = c => true; // c.salesman_id == userId;
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
            Expression<Func<trademark, bool>> customerQuery = c => true;
            if (request.customer_id != null && request.customer_id.Value > 0)
            {
                customerQuery = c => (c.customer_id == request.customer_id);
            }
            // 订单状态
            Expression<Func<trademark, bool>> statusQuery = c => true;
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
            Expression<Func<trademark, bool>> date1Query = c => true;
            Expression<Func<trademark, bool>> date2Query = c => true;
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
            Expression<Func<trademark, bool>> nameQuery = c => true;
            if (!string.IsNullOrEmpty(request.name))
            {
                nameQuery = c => (c.name.Contains(request.name));
            }
            Expression<Func<trademark, bool>> applicantQuery = c => true;
            if (!string.IsNullOrEmpty(request.applicant))
            {
                applicantQuery = c => (c.applicant.Contains(request.applicant));
            }

            // 录入开始日期
            Expression<Func<trademark, bool>> date1Created = c => true;
            Expression<Func<trademark, bool>> date2Created = c => true;
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

            Expression<Func<trademark, bool>> codeQuery = c => true;
            if (!string.IsNullOrEmpty(request.code))
            {
                codeQuery = c => c.code.ToLower().Contains(request.code.ToLower());
            }

            var list = Uof.ItrademarkService.GetAll(condition)
                .Where(customerQuery)
                .Where(statusQuery)
                .Where(date1Query)
                .Where(date2Query)
                .Where(nameQuery)
                .Where(applicantQuery)
                .Where(date1Created)
                .Where(date2Created)
                .Where(codeQuery)
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
                    trademark_type = c.trademark_type,
                    region = c.region,
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
                    date_trial = c.date_trial,
                    date_created = c.date_created

                }).ToPagedList(request.index, request.size).ToList();

            var totalRecord = Uof.ItrademarkService.GetAll(condition)
                .Where(customerQuery)
                .Where(statusQuery)
                .Where(date1Query)
                .Where(date2Query)
                .Where(nameQuery)
                .Where(applicantQuery)
                .Where(date1Created)
                .Where(date2Created)
                .Where(codeQuery).Count();

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

        public ActionResult Add(trademark trade, oldRequest oldRequest)
        {
            //if (trade.customer_id == null)
            //{
            //    return Json(new { success = false, message = "请选择客户" }, JsonRequestBehavior.AllowGet);
            //}
            //if (string.IsNullOrEmpty(trade.name))
            //{
            //    return Json(new { success = false, message = "请填写商标名称" }, JsonRequestBehavior.AllowGet);
            //}
            //if (string.IsNullOrEmpty(trade.applicant))
            //{
            //    return Json(new { success = false, message = "请填写申请人" }, JsonRequestBehavior.AllowGet);
            //}
            //if (string.IsNullOrEmpty(trade.region))
            //{
            //    return Json(new { success = false, message = "请填写商标地区" }, JsonRequestBehavior.AllowGet);
            //}
            //if (string.IsNullOrEmpty(trade.address))
            //{
            //    return Json(new { success = false, message = "请填写申请人地址" }, JsonRequestBehavior.AllowGet);
            //}
            //if (trade.date_transaction == null)
            //{
            //    return Json(new { success = false, message = "请填写成交日期" }, JsonRequestBehavior.AllowGet);
            //}
            //if (trade.amount_transaction == null)
            //{
            //    return Json(new { success = false, message = "请填写成交金额" }, JsonRequestBehavior.AllowGet);
            //}
            //if (trade.salesman_id == null)
            //{
            //    return Json(new { success = false, message = "请选择业务员" }, JsonRequestBehavior.AllowGet);
            //}
            //if (trade.waiter_id == null)
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
                       
            trade.status = 0;
            trade.review_status = -1;
            trade.creator_id = userId;
            //trade.salesman_id = userId;
            trade.organization_id = GetOrgIdByUserId(userId); // organization_id;

           
            var nowYear = DateTime.Now.Year;
            if (oldRequest.is_old == 0)
            {
                // 新单据
                trade.code = GetNextOrderCode(trade.salesman_id.Value, "SB");

                // 需要马上年检的 步骤审核流程
                if (trade.is_annual == 1)
                {
                    trade.date_finish = DateTime.Now;
                    trade.status = 4;
                    trade.review_status = 1;
                }
            }
            else
            {
                // 旧单据
                trade.status = 4;
                trade.review_status = 1;

                if (oldRequest.is_already_annual == 1)
                {
                    trade.annual_year = nowYear;
                }
                else
                {
                    // 未年检
                    trade.annual_year = nowYear - 1;
                }
            }

            var newTrade = Uof.ItrademarkService.AddEntity(trade);
            if (newTrade == null)
            {
                return Json(new { success = false, message = "添加失败" }, JsonRequestBehavior.AllowGet);
            }

            Uof.ItimelineService.AddEntity(new timeline()
            {
                source_id = newTrade.id,
                source_name = "reg_abroad",
                title = "新建订单",
                content = string.Format("{0}新建了订单, 档案号{1}", arrs[3], newTrade.code)
            });

            return Json(new { id = newTrade.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get(int id)
        {
            var reg = Uof.ItrademarkService.GetAll(a => a.id == id).Select(a => new
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
                trademark_type = a.trademark_type,
                region = a.region,
                reg_mode = a.reg_mode,
                date_transaction = a.date_transaction,
                amount_transaction = a.amount_transaction,
                currency = a.currency,
                rate = a.rate,

                date_receipt = a.date_receipt,
                date_accept = a.date_accept,
                date_trial = a.date_trial,
                date_regit = a.date_regit,
                date_exten = a.date_exten,
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
                description = a.description

            }).FirstOrDefault();

            return Json(reg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetView(int id)
        {
            var dbTrademar = Uof.ItrademarkService.GetAll(a => a.id == id).Select(a => new
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
                trademark_type = a.trademark_type,
                region = a.region,
                reg_mode = a.reg_mode,
                date_transaction = a.date_transaction,
                amount_transaction = a.amount_transaction,
                currency = a.currency,
                rate = a.rate,

                date_receipt = a.date_receipt,
                date_accept = a.date_receipt,
                date_trial = a.date_trial,
                date_regit = a.date_regit,
                date_exten = a.date_exten,
                progress = a.progress,
                date_finish = a.date_finish,

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
                description = a.description

            }).FirstOrDefault();

            var list = Uof.IincomeService.GetAll(i => i.source_id == dbTrademar.id && i.source_name == "trademark").Select(i => new {
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

            var balance = dbTrademar.amount_transaction - total;
            var incomes = new
            {
                items = list,
                total = total,
                balance = balance,

                rate = dbTrademar.rate,
                local_amount = dbTrademar.amount_transaction * dbTrademar.rate,
                local_total = total * dbTrademar.rate,
                local_balance = balance * dbTrademar.rate
            };

            return Json(new { order = dbTrademar, incomes = incomes }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(trademark trade)
        {
            var dbTrade = Uof.ItrademarkService.GetById(trade.id);

            if (trade.customer_id == dbTrade.customer_id &&
                trade.type == dbTrade.type &&
                trade.name == dbTrade.name &&
                trade.applicant == dbTrade.applicant &&
                trade.address == dbTrade.address &&
                trade.trademark_type == dbTrade.trademark_type &&
                trade.region == dbTrade.region &&
                trade.reg_mode == dbTrade.reg_mode &&
                trade.date_transaction == dbTrade.date_transaction &&
                trade.amount_transaction == dbTrade.amount_transaction &&
                trade.currency == dbTrade.currency &&
                //trade.date_receipt == dbTrade.date_receipt &&
                //trade.date_accept == dbTrade.date_accept &&
                //trade.date_trial == dbTrade.date_trial &&
                //trade.date_regit == dbTrade.date_regit &&
                //trade.date_exten == dbTrade.date_exten &&
                //trade.progress == dbTrade.progress &&

                trade.salesman_id == dbTrade.salesman_id &&
                trade.waiter_id == dbTrade.waiter_id &&
                trade.manager_id == dbTrade.manager_id && 
                trade.description == dbTrade.description &&
                trade.currency == dbTrade.currency &&
                trade.rate == dbTrade.rate &&
                trade.assistant_id == dbTrade.assistant_id
                )
            {
                return Json(new { id = trade.id }, JsonRequestBehavior.AllowGet);
            }

            var identityName = HttpContext.User.Identity.Name;
            var arrs = identityName.Split('|');
            if (arrs.Length == 0)
            {
                return new HttpUnauthorizedResult();
            }

            var isChangeCurrency = trade.currency != dbTrade.currency || trade.rate != dbTrade.rate;

            dbTrade.customer_id = trade.customer_id;
            dbTrade.type = trade.type;
            dbTrade.name = trade.name;
            dbTrade.applicant = trade.applicant;
            dbTrade.address = trade.address;
            dbTrade.trademark_type = trade.trademark_type;
            dbTrade.region = trade.region;
            dbTrade.reg_mode = trade.reg_mode;
            dbTrade.date_transaction = trade.date_transaction;
            dbTrade.amount_transaction = trade.amount_transaction;
            dbTrade.currency = trade.currency;
            dbTrade.rate = trade.rate;
            dbTrade.assistant_id = trade.assistant_id;

            //dbTrade.date_receipt = trade.date_receipt;
            //dbTrade.date_accept = trade.date_accept;
            //dbTrade.date_trial = trade.date_trial;
            //dbTrade.date_regit = trade.date_regit;
            //dbTrade.date_exten = trade.date_exten;
            //dbTrade.progress = trade.progress;

            dbTrade.salesman_id = trade.salesman_id;
            dbTrade.waiter_id = trade.waiter_id;
            dbTrade.manager_id = trade.manager_id;
            dbTrade.description = trade.description;

            dbTrade.date_updated = DateTime.Now;

            var r = Uof.ItrademarkService.UpdateEntity(dbTrade);

            if (r)
            {
                if (isChangeCurrency)
                {
                    var list = Uof.IincomeService.GetAll(i => i.source_id == trade.id && i.source_name == "trademark").ToList();
                    if (list.Count() > 0)
                    {
                        foreach (var item in list)
                        {
                            item.currency = trade.currency;
                            item.rate = trade.rate;
                        }

                        Uof.IincomeService.UpdateEntities(list);
                    }
                }

                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbTrade.id,
                    source_name = "trademark",
                    title = "修改订单资料",
                    content = string.Format("{0}修改了订单资料", arrs[3])
                });
            }
            return Json(new { success = r, id = dbTrade.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Submit(int id)
        {
            var dbTrade = Uof.ItrademarkService.GetById(id);
            if (dbTrade == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }

            dbTrade.status = 1;
            dbTrade.review_status = -1;
            dbTrade.date_updated = DateTime.Now;

            var r = Uof.ItrademarkService.UpdateEntity(dbTrade);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbTrade.id,
                    source_name = "trademark",
                    title = "提交审核",
                    content = string.Format("提交给财务审核")
                });

                //var ids = GetFinanceMembers();
                var auditor_id = GetAuditorByKey("CW_ID");
                if (auditor_id != null)
                {
                    var waitdeals = new List<waitdeal>();
                    waitdeals.Add(new waitdeal
                    {
                        source = "trademark",
                        source_id = dbTrade.id,
                        user_id = auditor_id,
                        router = "trademark_view",
                        content = "您有商标订单需要财务审核",
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

            var dbTrade = Uof.ItrademarkService.GetById(id);
            if (dbTrade == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }
            var t = "";
            var waitdeals = new List<waitdeal>();
            if (dbTrade.status == 1)
            {
                dbTrade.status = 2;
                dbTrade.review_status = 1;
                dbTrade.finance_reviewer_id = userId;
                dbTrade.finance_review_date = DateTime.Now;
                dbTrade.finance_review_moment = "";

                t = "财务审核";
                waitdeals.Add(new waitdeal
                {
                    source = "trademark",
                    source_id = dbTrade.id,
                    user_id = dbTrade.salesman_id,
                    router = "trademark_view",
                    content = "您的商标订单已通过财务审核",
                    read_status = 0
                });

                if (dbTrade.assistant_id != null && dbTrade.assistant_id != dbTrade.salesman_id)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "trademark",
                        source_id = dbTrade.id,
                        user_id = dbTrade.assistant_id,
                        router = "trademark_view",
                        content = "您的商标订单已通过财务审核",
                        read_status = 0
                    });
                }

                var jwId = GetSubmitMemberByKey("SB_ID");
                if (jwId != null && jwId > 0)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "trademark",
                        source_id = dbTrade.id,
                        user_id = jwId,
                        router = "trademark_view",
                        content = "您有商标订单需要提交审核",
                        read_status = 0
                    });
                }

                //var ids = GetSubmitMembers();
                //if (ids.Count() > 0)
                //{
                //    foreach (var item in ids)
                //    {
                //        waitdeals.Add(new waitdeal
                //        {
                //            source = "trademark",
                //            source_id = dbTrade.id,
                //            user_id = item,
                //            router = "trademark_view",
                //            content = "您有商标订单需要提交审核",
                //            read_status = 0
                //        });
                //    }
                //}
            }
            else
            {
                dbTrade.status = 3;
                dbTrade.review_status = 1;
                dbTrade.submit_reviewer_id = userId;
                dbTrade.submit_review_date = DateTime.Now;
                dbTrade.submit_review_moment = "";

                t = "提交的审核";
                waitdeals.Add(new waitdeal
                {
                    source = "trademark",
                    source_id = dbTrade.id,
                    user_id = dbTrade.salesman_id,
                    router = "trademark_view",
                    content = "您的商标订单已通过提交审核",
                    read_status = 0
                });

                if (dbTrade.assistant_id != null && dbTrade.assistant_id != dbTrade.salesman_id)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "trademark",
                        source_id = dbTrade.id,
                        user_id = dbTrade.assistant_id,
                        router = "trademark_view",
                        content = "您的商标订单已通过提交审核",
                        read_status = 0
                    });
                }
            }

            dbTrade.date_updated = DateTime.Now;

            var r = Uof.ItrademarkService.UpdateEntity(dbTrade);

            if (r)
            {
                Uof.IwaitdealService.AddEntities(waitdeals);

                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbTrade.id,
                    source_name = "trademark",
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

            var dbTrade = Uof.ItrademarkService.GetById(id);
            if (dbTrade == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }
            var t = "";
            var waitdeals = new List<waitdeal>();
            if (dbTrade.status == 1)
            {
                dbTrade.status = 0;
                dbTrade.review_status = 0;
                dbTrade.finance_reviewer_id = userId;
                dbTrade.finance_review_date = DateTime.Now;
                dbTrade.finance_review_moment = description;

                t = "驳回了财务审核";
                waitdeals.Add(new waitdeal
                {
                    source = "trademark",
                    source_id = dbTrade.id,
                    user_id = dbTrade.salesman_id,
                    router = "trademark_view",
                    content = "您的商标订单未通过财务审核",
                    read_status = 0
                });

                if (dbTrade.assistant_id != null && dbTrade.assistant_id != dbTrade.salesman_id)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "trademark",
                        source_id = dbTrade.id,
                        user_id = dbTrade.assistant_id,
                        router = "trademark_view",
                        content = "您的商标订单未通过财务审核",
                        read_status = 0
                    });
                }
            }
            else
            {
                dbTrade.status = 0;
                dbTrade.review_status = 0;
                dbTrade.submit_reviewer_id = userId;
                dbTrade.submit_review_date = DateTime.Now;
                dbTrade.submit_review_moment = description;

                t = "驳回了提交的审核";
                waitdeals.Add(new waitdeal
                {
                    source = "trademark",
                    source_id = dbTrade.id,
                    user_id = dbTrade.salesman_id,
                    router = "trademark_view",
                    content = "您的商标订单未通过提交审核",
                    read_status = 0
                });

                if (dbTrade.assistant_id != null && dbTrade.assistant_id != dbTrade.salesman_id)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "trademark",
                        source_id = dbTrade.id,
                        user_id = dbTrade.assistant_id,
                        router = "trademark_view",
                        content = "您的商标订单未通过提交审核",
                        read_status = 0
                    });
                }
            }

            dbTrade.date_updated = DateTime.Now;

            var r = Uof.ItrademarkService.UpdateEntity(dbTrade);

            if (r)
            {
                Uof.IwaitdealService.AddEntities(waitdeals);

                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbTrade.id,
                    source_name = "trademark",
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

            var dbReg = Uof.ItrademarkService.GetById(id);
            if (dbReg == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }
            dbReg.status = 4;
            dbReg.date_updated = DateTime.Now;
            dbReg.date_finish = date_finish;
            dbReg.progress = "已完成";
            var r = Uof.ItrademarkService.UpdateEntity(dbReg);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbReg.id,
                    source_name = "trademark",
                    title = "完成订单",
                    content = string.Format("{0}完成了订单，完成日期为：{1}", arrs[3], date_finish.ToString("yyyy-MM-dd"))
                });

                // TODO 通知 业务员
            }

            return Json(new { success = r, message = r ? "" : "保存失败" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProgress(int id)
        {
            var p = Uof.ItrademarkService.GetAll(r => r.id == id).Select(r => new
            {
                id = r.id,
                customer_id = r.customer_id,
                progress = r.progress,
                is_done = r.status == 4 ? 1 : 0,

                date_accept = r.date_accept,
                date_receipt = r.date_receipt,
                date_trial = r.date_trial,
                date_regit = r.date_regit,
                date_exten = r.date_exten
            }).FirstOrDefault();

            return Json(p, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateProgress(TtrademarkProgressRequest request)
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

            var dbtrademark = Uof.ItrademarkService.GetById(request.id);
            if (dbtrademark == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }

            if (request.progress_type != "p")
            {
                dbtrademark.status = 4;
                dbtrademark.date_updated = DateTime.Now;
                dbtrademark.date_accept = request.date_accept;
                dbtrademark.date_receipt = request.date_receipt;
                dbtrademark.date_trial = request.date_trial;
                dbtrademark.date_regit = request.date_regit;
                dbtrademark.date_exten = request.date_exten;

                if (dbtrademark.date_finish == null)
                {
                    dbtrademark.date_finish = request.date_finish ?? DateTime.Today;
                }
            }
            else
            {
                if (dbtrademark.progress == request.progress && dbtrademark.date_finish == request.date_finish)
                {
                    return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);
                }
                dbtrademark.status = 4;
                dbtrademark.date_finish = request.date_finish;
                dbtrademark.progress = request.progress;
            }

            var r = Uof.ItrademarkService.UpdateEntity(dbtrademark);

            if (r)
            {
                if (request.progress_type != "p")
                {
                    Uof.ItimelineService.AddEntity(new timeline()
                    {
                        source_id = dbtrademark.id,
                        source_name = "trademark",
                        title = "完善了注册资料",
                        content = string.Format("{0}完善了注册资料", arrs[3])
                    });

                    var waitdeals = new List<waitdeal>();
                    waitdeals.Add(new waitdeal
                    {
                        source = "trademark",
                        source_id = dbtrademark.id,
                        user_id = dbtrademark.salesman_id,
                        router = "trademark_view",
                        content = "您的商标订单已完成",
                        read_status = 0
                    });
                    if (dbtrademark.assistant_id != null && dbtrademark.assistant_id != dbtrademark.salesman_id)
                    {
                        waitdeals.Add(new waitdeal
                        {
                            source = "trademark",
                            source_id = dbtrademark.id,
                            user_id = dbtrademark.assistant_id,
                            router = "trademark_view",
                            content = "您的商标订单已完成",
                            read_status = 0
                        });
                    }
                    Uof.IwaitdealService.AddEntities(waitdeals);
                }
                else
                {
                    Uof.ItimelineService.AddEntity(new timeline()
                    {
                        source_id = dbtrademark.id,
                        source_name = "trademark",
                        title = "更新了订单进度",
                        content = string.Format("{0}更新了进度: {1} 预计完成日期 {2}", arrs[3], dbtrademark.progress, dbtrademark.date_finish.Value.ToString("yyyy-MM-dd"))
                    });

                    var waitdeals = new List<waitdeal>();
                    waitdeals.Add(new waitdeal
                    {
                        source = "trademark",
                        source_id = dbtrademark.id,
                        user_id = dbtrademark.salesman_id,
                        router = "trademark_view",
                        content = "您的商标订单更新了进度",
                        read_status = 0
                    });
                    if (dbtrademark.assistant_id != null && dbtrademark.assistant_id != dbtrademark.salesman_id)
                    {
                        waitdeals.Add(new waitdeal
                        {
                            source = "trademark",
                            source_id = dbtrademark.id,
                            user_id = dbtrademark.salesman_id,
                            router = "trademark_view",
                            content = "您的商标订单更新了进度",
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