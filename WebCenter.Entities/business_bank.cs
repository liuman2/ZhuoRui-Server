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
    
    public partial class business_bank:BaseModel
    {
    
    
    
    [EntityPrimKey("business_bank")]
        public int id { get; set; }
    
    
    
        public int customer_id { get; set; }
    
    
    
        public string source { get; set; }
    
    
    
        public int source_id { get; set; }
    
    
    
        public string name { get; set; }
    
    
    
        public string address { get; set; }
    
    
    
        public string manager { get; set; }
    
    
    
        public string tel { get; set; }
    
    
    
        public Nullable<System.DateTime> date_setup { get; set; }
    
    
    
        public Nullable<System.DateTime> date_created { get; set; }
    
    
    
        public string memo { get; set; }
    
    
    
        public Nullable<System.DateTime> date_updated { get; set; }
    
    
    
        public Nullable<int> is_audit { get; set; }
    
    
    
        public int audit_id { get; set; }
    
    
    
        public string account { get; set; }
    
    
    
        public Nullable<int> bank_id { get; set; }
    
    
    
        public string email { get; set; }
    
    
    
        public Nullable<int> manager_id { get; set; }
    
    
    
        public string branch { get; set; }
    
    
    
        public string area { get; set; }
    
        public virtual open_bank open_bank { get; set; }
    }
}
