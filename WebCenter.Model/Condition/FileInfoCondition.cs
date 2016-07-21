using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashLib.Model
{
    public class FileInfoCondition : ParameterQuery
    {
        /// <summary>
        /// 组织机构
        /// </summary>
        public string FolderId { get; set; }
         
        /// <summary>
        /// 样本类型
        /// </summary>
         public string TypeId { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
         public string ColumnName
         {
             get;
             set;
         }
        /// <summary>
        /// 字段值
        /// </summary>
         public string ColumnValue
         {
             get;
             set;
         }
        /// <summary>
        /// 当前文件ID
        /// </summary>
         public string CurrentHashID
         {
             get;
             set;
         }
    }
}
