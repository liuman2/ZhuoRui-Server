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

namespace WebCenter.Web.Controllers
{
    public class ReserveController : BaseController
    {
        public ReserveController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        public ActionResult List(int userId)
        {
            //Uof.icus
            return ErrorResult;
        }
    }
}