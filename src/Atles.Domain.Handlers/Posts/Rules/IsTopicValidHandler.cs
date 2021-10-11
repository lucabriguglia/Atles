using Atles.Data;
using Atles.Domain.Posts;
using Atles.Domain.Posts.Rules;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Queries;
using System.Threading.Tasks;

namespace Atles.Domain.Handlers.Posts.Rules
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
