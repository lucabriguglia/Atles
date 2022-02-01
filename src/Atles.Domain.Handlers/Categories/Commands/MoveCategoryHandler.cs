using System.Collections.Generic;
using Atles.Data;
using Atles.Data.Caching;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading.Tasks;
using Atles.Domain.Commands;
using Atles.Domain.Events;
using Atles.Domain.Models;
using Atles.Infrastructure.Commands;
using Atles.Infrastructure.Events;

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

        public async Task<IEnumerable<IEvent>> Handle(MoveCategory command)
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

            var categoryMoved = new CategoryMoved
            {
                SortOrder = category.SortOrder,
                TargetId = category.Id,
                TargetType = nameof(Category),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(categoryMoved.ToDbEntity());

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

            var adjacentCategoryMoved = new CategoryMoved
            {
                SortOrder = category.SortOrder,
                TargetId = adjacentCategory.Id,
                TargetType = nameof(Category),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(adjacentCategoryMoved.ToDbEntity());

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Categories(command.SiteId));

            return new IEvent[] { categoryMoved, adjacentCategoryMoved };
        }
    }
}
