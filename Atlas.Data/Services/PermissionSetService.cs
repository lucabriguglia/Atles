using System.Threading.Tasks;
using Atlas.Data.Caching;
using Atlas.Domain.PermissionSets;
using Atlas.Domain.PermissionSets.Commands;
using FluentValidation;

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

        public Task CreateAsync(CreatePermissionSet command)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateAsync(UpdatePermissionSet command)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(DeletePermissionSet command)
        {
            throw new System.NotImplementedException();
        }
    }
}