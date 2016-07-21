using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCenter.Entities
{
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
    }
}
