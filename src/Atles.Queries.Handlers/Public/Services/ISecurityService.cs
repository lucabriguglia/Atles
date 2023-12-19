using Atles.Domain;
using Atles.Models.Public;

namespace Atles.Queries.Handlers.Public.Services
{
    public interface ISecurityService
    {
        bool HasPermission(PermissionModel model);
        bool HasPermission(PermissionType type, IList<PermissionModel> models);
    }
}
