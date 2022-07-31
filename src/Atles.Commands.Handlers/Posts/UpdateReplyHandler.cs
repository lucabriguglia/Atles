using System.Data;
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

        public async Task<CommandResult> Handle(UpdateReply command)
        {
            await _validator.ValidateCommand(command);

            var reply = await _dbContext.Posts
                .FirstOrDefaultAsync(x =>
                    x.Id == command.ReplyId &&
                    x.TopicId == command.TopicId &&
                    x.Topic.ForumId == command.ForumId &&
                    x.Topic.Forum.Category.SiteId == command.SiteId &&
                    x.Status != PostStatusType.Deleted);

            if (reply == null)
            {
                throw new DataException($"Reply with Id {command.ReplyId} not found.");
            }

            reply.UpdateDetails(command.UserId, command.Content, command.Status);

            var @event = new ReplyUpdated
            {
                Content = reply.Content,
                Status = reply.Status,
                TargetId = command.ReplyId,
                TargetType = nameof(Post),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(@event.ToDbEntity());

            await _dbContext.SaveChangesAsync();

            return new Success(new IEvent[] { @event });
        }
    }
}
