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
    
    public partial class internal_shareholder:BaseModel
    {
    
    
    
    [EntityPrimKey("internal_shareholder")]
        public int id { get; set; }
    
    
    
        public int master_id { get; set; }
    
    
    
        public Nullable<int> history_id { get; set; }
    
    
    
        public string name { get; set; }
    
    
    
        public string source { get; set; }
    
    
    
        public string gender { get; set; }
    
    
    
        public string cardNo { get; set; }
    
    
    
        public string position { get; set; }
    
    
    
        public Nullable<float> takes { get; set; }
    
    
    
        public string type { get; set; }
    
    
    
        public string changed_type { get; set; }
    
    
    
        public Nullable<System.DateTime> date_changed { get; set; }
    
    
    
        public string memo { get; set; }
    
    
    
        public string attachment { get; set; }
    
    
    
        public Nullable<System.DateTime> date_created { get; set; }
    
    
    
        public Nullable<System.DateTime> date_updated { get; set; }
    }
}
