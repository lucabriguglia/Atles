using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Atlas.Data;
using Atlas.Models;

namespace Atlas.Areas.Admin.Pages.Forums
{
    public class CreateModel : PageModel
    {
        private readonly Atlas.Data.AtlasDbContext _context;

        public CreateModel(Atlas.Data.AtlasDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["ForumGroupId"] = new SelectList(_context.ForumGroups, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public Forum Forum { get; set; }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Forums.Add(Forum);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
