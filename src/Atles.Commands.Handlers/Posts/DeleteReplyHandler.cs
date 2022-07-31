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
    public class DeleteReplyHandler : ICommandHandler<DeleteReply>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public DeleteReplyHandler(AtlesDbContext dbContext, ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }

        public async Task<CommandResult> Handle(DeleteReply command)
        {
            var reply = await _dbContext.Posts
                .Include(x => x.CreatedByUser)
                .Include(x => x.Topic).ThenInclude(x => x.Forum).ThenInclude(x => x.Category)
                .Include(x => x.Topic).ThenInclude(x => x.Forum).ThenInclude(x => x.LastPost)
                .FirstOrDefaultAsync(x =>
                    x.Id == command.ReplyId &&
                    x.TopicId == command.TopicId &&
                    x.Topic.ForumId == command.ForumId &&
                    x.Topic.Forum.Category.SiteId == command.SiteId &&
                    x.Status != PostStatusType.Deleted);

            if (reply == null)
            {
                throw new DataException($"Reply with Id {command.ReplyId} not found.");
            }

            reply.Delete();

            var @event = new ReplyDeleted
            {
                TargetId = command.ReplyId,
                TargetType = nameof(Post),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(@event.ToDbEntity());

            if (reply.IsAnswer)
            {
                reply.Topic.SetAsAnswered(false);
            }

            reply.Topic.DecreaseRepliesCount();
            reply.Topic.Forum.DecreaseRepliesCount();
            reply.Topic.Forum.Category.DecreaseRepliesCount();
            reply.CreatedByUser.DecreaseRepliesCount();

            if (reply.Topic.Forum.LastPost != null && (reply.Id == reply.Topic.Forum.LastPostId || reply.Id == reply.Topic.Forum.LastPost.TopicId))
            {
                var newLastPost = await _dbContext.Posts
                    .Where(x => x.ForumId == reply.Topic.ForumId &&
                                x.Status == PostStatusType.Published &&
                                (x.Topic == null || x.Topic.Status == PostStatusType.Published) &&
                                x.Id != reply.Id)
                    .OrderByDescending(x => x.CreatedOn)
                    .FirstOrDefaultAsync();

                if (newLastPost != null)
                {
                    reply.Topic.Forum.UpdateLastPost(newLastPost.Id);
                }
                else
                {
                    reply.Topic.Forum.UpdateLastPost(null);
                }
            }

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Forum(reply.Topic.ForumId));

            return new Success(new IEvent[] { @event });
        }
    }
}
