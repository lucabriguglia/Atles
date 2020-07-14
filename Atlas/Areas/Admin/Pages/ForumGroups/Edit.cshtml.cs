using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Atlas.Data;
using Atlas.Domain;
using Atlas.Services;

namespace Atlas.Areas.Admin.Pages.ForumGroups
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
        public ForumGroupModel ForumGroup { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumGroup = await _dbContext.ForumGroups.FirstOrDefaultAsync(x => x.Id == id && x.Status != StatusType.Deleted);

            if (forumGroup == null)
            {
                return NotFound();
            }

            ForumGroup = new ForumGroupModel
            {
                Id = forumGroup.Id,
                Name = forumGroup.Name,
                PermissionSetId = forumGroup.PermissionSetId
            };

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

            var forumGroup = await _dbContext.ForumGroups.FirstOrDefaultAsync(x => x.Id == ForumGroup.Id && x.Status != StatusType.Deleted);

            if (forumGroup == null)
            {
                return NotFound();
            }

            forumGroup.UpdateDetails(ForumGroup.Name, ForumGroup.PermissionSetId);

            await _dbContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public class ForumGroupModel
        {
            public Guid Id { get; set; }

            [Required]
            public string Name { get; set; }

            public Guid? PermissionSetId { get; set; }
        }
    }
}
