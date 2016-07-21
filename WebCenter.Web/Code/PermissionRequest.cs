using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCenter.Web
{
    public class PermissionRequest
    {
        public int action_id { get; set; }
        public int[] new_permission_ids { get; set; }
        public int[] old_permission_ids { get; set; }
    }
}
