using Atlas.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Atlas.Domain.Posts;

namespace Atlas.Data.Rules
{
    public class TopicRules : ITopicRules
    {
        private readonly AtlasDbContext _dbContext;

        public TopicRules(AtlasDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsValidAsync(Guid siteId, Guid forumId, Guid id)
        {
            var any = await _dbContext.Posts
                .AnyAsync(x => x.ForumId == forumId &&
                               x.Forum.Category.SiteId == siteId &&
                               x.Id == id &&
                               x.Status == StatusType.Published);
            return any;
        }
    }
}
