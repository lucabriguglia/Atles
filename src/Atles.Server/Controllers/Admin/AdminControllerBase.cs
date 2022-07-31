using Atles.Core;
using Microsoft.AspNetCore.Authorization;

namespace Atles.Server.Controllers.Admin
{
    [Authorize(Policy = "Admin")]
    public abstract class AdminControllerBase : SiteControllerBase
    {
        protected AdminControllerBase(IDispatcher dispatcher) : base(dispatcher)
        {
        }
    }
}