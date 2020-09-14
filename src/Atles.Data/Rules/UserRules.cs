using System;
using System.Threading.Tasks;
using Atles.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Atles.Data.Rules
{
    public class UserRules : IUserRules
    {
        private readonly AtlesDbContext _dbContext;

        public UserRules(AtlesDbContext dbContext)
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
