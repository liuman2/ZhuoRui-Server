using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashLib.Model
{
    [Serializable]
    /// <summary>
    /// 登陆首页显示模式
    /// </summary>
    public class SourceTypeModel
    {
        /// <summary>
        /// 类型
        /// </summary>
        public string SourceType { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string iptStart { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string iptEnd { get; set; }
    }
}
