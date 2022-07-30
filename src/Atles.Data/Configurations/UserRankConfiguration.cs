using Atles.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atles.Data.Configurations
{
    public class UserRankConfiguration : IEntityTypeConfiguration<UserRank>
    {
        public void Configure(EntityTypeBuilder<UserRank> builder)
        {
            builder.ToTable("UserRank");

            builder.OwnsMany(s => s.UserRankRules, b =>
            {
                b.ToTable("UserRankRule")
                    .HasKey("UserRankId", "Type");

                b.Property(e => e.Name);
                b.Property(e => e.Description);
                b.Property(e => e.Type);
                b.Property(e => e.Count);
                b.Property(e => e.Badge);
            });

            var navigation = builder.Metadata.FindNavigation(nameof(UserRank.UserRankRules));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}