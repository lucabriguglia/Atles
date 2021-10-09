using System.Threading.Tasks;
using Atles.Data;
using Atles.Domain.PermissionSets;
using Atles.Domain.PermissionSets.Rules;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Queries;

namespace Atles.Domain.Handlers.PermissionSets.Rules
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