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
    
    public partial class member:BaseModel
    {
        public member()
        {
            this.customers = new HashSet<customer>();
            this.customers1 = new HashSet<customer>();
            this.annual_exam = new HashSet<annual_exam>();
            this.annual_exam1 = new HashSet<annual_exam>();
            this.annual_exam2 = new HashSet<annual_exam>();
            this.annual_exam3 = new HashSet<annual_exam>();
            this.annual_exam4 = new HashSet<annual_exam>();
            this.annual_exam5 = new HashSet<annual_exam>();
            this.annual_exam6 = new HashSet<annual_exam>();
            this.audits = new HashSet<audit>();
            this.audits1 = new HashSet<audit>();
            this.audits2 = new HashSet<audit>();
            this.audits3 = new HashSet<audit>();
            this.audits4 = new HashSet<audit>();
            this.audits5 = new HashSet<audit>();
            this.reg_abroad = new HashSet<reg_abroad>();
            this.reg_abroad1 = new HashSet<reg_abroad>();
            this.reg_abroad2 = new HashSet<reg_abroad>();
            this.reg_abroad3 = new HashSet<reg_abroad>();
            this.reg_abroad4 = new HashSet<reg_abroad>();
            this.reg_abroad5 = new HashSet<reg_abroad>();
            this.reg_abroad6 = new HashSet<reg_abroad>();
            this.patents = new HashSet<patent>();
            this.patents1 = new HashSet<patent>();
            this.patents2 = new HashSet<patent>();
            this.patents3 = new HashSet<patent>();
            this.patents4 = new HashSet<patent>();
            this.patents5 = new HashSet<patent>();
            this.reg_internal = new HashSet<reg_internal>();
            this.reg_internal1 = new HashSet<reg_internal>();
            this.reg_internal2 = new HashSet<reg_internal>();
            this.reg_internal3 = new HashSet<reg_internal>();
            this.reg_internal4 = new HashSet<reg_internal>();
            this.reg_internal5 = new HashSet<reg_internal>();
            this.reg_internal6 = new HashSet<reg_internal>();
            this.trademarks = new HashSet<trademark>();
            this.trademarks1 = new HashSet<trademark>();
            this.trademarks2 = new HashSet<trademark>();
            this.trademarks3 = new HashSet<trademark>();
            this.trademarks4 = new HashSet<trademark>();
            this.trademarks5 = new HashSet<trademark>();
            this.lectures = new HashSet<lecture>();
        }
    
    
    
    
    [EntityPrimKey("member")]
        public int id { get; set; }
    
    
    
        public Nullable<int> organization_id { get; set; }
    
    
    
        public Nullable<int> area_id { get; set; }
    
    
    
        public Nullable<int> position_id { get; set; }
    
    
    
        public string username { get; set; }
    
    
    
        public string name { get; set; }
    
    
    
        public string english_name { get; set; }
    
    
    
        public string password { get; set; }
    
    
    
        public string mobile { get; set; }
    
    
    
        public Nullable<System.DateTime> hiredate { get; set; }
    
    
    
        public Nullable<System.DateTime> birthday { get; set; }
    
    
    
        public Nullable<int> status { get; set; }
    
    
    
        public Nullable<System.DateTime> date_created { get; set; }
    
    
    
        public Nullable<System.DateTime> date_updated { get; set; }
    
        public virtual area area { get; set; }
        public virtual organization organization { get; set; }
        public virtual position position { get; set; }
        public virtual ICollection<customer> customers { get; set; }
        public virtual ICollection<customer> customers1 { get; set; }
        public virtual ICollection<annual_exam> annual_exam { get; set; }
        public virtual ICollection<annual_exam> annual_exam1 { get; set; }
        public virtual ICollection<annual_exam> annual_exam2 { get; set; }
        public virtual ICollection<annual_exam> annual_exam3 { get; set; }
        public virtual ICollection<annual_exam> annual_exam4 { get; set; }
        public virtual ICollection<annual_exam> annual_exam5 { get; set; }
        public virtual ICollection<annual_exam> annual_exam6 { get; set; }
        public virtual ICollection<audit> audits { get; set; }
        public virtual ICollection<audit> audits1 { get; set; }
        public virtual ICollection<audit> audits2 { get; set; }
        public virtual ICollection<audit> audits3 { get; set; }
        public virtual ICollection<audit> audits4 { get; set; }
        public virtual ICollection<audit> audits5 { get; set; }
        public virtual ICollection<reg_abroad> reg_abroad { get; set; }
        public virtual ICollection<reg_abroad> reg_abroad1 { get; set; }
        public virtual ICollection<reg_abroad> reg_abroad2 { get; set; }
        public virtual ICollection<reg_abroad> reg_abroad3 { get; set; }
        public virtual ICollection<reg_abroad> reg_abroad4 { get; set; }
        public virtual ICollection<reg_abroad> reg_abroad5 { get; set; }
        public virtual ICollection<reg_abroad> reg_abroad6 { get; set; }
        public virtual ICollection<patent> patents { get; set; }
        public virtual ICollection<patent> patents1 { get; set; }
        public virtual ICollection<patent> patents2 { get; set; }
        public virtual ICollection<patent> patents3 { get; set; }
        public virtual ICollection<patent> patents4 { get; set; }
        public virtual ICollection<patent> patents5 { get; set; }
        public virtual ICollection<reg_internal> reg_internal { get; set; }
        public virtual ICollection<reg_internal> reg_internal1 { get; set; }
        public virtual ICollection<reg_internal> reg_internal2 { get; set; }
        public virtual ICollection<reg_internal> reg_internal3 { get; set; }
        public virtual ICollection<reg_internal> reg_internal4 { get; set; }
        public virtual ICollection<reg_internal> reg_internal5 { get; set; }
        public virtual ICollection<reg_internal> reg_internal6 { get; set; }
        public virtual ICollection<trademark> trademarks { get; set; }
        public virtual ICollection<trademark> trademarks1 { get; set; }
        public virtual ICollection<trademark> trademarks2 { get; set; }
        public virtual ICollection<trademark> trademarks3 { get; set; }
        public virtual ICollection<trademark> trademarks4 { get; set; }
        public virtual ICollection<trademark> trademarks5 { get; set; }
        public virtual ICollection<lecture> lectures { get; set; }
    }
}
