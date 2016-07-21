using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashLib.Model
{
    public partial class Hash_Collect_Statistics
    {
        /// <summary>
        /// 部门ID
        /// </summary>
        public string DeptId{get;set;}
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName{get;set;}
        /// <summary>
        /// 部门所在区域级别
        /// </summary>
        public int DeptAreaLevel{get;set;}
        /// <summary>
        /// 查缉检材数合计
        /// </summary>
        public string CollectTotalAmount{get;set;}
        /// <summary>
        /// 报警数合计
        /// </summary>
        public string LevelsTotalAmount{get;set;}
        /// <summary>
        /// 报警数合计
        /// </summary>
        public string AlarmTotalAmount{get;set;}
        /// <summary>
        /// 查缉命中次数合计
        /// </summary>
        public string AvailTotalAmount{get;set;}
        /// <summary>
        /// 是否为末节点
        /// </summary>
        public bool IsChildLeaf { get; set; }
    }
}
