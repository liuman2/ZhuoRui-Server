using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashLib.Model
{
    /// <summary>
    /// 查询参数
    /// </summary>
    public class ParameterQuery
    {

        public int page { get; set; }

        public int rows { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int Total { get; set; }

        public string FileName { get; set; }

        public string FileVal { get; set; }

        public string Status { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string sort { get; set; }
        /// <summary>
        /// 排序方式 desc asc
        /// </summary>
        public string order { get; set; }

    }
}
