using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Atles.Domain.Forums;

namespace Atlas.Data.Rules
{
    public class ForumRules : IForumRules
    {
        private readonly AtlasDbContext _dbContext;

        public ForumRules(AtlasDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsNameUniqueAsync(Guid siteId, Guid categoryId, string name)
        {
            var any = await _dbContext.Forums
                .AnyAsync(x => x.Category.SiteId== siteId &&
                               x.CategoryId == categoryId && 
                               x.Name == name && 
                               x.Status != ForumStatusType.Deleted);
            return !any;
        }

        public async Task<bool> IsNameUniqueAsync(Guid siteId, Guid categoryId, string name, Guid id)
        {
            var any = await _dbContext.Forums
                .AnyAsync(x => x.Category.SiteId == siteId &&
                               x.CategoryId == categoryId &&
                               x.Name == name && 
                               x.Status != ForumStatusType.Deleted &&
                               x.Id != id);
            return !any;
        }

        public async Task<bool> IsValidAsync(Guid siteId, Guid id)
        {
            var any = await _dbContext.Forums
                .AnyAsync(x => x.Category.SiteId == siteId &&
                               x.Id == id &&
                               x.Status == ForumStatusType.Published);
            return any;
        }

        public async Task<bool> IsSlugUniqueAsync(Guid siteId, string slug)
        {
            var any = await _dbContext.Forums
                .AnyAsync(x => x.Category.SiteId == siteId &&
                               x.Slug == slug &&
                               x.Status != ForumStatusType.Deleted);
            return !any;
        }

        public async Task<bool> IsSlugUniqueAsync(Guid siteId, string slug, Guid id)
        {
            var any = await _dbContext.Forums
                .AnyAsync(x => x.Category.SiteId == siteId &&
                               x.Slug == slug &&
                               x.Status != ForumStatusType.Deleted &&
                               x.Id != id);
            return !any;
        }
    }
}
