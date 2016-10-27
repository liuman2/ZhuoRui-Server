using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebCenter.Entities;

namespace WebCenter.Web
{
    public class NoticeBase
    {
        public int id { get; set; }
        public int? creator_id { get; set; }
        public string creator { get; set; }
        public string title { get; set; }
        public string code { get; set; }
        public int? type { get; set; }
        public string content { get; set; }
        public int? status { get; set; }
        public string date_created { get; set; }
    }

    public class Notice: NoticeBase
    {
        public List<attachment> attachments { get; set; }
    }
}