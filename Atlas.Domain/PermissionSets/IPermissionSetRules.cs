using System;
using System.Threading.Tasks;

namespace Atlas.Domain.PermissionSets
{
    public interface IPermissionSetRules
    {
        Task<bool> IsNameUniqueAsync(Guid siteId, string name);
        Task<bool> IsNameUniqueAsync(Guid siteId, string name, Guid id);
        Task<bool> IsValid(Guid siteId, Guid id);
    }
}
