using System;
using System.Threading.Tasks;
using Atlify.Domain;
using Atlify.Domain.Members;
using Microsoft.EntityFrameworkCore;

namespace Atlify.Data.Rules
{
    public class MemberRules : IMemberRules
    {
        private readonly AtlifyDbContext _dbContext;

        public MemberRules(AtlifyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsDisplayNameUniqueAsync(string displayName)
        {
            var any = await _dbContext.Members
                .AnyAsync(x => x.DisplayName == displayName && 
                               x.Status != StatusType.Deleted);
            return !any;
        }

        public async Task<bool> IsDisplayNameUniqueAsync(string displayName, Guid id)
        {
            var any = await _dbContext.Members
                .AnyAsync(x => x.DisplayName == displayName && 
                               x.Status != StatusType.Deleted &&
                               x.Id != id);
            return !any;
        }
    }
}
