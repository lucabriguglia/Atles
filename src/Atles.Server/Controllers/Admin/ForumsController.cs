using System;
using System.Threading.Tasks;
using Atles.Domain;
using Atles.Domain.Forums;
using Atles.Domain.Forums.Commands;
using Atles.Infrastructure.Commands;
using Atles.Infrastructure.Queries;
using Atles.Models.Admin.Forums;
using Atles.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Controllers.Admin
{
    [Route("api/admin/forums")]
    public class ForumsController : AdminControllerBase
    {
        private readonly IContextService _contextService;
        private readonly IForumRules _forumRules;
        private readonly IForumModelBuilder _modelBuilder;
        private readonly ICommandSender _commandSender;
        private readonly IQuerySender _querySender;

        public ForumsController(IContextService contextService,
            IForumRules forumRules,
            IForumModelBuilder modelBuilder,
            ICommandSender commandSender,
            IQuerySender querySender)
        {
            _contextService = contextService;
            _forumRules = forumRules;
            _modelBuilder = modelBuilder;
            _commandSender = commandSender;
            _querySender = querySender;
        }

        [HttpGet("index-model")]
        public async Task<IndexPageModel> Index()
        {
            var site = await _contextService.CurrentSiteAsync();

            return await _modelBuilder.BuildIndexPageModelAsync(site.Id);
        }

        [HttpGet("index-model/{categoryId}")]
        public async Task<IndexPageModel> Index(Guid categoryId)
        {
            var site = await _contextService.CurrentSiteAsync();

            return await _modelBuilder.BuildIndexPageModelAsync(site.Id, categoryId);
        }

        [HttpGet("create")]
        public async Task<FormComponentModel> Create()
        {
            var site = await _contextService.CurrentSiteAsync();

            return await _modelBuilder.BuildCreateFormModelAsync(site.Id);
        }

        [HttpGet("create/{categoryId}")]
        public async Task<FormComponentModel> Create(Guid categoryId)
        {
            var site = await _contextService.CurrentSiteAsync();

            return await _modelBuilder.BuildCreateFormModelAsync(site.Id, categoryId);
        }

        [HttpPost("save")]
        public async Task<ActionResult> Save(FormComponentModel.ForumModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var user = await _contextService.CurrentUserAsync();

            var command = new CreateForum
            {
                CategoryId = model.CategoryId,
                Name = model.Name,
                Slug = model.Slug,
                Description = model.Description,
                PermissionSetId = model.PermissionSetId == Guid.Empty ? (Guid?)null : model.PermissionSetId,
                SiteId = site.Id,
                UserId = user.Id
            };

            await _commandSender.Send(command);

            return Ok();
        }

        [HttpGet("edit/{id}")]
        public async Task<ActionResult<FormComponentModel>> Edit(Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();

            var result = await _modelBuilder.BuildEditFormModelAsync(site.Id, id);

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost("update")]
        public async Task<ActionResult> Update(FormComponentModel.ForumModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var user = await _contextService.CurrentUserAsync();

            var command = new UpdateForum
            {
                Id = model.Id,
                CategoryId = model.CategoryId,
                Name = model.Name,
                Slug = model.Slug,
                Description = model.Description,
                PermissionSetId = model.PermissionSetId == Guid.Empty ? (Guid?)null : model.PermissionSetId,
                SiteId = site.Id,
                UserId = user.Id
            };

            await _commandSender.Send(command);

            return Ok();
        }

        [HttpPost("move-up")]
        public async Task<ActionResult> MoveUp([FromBody] Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var user = await _contextService.CurrentUserAsync();

            var command = new MoveForum
            {
                Id = id,
                SiteId = site.Id,
                UserId = user.Id,
                Direction = Direction.Up
            };

            await _commandSender.Send(command);

            return Ok();
        }

        [HttpPost("move-down")]
        public async Task<ActionResult> MoveDown([FromBody] Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var user = await _contextService.CurrentUserAsync();

            var command = new MoveForum
            {
                Id = id,
                SiteId = site.Id,
                UserId = user.Id,
                Direction = Direction.Down
            };

            await _commandSender.Send(command);

            return Ok();
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var user = await _contextService.CurrentUserAsync();

            var command = new DeleteForum
            {
                Id = id,
                SiteId = site.Id,
                UserId = user.Id
            };

            await _commandSender.Send(command);

            return Ok();
        }

        [HttpGet("is-name-unique/{categoryId}/{name}")]
        public async Task<IActionResult> IsNameUnique(Guid categoryId, string name)
        {
            var site = await _contextService.CurrentSiteAsync();
            var isNameUnique = await _forumRules.IsNameUniqueAsync(site.Id, categoryId, name);
            return Ok(isNameUnique);
        }

        [HttpGet("is-name-unique/{categoryId}/{name}/{id}")]
        public async Task<IActionResult> IsNameUnique(Guid categoryId, string name, Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var isNameUnique = await _forumRules.IsNameUniqueAsync(site.Id, categoryId, name, id);
            return Ok(isNameUnique);
        }

        [HttpGet("is-slug-unique/{slug}")]
        public async Task<IActionResult> IsNameUnique(string slug)
        {
            var site = await _contextService.CurrentSiteAsync();
            var isSlugUnique = await _forumRules.IsSlugUniqueAsync(site.Id, slug);
            return Ok(isSlugUnique);
        }

        [HttpGet("is-slug-unique/{slug}/{id}")]
        public async Task<IActionResult> IsNameUnique(string slug, Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var isSlugUnique = await _forumRules.IsSlugUniqueAsync(site.Id, slug, id);
            return Ok(isSlugUnique);
        }
    }
}
