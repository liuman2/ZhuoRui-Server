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
    
    public partial class reg_abroad:BaseModel
    {
    
    
    
    [EntityPrimKey("reg_abroad")]
        public int id { get; set; }
    
    
    
        public Nullable<int> customer_id { get; set; }
    
    
    
        public string code { get; set; }
    
    
    
        public string name_cn { get; set; }
    
    
    
        public string name_en { get; set; }
    
    
    
        public Nullable<System.DateTime> date_setup { get; set; }
    
    
    
        public string reg_no { get; set; }
    
    
    
        public string region { get; set; }
    
    
    
        public string address { get; set; }
    
    
    
        public string director { get; set; }
    
    
    
        public Nullable<sbyte> is_open_bank { get; set; }
    
    
    
        public Nullable<int> bank_id { get; set; }
    
    
    
        public Nullable<System.DateTime> date_transaction { get; set; }
    
    
    
        public Nullable<float> amount_transaction { get; set; }
    
    
    
        public string invoice_name { get; set; }
    
    
    
        public string invoice_tax { get; set; }
    
    
    
        public string invoice_address { get; set; }
    
    
    
        public string invoice_tel { get; set; }
    
    
    
        public string invoice_bank { get; set; }
    
    
    
        public string invoice_account { get; set; }
    
    
    
        public Nullable<sbyte> status { get; set; }
    
    
    
        public Nullable<int> finance_reviewer_id { get; set; }
    
    
    
        public Nullable<System.DateTime> finance_review_date { get; set; }
    
    
    
        public string finance_review_moment { get; set; }
    
    
    
        public Nullable<int> submit_reviewer_id { get; set; }
    
    
    
        public Nullable<System.DateTime> submit_review_date { get; set; }
    
    
    
        public string submit_review_moment { get; set; }
    
    
    
        public Nullable<int> review_status { get; set; }
    
    
    
        public Nullable<System.DateTime> date_finish { get; set; }
    
    
    
        public Nullable<int> creator_id { get; set; }
    
    
    
        public Nullable<int> salesman_id { get; set; }
    
    
    
        public Nullable<int> waiter_id { get; set; }
    
    
    
        public Nullable<int> manager_id { get; set; }
    
    
    
        public Nullable<int> outworker_id { get; set; }
    
    
    
        public Nullable<int> organization_id { get; set; }
    
    
    
        public string description { get; set; }
    
    
    
        public Nullable<System.DateTime> date_created { get; set; }
    
    
    
        public Nullable<System.DateTime> date_updated { get; set; }
    
    
    
        public string currency { get; set; }
    
    
    
        public string progress { get; set; }
    
    
    
        public Nullable<int> annual_year { get; set; }
    
        public virtual bank_account bank_account { get; set; }
        public virtual customer customer { get; set; }
        public virtual member member { get; set; }
        public virtual member member1 { get; set; }
        public virtual member member2 { get; set; }
        public virtual member member3 { get; set; }
        public virtual member member4 { get; set; }
        public virtual member member5 { get; set; }
        public virtual member member6 { get; set; }
    }
}
