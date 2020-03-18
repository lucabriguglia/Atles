using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Data;
using Atlas.Framework;
using Atlas.Models;
using Atlas.Services;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Areas.Admin.Pages.ForumGroups
{
    public class IndexModel : BasePageModel
    {
        private readonly AtlasDbContext _dbContext;
        private readonly IContextService _contextService;

        public IndexModel(AtlasDbContext dbContext, IContextService contextService)
        {
            _dbContext = dbContext;
            _contextService = contextService;
        }

        public async Task OnGetAsync()
        {
            var forumGroups = await _dbContext.ForumGroups
                .Include(x => x.PermissionSet)
                .Where(x => x.SiteId == _contextService.CurrentSite().Id && x.Status != StatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            foreach (var forumGroup in forumGroups)
            {
                ForumGroups.Add(new ForumGroupModel
                {
                    Id = forumGroup.Id,
                    Name = forumGroup.Name,
                    SortOrder = forumGroup.SortOrder,
                    TotalTopics = forumGroup.TopicsCount,
                    TotalReplies = forumGroup.RepliesCount
                });
            }
        }

        public IList<ForumGroupModel> ForumGroups { get; } = new List<ForumGroupModel>();

        public class ForumGroupModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public int SortOrder { get; set; }
            public int TotalTopics { get; set; }
            public int TotalReplies { get; set; }
        }
    }
}
