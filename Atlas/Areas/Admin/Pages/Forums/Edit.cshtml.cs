using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Atlas.Data;
using Atlas.Models;
using Atlas.Services;

namespace Atlas.Areas.Admin.Pages.Forums
{
    public class EditModel : PageModel
    {
        private readonly IContextService _contextService;
        private readonly AtlasDbContext _dbContext;

        public EditModel(IContextService contextService, AtlasDbContext dbContext)
        {
            _contextService = contextService;
            _dbContext = dbContext;
        }

        [BindProperty]
        public ForumModel Forum { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forum = await _dbContext.Forums.FirstOrDefaultAsync(m => m.Id == id);

            if (forum == null)
            {
                return NotFound();
            }

            Forum = new ForumModel
            {
                Id = forum.Id,
                ForumGroupId = forum.ForumGroupId,
                Name = forum.Name,
                PermissionSetId = forum.PermissionSetId
            };

            var siteId = _contextService.CurrentSite().Id;

            var groups = await _dbContext.ForumGroups
                .Where(x => x.SiteId == siteId && x.Status != StatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            var permissionSets = await _dbContext.PermissionSets
                .Where(x => x.SiteId == siteId && x.Status != StatusType.Deleted)
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

            var forum = await _dbContext.Forums.FirstOrDefaultAsync(x => x.Id == Forum.Id && x.Status != StatusType.Deleted);

            if (forum == null)
            {
                return NotFound();
            }

            forum.UpdateDetails(Forum.ForumGroupId, Forum.Name, Forum.PermissionSetId);

            await _dbContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public class ForumModel
        {
            public Guid Id { get; set; }

            [Required]
            public Guid ForumGroupId { get; set; }

            [Required]
            public string Name { get; set; }

            public Guid? PermissionSetId { get; set; }
        }
    }
}
