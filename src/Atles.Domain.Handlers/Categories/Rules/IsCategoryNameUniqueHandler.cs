using Atles.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Atles.Domain.Models;
using Atles.Domain.Rules;
using Atles.Infrastructure.Queries;

namespace Atles.Domain.Handlers.Categories.Rules
{
    public class IsCategoryNameUniqueHandler : IQueryHandler<IsCategoryNameUnique, bool>
    {
        private readonly AtlesDbContext _dbContext;

        public IsCategoryNameUniqueHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(IsCategoryNameUnique query)
        {
            bool any;

            if (query.Id != null)
            {
                any = await _dbContext.Categories
                    .AnyAsync(x => x.SiteId == query.SiteId &&
                                   x.Name == query.Name &&
                                   x.Status != CategoryStatusType.Deleted &&
                                   x.Id != query.Id);
            }
            else
            {
                any = await _dbContext.Categories
                    .AnyAsync(x => x.SiteId == query.SiteId &&
                                   x.Name == query.Name &&
                                   x.Status != CategoryStatusType.Deleted);
            }

            return !any;
        }
    }
}
