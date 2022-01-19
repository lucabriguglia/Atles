using Atles.Domain.Models.PermissionSets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atles.Data.Configurations
{
    public class PermissionSetConfiguration : IEntityTypeConfiguration<PermissionSet>
    {
        public void Configure(EntityTypeBuilder<PermissionSet> builder)
        {
            builder.ToTable("PermissionSet");

            builder.OwnsMany(s => s.Permissions, b =>
            {
                b.ToTable("Permission")
                    .HasKey("PermissionSetId", "RoleId", "Type");

                b.Property(e => e.Type);
                b.Property(e => e.RoleId);
            });

            //builder
            //    .Property(x => x.Permissions)
            //    .HasField("_permissions");

            //builder.Metadata.FindNavigation("Permissions")
            //    .SetPropertyAccessMode(PropertyAccessMode.Field);

            //builder
            //    .HasMany(e => e.Permissions)
            //    .WithOne(e => e.PermissionSet)
            //    .Metadata.PrincipalToDependent.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}