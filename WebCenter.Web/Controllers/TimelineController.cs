using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;
using System.Web.Security;
using Newtonsoft.Json;
using System.Text;
using System.Collections;

namespace WebCenter.Web.Controllers
{
    [KgAuthorize]
    public class TimelineController : BaseController
    {
        public TimelineController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        /// <summary>
        /// 时间线列表
        /// </summary>
        /// <param name="project_id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult List(int project_id, int page, int page_size)
        {

            var list = Uof.ItimelineService.GetAll(item => item.project_id == project_id && item.status != (int)LineStatus.Abandon).OrderByDescending(p => p.id).Select(p => new
            {
                content = p.content,
                date_created = p.date_created,
                date_updated = p.date_updated,
                id = p.id,
                opts = p.opts,
                project_id = p.project_id,
                source = p.source,
                source_id = p.source_id,
                status = p.status,
                user_id = p.user_id
            }).ToPagedList(page, page_size);

            var userIds = list.Select(p => p.user_id).Distinct().ToList();
            var userlist = Uof.IuserService.GetAll(item => userIds.Contains(item.id)).Select(p => new { id = p.id, user_name = p.name, is_admin = p.is_admin, photo = p.picture_url }).ToList();
            ArrayList al = new ArrayList();
            foreach (var item in list)
            {
                var user = userlist.Where(p => p.id == item.user_id).FirstOrDefault();
                var obj = new
                {
                    content = item.content,
                    date_created = item.date_created.GetValueOrDefault().ToString("yyyy年MM月dd日"),
                    date_updated = item.date_updated.GetValueOrDefault().ToString("yyyy年MM月dd日"),
                    date_updated_time = item.date_updated.GetValueOrDefault(),
                    date_created_time = item.date_created.GetValueOrDefault(),
                    id = item.id,
                    opts = item.opts,
                    project_id = item.project_id,
                    source = item.source,
                    source_id = item.source_id,
                    status = item.status,
                    user_id = item.user_id,
                    user = user
                };
                al.Add(obj);
            };
            var pageObj = new
            {
                total_count = list.TotalCount,
                list = al

            };
            return Json(pageObj, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取单个时间线对像
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Get(int sourceId, string sourceType)
        {
            if (sourceId <= 0 || string.IsNullOrEmpty(sourceType))
                return base.ErrorResult;
            SourceType type = (SourceType)Enum.Parse(typeof(SourceType), sourceType);
            switch (type)
            {
                case SourceType.Forum:
                    var forum = Uof.IforumService.GetAll(p => p.id == sourceId).Select(p => new
                    {
                        id = p.id,
                        content = p.content,
                        creator = p.creator,
                        date_created = p.date_created,
                        date_updated = p.date_updated,
                        project_id = p.project_id
                    }).FirstOrDefault();
                    return Json(forum, JsonRequestBehavior.AllowGet);
                case SourceType.Contract:
                    var con = Uof.IcontractService.GetAll(p => p.id == sourceId).Select(p => new
                    {
                        amount = p.amount,
                        creator = p.creator,
                        date_created = p.date_created,
                        date_signed = p.date_signed,
                        project_id = p.project_id,
                        status = p.status,
                        batch_id = p.batch_id,
                        id = p.id,
                        date_reviewed = p.date_reviewed,
                        review = p.review,
                        date_updated = p.date_updated
                    }).FirstOrDefault();
                    return Json(con, JsonRequestBehavior.AllowGet);

                case SourceType.Income:
                    var income = Uof.IincomeService.GetAll(p => p.id == sourceId).Select(p => new
                    {
                        amount = p.amount,
                        date_income = p.date_income,
                        date_created = p.date_created,
                        creator = p.creator,
                        project_id = p.project_id,
                        batch_id = p.batch_id,
                        date_reviewed = p.date_reviewed,
                        date_updated = p.date_updated,
                        id = p.id,
                        review = p.review,
                        reviewer = p.reviewer,
                        status = p.status
                    }).FirstOrDefault();
                    return Json(income, JsonRequestBehavior.AllowGet);

                case SourceType.Signedform:
                    var signb = Uof.IsignedformService.GetAll(p => p.id == sourceId).Select(p => new
                    {
                        amount = p.amount,
                        code = p.code,
                        bill_url = p.bill_url,
                        batch_id = p.batch_id,
                        date_signed = p.date_signed,
                        creator = p.creator,
                        date_created = p.date_created,
                        date_updated = p.date_updated,
                        project_id = p.project_id,
                        date_reviewed = p.date_reviewed,
                        id = p.id,
                        review = p.review,
                        reviewer = p.reviewer,
                        status = p.status

                    }).FirstOrDefault();
                    return Json(signb, JsonRequestBehavior.AllowGet);

                case SourceType.Settlement:
                    var set = Uof.IsettlementService.GetAll(p => p.id == sourceId).Select(p => new
                    {
                        amount = p.amount,
                        creator = p.creator,
                        date_signed = p.date_signed,
                        project_id = p.project_id,
                        status = p.status,
                        batch_id = p.batch_id,
                        date_reviewed = p.date_reviewed,
                        date_created = p.date_created,
                        date_updated = p.date_created,
                        id = p.id,
                        review = p.review,
                        reviewer = p.reviewer

                    }).FirstOrDefault();
                    return Json(set, JsonRequestBehavior.AllowGet);

                case SourceType.ProjectStart:
                case SourceType.ProjectFinish:
                    var pro = Uof.IprojectService.GetAll(p => p.id == sourceId).Select(p => new
                    {
                        id = p.id,
                        modify_status = p.modify_status,
                        company_id = p.company_id,
                        creator = p.creator,
                        date_created = p.date_created,
                        date_finished = p.date_finished,
                        date_started = p.date_started,
                        date_updated = p.date_updated,
                        name = p.name,
                        signed = p.signed,
                        status = p.status,
                        type = p.type,
                        area = p.area
                    }).FirstOrDefault();
                    return Json(pro, JsonRequestBehavior.AllowGet);
            }
            return base.ErrorResult;

        }



        /// <summary>
        /// 修改时间线数据
        /// </summary>
        /// <param name="timelineObj">原来时间线对像</param>
        /// <param name="modifyObj">修改的对像</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Modify(string timelineObj, string modifyObj)
        {
            var currentUser = HttpContext.User.Identity as UserIdentity;

            if (currentUser == null)
            {
                return ErrorResult;
            }

            timeline time = new timeline();

            time = JsonConvert.DeserializeObject<timeline>(timelineObj);
            SourceType type = (SourceType)Enum.Parse(typeof(SourceType), time.source, true);
            int sourceId = time.source_id.GetValueOrDefault();
            if (sourceId <= 0)
                return base.ErrorResult;
            StringBuilder sb = new StringBuilder();
            string title = "";


            var waitdealLines = new List<WaitdealLine>();

            switch (type)
            {

                #region 发言 Forum
                case SourceType.Forum:  //发言可马上生效 不需要审批
                    forum forumSource = Uof.IforumService.GetById(sourceId);
                    forum modify = JsonConvert.DeserializeObject<forum>(modifyObj);
                    forumSource.content = modify.content;
                    forumSource.date_updated = DateTime.Now;
                    Uof.IforumService.Save(sourceId, forumSource);   //更新发言表
                    time.content = forumSource.content;   //更新时间线
                    Uof.ItimelineService.UpdateEntity(time);
                    break;
                #endregion

                #region 合约 Contract
                case SourceType.Contract:
                    contract sourceContract = Uof.IcontractService.GetById(sourceId);
                    contract modifyContract = JsonConvert.DeserializeObject<contract>(modifyObj);
                    if (currentUser.is_admin == 1)
                    {
                        sourceContract.amount = modifyContract.amount;
                        sourceContract.date_signed = modifyContract.date_signed;
                        sourceContract.date_updated = DateTime.Now;
                        Uof.IcontractService.UpdateEntity(sourceContract);
                        //同时更新timeline
                        time = Uof.ItimelineService.GetById(time.id);
                        time.date_updated = DateTime.Now;
                        time.content = "设置合约金额￥" + sourceContract.amount.GetValueOrDefault().ToString("f2") + "元 " + "签订日期为" + sourceContract.date_signed.GetValueOrDefault().ToString("yyyy年M月d日");
                        Uof.ItimelineService.UpdateEntity(time);
                        return SuccessResult;
                    }

                    modifyContract.id = 0;
                    modifyContract.batch_id = sourceContract.id;
                    modifyContract.date_created = sourceContract.date_created;
                    modifyContract.date_updated = DateTime.Now;
                    modifyContract.project = null;
                    modifyContract.status = (int)LineStatus.WaitReview;
                    modifyContract.review = (int)ReviewStatus.WaitReview;
                    modifyContract.user = null;
                    modifyContract.user1 = null;
                    modifyContract = Uof.IcontractService.AddEntity(modifyContract);
                    sourceId = modifyContract.id;

                    updateTimelineStatus(time);
                    title = "更改合约";
                    // 项目名称
                    waitdealLines.Add(new WaitdealLine()
                    {
                        icon = "k-building-filled",
                        content = sourceContract.project.name,
                    });

                    if (sourceContract.amount != modifyContract.amount)
                    {
                        //sb.AppendLine("合约金额￥" + sourceContract.amount.ToString());
                        //sb.AppendLine("合约金额￥" + modifyContract.amount.ToString());

                        // 删除
                        waitdealLines.Add(new WaitdealLine()
                        {
                            icon = "k-trash-empty",
                            content = "合约金额￥" + sourceContract.amount.ToString(),
                        });
                        // 修改
                        waitdealLines.Add(new WaitdealLine()
                        {
                            icon = "k-right-circled",
                            content = "合约金额￥" + modifyContract.amount.ToString(),
                        });

                    }
                    if (sourceContract.date_signed.GetValueOrDefault() != modifyContract.date_signed.GetValueOrDefault())
                    {
                        //sb.AppendLine("合约签订日" + sourceContract.date_signed.GetValueOrDefault().ToString("yyyy年MM月dd日"));
                        //sb.AppendLine("合约签订日" + modifyContract.date_signed.GetValueOrDefault().ToString("yyyy年MM月dd日"));

                        // 删除
                        waitdealLines.Add(new WaitdealLine()
                        {
                            icon = "k-trash-empty",
                            content = "合约签订日" + sourceContract.date_signed.GetValueOrDefault().ToString("yyyy年M月d日")
                        });
                        // 修改
                        waitdealLines.Add(new WaitdealLine()
                        {
                            icon = "k-right-circled",
                            content = "合约签订日" + modifyContract.date_signed.GetValueOrDefault().ToString("yyyy年M月d日")
                        });
                    }

                    break;
                #endregion

                #region 收款 Iconme
                case SourceType.Income:

                    income sourceIncome = Uof.IincomeService.GetById(sourceId);
                    income modifyIncome = JsonConvert.DeserializeObject<income>(modifyObj);
                    if (currentUser.is_admin == 1)
                    {
                        sourceIncome.amount = modifyIncome.amount;
                        sourceIncome.date_income = modifyIncome.date_income;
                        sourceIncome.date_updated = DateTime.Now;
                        Uof.IincomeService.UpdateEntity(sourceIncome);
                        //同时更新timeline
                        time = Uof.ItimelineService.GetById(time.id);
                        time.date_updated = DateTime.Now;
                        //time.content = "设置合约金额￥" + sourceContract.amount.GetValueOrDefault().ToString("f2") + "元 " + "签订日期为" + sourceContract.date_signed.GetValueOrDefault().ToString("yyyy年MM月dd日");
                        if (sourceIncome.date_created.GetValueOrDefault().ToString("yyyyMMdd") != sourceIncome.date_income.GetValueOrDefault().ToString("yyyyMMdd"))
                        {
                            time.content = sourceIncome.date_income.GetValueOrDefault().ToString("M月d日") + "收到项目款￥" + sourceIncome.amount.GetValueOrDefault().ToString("f2") + "元";
                        }
                        else
                        {
                            time.content = "收到项目款￥" + sourceIncome.amount.GetValueOrDefault().ToString("f2") + "元";
                        }
                        Uof.ItimelineService.UpdateEntity(time);
                        return SuccessResult;
                    }
                    modifyIncome.id = 0;
                    modifyIncome.batch_id = sourceIncome.id;
                    modifyIncome.date_created = sourceIncome.date_created;
                    modifyIncome.date_updated = DateTime.Now;
                    modifyIncome.status = (sbyte)LineStatus.WaitReview;
                    modifyIncome.review = (sbyte)ReviewStatus.WaitReview;
                    modifyIncome.user = null;
                    modifyIncome.user1 = null;
                    modifyIncome.project = null;
                    modifyIncome = Uof.IincomeService.AddEntity(modifyIncome);
                    sourceId = modifyIncome.id;
                    updateTimelineStatus(time);
                    title = "更改收款信息";

                    waitdealLines.Add(new WaitdealLine()
                    {
                        icon = "k-building-filled",
                        content = sourceIncome.project.name
                    });

                    if (sourceIncome.amount != modifyIncome.amount)
                    {
                        //sb.AppendLine("收款￥" + sourceIncome.amount.ToString());
                        //sb.AppendLine("收款￥" + modifyIncome.amount.ToString());

                        waitdealLines.Add(new WaitdealLine()
                        {
                            icon = "k-trash-empty",
                            content = "收款￥" + sourceIncome.amount.ToString()
                        });
                        waitdealLines.Add(new WaitdealLine()
                        {
                            icon = "k-right-circled",
                            content = "收款￥" + modifyIncome.amount.ToString()
                        });
                    }
                    if (sourceIncome.date_income.GetValueOrDefault() != modifyIncome.date_income.GetValueOrDefault())
                    {
                        //sb.AppendLine("收款日期" + sourceIncome.date_income.GetValueOrDefault().ToString("yyyy年MM月dd日"));
                        //sb.AppendLine("收款日期" + modifyIncome.date_income.GetValueOrDefault().ToString("yyyy年MM月dd日"));

                        waitdealLines.Add(new WaitdealLine()
                        {
                            icon = "k-trash-empty",
                            content = "收款日期" + sourceIncome.date_income.GetValueOrDefault().ToString("yyyy年M月d日")
                        });
                        waitdealLines.Add(new WaitdealLine()
                        {
                            icon = "k-right-circled",
                            content = "收款日期" + modifyIncome.date_income.GetValueOrDefault().ToString("yyyy年M月d日")
                        });
                    }



                    break;
                #endregion

                #region 签证 Signedform
                case SourceType.Signedform:

                    signedform sourceSignedform = Uof.IsignedformService.GetById(sourceId);
                    signedform modifySignedform = JsonConvert.DeserializeObject<signedform>(modifyObj);
                    if (currentUser.is_admin == 1)
                    {
                        sourceSignedform.amount = modifySignedform.amount;
                        sourceSignedform.date_signed = modifySignedform.date_signed;
                        sourceSignedform.date_updated = DateTime.Now;
                        Uof.IsignedformService.UpdateEntity(sourceSignedform);
                        //同时更新timeline
                        time = Uof.ItimelineService.GetById(time.id);
                        time.date_updated = DateTime.Now;
                        if (sourceSignedform.date_created.GetValueOrDefault().ToString("yyyyMMdd") != sourceSignedform.date_signed.GetValueOrDefault().ToString("yyyyMMdd"))
                        {
                            time.content = sourceSignedform.date_signed.GetValueOrDefault().ToString("M月d日") + "增补签证￥" + sourceSignedform.amount.GetValueOrDefault().ToString("f2") + "元";
                        }
                        else
                        {
                            time.content = "增补签证￥" + sourceSignedform.amount.GetValueOrDefault().ToString("f2") + "元";
                        }
                        Uof.ItimelineService.UpdateEntity(time);
                        return SuccessResult;
                    }
                    modifySignedform.id = 0;
                    modifySignedform.batch_id = sourceSignedform.id;
                    modifySignedform.date_created = sourceSignedform.date_created;
                    modifySignedform.date_updated = DateTime.Now;
                    modifySignedform.creator = time.user_id;
                    modifySignedform.status = (int)LineStatus.WaitReview;
                    modifySignedform.review = (sbyte)ReviewStatus.WaitReview;
                    modifySignedform.user1 = null;
                    modifySignedform.user = null;
                    modifySignedform.reviewer = null;
                    modifySignedform.project = null;
                    modifySignedform = Uof.IsignedformService.AddEntity(modifySignedform);
                    sourceId = modifySignedform.id;
                    updateTimelineStatus(time);
                    title = "更改" + sourceSignedform.code + "号签证信息";

                    waitdealLines.Add(new WaitdealLine()
                    {
                        icon = "k-building-filled",
                        content = sourceSignedform.project.name
                    });

                    if (sourceSignedform.amount != modifySignedform.amount)
                    {
                        //sb.AppendLine("签证￥" + sourceSignedform.amount.ToString());
                        //sb.AppendLine("签证￥" + modifySignedform.amount.ToString());

                        waitdealLines.Add(new WaitdealLine()
                        {
                            icon = "k-trash-empty",
                            content = "签证￥" + sourceSignedform.amount.ToString()
                        });
                        waitdealLines.Add(new WaitdealLine()
                        {
                            icon = "k-right-circled",
                            content = "签证￥" + modifySignedform.amount.ToString()
                        });

                    }
                    if (sourceSignedform.date_signed.GetValueOrDefault() != modifySignedform.date_signed.GetValueOrDefault())
                    {
                        //sb.AppendLine("签证日期" + sourceSignedform.date_signed.GetValueOrDefault().ToString("yyyy年MM月dd日"));
                        //sb.AppendLine("签证日期" + modifySignedform.date_signed.GetValueOrDefault().ToString("yyyy年MM月dd日"));

                        waitdealLines.Add(new WaitdealLine()
                        {
                            icon = "k-trash-empty",
                            content = "签证日期" + sourceSignedform.date_signed.GetValueOrDefault().ToString("yyyy年M月d日")
                        });
                        waitdealLines.Add(new WaitdealLine()
                        {
                            icon = "k-right-circled",
                            content = "签证日期" + modifySignedform.date_signed.GetValueOrDefault().ToString("yyyy年M月d日")
                        });
                    }




                    break;
                #endregion

                #region 结算 Settlement
                case SourceType.Settlement:
                    settlement sourceSettlement = Uof.IsettlementService.GetById(sourceId);
                    settlement modifySettlement = JsonConvert.DeserializeObject<settlement>(modifyObj);

                    if (currentUser.is_admin == 1)
                    {
                        sourceSettlement.amount = modifySettlement.amount;
                        sourceSettlement.date_signed = modifySettlement.date_signed;
                        sourceSettlement.date_updated = DateTime.Now;
                        Uof.IsettlementService.UpdateEntity(sourceSettlement);
                        //同时更新timeline
                        time = Uof.ItimelineService.GetById(time.id);
                        time.date_updated = DateTime.Now;
                        time.content = "设置结算金额￥" + sourceSettlement.amount.GetValueOrDefault().ToString("f2") + "元 " + "结算日期为" + sourceSettlement.date_signed.GetValueOrDefault().ToString("yyyy年M月d日");
                        Uof.ItimelineService.UpdateEntity(time);
                        return SuccessResult;
                    }
                    modifySettlement.id = 0;
                    modifySettlement.batch_id = sourceSettlement.id;
                    modifySettlement.date_created = sourceSettlement.date_created;
                    modifySettlement.date_updated = DateTime.Now;
                    modifySettlement.project = null;
                    modifySettlement.status = (sbyte)LineStatus.WaitReview;
                    modifySettlement.review = (sbyte)ReviewStatus.WaitReview;
                    modifySettlement.user = null;
                    modifySettlement.user1 = null;
                    modifySettlement = Uof.IsettlementService.AddEntity(modifySettlement);
                    sourceId = modifySettlement.id;
                    updateTimelineStatus(time);
                    title = "更改结算日期";

                    waitdealLines.Add(new WaitdealLine()
                    {
                        icon = "k-building-filled",
                        content = sourceSettlement.project.name
                    });

                    if (sourceSettlement.amount != modifySettlement.amount)
                    {
                        //sb.AppendLine("结算金额￥" + sourceSettlement.amount.ToString());
                        //sb.AppendLine("结算金额￥" + modifySettlement.amount.ToString());

                        waitdealLines.Add(new WaitdealLine()
                        {
                            icon = "k-trash-empty",
                            content = "结算金额￥" + sourceSettlement.amount.ToString()
                        });
                        waitdealLines.Add(new WaitdealLine()
                        {
                            icon = "k-right-circled",
                            content = "结算金额￥" + modifySettlement.amount.ToString()
                        });
                    }
                    if (sourceSettlement.date_signed.GetValueOrDefault() != modifySettlement.date_signed.GetValueOrDefault())
                    {
                        //sb.AppendLine("结算日期" + sourceSettlement.date_signed.GetValueOrDefault().ToString("yyyy年MM月dd日"));
                        //sb.AppendLine("结算日期" + modifySettlement.date_signed.GetValueOrDefault().ToString("yyyy年MM月dd日"));

                        waitdealLines.Add(new WaitdealLine()
                        {
                            icon = "k-trash-empty",
                            content = "结算日期" + sourceSettlement.date_signed.GetValueOrDefault().ToString("yyyy年M月d日")
                        });
                        waitdealLines.Add(new WaitdealLine()
                        {
                            icon = "k-right-circled",
                            content = "结算日期" + modifySettlement.date_signed.GetValueOrDefault().ToString("yyyy年M月d日")
                        });
                    }



                    break;
                #endregion

                #region 开工 竣工
                case SourceType.ProjectStart:
                case SourceType.ProjectFinish:
                    project sourceProject = Uof.IprojectService.GetById(time.project_id.GetValueOrDefault());
                    project modifyProject = JsonConvert.DeserializeObject<project>(modifyObj);
                    projectmodify pm = new projectmodify();

                    if (currentUser.is_admin == 1)
                    {
                        time = Uof.ItimelineService.GetById(time.id);
                        if (type == SourceType.ProjectStart)
                        {
                            sourceProject.date_started = modifyProject.date_started;
                            //line.content = "设置开工日期为" + date_started.ToString("MM月dd日");//+ "|" + "今日开工";
                            sourceProject.date_updated = DateTime.Now;
                            time.content = "设置开工日期为" + sourceProject.date_started.GetValueOrDefault().ToString("M月d日");//+ "|" + "今日开工";
                        }
                        if (type == SourceType.ProjectFinish)
                        {
                            sourceProject.date_finished = modifyProject.date_finished;
                            sourceProject.date_updated = DateTime.Now;
                            time.content = "设置竣工日期为" + sourceProject.date_finished.GetValueOrDefault().ToString("M月d日");
                        }
                        Uof.IprojectService.UpdateEntity(sourceProject);
                        return SuccessResult;
                    }

                    pm.creator = time.user_id;
                    pm.date_created = DateTime.Now;
                    pm.date_updated = DateTime.Now;
                    pm.review = (int)ReviewStatus.WaitReview;
                    pm.status = (int)LineStatus.WaitReview;
                    pm.project_id = time.project_id;
                    if (type == SourceType.ProjectStart)
                    {
                        sourceProject.modify_status = (int)ProjectModifyStatus.WaitReviewStartModify;
                        Uof.IprojectService.UpdateEntity(sourceProject);  //更改project modify_status
                        // time.content = modifyProject.date_started.GetValueOrDefault(DateTime.Now).ToString("yyyy-MM-dd");


                        pm.date_modified = modifyProject.date_started;
                        pm.modifytype = (int)ProjectModifyType.StartDate;
                        title = "更改开工日期";

                        waitdealLines.Add(new WaitdealLine()
                        {
                            icon = "k-building-filled",
                            content = sourceProject.name
                        });

                        if (sourceProject.date_started != modifyProject.date_started)
                        {
                            //sb.AppendLine("开工日" + sourceProject.date_started.GetValueOrDefault().ToString("yyyy年MM月dd日"));
                            //sb.AppendLine("开工日" + modifyProject.date_started.GetValueOrDefault().ToString("yyyy年MM月dd日"));

                            waitdealLines.Add(new WaitdealLine()
                            {
                                icon = "k-trash-empty",
                                content = "开工日" + sourceProject.date_started.GetValueOrDefault().ToString("yyyy年M月d日")
                            });
                            waitdealLines.Add(new WaitdealLine()
                            {
                                icon = "k-right-circled",
                                content = "开工日" + modifyProject.date_started.GetValueOrDefault().ToString("yyyy年M月d日")
                            });

                        }

                    }
                    else
                    {

                        sourceProject.modify_status = (int)ProjectModifyStatus.WaitReviewEndModify;
                        Uof.IprojectService.UpdateEntity(sourceProject);  //更改project modify_status
                        title = "更改竣工日期";
                        // time.content = modifyProject.date_finished.GetValueOrDefault(DateTime.Now).ToString("yyyy-MM-dd");
                        time.source_id = sourceId;
                        updateTimelineStatus(time);
                        pm.date_modified = modifyProject.date_finished;
                        pm.modifytype = (int)ProjectModifyType.FinishDate;
                        if (sourceProject.date_finished != modifyProject.date_finished)
                        {
                            //sb.AppendLine("竣工日" + sourceProject.date_finished.GetValueOrDefault().ToString("yyyy年MM月dd日"));
                            //sb.AppendLine("竣工日" + modifyProject.date_finished.GetValueOrDefault().ToString("yyyy年MM月dd日"));

                            waitdealLines.Add(new WaitdealLine()
                            {
                                icon = "k-trash-empty",
                                content = "竣工日" + sourceProject.date_finished.GetValueOrDefault().ToString("yyyy年M月d日")
                            });
                            waitdealLines.Add(new WaitdealLine()
                            {
                                icon = "k-right-circled",
                                content = "竣工日" + modifyProject.date_finished.GetValueOrDefault().ToString("yyyy年M月d日")
                            });
                        }

                    }
                    pm = Uof.IprojectmodifyService.AddEntity(pm);
                    sourceId = pm.id;
                    time.source = type.ToString();
                    time.source_id = sourceId;
                    updateTimelineStatus(time);
                    break;
                #endregion

                default:
                    break;
            }
            if (type != SourceType.Forum)
            {
                // if (!string.IsNullOrEmpty(sb.ToString()))
                if (waitdealLines.Count > 0)
                {
                    waitdeal wd = new waitdeal();
                    //switch (type)
                    //{
                    //    case SourceType.Forum:
                    //        break;
                    //    case SourceType.Contract:
                    //        wd.title = "更改合约";
                    //        break;
                    //    case SourceType.Income:
                    //        wd.title = "更改收款信息";
                    //        break;
                    //    case SourceType.Signedform:
                    //        wd.title = "更改：合约";
                    //        break;
                    //    case SourceType.Settlement:
                    //        wd.title = "更改结算信息";
                    //        break;
                    //    case SourceType.ProjectStart:
                    //        wd.title = "更改开工日期";
                    //        break;
                    //    case SourceType.ProjectFinish:
                    //        wd.title = "更改竣工日期";
                    //        break;
                    //    case SourceType.JoinCompany:
                    //        break;
                    //    default:
                    //        break;
                    //}

                    wd.title = title;

                    wd.source = time.source;
                    wd.project_id = time.project_id;
                    wd.source_id = sourceId;
                    wd.creator = currentUser.id;  // TODO: 发起人
                    wd.status = (sbyte)LineStatus.WaitReview;
                    wd.content = JsonConvert.SerializeObject(waitdealLines); // sb.ToString();
                    wd.company_id = getCompanyIdByProjectId(time.project_id.GetValueOrDefault());
                    wd.date_created = DateTime.Now;
                    wd.date_updated = DateTime.Now;
                    Uof.IwaitdealService.AddEntity(wd);
                }
            }

            return base.SuccessResult;
        }

        private void updateTimelineStatus(timeline time)
        {
            time.status = (sbyte)LineStatus.WaitReview;
            Uof.ItimelineService.Save(time.id, time);
        }

        [HttpPost]
        public ActionResult RemoveTimeLine(int timeLineId)
        {
            var currentUser = HttpContext.User.Identity as UserIdentity;
            if (currentUser == null)
            {
                return ErrorResult;
            }
            bool isok = false;
            timeline line = Uof.ItimelineService.GetById(timeLineId);
            int company_id = getCompanyIdByProjectId(line.project_id.GetValueOrDefault());
            
            if (line != null)
            {
                if (currentUser.is_admin == 1)
                {
                    line.status =(sbyte)LineStatus.Abandon;
                    line.date_updated = DateTime.Now;
                    Uof.ItimelineService.UpdateEntity(line);
                    project proj = Uof.IprojectService.GetById(line.project_id.GetValueOrDefault());
                    if (line.source == SourceType.ProjectStart.ToString())
                    {

                        var cc = proj.signedforms.Where(c => c.status == (int)LineStatus.OK).Count();
                        var ic = proj.incomes.Where(i => i.status == (int)LineStatus.OK).Count();
                        if (cc == 0 && ic == 0)
                        {
                            proj.status = (int)ProjectStatus.Ready; // 变为准备中的项目
                        }

                        proj.modify_status = (int)ProjectModifyStatus.Normal;
                        proj.date_started = null;
                        Uof.IprojectService.UpdateEntity(proj);  
                    }
                    if (line.source == SourceType.ProjectFinish.ToString())
                    {
                        // 如果有结算记录则竣工状态不变
                        var sc = proj.settlements.Where(c => c.status == (int)LineStatus.OK).Count();
                        if (sc == 0)
                        {
                            // 如果没有签证记录 并且没有收款记录 则变为准备中的项目
                            var cc = proj.signedforms.Where(c => c.status == (int)LineStatus.OK).Count();
                            var ic = proj.incomes.Where(i => i.status == (int)LineStatus.OK).Count();
                            if (cc == 0 && ic == 0)
                            {
                                proj.status = (int)ProjectStatus.Ready; // 变为准备中的项目
                            }
                            else
                            {
                                proj.status = (int)ProjectStatus.Starting; // 变为开工中的项目
                            }
                        }

                        proj.modify_status = (int)ProjectModifyStatus.Normal;
                        proj.date_finished = null;
                        Uof.IprojectService.UpdateEntity(proj);
                    }
                    if (line.source==SourceType.Contract.ToString())
                    {
                        contract cont = Uof.IcontractService.GetById(line.source_id.GetValueOrDefault());
                        cont.status = (int)LineStatus.Abandon;
                        cont.date_updated = DateTime.Now;
                        Uof.IcontractService.UpdateEntity(cont); 
                    }

                   
                    if (line.source == SourceType.Settlement.ToString())
                    {
                        settlement sett = Uof.IsettlementService.GetById(line.source_id.GetValueOrDefault());
                        sett.status = (int)LineStatus.Abandon;
                        sett.date_updated = DateTime.Now;
                        Uof.IsettlementService.UpdateEntity(sett);
                    }
                    if (line.source == SourceType.Income.ToString())
                    {
                        income inco = Uof.IincomeService.GetById(line.source_id.GetValueOrDefault());
                        inco.status = (int)LineStatus.Abandon;
                        inco.date_updated = DateTime.Now;
                        Uof.IincomeService.UpdateEntity(inco);
                        //检查是否更新开工状态
                        
                        if (proj != null)
                        {
                            var cc = proj.signedforms.Where(c => c.status == (int)LineStatus.OK).Count();
                            var ic = proj.incomes.Where(i => i.status == (int)LineStatus.OK&&i.id!=inco.id).Count();
                            var tl = Uof.ItimelineService.GetAll(p => p.project_id == proj.id && p.source == SourceType.ProjectStart.ToString() && p.status == (sbyte)LineStatus.OK).Count();
                            if (cc == 0 && ic == 0 && tl == 0)                               
                                {
                                    proj.status = (int)ProjectStatus.Ready; // 变为准备中的项目
                                    proj.modify_status = (int)ProjectModifyStatus.Normal;
                                    proj.date_started = null;
                                    Uof.IprojectService.UpdateEntity(proj);  
                                }
                           
                        }
                    }  
                    if (line.source == SourceType.Signedform.ToString())
                    {
                        signedform sign = Uof.IsignedformService.GetById(line.source_id.GetValueOrDefault());
                        sign.status = (int)LineStatus.Abandon;
                        sign.date_updated = DateTime.Now;
                        Uof.IsignedformService.UpdateEntity(sign);
                        if (proj != null)
                        {
                            var cc = proj.signedforms.Where(c => c.status == (int)LineStatus.OK&&c.id!=sign.id).Count();
                            var ic = proj.incomes.Where(i => i.status == (int)LineStatus.OK).Count();
                            var tl = Uof.ItimelineService.GetAll(p => p.project_id == proj.id && p.source == SourceType.ProjectStart.ToString() && p.status == (sbyte)LineStatus.OK).Count();
                            if (cc == 0 && ic == 0 && tl == 0)
                            {
                                proj.status = (int)ProjectStatus.Ready; // 变为准备中的项目
                                proj.modify_status = (int)ProjectModifyStatus.Normal;
                                proj.date_started = null;
                                Uof.IprojectService.UpdateEntity(proj);
                            }
                            
                        }
                    }
                    return SuccessResult; 
                }
                int sourceId = line.source_id.GetValueOrDefault(0);
                SourceType type = (SourceType)Enum.Parse(typeof(SourceType), line.source, true);

                switch (type)
                {
                    case SourceType.Forum:
                        forum form = Uof.IforumService.GetById(sourceId);
                        if (form != null)
                        {
                            Uof.IforumService.DeleteEntity(form);
                        }
                        line.date_updated = DateTime.Now;
                        line.status = (SByte)LineStatus.Abandon;
                        Uof.ItimelineService.UpdateEntity(line);
                        isok = true;
                        break;
                    case SourceType.Contract:
                        contract cont = Uof.IcontractService.GetById(sourceId);
                        if (cont != null)
                        {
                            cont.status = (int)LineStatus.WaitReview;
                            cont.batch_id = cont.id;
                            contract addCont = new contract();
                            addCont.project_id = cont.project_id;
                            addCont.amount = cont.amount;
                            addCont.date_signed = cont.date_signed;
                            addCont.creator = currentUser.id;
                            addCont.date_created = DateTime.Now;
                            addCont.id = 0;
                            addCont.batch_id = cont.id;
                            addCont.project = null;
                            addCont.user = null;
                            addCont.user1 = null;
                            addCont.review = (sbyte)ReviewStatus.WaitReview;
                            addCont.status = (sbyte)LineStatus.WaitReview;

                            addCont = Uof.IcontractService.AddEntity(addCont);
                            if (addCont.id > 0)
                            {
                                waitdeal wd = new waitdeal();
                                wd.title = "删除合约";

                                //StringBuilder sb = new StringBuilder();
                                //sb.AppendLine(string.Format("合约价络￥{0}", addCont.amount.ToString()));
                                //sb.AppendLine(string.Format("{0}", addCont.date_signed.GetValueOrDefault().ToString("yyyy年MM月dd日")));

                                var waitdealLines = new List<WaitdealLine>();
                                waitdealLines.Add(new WaitdealLine()
                                {
                                    icon = "k-building-filled",
                                    content = cont.project.name
                                });
                                waitdealLines.Add(new WaitdealLine()
                                {
                                    icon = "k-trash-empty",
                                    content = string.Format("合约价络￥{0}", addCont.amount.ToString())
                                });
                                waitdealLines.Add(new WaitdealLine()
                                {
                                    icon = "k-trash-empty",
                                    content = string.Format("{0}", addCont.date_signed.GetValueOrDefault().ToString("yyyy年M月d日"))
                                });
                                waitdealLines.Add(new WaitdealLine()
                                {
                                    icon = "k-attention-circled",
                                    content = "删除合约将影响项目状态"
                                });


                                wd.source = line.source;
                                wd.source_id = addCont.id;
                                wd.content = JsonConvert.SerializeObject(waitdealLines); // sb.ToString();
                                wd.project_id = line.project_id;
                                wd.company_id = company_id;
                                wd.status = (sbyte)ReviewStatus.WaitReview;
                                wd.date_created = DateTime.Now;
                                wd.date_updated = DateTime.Now;
                                wd.creator = currentUser.id;
                                Uof.IwaitdealService.AddEntity(wd);
                                line.date_updated = DateTime.Now;
                                line.status = (SByte)LineStatus.WaitDeleteReview;
                                Uof.ItimelineService.UpdateEntity(line);
                                isok = true;
                            }

                        }
                        break;
                    case SourceType.Income:
                        income inc = Uof.IincomeService.GetById(sourceId);
                        if (inc != null)
                        {
                            inc.status = (int)LineStatus.WaitReview;

                            income addInc = new income();
                            addInc.project_id = inc.project_id;
                            addInc.amount = inc.amount;
                            addInc.creator = currentUser.id;
                            addInc.date_income = inc.date_income;
                            addInc.review = (sbyte)ReviewStatus.WaitReview;

                            addInc.date_created = DateTime.Now;
                            addInc.id = 0;
                            addInc.batch_id = inc.id;
                            addInc.status = (sbyte)LineStatus.WaitReview;
                            addInc.project = null;
                            addInc.user = null;
                            addInc.user1 = null;
                            addInc = Uof.IincomeService.AddEntity(addInc);
                            if (addInc.id > 0)
                            {
                                waitdeal wd = new waitdeal();
                                wd.title = "删除收款信息";

                                //StringBuilder sb = new StringBuilder();
                                //sb.AppendLine(string.Format("收款￥{0}", addInc.amount.ToString()));
                                //sb.AppendLine(string.Format("收款日期{0}", addInc.date_income.GetValueOrDefault().ToString("yyyy年MM月dd日")));

                                var waitdealLines = new List<WaitdealLine>();
                                waitdealLines.Add(new WaitdealLine()
                                {
                                    icon = "k-building-filled",
                                    content = addInc.project.name
                                });
                                waitdealLines.Add(new WaitdealLine()
                                {
                                    icon = "k-trash-empty",
                                    content = string.Format("收款￥{0}", addInc.amount.ToString())
                                });
                                waitdealLines.Add(new WaitdealLine()
                                {
                                    icon = "k-trash-empty",
                                    content = string.Format("收款日期{0}", addInc.date_income.GetValueOrDefault().ToString("yyyy年M月d日"))
                                });
                                waitdealLines.Add(new WaitdealLine()
                                {
                                    icon = "k-attention-circled",
                                    content = "删除该项将影响总拿款金额"
                                });

                                wd.source = line.source;
                                wd.source_id = addInc.id;
                                wd.content = JsonConvert.SerializeObject(waitdealLines); //sb.ToString();
                                wd.project_id = line.project_id;
                                wd.status = (sbyte)ReviewStatus.WaitReview;
                                wd.date_created = DateTime.Now;
                                wd.company_id = company_id;
                                wd.date_updated = DateTime.Now;
                                wd.creator = currentUser.id;
                                Uof.IwaitdealService.AddEntity(wd);
                                line.date_updated = DateTime.Now;
                                line.status = (SByte)LineStatus.WaitDeleteReview;
                                Uof.ItimelineService.UpdateEntity(line);
                                isok = true;
                            }
                           
                           

                        }
                        break;
                    case SourceType.Signedform:

                        signedform sign = Uof.IsignedformService.GetById(sourceId);
                        if (sign != null)
                        {
                            sign.status = (int)LineStatus.WaitReview;

                            signedform addsign = new signedform();
                            addsign.project_id = sign.project_id;
                            addsign.amount = sign.amount;
                            addsign.code = sign.code;
                            addsign.date_signed = sign.date_signed;
                            addsign.date_updated = sign.date_updated;
                            addsign.creator = currentUser.id;
                            addsign.date_created = DateTime.Now;
                            addsign.id = 0;
                            addsign.batch_id = sign.id;
                            addsign.status = (sbyte)LineStatus.WaitReview;
                            addsign.review = (sbyte)ReviewStatus.WaitReview;
                            addsign.project = null;
                            addsign.user = null;
                            addsign.user1 = null;
                            addsign = Uof.IsignedformService.AddEntity(addsign);
                            if (addsign.id > 0)
                            {
                                waitdeal wd = new waitdeal();
                                wd.title = string.Format("删除{0}号签证信息", addsign.code.ToString());

                                //StringBuilder sb = new StringBuilder();
                                //sb.AppendLine(string.Format("签证￥{0}", addsign.amount.ToString()));
                                //sb.AppendLine(string.Format("签证日期{0}", addsign.date_signed.GetValueOrDefault().ToString("yyyy年MM月dd日")));

                                var waitdealLines = new List<WaitdealLine>();
                                waitdealLines.Add(new WaitdealLine()
                                {
                                    icon = "k-building-filled",
                                    content = addsign.project.name
                                });
                                waitdealLines.Add(new WaitdealLine()
                                {
                                    icon = "k-trash-empty",
                                    content = string.Format("签证￥{0}", addsign.amount.ToString())
                                });
                                waitdealLines.Add(new WaitdealLine()
                                {
                                    icon = "k-trash-empty",
                                    content = string.Format("签证日期{0}", addsign.date_signed.GetValueOrDefault().ToString("yyyy年M月d日"))
                                });
                                waitdealLines.Add(new WaitdealLine()
                                {
                                    icon = "k-attention-circled",
                                    content = "删除该项将影响总签证金额"
                                });

                                wd.source = line.source;
                                wd.source_id = addsign.id;
                                wd.company_id = company_id;
                                wd.content = JsonConvert.SerializeObject(waitdealLines); // sb.ToString();
                                wd.project_id = line.project_id;
                                wd.creator = currentUser.id;
                                wd.status = (sbyte)ReviewStatus.WaitReview;
                                wd.date_created = DateTime.Now;
                                wd.date_updated = DateTime.Now;
                                Uof.IwaitdealService.AddEntity(wd);
                                line.date_updated = DateTime.Now;
                                line.status = (SByte)LineStatus.WaitDeleteReview;
                                Uof.ItimelineService.UpdateEntity(line);
                                isok = true;
                            }
                            //todo 检查是否改变开工状态

                        }
                        break;
                    case SourceType.Settlement:

                        settlement settle = Uof.IsettlementService.GetById(sourceId);
                        if (settle != null)
                        {
                            settle.status = (int)LineStatus.WaitReview;
                            settlement addSettle = new settlement();
                            addSettle.project_id = settle.project_id;
                            addSettle.amount = settle.amount;
                            addSettle.date_signed = settle.date_signed;
                            addSettle.creator = currentUser.id;
                            addSettle.date_created = DateTime.Now;
                            addSettle.id = 0;
                            addSettle.batch_id = settle.id;
                            addSettle.review = (sbyte)ReviewStatus.WaitReview;
                            addSettle.status = (sbyte)LineStatus.WaitReview;
                            addSettle.project = null;
                            addSettle.user = null;
                            addSettle.user1 = null;
                            addSettle = Uof.IsettlementService.AddEntity(addSettle);
                            if (addSettle.id > 0)
                            {
                                waitdeal wd = new waitdeal();
                                wd.title = "删除结算信息";

                                //StringBuilder sb = new StringBuilder();
                                //sb.AppendLine(string.Format("结算￥{0}", addSettle.amount.ToString()));
                                //sb.AppendLine(string.Format("结算日期{0}", addSettle.date_signed.GetValueOrDefault().ToString("yyyy年MM月dd日")));

                                var waitdealLines = new List<WaitdealLine>();
                                waitdealLines.Add(new WaitdealLine()
                                {
                                    icon = "k-building-filled",
                                    content = addSettle.project.name
                                });
                                waitdealLines.Add(new WaitdealLine()
                                {
                                    icon = "k-trash-empty",
                                    content = string.Format("结算￥{0}", addSettle.amount.ToString())
                                });
                                waitdealLines.Add(new WaitdealLine()
                                {
                                    icon = "k-trash-empty",
                                    content = string.Format("结算日期{0}", addSettle.date_signed.GetValueOrDefault().ToString("yyyy年M月d日"))
                                });
                                waitdealLines.Add(new WaitdealLine()
                                {
                                    icon = "k-attention-circled",
                                    content = "删除该项目，系统将暂时以合约价格＋签证价格的方式计算总价"
                                });

                                wd.source = line.source;
                                wd.source_id = addSettle.id;
                                wd.company_id = company_id;
                                wd.content = JsonConvert.SerializeObject(waitdealLines); // sb.ToString();
                                wd.project_id = line.project_id;
                                wd.creator = currentUser.id;
                                wd.status = (sbyte)ReviewStatus.WaitReview;
                                wd.date_created = DateTime.Now;
                                wd.date_updated = DateTime.Now;
                                Uof.IwaitdealService.AddEntity(wd);
                                line.date_updated = DateTime.Now;
                                line.status = (SByte)LineStatus.WaitDeleteReview;
                                Uof.ItimelineService.UpdateEntity(line);
                                isok = true;
                            }
                        }
                        break;
                    case SourceType.ProjectStart:
                        project proj = Uof.IprojectService.GetById(line.project_id.GetValueOrDefault());
                        if (proj != null)
                        {
                            proj.modify_status = (int)ProjectModifyStatus.WaitReviewStartDelete;
                            Uof.IprojectService.UpdateEntity(proj);
                            projectmodify pm = new projectmodify();

                            pm.date_updated = DateTime.Now;
                            pm.review = (int)ReviewStatus.WaitReview;
                            pm.status = (int)LineStatus.WaitReview;
                            pm.project_id = line.project_id;
                            pm.modifytype = (int)ProjectModifyType.StartDate;
                            pm.creator = currentUser.id;
                            pm = Uof.IprojectmodifyService.AddEntity(pm);


                            waitdeal wd = new waitdeal();
                            wd.title = "删除开工日期";

                            //StringBuilder sb = new StringBuilder();
                            //sb.AppendLine(string.Format("开工日期{0}", proj.date_started.GetValueOrDefault().ToString("yyyy年MM月dd日")));

                            var waitdealLines = new List<WaitdealLine>();
                            waitdealLines.Add(new WaitdealLine()
                            {
                                icon = "k-building-filled",
                                content = proj.name
                            });
                            waitdealLines.Add(new WaitdealLine()
                            {
                                icon = "k-trash-empty",
                                content = string.Format("开工日期{0}", proj.date_started.GetValueOrDefault().ToString("yyyy年M月d日"))
                            });
                            waitdealLines.Add(new WaitdealLine()
                            {
                                icon = "k-attention-circled",
                                content = "删除开工时间项目状态将变更为“准备的项目”"
                            });

                            wd.source = SourceType.ProjectStart.ToString();
                            wd.source_id = pm.id;

                            wd.content = JsonConvert.SerializeObject(waitdealLines); // sb.ToString();
                            wd.project_id = line.project_id;
                            wd.company_id = company_id;
                            wd.status = (sbyte)ReviewStatus.WaitReview;
                            wd.date_created = DateTime.Now;
                            wd.date_updated = DateTime.Now;
                            wd.creator = currentUser.id;
                            Uof.IwaitdealService.AddEntity(wd);
                            line.date_updated = DateTime.Now;
                            line.source_id = pm.id;
                            line.source = SourceType.ProjectStart.ToString();
                            line.status = (SByte)LineStatus.WaitDeleteReview;
                            Uof.ItimelineService.UpdateEntity(line);

                            isok = true;
                        }
                        break;
                    case SourceType.ProjectFinish:

                        project endProj = Uof.IprojectService.GetById(line.project_id.GetValueOrDefault());
                        if (endProj != null)
                        {

                            endProj.modify_status = (int)ProjectModifyStatus.WaitReviewEndDelete;
                            Uof.IprojectService.UpdateEntity(endProj);
                            projectmodify pm = new projectmodify();
                            pm.date_updated = DateTime.Now;
                            pm.review = (int)ReviewStatus.WaitReview;
                            pm.status = (int)LineStatus.WaitReview;
                            pm.project_id = line.project_id;
                            pm.modifytype = (int)ProjectModifyType.StartDate;
                            pm.creator = currentUser.id;
                            pm = Uof.IprojectmodifyService.AddEntity(pm);


                            waitdeal wd = new waitdeal();
                            wd.title = "删除竣工日期";

                            //StringBuilder sb = new StringBuilder();
                            //sb.AppendLine(string.Format("竣工日期{0}", endProj.date_finished.GetValueOrDefault().ToString("yyyy年MM月dd日")));

                            var waitdealLines = new List<WaitdealLine>();
                            waitdealLines.Add(new WaitdealLine()
                            {
                                icon = "k-building-filled",
                                content = endProj.name
                            });
                            waitdealLines.Add(new WaitdealLine()
                            {
                                icon = "k-trash-empty",
                                content = string.Format("竣工日期{0}", endProj.date_finished.GetValueOrDefault().ToString("yyyy年M月d日"))
                            });
                            waitdealLines.Add(new WaitdealLine()
                            {
                                icon = "k-attention-circled",
                                content = "删除开工时间项目状态将变更为“开工的项目”"
                            });

                            wd.source = SourceType.ProjectFinish.ToString();
                            wd.source_id = pm.id;
                            wd.content = JsonConvert.SerializeObject(waitdealLines); // sb.ToString();
                            wd.creator = currentUser.id;
                            wd.creator = currentUser.id;
                            wd.project_id = line.project_id;
                            wd.company_id = company_id;
                            wd.status = (sbyte)ReviewStatus.WaitReview;
                            wd.date_created = DateTime.Now;
                            wd.date_updated = DateTime.Now;
                            Uof.IwaitdealService.AddEntity(wd);
                            line.date_updated = DateTime.Now;
                            line.source_id = pm.id;
                            line.source = SourceType.ProjectFinish.ToString();
                            line.status = (SByte)LineStatus.WaitDeleteReview;
                            Uof.ItimelineService.UpdateEntity(line);
                            isok = true;
                        }
                        break;
                    case SourceType.JoinCompany:
                        break;
                    case SourceType.Archive:
                        break;
                    case SourceType.Revert:
                        break;
                    default:
                        break;
                }
            }
            if (isok)
            {
                return SuccessResult;

            }
            else
                return ErrorResult;
        }


        [HttpGet]
        public ActionResult Logout()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            return Redirect("/index.html");
        }
        //需要授权才可访问
        [Authorize]
        public ActionResult someAction()
        {
            return Content("");
        }

        [HttpGet]
        public ActionResult Test()
        {
            user u = new user();
            u.mobile = "13806013024";
            u.ExtendProperty.Add("SS", "BB");
            u.ExtendProperty.Add("DD", "EE");
            return Json(u, JsonRequestBehavior.AllowGet);
        }
    }
}