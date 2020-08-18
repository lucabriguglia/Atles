using System.Reflection;
using Atlify.Domain;
using Atlify.Domain.Categories;
using Atlify.Domain.Forums;
using Atlify.Domain.Members;
using Atlify.Domain.PermissionSets;
using Atlify.Domain.Posts;
using Atlify.Domain.Sites;
using Microsoft.EntityFrameworkCore;

namespace Atlify.Data
{
    public class AtlifyDbContext : DbContext
    {
        public AtlifyDbContext(DbContextOptions<AtlifyDbContext> options)
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
        public DbSet<Post> Posts { get; set; }
        public DbSet<Site> Sites { get; set; }
    }
}
