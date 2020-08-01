using Atlas.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Atlas.Domain.Members;

namespace Atlas.Data.Rules
{
    public class MemberRules : IMemberRules
    {
        private readonly AtlasDbContext _dbContext;

        public MemberRules(AtlasDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsDisplayNameUniqueAsync(string name)
        {
            var any = await _dbContext.Members
                .AnyAsync(x => x.DisplayName == name && 
                               x.Status != StatusType.Deleted);
            return !any;
        }

        public async Task<bool> IsDisplayNameUniqueAsync(string name, Guid id)
        {
            var any = await _dbContext.Members
                .AnyAsync(x => x.DisplayName == name && 
                               x.Status != StatusType.Deleted &&
                               x.Id != id);
            return !any;
        }
    }
}
