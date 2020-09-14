using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Atles.Data.Caching;
using Atles.Domain;
using Atles.Domain.Posts;
using Atles.Domain.Posts.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Post = Atles.Domain.Posts.Post;

namespace Atles.Data.Services
{
    public class TopicService : ITopicService
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IValidator<CreateTopic> _createValidator;
        private readonly IValidator<UpdateTopic> _updateValidator;

        public TopicService(AtlesDbContext dbContext,
            ICacheManager cacheManager,
            IValidator<CreateTopic> createValidator,
            IValidator<UpdateTopic> updateValidator)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<string> CreateAsync(CreateTopic command)
        {
            await _createValidator.ValidateCommandAsync(command);

            var title = Regex.Replace(command.Title, @"\s+", " "); // Remove multiple spaces from title

            var slug = string.IsNullOrWhiteSpace(command.Slug)
                ? await GenerateSlugAsync(command.ForumId, title)
                : command.Slug;

            var topic = Post.CreateTopic(command.Id,
                command.ForumId,
                command.UserId,
                title,
                slug,
                command.Content,
                command.Status);

            _dbContext.Posts.Add(topic);
            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Created,
                typeof(Post),
                command.Id,
                new 
                {
                    command.ForumId,
                    title,
                    Slug = slug,
                    command.Content,
                    command.Status
                }));

            var forum = await _dbContext.Forums.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == topic.ForumId);
            forum.UpdateLastPost(topic.Id);
            forum.IncreaseTopicsCount();
            forum.Category.IncreaseTopicsCount();

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == topic.CreatedBy);
            user.IncreaseTopicsCount();

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Forum(forum.Id));

            return slug;
        }

        public async Task<string> UpdateAsync(UpdateTopic command)
        {
            await _updateValidator.ValidateCommandAsync(command);

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

            var title = Regex.Replace(command.Title, @"\s+", " "); // Remove multiple spaces from title

            var slug = string.IsNullOrWhiteSpace(command.Slug)
                ? await GenerateSlugAsync(command.ForumId, title)
                : command.Slug;

            topic.UpdateDetails(command.UserId, title, slug, command.Content, command.Status);

            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Updated,
                typeof(Post),
                command.Id,
                new
                {
                    title,
                    Slug = slug,
                    command.Content,
                    command.Status
                }));

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Forum(topic.ForumId));

            return slug;
        }

        public async Task<string> GenerateSlugAsync(Guid forumId, string title)
        {
            var slug = string.Empty;
            var exists = true;
            var repeat = 0;

            while (exists && repeat < 5)
            {
                var suffix = repeat > 0 ? $"-{repeat}" : string.Empty;
                slug = $"{title.ToSlug()}{suffix}";
                exists = await _dbContext.Posts.AnyAsync(x => 
                    x.ForumId == forumId && 
                    x.Slug == slug && 
                    x.Status == PostStatusType.Published);
                repeat++;
            }

            if (exists)
            {
                slug = Guid.NewGuid().ToString();
            }

            return slug;
        }

        public async Task PinAsync(PinTopic command)
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

        public async Task LockAsync(LockTopic command)
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

            topic.Lock(command.Locked);

            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Locked,
                typeof(Post),
                command.Id));

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(DeleteTopic command)
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
            topic.Forum.Category.DecreaseTopicsCount();
            topic.CreatedByUser.DecreaseTopicsCount();

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