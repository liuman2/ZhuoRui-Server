using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using System.IO;
using Common;
using System.Collections;

namespace WebCenter.Web.Controllers
{
    public class SignformController : BaseController
    {
        public SignformController(IUnitOfWork UOF)
            : base(UOF)
        {
        }

        [HttpGet]
        public ActionResult GetSignList(int projectId)
        {
            var list = Uof.IsignedformService.GetAll(p => p.project_id == projectId && p.status == (sbyte)LineStatus.OK && p.review == (sbyte)ReviewStatus.Accept).
                OrderByDescending(p => p.id).Select(p => new
                {
                    amount = p.amount,
                    date_signed = p.date_signed,
                    creator = p.creator,
                    date_created = p.date_created,
                    bill_url=p.bill_url,
                    code=p.code
                }).ToList();
            var ids = list.Select(p=>p.creator).ToList();
            var users = Uof.IuserService.GetAll(p => ids.Contains(p.id)).Select(p => new { 
             id=p.id,
             name=p.name
            }).ToList();
            ArrayList al = new ArrayList();
            foreach(var item in list)
            {
                var obj = new
                {
                    amount = item.amount,
                    date_signed = item.date_signed.GetValueOrDefault().ToString("yyyy年MM月dd日"),
                    date_created = item.date_created.GetValueOrDefault().ToString("yyyy年MM月dd日"),
                    user_name = users.Where(p => p.id == item.creator).FirstOrDefault().name,
                    bill_url=item.bill_url,
                    code=item.code
                };
                al.Add(obj);
            }
            return Json(al, JsonRequestBehavior.AllowGet);
        } 
    }
}