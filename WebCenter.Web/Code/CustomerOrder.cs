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
}