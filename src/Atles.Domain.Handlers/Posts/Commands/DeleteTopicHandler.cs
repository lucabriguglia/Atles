using System.Collections.Generic;
using Atles.Data;
using Atles.Data.Caching;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Atles.Domain.Commands;
using Atles.Domain.Events;
using Atles.Domain.Models;
using Atles.Infrastructure.Commands;
using Atles.Infrastructure.Events;

namespace Atles.Domain.Handlers.Posts.Commands
{
    public class DeleteTopicHandler : ICommandHandler<DeleteTopic>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public DeleteTopicHandler(AtlesDbContext dbContext, ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }

        public async Task<IEnumerable<IEvent>> Handle(DeleteTopic command)
        {
            var topic = await _dbContext.Posts
                .Include(x => x.CreatedByUser)
                .Include(x => x.Forum).ThenInclude(x => x.Category)
                .Include(x => x.Forum).ThenInclude(x => x.LastPost)
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id &&
                    x.TopicId == null &&
                    x.ForumId == command.ForumId &&
                    x.Forum.Category.SiteId == command.SiteId &&
                    x.Status != PostStatusType.Deleted);

            if (topic == null)
            {
                throw new DataException($"Topic with Id {command.Id} not found.");
            }

            topic.Delete();

            var @event = new TopicDeleted
            {
                TargetId = command.Id,
                TargetType = nameof(Post),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(@event.ToDbEntity());

            topic.Forum.DecreaseTopicsCount();
            topic.Forum.DecreaseRepliesCount(topic.RepliesCount);
            topic.Forum.Category.DecreaseTopicsCount();
            topic.Forum.Category.DecreaseRepliesCount(topic.RepliesCount);
            topic.CreatedByUser.DecreaseTopicsCount();
            topic.CreatedByUser.DecreaseRepliesCount(topic.RepliesCount);

            if (topic.Forum.LastPost != null && (topic.Id == topic.Forum.LastPostId || topic.Id == topic.Forum.LastPost.TopicId))
            {
                var newLastPost = await _dbContext.Posts
                    .Where(x => x.ForumId == topic.ForumId &&
                                x.Status == PostStatusType.Published &&
                                (x.Topic == null || x.Topic.Status == PostStatusType.Published) &&
                                x.Id != topic.Id)
                    .OrderByDescending(x => x.CreatedOn)
                    .FirstOrDefaultAsync();

                if (newLastPost != null)
                {
                    topic.Forum.UpdateLastPost(newLastPost.Id);
                }
                else
                {
                    topic.Forum.UpdateLastPost(null);
                }
            }

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Forum(topic.ForumId));

            return new IEvent[] { @event };
        }
    }
}
