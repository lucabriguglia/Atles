using System;
using System.Threading.Tasks;
using Atles.Domain.Categories;
using Microsoft.EntityFrameworkCore;

namespace Atles.Data.Rules
{
    public class CategoryRules : ICategoryRules
    {
        private readonly AtlesDbContext _dbContext;

        public CategoryRules(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsNameUniqueAsync(Guid siteId, string name)
        {
            var any = await _dbContext.Categories
                .AnyAsync(x => x.SiteId == siteId && 
                               x.Name == name && 
                               x.Status != CategoryStatusType.Deleted);
            return !any;
        }

        public async Task<bool> IsNameUniqueAsync(Guid siteId, string name, Guid id)
        {
            var any = await _dbContext.Categories
                .AnyAsync(x => x.SiteId == siteId && 
                               x.Name == name && 
                               x.Status != CategoryStatusType.Deleted &&
                               x.Id != id);
            return !any;
        }
    }
}
