using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas.Data;
using Atlas.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Atlas.Areas.Admin.Pages.Forums
{
    public class IndexModel : PageModel
    {
        private readonly AtlasDbContext _context;
        private readonly IContextService _contextService;
        private readonly IForumGroupService _forumGroupService;
        private readonly IForumService _forumService;

        public IndexModel(AtlasDbContext context, 
            IContextService contextService, 
            IForumGroupService forumGroupService, 
            IForumService forumService)
        {
            _context = context;
            _contextService = contextService;
            _forumGroupService = forumGroupService;
            _forumService = forumService;
        }

        public async Task OnGetAsync(Guid? forumGroupId)
        {
            var siteId = _contextService.CurrentSite().Id;

            var forumGroups = await _forumGroupService.GetAll(siteId);

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

            var forums = await _forumService.GetAll(currentForumGroup.Id);

            foreach (var entity in forums)
            {
                var permissionSetName = string.IsNullOrEmpty(entity.PermissionSetName())
                    ? string.IsNullOrEmpty(currentForumGroup.PermissionSetName())
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
