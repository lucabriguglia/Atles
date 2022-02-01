using Atles.Core;
using Microsoft.AspNetCore.Authorization;

namespace Atles.Server.Controllers.Admin
{
    [Authorize(Policy = "Admin")]
    public abstract class AdminControllerBase : SiteControllerBase
    {
        public AdminControllerBase(IDispatcher dispatcher) : base(dispatcher)
        {
        }
    }
}