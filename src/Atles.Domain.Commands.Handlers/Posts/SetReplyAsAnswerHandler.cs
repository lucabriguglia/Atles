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
                    x.Id == command.ReplyId &&
                    x.TopicId == command.TopicId &&
                    x.Topic.ForumId == command.ForumId &&
                    x.Topic.Forum.Category.SiteId == command.SiteId &&
                    x.Status == PostStatusType.Published);

            if (reply == null)
            {
                throw new DataException($"Reply with Id {command.ReplyId} not found.");
            }

            reply.SetAsAnswer(command.IsAnswer);

            var @event = new ReplySetAsAnswer
            {
                IsAnswer = reply.IsAnswer,
                TargetId = command.ReplyId,
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
