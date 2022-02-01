using Atles.Core.Queries;
using Atles.Data;
using Atles.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Atles.Domain.Rules.Handlers
{
    public class IsTopicValidHandler : IQueryHandler<IsTopicValid, bool>
    {
        private readonly AtlesDbContext _dbContext;

        public IsTopicValidHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(IsTopicValid query)
        {
            var any = await _dbContext.Posts
                .AnyAsync(x => x.ForumId == query.ForumId &&
                               x.Forum.Category.SiteId == query.SiteId &&
                               x.Id == query.Id &&
                               x.TopicId == null &&
                               x.Status == PostStatusType.Published);
            return any;
        }
    }
}
