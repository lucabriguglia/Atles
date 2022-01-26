using Atles.Data;
using Atles.Data.Caching;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Atles.Domain.Models;
using Atles.Domain.Models.Posts;
using Atles.Domain.Models.Posts.Commands;
using Atles.Domain.Models.Posts.Events;
using Atles.Infrastructure.Commands;

namespace Atles.Domain.Handlers.Posts.Commands
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

        public async Task Handle(DeleteReply command)
        {
            var reply = await _dbContext.Posts
                .Include(x => x.CreatedByUser)
                .Include(x => x.Topic).ThenInclude(x => x.Forum).ThenInclude(x => x.Category)
                .Include(x => x.Topic).ThenInclude(x => x.Forum).ThenInclude(x => x.LastPost)
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id &&
                    x.TopicId == command.TopicId &&
                    x.Topic.ForumId == command.ForumId &&
                    x.Topic.Forum.Category.SiteId == command.SiteId &&
                    x.Status != PostStatusType.Deleted);

            if (reply == null)
            {
                throw new DataException($"Reply with Id {command.Id} not found.");
            }

            reply.Delete();

            var @event = new ReplyDeleted
            {
                TargetId = command.Id,
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
        }
    }
}
