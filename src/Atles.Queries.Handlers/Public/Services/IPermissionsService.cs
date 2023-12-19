using Atles.Models.Public;

namespace Atles.Queries.Handlers.Public.Services;

public interface IPermissionsService
{
    Task<IList<PermissionModel>> GetPermissions(Guid siteId, Guid? forumId, Guid? permissionSetId);
}
