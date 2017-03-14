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
    public class LetterController : BaseController
    {
        public LetterController(IUnitOfWork UOF)
            : base(UOF)
        {
        }

        public ActionResult Search(LetterRequest request)
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

            var ops = arrs[4].Split(',');
            var hasCompany = ops.Where(o => o == "1").FirstOrDefault();

            Expression<Func<mail, bool>> userQuery = c => true;
            if (hasCompany == null)
            {
                userQuery = c => (c.creator_id == userId || c.audit_id == userId);
            }

            Expression<Func<mail, bool>> statusQuery = c => true;
            if (request.status != null)
            {
                statusQuery = c => c.review_status == request.status;
            }

            Expression<Func<mail, bool>> nameQuery = c => true;
            if (!string.IsNullOrEmpty(request.name))
            {
                nameQuery = c => (c.order_name.Contains(request.name) || c.order_code.Contains(request.name) || c.code.Contains(request.name));
            }

            Expression<Func<mail, bool>> condition = c => true;
            if (!string.IsNullOrEmpty(request.type))
            {
                condition = c => (c.type == request.type);
            }

            // 开始日期
            Expression<Func<mail, bool>> date1Query = c => true;
            Expression<Func<mail, bool>> date2Query = c => true;
            if (request.start_time != null)
            {
                date1Query = c => (c.date_at >= request.start_time.Value);
            }
            // 结束日期
            if (request.end_time != null)
            {
                var endTime = request.end_time.Value.AddDays(1);
                date2Query = c => (c.date_at < endTime);
            }
            
            var list = Uof.ImailService.GetAll(condition)                
                .Where(statusQuery)
                .Where(nameQuery)
                .Where(date1Query)
                .Where(date2Query)
                .Where(userQuery)
                .OrderByDescending(item => item.id).Select(c => new
                {
                    id = c.id,
                    code = c.code,
                    date_at = c.date_at,
                    description = c.description,
                    file_url = c.file_url,
                    owner = c.owner,
                    merchant = c.merchant,
                    address = c.address,
                    letter_type = c.letter_type,
                    audit_id = c.audit_id,
                    audit_name = c.member.name,
                    type = c.type,
                    order_id = c.order_id,
                    order_name = c.order_name,
                    order_source = c.order_source,
                    order_code = c.order_code,
                    receiver = c.receiver,
                    review_date = c.review_date,
                    review_moment = c.review_moment,
                    review_status = c.review_status,
                    tel = c.tel,
                    paymode = c.paymode,
                    creator_id = c.creator_id,
                    creator_name = c.member1.name,
                    deleteable = c.creator_id == userId && c.review_status != 1,

                }).ToPagedList(request.index, request.size).ToList();

            var totalRecord = Uof.ImailService
                .GetAll(condition)
                .Where(statusQuery)
                .Where(nameQuery)
                .Where(date1Query)
                .Where(date2Query)
                .Where(userQuery)
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

        [HttpPost]
        public ActionResult Add(mail l)
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
            int.TryParse(arrs[0], out userId);

            l.creator_id = userId;
            l.date_created = DateTime.Now;
            l.review_status = 0;

            var _l = Uof.ImailService.AddEntity(l);

            if (_l != null)
            {
                try
                {
                    Uof.IwaitdealService.AddEntity(new waitdeal
                    {
                        source = "mail",
                        source_id = l.id,
                        user_id = l.audit_id,
                        router = l.type == "寄件" ? "letter_view" : "inbox_view",
                        content = string.Format("您有一笔信件资料需要审核, 信件单号：{0}", _l.code),
                        read_status = 0
                    });

                    if (_l.order_id != null)
                    {
                        var auditor = Uof.ImemberService.GetAll(m => m.id == _l.audit_id).Select(m => m.name).FirstOrDefault();

                        Uof.ItimelineService.AddEntity(new timeline()
                        {
                            source_id = _l.order_id,
                            source_name = _l.order_source,
                            title = string.Format("新增{0}记录", _l.type),
                            is_system = 1,
                            content = string.Format("{0}新建了一笔{1}记录, 审核人{2}, 信件单号: {3}，信件内容: {4}", arrs[3], _l.type, auditor, _l.code, _l.letter_type)
                        });
                    }
                }
                catch (Exception)
                {
                }
            }

            return Json(new { id = _l.id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InsertInbox(mail l, List<InboxOrder> inboxOrders)
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
            int.TryParse(arrs[0], out userId);

            l.creator_id = userId;
            l.date_created = DateTime.Now;
            l.review_status = 0;

            var mails = new List<mail>();
            foreach (var inboxOrder in inboxOrders)
            {
                mails.Add(new mail
                {
                    letter_type = l.letter_type,
                    type = l.type,
                    address = l.address,

                    audit_id = inboxOrder.audit_id,
                    order_source = inboxOrder.order_source,
                    order_code = inboxOrder.order_code,
                    order_id = inboxOrder.order_id,
                    order_name = inboxOrder.order_name,

                    code = l.code,
                    creator_id = l.creator_id,
                    date_at = l.date_at,
                    description = l.description,
                    file_url = l.file_url,
                    owner = l.owner,
                    merchant = l.merchant,
                    paymode = l.paymode,
                    review_status = 0
                });
            }

            var count = Uof.ImailService.AddEntities(mails);

            if (count > 0)
            {
                try
                {
                    var dbMails = Uof.ImailService.GetAll(m => m.code == l.code).ToList();
                    var waitdeals = new List<waitdeal>();

                    foreach (var item in dbMails)
                    {
                        waitdeals.Add(new waitdeal
                        {
                            source = "mail",
                            source_id = item.id,
                            user_id = item.audit_id,
                            router = "inbox_view",
                            content = string.Format("您有一笔信件资料需要审核, 信件单号：{0}", item.code),
                            read_status = 0
                        });
                    }
                    Uof.IwaitdealService.AddEntities(waitdeals);
                }
                catch (Exception)
                {
                }
            }

            return SuccessResult;
        }

        [HttpPost]
        public ActionResult Insert(mail l, List<int> auditIds)
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
            int.TryParse(arrs[0], out userId);

            l.creator_id = userId;
            l.date_created = DateTime.Now;
            l.review_status = 0;

            var mails = new List<mail>();
            foreach (var auditId in auditIds)
            {
                mails.Add(new mail
                {
                    letter_type = l.letter_type,
                    type = l.type,
                    address = l.address,
                    audit_id = auditId,
                    code = l.code,
                    creator_id = l.creator_id,
                    date_at = l.date_at,
                    description = l.description,
                    file_url = l.file_url,
                    owner = l.owner,
                    merchant = l.merchant,
                    paymode = l.paymode,
                    review_status = 0
                });
            }

            var count = Uof.ImailService.AddEntities(mails);

            if (count > 0)
            {
                try
                {
                    var dbMails = Uof.ImailService.GetAll(m => m.code == l.code).ToList();
                    var waitdeals = new List<waitdeal>();

                    foreach (var item in dbMails)
                    {
                        waitdeals.Add(new waitdeal
                        {
                            source = "mail",
                            source_id = item.id,
                            user_id = item.audit_id,
                            router = "inbox_view",
                            content = string.Format("您有一笔信件资料需要审核, 信件单号：{0}", item.code),
                            read_status = 0
                        });
                    }
                    Uof.IwaitdealService.AddEntities(waitdeals);
                }
                catch (Exception)
                {
                }
            }

            return SuccessResult;
        }

        [HttpPost]
        public ActionResult Update(mail c)
        {
            var au = HttpContext.User.Identity.IsAuthenticated;
            if (!au)
            {
                return new HttpUnauthorizedResult();
            }

            var identityName = HttpContext.User.Identity.Name;
            var arrs = identityName.Split('|');
            if (arrs.Length == 0)
            {
                return new HttpUnauthorizedResult();
            }

            var newAutid = c.audit_id;
            var _c = Uof.ImailService.GetById(c.id);
            var oldAutid = _c.audit_id;

            var needReview = false;
            if (c.review_status == -1)
            {
                c.review_status = 0;
                needReview = true;
            }

            if (c.review_status == null)
            {
                c.review_status = 0;
                needReview = true;
            }

            _c.type = c.type;
            _c.owner = c.owner;
            _c.letter_type = c.letter_type;
            _c.merchant = c.merchant;
            _c.date_at = c.date_at;
            _c.date_at = c.date_at;
            _c.code = c.code;
            _c.date_at = c.date_at;
            _c.description = c.description;
            _c.address = c.address;
            _c.audit_id = c.audit_id;
            _c.date_updated = DateTime.Now;
            _c.order_code = c.order_code;
            _c.order_id = c.order_id;
            _c.order_name = c.order_name;
            _c.order_source = c.order_source;
            _c.receiver = c.receiver;
            _c.review_status = c.review_status;
            _c.paymode = c.paymode;
            _c.tel = c.tel;

            var r = Uof.ImailService.UpdateEntity(_c);

            if (r)
            {
                try
                {
                    if (oldAutid != newAutid || needReview)
                    {
                        Uof.IwaitdealService.AddEntity(new waitdeal
                        {
                            source = "mail",
                            source_id = _c.id,
                            user_id = newAutid,
                            router = _c.type == "寄件" ? "letter_view" : "inbox_view",
                            content = string.Format("您有一笔信件资料需要审核, 信件单号：{0}", _c.code),
                            read_status = 0
                        });
                    }

                    if (_c.type == "寄件")
                    {
                        var auditor = Uof.ImemberService.GetAll(m => m.id == newAutid).Select(m => m.name).FirstOrDefault();
                        Uof.ItimelineService.AddEntity(new timeline()
                        {
                            source_id = _c.order_id,
                            source_name = _c.order_source,
                            title = string.Format("修改{0}记录", _c.type),
                            is_system = 1,
                            content = string.Format("{0}修改了一笔{1}记录, 审核人{2}, 信件单号: {3}，信件内容: {4}", arrs[3], _c.type, auditor, _c.code, _c.letter_type)
                        });
                    }
                }
                catch (Exception)
                {
                }
            }

            return Json(new { success = r, id = _c.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            var c = Uof.ImailService.GetById(id);
            if (c == null)
            {
                return ErrorResult;
            }

            var r = Uof.ImailService.DeleteEntity(c);

            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get(int id)
        {
            var _l = Uof.ImailService.GetAll(l => l.id == id).Select(c => new
            {
                id = c.id,
                code = c.code,
                date_at = c.date_at,
                description = c.description,
                file_url = c.file_url,
                owner = c.owner,
                address = c.address,
                merchant = c.merchant,
                letter_type = c.letter_type,
                audit_id = c.audit_id,
                audit_name = c.member.name,
                type = c.type,
                order_id = c.order_id,
                order_name = c.order_name,
                order_source = c.order_source,
                order_code = c.order_code,
                receiver = c.receiver,
                review_date = c.review_date,
                review_moment = c.review_moment,
                review_status = c.review_status,
                tel = c.tel,
                creator_id = c.creator_id,
                creator_name = c.member1.name,
                paymode = c.paymode
            }).FirstOrDefault();

            return Json(_l, JsonRequestBehavior.AllowGet);
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

            var dbMail = Uof.ImailService.GetById(id);
            if (dbMail == null)
            {
                return Json(new { success = false, message = "找不到该数据" }, JsonRequestBehavior.AllowGet);
            }
            var waitdeals = new List<waitdeal>();

            dbMail.review_status = 1;
            dbMail.review_date = DateTime.Now;
            dbMail.review_moment = "";

            waitdeals.Add(new waitdeal
            {
                source = "mail",
                source_id = dbMail.id,
                user_id = dbMail.creator_id,
                router = dbMail.type == "寄件" ? "letter_view" : "inbox_view",
                content = string.Format("您的笔信件资料通过审核, 编号：{0}", dbMail.code),
                read_status = 0
            });

            dbMail.date_updated = DateTime.Now;

            var r = Uof.ImailService.UpdateEntity(dbMail);

            if (r)
            {
                Uof.IwaitdealService.AddEntities(waitdeals);
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

            var dbMail = Uof.ImailService.GetById(id);
            if (dbMail == null)
            {
                return Json(new { success = false, message = "找不到该数据" }, JsonRequestBehavior.AllowGet);
            }
            var waitdeals = new List<waitdeal>();

            dbMail.review_status = -1;
            dbMail.review_date = DateTime.Now;
            dbMail.review_moment = description;

            waitdeals.Add(new waitdeal
            {
                source = "mail",
                source_id = dbMail.id,
                user_id = dbMail.creator_id,
                //router = "letter_view",
                router = dbMail.type == "寄件" ? "letter_view" : "inbox_view",
                content = string.Format("您的笔信件资料通过审核, 编号：{0}", dbMail.code),
                read_status = 0
            });

            dbMail.date_updated = DateTime.Now;

            var r = Uof.ImailService.UpdateEntity(dbMail);

            if (r)
            {
                Uof.IwaitdealService.AddEntities(waitdeals);
            }
            return Json(new { success = r, message = r ? "" : "审核失败" }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SearchOrder(LetterOrderRequest request)
        {
            var page = new
            {
                current_index = request.index,
                current_size = request.size,
                total_size = 0,
                total_page = 0
            };

            var totalRecord = 0;
            var totalPages = 0;

            var result = new
            {
                page = page,
                items = new List<LetterOrder>()
            };

            if (string.IsNullOrEmpty(request.type))
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            //reg_abroad
            //reg_internal
            //audit
            //patent
            //trademark
            var list = new List<LetterOrder>();
            switch (request.type)
            {
                case "reg_abroad":
                    Expression<Func<reg_abroad, bool>> condition1 = c => true;
                    if (!string.IsNullOrEmpty(request.name))
                    {
                        condition1 = c => (c.name_cn.Contains(request.name) || c.name_en.Contains(request.name) || c.code.Contains(request.name));
                    }
                    list = Uof.Ireg_abroadService.GetAll(condition1).OrderBy(item => item.code).Select(a => new LetterOrder
                    {
                        order_code = a.code,
                        order_id = a.id,
                        order_name = a.name_en ?? a.name_cn,
                        order_source = "reg_abroad",
                        salesman_id = a.salesman_id,
                        assistant_id = a.assistant_id
                    }).ToPagedList(request.index, request.size).ToList();


                    totalRecord = Uof.Ireg_abroadService.GetAll(condition1).Count();
                    if (totalRecord > 0)
                    {
                        totalPages = (totalRecord + request.size - 1) / request.size;
                    }
                    page = new
                    {
                        current_index = request.index,
                        current_size = request.size,
                        total_size = totalRecord,
                        total_page = totalPages
                    };
                    break;
                case "reg_internal":
                    Expression<Func<reg_internal, bool>> condition2 = c => true;
                    if (!string.IsNullOrEmpty(request.name))
                    {
                        condition2 = c => (c.name_cn.Contains(request.name) || c.code.Contains(request.name));
                    }
                    list = Uof.Ireg_internalService.GetAll(condition2).OrderBy(item => item.code).Select(a => new LetterOrder
                    {
                        order_code = a.code,
                        order_id = a.id,
                        order_name = a.name_cn,
                        order_source = "reg_internal",
                        salesman_id = a.salesman_id,
                        assistant_id = a.assistant_id
                    }).ToPagedList(request.index, request.size).ToList();
                    
                    totalRecord = Uof.Ireg_internalService.GetAll(condition2).Count();
                    if (totalRecord > 0)
                    {
                        totalPages = (totalRecord + request.size - 1) / request.size;
                    }
                    page = new
                    {
                        current_index = request.index,
                        current_size = request.size,
                        total_size = totalRecord,
                        total_page = totalPages
                    };
                    break;
                case "audit":
                    Expression<Func<audit, bool>> condition3 = c => true;
                    if (!string.IsNullOrEmpty(request.name))
                    {
                        condition3 = c => (c.name_cn.Contains(request.name) || c.code.Contains(request.name));
                    }
                    list = Uof.IauditService.GetAll(condition3).OrderBy(item => item.code).Select(a => new LetterOrder
                    {
                        order_code = a.code,
                        order_id = a.id,
                        order_name = a.name_cn ?? a.name_en,
                        order_source = "audit",
                        salesman_id = a.salesman_id,
                        assistant_id = a.assistant_id
                    }).ToPagedList(request.index, request.size).ToList();

                    totalRecord = Uof.IauditService.GetAll(condition3).Count();
                    if (totalRecord > 0)
                    {
                        totalPages = (totalRecord + request.size - 1) / request.size;
                    }
                    page = new
                    {
                        current_index = request.index,
                        current_size = request.size,
                        total_size = totalRecord,
                        total_page = totalPages
                    };
                    break;
                case "patent":
                    Expression<Func<patent, bool>> condition4 = c => true;
                    if (!string.IsNullOrEmpty(request.name))
                    {
                        condition4 = c => (c.name.Contains(request.name) || c.code.Contains(request.name));
                    }
                    list = Uof.IpatentService.GetAll(condition4).OrderBy(item => item.code).Select(a => new LetterOrder
                    {
                        order_code = a.code,
                        order_id = a.id,
                        order_name = a.name,
                        order_source = "patent",
                        salesman_id = a.salesman_id,
                        assistant_id = a.assistant_id
                    }).ToPagedList(request.index, request.size).ToList();

                    totalRecord = Uof.IpatentService.GetAll(condition4).Count();
                    if (totalRecord > 0)
                    {
                        totalPages = (totalRecord + request.size - 1) / request.size;
                    }
                    page = new
                    {
                        current_index = request.index,
                        current_size = request.size,
                        total_size = totalRecord,
                        total_page = totalPages
                    };
                    break;
                case "trademark":
                    Expression<Func<trademark, bool>> condition5 = c => true;
                    if (!string.IsNullOrEmpty(request.name))
                    {
                        condition5 = c => (c.name.Contains(request.name) || c.code.Contains(request.name));
                    }
                    list = Uof.ItrademarkService.GetAll(condition5).OrderBy(item => item.code).Select(a => new LetterOrder
                    {
                        order_code = a.code,
                        order_id = a.id,
                        order_name = a.name,
                        order_source = "trademark",
                        salesman_id = a.salesman_id,
                        assistant_id = a.assistant_id
                    }).ToPagedList(request.index, request.size).ToList();

                    totalRecord = Uof.ItrademarkService.GetAll(condition5).Count();
                    if (totalRecord > 0)
                    {
                        totalPages = (totalRecord + request.size - 1) / request.size;
                    }
                    page = new
                    {
                        current_index = request.index,
                        current_size = request.size,
                        total_size = totalRecord,
                        total_page = totalPages
                    };
                    break;
                default:
                    break;
            }

            result = new
            {
                page = page,
                items = list
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PassInbox(PassInbox passInbox)
        {
            var _c = Uof.ImailService.GetById(passInbox.id);

            _c.review_status = 1;
            _c.letter_type = passInbox.letter_type;
            _c.order_code = passInbox.order_code;
            _c.order_id = passInbox.order_id;
            _c.order_name = passInbox.order_name;
            _c.order_source = passInbox.order_source;
            
            var r = Uof.ImailService.UpdateEntity(_c);

            if (r)
            {
                try
                {
                    if (_c.order_id != null)
                    {
                        var m1 = Uof.ImemberService.GetAll(m => m.id == _c.creator_id).Select(m => m.name).FirstOrDefault();
                        var creator = "";
                        if (m1 != null)
                        {
                            creator = m1;
                        }

                        Uof.ItimelineService.AddEntity(new timeline()
                        {
                            source_id = _c.order_id,
                            source_name = _c.order_source,
                            title = string.Format("新增{0}记录", _c.type),
                            is_system = 1,
                            content = string.Format("{0}新建了一笔{1}记录, 审核人{2}, 信件单号: {3}，信件内容: {4}", creator, _c.type,  _c.member.name, _c.code, _c.letter_type)
                        });
                    }
                }
                catch (Exception)
                {
                }
            }

            return Json(new { success = r, id = _c.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLetterForAudit()
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

            var list = Uof.ImailService.GetAll(m => m.review_status == 0 && m.audit_id == userId).Select(c => new
            {
                id = c.id,
                code = c.code,
                date_at = c.date_at,
                description = c.description,
                letter_type = c.letter_type,
                audit_id = c.audit_id,
                audit_name = c.member.name,
                type = c.type,
                order_id = c.order_id,
                order_name = c.order_name,
                order_source = c.order_source,
                order_code = c.order_code,               
                review_date = c.review_date,
                review_moment = c.review_moment,
                review_status = c.review_status,
                creator_id = c.creator_id,
            }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}