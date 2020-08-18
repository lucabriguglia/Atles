using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Atlify.Data.Caching;
using Atlify.Domain;
using Atlify.Domain.Forums;
using Atlify.Domain.Forums.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Atlify.Data.Services
{
    public class ForumService : IForumService
    {
        private readonly AtlifyDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IValidator<CreateForum> _createValidator;
        private readonly IValidator<UpdateForum> _updateValidator;

        public ForumService(AtlifyDbContext dbContext,
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
                .Where(x => x.CategoryId == command.CategoryId && x.Status != StatusType.Deleted)
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
                command.MemberId,
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
                .FirstOrDefaultAsync(x =>
                    x.Category.SiteId == command.SiteId &&
                    x.Id == command.Id && 
                    x.Status != StatusType.Deleted);

            if (forum == null)
            {
                throw new DataException($"Forum with Id {command.Id} not found.");
            }

            var originalCategoryId = forum.CategoryId;

            if (originalCategoryId != command.CategoryId)
            {
                await ReorderForumsInCategory(originalCategoryId, command.Id, command.SiteId, command.MemberId);

                var newCategoryForumsCount = await _dbContext.Forums
                    .Where(x => x.CategoryId == command.CategoryId && x.Status != StatusType.Deleted)
                    .CountAsync();

                forum.Reorder(newCategoryForumsCount + 1);
            }

            forum.UpdateDetails(command.CategoryId, command.Name, command.Slug, command.Description, command.PermissionSetId);
            _dbContext.Events.Add(new Event(command.SiteId,
                command.MemberId,
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
                    x.Status != StatusType.Deleted);

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
                command.MemberId,
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
                    x.Status != StatusType.Deleted);

            if (command.Direction == Direction.Up)
            {
                adjacentForum.MoveDown();
            }
            else if (command.Direction == Direction.Down)
            {
                adjacentForum.MoveUp();
            }

            _dbContext.Events.Add(new Event(command.SiteId,
                command.MemberId,
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
                    x.Status != StatusType.Deleted);

            if (forum == null)
            {
                throw new DataException($"Forum with Id {command.Id} not found.");
            }

            forum.Delete();
            _dbContext.Events.Add(new Event(command.SiteId,
                command.MemberId,
                EventType.Deleted,
                typeof(Forum),
                forum.Id));

            await ReorderForumsInCategory(forum.CategoryId, command.Id, command.SiteId, command.MemberId);

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
                    x.Status != StatusType.Deleted)
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
