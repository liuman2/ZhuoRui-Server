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
    
    public partial class lecture_customer:BaseModel
    {
    
    
    
    [EntityPrimKey("lecture_customer")]
        public int id { get; set; }
    
    
    
        public Nullable<int> lecture_id { get; set; }
    
    
    
        public Nullable<int> customer_id { get; set; }
    
        public virtual customer customer { get; set; }
        public virtual lecture lecture { get; set; }
    }
}
