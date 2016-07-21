using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashLib.Model
{
    /// <summary>
    /// 样本审核脚本
    /// </summary>
    public class FileAuditModel
    {
        public string File_Info_ID { get; set; }
        public string MD5 { get; set; }
        public string File_Translate { get; set; }
        public string Public_ID { get; set; }
        public string Language { get; set; }
        public string Sub_Title { get; set; }
        public string Descriptions { get; set; }
        public string Public_Name { get; set; }
        public string File_Type { get; set; }
        public string Levels { get; set; }
        public string Opinion { get; set; }
        public int Status { get; set; }

        public string CurrUserId { get; set; }
    }
    /// <summary>
    /// 样本审批脚本
    /// </summary>
    public class FileApprovalModel
    {
        public string File_Info_ID { get; set; }
        public string MD5 { get; set; }
        public string Opinion { get; set; }
        public string Status { get; set; }

        public string CurrUserId { get; set; }
    }
}
