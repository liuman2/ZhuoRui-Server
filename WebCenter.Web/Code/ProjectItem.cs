using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCenter.Web
{
    public class ProjectItem
    {
        public int id { get; set; }
        public int? company_id { get; set; }
        public string name { get; set; }
        public string icon_text { get; set; }
        public string type { get; set; }
        public float? area { get; set; }
        public int? status { get; set; }
        public DateTime? date_started { get; set; }
        public DateTime? date_finished { get; set; }
        public int? modify_status { get; set; }
        public bool? signed { get; set; }
        public int? creator { get; set; }
        public string creator_name { get; set; }
        public DateTime? date_created { get; set; }
        public DateTime? date_updated { get; set; }
        public string progress { get; set; }
    }
}