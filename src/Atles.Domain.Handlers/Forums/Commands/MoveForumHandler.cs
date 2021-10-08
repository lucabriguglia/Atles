using System.Data;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain.Forums;
using Atles.Domain.Forums.Commands;
using Atles.Infrastructure.Commands;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Commands;

namespace Atles.Domain.Handlers.Forums.Commands
{
    public class MoveForumHandler : ICommandHandler<MoveForum>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public MoveForumHandler(AtlesDbContext dbContext,
            ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }

        public async Task Handle(MoveForum command)
        {
            var forum = await _dbContext.Forums
                .FirstOrDefaultAsync(x =>
                    x.Category.SiteId == command.SiteId &&
                    x.Id == command.Id &&
                    x.Status != ForumStatusType.Deleted);

            if (forum == null)
            {
                throw new DataException($"Forum with Id {command.Id} not found.");
            }

            if (command.Direction == Direction.Up)
            {
                forum.MoveUp();
            }
            else if (command.Direction == Direction.Down)
            {
                forum.MoveDown();
            }

            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Reordered,
                typeof(Forum),
                forum.Id,
                new
                {
                    forum.SortOrder
                }));

            var sortOrderToReplace = forum.SortOrder;

            var adjacentForum = await _dbContext.Forums
                .FirstOrDefaultAsync(x =>
                    x.CategoryId == forum.CategoryId &&
                    x.SortOrder == sortOrderToReplace &&
                    x.Status != ForumStatusType.Deleted);

            if (command.Direction == Direction.Up)
            {
                adjacentForum.MoveDown();
            }
            else if (command.Direction == Direction.Down)
            {
                adjacentForum.MoveUp();
            }

            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Reordered,
                typeof(Forum),
                adjacentForum.Id,
                new
                {
                    adjacentForum.SortOrder
                }));

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Categories(command.SiteId));
        }
    }
}
