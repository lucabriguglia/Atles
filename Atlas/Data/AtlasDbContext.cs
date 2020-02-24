using Atlas.Models;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Data
{
    public class AtlasDbContext : DbContext
    {
        public AtlasDbContext(DbContextOptions<AtlasDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Site>()
                .ToTable("Site");
        }

        public DbSet<Site> Sites { get; set; }
    }
}
