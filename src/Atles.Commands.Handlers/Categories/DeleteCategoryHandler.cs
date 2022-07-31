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
using Atles.Events.Forums;
using Microsoft.EntityFrameworkCore;

namespace Atles.Commands.Handlers.Categories
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

        public async Task<CommandResult> Handle(DeleteCategory command)
        {
            var category = await _dbContext.Categories
                .Include(x => x.Forums)
                .FirstOrDefaultAsync(x =>
                    x.SiteId == command.SiteId &&
                    x.Id == command.CategoryId &&
                    x.Status != CategoryStatusType.Deleted);

            if (category == null)
            {
                throw new DataException($"Category with Id {command.CategoryId} not found.");
            }

            category.Delete();

            var categoryDeleted = new CategoryDeleted
            {
                TargetId = category.Id,
                TargetType = nameof(Category),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(categoryDeleted.ToDbEntity());

            var otherCategories = await _dbContext.Categories
                .Where(x =>
                    x.SiteId == command.SiteId &&
                    x.Id != command.CategoryId &&
                    x.Status != CategoryStatusType.Deleted)
                .ToListAsync();

            for (var i = 0; i < otherCategories.Count; i++)
            {
                otherCategories[i].Reorder(i + 1);

                var categoryMoved = new CategoryMoved
                {
                    SortOrder = otherCategories[i].SortOrder,
                    TargetId = otherCategories[i].Id,
                    TargetType = nameof(Category),
                    SiteId = command.SiteId,
                    UserId = command.UserId
                };

                _dbContext.Events.Add(categoryMoved.ToDbEntity());
            }

            foreach (var forum in category.Forums)
            {
                forum.Delete();

                var forumDeleted = new ForumDeleted
                {
                    TargetId = forum.Id,
                    TargetType = nameof(Category),
                    SiteId = command.SiteId,
                    UserId = command.UserId
                };

                _dbContext.Events.Add(forumDeleted.ToDbEntity());
            }

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Categories(command.SiteId));
            _cacheManager.Remove(CacheKeys.CurrentForums(command.SiteId));

            return new Success(new IEvent[] { categoryDeleted });
        }
    }
}
