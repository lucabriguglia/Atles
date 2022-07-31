using System.Text.RegularExpressions;
using Atles.Commands.Handlers.Posts.Services;
using Atles.Commands.Posts;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Core.Results;
using Atles.Core.Results.Types;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain;
using Atles.Events.Posts;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Atles.Commands.Handlers.Posts
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

        public async Task<CommandResult> Handle(CreateTopic command)
        {
            await _validator.ValidateCommand(command);

            var title = Regex.Replace(command.Title, @"\s+", " "); // Remove multiple spaces from title

            var slug = await _topicSlugGenerator.GenerateTopicSlug(command.ForumId, title);

            var topic = Post.CreateTopic(command.TopicId,
                command.ForumId,
                command.UserId,
                title,
                slug,
                command.Content,
                command.Status);

            _dbContext.Posts.Add(topic);

            var forum = await _dbContext.Forums.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == topic.ForumId);
            forum.UpdateLastPost(topic.Id);
            forum.IncreaseTopicsCount();
            forum.Category.IncreaseTopicsCount();

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == topic.CreatedBy);
            user.IncreaseTopicsCount();

            if (command.Subscribe)
            {
                var subscription = new Subscription(command.UserId, SubscriptionType.Topic, command.TopicId);
                _dbContext.Subscriptions.Add(subscription);
            }

            var @event = new TopicCreated
            {
                ForumId = command.ForumId,
                Title = title,
                Slug = slug,
                Content = command.Content,
                Status = command.Status,
                TargetId = command.TopicId,
                TargetType = nameof(Post),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(@event.ToDbEntity());

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Forum(forum.Id));

            return new Success(new IEvent[] { @event });
        }
    }
}
