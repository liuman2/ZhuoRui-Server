using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashLib.Model
{
    public enum UserStatus
    {
        UnActive = 0,
        Active = 1,
        Delete = 2

    }
    /// <summary>
    /// 命中类型
    /// </summary>
    public enum HitType
    {
        /// <summary>
        /// 哈希命中
        /// </summary>
        AlarmHit = 1,
        /// <summary>
        /// 属性命中
        /// </summary>
        PropertyHit = 2
    }
    /// <summary>
    /// 样本文件类型
    /// </summary>
    public enum FileInfoStatus
    {
        未审核 = 0,
        审核通过 = 2,
        审核未通过 = -2,
        审批通过 = 3,
        驳回 = -3,
        重新认定样本 = -4,
        无法识别样本 = -5,
        删除 = -1
    }
    /// <summary>
    /// 样本来源
    /// </summary>
    public enum FileSource
    {
        未知 = 0,
        网上发现 = 1,
        网下查获 = 2

    }
    /// <summary>
    /// 样本来源（语言-3、分类-2、文件类型-4、等级-5）
    /// </summary>
    public enum DictionaryType
    {
        查获介质类型 = 1,
        组织机构 = 2,
        语言 = 3,
        文件类型 = 4,
        等级 = 5
    }
    /// <summary>
    /// 状态
    /// </summary>
    public enum UploadStatus
    {
        Computing,//计算MD5中
        ComputeComplete,//计算MD5完成
        Uploading,//上传中
        UploadComplete, //上传完成
        Error = -1 //异常
    }
    /// <summary>
    /// 样本文件类型
    /// </summary>
    public enum FileType
    {
        其他 = 0,
        音频 = 1,
        视频 = 2,
        电子书 = 3
    }


    /// <summary>
    ///区域级别
    /// </summary>
    public enum AreaLevel
    {
        省级 = 1,
        地市级 = 2,
        区县级 = 3,
        个人 = 4
    }

    /// <summary>
    /// 文件上报统计预处理类型
    /// </summary>
    public enum FileUploadStatisPreType
    {
        /// <summary>
        /// 文件上传
        /// </summary>
        FileUpload,
        /// <summary>
        /// 审核通过
        /// </summary>
        FirstApproved,
        /// <summary>
        /// 审核不通过
        /// </summary>
        FirstNotApprove,
        /// <summary>
        /// 审批通过
        /// </summary>
        FinalApproved,
        /// <summary>
        /// 审批不通过
        /// </summary>
        FinalNotApproved,
        /// <summary>
        /// 重新认定审批通过（比较特殊，不需要更新正式样本统计总数）
        /// </summary>
        SuggestInfoFinalApproved
    }

    /// <summary>
    /// 模块名称
    /// </summary>
    public enum ModuleName
    {
        登录,
        退出,
        疑似样本上报,
        查缉日志上报,
        故障上报,
        样本查询,
        样本审核,
        样本审批,
        样本认定建议,
        我的工作统计,
        软件下载,
        特征库版本,
        查缉统计,
        样本统计,
        样本采集统计,
        特征库应用统计,
        样本认定统计,
        特征库下载,
        MD5计算,
        用户管理,
        数据字典管理,
        单位管理,
        角色管理,
        日志管理,
        参数管理,
        修改密码,
        软件管理


    }

    /// <summary>
    /// 查缉材料类型 
    /// </summary>
    public enum CollectSourceType
    {
        手机,
        计算机,
        移动存储,
        其它
    }

    public enum CollectHitLevel
    {
        LevelOne,
        LevelSecond,
        LevelThird,
        LevelFourth,
        Other
    }

    /// <summary>
    /// 命中组织类型统计
    /// </summary>
    public enum CollectHitOrgType
    {
        东伊运,
        乌伊运,
        世维会,
        ISIS,
        其他
    }

    public enum CollectHitFileType
    {
        视频,
        音频,
        电子书,
        图片,
        应用程序,
        其它文件类型
    }
    /// <summary>
    /// 解析的状态
    /// </summary>
    public enum AnalyseResult
    {
        Success,
        Exception,
        Failed,
        Default,
        NotFound
    }


    /// <summary>
    /// 查询时间范围枚举
    /// </summary>
    public enum DateRangeType
    {
        今日,
        本月,
        今年,
        去年,
        自定义时间
    }
    public enum ApproveType
    {
        审核,
        审批
    }



    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogType
    {
        登录 = 0,
        操作 = 1,
        退出 = 2
    }

    /// <summary>
    /// 个人工作统计类型
    /// </summary>
    public enum PersonalWorkType
    {
        今日 = 1,
        近一个月 = 2,
        累计 = 3
    }

    /// <summary>
    /// 统计汇总的类型详细归属类与DetailsType通用
    /// </summary>
    public enum DetailSummaryType
    {
        东伊运,
        乌伊运,
        世维会,
        ISIS,
        其它组织,
        视频,
        音频,
        电子书,
        应用程序,
        图片,
        其它
    }

    /// <summary>
    /// 参数组
    /// </summary>
    public enum ItemGroup
    {
        FTP服务=1,
        解析服务=2,
        其它=0
    }

   

}
