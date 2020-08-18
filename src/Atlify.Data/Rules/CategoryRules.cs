using System;
using System.Threading.Tasks;
using Atlify.Domain;
using Atlify.Domain.Categories;
using Microsoft.EntityFrameworkCore;

namespace Atlify.Data.Rules
{
    public class CategoryRules : ICategoryRules
    {
        private readonly AtlifyDbContext _dbContext;

        public CategoryRules(AtlifyDbContext dbContext)
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
