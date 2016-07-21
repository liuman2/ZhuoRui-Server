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
using System.Text;

namespace WebCenter.Web.Controllers
{
    [KgAuthorize]
    public class ActionsController : BaseController
    {
        public ActionsController(IUnitOfWork UOF)
            : base(UOF)
        {

        }


        /// <summary>        
        //发言
        /// </summary>
        /// <param name="fum"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SendForum(forum fum)
        {

            //todo sendform
            forum _forum = fum;
            if (_forum.id > 0)
            {
                var objForum = Uof.IforumService.GetById(_forum.id);
                objForum.date_updated = DateTime.Now;
                objForum.creator = _forum.creator;
                objForum.content = _forum.content;
                bool i = Uof.IforumService.UpdateEntity(objForum);
                if (i)
                {
                    //修改timeline
                    var obj = Uof.ItimelineService.GetAll(it => it.source_id == _forum.id && it.project_id == _forum.project_id && it.source == SourceType.Forum.ToString()).FirstOrDefault();
                    obj.date_updated = DateTime.Now;
                    obj.content = _forum.content;
                    bool b = Uof.ItimelineService.UpdateEntity(obj);
                    if (b)
                        return SuccessResult;
                }

            }
            else
            {
                _forum.date_created = DateTime.Now;
                _forum.date_updated = _forum.date_created;
                _forum = Uof.IforumService.AddEntity(_forum);
                if (_forum.id > 0)
                {
                    //添加Timeline
                    timeline line = new timeline();
                    line.date_created = DateTime.Now;
                    line.date_updated = line.date_created;
                    line.content = _forum.content;
                    line.source_id = _forum.id;
                    line.project_id = _forum.project_id;
                    line.source = SourceType.Forum.ToString();
                    line.status = (sbyte)LineStatus.OK;
                    line.user_id = _forum.creator;
                    line.opts = (int)OptType.ModifyDelete;

                    line = Uof.ItimelineService.AddEntity(line);
                    if (line.id > 0)
                    {
                        return SuccessResult;
                    }
                }

            }

            return ErrorResult;
        }

        /// <summary>
        /// 添加合约
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddContract(contract con)
        {

            //todo sendform
            contract _contract = con;
            _contract.date_created = DateTime.Now;
            _contract.review = (sbyte)ReviewStatus.Accept;
            _contract.date_updated = _contract.date_created;
            _contract.status = (int)LineStatus.OK;
            _contract = Uof.IcontractService.AddEntity(_contract);
            if (_contract.id > 0)
            {
                //添加Timeline
                timeline line = new timeline();
                line.date_created = DateTime.Now;
                line.date_updated = line.date_created;
                line.content = "设置合约金额￥" + _contract.amount.GetValueOrDefault().ToString("f2") + "元 " + "签订日期为" + _contract.date_signed.GetValueOrDefault().ToString("yyyy年M月d日");
                line.source_id = _contract.id;
                line.project_id = _contract.project_id;
                line.source = SourceType.Contract.ToString();
                line.status = (sbyte)LineStatus.OK;
                line.user_id = _contract.creator;
                line.opts = (int)OptType.ModifyDelete;
                line = Uof.ItimelineService.AddEntity(line);
                if (line.id > 0)
                {
                    return SuccessResult;
                }
            }
            return ErrorResult;
        }

        /// <summary>
        /// 开工
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult StarProject(int project_id, int user_id, DateTime date_started)
        {
            var proj = Uof.IprojectService.GetById(project_id);
            proj.status = (int)ProjectStatus.Starting;
            proj.date_started = date_started;
            proj.date_updated = DateTime.Now;

            bool b = Uof.IprojectService.UpdateEntity(proj);
            if (b)
            {
                //添加Timeline
                timeline line = new timeline();
                line.date_created = DateTime.Now;
                line.date_updated = line.date_created;
                line.content = "设置开工日期为" + date_started.ToString("M月d日");//+ "|" + "今日开工";
                line.source_id = project_id;
                line.project_id = project_id;
                line.source = SourceType.ProjectStart.ToString();
                line.status = (sbyte)LineStatus.OK;
                line.user_id = user_id;
                line.opts = (int)OptType.ModifyDelete;
                line = Uof.ItimelineService.AddEntity(line);
                if (line.id > 0)
                {
                    return SuccessResult;
                }
            }
            return ErrorResult;
        }

