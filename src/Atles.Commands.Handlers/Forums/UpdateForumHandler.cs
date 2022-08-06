using System.Data;
using Atles.Commands.Forums;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Core.Results;
using Atles.Core.Results.Types;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain;
using Atles.Events.Forums;
using Microsoft.EntityFrameworkCore;

namespace Atles.Commands.Handlers.Forums;

public class UpdateForumHandler : ICommandHandler<UpdateForum>
{
    private readonly AtlesDbContext _dbContext;
    private readonly ICacheManager _cacheManager;

    public UpdateForumHandler(AtlesDbContext dbContext, ICacheManager cacheManager)
    {
        _dbContext = dbContext;
        _cacheManager = cacheManager;
    }

    public async Task<CommandResult> Handle(UpdateForum command)
    {
        var forum = await _dbContext.Forums
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x =>
                x.Category.SiteId == command.SiteId &&
                x.Id == command.ForumId &&
                x.Status != ForumStatusType.Deleted);

        if (forum == null)
        {
            throw new DataException($"Forum with Id {command.ForumId} not found.");
        }

        var originalCategoryId = forum.CategoryId;

        if (originalCategoryId != command.CategoryId)
        {
            forum.Category.DecreaseTopicsCount(forum.TopicsCount);
            forum.Category.DecreaseRepliesCount(forum.RepliesCount);

            var newCategory = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == command.CategoryId);
            newCategory.IncreaseTopicsCount(forum.TopicsCount);
            newCategory.IncreaseRepliesCount(forum.RepliesCount);

            await ReorderForumsInCategory(originalCategoryId, command.ForumId, command.SiteId, command.UserId);

            var newCategoryForumsCount = await _dbContext.Forums
                .Where(x => x.CategoryId == command.CategoryId && x.Status != ForumStatusType.Deleted)
                .CountAsync();

            forum.Reorder(newCategoryForumsCount + 1);
        }

        forum.UpdateDetails(command.CategoryId, command.Name, command.Slug, command.Description, command.PermissionSetId);

        var @event = new ForumUpdated
        {
            Name = forum.Name,
            Slug = forum.Slug,
            Description = forum.Description,
            CategoryId = forum.CategoryId,
            PermissionSetId = forum.PermissionSetId,
            SortOrder = forum.SortOrder,
            TargetId = forum.Id,
            TargetType = nameof(Forum),
            SiteId = command.SiteId,
            UserId = command.UserId
        };

        _dbContext.Events.Add(@event.ToDbEntity());

        await _dbContext.SaveChangesAsync();

        _cacheManager.Remove(CacheKeys.Forum(command.ForumId));
        _cacheManager.Remove(CacheKeys.CurrentForums(command.SiteId));

        return new Success(new IEvent[] { @event });
    }

    private async Task ReorderForumsInCategory(Guid categoryId, Guid forumIdToExclude, Guid siteId, Guid userId)
    {
        var forums = await _dbContext.Forums
            .Where(x =>
                x.CategoryId == categoryId &&
                x.Id != forumIdToExclude &&
                x.Status != ForumStatusType.Deleted)
            .OrderBy(x => x.SortOrder)
            .ToListAsync();

        for (var i = 0; i < forums.Count; i++)
        {
            forums[i].Reorder(i + 1);

            var @event = new ForumMoved
            {
                SortOrder = forums[i].SortOrder,
                TargetId = forums[i].Id,
                TargetType = nameof(Forum),
                SiteId = siteId,
                UserId = userId
            };

            _dbContext.Events.Add(@event.ToDbEntity());
        }
    }
}
