//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebCenter.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Sys_Config : BaseModel
    {
    	[EntityPrimKey]
        public string Config_ID { get; set; }
        public string Item_Name { get; set; }
        public string Item_Value { get; set; }
        public string Item_Type { get; set; }
        public Nullable<int> Integer_ID { get; set; }
        public Nullable<int> Parent_ID { get; set; }
        public string Remark { get; set; }
        public Nullable<bool> Is_Edit { get; set; }
        public Nullable<int> Item_Group { get; set; }
    }
}
