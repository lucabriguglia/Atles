using Atlify.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlify.Data.Configurations
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable("Event");

            builder
                .HasOne(x => x.Member)
                .WithMany(x => x.Events)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}