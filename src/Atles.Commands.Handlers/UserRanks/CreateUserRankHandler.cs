using Atles.Commands.UserRanks;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Core.Results;
using Atles.Core.Results.Types;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain;
using Atles.Events.UserRanks;
using Microsoft.EntityFrameworkCore;

namespace Atles.Commands.Handlers.UserRanks;

public class CreateUserRankHandler : ICommandHandler<CreateUserRank>
{
    private readonly AtlesDbContext _dbContext;
    private readonly ICacheManager _cacheManager;

    public CreateUserRankHandler(AtlesDbContext dbContext, ICacheManager cacheManager)
    {
        _dbContext = dbContext;
        _cacheManager = cacheManager;
    }

    public async Task<CommandResult> Handle(CreateUserRank command)
    {
        var count = await _dbContext.UserRanks
            .Where(x => 
                x.SiteId == command.SiteId && 
                x.Status != UserRankStatusType.Deleted)
            .CountAsync();

        var sortOrder = count + 1;

        var userRank = new UserRank(
            command.SiteId, 
            command.Name, 
            command.Description, 
            sortOrder, 
            command.Badge, 
            command.Role, 
            command.Status, 
            command.UserRankRules.ToDomainRules());

        _dbContext.UserRanks.Add(userRank);

        var @event = new UserRankCreated
        {
            Name = userRank.Name,
            Description = userRank.Description,
            SortOrder = userRank.SortOrder,
            Badge = userRank.Badge,
            Role = userRank.Role,
            Status = userRank.Status,
            UserRankRules = command.UserRankRules.ToDomainRules(),
            TargetId = userRank.Id,
            TargetType = nameof(UserRank),
            SiteId = command.SiteId,
            UserId = command.UserId
        };

        _dbContext.Events.Add(@event.ToDbEntity());

        await _dbContext.SaveChangesAsync();

        _cacheManager.Remove(CacheKeys.UserRanks(command.SiteId));

        return new Success(new IEvent[] { @event });
    }
}
