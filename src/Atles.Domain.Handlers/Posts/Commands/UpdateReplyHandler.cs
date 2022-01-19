using Atles.Data;
using Atles.Data.Caching;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading.Tasks;
using Atles.Domain.Models;
using Atles.Domain.Models.Posts;
using Atles.Domain.Models.Posts.Commands;
using Atles.Infrastructure.Commands;

namespace Atles.Domain.Handlers.Posts.Commands
{
    public class UpdateReplyHandler : ICommandHandler<UpdateReply>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly IValidator<UpdateReply> _validator;
        private readonly ICacheManager _cacheManager;

        public UpdateReplyHandler(AtlesDbContext dbContext, IValidator<UpdateReply> validator, ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _validator = validator;
            _cacheManager = cacheManager;
        }

        public async Task Handle(UpdateReply command)
        {
            await _validator.ValidateCommandAsync(command);

            var reply = await _dbContext.Posts
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id &&
                    x.TopicId == command.TopicId &&
                    x.Topic.ForumId == command.ForumId &&
                    x.Topic.Forum.Category.SiteId == command.SiteId &&
                    x.Status != PostStatusType.Deleted);

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
    }
}
