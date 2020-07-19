using Atlas.Server.Caching;
using Atlas.Server.Data;
using Atlas.Server.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Atlas.Server.Domain.ForumGroups
{
    public interface IForumGroupService
    {
        Task DeleteAsync(Guid id);
    }

    public class ForumGroupService : IForumGroupService
    {
        private readonly AtlasDbContext _dbContext;
        private readonly IContextService _contextService;
        private readonly ICacheManager _cacheManager;

        public ForumGroupService(AtlasDbContext dbContext, 
            IContextService contextService, 
            ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _contextService = contextService;
            _cacheManager = cacheManager;
        }

        public async Task DeleteAsync(Guid id)
        {
            var forumGroup = await _dbContext.ForumGroups.FirstOrDefaultAsync(x => x.Id == id);

            if (forumGroup == null)
            {
                throw new DataException($"Forum Group with Id {id} not found.");
            }

            var currentMember = await _contextService.CurrentMemberAsync();

            forumGroup.Delete();
            _dbContext.Events.Add(new Event(nameof(ForumGroup), EventType.Deleted, forumGroup.Id, currentMember.Id));

            var forums = await _dbContext.Forums
                .Where(x => x.ForumGroupId == forumGroup.Id)
                .ToListAsync();

            foreach (var forum in forums)
            {
                forum.Delete();
                _dbContext.Events.Add(new Event(nameof(Forum), EventType.Deleted, forum.Id, currentMember.Id));
            }

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.ForumGroups(forumGroup.SiteId));
        }
    }
}
