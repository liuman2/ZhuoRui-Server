using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCenter.Web
{
    public class ProjectCreateRequest
    {
        public int company_id { get; set; }

        public string name { get; set; }

        public string type { get; set; }

        public float? area { get; set; }

        public int creator { get; set; }

        public float? contract { get; set; }

        public int[] members { get; set; }
    }
}
