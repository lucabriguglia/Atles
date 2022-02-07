using System;
using System.Threading.Tasks;
using Atles.Core;
using Atles.Domain.Commands.Subscriptions;
using Atles.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atles.Server.Controllers.Public
{
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
            var site = await CurrentSite();
            var user = await CurrentUser();

            var command = new AddSubscription
            {
                Type = SubscriptionType.Topic,
                ItemId = itemId,
                ForumId = forumId,
                SiteId = site.Id,
                UserId = user.Id
            };

            await _dispatcher.Send(command);

            return Ok();
        }

        [HttpPost("remove-subscription/{forumId}/{itemId}")]
        public async Task<ActionResult> RemoveSubscription(Guid forumId, Guid itemId)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var command = new RemoveSubscription
            {
                ItemId = itemId,
                SiteId = site.Id,
                UserId = user.Id
            };

            await _dispatcher.Send(command);

            return Ok();
        }
    }
}
