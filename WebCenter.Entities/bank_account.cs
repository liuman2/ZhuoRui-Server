//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebCenter.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class bank_account:BaseModel
    {
        public bank_account()
        {
            this.reg_abroad = new HashSet<reg_abroad>();
            this.reg_internal = new HashSet<reg_internal>();
        }
    
    
    
    
    [EntityPrimKey("bank_account")]
        public int id { get; set; }
    
    
    
        public Nullable<int> customer_id { get; set; }
    
    
    
        public string bank { get; set; }
    
    
    
        public string holder { get; set; }
    
    
    
        public string account { get; set; }
    
    
    
        public Nullable<System.DateTime> date_created { get; set; }
    
    
    
        public Nullable<System.DateTime> date_updated { get; set; }
    
        public virtual ICollection<reg_abroad> reg_abroad { get; set; }
        public virtual ICollection<reg_internal> reg_internal { get; set; }
        public virtual customer customer { get; set; }
    }
}
