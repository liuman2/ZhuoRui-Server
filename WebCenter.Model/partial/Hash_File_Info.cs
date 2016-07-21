using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashLib.Model
{
    public partial class Hash_File_Info
    {
        /// <summary>
        /// 文件上传ID
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string File_Name { get; set; }
        ///// <summary>
        ///// 文件扩展名
        ///// </summary>
        //public string Extent_Name { get; set; }

        /// <summary>
        /// 样本驳回-审批意见
        /// </summary>
        public string ApprovalInfo { get; set; }

    }
}
