using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Data.Caching;
using Atlas.Domain;
using Atlas.Domain.Forums;
using Atlas.Domain.Forums.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Data.Services
{
    public class ForumService : IForumService
    {
        private readonly AtlasDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IValidator<CreateForum> _createValidator;
        private readonly IValidator<UpdateForum> _updateValidator;

        public ForumService(AtlasDbContext dbContext,
            ICacheManager cacheManager,
            IValidator<CreateForum> createValidator,
            IValidator<UpdateForum> updateValidator)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task CreateAsync(CreateForum command)
        {
            await _createValidator.ValidateCommandAsync(command);

            var forumsCount = await _dbContext.Forums
                .Where(x => x.CategoryId == command.CategoryId && x.Status != ForumStatusType.Deleted)
                .CountAsync();

            var sortOrder = forumsCount + 1;

            var forum = new Forum(command.Id,
                command.CategoryId,
                command.Name,
                command.Slug,
                command.Description,
                sortOrder,
                command.PermissionSetId);

            _dbContext.Forums.Add(forum);
            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Created,
                typeof(Forum),
                forum.Id,
                new
                {
                    forum.Name,
                    forum.Slug,
                    forum.Description,
                    forum.CategoryId,
                    forum.PermissionSetId,
                    forum.SortOrder
                }));

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Categories(command.SiteId)); 
            _cacheManager.Remove(CacheKeys.CurrentForums(command.SiteId));
        }

        public async Task UpdateAsync(UpdateForum command)
        {
            await _updateValidator.ValidateCommandAsync(command);

            var forum = await _dbContext.Forums
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x =>
                    x.Category.SiteId == command.SiteId &&
                    x.Id == command.Id && 
                    x.Status != ForumStatusType.Deleted);

            if (forum == null)
            {
                throw new DataException($"Forum with Id {command.Id} not found.");
            }

            var originalCategoryId = forum.CategoryId;

            if (originalCategoryId != command.CategoryId)
            {
                forum.Category.DecreaseTopicsCount(forum.TopicsCount);
                forum.Category.DecreaseRepliesCount(forum.RepliesCount);

                var newCategory = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == command.CategoryId);
                newCategory.IncreaseTopicsCount(forum.TopicsCount);
                newCategory.IncreaseRepliesCount(forum.RepliesCount);

                await ReorderForumsInCategory(originalCategoryId, command.Id, command.SiteId, command.UserId);

                var newCategoryForumsCount = await _dbContext.Forums
                    .Where(x => x.CategoryId == command.CategoryId && x.Status != ForumStatusType.Deleted)
                    .CountAsync();

                forum.Reorder(newCategoryForumsCount + 1);
            }

            forum.UpdateDetails(command.CategoryId, command.Name, command.Slug, command.Description, command.PermissionSetId);
            
            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Updated,
                typeof(Forum),
                forum.Id,
                new
                {
                    forum.Name,
                    forum.Slug,
                    forum.Description,
                    forum.CategoryId,
                    forum.PermissionSetId
                }));

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Forum(command.Id));
            _cacheManager.Remove(CacheKeys.CurrentForums(command.SiteId));
        }

        public async Task MoveAsync(MoveForum command)
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

        public async Task DeleteAsync(DeleteForum command)
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

            forum.Delete();
            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Deleted,
                typeof(Forum),
                forum.Id));

            await ReorderForumsInCategory(forum.CategoryId, command.Id, command.SiteId, command.UserId);

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Forum(forum.Id));
            _cacheManager.Remove(CacheKeys.Categories(command.SiteId));
            _cacheManager.Remove(CacheKeys.CurrentForums(command.SiteId));
        }

        private async Task ReorderForumsInCategory(Guid categoryId, Guid forumIdToExclude, Guid siteId, Guid memberId)
        {
            var forums = await _dbContext.Forums
                .Where(x =>
                    x.CategoryId == categoryId &&
                    x.Id != forumIdToExclude &&
                    x.Status != ForumStatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            for (int i = 0; i < forums.Count; i++)
            {
                forums[i].Reorder(i + 1);
                _dbContext.Events.Add(new Event(siteId,
                    memberId,
                    EventType.Reordered,
                    typeof(Forum),
                    forums[i].Id,
                    new
                    {
                        forums[i].SortOrder
                    }));
            }
        }
    }
}
