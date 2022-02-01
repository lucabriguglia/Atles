using Atles.Core.Queries;
using Atles.Data;
using Atles.Domain.Models;
using Atles.Domain.Rules.PermissionSets;
using Microsoft.EntityFrameworkCore;

namespace Atles.Domain.Rules.Handlers.PermissionSets
{
    public class IsPermissionSetNameUniqueHandler : IQueryHandler<IsPermissionSetNameUnique, bool>
    {
        private readonly AtlesDbContext _dbContext;

        public IsPermissionSetNameUniqueHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(IsPermissionSetNameUnique query)
        {
            bool any;

            if (query.Id != null)
            {
                any = await _dbContext.PermissionSets
                    .AnyAsync(x => x.SiteId == query.SiteId &&
                                   x.Name == query.Name &&
                                   x.Status != PermissionSetStatusType.Deleted &&
                                   x.Id != query.Id);
            }
            else
            {
                any = await _dbContext.PermissionSets
                    .AnyAsync(x => x.SiteId == query.SiteId &&
                                   x.Name == query.Name &&
                                   x.Status != PermissionSetStatusType.Deleted);
            }

            return !any;
        }
    }
}