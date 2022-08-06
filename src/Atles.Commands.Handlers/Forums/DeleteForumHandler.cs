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

public class DeleteForumHandler : ICommandHandler<DeleteForum>
{
    private readonly AtlesDbContext _dbContext;
    private readonly ICacheManager _cacheManager;

    public DeleteForumHandler(AtlesDbContext dbContext, ICacheManager cacheManager)
    {
        _dbContext = dbContext;
        _cacheManager = cacheManager;
    }

    public async Task<CommandResult> Handle(DeleteForum command)
    {
        var forum = await _dbContext.Forums
            .FirstOrDefaultAsync(x =>
                x.Category.SiteId == command.SiteId &&
                x.Id == command.ForumId &&
                x.Status != ForumStatusType.Deleted);

        if (forum == null)
        {
            throw new DataException($"Forum with Id {command.ForumId} not found.");
        }

        forum.Delete();

        var @event = new ForumDeleted
        {
            TargetId = forum.Id,
            TargetType = nameof(Forum),
            SiteId = command.SiteId,
            UserId = command.UserId
        };

        _dbContext.Events.Add(@event.ToDbEntity());

        await ReorderForumsInCategory(forum.CategoryId, command.ForumId, command.SiteId, command.UserId);

        await _dbContext.SaveChangesAsync();

        _cacheManager.Remove(CacheKeys.Forum(forum.Id));
        _cacheManager.Remove(CacheKeys.Categories(command.SiteId));
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
