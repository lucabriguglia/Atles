using Atlas.Domain;
using Atlas.Domain.ForumGroups;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Atlas.Data.Rules
{
    public class ForumGroupRules : IForumGroupRules
    {
        private readonly AtlasDbContext _dbContext;

        public ForumGroupRules(AtlasDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsNameUniqueAsync(Guid siteId, string name)
        {
            var any = await _dbContext.ForumGroups.AnyAsync(x => x.SiteId == siteId && x.Name == name && x.Status != StatusType.Deleted);
            return !any;
        }

        public async Task<bool> IsNameUniqueAsync(Guid siteId, Guid id, string name)
        {
            var any = await _dbContext.ForumGroups.AnyAsync(x => x.SiteId == siteId && x.Id != id && x.Name == name && x.Status != StatusType.Deleted);
            return !any;
        }
    }
}
