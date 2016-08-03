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
        string username { get; }
    }
}