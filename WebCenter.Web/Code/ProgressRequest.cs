using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCenter.Web
{
    public class ProgressRequest
    {
        public int id { get; set; }
        public int customer_id { get; set; }
        //public int is_done { get; set; }
        public string progress { get; set; }
        public DateTime? date_finish { get; set; }
        public string progress_type { get; set; }    
    }

    public class RegAbroadProgressRequest: ProgressRequest
    {
        public string address { get; set; }
        public DateTime? date_setup { get; set; }
        public string reg_no { get; set; }
        public sbyte? is_open_bank { get; set; }
        public int? bank_id { get; set; }
    }

    public class RegInternalProgressRequest : ProgressRequest
    {
        //public string address { get; set; }
        public DateTime? date_setup { get; set; }
        public string reg_no { get; set; }

        public sbyte? is_customs { get; set; }
        public string customs_name { get; set; }
        public string customs_address { get; set; }

        public string taxpayer { get; set; }
        public int? bank_id { get; set; }
        public sbyte? is_bookkeeping { get; set; }
        public float? amount_bookkeeping { get; set; }
    }

    public class PatentProgressRequest : ProgressRequest
    {
        public DateTime? date_accept { get; set; }
        public DateTime? date_empower { get; set; }
    }

    public class TtrademarkProgressRequest : ProgressRequest
    {
        public DateTime? date_accept { get; set; }
        public DateTime? date_receipt { get; set; }
        public DateTime? date_trial { get; set; }
        public DateTime? date_regit { get; set; }
        public DateTime? date_exten { get; set; }
    }
}