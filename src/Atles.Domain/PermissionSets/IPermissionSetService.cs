using System.Threading.Tasks;
using Atles.Domain.PermissionSets.Commands;

namespace Atles.Domain.PermissionSets
{
    public interface IPermissionSetService
    {
        Task CreateAsync(CreatePermissionSet command);
        Task UpdateAsync(UpdatePermissionSet command);
        Task DeleteAsync(DeletePermissionSet command);
    }
}
