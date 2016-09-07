using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCenter.Web
{
    public class OrderSearchRequest
    {
        public int index { get; set; }
        public int size { get; set; }
        public int? customer_id { get; set; }
        public int? status { get; set; }
        public DateTime? start_time { get; set; }
        public DateTime? end_time { get; set; }
    }

    public class TrademarkRequest : OrderSearchRequest
    {
        public string name { get; set; }
        public string applicant { get; set; }
    }

    public class LectureRequest : OrderSearchRequest
    {
        public string title { get; set; }
        public string form { get; set; }
    }
}