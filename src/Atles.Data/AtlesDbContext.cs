using System.Reflection;
using Atles.Domain;
using Microsoft.EntityFrameworkCore;

namespace Atles.Data
{
    public class AtlesDbContext : DbContext
    {
        public AtlesDbContext(DbContextOptions<AtlesDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionSet> PermissionSets { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostReaction> PostReactions { get; set; }
        public DbSet<PostReactionSummary> PostReactionSummaries { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRank> UserRanks { get; set; }
    }
}
