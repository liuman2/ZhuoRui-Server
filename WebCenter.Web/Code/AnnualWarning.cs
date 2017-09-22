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
        public string order_name_en { get; set; }
        public string order_type { get; set; }
        public string order_type_name { get; set; }
        public string saleman { get; set; }
        public string waiter { get; set; }
        public string assistant_name { get; set; }

        public DateTime? submit_review_date { get; set; }
        public DateTime? date_finish { get; set; }
        public DateTime? date_setup { get; set; }
        public DateTime? annual_date { get; set; }

        public int? annual_year { get; set; }
        public int? month { get; set; }
        public int? exten_period { get; set; }
        public int? order_status { get; set; }

        public DateTime? date_last { get; set; }
        public string title_last { get; set; }
        public DateTime? date_wait { get; set; }

        public string setup_day { get; set; }

        public string region { get; set; }
        public float? reference_price { get; set; }

        public string resell_code { get; set; }

    }

    public class FinanceCheck
    {
        public int? id { get; set; }
        public int? customer_id { get; set; }
        public string customer_name { get; set; }
        public string customer_code { get; set; }
        public string order_code { get; set; }
        public string order_name { get; set; }
        public string order_name_en { get; set; }
        public string order_type { get; set; }
        public string order_type_name { get; set; }
        public float? amount_transaction { get; set; }
        public sbyte? status { get; set; }
        public int? review_status { get; set; }
        public string salesman { get; set; }
        public string waitor { get; set; }
        public string source { get; set; }

        public DateTime? date_transaction { get; set; }
        public DateTime? date_created { get; set; }
        public DateTime? date_setup { get; set; }
        public string type { get; set; }
    }
}