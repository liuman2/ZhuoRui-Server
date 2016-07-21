using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheManager.Core;

namespace Common
{
    public class CacheUtil
    {
        static ICacheManager<object> _cache;
        static object lockobj = new object();
        public static ICacheManager<object> Cache
        {
            get
            {
                lock (lockobj)
                {
                    if (_cache == null)
                    {
                        _cache = CacheFactory.FromConfiguration<object>("myCache");
                        return _cache;
                    }
                    else
                    {
                        return _cache;
                    }
                }
            }
        }
    }
}
