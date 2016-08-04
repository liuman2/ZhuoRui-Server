using System;
using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using System.Linq.Expressions;

namespace WebCenter.Web.Controllers
{
    public class DictionaryController : BaseController
    {
        public DictionaryController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        public ActionResult Groups()
        {
            var list = Uof.Idictionary_groupService.GetAll().Select(d => new { id = d.id, name = d.name, group = d.group, parent_id = 0 }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDictionaryByGroup(string group)
        {
            var list = Uof.IdictionaryService.GetAll(d=>d.group == group).Select(d => new { id = d.id, name = d.name, value = d.name }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 下拉选择
        /// </summary>
        /// <param name="group"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ActionResult DroplistByGroup(string group, string name)
        {
            Expression<Func<dictionary, bool>> condition = m => m.group == group;
            if (!string.IsNullOrEmpty(name))
            {
                Expression<Func<dictionary, bool>> tmp = m => (m.name.IndexOf(name) > -1);
                condition = tmp;
            }

            var list = Uof.IdictionaryService.GetAll(condition).Select(d => new { id = d.name, name = d.name, value = d.name }).ToList();

            var result = new
            {
                page = 1,
                items = list
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Add(string name, string group)
        {
            var r = Uof.IdictionaryService.AddEntity(new dictionary()
            {
                name = name,
                group = group,
                date_created = DateTime.Now,
                date_updated = DateTime.Now
            });

            return SuccessResult;
        }

        public ActionResult Get(int id)
        {
            var _dict = Uof.IdictionaryService.GetAll(a => a.id == id).FirstOrDefault();
            if (_dict == null)
            {
                return ErrorResult;
            }

            return Json(_dict, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            var d = Uof.IdictionaryService.GetById(id);

            if (d == null)
            {
                return ErrorResult;
            }
            
            var r = Uof.IdictionaryService.DeleteEntity(d);
            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update(int id, string name)
        {
            var _d = Uof.IdictionaryService.GetAll(a => a.id == id).FirstOrDefault();
            if (_d == null)
            {
                return ErrorResult;
            }
            if (_d.name == name)
            {
                return SuccessResult;
            }
            _d.name = name;

            var r = Uof.IdictionaryService.UpdateEntity(_d);
            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }
    }
}