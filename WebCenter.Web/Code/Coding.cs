using System;
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
}