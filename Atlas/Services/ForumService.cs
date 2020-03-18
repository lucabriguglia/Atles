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

        public async Task<Forum> GetAsync(Guid forumId)
        {
            return await _context.Forums
                .Include(x => x.ForumGroup)
                .Include(x => x.PermissionSet)
                .FirstOrDefaultAsync(x => x.Id == forumId);
        }

        public async Task<IEnumerable<Forum>> GetAllAsync(Guid forumGroupId)
        {
            return await _context.Forums
                .Include(x => x.PermissionSet)
                .Where(x => x.ForumGroupId == forumGroupId)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();
        }

        public async Task<int> GetNextSortOrderAsync(Guid forumGroupId)
        {
            var count = await _context.Forums.Where(x => x.ForumGroupId == forumGroupId).CountAsync();
            return count + 1;
        }

        public async Task CreateAsync(Forum forum)
        {
            _context.Forums.Add(forum);
            await _context.SaveChangesAsync();
        }
    }
}