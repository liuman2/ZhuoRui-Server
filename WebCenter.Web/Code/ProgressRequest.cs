using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCenter.Web
{
    public class ProgressRequest
    {
        public int id { get; set; }
        public string name { get; set; }
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