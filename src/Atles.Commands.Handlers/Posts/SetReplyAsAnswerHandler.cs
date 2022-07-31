using System.Data;
using Atles.Commands.Posts;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Core.Results;
using Atles.Core.Results.Types;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain;
using Atles.Events.Posts;
using Microsoft.EntityFrameworkCore;

namespace Atles.Commands.Handlers.Posts
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

        public async Task<CommandResult> Handle(SetReplyAsAnswer command)
        {
            var reply = await _dbContext.Posts
                .Include(x => x.Topic)
                .Include(x => x.CreatedByUser)
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
            reply.Topic.SetAsAnswered(command.IsAnswer);

            if (command.IsAnswer)
            {
                reply.CreatedByUser.IncreaseAnswersCount();
            }
            else
            {
                reply.CreatedByUser.DecreaseAnswersCount();
            }
            
            var @event = new ReplySetAsAnswer
            {
                IsAnswer = reply.IsAnswer,
                TargetId = command.ReplyId,
                TargetType = nameof(Post),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(@event.ToDbEntity());

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Forum(reply.Topic.ForumId));

            return new Success(new IEvent[] { @event });
        }
    }
}
