using Atles.Domain.PermissionSets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atles.Data.Configurations
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permission");

            builder.HasKey(x => new { x.PermissionSetId, x.RoleId, x.Type });

            builder
                .HasOne(x => x.PermissionSet)
                .WithMany(x => x.Permissions)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
        }
    }
}