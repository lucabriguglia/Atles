using Atles.Data;
using Atles.Domain.Models.Posts;
using Atles.Domain.PostReactions.Commands;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Commands;
using System.Data;
using System.Threading.Tasks;

namespace Atles.Domain.Handlers.PostReactions.Commands
{
    public class RemoveReactionHandler : ICommandHandler<RemoveReaction>
    {
        private readonly AtlesDbContext _dbContext;

        public RemoveReactionHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(RemoveReaction command)
        {
            var postReaction = await _dbContext.PostReactions
                .Include(x => x.Post).ThenInclude(x => x.PostReactionCounts)
                .FirstOrDefaultAsync(x =>
                    x.PostId == command.PostId &&
                    x.UserId == command.UserId &&
                    x.Post.Forum.Category.SiteId == command.SiteId &&
                    x.Post.Status != PostStatusType.Deleted);

            if (postReaction == null)
            {
                throw new DataException($"Post reaction for post id {command.PostId} and user id {command.UserId} not found.");
            }

            postReaction.Post.DecreaseReactionCount(postReaction.Type);

            _dbContext.PostReactions.Remove(postReaction);

            await _dbContext.SaveChangesAsync();
        }
    }
}
