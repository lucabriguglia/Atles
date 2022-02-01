using System;
using System.Threading.Tasks;
using Atles.Domain.Commands;
using Atles.Domain.Rules;
using Atles.Infrastructure;
using Atles.Reporting.Models.Admin.PermissionSets;
using Atles.Reporting.Models.Admin.PermissionSets.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Controllers.Admin
{
    [Route("api/admin/permission-sets")]
    public class PermissionSetsController : AdminControllerBase
    {
        private readonly IDispatcher _dispatcher;

        public PermissionSetsController(IDispatcher dispatcher) : base(dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [HttpGet("list")]
        public async Task<IndexPageModel> List()
        {
            var site = await CurrentSite();

            return await _dispatcher.Get(new GetPermissionSetsIndex { SiteId = site.Id });
        }

        [HttpGet("create")]
        public async Task<FormComponentModel> Create()
        {
            var site = await CurrentSite();

            return await _dispatcher.Get(new GetPermissionSetCreateForm { SiteId = site.Id });
        }

        [HttpPost("save")]
        public async Task<ActionResult> Save(FormComponentModel.PermissionSetModel model)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var command = new CreatePermissionSet
            {
                Name = model.Name,
                Permissions = model.Permissions.ToPermissionCommands(),
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

            var result = await _dispatcher.Get(new GetPermissionSetEditForm { SiteId = site.Id, Id = id });

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost("update")]
        public async Task<ActionResult> Update(FormComponentModel.PermissionSetModel model)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var command = new UpdatePermissionSet
            {
                Id = model.Id,
                Name = model.Name,
                Permissions = model.Permissions.ToPermissionCommands(),
                SiteId = site.Id,
                UserId = user.Id
            };

            await _dispatcher.Send(command);

            return Ok();
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var command = new DeletePermissionSet
            {
                Id = id,
                SiteId = site.Id,
                UserId = user.Id
            };

            await _dispatcher.Send(command);

            return Ok();
        }

        [HttpGet("is-name-unique/{name}")]
        public async Task<IActionResult> IsNameUnique(string name)
        {
            var site = await CurrentSite();
            var query = new IsPermissionSetNameUnique { SiteId = site.Id, Name = name };
            var isNameUnique = await _dispatcher.Get(query);
            return Ok(isNameUnique);
        }

        [HttpGet("is-name-unique/{name}/{id}")]
        public async Task<IActionResult> IsNameUnique(string name, Guid id)
        {
            var site = await CurrentSite();
            var query = new IsPermissionSetNameUnique { SiteId = site.Id, Name = name, Id = id };
            var isNameUnique = await _dispatcher.Get(query);
            return Ok(isNameUnique);
        }
    }
}
