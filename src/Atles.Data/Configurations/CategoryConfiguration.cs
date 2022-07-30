using Atles.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atles.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Category");

            builder
                .HasOne(x => x.Site)
                .WithMany(x => x.Categories)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            builder
                .HasOne(x => x.PermissionSet)
                .WithMany(x => x.Categories)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}