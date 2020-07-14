using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Caching;
using Atlas.Data;
using Atlas.Domain;
using Atlas.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Atlas.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AtlasDbContext _dbContext;
        private readonly IContextService _contextService;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(AtlasDbContext dbContext, 
            IContextService contextService, 
            ICacheManager cacheManager, 
            ILogger<IndexModel> logger)
        {
            _dbContext = dbContext;
            _contextService = contextService;
            _cacheManager = cacheManager;
            _logger = logger;
        }

        public IList<ForumGroupModel> ForumGroups { get; set; } = new List<ForumGroupModel>();

        public async Task OnGetAsync()
        {
            var site = await _contextService.CurrentSiteAsync();

            ForumGroups = await _cacheManager.GetOrSetAsync(CacheKeys.ForumGroups(site.Id), async () =>
            {
                var forumGroups = await _dbContext.ForumGroups
                    .Include(x => x.PermissionSet)
                    .Where(x => x.SiteId == site.Id && x.Status != StatusType.Deleted)
                    .OrderBy(x => x.SortOrder)
                    .ToListAsync();

                return forumGroups.Select(forumGroup => new ForumGroupModel
                {
                    Id = forumGroup.Id, 
                    Name = forumGroup.Name
                }).ToList();
            });

            foreach (var forumGroup in ForumGroups)
            {
                forumGroup.Forums = await _cacheManager.GetOrSetAsync(CacheKeys.Forums(forumGroup.Id), async () =>
                {
                    var forums = await _dbContext.Forums
                        .Include(x => x.PermissionSet)
                        .Where(x => x.ForumGroupId == forumGroup.Id && x.Status != StatusType.Deleted)
                        .OrderBy(x => x.SortOrder)
                        .ToListAsync();

                    return forums.Select(forum => new ForumModel
                    {
                        Id = forum.Id,
                        Name = forum.Name,
                        TotalTopics = forum.TopicsCount,
                        TotalReplies = forum.RepliesCount
                    }).ToList();
                });
            }
        }

        public class ForumGroupModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }

            public IList<ForumModel> Forums { get; set; }
        }

        public class ForumModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public int TotalTopics { get; set; }
            public int TotalReplies { get; set; }
        }
    }
}
