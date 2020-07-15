using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atlas.Caching;
using Atlas.Data;
using Atlas.Domain;
using Atlas.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Atlas.Pages
{
    public class ForumModel : PageModel
    {
        private readonly AtlasDbContext _dbContext;
        private readonly IContextService _contextService;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<IndexModel> _logger;

        public ForumModel(AtlasDbContext dbContext, 
            IContextService contextService, 
            ICacheManager cacheManager, 
            ILogger<IndexModel> logger)
        {
            _dbContext = dbContext;
            _contextService = contextService;
            _cacheManager = cacheManager;
            _logger = logger;
        }

        public string ForumName { get; set; }
        public IList<TopicModel> Topics { get; set; } = new List<TopicModel>();

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forum = await _dbContext.Forums.FirstOrDefaultAsync(x => x.Id == id && x.Status == StatusType.Published);

            if (forum == null)
            {
                return NotFound();
            }

            ForumName = forum.Name;

            return Page();
        }

        public class TopicModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }

            public IList<ForumModel> Forums { get; set; }
        }
    }
}
