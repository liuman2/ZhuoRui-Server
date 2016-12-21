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

    public class Customer
    {
        public int id { get; set; }
        public string name { get; set; }
        public string contact { get; set; }
        public string mobile { get; set; }
        public string salesman { get; set; }
        public string source { get; set; }
        public int? source_id { get; set; }
        public string source_name { get; set; }
        public int? assistant_id { get; set; }
        public string assistant_name { get; set; }
        public string tel { get; set; }
        public string code { get; set; }
        public string industry { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string county { get; set; }
        public string address { get; set; }
    }

    public class simpleNotice
    {
        public int id { get; set; }
        public string title { get; set; }
        public DateTime? created { get; set; }
        public bool isNew { get; set; }
    }
}
