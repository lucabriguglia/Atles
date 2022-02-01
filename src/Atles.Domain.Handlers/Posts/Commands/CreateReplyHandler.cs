using System.Collections.Generic;
using Atles.Data;
using Atles.Data.Caching;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Atles.Domain.Commands;
using Atles.Domain.Events;
using Atles.Domain.Models;
using Atles.Infrastructure.Commands;
using Atles.Infrastructure.Events;

namespace Atles.Domain.Handlers.Posts.Commands
{
    public class CreateReplyHandler : ICommandHandler<CreateReply>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly IValidator<CreateReply> _validator;
        private readonly ICacheManager _cacheManager;

        public CreateReplyHandler(AtlesDbContext dbContext, IValidator<CreateReply> validator, ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _validator = validator;
            _cacheManager = cacheManager;
        }

        public async Task<IEnumerable<IEvent>> Handle(CreateReply command)
        {
            await _validator.ValidateCommand(command);

            var reply = Post.CreateReply(command.Id,
                command.TopicId,
                command.ForumId,
                command.UserId,
                command.Content,
                command.Status);

            _dbContext.Posts.Add(reply);

            var @event = new ReplyCreated
            {
                TopicId = command.TopicId,
                ForumId = command.ForumId,
                Content = command.Content,
                Status = command.Status,
                TargetId = command.Id,
                TargetType = nameof(Post),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(@event.ToDbEntity());

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

            return new IEvent[] { @event };
        }
    }
}
