using Atlas.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Data.Configurations
{
    public class PermissionSetConfiguration : IEntityTypeConfiguration<PermissionSet>
    {
        public void Configure(EntityTypeBuilder<PermissionSet> builder)
        {
            builder.ToTable("PermissionSet");

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