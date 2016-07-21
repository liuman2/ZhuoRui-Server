using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashLib.Model
{
    public partial class Hash_Property_Update_Statistics
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
        /// 各个版本特征库下载次数合计
        /// </summary>
        public int DBVersionTotalAmount { get; set; }
        /// <summary>
        /// 是否为末节点
        /// </summary>
        public bool IsChildLeaf { get; set; }
    }
}
