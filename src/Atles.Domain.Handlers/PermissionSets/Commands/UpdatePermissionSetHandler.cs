using System.Data;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain.PermissionSets;
using Atles.Domain.PermissionSets.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Commands;

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

        public async Task Handle(UpdatePermissionSet command)
        {
            await _validator.ValidateCommandAsync(command);

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

            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Updated,
                typeof(PermissionSet),
                command.Id,
                new
                {
                    command.Name,
                    command.Permissions
                }));

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.PermissionSet(command.Id));
        }
    }
}
