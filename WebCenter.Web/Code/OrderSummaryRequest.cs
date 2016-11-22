using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCenter.Web
{
    public class OrderSummaryRequest
    {
        public int? customer_id { get; set; }
        public int? salesman_id { get; set; }
        public int? order_status { get; set; }
        public DateTime? start_time { get; set; }
        public DateTime? end_time { get; set; }
        public string order_type { get; set; }
        public string name { get; set; }
        public int range { get; set; }
        public DateTime? start_create { get; set; }
        public DateTime? end_create { get; set; }

    }
}