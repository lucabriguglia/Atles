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

public class MoveForumHandler : ICommandHandler<MoveForum>
{
    private readonly AtlesDbContext _dbContext;
    private readonly ICacheManager _cacheManager;

    public MoveForumHandler(AtlesDbContext dbContext,
        ICacheManager cacheManager)
    {
        _dbContext = dbContext;
        _cacheManager = cacheManager;
    }

    public async Task<CommandResult> Handle(MoveForum command)
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

        if (command.Direction == DirectionType.Up)
        {
            forum.MoveUp();
        }
        else if (command.Direction == DirectionType.Down)
        {
            forum.MoveDown();
        }

        var @event = new ForumMoved
        {
            SortOrder = forum.SortOrder,
            TargetId = forum.Id,
            TargetType = nameof(Forum),
            SiteId = command.SiteId,
            UserId = command.UserId
        };

        _dbContext.Events.Add(@event.ToDbEntity());

        var sortOrderToReplace = forum.SortOrder;

        var adjacentForum = await _dbContext.Forums
            .FirstOrDefaultAsync(x =>
                x.CategoryId == forum.CategoryId &&
                x.SortOrder == sortOrderToReplace &&
                x.Status != ForumStatusType.Deleted);

        if (command.Direction == DirectionType.Up)
        {
            adjacentForum.MoveDown();
        }
        else if (command.Direction == DirectionType.Down)
        {
            adjacentForum.MoveUp();
        }

        var adjacentForumMoved = new ForumMoved
        {
            SortOrder = adjacentForum.SortOrder,
            TargetId = adjacentForum.Id,
            TargetType = nameof(Forum),
            SiteId = command.SiteId,
            UserId = command.UserId
        };

        _dbContext.Events.Add(adjacentForumMoved.ToDbEntity());

        await _dbContext.SaveChangesAsync();

        _cacheManager.Remove(CacheKeys.Categories(command.SiteId));

        return new Success(new IEvent[] { @event });
    }
}
