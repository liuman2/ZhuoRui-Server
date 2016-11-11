using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCenter.Web
{
    public class AnnualOrigOrder
    {
        public int order_id { get; set; }
        public string order_type_name { get; set; }
        public string order_code { get; set; }
        public string name_cn { get; set; }
        public string name_en { get; set; }

        public int? customer_id { get; set; }
        public string customer_code { get; set; }
        public string customer_name { get; set; }

        public DateTime? date_setup { get; set; }

        public int? salesman_id { get; set; }
        public string salesman { get; set; }

        public int? assistant_id { get; set; }
        public string assistant_name { get; set; }

    }
}