        /// <summary>
        /// 竣工
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FinishedProject(int project_id, int user_id, DateTime date_finished)
        {
            var proj = Uof.IprojectService.GetById(project_id);
            proj.status = (int)ProjectStatus.Finished;
            proj.date_finished = date_finished;
            proj.date_updated = DateTime.Now;
            bool b = Uof.IprojectService.UpdateEntity(proj);
            if (b)
            {
                //添加Timeline
                timeline line = new timeline();
                line.date_created = DateTime.Now;
                line.date_updated = line.date_created;
                line.content = "设置竣工日期为" + date_finished.ToString("M月d日"); // + "|" + "验收完毕，项目竣工";
                line.source_id = project_id;
                line.project_id = project_id;
                line.source = SourceType.ProjectFinish.ToString();
                line.status = (sbyte)LineStatus.OK;
                line.user_id = user_id;
                line.opts = (int)OptType.ModifyDelete;
                line = Uof.ItimelineService.AddEntity(line);
                if (line.id > 0)
                {
                    return SuccessResult;
                }
            }
            return ErrorResult;

        }

        /// <summary>
        ///收款
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddIncome(income income)
        {
            income _income = income;
            _income.date_created = DateTime.Now;
            _income.review = (sbyte)ReviewStatus.Accept;
            _income.status = (sbyte)LineStatus.OK;
            _income.date_updated = _income.date_created;
            _income.status = (int)LineStatus.OK;
            _income = Uof.IincomeService.AddEntity(_income);
            if (_income.id > 0)
            {
                //添加Timeline
                timeline line = new timeline();
                line.date_created = DateTime.Now;
                line.date_updated = line.date_created;
                if (_income.date_created.GetValueOrDefault().ToString("yyyyMMdd") != _income.date_income.GetValueOrDefault().ToString("yyyyMMdd"))
                {
                    line.content = _income.date_income.GetValueOrDefault().ToString("M月d日") + "收到项目款￥" + _income.amount.GetValueOrDefault().ToString("f2") + "元";
                }
                else
                {
                    line.content = "收到项目款￥" + _income.amount.GetValueOrDefault().ToString("f2") + "元";
                }

                line.source_id = _income.id;
                line.project_id = _income.project_id;
                line.source = SourceType.Income.ToString();
                line.status = (sbyte)LineStatus.OK;
                line.user_id = _income.creator;
                line.opts = (int)OptType.ModifyDelete;
                line = Uof.ItimelineService.AddEntity(line);

                // 新增收款，如果项目状态还未开工 则变成开工
                var _p = Uof.IprojectService.GetAll(p => p.id == _income.project_id).FirstOrDefault();
                if (_p != null && _p.status != (int)ProjectStatus.Finished && _p.status != (int)ProjectStatus.Starting)
                {
                    _p.status = (int)ProjectStatus.Starting;
                    Uof.IprojectService.UpdateEntity(_p);
                }

                if (line.id > 0)
                {
                    return SuccessResult;
                }
            }
            return ErrorResult;

        }

