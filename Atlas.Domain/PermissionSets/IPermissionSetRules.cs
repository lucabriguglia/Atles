using System;
using System.Threading.Tasks;

namespace Atlas.Domain.PermissionSets
{
    public interface IPermissionSetRules
    {
        Task<bool> IsValid(Guid siteId, Guid id);
    }
}
