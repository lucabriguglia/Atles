using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atlas.Models;

namespace Atlas.Services
{
    public interface IForumService
    {
        Task<IEnumerable<Forum>> GetAll(Guid forumGroupId);
    }
}