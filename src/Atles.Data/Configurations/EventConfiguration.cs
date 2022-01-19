using Atles.Domain;
using Atles.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atles.Data.Configurations
{
    public class EventConfiguration : IEntityTypeConfiguration<HistoryItem>
    {
        public void Configure(EntityTypeBuilder<HistoryItem> builder)
        {
            builder.ToTable("Event");

            builder
                .HasOne(x => x.User)
                .WithMany(x => x.Events)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}