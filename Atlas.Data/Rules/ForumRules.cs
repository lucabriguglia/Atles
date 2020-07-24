using Atlas.Domain;
using Atlas.Domain.Forums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Atlas.Data.Rules
{
    public class ForumRules : IForumRules
    {
        private readonly AtlasDbContext _dbContext;

        public ForumRules(AtlasDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsNameUniqueAsync(Guid categoryId, string name)
        {
            var any = await _dbContext.Forums
                .AnyAsync(x => x.CategoryId == categoryId && 
                               x.Name == name && 
                               x.Status != StatusType.Deleted);
            return !any;
        }

        public async Task<bool> IsNameUniqueAsync(Guid categoryId, string name, Guid id)
        {
            var any = await _dbContext.Forums
                .AnyAsync(x => x.CategoryId == categoryId && 
                               x.Name == name && 
                               x.Status != StatusType.Deleted &&
                               x.Id != id);
            return !any;
        }
    }
}
