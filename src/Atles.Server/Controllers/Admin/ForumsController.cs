using System;
using System.Threading.Tasks;
using Atles.Domain;
using Atles.Domain.Forums.Commands;
using Atles.Domain.Forums.Rules;
using Atles.Models.Admin.Forums;
using Atles.Reporting.Admin.Forums;
using Atles.Server.Services;
using Microsoft.AspNetCore.Mvc;
using OpenCqrs.Commands;
using OpenCqrs.Queries;

namespace Atles.Server.Controllers.Admin
{
    [Route("api/admin/forums")]
    public class ForumsController : AdminControllerBase
    {
        private readonly IContextService _contextService;
        private readonly ICommandSender _commandSender;
        private readonly IQuerySender _querySender;

        public ForumsController(IContextService contextService,
            ICommandSender commandSender,
            IQuerySender querySender)
        {
            _contextService = contextService;
            _commandSender = commandSender;
            _querySender = querySender;
        }

        [HttpGet("index-model")]
        public async Task<IndexPageModel> Index()
        {
            var site = await _contextService.CurrentSiteAsync();

            return await _querySender.Send(new GetForumsIndex { SiteId = site.Id });
        }

        [HttpGet("index-model/{categoryId}")]
        public async Task<IndexPageModel> Index(Guid categoryId)
        {
            var site = await _contextService.CurrentSiteAsync();

            return await _querySender.Send(new GetForumsIndex { SiteId = site.Id, CategoryId = categoryId });
        }

        [HttpGet("create")]
        public async Task<FormComponentModel> Create()
        {
            var site = await _contextService.CurrentSiteAsync();

            return await _querySender.Send(new GetForumCreateForm { SiteId = site.Id });
        }

        [HttpGet("create/{categoryId}")]
        public async Task<FormComponentModel> Create(Guid categoryId)
        {
            var site = await _contextService.CurrentSiteAsync();

            return await _querySender.Send(new GetForumCreateForm { SiteId = site.Id, CategoryId = categoryId });
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

            var result = await _querySender.Send(new GetForumEditForm { SiteId = site.Id, Id = id });

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
            var isNameUnique = await _querySender.Send(new IsForumNameUnique { SiteId = site.Id, CategoryId = categoryId, Name = name });
            return Ok(isNameUnique);
        }

        [HttpGet("is-name-unique/{categoryId}/{name}/{id}")]
        public async Task<IActionResult> IsNameUnique(Guid categoryId, string name, Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var isNameUnique = await _querySender.Send(new IsForumNameUnique { SiteId = site.Id, CategoryId = categoryId, Name = name, Id = id });
            return Ok(isNameUnique);
        }

        [HttpGet("is-slug-unique/{slug}")]
        public async Task<IActionResult> IsNameUnique(string slug)
        {
            var site = await _contextService.CurrentSiteAsync();
            var isSlugUnique = await _querySender.Send(new IsForumSlugUnique { SiteId = site.Id, Slug = slug });
            return Ok(isSlugUnique);
        }

        [HttpGet("is-slug-unique/{slug}/{id}")]
        public async Task<IActionResult> IsNameUnique(string slug, Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var isSlugUnique = await _querySender.Send(new IsForumSlugUnique { SiteId = site.Id, Slug = slug, Id = id });
            return Ok(isSlugUnique);
        }
    }
}
