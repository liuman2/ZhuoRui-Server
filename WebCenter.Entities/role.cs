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
    
    public partial class role:BaseModel
    {
    
    
    
    [EntityPrimKey("role")]
        public int id { get; set; }
    
    
    
        public string name { get; set; }
    
    
    
        public string code { get; set; }
    
    
    
        public Nullable<sbyte> is_system { get; set; }
    
    
    
        public string description { get; set; }
    
    
    
        public Nullable<System.DateTime> date_created { get; set; }
    
    
    
        public Nullable<System.DateTime> date_updated { get; set; }
    }
}
