using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Domain.Models;
using Atles.Domain.Models.Posts;
using Atles.Domain.Models.Posts.Commands;
using Atles.Domain.Models.Posts.Events;
using Atles.Infrastructure.Commands;
using Atles.Infrastructure.Events;
using Microsoft.EntityFrameworkCore;

namespace Atles.Domain.Handlers.Posts.Commands
{
    public class RemoveReactionHandler : ICommandHandler<RemoveReaction>
    {
        private readonly AtlesDbContext _dbContext;

        public RemoveReactionHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<IEvent>> Handle(RemoveReaction command)
        {
            var post = await _dbContext.Posts
                .Include(x => x.PostReactions)
                .FirstOrDefaultAsync(x =>
                    x.Forum.Category.SiteId == command.SiteId &&
                    x.Id == command.Id &&
                    x.Status != PostStatusType.Deleted);

            if (post == null)
            {
                throw new DataException($"Post with Id {command.Id} not found.");
            }

            post.RemoveReaction(command.Type);

            var @event = new ReactionRemoved
            {
                Type = command.Type,
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
