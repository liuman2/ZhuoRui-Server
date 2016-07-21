using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCenter.Web
{
    public enum ReviewStatus
    {
        /// <summary>
        /// 未审核
        /// </summary>
        WaitReview = 0,

        /// <summary>
        /// 未通过
        /// </summary>
        Reject = -1,

        /// <summary>
        /// 通过
        /// </summary>
        Accept = 1
    }
    public enum LineStatus
    {
        /// <summary>
        /// 作废，审核不通过
        /// </summary>
        Abandon = -1,

        /// <summary>
        /// 待确认,待审核
        /// </summary>
        WaitReview = 0,

        /// <summary>
        /// 正常
        /// </summary>
        OK = 1,
        /// <summary>
        /// 删除待审核中
        /// </summary>
        WaitDeleteReview=2
    }

    public enum ProjectStatus 
    {
        /// <summary>
        /// 准备
        /// </summary>
        Ready = 0,

        /// <summary>
        /// 开工
        /// </summary>
        Starting = 1,

        /// <summary>
        /// 竣工
        /// </summary>
        Finished = 2,

        /// <summary>
        /// 归档
        /// </summary>
        BackProfile = -1,

        /// <summary>
        /// 已删除
        /// </summary>
        Deleted = -2
    }

    public enum OptType
    {
        ReadOnly=0,
        Delete=1,
        Modify=4,
        ModifyDelete=8,
    }

    /// <summary>
    /// 待办表 业务来源类型
    /// 时间轴 消息来源类型
    /// </summary>
    public enum SourceType
    {
      
        /// <summary>
        /// 发言
        /// </summary>
        Forum,

        /// <summary>
        /// 合约
        /// </summary>
        Contract,

        /// <summary>
        /// 收款
        /// </summary>
        Income,

        /// <summary>
        /// 签证
        /// </summary>
        Signedform,

        /// <summary>
        /// 结算
        /// </summary>
        Settlement,

        /// <summary>
        /// 开工
        /// </summary>
        ProjectStart,

        /// <summary>
        /// 竣工
        /// </summary>
        ProjectFinish,

        /// <summary>
        /// 加入组织
        /// </summary>
        JoinCompany,

        /// <summary>
        /// 项目归档-记录不可删除
        /// </summary>
        Archive,

        /// <summary>
        /// 项目从归档恢复准备中-记录不可删除
        /// </summary>
        Revert,
        /// <summary>
        /// 新建项目
        /// </summary>
        CreateProject,
        /// <summary>
        /// 项目基本信息修改
        /// </summary>
        ProjectInfoModify
    }

    public enum ProjectModifyStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 开工时间修改待审中
        /// </summary>
        WaitReviewStartModify = 1,

        /// <summary>
        /// 开工时间删除待审中
        /// </summary>
        WaitReviewStartDelete = 2, 

        /// <summary>
        /// 竣工时间修改待审中
        /// </summary>
        WaitReviewEndModify = 3,

        /// <summary>
        /// 竣工时间删除待审中
        /// </summary>
        WaitReviewEndDelete = 4,

        /// <summary>
        /// 基本信息修改待审中
        /// </summary>
        WaitReviewInfoModify = 5

    }

    public enum DictionaryGroup
    {
        /// <summary>
        /// 项目类型
        /// </summary>
        ProjectType
    }
    
    /// <summary>
    /// 项目修改类型
    /// </summary>
    public enum ProjectModifyType
    {
        /// <summary>
        /// 基本信息
        /// </summary>
        Info,
        /// <summary>
        /// 开工时间
        /// </summary>
        StartDate,
        /// <summary>
        /// 竣工时间
        /// </summary>
        FinishDate
    }

    public enum MessageType
    {
        /// <summary>
        /// 组织
        /// </summary>
        Organization = 0,

        /// <summary>
        /// 项目
        /// </summary>
        Project,

        /// <summary>
        /// 业务
        /// </summary>
        Business,

        /// <summary>
        /// 权限
        /// </summary>
        Permission
    }
}
