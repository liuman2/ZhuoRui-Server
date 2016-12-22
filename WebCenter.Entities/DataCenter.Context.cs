﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<organization> organizations { get; set; }
        public virtual DbSet<position> positions { get; set; }
        public virtual DbSet<area> areas { get; set; }
        public virtual DbSet<role> roles { get; set; }
        public virtual DbSet<dictionary_group> dictionary_group { get; set; }
        public virtual DbSet<dictionary> dictionaries { get; set; }
        public virtual DbSet<sequence> sequences { get; set; }
        public virtual DbSet<bank_account> bank_account { get; set; }
        public virtual DbSet<customer_timeline> customer_timeline { get; set; }
        public virtual DbSet<timeline> timelines { get; set; }
        public virtual DbSet<role_member> role_member { get; set; }
        public virtual DbSet<role_memu> role_memu { get; set; }
        public virtual DbSet<income> incomes { get; set; }
        public virtual DbSet<reg_history> reg_history { get; set; }
        public virtual DbSet<reg_internal_history> reg_internal_history { get; set; }
        public virtual DbSet<operation> operations { get; set; }
        public virtual DbSet<role_operation> role_operation { get; set; }
        public virtual DbSet<lecture_customer> lecture_customer { get; set; }
        public virtual DbSet<menu> menus { get; set; }
        public virtual DbSet<member> members { get; set; }
        public virtual DbSet<audit_bank> audit_bank { get; set; }
        public virtual DbSet<waitdeal> waitdeals { get; set; }
        public virtual DbSet<history> histories { get; set; }
        public virtual DbSet<attachment> attachments { get; set; }
        public virtual DbSet<lecture> lectures { get; set; }
        public virtual DbSet<bank> banks { get; set; }
        public virtual DbSet<leave> leaves { get; set; }
        public virtual DbSet<notice> notices { get; set; }
        public virtual DbSet<annual_exam> annual_exam { get; set; }
        public virtual DbSet<patent> patents { get; set; }
        public virtual DbSet<reg_abroad> reg_abroad { get; set; }
        public virtual DbSet<reg_internal> reg_internal { get; set; }
        public virtual DbSet<mail> mails { get; set; }
        public virtual DbSet<setting> settings { get; set; }
        public virtual DbSet<audit> audits { get; set; }
        public virtual DbSet<receipt> receipts { get; set; }
        public virtual DbSet<trademark> trademarks { get; set; }
        public virtual DbSet<customer> customers { get; set; }
    }
}
