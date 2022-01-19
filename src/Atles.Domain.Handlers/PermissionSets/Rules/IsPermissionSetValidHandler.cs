using System.Threading.Tasks;
using Atles.Data;
using Atles.Domain.Models.PermissionSets;
using Atles.Domain.Models.PermissionSets.Rules;
using Atles.Infrastructure.Queries;
using Microsoft.EntityFrameworkCore;

namespace Atles.Domain.Handlers.PermissionSets.Rules
{
    public class IsPermissionSetValidHandler : IQueryHandler<IsPermissionSetValid, bool>
    {
        private readonly AtlesDbContext _dbContext;

        public IsPermissionSetValidHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(IsPermissionSetValid query)
        {
            var any = await _dbContext.PermissionSets
                .AnyAsync(x => x.SiteId == query.SiteId &&
                               x.Id == query.Id &&
                               x.Status == PermissionSetStatusType.Published);
            return any;
        }
    }
}