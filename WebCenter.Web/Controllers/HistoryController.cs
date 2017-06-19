﻿using System;
using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using Common;
using System.Web.Security;
using System.Collections.Generic;
using WebCenter.Entities;
using System.IO;
using System.Drawing;
using System.Web;
using System.Linq.Expressions;
using System.Web.Script.Serialization;

namespace WebCenter.Web.Controllers
{
    public class HistoryController : BaseController
    {
        public HistoryController(IUnitOfWork UOF)
            : base(UOF)
        {

        }
        
        public ActionResult Add(history _history, oldRequest oldRequest, List<Shareholder> shareholderList)
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

            var customer_id = 0;
            switch (_history.source)
            {
                case "reg_abroad":
                    customer_id = Uof.Ireg_abroadService.GetAll(a => a.id == _history.source_id).Select(a => a.customer_id.Value).FirstOrDefault();
                     break;
                case "reg_internal":
                    customer_id = Uof.Ireg_internalService.GetAll(a => a.id == _history.source_id).Select(a => a.customer_id.Value).FirstOrDefault();
                    break;
                case "patent":
                    customer_id = Uof.IpatentService.GetAll(a => a.id == _history.source_id).Select(a => a.customer_id.Value).FirstOrDefault();
                    break;
                case "trademark":
                    customer_id = Uof.ItrademarkService.GetAll(a => a.id == _history.source_id).Select(a => a.customer_id.Value).FirstOrDefault();
                    break;
                default:
                    break;
            }

            _history.customer_id = customer_id;

            var userId = 0;
            var organization_id = 0;
            int.TryParse(arrs[0], out userId);
            int.TryParse(arrs[2], out organization_id);

            _history.status = 0;
            _history.review_status = -1;
            _history.creator_id = userId;

            var nowYear = DateTime.Now.Year;
            if (oldRequest.is_old == 0)
            {
            }
            else
            {
                _history.status = 4;
                _history.review_status = 1;
            }

            var newHistory = Uof.IhistoryService.AddEntity(_history);
            if (newHistory == null)
            {
                return Json(new { success = false, message = "添加失败" }, JsonRequestBehavior.AllowGet);
            }

            if (shareholderList != null && shareholderList.Count() > 0)
            {
                switch (_history.source)
                {
                    case "reg_abroad":
                        break;
                    case "reg_internal":
                        break;
                }

                var historyHoders = new List<history_shareholder>();
                //var newHolders = new List<abroad_shareholder>();
                //var updateHolders = new List<abroad_shareholder>();

                foreach (var item in shareholderList)
                {
                    historyHoders.Add(new history_shareholder()
                    {
                        master_id = _history.source_id.Value,
                        history_id = newHistory.id,
                        name = item.name,
                        cardNo = item.cardNo,
                        changed_type = item.changed_type,
                        source = _history.source,
                        gender = item.gender,
                        takes = item.takes,
                        type = item.type,
                        memo = item.memo,
                        date_created = DateTime.Today,
                        person_id = item.person_id,
                        date_changed = DateTime.Today,
                    });
                }

                Uof.Ihistory_shareholderService.AddEntities(historyHoders);                
            }

