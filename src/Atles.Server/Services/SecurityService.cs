using Atles.Domain;
using Atles.Models.Public;

namespace Atles.Server.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SecurityService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool HasPermission(PermissionModel model)
        {
            if (model.AllUsers)
            {
                return true;
            }

            if (model.RegisteredUsers)
            {
                return _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
            }

            foreach (var role in model.Roles)
            {
                if (_httpContextAccessor.HttpContext.User.IsInRole(role))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasPermission(PermissionType permissionType, IList<PermissionModel> models)
        {
            var model = models.FirstOrDefault(x => x.Type == permissionType);

            return model != null && HasPermission(model);
        }
    }
}