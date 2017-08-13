using System;
using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;
using System.Linq.Expressions;
using System.Collections;

namespace WebCenter.Web.Controllers
{
    public class MemberController : BaseController
    {
        public MemberController(IUnitOfWork UOF)
            : base(UOF)
        {
        }

        public ActionResult List(int index = 1, int size = 10, string name = "")
        {
            Expression<Func<member, bool>> condition = m => true;
            if (!string.IsNullOrEmpty(name))
            {
                condition = m => (m.name.IndexOf(name) > -1);
            }

            var list = Uof.ImemberService.GetAll(condition).Where(m => m.username != "admin").OrderByDescending(item => item.id).Select(m => new
            {
                id = m.id,
                name = m.name,
                text = m.name,
                english_name = m.english_name,
                username = m.username,
                department = m.organization.name,
                area = m.area.name,
                position = m.position.name,
                status = m.status
            }).ToPagedList(index, size).ToList();

            var totalRecord = Uof.ImemberService.GetAll(condition).Count();

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

        public ActionResult GetMembersForRole(int index = 1, int size = 500, string name = "")
        {
            var userIds = Uof.Irole_memberService.GetAll().Select(m => m.member_id).ToList();

            Expression<Func<member, bool>> condition = m => true;
            if (!string.IsNullOrEmpty(name))
            {
                condition = m => (m.name.IndexOf(name) > -1 && m.username != "admin");
            }

            Expression<Func<member, bool>> excludeIds = m => true;
            if (userIds.Count() > 0)
            {
                excludeIds = m => !userIds.Contains(m.id);
            }

            var list = Uof.ImemberService.GetAll(condition).Where(excludeIds).Where(m => m.username != "admin").OrderByDescending(item => item.id).Select(m => new
            {
                id = m.id,
                name = m.name,
                english_name = m.english_name,
                username = m.username,
                department = m.organization.name,
                area = m.area.name,
                position = m.position.name,
                status = m.status
            }).ToPagedList(index, size).ToList();

            var totalRecord = Uof.ImemberService.GetAll(condition).Count();

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
            var _member = Uof.ImemberService.GetAll(a => a.id == id).Select(m => new
            {
                id = m.id,
                name = m.name,
                english_name = m.english_name,
                username = m.username,
                mobile = m.mobile,
                birthday = m.birthday,
                organization_id = m.organization_id,
                organization_name = m.organization.name,

                area_id = m.area_id,
                area_name = m.area.name,

                position_id = m.position_id,
                position_name = m.position.name,

                hiredate = m.hiredate,
                status = m.status

            }).FirstOrDefault();
            if (_member == null)
            {
                return ErrorResult;
            }

            return Json(_member, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Add(member _member)
        {
            if (string.IsNullOrEmpty(_member.name))
            {
                return Json(new { success = false, message = "姓名不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(_member.username))
            {
                return Json(new { success = false, message = "用户名不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (_member.organization_id == null)
            {
                return Json(new { success = false, message = "部门不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (_member.area_id == null)
            {
                return Json(new { success = false, message = "区域不能为空" }, JsonRequestBehavior.AllowGet);
            }

            string hashPassword = HashPassword.GetHashPassword("1"); // 默认密码1
            
            var r = Uof.ImemberService.AddEntity(new member
            {
                name = _member.name,
                username = _member.username,
                password = hashPassword,
                english_name = _member.english_name,
                area_id = _member.area_id,
                birthday = _member.birthday,
                hiredate = _member.hiredate,
                mobile = _member.mobile,
                organization_id = _member.organization_id,
                position_id = _member.position_id,
                status = _member.status
            });

            return Json(r, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update(member _member)
        {
            var dbMember = Uof.ImemberService.GetAll(a=>a.id == _member.id).FirstOrDefault();
            if (dbMember == null)
            {
                return ErrorResult;
            }

            if (dbMember.name == _member.name &&
                dbMember.username == _member.username &&
                dbMember.english_name == _member.english_name &&
                dbMember.area_id == _member.area_id &&
                dbMember.birthday == _member.birthday &&
                dbMember.hiredate == _member.hiredate &&
                dbMember.mobile == _member.mobile &&
                dbMember.organization_id == _member.organization_id &&
                dbMember.position_id == _member.position_id &&
                dbMember.status == _member.status
                )
            {
                return SuccessResult;
            }
            dbMember.name = _member.name;
            dbMember.username = _member.username;
            dbMember.english_name = _member.english_name;
            dbMember.area_id = _member.area_id;
            dbMember.birthday = _member.birthday;
            dbMember.hiredate = _member.hiredate;
            dbMember.mobile = _member.mobile;
            dbMember.organization_id = _member.organization_id;
            dbMember.status = _member.status;
            dbMember.position_id = _member.position_id;

            var r = Uof.ImemberService.UpdateEntity(dbMember);
            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            var _area = Uof.IareaService.GetAll(a => a.id == id).FirstOrDefault();
            if (_area == null)
            {
                return ErrorResult;
            }

            var r = Uof.IareaService.DeleteEntity(_area);
            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExistUsername(string username, int? id)
        {
            Expression<Func<member, bool>> condition = m => (m.username == username);
            Expression<Func<member, bool>> idQuery = m => true;
            if (id != null)
            {
                idQuery = m => (m.id == id.Value);
            }

            var _member = Uof.ImemberService.GetAll(condition).Where(idQuery).Select(m => new {
                id = m.id,
                name = m.name,
                username = m.username
            }).FirstOrDefault();

            if (_member == null)
            {
                return Json(new { ok = "验证成功" }, JsonRequestBehavior.AllowGet);
            }

            if (_member != null && _member.id == id)
            {
                return Json(new { ok = "验证成功" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = "用户名已存在" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ResetPwd(int id)
        {
            var user = Uof.ImemberService.GetAll(m => m.id == id).FirstOrDefault();
            if (user == null)
            {
                return ErrorResult;
            }
            var newPassword = HashPassword.GetHashPassword("123456");

            user.password = newPassword;
            var r = Uof.ImemberService.UpdateEntity(user);
            return Json(new { success = r, pwd = "123456" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAll()
        {
            var items = Uof.ImemberService.GetAll().Where(m=>m.username != "admin").Select(m => new
            {
                id = m.id,
                name = m.name
            }).ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }
    }
}