using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashLib.Model
{
    /// <summary>
    /// 个人工作统计模型
    /// </summary>
    public class PersonalWorkModel
    {
        /// <summary>
        /// 统计类型
        /// </summary>
        public string StatisticsType
        {
            get;
            set;
        }
        /// <summary>
        /// 命中次数总计
        /// </summary>
        public int AvailTotalCount
        {
            get;
            set;
        }
        /// <summary>
        /// 采集次数总计
        /// </summary>
        public int CollectTotalCount
        {
            get;
            set;
        }
        /// <summary>
        /// 排序
        /// </summary>
        public int Order
        {
            get;
            set;
        }
    }
}
