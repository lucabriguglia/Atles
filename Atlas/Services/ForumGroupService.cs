using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Data;
using Atlas.Domain;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Services
{
    public class ForumGroupService : IForumGroupService
    {
        private readonly AtlasDbContext _context;

        public ForumGroupService(AtlasDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ForumGroup>> GetAll(Guid siteId)
        {
            return await _context.ForumGroups
                .Include(x => x.PermissionSet)
                .Where(x => x.SiteId == siteId)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();
        }
    }
}