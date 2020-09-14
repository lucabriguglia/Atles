using System;
using System.Threading.Tasks;
using Atles.Domain.Posts;
using Microsoft.EntityFrameworkCore;

namespace Atles.Data.Rules
{
    public class TopicRules : ITopicRules
    {
        private readonly AtlesDbContext _dbContext;

        public TopicRules(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsValidAsync(Guid siteId, Guid forumId, Guid id)
        {
            var any = await _dbContext.Posts
                .AnyAsync(x => x.ForumId == forumId &&
                               x.Forum.Category.SiteId == siteId &&
                               x.Id == id &&
                               x.TopicId == null &&
                               x.Status == PostStatusType.Published);
            return any;
        }
    }
}
