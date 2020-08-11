using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Data.Caching;
using Atlas.Domain;
using Atlas.Domain.Posts;
using Atlas.Domain.Posts.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Post = Atlas.Domain.Posts.Post;

namespace Atlas.Data.Services
{
    public class TopicService : ITopicService
    {
        private readonly AtlasDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IValidator<CreateTopic> _createValidator;
        private readonly IValidator<UpdateTopic> _updateValidator;

        public TopicService(AtlasDbContext dbContext,
            ICacheManager cacheManager,
            IValidator<CreateTopic> createValidator,
            IValidator<UpdateTopic> updateValidator)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task CreateAsync(CreateTopic command)
        {
            await _createValidator.ValidateCommandAsync(command);

            var topic = Post.CreateTopic(command.Id,
                command.ForumId,
                command.MemberId,
                command.Title,
                command.Content,
                command.Status);

            _dbContext.Posts.Add(topic);
            _dbContext.Events.Add(new Event(command.SiteId,
                command.MemberId,
                EventType.Created,
                typeof(Post),
                command.Id,
                new 
                {
                    command.ForumId,
                    command.Title,
                    command.Content,
                    command.Status
                }));

            var forum = await _dbContext.Forums.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == topic.ForumId);
            forum.UpdateLastPost(topic.Id);
            forum.IncreaseTopicsCount();
            forum.Category.IncreaseTopicsCount();

            var member = await _dbContext.Members.FirstOrDefaultAsync(x => x.Id == topic.MemberId);
            member.IncreaseTopicsCount();

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Forum(forum.Id));
        }

        public async Task UpdateAsync(UpdateTopic command)
        {
            await _updateValidator.ValidateCommandAsync(command);

            var topic = await _dbContext.Posts
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id &&
                    x.TopicId == null &&
                    x.ForumId == command.ForumId &&
                    x.Forum.Category.SiteId == command.SiteId &&
                    x.Status != StatusType.Deleted);

            if (topic == null)
            {
                throw new DataException($"Topic with Id {command.Id} not found.");
            }

            topic.UpdateDetails(command.Title, command.Content, command.Status);
            _dbContext.Events.Add(new Event(command.SiteId,
                command.MemberId,
                EventType.Updated,
                typeof(Post),
                command.Id,
                new
                {
                    command.Title,
                    command.Content,
                    command.Status
                }));

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Forum(topic.ForumId));
        }

        public async Task DeleteAsync(DeleteTopic command)
        {
            var topic = await _dbContext.Posts
                .Include(x => x.Member)
                .Include(x => x.Forum).ThenInclude(x => x.Category)
                .Include(x => x.Forum).ThenInclude(x => x.LastPost)
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id &&
                    x.TopicId == null &&
                    x.ForumId == command.ForumId &&
                    x.Forum.Category.SiteId == command.SiteId &&
                    x.Status != StatusType.Deleted);

            if (topic == null)
            {
                throw new DataException($"Topic with Id {command.Id} not found.");
            }

            topic.Delete();
            _dbContext.Events.Add(new Event(command.SiteId,
                command.MemberId,
                EventType.Deleted,
                typeof(Post),
                command.Id));

            topic.Forum.DecreaseTopicsCount();
            topic.Forum.Category.DecreaseTopicsCount();
            topic.Member.DecreaseTopicsCount();

            if (topic.Forum.LastPost != null && (topic.Id == topic.Forum.LastPostId || topic.Id == topic.Forum.LastPost.TopicId))
            {
                var newLastPost = await _dbContext.Posts
                    .Where(x => x.ForumId == topic.ForumId && 
                                x.Status == StatusType.Published &&
                                (x.Topic == null || x.Topic.Status == StatusType.Published) &&
                                x.Id != topic.Id)
                    .OrderByDescending(x => x.TimeStamp)
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