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
    
    public partial class customer:BaseModel
    {
        public customer()
        {
            this.customer_timeline = new HashSet<customer_timeline>();
            this.bank_account = new HashSet<bank_account>();
        }
    
    
    
    
    [EntityPrimKey("customer")]
        public int id { get; set; }
    
    
    
        public string code { get; set; }
    
    
    
        public string name { get; set; }
    
    
    
        public string industry { get; set; }
    
    
    
        public string province { get; set; }
    
    
    
        public string city { get; set; }
    
    
    
        public string county { get; set; }
    
    
    
        public string address { get; set; }
    
    
    
        public string contact { get; set; }
    
    
    
        public string mobile { get; set; }
    
    
    
        public string tel { get; set; }
    
    
    
        public string fax { get; set; }
    
    
    
        public string email { get; set; }
    
    
    
        public string QQ { get; set; }
    
    
    
        public string wechat { get; set; }
    
    
    
        public string source { get; set; }
    
    
    
        public Nullable<int> creator_id { get; set; }
    
    
    
        public Nullable<int> salesman_id { get; set; }
    
    
    
        public Nullable<int> waiter_id { get; set; }
    
    
    
        public Nullable<int> manager_id { get; set; }
    
    
    
        public Nullable<int> outworker_id { get; set; }
    
    
    
        public Nullable<int> organization_id { get; set; }
    
    
    
        public Nullable<int> source_id { get; set; }
    
    
    
        public Nullable<sbyte> status { get; set; }
    
    
    
        public Nullable<System.DateTime> date_created { get; set; }
    
    
    
        public Nullable<System.DateTime> date_updated { get; set; }
    
    
    
        public string description { get; set; }
    
        public virtual member member { get; set; }
        public virtual member member1 { get; set; }
        public virtual member member2 { get; set; }
        public virtual member member3 { get; set; }
        public virtual member member4 { get; set; }
        public virtual ICollection<customer_timeline> customer_timeline { get; set; }
        public virtual ICollection<bank_account> bank_account { get; set; }
    }
}