        /// <summary>
        ///签证单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddSignForm(signedform sign)
        {
            signedform _sign = sign;
            _sign.date_created = DateTime.Now;
            _sign.review = (sbyte)ReviewStatus.Accept;
            _sign.status = (sbyte)LineStatus.OK;
            _sign.date_updated = _sign.date_created;
            _sign.status = (int)LineStatus.OK;
            _sign.bill_url = sign.bill_url;
            var list = Uof.IsignedformService.GetAll(it => it.project_id == _sign.project_id).Select(p => p.code).ToList();
            if (list.Count == 0)
                _sign.code = "01";
            else
            {
                int max = 0;
                foreach (var item in list)
                {
                    try
                    {
                        int t = Convert.ToInt32(item);
                        if (t > max)
                            max = t;
                    }
                    catch { }
                }
                _sign.code = "0" + (max + 1).ToString();
            }
            _sign = Uof.IsignedformService.AddEntity(_sign);
            if (_sign.id > 0)
            {
                //添加Timeline
                timeline line = new timeline();
                line.date_created = DateTime.Now;
                line.date_updated = line.date_created;
                if (_sign.date_created.GetValueOrDefault().ToString("yyyyMMdd") != _sign.date_signed.GetValueOrDefault().ToString("yyyyMMdd"))
                {
                    line.content = _sign.date_signed.GetValueOrDefault().ToString("M月d日") + "增补签证￥" + _sign.amount.GetValueOrDefault().ToString("f2") + "元";
                }
                else
                {
                    line.content = "增补签证￥" + _sign.amount.GetValueOrDefault().ToString("f2") + "元";
                }
                line.source_id = _sign.id;
                line.project_id = _sign.project_id;
                line.source = SourceType.Signedform.ToString();
                line.status = (sbyte)LineStatus.OK;
                line.user_id = _sign.creator;
                line.opts = (int)OptType.ModifyDelete;
                line = Uof.ItimelineService.AddEntity(line);

                // 新增签证，如果项目状态还未开工 则变成开工
                var _p = Uof.IprojectService.GetAll(p => p.id == _sign.project_id).FirstOrDefault();
                if (_p != null && _p.status != (int)ProjectStatus.Finished && _p.status != (int)ProjectStatus.Starting)
                {
                    _p.status = (int)ProjectStatus.Starting;
                    Uof.IprojectService.UpdateEntity(_p);
                }

                if (line.id > 0)
                {
                    return SuccessResult;
                }
            }
            return ErrorResult;
        }

        /// <summary>
        ///结算
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddSettlement(settlement set)
        {
            settlement _set = set;
            _set.review = (sbyte)ReviewStatus.Accept;
            _set.date_created = DateTime.Now;
            _set.status = (sbyte)LineStatus.OK;
            _set.date_updated = _set.date_created;
            _set.status = (int)LineStatus.OK;
            _set = Uof.IsettlementService.AddEntity(_set);
            if (_set.id > 0)
            {
                //添加Timeline
                timeline line = new timeline();
                line.date_created = DateTime.Now;
                line.date_updated = line.date_created;
                //line.content = _set.date_signed.GetValueOrDefault().ToString("MM月-dd日") + "|" + "本项目结算金额为￥" + _set.amount.GetValueOrDefault(0).ToString();
                line.content = "设置结算金额￥" + _set.amount.GetValueOrDefault().ToString("f2") + "元 " + "结算日期为" + _set.date_signed.GetValueOrDefault().ToString("yyyy年M月d日");
                line.source_id = _set.id;
                line.project_id = _set.project_id;
                line.source = SourceType.Settlement.ToString();
                line.status = (sbyte)LineStatus.OK;
                line.user_id = _set.creator;
                line.opts = (int)OptType.ModifyDelete;
                line = Uof.ItimelineService.AddEntity(line);

                // 新增结算，如果项目状态还未竣工 则变成竣工
                var _p = Uof.IprojectService.GetAll(p => p.id == _set.project_id).FirstOrDefault();
                if (_p != null && _p.status != (int)ProjectStatus.Finished)
                {
                    _p.status = (int)ProjectStatus.Finished;
                    Uof.IprojectService.UpdateEntity(_p);
                }

                if (line.id > 0)
                {
                    return SuccessResult;
                }
            }
            return ErrorResult;
        }


