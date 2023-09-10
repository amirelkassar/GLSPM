using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.EFCore.EntitiesConfigurations
{
    public class IdentityRoleConfig : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(new IdentityRole
            {
                Id="1",
                Name="Admin",
                NormalizedName="Admin".ToUpper(),
            },
            new IdentityRole
            {
                Id = "2",
                Name = "User",
                NormalizedName = "User".ToUpper(),

            });
        }
    }
}
