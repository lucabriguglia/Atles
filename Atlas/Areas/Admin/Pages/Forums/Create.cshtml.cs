using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Data;
using Atlas.Domain;
using Atlas.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Areas.Admin.Pages.Forums
{
    public class CreateModel : PageModel
    {
        private readonly IContextService _contextService;
        private readonly AtlasDbContext _dbContext;

        public CreateModel(IContextService contextService, AtlasDbContext dbContext)
        {
            _contextService = contextService;
            _dbContext = dbContext;
        }

        [BindProperty]
        public ForumModel Forum { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var site = await _contextService.CurrentSiteAsync();

            var groups = await _dbContext.ForumGroups
                .Where(x => x.SiteId == site.Id && x.Status != StatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            var permissionSets = await _dbContext.PermissionSets
                .Where(x => x.SiteId == site.Id && x.Status != StatusType.Deleted)
                .ToListAsync();

            ViewData["ForumGroupId"] = new SelectList(groups, "Id", "Name");
            ViewData["PermissionSetId"] = new SelectList(permissionSets, "Id", "Name");

            return Page();
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var forumsCount = await _dbContext.Forums
                .Where(x => x.ForumGroupId == Forum.ForumGroupId && x.Status != StatusType.Deleted)
                .CountAsync();

            var sortOrder = forumsCount + 1;

            var forum = new Forum(Forum.ForumGroupId, 
                Forum.Name, 
                sortOrder, 
                Forum.PermissionSetId);

            _dbContext.Forums.Add(forum);
            await _dbContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public class ForumModel
        {
            [Required]
            public Guid ForumGroupId { get; set; }

            [Required]
            public string Name { get; set; }

            public Guid? PermissionSetId { get; set; }
        }
    }
}
