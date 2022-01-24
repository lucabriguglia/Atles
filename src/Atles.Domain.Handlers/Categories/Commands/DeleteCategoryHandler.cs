using Atles.Data;
using Atles.Data.Caching;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Atles.Domain.Models;
using Atles.Domain.Models.Categories;
using Atles.Domain.Models.Categories.Commands;
using Atles.Domain.Models.Categories.Events;
using Atles.Domain.Models.Forums.Events;
using Atles.Infrastructure.Commands;

namespace Atles.Domain.Handlers.Categories.Commands
{
    public class DeleteCategoryHandler : ICommandHandler<DeleteCategory>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public DeleteCategoryHandler(AtlesDbContext dbContext, ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }

        public async Task Handle(DeleteCategory command)
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

            var categoryDeletedEvent = new CategoryDeleted
            {
                TargetId = category.Id,
                TargetType = nameof(Category),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(categoryDeletedEvent.ToDbEntity());

            var otherCategories = await _dbContext.Categories
                .Where(x =>
                    x.SiteId == command.SiteId &&
                    x.Id != command.Id &&
                    x.Status != CategoryStatusType.Deleted)
                .ToListAsync();

            for (var i = 0; i < otherCategories.Count; i++)
            {
                otherCategories[i].Reorder(i + 1);

                var categoryMovedEvent = new CategoryMoved
                {
                    SortOrder = otherCategories[i].SortOrder,
                    TargetId = otherCategories[i].Id,
                    TargetType = nameof(Category),
                    SiteId = command.SiteId,
                    UserId = command.UserId
                };

                _dbContext.Events.Add(categoryMovedEvent.ToDbEntity());
            }

            foreach (var forum in category.Forums)
            {
                forum.Delete();

                var forumDeletedEvent = new ForumDeleted
                {
                    TargetId = forum.Id,
                    TargetType = nameof(Category),
                    SiteId = command.SiteId,
                    UserId = command.UserId
                };

                _dbContext.Events.Add(forumDeletedEvent.ToDbEntity());
            }

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Categories(command.SiteId));
            _cacheManager.Remove(CacheKeys.CurrentForums(command.SiteId));
        }
    }
}
