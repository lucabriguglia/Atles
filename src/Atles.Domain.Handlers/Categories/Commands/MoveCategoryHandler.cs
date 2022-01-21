using Atles.Data;
using Atles.Data.Caching;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading.Tasks;
using Atles.Domain.Models;
using Atles.Domain.Models.Categories;
using Atles.Domain.Models.Categories.Commands;
using Atles.Domain.Models.Categories.Events;
using Atles.Infrastructure.Commands;

namespace Atles.Domain.Handlers.Categories.Commands
{
    public class MoveCategoryHandler : ICommandHandler<MoveCategory>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public MoveCategoryHandler(AtlesDbContext dbContext, ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }

        public async Task Handle(MoveCategory command)
        {
            var category = await _dbContext.Categories
                .FirstOrDefaultAsync(x =>
                    x.SiteId == command.SiteId &&
                    x.Id == command.Id &&
                    x.Status != CategoryStatusType.Deleted);

            if (category == null)
            {
                throw new DataException($"Category with Id {command.Id} not found.");
            }

            if (command.Direction == Direction.Up)
            {
                category.MoveUp();
            }
            else if (command.Direction == Direction.Down)
            {
                category.MoveDown();
            }

            var categoryMovedEvent = new CategoryMoved
            {
                SortOrder = category.SortOrder,
                TargetId = category.Id,
                TargetType = nameof(Category),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(categoryMovedEvent.ToDbEntity());

            var sortOrderToReplace = category.SortOrder;

            var adjacentCategory = await _dbContext.Categories
                .FirstOrDefaultAsync(x =>
                    x.SiteId == command.SiteId &&
                    x.SortOrder == sortOrderToReplace &&
                    x.Status != CategoryStatusType.Deleted);

            if (command.Direction == Direction.Up)
            {
                adjacentCategory.MoveDown();
            }
            else if (command.Direction == Direction.Down)
            {
                adjacentCategory.MoveUp();
            }

            var adjacentCategoryMovedEvent = new CategoryMoved
            {
                SortOrder = category.SortOrder,
                TargetId = category.Id,
                TargetType = nameof(Category),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(adjacentCategoryMovedEvent.ToDbEntity());

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Categories(command.SiteId));
        }
    }
}
