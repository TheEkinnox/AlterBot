using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AlterBotNet.Ressources.Database
{
    public class SqliteDBContext : DbContext
    {
        public DbSet<Bank> Banks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string dbLocation = Assembly.GetEntryAssembly().Location.Replace(@"bin\Debug\netcoreapp2.1", @"Data\Database.sqlite");
            options.UseSqlite("Data Source=" + dbLocation);
        }
    }
}
