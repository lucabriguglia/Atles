using System.Data;
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
    public class UpdateTopicHandler : ICommandHandler<UpdateTopic>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly IValidator<UpdateTopic> _validator;
        private readonly ICacheManager _cacheManager;
        private readonly ITopicSlugGenerator _topicSlugGenerator;

        public UpdateTopicHandler(AtlesDbContext dbContext,
                                  IValidator<UpdateTopic> validator,
                                  ICacheManager cacheManager,
                                  ITopicSlugGenerator topicSlugGenerator)
        {
            _dbContext = dbContext;
            _validator = validator;
            _cacheManager = cacheManager;
            _topicSlugGenerator = topicSlugGenerator;
        }

        public async Task<CommandResult> Handle(UpdateTopic command)
        {
            await _validator.ValidateCommand(command);

            var topic = await _dbContext.Posts
                .FirstOrDefaultAsync(x =>
                    x.Id == command.TopicId &&
                    x.TopicId == null &&
                    x.ForumId == command.ForumId &&
                    x.Forum.Category.SiteId == command.SiteId &&
                    x.Status != PostStatusType.Deleted);

            if (topic == null)
            {
                throw new DataException($"Topic with Id {command.TopicId} not found.");
            }

            var title = Regex.Replace(command.Title, @"\s+", " "); // Remove multiple spaces from title
            var slug = title != topic.Title ? await _topicSlugGenerator.GenerateTopicSlug(topic.ForumId, title) : topic.Slug;
            topic.UpdateDetails(command.UserId, title, slug, command.Content, command.Status);

            var subscription = await _dbContext.Subscriptions.FirstOrDefaultAsync(x => x.UserId == command.UserId && x.ItemId == command.TopicId);
            switch (command.Subscribe)
            {
                case true when subscription == null:
                    _dbContext.Subscriptions.Add(new Subscription(command.UserId, SubscriptionType.Topic, command.TopicId));
                    break;
                case false when subscription != null:
                    _dbContext.Subscriptions.Remove(subscription);
                    break;
            }

            var @event = new TopicUpdated
            {
                Title = topic.Title,
                Slug = topic.Slug,
                Content = topic.Content,
                Status = topic.Status,
                TargetId = topic.Id,
                TargetType = nameof(Post),
                SiteId = command.SiteId,
                UserId = command.UserId
            };
            _dbContext.Events.Add(@event.ToDbEntity());

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Forum(topic.ForumId));

            return new Success(new IEvent[] { @event });
        }
    }
}
