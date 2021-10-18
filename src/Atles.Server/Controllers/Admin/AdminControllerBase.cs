using Microsoft.AspNetCore.Authorization;
using OpenCqrs;

namespace Atles.Server.Controllers.Admin
{
    [Authorize(Policy = "Admin")]
    public abstract class AdminControllerBase : SiteControllerBase
    {
        public AdminControllerBase(IDispatcher sender) : base(sender)
        {
        }
    }
}