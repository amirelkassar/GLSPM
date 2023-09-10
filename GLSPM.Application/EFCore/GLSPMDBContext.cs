using GLSPM.Application.EFCore.EntitiesConfigurations;
using GLSPM.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.EFCore
{
    public class GLSPMDBContext : IdentityDbContext<ApplicationUser>
    {
        public GLSPMDBContext(DbContextOptions<GLSPMDBContext> options):base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Card> Cards { get; set; }
        public DbSet<Password> Passwords { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Card>(config =>
            {
                config.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserID);

                config.HasQueryFilter(c => c.IsSoftDeleted == false);
            });

            builder.Entity<Password>(config =>
            {
                config.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserID);

                config.HasQueryFilter(p => p.IsSoftDeleted == false);

            });

            builder.ApplyConfiguration(new IdentityRoleConfig());
        }

    }
}
