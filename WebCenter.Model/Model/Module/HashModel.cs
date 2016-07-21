using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashLib.Model
{

    /// <summary>
    /// 样本审核
    /// </summary>
    public class HashListModel
    {
        /// <summary>
        /// 样本类型Json字符串
        /// </summary>
        public string FolderListJson { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 文件状态数量信息
        /// </summary>
        public HashStatusNumModel HashStatusNum = new HashStatusNumModel();
        /// <summary>
        /// 菜单列表
        /// </summary>
        public MenuItem MenuList = new MenuItem();

    }
    /// <summary>
    /// 文件状态数量信息
    /// </summary>
    public class HashStatusNumModel
    {
        /// <summary>
        /// 待审核样本
        /// </summary>
        public int Num_WaitAudit { get; set; }
        /// <summary>
        /// 新上报样本
        /// </summary>
        public int Num_ReSubmit { get; set; }
        /// <summary>
        /// 驳回样本
        /// </summary>
        public int Num_Refuse { get; set; }
        /// <summary>
        /// 重新认定样本
        /// </summary>
        public int Num_Maintain { get; set; }
        /// <summary>
        /// 无法识别样本
        /// </summary>
        public int Num_UnKnow { get; set; }
    }
    /// <summary>
    /// 样本审核信息返回
    /// </summary>
    public class FileAuditModel_Result
    {
        /// <summary>
        /// 待审核样本
        /// </summary>
        public JsonMessage jsonResult = new JsonMessage();
        /// <summary>
        /// 文件状态数量信息
        /// </summary>
        public HashStatusNumModel HashStatusNum = new HashStatusNumModel();
    }
    /// <summary>
    /// 样本列表返回
    /// </summary>
    public class FileMenuModel_Result
    {
        /// <summary>
        /// 样本状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 菜单列表
        /// </summary>
        public MenuItem MenuList = new MenuItem();
    }

    /// <summary>
    /// 样本明细模型
    /// </summary>
    public class FileDetailModel
    {
        /// <summary>
        /// 样本对象
        /// </summary>
        public Hash_File_Info FileInfo = new Hash_File_Info();
        /// <summary>
        /// 样本对象
        /// </summary>
        public Hash_File_Info PrevFileInfo = new Hash_File_Info();
        /// <summary>
        /// 样本审核
        /// </summary>
        public List<FileAuditItem> FileAuditList = new List<FileAuditItem>();
        /// <summary>
        /// 审核内容
        /// </summary>
        public Hash_File_Info_Audit AuditDetail = new Hash_File_Info_Audit();
        /// <summary>
        /// 样本采集记录
        /// </summary>
        public Hash_Collect_Record CollectRecord = new Hash_Collect_Record();
        /// <summary>
        /// 上报/采集用户明细
        /// </summary>
        public Sys_User UplaodUser = new Sys_User();
        /// <summary>
        /// 上报/采集用户明细
        /// </summary>
        public Sys_User AuditUser = new Sys_User();
        /////////////////////基础数据////////////////////////////////////////
        /// <summary>
        /// 当前用户
        /// </summary>
        public Sys_User CurrentUser = new Sys_User();
        /// <summary>
        /// 查获地点
        /// </summary>
        public string SourcePlace { get; set; }
        /// <summary>
        /// 样本文件是否存在
        /// </summary>
        public FileExitInfo FileExitInfo = new FileExitInfo();
        /// <summary>
        /// 字典表类型（语言、分类、文件类型、等级）
        /// </summary>
        public Dictionary<string, List<SimpleValueModel>> DictionaryList { get; set; }
    }
    /// <summary>
    /// 样本审核脚本
    /// </summary>
    public class FileAuditItem
    {
        /// <summary>
        /// 步骤
        /// </summary>
        public int IndexNo { get; set; }
        /// <summary>
        /// 人员
        /// </summary>
        public string OperatorName { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 意见
        /// </summary>
        public string Options { get; set; }
        /// <summary>
        /// 操作
        /// </summary>
        public string StatusName { get; set; }
    }
}
