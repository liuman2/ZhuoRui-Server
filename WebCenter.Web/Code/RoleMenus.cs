﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCenter.Web
{
    public class RoleMenus
    {
        public int id { get; set; }
        public int? parent_id { get; set; }        
        public string route { get; set; }
        public string icon { get; set; }
        public string name { get; set; }
        public bool check { get; set; }
    }
}