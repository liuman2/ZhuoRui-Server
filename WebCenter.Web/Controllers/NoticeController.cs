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
    public class NoticeController : BaseController
    {
        public NoticeController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        public ActionResult Search(int type, int index = 1, int size = 10, string name = "")
        {
            Expression<Func<notice, bool>> condition = c => c.type == type;
            Expression<Func<notice, bool>> nameQuery = c => true;

            if (!string.IsNullOrEmpty(name))
            {
                nameQuery = c => (c.title.IndexOf(name) > -1 || c.code.IndexOf(name) > -1);
            }
            
            var list = Uof.InoticeService.GetAll(condition)
                .Where(nameQuery)
                .OrderByDescending(item => item.id).Select(c => new
                {
                    id = c.id,
                    code = c.code,
                    title = c.title,
                    type = c.type,
                    creator_id = c.creator_id,
                    content = c.content,
                    date_created = c.date_created,
                    status = c.status
                }).ToPagedList(index, size).ToList();

            var totalRecord = Uof.InoticeService.GetAll(condition).Count();

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

        public ActionResult Add(Notice c)
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
            c.creator_id = userId;
            c.status = 1;
            
            var _c = Uof.InoticeService.AddEntity(new notice
            {
                creator_id = c.creator_id,
                title = c.title,
                code = c.code,
                type = c.type,
                content = c.content,
                status = 1
            });

            if (_c != null && c.attachments != null && c.attachments.Count > 0)
            {
                var atts = new List<attachment>();
                foreach (var item in c.attachments)
                {
                    atts.Add(new attachment
                    {
                        source_id = _c.id,
                        source_name = "notice",
                        name = item.name ?? "",
                        attachment_url = item.attachment_url,
                    });
                }
                var newCustomer = Uof.IattachmentService.AddEntities(atts);
            }

            return SuccessResult;
        }

        public ActionResult Update(Notice c)
        {
            var _c = Uof.InoticeService.GetById(c.id);

            _c.title = c.title;
            _c.code = c.code;
            _c.content = c.content;
            _c.date_updated = DateTime.Now;

            var r = Uof.InoticeService.UpdateEntity(_c);

            if (r && c.attachments != null && c.attachments.Count > 0)
            {
                var newAtts = new List<attachment>();
                var attIds = Uof.IattachmentService.GetAll(a => a.source_id == _c.id && a.source_name == "notice").Select(a => a.id).ToList();
                if (attIds.Count > 0)
                {
                    newAtts = c.attachments.Where(a => !attIds.Contains(a.id)).ToList();
                }
                else
                {
                    newAtts = c.attachments;
                }

                if (newAtts.Count > 0)
                {
                    foreach (var item in newAtts)
                    {
                        item.source_id = _c.id;
                        item.source_name = "notice";
                    }
                    Uof.IattachmentService.AddEntities(newAtts);
                }
            }

            return SuccessResult;
        }

        public ActionResult Get(int id)
        {
            var _notice = Uof.InoticeService.GetById(id);

            var noticeResponse = new Notice();
            if (_notice != null)
            {
                var atts = Uof.IattachmentService.GetAll(a => a.source_id == _notice.id && a.source_name == "notice").ToList();
                noticeResponse.attachments = atts;

                noticeResponse.id = _notice.id;
                noticeResponse.creator_id = _notice.creator_id;
                noticeResponse.title = _notice.title;
                noticeResponse.code = _notice.code;
                noticeResponse.type = _notice.type;
                noticeResponse.content = _notice.content;
                noticeResponse.status = _notice.status;
                noticeResponse.date_created = _notice.date_created.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
                        
            return Json(noticeResponse, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Cancel(int id)
        {
            var c = Uof.InoticeService.GetById(id);
            if (c == null)
            {
                return ErrorResult;
            }

            c.status = 2;
            c.date_updated = DateTime.Now;

            var r = Uof.InoticeService.UpdateEntity(c);

            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Release(int id)
        {
            var c = Uof.InoticeService.GetById(id);
            if (c == null)
            {
                return ErrorResult;
            }

            c.status = 1;
            c.date_updated = DateTime.Now;

            var r = Uof.InoticeService.UpdateEntity(c);

            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            var c = Uof.InoticeService.GetById(id);
            if (c == null)
            {
                return ErrorResult;
            }

            var r = Uof.InoticeService.DeleteEntity(c);

            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTop3()
        {
            var list = Uof.InoticeService.GetAll(n => n.status == 1).Select(n => new simpleNotice
            {
                id = n.id,
                title = n.title,
                created = n.date_created,
                isNew = false,
            }).OrderByDescending(n => n.id).Take(3).ToList();

            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    if (item.created.Value.AddDays(6) >= DateTime.Today)
                    {
                        item.isNew = true;
                    }
                }
            }
            var total = Uof.InoticeService.GetAll(n => n.status == 1).Count();
            return Json(new { list = list, total = total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Views(int index = 1, int size = 10, string name = "")
        {
            Expression<Func<notice, bool>> condition = c => c.status == 1;
            Expression<Func<notice, bool>> nameQuery = c => true;

            if (!string.IsNullOrEmpty(name))
            {
                nameQuery = c => (c.title.IndexOf(name) > -1 || c.code.IndexOf(name) > -1);
            }

            var list = Uof.InoticeService.GetAll(condition)
                .Where(nameQuery)
                .OrderByDescending(item => item.id).Select(c => new
                {
                    id = c.id,
                    code = c.code,
                    title = c.title,
                    date_created = c.date_created
                }).ToPagedList(index, size).ToList();

            var totalRecord = Uof.InoticeService.GetAll(condition).Count();

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
    }
}