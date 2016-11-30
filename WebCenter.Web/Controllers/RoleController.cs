using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;

namespace WebCenter.Web.Controllers
{
    public class RoleController : BaseController
    {
        public RoleController(IUnitOfWork UOF)
            : base(UOF)
        {
        }

        public ActionResult List()
        {
            var list = Uof.IroleService.GetAll().Select(a => new
            {
                id = a.id,
                name = a.name,
                is_system = a.is_system,
                description = a.description
            }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Tree()
        {
            var list = Uof.IroleService.GetAll().Select(d => new { id = d.id, name = d.name, parent_id = 0 }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search(int index = 1, int size = 10, string name = "")
        {
            Expression<Func<role, bool>> condition = m => true;
            if (!string.IsNullOrEmpty(name))
            {
                condition = m => (m.name.IndexOf(name) > -1);
            }

            var list = Uof.IroleService.GetAll(condition).OrderBy(item => item.id).Select(m => new
            {
                id = m.id,
                name = m.name
            }).ToPagedList(index, size).ToList();

            var totalRecord = Uof.IroleService.GetAll(condition).Count();

            var totalPages = 0;
            if (totalRecord > 0)
            {
                totalPages = (totalRecord + size - 1) / size;
            }
            var page = new
            {
                current_index = index,
                current_size = size,
                total_size = totalRecord,
                total_page = totalPages
            };

            var result = new
            {
                page = page,
                items = list
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get(int id)
        {
            var _role = Uof.IroleService.GetAll(a => a.id == id).FirstOrDefault();
            if (_role == null)
            {
                return ErrorResult;
            }

            return Json(_role, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Add(string name, string description)
        {
            var r = Uof.IroleService.AddEntity(new role()
            {
                name = name,
                is_system = 0,
                code = "0",
                description = description
            });

            return SuccessResult;
        }

        [HttpPost]
        public ActionResult Update(int id, string name, string description)
        {
            var _role = Uof.IroleService.GetAll(a=>a.id == id).FirstOrDefault();
            if (_role == null)
            {
                return ErrorResult;
            }
            if (_role.name == name && _role.description == description)
            {
                return SuccessResult;
            }
            _role.name = name;
            _role.description = description;

            var r = Uof.IroleService.UpdateEntity(_role);
            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            var _role = Uof.IroleService.GetAll(a => a.id == id).FirstOrDefault();
            if (_role == null)
            {
                return ErrorResult;
            }

            if (_role.is_system == 1)
            {
                return Json(new { success = false, message = "系统默认角色，不可删除" }, JsonRequestBehavior.AllowGet);
            }

            var r = Uof.IroleService.DeleteEntity(_role);
            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMenuByRoleId(int roleId)
        {
            // 菜单
            var roleMenus = Uof.Irole_memuService.GetAll(m => m.role_id == roleId).Select(m => m.memu_id).ToList();

            var menus = Uof.ImenuService.GetAll().OrderBy(m => m.order).Select(m => new RoleMenus {
                id = m.id,
                check = false,
                icon = m.icon,
                name = m.name,
                parent_id = m.parent_id,
                route = m.route
            }).ToList();

            if (roleMenus.Count() > 0)
            {
                foreach (var item in roleMenus)
                {
                   var hasMenu =  menus.Where(m => m.id == item).FirstOrDefault();
                    if (hasMenu != null)
                    {
                        hasMenu.check = true;
                    }
                }
            }

            // 操作
            var roleOpers = Uof.Irole_operationService.GetAll(o => o.role_id == roleId).Select(o => o.operation_id).ToList();

            var opers = Uof.IoperationService.GetAll().Select(o => new RoleOpers
            {
                id = o.id,
                check = false,
                name = o.name,
                parent_id = 0
            }).ToList();

            if (roleOpers.Count() > 0)
            {
                foreach (var item in roleOpers)
                {
                    var hasOperu = opers.Where(o => o.id == item).FirstOrDefault();
                    if (hasOperu != null)
                    {
                        hasOperu.check = true;
                    }
                }
            }

            var perms = new RolePermission()
            {
                menus = menus,
                opers = opers
            };

            return Json(perms, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMemberByRoleId(int roleId)
        {
            var roleMembers = Uof.Irole_memberService.GetAll(m => m.role_id == roleId).Select(m => m.member_id).ToList();

            var allMembers = Uof.ImemberService.GetAll().Select(m => new RoleMembers()
            {
                id = m.id,
                name = m.name,
                english_name = m.english_name,
                department = m.organization.name,
                area = m.area.name
            }).ToList();

            var _roleMembers = new List<RoleMembers>();
            if (roleMembers.Count() > 0)
            {
                foreach (var item in roleMembers)
                {
                    var hasMember = allMembers.Where(m => m.id == item).FirstOrDefault();
                    if (hasMember != null)
                    {
                        _roleMembers.Add(hasMember);
                    }
                }
            }

            return Json(_roleMembers, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveRoleMenu(int roleId, int[] menuIds, int[] operIds)
        {
            if (menuIds == null)
            {
                menuIds = new int[0];
            }

            if (operIds == null)
            {
                operIds = new int[0];
            }

            #region 菜单
            var oldMenus = Uof.Irole_memuService.GetAll(m => m.role_id == roleId).ToList();
            var addMenus = new List<role_memu>();

            if (oldMenus.Count() == 0)
            {
                foreach (var item in menuIds)
                {
                    addMenus.Add(new role_memu()
                    {
                        role_id = roleId,
                        memu_id = item
                    });
                }

                if (addMenus.Count() > 0)
                {
                    Uof.Irole_memuService.AddEntities(addMenus);
                }
            }
            else
            {
                var deleteMenus = new List<role_memu>();
                foreach (var item in oldMenus)
                {
                    var exist = menuIds.Where(m => m == item.memu_id);
                    if (exist.Count() == 0)
                    {
                        deleteMenus.Add(item);
                    }
                }

                foreach (var item in menuIds)
                {
                    var exist = oldMenus.Where(o => o.memu_id == item);
                    if (exist.Count() == 0)
                    {
                        addMenus.Add(new role_memu()
                        {
                            memu_id = item,
                            role_id = roleId
                        });
                    }
                }

                if (deleteMenus.Count() > 0)
                {
                    foreach (var delete in deleteMenus)
                    {
                        Uof.Irole_memuService.DeleteEntity(delete);
                    }
                }

                if (addMenus.Count() > 0)
                {
                    Uof.Irole_memuService.AddEntities(addMenus);
                }
            }
            #endregion

            #region 操作
            var oldOpers = Uof.Irole_operationService.GetAll(o => o.role_id == roleId).ToList();
            var addOpers = new List<role_operation>();
            if (addOpers.Count() == 0)
            {
                foreach (var item in operIds)
                {
                    addOpers.Add(new role_operation()
                    {
                        role_id = roleId,
                        operation_id = item
                        
                    });
                }

                if (addOpers.Count() > 0)
                {
                    Uof.Irole_operationService.AddEntities(addOpers);
                }
            }
            else
            {
                var deleteOpers = new List<role_operation>();
                foreach (var item in oldOpers)
                {
                    var exist = operIds.Where(m => m == item.operation_id);
                    if (exist.Count() == 0)
                    {
                        deleteOpers.Add(item);
                    }
                }

                foreach (var item in operIds)
                {
                    var exist = oldOpers.Where(o => o.operation_id == item);
                    if (exist.Count() == 0)
                    {
                        addOpers.Add(new role_operation()
                        {
                            operation_id = item,
                            role_id = roleId
                        });
                    }
                }

                if (deleteOpers.Count() > 0)
                {
                    foreach (var delete in deleteOpers)
                    {
                        Uof.Irole_operationService.DeleteEntity(delete);
                    }
                }

                if (addOpers.Count() > 0)
                {
                    Uof.Irole_operationService.AddEntities(addOpers);
                }
            }
            #endregion

            return SuccessResult;
        }

        [HttpPost]
        public ActionResult SaveRoleMember(int roleId, int[] memberIds)
        {

            var oldMembers = Uof.Irole_memberService.GetAll(m => m.role_id == roleId).ToList();
            var adds = new List<role_member>();

            if (oldMembers.Count() == 0)
            {
                foreach (var item in memberIds)
                {
                    adds.Add(new role_member()
                    {
                        role_id = roleId,
                        member_id = item
                    });
                }

                Uof.Irole_memberService.AddEntities(adds);
                return SuccessResult;
            }
                        
            foreach (var item in memberIds)
            {
                var exist = oldMembers.Where(o => o.member_id == item);
                if (exist.Count() == 0)
                {
                    adds.Add(new role_member()
                    {
                        member_id = item,
                        role_id = roleId
                    });
                }
            }
            
            if (adds.Count() > 0)
            {
                Uof.Irole_memberService.AddEntities(adds);
            }

            return SuccessResult;
        }

        public ActionResult DeleteRoleMember(int roleId, int userId)
        {
            var roleMember = Uof.Irole_memberService.GetAll(m => m.member_id == userId && m.role_id == roleId).FirstOrDefault();

            if (roleMember == null)
            {
                return ErrorResult;
            }

            var r = Uof.Irole_memberService.DeleteEntity(roleMember);

            return Json(new { success = r, message = r ? "" : "删除失败" }, JsonRequestBehavior.AllowGet);
        }
    }
}