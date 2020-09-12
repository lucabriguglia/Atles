using System.Threading.Tasks;
using Atlas.Data;
using Atlas.Domain;
using Atlas.Domain.Users;
using Atlas.Domain.Users.Commands;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Server.Services
{
    public class IntegrityService : IIntegrityService
    {
        private readonly IContextService _contextService;
        private readonly IUserService _userService;
        private readonly AtlasDbContext _dbContext;

        public IntegrityService(IContextService contextService, 
            IUserService userService, 
            AtlasDbContext dbContext)
        {
            _contextService = contextService;
            _userService = userService;
            _dbContext = dbContext;
        }

        public async Task EnsureUserCreatedAsync(IdentityUser identityUser, bool confirm = false)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.IdentityUserId == identityUser.Id);

            if (user == null)
            {
                var site = await _contextService.CurrentSiteAsync();

                await _userService.CreateAsync(new CreateUser
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
                var site = await _contextService.CurrentSiteAsync();

                await _userService.ConfirmAsync(new ConfirmUser
                {
                    Id = user.Id,
                    SiteId = site.Id
                });
            }
        }
    }
}