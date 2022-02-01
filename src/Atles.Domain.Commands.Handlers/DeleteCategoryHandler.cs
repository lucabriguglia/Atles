using System.Data;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain.Events;
using Atles.Domain.Models;
using Atles.Infrastructure.Commands;
using Atles.Infrastructure.Events;
using Microsoft.EntityFrameworkCore;

namespace Atles.Domain.Commands.Handlers
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

        public async Task<IEnumerable<IEvent>> Handle(DeleteCategory command)
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
                    x.Id != command.Id &&
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

            return new IEvent[] { categoryDeleted };
        }
    }
}
