using System.Collections.Generic;
using Atles.Data;
using Atles.Data.Caching;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading.Tasks;
using Atles.Domain.Models;
using Atles.Domain.Models.Posts;
using Atles.Domain.Models.Posts.Commands;
using Atles.Domain.Models.Posts.Events;
using Atles.Infrastructure.Commands;
using Atles.Infrastructure.Events;

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

        public async Task<IEnumerable<IEvent>> Handle(SetReplyAsAnswer command)
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

            var @event = new ReplySetAsAnswer
            {
                IsAnswer = reply.IsAnswer,
                TargetId = command.Id,
                TargetType = nameof(Post),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(@event.ToDbEntity());

            reply.Topic.SetAsAnswered(command.IsAnswer);

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Forum(reply.Topic.ForumId));

            return new IEvent[] { @event };
        }
    }
}
