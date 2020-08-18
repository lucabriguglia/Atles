using System;
using System.Linq;
using System.Threading.Tasks;
using Atlify.Data;
using Atlify.Models.Public;
using Atlify.Models.Public.Topics;
using Atlify.Server.Services;
using Atlify.Domain;
using Atlify.Domain.PermissionSets;
using Atlify.Domain.Posts;
using Atlify.Domain.Posts.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Atlify.Server.Controllers.Public
{
    [Authorize]
    [Route("api/public/replies")]
    [ApiController]
    public class RepliesController : ControllerBase
    {
        private readonly IContextService _contextService;
        private readonly IReplyService _replyService;
        private readonly ISecurityService _securityService;
        private readonly AtlifyDbContext _dbContext;
        private readonly IPermissionModelBuilder _permissionModelBuilder;
        private readonly ILogger<RepliesController> _logger;

        public RepliesController(IContextService contextService,
            IReplyService replyService, 
            ISecurityService securityService, 
            AtlifyDbContext dbContext, 
            IPermissionModelBuilder permissionModelBuilder, 
            ILogger<RepliesController> logger)
        {
            _contextService = contextService;
            _replyService = replyService;
            _securityService = securityService;
            _dbContext = dbContext;
            _permissionModelBuilder = permissionModelBuilder;
            _logger = logger;
        }

        [HttpPost("create-reply")]
        public async Task<ActionResult> CreateReply(TopicPageModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var permissions = await _permissionModelBuilder.BuildPermissionModelsByForumId(site.Id, model.Forum.Id);
            var canReply = _securityService.HasPermission(PermissionType.Reply, permissions) && !member.IsSuspended;

            if (!canReply)
            {
                _logger.LogWarning("Unauthorized access to create reply.", new
                {
                    SiteId = site.Id,
                    ForumId = model.Forum?.Id,
                    TopicId = model.Topic?.Id,
                    User = User.Identity.Name
                });

                return Unauthorized();
            }

            var command = new CreateReply
            {
                ForumId = model.Forum.Id,
                TopicId = model.Topic.Id,
                Content = model.Post.Content,
                Status = StatusType.Published,
                SiteId = site.Id,
                MemberId = member.Id
            };

            await _replyService.CreateAsync(command);

            return Ok();
        }

        [HttpPost("update-reply")]
        public async Task<ActionResult> UpdateReply(TopicPageModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new UpdateReply
            {
                Id = model.Post.Id.Value,
                ForumId = model.Forum.Id,
                TopicId = model.Topic.Id,
                Content = model.Post.Content,
                Status = StatusType.Published,
                SiteId = site.Id,
                MemberId = member.Id
            };

            var replyMemberId = await _dbContext.Posts
                .Where(x =>
                    x.Id == command.Id &&
                    x.TopicId == command.TopicId &&
                    x.Topic.ForumId == command.ForumId &&
                    x.Topic.Forum.Category.SiteId == command.SiteId &&
                    x.Status != StatusType.Deleted)
                .Select(x => x.MemberId)
                .FirstOrDefaultAsync();

            var permissions = await _permissionModelBuilder.BuildPermissionModelsByForumId(site.Id, model.Forum.Id);
            var canEdit = _securityService.HasPermission(PermissionType.Edit, permissions) && !member.IsSuspended;
            var canModerate = _securityService.HasPermission(PermissionType.Moderate, permissions) && !member.IsSuspended;
            var authorized = canEdit && replyMemberId == member.Id || canModerate;

            if (!authorized)
            {
                _logger.LogWarning("Unauthorized access to update reply.", new
                {
                    SiteId = site.Id,
                    ForumId = model.Forum?.Id,
                    TopicId = model.Topic?.Id,
                    ReplyId = model.Post?.Id,
                    User = User.Identity.Name
                });

                return Unauthorized();
            }

            await _replyService.UpdateAsync(command);

            return Ok();
        }

        [HttpDelete("delete-reply/{forumId}/{topicId}/{replyId}")]
        public async Task<ActionResult> DeleteReply(Guid forumId, Guid topicId, Guid replyId)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new DeleteReply
            {
                Id = replyId,
                TopicId = topicId,
                ForumId = forumId,
                SiteId = site.Id,
                MemberId = member.Id
            };

            var replyMemberId = await _dbContext.Posts
                .Where(x =>
                    x.Id == command.Id &&
                    x.TopicId == command.TopicId &&
                    x.Topic.ForumId == command.ForumId &&
                    x.Topic.Forum.Category.SiteId == command.SiteId &&
                    x.Status != StatusType.Deleted)
                .Select(x => x.MemberId)
                .FirstOrDefaultAsync();

            var permissions = await _permissionModelBuilder.BuildPermissionModelsByForumId(site.Id, forumId);
            var canDelete = _securityService.HasPermission(PermissionType.Delete, permissions) && !member.IsSuspended;
            var canModerate = _securityService.HasPermission(PermissionType.Moderate, permissions) && !member.IsSuspended;
            var authorized = canDelete && replyMemberId == member.Id || canModerate;

            if (!authorized)
            {
                _logger.LogWarning("Unauthorized access to delete reply.", new
                {
                    SiteId = site.Id,
                    ForumId = forumId,
                    TopicId = topicId,
                    ReplyId = replyId,
                    User = User.Identity.Name
                });

                return Unauthorized();
            }

            await _replyService.DeleteAsync(command);

            return Ok();
        }
    }
}
