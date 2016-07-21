using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using System.IO;
using Common;

namespace WebCenter.Web.Controllers
{
    public class UserController : BaseController
    {
        public UserController(IUnitOfWork UOF)
            : base(UOF)
        {

        }
       

        
    }
}