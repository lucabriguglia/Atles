using System;
using System.Threading.Tasks;

namespace Atlas.Domain.ForumGroups
{
    public interface IForumGroupRules
    {
        Task<bool> IsNameUniqueAsync(Guid siteId, string name);
        Task<bool> IsNameUniqueAsync(Guid siteId, string name, Guid id);
    }
}
