using System;
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

        public async Task EnsureMemberCreatedAsync(IdentityUser user, bool confirm = false)
        {
            var member = await _dbContext.Users.FirstOrDefaultAsync(x => x.IdentityUserId == user.Id);

            if (member == null)
            {
                var site = await _contextService.CurrentSiteAsync();

                await _memberService.CreateAsync(new CreateUser
                {
                    IdentityUserId = user.Id,
                    Email = user.Email,
                    SiteId = site.Id,
                    Confirm = confirm
                });
            }
        }

        public async Task EnsureMemberConfirmedAsync(IdentityUser user)
        {
            var member = await _dbContext.Users.FirstOrDefaultAsync(x => x.IdentityUserId == user.Id);

            if (member != null && member.Status == StatusType.Pending)
            {
                var site = await _contextService.CurrentSiteAsync();

                await _memberService.ConfirmAsync(new ConfirmUser
                {
                    Id = member.Id,
                    SiteId = site.Id
                });
            }
        }
    }
}