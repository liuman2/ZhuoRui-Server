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
    public class OperateController : BaseController
    {
        public OperateController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        /// <summary>
        /// 操作记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Clear(int type=0)
        {
            //清空
            if (type == 0)
            {
                int i = Uof.Ioperate_logService.DeleteEntity(item => item.Id > 0);

                if (i > 0)
                {
                    AddLog("清空日志", "清空日志", "成功");
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                }
                   
            }//清空一个月前的
            else if (type == 1)
            {
                DateTime dt = DateTime.Now.AddMonths(-1);
                int i = Uof.Ioperate_logService.DeleteEntity(item =>item.create_time.HasValue&& item.create_time< dt);
                if (i > 0)
                {
                    AddLog("清空一个月前的日志", "清空一个月前的日志", "成功");
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                }
                  
            }
            AddLog("清空日志", "清空日志", "失败");
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 记录列表
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        
        public ActionResult List(int page_index = 1, int page_size = 20)
        {
            PagedList<operate_log> list = Uof.Ioperate_logService.GetAll().OrderByDescending(item => item.Id).ToPagedList(page_index, page_size);
            if (list != null)
            {
                ArrayList al = new ArrayList();
                foreach (var item in list)
                {
                    var obj = new
                    {
                        id = item.Id,
                        name = item.name,
                        descript = item.desript,
                        user_name = item.user_name,
                        user_id = item.user_id,
                        result = item.result,
                        create_time = item.create_time.GetValueOrDefault(new DateTime(2000,01,01)).ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    al.Add(obj);
                }
                var retObj = new
                {
                    total_count = list.TotalCount,
                    curent_page = page_index,
                    page_size = page_size,
                    items = al

                };
                return Json(retObj, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }

        }



    }
}