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
    
    public partial class schedule:BaseModel
    {
    
    
    
    [EntityPrimKey("schedule")]
        public int id { get; set; }
    
    
    
        public string title { get; set; }
    
    
    
        public Nullable<System.DateTime> start { get; set; }
    
    
    
        public Nullable<System.DateTime> end { get; set; }
    
    
    
        public string color { get; set; }
    
    
    
        public Nullable<int> type { get; set; }
    
    
    
        public string people { get; set; }
    
    
    
        public string location { get; set; }
    
    
    
        public string memo { get; set; }
    
    
    
        public string attachment { get; set; }
    
    
    
        public int created_id { get; set; }
    
    
    
        public Nullable<System.DateTime> date_created { get; set; }
    
    
    
        public int updated_id { get; set; }
    
    
    
        public Nullable<System.DateTime> date_updated { get; set; }
    
    
    
        public Nullable<sbyte> all_day { get; set; }
    }
}