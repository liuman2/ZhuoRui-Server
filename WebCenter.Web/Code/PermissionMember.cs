using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCenter.Web
{
    public class PermissionMember
    {
        public int id { get; set; }
        public int action_id { get; set; }
        public string name { get; set; }
        public string picture_url { get; set; }
        public bool has_right { get; set; }
        public int is_admin { get; set; }
    }
}
