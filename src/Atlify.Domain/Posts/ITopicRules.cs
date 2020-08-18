using System;
using System.Threading.Tasks;

namespace Atlify.Domain.Posts
{
    public interface ITopicRules
    {
        Task<bool> IsValidAsync(Guid siteId, Guid forumId, Guid id);
    }
}
