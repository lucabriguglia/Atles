using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Services
{
    public interface IForumGroupService
    {
        Task<IEnumerable<ForumGroup>> GetAll(Guid siteId);
    }
}