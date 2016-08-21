using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCenter.Web
{
    public class AnnualWarning
    {
        public int? id { get; set; }
        public int? customer_id { get; set; }
        public string customer_name { get; set; }
        public string customer_code { get; set; }
        public string order_code  { get; set; }
        public string order_name  { get; set; }
        public string order_type { get; set; }
        public string order_type_name { get; set; }
        public DateTime? submit_review_date { get; set; }
        public DateTime? date_finish { get; set; }
    }
}