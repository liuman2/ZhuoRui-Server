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
    
    public partial class Hash_File_Info : BaseModel
    {
        public Hash_File_Info()
        {
            this.hash_arithmetic = new HashSet<Hash_Arithmetic>();
            this.hash_file_property = new HashSet<Hash_File_Property>();
            this.hash_file_info_audit = new HashSet<Hash_File_Info_Audit>();
            this.hash_hit_info = new HashSet<Hash_Hit_Info>();
        }
    
    	[EntityPrimKey]
        public string File_Info_ID { get; set; }
        public string Swatch_Name { get; set; }
        public string Swatch_Path { get; set; }
        public string MD5 { get; set; }
        public string File_Translate { get; set; }
        public Nullable<decimal> File_Size { get; set; }
        public string Source { get; set; }
        public Nullable<System.DateTime> Audit_Time { get; set; }
        public Nullable<System.DateTime> Upload_Time { get; set; }
        public Nullable<System.DateTime> Check_Time { get; set; }
        public string Path { get; set; }
        public Nullable<int> Status { get; set; }
        public string Public_Name { get; set; }
        public string Sub_Title { get; set; }
        public string Descriptions { get; set; }
        public string Remark { get; set; }
        public string SuggestInfo { get; set; }
        public string Extent_Name { get; set; }
    
        public virtual ICollection<Hash_Arithmetic> hash_arithmetic { get; set; }
        public virtual Sys_User sys_user { get; set; }
        public virtual ICollection<Hash_File_Property> hash_file_property { get; set; }
        public virtual Hash_File_Upload_Info hash_file_upload_info { get; set; }
        public virtual ICollection<Hash_File_Info_Audit> hash_file_info_audit { get; set; }
        public virtual Sys_Dictionary sys_dictionary_fileType { get; set; }
        public virtual Sys_Dictionary sys_dictionary_language { get; set; }
        public virtual Sys_Dictionary sys_dictionary_levels { get; set; }
        public virtual Sys_Dictionary sys_dictionary_publicid { get; set; }
        public virtual ICollection<Hash_Hit_Info> hash_hit_info { get; set; }
    }
}
