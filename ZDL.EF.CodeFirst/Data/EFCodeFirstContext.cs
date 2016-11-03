using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFModels = ZDL.EF.CodeFirst.Model;

namespace ZDL.EF.CodeFirst.Data
{
    public class EFCodeFirstContext : DbContext
    {
        public EFCodeFirstContext(string dbBaseName) : base(dbBaseName) { }
      
        public DbSet<EFModels.Address> Addresses { get; set; }
        public DbSet<EFModels.Contact> Contacts { get; set; }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //    modelBuilder.Entity<EFModels.Contact>().HasOptional(c => c.Address)
        //        .WithOptionalDependent(add => add.Contact);
        //    modelBuilder.Entity<EFModels.CGroup>().HasMany(c => c.Contacts)
        //        .WithRequired(c => c.CGroup).WillCascadeOnDelete(false);
        //    modelBuilder.Entity<EFModels.Contact>().HasOptional(c => c.CGroup)
        //        .WithMany(c => c.Contacts).WillCascadeOnDelete(true);
        //}
    }
}
