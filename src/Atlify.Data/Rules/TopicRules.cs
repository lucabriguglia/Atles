using System;
using System.Threading.Tasks;
using Atlify.Domain;
using Atlify.Domain.Posts;
using Microsoft.EntityFrameworkCore;

namespace Atlify.Data.Rules
{
    public class TopicRules : ITopicRules
    {
        private readonly AtlifyDbContext _dbContext;

        public TopicRules(AtlifyDbContext dbContext)
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
                               x.Status == StatusType.Published);
            return any;
        }
    }
}
