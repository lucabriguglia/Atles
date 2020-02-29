using Atlas.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Data.Configurations
{
    public class PermissionSetConfiguration : IEntityTypeConfiguration<PermissionSet>
    {
        public void Configure(EntityTypeBuilder<PermissionSet> builder)
        {
            builder.ToTable("PermissionSet");
        }
    }
}