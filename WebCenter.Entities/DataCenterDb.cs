using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCenter.Entities
{
    public partial class DataCenter : DbContext
    {
        public DataCenter(DbConnection conn)
            : base(conn, true)
        {
           
        }

        public DataCenter(string connstrName)   ///datacenterEntities1
            : base(connstrName)  /// : base("name=datacenterEntities1")
        {
        }
    }
}
