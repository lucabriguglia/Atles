﻿using Atles.Core.Queries;
using Atles.Data;
using Atles.Domain.Rules.Forums;
using Microsoft.EntityFrameworkCore;

namespace Atles.Domain.Rules.Handlers.Forums
{
    public class IsForumValidHandler : IQueryHandler<IsForumValid, bool>
    {
        private readonly AtlesDbContext _dbContext;

        public IsForumValidHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(IsForumValid query)
        {
            var any = await _dbContext.Forums
                .AnyAsync(x => x.Category.SiteId == query.SiteId &&
                               x.Id == query.Id &&
                               x.Status == ForumStatusType.Published);
            return any;
        }
    }
}
