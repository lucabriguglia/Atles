using System;
using System.Threading.Tasks;

namespace Atlas.Domain.Posts
{
    public interface ITopicRules
    {
        Task<bool> IsValidAsync(Guid siteId, Guid forumId, Guid id);
    }
}
