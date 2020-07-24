using System.Reflection;
using Atlas.Domain;
using Atlas.Domain.Categories;
using Atlas.Domain.Forums;
using Atlas.Domain.PermissionSets;
using Atlas.Domain.Sites;
using Atlas.Domain.Topics;
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

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<PermissionSet> PermissionSets { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Topic> Topics { get; set; }
    }
}
