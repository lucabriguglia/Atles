using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas.Data;
using Atlas.Models;

namespace Atlas.Areas.Admin.Pages.Member
{
    public class IndexModel : PageModel
    {
        private readonly Atlas.Data.AtlasDbContext _context;

        public IndexModel(Atlas.Data.AtlasDbContext context)
        {
            _context = context;
        }

        public IList<Models.Member> Member { get;set; }

        public async Task OnGetAsync()
        {
            Member = await _context.Members.ToListAsync();
        }
    }
}
