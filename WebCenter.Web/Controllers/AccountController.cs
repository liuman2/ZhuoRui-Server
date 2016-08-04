﻿using System;
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

        [HttpPost]
        public ActionResult SignIn(string username, string password)
        {
            string pwd = HashPassword.GetHashPassword(password);

            var _user = Uof.ImemberService.GetAll(a => a.username == username && a.password == pwd).Select(u => new
            {
                id = u.id,
                name = u.name,
                username = u.username,
                organization_id = u.organization_id
            }).FirstOrDefault();

            if (_user == null)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

            FormsAuthentication.SetAuthCookie(string.Format("{0}|{1}|{2}", _user.id, _user.username, _user.organization_id), true);
            Session["UserName"] = username;

            return Json(new { success = true, user = _user }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetProfile()
        {
            var r = HttpContext.User.Identity.IsAuthenticated;
            if (!r)
            {
                return new HttpUnauthorizedResult();
            }

            try
            {
                var identityName = HttpContext.User.Identity.Name;
                var arrs = identityName.Split('|');
                if (arrs.Length == 0)
                {
                    return new HttpUnauthorizedResult();
                }
                if (arrs.Length < 3)
                {
                    return new HttpUnauthorizedResult();
                }
                var username = arrs[1];
                var user = Uof.ImemberService.GetAll(m => m.username == username).FirstOrDefault();
                user.password = "";
                return Json(new { success = true, user = user }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}