        [HttpGet]
        public ActionResult CheckHasContract(int project_id)
        {
            var cont = Uof.IcontractService.GetAll(item => item.project_id == project_id && item.status == (int)LineStatus.OK)
                .Select(item => new
                {
                    amount = item.amount,
                    id = item.id,
                    project_id = item.project_id,
                    review = item.review,
                    status = item.status,
                    creator = item.creator,
                    date_signed = item.date_signed,
                    date_created = item.date_created
                }).FirstOrDefault();


            if (cont != null && cont.id > 0)
            {
                var objcont = new
                {
                    amount = cont.amount,
                    id = cont.id,
                    project_id = cont.project_id,
                    review = cont.review,
                    status = cont.status,
                    creator = cont.creator,
                    date_signed = cont.date_signed.GetValueOrDefault().ToString("yyyy年MM月dd日"),
                    date_created = cont.date_created.GetValueOrDefault().ToString("yyyy年MM月dd日")
                };
                user u = Uof.IuserService.GetById(objcont.creator.GetValueOrDefault());

                if (u != null)
                {
                    var ob = new
                    {
                        user_name = u.name,
                        contract = objcont,
                        result = true
                    };
                    return Json(ob, JsonRequestBehavior.AllowGet);
                }


            }
            return Json(new { result = false, contract = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CheckHasSettlement(int project_id)
        {
            var sett = Uof.IsettlementService.GetAll(item => item.project_id == project_id && item.status == (int)LineStatus.OK)
                .Select(p => new
                {
                    amount = p.amount,
                    id = p.id,
                    project_id = p.project_id,
                    review = p.review,
                    status = p.status,
                    creator = p.creator,
                    date_signed = p.date_signed,
                    date_created = p.date_created
                }).FirstOrDefault();


            if (sett != null && sett.id > 0)
            {
                var objsett = new
                {
                    amount = sett.amount,
                    id = sett.id,
                    project_id = sett.project_id,
                    review = sett.review,
                    status = sett.status,
                    creator = sett.creator,
                    date_signed = sett.date_signed.GetValueOrDefault().ToString("yyyy年MM月dd日"),
                    date_created = sett.date_created.GetValueOrDefault().ToString("yyyy年MM月dd日")
                };
                user u = Uof.IuserService.GetById(objsett.creator.GetValueOrDefault());

                if (u != null)
                {
                    var ob = new
                    {
                        user_name = u.name,
                        settment = objsett,
                        result = true
                    };
                    return Json(ob, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { result = false, contract = "" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取得项目总览信息
        /// </summary>
        /// <param name="project_id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetProjectInformation(int project_id)
        {
            var project = Uof.IprojectService.GetAll(p => p.id == project_id).Select(p => new
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
                type = p.type
            }).FirstOrDefault();
            float contractAmount = 0;
            string contractDate = null;
            if (project != null)
            {
                var contract = Uof.IcontractService.GetAll(p => p.project_id == project_id && p.status == (sbyte)LineStatus.OK && p.review == (sbyte)ReviewStatus.Accept).Select(p => new
                {
                    amount = p.amount,
                    date_signed = p.date_signed
                }).FirstOrDefault();
                if (contract != null)
                {
                    contractAmount = contract.amount.GetValueOrDefault(0);
                    contractDate = contract.date_signed.GetValueOrDefault().ToString("yyyy年M月d日");
                }
                float sigeAmount = 0;
                var amount = Uof.IsignedformService.GetAll(p => p.project_id == project_id && p.status == (sbyte)LineStatus.OK && p.review == (sbyte)ReviewStatus.Accept).Sum(p => p.amount);
                sigeAmount = amount.GetValueOrDefault(0);
                float incomeAmount = 0;
                var inc = Uof.IincomeService.GetAll(p => p.project_id == project_id && p.status == (sbyte)LineStatus.OK && p.review == (sbyte)ReviewStatus.Accept).Sum(p => p.amount);
                incomeAmount = inc.GetValueOrDefault(0);
                float setAmount = 0;
                string settmentDate = string.Empty;
                var settment = Uof.IsettlementService.GetAll(p => p.project_id == project_id && p.status == (sbyte)LineStatus.OK && p.review == (sbyte)ReviewStatus.Accept).Select(p => new
                {
                    amount = p.amount,
                    date_signed = p.date_signed
                }).FirstOrDefault();
                if (settment != null)
                {
                    setAmount = settment.amount.GetValueOrDefault(0);
                    settmentDate = settment.date_signed.GetValueOrDefault().ToString("yyyy年M月d日");
                }
                string sdate = project.date_started.GetValueOrDefault().ToString("yyyy年M月d日");
                string edate = project.date_finished.GetValueOrDefault().ToString("yyyy年M月d日");
                if (sdate.IndexOf("0001") > -1)
                {
                    sdate = "未填";
                }
                if (edate.IndexOf("0001") > -1)
                {
                    edate = "未竣工";
                }
                var obj = new
                {
                    project_id = project.id,
                    project_name = project.name,
                    project_type = project.type,
                    project_area = project.area,
                    project_sdate = sdate,
                    project_edate = edate,
                    contract_amount = contractAmount,
                    contract_date = contractDate,
                    signform_amount = sigeAmount,
                    income_amount = incomeAmount,
                    settlement_amount = setAmount,
                    settlement_date = settmentDate
                };
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            return ErrorResult;

        }

        [HttpPost]
        public ActionResult SaveProjectBaseInfo(int id, string name, string type, string area, int user_id)
        {
            var currentUser = HttpContext.User.Identity as UserIdentity;

            if (currentUser == null)
            {
                return ErrorResult;
            }

            if (id > 0)
            {
                project pro = Uof.IprojectService.GetById(id);

                if (pro.name == name && pro.type == type && pro.area == (float)Convert.ToDecimal(area==""?"0":area))
                {
                    return SuccessResult;
                }
                if (currentUser.is_admin == 1)
                {
                    pro.name = name;
                    if (type!="null"&&type.Length>0)
                        pro.type = type;
                    else
                        pro.type = null;
                    if (area != "null" && area.Length > 0)
                    {
                        pro.area = (float)Convert.ToDecimal(area);
                    }
                    else
                    {
                        pro.area = null;
                    }
                    pro.date_updated = DateTime.Now;
                    Uof.IprojectService.UpdateEntity(pro);
                    return SuccessResult;
                }
                else
                {
                    projectmodify pm = new projectmodify();
                    pm.modifytype = (int)ProjectModifyType.Info;
                    pm.name = name;
                    pm.type = type;
                    pm.project_id = id;
                    pm.creator = user_id;
                    pm.date_created = DateTime.Now;
                    pm.date_updated = DateTime.Now;
                    pm.status = (int)LineStatus.WaitReview;
                    pm.area = (float)Convert.ToDecimal(area);
                    pm.review = (int)ReviewStatus.WaitReview;

                    pro.modify_status = (int)ProjectModifyStatus.WaitReviewInfoModify;
                    pro.date_updated = DateTime.Now;
                    Uof.IprojectService.UpdateEntity(pro);

                    pm = Uof.IprojectmodifyService.AddEntity(pm);
                    if (pm.id > 0)
                    {

                        var waitdealLines = new List<WaitdealLine>();
                        waitdealLines.Add(new WaitdealLine()
                        {
                            icon = "k-building-filled",
                            content = pro.name
                        });

                        if (pro.name != name)
                        {
                            waitdealLines.Add(new WaitdealLine()
                            {
                                icon = "k-right-circled",
                                content = string.Format("项目名称 {0}", name)
                            });
                        }

                        if (pro.area != pm.area)
                        {
                            waitdealLines.Add(new WaitdealLine()
                            {
                                icon = "k-right-circled",
                                content = string.Format("项目面积 {0}平方米", area)
                            });
                        }

                        if (pro.type != pm.type)
                        {
                            waitdealLines.Add(new WaitdealLine()
                            {
                                icon = "k-right-circled",
                                content = string.Format("项目类型 {0}", type)
                            });
                        }

                        waitdeal wd = new waitdeal();
                        wd.source = SourceType.ProjectInfoModify.ToString();
                        wd.status = (int)LineStatus.WaitReview;
                        wd.title = "更改项目信息";
                        //StringBuilder sb = new StringBuilder();
                        wd.project_id = id;
                        wd.source_id = pm.id;
                        wd.creator = user_id;
                        wd.status = (sbyte)LineStatus.WaitReview;
                        wd.content = JsonConvert.SerializeObject(waitdealLines); // sb.ToString();
                        wd.company_id = getCompanyIdByProjectId(id);
                        wd.date_created = DateTime.Now;
                        wd.date_updated = DateTime.Now;
                        Uof.IwaitdealService.AddEntity(wd);
                        return SuccessResult;
                    }


                }


            }
            return ErrorResult;
        }
    }
}