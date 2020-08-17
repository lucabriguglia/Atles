using System;
using System.Threading.Tasks;
using Atlas.Domain.PermissionSets;
using Atlas.Models;
using Atlas.Models.Public;
using Atlas.Models.Public.Forums;
using Atlas.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atlas.Server.Controllers.Public
{
    [Route("api/public/forums")]
    [ApiController]
    public class ForumsController : ControllerBase
    {
        private readonly IContextService _contextService;
        private readonly IForumModelBuilder _modelBuilder;
        private readonly ISecurityService _securityService;
        private readonly IPermissionModelBuilder _permissionModelBuilder;
        private readonly ILogger<ForumsController> _logger;

        public ForumsController(IContextService contextService,
            IForumModelBuilder modelBuilder,
            ISecurityService securityService,
            IPermissionModelBuilder permissionModelBuilder, 
            ILogger<ForumsController> logger)
        {
            _contextService = contextService;
            _modelBuilder = modelBuilder;
            _securityService = securityService;
            _permissionModelBuilder = permissionModelBuilder;
            _logger = logger;
        }

        [HttpGet("{slug}")]
        public async Task<ActionResult<ForumPageModel>> Forum(string slug, [FromQuery] int? page = 1, [FromQuery] string search = null)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var model = await _modelBuilder.BuildForumPageModelAsync(site.Id, slug, new QueryOptions(search, page));

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

            var permissions = await _permissionModelBuilder.BuildPermissionModelsByForumId(site.Id, model.Forum.Id);

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
            model.CanStart = _securityService.HasPermission(PermissionType.Start, permissions) && !member.IsSuspended;

            return model;
        }

        [HttpGet("{id}/topics")]
        public async Task<ActionResult<PaginatedData<ForumPageModel.TopicModel>>> Topics(Guid id, [FromQuery] int? page = 1, [FromQuery] string search = null)
        {
            var site = await _contextService.CurrentSiteAsync();

            var permissions = await _permissionModelBuilder.BuildPermissionModelsByForumId(site.Id, id);

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

            var result = await _modelBuilder.BuildForumPageModelTopicsAsync(id, new QueryOptions(search, page));

            return result;
        }
    }
}
