using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Data;
using Atlas.Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Atlas.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Areas.Admin.Pages.Forums
{
    public class IndexModel : PageModel
    {
        private readonly IContextService _contextService;
        private readonly AtlasDbContext _dbContext;

        public IndexModel(IContextService contextService, AtlasDbContext dbContext)
        {
            _contextService = contextService;
            _dbContext = dbContext;
        }

        public IList<ForumModel> Forums { get; } = new List<ForumModel>();

        public async Task OnGetAsync(Guid? forumGroupId)
        {
            var siteId = _contextService.CurrentSite().Id;

            var forumGroups = await _dbContext.ForumGroups
                .Where(x => x.SiteId == siteId && x.Status != StatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            if (!forumGroups.Any())
            {
                throw new ApplicationException("No Forum Groups found");
            }

            var currentForumGroup = forumGroupId == null 
                ? forumGroups.FirstOrDefault() 
                : forumGroups.FirstOrDefault(x => x.Id == forumGroupId);

            if (currentForumGroup == null)
            {
                throw new ApplicationException("Forum Group not found");
            }

            ViewData["ForumGroupId"] = new SelectList(forumGroups, "Id", "Name", currentForumGroup.Id);

            var forums = await _dbContext.Forums
                .Include(x => x.PermissionSet)
                .Where(x => x.ForumGroupId == currentForumGroup.Id && x.Status != StatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            foreach (var entity in forums)
            {
                var permissionSetName = !entity.HasPermissionSet()
                    ? !currentForumGroup.HasPermissionSet()
                        ? "<None>"
                        : currentForumGroup.PermissionSetName()
                    : entity.PermissionSetName();

                Forums.Add(new ForumModel
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    SortOrder = entity.SortOrder,
                    TotalTopics = entity.TopicsCount,
                    TotalReplies = entity.RepliesCount,
                    PermissionSetName = permissionSetName
                });
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            var forum = await _dbContext.Forums.FirstOrDefaultAsync(x => x.Id == id);

            if (forum == null)
            {
                return NotFound();
            }

            forum.Delete();

            await _dbContext.SaveChangesAsync();

            return RedirectToPage(new { forum.ForumGroupId });
        }

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
