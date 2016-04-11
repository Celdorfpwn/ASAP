using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace EntityFrameworkDb
{
    class DatabaseContext : DbContext
    {
        public DbSet<Setting> Settings { get;set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Setting>().HasKey(setting => setting.Name);

            base.OnModelCreating(modelBuilder);
        }
    }
}
