using Atles.Data;
using Atles.Domain.PostReactions.Commands;
using Atles.Domain.Posts;
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
            var postLike = await _dbContext.PostLikes
                .FirstOrDefaultAsync(x =>
                    x.PostId == command.PostId &&
                    x.UserId == command.UserId);

            if (postLike == null)
            {
                throw new DataException($"Post like for post id {command.PostId} and user id {command.UserId} not found.");
            }

            var post = await _dbContext.Posts
                .FirstOrDefaultAsync(x =>
                    x.Forum.Category.SiteId == command.SiteId &&
                    x.Id == postLike.PostId &&
                    x.Status != PostStatusType.Deleted);

            if (post == null)
            {
                throw new DataException($"Post with Id {postLike.PostId} not found.");
            }

            if (postLike.Like)
            {
                post.DecreaseLikesCount();
            }
            else
            {
                post.DecreaseDislikesCount();
            }

            _dbContext.PostLikes.Remove(postLike);

            await _dbContext.SaveChangesAsync();
        }
    }
}
