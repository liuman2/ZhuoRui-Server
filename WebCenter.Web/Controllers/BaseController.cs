using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using CacheManager.Core;
using System.Configuration;
using Common;
using Newtonsoft.Json;
using WebCenter.Web.Code;



namespace WebCenter.Web.Controllers
{
    [JsonObject(IsReference = true)]
    public class BaseController : Controller
    {

        protected ICache<object> Cache;
        protected IUnitOfWork Uof;



        public BaseController(IUnitOfWork uof)
        {
            Uof = uof;
            Cache = CacheUtil.Cache;

        }

        public ActionResult ErrorResult
        {
            get { return Json(new { result = false }, JsonRequestBehavior.AllowGet); }
        }

        public ActionResult SuccessResult
        {
            get { return Json(new { result = true }, JsonRequestBehavior.AllowGet); }
        }

        public void AddLog(string name, string descipt, string result)
        {

        }

        /// <summary>
        /// GetCompandyId
        /// </summary>
        /// <param name="project_id"></param>
        /// <returns></returns>
        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }
    }
}