using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCenter.Web
{
    public class WaitdealResponse
    {
        public int id { get; set; }
        public int? company_id { get; set; }
        public int? project_id { get; set; }
        public int? source_id { get; set; }
        public int? creator { get; set; }
        public string icon { get; set; }
        public string creator_name { get; set; }
        public string source { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public List<WaitdealLine> content_line { get; set; }
        public DateTime? date_created { get; set; }
        public string time_desc { get; set; }
        public string operatorType { get; set; }
    }
}