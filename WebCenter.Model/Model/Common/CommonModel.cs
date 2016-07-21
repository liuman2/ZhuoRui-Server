using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashLib.Model
{
    #region 数据操作结果
    /// <summary>
    /// 数据操作结果
    /// </summary>
    public class JsonMessage
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
    }
    #endregion
    [Serializable]
    /// <summary>
    /// 简单模型
    /// </summary>
    public class SimpleModel
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public int? Valuse { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Names { get; set; }

        /// <summary>
        /// 地区级别:省1/市2/县3
        /// </summary>
        public int? AreaLevel { get; set; }
    }
    [Serializable]
    /// <summary>
    /// 简单模型
    /// </summary>
    public class SimpleValueModel
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public string Valuse { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Names { get; set; }
    }
    #region 树形菜单模型
    /// <summary>
    /// 树形菜单模型
    /// </summary>
    public class TreeItem
    {
        public string id
        {
            get;
            set;
        }
        public string text
        {
            get;
            set;
        }
        public string state
        {
            get;
            set;
        }
        public int attributesType
        {
            get;
            set;
        }
        public object attributes
        {
            get;
            set;
        }
        public IList<TreeItem> children
        {
            get;
            set;
        }
        public int recordCount
        {
            get;
            set;
        }
        public string parentNo
        {
            get;
            set;
        }
    }
    #endregion

}
