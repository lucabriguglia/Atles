﻿using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Data;
using Atles.Domain.Rules.Categories;
using Microsoft.EntityFrameworkCore;

namespace Atles.Domain.Rules.Handlers.Categories
{
    public class IsCategoryNameUniqueHandler : IQueryHandler<IsCategoryNameUnique, bool>
    {
        private readonly AtlesDbContext _dbContext;

        public IsCategoryNameUniqueHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<QueryResult<bool>> Handle(IsCategoryNameUnique query)
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
