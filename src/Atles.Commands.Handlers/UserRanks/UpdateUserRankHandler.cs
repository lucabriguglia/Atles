using System.Data;
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

public class UpdateUserRankHandler : ICommandHandler<UpdateUserRank>
{
    private readonly AtlesDbContext _dbContext;
    private readonly ICacheManager _cacheManager;

    public UpdateUserRankHandler(AtlesDbContext dbContext, ICacheManager cacheManager)
    {
        _dbContext = dbContext;
        _cacheManager = cacheManager;
    }

    public async Task<CommandResult> Handle(UpdateUserRank command)
    {
        var userRank = await _dbContext.UserRanks
            .FirstOrDefaultAsync(x =>
                x.SiteId == command.SiteId &&
                x.Id == command.Id &&
                x.Status != UserRankStatusType.Deleted);

        if (userRank == null)
        {
            throw new DataException($"User rank with Id {command.Id} not found.");
        }

        userRank.UpdateDetails(
            command.Name,
            command.Description,
            command.Badge,
            command.Role,
            command.Status,
            command.UserRankRules.ToDomainRules());

        var @event = new UserRankUpdated
        {
            Name = userRank.Name,
            Description = userRank.Description,
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
