using Atles.Data;
using Atles.Domain.PostLikes;
using Atles.Domain.PostLikes.Commands;
using Atles.Domain.Posts;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Commands;
using System.Data;
using System.Threading.Tasks;

namespace Atles.Domain.Handlers.PostLikes.Commands
{
    public class AddLikeHandler : ICommandHandler<AddLike>
    {
        private readonly AtlesDbContext _dbContext;

        public AddLikeHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(AddLike command)
        {
            var post = await _dbContext.Posts
                .FirstOrDefaultAsync(x =>
                    x.Forum.Category.SiteId == command.SiteId &&
                    x.Id == command.PostId &&
                    x.Status != PostStatusType.Deleted);

            if (post == null)
            {
                throw new DataException($"Post with Id {command.Id} not found.");
            }

            if (command.Like)
            {
                post.IncreaseLikesCount();
            }
            else
            {
                post.IncreaseDislikesCount();
            }

            var postLike = new PostLike(command.PostId, command.UserId, command.Like);

            _dbContext.PostLikes.Add(postLike);

            await _dbContext.SaveChangesAsync();
        }
    }
}
