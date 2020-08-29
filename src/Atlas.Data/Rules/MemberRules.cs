using Atlas.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Atlas.Domain.Users;

namespace Atlas.Data.Rules
{
    public class MemberRules : IUserRules
    {
        private readonly AtlasDbContext _dbContext;

        public MemberRules(AtlasDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsDisplayNameUniqueAsync(string displayName)
        {
            var any = await _dbContext.Users
                .AnyAsync(x => x.DisplayName == displayName && 
                               x.Status != StatusType.Deleted);
            return !any;
        }

        public async Task<bool> IsDisplayNameUniqueAsync(string displayName, Guid id)
        {
            var any = await _dbContext.Users
                .AnyAsync(x => x.DisplayName == displayName && 
                               x.Status != StatusType.Deleted &&
                               x.Id != id);
            return !any;
        }
    }
}
