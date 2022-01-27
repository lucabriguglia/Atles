using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain.Models;
using Atles.Domain.Models.PermissionSets;
using Atles.Domain.Models.PermissionSets.Commands;
using Atles.Domain.Models.PermissionSets.Events;
using Atles.Infrastructure.Commands;
using Atles.Infrastructure.Events;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Atles.Domain.Handlers.PermissionSets.Commands
{
    public class UpdatePermissionSetHandler : ICommandHandler<UpdatePermissionSet>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly IValidator<UpdatePermissionSet> _validator;
        private readonly ICacheManager _cacheManager;

        public UpdatePermissionSetHandler(AtlesDbContext dbContext,
            IValidator<UpdatePermissionSet> validator,
            ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _validator = validator;
            _cacheManager = cacheManager;
        }

        public async Task<IEnumerable<IEvent>> Handle(UpdatePermissionSet command)
        {
            await _validator.ValidateCommand(command);

            var permissionSet = await _dbContext.PermissionSets
                .Include(x => x.Permissions)
                .FirstOrDefaultAsync(x =>
                    x.SiteId == command.SiteId &&
                    x.Id == command.Id &&
                    x.Status != PermissionSetStatusType.Deleted);

            if (permissionSet == null)
            {
                throw new DataException($"Permission Set with Id {command.Id} not found.");
            }

            foreach (var permission in permissionSet.Permissions)
            {
                _dbContext.Permissions.Remove(permission);
            }

            permissionSet.UpdateDetails(command.Name, command.Permissions);

            var @event = new PermissionSetUpdated
            {
                Name = permissionSet.Name,
                Permissions = command.Permissions,
                TargetId = permissionSet.Id,
                TargetType = nameof(PermissionSet),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(@event.ToDbEntity());

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.PermissionSet(command.Id));

            return new IEvent[] { @event };
        }
    }
}
