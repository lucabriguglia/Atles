using Atles.Data;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Queries;
using System.Threading.Tasks;
using Atles.Domain.Models.Users;
using Atles.Domain.Models.Users.Rules;

namespace Atles.Domain.Handlers.Users.Rules
{
    public class IsUserDisplayNameUniqueHandler : IQueryHandler<IsUserDisplayNameUnique, bool>
    {
        private readonly AtlesDbContext _dbContext;

        public IsUserDisplayNameUniqueHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(IsUserDisplayNameUnique query)
        {
            bool any;

            if (query.Id != null)
            {
                any = await _dbContext.Users
                .AnyAsync(x => x.DisplayName == query.DisplayName &&
                               x.Status != UserStatusType.Deleted &&
                               x.Id != query.Id);
            }
            else
            {
                any = await _dbContext.Users
                .AnyAsync(x => x.DisplayName == query.DisplayName &&
                               x.Status != UserStatusType.Deleted);
            }

            return !any;
        }
    }
}
