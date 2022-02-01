using Atles.Core.Queries;
using Atles.Data;
using Atles.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Atles.Domain.Rules.Handlers
{
    public class IsForumSlugUniqueHandler : IQueryHandler<IsForumSlugUnique, bool>
    {
        private readonly AtlesDbContext _dbContext;

        public IsForumSlugUniqueHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(IsForumSlugUnique query)
        {
            bool any;

            if (query.Id != null)
            {
                any = await _dbContext.Forums
                    .AnyAsync(x => x.Category.SiteId == query.SiteId &&
                                   x.Slug == query.Slug &&
                                   x.Status != ForumStatusType.Deleted &&
                                   x.Id != query.Id);
            }
            else
            {
                any = any = await _dbContext.Forums
                    .AnyAsync(x => x.Category.SiteId == query.SiteId &&
                                   x.Slug == query.Slug &&
                                   x.Status != ForumStatusType.Deleted);
            }

            return !any;
        }
    }
}
