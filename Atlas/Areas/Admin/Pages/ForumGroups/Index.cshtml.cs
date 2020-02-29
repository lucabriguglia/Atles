using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Data;
using Atlas.Framework;
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
            var entities = await _dbContext.ForumGroups
                .Include(x => x.PermissionSet)
                .Where(x => x.SiteId == _contextService.CurrentSite().Id)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            foreach (var entity in entities)
            {
                ForumGroups.Add(new ForumGroupModel
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    SortOrder = entity.SortOrder,
                    TotalTopics = entity.TopicsCount,
                    TotalReplies = entity.RepliesCount
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
