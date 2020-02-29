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
    public class DetailsModel : PageModel
    {
        private readonly Atlas.Data.AtlasDbContext _context;

        public DetailsModel(Atlas.Data.AtlasDbContext context)
        {
            _context = context;
        }

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
    }
}
