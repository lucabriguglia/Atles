using System;
using System.Threading.Tasks;
using Atles.Commands.Forums;
using Atles.Core;
using Atles.Domain;
using Atles.Domain.Rules.Forums;
using Atles.Models.Admin.Forums;
using Atles.Queries.Admin;
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
        public async Task<ActionResult> Index()
        {
            return await ProcessGet(new GetForumsIndex
            {
                SiteId = CurrentSite.Id
            });
        }

        [HttpGet("index-model/{categoryId}")]
        public async Task<ActionResult> Index(Guid categoryId)
        {
            return await ProcessGet(new GetForumsIndex
            {
                SiteId = CurrentSite.Id,
                CategoryId = categoryId
            });
        }

        [HttpGet("create")]
        public async Task<ActionResult> Create()
        {
            return await ProcessGet(new GetForumCreateForm
            {
                SiteId = CurrentSite.Id
            });
        }

        [HttpGet("create/{categoryId}")]
        public async Task<ActionResult> Create(Guid categoryId)
        {
            return await ProcessGet(new GetForumCreateForm
            {
                SiteId = CurrentSite.Id, 
                CategoryId = categoryId
            });
        }

        [HttpPost("save")]
        public async Task<ActionResult> Save(FormComponentModel.ForumModel model)
        {
            var command = new CreateForum
            {
                CategoryId = model.CategoryId,
                Name = model.Name,
                Slug = model.Slug,
                Description = model.Description,
                PermissionSetId = model.PermissionSetId == Guid.Empty ? (Guid?)null : model.PermissionSetId,
                SiteId = CurrentSite.Id,
                UserId = CurrentUser.Id
            };

            await _dispatcher.Send(command);

            return Ok();
        }

        [HttpGet("edit/{id}")]
        public async Task<ActionResult<FormComponentModel>> Edit(Guid id)
        {
            return await ProcessGet(new GetForumEditForm
            {
                SiteId = CurrentSite.Id, Id = id
            });
        }

        [HttpPost("update")]
        public async Task<ActionResult> Update(FormComponentModel.ForumModel model)
        {
            var command = new UpdateForum
            {
                ForumId = model.Id,
                CategoryId = model.CategoryId,
                Name = model.Name,
                Slug = model.Slug,
                Description = model.Description,
                PermissionSetId = model.PermissionSetId == Guid.Empty ? (Guid?)null : model.PermissionSetId,
                SiteId = CurrentSite.Id,
                UserId = CurrentUser.Id
            };

            await _dispatcher.Send(command);

            return Ok();
        }

        [HttpPost("move-up")]
        public async Task<ActionResult> MoveUp([FromBody] Guid id)
        {
            var command = new MoveForum
            {
                ForumId = id,
                Direction = DirectionType.Up,
                SiteId = CurrentSite.Id,
                UserId = CurrentUser.Id
            };

            await _dispatcher.Send(command);

            return Ok();
        }

        [HttpPost("move-down")]
        public async Task<ActionResult> MoveDown([FromBody] Guid id)
        {
            var command = new MoveForum
            {
                ForumId = id,
                Direction = DirectionType.Down,
                SiteId = CurrentSite.Id,
                UserId = CurrentUser.Id
            };

            await _dispatcher.Send(command);

            return Ok();
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var command = new DeleteForum
            {
                ForumId = id,
                SiteId = CurrentSite.Id,
                UserId = CurrentUser.Id
            };

            await _dispatcher.Send(command);

            return Ok();
        }

        [HttpGet("is-name-unique/{categoryId}/{name}")]
        public async Task<IActionResult> IsNameUnique(Guid categoryId, string name)
        {
            return await ProcessGet(new IsForumNameUnique
            {
                SiteId = CurrentSite.Id, 
                CategoryId = categoryId, 
                Name = name
            });
        }

        [HttpGet("is-name-unique/{categoryId}/{name}/{id}")]
        public async Task<IActionResult> IsNameUnique(Guid categoryId, string name, Guid id)
        {
            return await ProcessGet(new IsForumNameUnique
            {
                SiteId = CurrentSite.Id, 
                CategoryId = categoryId, 
                Name = name, 
                Id = id
            });
        }

        [HttpGet("is-slug-unique/{slug}")]
        public async Task<IActionResult> IsNameUnique(string slug)
        {
            return await ProcessGet(new IsForumSlugUnique 
            { 
                SiteId = CurrentSite.Id, 
                Slug = slug
            });
        }

        [HttpGet("is-slug-unique/{slug}/{id}")]
        public async Task<IActionResult> IsNameUnique(string slug, Guid id)
        {
            return await ProcessGet(new IsForumSlugUnique
            {
                SiteId = CurrentSite.Id, 
                Slug = slug, 
                Id = id
            });
        }
    }
}
