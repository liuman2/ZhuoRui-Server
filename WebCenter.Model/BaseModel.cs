using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;

namespace HashLib.Model
{
    /// <summary>
    /// 模型基类
    /// </summary>
    public class BaseModel
    {
        private Dictionary<string, object> _extendProperty;

        /// <summary>
        /// 扩展属性
        /// </summary>
        public Dictionary<string, object> ExtendProperty
        {
            get { return _extendProperty; }
        }

        ///// <summary>
        ///// 当前实体的实体键
        ///// </summary>
        //public Dictionary<string, Type> KeyFiles
        //{
        //    get { return GetKeyFields(); }
        //}

        public BaseModel()
        {
            _extendProperty = new Dictionary<string, object>();
        }

        //private Dictionary<string, Type> GetKeyFields()
        //{
        //    Dictionary<string, Type> _keyFields = new Dictionary<string, Type>();
        //    foreach (var prop in this.GetType().GetProperties())
        //    {
        //        var attr = prop.GetCustomAttributes(typeof(EdmScalarPropertyAttribute), false).FirstOrDefault() as EdmScalarPropertyAttribute;
        //        if (attr != null && attr.EntityKeyProperty)
        //        {
        //            _keyFields.Add(prop.Name,prop.PropertyType);
        //        }
        //    }

        //    return _keyFields;
        //}

    }
}
