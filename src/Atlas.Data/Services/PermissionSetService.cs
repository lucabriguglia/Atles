using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Data.Caching;
using Atlas.Domain;
using Atlas.Domain.PermissionSets;
using Atlas.Domain.PermissionSets.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Data.Services
{
    public class PermissionSetService : IPermissionSetService
    {
        private readonly AtlasDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IValidator<CreatePermissionSet> _createValidator;
        private readonly IValidator<UpdatePermissionSet> _updateValidator;

        public PermissionSetService(AtlasDbContext dbContext,
            ICacheManager cacheManager,
            IValidator<CreatePermissionSet> createValidator,
            IValidator<UpdatePermissionSet> updateValidator)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task CreateAsync(CreatePermissionSet command)
        {
            await _createValidator.ValidateCommandAsync(command);

            var permissionSet = new PermissionSet(command.Id,
                command.SiteId,
                command.Name,
                command.Permissions);

            _dbContext.PermissionSets.Add(permissionSet);

            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Created,
                typeof(PermissionSet),
                command.Id,
                new
                {
                    command.SiteId,
                    command.Name,
                    command.Permissions
                }));

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(UpdatePermissionSet command)
        {
            await _updateValidator.ValidateCommandAsync(command);

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

        public async Task DeleteAsync(DeletePermissionSet command)
        {
            var permissionSet = await _dbContext.PermissionSets
                .Include(x => x.Categories)
                .Include(x => x.Forums)
                .FirstOrDefaultAsync(x =>
                    x.SiteId == command.SiteId &&
                    x.Id == command.Id &&
                    x.Status != PermissionSetStatusType.Deleted);

            if (permissionSet == null)
            {
                throw new DataException($"Permission set with Id {command.Id} not found.");
            }

            if (permissionSet.Categories.Any() || permissionSet.Forums.Any())
            {
                throw new DataException($"Permission set with Id {command.Id} is in use and cannot be deleted.");
            }

            permissionSet.Delete();

            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Deleted,
                typeof(PermissionSet),
                command.Id));

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.PermissionSet(command.Id));
        }
    }
}