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
    
    public partial class Sys_Dictionary : BaseModel
    {
        public Sys_Dictionary()
        {
            this.hash_hit_statistics = new HashSet<Hash_Hit_Statistics>();
            this.hash_file_info = new HashSet<Hash_File_Info>();
            this.hash_file_info1 = new HashSet<Hash_File_Info>();
            this.hash_file_info2 = new HashSet<Hash_File_Info>();
            this.hash_file_info3 = new HashSet<Hash_File_Info>();
        }
    
    	[EntityPrimKey]
        public string Dictionary_ID { get; set; }
        public Nullable<int> Dictionary_Type { get; set; }
        public string Dictionary_Name { get; set; }
        public string Dictionary_Value { get; set; }
        public string Remark { get; set; }
        public Nullable<int> Sequence { get; set; }
    
        public virtual ICollection<Hash_Hit_Statistics> hash_hit_statistics { get; set; }
        public virtual ICollection<Hash_File_Info> hash_file_info { get; set; }
        public virtual ICollection<Hash_File_Info> hash_file_info1 { get; set; }
        public virtual ICollection<Hash_File_Info> hash_file_info2 { get; set; }
        public virtual ICollection<Hash_File_Info> hash_file_info3 { get; set; }
    }
}
