using Atles.Commands.PermissionSets;
using Atles.Core;
using Atles.Models.Admin.PermissionSets;
using Atles.Queries.Admin;
using Atles.Server.Mapping;
using Atles.Validators.ValidationRules;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Controllers.Admin;

[Route("api/admin/permission-sets")]
public class PermissionSetsController : AdminControllerBase
{
    private readonly IPermissionSetValidationRules _permissionSetValidationRules;
    private readonly IMapper<PermissionSetFormModel.PermissionSetModel, CreatePermissionSet> _createPermissionSetMapper;
    private readonly IMapper<PermissionSetFormModel.PermissionSetModel, UpdatePermissionSet> _updatePermissionSetMapper;
    private readonly IValidator<PermissionSetFormModel.PermissionSetModel> _permissionSetValidator;

    public PermissionSetsController(
        IDispatcher dispatcher,
        IPermissionSetValidationRules permissionSetValidationRules,
        IMapper<PermissionSetFormModel.PermissionSetModel, CreatePermissionSet> createPermissionSetMapper,
        IMapper<PermissionSetFormModel.PermissionSetModel, UpdatePermissionSet> updatePermissionSetMapper,
        IValidator<PermissionSetFormModel.PermissionSetModel> permissionSetValidator) 
        : base(dispatcher)
    {
        _permissionSetValidationRules = permissionSetValidationRules;
        _createPermissionSetMapper = createPermissionSetMapper;
        _updatePermissionSetMapper = updatePermissionSetMapper;
        _permissionSetValidator = permissionSetValidator;
    }

    [HttpGet("list")]
    public async Task<ActionResult> List() => 
        await ProcessGet(new GetPermissionSetsIndex(CurrentSite.Id));

    [HttpGet("create")]
    public async Task<ActionResult> Create() => 
        await ProcessGet(new GetPermissionSetCreateForm(CurrentSite.Id));

    [HttpPost("save")]
    public async Task<ActionResult> Save(PermissionSetFormModel.PermissionSetModel model) => 
        await ProcessPost(model, _createPermissionSetMapper, _permissionSetValidator);

    [HttpGet("edit/{id}")]
    public async Task<ActionResult<PermissionSetFormModel>> Edit(Guid id) => 
        await ProcessGet(new GetPermissionSetEditForm(CurrentSite.Id, id));

    [HttpPost("update")]
    public async Task<ActionResult> Update(PermissionSetFormModel.PermissionSetModel model) => 
        await ProcessPost(model, _updatePermissionSetMapper, _permissionSetValidator);

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult> Delete(Guid id) =>
        await ProcessPost(new DeletePermissionSet
        {
            PermissionSetId = id,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id
        });

    [HttpGet("is-name-unique/{id}/{name}")]
    public async Task<ActionResult> IsNameUnique(string name, Guid id) => 
        Ok(await _permissionSetValidationRules.IsPermissionSetNameUnique(CurrentSite.Id, id, name));
}
