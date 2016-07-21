using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCenter.Web
{
    public class CheckUserExistResponse
    {
        public bool IsExist { get; set; }
    }

    public class LoginRequest
    {
        public string account { get; set; }
        public string password { get; set; }
    }

    public class LoginResponse
    {
        public int id { get; set; }
        public string name { get; set; }
        public string mobile { get; set; }
        public int? company_id { get; set; }
        public string picture_url { get; set; }
        public int? status { get; set; }
        public DateTime? date_created { get; set; }
        public DateTime? date_updated { get; set; }
        public Nullable<sbyte> is_admin { get; set; }
        public string company_name { get; set; }

        public List<int?> permissions { get; set; }
    }
}
