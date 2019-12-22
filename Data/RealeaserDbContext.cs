using System;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Releaser.Models.LbCode;

namespace Releaser.Data
{
    public class RealeaserDbContext : DbContext
    {
        public DbSet<DBContact> Contacts { get; set; }


        public RealeaserDbContext() : base()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=amlreleaser.db");
        }
    }
    public class DBContact : Contact
    {
        public Boolean IsMessageSanded { get; set; }
    }
}