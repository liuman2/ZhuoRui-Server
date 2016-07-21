using System;
using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;
using System.Collections.Generic;
using Newtonsoft.Json;
//using Newtonsoft.Json;

namespace WebCenter.Web.Controllers
{
    
    public class WaitdealController : BaseController
    {
        public WaitdealController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        [HttpGet]
        public ActionResult GetBadge(int companyId)
        {
            var count = Uof.IwaitdealService.GetAll(w => w.company_id == companyId && w.status == (int)LineStatus.WaitReview).Count();

            return Json(new { badge = count }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [KgAuthorize]
        public ActionResult GetWaitdeal(int companyId)
        {
            var currentUser = HttpContext.User.Identity as UserIdentity;
            if (currentUser.is_admin != 1)
            {
                return Json(new { waitdeals = new List<WaitdealResponse>(), count = 0 }, JsonRequestBehavior.AllowGet);
            }

            if (currentUser.company_id != companyId)
            {
                return Json(new { waitdeals = new List<WaitdealResponse>(), count = 0 }, JsonRequestBehavior.AllowGet);
            }


            var list = Uof.IwaitdealService.GetAll(w => w.company_id == companyId && w.status == (int)LineStatus.WaitReview)
                .OrderBy(w => w.id)
                .Select(w => new WaitdealResponse
                {
                    id = w.id,
                    company_id = w.company_id,
                    content = w.content,
                    date_created = w.date_created,
                    project_id = w.project_id,
                    source = w.source,
                    source_id = w.source_id,
                    title = w.title,
                    time_desc = "",
                    icon = w.source == "JoinCompany" ? w.user.picture_url : "",
                    creator = w.creator,
                    operatorType = "",
                    creator_name = w.user.name
                }).ToList();

            if (list.Count > 0)
            {
                var now = DateTime.Now;
                foreach (var item in list)
                {
                    var _source = (SourceType)Enum.Parse(typeof(SourceType), item.source, true);

                    switch (_source)
                    {
                        case SourceType.JoinCompany:
                            item.title = string.Format("{0}申请加入公司", item.creator_name);

                            var waitdealLines = new List<WaitdealLine>();
                            waitdealLines.Add(new WaitdealLine()
                            {
                                icon = "k-comment-empty",
                                content = string.Format("我是{0},快让我加入公司吧", item.creator_name)
                            });

                            item.content_line = waitdealLines;
                            break;
                        default:
                            if(item.title.Contains("更改"))
                            {
                                item.icon = "k-pencil";
                                item.operatorType = "modify";
                            } else if(item.title.Contains("删除"))
                            {
                                item.icon = "k-trash";
                                item.operatorType = "delete";
                            }
                            item.content_line = JsonConvert.DeserializeObject<List<WaitdealLine>>(item.content);
                            break;
                    }

                    item.content = "";

                    if (item.date_created == null)
                    {
                        continue;
                    }

                    var diff = now - item.date_created.Value;
                    if (diff.Days > 0)
                    {
                        item.time_desc = string.Format("由{0}在{1}天前发起", item.creator_name, diff.Days);
                    } else if(diff.Hours > 0)
                    {
                        item.time_desc = string.Format("由{0}在{1}小时前发起", item.creator_name, diff.Hours);
                    }
                    else if (diff.Minutes > 0)
                    {
                        item.time_desc = string.Format("由{0}在{1}分钟前发起", item.creator_name, diff.Minutes);
                    } else
                    {
                        item.time_desc = string.Format("由{0}刚刚发起", item.creator_name);
                    }
                }
            }

            return Json(new { waitdeals = list, count = list.Count }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 审核通过
        /// </summary>
        /// <param name="id">waitdeal.id</param>
        /// <param name="optType">更改/删除 modify/delete</param>
        /// <param name="isPass">是否通过</param>
        /// <returns></returns>
        [HttpPost]
        [KgAuthorize]
        public ActionResult DoWaitDeal(int id, string optType, bool isPass)
        {
            var _waitDeal = Uof.IwaitdealService.GetById(id);
            var creator = _waitDeal.user;

            var sourceId = _waitDeal.source_id.Value;
            var result = false;
            var _source = (SourceType)Enum.Parse(typeof(SourceType), _waitDeal.source);

            var msgType = MessageType.Business;
            var msgForMember = "";
            var msgForAdmin = "";

            switch (_source)
            {
                case SourceType.Contract:
                    result = doContract(sourceId, optType, isPass);
                    msgForMember = string.Format("你{0}合约信息的申请{1}", (optType == "delete" ? "删除" : "更改"), isPass ? "通过了" : "被拒绝了");
                    msgForAdmin = string.Format("你{0}了{1}{2}合约信息的申请", (isPass ? "通过" : "拒绝"), creator.name, (optType == "delete" ? "删除" : "更改"));
                    break;
                case SourceType.Income:
                    result = doIncome(sourceId, optType, isPass);
                    msgForMember = string.Format("你{0}收款信息的申请{1}", (optType == "delete" ? "删除" : "更改"), isPass ? "通过了" : "被拒绝了");
                    msgForAdmin = string.Format("你{0}了{1}{2}收款信息的申请", (isPass ? "通过" : "拒绝"), creator.name, (optType == "delete" ? "删除" : "更改"));
                    break;
                case SourceType.Signedform:
                    result = doSignedform(sourceId, optType, isPass);
                    msgForMember = string.Format("你{0}签证信息的申请{1}", (optType == "delete" ? "删除" : "更改"), isPass ? "通过了" : "被拒绝了");
                    msgForAdmin = string.Format("你{0}了{1}{2}签证信息的申请", (isPass ? "通过" : "拒绝"), creator.name, (optType == "delete" ? "删除" : "更改"));
                    break;
                case SourceType.Settlement:
                    result = doSettlement(sourceId, optType, isPass);
                    msgForMember = string.Format("你{0}结算信息的申请{1}", (optType == "delete" ? "删除" : "更改"), isPass ? "通过了" : "被拒绝了");
                    msgForAdmin = string.Format("你{0}了{1}{2}结算信息的申请", (isPass ? "通过" : "拒绝"), creator.name, (optType == "delete" ? "删除" : "更改"));
                    break;
                case SourceType.ProjectStart:
                    result = doProjectStart(sourceId, optType, isPass);
                    msgForMember = string.Format("你{0}开工日期的申请{1}", (optType == "delete" ? "删除" : "更改"), isPass ? "通过了" : "被拒绝了");
                    msgForAdmin = string.Format("你{0}了{1}{2}开工日期的申请", (isPass ? "通过" : "拒绝"), creator.name, (optType == "delete" ? "删除" : "更改"));
                    break;
                case SourceType.ProjectFinish:
                    result = doProjectFinish(sourceId, optType, isPass);
                    msgForMember = string.Format("你{0}竣工日期的申请{1}", (optType == "delete" ? "删除" : "更改"), isPass ? "通过了" : "被拒绝了");
                    msgForAdmin = string.Format("你{0}了{1}{2}竣工日期的申请", (isPass ? "通过" : "拒绝"), creator.name, (optType == "delete" ? "删除" : "更改"));
                    break;
                case SourceType.ProjectInfoModify:
                    result = doProjectModify(sourceId, isPass);
                    msgForMember = string.Format("你更改项目信息的申请{0}", isPass ? "通过了" : "被拒绝了");
                    msgForAdmin = string.Format("你{0}了{1}更改项目信息的申请", (isPass ? "通过" : "拒绝"), creator.name);
                    break;
                case SourceType.JoinCompany:
                    result = doJoinCompany(sourceId, isPass);
                    msgForMember = string.Format("你加入公司的申请{0}", isPass ? "通过了" : "被拒绝了");
                    msgForAdmin = string.Format("你{0}了{1}加入公司的申请", (isPass ? "通过" : "拒绝"), creator.name);
                    msgType = MessageType.Organization;
                    break;
                default:
                    break;
            }
                        
            if (result)
            {
                var currentUser = HttpContext.User.Identity as UserIdentity;
                _waitDeal.status = isPass ? (sbyte)LineStatus.OK : (sbyte)LineStatus.Abandon;
                _waitDeal.reviewer = currentUser.id;
                _waitDeal.date_updated = DateTime.Now;
                Uof.IwaitdealService.UpdateEntity(_waitDeal);

                var msgs = new List<message>();
                msgs.Add(new message()
                {
                    content = msgForMember,
                    source_id = _waitDeal.id,
                    user_id = _waitDeal.creator,
                    type = (sbyte)msgType,
                    date_created = DateTime.Now
                });
                msgs.Add(new message()
                {
                    content = msgForAdmin,
                    source_id = _waitDeal.id,
                    user_id = currentUser.id,
                    type = (sbyte)msgType,
                    date_created = DateTime.Now
                });
                Uof.ImessageService.AddEntities(msgs);
            }

            return base.SuccessResult;
        }

        private bool doProjectModify(int id, bool isPass)
        {
            var _source = SourceType.ProjectInfoModify.ToString();

            // 需要审核的项目修改数据
            var _needReviewProject = Uof.IprojectmodifyService.GetById(id);
            // 原始项目数据
            var _origProject = Uof.IprojectService.GetById(_needReviewProject.project_id.Value);
            // 时间轴数据
            //var _timeline = Uof.ItimelineService.GetAll(t => t.source_id == id && t.source == _source).FirstOrDefault();
            //当前操作者
            var currentUser = HttpContext.User.Identity as UserIdentity;

            if (isPass)
            {
                _origProject.modify_status = (int)ProjectModifyStatus.Normal;
                _origProject.name = _needReviewProject.name;
                _origProject.area = _needReviewProject.area;
                _origProject.type = _needReviewProject.type;
                Uof.IprojectService.UpdateEntity(_origProject);
            }
            else
            {
                _origProject.modify_status = (int)ProjectModifyStatus.Normal;
                Uof.IprojectService.UpdateEntity(_origProject);
            }

            // projectmodify表:
            //  1. review字段 通过为ReviewStatus.Accept， 不通过为ReviewStatus.Reject
            //  2. status字段 通过为LineStatus.OK， 不通过为LineStatus.Abandon            
            _needReviewProject.status = isPass ? (int)LineStatus.OK : (int)LineStatus.Abandon;
            _needReviewProject.review = isPass ? (sbyte)ReviewStatus.Accept : (sbyte)ReviewStatus.Reject;
            _needReviewProject.reviewer = currentUser.id;
            _needReviewProject.date_reviewed = DateTime.Now;
            Uof.IprojectmodifyService.UpdateEntity(_needReviewProject);

            // 时间轴表: 通过 状态改为OK， 不通过 状态改为 Abandon
            //if (_timeline)
            //{
            //    _timeline.status = isPass ? (sbyte)LineStatus.OK : (sbyte)LineStatus.Abandon;
            //    _timeline.date_updated = DateTime.Now;
            //    Uof.ItimelineService.UpdateEntity(_timeline);
            //}           

            return true;
        }

        /// <summary>
        /// 更改开工时间
        /// </summary>
        /// <param name="id"></param>
        /// <param name="optType"></param>
        /// <param name="isPass"></param>
        /// <returns></returns>
        private bool doProjectStart(int id, string optType, bool isPass)
        {
            var _source = SourceType.ProjectStart.ToString();  

            // 需要审核的项目修改数据
            var _needReviewProject = Uof.IprojectmodifyService.GetById(id);
            // 原始项目数据
            var _origProject = Uof.IprojectService.GetById(_needReviewProject.project_id.Value);
            // 时间轴数据
            var _timeline = Uof.ItimelineService.GetAll(t=>t.source_id == id && t.source == _source).FirstOrDefault();
            //当前操作者
            var currentUser = HttpContext.User.Identity as UserIdentity;

            switch (optType)
            {
                // 删除开工时间
                // project表: 
                //  1. modify_status状态改为0(Normal)
                //  2. 通过: 审核通过 项目变为准备  审核不通过 项目状态不修改
                //  3. 开工时间 date_started 为null
                case "delete":
                    if (isPass)
                    {
                        // 如果没有签证记录 并且没有收款记录 则变为准备中的项目
                        var cc = _origProject.signedforms.Where(c => c.status == (int)LineStatus.OK).Count();
                        var ic = _origProject.incomes.Where(i => i.status == (int)LineStatus.OK).Count();
                        if (cc == 0 && ic == 0)
                        {
                            _origProject.status = (int)ProjectStatus.Ready; // 变为准备中的项目
                        }
                        
                        _origProject.modify_status = (int)ProjectModifyStatus.Normal;
                        _origProject.date_started = null;
                        Uof.IprojectService.UpdateEntity(_origProject);                       
                    }
                    else
                    {
                        _origProject.modify_status = (int)ProjectModifyStatus.Normal;
                        Uof.IprojectService.UpdateEntity(_origProject);
                    }
                    break;
                // 修改开工时间
                //  1. modify_status状态改为0(Normal)
                //  2. 通过:  date_started =  _needReviewProject.date_modified
                //  3. 开工时间 date_started 为null
                case "modify":
                    if (isPass)
                    {

                        _timeline.content = "设置开工日期为" +  _needReviewProject.date_modified.GetValueOrDefault().ToString("M月d日");
                        _origProject.modify_status = (int)ProjectModifyStatus.Normal;
                        _origProject.date_started = _needReviewProject.date_modified;  // 开工时间改为改后的时间
                        Uof.IprojectService.UpdateEntity(_origProject);
                    }
                    else
                    {
                        _origProject.modify_status = (int)ProjectModifyStatus.Normal;
                        Uof.IprojectService.UpdateEntity(_origProject);
                    }
                    break;
                default:
                    break;
            }

            // projectmodify表:
            //  1. review字段 通过为ReviewStatus.Accept， 不通过为ReviewStatus.Reject
            //  2. status字段 通过为LineStatus.OK， 不通过为LineStatus.Abandon            
            _needReviewProject.status = isPass ? (int)LineStatus.OK : (int)LineStatus.Abandon;
            _needReviewProject.review = isPass ? (sbyte)ReviewStatus.Accept : (sbyte)ReviewStatus.Reject;
            _needReviewProject.reviewer = currentUser.id;
            _needReviewProject.date_reviewed = DateTime.Now;
            Uof.IprojectmodifyService.UpdateEntity(_needReviewProject);

           
            if (optType == "delete")
            {
                // 时间轴表: 删除 通过 状态改为abandon 不通过 状态改为 正常
                _timeline.status = isPass ? (sbyte)LineStatus.Abandon : (sbyte)LineStatus.OK;
            }
            else
            {
                // 时间轴表: 修改 通过 状态改为OK 不通过 也改为OK
                if (isPass)
                {
                    _timeline.source_id = _needReviewProject.id;
                }
                _timeline.status = (sbyte)LineStatus.OK; // isPass ? (sbyte)LineStatus.OK : (sbyte)LineStatus.OK;
            }
          
            _timeline.date_updated = DateTime.Now;
            Uof.ItimelineService.UpdateEntity(_timeline);

            return true;
        }

        /// <summary>
        /// 更改竣工时间
        /// </summary>
        /// <param name="id"></param>
        /// <param name="optType"></param>
        /// <param name="isPass"></param>
        /// <returns></returns>
        private bool doProjectFinish(int id, string optType, bool isPass)
        {
            var _source = SourceType.ProjectFinish.ToString();

            // 需要审核的项目修改数据
            var _needReviewProject = Uof.IprojectmodifyService.GetById(id);
            // 原始项目数据
            var _origProject = Uof.IprojectService.GetById(_needReviewProject.project_id.Value);
            // 时间轴数据
            var _timeline = Uof.ItimelineService.GetAll(t => t.source_id == id && t.source == _source).FirstOrDefault();
            //当前操作者
            var currentUser = HttpContext.User.Identity as UserIdentity;

            switch (optType)
            {
                // 删除竣工时间
                // project表: 
                //  1. modify_status状态改为0(Normal)
                //  2. 通过: 审核通过 项目变为开工  审核不通过 项目状态不修改
                //  3. 开工时间 date_finished 为null
                case "delete":
                    if (isPass)
                    {

                        // 如果有结算记录则竣工状态不变
                        var sc = _origProject.settlements.Where(c => c.status == (int)LineStatus.OK).Count();
                        if (sc == 0)
                        {
                            // 如果没有签证记录 并且没有收款记录 则变为准备中的项目
                            var cc = _origProject.signedforms.Where(c => c.status == (int)LineStatus.OK).Count();
                            var ic = _origProject.incomes.Where(i => i.status == (int)LineStatus.OK).Count();
                            if (cc == 0 && ic == 0)
                            {
                                _origProject.status = (int)ProjectStatus.Ready; // 变为准备中的项目
                            } else
                            {
                                _origProject.status = (int)ProjectStatus.Starting; // 变为开工中的项目
                            }
                        }

                        _origProject.modify_status = (int)ProjectModifyStatus.Normal;
                        _origProject.date_finished = null;
                        Uof.IprojectService.UpdateEntity(_origProject);
                    }
                    else
                    {
                        _origProject.modify_status = (int)ProjectModifyStatus.Normal;
                        Uof.IprojectService.UpdateEntity(_origProject);
                    }
                    break;
                // 修改竣工时间
                //  1. modify_status状态改为0(Normal)
                //  2. 通过:  date_finished =  _needReviewProject.date_modified
                //  3. 开工时间 date_started 为null
                case "modify":
                    if (isPass)
                    {
                        _timeline.content = "设置竣工日期为" + _needReviewProject.date_modified.GetValueOrDefault().ToString("M月d日"); // + "|" + "验收完毕，项目竣工";
                        _origProject.modify_status = (int)ProjectModifyStatus.Normal;
                        _origProject.date_finished = _needReviewProject.date_modified;  // 竣工时间改为改后的时间
                        Uof.IprojectService.UpdateEntity(_origProject);
                    }
                    else
                    {
                        _origProject.modify_status = (int)ProjectModifyStatus.Normal;
                        Uof.IprojectService.UpdateEntity(_origProject);
                    }
                    break;
                default:
                    break;
            }

            // projectmodify表:
            //  1. review字段 通过为ReviewStatus.Accept， 不通过为ReviewStatus.Reject
            //  2. status字段 通过为LineStatus.OK， 不通过为LineStatus.Abandon            
            _needReviewProject.status = isPass ? (int)LineStatus.OK : (int)LineStatus.Abandon;
            _needReviewProject.review = isPass ? (sbyte)ReviewStatus.Accept : (sbyte)ReviewStatus.Reject;
            _needReviewProject.reviewer = currentUser.id;
            _needReviewProject.date_reviewed = DateTime.Now;
            Uof.IprojectmodifyService.UpdateEntity(_needReviewProject);

            // 时间轴表: 通过 状态改为OK， 不通过 状态改为 Abandon


            if (optType == "delete")
            {
                // 时间轴表: 删除 通过 状态改为abandon 不通过 状态改为 正常
                _timeline.status = isPass ? (sbyte)LineStatus.Abandon : (sbyte)LineStatus.OK;
            }
            else
            {
                // 时间轴表: 修改 通过 状态改为OK 不通过 也改为OK
                if (isPass)
                {
                    _timeline.source_id = _needReviewProject.id;
                }
                _timeline.status = (sbyte)LineStatus.OK; // isPass ? (sbyte)LineStatus.OK : (sbyte)LineStatus.OK;
            }

          //  _timeline.status = isPass ? (sbyte)LineStatus.OK : (sbyte)LineStatus.Abandon;
            _timeline.date_updated = DateTime.Now;
            Uof.ItimelineService.UpdateEntity(_timeline);

            return true;
        }

        /// <summary>
        /// 加入公司
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="isPass">是否通过</param>
        /// <returns></returns>
        private bool doJoinCompany(int id, bool isPass)
        {
            var _user = Uof.IuserService.GetById(id);
            _user.status = isPass ? (int)ReviewStatus.Accept : (int)ReviewStatus.Reject;
            if (!isPass)
            {
                // 审核不通过 company_id设为null
                _user.company_id = null;
            }            
            var r = Uof.IuserService.UpdateEntity(_user);

            return r;
        }

        /// <summary>
        /// 结算
        /// </summary>
        /// <param name="id">需要审核的那笔数据id</param>
        /// <param name="optType">操作类型删除还是修改</param>
        /// <param name="isPass">是否审核通过</param>
        /// <returns></returns>
        private bool doSettlement(int id, string optType, bool isPass)
        {
            var _source = SourceType.Settlement.ToString();

            // 需要审核的数据
            var _needReviewSettlement = Uof.IsettlementService.GetById(id);
            // 原始数据
            var _origSettlement = Uof.IsettlementService.GetById(_needReviewSettlement.batch_id.Value);
            // 时间线数据
            var _timeline = Uof.ItimelineService.GetAll(t => t.source_id == _origSettlement.id && t.source == _source).FirstOrDefault();
            //当前操作者
            var currentUser = HttpContext.User.Identity as UserIdentity;
            switch (optType)
            {
                case "delete":
                    // 删除审核通过：
                    // 1. 原始数据作废
                    // 2. 删除新增的那笔数据作废
                    // 3. 时间轴数据数据作废

                    // 删除审核不通过：
                    // 1. 原始数据不变
                    // 2. 删除新增的那笔数据作废
                    // 3. 时间轴数据数据正常

                    // 删除审核通过： 原始数据作废，不通过则不处理原始数据
                    if (isPass && _origSettlement != null)
                    {
                        _origSettlement.status = (int)LineStatus.Abandon;
                        _origSettlement.reviewer = currentUser.id;
                        _origSettlement.date_reviewed = DateTime.Now;
                        Uof.IsettlementService.UpdateEntity(_origSettlement);
                    }

                    // 删除新增的那笔数据作废
                    if (_needReviewSettlement != null)
                    {
                        // 不管审核是否通过  删除新增的那笔数据都作废
                        _needReviewSettlement.status = (int)LineStatus.Abandon;
                        _needReviewSettlement.review = isPass ? (sbyte)ReviewStatus.Accept : (sbyte)ReviewStatus.Reject;
                        _needReviewSettlement.reviewer = currentUser.id;
                        _needReviewSettlement.date_reviewed = DateTime.Now;
                        Uof.IsettlementService.UpdateEntity(_needReviewSettlement);
                    }

                    // 时间轴数据
                    if (_timeline != null)
                    {
                        _timeline.status = isPass ? (sbyte)LineStatus.Abandon : (sbyte)LineStatus.OK;
                        _timeline.date_updated = DateTime.Now;
                        Uof.ItimelineService.UpdateEntity(_timeline);
                    }
                    break;
                case "modify":
                    // 修改审核通过：
                    // 1. 原始数据状态作废
                    // 2. 待审核所新增的数据状态正常
                    // 3. 时间轴数据状态正常，时间轴的source_id = 待审核所新增的数据的id

                    // 修改审核不通过
                    // 1. 原始数据不变
                    // 2. 待审核所新增的数据作废
                    // 3. 时间轴数据状态正常，source_id不变

                    // 修改审核通过：
                    if (isPass)
                    {
                        // 1. 原始数据状态作废
                        if (_origSettlement != null)
                        {
                            _origSettlement.status = (int)LineStatus.Abandon;
                            _origSettlement.reviewer = currentUser.id;
                            _origSettlement.date_reviewed = DateTime.Now;
                            Uof.IsettlementService.UpdateEntity(_origSettlement);
                        }
                        // 2. 待审核所新增的数据状态正常
                        if (_needReviewSettlement != null)
                        {
                            _needReviewSettlement.status = (int)LineStatus.OK;
                            _needReviewSettlement.review = (int)ReviewStatus.Accept;
                            _needReviewSettlement.reviewer = currentUser.id;
                            _needReviewSettlement.date_reviewed = DateTime.Now;
                            Uof.IsettlementService.UpdateEntity(_needReviewSettlement);
                        }

                        // 3. 时间轴数据正常，source_id = _needReviewContract.id
                        if (_timeline != null)
                        {
                            _timeline.content = "设置结算金额￥" + _needReviewSettlement.amount.GetValueOrDefault().ToString("f2") + "元 " + "结算日期为" + _needReviewSettlement.date_signed.GetValueOrDefault().ToString("yyyy年M月d日");
                            _timeline.status = (sbyte)LineStatus.OK;
                            _timeline.source_id = _needReviewSettlement.id;
                            _timeline.date_updated = DateTime.Now;
                            Uof.ItimelineService.UpdateEntity(_timeline);
                        }
                    }
                    else
                    {
                        // 1. 原始数据不变

                        // 2. 待审核所新增的数据作废
                        if (_needReviewSettlement != null)
                        {
                            _needReviewSettlement.status = (int)LineStatus.Abandon;
                            _needReviewSettlement.review = (int)ReviewStatus.Reject;
                            _needReviewSettlement.reviewer = currentUser.id;
                            _needReviewSettlement.date_reviewed = DateTime.Now;
                            Uof.IsettlementService.UpdateEntity(_needReviewSettlement);
                        }
                        // 3. 时间轴数据状态正常，source_id不变
                        if (_timeline != null)
                        {
                            _timeline.status = (sbyte)LineStatus.OK;
                            _timeline.date_updated = DateTime.Now;
                            Uof.ItimelineService.UpdateEntity(_timeline);
                        }
                    }
                    break;
                default:
                    break;
            }

            return true;
        }

        /// <summary>
        /// 签证
        /// </summary>
        /// <param name="id">需要审核的那笔数据id</param>
        /// <param name="optType">操作类型删除还是修改</param>
        /// <param name="isPass">是否审核通过</param>
        /// <returns></returns>
        private bool doSignedform(int id, string optType, bool isPass)
        {
            var _source = SourceType.Signedform.ToString();

            // 需要审核的数据
            var _needReviewSignedform = Uof.IsignedformService.GetById(id);
            // 原始数据
            var _origSignedform = Uof.IsignedformService.GetById(_needReviewSignedform.batch_id.Value);
            // 时间线数据
            var _timeline = Uof.ItimelineService.GetAll(t => t.source_id == _origSignedform.id && t.source == _source).FirstOrDefault();
            //当前操作者
            var currentUser = HttpContext.User.Identity as UserIdentity;
            switch (optType)
            {
                case "delete":
                    // 删除审核通过：
                    // 1. 原始数据作废
                    // 2. 删除新增的那笔数据作废
                    // 3. 时间轴数据数据作废

                    // 删除审核不通过：
                    // 1. 原始数据不变
                    // 2. 删除新增的那笔数据作废
                    // 3. 时间轴数据数据正常

                    // 删除审核通过： 原始数据作废，不通过则不处理原始数据
                    if (isPass && _origSignedform != null)
                    {
                        _origSignedform.status = (int)LineStatus.Abandon;
                        _origSignedform.reviewer = currentUser.id;
                        _origSignedform.date_reviewed = DateTime.Now;
                        Uof.IsignedformService.UpdateEntity(_origSignedform);
                    }

                    // 删除新增的那笔数据作废
                    if (_needReviewSignedform != null)
                    {
                        // 不管审核是否通过  删除新增的那笔数据都作废
                        _needReviewSignedform.status = (int)LineStatus.Abandon;
                        _needReviewSignedform.review = isPass ? (sbyte)ReviewStatus.Accept : (sbyte)ReviewStatus.Reject;
                        _needReviewSignedform.reviewer = currentUser.id;
                        _needReviewSignedform.date_reviewed = DateTime.Now;
                        Uof.IsignedformService.UpdateEntity(_needReviewSignedform);
                    }

                    // 时间轴数据
                    if (_timeline != null)
                    {
                        _timeline.status = isPass ? (sbyte)LineStatus.Abandon : (sbyte)LineStatus.OK;
                        _timeline.date_updated = DateTime.Now;
                        Uof.ItimelineService.UpdateEntity(_timeline);
                    }

                    // 判断开工状态
                    if (isPass)
                    {
                        var proj = Uof.IprojectService.GetAll(p => p.id == _needReviewSignedform.project_id).FirstOrDefault();
                        if (proj != null)
                        {
                            var cc = proj.signedforms.Where(c => c.status == (int)LineStatus.OK).Count();
                            var ic = proj.incomes.Where(i => i.status == (int)LineStatus.OK).Count();
                            var tl = Uof.ItimelineService.GetAll(p => p.project_id == proj.id && p.source == SourceType.ProjectStart.ToString() && p.status == (sbyte)LineStatus.OK).Count();
                            if (cc == 0 && ic == 0&&tl==0)
                            {
                                proj.status = (int)ProjectStatus.Ready; // 变为准备中的项目
                                proj.modify_status = (int)ProjectModifyStatus.Normal;
                                proj.date_started = null;
                                Uof.IprojectService.UpdateEntity(proj);
                            }
                        }
                    }
                    break;
                case "modify":
                    // 修改审核通过：
                    // 1. 原始数据状态作废
                    // 2. 待审核所新增的数据状态正常
                    // 3. 时间轴数据状态正常，时间轴的source_id = 待审核所新增的数据的id

                    // 修改审核不通过
                    // 1. 原始数据不变
                    // 2. 待审核所新增的数据作废
                    // 3. 时间轴数据状态正常，source_id不变

                    // 修改审核通过：
                    if (isPass)
                    {
                        // 1. 原始数据状态作废
                        if (_origSignedform != null)
                        {
                            _origSignedform.status = (int)LineStatus.Abandon;
                            _origSignedform.reviewer = currentUser.id;
                            _origSignedform.date_reviewed = DateTime.Now;
                            Uof.IsignedformService.UpdateEntity(_origSignedform);
                        }
                        // 2. 待审核所新增的数据状态正常
                        if (_needReviewSignedform != null)
                        {
                            _needReviewSignedform.status = (int)LineStatus.OK;
                            _needReviewSignedform.review = (int)ReviewStatus.Accept;
                            _needReviewSignedform.reviewer = currentUser.id;
                            _needReviewSignedform.date_reviewed = DateTime.Now;
                            Uof.IsignedformService.UpdateEntity(_needReviewSignedform);
                        }

                        // 3. 时间轴数据正常，source_id = _needReviewContract.id
                        if (_timeline != null)
                        {
                            if (_needReviewSignedform.date_created.GetValueOrDefault().ToString("yyyyMMdd") != _needReviewSignedform.date_signed.GetValueOrDefault().ToString("yyyyMMdd"))
                            {
                                _timeline.content = _needReviewSignedform.date_signed.GetValueOrDefault().ToString("M月d日") + "增补签证￥" + _needReviewSignedform.amount.GetValueOrDefault().ToString("f2") + "元";
                            }
                            else
                            {
                                _timeline.content = "增补签证￥" + _needReviewSignedform.amount.GetValueOrDefault().ToString("f2") + "元";
                            }   
                            _timeline.status = (sbyte)LineStatus.OK;
                            _timeline.source_id = _needReviewSignedform.id;
                            _timeline.date_updated = DateTime.Now;
                            Uof.ItimelineService.UpdateEntity(_timeline);
                        }
                    }
                    else
                    {
                        // 1. 原始数据不变

                        // 2. 待审核所新增的数据作废
                        if (_needReviewSignedform != null)
                        {
                            _needReviewSignedform.status = (int)LineStatus.Abandon;
                            _needReviewSignedform.review = (int)ReviewStatus.Reject;
                            _needReviewSignedform.reviewer = currentUser.id;
                            _needReviewSignedform.date_reviewed = DateTime.Now;
                            Uof.IsignedformService.UpdateEntity(_needReviewSignedform);
                        }
                        // 3. 时间轴数据状态正常，source_id不变
                        if (_timeline != null)
                        {
                            _timeline.status = (sbyte)LineStatus.OK;
                            _timeline.date_updated = DateTime.Now;
                            Uof.ItimelineService.UpdateEntity(_timeline);
                        }
                    }
                    break;
                default:
                    break;
            }

            return true;
        }

        /// <summary>
        /// 收款
        /// </summary>
        /// <param name="id">需要审核的那笔数据id</param>
        /// <param name="optType">操作类型删除还是修改</param>
        /// <param name="isPass">是否审核通过</param>
        /// <returns></returns>
        private bool doIncome(int id, string optType, bool isPass)
        {
            var _source = SourceType.Income.ToString();

            // 需要审核的数据
            var _needReviewIncome = Uof.IincomeService.GetById(id);
            // 原始数据
            var _origIncome = Uof.IincomeService.GetById(_needReviewIncome.batch_id.Value);
            // 时间线数据
            var _timeline = Uof.ItimelineService.GetAll(t => t.source_id == _origIncome.id && t.source == _source).FirstOrDefault();
            //当前操作者
            var currentUser = HttpContext.User.Identity as UserIdentity;
            switch (optType)
            {
                case "delete":
                    // 删除审核通过：
                    // 1. 原始数据作废
                    // 2. 删除新增的那笔数据作废
                    // 3. 时间轴数据数据作废

                    // 删除审核不通过：
                    // 1. 原始数据不变
                    // 2. 删除新增的那笔数据作废
                    // 3. 时间轴数据数据正常

                    // 删除审核通过： 原始数据作废，不通过则不处理原始数据
                    if (isPass && _origIncome != null)
                    {
                        _origIncome.status = (int)LineStatus.Abandon;
                        _origIncome.reviewer = currentUser.id;
                        _origIncome.date_reviewed = DateTime.Now;
                        Uof.IincomeService.UpdateEntity(_origIncome);
                    }

                    // 删除新增的那笔数据作废
                    if (_needReviewIncome != null)
                    {
                        // 不管审核是否通过  删除新增的那笔数据都作废
                        _needReviewIncome.status = (int)LineStatus.Abandon;
                        _needReviewIncome.review = isPass ? (sbyte)ReviewStatus.Accept : (sbyte)ReviewStatus.Reject;
                        _needReviewIncome.reviewer = currentUser.id;
                        _needReviewIncome.date_reviewed = DateTime.Now;
                        Uof.IincomeService.UpdateEntity(_needReviewIncome);
                    }

                    // 时间轴数据
                    if (_timeline != null)
                    {
                        _timeline.status = isPass ? (sbyte)LineStatus.Abandon : (sbyte)LineStatus.OK;
                        _timeline.date_updated = DateTime.Now;
                        Uof.ItimelineService.UpdateEntity(_timeline);
                    }

                    // 判断开工状态
                    if (isPass)
                    {
                        var proj = Uof.IprojectService.GetAll(p => p.id == _needReviewIncome.project_id).FirstOrDefault();
                        if (proj != null)
                        {
                            var cc = proj.signedforms.Where(c => c.status == (int)LineStatus.OK).Count();
                            var ic = proj.incomes.Where(i => i.status == (int)LineStatus.OK).Count();
                            var tl = Uof.ItimelineService.GetAll(p => p.project_id == proj.id && p.source == SourceType.ProjectStart.ToString() && p.status == (sbyte)LineStatus.OK).Count();
                            if (cc == 0 && ic == 0&&tl==0)
                            {
                                proj.status = (int)ProjectStatus.Ready; // 变为准备中的项目
                                proj.modify_status = (int)ProjectModifyStatus.Normal;
                                proj.date_started = null;
                                Uof.IprojectService.UpdateEntity(proj);
                            }
                        }
                    }
                    break;
                case "modify":
                    // 修改审核通过：
                    // 1. 原始数据状态作废
                    // 2. 待审核所新增的数据状态正常
                    // 3. 时间轴数据状态正常，时间轴的source_id = 待审核所新增的数据的id

                    // 修改审核不通过
                    // 1. 原始数据不变
                    // 2. 待审核所新增的数据作废
                    // 3. 时间轴数据状态正常，source_id不变

                    // 修改审核通过：
                    if (isPass)
                    {
                        // 1. 原始数据状态作废
                        if (_origIncome != null)
                        {
                            _origIncome.status = (int)LineStatus.Abandon;
                            _origIncome.reviewer = currentUser.id;
                            _origIncome.date_reviewed = DateTime.Now;
                            Uof.IincomeService.UpdateEntity(_origIncome);
                        }
                        // 2. 待审核所新增的数据状态正常
                        if (_needReviewIncome != null)
                        {
                            _needReviewIncome.status = (int)LineStatus.OK;
                            _needReviewIncome.review = (int)ReviewStatus.Accept;
                            _needReviewIncome.reviewer = currentUser.id;
                            _needReviewIncome.date_reviewed = DateTime.Now;
                            Uof.IincomeService.UpdateEntity(_needReviewIncome);
                        }

                        // 3. 时间轴数据正常，source_id = _needReviewContract.id
                        if (_timeline != null)
                        {
                            if (_needReviewIncome.date_created.GetValueOrDefault().ToString("yyyyMMdd") != _needReviewIncome.date_income.GetValueOrDefault().ToString("yyyyMMdd"))
                            {
                                _timeline.content = _needReviewIncome.date_income.GetValueOrDefault().ToString("M月d日") + "收到项目款￥" + _needReviewIncome.amount.GetValueOrDefault().ToString("f2") + "元";
                            }
                            else
                            {
                                _timeline.content = "收到项目款￥" + _needReviewIncome.amount.GetValueOrDefault().ToString("f2") + "元";
                            }
                            _timeline.status = (sbyte)LineStatus.OK;
                            _timeline.source_id = _needReviewIncome.id;
                            _timeline.date_updated = DateTime.Now;
                            Uof.ItimelineService.UpdateEntity(_timeline);
                        }
                    }
                    else
                    {
                        // 1. 原始数据不变

                        // 2. 待审核所新增的数据作废
                        if (_needReviewIncome != null)
                        {
                            _needReviewIncome.status = (int)LineStatus.Abandon;
                            _needReviewIncome.review = (int)ReviewStatus.Reject;
                            _needReviewIncome.reviewer = currentUser.id;
                            _needReviewIncome.date_reviewed = DateTime.Now;
                            Uof.IincomeService.UpdateEntity(_needReviewIncome);
                        }
                        // 3. 时间轴数据状态正常，source_id不变
                        if (_timeline != null)
                        {
                            _timeline.status = (sbyte)LineStatus.OK;
                            _timeline.date_updated = DateTime.Now;
                            Uof.ItimelineService.UpdateEntity(_timeline);
                        }
                    }
                    break;
                default:
                    break;
            }

            return true;
        }
        
        /// <summary>
        /// 合约
        /// </summary>
        /// <param name="id">需要审核的那笔数据id</param>
        /// <param name="optType">操作类型删除还是修改</param>
        /// <param name="isPass">是否审核通过</param>
        /// <returns></returns>
        public bool doContract(int id, string optType, bool isPass)
        {
            var _source = SourceType.Contract.ToString();

            // 需要审核的数据
            var _needReviewContract = Uof.IcontractService.GetAll(c => c.id == id).FirstOrDefault();
            // 原始数据
            var _origContract = Uof.IcontractService.GetAll(c => c.id == _needReviewContract.batch_id.Value).FirstOrDefault();
            // 时间线数据
            var _timeline = Uof.ItimelineService.GetAll(t => t.source_id == _origContract.id && t.source == _source).FirstOrDefault();
            //当前操作者
            var currentUser = HttpContext.User.Identity as UserIdentity;
            switch (optType)
            {
                case "delete":
                    // 删除审核通过：
                    // 1. 原始数据作废
                    // 2. 删除新增的那笔数据作废
                    // 3. 时间轴数据数据作废

                    // 删除审核不通过：
                    // 1. 原始数据不变
                    // 2. 删除新增的那笔数据作废
                    // 3. 时间轴数据数据正常

                    // 删除审核通过： 原始数据作废，不通过则不处理原始数据
                    if (isPass && _origContract != null)
                    {
                        _origContract.status = (int)LineStatus.Abandon;
                        _origContract.reviewer = currentUser.id;
                        _origContract.date_reviewed = DateTime.Now;
                        Uof.IcontractService.UpdateEntity(_origContract);
                    }

                    // 删除新增的那笔数据作废
                    if (_needReviewContract != null)
                    {
                        // 不管审核是否通过  删除新增的那笔数据都作废
                        _needReviewContract.status = (int)LineStatus.Abandon;
                        _needReviewContract.review = isPass ? (int)ReviewStatus.Accept : (int)ReviewStatus.Reject;
                        _needReviewContract.reviewer = currentUser.id;
                        _needReviewContract.date_reviewed = DateTime.Now;
                        Uof.IcontractService.UpdateEntity(_needReviewContract);
                    }

                    // 时间轴数据
                    if (_timeline != null)
                    {
                        _timeline.status = isPass ? (sbyte)LineStatus.Abandon : (sbyte)LineStatus.OK;
                        _timeline.date_updated = DateTime.Now;
                        Uof.ItimelineService.UpdateEntity(_timeline);
                    }
                    break;
                case "modify":
                    // 修改审核通过：
                    // 1. 原始数据状态作废
                    // 2. 待审核所新增的数据状态正常
                    // 3. 时间轴数据状态正常，时间轴的source_id = 待审核所新增的数据的id

                    // 修改审核不通过
                    // 1. 原始数据不变
                    // 2. 待审核所新增的数据作废
                    // 3. 时间轴数据状态正常，source_id不变

                    // 修改审核通过：
                    if (isPass)
                    {
                        // 1. 原始数据状态作废
                        if (_origContract != null)
                        {
                            _origContract.status = (int)LineStatus.Abandon;
                            _origContract.reviewer = currentUser.id;
                            _origContract.date_reviewed = DateTime.Now;
                            Uof.IcontractService.UpdateEntity(_origContract);
                        }
                        // 2. 待审核所新增的数据状态正常
                        if (_needReviewContract != null)
                        {
                            _needReviewContract.status = (int)LineStatus.OK;
                            _needReviewContract.review = (int)ReviewStatus.Accept;
                            _needReviewContract.reviewer = currentUser.id;
                            _needReviewContract.date_reviewed = DateTime.Now;
                            Uof.IcontractService.UpdateEntity(_needReviewContract);
                        }

                        // 3. 时间轴数据正常，source_id = _needReviewContract.id
                        if (_timeline != null)
                        {
                            _timeline.content = "设置合约金额￥" + _needReviewContract.amount.GetValueOrDefault().ToString("f2") + "元 " + "签订日期为" + _needReviewContract.date_signed.GetValueOrDefault().ToString("yyyy年M月d日");
                            _timeline.status = (sbyte)LineStatus.OK;
                            _timeline.source_id = _needReviewContract.id;
                            _timeline.date_updated = DateTime.Now;
                            Uof.ItimelineService.UpdateEntity(_timeline);
                        }
                    } else
                    {
                        // 1. 原始数据不变

                        // 2. 待审核所新增的数据作废
                        if (_needReviewContract != null)
                        {
                            _needReviewContract.status = (int)LineStatus.Abandon;
                            _needReviewContract.review = (int)ReviewStatus.Reject;
                            _needReviewContract.reviewer = currentUser.id;
                            _needReviewContract.date_reviewed = DateTime.Now;
                            Uof.IcontractService.UpdateEntity(_needReviewContract);
                        }
                        // 3. 时间轴数据状态正常，source_id不变
                        if (_timeline != null)
                        {
                            _timeline.status = (sbyte)LineStatus.OK;
                            _timeline.date_updated = DateTime.Now;
                            Uof.ItimelineService.UpdateEntity(_timeline);
                        }
                    }
                    break;
                default:
                    break;
            }

            return true;
        }
    }
}