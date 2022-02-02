using Atles.Core.Queries;
using Atles.Data;
using Atles.Domain.Models;
using Atles.Domain.Rules.Posts;
using Microsoft.EntityFrameworkCore;

namespace Atles.Domain.Rules.Handlers.Posts
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
            return await _dbContext.Posts
                .AnyAsync(x => 
                    x.ForumId == query.ForumId &&
                    x.Forum.Category.SiteId == query.SiteId &&
                    x.Id == query.Id &&
                    x.TopicId == null &&
                    x.Status == PostStatusType.Published);
        }
    }
}
