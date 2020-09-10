using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
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

    public class SiteIdentityDbContext : ApiAuthorizationDbContext<SiteUser, SiteRole>
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
    }

    public class ApiAuthorizationDbContext<TUser, TRole> : IdentityDbContext<TUser, TRole, string>, IPersistedGrantDbContext 
        where TUser : IdentityUser
        where TRole : IdentityRole
    {
        private readonly IOptions<OperationalStoreOptions> _operationalStoreOptions;

        public ApiAuthorizationDbContext(
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
        }
    }

    public class SiteUserStore : UserStore<SiteUser, SiteRole, SiteIdentityDbContext>
    {
        public SiteUserStore(SiteIdentityDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
        }

        public override Task<SiteUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var siteId = Guid.NewGuid(); // Get current site id.

            return Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail && u.SiteId == siteId, cancellationToken);
        }

        public override Task<SiteUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var siteId = Guid.NewGuid(); // Get current site id.

            return Users.FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName && u.SiteId == siteId, cancellationToken);
        }

        protected override Task<SiteRole> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var siteId = Guid.NewGuid(); // Get current site id.

            return Context.Roles.SingleOrDefaultAsync(r => r.NormalizedName == normalizedRoleName && r.SiteId == siteId, cancellationToken);
        }
    }

    public class SiteRoleStore : RoleStore<SiteRole, SiteIdentityDbContext>
    {
        public SiteRoleStore(SiteIdentityDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
        }

        public override Task<SiteRole> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var siteId = Guid.NewGuid(); // Get current site id.

            return Roles.FirstOrDefaultAsync(r => r.NormalizedName == normalizedName && r.SiteId == siteId, cancellationToken);
        }
    }

    public static class Temp
    {
        public static Guid DefaultSiteId => new Guid("C063F119-A898-4ADF-83F9-D69CAF163C6F");
    }
}
