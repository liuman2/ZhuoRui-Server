using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;
using System.Web.Security;

namespace WebCenter.Web.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        [HttpPost]
        public ActionResult Login(string username, string password, string return_url)
        {
            string hashPassword = HashPassword.GetHashPassword(password);
            sys_user _user = Uof.Isys_userService.GetAll(item => item.user_name == username && item.password == hashPassword).FirstOrDefault();

            if (_user != null)
            {
                FormsAuthentication.SetAuthCookie(username, true);
                Session["UserName"] = username;
                var url = "";
                if (return_url.Trim().Length != 0)
                {
                    url = return_url;
                }
                else
                {
                    url = "/index.html";
                }
                var returnObj = new
                {
                    message = "",
                    result = true,
                    url = url
                };
                return Json(returnObj);
            }
            else
            {
                var returnObj = new
                {
                    message = "用户名或者密码错误",
                    result = false,
                    url = ""
                };
                return Json(returnObj);
                
            }

        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            return Redirect("/index.html");
        }
        //需要授权才可访问
        [Authorize]
        public ActionResult someAction()
        {
            return Content("");
        }
    }
}