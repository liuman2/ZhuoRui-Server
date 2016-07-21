using System;
using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebCenter.Web.Controllers
{
    [KgAuthorize]
    public class PermissionController : BaseController
    {
        public PermissionController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        [HttpGet]
        public ActionResult GetMembersForAdmin(int companyId)
        {
            var members = Uof.IuserService.GetAll(u => u.company_id == companyId && u.status == (int)ReviewStatus.Accept).Select(u => new PermissionMember
            {
                id = u.id,
                name = u.name,
                picture_url = u.picture_url,
                is_admin = u.is_admin.Value
            }).ToList();

            return Json(new { members = members }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetMembers(int companyId, int actionId)
        {
            var members = Uof.IuserService.GetAll(u => u.company_id == companyId && u.status == (int)ReviewStatus.Accept).Select( u=> new PermissionMember {
                id = u.id,
                name = u.name,
                action_id = actionId,
                picture_url = u.picture_url,
                has_right = u.is_admin == 1,
                is_admin = u.is_admin.Value
            }).ToList();

            
            if(members.Count == 0)
            {
                return Json(new { members = members, oldPermissions = new List<int>()}, JsonRequestBehavior.AllowGet);
            }

            var uids = members.Select(m => m.id).ToList();

            var pids = Uof.IpermissionService.GetAll(p => p.action_id == actionId && uids.Contains(p.user_id.Value)).Select(p => p.user_id).ToList();

            if (pids.Count() > 0)
            {
                foreach (var pid in pids)
                {
                    var _m = members.Where(m => m.id == pid).FirstOrDefault();
                    if(_m!=null)
                    {
                        _m.has_right = true;
                    }
                }
            }

            // permissionIds 拥有该action权限的用户id 一起返回  为修改时回传方便更新
            return Json(new { members = members, oldPermissions = pids }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdatePermission(PermissionRequest permissionRequest)
        {
            if (permissionRequest.old_permission_ids == null && permissionRequest.new_permission_ids == null)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            if (permissionRequest.old_permission_ids == null)
            {
                permissionRequest.old_permission_ids = new int[0];
            }
            if (permissionRequest.new_permission_ids == null)
            {
                permissionRequest.new_permission_ids = new int[0];
            }

            if (permissionRequest.old_permission_ids != null && permissionRequest.new_permission_ids != null)
            {
                var oldStr = string.Join(",", permissionRequest.old_permission_ids);
                var newStr = string.Join(",", permissionRequest.new_permission_ids);

                if (oldStr == newStr)
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
            }
            if (permissionRequest.new_permission_ids == permissionRequest.old_permission_ids)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            if (permissionRequest.old_permission_ids.Length == 0 && permissionRequest.new_permission_ids.Length == 0)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            var newMembers = new List<int>();
            var deleteMembers = new List<int>();

            if (permissionRequest.old_permission_ids.Length == 0)
            {
                newMembers = permissionRequest.new_permission_ids.OfType<int>().ToList();
            }
            else if (permissionRequest.new_permission_ids.Length == 0)
            {
                deleteMembers = permissionRequest.old_permission_ids.OfType<int>().ToList();
            }
            else
            {
                foreach (var item in permissionRequest.old_permission_ids)
                {
                    if (!permissionRequest.new_permission_ids.Contains(item))
                    {
                        deleteMembers.Add(item);
                    }
                }

                foreach (var item in permissionRequest.new_permission_ids)
                {
                    if (!permissionRequest.old_permission_ids.Contains(item))
                    {
                        newMembers.Add(item);
                    }
                }
            }

            var msgs = new List<message>();
            if (deleteMembers.Count > 0)
            {
                var olds = Uof.IpermissionService.GetAll(p => p.action_id == permissionRequest.action_id && deleteMembers.Contains(p.user_id.Value)).ToList();
                foreach (var item in olds)
                {
                    Uof.IpermissionService.DeleteEntity(item);
                    msgs.Add(new message()
                    {
                        content = string.Format("{0}的权限被管理员收回来", (permissionRequest.action_id == 0 ? "创建项目" : "发布公告")),
                        date_created = DateTime.Now,
                        type = (int)MessageType.Permission,
                        user_id = item.user_id
                    });
                }
            }

            if (newMembers.Count > 0)
            {
                var currentUser = HttpContext.User.Identity as UserIdentity;

                var ps = new List<permission>();
                var count = newMembers.Count;
                foreach (var item in newMembers)
                {
                    if (item == currentUser.id)
                    {
                        count = count - 1;
                        continue;
                    }

                    var p = new permission()
                    {
                        action_id = permissionRequest.action_id,
                        user_id = item
                    };
                    ps.Add(p);
                                        
                    msgs.Add(new message()
                    {
                        content = string.Format("可以{0}了,你被授予了权限", (permissionRequest.action_id == 0 ? "创建项目" : "发布公告")),
                        date_created = DateTime.Now,
                        type = (int)MessageType.Permission,
                        user_id = item
                    });
                }
                Uof.IpermissionService.AddEntities(ps);

                var _user = Uof.IuserService.GetAll(u => newMembers.Contains(u.id) && u.id != currentUser.id).OrderBy(u => u.name).FirstOrDefault();
                if (_user != null)
                {                   
                    msgs.Add(new message()
                    {
                        content = string.Format("你授予了{0}{1}{2}的权限", _user.name, (count > 1 ? "等" + count + "人" : ""), (permissionRequest.action_id == 0 ? "创建项目" : "发布公告")),
                        date_created = DateTime.Now,
                        type = (int)MessageType.Permission,
                        user_id = currentUser.id
                    });
                }
            }

            if (msgs.Count > 0)
            {
                Uof.ImessageService.AddEntities(msgs);
            }

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SetCompanyAdmin(int company_id, int admin_id)
        {
            var oldAdmin = Uof.IuserService.GetAll(u => u.company_id == company_id && u.is_admin == 1).FirstOrDefault();

            if (oldAdmin != null && oldAdmin.id == admin_id)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            if (oldAdmin != null && oldAdmin.id != admin_id)
            {
                oldAdmin.is_admin = 0;
            }

            var newAdmin = Uof.IuserService.GetById(admin_id);
            newAdmin.is_admin = 1;

            var users = new List<user>();
            users.Add(newAdmin);
            if (oldAdmin != null)
            {
                users.Add(oldAdmin);
            }

            var r = Uof.IuserService.UpdateEntities(users);

            return Json(new { success = r, message = r ? "" : "更新失败" }, JsonRequestBehavior.AllowGet);
        }
    }
}