using System;
using System.Threading.Tasks;

namespace Atlas.Domain.PermissionSets
{
    public interface IPermissionSetRules
    {
        Task<bool> IsNameUniqueAsync(Guid siteId, string name);
        Task<bool> IsNameUniqueAsync(Guid siteId, string name, Guid id);
        Task<bool> IsValidAsync(Guid siteId, Guid id);
        Task<bool> IsInUseAsync(Guid siteId, Guid id);
    }
}
