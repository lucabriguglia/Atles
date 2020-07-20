using System;
using System.Threading.Tasks;
using Atlas.Domain;
using Atlas.Domain.PermissionSets;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Data.Rules
{
    public class PermissionSetRules : IPermissionSetRules
    {
        private readonly AtlasDbContext _dbContext;

        public PermissionSetRules(AtlasDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsValid(Guid siteId, Guid id)
        {
            var any = await _dbContext.PermissionSets
                .AnyAsync(x => x.SiteId == siteId && 
                               x.Id == id && 
                               x.Status != StatusType.Deleted);
            return any;
        }
    }
}