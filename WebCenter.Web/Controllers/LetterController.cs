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
            Expression<Func<mail, bool>> ownerQuery = c => true;
            if (!string.IsNullOrEmpty(request.name))
            {
                ownerQuery = c => (c.owner.IndexOf(request.name) > -1);
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
                .Where(ownerQuery)
                .Where(date1Query)
                .Where(date2Query)
                .OrderByDescending(item => item.id).Select(c => new
                {
                    id = c.id,
                    code = c.code,
                    date_at = c.date_at,
                    description = c.description,
                    file_url = c.file_url,
                    owner = c.owner,
                    merchant = c.merchant,
                    letter_type = c.letter_type,
                    type = c.type
                }).ToPagedList(request.index, request.size).ToList();

            var totalRecord = Uof.ImailService
                .GetAll(condition)
                .Where(ownerQuery)
                .Where(date1Query)
                .Where(date2Query)
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

            var _l = Uof.ImailService.AddEntity(l);
            return Json(new { id = _l.id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update(mail c)
        {
            var _c = Uof.ImailService.GetById(c.id);

            if (_c.type == c.type &&
                _c.owner == c.owner &&
                _c.letter_type == c.letter_type &&
                _c.merchant == c.merchant &&
                _c.date_at == c.date_at &&
                _c.code == c.code &&
                _c.date_at == c.date_at &&

                _c.description == c.description &&
                _c.file_url == c.file_url

                )
            {
                return SuccessResult;
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

            _c.date_updated = DateTime.Now;

            var r = Uof.ImailService.UpdateEntity(_c);

            return Json(new { success = r, id = _c.id }, JsonRequestBehavior.AllowGet);
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
                merchant = c.merchant,
                letter_type = c.letter_type,
                type = c.type
            }).FirstOrDefault();

            return Json(_l, JsonRequestBehavior.AllowGet);
        }
    }
}