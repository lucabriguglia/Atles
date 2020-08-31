using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Data.Caching;
using Atlas.Domain;
using Atlas.Domain.Posts;
using Atlas.Domain.Posts.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Data.Services
{
    public class ReplyService : IReplyService
    {
        private readonly AtlasDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IValidator<CreateReply> _createValidator;
        private readonly IValidator<UpdateReply> _updateValidator;

        public ReplyService(AtlasDbContext dbContext,
            ICacheManager cacheManager,
            IValidator<CreateReply> createValidator,
            IValidator<UpdateReply> updateValidator)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task CreateAsync(CreateReply command)
        {
            await _createValidator.ValidateCommandAsync(command);

            var reply = Post.CreateReply(command.Id,
                command.TopicId,
                command.ForumId,
                command.UserId,
                command.Content,
                command.Status);

            _dbContext.Posts.Add(reply);

            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Created,
                typeof(Post),
                command.Id,
                new 
                {
                    command.TopicId,
                    command.ForumId,
                    command.Content,
                    command.Status
                }));

            var topic = await _dbContext.Posts
                    .Include(x => x.Forum)
                        .ThenInclude(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == reply.TopicId);

            topic.UpdateLastReply(reply.Id);
            topic.IncreaseRepliesCount();
            topic.Forum.UpdateLastPost(reply.Id);
            topic.Forum.IncreaseRepliesCount();
            topic.Forum.Category.IncreaseRepliesCount();

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == reply.CreatedBy);
            user.IncreaseRepliesCount();

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Forum(topic.ForumId));
        }

        public async Task UpdateAsync(UpdateReply command)
        {
            await _updateValidator.ValidateCommandAsync(command);

            var reply = await _dbContext.Posts
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id &&
                    x.TopicId == command.TopicId &&
                    x.Topic.ForumId == command.ForumId &&
                    x.Topic.Forum.Category.SiteId == command.SiteId &&
                    x.Status != StatusType.Deleted);

            if (reply == null)
            {
                throw new DataException($"Reply with Id {command.Id} not found.");
            }

            reply.UpdateDetails(command.UserId, command.Content, command.Status);

            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Updated,
                typeof(Post),
                command.Id,
                new
                {
                    command.Content,
                    command.Status
                }));

            await _dbContext.SaveChangesAsync();
        }

        public async Task SetAsAnswerAsync(SetReplyAsAnswer command)
        {
            var reply = await _dbContext.Posts
                .Include(x => x.Topic)
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id &&
                    x.TopicId == command.TopicId &&
                    x.Topic.ForumId == command.ForumId &&
                    x.Topic.Forum.Category.SiteId == command.SiteId &&
                    x.Status == StatusType.Published);

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
        }

        public async Task DeleteAsync(DeleteReply command)
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
                    x.Status != StatusType.Deleted);

            if (reply == null)
            {
                throw new DataException($"Reply with Id {command.Id} not found.");
            }

            reply.Delete();

            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Deleted,
                typeof(Post),
                command.Id));

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
                                x.Status == StatusType.Published &&
                                (x.Topic == null || x.Topic.Status == StatusType.Published) &&
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