using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashLib.Model
{
    /// <summary>
    /// 上传文件信息
    /// </summary>
    public class UploadFileInfo
    {
        /// <summary>
        /// 当前序列
        /// </summary>
        public int OrderNumber { get; set; }
        /// <summary>
        /// 文件唯一标识
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 扩展名
        /// </summary>
        public string Ext { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// 格式化后文件大小
        /// </summary>
        public string FormatFileSize { get; set; }

        /// <summary>
        /// 文件物理路径
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 文件上传状态
        /// </summary>
        public UploadStatus UploadStatus { get; set; }

        /// <summary>
        /// MD5值
        /// </summary>
        public string MD5 { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public string ErrorInfo { get; set; }

        /// <summary>
        /// 上传速度信息
        /// </summary>
        public string SpeedInfo { get; set; }

        /// <summary>
        /// 进度信息
        /// </summary>
        public string ProgressInfo { get; set; }

        /// <summary>
        /// 哈希值是否存在
        /// </summary>
        public bool IsHashExist { get; set; }

        /// <summary>
        /// 文件属性集合
        /// </summary>
        public List<FileProperty> FileDetail { get; set; }

        /// <summary>
        /// 保存目录
        /// </summary>
        public string SavePath { get; set; }

        /// <summary>
        /// 简要说明
        /// </summary>
        public string Remark { get; set; }

    }
    /// <summary>
    /// 文件属性
    /// </summary>
    public class FileProperty
    {
        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }
    }

    /// <summary>
    /// 数据操作结果
    /// </summary>
    public class UplaodInfoModel
    {
        /// <summary>
        /// 上传控件模型
        /// </summary>
        public FileUploadModel FileUpload { get; set; }
        /// <summary>
        /// 当前用户信息
        /// </summary>
        public Sys_User CurrentUser { get; set; }
        /// <summary>
        /// 设备类型
        /// </summary>
        public Dictionary<string, string> EquipementTypeList { get; set; }
        /// <summary>
        /// 区域列表
        /// </summary>
        public List<SimpleModel> AreaList = new List<SimpleModel>();
        /// <summary>
        /// 配置信息
        /// </summary>
        public IList<Sys_Config> ConfigList { get; set; }
    }
    /// <summary>
    /// 文件特征
    /// </summary>
    public class FileInfoModel
    {
        /// <summary>
        /// 文件上传ID
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropName { get; set; }
        /// <summary>
        /// 属性值
        /// </summary>
        public string PropValue { get; set; }
    }
    /// <summary>
    /// 客户端信息
    /// </summary>
    public class LocalInfo
    {
        /// <summary>
        /// 主机名
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// mac地址
        /// </summary>
        public string MacAddress { get; set; }

        /// <summary>
        /// 本地上传的时间
        /// </summary>
        public DateTime LocalUploadTime { get; set; }
    }
}
