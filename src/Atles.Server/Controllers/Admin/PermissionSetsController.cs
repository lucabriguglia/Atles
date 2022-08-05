using System;
using System.Threading.Tasks;
using Atles.Commands.PermissionSets;
using Atles.Core;
using Atles.Models.Admin.PermissionSets;
using Atles.Queries.Admin;
using Atles.Validators.ValidationRules;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Controllers.Admin;

[Route("api/admin/permission-sets")]
public class PermissionSetsController : AdminControllerBase
{
    private readonly IDispatcher _dispatcher;
    private readonly IPermissionSetValidationRules _permissionSetValidationRules;

    public PermissionSetsController(IDispatcher dispatcher, IPermissionSetValidationRules permissionSetValidationRules) : base(dispatcher)
    {
        _dispatcher = dispatcher;
        _permissionSetValidationRules = permissionSetValidationRules;
    }

    [HttpGet("list")]
    public async Task<ActionResult> List()
    {
        return await ProcessGet(new GetPermissionSetsIndex
        {
            SiteId = CurrentSite.Id
        });
    }

    [HttpGet("create")]
    public async Task<ActionResult> Create()
    {
        return await ProcessGet(new GetPermissionSetCreateForm
        {
            SiteId = CurrentSite.Id
        });
    }

    [HttpPost("save")]
    public async Task<ActionResult> Save(FormComponentModel.PermissionSetModel model)
    {
        var command = new CreatePermissionSet
        {
            Name = model.Name,
            Permissions = model.Permissions.ToPermissionCommands(),
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id
        };

        await _dispatcher.Send(command);

        return Ok();
    }

    [HttpGet("edit/{id}")]
    public async Task<ActionResult<FormComponentModel>> Edit(Guid id)
    {
        return await ProcessGet(new GetPermissionSetEditForm
        {
            SiteId = CurrentSite.Id, 
            Id = id
        });
    }

    [HttpPost("update")]
    public async Task<ActionResult> Update(FormComponentModel.PermissionSetModel model)
    {
        var command = new UpdatePermissionSet
        {
            PermissionSetId = model.Id,
            Name = model.Name,
            Permissions = model.Permissions.ToPermissionCommands(),
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id
        };

        await _dispatcher.Send(command);

        return Ok();
    }

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var command = new DeletePermissionSet
        {
            PermissionSetId = id,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id
        };

        await _dispatcher.Send(command);

        return Ok();
    }

    [HttpGet("is-name-unique/{name}")]
    public async Task<ActionResult> IsNameUnique(string name)
    {
        var isNameUnique = await _permissionSetValidationRules.IsPermissionSetNameUnique(CurrentSite.Id, name);
        return Ok(isNameUnique);
    }

    [HttpGet("is-name-unique/{name}/{id}")]
    public async Task<ActionResult> IsNameUnique(string name, Guid id)
    {
        var isNameUnique = await _permissionSetValidationRules.IsPermissionSetNameUnique(CurrentSite.Id, name, id);
        return Ok(isNameUnique);
    }

    [HttpGet("is-valid/{id}")]
    public async Task<ActionResult> IsValid(Guid id)
    {
        var isValid = await _permissionSetValidationRules.IsPermissionSetValid(CurrentSite.Id, id);
        return Ok(isValid);
    }
}
