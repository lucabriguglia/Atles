using Atles.Data;
using Atles.Data.Caching;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Atles.Domain.Handlers.Posts.Services;
using Atles.Domain.Models;
using Atles.Domain.Models.Posts;
using Atles.Domain.Models.Posts.Commands;
using Atles.Domain.Models.Posts.Events;
using Atles.Infrastructure.Commands;

namespace Atles.Domain.Handlers.Posts.Commands
{
    public class CreateTopicHandler : ICommandHandler<CreateTopic>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly IValidator<CreateTopic> _validator;
        private readonly ICacheManager _cacheManager;
        private readonly ITopicSlugGenerator _topicSlugGenerator;

        public CreateTopicHandler(AtlesDbContext dbContext,
                                  IValidator<CreateTopic> validator,
                                  ICacheManager cacheManager,
                                  ITopicSlugGenerator topicSlugGenerator)
        {
            _dbContext = dbContext;
            _validator = validator;
            _cacheManager = cacheManager;
            _topicSlugGenerator = topicSlugGenerator;
        }

        public async Task Handle(CreateTopic command)
        {
            await _validator.ValidateCommand(command);

            var title = Regex.Replace(command.Title, @"\s+", " "); // Remove multiple spaces from title

            var slug = await _topicSlugGenerator.GenerateTopicSlug(command.ForumId, title);

            var topic = Post.CreateTopic(command.Id,
                command.ForumId,
                command.UserId,
                title,
                slug,
                command.Content,
                command.Status);

            _dbContext.Posts.Add(topic);

            var @event = new TopicCreated
            {
                ForumId = command.ForumId,
                Title = title,
                Slug = slug,
                Content = command.Content,
                Status = command.Status,
                TargetId = command.Id,
                TargetType = nameof(Post),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(@event.ToDbEntity());

            var forum = await _dbContext.Forums.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == topic.ForumId);
            forum.UpdateLastPost(topic.Id);
            forum.IncreaseTopicsCount();
            forum.Category.IncreaseTopicsCount();

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == topic.CreatedBy);
            user.IncreaseTopicsCount();

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Forum(forum.Id));
        }
    }
}
