using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCenter.Web
{
    public class RolePermission
    {
        public List<RoleMenus> menus { get; set; }

        public List<RoleOpers> opers { get; set; }
    }

    public class ParamSetting
    {
       //public string  patent_period { get; set; }
       // public string trademark_period { get; set; }

        public string name { get; set; }
        public string value { get; set; }
        public string memo { get; set; }
    }
}