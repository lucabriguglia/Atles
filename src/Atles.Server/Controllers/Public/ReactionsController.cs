using System.Threading.Tasks;
using Atles.Domain.Models.PermissionSets;
using Atles.Domain.Models.Posts.Commands;
using Atles.Infrastructure;
using Atles.Reporting.Models.Public;
using Atles.Reporting.Models.Public.Queries;
using Atles.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atles.Server.Controllers.Public
{
    [Route("api/public/reactions")]
    [ApiController]
    public class ReactionsController : SiteControllerBase
    {
        private readonly IDispatcher _dispatcher;
        private readonly ISecurityService _securityService;
        private readonly ILogger<ReactionsController> _logger;

        public ReactionsController(IDispatcher dispatcher, ISecurityService securityService, ILogger<ReactionsController> logger) : base(dispatcher)
        {
            _dispatcher = dispatcher;
            _securityService = securityService;
            _logger = logger;
        }

        //[HttpPost("add-reaction")]
        //public async Task<ActionResult> AddReaction(TopicPageModel model)
        //{
        //    var site = await CurrentSite();
        //    var user = await CurrentUser();

        //    var permissions = await _dispatcher.Get(new GetPermissions { SiteId = site.Id, ForumId = model.Forum.Id });
        //    var canReact = _securityService.HasPermission(PermissionType.Reactions, permissions) && !user.IsSuspended;

        //    if (!canReact)
        //    {
        //        _logger.LogWarning("Unauthorized access to add reaction.", new
        //        {
        //            SiteId = site.Id,
        //            ForumId = model.Forum?.Id,
        //            TopicId = model.Topic?.Id,
        //            User = User.Identity.Name
        //        });

        //        return Unauthorized();
        //    }

        //    var command = new AddReaction
        //    {
        //        Id = postId,
        //        Type = type,
        //        SiteId = site.Id,
        //        UserId = user.Id
        //    };

        //    await _dispatcher.Send(command);

        //    return Ok();
        //}
    }
}