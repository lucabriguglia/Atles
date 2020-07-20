using Atlas.Server.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Data.Configurations
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