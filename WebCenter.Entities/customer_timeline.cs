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
    
    public partial class customer_timeline:BaseModel
    {
    
    
    
    [EntityPrimKey("customer_timeline")]
        public int id { get; set; }
    
    
    
        public Nullable<int> customer_id { get; set; }
    
    
    
        public string title { get; set; }
    
    
    
        public string content { get; set; }
    
    
    
        public Nullable<sbyte> is_system { get; set; }
    
    
    
        public Nullable<System.DateTime> date_business { get; set; }
    
    
    
        public Nullable<System.DateTime> date_created { get; set; }
    
    
    
        public Nullable<System.DateTime> date_updated { get; set; }
    
    
    
        public Nullable<int> creator_id { get; set; }
    
        public virtual customer customer { get; set; }
        public virtual member member { get; set; }
    }
}
