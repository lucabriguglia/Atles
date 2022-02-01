using System.Data;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain.Commands.Handlers.Extensions;
using Atles.Domain.Events;
using Atles.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Atles.Domain.Commands.Handlers
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

        public async Task<IEnumerable<IEvent>> Handle(DeletePermissionSet command)
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

            return new IEvent[] { @event };
        }
    }
}
