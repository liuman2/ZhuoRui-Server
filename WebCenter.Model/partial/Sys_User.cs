using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashLib.Model
{
    public partial class Sys_User
    {
        public List<string> RoleList = new List<string>();

        public string Operation { get; set; }

        public string RoleIDs { get; set; }

    }

    public partial class AreaEx
    {
        public int id { get; set; }
        public string text { get; set; }

        public List<AreaEx> children { get; set; }

        public string Paths { get; set; }

        public int IntegerID { get; set; }

        public int ParentID { get; set; }

        public string state { get; set; }

        public object attributes { get; set; }
    }
    public partial class FolderEx
    {
        public string id { get; set; }
        public string text { get; set; }

        public List<FolderEx> children { get; set; }

        public string Paths { get; set; }

        public string state { get; set; }

        public object attributes { get; set; }
    }

    public partial class SystemEx
    {
        public string id { get; set; }
        public string text { get; set; }

        public List<SystemEx> children { get; set; }

        public string state { get; set; }

        public object attributes { get; set; }
    }

    public partial class StatisticsEx
    {
        public string id { get; set; }
        public string text { get; set; }

        public List<StatisticsEx> children { get; set; }

        public string state { get; set; }

        public object attributes { get; set; }
    }

    public partial class DepartmentEx
    {
        public string id { get; set; }
        public string text { get; set; }

        public List<DepartmentEx> children { get; set; }

        public string Paths { get; set; }

        public int IntegerID { get; set; }

        public int ParentID { get; set; }

        public string state { get; set; }

        public object attributes { get; set; }
    }
}
