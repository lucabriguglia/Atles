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
    public class SetReplyAsAnswerHandler : ICommandHandler<SetReplyAsAnswer>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public SetReplyAsAnswerHandler(AtlesDbContext dbContext, ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }

        public async Task Handle(SetReplyAsAnswer command)
        {
            var reply = await _dbContext.Posts
                .Include(x => x.Topic)
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id &&
                    x.TopicId == command.TopicId &&
                    x.Topic.ForumId == command.ForumId &&
                    x.Topic.Forum.Category.SiteId == command.SiteId &&
                    x.Status == PostStatusType.Published);

            if (reply == null)
            {
                throw new DataException($"Reply with Id {command.Id} not found.");
            }

            reply.SetAsAnswer(command.IsAnswer);

            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Updated,
                typeof(Post),
                command.Id,
                new
                {
                    command.IsAnswer
                }));

            reply.Topic.SetAsAnswered(command.IsAnswer);

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Forum(reply.Topic.ForumId));
        }
    }
}
