using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

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

            Expression<Func<reg_abroad, bool>> condition = c => true; //  c.salesman_id == userId;
            var ops = arrs[4].Split(',');
            if (ops.Count() == 0)
            {
                condition = c => (c.salesman_id == userId || c.assistant_id == userId || c.creator_id == userId);
            }
            else
            {
                var hasCompany = ops.Where(o => o == "1").FirstOrDefault();
                var hasDepart = ops.Where(o => o == "2").FirstOrDefault();
                if (hasCompany == null)
                {
                    if (hasDepart == null)
                    {
                        condition = c => (c.salesman_id == userId || c.assistant_id == userId || c.creator_id == userId);
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
            //Expression<Func<reg_abroad, bool>> customerQuery = c => true;
            //if (request.customer_id != null && request.customer_id.Value > 0)
            //{
            //    customerQuery = c => (c.customer_id == request.customer_id);
            //}
            // 订单状态
            Expression<Func<reg_abroad, bool>> statusQuery = c => true;
            if (request.status != null)
            {
                if (request.status == 2)
                {
                    statusQuery = c => (c.status == 2 || c.status == 3);
                }
                else if (request.status == 5)
                {
                    statusQuery = c => (c.order_status == 1);
                }
                else if (request.status == 6)
                {
                    statusQuery = c => (c.order_status == 2);
                }
                else if (request.status == 7)
                {
                    statusQuery = c => (c.order_status == 3);
                }
                else
                {
                    statusQuery = c => (c.status == request.status.Value);
                }
            }

            // 成交开始日期
            //Expression<Func<reg_abroad, bool>> date1Query = c => true;
            //Expression<Func<reg_abroad, bool>> date2Query = c => true;
            //if (request.start_time != null)
            //{
            //    date1Query = c => (c.date_transaction >= request.start_time.Value);
            //}
            //// 成交结束日期
            //if (request.end_time != null)
            //{
            //    var endTime = request.end_time.Value.AddDays(1);
            //    date2Query = c => (c.date_transaction < endTime);
            //}

            // 录入开始日期
            //Expression<Func<reg_abroad, bool>> date1Created = c => true;
            //Expression<Func<reg_abroad, bool>> date2Created = c => true;
            //if (request.start_create != null)
            //{
            //    date1Created = c => (c.date_created >= request.start_create.Value);
            //}
            //// 录入结束日期
            //if (request.end_create != null)
            //{
            //    var endTime = request.end_create.Value.AddDays(1);
            //    date2Created = c => (c.date_created < endTime);
            //}

            Expression<Func<reg_abroad, bool>> nameQuery = c => true;
            if (!string.IsNullOrEmpty(request.name))
            {
                nameQuery = c => (
                c.name_cn.ToLower().Contains(request.name.ToLower()) || 
                c.name_en.ToLower().Contains(request.name.ToLower()) || 
                c.code.ToLower().Contains(request.name.ToLower()));
            }

            //Expression<Func<reg_abroad, bool>> codeQuery = c => true;
            //if (!string.IsNullOrEmpty(request.code))
            //{
            //    codeQuery = c => c.code.ToLower().Contains(request.code.ToLower());
            //}

            Expression<Func<reg_abroad, bool>> areaQuery = c => true;
            if (!string.IsNullOrEmpty(request.area))
            {
                areaQuery = c => c.code.Contains(request.area);
            }

            var list = Uof.Ireg_abroadService
                .GetAll(condition)                
                .Where(statusQuery)                
                .Where(nameQuery)
                .Where(areaQuery)
                .OrderByDescending(item => item.date_created).Select(c => new
                {
                    id = c.id,
                    code = c.code,
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
                    salesman_name = c.member4.name,
                    date_created = c.date_created,
                    assistant_id = c.assistant_id,
                    assistant_name = c.member7.name,

                    finance_review_moment = c.finance_review_moment,
                    submit_review_moment = c.submit_review_moment,
                    date_setup = c.date_setup,
                    is_annual = c.is_annual ?? 0,
                    order_status = c.order_status ?? 0,
                    description = c.description,

                    annual_date = c.annual_date,
                    annual_id = c.annual_id,

                }).ToPagedList(request.index, request.size).ToList();

            var totalRecord = Uof.Ireg_abroadService
                .GetAll(condition)
                .Where(statusQuery)
                .Where(nameQuery)
                .Where(areaQuery)
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

        public ActionResult Add(reg_abroad aboad, oldRequest oldRequest, List<Shareholder> shareholderList)
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
                        
            aboad.status = 0;
            aboad.review_status = -1;
            aboad.creator_id = userId;
            aboad.organization_id = GetOrgIdByUserId(userId); // organization_id;

            if (aboad.customer_id != null)
            {
                var salesman_id = Uof.IcustomerService.GetAll(c => c.id == aboad.customer_id).Select(c => c.salesman_id).FirstOrDefault();
                if (salesman_id != null)
                {
                    aboad.salesman_id = salesman_id;
                }
            }

            var nowYear = DateTime.Now.Year;

            if (oldRequest.is_old == 0)
            {
                // 新单据
                //aboad.code = GetNextOrderCode(aboad.salesman_id.Value, "ZW");    
                aboad.code = GetNextOrderCode(aboad.creator_id.Value, "ZW");
            } else
            {
                // 旧单据
                aboad.status = 4;
                aboad.review_status = 1; 
                
                if (oldRequest.is_already_annual == 1)
                {
                    // 已年检
                    aboad.annual_year = nowYear;
                }
                else
                {
                    // 未年检
                    aboad.annual_year = nowYear - 1;
                }
            }
            // 需要马上年检的 不走审核流程
            if (aboad.is_annual == 1)
            {
                aboad.date_finish = DateTime.Now;
                aboad.status = 4;
                aboad.review_status = 1;
            } else
            {
                aboad.is_annual = 0;
            }

            if (aboad.is_open_bank == 0)
            {
                aboad.bank_id = null;
            }

            aboad.order_status = 0;

            var newAbroad = Uof.Ireg_abroadService.AddEntity(aboad);
            if (newAbroad == null)
            {
                return Json(new { success = false, message = "添加失败" }, JsonRequestBehavior.AllowGet);
            }

            if (shareholderList != null && shareholderList.Count() > 0)
            {
                var hoders = new List<abroad_shareholder>();

                foreach (var item in shareholderList)
                {
                    hoders.Add(new abroad_shareholder()
                    {
                        master_id = newAbroad.id,
                        history_id = null,
                        name = item.name,
                        cardNo = item.cardNo,
                        changed_type = "original",
                        source = "reg_abroad",
                        gender = item.gender,
                        takes = item.takes,
                        type = item.type,
                        memo = item.memo,
                        date_created = DateTime.Today,
                    });
                }

                Uof.Iabroad_shareholderService.AddEntities(hoders);
            }

            Uof.ItimelineService.AddEntity(new timeline()
            {
                source_id = newAbroad.id,
                source_name = "reg_abroad",
                title = "新建订单",
                is_system = 1,
                content = string.Format("{0}新建了订单, 档案号{1}", arrs[3], aboad.code)
            });

            return Json(new { id = newAbroad.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
           var abroad = Uof.Ireg_abroadService.GetById(id);

            var r =  Uof.Ireg_abroadService.DeleteEntity(abroad);
            if (r)
            {
               var incomes = Uof.IincomeService.GetAll(i => i.source_name == "reg_abroad" && i.source_id == id).ToList();
                if (incomes.Count > 0)
                {
                    foreach (var item in incomes)
                    {
                        Uof.IincomeService.DeleteEntity(item);
                    }
                }
            }

            return SuccessResult;
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
                rate = a.rate,
                progress = a.progress,

                invoice_name = a.invoice_name,
                invoice_tax = a.invoice_tax,
                invoice_address = a.invoice_address,
                invoice_tel = a.invoice_tel,
                invoice_bank = a.invoice_bank,
                invoice_account = a.invoice_account,

                //salesman_id = a.salesman_id,
                //salesman = a.member4.name,

                salesman_id = a.customer.salesman_id,
                salesman = a.customer.member1.name,

                waiter_id = a.waiter_id,
                waiter_name = a.member6.name,
                manager_id = a.manager_id,
                manager_name = a.member2.name,

                // TODO: 
                //assistant_id = a.assistant_id,
                //assistant_name = a.member7.name,
                assistant_id = a.creator_id,
                assistant_name = a.member.name,
                creator = a.member.name,

                status = a.status,
                review_status = a.review_status,
                is_annual = a.is_annual ?? 0,
                order_status = a.order_status ?? 0,
                description = a.description,
                shareholder = a.shareholder,
                need_annual = a.need_annual,

                trader_id = a.trader_id,
                trader_name = a.customer1.name,

                annual_owner = a.annual_owner,
                annual_owner_name = a.member8.name,

            }).FirstOrDefault();

            var shareholderList = Uof.Iabroad_shareholderService.GetAll(s => s.master_id == id && s.source == "reg_abroad" && s.type == "股东" && s.changed_type != "exit").ToList();
            var directorList = Uof.Iabroad_shareholderService.GetAll(s => s.master_id == id && s.source == "reg_abroad" && s.type == "董事" && s.changed_type != "exit").ToList();

            return Json(new { order = reg, shareholderList = shareholderList, directorList = directorList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetView(int id)
        {
            #region history 旧数据处理
            var historyReocrd = Uof.Iabroad_historyService
                .GetAll(h => h.master_id == id)
                .FirstOrDefault();
            if (historyReocrd == null)
            {
                historyReocrd = new abroad_history();
                historyReocrd.master_id = id;
                var dbHistoryRecord = Uof.IhistoryService.GetAll(h => h.source_id == id && h.source == "reg_abroad" && h.status >=3).OrderByDescending(h => h.id).FirstOrDefault();
                if (dbHistoryRecord != null)
                {
                    if (dbHistoryRecord.value != null && dbHistoryRecord.value != "{}" && dbHistoryRecord.value.Length > 0)
                    {
                        var dbReg = Uof.Ireg_abroadService.GetAll(a => a.id == id).FirstOrDefault();

                        JavaScriptSerializer jsonSerialize = new JavaScriptSerializer();
                        var obj = jsonSerialize.Deserialize<HistoryAbroad>(dbHistoryRecord.value);
                        if (obj.name_en != null || obj.name_cn != null || obj.address != null || obj.reg_no != null)
                        {
                            if (obj.name_cn != null && obj.name_cn.Length > 0)
                            {
                                historyReocrd.name_cn = dbReg.name_cn + "|" + dbHistoryRecord.date_created.Value.ToString("yyyy-MM-dd");
                                dbReg.name_cn = obj.name_cn;
                            }

                            if (obj.name_en != null && obj.name_en.Length > 0)
                            {
                                historyReocrd.name_en = dbReg.name_en + "|" + dbHistoryRecord.date_created.Value.ToString("yyyy-MM-dd");
                                dbReg.name_en = obj.name_en;
                            }

                            if (obj.reg_no != null && obj.reg_no.Length > 0)
                            {
                                historyReocrd.reg_no = dbReg.reg_no + "|" + dbHistoryRecord.date_created.Value.ToString("yyyy-MM-dd");
                                dbReg.reg_no = obj.reg_no;
                            }

                            if (obj.address != null && obj.address.Length > 0)
                            {
                                historyReocrd.address = dbReg.address + "|" + dbHistoryRecord.date_created.Value.ToString("yyyy-MM-dd");
                                dbReg.address = obj.address;
                            }

                            Uof.Ireg_abroadService.UpdateEntity(dbReg);
                            Uof.Iabroad_historyService.AddEntity(historyReocrd);
                        }
                    }
                }
            }

            #endregion

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
                rate = a.rate ?? 1,
                progress = a.progress,

                invoice_name = a.invoice_name,
                invoice_tax = a.invoice_tax,
                invoice_address = a.invoice_address,
                invoice_tel = a.invoice_tel,
                invoice_bank = a.invoice_bank,
                invoice_account = a.invoice_account,

                //salesman_id = a.salesman_id,
                //salesman = a.member4.name,
                salesman_id = a.customer.salesman_id,
                salesman = a.customer.member1.name,

                waiter_id = a.waiter_id,
                waiter_name = a.member6.name,
                manager_id = a.manager_id,
                manager_name = a.member2.name,

                // TODO:  修改成订单归属
                assistant_id = a.creator_id, 
                assistant_name = a.member.name,
                creator = a.member.name,

                status = a.status,
                review_status = a.review_status,
                finance_review_moment = a.finance_review_moment,
                submit_review_moment = a.submit_review_moment,
                is_annual = a.is_annual ?? 0,
                description = a.description,
                shareholder = a.shareholder,
                order_status = a.order_status ?? 0,
                need_annual = a.need_annual ?? 1,

                trader_id = a.trader_id,
                trader_name = a.customer1.name,

                annual_owner = a.annual_owner,
                annual_owner_name = a.member8.name,

            }).FirstOrDefault();

            var list = Uof.IincomeService.GetAll(i => i.source_id == reg.id && i.source_name == "reg_abroad").Select(i => new
            {
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

            var shareholderList = Uof.Iabroad_shareholderService.GetAll(s => s.master_id == id && s.source == "reg_abroad" && s.type == "股东" && s.changed_type != "exit").ToList();
            var directorList = Uof.Iabroad_shareholderService.GetAll(s => s.master_id == id && s.source == "reg_abroad" && s.type == "董事" &&
            s.changed_type != "exit").ToList();

            var _historyReocrd = new HistoryAbroad();
            if (historyReocrd != null)
            {
                _historyReocrd = new HistoryAbroad
                {
                    name_cn = historyReocrd.name_cn,
                    name_en = historyReocrd.name_en,
                    address = historyReocrd.address,
                    others = historyReocrd.others,
                    reg_no = historyReocrd.reg_no
                };
            }
            return Json(new
            {
                order = reg,
                incomes = incomes,
                shareholderList = shareholderList,
                directorList = directorList,
                historyReocrd = _historyReocrd
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(reg_abroad reg, List<Shareholder> shareholderList)
        {
            var dbReg = Uof.Ireg_abroadService.GetById(reg.id);

            //if (reg.customer_id == dbReg.customer_id && 
            //    reg.name_cn == dbReg.name_cn && 
            //    reg.name_en == dbReg.name_en &&
            //    reg.date_setup == dbReg.date_setup &&
            //    reg.reg_no == dbReg.reg_no &&
            //    reg.region == dbReg.region &&
            //    reg.address == dbReg.address &&
            //    reg.director == dbReg.director &&
            //    reg.is_open_bank == dbReg.is_open_bank &&
            //    reg.bank_id == dbReg.bank_id &&
            //    reg.date_transaction == dbReg.date_transaction &&
            //    reg.amount_transaction == dbReg.amount_transaction &&
            //    reg.invoice_name == dbReg.invoice_name &&
            //    reg.invoice_tax == dbReg.invoice_tax &&
            //    reg.invoice_address == dbReg.invoice_address &&
            //    reg.invoice_tel == dbReg.invoice_tel &&
            //    reg.invoice_bank == dbReg.invoice_bank &&
            //    reg.invoice_account == dbReg.invoice_account &&

            //    reg.salesman_id == dbReg.salesman_id &&
            //    reg.waiter_id == dbReg.waiter_id &&
            //    reg.manager_id == dbReg.manager_id && 
            //    reg.description == dbReg.description &&
            //    reg.currency == dbReg.currency && 
            //    reg.rate == dbReg.rate &&
            //    reg.assistant_id == dbReg.assistant_id && 
            //    reg.is_annual == dbReg.is_annual &&
            //    reg.shareholder == dbReg.shareholder
            //    )
            //{
            //    return Json(new { id = reg.id }, JsonRequestBehavior.AllowGet);
            //}

            var identityName = HttpContext.User.Identity.Name;
            var arrs = identityName.Split('|');
            if (arrs.Length == 0)
            {
                return new HttpUnauthorizedResult();
            }

            var isChangeCurrency = reg.currency != dbReg.currency || reg.rate != dbReg.rate;

            if (reg.customer_id != null)
            {
                var salesman_id = Uof.IcustomerService.GetAll(c => c.id == reg.customer_id).Select(c => c.salesman_id).FirstOrDefault();
                if (salesman_id != null)
                {
                    reg.salesman_id = salesman_id;
                }
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

            dbReg.salesman_id = reg.salesman_id;
            dbReg.waiter_id = reg.waiter_id;
            dbReg.manager_id = reg.manager_id;
            dbReg.description = reg.description;
            dbReg.currency = reg.currency;
            dbReg.rate = reg.rate;
            //dbReg.assistant_id = reg.assistant_id;
            dbReg.shareholder = reg.shareholder;

            dbReg.trader_id = reg.trader_id;

            dbReg.need_annual = reg.need_annual ?? 1;

            dbReg.annual_owner = reg.annual_owner;

            if (reg.is_open_bank == 0)
            {
                dbReg.bank_id = null;
            }

            // 需要马上年检的 不走审核流程
            if (reg.is_annual == 1 && dbReg.is_annual != 1)
            {
                dbReg.date_finish = DateTime.Now;
                dbReg.status = 4;
                dbReg.review_status = 1;
                dbReg.is_annual = 1;
            }
            if (reg.is_annual == 0 && dbReg.is_annual == 1)
            {
                dbReg.date_finish = null;
                dbReg.status = 0;
                dbReg.review_status = -1;
                dbReg.is_annual = 0;
            }

            dbReg.date_updated = DateTime.Now;

            var r = Uof.Ireg_abroadService.UpdateEntity(dbReg);

            if (r)
            {
                //if (isChangeCurrency)
                //{
                //    var list = Uof.IincomeService.GetAll(i => i.source_id == reg.id && i.source_name == "reg_abroad").ToList();
                //    if (list.Count() > 0)
                //    {
                //        foreach (var item in list)
                //        {
                //            item.currency = reg.currency;
                //            item.rate = reg.rate;
                //        }

                //        Uof.IincomeService.UpdateEntities(list);
                //    }
                //}

                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbReg.id,
                    source_name = "reg_abroad",
                    title = "修改订单资料",
                    is_system = 1,
                    content = string.Format("{0}修改了订单资料", arrs[3])
                });

                #region 股东 董事
                var dbHolders = Uof.Iabroad_shareholderService.GetAll(s => s.master_id == dbReg.id && s.source == "reg_abroad" && s.changed_type != "exit").ToList();

                var newHolders = new List<abroad_shareholder>();
                var deleteHolders = new List<abroad_shareholder>();
                var updateHolders = new List<abroad_shareholder>();

                if (shareholderList != null && shareholderList.Count() > 0)
                {
                    foreach (var item in shareholderList)
                    {
                        if (item.id == null)
                        {
                            newHolders.Add(new abroad_shareholder
                            {
                                master_id = dbReg.id,
                                history_id = null,
                                name = item.name,
                                cardNo = item.cardNo,
                                changed_type = "original",
                                source = "reg_abroad",
                                gender = item.gender,
                                takes = item.takes,
                                type = item.type,
                                memo = item.memo,
                                date_created = DateTime.Today,
                            });
                        }

                        if (item.id > 0)
                        {
                           var updateHolder = dbHolders.Where(d => d.id == item.id).FirstOrDefault();
                            if (updateHolder != null)
                            {
                                updateHolder.name = item.name;
                                updateHolder.cardNo = item.cardNo;
                                updateHolder.gender = item.gender;
                                updateHolder.takes = item.takes;

                                updateHolders.Add(updateHolder);
                            }
                        }
                    }

                    if (dbHolders.Count() > 0)
                    {
                        foreach (var item in dbHolders)
                        {
                            var shareholder = shareholderList.Where(s => s.id == item.id).FirstOrDefault();
                            if (shareholder == null)
                            {
                                deleteHolders.Add(item);
                            }
                        }
                    }
                }
                else
                {
                    deleteHolders = dbHolders;
                }

                try
                {
                    if (deleteHolders.Count > 0)
                    {
                        foreach (var item in deleteHolders)
                        {
                            Uof.Iabroad_shareholderService.DeleteEntity(item);
                        }                        
                    }

                    if (updateHolders.Count > 0)
                    {
                        Uof.Iabroad_shareholderService.UpdateEntities(updateHolders);
                    }

                    if (newHolders.Count > 0)
                    {
                        Uof.Iabroad_shareholderService.AddEntities(newHolders);
                    }
                }
                catch (Exception)
                {

                }

                #endregion
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
            dbReg.review_status = -1;
            dbReg.date_updated = DateTime.Now;

            var r = Uof.Ireg_abroadService.UpdateEntity(dbReg);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbReg.id,
                    source_name = "reg_abroad",
                    title = "提交审核",
                    is_system = 1,
                    content = string.Format("提交给财务审核")
                });

                //var ids = GetFinanceMembers();
                var auditor_id = GetAuditorByKey("CW_ID");
                if (auditor_id!= null)
                {
                    var waitdeals = new List<waitdeal>();
                    waitdeals.Add(new waitdeal
                    {
                        source = "reg_abroad",
                        source_id = dbReg.id,
                        user_id = auditor_id,
                        router = "abroad_view",
                        content = "您有境外注册订单需要财务审核",
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

            var dbReg = Uof.Ireg_abroadService.GetById(id);
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
                    source = "reg_abroad",
                    source_id = dbReg.id,
                    user_id = dbReg.salesman_id,
                    router = "abroad_view",
                    content = "您的境外注册订单已通过财务审核",
                    read_status = 0
                });

                if (dbReg.assistant_id != null && dbReg.assistant_id != dbReg.salesman_id)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "reg_abroad",
                        source_id = dbReg.id,
                        user_id = dbReg.assistant_id,
                        router = "abroad_view",
                        content = "您的境外注册订单已通过财务审核",
                        read_status = 0
                    });
                }

                //var ids = GetSubmitMembers();
                var jwId = GetSubmitMemberByKey("JW_ID");
                if (jwId != null && jwId > 0)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "reg_abroad",
                        source_id = dbReg.id,
                        user_id = jwId,
                        router = "abroad_view",
                        content = "您有境外注册订单需要提交审核",
                        read_status = 0
                    });
                }
            }
            else
            {
                dbReg.status = 3;
                dbReg.review_status = 1;
                dbReg.submit_reviewer_id = userId;
                dbReg.submit_review_date = DateTime.Now;
                dbReg.submit_review_moment = "";

                t = "提交的审核";
                waitdeals.Add(new waitdeal
                {
                    source = "reg_abroad",
                    source_id = dbReg.id,
                    user_id = dbReg.salesman_id,
                    router = "abroad_view",
                    content = "您的境外注册订单已通过提交审核",
                    read_status = 0
                });
                if (dbReg.assistant_id != null && dbReg.assistant_id != dbReg.salesman_id)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "reg_abroad",
                        source_id = dbReg.id,
                        user_id = dbReg.assistant_id,
                        router = "abroad_view",
                        content = "您的境外注册订单已通过提交审核",
                        read_status = 0
                    });
                }
            }

            dbReg.date_updated = DateTime.Now;

            var r = Uof.Ireg_abroadService.UpdateEntity(dbReg);

            if (r)
            {
                Uof.IwaitdealService.AddEntities(waitdeals);

                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbReg.id,
                    source_name = "reg_abroad",
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

            var dbReg = Uof.Ireg_abroadService.GetById(id);
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
                    source = "reg_abroad",
                    source_id = dbReg.id,
                    user_id = dbReg.salesman_id,
                    router = "abroad_view",
                    content = "您的境外注册订单未通过财务审核",
                    read_status = 0
                });
                if (dbReg.assistant_id != null && dbReg.assistant_id != dbReg.salesman_id)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "reg_abroad",
                        source_id = dbReg.id,
                        user_id = dbReg.assistant_id,
                        router = "abroad_view",
                        content = "您的境外注册订单未通过财务审核",
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
                    source = "reg_abroad",
                    source_id = dbReg.id,
                    user_id = dbReg.salesman_id,
                    router = "abroad_view",
                    content = "您的境外注册订单未通过提交审核",
                    read_status = 0
                });
                if (dbReg.assistant_id != null && dbReg.assistant_id != dbReg.salesman_id)
                {
                    waitdeals.Add(new waitdeal
                    {
                        source = "reg_abroad",
                        source_id = dbReg.id,
                        user_id = dbReg.assistant_id,
                        router = "abroad_view",
                        content = "您的境外注册订单未通过提交审核",
                        read_status = 0
                    });
                }
            }

            dbReg.date_updated = DateTime.Now;

            var r = Uof.Ireg_abroadService.UpdateEntity(dbReg);

            if (r)
            {
                Uof.IwaitdealService.AddEntities(waitdeals);

                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbReg.id,
                    source_name = "reg_abroad",
                    title = "驳回审核",
                    is_system = 1,
                    content = string.Format("{0}{1}, 驳回理由: {2}", arrs[3], t, description)
                });
            }

            return Json(new { success = r, message = r ? "" : "审核失败" }, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public ActionResult Finish(int id, DateTime date_finish)
        //{
        //    var u = HttpContext.User.Identity.IsAuthenticated;
        //    if (!u)
        //    {
        //        return new HttpUnauthorizedResult();
        //    }

        //    var identityName = HttpContext.User.Identity.Name;
        //    var arrs = identityName.Split('|');
        //    if (arrs.Length == 0)
        //    {
        //        return new HttpUnauthorizedResult();
        //    }

        //    var userId = 0;
        //    int.TryParse(arrs[0], out userId);

        //    var dbReg = Uof.Ireg_abroadService.GetById(id);
        //    if (dbReg == null)
        //    {
        //        return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
        //    }
        //    dbReg.status = 4;
        //    dbReg.date_updated = DateTime.Now;
        //    dbReg.date_finish = date_finish;
        //    dbReg.progress = "已完成";
        //    var r = Uof.Ireg_abroadService.UpdateEntity(dbReg);

        //    if (r)
        //    {
        //        var h = new reg_history()
        //        {
        //            reg_id = dbReg.id,
        //            name_cn = dbReg.name_cn,
        //            name_en = dbReg.name_en,
        //            address = dbReg.address,
        //            date_setup = dbReg.date_setup,
        //            director = dbReg.director,
        //            region = dbReg.region,
        //            reg_no = dbReg.reg_no
        //        };

        //        Uof.Ireg_historyService.AddEntity(h);

        //        Uof.ItimelineService.AddEntity(new timeline()
        //        {
        //            source_id = dbReg.id,
        //            source_name = "reg_abroad",
        //            title = "完成订单",
        //            content = string.Format("{0}完成了订单，完成日期为：{1}", arrs[3], date_finish.ToString("yyyy-MM-dd"))
        //        });
        //    }

        //    return Json(new { success = r, message = r ? "" : "保存失败" }, JsonRequestBehavior.AllowGet);
        //}

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
                history.others == dbReg.description &&
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
            dbReg.description = history.others ?? dbReg.description;

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
                customer_id = r.customer_id,
                is_done = r.status == 4 ? 1 : 0,
                date_finish = r.date_finish,
                progress = r.progress,

                address = r.address,
                date_setup = r.date_setup,
                reg_no = r.reg_no,
                is_open_bank = r.is_open_bank,
                bank_id = r.bank_id,
                bank_name = r.bank_account.bank

            }).FirstOrDefault();

            return Json(p, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateProgress(RegAbroadProgressRequest request)
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

            var dbAbroad = Uof.Ireg_abroadService.GetById(request.id);
            if (dbAbroad == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }

            if (request.progress_type != "p")
            {
                dbAbroad.status = 4;
                dbAbroad.date_updated = DateTime.Now;
                dbAbroad.date_setup = request.date_setup;
                dbAbroad.reg_no = request.reg_no;
                dbAbroad.address = request.address;
                dbAbroad.is_open_bank = request.is_open_bank;

                if (dbAbroad.date_finish == null)
                {
                    dbAbroad.date_finish = request.date_finish ?? DateTime.Today;
                }
                
                if (request.is_open_bank == 1)
                {
                    dbAbroad.bank_id = request.bank_id;
                }
            } else
            {
                if (dbAbroad.progress == request.progress && dbAbroad.date_finish == request.date_finish)
                {
                    return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);
                }
                dbAbroad.status = 4;
                dbAbroad.date_updated = DateTime.Now;
                dbAbroad.date_finish = request.date_finish;
                dbAbroad.progress = request.progress;
            }
            
            var r = Uof.Ireg_abroadService.UpdateEntity(dbAbroad);

            if (r)
            {
                if (request.progress_type != "p")
                {
                    Uof.ItimelineService.AddEntity(new timeline()
                    {
                        source_id = dbAbroad.id,
                        source_name = "reg_abroad",
                        title = "完善了注册资料",
                        is_system = 1,
                        content = string.Format("{0}完善了注册资料", arrs[3])
                    });

                    var waitdeals = new List<waitdeal>();
                    waitdeals.Add(new waitdeal
                    {
                        source = "reg_abroad",
                        source_id = dbAbroad.id,
                        user_id = dbAbroad.salesman_id,
                        router = "abroad_view",
                        content = "您的境外注册订单已完成",
                        read_status = 0
                    });
                    if (dbAbroad.assistant_id != null && dbAbroad.assistant_id != dbAbroad.salesman_id)
                    {
                        waitdeals.Add(new waitdeal
                        {
                            source = "reg_abroad",
                            source_id = dbAbroad.id,
                            user_id = dbAbroad.assistant_id,
                            router = "abroad_view",
                            content = "您的境外注册订单已完成",
                            read_status = 0
                        });
                    }
                    Uof.IwaitdealService.AddEntities(waitdeals);
                }
                else
                {
                    Uof.ItimelineService.AddEntity(new timeline()
                    {
                        source_id = dbAbroad.id,
                        source_name = "reg_abroad",
                        title = "更新了订单进度",
                        is_system = 1,
                        content = string.Format("{0}更新了进度: {1} 预计完成日期 {2}", arrs[3], dbAbroad.progress, dbAbroad.date_finish.Value.ToString("yyyy-MM-dd"))
                    });

                   var waitdeals = new List<waitdeal>();
                    waitdeals.Add(new waitdeal
                    {
                        source = "reg_abroad",
                        source_id = dbAbroad.id,
                        user_id = dbAbroad.salesman_id,
                        router = "abroad_view",
                        content = "您的境外注册订单更新了进度",
                        read_status = 0
                    });
                    if (dbAbroad.assistant_id != null && dbAbroad.assistant_id != dbAbroad.salesman_id)
                    {
                        waitdeals.Add(new waitdeal
                        {
                            source = "reg_abroad",
                            source_id = dbAbroad.id,
                            user_id = dbAbroad.assistant_id,
                            router = "abroad_view",
                            content = "您的境外注册订单更新了进度",
                            read_status = 0
                        });
                    }
                    Uof.IwaitdealService.AddEntities(waitdeals);
                }
            }

            return Json(new { success = r, message = r ? "" : "更新失败" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateHistory(reg_history history)
        {
            if (history.reg_id == null)
            {
                return Json(new { success = false, message = "参数reg_id不可为空" }, JsonRequestBehavior.AllowGet);
            }

            var dbReg = Uof.Ireg_abroadService.GetAll(a => a.id == history.reg_id).FirstOrDefault();
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
                history.others == dbReg.description &&
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
            dbReg.description = history.others ?? dbReg.description;

            dbReg.date_updated = DateTime.Now;

            var r = Uof.Ireg_abroadService.UpdateEntity(dbReg);

            if (r)
            {
                var dbHistory = Uof.Ireg_historyService.GetById(history.id);
                dbHistory.address = history.address;
                dbHistory.date_setup = history.date_setup;
                dbHistory.director = history.director;
                dbHistory.name_cn = history.name_cn;
                dbHistory.name_en = history.name_en;
                dbHistory.region = history.region;
                dbHistory.others = history.others;
                dbHistory.reg_no = history.reg_no;


                Uof.Ireg_historyService.UpdateEntity(dbHistory);

                return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, message = "更新失败" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult HistoryHolder(int master_id, string source, string type)
        {
            var list = Uof.Ihistory_shareholderService.GetAll(s => s.master_id == master_id && s.source == source && s.type == type).OrderByDescending(s=>s.id).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateCreator(int id, int creator_id, string creator)
        {
            var auth = HttpContext.User.Identity.IsAuthenticated;
            if (!auth)
            {
                return new HttpUnauthorizedResult();
            }

            var identityName = HttpContext.User.Identity.Name;
            var arrs = identityName.Split('|');
            if (arrs.Length == 0)
            {
                return new HttpUnauthorizedResult();
            }

            var reg = Uof.Ireg_abroadService.GetAll(r => r.id == id).FirstOrDefault();
            if (reg != null)
            {
                reg.creator_id = creator_id;
                Uof.Ireg_abroadService.UpdateEntity(reg);

                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = reg.id,
                    source_name = "reg_abroad",
                    title = "修改订单归属人",
                    is_system = 1,
                    content = string.Format("{0}把订单归属人修改为{1}", arrs[3], creator)
                });
            }

            return SuccessResult;
        }
    }
}