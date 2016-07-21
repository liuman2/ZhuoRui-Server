using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashLib.Model
{
    public partial class Hash_File_Info_Upload_Statistics
    {
        /// <summary>
        /// 部门ID
        /// </summary>
        public string DeptId { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName { get; set; }
        /// <summary>
        /// 部门所在区域级别
        /// </summary>
        public int DeptAreaLevel { get; set; }
        /// <summary>
        /// 是否是子叶子
        /// </summary>
        public bool IsChildLeaf { get; set; }
    }
}
