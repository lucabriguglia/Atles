using System.Reflection;
using Atles.Domain.Models;
using Atles.Domain.Models.Categories;
using Atles.Domain.Models.Forums;
using Atles.Domain.Models.PermissionSets;
using Atles.Domain.Models.PostReactions;
using Atles.Domain.Models.Posts;
using Atles.Domain.Models.Sites;
using Atles.Domain.Models.Users;
using Atles.Domain.Posts;
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
        public DbSet<User> Users { get; set; }
        public DbSet<PermissionSet> PermissionSets { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PostReactionCount> PostReactionCounts { get; set; }
        public DbSet<PostReaction> PostReactions { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Site> Sites { get; set; }
    }
}
