using System;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Releaser.Models.LbCode;

namespace Releaser.Data
{
    public class RealeaserDbContext : DbContext
    {
        public DbSet<Contact> Contacts { get; set; }


        public RealeaserDbContext() : base()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=amlreleaser.db");
        }
    }
}