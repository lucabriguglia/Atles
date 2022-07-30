using System;
using System.Threading.Tasks;
using Atles.Commands.Forums;
using Atles.Core;
using Atles.Domain;
using Atles.Domain.Rules.Forums;
using Atles.Reporting.Models.Admin.Forums;
using Atles.Reporting.Models.Admin.Forums.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Controllers.Admin
{
    [Route("api/admin/forums")]
    public class ForumsController : AdminControllerBase
    {
        private readonly IDispatcher _dispatcher;

        public ForumsController(IDispatcher dispatcher) : base(dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [HttpGet("index-model")]
        public async Task<IndexPageModel> Index()
        {
            var site = await CurrentSite();

            return await _dispatcher.Get(new GetForumsIndex { SiteId = site.Id });
        }

        [HttpGet("index-model/{categoryId}")]
        public async Task<IndexPageModel> Index(Guid categoryId)
        {
            var site = await CurrentSite();

            return await _dispatcher.Get(new GetForumsIndex { SiteId = site.Id, CategoryId = categoryId });
        }

        [HttpGet("create")]
        public async Task<FormComponentModel> Create()
        {
            var site = await CurrentSite();

            return await _dispatcher.Get(new GetForumCreateForm { SiteId = site.Id });
        }

        [HttpGet("create/{categoryId}")]
        public async Task<FormComponentModel> Create(Guid categoryId)
        {
            var site = await CurrentSite();

            return await _dispatcher.Get(new GetForumCreateForm { SiteId = site.Id, CategoryId = categoryId });
        }

        [HttpPost("save")]
        public async Task<ActionResult> Save(FormComponentModel.ForumModel model)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

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

            await _dispatcher.Send(command);

            return Ok();
        }

        [HttpGet("edit/{id}")]
        public async Task<ActionResult<FormComponentModel>> Edit(Guid id)
        {
            var site = await CurrentSite();

            var result = await _dispatcher.Get(new GetForumEditForm { SiteId = site.Id, Id = id });

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost("update")]
        public async Task<ActionResult> Update(FormComponentModel.ForumModel model)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var command = new UpdateForum
            {
                ForumId = model.Id,
                CategoryId = model.CategoryId,
                Name = model.Name,
                Slug = model.Slug,
                Description = model.Description,
                PermissionSetId = model.PermissionSetId == Guid.Empty ? (Guid?)null : model.PermissionSetId,
                SiteId = site.Id,
                UserId = user.Id
            };

            await _dispatcher.Send(command);

            return Ok();
        }

        [HttpPost("move-up")]
        public async Task<ActionResult> MoveUp([FromBody] Guid id)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var command = new MoveForum
            {
                ForumId = id,
                SiteId = site.Id,
                UserId = user.Id,
                Direction = DirectionType.Up
            };

            await _dispatcher.Send(command);

            return Ok();
        }

        [HttpPost("move-down")]
        public async Task<ActionResult> MoveDown([FromBody] Guid id)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var command = new MoveForum
            {
                ForumId = id,
                SiteId = site.Id,
                UserId = user.Id,
                Direction = DirectionType.Down
            };

            await _dispatcher.Send(command);

            return Ok();
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var command = new DeleteForum
            {
                ForumId = id,
                SiteId = site.Id,
                UserId = user.Id
            };

            await _dispatcher.Send(command);

            return Ok();
        }

        [HttpGet("is-name-unique/{categoryId}/{name}")]
        public async Task<IActionResult> IsNameUnique(Guid categoryId, string name)
        {
            var site = await CurrentSite();
            var isNameUnique = await _dispatcher.Get(new IsForumNameUnique { SiteId = site.Id, CategoryId = categoryId, Name = name });
            return Ok(isNameUnique);
        }

        [HttpGet("is-name-unique/{categoryId}/{name}/{id}")]
        public async Task<IActionResult> IsNameUnique(Guid categoryId, string name, Guid id)
        {
            var site = await CurrentSite();
            var isNameUnique = await _dispatcher.Get(new IsForumNameUnique { SiteId = site.Id, CategoryId = categoryId, Name = name, Id = id });
            return Ok(isNameUnique);
        }

        [HttpGet("is-slug-unique/{slug}")]
        public async Task<IActionResult> IsNameUnique(string slug)
        {
            var site = await CurrentSite();
            var isSlugUnique = await _dispatcher.Get(new IsForumSlugUnique { SiteId = site.Id, Slug = slug });
            return Ok(isSlugUnique);
        }

        [HttpGet("is-slug-unique/{slug}/{id}")]
        public async Task<IActionResult> IsNameUnique(string slug, Guid id)
        {
            var site = await CurrentSite();
            var isSlugUnique = await _dispatcher.Get(new IsForumSlugUnique { SiteId = site.Id, Slug = slug, Id = id });
            return Ok(isSlugUnique);
        }
    }
}
