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

            var hasOpers = new List<int>();

            if (_user.username == "admin")
            {
                int[] a = { 1, 2, 3, 4, 5 };
                hasOpers.AddRange(a);
            } else
            {
                var role = Uof.Irole_memberService.GetAll(m => m.member_id == _user.id).FirstOrDefault();
                if (role != null)
                {
                    hasOpers = Uof.Irole_operationService.GetAll(o => o.role_id == role.role_id).Select(o => o.operation_id.Value).ToList();
                }
            }

            var ops = "";
            if (hasOpers.Count > 0)
            {
                ops = string.Join(",", hasOpers);
            }

            FormsAuthentication.SetAuthCookie(string.Format("{0}|{1}|{2}|{3}|{4}", _user.id, _user.username, _user.organization_id, _user.name, ops), true);
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
                var user = Uof.ImemberService.GetAll(m => m.username == username).Select(m => new
                {
                    id = m.id,
                    username = m.username,
                    name = m.name,
                    english_name = m.username,
                    mobile = m.mobile,
                    birthday = m.birthday,
                    position = m.position.name,
                    department = m.organization.name,
                    url = m.url

                }).FirstOrDefault();

                var menus = Uof.ImenuService.GetAll().ToList();
                var opers = new List<int>();
                if (user.username == "admin")
                {
                    int[] a = { 1, 2, 3, 4, 5 };
                    opers.AddRange(a);
                    return Json(new { success = true, user = user, menus = getUserMenus(menus), opers = opers }, JsonRequestBehavior.AllowGet);
                }

                var memberMenus = new List<menu>();

                var role = Uof.Irole_memberService.GetAll(m => m.member_id == user.id).FirstOrDefault();

                if (role == null)
                {
                    return Json(new { success = true, user = user, menus = getUserMenus(memberMenus), opers = opers }, JsonRequestBehavior.AllowGet);
                }

                var hasMenus = Uof.Irole_memuService.GetAll(m => m.role_id == role.role_id).ToList();

                var hasOpers = Uof.Irole_operationService.GetAll(o => o.role_id == role.role_id).Select(o => o.operation_id.Value).ToList();

                if (hasMenus.Count() == 0)
                {
                    return Json(new { success = true, user = user, menus = getUserMenus(memberMenus), opers = hasOpers }, JsonRequestBehavior.AllowGet);
                }

                foreach (var item in hasMenus)
                {
                    var _m = menus.Where(m => m.id == item.memu_id).FirstOrDefault();
                    if (_m != null)
                    {
                        memberMenus.Add(_m);
                    }
                }
                return Json(new { success = true, user = user, menus = getUserMenus(memberMenus), opers = hasOpers }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();

            return SuccessResult;
        }

        public ActionResult ChangePwd(int userId, string old_password, string new_password)
        {
            var user = Uof.ImemberService.GetAll(m => m.id == userId).FirstOrDefault();

            var oldPassword = HashPassword.GetHashPassword(old_password);
            if (oldPassword != user.password)
            {
                return Json(new { success = false, message="旧密码不正确" }, JsonRequestBehavior.AllowGet);
            }

            var newPassword = HashPassword.GetHashPassword(new_password);

            user.password = newPassword;
            user.date_updated = DateTime.Now;

            var r = Uof.ImemberService.UpdateEntity(user);
            if (r)
            {
                FormsAuthentication.SignOut();
            }
            return Json(new { success = r, message = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Upload()
        {
            HttpFileCollectionBase files = Request.Files;

            if (files.Count <= 0)
            {
                return Json(new { result = true, url = "" }, JsonRequestBehavior.AllowGet);
            }

            var fileName = files[0].FileName;

            byte[] fileData = null;
            using (var binaryReader = new BinaryReader(files[0].InputStream))
            {
                fileData = binaryReader.ReadBytes(files[0].ContentLength);
            }
            var directory = AppDomain.CurrentDomain.BaseDirectory;
            var uploadDir = Path.Combine(directory, "Uploads");
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            // 不同业务的文件 分不同文件夹保存
            var docType = Request.Params["DocType"];
            var userId = Request.Params["UserId"];

            var folder = "image";
            var isThumbnail = false;
            var size = new Size(100, 100);
            if (!string.IsNullOrEmpty(docType))
            {
                switch (docType.ToLower())
                {
                    case "profile":
                        folder = "photo";
                        isThumbnail = true;
                        break;
                    case "image":
                        folder = "image";
                        isThumbnail = false;
                        break;
                    default:
                        folder = "doc";
                        isThumbnail = false;
                        break;
                }
            }

            var folderDir = Path.Combine(uploadDir, folder);
            if (!Directory.Exists(folderDir))
            {
                Directory.CreateDirectory(folderDir);
            }

            var uploadFile = Path.Combine(folderDir, fileName);

            // 防止重复
            if (System.IO.File.Exists(uploadFile))
            {
                var fileArr = fileName.Split('.');
                var _name = DateTime.Now.ToString("yyyyMMddHHmmss");
                fileName = _name + "." + fileArr[1];
            }

            uploadFile = Path.Combine(folderDir, fileName);
            using (FileStream fs = new FileStream(uploadFile, FileMode.Create))
            {
                fs.Write(fileData, 0, fileData.Length);
            }

            var photoUrl = string.Format("{0}://{1}:{2}/Uploads/{3}/{4}", Request.Url.Scheme, Request.Url.Host, Request.Url.Port, folder, fileName);

            if (isThumbnail)
            {
                var thumbnailDir = Path.Combine(folderDir, "thumbnail");
                if (!Directory.Exists(thumbnailDir))
                {
                    Directory.CreateDirectory(thumbnailDir);
                }

                var thumbnail = Path.Combine(thumbnailDir, fileName);

                Image image = Image.FromFile(uploadFile);
                Image thumb = image.GetThumbnailImage(size.Width, size.Height, () => false, IntPtr.Zero);
                thumb.Save(thumbnail);

                photoUrl = string.Format("{0}://{1}:{2}/Uploads/{3}/thumbnail/{4}", Request.Url.Scheme, Request.Url.Host, Request.Url.Port, folder, fileName);
            }

            int id = 0;
            int.TryParse(userId, out id);
            var user = Uof.ImemberService.GetAll(m => m.id == id).FirstOrDefault();
            if (user != null)
            {
                user.url = photoUrl;

                Uof.ImemberService.UpdateEntity(user);
            }

            return Json(new { result = true, url = photoUrl }, JsonRequestBehavior.AllowGet);
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