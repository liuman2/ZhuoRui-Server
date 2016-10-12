using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebCenter.Entities;

namespace WebCenter.Web
{
    public class UserMenus
    {
        public int id { get; set; }
        public Nullable<int> parent_id { get; set; }
        public string route { get; set; }
        public string icon { get; set; }
        public string name { get; set; }

        public List<menu> children { get; set; }
    }

    public class DepartmentIds
    {
        public int id { get; set; }
        public int? parent_id { get; set; }
    }
}