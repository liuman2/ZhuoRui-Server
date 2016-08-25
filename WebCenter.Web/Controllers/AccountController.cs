using System;
using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using Common;
using System.Web.Security;
using System.Collections.Generic;
using WebCenter.Entities;

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

            FormsAuthentication.SetAuthCookie(string.Format("{0}|{1}|{2}|{3}", _user.id, _user.username, _user.organization_id, _user.name), true);
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

                var menus = Uof.ImenuService.GetAll().ToList();
                if (user.username == "admin")
                {
                    return Json(new { success = true, user = user, menus = getUserMenus(menus) }, JsonRequestBehavior.AllowGet);
                }

                var memberMenus = new List<menu>();

                var role = Uof.Irole_memberService.GetAll(m => m.member_id == user.id).FirstOrDefault();
                var hasMenus = Uof.Irole_memuService.GetAll(m => m.role_id == role.role_id).ToList();
                if (hasMenus.Count() == 0)
                {
                    return Json(new { success = true, user = user, menus = getUserMenus(memberMenus) }, JsonRequestBehavior.AllowGet);
                }

                foreach (var item in hasMenus)
                {
                    var _m = menus.Where(m => m.id == item.memu_id).FirstOrDefault();
                    if (_m != null)
                    {
                        memberMenus.Add(_m);
                    }
                }
                return Json(new { success = true, user = user, menus = getUserMenus(memberMenus) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        private List<UserMenus> getUserMenus(List<menu> ms)
        {
            if (ms.Count() == 0)
            {
                return new List<UserMenus>();
            }

            var parents = ms.Where(m => m.parent_id == 0).OrderBy(m => m.id).ToList();

            if (parents.Count() == 0)
            {
                return new List<UserMenus>();
            }

            var userMenus = new List<UserMenus>();
            foreach (var parent in parents)
            {
                var children = ms.Where(m => m.parent_id == parent.id).ToList();
                userMenus.Add(new UserMenus()
                {
                    id = parent.id,
                    parent_id = parent.parent_id,
                    route = parent.route,
                    name = parent.name,
                    icon = parent.icon,
                    children = children
                });
            }

            return userMenus;
        }
    }
}