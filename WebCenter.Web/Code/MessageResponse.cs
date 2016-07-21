using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCenter.Web
{
    public class MessageResponse
    {
        public int id { get; set; }
        public int? source_id { get; set; }
        public string content { get; set; }
        public string date_created { get; set; }
        public int type { get; set; }
        public string icon { get; set; }
    }

    public class MessageDetailResponse
    {
        public int id { get; set; }
        public string icon { get; set; }
        public string reviewer_name { get; set; }
        public string creator_name { get; set; }
        public string title { get; set; }
        public List<WaitdealLine> content_line { get; set; }
        public DateTime? date_created { get; set; }
        public string time_desc { get; set; }
        public string pass_desc { get; set; }
    }
}