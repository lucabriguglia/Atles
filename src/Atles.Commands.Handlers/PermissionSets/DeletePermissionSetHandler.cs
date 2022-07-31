using System.Data;
using Atles.Commands.PermissionSets;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Core.Results;
using Atles.Core.Results.Types;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain;
using Atles.Events.PermissionSets;
using Microsoft.EntityFrameworkCore;

namespace Atles.Commands.Handlers.PermissionSets
{
    public class DeletePermissionSetHandler : ICommandHandler<DeletePermissionSet>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public DeletePermissionSetHandler(AtlesDbContext dbContext,
            ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }

        public async Task<CommandResult> Handle(DeletePermissionSet command)
        {
            var permissionSet = await _dbContext.PermissionSets
                .Include(x => x.Categories)
                .Include(x => x.Forums)
                .FirstOrDefaultAsync(x =>
                    x.SiteId == command.SiteId &&
                    x.Id == command.PermissionSetId &&
                    x.Status != PermissionSetStatusType.Deleted);

            if (permissionSet == null)
            {
                throw new DataException($"Permission set with Id {command.PermissionSetId} not found.");
            }

            if (permissionSet.Categories.Any() || permissionSet.Forums.Any())
            {
                throw new DataException($"Permission set with Id {command.PermissionSetId} is in use and cannot be deleted.");
            }

            permissionSet.Delete();

            var @event = new PermissionSetDeleted
            {
                TargetId = permissionSet.Id,
                TargetType = nameof(PermissionSet),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(@event.ToDbEntity());

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.PermissionSet(command.PermissionSetId));

            return new Success(new IEvent[] { @event });
        }
    }
}
