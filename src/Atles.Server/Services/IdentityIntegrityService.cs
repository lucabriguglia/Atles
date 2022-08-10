using System.Threading.Tasks;
using Atles.Commands.Users;
using Atles.Core;
using Atles.Data;
using Atles.Queries.Public;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atles.Server.Services
{
    public class IdentityIntegrityService : IIdentityIntegrityService
    {
        private readonly IDispatcher _dispatcher;
        private readonly AtlesDbContext _dbContext;

        public IdentityIntegrityService(IDispatcher dispatcher, AtlesDbContext dbContext)
        {
            _dispatcher = dispatcher;
            _dbContext = dbContext;
        }

        public async Task EnsureUserCreatedAsync(IdentityUser identityUser, bool confirm = false)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.IdentityUserId == identityUser.Id);

            if (user == null)
            {
                // TODO: To be moved to a service
                var getCurrentSiteResult = await _dispatcher.Get(new GetCurrentSite());
                var site = getCurrentSiteResult.AsT0;

                await _dispatcher.Send(new CreateUser
                {
                    Email = identityUser.Email,
                    SiteId = site.Id,
                    Confirm = confirm
                });
            }
        }

        public async Task ConfirmUserAsync(IdentityUser identityUser)
        {
            // TODO: To be moved to a service
            var getCurrentSiteResult = await _dispatcher.Get(new GetCurrentSite());
            var site = getCurrentSiteResult.AsT0;

            await _dispatcher.Send(new ConfirmUser
            {
                IdentityUserId = identityUser.Id,
                SiteId = site.Id
            });
        }

        public async Task UpdateEmailAsync(IdentityUser identityUser)
        {
            // TODO: To be moved to a service
            var getCurrentSiteResult = await _dispatcher.Get(new GetCurrentSite());
            var site = getCurrentSiteResult.AsT0;

            await _dispatcher.Send(new ChangeEmail
            {
                IdentityUserId = identityUser.Id,
                Email = identityUser.Email,
                SiteId = site.Id
            });
        }
    }
}