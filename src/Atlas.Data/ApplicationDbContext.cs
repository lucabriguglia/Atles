using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Atlas.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<IdentityUser>
    {
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }
    }

    public class SiteIdentityDbContext : SiteApiAuthorizationDbContext<SiteUser>
    {
        public SiteIdentityDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }
    }

    public class SiteUser : IdentityUser
    {
        public Guid SiteId { get; set; }
    }

    public class SiteRole : IdentityRole
    {
        public Guid SiteId { get; set; }

        public SiteRole()
        {
        }

        public SiteRole(Guid siteId, string roleName) : base(roleName)
        {
            SiteId = siteId;
        }
    }

    public class SiteApiAuthorizationDbContext<TUser> : SiteApiAuthorizationDbContext<TUser, SiteRole, string>
        where TUser : IdentityUser
    {
        public SiteApiAuthorizationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions)
        {
        }
    }

    public class SiteApiAuthorizationDbContext<TUser, TRole, TKey> : IdentityDbContext<TUser, TRole, TKey>, IPersistedGrantDbContext 
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly IOptions<OperationalStoreOptions> _operationalStoreOptions;

        public SiteApiAuthorizationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options)
        {
            _operationalStoreOptions = operationalStoreOptions;
        }

        public DbSet<PersistedGrant> PersistedGrants { get; set; }

        public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }

        Task<int> IPersistedGrantDbContext.SaveChangesAsync() => base.SaveChangesAsync();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ConfigurePersistedGrantContext(_operationalStoreOptions.Value);

            //builder.Entity<SiteUser>(b =>
            //{
            //    b.HasIndex(r => new { r.SiteId, r.NormalizedEmail }).HasName("EmailIndex2").IsUnique();
            //    b.HasIndex(r => new { r.SiteId, r.NormalizedUserName }).HasName("UserNameIndex2").IsUnique();
            //});

            //builder.Entity<SiteRole>(b =>
            //{
            //    b.HasIndex(r => new { r.SiteId, r.NormalizedName }).HasName("RoleNameIndex2").IsUnique();
            //});
        }
    }

    public class SiteUserStore : UserStore<SiteUser, SiteRole, SiteIdentityDbContext>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AtlasDbContext _dbContext;

        public SiteUserStore(SiteIdentityDbContext context, IHttpContextAccessor httpContextAccessor, AtlasDbContext dbContext, IdentityErrorDescriber describer = null) : base(context, describer)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        public override Task<SiteUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var siteId = new Temp(_httpContextAccessor, _dbContext).GetSiteId(); // Get current site id.

            return Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail && u.SiteId == siteId, cancellationToken);
        }

        public override Task<SiteUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var siteId = new Temp(_httpContextAccessor, _dbContext).GetSiteId(); // Get current site id.

            return Users.FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName && u.SiteId == siteId, cancellationToken);
        }

        protected override Task<SiteRole> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var siteId = new Temp(_httpContextAccessor, _dbContext).GetSiteId(); // Get current site id.

            return Context.Roles.SingleOrDefaultAsync(r => r.NormalizedName == normalizedRoleName && r.SiteId == siteId, cancellationToken);
        }
    }

    public class SiteRoleStore : RoleStore<SiteRole, SiteIdentityDbContext>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AtlasDbContext _dbContext;

        public SiteRoleStore(SiteIdentityDbContext context, IHttpContextAccessor httpContextAccessor, AtlasDbContext dbContext, IdentityErrorDescriber describer = null) : base(context, describer)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        public override Task<SiteRole> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var siteId = new Temp(_httpContextAccessor, _dbContext).GetSiteId(); // Get current site id.

            return Roles.FirstOrDefaultAsync(r => r.NormalizedName == normalizedName && r.SiteId == siteId, cancellationToken);
        }
    }

    public class Temp
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AtlasDbContext _dbContext;

        public Temp(IHttpContextAccessor httpContextAccessor, AtlasDbContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        public Guid GetSiteId()
        {
            //return new Guid("3b261ff5-4d37-4ed6-b810-dbadd8b8c0d5");

            var host = _httpContextAccessor.HttpContext != null
                ? _httpContextAccessor.HttpContext.Request.Host.Value
                : string.Empty;

            var name = string.Empty;

            if (host == "localhost:5001")
            {
                name = "Default";
            }
            else if (host == "localhost:6001")
            {
                name = "X";
            }

            return _dbContext.Sites.FirstOrDefault(x => x.Name == name).Id;
        }
    }
}
