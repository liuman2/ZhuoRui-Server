using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebCenter.Entities;
using WebCenter.IServices;
using WebCenter.Web.Controllers;


namespace WebCenter.Web.Areas.Admin.Controllers
{
    public class CityController : BaseController
    {
        public CityController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

      
        /// <summary>
        /// get citys by privince
        /// </summary>
        /// <param name="province"></param>
        /// <returns></returns>
         
          [HttpGet]
        public ActionResult GetCitys(string province="")
        {
            if (string.IsNullOrEmpty(province))
            {

                var list = Uof.IcityService.GetAll().Select(it => new { id = it.Id, province = it.province, city = it.city_name }).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string prov = province.Trim();
                var list = Uof.IcityService.GetAll(item => item.province == prov).Select(it => new { id = it.Id, province = it.province, city = it.city_name }).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
           
        }

        /// <summary>
        /// Get province
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetProvinces()
        {
            var list=Uof.IcityService.GetAll().Distinct(it => it.province).Select(item => item.province).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }



 
       
        
 
    }
}