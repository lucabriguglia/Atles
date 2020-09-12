using Atlas.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Atlas.Domain.Users;

namespace Atlas.Data.Rules
{
    public class UserRules : IUserRules
    {
        private readonly AtlasDbContext _dbContext;

        public UserRules(AtlasDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsDisplayNameUniqueAsync(string displayName)
        {
            var any = await _dbContext.Users
                .AnyAsync(x => x.DisplayName == displayName && 
                               x.Status != UserStatusType.Deleted);
            return !any;
        }

        public async Task<bool> IsDisplayNameUniqueAsync(string displayName, Guid id)
        {
            var any = await _dbContext.Users
                .AnyAsync(x => x.DisplayName == displayName && 
                               x.Status != UserStatusType.Deleted &&
                               x.Id != id);
            return !any;
        }
    }
}
