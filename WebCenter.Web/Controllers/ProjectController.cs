using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using System.IO;
using Common;
using System.Linq.Expressions;

namespace WebCenter.Web.Controllers
{
    [KgAuthorize]
    public class ProjectController : BaseController
    {
        public ProjectController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        [HttpGet]
        public ActionResult Overview(int userId, int companyId)
        {
            // 获取用户信息
            var _user = Uof.IuserService.GetById(userId);

            var projects = new List<project>();

            // 管理员 取所有该公司的项目
            if (_user.is_admin == 1)
            {
                projects = Uof.IprojectService.GetAll(p => p.company_id == companyId && p.status != (int)ProjectStatus.Deleted).ToList();
            }
            else if (_user.status == 1)
            {
                // 自己所属项目
                var ownProjectIds = Uof.ImemberService.GetAll(m => m.userid == userId).Select(m => m.project_id).Distinct().ToList();
                projects = Uof.IprojectService.GetAll(p => ownProjectIds.Contains(p.id) && p.status != (int)ProjectStatus.Deleted).ToList();
            }

            // 开工的项目数
            var countOfDoing = projects.Where(p => p.status == (int)ProjectStatus.Starting).Count();
            // 准备的项目数
            var countOfPrepare = projects.Where(p => p.status == (int)ProjectStatus.Ready).Count();
            // 竣工的项目数
            var countOfDone = projects.Where(p => p.status == (int)ProjectStatus.Finished).Count();
            // 归档的项目数
            var countOfArchive = projects.Where(p => p.status == (int)ProjectStatus.BackProfile).Count();

            var creators = new List<user>();
            if (projects.Count() > 0)
            {
                var userIds = projects.Select(p => p.creator).ToList();
                creators = Uof.IuserService.GetAll(u => userIds.Contains(u.id)).Distinct().ToList();
            }

            var finishItems = projects.Where(p => p.status == 2).OrderByDescending(item => item.date_updated).Select(p => new ProjectItem
            {
                id = p.id,
                company_id = p.company_id,
                name = p.name,
                icon_text = p.name.Length >= 2 ? p.name.Substring(0, 2) : p.name.Substring(0, 1),
                type = p.type,
                area = p.area,
                status = p.status,
                date_started = p.date_started,
                date_finished = p.date_finished,
                modify_status = p.modify_status,
                signed = p.signed,
                creator = p.creator,
                date_created = p.date_created,
                date_updated = p.date_updated,
                progress = "0%"
            }).Take(3).ToList();

            if (finishItems.Count() > 0)
            {
                foreach (var item in finishItems)
                {
                    caculatePercent(item);
                }
            }


            var now = DateTime.Now.Date;
            var overView = new
            {
                Starting = new
                {
                    Count = countOfDoing,
                    Text = string.Format("开工中（{0}）", countOfDoing),
                    // 开工的项目 全部返回
                    Items = projects.Where(p => p.status == 1).Select(p => new
                    {
                        id = p.id,
                        company_id = p.company_id,
                        name = p.name,
                        icon_text = p.name.Length >= 2 ? p.name.Substring(0, 2) : p.name.Substring(0, 1),
                        type = p.type,
                        area = p.area,
                        status = p.status,
                        date_started = p.date_started,
                        date_finished = p.date_finished,
                        modify_status = p.modify_status,
                        signed = p.signed,
                        creator = p.creator,
                        date_created = p.date_created,
                        date_updated = p.date_updated,
                        progress = p.date_started == null ? "" : string.Format("进行了{0}日", (now - p.date_started).Value.Days)
                    }).ToList()
                },
                Ready = new
                {
                    Count = countOfPrepare,
                    Text = string.Format("准备中（{0}）", countOfPrepare),
                    Items = projects.Where(p => p.status == 0).OrderByDescending(item => item.date_updated).Select(p => new
                    {
                        id = p.id,
                        company_id = p.company_id,
                        name = p.name,
                        icon_text = p.name.Length >= 2 ? p.name.Substring(0, 2) : p.name.Substring(0, 1),
                        type = p.type,
                        area = p.area,
                        status = p.status,
                        date_started = p.date_started,
                        date_finished = p.date_finished,
                        modify_status = p.modify_status,
                        signed = p.signed,
                        creator = p.creator,
                        date_created = p.date_created,
                        date_updated = p.date_updated,
                        creator_name = creators.Where(c => c.id == p.creator).Select(c => c.name),
                        progress = string.Format("于{0}创建", p.date_created.Value.Year == DateTime.Today.Year ? p.date_created.Value.ToString("MM月dd日") : p.date_created.Value.ToString("yyyy年MM月dd日")) 
                    }).Take(3).ToList()
                },
                Finished = new
                {
                    Count = countOfDone,
                    Text = string.Format("已竣工（{0}）", countOfDone),
                    Items = finishItems
                },
                BackProfile = new
                {
                    Count = countOfArchive,
                    Text = string.Format("已归档（{0}）", countOfArchive),
                    // 归档的项目 取最新1个
                    Items = projects.Where(p => p.status == -1).OrderByDescending(item => item.date_updated).Select(p => new
                    {
                        id = p.id,
                        company_id = p.company_id,
                        name = p.name,
                        icon_text = p.name.Length >= 2 ? p.name.Substring(0, 2) : p.name.Substring(0, 1),
                        type = p.type,
                        area = p.area,
                        status = p.status,
                        date_started = p.date_started,
                        date_finished = p.date_finished,
                        modify_status = p.modify_status,
                        signed = p.signed,
                        creator = p.creator,
                        date_created = p.date_created,
                        date_updated = p.date_updated,
                        creator_name = creators.Where(c => c.id == p.creator).Select(c => c.name),
                        progress = string.Format("于{0}归档", p.date_updated.Value.Year == DateTime.Today.Year ? p.date_updated.Value.ToString("MM月dd日") : p.date_updated.Value.ToString("yyyy年MM月dd日"))
                    }).Take(1).ToList()
                }
            };


            return Json(overView, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMore(int pageIndex, int pageSize, int companyId, string status)
        {
            var _status = (ProjectStatus)Enum.Parse(typeof(ProjectStatus), status);
            var _count = Uof.IprojectService.GetAll(p => p.company_id == companyId && p.status == (int)_status).Count();

            var list = Uof.IprojectService.GetAll(p => p.company_id == companyId && p.status == (int)_status).OrderByDescending(item => item.date_updated)
                .Select(p => new ProjectItem
                {
                    id = p.id,
                    company_id = p.company_id,
                    name = p.name,
                    icon_text = p.name.Length >= 2 ? p.name.Substring(0, 2) : p.name.Substring(0, 1),
                    type = p.type,
                    area = p.area,
                    status = p.status,
                    date_started = p.date_started,
                    date_finished = p.date_finished,
                    modify_status = p.modify_status,
                    signed = p.signed,
                    creator = p.creator,
                    date_created = p.date_created,
                    date_updated = p.date_updated,
                    creator_name = "",
                    progress = ""
                }).ToPagedList(pageIndex, pageSize).ToList();

            var creators = new List<user>();
            if (list.Count() > 0)
            {
                var userIds = list.Select(p => p.creator).ToList();
                creators = Uof.IuserService.GetAll(u => userIds.Contains(u.id)).Distinct().ToList();
            }

            if (list.Count() > 0)
            {
                foreach (var item in list)
                {
                    switch (_status)
                    {
                        case ProjectStatus.Ready:
                            var _u = creators.Where(c => c.id == item.creator).Select(c => c.name).FirstOrDefault();
                            var progress = string.Format("于{0}创建", item.date_created.Value.Year == DateTime.Today.Year ? item.date_created.Value.ToString("MM月dd日") : item.date_created.Value.ToString("yyyy年MM月dd日"));
                            item.progress = string.Format("由{0}{1}", _u, progress);
                            break;
                        case ProjectStatus.Starting:
                            break;
                        case ProjectStatus.Finished:
                            caculatePercent(item);
                            break;
                        case ProjectStatus.BackProfile:
                            var _u1 = creators.Where(c => c.id == item.creator).Select(c => c.name).FirstOrDefault();
                            var progress1 = string.Format("于{0}归档", item.date_updated.Value.Year == DateTime.Today.Year ? item.date_updated.Value.ToString("MM月dd日") : item.date_updated.Value.ToString("yyyy年MM月dd日"));
                            item.progress = string.Format("由{0}{1}", _u1, progress1);
                            break;
                        case ProjectStatus.Deleted:
                            break;
                        default:
                            break;
                    }
                }
            }
            

            return Json(new { total=_count, projects = list }, JsonRequestBehavior.AllowGet);
        }

        private void caculatePercent(ProjectItem item)
        {
            // 收款总额
            var incomeAmounts = Uof.IincomeService.GetAll(i => i.project_id == item.id && i.status == 1 && i.review == 1).Sum(i => i.amount);
            if (incomeAmounts == null)
            {
                incomeAmounts = 0.0f;
            }
            // 签证总金额
            var signedformAmounts = Uof.IsignedformService.GetAll(s => s.project_id == item.id && s.status == 1 && s.review == 1).Sum(i => i.amount);
            if (signedformAmounts == null)
            {
                signedformAmounts = 0.0f;
            }
            // 合约金额
            var contractAmounts = Uof.IcontractService.GetAll(s => s.project_id == item.id && s.status == 1 && s.review == 1).Sum(i => i.amount);
            if (contractAmounts == null)
            {
                contractAmounts = 0.0f;
            }

            // 结算金额
            var settlementAmounts = Uof.IsettlementService.GetAll(s => s.project_id == item.id && s.status == 1 && s.review == 1).Sum(i => i.amount);
            if (settlementAmounts == null)
            {
                settlementAmounts = 0.0f;
            }

            var total = settlementAmounts > 0 ? settlementAmounts : contractAmounts + signedformAmounts;
            if (total > 0)
            {
                //var percent = (incomeAmounts / total) * 100;
                var percent = String.Format("{0:N0}", (incomeAmounts / total) * 100);
                //var percent = Math.Round((incomeAmounts / total), 2) * 100;
                item.progress = string.Format("当前已收款{0}%", percent);
            } else
            {
                item.progress = string.Format("当前已收款{0}%", 0);
            }
        }

        public ActionResult GetValidProjects(int pageIndex, int pageSize, int companyId)
        {
            var _count = Uof.IprojectService.GetAll(p => p.company_id == companyId && (p.status == (int)ProjectStatus.Starting || p.status == (int)ProjectStatus.Finished)).Count();

            var list = Uof.IprojectService.GetAll(p => p.company_id == companyId && (p.status == (int)ProjectStatus.Starting || p.status == (int)ProjectStatus.Finished)).OrderByDescending(item => item.id)
                .Select(p => new ProjectItem
                {
                    id = p.id,
                    company_id = p.company_id,
                    name = p.name,
                    icon_text = p.name.Length >= 2 ? p.name.Substring(0, 2) : p.name.Substring(0, 1),
                    type = p.type,
                    area = p.area,
                    status = p.status,
                    date_started = p.date_started,
                    date_finished = p.date_finished,
                    modify_status = p.modify_status,
                    signed = p.signed,
                    creator = p.creator,
                    date_created = p.date_created,
                    date_updated = p.date_updated
                }).ToPagedList(pageIndex, pageSize).ToList();

            var creators = new List<user>();
            if (list.Count() > 0)
            {
                var userIds = list.Select(p => p.creator).ToList();
                creators = Uof.IuserService.GetAll(u => userIds.Contains(u.id)).Distinct().ToList();
            }

            if (list.Count() > 0)
            {
                foreach (var item in list)
                {
                    switch (item.status)
                    {
                        case 0:
                            var _u = creators.Where(c => c.id == item.creator).Select(c => c.name).FirstOrDefault();
                            var progress = string.Format("于{0}创建", item.date_created.Value.Year == DateTime.Today.Year ? item.date_created.Value.ToString("MM月dd日") : item.date_created.Value.ToString("yyyy年MM月dd日"));
                            item.progress = string.Format("由{0}{1}", _u, progress);
                            break;
                        case 1:
                            var now = DateTime.Now.Date;
                            item.progress = item.date_started == null ? "" : string.Format("进行了{0}日", (now - item.date_started).Value.Days);
                            break;
                        case 2:
                            //caculatePercent(item);
                            item.progress = "已经竣工";
                            break;
                        case 3:
                            var _u1 = creators.Where(c => c.id == item.creator).Select(c => c.name).FirstOrDefault();
                            var progress1 = string.Format("于{0}归档", item.date_updated.Value.Year == DateTime.Today.Year ? item.date_updated.Value.ToString("MM月dd日") : item.date_updated.Value.ToString("yyyy年MM月dd日"));
                            item.progress = string.Format("由{0}{1}", _u1, progress1);
                            break;
                        case 4:
                            break;
                        default:
                            break;
                    }
                }
            }

            return Json(new { total = _count, projects = list }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMembers(int projectId)
        {
            var members = Uof.ImemberService.GetAll(m => m.project_id == projectId).Select(m => new {
                id = m.userid,
                name = m.user.name,
                picture_url = m.user.picture_url
            }).ToList();
            return Json(members, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetMemberIds(int projectId)
        {
            var members = Uof.ImemberService.GetAll(m => m.project_id == projectId).Select(m => m.userid).ToList();
            return Json(members, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateMembers(int projectId, int[] members)
        {
            if(members == null)
            {
                members = new int[0];
            }
            var oldMembers = Uof.ImemberService.GetAll(m => m.project_id == projectId).OrderBy(m=>m.userid).ToList();
            var oldMemberIds = oldMembers.Select(m => m.userid.Value).ToList();
            
            if (oldMemberIds.Count == 0 && members.Length == 0)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            if (oldMemberIds.Count > 0)
            {
                if (string.Join(",", members) == string.Join(",", oldMemberIds))
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
            }
            

            var _project = Uof.IprojectService.GetById(projectId);
            var deleteMemberIds = new List<int>();
            var deleteMembers = new List<member>();
            var newMemberIds = new List<int>();

            if (oldMemberIds.Count == 0)
            {
                newMemberIds = members.OfType<int>().ToList();
            }
            else if (members.Length == 0)
            {
                deleteMemberIds = oldMemberIds;
                deleteMembers = oldMembers;
            }
            else
            {
                foreach (var item in oldMembers)
                {
                    if (!members.Contains(item.userid.Value))
                    {
                        deleteMemberIds.Add(item.userid.Value);
                        deleteMembers.Add(item);
                    }
                }

                foreach (var item in members)
                {
                    if (!oldMemberIds.Contains(item))
                    {
                        newMemberIds.Add(item);
                    }
                }
            }
            
            if (deleteMemberIds.Count > 0)
            {
                var msgs = new List<message>();
                foreach (var item in deleteMembers)
                {
                    Uof.ImemberService.DeleteEntity(item);
                    msgs.Add(new message()
                    {
                        content = string.Format("你退出了项目组{0}", _project.name),
                        date_created = DateTime.Now,
                        type = (int)MessageType.Project,
                        source_id = _project.id,
                        user_id = item.userid
                    });
                }

                Uof.ImessageService.AddEntities(msgs);
            }

            if (newMemberIds.Count > 0)
            {
                var msgs = new List<message>();
                var mems = new List<member>();
                foreach (var item in newMemberIds)
                {
                    mems.Add(new member()
                    {
                        date_created = DateTime.Now,
                        project_id = projectId,
                        userid = item
                    });

                    msgs.Add(new message()
                    {
                        content = string.Format("你加入了项目组{0}", _project.name),
                        date_created = DateTime.Now,
                        type = (int)MessageType.Project,
                        source_id = _project.id,
                        user_id = item
                    });
                }

                Uof.ImemberService.AddEntities(mems);
                Uof.ImessageService.AddEntities(msgs);
            }


            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Create(ProjectCreateRequest request)
        {
            if (request.company_id == 0)
            {
                return Json(new { success = false, message = "公司ID不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(request.name))
            {
                return Json(new { success = false, message = "项目名称不能为空" }, JsonRequestBehavior.AllowGet);
            }
            //if (request.area == null || request.area <= 0)
            //{
            //    return Json(new { success = false, message = "项目面积不能为空" }, JsonRequestBehavior.AllowGet);
            //}

            var now = DateTime.Now;
            var _project = new project()
            {
                company_id = request.company_id,
                name = request.name,
                area = request.area,
                type = request.type,
                creator = request.creator,
                date_created = now,
                date_updated = now,
                status = (int)ProjectStatus.Ready,
                signed = false,
                modify_status = 0
            };

            var newProject = Uof.IprojectService.AddEntity(_project);

            if(request.members != null && request.members.Length > 0)
            {
                var list = new List<member>();
                var msgs = new List<message>();
                foreach (var item in request.members)
                {
                    list.Add(new member()
                    {
                        project_id = newProject.id,
                        userid = item,
                        date_created = DateTime.Now
                    });

                    msgs.Add(new message()
                    {
                        content = string.Format("你加入了项目组{0}", _project.name),
                        user_id = item,
                        source_id = _project.id,
                        type = (int)MessageType.Project,
                        date_created = DateTime.Now
                    });
                }
                Uof.ImemberService.AddEntities(list);
                Uof.ImessageService.AddEntities(msgs);
            }

            if (request.contract != null && request.contract > 0)
            {
                Uof.IcontractService.AddEntity(new contract()
                {
                    project_id = newProject.id,
                    amount = request.contract,
                    review = (int)ReviewStatus.Accept,
                    status = (int)LineStatus.OK,
                    creator = request.creator,
                    date_created = DateTime.Now
                });
            }


            // 时间轴
            Uof.ItimelineService.AddEntity(new timeline
            {
                content = "建立了本项目",                
                project_id = newProject.id,
                source_id = newProject.id,
                source = SourceType.CreateProject.ToString(),
                user_id = request.creator,
                opts = (int)OptType.ReadOnly,
                status = (sbyte)LineStatus.OK,
                date_created = DateTime.Now
            });



            var np = Uof.IprojectService.GetAll(p => p.id == newProject.id).Select(p => new
            {
                id = p.id,
                company_id = p.company_id,
                name = p.name,
                icon_text = p.name.Length >= 2 ? p.name.Substring(0, 2) : p.name.Substring(0, 1),
                type = p.type,
                area = p.area,
                status = p.status,
                date_started = p.date_started,
                date_finished = p.date_finished,
                modify_status = p.modify_status,
                signed = p.signed,
                creator = p.creator,
                date_created = p.date_created,
                date_updated = p.date_updated,
                creator_name = "",
                progress = ""
            }).FirstOrDefault();


            return Json(new { success = true, result = np }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Archive(int projectId, int userId, string userName)
        {
            var _project = Uof.IprojectService.GetById(projectId);

            _project.date_updated = DateTime.Now;
            _project.status = (int)ProjectStatus.BackProfile;
            Uof.IprojectService.UpdateEntity(_project);

            var t = new timeline()
            {
                project_id = projectId,
                source_id = projectId,
                user_id = userId,
                source = SourceType.Archive.ToString(),
                opts = (int)OptType.ReadOnly,
                content = string.Format("{0}月{1}日，{2}归档了项目", DateTime.Today.Month, DateTime.Today.Day, userName),
                status = (int)LineStatus.OK,
                date_created = DateTime.Now
            };

            var _timeline = Uof.ItimelineService.AddEntity(t);

            var members = Uof.ImemberService.GetAll(m => m.project_id == projectId).Select(m=>m.userid).ToList();
            if(members.Count > 0)
            {
                var msgs = new List<message>();
                foreach (var item in members)
                {
                    msgs.Add(new message()
                    {
                        content = string.Format("你参与的项目{0}被归档了", _project.name),
                        source_id = _project.id,
                        user_id = item,
                        type = (sbyte)MessageType.Project,
                        date_created = DateTime.Now
                    });
                }

                Uof.ImessageService.AddEntities(msgs);
            }
            
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Revert(int projectId, int userId, string userName)
        {
            var _project = Uof.IprojectService.GetById(projectId);

            _project.date_updated = DateTime.Now;
            _project.status = (int)ProjectStatus.Ready;
            Uof.IprojectService.UpdateEntity(_project);

            var t = new timeline()
            {
                project_id = projectId,
                source_id = projectId,
                user_id = userId,
                source = SourceType.Archive.ToString(),
                opts = (int)OptType.ReadOnly,
                content = string.Format("{0}月{1}日，{2}恢复了项目", DateTime.Today.Month, DateTime.Today.Day, userName),
                status = (int)LineStatus.OK,
                date_created = DateTime.Now
            };
            var _timeline = Uof.ItimelineService.AddEntity(t);

            var members = Uof.ImemberService.GetAll(m => m.project_id == projectId).Select(m => m.userid).ToList();
            if (members.Count > 0)
            {
                var msgs = new List<message>();
                foreach (var item in members)
                {
                    msgs.Add(new message()
                    {
                        content = string.Format("你参与的项目{0}被恢复了", _project.name),
                        source_id = _project.id,
                        user_id = item,
                        type = (sbyte)MessageType.Project,
                        date_created = DateTime.Now
                    });
                }

                Uof.ImessageService.AddEntities(msgs);
            }

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(int projectId)
        {
            var currentUser = HttpContext.User.Identity as UserIdentity;
            if (currentUser.is_admin != 1)
            {
                return Json(new { success = false, message = "您不是公司管理员，无法删除项目" }, JsonRequestBehavior.AllowGet);
            }
                        
            var tm = Uof.ItimelineService.GetAll(t => 
                t.status == 1 && 
                t.project_id == projectId && 
                t.source != SourceType.Forum.ToString() &&
                t.source != SourceType.Archive.ToString() &&
                t.source != SourceType.CreateProject.ToString() &&
                t.source != SourceType.Revert.ToString()).Count();

            if (tm > 0)
            {
                return Json(new { success = false, message = "删除项目前请先删除所有动态消息" }, JsonRequestBehavior.AllowGet);
            }

            var _project = Uof.IprojectService.GetById(projectId);

            if (currentUser.company_id != _project.company_id)
            {
                return Json(new { success = false, message = "当前项目非公司项目，无法删除" }, JsonRequestBehavior.AllowGet);
            }

            _project.date_updated = DateTime.Now;
            _project.status = (int)ProjectStatus.Deleted;
            var r = Uof.IprojectService.UpdateEntity(_project);

            return Json(new { success = r, message = r ? "" : "删除失败" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetProjectById(int projectId)
        {
            var proj = Uof.IprojectService.GetAll(it => it.id == projectId).Select(p => new
            {
                id = p.id,
                date_created = p.date_created,
                date_started = p.date_started,
                area = p.area,
                company_id = p.company_id,
                date_finished = p.date_finished,
                creator = p.creator,
                modify_status = p.modify_status,
                name = p.name,
                signed = p.signed,
                status = p.status,
                type = p.type,
                date_updated=p.date_updated
            }).FirstOrDefault();
            if (proj != null)
            {
                return Json(proj, JsonRequestBehavior.AllowGet);
            }
            return ErrorResult;
        }
    }
}