using System.Linq;
using System.Security.Claims;
using Atlas.Caching;
using Atlas.Data;
using Atlas.Models;
using Microsoft.AspNetCore.Http;

namespace Atlas.Services
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

        public Site CurrentSite() =>
            _cacheManager.GetOrSet(CacheKeys.Site("Default"), () => 
                _dbContext.Sites.FirstOrDefault(x => x.Name == "Default"));

        public Member CurrentMember()
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
                    member = _dbContext.Members.FirstOrDefault(x => x.UserId == userId);

                    if (member == null)
                    {
                        member = new Member(userId);
                        _dbContext.Members.Add(member);
                        _dbContext.SaveChanges();
                    }
                }
            }

            _httpContextAccessor.HttpContext.Items[key] = member;

            return member;
        }
    }
}