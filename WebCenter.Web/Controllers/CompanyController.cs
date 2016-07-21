using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using System.IO;
using Common;
using Newtonsoft.Json;

namespace WebCenter.Web.Controllers
{
    public class CompanyController : BaseController
    {
        public CompanyController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        public ActionResult GetAllCompany()
        {
            var list = Uof.IcompanyService.GetAll().Select(c=> new {
                id = c.id,
                name = c.name,
                short_name = c.short_name,
                logo_url = c.logo_url,
                industry_type = c.industry_type
            }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Create(string name, int userId)
        {
            var exist = Uof.IcompanyService.GetAll(c => c.name == name).FirstOrDefault();
            if (exist != null)
            {
                return Json(new { success = false, message = "公司名已经存在" }, JsonRequestBehavior.AllowGet);
            }

            var _user = Uof.IuserService.GetById(userId);
            if (_user.company_id != null)
            {
                return Json(new { success = false, message = "你已加入公司，请退出程序后重新登录" }, JsonRequestBehavior.AllowGet);
            }
            
            var _company = new company()
            {
                name = name,
                short_name = name,
                date_created = DateTime.Now
            };

            
            var newCompany = Uof.IcompanyService.AddEntity(_company);
            _user.company_id = newCompany.id;
            _user.is_admin = 1;
            _user.status = (int)ReviewStatus.Accept; // 公司创建者为公司管理员  所以status=1
            Uof.IuserService.UpdateEntity(_user);

            InsertMessage(string.Format("你创建了“{0}”", newCompany.name), newCompany.id, userId, (int)MessageType.Organization);

            return Json(new { success = true, message = "", companyId = newCompany.id }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Join(int userId, int companyId, string companyName)
        {
            // 获取公司管理员
            var admin = Uof.IuserService.GetAll(u => u.company_id == companyId && u.is_admin == 1).Select(u => new { id = u.id }).FirstOrDefault();
            if (admin == null)
            {
                return Json(new { success = false, message = "该公司不存在管理员，请联系系统管理员" }, JsonRequestBehavior.AllowGet);
            }

            var _user = Uof.IuserService.GetById(userId);

            var waitdealLines = new List<WaitdealLine>();
            waitdealLines.Add(new WaitdealLine()
            {
                icon = "k-comment-empty",
                content = "快让我加入公司吧"
            });

            // 插入待办表通知管理员审核
            var newWaitDeal = new waitdeal()
            {
                source_id = _user.id,
                creator = _user.id,
                source = SourceType.JoinCompany.ToString(),
                title = "申请加入公司",
                content = JsonConvert.SerializeObject(waitdealLines), // "快让我加入公司吧", // string.Format("“我是{0}，”", _user.name),
                status = (int)LineStatus.WaitReview,
                company_id = companyId,
                date_created = DateTime.Now
            };

            var exist = Uof.IwaitdealService.GetAll(w => w.source_id == _user.id && w.source == SourceType.JoinCompany.ToString()).FirstOrDefault();
            if (exist == null)
            {
                var _waitdeal = Uof.IwaitdealService.AddEntity(newWaitDeal);
            }
            else
            {
                exist.company_id = companyId;
                exist.status = (int)LineStatus.WaitReview;
                exist.date_created = DateTime.Now;
                Uof.IwaitdealService.UpdateEntity(exist);
            }

            // 更新申请者user表的company_id 字段            
            _user.company_id = companyId;
            _user.status = (int)ReviewStatus.WaitReview; // 待审核
            _user.is_admin = 0;
            Uof.IuserService.UpdateEntity(_user);

            var messages = new List<message>();
            messages.Add(new message()
            {
                content = string.Format("你加入了“{0}”", companyName),
                source_id = companyId,
                user_id = userId,
                type = (int)MessageType.Organization,
                date_created = DateTime.Now
            });
            messages.Add(new message()
            {
                content = string.Format("{0}加入了“{1}”", _user.name, companyName),
                source_id = companyId,
                user_id = admin.id,
                type = (int)MessageType.Organization,
                date_created = DateTime.Now
            });
            Uof.ImessageService.AddEntities(messages);

            return Json(new { success = true, message = "", companyId = companyId }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult JoinByName(int userId, string companyName)
        {
            var _company = Uof.IcompanyService.GetAll(u => u.name == companyName).FirstOrDefault();
            if (_company == null)
            {
                return Json(new { success = false, message = "此公司名称不存在" }, JsonRequestBehavior.AllowGet);
            }

            // 获取公司管理员
            var admin = Uof.IuserService.GetAll(u => u.company_id == _company.id && u.is_admin == 1).Select(u => new { id = u.id }).FirstOrDefault();
            if (admin == null)
            {
                return Json(new { success = false, message = "该公司不存在管理员，请联系系统管理员" }, JsonRequestBehavior.AllowGet);
            }

            var _user = Uof.IuserService.GetById(userId);

            var waitdealLines = new List<WaitdealLine>();
            waitdealLines.Add(new WaitdealLine()
            {
                icon = "k-comment-empty",
                content = "快让我加入公司吧"
            });

            // 插入待办表通知管理员审核
            var newWaitDeal = new waitdeal()
            {
                source_id = _user.id,
                creator = _user.id,
                source = SourceType.JoinCompany.ToString(),
                title = "申请加入公司",
                content = JsonConvert.SerializeObject(waitdealLines), // "快让我加入公司吧", // string.Format("“我是{0}，”", _user.name),
                status = (int)LineStatus.WaitReview,
                company_id = _company.id,
                date_created = DateTime.Now
            };

            var exist = Uof.IwaitdealService.GetAll(w => w.source_id == _user.id && w.source == SourceType.JoinCompany.ToString()).FirstOrDefault();
            if (exist == null)
            {
                var _waitdeal = Uof.IwaitdealService.AddEntity(newWaitDeal);
            }
            else
            {
                exist.company_id = _company.id;
                exist.status = (int)LineStatus.WaitReview;
                exist.date_created = DateTime.Now;
                Uof.IwaitdealService.UpdateEntity(exist);
            }

            // 更新申请者user表的company_id 字段            
            _user.company_id = _company.id;
            _user.status = (int)ReviewStatus.WaitReview; // 待审核
            _user.is_admin = 0;
            Uof.IuserService.UpdateEntity(_user);

            var messages = new List<message>();
            messages.Add(new message()
            {
                content = string.Format("你加入了“{0}”", companyName),
                source_id = _company.id,
                user_id = userId,
                type = (int)MessageType.Organization,
                date_created = DateTime.Now
            });
            messages.Add(new message()
            {
                content = string.Format("{0}加入了“{1}”", _user.name, companyName),
                source_id = _company.id,
                user_id = admin.id,
                type = (int)MessageType.Organization,
                date_created = DateTime.Now
            });
            Uof.ImessageService.AddEntities(messages);

            return Json(new { success = true, message = "", companyId = _company.id }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetMembers(int companyId)
        {
            var users = Uof.IuserService.GetAll(u => u.company_id == companyId && u.status == (int)ReviewStatus.Accept).
                Select(u => new {
                    id = u.id,
                    name = u.name,
                    mobile = u.mobile,
                    picture_url = u.picture_url,
                    status = u.status,
                    is_admin = u.is_admin
                }).ToList();
            return Json(users, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteMember(int companyId, int userId, int operatorId)
        {
            if(companyId == 0)
            {
                return Json(new { success = false, message = "parameter companyId is required" }, JsonRequestBehavior.AllowGet);
            }
            if (userId == 0)
            {
                return Json(new { success = false, message = "parameter userId is required" }, JsonRequestBehavior.AllowGet);
            }
            if (operatorId == 0)
            {
                return Json(new { success = false, message = "parameter operatorId is required" }, JsonRequestBehavior.AllowGet);
            }

            var _operator = Uof.IuserService.GetAll(u => u.id == operatorId && u.company_id == companyId && u.status == (int)ReviewStatus.Accept).FirstOrDefault();
            if (_operator == null)
            {
                return Json(new { success = false, message = "您没有此操作权限" }, JsonRequestBehavior.AllowGet);
            }

            if (_operator.is_admin != 1)
            {
                return Json(new { success = false, message = "您没有此操作权限" }, JsonRequestBehavior.AllowGet);
            }

            var _u = Uof.IuserService.GetById(userId);
            if(_u == null)
            {
                return Json(new { success = false, message = "没有找到该用户，移除失败" }, JsonRequestBehavior.AllowGet);
            }

            _u.company_id = null;
            _u.status = (int)ReviewStatus.WaitReview;
            var result = Uof.IuserService.UpdateEntity(_u);

            if(result)
            {
                var _c = Uof.IcompanyService.GetById(companyId);
               InsertMessage(string.Format("你被移出了公司{0}", _c.name), null, _u.id, (int)MessageType.Organization);
            }

            return Json(new { success = result, message = result ? "" : "移除失败" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetInfo(int companyId)
        {
            if (companyId == 0)
            {
                return Json(new { success = false, message = "parameter companyId is required" }, JsonRequestBehavior.AllowGet);
            }

            var memberCount = Uof.IuserService.GetAll(u => u.company_id == companyId && u.status == (int)ReviewStatus.Accept).Count();
            var projectCount = Uof.IprojectService.GetAll(p => p.company_id == companyId && (p.status == (int)ProjectStatus.Starting || p.status == (int)ProjectStatus.Finished)).Count();

            return Json(new { success = true, result = new { memberCount = memberCount, projectCount = projectCount }}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get(int companyId)
        {
            var _company = Uof.IcompanyService.GetAll(c=> c.id == companyId).Select(c=>new {
                id = c.id,
                name = c.name,
                short_name = c.short_name,
                logo_url = c.logo_url
            }).FirstOrDefault();

            return Json(_company, JsonRequestBehavior.AllowGet);
        }

        [HttpPost] 
        public ActionResult Save(company _company)
        {
            if(string.IsNullOrEmpty(_company.name))
            {
                return Json(new { success = false, message = "公司名称不能为空" }, JsonRequestBehavior.AllowGet);
            }

            if (_company.id == 0)
            {
                return Json(new { success = false, message = "parameter id is required" }, JsonRequestBehavior.AllowGet);
            }

            var dbCompany = Uof.IcompanyService.GetById(_company.id);

            dbCompany.name = _company.name;
            dbCompany.short_name = _company.name;
            dbCompany.logo_url = _company.logo_url;
            dbCompany.date_updated = DateTime.Now;

            var rst = Uof.IcompanyService.UpdateEntity(dbCompany);

            return Json(new { success = rst, message = rst ? "" : "更新失败" }, JsonRequestBehavior.AllowGet);
        }

    }
}