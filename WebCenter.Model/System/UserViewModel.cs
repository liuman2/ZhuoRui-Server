using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashLib.Model
{
    public class UserViewModel
    {
        public string Operation { get; set; }

        public Sys_User Sys_User { get; set; }

        public List<CheckBoxModel> RoleList = new List<CheckBoxModel>();

        /// <summary>
        /// 系统版本号
        /// </summary>
        public string SystemVersionNo
        {
            get;
            set;
        }
    }

}
