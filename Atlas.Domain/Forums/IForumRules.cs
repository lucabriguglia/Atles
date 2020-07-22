using System;
using System.Threading.Tasks;

namespace Atlas.Domain.Forums
{
    public interface IForumRules
    {
        Task<bool> IsNameUniqueAsync(Guid siteId, string name);
        Task<bool> IsNameUniqueAsync(Guid siteId, string name, Guid id);
    }
}
