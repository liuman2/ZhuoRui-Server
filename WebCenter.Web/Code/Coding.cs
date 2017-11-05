﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCenter.Web
{
    public class Coding
    {
        public CustomerCoding customer {get;set;}
        public OrderCoding order { get; set; }
    }


    public class CustomerCoding
    {
        public int suffix { get; set; }
        public List<AreaCoding> area_code { get; set; }
    }

    public class OrderCoding
    {
        public int suffix { get; set; }
        public List<ModuleCoding> code { get; set; }
    }

    public class AreaCoding
    {
        public int id { get; set; }
        public string name { get; set; }
        public string value { get; set; }
    }

    public class ModuleCoding
    {
        public string module { get; set; }
        public string module_name { get; set; }
        public string value { get; set; }
    }

    public class InboxOrder
    {
        public string order_source { get; set; }
        public int? order_id { get; set; }
        public string order_name { get; set; }
        public string order_code { get; set; }
        public int audit_id { get; set; }
    }

    public class CustomerTimelineEntity
    {
        public int id { get; set; }
        public Nullable<int> customer_id { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public Nullable<sbyte> is_system { get; set; }
        public Nullable<System.DateTime> date_business { get; set; }
        public Nullable<System.DateTime> date_created { get; set; }
        public Nullable<System.DateTime> date_updated { get; set; }
        public Nullable<int> creator_id { get; set; }

        public bool is_notify { get; set; }
        public Nullable<System.DateTime> date_notify { get; set; }
        public Nullable<System.DateTime> dealt_date { get; set; }        
    }
}