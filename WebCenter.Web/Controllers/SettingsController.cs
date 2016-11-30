using System;
using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using Common;
using System.Web.Security;
using Newtonsoft.Json;
using System.Collections.Generic;
using WebCenter.Entities;

namespace WebCenter.Web.Controllers
{
    public class SettingsController : BaseController
    {
        public SettingsController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        public ActionResult Get()
        {
            var coding = Uof.IsettingService.GetAll(s => s.name == "CODING").FirstOrDefault();

            var codingObj = JsonConvert.DeserializeObject<Coding>(coding.value);

            var areaCode = codingObj.customer.area_code;

            var areas = Uof.IareaService.GetAll().ToList();

            foreach (var item in areas)
            {
                var ac = areaCode.Where(a => a.id == item.id).FirstOrDefault();
                if (ac == null)
                {
                    areaCode.Add(new AreaCoding
                    {
                        id = item.id,
                        name = item.name,
                        value = ""
                    });
                } else
                {
                    ac.name = item.name;
                }
            }

            return Json(codingObj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetParams()
        {
            var param = Uof.IsettingService.GetAll(s => s.name != "CODING").Select(s=> new ParamSetting
            {
                name = s.name,
                value = s.value,
                memo = s.memo
            }).ToList();
            
            return Json(param, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ParamUpdate(List<ParamSetting> paramList)
        {
            var keys = paramList.Select(p => p.name).ToList();
            var dbSettings = Uof.IsettingService.GetAll(s => keys.Contains(s.name)).ToList();

            //var trademarkPeriod = Uof.IsettingService.GetAll(s => s.name == "TRADEMARK_PERIOD").FirstOrDefault();

            foreach (var item in dbSettings)
            {
                var param = paramList.Where(p => p.name == item.name).FirstOrDefault();
                if (param != null)
                {
                    item.value = param.value;
                    item.memo = param.memo;
                }
            }
            var r = Uof.IsettingService.UpdateEntities(dbSettings);
            return Json(new { success = r, message = r ? "" : "保存失败" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update(Coding codes)
        {
            var v = JsonConvert.SerializeObject(codes);

            var coding = Uof.IsettingService.GetAll(s => s.name == "CODING").FirstOrDefault();

            if (coding == null)
            {
                return Json(new { success = false, message = "保存失败" }, JsonRequestBehavior.AllowGet);
            }

            coding.value = v;

            var r = Uof.IsettingService.UpdateEntity(coding);

            return Json(new { success = r, message = r ? "" : "保存失败" }, JsonRequestBehavior.AllowGet);
        }
    }
}