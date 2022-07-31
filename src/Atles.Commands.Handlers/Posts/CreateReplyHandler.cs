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

        public async Task<CommandResult> Handle(CreateReply command)
        {
            await _validator.ValidateCommand(command);

            var reply = Post.CreateReply(command.ReplyId,
                command.TopicId,
                command.ForumId,
                command.UserId,
                command.Content,
                command.Status);

            _dbContext.Posts.Add(reply);

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

            var @event = new ReplyCreated
            {
                TopicId = command.TopicId,
                ForumId = command.ForumId,
                Content = command.Content,
                Status = command.Status,
                TargetId = command.ReplyId,
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
