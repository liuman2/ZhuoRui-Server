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
    
    public partial class accounting_progress:BaseModel
    {
    
    
    
    [EntityPrimKey("accounting_progress")]
        public int id { get; set; }
    
    
    
        public int master_id { get; set; }
    
    
    
        public Nullable<System.DateTime> date_start { get; set; }
    
    
    
        public string progress { get; set; }
    
    
    
        public string attachment { get; set; }
    
    
    
        public Nullable<System.DateTime> date_created { get; set; }
    
    
    
        public Nullable<System.DateTime> date_updated { get; set; }
    }
}