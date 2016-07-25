using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using System.IO;
using Common;
using System.Web.Security;
using System.Text;
using System.Net;
using System.Drawing;

namespace WebCenter.Web.Controllers
{
    public class OrganizationController : BaseController
    {
        public OrganizationController(IUnitOfWork UOF)
            : base(UOF)
        {
        }

        public ActionResult List()
        {
            var list = Uof.IorganizationService.GetAll().Select(o=>new
            {
                id = o.id,
                parent_id = o.parent_id,
                name = o.name,
                description = o.description
            }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

    }
}