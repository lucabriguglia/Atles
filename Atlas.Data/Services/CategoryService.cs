using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Data.Caching;
using Atlas.Domain;
using Atlas.Domain.Categories;
using Atlas.Domain.Categories.Commands;
using Atlas.Domain.Categories.Events;
using Atlas.Domain.Forums;
using Atlas.Domain.Forums.Events;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Data.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AtlasDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IValidator<CreateCategory> _createValidator;
        private readonly IValidator<UpdateCategory> _updateValidator;

        public CategoryService(AtlasDbContext dbContext,
            ICacheManager cacheManager,
            IValidator<CreateCategory> createValidator,
            IValidator<UpdateCategory> updateValidator)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task CreateAsync(CreateCategory command)
        {
            await _createValidator.ValidateCommandAsync(command);

            var categoriesCount = await _dbContext.Categories
                .Where(x => x.SiteId == command.SiteId && x.Status != StatusType.Deleted)
                .CountAsync();

            var sortOrder = categoriesCount + 1;

            var category = new Category(command.Id,
                command.SiteId,
                command.Name,
                sortOrder,
                command.PermissionSetId);

            _dbContext.Categories.Add(category);
            _dbContext.Events.Add(new Event(new CategoryCreated
            {
                SiteId = command.SiteId,
                MemberId = command.MemberId,
                TargetId = category.Id,
                TargetType = typeof(Category).Name,
                Name = category.Name,
                PermissionSetId = category.PermissionSetId,
                SortOrder = category.SortOrder
            }));

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Categories(command.SiteId));
        }

        public async Task UpdateAsync(UpdateCategory command)
        {
            await _updateValidator.ValidateCommandAsync(command);

            var category = await _dbContext.Categories.FirstOrDefaultAsync(x => x.SiteId == command.SiteId && x.Id == command.Id && x.Status != StatusType.Deleted);

            if (category == null)
            {
                throw new DataException($"Category with Id {command.Id} not found.");
            }

            category.UpdateDetails(command.Name, command.PermissionSetId);
            _dbContext.Events.Add(new Event(new CategoryUpdated
            {
                SiteId = command.SiteId,
                MemberId = command.MemberId,
                TargetId = category.Id,
                TargetType = typeof(Category).Name,
                Name = category.Name,
                PermissionSetId = category.PermissionSetId
            }));

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Categories(command.SiteId));
        }

        public async Task MoveAsync(MoveCategory command)
        {
            var category = await _dbContext.Categories
                .FirstOrDefaultAsync(x => 
                    x.SiteId == command.SiteId && 
                    x.Id == command.Id && 
                    x.Status != StatusType.Deleted);

            if (category == null)
            {
                throw new DataException($"Category with Id {command.Id} not found.");
            }

            var currentSortOrder = category.SortOrder;

            if (command.Direction == Direction.Up)
            {
                category.MoveUp();
            }
            else if (command.Direction == Direction.Down)
            {
                category.MoveDown();
            }

            _dbContext.Events.Add(new Event(new CategoryReordered
            {
                SiteId = command.SiteId,
                MemberId = command.MemberId,
                TargetId = category.Id,
                TargetType = typeof(Category).Name,
                SortOrder = category.SortOrder
            }));

            var sortOrderToReplace = category.SortOrder;

            var adjacentCategory = await _dbContext.Categories
                .FirstOrDefaultAsync(x => 
                    x.SiteId == command.SiteId && 
                    x.SortOrder == sortOrderToReplace && 
                    x.Status != StatusType.Deleted);

            if (command.Direction == Direction.Up)
            {
                adjacentCategory.MoveDown();
            }
            else if (command.Direction == Direction.Down)
            {
                adjacentCategory.MoveUp();
            }

            _dbContext.Events.Add(new Event(new CategoryReordered
            {
                SiteId = command.SiteId,
                MemberId = command.MemberId,
                TargetId = adjacentCategory.Id,
                TargetType = typeof(Category).Name,
                SortOrder = adjacentCategory.SortOrder
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
                    x.Status != StatusType.Deleted);

            if (category == null)
            {
                throw new DataException($"Category with Id {command.Id} not found.");
            }

            category.Delete();
            _dbContext.Events.Add(new Event(new CategoryDeleted
            {
                SiteId = command.SiteId,
                MemberId = command.MemberId,
                TargetId = category.Id,
                TargetType = typeof(Category).Name
            }));

            var otherCategories = await _dbContext.Categories
                .Where(x =>
                    x.SiteId == command.SiteId &&
                    x.Id != command.Id &&
                    x.Status != StatusType.Deleted)
                .ToListAsync();

            for (int i = 0; i < otherCategories.Count; i++)
            {
                otherCategories[i].Reorder(i + 1);
                _dbContext.Events.Add(new Event(new CategoryReordered
                {
                    SiteId = command.SiteId,
                    MemberId = command.MemberId,
                    TargetId = otherCategories[i].Id,
                    TargetType = typeof(Category).Name,
                    SortOrder = otherCategories[i].SortOrder
                }));
            }

            foreach (var forum in category.Forums)
            {
                forum.Delete();
                _dbContext.Events.Add(new Event(new ForumDeleted
                {
                    SiteId = command.SiteId,
                    MemberId = command.MemberId,
                    TargetId = forum.Id,
                    TargetType = typeof(Forum).Name
                }));
            }

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Categories(command.SiteId));
        }
    }
}
