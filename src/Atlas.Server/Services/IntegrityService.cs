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
        private readonly IUserService _memberService;
        private readonly AtlasDbContext _dbContext;

        public IntegrityService(IContextService contextService, 
            IUserService memberService, 
            AtlasDbContext dbContext)
        {
            _contextService = contextService;
            _memberService = memberService;
            _dbContext = dbContext;
        }

        public async Task EnsureUserCreatedAsync(IdentityUser identityUser, bool confirm = false)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.IdentityUserId == identityUser.Id);

            if (user == null)
            {
                var site = await _contextService.CurrentSiteAsync();

                await _memberService.CreateAsync(new CreateUser
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

            if (user != null && user.Status == StatusType.Pending)
            {
                var site = await _contextService.CurrentSiteAsync();

                await _memberService.ConfirmAsync(new ConfirmUser
                {
                    Id = user.Id,
                    SiteId = site.Id
                });
            }
        }
    }
}