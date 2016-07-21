using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashLib.Model
{
    /// <summary>
    /// 数据统计查询条件
    /// </summary>
    public class StatisticsCondition:ParameterQuery
    {
        /// <summary>
        /// 单位GUID
        /// </summary>
        public string DepartmentId { get; set; }
         
        /// <summary>
        /// 单位级别
        /// </summary>
         public string DepartmentLevel { get; set; }

        /// <summary>
        /// 参数值类型
        /// </summary>
         public string ParamType
         {
             get;
             set;
         }
        /// <summary>
        /// 参数值一
        /// </summary>
         public string ParamOne
         {
             get;
             set;
         }
        /// <summary>
        /// 参数值二
        /// </summary>
         public string ParamSecond
         {
             get;
             set;
         }
    }
}
