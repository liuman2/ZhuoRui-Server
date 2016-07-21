using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCenter.Entities
{
    public class EntityPrimKey : Attribute
    {
        private string tableName = string.Empty;
        public EntityPrimKey(string strTableName)
        {
            this.tableName = strTableName;
        }
        public string TableName
        {
            get
            {
                return this.tableName;
            }
        }
    }
}
