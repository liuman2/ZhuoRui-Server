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
        public virtual DbSet<sequence> sequences { get; set; }
        public virtual DbSet<area> areas { get; set; }
        public virtual DbSet<role> roles { get; set; }
        public virtual DbSet<dictionary_group> dictionary_group { get; set; }
        public virtual DbSet<dictionary> dictionaries { get; set; }
        public virtual DbSet<member> members { get; set; }
        public virtual DbSet<customer> customers { get; set; }
    }
}
