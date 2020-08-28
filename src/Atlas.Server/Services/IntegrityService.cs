using System.Threading.Tasks;
using Atlas.Data;
using Atlas.Domain;
using Atlas.Domain.Members;
using Atlas.Domain.Members.Commands;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Server.Services
{
    public class IntegrityService : IIntegrityService
    {
        private readonly IContextService _contextService;
        private readonly IMemberService _memberService;
        private readonly AtlasDbContext _dbContext;

        public IntegrityService(IContextService contextService, 
            IMemberService memberService, 
            AtlasDbContext dbContext)
        {
            _contextService = contextService;
            _memberService = memberService;
            _dbContext = dbContext;
        }

        public async Task EnsureMemberCreatedAsync(IdentityUser user)
        {
            var member = await _dbContext.Members.FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (member == null)
            {
                var site = await _contextService.CurrentSiteAsync();

                await _memberService.CreateAsync(new CreateMember
                {
                    UserId = user.Id,
                    Email = user.Email,
                    SiteId = site.Id
                });
            }
        }

        public async Task EnsureMemberConfirmedAsync(IdentityUser user)
        {
            var member = await _dbContext.Members.FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (member != null && member.Status == StatusType.Pending)
            {
                var site = await _contextService.CurrentSiteAsync();

                await _memberService.ConfirmAsync(new ConfirmMember
                {
                    UserId = user.Id,
                    SiteId = site.Id
                });
            }
        }
    }
}