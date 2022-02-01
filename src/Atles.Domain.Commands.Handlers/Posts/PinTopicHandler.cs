using System.Data;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain.Commands.Posts;
using Atles.Domain.Events;
using Atles.Domain.Events.Posts;
using Atles.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Atles.Domain.Commands.Handlers.Posts
{
    public class PinTopicHandler : ICommandHandler<PinTopic>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public PinTopicHandler(AtlesDbContext dbContext, ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }

        public async Task<IEnumerable<IEvent>> Handle(PinTopic command)
        {
            var topic = await _dbContext.Posts
                .FirstOrDefaultAsync(x =>
                    x.Id == command.TopicId &&
                    x.TopicId == null &&
                    x.ForumId == command.ForumId &&
                    x.Forum.Category.SiteId == command.SiteId &&
                    x.Status != PostStatusType.Deleted);

            if (topic == null)
            {
                throw new DataException($"Topic with Id {command.TopicId} not found.");
            }

            topic.Pin(command.Pinned);

            var @event = new TopicPinned
            {
                Pinned = topic.Pinned,
                TargetId = command.TopicId,
                TargetType = nameof(Post),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(@event.ToDbEntity());

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Forum(topic.ForumId));

            return new IEvent[] { @event };
        }
    }
}
