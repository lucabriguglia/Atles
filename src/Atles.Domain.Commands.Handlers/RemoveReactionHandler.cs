using System.Data;
using Atles.Data;
using Atles.Domain.Events;
using Atles.Domain.Models;
using Atles.Infrastructure.Commands;
using Atles.Infrastructure.Events;
using Microsoft.EntityFrameworkCore;

namespace Atles.Domain.Commands.Handlers
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
            var postReaction = await _dbContext.PostReactions
                .Include(x => x.Post).ThenInclude(x => x.PostReactionSummaries)
                .FirstOrDefaultAsync(x =>
                    x.PostId == command.Id &&
                    x.UserId == command.UserId &&
                    x.Post.ForumId == command.ForumId &&
                    x.Post.Forum.Category.SiteId == command.SiteId &&
                    x.Post.Status != PostStatusType.Deleted);

            if (postReaction == null)
            {
                throw new DataException($"Post reaction for post id {command.Id} and user id {command.UserId} not found.");
            }

            postReaction.Post.RemoveReactionFromSummary(postReaction.Type);

            _dbContext.PostReactions.Remove(postReaction);

            var @event = new ReactionRemoved
            {
                Type = postReaction.Type,
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
