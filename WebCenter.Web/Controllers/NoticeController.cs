using System;
using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;

namespace WebCenter.Web.Controllers
{
    [KgAuthorize]
    public class NoticeController : BaseController
    {


        public NoticeController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        public ActionResult List(int pageIndex, int pageSize, int companyId)
        {
            var _count = Uof.InoticeService.GetAll(n => n.company_id == companyId).Count();
            //var currentUser = HttpContext.User.Identity as UserIdentity;
            var list = Uof.InoticeService.GetAll(n => n.company_id == companyId).OrderByDescending(n => n.id)
                .Select(n => new { n.id, n.title, n.content, n.company_id, n.creator, n.user.name, n.date_created })
                .ToPagedList(pageIndex, pageSize)
                .ToList()
                .Select(s => new
                {
                    id = s.id,
                    title = s.title,
                    content = s.content,
                    company_id = s.company_id,
                    creator = s.creator,
                    creator_name = s.name,
                    date_created = s.date_created.Value.ToString("yyyy-MM-dd")
                });

            return Json(new { total = _count, notices = list }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownRefresh(int firstId, int companyId)
        {
            var list = Uof.InoticeService.GetAll(n => n.company_id == companyId && n.id > firstId).OrderBy(n => n.id)
                .Select(n => new { n.id, n.title, n.content, n.company_id, n.creator, n.user.name, n.date_created })
                .ToList()
                .Select(s => new
                {
                    id = s.id,
                    title = s.title,
                    content = s.content,
                    company_id = s.company_id,
                    creator = s.creator,
                    creator_name = s.name,
                    date_created = s.date_created.Value.ToString("yyyy-MM-dd")
                }).ToList();

            return Json(new { notices = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Create(notice _notice)
        {
            if(_notice.creator == null)
            {
                return Json(new { success = false, message = "parameter creator is required" }, JsonRequestBehavior.AllowGet);
            }
            if (_notice.company_id == null)
            {
                return Json(new { success = false, message = "parameter company_id is required" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(_notice.title))
            {
                return Json(new { success = false, message = "标题不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(_notice.content))
            {
                return Json(new { success = false, message = "内容不能为空" }, JsonRequestBehavior.AllowGet);
            }

            _notice.date_created = DateTime.Now;

            var newNotice = Uof.InoticeService.AddEntity(_notice);

            return Json(new { success = true, result = newNotice.id }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Delete(int id, int deletedBy)
        {
            if (id == 0)
            {
                return Json(new { success = false, message = "parameter id is required" }, JsonRequestBehavior.AllowGet);
            }

            var _notice = Uof.InoticeService.GetById(id);
            var _user = Uof.IuserService.GetById(deletedBy);

            if(_user.is_admin == 0 && _notice.creator.Value != deletedBy)
            {
                return Json(new { success = false, message = "您没有权限删除该公告" }, JsonRequestBehavior.AllowGet);
            }

            if(_user.is_admin == 1 && _notice.company_id != _user.company_id)
            {
                return Json(new { success = false, message = "您没有权限删除该公告" }, JsonRequestBehavior.AllowGet);
            }

            var r = Uof.InoticeService.DeleteEntity(_notice);

            return Json(new { success = r, message = r ? "" : "删除失败" }, JsonRequestBehavior.AllowGet);
        }
    }
}