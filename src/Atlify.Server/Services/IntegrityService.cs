using System.Threading.Tasks;
using Atlify.Data;
using Atlify.Domain.Members;
using Atlify.Domain.Members.Commands;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atlify.Server.Services
{
    public class IntegrityService : IIntegrityService
    {
        private readonly IContextService _contextService;
        private readonly IMemberService _memberService;
        private readonly AtlifyDbContext _dbContext;

        public IntegrityService(IContextService contextService, 
            IMemberService memberService, 
            AtlifyDbContext dbContext)
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
    }
}