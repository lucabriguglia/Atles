using Atles.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Atles.Domain.Models.Forums;
using Atles.Domain.Models.Forums.Rules;
using Atles.Infrastructure.Queries;

namespace Atles.Domain.Handlers.Forums.Rules
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
