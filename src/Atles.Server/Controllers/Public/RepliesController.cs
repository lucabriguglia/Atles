using Atles.Commands.Posts;
using Atles.Core;
using Atles.Data;
using Atles.Domain;
using Atles.Models.Public;
using Atles.Queries.Public;
using Atles.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Atles.Server.Controllers.Public
{
    [Authorize]
    [Route("api/public/replies")]
    public class RepliesController : SiteControllerBase
    {
        private readonly ISecurityService _securityService;
        private readonly AtlesDbContext _dbContext;
        private readonly ILogger<RepliesController> _logger;
        private readonly IDispatcher _dispatcher;

        public RepliesController(ISecurityService securityService, 
            AtlesDbContext dbContext, 
            ILogger<RepliesController> logger,
            IDispatcher dispatcher) : base(dispatcher)
        {
            _securityService = securityService;
            _dbContext = dbContext;
            _logger = logger;
            _dispatcher = dispatcher;
        }

        [HttpPost("create-reply")]
        public async Task<ActionResult> CreateReply(TopicPageModel model)
        {
            // TODO: Refactoring

            var permissions = await _dispatcher.Get(new GetPermissions { SiteId = CurrentSite.Id, ForumId = model.Forum.Id });
            var canReply = _securityService.HasPermission(PermissionType.Reply, permissions.AsT0) && !CurrentUser.IsSuspended;

            if (!canReply)
            {
                _logger.LogWarning("Unauthorized access to create reply.");
                return Unauthorized();
            }

            var command = new CreateReply
            {
                ForumId = model.Forum.Id,
                TopicId = model.Topic.Id,
                Content = model.Post.Content,
                Status = PostStatusType.Published,
                SiteId = CurrentSite.Id,
                UserId = CurrentUser.Id
            };

            await _dispatcher.Send(command);

            return Ok();
        }

        [HttpPost("update-reply")]
        public async Task<ActionResult> UpdateReply(TopicPageModel model)
        {
            // TODO: Refactoring

            var command = new UpdateReply
            {
                ReplyId = model.Post.Id.Value,
                ForumId = model.Forum.Id,
                TopicId = model.Topic.Id,
                Content = model.Post.Content,
                Status = PostStatusType.Published,
                SiteId = CurrentSite.Id,
                UserId = CurrentUser.Id
            };

            var replyUserId = await _dbContext.Posts
                .Where(x =>
                    x.Id == command.ReplyId &&
                    x.TopicId == command.TopicId &&
                    x.Topic.ForumId == command.ForumId &&
                    x.Topic.Forum.Category.SiteId == command.SiteId &&
                    x.Status != PostStatusType.Deleted)
                .Select(x => x.CreatedBy)
                .FirstOrDefaultAsync();

            var permissions = await _dispatcher.Get(new GetPermissions { SiteId = CurrentSite.Id, ForumId = model.Forum.Id });
            var canEdit = _securityService.HasPermission(PermissionType.Edit, permissions.AsT0);
            var canModerate = _securityService.HasPermission(PermissionType.Moderate, permissions.AsT0);
            var authorized = (canEdit && replyUserId == CurrentUser.Id || canModerate) && !CurrentUser.IsSuspended;

            if (!authorized)
            {
                _logger.LogWarning("Unauthorized access to update reply.");
                return Unauthorized();
            }

            await _dispatcher.Send(command);

            return Ok();
        }

        [HttpPost("set-reply-as-answer/{forumId}/{topicId}/{replyId}")]
        public async Task<ActionResult> SetReplyAsAnswer(Guid forumId, Guid topicId, Guid replyId, [FromBody] bool isAnswer)
        {
            // TODO: Refactoring

            var command = new SetReplyAsAnswer
            {
                ReplyId = replyId,
                TopicId = topicId,
                ForumId = forumId,
                SiteId = CurrentSite.Id,
                UserId = CurrentUser.Id,
                IsAnswer = isAnswer
            };

            var topicUserId = await _dbContext.Posts
                .Where(x =>
                    x.Id == command.ReplyId &&
                    x.TopicId == command.TopicId &&
                    x.Topic.ForumId == command.ForumId &&
                    x.Topic.Forum.Category.SiteId == command.SiteId &&
                    x.Status == PostStatusType.Published)
                .Select(x => x.Topic.CreatedBy)
                .FirstOrDefaultAsync();

            var permissions = await _dispatcher.Get(new GetPermissions { SiteId = CurrentSite.Id, ForumId = forumId });
            var canEdit = _securityService.HasPermission(PermissionType.Edit, permissions.AsT0);
            var canModerate = _securityService.HasPermission(PermissionType.Moderate, permissions.AsT0);
            var authorized = (canEdit && topicUserId == CurrentUser.Id || canModerate) && !CurrentUser.IsSuspended;

            if (!authorized)
            {
                _logger.LogWarning("Unauthorized access to set reply as answer.");
                return Unauthorized();
            }

            await _dispatcher.Send(command);

            return Ok();
        }

        [HttpDelete("delete-reply/{forumId}/{topicId}/{replyId}")]
        public async Task<ActionResult> DeleteReply(Guid forumId, Guid topicId, Guid replyId)
        {
            // TODO: Refactoring

            var command = new DeleteReply
            {
                ReplyId = replyId,
                TopicId = topicId,
                ForumId = forumId,
                SiteId = CurrentSite.Id,
                UserId = CurrentUser.Id
            };

            var replyUserId = await _dbContext.Posts
                .Where(x =>
                    x.Id == command.ReplyId &&
                    x.TopicId == command.TopicId &&
                    x.Topic.ForumId == command.ForumId &&
                    x.Topic.Forum.Category.SiteId == command.SiteId &&
                    x.Status != PostStatusType.Deleted)
                .Select(x => x.CreatedBy)
                .FirstOrDefaultAsync();

            var permissions = await _dispatcher.Get(new GetPermissions { SiteId = CurrentSite.Id, ForumId = forumId });
            var canDelete = _securityService.HasPermission(PermissionType.Delete, permissions.AsT0);
            var canModerate = _securityService.HasPermission(PermissionType.Moderate, permissions.AsT0);
            var authorized = (canDelete && replyUserId == CurrentUser.Id || canModerate) && !CurrentUser.IsSuspended;

            if (!authorized)
            {
                _logger.LogWarning("Unauthorized access to delete reply.");
                return Unauthorized();
            }

            await _dispatcher.Send(command);

            return Ok();
        }
    }
}
