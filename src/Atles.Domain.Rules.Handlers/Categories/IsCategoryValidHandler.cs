using Atles.Core.Queries;
using Atles.Data;
using Atles.Domain.Models;
using Atles.Domain.Rules.Categories;
using Microsoft.EntityFrameworkCore;

namespace Atles.Domain.Rules.Handlers.Categories
{
    public class IsCategoryValidHandler : IQueryHandler<IsCategoryValid, bool>
    {
        private readonly AtlesDbContext _dbContext;

        public IsCategoryValidHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(IsCategoryValid query)
        {
            return await _dbContext.Categories
                .AnyAsync(x => 
                    x.SiteId == query.SiteId &&
                    x.Id == query.Id &&
                    x.Status != CategoryStatusType.Deleted);
        }
    }
}
