using System;
using System.Threading.Tasks;

namespace Atlify.Domain.Categories
{
    public interface ICategoryRules
    {
        Task<bool> IsNameUniqueAsync(Guid siteId, string name);
        Task<bool> IsNameUniqueAsync(Guid siteId, string name, Guid id);
    }
}
