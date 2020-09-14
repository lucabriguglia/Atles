using System;
using System.Threading.Tasks;
using Atles.Domain.PermissionSets;
using Microsoft.EntityFrameworkCore;

namespace Atles.Data.Rules
{
    public class PermissionSetRules : IPermissionSetRules
    {
        private readonly AtlesDbContext _dbContext;

        public PermissionSetRules(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsNameUniqueAsync(Guid siteId, string name)
        {
            var any = await _dbContext.PermissionSets
                .AnyAsync(x => x.SiteId == siteId &&
                               x.Name == name &&
                               x.Status != PermissionSetStatusType.Deleted);
            return !any;
        }

        public async Task<bool> IsNameUniqueAsync(Guid siteId, string name, Guid id)
        {
            var any = await _dbContext.PermissionSets
                .AnyAsync(x => x.SiteId == siteId &&
                               x.Name == name &&
                               x.Status != PermissionSetStatusType.Deleted &&
                               x.Id != id);
            return !any;
        }

        public async Task<bool> IsValidAsync(Guid siteId, Guid id)
        {
            var any = await _dbContext.PermissionSets
                .AnyAsync(x => x.SiteId == siteId && 
                               x.Id == id && 
                               x.Status == PermissionSetStatusType.Published);
            return any;
        }

        public Task<bool> IsInUseAsync(Guid siteId, Guid id)
        {
            throw new NotImplementedException();
        }
    }
}