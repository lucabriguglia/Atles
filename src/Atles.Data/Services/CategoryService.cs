using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Atles.Data.Caching;
using Atles.Domain;
using Atles.Domain.Categories;
using Atles.Domain.Categories.Commands;
using Atles.Domain.Forums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Atles.Data.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IValidator<CreateCategory> _createValidator;
        private readonly IValidator<UpdateCategory> _updateValidator;

        public CategoryService(AtlesDbContext dbContext,
            ICacheManager cacheManager,
            IValidator<CreateCategory> createValidator,
            IValidator<UpdateCategory> updateValidator)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task MoveAsync(MoveCategory command)
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

            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Reordered,
                typeof(Category),
                category.Id,
                new
                {
                    category.SortOrder
                }));

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

            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Reordered,
                typeof(Category),
                adjacentCategory.Id,
                new
                {
                    adjacentCategory.SortOrder
                }));

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Categories(command.SiteId));
        }

        public async Task DeleteAsync(DeleteCategory command)
        {
            var category = await _dbContext.Categories
                .Include(x => x.Forums)
                .FirstOrDefaultAsync(x => 
                    x.SiteId == command.SiteId && 
                    x.Id == command.Id &&
                    x.Status != CategoryStatusType.Deleted);

            if (category == null)
            {
                throw new DataException($"Category with Id {command.Id} not found.");
            }

            category.Delete();
            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Deleted,
                typeof(Category),
                category.Id));

            var otherCategories = await _dbContext.Categories
                .Where(x =>
                    x.SiteId == command.SiteId &&
                    x.Id != command.Id &&
                    x.Status != CategoryStatusType.Deleted)
                .ToListAsync();

            for (int i = 0; i < otherCategories.Count; i++)
            {
                otherCategories[i].Reorder(i + 1);
                _dbContext.Events.Add(new Event(command.SiteId,
                    command.UserId,
                    EventType.Reordered,
                    typeof(Category),
                    otherCategories[i].Id,
                    new
                    {
                        otherCategories[i].SortOrder
                    }));
            }

            foreach (var forum in category.Forums)
            {
                forum.Delete();
                _dbContext.Events.Add(new Event(command.SiteId,
                    command.UserId,
                    EventType.Deleted,
                    typeof(Forum),
                    forum.Id));
            }

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Categories(command.SiteId));
            _cacheManager.Remove(CacheKeys.CurrentForums(command.SiteId));
        }
    }
}
