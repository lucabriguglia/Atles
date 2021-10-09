using System;
using System.Threading.Tasks;
using Atles.Domain.PermissionSets;
using Atles.Domain.PermissionSets.Commands;
using Atles.Models.Admin.PermissionSets;
using Atles.Server.Services;
using Microsoft.AspNetCore.Mvc;
using OpenCqrs.Commands;
using OpenCqrs.Queries;

namespace Atles.Server.Controllers.Admin
{
    [Route("api/admin/permission-sets")]
    public class PermissionSetsController : AdminControllerBase
    {
        private readonly IContextService _contextService;
        private readonly IPermissionSetRules _permissionSetRules;
        private readonly IPermissionSetModelBuilder _modelBuilder;
        private readonly ICommandSender _commandSender;
        private readonly IQuerySender _querySender;

        public PermissionSetsController(IContextService contextService,
            IPermissionSetRules permissionSetRules,
            IPermissionSetModelBuilder modelBuilder,
            ICommandSender commandSender,
            IQuerySender querySender)
        {
            _contextService = contextService;
            _permissionSetRules = permissionSetRules;
            _modelBuilder = modelBuilder;
            _commandSender = commandSender;
            _querySender = querySender;
        }

        [HttpGet("list")]
        public async Task<IndexPageModel> List()
        {
            var site = await _contextService.CurrentSiteAsync();

            return await _modelBuilder.BuildIndexPageModelAsync(site.Id);
        }

        [HttpGet("create")]
        public async Task<FormComponentModel> Create()
        {
            var site = await _contextService.CurrentSiteAsync();

            return await _modelBuilder.BuildCreateFormModelAsync(site.Id);
        }

        [HttpPost("save")]
        public async Task<ActionResult> Save(FormComponentModel.PermissionSetModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var user = await _contextService.CurrentUserAsync();

            var command = new CreatePermissionSet
            {
                Name = model.Name,
                Permissions = model.Permissions.ToPermissionCommands(),
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
        public async Task<ActionResult> Update(FormComponentModel.PermissionSetModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var user = await _contextService.CurrentUserAsync();

            var command = new UpdatePermissionSet
            {
                Id = model.Id,
                Name = model.Name,
                Permissions = model.Permissions.ToPermissionCommands(),
                SiteId = site.Id,
                UserId = user.Id
            };

            await _commandSender.Send(command);

            return Ok();
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var user = await _contextService.CurrentUserAsync();

            var command = new DeletePermissionSet
            {
                Id = id,
                SiteId = site.Id,
                UserId = user.Id
            };

            await _commandSender.Send(command);

            return Ok();
        }

        [HttpGet("is-name-unique/{name}")]
        public async Task<IActionResult> IsNameUnique(string name)
        {
            var site = await _contextService.CurrentSiteAsync();
            var isNameUnique = await _permissionSetRules.IsNameUniqueAsync(site.Id, name);
            return Ok(isNameUnique);
        }

        [HttpGet("is-name-unique/{name}/{id}")]
        public async Task<IActionResult> IsNameUnique(string name, Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var isNameUnique = await _permissionSetRules.IsNameUniqueAsync(site.Id, name, id);
            return Ok(isNameUnique);
        }
    }
}
