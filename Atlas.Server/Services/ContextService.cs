using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Atlas.Server.Caching;
using Atlas.Data;
using Atlas.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Server.Services
{
    public class ContextService : IContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICacheManager _cacheManager;
        private readonly AtlasDbContext _dbContext;

        public ContextService(IHttpContextAccessor httpContextAccessor, 
            ICacheManager cacheManager,
            AtlasDbContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _cacheManager = cacheManager;
            _dbContext = dbContext;
        }

        public async Task<Site> CurrentSiteAsync() =>
            await _cacheManager.GetOrSetAsync(CacheKeys.Site("Default"), () => 
                _dbContext.Sites.FirstOrDefaultAsync(x => x.Name == "Default"));

        public async Task<Member> CurrentMemberAsync()
        {
            const string key = "Member";

            if (_httpContextAccessor.HttpContext.Items[key] != null)
            {
                return (Member)_httpContextAccessor.HttpContext.Items[key];
            }

            Member member = null;

            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var userId = _httpContextAccessor.HttpContext.User.Identities.First().Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    member = await _dbContext.Members.FirstOrDefaultAsync(x => x.UserId == userId);

                    if (member == null)
                    {
                        member = new Member(userId);
                        _dbContext.Members.Add(member);
                        await _dbContext.SaveChangesAsync();
                    }
                }
            }

            _httpContextAccessor.HttpContext.Items[key] = member;

            return member;
        }
    }
}