using Atles.Data;
using Atles.Data.Caching;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading.Tasks;
using Atles.Domain.Models;
using Atles.Domain.Models.Posts;
using Atles.Domain.Models.Posts.Commands;
using Atles.Infrastructure.Commands;

namespace Atles.Domain.Handlers.Posts.Commands
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

        public async Task Handle(PinTopic command)
        {
            var topic = await _dbContext.Posts
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

            topic.Pin(command.Pinned);

            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Pinned,
                typeof(Post),
                command.Id));

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Forum(topic.ForumId));
        }
    }
}
