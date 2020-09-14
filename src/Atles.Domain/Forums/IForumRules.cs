using System;
using System.Threading.Tasks;

namespace Atlas.Domain.Forums
{
    public interface IForumRules
    {
        Task<bool> IsNameUniqueAsync(Guid siteId, Guid categoryId, string name);
        Task<bool> IsNameUniqueAsync(Guid siteId, Guid categoryId, string name, Guid id);
        Task<bool> IsValidAsync(Guid siteId, Guid id);
        Task<bool> IsSlugUniqueAsync(Guid siteId, string slug);
        Task<bool> IsSlugUniqueAsync(Guid siteId, string slug, Guid id);
    }
}
