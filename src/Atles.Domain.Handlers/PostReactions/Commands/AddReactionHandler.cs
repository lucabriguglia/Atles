using Atles.Data;
using Atles.Domain.PostReactions.Commands;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading.Tasks;
using Atles.Domain.Models;
using Atles.Domain.Models.PostReactions;
using Atles.Domain.Models.PostReactions.Events;
using Atles.Domain.Models.Posts;
using Atles.Infrastructure.Commands;

namespace Atles.Domain.Handlers.PostReactions.Commands
{
    public class AddReactionHandler : ICommandHandler<AddReaction>
    {
        private readonly AtlesDbContext _dbContext;

        public AddReactionHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(AddReaction command)
        {
            var post = await _dbContext.Posts
                .Include(x => x.PostReactionCounts)
                .FirstOrDefaultAsync(x =>
                    x.Forum.Category.SiteId == command.SiteId &&
                    x.Id == command.PostId &&
                    x.Status != PostStatusType.Deleted);

            if (post == null)
            {
                throw new DataException($"Post with Id {command.Id} not found.");
            }

            post.IncreaseReactionCount(command.Type);

            var postReaction = new PostReaction(command.PostId, command.UserId, command.Type);

            _dbContext.PostReactions.Add(postReaction);

            var @event = new ReactionAdded
            {
                Type = postReaction.Type,
                TargetId = postReaction.PostId,
                TargetType = nameof(Post),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(@event.ToDbEntity());

            await _dbContext.SaveChangesAsync();
        }
    }
}
