using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas.Data;
using Atlas.Models;

namespace Atlas.Areas.Admin.Pages.Forums
{
    public class DeleteModel : PageModel
    {
        private readonly Atlas.Data.AtlasDbContext _context;

        public DeleteModel(Atlas.Data.AtlasDbContext context)
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
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Forum = await _context.Forums.FindAsync(id);

            if (Forum != null)
            {
                _context.Forums.Remove(Forum);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
