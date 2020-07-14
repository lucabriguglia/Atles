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

namespace Atlas.Areas.Admin.Pages.ForumGroups
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
        public ForumGroupModel ForumGroup { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var siteId = _contextService.CurrentSite().Id;

            var permissionSets = await _dbContext.PermissionSets
                .Where(x => x.SiteId == siteId && x.Status != StatusType.Deleted)
                .ToListAsync();

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

            var siteId = _contextService.CurrentSite().Id;

            var forumGroupsCount = await _dbContext.ForumGroups
                .Where(x => x.SiteId == siteId && x.Status != StatusType.Deleted)
                .CountAsync();

            var sortOrder = forumGroupsCount + 1;

            var forumGroup = new ForumGroup(siteId, 
                ForumGroup.Name, 
                sortOrder, 
                ForumGroup.PermissionSetId);

            _dbContext.ForumGroups.Add(forumGroup);
            await _dbContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public class ForumGroupModel
        {
            [Required]
            public string Name { get; set; }

            public Guid? PermissionSetId { get; set; }
        }
    }
}
