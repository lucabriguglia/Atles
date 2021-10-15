﻿using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain.Posts;
using Atles.Domain.Posts.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Commands;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Atles.Domain.Handlers.Posts.Commands
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

        public async Task Handle(UpdateTopic command)
        {
            await _validator.ValidateCommandAsync(command);

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

            var slug = title != topic.Title ? await _topicSlugGenerator.GenerateTopicSlug(topic.ForumId, title) : topic.Slug;

            topic.UpdateDetails(command.UserId, title, slug, command.Content, command.Status);

            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Updated,
                typeof(Post),
                command.Id,
                new
                {
                    title,
                    slug,
                    command.Content,
                    command.Status
                }));

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Forum(topic.ForumId));
        }
    }
}
