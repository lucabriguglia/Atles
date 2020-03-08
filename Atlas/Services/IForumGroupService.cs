using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atlas.Models;

namespace Atlas.Services
{
    public interface IForumGroupService
    {
        Task<IEnumerable<ForumGroup>> GetAll(Guid siteId);
    }
}