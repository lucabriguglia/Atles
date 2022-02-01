using System.Collections.Generic;
using Atles.Data;
using Atles.Data.Caching;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading.Tasks;
using Atles.Domain.Commands;
using Atles.Domain.Events;
using Atles.Domain.Models;
using Atles.Infrastructure.Commands;
using Atles.Infrastructure.Events;

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

        public async Task<IEnumerable<IEvent>> Handle(UpdateReply command)
        {
            await _validator.ValidateCommand(command);

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

            var @event = new ReplyUpdated
            {
                Content = reply.Content,
                Status = reply.Status,
                TargetId = command.Id,
                TargetType = nameof(Post),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(@event.ToDbEntity());

            await _dbContext.SaveChangesAsync();

            return new IEvent[] { @event };
        }
    }
}
