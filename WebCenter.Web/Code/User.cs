using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCenter.Web
{
    public class User
    {
        public int id { get; set; }
        public int? organization_id { get; set; }
        public int? position_id { get; set; }
        public string username { get; set; }
        public string name { get; set; }
        public string english_name { get; set; }
        public string mobile { get; set; }
        public DateTime? birthday { get; set; }
        public string position { get; set; }
        public string department { get; set; }
        public string url { get; set; }
    }
}