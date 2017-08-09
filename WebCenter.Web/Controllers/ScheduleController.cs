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
    public class ScheduleController : BaseController
    {
        public ScheduleController(IUnitOfWork UOF)
            : base(UOF)
        {

        }


    }
}