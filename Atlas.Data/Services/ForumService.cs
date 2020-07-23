using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Data.Caching;
using Atlas.Domain;
using Atlas.Domain.Forums;
using Atlas.Domain.Forums.Commands;
using Atlas.Domain.Forums.Events;
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
                .Where(x => x.ForumGroupId == command.ForumGroupId && x.Status != StatusType.Deleted)
                .CountAsync();

            var sortOrder = forumsCount + 1;

            var forum = new Forum(command.Id,
                command.ForumGroupId,
                command.Name,
                sortOrder,
                command.PermissionSetId);

            _dbContext.Forums.Add(forum);
            _dbContext.Events.Add(new Event(new ForumCreated
            {
                SiteId = command.SiteId,
                MemberId = command.MemberId,
                TargetId = forum.Id,
                TargetType = typeof(Forum).Name,
                Name = forum.Name,
                ForumGorupId = forum.ForumGroupId,
                PermissionSetId = forum.PermissionSetId,
                SortOrder = forum.SortOrder
            }));

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Forums(command.ForumGroupId));
        }

        public async Task UpdateAsync(UpdateForum command)
        {
            await _updateValidator.ValidateCommandAsync(command);

            var forum = await _dbContext.Forums
                .FirstOrDefaultAsync(x => 
                    x.Id == command.Id && 
                    x.Status != StatusType.Deleted);

            if (forum == null)
            {
                throw new DataException($"Forum with Id {command.Id} not found.");
            }

            var originalForumGroupId = forum.ForumGroupId;

            if (originalForumGroupId != command.ForumGroupId)
            {
                await ReorderForumsInForumGroup(originalForumGroupId, command.Id, command.SiteId, command.MemberId);

                var newGroupForumsCount = await _dbContext.Forums
                    .Where(x => x.ForumGroupId == command.ForumGroupId && x.Status != StatusType.Deleted)
                    .CountAsync();

                forum.Reorder(newGroupForumsCount + 1);
            }

            forum.UpdateDetails(command.ForumGroupId, command.Name, command.PermissionSetId);
            _dbContext.Events.Add(new Event(new ForumUpdated
            {
                SiteId = command.SiteId,
                MemberId = command.MemberId,
                TargetId = forum.Id,
                TargetType = typeof(Forum).Name,
                ForumGroupId = forum.ForumGroupId,
                Name = forum.Name,
                PermissionSetId = forum.PermissionSetId
            }));

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Forums(command.ForumGroupId));
        }

        public async Task MoveAsync(MoveForum command)
        {
            var forum = await _dbContext.Forums
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id && 
                    x.Status != StatusType.Deleted);

            if (forum == null)
            {
                throw new DataException($"Forum with Id {command.Id} not found.");
            }

            var currentSortOrder = forum.SortOrder;

            if (command.Direction == Direction.Up)
            {
                forum.MoveUp();
            }
            else if (command.Direction == Direction.Down)
            {
                forum.MoveDown();
            }

            _dbContext.Events.Add(new Event(new ForumReordered
            {
                SiteId = command.SiteId,
                MemberId = command.MemberId,
                TargetId = forum.Id,
                TargetType = typeof(Forum).Name,
                SortOrder = forum.SortOrder
            }));

            var sortOrderToReplace = forum.SortOrder;

            var adjacentForum = await _dbContext.Forums
                .FirstOrDefaultAsync(x => 
                    x.ForumGroupId == forum.ForumGroupId && 
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

            _dbContext.Events.Add(new Event(new ForumReordered
            {
                SiteId = command.SiteId,
                MemberId = command.MemberId,
                TargetId = adjacentForum.Id,
                TargetType = typeof(Forum).Name,
                SortOrder = adjacentForum.SortOrder
            }));

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Forums(forum.ForumGroupId));
        }

        public async Task DeleteAsync(DeleteForum command)
        {
            var forum = await _dbContext.Forums
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id &&
                    x.Status != StatusType.Deleted);

            if (forum == null)
            {
                throw new DataException($"Forum with Id {command.Id} not found.");
            }

            forum.Delete();
            _dbContext.Events.Add(new Event(new ForumDeleted
            {
                SiteId = command.SiteId,
                MemberId = command.MemberId,
                TargetId = forum.Id,
                TargetType = typeof(Forum).Name
            }));

            await ReorderForumsInForumGroup(forum.ForumGroupId, command.Id, command.SiteId, command.MemberId);

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Forums(forum.ForumGroupId));
        }

        private async Task ReorderForumsInForumGroup(Guid forumGroupId, Guid forumIdToExclude, Guid siteId, Guid memberId)
        {
            var otherForums = await _dbContext.Forums
                .Where(x =>
                    x.ForumGroupId == forumGroupId &&
                    x.Id != forumIdToExclude &&
                    x.Status != StatusType.Deleted)
                .ToListAsync();

            for (int i = 0; i < otherForums.Count; i++)
            {
                otherForums[i].Reorder(i + 1);
                _dbContext.Events.Add(new Event(new ForumReordered
                {
                    SiteId = siteId,
                    MemberId = memberId,
                    TargetId = otherForums[i].Id,
                    TargetType = typeof(Forum).Name,
                    SortOrder = otherForums[i].SortOrder
                }));
            }
        }
    }
}
