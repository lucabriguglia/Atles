using System;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Data;
using Atlas.Domain;
using Atlas.Domain.PermissionSets;
using Atlas.Domain.Posts;
using Atlas.Domain.Posts.Commands;
using Atlas.Models;
using Atlas.Models.Public;
using Atlas.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Server.Controllers.Public
{
    [Route("api/public/topics")]
    [ApiController]
    public class TopicsController : ControllerBase
    {
        private readonly IContextService _contextService;
        private readonly IPublicModelBuilder _modelBuilder;
        private readonly ITopicService _topicService;
        private readonly ISecurityService _securityService;
        private readonly AtlasDbContext _dbContext;
        private readonly IPermissionModelBuilder _permissionModelBuilder;

        public TopicsController(IContextService contextService, 
            IPublicModelBuilder modelBuilder, 
            ITopicService topicService,
            ISecurityService securityService, 
            AtlasDbContext dbContext, 
            IPermissionModelBuilder permissionModelBuilder)
        {
            _contextService = contextService;
            _modelBuilder = modelBuilder;
            _topicService = topicService;
            _securityService = securityService;
            _dbContext = dbContext;
            _permissionModelBuilder = permissionModelBuilder;
        }

        [HttpGet("{forumId}/{topicId}")]
        public async Task<ActionResult<TopicPageModel>> Topic(Guid forumId, Guid topicId, [FromQuery] int? page = 1, [FromQuery] string search = null)
        {
            var site = await _contextService.CurrentSiteAsync();

            var permissions = await _permissionModelBuilder.BuildPermissionModelsByForumId(site.Id, forumId);

            var canRead = _securityService.HasPermission(PermissionType.Read, permissions);

            if (!canRead)
            {
                return Unauthorized();
            }

            var model = await _modelBuilder.BuildTopicPageModelAsync(site.Id, forumId, topicId, new QueryOptions(search, page));

            if (model == null)
            {
                return NotFound();
            }

            model.CanEdit = _securityService.HasPermission(PermissionType.Edit, permissions);
            model.CanReply = _securityService.HasPermission(PermissionType.Reply, permissions);
            model.CanDelete = _securityService.HasPermission(PermissionType.Delete, permissions);
            model.CanModerate = _securityService.HasPermission(PermissionType.Moderate, permissions);

            return model;
        }

        [HttpGet("{forumId}/{topicId}/replies")]
        public async Task<ActionResult<PaginatedData<TopicPageModel.ReplyModel>>> Replies(Guid forumId, Guid topicId, [FromQuery] int? page = 1, [FromQuery] string search = null)
        {
            var site = await _contextService.CurrentSiteAsync();

            var permissions = await _permissionModelBuilder.BuildPermissionModelsByForumId(site.Id, forumId);

            var canRead = _securityService.HasPermission(PermissionType.Read, permissions);

            if (!canRead)
            {
                return Unauthorized();
            }

            var result = await _modelBuilder.BuildTopicPageModelRepliesAsync(topicId, new QueryOptions(search, page));

            return result;
        }

        [Authorize]
        [HttpGet("{forumId}/new-topic")]
        public async Task<ActionResult<PostPageModel>> NewTopic(Guid forumId)
        {
            var site = await _contextService.CurrentSiteAsync();

            var model = await _modelBuilder.BuildNewPostPageModelAsync(site.Id, forumId);

            if (model == null)
            {
                return NotFound();
            }

            var permissions = await _permissionModelBuilder.BuildPermissionModelsByForumId(site.Id, model.Forum.Id);
            var canPost = _securityService.HasPermission(PermissionType.Start, permissions);

            if (!canPost)
            {
                return Unauthorized();
            }

            return model;
        }

        [Authorize]
        [HttpGet("{forumId}/edit-topic/{topicId}")]
        public async Task<ActionResult<PostPageModel>> EditTopic(Guid forumId, Guid topicId)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var model = await _modelBuilder.BuildEditPostPageModelAsync(site.Id, forumId, topicId);

            if (model == null)
            {
                return NotFound();
            }

            var permissions = await _permissionModelBuilder.BuildPermissionModelsByForumId(site.Id, forumId);
            var canEdit = _securityService.HasPermission(PermissionType.Edit, permissions);
            var canModerate = _securityService.HasPermission(PermissionType.Moderate, permissions);
            var authorized = canEdit && model.Topic.MemberId == member.Id || canModerate;

            if (!authorized)
            {
                return Unauthorized();
            }

            return model;
        }

        [Authorize]
        [HttpPost("create-topic")]
        public async Task<ActionResult> CreateTopic(PostPageModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var permissions = await _permissionModelBuilder.BuildPermissionModelsByForumId(site.Id, model.Forum.Id);
            var canPost = _securityService.HasPermission(PermissionType.Start, permissions);

            if (!canPost)
            {
                return Unauthorized();
            }

            var command = new CreateTopic
            {
                ForumId = model.Forum.Id,
                Title = model.Topic.Title,
                Content = model.Topic.Content,
                Status = StatusType.Published,
                SiteId = site.Id,
                MemberId = member.Id
            };

            await _topicService.CreateAsync(command);

            return Ok(command.Id);
        }

        [Authorize]
        [HttpPost("update-topic")]
        public async Task<ActionResult> UpdateTopic(PostPageModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new UpdateTopic
            {
                Id = model.Topic.Id,
                ForumId = model.Forum.Id,
                Title = model.Topic.Title,
                Content = model.Topic.Content,
                Status = StatusType.Published,
                SiteId = site.Id,
                MemberId = member.Id
            };

            var topicMemberId = await _dbContext.Posts
                .Where(x =>
                    x.Id == command.Id &&
                    x.TopicId == null &&
                    x.ForumId == command.ForumId &&
                    x.Forum.Category.SiteId == command.SiteId &&
                    x.Status != StatusType.Deleted)
                .Select(x => x.MemberId)
                .FirstOrDefaultAsync();

            var permissions = await _permissionModelBuilder.BuildPermissionModelsByForumId(site.Id, model.Forum.Id);
            var canEdit = _securityService.HasPermission(PermissionType.Edit, permissions);
            var canModerate = _securityService.HasPermission(PermissionType.Moderate, permissions);
            var authorized = canEdit && topicMemberId == member.Id || canModerate;

            if (!authorized)
            {
                return Unauthorized();
            }

            await _topicService.UpdateAsync(command);

            return Ok(command.Id);
        }

        [Authorize]
        [HttpDelete("delete-topic/{forumId}/{topicId}")]
        public async Task<ActionResult> DeleteTopic(Guid forumId, Guid topicId)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new DeleteTopic
            {
                Id = topicId,
                ForumId = forumId,
                SiteId = site.Id,
                MemberId = member.Id
            };

            var topicMemberId = await _dbContext.Posts
                .Where(x =>
                    x.Id == command.Id &&
                    x.TopicId == null &&
                    x.ForumId == command.ForumId &&
                    x.Forum.Category.SiteId == command.SiteId &&
                    x.Status != StatusType.Deleted)
                .Select(x => x.MemberId)
                .FirstOrDefaultAsync();

            var permissions = await _permissionModelBuilder.BuildPermissionModelsByForumId(site.Id, forumId);
            var canDelete = _securityService.HasPermission(PermissionType.Delete, permissions);
            var canModerate = _securityService.HasPermission(PermissionType.Moderate, permissions);
            var authorized = canDelete && topicMemberId == member.Id || canModerate;

            if (!authorized)
            {
                return Unauthorized();
            }

            await _topicService.DeleteAsync(command);

            return Ok();
        }
    }
}
