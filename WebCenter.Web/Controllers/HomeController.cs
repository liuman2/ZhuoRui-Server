using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using System.Web.Security;

namespace WebCenter.Web.Controllers
{
    public class HomeController : BaseController
    {


        public HomeController(IUnitOfWork UOF)
            : base(UOF)
        {
        }
        public ActionResult Index()
        {
            return null;
            for (var i = 0; i < 300;i++ )
            {
           // IList<company> list = Uof.IcompanyService.GetAll().OrderByDescending(item=>item.Id).Skip(200).Take(50).ToList();
                Uof.IcompanyService.AddEntity(new company() { 
                 address="sss",
                  city_id=1,
                   company_image_path="sss.jpg",
                    contact_name="peisheng",
                     name="test"
                 });
                Uof.Iproject_caseService.AddEntity(new project_case() { 
                 descript="sssss",
                  content="contents",
                   create_time=DateTime.Now,
                    title="titless"
                });

                Uof.IuserService.AddEntity(new user()
                {
                    email="ilvoemeyou20@ass.com",
                     user_name="ilove"+i.ToString(),
                      password="",
                       is_admin=1
                });

                Uof.Isys_dictionaryService.AddEntity(new sys_dictionary()
                {
                      name="name"+i.ToString(),
                       value="value"+i.ToString()
                });

                Uof.Ioperate_logService.AddEntity(new operate_log()
                { desript="log",
                     name=@"sss
                     ",
                       result="Ok"
                      
                     
                });
                
            }
           
            return View();
        }
        [HttpPost]
        public ActionResult Login(string userName, string password)
        {
            FormsAuthentication.SetAuthCookie(userName, true);
            return null;
        }
    }
}