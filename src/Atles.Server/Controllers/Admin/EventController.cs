using Atles.Core;
using Atles.Queries.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Controllers.Admin;

[Route("api/admin/events")]
public class EventController : AdminControllerBase
{
    public EventController(IDispatcher dispatcher) : base(dispatcher)
    {
    }

    [HttpGet("target-model/{id}")]
    public async Task<ActionResult> Target(Guid id)
    {
        return await ProcessGet(new GetTargetEventsComponent
        {
            SiteId = CurrentSite.Id,
            Id = id
        });
    }
}
