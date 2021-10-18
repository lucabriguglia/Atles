using System.Threading.Tasks;
using Atles.Data;
using Atles.Domain.Users;
using Atles.Domain.Users.Commands;
using Atles.Reporting.Public.Queries;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenCqrs;

namespace Atles.Server.Services
{
    public class IntegrityService : IIntegrityService
    {
        private readonly IDispatcher _dispatcher;
        private readonly AtlesDbContext _dbContext;

        public IntegrityService(IDispatcher sender, AtlesDbContext dbContext)
        {
            _dispatcher = sender;
            _dbContext = dbContext;
        }

        public async Task EnsureUserCreatedAsync(IdentityUser identityUser, bool confirm = false)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.IdentityUserId == identityUser.Id);

            if (user == null)
            {
                var site = await _dispatcher.Get(new GetCurrentSite());

                await _dispatcher.Send(new CreateUser
                {
                    IdentityUserId = identityUser.Id,
                    Email = identityUser.Email,
                    SiteId = site.Id,
                    Confirm = confirm
                });
            }
        }

        public async Task EnsureUserConfirmedAsync(IdentityUser identityUser)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.IdentityUserId == identityUser.Id);

            if (user != null && user.Status == UserStatusType.Pending)
            {
                var site = await _dispatcher.Get(new GetCurrentSite());

                await _dispatcher.Send(new ConfirmUser
                {
                    Id = user.Id,
                    SiteId = site.Id
                });
            }
        }
    }
}