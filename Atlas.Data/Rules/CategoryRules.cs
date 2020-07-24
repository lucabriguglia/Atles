using Atlas.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Atlas.Domain.Categories;

namespace Atlas.Data.Rules
{
    public class CategoryRules : ICategoryRules
    {
        private readonly AtlasDbContext _dbContext;

        public CategoryRules(AtlasDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsNameUniqueAsync(Guid siteId, string name)
        {
            var any = await _dbContext.Categories
                .AnyAsync(x => x.SiteId == siteId && 
                               x.Name == name && 
                               x.Status != StatusType.Deleted);
            return !any;
        }

        public async Task<bool> IsNameUniqueAsync(Guid siteId, string name, Guid id)
        {
            var any = await _dbContext.Categories
                .AnyAsync(x => x.SiteId == siteId && 
                               x.Name == name && 
                               x.Status != StatusType.Deleted &&
                               x.Id != id);
            return !any;
        }
    }
}
