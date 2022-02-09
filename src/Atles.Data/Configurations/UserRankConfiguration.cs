using Atles.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atles.Data.Configurations
{
    public class UserRankConfiguration : IEntityTypeConfiguration<UserRank>
    {
        public void Configure(EntityTypeBuilder<UserRank> builder)
        {
            builder.ToTable("UserRank");

            builder.OwnsMany(s => s.UserLevels, b =>
            {
                b.ToTable("UserLevel")
                    .HasKey("UserRankId", "Type");

                b.Property(e => e.UserRankId);
                b.Property(e => e.Type);
                b.Property(e => e.Count);
                b.Property(e => e.Badge);
            });

            var navigation = builder.Metadata.FindNavigation(nameof(UserRank.UserLevels));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}