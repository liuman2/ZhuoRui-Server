using System;
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

            if (_history.logoff == 1)
            {
                _history.value = "{}";
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

            if (_history.logoff != 1)
            {
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
                            position = item.position,
                            date_changed = DateTime.Today,
                        });
                    }

                    Uof.Ihistory_shareholderService.AddEntities(historyHoders);
                }
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
            dbHistory.logoff = _history.logoff;
            dbHistory.logoff_memo = _history.logoff_memo;
            dbHistory.date_updated = DateTime.Now;
            dbHistory.change_owner = _history.change_owner;

            if (dbHistory.logoff == 1)
            {
                dbHistory.value = "{}";
            }

            var result = Uof.IhistoryService.UpdateEntity(dbHistory);
            if (!result)
            {
                return Json(new { success = false, message = "添加失败" }, JsonRequestBehavior.AllowGet);
            }

            if (dbHistory.logoff == 1)
            {
                // 注销
                return Json(new { success = result, id = dbHistory.id }, JsonRequestBehavior.AllowGet);
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
                            position = item.position,
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
            if (source == "reg_internal")
            {
                #region 旧数据处理
                try
                {
                    var dbReg = Uof.Ireg_internalService.GetAll(i => i.id == source_id).FirstOrDefault();
                    var shareHolder = new List<internal_shareholder>();
                    if (dbReg.shareholders != null && dbReg.shareholders.Length > 0)
                    {
                        JavaScriptSerializer jsonSerialize = new JavaScriptSerializer();
                        var shareHolderList = jsonSerialize.Deserialize<List<Shareholder>>(dbReg.shareholders);
                        if (shareHolderList != null && shareHolderList.Count > 0)
                        {
                            foreach (var item in shareHolderList)
                            {
                                shareHolder.Add(new internal_shareholder()
                                {
                                    cardNo = item.cardNo,
                                    changed_type = "original",
                                    date_changed = null,
                                    date_created = dbReg.date_created,
                                    gender = item.gender,
                                    master_id = dbReg.id,
                                    memo = item.memo,
                                    name = item.name,
                                    position = item.position,
                                    source = "reg_internal",
                                    takes = item.takes,
                                    type = "股东"
                                });
                            }
                            dbReg.shareholders = null;
                        }
                    }

                    //if (reg.director != null && reg.director.Length > 0)
                    //{
                    //    shareHolder.Add(new internal_shareholder()
                    //    {
                    //        cardNo = reg.director_card_no,
                    //        changed_type = "original",
                    //        date_changed = null,
                    //        date_created = reg.date_created,
                    //        gender = null,
                    //        master_id = reg.id,
                    //        memo = null,
                    //        name = reg.director,
                    //        position = null,
                    //        source = "reg_internal",
                    //        takes = null,
                    //        type = "监事"
                    //    });

                    //    dbReg.director = null;
                    //    dbReg.director_card_no = null;
                    //}

                    if (shareHolder.Count() > 0)
                    {
                        var count = Uof.Iinternal_shareholderService.AddEntities(shareHolder);
                        if (count > 0)
                        {
                            Uof.Ireg_internalService.UpdateEntity(dbReg);
                        }
                    }
                }
                catch (Exception)
                {
                }

                #endregion
            }
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

            var orderName = "";
            switch (source)
            {
                case "reg_abroad":
                    orderName = Uof.Ireg_abroadService.GetAll(a => a.id == source_id).Select(a => a.name_en + " " + a.name_cn).FirstOrDefault();
                    break;
                case "reg_internal":
                    orderName = Uof.Ireg_internalService.GetAll(a => a.id == source_id).Select(a => a.name_cn).FirstOrDefault();
                    break;
                case "patent":
                    orderName = Uof.IpatentService.GetAll(a => a.id == source_id).Select(a => a.name).FirstOrDefault();
                    break;
                case "trademark":
                    orderName = Uof.ItrademarkService.GetAll(a => a.id == source_id).Select(a => a.name).FirstOrDefault();
                    break;
                default:
                    break;
            }

            var result = new
            {
                orderName = orderName,
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
                rate = c.rate ?? 1,
                currency = c.currency,
                amount_transaction = c.amount_transaction,
                amount_income = 0,
                amount_unreceive = 0,
                progress = c.progress,
                salesman_id = c.salesman_id,
                salesman = c.member2.name,
                finance_review_moment = c.finance_review_moment,
                submit_review_moment = c.submit_review_moment,
                logoff = c.logoff,
                logoff_memo = c.logoff_memo,

                change_owner = c.change_owner,
                change_owner_name = c.member4.name,

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
                submit_review_moment = c.submit_review_moment,
                logoff = c.logoff,
                logoff_memo = c.logoff_memo,
                change_owner = c.change_owner,
                change_owner_name = c.member4.name,

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
                    if (dbAudit.logoff == 0)
                    {
                        switch (dbAudit.source)
                        {
                            case "reg_abroad":
                                //dbAudit.value
                                #region reg_abroad
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
                                #endregion
                                #region 股东董事
                                if (holders != null && holders.Count() > 0)
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
                                break;
                            case "reg_internal":
                                #region reg_internal
                                if (dbAudit.value.Length > 0 && dbAudit.value != "{}")
                                {
                                    JavaScriptSerializer jsonSerialize = new JavaScriptSerializer();
                                    var obj = jsonSerialize.Deserialize<HistoryInternal>(dbAudit.value);
                                    var dbInternal = Uof.Ireg_internalService.GetAll(a => a.id == dbAudit.source_id).FirstOrDefault();

                                    var dbInternalHistory = Uof.Iinternal_historyService.GetAll(a => a.master_id == dbAudit.source_id).FirstOrDefault();
                                    if (dbInternalHistory == null)
                                    {
                                        dbInternalHistory = new internal_history();
                                    }

                                    if (obj.name_cn != null && obj.name_cn.Length > 0)
                                    {
                                        dbInternalHistory.name_cn = dbInternal.name_cn + "|" + dbAudit.date_created.Value.ToString("yyyy-MM-dd");
                                        dbInternal.name_cn = obj.name_cn;
                                    }
                                    if (obj.legal != null && obj.legal.Length > 0)
                                    {
                                        dbInternalHistory.legal = dbInternal.legal + "|" + dbAudit.date_created.Value.ToString("yyyy-MM-dd");
                                        dbInternal.legal = obj.legal;
                                    }
                                    if (obj.address != null && obj.address.Length > 0)
                                    {
                                        dbInternalHistory.address = dbInternal.address + "|" + dbAudit.date_created.Value.ToString("yyyy-MM-dd");
                                        dbInternal.address = obj.address;
                                    }
                                    if (obj.reg_no != null && obj.reg_no.Length > 0)
                                    {
                                        dbInternalHistory.reg_no = dbInternal.reg_no + "|" + dbAudit.date_created.Value.ToString("yyyy-MM-dd");
                                        dbInternal.reg_no = obj.reg_no;
                                    }
                                    if (obj.director != null && obj.director.Length > 0)
                                    {
                                        dbInternalHistory.director = dbInternal.director + "|" + dbAudit.date_created.Value.ToString("yyyy-MM-dd");
                                        dbInternal.director = obj.director;
                                    }
                                    if (obj.others != null && obj.others.Length > 0)
                                    {
                                        dbInternalHistory.others = obj.others;
                                    }

                                    try
                                    {
                                        Uof.Ireg_internalService.UpdateEntity(dbInternal);

                                        if (dbInternalHistory.name_cn.Length > 0
                                        || dbInternalHistory.legal.Length > 0
                                        || dbInternalHistory.director.Length > 0
                                        || dbInternalHistory.address.Length > 0
                                        || dbInternalHistory.reg_no.Length > 0)
                                        {
                                            if (dbInternalHistory.id > 0)
                                            {
                                                Uof.Iinternal_historyService.UpdateEntity(dbInternalHistory);
                                            }
                                            else
                                            {
                                                dbInternalHistory.master_id = dbInternal.id;
                                                Uof.Iinternal_historyService.AddEntity(dbInternalHistory);
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                                #endregion
                                #region 股东董事
                                if (holders != null && holders.Count() > 0)
                                {
                                    //dbAudit.source_id
                                    var dbHolders = Uof.Iinternal_shareholderService.GetAll(a => a.master_id == dbAudit.source_id && a.source == dbAudit.source).ToList();
                                    var newHolders = new List<internal_shareholder>();
                                    var updateHolders = new List<internal_shareholder>();

                                    foreach (var item in holders)
                                    {
                                        var holder = dbHolders.Where(d => d.id == item.person_id).FirstOrDefault();
                                        if (holder == null)
                                        {
                                            newHolders.Add(new internal_shareholder()
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
                                                position = item.position,
                                                memo = item.memo
                                            });
                                        }
                                        else
                                        {
                                            holder.changed_type = item.changed_type;
                                            holder.takes = item.takes;
                                            holder.position = item.position;
                                            holder.history_id = item.history_id;
                                            holder.date_changed = item.date_changed;
                                            updateHolders.Add(holder);
                                        }
                                    }

                                    if (newHolders != null && newHolders.Count() > 0)
                                    {
                                        Uof.Iinternal_shareholderService.AddEntities(newHolders);
                                    }
                                    if (updateHolders != null && updateHolders.Count() > 0)
                                    {
                                        Uof.Iinternal_shareholderService.UpdateEntities(updateHolders);
                                    }
                                }
                                #endregion
                                break;
                            default:
                                break;
                        }
                    }

                    if (dbAudit.logoff == 1)
                    {
                        switch (dbAudit.source)
                        {
                            case "reg_abroad":
                                var dbAbroad = Uof.Ireg_abroadService.GetAll(a => a.id == dbAudit.source_id).FirstOrDefault();
                                dbAbroad.order_status = 2;
                                dbAbroad.date_updated = DateTime.Now;
                                Uof.Ireg_abroadService.UpdateEntity(dbAbroad);

                                Uof.ItimelineService.AddEntity(new timeline()
                                {
                                    source_id = dbAbroad.id,
                                    source_name = "reg_abroad",
                                    title = "注销变更",
                                    is_system = 1,
                                    content = string.Format("{0}审核通过了注销变更，该订单被注销了", arrs[3], t)
                                });
                                break;
                            case "reg_internal":
                                var dbInternal = Uof.Ireg_internalService.GetAll(a => a.id == dbAudit.source_id).FirstOrDefault();
                                dbInternal.order_status = 2;
                                dbInternal.date_updated = DateTime.Now;
                                Uof.Ireg_internalService.UpdateEntity(dbInternal);

                                Uof.ItimelineService.AddEntity(new timeline()
                                {
                                    source_id = dbInternal.id,
                                    source_name = "reg_internal",
                                    title = "注销变更",
                                    is_system = 1,
                                    content = string.Format("{0}审核通过了注销变更，该订单被注销了", arrs[3], t)
                                });

                                break;
                            case "trademark":
                                var dbTrademark = Uof.ItrademarkService.GetAll(a => a.id == dbAudit.source_id).FirstOrDefault();
                                dbTrademark.order_status = 2;
                                dbTrademark.date_updated = DateTime.Now;
                                Uof.ItrademarkService.UpdateEntity(dbTrademark);

                                Uof.ItimelineService.AddEntity(new timeline()
                                {
                                    source_id = dbTrademark.id,
                                    source_name = "trademark",
                                    title = "注销变更",
                                    is_system = 1,
                                    content = string.Format("{0}审核通过了注销变更，该订单被注销了", arrs[3], t)
                                });

                                break;
                            case "patent":
                                var dbPatent = Uof.IpatentService.GetAll(a => a.id == dbAudit.source_id).FirstOrDefault();
                                dbPatent.order_status = 2;
                                dbPatent.date_updated = DateTime.Now;
                                Uof.IpatentService.UpdateEntity(dbPatent);

                                Uof.ItimelineService.AddEntity(new timeline()
                                {
                                    source_id = dbPatent.id,
                                    source_name = "patent",
                                    title = "注销变更",
                                    is_system = 1,
                                    content = string.Format("{0}审核通过了注销变更，该订单被注销了", arrs[3], t)
                                });
                                break;
                            default:
                                break;
                        }
                    }                    
                }                
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
                    var list1 = Uof.Iabroad_shareholderService
                        .GetAll(s => s.master_id == id && s.source == source && s.type == "股东" && s.changed_type != "exit")
                        .ToList();
                    return Json(list1, JsonRequestBehavior.AllowGet);
                case "internal":
                    source = "reg_internal";

                    var list2 = Uof.Iinternal_shareholderService
                        .GetAll(s => s.master_id == id && s.source == source && s.type == "股东" && s.changed_type != "exit")
                        .ToList();
                    return Json(list2, JsonRequestBehavior.AllowGet);
            }

            return SuccessResult;
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