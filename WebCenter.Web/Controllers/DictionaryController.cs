using System;
using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;

namespace WebCenter.Web.Controllers
{
    public class DictionaryController : BaseController
    {
        public DictionaryController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        public ActionResult GetDictionaryByGroup(string group)
        {
            var list = Uof.IdictionaryService.GetAll(d => d.group == group).Select(d=> new { value = d.value, text = d.name }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}