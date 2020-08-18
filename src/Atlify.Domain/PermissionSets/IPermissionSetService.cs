using System.Threading.Tasks;
using Atlify.Domain.PermissionSets.Commands;

namespace Atlify.Domain.PermissionSets
{
    public interface IPermissionSetService
    {
        Task CreateAsync(CreatePermissionSet command);
        Task UpdateAsync(UpdatePermissionSet command);
        Task DeleteAsync(DeletePermissionSet command);
    }
}
