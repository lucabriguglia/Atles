using System;
using System.Threading.Tasks;

namespace Atlas.Domain.Topics
{
    public interface ITopicRules
    {
        Task<bool> IsValidAsync(Guid siteId, Guid forumId, Guid id);
    }
}
