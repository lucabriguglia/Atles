using System;
using System.Threading.Tasks;
using Atles.Domain.PermissionSets;
using Atles.Models;
using Atles.Models.Public.Forums;
using Atles.Reporting.Public.Queries;
using Atles.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenCqrs;

namespace Atles.Server.Controllers.Public
{
    [Route("api/public/forums")]
    [ApiController]
    public class ForumsController : ControllerBase
    {
        private readonly IContextService _contextService;
        private readonly ISecurityService _securityService;
        private readonly ILogger<ForumsController> _logger;
        private readonly ISender _sender;

        public ForumsController(IContextService contextService,
            ISecurityService securityService,
            ILogger<ForumsController> logger,
            ISender sender)
        {
            _contextService = contextService;
            _securityService = securityService;
            _logger = logger;
            _sender = sender;
        }

        [HttpGet("{slug}")]
        public async Task<ActionResult<ForumPageModel>> Forum(string slug, [FromQuery] int? page = 1, [FromQuery] string search = null)
        {
            var site = await _contextService.CurrentSiteAsync();
            var user = await _contextService.CurrentUserAsync();

            var model = await _sender.Send(new GetForumPage { SiteId = site.Id, Slug = slug, Options = new QueryOptions(page, search) });

            if (model == null)
            {
                _logger.LogWarning("Forum not found.", new
                {
                    SiteId = site.Id,
                    ForumSlug = slug,
                    User = User.Identity.Name
                });

                return NotFound();
            }

            var permissions = await _sender.Send(new GetPermissions { SiteId = site.Id, ForumId = model.Forum.Id });

            var canViewForum = _securityService.HasPermission(PermissionType.ViewForum, permissions);
            var canViewTopics = _securityService.HasPermission(PermissionType.ViewTopics, permissions);

            if (!canViewForum || !canViewTopics)
            {
                _logger.LogWarning("Unauthorized access to forum.", new
                {
                    SiteId = site.Id,
                    ForumSlug = slug,
                    User = User.Identity.Name
                });

                return Unauthorized();
            }

            model.CanRead = _securityService.HasPermission(PermissionType.Read, permissions);
            model.CanStart = _securityService.HasPermission(PermissionType.Start, permissions) && !user.IsSuspended;

            return model;
        }

        [HttpGet("{id}/topics")]
        public async Task<ActionResult<PaginatedData<ForumPageModel.TopicModel>>> Topics(Guid id, [FromQuery] int? page = 1, [FromQuery] string search = null)
        {
            var site = await _contextService.CurrentSiteAsync();

            var permissions = await _sender.Send(new GetPermissions { SiteId = site.Id, ForumId = id });

            var canViewForum = _securityService.HasPermission(PermissionType.ViewForum, permissions);
            var canViewTopics = _securityService.HasPermission(PermissionType.ViewTopics, permissions);

            if (!canViewForum || !canViewTopics)
            {
                _logger.LogWarning("Unauthorized access to forum topics.", new
                {
                    SiteId = site.Id,
                    ForumId = id,
                    User = User.Identity.Name
                });

                return Unauthorized();
            }

            var result = await _sender.Send(new GetForumPageTopics { SiteId = site.Id, ForumId = id, Options = new QueryOptions(page, search) });

            return result;
        }
    }
}
