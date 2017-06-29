using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCenter.Web
{
    public class CustomerOrder
    {
        public string order_name { get; set; }

        public int count { get; set; }

        public DateTime? last_date { get; set; }
    }

    public class ShortInfoCustomer
    {
        public int id { get; set; }
        public string name { get; set; }
        public string industry { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string county { get; set; }
        public string address { get; set; }
        public string contact { get; set; }
        public string mobile { get; set; }
        public string tel { get; set; }
        public string fax { get; set; }
        public string email { get; set; }
        public string QQ { get; set; }
        public string wechat { get; set; }
        public string description { get; set; }
    }
}