            return Json(new { id = newHistory.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(history _history, List<Shareholder> shareholderList)
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


            var dbHistory = Uof.IhistoryService.GetAll(h => h.id == _history.id).FirstOrDefault();
            if (dbHistory == null)
            {
                return Json(new { success = false, message = "找不到该条数据" }, JsonRequestBehavior.AllowGet);
            }

            dbHistory.value = _history.value;
            dbHistory.salesman_id = _history.salesman_id;
            dbHistory.date_transaction = _history.date_transaction;
            dbHistory.amount_transaction = _history.amount_transaction;
            dbHistory.currency = _history.currency;
            dbHistory.rate = _history.rate;

            var result = Uof.IhistoryService.UpdateEntity(dbHistory);
            if (!result)
            {
                return Json(new { success = false, message = "添加失败" }, JsonRequestBehavior.AllowGet);
            }

            if (shareholderList != null && shareholderList.Count() > 0)
            {
                switch (_history.source)
                {
                    case "reg_abroad":
                        break;
                    case "reg_internal":
                        break;
                }

                var historyHoders = new List<history_shareholder>();
                var updateHolders = new List<history_shareholder>();
                var deleteHolders = new List<history_shareholder>();
                //var newHolders = new List<abroad_shareholder>();
                //var updateHolders = new List<abroad_shareholder>();

                var dbHistoryHolders = Uof.Ihistory_shareholderService.GetAll(h => h.history_id == dbHistory.id).ToList();

                foreach (var item in shareholderList)
                {
                    var exist = dbHistoryHolders.Where(h => h.id == item.id).FirstOrDefault();
                    if (exist == null)
                    {
                        historyHoders.Add(new history_shareholder()
                        {
                            master_id = dbHistory.source_id.Value,
                            history_id = dbHistory.id,
                            name = item.name,
                            cardNo = item.cardNo,
                            changed_type = item.changed_type,
                            source = _history.source,
                            gender = item.gender,
                            takes = item.takes,
                            type = item.type,
                            memo = item.memo,
                            date_created = DateTime.Today,
                            date_changed = DateTime.Today,
                        });
                        #region
                        //if (item.changed_type == "new" || item.person_id == -1)
                        //{
                        //    newHolders.Add(new abroad_shareholder()
                        //    {
                        //        master_id = _history.source_id.Value,
                        //        history_id = dbHistory.id,
                        //        name = item.name,
                        //        cardNo = item.cardNo,
                        //        changed_type = item.changed_type,
                        //        source = _history.source,
                        //        gender = item.gender,
                        //        takes = item.takes,
                        //        type = item.type,
                        //        memo = item.memo,
                        //        date_created = DateTime.Today,
                        //    });
                        //}
                        //if ((item.changed_type == "exit" || item.changed_type == "takes") && item.person_id > 0)
                        //{
                        //    updateHolders.Add(new abroad_shareholder()
                        //    {
                        //        id = item.person_id.Value,
                        //        master_id = _history.source_id.Value,
                        //        history_id = dbHistory.id,
                        //        name = item.name,
                        //        cardNo = item.cardNo,
                        //        changed_type = item.changed_type,
                        //        source = _history.source,
                        //        gender = item.gender,
                        //        takes = item.takes,
                        //        type = item.type,
                        //        memo = item.memo,
                        //        date_created = DateTime.Today,
                        //    });
                        //}
                        #endregion
                    }
                    else
                    {
                        exist.name = item.name;
                        exist.cardNo = item.cardNo;
                        exist.changed_type = item.changed_type;
                        exist.gender = item.gender;
                        exist.takes = item.takes;
                        exist.memo = item.memo;
                        exist.date_changed = DateTime.Today;
                        updateHolders.Add(exist);
                    }
                }

                if (dbHistoryHolders != null && dbHistoryHolders.Count() > 0)
                {
                    if (shareholderList == null)
                    {
                        deleteHolders = dbHistoryHolders;
                    }
                    if (shareholderList.Count() == 0)
                    {
                        deleteHolders = dbHistoryHolders;
                    }

                    if (shareholderList != null && shareholderList.Count() > 0)
                    {
                        foreach (var item in dbHistoryHolders)
                        {
                           var existHolder = shareholderList.Where(s => s.id == item.id).FirstOrDefault();
                            if (existHolder == null)
                            {
                                deleteHolders.Add(item);
                            }
                        }
                    }
                }

                if (historyHoders != null && historyHoders.Count() > 0)
                {
                    Uof.Ihistory_shareholderService.AddEntities(historyHoders);
                }

                if (updateHolders != null && updateHolders.Count() > 0)
                {
                    Uof.Ihistory_shareholderService.UpdateEntities(updateHolders);
                }

                if (deleteHolders != null && deleteHolders.Count() > 0)
                {
                    foreach (var item in deleteHolders)
                    {
                        Uof.Ihistory_shareholderService.DeleteEntity(item);
                    }                    
                }

                #region
                //if (newHolders != null && newHolders.Count() > 0)
                //{
                //    Uof.Iabroad_shareholderService.AddEntities(newHolders);
                //}

                //if (updateHolders != null && updateHolders.Count() > 0)
                //{
                //    var ids = updateHolders.Select(u => u.id).ToList();
                //    var dbHolders = Uof.Iabroad_shareholderService.GetAll(s => ids.Contains(s.id)).ToList();

                //    if (dbHolders != null && dbHolders.Count() > 0)
                //    {
                //        foreach (var item in dbHolders)
                //        {
                //            var updateHolder = updateHolders.Where(u => u.id == item.id).FirstOrDefault();
                //            if (updateHolder != null)
                //            {
                //                item.changed_type = updateHolder.changed_type;
                //                item.takes = updateHolder.takes;
                //                item.date_updated = DateTime.Today;
                //            }
                //        }

                //        Uof.Iabroad_shareholderService.UpdateEntities(dbHolders);
                //    }
                //}
                #endregion

            }

            return Json(new { success = result, id = dbHistory.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult List(int source_id, string source, int index, int size)
        {
            var list = Uof.IhistoryService.GetAll(c=>c.source == source && c.source_id == source_id)
                .OrderByDescending(item => item.id).Select(c => new
                {
                    id = c.id,
                    source = c.source,
                    source_id = c.source_id,
                    order_code = c.order_code,
                    customer_id = c.customer_id,
                    value = c.value,

                    status = c.status,
                    review_status = c.review_status,
                    date_transaction = c.date_transaction,
                    amount_transaction = c.amount_transaction,
                    amount_income = 0,
                    amount_unreceive = 0,
                    progress = c.progress,
                    salesman_id = c.salesman_id,
                    salesman_name = c.member2.name,
                    finance_review_moment = c.finance_review_moment,
                    submit_review_moment = c.submit_review_moment

                }).ToPagedList(index, size).ToList();

            var totalRecord = Uof.IhistoryService.GetAll(c => c.source == source && c.source_id == source_id).Count();

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

        public ActionResult GetView(int id)
        {
            var reg = Uof.IhistoryService.GetAll(c => c.id == id).Select(c => new
            {
                id = c.id,
                source = c.source,
                source_id = c.source_id,
                customer_id = c.customer_id,
                order_code = c.order_code,
                value = c.value,
                status = c.status,
                review_status = c.review_status,
                date_transaction = c.date_transaction,
                rate = c.rate,
                currency = c.currency,
                amount_transaction = c.amount_transaction,
                amount_income = 0,
                amount_unreceive = 0,
                progress = c.progress,
                salesman_id = c.salesman_id,
                salesman = c.member2.name,
                finance_review_moment = c.finance_review_moment,
                submit_review_moment = c.submit_review_moment

            }).FirstOrDefault();

            var list = Uof.IincomeService.GetAll(i => i.source_id == reg.id && i.source_name == "history").Select(i => new {
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

            var shareholderList = Uof.Ihistory_shareholderService.GetAll(s => s.history_id == reg.id && s.type == "股东").ToList();
            var directoryList = Uof.Ihistory_shareholderService.GetAll(s => s.history_id == reg.id && s.type == "董事").ToList();

            return Json(new { order = reg, incomes = incomes, shareholderList = shareholderList, directoryList = directoryList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get(int id)
        {
            var reg = Uof.IhistoryService.GetAll(c => c.id == id).Select(c => new
            {
                id = c.id,
                source = c.source,
                source_id = c.source_id,
                customer_id = c.customer_id,
                order_code = c.order_code,
                value = c.value,
                status = c.status,
                review_status = c.review_status,
                date_transaction = c.date_transaction,
                rate = c.rate,
                currency = c.currency,
                amount_transaction = c.amount_transaction,
                amount_income = 0,
                amount_unreceive = 0,
                progress = c.progress,
                salesman_id = c.salesman_id,
                salesman = c.member2.name,
                finance_review_moment = c.finance_review_moment,
                submit_review_moment = c.submit_review_moment

            }).FirstOrDefault();

            var shareholderList =  Uof.Ihistory_shareholderService.GetAll(s => s.history_id == reg.id && s.type == "股东").ToList();
            var directoryList = Uof.Ihistory_shareholderService.GetAll(s => s.history_id == reg.id && s.type == "董事").ToList();

            return Json(new { order = reg, shareholderList  = shareholderList, directoryList = directoryList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Submit(int id)
        {
            var dbReg = Uof.IhistoryService.GetById(id);
            if (dbReg == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }

            dbReg.status = 1;
            dbReg.review_status = -1;
            dbReg.date_updated = DateTime.Now;

            var r = Uof.IhistoryService.UpdateEntity(dbReg);

            if (r)
            {
                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbReg.id,
                    source_name = "history",
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
                        source = "history",
                        source_id = dbReg.id,
                        user_id = auditor_id,
                        router = "history_view",
                        content = "您有数据变更订单需要财务审核",
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

            var dbAudit = Uof.IhistoryService.GetById(id);
            if (dbAudit == null)
            {
                return Json(new { success = false, message = "找不到该订单" }, JsonRequestBehavior.AllowGet);
            }

            var t = "";
            var waitdeals = new List<waitdeal>();
            var holders = new List<history_shareholder>();

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
                    source = "history",
                    source_id = dbAudit.id,
                    user_id = dbAudit.salesman_id,
                    router = "history_view",
                    content = "您的变更订单已通过财务审核",
                    read_status = 0
                });

                //var ids = GetSubmitMembers();
                var jwId = GetSubmitForHistory(dbAudit);
                if (jwId != null && jwId > 0)
                {
                    waitdeals.Add(new waitdeal
                    {
                       source = "history",
                       source_id = dbAudit.id,
                       user_id = jwId,
                       router = "history_view",
                       content = "您有变更订单需要提交审核",
                       read_status = 0
                    });
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
                    source = "history",
                    source_id = dbAudit.id,
                    user_id = dbAudit.salesman_id,
                    router = "history_view",
                    content = "您的变更订单已通过提交审核",
                    read_status = 0
                });

                holders = Uof.Ihistory_shareholderService.GetAll(h => h.history_id == dbAudit.id && h.master_id == dbAudit.source_id).ToList();
            }

            dbAudit.date_updated = DateTime.Now;

            var r = Uof.IhistoryService.UpdateEntity(dbAudit);

            if (r)
            {
                Uof.IwaitdealService.AddEntities(waitdeals);

                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbAudit.id,
                    source_name = "history",
                    title = "通过审核",
                    is_system = 1,
                    content = string.Format("{0}通过了{1}", arrs[3], t)
                });

                if(dbAudit.status == 3)
                {
                    switch (dbAudit.source)
                    {
                        case "reg_abroad":
                            //dbAudit.value
                            if (dbAudit.value.Length > 0 && dbAudit.value != "{}")
                            {
                                JavaScriptSerializer jsonSerialize = new JavaScriptSerializer();
                                var obj = jsonSerialize.Deserialize<HistoryAbroad>(dbAudit.value);
                                var dbAbroad = Uof.Ireg_abroadService.GetAll(a => a.id == dbAudit.source_id).FirstOrDefault();

                                var dbAbroadHistory = Uof.Iabroad_historyService.GetAll(a => a.master_id == dbAudit.source_id).FirstOrDefault();
                                if (dbAbroadHistory == null)
                                {
                                    dbAbroadHistory = new abroad_history();
                                }

                                if (obj.name_cn != null && obj.name_cn.Length > 0)
                                {
                                    dbAbroadHistory.name_cn = dbAbroad.name_cn + "|" + dbAudit.date_created.Value.ToString("yyyy-MM-dd");
                                    dbAbroad.name_cn = obj.name_cn;                                    
                                }
                                if (obj.name_en != null && obj.name_en.Length > 0)
                                {
                                    dbAbroadHistory.name_en = dbAbroad.name_en + "|" + dbAudit.date_created.Value.ToString("yyyy-MM-dd");
                                    dbAbroad.name_en = obj.name_en;
                                }
                                if (obj.address != null && obj.address.Length > 0)
                                {
                                    dbAbroadHistory.address = dbAbroad.address + "|" + dbAudit.date_created.Value.ToString("yyyy-MM-dd");
                                    dbAbroad.address = obj.address;
                                }
                                if (obj.reg_no != null && obj.reg_no.Length > 0)
                                {
                                    dbAbroadHistory.reg_no = dbAbroad.reg_no + "|" + dbAudit.date_created.Value.ToString("yyyy-MM-dd");
                                    dbAbroad.reg_no = obj.reg_no;
                                }
                                if (obj.others != null && obj.others.Length > 0)
                                {
                                    dbAbroadHistory.others = obj.others;
                                }

                                try
                                {
                                    Uof.Ireg_abroadService.UpdateEntity(dbAbroad);

                                    if (dbAbroadHistory.name_cn.Length > 0
                                    || dbAbroadHistory.name_en.Length > 0
                                    || dbAbroadHistory.address.Length > 0
                                    || dbAbroadHistory.reg_no.Length > 0)
                                    {
                                        if (dbAbroadHistory.id > 0)
                                        {
                                            Uof.Iabroad_historyService.UpdateEntity(dbAbroadHistory);
                                        }
                                        else
                                        {
                                            dbAbroadHistory.master_id = dbAbroad.id;
                                            Uof.Iabroad_historyService.AddEntity(dbAbroadHistory);
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }
                            
                            break;
                        default:
                            break;
                    }
                }

                #region 股东董事
                if (holders!= null && holders.Count() > 0 && dbAudit.status == 3)
                {
                    //dbAudit.source_id
                   var dbHolders = Uof.Iabroad_shareholderService.GetAll(a => a.master_id == dbAudit.source_id && a.source == dbAudit.source).ToList();
                   var newHolders = new List<abroad_shareholder>();
                   var updateHolders = new List<abroad_shareholder>();

                    foreach (var item in holders)
                    {
                        var holder = dbHolders.Where(d => d.id == item.person_id).FirstOrDefault();
                        if (holder == null)
                        {
                            newHolders.Add(new abroad_shareholder()
                            {
                                name = item.name,
                                cardNo = item.name,
                                changed_type = item.changed_type,
                                date_created = DateTime.Now,
                                date_changed = item.date_changed,
                                gender = item.gender,
                                history_id = item.history_id,
                                master_id = item.master_id,
                                source = item.source,
                                takes = item.takes,
                                type = item.type,
                                memo = item.memo
                            });
                        }
                        else
                        {
                            holder.changed_type = item.changed_type;
                            holder.takes = item.takes;
                            holder.history_id = item.history_id;
                            holder.date_changed = item.date_changed;
                            updateHolders.Add(holder);
                        }
                    }

                    if (newHolders != null && newHolders.Count() > 0)
                    {
                        Uof.Iabroad_shareholderService.AddEntities(newHolders);
                    }
                    if (updateHolders != null && updateHolders.Count() > 0)
                    {
                        Uof.Iabroad_shareholderService.UpdateEntities(updateHolders);
                    }
                }
                #endregion
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

            var dbAudit = Uof.IhistoryService.GetById(id);
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
                    source = "history",
                    source_id = dbAudit.id,
                    user_id = dbAudit.salesman_id,
                    router = "history_view",
                    content = "您的变更订单未通过财务审核",
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
                    source = "history",
                    source_id = dbAudit.id,
                    user_id = dbAudit.salesman_id,
                    router = "history_view",
                    content = "您的变更订单未通过提交审核",
                    read_status = 0
                });
            }

            dbAudit.date_updated = DateTime.Now;

            var r = Uof.IhistoryService.UpdateEntity(dbAudit);

            if (r)
            {
                Uof.IwaitdealService.AddEntities(waitdeals);

                Uof.ItimelineService.AddEntity(new timeline()
                {
                    source_id = dbAudit.id,
                    source_name = "history",
                    title = "驳回审核",
                    is_system = 1,
                    content = string.Format("{0}{1}, 驳回理由: {2}", arrs[3], t, description)
                });
            }

            return Json(new { success = r, message = r ? "" : "审核失败" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            var abroad = Uof.IhistoryService.GetById(id);

            var r = Uof.IhistoryService.DeleteEntity(abroad);
            if (r)
            {
                var incomes = Uof.IincomeService.GetAll(i => i.source_name == "history" && i.source_id == id).ToList();
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

        private int? GetSubmitForHistory(history dbHistory)
        {
            var key = "";
            switch (dbHistory.source)
            {
                case "reg_abroad":
                    key = "JW_ID";
                    break;
                case "reg_internal":
                    key = "GN_ID";
                    break;
                case "patent":
                    key = "ZL_ID";
                    break;
                case "trademark":
                    key = "SB_ID";
                    break;
                default:
                    break;
            }

            return GetSubmitMemberByKey(key);
        }

        public ActionResult GetShareHolder(string name, int id)
        {
            var source = "";
            switch (name)
            {
                case "abroad":
                    source = "reg_abroad";
                    break;
                case "internal":
                    source = "reg_internal";
                    break;
                default:
                    break;
            }
            var list = Uof.Iabroad_shareholderService
                .GetAll(s => s.master_id == id && s.source == source && s.type == "股东" && s.changed_type != "exit")
                .ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDirectory(string name, int id)
        {
            var source = "";
            switch (name)
            {
                case "abroad":
                    source = "reg_abroad";
                    break;
                case "internal":
                    source = "reg_internal";
                    break;
                default:
                    break;
            }
            var list = Uof.Iabroad_shareholderService
                .GetAll(s => s.master_id == id && s.source == source && s.type == "董事" && s.changed_type != "exit")
                .ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}