using System;
using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using Common;
using System.Web.Security;
using System.Collections.Generic;
using WebCenter.Entities;
using System.IO;
using System.Drawing;
using System.Web;

namespace WebCenter.Web.Controllers
{
    public class AttachmentController : BaseController
    {
        public AttachmentController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        [HttpPost]
        public ActionResult Add(attachment attach)
        {
            var r = Uof.IattachmentService.AddEntity(attach);

            return SuccessResult;
        }

        public ActionResult List(int source_id, string source_name)
        {
            var list = Uof.IattachmentService.GetAll(a => a.source_id == source_id && a.source_name == source_name).Select(a => new
            {
                id = a.id,
                source_id = a.source_id,
                source_name = a.source_name,
                name = a.name,
                attachment_url = a.attachment_url,
                description = a.description,
                date_created = a.date_created,
            }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            var attach = Uof.IattachmentService.GetAll(a => a.id == id).FirstOrDefault();
            Uof.IattachmentService.DeleteEntity(attach);
            return SuccessResult;
        }
    }
}