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
    
    public partial class reg_internal_items:BaseModel
    {
    
    
    
    [EntityPrimKey("reg_internal_items")]
        public int id { get; set; }
    
    
    
        public int master_id { get; set; }
    
    
    
        public string name { get; set; }
    
    
    
        public string material { get; set; }
    
    
    
        public Nullable<int> spend { get; set; }
    
    
    
        public Nullable<float> price { get; set; }
    
    
    
        public string memo { get; set; }
    
    
    
        public string finisher { get; set; }
    
    
    
        public Nullable<int> status { get; set; }
    
    
    
        public Nullable<System.DateTime> date_finished { get; set; }
    
    
    
        public Nullable<System.DateTime> date_created { get; set; }
    
    
    
        public Nullable<System.DateTime> date_updated { get; set; }
    
    
    
        public string sub_items { get; set; }
    }
}
