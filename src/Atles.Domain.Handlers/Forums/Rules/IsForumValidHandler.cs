using Atles.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Atles.Domain.Models.Forums;
using Atles.Domain.Models.Forums.Rules;
using Atles.Infrastructure.Queries;

namespace Atles.Domain.Handlers.Forums.Rules
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
