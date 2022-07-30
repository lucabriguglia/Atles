using System.Data;
using Atles.Commands.Posts;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Data;
using Atles.Domain.Events.Posts;
using Atles.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Atles.Domain.Commands.Handlers.Posts
{
    public class RemovePostReactionHandler : ICommandHandler<RemovePostReaction>
    {
        private readonly AtlesDbContext _dbContext;

        public RemovePostReactionHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<IEvent>> Handle(RemovePostReaction command)
        {
            var postReaction = await _dbContext.PostReactions
                .Include(x => x.Post).ThenInclude(x => x.PostReactionSummaries)
                .FirstOrDefaultAsync(x =>
                    x.PostId == command.PostId &&
                    x.UserId == command.UserId &&
                    x.Post.ForumId == command.ForumId &&
                    x.Post.Forum.Category.SiteId == command.SiteId &&
                    x.Post.Status != PostStatusType.Deleted);

            if (postReaction == null)
            {
                throw new DataException($"Post reaction for post id {command.PostId} and user id {command.UserId} not found.");
            }

            postReaction.Post.RemoveReactionFromSummary(postReaction.Type);

            _dbContext.PostReactions.Remove(postReaction);

            var @event = new PostReactionRemoved
            {
                Type = postReaction.Type,
                TargetId = command.PostId,
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
