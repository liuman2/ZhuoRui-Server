using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace WebCenter.Web
{
    public interface IUserIdentity : IIdentity
    {
        int id { get; }
        string name { get; }
        string mobile { get; }
        int? company_id { get; }
        string picture_url { get; }
        int? status { get; }
        int? is_admin { get; }
    }
}