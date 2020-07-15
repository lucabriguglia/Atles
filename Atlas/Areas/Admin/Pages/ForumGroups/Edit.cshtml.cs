using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Caching;
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
        private readonly ICacheManager _cacheManager;

        public EditModel(IContextService contextService, AtlasDbContext dbContext, ICacheManager cacheManager)
        {
            _contextService = contextService;
            _dbContext = dbContext;
            _cacheManager = cacheManager;
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

            var site = await _contextService.CurrentSiteAsync();

            var permissionSets = await _dbContext.PermissionSets
                .Where(x => x.SiteId == site.Id && x.Status != StatusType.Deleted)
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

            var member = await _contextService.CurrentMemberAsync();

            var forumGroup = await _dbContext.ForumGroups.FirstOrDefaultAsync(x => x.Id == ForumGroup.Id && x.Status != StatusType.Deleted);

            if (forumGroup == null)
            {
                return NotFound();
            }

            forumGroup.UpdateDetails(ForumGroup.Name, ForumGroup.PermissionSetId);

            _dbContext.Events.Add(new Event(nameof(ForumGroup), EventType.Updated, forumGroup.Id, member.Id, new
            {
                forumGroup.Name,
                forumGroup.PermissionSetId
            }));

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.ForumGroups(forumGroup.SiteId));

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
