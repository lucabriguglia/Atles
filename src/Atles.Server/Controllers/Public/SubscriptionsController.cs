using Atles.Commands.Subscriptions;
using Atles.Core;
using Atles.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Controllers.Public;

[Authorize]
[Route("api/public/subscriptions")]
public class SubscriptionsController : SiteControllerBase
{
    private readonly IDispatcher _dispatcher;
    private readonly ILogger<SubscriptionsController> _logger;

    public SubscriptionsController(IDispatcher dispatcher, ILogger<SubscriptionsController> logger) : base(dispatcher)
    {
        _dispatcher = dispatcher;
        _logger = logger;
    }

    [HttpPost("add-subscription/{forumId}/{itemId}")]
    public async Task<ActionResult> AddSubscription(Guid forumId, Guid itemId)
    {
        var command = new AddSubscription
        {
            Type = SubscriptionType.Topic,
            ItemId = itemId,
            ForumId = forumId,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id
        };

        await _dispatcher.Send(command);

        return Ok();
    }

    [HttpPost("remove-subscription/{forumId}/{itemId}")]
    public async Task<ActionResult> RemoveSubscription(Guid forumId, Guid itemId)
    {
        var command = new RemoveSubscription
        {
            ItemId = itemId,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id
        };

        await _dispatcher.Send(command);

        return Ok();
    }
}
