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
}