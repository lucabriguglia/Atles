using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Data;
using Atlas.Models;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Services
{
    public class PermissionSetService : IPermissionSetService
    {
        private readonly AtlasDbContext _context;

        public PermissionSetService(AtlasDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PermissionSet>> GetAll(Guid siteId)
        {
            return await _context.PermissionSets
                .Where(x => x.SiteId == siteId)
                .ToListAsync();
        }
    }
}