using System.Data;
using Atles.Commands.Categories;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Core.Results;
using Atles.Core.Results.Types;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain;
using Atles.Events.Categories;
using Microsoft.EntityFrameworkCore;

namespace Atles.Commands.Handlers.Categories
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

        public async Task<CommandResult> Handle(MoveCategory command)
        {
            var category = await _dbContext.Categories
                .FirstOrDefaultAsync(x =>
                    x.SiteId == command.SiteId &&
                    x.Id == command.CategoryId &&
                    x.Status != CategoryStatusType.Deleted);

            if (category == null)
            {
                throw new DataException($"Category with Id {command.CategoryId} not found.");
            }

            if (command.Direction == DirectionType.Up)
            {
                category.MoveUp();
            }
            else if (command.Direction == DirectionType.Down)
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

            if (command.Direction == DirectionType.Up)
            {
                adjacentCategory.MoveDown();
            }
            else if (command.Direction == DirectionType.Down)
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

            return new Success(new IEvent[] { categoryMoved, adjacentCategoryMoved  });
        }
    }
}
