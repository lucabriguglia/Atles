using System;
using System.Threading.Tasks;
using Atlify.Domain;
using Atlify.Domain.Forums;
using Microsoft.EntityFrameworkCore;

namespace Atlify.Data.Rules
{
    public class ForumRules : IForumRules
    {
        private readonly AtlifyDbContext _dbContext;

        public ForumRules(AtlifyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsNameUniqueAsync(Guid siteId, Guid categoryId, string name)
        {
            var any = await _dbContext.Forums
                .AnyAsync(x => x.Category.SiteId== siteId &&
                               x.CategoryId == categoryId && 
                               x.Name == name && 
                               x.Status != StatusType.Deleted);
            return !any;
        }

        public async Task<bool> IsNameUniqueAsync(Guid siteId, Guid categoryId, string name, Guid id)
        {
            var any = await _dbContext.Forums
                .AnyAsync(x => x.Category.SiteId == siteId &&
                               x.CategoryId == categoryId &&
                               x.Name == name && 
                               x.Status != StatusType.Deleted &&
                               x.Id != id);
            return !any;
        }

        public async Task<bool> IsValidAsync(Guid siteId, Guid id)
        {
            var any = await _dbContext.Forums
                .AnyAsync(x => x.Category.SiteId == siteId &&
                               x.Id == id &&
                               x.Status == StatusType.Published);
            return any;
        }

        public async Task<bool> IsSlugUniqueAsync(Guid siteId, string slug)
        {
            var any = await _dbContext.Forums
                .AnyAsync(x => x.Category.SiteId == siteId &&
                               x.Slug == slug &&
                               x.Status != StatusType.Deleted);
            return !any;
        }

        public async Task<bool> IsSlugUniqueAsync(Guid siteId, string slug, Guid id)
        {
            var any = await _dbContext.Forums
                .AnyAsync(x => x.Category.SiteId == siteId &&
                               x.Slug == slug &&
                               x.Status != StatusType.Deleted &&
                               x.Id != id);
            return !any;
        }
    }
}
