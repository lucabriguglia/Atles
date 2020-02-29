using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Atlas.Data;
using Atlas.Models;

namespace Atlas.Areas.Admin.Pages.Forums
{
    public class EditModel : PageModel
    {
        private readonly AtlasDbContext _context;

        public EditModel(AtlasDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Forum Forum { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Forum = await _context.Forums
                .Include(f => f.ForumGroup).FirstOrDefaultAsync(m => m.Id == id);

            if (Forum == null)
            {
                return NotFound();
            }
            ViewData["ForumGroupId"] = new SelectList(_context.ForumGroups, "Id", "Id");
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

            _context.Attach(Forum).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ForumExists(Forum.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ForumExists(Guid id)
        {
            return _context.Forums.Any(e => e.Id == id);
        }
    }
}
