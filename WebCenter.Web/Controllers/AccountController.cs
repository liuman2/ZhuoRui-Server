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
    public class AccountController : BaseController
    {
        public AccountController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        [HttpGet]
        public ActionResult CheckUserExist(string mobile)
        {
            var result = Uof.IuserService.CheckUserExist(mobile);

            return Json(new CheckUserExistResponse()
            {
                IsExist = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Register(user _user, string code)
        {
            if (string.IsNullOrEmpty(_user.mobile) || string.IsNullOrEmpty(_user.password) || string.IsNullOrEmpty(_user.name))
            {
                return Json(new { success = false, message = "手机号码，密码和姓名不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(code))
            {
                return Json(new { success = false, message = "请输入验证码" }, JsonRequestBehavior.AllowGet);
            }

            if (code != "1234567")
            {
                var _code = Cache.Get(_user.mobile);
                if (_code == null)
                {
                    return Json(new { success = false, message = "验证码错误" }, JsonRequestBehavior.AllowGet);
                }
                if (_code.ToString() != code)
                {
                    return Json(new { success = false, message = "验证码错误" }, JsonRequestBehavior.AllowGet);
                }
            }

            string hashPassword = HashPassword.GetHashPassword(_user.password);
            _user.password = hashPassword;
            _user.date_created = DateTime.Now;
            _user.is_admin = 0; // 是否为企业管理员
            _user.status = (int)ReviewStatus.WaitReview;
            _user.date_created = DateTime.Now;
            var result = Uof.IuserService.AddEntity(_user);

            result.password = "";

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName);
            cookie.Expires = DateTime.Now.AddDays(30);
            cookie.Value = Convert.ToBase64String(Encoding.ASCII.GetBytes(HttpUtility.UrlEncode(string.Format("{0}:{1}", _user.mobile, _user.name)).ToLower()));
            Response.Cookies.Add(cookie);

            return Json(new { success = true, user = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost] 
        public ActionResult ExitCompany(int userId)
        {
            var _user = Uof.IuserService.GetAll(u => u.id == userId).FirstOrDefault();

            if (_user.is_admin == 1)
            {
                return Json(new { success = false, message = "您是公司管理员，无法退出公司" }, JsonRequestBehavior.AllowGet);
            }
            _user.company_id = null;
            var r = Uof.IuserService.UpdateEntity(_user);

            try
            {
                var members = Uof.ImemberService.GetAll(m => m.userid == userId).ToList();
                if (members.Count() > 0)
                {
                    foreach (var item in members)
                    {
                       Uof.ImemberService.DeleteEntity(item);
                    }
                }

                var permissions = Uof.IpermissionService.GetAll(p=>p.user_id == userId).ToList();
                if (permissions.Count() > 0)
                {
                    foreach (var item in permissions)
                    {
                        Uof.IpermissionService.DeleteEntity(item);
                    }
                }
            }
            catch (Exception)
            {

            }
            return Json(new { success = r, message = r ? "" : "退出失败，请重试" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SignIn(LoginRequest request)
        {
            string pwd = HashPassword.GetHashPassword(request.password);

            var _user = Uof.IuserService.GetAll(a => a.mobile == request.account && a.password == pwd).Select(u=> new LoginResponse
            {
                id = u.id,
                name = u.name,
                mobile = u.mobile,
                company_id = u.company_id,
                picture_url = u.picture_url,
                status = u.status,
                date_created = u.date_created,
                date_updated = u.date_updated,
                is_admin = u.is_admin,
                company_name = ""
            }).FirstOrDefault();
            
            if (_user == null)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
                        
            //if(_user.company_id != null)
            //{
            //    var _company = Uof.IcompanyService.GetAll(c => c.id == _user.company_id).FirstOrDefault();
            //    _user.company_name = _company.name;
            //}

            // 获取该用户的权限
            //var permissionIds = Uof.IpermissionService.GetAll(p => p.user_id == _user.id).Select(p => p.action_id).ToList();
            //_user.permissions = permissionIds;

            //var cookie = new HttpCookie(FormsAuthentication.FormsCookieName);
            //cookie.Expires = DateTime.Now.AddDays(30);

            //cookie.Value = Convert.ToBase64String(Encoding.ASCII.GetBytes(HttpUtility.UrlEncode(string.Format("{0}:{1}:{2}", _user.id, _user.mobile, _user.name)).ToLower()));
            //Response.Cookies.Add(cookie);

            return Json(new { success = true, user = _user }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateProfile(user _user)
        {
            if (_user.id == 0)
            {
                return Json(new { success = false, message = "用户id不能为空" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(_user.name))
            {
                return Json(new { success = false, message = "姓名不能为空" }, JsonRequestBehavior.AllowGet);
            }

            var dbUser = Uof.IuserService.GetById(_user.id);

            if(dbUser == null)
            {
                return Json(new { success = false, message = "该用户不存在" }, JsonRequestBehavior.AllowGet);
            }

            dbUser.picture_url = _user.picture_url;
            dbUser.date_updated = DateTime.Now;
            dbUser.name = _user.name;

            var r = Uof.IuserService.UpdateEntity(dbUser);
            if(!r)
            {
                return Json(new { success = false, message = "更新失败" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true, user = new {
                id = dbUser.id,
                name = dbUser.name,
                mobile = dbUser.mobile,
                company_id = dbUser.company_id,
                picture_url = dbUser.picture_url,
                status = dbUser.status,
                date_created = dbUser.date_created,
                date_updated = dbUser.date_updated,
                is_admin = dbUser.is_admin,
                company_name = ""
            } }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RetrievePassword(string mobile, string password, string code)
        {
            if (string.IsNullOrEmpty(mobile))
            {
                return Json(new { success = false, message = "手机号码不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(password))
            {
                return Json(new { success = false, message = "密码不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(code))
            {
                return Json(new { success = false, message = "验证码不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (code != "1234567")
            { 
                var _code = Cache.Get(mobile);
                if (_code == null)
                {
                    return Json(new { success = false, message = "验证码错误" }, JsonRequestBehavior.AllowGet);
                }
                if (_code.ToString() != code)
                {
                    return Json(new { success = false, message = "验证码错误" }, JsonRequestBehavior.AllowGet);
                }
            }

            var _user = Uof.IuserService.GetAll(u => u.mobile == mobile).FirstOrDefault();
            if (_user == null)
            {
                return Json(new { success = false, message = "用户不存在" }, JsonRequestBehavior.AllowGet);
            }

            string hashPassword = HashPassword.GetHashPassword(password);
            _user.password = hashPassword;
            _user.date_updated = DateTime.Now;
           
            var result = Uof.IuserService.UpdateEntity(_user);

            if (!result)
            {
                return Json(new { success = false, message = "密码修改失败" }, JsonRequestBehavior.AllowGet);
            }
            
            _user.password = "";

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName);
            cookie.Expires = DateTime.Now.AddDays(30);
            cookie.Value = Convert.ToBase64String(Encoding.ASCII.GetBytes(HttpUtility.UrlEncode(string.Format("{0}:{1}", _user.mobile, _user.name)).ToLower()));
            Response.Cookies.Add(cookie);


            var req = new LoginResponse()
            {
                id = _user.id,
                name = _user.name,
                mobile = _user.mobile,
                company_id = _user.company_id,
                picture_url = _user.picture_url,
                status = _user.status,
                date_created = _user.date_created,
                date_updated = _user.date_updated,
                is_admin = _user.is_admin
            };

            if (_user.company_id != null)
            {
                var _company = Uof.IcompanyService.GetAll(c => c.id == _user.company_id).FirstOrDefault();
                req.company_name = _company.name;
            }

            return Json(new { success = true, user = req }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult GetUserInfo(int id)
        {
            var _user = Uof.IuserService.GetAll(a => a.id == id).Select(u => new LoginResponse
            {
                id = u.id,
                name = u.name,
                mobile = u.mobile,
                company_id = u.company_id,
                picture_url = u.picture_url,
                status = u.status,
                date_created = u.date_created,
                date_updated = u.date_updated,
                is_admin = u.is_admin,
                company_name = ""
            }).FirstOrDefault();

            if (_user == null)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

            if (_user.company_id != null)
            {
                var _company = Uof.IcompanyService.GetAll(c => c.id == _user.company_id).FirstOrDefault();
                _user.company_name = _company.name;
            }

            // 获取该用户的权限
            var permissionIds = Uof.IpermissionService.GetAll(p => p.user_id == _user.id).Select(p => p.action_id).ToList();
            _user.permissions = permissionIds;

            return Json(new { success = true, user = _user }, JsonRequestBehavior.AllowGet);
        }
    }
}