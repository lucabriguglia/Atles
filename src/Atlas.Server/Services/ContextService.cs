using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Atlas.Data;
using Atlas.Data.Builders;
using Atlas.Data.Caching;
using Atlas.Domain;
using Atlas.Models.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Atlas.Server.Services
{
    public class ContextService : IContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICacheManager _cacheManager;
        private readonly AtlasDbContext _dbContext;
        private readonly IGravatarService _gravatarService;

        public ContextService(IHttpContextAccessor httpContextAccessor, 
            ICacheManager cacheManager,
            AtlasDbContext dbContext,
            IGravatarService gravatarService)
        {
            _httpContextAccessor = httpContextAccessor;
            _cacheManager = cacheManager;
            _dbContext = dbContext;
            _gravatarService = gravatarService;
        }

        public async Task<CurrentSiteModel> CurrentSiteAsync()
        {
            var currentSite = await _cacheManager.GetOrSetAsync(CacheKeys.CurrentSite("Default"), () => 
                _dbContext.Sites.FirstOrDefaultAsync(x => x.Name == "Default"));

            return new CurrentSiteModel
            {
                Id = currentSite.Id,
                Name = currentSite.Name,
                Title = currentSite.Title,
                Theme = currentSite.PublicTheme,
                CssPublic = currentSite.PublicCss,
                CssAdmin = currentSite.AdminCss,
                Language = currentSite.Language
            };
        }

        public async Task<CurrentMemberModel> CurrentMemberAsync()
        {
            var result = new CurrentMemberModel();

            var user = _httpContextAccessor.HttpContext.User;

            if (user.Identity.IsAuthenticated)
            {
                var userId = _httpContextAccessor.HttpContext.User.Identities.First().Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    var member = await _dbContext.Members.FirstOrDefaultAsync(x => x.UserId == userId);

                    if (member != null)
                    {
                        result = new CurrentMemberModel
                        {
                            Id = member.Id,
                            UserId = member.UserId,
                            Email = member.Email,
                            DisplayName = member.DisplayName,
                            GravatarHash = _gravatarService.HashEmailForGravatar(member.Email)
                        };
                    }
                }
            }

            return result;
        }

        public async Task<IList<CurrentForumModel>> CurrentForumsAsync()
        {
            var site = await CurrentSiteAsync();

            return await _cacheManager.GetOrSetAsync(CacheKeys.CurrentForums(site.Id), async () =>
            {
                var forums = await _dbContext.Forums
                    .Where(x => x.Category.SiteId == site.Id && x.Status == StatusType.Published)
                    .Select(x => new
                    {
                        x.Id,
                        PermissionSetId = x.PermissionSetId ?? x.Category.PermissionSetId
                    })
                    .ToListAsync();

                return forums.Select(forum => new CurrentForumModel
                {
                    Id = forum.Id,
                    PermissionSetId = forum.PermissionSetId
                }).ToList();
            });
        }
    }
}