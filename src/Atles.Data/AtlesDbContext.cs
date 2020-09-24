using System.Reflection;
using Atles.Domain;
using Atles.Domain.Categories;
using Atles.Domain.Forums;
using Atles.Domain.PermissionSets;
using Atles.Domain.Posts;
using Atles.Domain.Sites;
using Atles.Domain.Themes;
using Atles.Domain.Users;
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
        public DbSet<PermissionSet> PermissionSets { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Theme> Themes { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
