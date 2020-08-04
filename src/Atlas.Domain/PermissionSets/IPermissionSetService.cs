using System.Threading.Tasks;
using Atlas.Domain.PermissionSets.Commands;

namespace Atlas.Domain.PermissionSets
{
    public interface IPermissionSetService
    {
        Task CreateAsync(CreatePermissionSet command);
        Task UpdateAsync(UpdatePermissionSet command);
        Task DeleteAsync(DeletePermissionSet command);
    }
}
