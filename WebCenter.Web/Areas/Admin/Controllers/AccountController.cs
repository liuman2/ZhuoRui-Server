using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebCenter.Entities;
using WebCenter.IServices;
using WebCenter.Web.Controllers;

namespace WebCenter.Web.Areas.Admin.Controllers
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
                    username=username,
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
          [Authorize]
        [HttpPost]
        public ActionResult changePassword(string oldPwd,string newPwd)
        {
            string curentUser = ControllerContext.HttpContext.User.Identity.Name;
            string hashPassword = HashPassword.GetHashPassword(oldPwd);
            string newPassword = HashPassword.GetHashPassword(newPwd);
            sys_user _user = Uof.Isys_userService.GetAll(item => item.user_name == curentUser && item.password == hashPassword).FirstOrDefault();
            if (_user != null||string.IsNullOrEmpty(curentUser))
            {
                _user.password = newPassword;
                bool b = Uof.Isys_userService.UpdateEntity(_user);
                var ret = new { result = b };
                if (b)
                {
                    FormsAuthentication.SignOut();
                }
                AddLog("修改密码","修改密码","成功");
                return Json(ret);
            }
            else
            {
                AddLog("修改密码", "修改密码", "失败");
                return Json(new { result=false});
            }


        }

        /// <summary>
        /// 判断是否登录
        /// </summary>
        /// <returns></returns>
        public ActionResult CheckLogin()
        {
            bool b = ControllerContext.HttpContext.User.Identity.IsAuthenticated;

            return Json(new {result=b},JsonRequestBehavior.AllowGet);

        }


        [HttpGet]
        
        public ActionResult Logout()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            return Json(new { result=true},JsonRequestBehavior.AllowGet);
        }
        //需要授权才可访问
        [Authorize]
        public ActionResult someAction()
        {
            return Content("");
        }
    }
}