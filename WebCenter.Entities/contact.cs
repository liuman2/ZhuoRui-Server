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
    
    public partial class contact:BaseModel
    {
    
    
    
    [EntityPrimKey("contact")]
        public int id { get; set; }
    
    
    
        public int customer_id { get; set; }
    
    
    
        public string name { get; set; }
    
    
    
        public string mobile { get; set; }
    
    
    
        public string tel { get; set; }
    
    
    
        public string position { get; set; }
    
    
    
        public string email { get; set; }
    
    
    
        public string wechat { get; set; }
    
    
    
        public string QQ { get; set; }
    
    
    
        public string type { get; set; }
    
    
    
        public string memo { get; set; }
    
    
    
        public Nullable<System.DateTime> date_created { get; set; }
    
    
    
        public Nullable<System.DateTime> date_updated { get; set; }
    
    
    
        public string responsable { get; set; }
    }
}
