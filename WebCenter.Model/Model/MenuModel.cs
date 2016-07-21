using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashLib.Model
{
    /// <summary>
    /// 登陆首页模型
    /// </summary>
    public class Main_DeskModel
    {
        /// <summary>
        /// 当前用户
        /// </summary>
        public Sys_User CurUser = new Sys_User();
        /// <summary>
        /// 菜单模型
        /// </summary>
        public List<MenuModel> MenuList = new List<MenuModel>();
        /// <summary>
        /// 数据库版本
        /// </summary>
        public string DBVersion { get; set; }
        /// <summary>
        /// 1.样本库下载信息
        /// </summary>
        public Statistic_FileDownInfo FileDownInfo = new Statistic_FileDownInfo();
        /// <summary>
        /// 2.系统运行数据
        /// </summary>
        public MainCountModel MainCountInfo = new MainCountModel();
        /// <summary>
        /// 3.查缉工具
        /// </summary>
        public List<DownLoadToolInfo> ToolInfoList { get; set; }
        /// <summary>
        /// 样本统计
        /// </summary>
        public Hash_File_Info_Upload_Statistics FileUploadStatistics = new Hash_File_Info_Upload_Statistics();
        /// <summary>
        /// 查缉统计
        /// </summary>
        public List<Hash_Collect_Statistics> FileCollectStatistics = new List<Hash_Collect_Statistics>();
        /// <summary>
        /// 我的工作统计
        /// </summary>
        public List<PersonalWorkModel> PersonalWorkStatistics = new List<PersonalWorkModel>();
        /// <summary>
        /// 首页系统名称
        /// </summary>
        public string SystemName { get; set; }
    }
    #region 首页右侧统计

    /// <summary>
    /// 1.样本库下载信息
    /// </summary>
    public class Statistic_FileDownInfo
    {
        /// <summary>
        /// 特征库id
        /// </summary>
        public string DBVersion_ID
        {
            get;
            set;
        }
        /// <summary>
        /// 样本文件数
        /// </summary>
        public long Num_Files { get; set; }
        /// <summary>
        /// 最新特征库
        /// </summary>
        public string Versions { get; set; }
        /// <summary>
        /// 最新生成时间
        /// </summary>
        public DateTime Create_Time { get; set; }
        /// <summary>
        /// 下载数
        /// </summary>
        public long Download_Count { get; set; }
    }

    /// <summary>
    /// 2.系统运行数据
    /// </summary>
    public class MainCountModel
    {
         /// <summary>
        /// 哈希库下载次数
        /// </summary>
        public int HashDownCount { get; set; }
        /// <summary>
        /// 特征库下载次数
        /// </summary>
        public int FileInfoDownCount { get; set; }
        /// <summary>
        /// 终端设备
        /// </summary>
        public int TerminalCount { get; set; }
        /// <summary>
        /// 终端设备
        /// </summary>
        public string TerminalIdList { get; set; }
        /// <summary>
        /// 报警次数
        /// </summary>
        public int HitInfoCount { get; set; }
        /// <summary>
        /// 已检测手机
        /// </summary>
        public int SourceMBCount { get; set; }
        /// <summary>
        /// 已检测U盘/硬盘/SD卡/磁盘
        /// </summary>
        public int SourceDiskCount { get; set; }
    }
    /// <summary>
    /// 3.查缉工具
    /// </summary>
    public class DownLoadToolInfo
    {
        public string Soft_ID { get; set; }

        /// <summary>
        /// 工具
        /// </summary>
        public string ToolName { get; set; }
        /// <summary>
        /// 软件简介
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 最新版本
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// 下载次数
        /// </summary>
        public int DownloadCount { get; set; }

        /// <summary>
        /// 软件下载路径
        /// </summary>
        public string ToolPath { get; set; }
    }

    #endregion
    /// <summary>
    /// 菜单模型
    /// </summary>
    public class MenuModel
    {
        /// <summary>
        /// 菜单ID
        /// </summary>
        public string Menu_ID { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Menu_Name { get; set; }
        /// <summary>
        /// 页面地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Menu_Image { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int? Sequence { get; set; }
        /// <summary>
        /// 窗体高度扩展属性
        /// </summary>
        public int Height
        {
            get;
            set;
        }
        /// <summary>
        /// 窗体距浏览器顶部高度扩展属性
        /// </summary>
        public int Top
        {
            get;
            set;
        } 
    }

    public class RoleMenuModel
    {
        public string TreeMenu { get;set; }

        public string Role_ID{get;set;}

        public string Role_Name { get; set; }
    }
}
