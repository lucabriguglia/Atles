using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Data;
using Atlas.Models;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Services
{
    public class ForumService : IForumService
    {
        private readonly AtlasDbContext _context;

        public ForumService(AtlasDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Forum>> GetAll(Guid forumGroupId)
        {
            return await _context.Forums
                .Include(x => x.PermissionSet)
                .Where(x => x.ForumGroupId == forumGroupId)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();
        }
    }
}