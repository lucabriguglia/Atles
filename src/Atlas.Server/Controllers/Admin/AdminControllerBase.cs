using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Server.Controllers.Admin
{
    [Authorize(Policy = "Admin")]
    [ApiController]
    public class AdminControllerBase : ControllerBase
    {
    }
}