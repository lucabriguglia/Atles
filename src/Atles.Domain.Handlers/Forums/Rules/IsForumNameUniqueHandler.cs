using Atles.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Atles.Domain.Models.Forums;
using Atles.Domain.Models.Forums.Rules;
using Atles.Infrastructure.Queries;

namespace Atles.Domain.Handlers.Forums.Rules
{
    public class IsForumNameUniqueHandler : IQueryHandler<IsForumNameUnique, bool>
    {
        private readonly AtlesDbContext _dbContext;

        public IsForumNameUniqueHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(IsForumNameUnique query)
        {
            bool any;

            if (query.Id != null)
            {
                any = await _dbContext.Forums
                    .AnyAsync(x => x.Category.SiteId == query.SiteId && 
                                   x.CategoryId == query.CategoryId &&
                                   x.Name == query.Name &&
                                   x.Status != ForumStatusType.Deleted &&
                                   x.Id != query.Id);
            }
            else
            {
                any = any = await _dbContext.Forums
                    .AnyAsync(x => x.Category.SiteId == query.SiteId &&
                                   x.CategoryId == query.CategoryId &&
                                   x.Name == query.Name &&
                                   x.Status != ForumStatusType.Deleted);
            }

            return !any;
        }
    }
}
