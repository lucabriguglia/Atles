using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atlas.Domain;

namespace Atlas.Services
{
    public interface IForumGroupService
    {
        Task<IEnumerable<ForumGroup>> GetAll(Guid siteId);
    }
}