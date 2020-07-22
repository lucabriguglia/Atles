using System;
using System.Threading.Tasks;

namespace Atlas.Domain.Forums
{
    public interface IForumRules
    {
        Task<bool> IsNameUniqueAsync(Guid forumGroupId, string name);
        Task<bool> IsNameUniqueAsync(Guid forumGroupId, string name, Guid id);
    }
}
