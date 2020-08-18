using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlify.Server.Controllers.Admin
{
    [Authorize(Policy = "Admin")]
    [ApiController]
    public class AdminControllerBase : ControllerBase
    {
    }
}