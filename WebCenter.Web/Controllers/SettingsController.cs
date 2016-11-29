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

        public ActionResult GetPeriod()
        {
            var param = Uof.IsettingService.GetAll(s => s.name == "PATENT_PERIOD" || s.name == "TRADEMARK_PERIOD").ToList();
            var obj = new ParamSetting();
            obj.patent_period = "10";
            obj.trademark_period = "10";
            if (param.Count > 0)
            {
                obj.patent_period = param.Where(s => s.name == "PATENT_PERIOD").Select(s => s.value).FirstOrDefault();
                obj.trademark_period = param.Where(s => s.name == "TRADEMARK_PERIOD").Select(s => s.value).FirstOrDefault();
            }

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PeriodUpdate(List<ParamSetting> paramList)
        {
            foreach (var item in paramList)
            {

            }
            var patentPeriod = Uof.IsettingService.GetAll(s => s.name == "PATENT_PERIOD").FirstOrDefault();
            var trademarkPeriod = Uof.IsettingService.GetAll(s => s.name == "TRADEMARK_PERIOD").FirstOrDefault();

            patentPeriod.value = param.patent_period;
            trademarkPeriod.value = param.trademark_period;

            var ss = new List<setting>();
            ss.Add(patentPeriod);
            ss.Add(trademarkPeriod);
            var r = Uof.IsettingService.UpdateEntities(ss);

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