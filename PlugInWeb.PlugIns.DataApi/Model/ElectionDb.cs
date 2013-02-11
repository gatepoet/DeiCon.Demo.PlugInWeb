using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace PlugInWeb.PlugIns.DataApi.Model
{
    public class ElectionDb : DbContext
    {
        public ElectionDb(string nameOrConnectionString) : base()
        {
            
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Election> Elections { get; set; }

    }
}