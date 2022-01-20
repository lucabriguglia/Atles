using Atles.Data;
using Atles.Data.Caching;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Atles.Domain.Models;
using Atles.Domain.Models.Posts;
using Atles.Domain.Models.Posts.Commands;
using Atles.Infrastructure.Commands;

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

        public async Task Handle(CreateReply command)
        {
            await _validator.ValidateCommandAsync(command);

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
    }
}
