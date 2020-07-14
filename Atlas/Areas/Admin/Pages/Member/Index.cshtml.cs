using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas.Data;

namespace Atlas.Areas.Admin.Pages.Member
{
    public class IndexModel : PageModel
    {
        private readonly AtlasDbContext _context;

        public IndexModel(AtlasDbContext context)
        {
            _context = context;
        }

        public IList<Domain.Member> Member { get;set; }

        public async Task OnGetAsync()
        {
            Member = await _context.Members.ToListAsync();
        }
    }
}
