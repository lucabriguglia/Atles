using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain.Posts;
using Atles.Domain.Posts.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Commands;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task Handle(DeleteTopic command)
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

            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Deleted,
                typeof(Post),
                command.Id));

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
        }
    }
}
