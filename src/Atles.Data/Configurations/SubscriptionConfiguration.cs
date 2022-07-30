using Atles.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atles.Data.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscription");

        builder.HasKey(x => new { x.UserId, TargetId = x.ItemId });

        builder
            .HasOne(x => x.User)
            .WithMany(x => x.Subscriptions)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
}