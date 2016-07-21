using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.UserModels
{
    
    [Table("tbl_advertisement")]
    public class Advertisement : DbSetBase
    {
        
        public Advertisement()
        { 
        }
    }
}
