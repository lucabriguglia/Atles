using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atlas.Models;

namespace Atlas.Services
{
    public interface IForumService
    {
        Task<Forum> GetAsync(Guid forumId);
        Task<IEnumerable<Forum>> GetAllAsync(Guid forumGroupId);
        Task<int> GetNextSortOrderAsync(Guid forumGroupId);
        Task CreateAsync(Forum forum);
    }
}