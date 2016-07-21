using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using WebCenter.Entities;

namespace WebCenter.Services.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<DataCenter>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(DataCenter context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            //var syshelps = new[]{
            //           new SysHelp {  Title = "DbSet<T>.AddOrUpdate()测试记录。", Content = "默认以主键来判断该记录是否存在，并进行添加更新。" },
            //           new SysHelp {  Title = "新加功能模块及内容可在Configuraion.cs中进行添加", Content = "默认以主键来判断该记录是否存在，并进行添加更新。" } 
            //    };

            //context.SysHelps.AddOrUpdate(a => new { a.Title }, syshelps);
        }
    }
}