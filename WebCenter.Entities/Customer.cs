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
            this.annual_exam = new HashSet<annual_exam>();
            this.audits = new HashSet<audit>();
            this.bank_account = new HashSet<bank_account>();
            this.reg_abroad = new HashSet<reg_abroad>();
            this.sub_audit = new HashSet<sub_audit>();
            this.lecture_customer = new HashSet<lecture_customer>();
            this.incomes = new HashSet<income>();
            this.patents = new HashSet<patent>();
            this.customer_timeline = new HashSet<customer_timeline>();
            this.trademarks = new HashSet<trademark>();
            this.reg_internal = new HashSet<reg_internal>();
        }
    
    
    
    
    [EntityPrimKey("customer")]
        public int id { get; set; }
    
    
    
        public string code { get; set; }
    
    
    
        public string name { get; set; }
    
    
    
        public string industry { get; set; }
    
    
    
        public string business_nature { get; set; }
    
    
    
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
    
    
    
        public Nullable<int> organization_id { get; set; }
    
    
    
        public Nullable<int> source_id { get; set; }
    
    
    
        public Nullable<sbyte> status { get; set; }
    
    
    
        public Nullable<System.DateTime> date_created { get; set; }
    
    
    
        public Nullable<System.DateTime> date_updated { get; set; }
    
    
    
        public string description { get; set; }
    
    
    
        public string contacts { get; set; }
    
    
    
        public Nullable<int> assistant_id { get; set; }
    
    
    
        public string assistants { get; set; }
    
        public virtual ICollection<annual_exam> annual_exam { get; set; }
        public virtual ICollection<audit> audits { get; set; }
        public virtual ICollection<bank_account> bank_account { get; set; }
        public virtual ICollection<reg_abroad> reg_abroad { get; set; }
        public virtual ICollection<sub_audit> sub_audit { get; set; }
        public virtual ICollection<lecture_customer> lecture_customer { get; set; }
        public virtual ICollection<income> incomes { get; set; }
        public virtual member member { get; set; }
        public virtual member member1 { get; set; }
        public virtual ICollection<patent> patents { get; set; }
        public virtual ICollection<customer_timeline> customer_timeline { get; set; }
        public virtual ICollection<trademark> trademarks { get; set; }
        public virtual ICollection<reg_internal> reg_internal { get; set; }
    }
}
