using Atles.Domain;
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

            var navigation = builder.Metadata.FindNavigation(nameof(PermissionSet.Permissions));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}