using System;
using System.Threading.Tasks;

namespace Atles.Domain.Posts
{
    public interface ITopicRules
    {
        Task<bool> IsValidAsync(Guid siteId, Guid forumId, Guid id);
    }
}
