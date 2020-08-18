using System;
using System.Threading.Tasks;
using Atlify.Domain;
using Atlify.Domain.PermissionSets;
using Microsoft.EntityFrameworkCore;

namespace Atlify.Data.Rules
{
    public class PermissionSetRules : IPermissionSetRules
    {
        private readonly AtlifyDbContext _dbContext;

        public PermissionSetRules(AtlifyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsNameUniqueAsync(Guid siteId, string name)
        {
            var any = await _dbContext.PermissionSets
                .AnyAsync(x => x.SiteId == siteId &&
                               x.Name == name &&
                               x.Status != StatusType.Deleted);
            return !any;
        }

        public async Task<bool> IsNameUniqueAsync(Guid siteId, string name, Guid id)
        {
            var any = await _dbContext.PermissionSets
                .AnyAsync(x => x.SiteId == siteId &&
                               x.Name == name &&
                               x.Status != StatusType.Deleted &&
                               x.Id != id);
            return !any;
        }

        public async Task<bool> IsValidAsync(Guid siteId, Guid id)
        {
            var any = await _dbContext.PermissionSets
                .AnyAsync(x => x.SiteId == siteId && 
                               x.Id == id && 
                               x.Status == StatusType.Published);
            return any;
        }

        public Task<bool> IsInUseAsync(Guid siteId, Guid id)
        {
            throw new NotImplementedException();
        }
    }
}