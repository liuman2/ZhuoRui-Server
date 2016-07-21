using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashLib.Model
{
    #region 菜单模型
    /// <summary>
    /// 菜单模型
    /// </summary>
    public class MenuItem
    {
        public string id { get; set; }
        public string parentId { get; set; }
        public string MenuName { get; set; }
        public int Number { get; set; }
        public string Url { get; set; }

        public List<MenuItem> ChildList = new List<MenuItem>();
    }
    #endregion

}
