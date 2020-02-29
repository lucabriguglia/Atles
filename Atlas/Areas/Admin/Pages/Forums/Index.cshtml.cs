using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas.Data;
using Atlas.Models;
using Atlas.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Atlas.Areas.Admin.Pages.Forums
{
    public class IndexModel : PageModel
    {
        private readonly AtlasDbContext _context;
        private readonly IContextService _contextService;

        public IndexModel(AtlasDbContext context, IContextService contextService)
        {
            _context = context;
            _contextService = contextService;
        }

        public async Task OnGetAsync(Guid? forumGroupId)
        {
            var siteId = _contextService.CurrentSite().Id;

            var forumGroups = await _context.ForumGroups
                .Include(x => x.PermissionSet)
                .Where(x => x.SiteId == siteId)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            if (forumGroupId == null)
            {
                forumGroupId = forumGroups.FirstOrDefault().Id;
            }

            ViewData["ForumGroupId"] = new SelectList(forumGroups, "Id", "Id", forumGroupId);

            var entities = await _context.Forums
                .Include(x => x.PermissionSet)
                .Where(x => x.ForumGroupId == forumGroupId)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            foreach (var entity in entities)
            {
                Forums.Add(new ForumModel
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    SortOrder = entity.SortOrder,
                    TotalTopics = entity.TopicsCount,
                    TotalReplies = entity.RepliesCount,
                    PermissionSetName = entity.PermissionSet?.Name ?? "<Inherited>"
                });
            }
        }

        public IList<ForumModel> Forums { get; } = new List<ForumModel>();

        public class ForumModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public int SortOrder { get; set; }
            public int TotalTopics { get; set; }
            public int TotalReplies { get; set; }
            public string PermissionSetName { get; set; }
        }
    }
}
